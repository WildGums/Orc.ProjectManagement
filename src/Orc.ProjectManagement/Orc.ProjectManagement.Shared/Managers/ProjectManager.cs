// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Reflection;

    internal class ProjectManager : IProjectManager
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IProjectRefresherSelector _projectRefresherSelector;
        private readonly IProjectSerializerSelector _projectSerializerSelector;
        private readonly IProjectInitializer _projectInitializer;
        private readonly IProjectValidator _projectValidator;
        private bool _isLoading;
        private bool _isSaving;
        private Stack<string> _activationHistory;
        private IDictionary<string, IProject> _projects;
        private IDictionary<string, IProjectRefresher> _projectRefreshers;
        #endregion

        #region Constructors
        public ProjectManager(IProjectValidator projectValidator, IProjectRefresherSelector projectRefresherSelector, IProjectSerializerSelector projectSerializerSelector,
            IProjectInitializer projectInitializer)
        {
            Argument.IsNotNull(() => projectValidator);
            Argument.IsNotNull(() => projectRefresherSelector);
            Argument.IsNotNull(() => projectSerializerSelector);
            Argument.IsNotNull(() => projectInitializer);

            _projectValidator = projectValidator;
            _projectRefresherSelector = projectRefresherSelector;
            _projectSerializerSelector = projectSerializerSelector;
            _projectInitializer = projectInitializer;

            _projects = new ConcurrentDictionary<string, IProject>();
            _projectRefreshers = new ConcurrentDictionary<string, IProjectRefresher>();
            _activationHistory = new Stack<string>();
        }
        #endregion

        #region Properties
        public IEnumerable<IProject> Projects
        {
            get { return _projects.Values; }
        }

        [ObsoleteEx(Message = "Use ActiveProject.Location instead", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public string Location {
            get
            {
                var activeProject = ActiveProject;
                return activeProject == null ? string.Empty : activeProject.Location;
            }
        }

        [ObsoleteEx(ReplacementTypeOrMember = "ActiveProject", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public IProject Project
        {
            get { return ActiveProject; }
        }

        public virtual IProject ActiveProject { get; private set; }
        #endregion

        #region Events
        public event EventHandler<ProjectEventArgs> ProjectRefreshRequired;

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
        public event AsyncEventHandler<ProjectUpdatedCancelEventArgs> ProjectActivation;
        public event AsyncEventHandler<ProjectUpdatedEventArgs> ProjectActivated;
        public event AsyncEventHandler<ProjectEventArgs> ProjectActivationCanceled;
        public event AsyncEventHandler<ProjectErrorEventArgs> ProjectActivationFailed;
        #endregion

        #region IProjectManager Members
        public async Task Initialize()
        {
            foreach (var location in _projectInitializer.GetInitialLocations().Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                Log.Debug("Loading initial project from location '{0}'", location);
                await Load(location);
            }
        }

        public async Task Refresh()
        {
            var project = ActiveProject;

            if (project == null)
            {
                return;
            }

            await Refresh(project);
        }

        public virtual async Task Refresh(IProject project)
        {
            Argument.IsNotNull(() => project);

            var location = project.Location;

            var activeProject = ActiveProject;

            var activeProjectLocation = this.GetActiveProjectLocation();

            Log.Debug("Refreshing project from '{0}'", location);

            var isRefreshingActiveProject = string.Equals(activeProjectLocation, location);

            if (isRefreshingActiveProject)
            {
                await SetActiveProject(null);
            }

            await Load(location, false, isRefreshingActiveProject);

            if (isRefreshingActiveProject)
            {
                ProjectUpdated.SafeInvoke(this, new ProjectUpdatedEventArgs(activeProject, ActiveProject));
            }

            Log.Info("Refreshed project from '{0}'", location);
        }

        public virtual Task<bool> Load(string location)
        {
            return LoadProject(location);
        }

        public virtual Task<bool> Load(string location, bool updateActive)
        {
            return LoadProject(location, updateActive);
        }

        public virtual Task<bool> Load(string location, bool updateActive, bool activateLoaded)
        {
            return LoadProject(location, updateActive, activateLoaded);
        }

        private async Task<bool> LoadProject(string location, bool updateActive = true, bool activateLoaded = true)
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

                    return false;
                }

                Log.Debug("Validating to see if we can load the project from '{0}'", location);

                if (!await _projectValidator.CanStartLoadingProjectAsync(location))
                {
                    Log.Error("Cannot load project from '{0}'", location);
                    await ProjectLoadingFailed.SafeInvoke(this, new ProjectErrorEventArgs(location));

                    return false;
                }

                var projectReader = _projectSerializerSelector.GetReader(location);
                if (projectReader == null)
                {
                    Log.ErrorAndThrowException<InvalidOperationException>(string.Format("No project reader is found for location '{0}'", location));
                }

                Log.Debug("Using project reader '{0}'", projectReader.GetType().Name);

                var project = await ReadProjectAndValidate(location, projectReader);

                if (project == null)
                {
                    return false;
                }

                var projectLocation = project.Location;
                _projects[projectLocation] = project;

                var activeProject = ActiveProject;

                if (updateActive && activeProject != null)
                {
                    await Close(activeProject);
                }

                InitializeProjectRefresher(projectLocation);

                await ProjectLoaded.SafeInvoke(this, new ProjectEventArgs(project));

                if (activateLoaded)
                {
                    await SetActiveProject(project);
                }

                if (updateActive && !Equals(activeProject, project))
                {
                    ProjectUpdated.SafeInvoke(this, new ProjectUpdatedEventArgs(activeProject, ActiveProject));
                }

                Log.Info("Loaded project from '{0}'", location);
            }

            return true;
        }

        public async Task<bool> Save(string location = null)
        {
            var project = ActiveProject;
            if (project == null)
            {
                Log.Error("Cannot save empty project");
                return false;
            }

            if (string.IsNullOrWhiteSpace(location))
            {
                location = project.Location;
            }

            return await Save(project, location);
        }

        public async Task<bool> Save(IProject project, string location = null)
        {
            Argument.IsNotNull(() => project);

            if (string.IsNullOrWhiteSpace(location))
            {
                location = project.Location;
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
                    return false;
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
                }
                catch (Exception ex)
                {
                    error = ex;
                    Log.Error(ex, "Failed to save project '{0}' to '{1}'", project, location);
                }

                if (error != null)
                {
                    await ProjectSavingFailed.SafeInvoke(this, new ProjectErrorEventArgs(project, error));

                    return false;
                }

                await ProjectSaved.SafeInvoke(this, new ProjectEventArgs(project));

                var peojectString = project.ToString();
                Log.Info("Saved project '{0}' to '{1}'", peojectString, location);
            }

            return true;
        }

        public async Task<bool> Close()
        {
            var project = ActiveProject;
            if (project == null)
            {
                return false;
            }

            return await Close(project);
        }

        public virtual async Task<bool> Close(IProject project)
        {
            Argument.IsNotNull(() => project);

            Log.Debug("Closing project '{0}'", project);

            var cancelEventArgs = new ProjectCancelEventArgs(project);
            await ProjectClosing.SafeInvoke(this, cancelEventArgs);

            if (cancelEventArgs.Cancel)
            {
                Log.Debug("Canceled closing project '{0}'", project);
                await ProjectClosingCanceled.SafeInvoke(this, new ProjectEventArgs(project));
                return false;
            }

            await SetActiveProject(null);

            var location = project.Location;

            if (_projects.ContainsKey(location))
            {
                _projects.Remove(location);
            }

            ReleaseProjectRefresher(project);

            var lastAcive = GetLastActiveProject();

            await SetActiveProject(lastAcive);

            await ProjectClosed.SafeInvoke(this, new ProjectEventArgs(project));

            Log.Info("Closed project '{0}'", project);

            return true;
        }

        public virtual async Task<bool> SetActiveProject(IProject project)
        {
            var activeProject = ActiveProject;

            var activeProjectLocation = activeProject == null ? null : activeProject.Location;
            var newProjectLocation = project == null ? null : project.Location;

            if (string.Equals(activeProjectLocation, newProjectLocation))
            {
                return false;
            }

            var eventArgs = new ProjectUpdatedCancelEventArgs(activeProject, project);

            await ProjectActivation.SafeInvoke(this, eventArgs);

            if (eventArgs.Cancel)
            {
                await ProjectActivationCanceled.SafeInvoke(this, new ProjectEventArgs(project));
                return false;
            }

            Exception exception = null;

            try
            {
                if (!string.IsNullOrWhiteSpace(newProjectLocation))
                {
                    _activationHistory.Push(newProjectLocation);
                }

                ActiveProject = project;
            }
            catch(Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                await ProjectActivationFailed.SafeInvoke(this, new ProjectErrorEventArgs(project, exception));
                return false;
            }


            await ProjectActivated.SafeInvoke(this, new ProjectUpdatedEventArgs(activeProject, project));

            return true;
        }

        private IProject GetLastActiveProject()
        {
            IProject projectToActivate = null;
            while (_activationHistory.Any() && projectToActivate == null)
            {
                var projectId = _activationHistory.Pop();
                _projects.TryGetValue(projectId, out projectToActivate);
            }

            return projectToActivate;
        }

        private void InitializeProjectRefresher(string projectLocation)
        {
            IProjectRefresher projectRefresher;

            if (!_projectRefreshers.TryGetValue(projectLocation, out projectRefresher) || projectRefresher == null)
            {
                try
                {
                    projectRefresher = _projectRefresherSelector.GetProjectRefresher(projectLocation);

                    if (projectRefresher != null)
                    {
                        Log.Debug("Subscribing to project refresher '{0}'", projectRefresher.GetType().GetSafeFullName());

                        projectRefresher.Updated += OnProjectRefresherUpdated;
                        projectRefresher.Subscribe();
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to subscribe to project refresher");
                }

                if (projectRefresher != null)
                {
                    _projectRefreshers[projectLocation] = projectRefresher;
                }
            }
        }

        private void ReleaseProjectRefresher(IProject project)
        {
            IProjectRefresher projectRefresher;

            if (_projectRefreshers.TryGetValue(project.Location, out projectRefresher) && projectRefresher != null)
            {
                try
                {
                    Log.Debug("Unsubscribing from project refresher '{0}'", projectRefresher.GetType().GetSafeFullName());

                    projectRefresher.Unsubscribe();
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to unsubscribe from project refresher");
                }

                projectRefresher.Updated -= OnProjectRefresherUpdated;
            }
        }

        private async Task<IProject> ReadProjectAndValidate(string location, IProjectReader projectReader)
        {
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

                return null;
            }

            return project;
        }

        private void OnProjectRefresherUpdated(object sender, ProjectEventArgs e)
        {
            if (_isLoading || _isSaving)
            {
                return;
            }

            ProjectRefreshRequired.SafeInvoke(this, e);
        }
        #endregion
    }
}