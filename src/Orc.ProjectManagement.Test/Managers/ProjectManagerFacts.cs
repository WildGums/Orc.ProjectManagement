// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagerFacts.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Test.Managers
{
    using Catel.IoC;
    using Mocks;
    using NUnit.Framework;
    using ProjectManagement.Serialization;

    public class ProjectManagerFacts
    {
        [TestFixture]
        public class TheLoadMethod
        {
            [TestCase("myLocation")]
            public async void UpdatesLocationAfterLoadingProject(string newLocation)
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                Assert.AreEqual(null, projectManager.Location);

                await projectManager.Load(newLocation);

                Assert.AreEqual(newLocation, projectManager.Location);
            }

            [TestCase]
            public async void RaisesProjectLoadingEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                var eventRaised = false;
                projectManager.ProjectLoading += (sender, e) => eventRaised = true;

                await projectManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async void RaisesProjectLoadedEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                var eventRaised = false;
                projectManager.ProjectLoaded += (sender, e) => eventRaised = true;

                await projectManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheRefreshMethod
        {
            [TestCase]
            public async void DoesNothingWithoutProject()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                Assert.IsNull(projectManager.Project);

                await projectManager.Refresh();

                Assert.IsNull(projectManager.Project);
            }

            [TestCase]
            public async void RaisesProjectUpdatedEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectUpdated += (sender, e) => eventRaised = true;

                await projectManager.Refresh(); 

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheSaveMethod
        {
            [TestCase("myLocation")]
            public async void UpdatesLocationAfterSavingProject(string newLocation)
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                await projectManager.Load("dummyLocation");

                Assert.AreEqual("dummyLocation", projectManager.Location);

                await projectManager.Save(newLocation);

                Assert.AreEqual(newLocation, projectManager.Location);
            }

            [TestCase]
            public async void RaisesProjectSavingEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectSaving += (sender, e) => eventRaised = true;

                await projectManager.Save();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async void RaisesProjectSavedEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectSaved += (sender, e) => eventRaised = true;

                await projectManager.Save();

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheCloseMethod
        {
            [TestCase]
            public async void UpdatesProjectAfterClosingProject()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                await projectManager.Load("dummyLocation");

                Assert.IsNotNull(projectManager.Project);

                projectManager.Close();

                Assert.IsNull(projectManager.Project);
            }

            [TestCase]
            public async void UpdatesLocationAfterClosingProject()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                await projectManager.Load("dummyLocation");

                Assert.AreEqual("dummyLocation", projectManager.Location);

                projectManager.Close();

                Assert.AreEqual(null, projectManager.Location);
            }

            [TestCase]
            public async void RaisesProjectClosingEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectClosing += (sender, e) => eventRaised = true;

                projectManager.Close();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public async void RaisesProjectClosedEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                await projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectClosed += (sender, e) => eventRaised = true;

                projectManager.Close();

                Assert.IsTrue(eventRaised);
            }
        }
    }
}