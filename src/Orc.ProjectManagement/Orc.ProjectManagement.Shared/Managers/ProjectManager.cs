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
        private readonly string _initialLocation;
        private readonly IProjectRefresherSelector _projectRefresherSelector;
        private readonly IProjectSerializerSelector _projectSerializerSelector;
        private readonly IProjectValidator _projectValidator;
        private bool _isLoading;
        private bool _isSaving;
        private Stack<string> _selectionHistory;
        private IProjectRefresher _projectRefresher;
        private IDictionary<string, IProject> _projects;
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

            _projects = new ConcurrentDictionary<string, IProject>();
            _selectionHistory = new Stack<string>();

            var location = projectInitializer.GetInitialLocation();

            _initialLocation = location;
        }
        #endregion

        #region Properties
        public IEnumerable<IProject> Projects
        {
            get { return _projects.Values; }
        }

        [ObsoleteEx(Message = "Use SelectedProject.Location instead", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public string Location {
            get
            {
                var selectedProject = SelectedProject;
                return selectedProject == null ? string.Empty : selectedProject.Location;
            }
        }

        [ObsoleteEx(ReplacementTypeOrMember = "SelectedProject", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public IProject Project
        {
            get { return SelectedProject; }
        }

        public IProject SelectedProject { get; private set; }
        #endregion

        #region Events
        public event EventHandler<EventArgs> ProjectRefreshRequired;

        public event AsyncEventHandler<ProjectLocationCancelEventArgs> ProjectLoading;
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
        public event AsyncEventHandler<ProjectCancelEventArgs> ProjectSelecting;
        public event AsyncEventHandler<ProjectEventArgs> ProjectSelected;
        public event AsyncEventHandler<ProjectEventArgs> ProjectSelectionCanceled;
        public event AsyncEventHandler<ProjectErrorEventArgs> ProjectSelectionFailed;
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

            await Load(location);

            Log.Info("Refreshed project from '{0}'", location);
        }

        public async Task<bool> Load(string location, bool selectLoaded = true)
        {
            Argument.IsNotNullOrWhitespace("location", location);

            using (new DisposableToken(null, token => _isLoading = true, token => _isLoading = false))
            {
                Log.Debug("Loading project from '{0}'", location);

                var cancelEventArgs = new ProjectLocationCancelEventArgs(location);
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

                    return false;
                }

                if (project != null)
                {
                    IProject oldProject;

                    var key = project.Location;

                    _projects.TryGetValue(key, out oldProject);

                    _projects[key] = project;

                    if (oldProject != null)
                    {
                        HandleProjectUpdate(oldProject, project);
                    }
                }
                

                if (selectLoaded || _projects.Count > 1)
                {
                    await SelectProject(project);
                }

                await ProjectLoaded.SafeInvoke(this, new ProjectEventArgs(project));

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

            var location = project.Location;

            if (_projects.ContainsKey(location))
            {
                var valuePair = _projects.First(x => Equals(x.Key, location));
                _projects.Remove(valuePair);
            }

            var selectedProject = SelectedProject;

            if (selectedProject != null && Equals(selectedProject.Location, location))
            {
                var lastSelected = GetLastSelected();
                if (lastSelected == null)
                {
                    await ClearSelection();
                }
                else
                {
                    await SelectProject(lastSelected, false);
                }
            }

            await ProjectClosed.SafeInvoke(this, new ProjectEventArgs(project));

            Log.Info("Closed project '{0}'", project);

            return true;
        }

        private async Task ClearSelection()
        {
            // TODO: add events
            SelectedProject = null;
        }

        private IProject GetLastSelected()
        {
            IProject projectToSelect = null;
            while (_selectionHistory.Any() && projectToSelect == null)
            {
                var projectId = _selectionHistory.Pop();
                _projects.TryGetValue(projectId, out projectToSelect);
            }

            return projectToSelect;
        }

        public async Task<bool> SelectProject(IProject project, bool rememberPrevious = true)
        {
            if (project == null)
            {
                return false;
            }

            var eventArgs = new ProjectCancelEventArgs(project);

            await ProjectSelecting.SafeInvoke(this, eventArgs);

            if (eventArgs.Cancel)
            {
                await ProjectSelectionCanceled.SafeInvoke(this, new ProjectEventArgs(project));
                return false;
            }

            Exception exception = null;

            try
            {
                var location = project.Location;
                var selectedProject = SelectedProject;

                if (rememberPrevious && selectedProject != null && !Equals(selectedProject.Location, location))
                {
                    _selectionHistory.Push(location);
                }

                SelectedProject = project;
            }
            catch(Exception ex)
            {
                exception = ex;
            }

            if (exception != null)
            {
                await ProjectSelectionFailed.SafeInvoke(this, new ProjectErrorEventArgs(project, exception));
                return false;
            }

            await ProjectSelected.SafeInvoke(this, new ProjectEventArgs(project));

            return true;
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