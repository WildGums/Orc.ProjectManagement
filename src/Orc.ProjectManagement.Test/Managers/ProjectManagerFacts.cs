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
            public void UpdatesLocationAfterLoadingProject(string newLocation)
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                Assert.AreEqual(null, projectManager.Location);

                projectManager.Load(newLocation);

                Assert.AreEqual(newLocation, projectManager.Location);
            }

            [TestCase]
            public void RaisesProjectLoadingEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                var eventRaised = false;
                projectManager.ProjectLoading += (sender, e) => eventRaised = true;

                projectManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public void RaisesProjectLoadedEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                var eventRaised = false;
                projectManager.ProjectLoaded += (sender, e) => eventRaised = true;

                projectManager.Load("dummyLocation");

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheRefreshMethod
        {
            [TestCase]
            public void DoesNothingWithoutProject()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                Assert.IsNull(projectManager.Project);

                projectManager.Refresh();

                Assert.IsNull(projectManager.Project);
            }

            [TestCase]
            public void RaisesProjectUpdatedEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectUpdated += (sender, e) => eventRaised = true;

                projectManager.Refresh(); 

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheSaveMethod
        {
            [TestCase("myLocation")]
            public void UpdatesLocationAfterSavingProject(string newLocation)
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                projectManager.Load("dummyLocation");

                Assert.AreEqual("dummyLocation", projectManager.Location);

                projectManager.Save(newLocation);

                Assert.AreEqual(newLocation, projectManager.Location);
            }

            [TestCase]
            public void RaisesProjectSavingEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectSaving += (sender, e) => eventRaised = true;

                projectManager.Save();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public void RaisesProjectLoadedEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectSaved += (sender, e) => eventRaised = true;

                projectManager.Save();

                Assert.IsTrue(eventRaised);
            }
        }

        [TestFixture]
        public class TheCloseMethod
        {
            [TestCase]
            public void UpdatesProjectAfterClosingProject()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                projectManager.Load("dummyLocation");

                Assert.IsNotNull(projectManager.Project);

                projectManager.Close();

                Assert.IsNull(projectManager.Project);
            }

            [TestCase]
            public void UpdatesLocationAfterClosingProject()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                projectManager.Load("dummyLocation");

                Assert.AreEqual("dummyLocation", projectManager.Location);

                projectManager.Close();

                Assert.AreEqual(null, projectManager.Location);
            }

            [TestCase]
            public void RaisesProjectClosingEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectClosing += (sender, e) => eventRaised = true;

                projectManager.Close();

                Assert.IsTrue(eventRaised);
            }

            [TestCase]
            public void RaisesProjectClosedEvent()
            {
                var projectManager = new ProjectManager(new EmptyProjectInitializer(), new FixedProjectSerializerSelector<MemoryProjectReader, MemoryProjectWriter>());

                projectManager.Load("dummyLocation");

                var eventRaised = false;
                projectManager.ProjectClosed += (sender, e) => eventRaised = true;

                projectManager.Close();

                Assert.IsTrue(eventRaised);
            }
        }
    }
}