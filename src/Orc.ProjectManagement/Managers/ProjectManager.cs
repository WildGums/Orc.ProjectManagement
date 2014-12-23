// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManager.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
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
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IProjectValidator _projectValidator;
        private readonly IProjectRefresherSelector _projectRefresherSelector;
        private readonly IProjectSerializerSelector _projectSerializerSelector;

        private IProject _project;
        private IProjectRefresher _projectRefresher;
        private bool _isLoading;
        private bool _isSaving;

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

            Location = location;
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

        public event EventHandler<ProjectEventArgs> ProjectLoading;
        public event EventHandler<ProjectErrorEventArgs> ProjectLoadingFailed;
        public event EventHandler<ProjectEventArgs> ProjectLoaded;

        public event EventHandler<ProjectEventArgs> ProjectSaving;
        public event EventHandler<ProjectErrorEventArgs> ProjectSavingFailed;
        public event EventHandler<ProjectEventArgs> ProjectSaved;

        public event EventHandler<ProjectUpdatedEventArgs> ProjectUpdated;

        public event EventHandler<ProjectEventArgs> ProjectClosing;
        public event EventHandler<ProjectEventArgs> ProjectClosed;
        #endregion

        #region IProjectManager Members
        public async Task Initialize()
        {
            var location = Location;
            if (!string.IsNullOrEmpty(location))
            {
                Log.Debug("Initial location is '{0}', loading initial project", location);

                // TODO: Determine if this should be moved to a separate method
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

            Log.Debug("Loading project from '{0}'", location);

            _isLoading = true;

            ProjectLoading.SafeInvoke(this, new ProjectEventArgs(location));

            Log.Debug("Validating to see if we can load the project from '{0}'", location);

            if (!await _projectValidator.CanStartLoadingProject(location))
            {
                Log.Error("Cannot load project from '{0}'", location);
                ProjectLoadingFailed.SafeInvoke(this, new ProjectErrorEventArgs(location));

                _isLoading = false;

                return;
            }

            var projectReader = _projectSerializerSelector.GetReader(location);
            if (projectReader == null)
            {
                _isLoading = false;

                Log.ErrorAndThrowException<InvalidOperationException>(string.Format("No project reader is found for location '{0}'", location));
            }

            Log.Debug("Using project reader '{0}'", projectReader.GetType().Name);

            IProject project;
            IValidationContext validationContext = null;

            try
            {
                project = await projectReader.Read(location);
                if (project == null)
                {
                    Log.ErrorAndThrowException<InvalidOperationException>(string.Format("Project could not be loaded from '{0}'", location));
                }

                validationContext = await _projectValidator.ValidateProject(project);
                if (validationContext.HasErrors)
                {
                    Log.ErrorAndThrowException<InvalidOperationException>(string.Format("Project data was loaded from '{0}', but the validator returned errors", location));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load project from '{0}'", location);
                ProjectLoadingFailed.SafeInvoke(this, new ProjectErrorEventArgs(location, ex, validationContext));

                _isLoading = false;

                return;
            }

            Location = location;
            Project = project;

            ProjectLoaded.SafeInvoke(this, new ProjectEventArgs(project));

            _isLoading = false;

            Log.Info("Loaded project from '{0}'", location);
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

            Log.Debug("Saving project '{0}' to '{1}'", project, location);

            _isSaving = true;

            var eventArgs = new ProjectEventArgs(project);
            ProjectSaving.SafeInvoke(this, eventArgs);

            var projectWriter = _projectSerializerSelector.GetWriter(location);
            if (projectWriter == null)
            {
                _isSaving = false;

                Log.ErrorAndThrowException<NotSupportedException>(string.Format("No project writer is found for location '{0}'", location));
            }

            Log.Debug("Using project writer '{0}'", projectWriter.GetType().Name);

            try
            {
                await projectWriter.Write(project, location);
                Location = location;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save project '{0}' to '{1}'", project, location);
                ProjectSavingFailed.SafeInvoke(this, new ProjectErrorEventArgs(project, ex));

                _isSaving = false;

                return;
            }

            ProjectSaved.SafeInvoke(this, eventArgs);

            _isSaving = false;

            Log.Info("Saved project '{0}' to '{1}'", project, location);
        }

        public void Close()
        {
            var project = Project;
            if (project == null)
            {
                return;
            }

            Log.Debug("Closing project '{0}'", project);

            var eventArgs = new ProjectEventArgs(project);
            ProjectClosing.SafeInvoke(this, eventArgs);

            Project = null;
            Location = null;

            ProjectClosed.SafeInvoke(this, eventArgs);

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