// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Reflection;

    public class ProjectManager : IProjectManager
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly string _initialLocation;
        private readonly IProjectRefresherSelector _projectRefresherSelector;
        private readonly IProjectSerializerSelector _projectSerializerSelector;
        private readonly IProjectValidator _projectValidator;
        private bool _isLoading;
        private bool _isSaving;
        private IProject _project;
        private IProjectRefresher _projectRefresher;
        #endregion

        #region Constructors
        public ProjectManager(IProjectValidator projectValidator, IProjectInitializer projectInitializer, IProjectRefresherSelector projectRefresherSelector, IProjectSerializerSelector projectSerializerSelector)
        {
            Argument.IsNotNull(() => projectInitializer);
            Argument.IsNotNull(() => projectValidator);
            Argument.IsNotNull(() => projectRefresherSelector);
            Argument.IsNotNull(() => projectSerializerSelector);

            _projectValidator = projectValidator;
            _projectRefresherSelector = projectRefresherSelector;
            _projectSerializerSelector = projectSerializerSelector;

            var location = projectInitializer.GetInitialLocation();

            _initialLocation = location;
        }
        #endregion

        #region Properties
        public string Location { get; private set; }

        public IProject Project
        {
            get { return _project; }
            private set
            {
                var oldProject = _project;
                var newProject = value;

                _project = value;

                HandleProjectUpdate(oldProject, newProject);
            }
        }
        #endregion

        #region Events
        public event EventHandler<EventArgs> ProjectRefreshRequired;

        public event AsyncEventHandler<ProjectCancelEventArgs> ProjectLoading;
        public event AsyncEventHandler<ProjectErrorEventArgs> ProjectLoadingFailed;
        public event AsyncEventHandler<ProjectEventArgs> ProjectLoadingCanceled;
        public event AsyncEventHandler<ProjectEventArgs> ProjectLoaded;

        public event AsyncEventHandler<ProjectCancelEventArgs> ProjectSaving;
        public event AsyncEventHandler<ProjectErrorEventArgs> ProjectSavingFailed;
        public event AsyncEventHandler<ProjectEventArgs> ProjectSavingCanceled;
        public event AsyncEventHandler<ProjectEventArgs> ProjectSaved;

        public event EventHandler<ProjectUpdatedEventArgs> ProjectUpdated;

        public event AsyncEventHandler<ProjectCancelEventArgs> ProjectClosing;
        public event AsyncEventHandler<ProjectEventArgs> ProjectClosingCanceled;
        public event AsyncEventHandler<ProjectEventArgs> ProjectClosed;
        #endregion

        #region IProjectManager Members
        public async Task Initialize()
        {
            var location = _initialLocation;
            if (!string.IsNullOrEmpty(location))
            {
                Log.Debug("Initial location is '{0}', loading initial project", location);

                await Load(location);
            }
        }

        public async Task Refresh()
        {
            if (Project == null)
            {
                return;
            }

            var location = Location;

            Log.Debug("Refreshing project from '{0}'", location);

            await Load(location);

            Log.Info("Refreshed project from '{0}'", location);
        }

        public async Task Load(string location)
        {
            Argument.IsNotNullOrWhitespace("location", location);

            using (new DisposableToken(null, token => _isLoading = true, token => _isLoading = false))
            {
                Log.Debug("Loading project from '{0}'", location);

                var cancelEventArgs = new ProjectCancelEventArgs(location);
                await ProjectLoading.SafeInvoke(this, cancelEventArgs);

                if (cancelEventArgs.Cancel)
                {
                    Log.Debug("Canceled loading of project from '{0}'", location);
                    await ProjectLoadingCanceled.SafeInvoke(this, new ProjectEventArgs(location));

                    return;
                }

                Log.Debug("Validating to see if we can load the project from '{0}'", location);

                if (!await _projectValidator.CanStartLoadingProjectAsync(location))
                {
                    Log.Error("Cannot load project from '{0}'", location);
                    await ProjectLoadingFailed.SafeInvoke(this, new ProjectErrorEventArgs(location));

                    return;
                }

                var projectReader = _projectSerializerSelector.GetReader(location);
                if (projectReader == null)
                {
                    Log.ErrorAndThrowException<InvalidOperationException>(string.Format("No project reader is found for location '{0}'", location));
                }

                Log.Debug("Using project reader '{0}'", projectReader.GetType().Name);

                IProject project = null;
                IValidationContext validationContext = null;

                Exception error = null;
                try
                {
                    project = await projectReader.Read(location);
                    if (project == null)
                    {
                        Log.ErrorAndThrowException<InvalidOperationException>(string.Format("Project could not be loaded from '{0}'", location));
                    }

                    validationContext = await _projectValidator.ValidateProjectAsync(project);
                    if (validationContext.HasErrors)
                    {
                        Log.ErrorAndThrowException<InvalidOperationException>(string.Format("Project data was loaded from '{0}', but the validator returned errors", location));
                    }
                }
                catch (Exception ex)
                {
                    error = ex;
                    Log.Error(ex, "Failed to load project from '{0}'", location);
                }

                if (error != null)
                {
                    await ProjectLoadingFailed.SafeInvoke(this, new ProjectErrorEventArgs(location, error, validationContext));

                    return;
                }

                Location = location;
                Project = project;

                await ProjectLoaded.SafeInvoke(this, new ProjectEventArgs(project));

                Log.Info("Loaded project from '{0}'", location);
            }
        }

        public async Task Save(string location = null)
        {
            var project = Project;
            if (project == null)
            {
                Log.Error("Cannot save empty project");
                throw new InvalidProjectException(project);
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                location = Location;
            }

            using (new DisposableToken(null, token => _isSaving = true, token => _isSaving = false))
            {
                Log.Debug("Saving project '{0}' to '{1}'", project, location);

                var cancelEventArgs = new ProjectCancelEventArgs(project);
                await ProjectSaving.SafeInvoke(this, cancelEventArgs);

                if (cancelEventArgs.Cancel)
                {
                    Log.Debug("Canceled saving of project to '{0}'", location);
                    await ProjectSavingCanceled.SafeInvoke(this, new ProjectEventArgs(project));
                    return;
                }

                var projectWriter = _projectSerializerSelector.GetWriter(location);
                if (projectWriter == null)
                {
                    Log.ErrorAndThrowException<NotSupportedException>(string.Format("No project writer is found for location '{0}'", location));
                }

                Log.Debug("Using project writer '{0}'", projectWriter.GetType().Name);

                Exception error = null;
                try
                {
                    await projectWriter.Write(project, location);
                    Location = location;
                }
                catch (Exception ex)
                {
                    error = ex;
                    Log.Error(ex, "Failed to save project '{0}' to '{1}'", project, location);
                }

                if (error != null)
                {
                    await ProjectSavingFailed.SafeInvoke(this, new ProjectErrorEventArgs(project, error));

                    return;
                }

                await ProjectSaved.SafeInvoke(this, new ProjectEventArgs(project));

                var peojectString = project.ToString();
                Log.Info("Saved project '{0}' to '{1}'", peojectString, location);
            }
        }

        public async Task Close()
        {
            var project = Project;
            if (project == null)
            {
                return;
            }

            Log.Debug("Closing project '{0}'", project);

            var cancelEventArgs = new ProjectCancelEventArgs(project);
            await ProjectClosing.SafeInvoke(this, cancelEventArgs);

            if (cancelEventArgs.Cancel)
            {
                Log.Debug("Canceled closing project '{0}'", project);
                await ProjectClosingCanceled.SafeInvoke(this, new ProjectEventArgs(project));
                return;
            }

            Project = null;
            Location = null;

            await ProjectClosed.SafeInvoke(this, new ProjectEventArgs(project));

            Log.Info("Closed project '{0}'", project);
        }

        private void HandleProjectUpdate(IProject oldProject, IProject newProject)
        {
            if (_projectRefresher != null)
            {
                try
                {
                    Log.Debug("Unsubscribing from project refresher '{0}'", _projectRefresher.GetType().GetSafeFullName());

                    _projectRefresher.Unsubscribe();
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to unsubscribe from project refresher");
                }

                _projectRefresher.Updated -= OnProjectRefresherUpdated;
                _projectRefresher = null;
            }

            if (newProject != null)
            {
                try
                {
                    var projectRefresher = _projectRefresherSelector.GetProjectRefresher(newProject.Location);
                    if (projectRefresher != null)
                    {
                        Log.Debug("Subscribing to project refresher '{0}'", projectRefresher.GetType().GetSafeFullName());

                        _projectRefresher = projectRefresher;
                        _projectRefresher.Updated += OnProjectRefresherUpdated;
                        _projectRefresher.Subscribe();
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to subscribe to project refresher");
                }
            }

            ProjectUpdated.SafeInvoke(this, new ProjectUpdatedEventArgs(oldProject, newProject));
        }

        private void OnProjectRefresherUpdated(object sender, EventArgs e)
        {
            if (_isLoading || _isSaving)
            {
                return;
            }

            ProjectRefreshRequired.SafeInvoke(this);
        }
        #endregion
    }
}