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

    public class ProjectManager : IProjectManager
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IProjectRefresherSelector _projectRefresherSelector;
        private readonly IProjectSerializerSelector _projectSerializerSelector;
        private readonly IProjectValidator _projectValidator;
        private bool _isLoading;
        private bool _isSaving;
        private Stack<string> _activationHistory;
        private IDictionary<string, IProject> _projects;
        private IDictionary<string, IProjectRefresher> _projectRefreshers;
        #endregion

        #region Constructors
        public ProjectManager(IProjectValidator projectValidator, IProjectRefresherSelector projectRefresherSelector, IProjectSerializerSelector projectSerializerSelector)
        {
            Argument.IsNotNull(() => projectValidator);
            Argument.IsNotNull(() => projectRefresherSelector);
            Argument.IsNotNull(() => projectSerializerSelector);

            _projectValidator = projectValidator;
            _projectRefresherSelector = projectRefresherSelector;
            _projectSerializerSelector = projectSerializerSelector;

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

        [ObsoleteEx(Message = "Use CurrentProject.Location instead", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public string Location {
            get
            {
                var currentProject = CurrentProject;
                return currentProject == null ? string.Empty : currentProject.Location;
            }
        }

        [ObsoleteEx(ReplacementTypeOrMember = "CurrentProject", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public IProject Project
        {
            get { return CurrentProject; }
        }

        public IProject CurrentProject { get; private set; }
        #endregion

        #region Events
        public event EventHandler<EventArgs> ProjectRefreshRequired;

        [ObsoleteEx(ReplacementTypeOrMember = "ProjectLocationLoading", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public event AsyncEventHandler<ProjectCancelEventArgs> ProjectLoading
        {
            add { throw new NotSupportedException("You're trying to subscribe to obsolete event 'ProjectLoading'. Use 'ProjectLocationLoading' instead"); }
            remove{}
        }

        [ObsoleteEx(ReplacementTypeOrMember = "ProjectLocationLoadingFailed", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public event AsyncEventHandler<ProjectErrorEventArgs> ProjectLoadingFailed
        {
            add { throw new NotSupportedException("You're trying to subscribe to obsolete event 'ProjectLoadingFailed'. Use 'ProjectLocationLoadingFailed' instead"); }
            remove { }
        }

        [ObsoleteEx(ReplacementTypeOrMember = "ProjectLocationLoadingCanceled", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public event AsyncEventHandler<ProjectEventArgs> ProjectLoadingCanceled
        {
            add { throw new NotSupportedException("You're trying to subscribe to obsolete event 'ProjectLoadingCanceled'. Use 'ProjectLocationLoadingCanceled' instead"); }
            remove { }
        }

        public event AsyncEventHandler<ProjectLocationCancelEventArgs> ProjectLocationLoading;
        public event AsyncEventHandler<ProjectLocationErrorEventArgs> ProjectLocationLoadingFailed;
        public event AsyncEventHandler<ProjectLocationEventArgs> ProjectLocationLoadingCanceled;
        public event AsyncEventHandler<ProjectEventArgs> ProjectLoaded;

        public event AsyncEventHandler<ProjectCancelEventArgs> ProjectSaving;
        public event AsyncEventHandler<ProjectErrorEventArgs> ProjectSavingFailed;
        public event AsyncEventHandler<ProjectEventArgs> ProjectSavingCanceled;
        public event AsyncEventHandler<ProjectEventArgs> ProjectSaved;

        public event EventHandler<ProjectUpdatedEventArgs> ProjectUpdated;

        public event AsyncEventHandler<ProjectCancelEventArgs> ProjectClosing;
        public event AsyncEventHandler<ProjectEventArgs> ProjectClosingCanceled;
        public event AsyncEventHandler<ProjectEventArgs> ProjectClosed;
        public event AsyncEventHandler<ProjectUpdatedCancelEventArgs> CurrentProjectChanging;
        public event AsyncEventHandler<ProjectUpdatedEventArgs> CurrentProjectChanged;
        public event AsyncEventHandler<ProjectEventArgs> ChangingCurrentProjectCanceled;
        public event AsyncEventHandler<ProjectErrorEventArgs> ChangingCurrentProjectFailed;
        #endregion

        #region IProjectManager Members
        [ObsoleteEx(RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public async Task Initialize()
        {

        }

        public async Task Refresh()
        {
            var project = Project;

            if (project == null)
            {
                return;
            }

            await Refresh(project);
        }

        public async Task Refresh(IProject project)
        {
            Argument.IsNotNull(() => project);

            var location = project.Location;

            Log.Debug("Refreshing project from '{0}'", location);

            await Load(location, false);

            Log.Info("Refreshed project from '{0}'", location);
        }

        public async Task<bool> Load(string location, bool updateCurrent = true)
        {
            Argument.IsNotNullOrWhitespace("location", location);

            using (new DisposableToken(null, token => _isLoading = true, token => _isLoading = false))
            {
                Log.Debug("Loading project from '{0}'", location);

                var cancelEventArgs = new ProjectLocationCancelEventArgs(location);

                await ProjectLocationLoading.SafeInvoke(this, cancelEventArgs);

                if (cancelEventArgs.Cancel)
                {
                    Log.Debug("Canceled loading of project from '{0}'", location);
                    await ProjectLocationLoadingCanceled.SafeInvoke(this, new ProjectLocationEventArgs(location));

                    return false;
                }

                Log.Debug("Validating to see if we can load the project from '{0}'", location);

                if (!await _projectValidator.CanStartLoadingProjectAsync(location))
                {
                    Log.Error("Cannot load project from '{0}'", location);
                    await ProjectLocationLoadingFailed.SafeInvoke(this, new ProjectLocationErrorEventArgs(location));

                    return false;
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
                    await ProjectLocationLoadingFailed.SafeInvoke(this, new ProjectLocationErrorEventArgs(location, error, validationContext));

                    return false;
                }

                if (project != null)
                {
                    _projects[project.Location] = project;

                    var currentProject = CurrentProject;

                    if (updateCurrent && currentProject != null)
                    {
                        await Close(currentProject);
                    }

                    try
                    {
                        var projectRefresher = _projectRefresherSelector.GetProjectRefresher(project.Location);
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

                    await ProjectLoaded.SafeInvoke(this, new ProjectEventArgs(project));

                    await SetCurrentProject(project);

                    if (updateCurrent && !Equals(currentProject, CurrentProject))
                    {
                        ProjectUpdated.SafeInvoke(this, new ProjectUpdatedEventArgs(currentProject, CurrentProject));
                    }
                }

                Log.Info("Loaded project from '{0}'", location);
            }

            return true;
        }

        public async Task<bool> Save(string location = null)
        {
            var project = Project;
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

        public async Task<bool> Save(IProject project, string location)
        {
            Argument.IsNotNull(() => project);
            Argument.IsNotNullOrEmpty(() => location);

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
            var project = Project;
            if (project == null)
            {
                return false;
            }

            return await Close(project);
        }

        public async Task<bool> Close(IProject project)
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

            await SetCurrentProject(null, false);

            var location = project.Location;

            if (_projects.ContainsKey(location))
            {
                _projects.Remove(location);
            }

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

            var lastAcive = GetLastActiveProject();

            await SetCurrentProject(lastAcive, false);

            await ProjectClosed.SafeInvoke(this, new ProjectEventArgs(project));

            Log.Info("Closed project '{0}'", project);

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

        public async Task<bool> SetCurrentProject(IProject project, bool rememberPrevious = true)
        {
            var currentProject = CurrentProject;

            var currentProjectLocation = currentProject == null ? null : currentProject.Location;
            var newProjectLocation = project == null ? null : project.Location;

            if (string.Equals(currentProjectLocation, newProjectLocation))
            {
                return false;
            }

            var eventArgs = new ProjectUpdatedCancelEventArgs(currentProject, project);

            await CurrentProjectChanging.SafeInvoke(this, eventArgs);

            if (eventArgs.Cancel)
            {
                await ChangingCurrentProjectCanceled.SafeInvoke(this, new ProjectEventArgs(project));
                return false;
            }

            Exception exception = null;

            try
            {
                if (rememberPrevious && !string.IsNullOrWhiteSpace(newProjectLocation))
                {
                    _activationHistory.Push(newProjectLocation);
                }

                CurrentProject = project;
            }
            catch(Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                await ChangingCurrentProjectFailed.SafeInvoke(this, new ProjectErrorEventArgs(project, exception));
                return false;
            }


            await CurrentProjectChanged.SafeInvoke(this, new ProjectUpdatedEventArgs(currentProject, project));

            return true;
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