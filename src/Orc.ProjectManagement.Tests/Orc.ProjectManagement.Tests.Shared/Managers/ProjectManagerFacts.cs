// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagerFacts.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Test.Managers
{
    using System.Threading.Tasks;
    using Catel.IoC;
    using Mocks;
    using NUnit.Framework;
    using ProjectManagement.Serialization;

    public class ProjectManagerFacts
    {
        #region Methods
        private static ProjectManager GetProjectManager()
        {
            var projectManager = new ProjectManager(new EmptyProjectValidator(), new EmptyProjectInitializer(),
                new DefaultProjectRefresherSelector(ServiceLocator.Default, TypeFactory.Default),
                new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

            return projectManager;
        }
        #endregion

        [TestFixture]
        public class TheLoadMethod
        {
            #region Methods
            [TestCase("myLocation")]
            public async Task UpdatesLocationAfterLoadingProject(string newLocation)
            {
                var projectManager = GetProjectManager();

                Assert.AreEqual(null, projectManager.SelectedProject);

                await projectManager.Load(newLocation);

                Assert.AreEqual(newLocation, projectManager.SelectedProject.Location);
            }

            [TestCase]
            public async Task RaisesProjectLoadingEvent()
            {
                var projectManager = GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoading += async (sender, e) => eventRaised = true;

                await projectManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadingFailedEvent()
            {
                var projectManager = GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoadingFailed += async (sender, e) => eventRaised = true;

                await projectManager.Load("cannotload");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadingCanceledEvent()
            {
                var projectManager = GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoading += async (sender, e) => e.Cancel = true;
                projectManager.ProjectLoadingCanceled += async (sender, e) => eventRaised = true;

                await projectManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectLoadedEvent()
            {
                var projectManager = GetProjectManager();

                var eventRaised = false;
                projectManager.ProjectLoaded += async (sender, e) => eventRaised = true;

                await projectManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }
            #endregion
        }

        [TestFixture]
        public class TheRefreshMethod
        {
            #region Methods
            [TestCase]
            public async Task DoesNothingWithoutProject()
            {
                var projectManager = GetProjectManager();

                Assert.IsNull(projectManager.SelectedProject);

                await projectManager.Refresh();

                Assert.IsNull(projectManager.SelectedProject);
            }

            [TestCase]
            public async Task RaisesProjectUpdatedEvent()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectUpdated += (sender, e) => eventRaised = true;

                await projectManager.Refresh();

                Assert.IsTrue(eventRaised);
            }
            #endregion
        }

        [TestFixture]
        public class TheSaveMethod
        {
            #region Methods
            [TestCase("myLocation")]
            public async Task UpdatesLocationAfterSavingProject(string newLocation)
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                Assert.AreEqual("dummyLocation", projectManager.SelectedProject.Location);

                await projectManager.Save(newLocation);

                Assert.AreEqual(newLocation, projectManager.SelectedProject.Location);
            }

            [TestCase]
            public async Task RaisesProjectSavingEvent()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectSaving += async (sender, e) => eventRaised = true;

                await projectManager.Save();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectSavedEvent()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectSaved += async (sender, e) => eventRaised = true;

                await projectManager.Save();

                Assert.IsTrue(eventRaised);
            }
            #endregion
        }

        [TestFixture]
        public class TheCloseMethod
        {
            #region Methods
            [TestCase]
            public async Task UpdatesProjectAfterClosingProject()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                Assert.IsNotNull(projectManager.SelectedProject);

                await projectManager.Close();

                Assert.IsNull(projectManager.SelectedProject);
            }

            [TestCase]
            public async Task UpdatesLocationAfterClosingProject()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                Assert.AreEqual("dummyLocation", projectManager.SelectedProject.Location);

                await projectManager.Close();

                Assert.AreEqual(null, projectManager.SelectedProject);
            }

            [TestCase]
            public async Task RaisesProjectClosingEvent()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectClosing += async (sender, e) => eventRaised = true;

                await projectManager.Close();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async Task RaisesProjectClosedEvent()
            {
                var projectManager = GetProjectManager();

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectClosed += async (sender, e) => eventRaised = true;

                await projectManager.Close();

                Assert.IsTrue(eventRaised);
            }
            #endregion
        }
    }
}