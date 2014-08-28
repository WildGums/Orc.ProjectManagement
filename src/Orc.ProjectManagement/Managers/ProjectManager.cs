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
    using Catel.Logging;

    public class ProjectManager : IProjectManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IProjectSerializerSelector _projectSerializerSelector;

        private IProject _project;

        #region Constructors
        public ProjectManager(IProjectInitializer projectInitializer, IProjectSerializerSelector projectSerializerSelector)
        {
            Argument.IsNotNull(() => projectInitializer);
            Argument.IsNotNull(() => projectSerializerSelector);

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

                ProjectUpdated.SafeInvoke(this, new ProjectUpdatedEventArgs(oldProject, newProject));
            }
        }
        #endregion

        #region Events
        public event EventHandler<ProjectEventArgs> ProjectLoading;
        public event EventHandler<ProjectEventArgs> ProjectLoaded;

        public event EventHandler<ProjectEventArgs> ProjectSaving;
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

            ProjectLoading.SafeInvoke(this, new ProjectEventArgs(location));

            var projectReader = _projectSerializerSelector.GetReader(location);
            if (projectReader == null)
            {
                Log.ErrorAndThrowException<NotSupportedException>(string.Format("No project reader is found for location '{0}'", location));
            }

            var project = await projectReader.Read(location);

            Location = location;
            Project = project;

            ProjectLoaded.SafeInvoke(this, new ProjectEventArgs(project));

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

            var eventArgs = new ProjectEventArgs(project);
            ProjectSaving.SafeInvoke(this, eventArgs);

            var projectWriter = _projectSerializerSelector.GetWriter(location);
            if (projectWriter == null)
            {
                Log.ErrorAndThrowException<NotSupportedException>(string.Format("No project writer is found for location '{0}'", location));
            }

            await projectWriter.Write(project, location);
            Location = location;

            ProjectSaved.SafeInvoke(this, eventArgs);

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
        #endregion
    }
}