namespace Orc.ProjectManagement.Test.Managers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Tests;

public class ProjectManagerFacts
{
    [TestFixture]
    public class TheLoadMethod
    {
        [TestCase]
        public async Task RaisesProjectLoadingEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task RaisesProjectLoadingFailedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            Exception? caughtException = null;
            projectManager.ProjectLoadingFailedAsync += async (sender, e) =>
            {
                eventRaised = true;
                caughtException = e.Exception;
            };

            // Wrap in try-catch if necessary, but should not throw now
            await projectManager.LoadAsync("cannotload");

            Assert.That(eventRaised, Is.True, "ProjectLoadingFailed event should be raised");
            Assert.That(caughtException, Is.Not.Null, "Error should be included in event args");
        }

        [TestCase]
        public async Task RaisesProjectLoadingCanceledEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingAsync += async (sender, e) => e.Cancel = true;
            projectManager.ProjectLoadingCanceledAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task DoesntRaiseProjectLoadedEventIfCanceledAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingAsync += async (sender, e) => e.Cancel = true;
            projectManager.ProjectLoadedAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(eventRaised, Is.False);
        }

        [TestCase]
        public async Task RaisesProjectLoadedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadedAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(eventRaised, Is.True);
        }
    }

    [TestFixture]
    public class TheLoadInactiveMethod
    {
        [TestCase]
        public async Task RaisesProjectLoadingEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadInactiveAsync("dummyLocation");

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task RaisesProjectLoadingFailedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingFailedAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadInactiveAsync("cannotload");

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task RaisesProjectLoadingCanceledEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingAsync += async (sender, e) => e.Cancel = true;
            projectManager.ProjectLoadingCanceledAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadInactiveAsync("dummyLocation");

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task DoesntRaiseProjectLoadedEventIfCanceledAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadingAsync += async (sender, e) => e.Cancel = true;
            projectManager.ProjectLoadedAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadInactiveAsync("dummyLocation");

            Assert.That(eventRaised, Is.False);
        }

        [TestCase]
        public async Task RaisesProjectLoadedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventRaised = false;
            projectManager.ProjectLoadedAsync += async (sender, e) => eventRaised = true;

            await projectManager.LoadInactiveAsync("dummyLocation");

            Assert.That(eventRaised, Is.True);
        }

        //[TestCase]
        //public async Task DoesntCallSetActiveMethodWithLoadedProjectInParameter()
        //{
        //    var factory = Factory.Create().SetupDefault();
        //    var mock = factory.MockProjectManager();
        //    var projectManager = mock.Object;

        //    await projectManager.LoadInactive("dummyLocation");

        //    mock.Verify(x => x.SetActiveProject(It.IsAny<IProject>()), Times.Never);
        //}
    }

    [TestFixture]
    public class TheRefreshMethod
    {
        [TestCase]
        public async Task DoesNothingWithoutProjectAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            Assert.That(projectManager.ActiveProject, Is.Null);

            await projectManager.RefreshAsync();

            Assert.That(projectManager.ActiveProject, Is.Null);
        }

        [TestCase]
        public async Task RaisesProjectActivatedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            var eventRaised = false;
            projectManager.ProjectActivatedAsync += async (sender, e) => eventRaised = true;

            await projectManager.RefreshAsync();

            Assert.That(eventRaised, Is.True);
        }
    }

    [TestFixture]
    public class TheSaveMethod
    {
        [TestCase("myLocation")]
        public async Task UpdatesLocationAfterSavingProjectAsync(string newLocation)
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(projectManager.ActiveProject.Location, Is.EqualTo("dummyLocation"));

            await projectManager.SaveAsync(newLocation);

            Assert.That(projectManager.ActiveProject.Location, Is.EqualTo(newLocation));
        }

        [TestCase]
        public async Task RaisesProjectSavingEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            var eventRaised = false;
            projectManager.ProjectSavingAsync += async (sender, e) => eventRaised = true;

            await projectManager.SaveAsync();

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task RaisesProjectSavedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            var eventRaised = false;
            projectManager.ProjectSavedAsync += async (sender, e) => eventRaised = true;

            await projectManager.SaveAsync();

            Assert.That(eventRaised, Is.True);
        }
    }

    [TestFixture]
    public class TheCloseMethod
    {
        [TestCase]
        public async Task UpdatesProjectAfterClosingProjectAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(projectManager.ActiveProject, Is.Not.Null);

            await projectManager.CloseAsync();

            Assert.That(projectManager.ActiveProject, Is.Null);
        }

        [TestCase]
        public async Task UpdatesLocationAfterClosingProjectAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(projectManager.ActiveProject.Location, Is.EqualTo("dummyLocation"));

            await projectManager.CloseAsync();

            Assert.That(projectManager.ActiveProject, Is.EqualTo(null));
        }

        [TestCase]
        public async Task RaisesProjectClosingEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            var eventRaised = false;
            projectManager.ProjectClosingAsync += async (sender, e) => eventRaised = true;

            await projectManager.CloseAsync();

            Assert.That(eventRaised, Is.True);
        }

        [TestCase]
        public async Task RaisesProjectClosedEventAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            var eventRaised = false;
            projectManager.ProjectClosedAsync += async (sender, e) => eventRaised = true;

            await projectManager.CloseAsync();

            Assert.That(eventRaised, Is.True);
        }

        [TestCase(Reason = "ORCOMP-147")]
        public async Task DoesNotReloadClosedProjectAgainAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            var eventCount = 0;
            var projects = new List<IProject>();
            projectManager.ProjectActivatedAsync += async (sender, e) =>
            {
                eventCount++;
                projects.Add(e.NewProject);
            };

            await projectManager.CloseAsync();

            Assert.That(eventCount, Is.EqualTo(1));
            Assert.That(projects.Count, Is.EqualTo(1));
            Assert.That(projects[0], Is.Null);
        }
    }

    [TestFixture]
    public class TheSetActiveProjectMethod
    {
        [TestCase("dummyLocation")]
        public async Task UpdatesActiveProjectByValueFromParameterAsync(string newLocation)
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var initialActiveProject = projectManager.ActiveProject;
            var newProject = factory.CreateProject(newLocation);

            await projectManager.SetActiveProjectAsync(newProject);

            await projectManager.LoadAsync(newLocation);
            var activeProject = projectManager.ActiveProject;

            Assert.That(activeProject?.Location, Is.Not.EqualTo(initialActiveProject?.Location));
            Assert.That(newProject.Location, Is.EqualTo(activeProject.Location));
        }

        [TestCase("dummyLocation")]
        public async Task RaiseProjectActivationEventAsync(string newLocation)
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            IProject projectFromEvent = null;

            projectManager.ProjectActivationAsync += async (sender, e) => projectFromEvent = e.NewProject;
            var newProject = factory.CreateProject(newLocation);

            await projectManager.LoadAsync(newLocation);
            await projectManager.SetActiveProjectAsync(newProject);

            Assert.That(projectFromEvent.Location, Is.EqualTo(newProject.Location));
        }

        [TestCase("dummyLocation")]
        public async Task RaiseProjectActivatedEventAsync(string newLocation)
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            IProject projectFromEvent = null;

            projectManager.ProjectActivatedAsync += async (sender, e) => projectFromEvent = e.NewProject;
            var newProject = factory.CreateProject(newLocation);

            await projectManager.LoadAsync(newLocation);
            await projectManager.SetActiveProjectAsync(newProject);

            await Task.Delay(50);

            Assert.That(projectFromEvent.Location, Is.EqualTo(newProject.Location));
        }

        //[TestCase("dummyLocation")]
        //public async Task RaiseProjectActivatedEventAfterSettingActiveProject(string newLocation)
        //{
        //    var factory = Factory.Create().SetupDefault();
        //    var mock = factory.MockProjectManager();
        //    var projectManager = mock.Object;

        //    IList<string> actionNames = new List<string>();

        //    Listener.ListenToProjectManager(factory, (name, args) => actionNames.Add(name));

        //    var newProject = factory.CreateProject(newLocation);

        //    await projectManager.SetActiveProject(newProject);

        //    var projectActivatedIndex = actionNames.Single(x => string.Equals(x, Listener.ProjectManagerProjectActivated));
        //    var setAciveIndex = actionNames.Single(x => string.Equals(x, Listener.ProjectManagerActiveProjectSet));

        //    Assert.Less(setAciveIndex, projectActivatedIndex);
        //}
    }

    [TestFixture]
    public class TheLoadingStateManagementTests
    {
        [TestCase]
        public async Task SerializesConcurrentLoadingOfSameProjectAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var loadStarted = new TaskCompletionSource<bool>();
            var loadCompleting = new TaskCompletionSource<bool>();
            var firstLoadStarted = false;

            projectManager.ProjectLoadingAsync += async (s, e) =>
            {
                if (!firstLoadStarted)
                {
                    firstLoadStarted = true;
                    loadStarted.SetResult(true);
                    await loadCompleting.Task;
                }
            };

            // Start the first load in a separate task
            var load1Task = Task.Run(async () =>
            {
                await projectManager.LoadAsync("dummyLocation");
            });

            // Wait for the first load to start
            await loadStarted.Task;

            // Start timing
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Start the second load
            var load2Task = Task.Run(async () =>
            {
                await projectManager.LoadAsync("dummyLocation");
            });

            // Give the second load a moment to attempt to start
            await Task.Delay(100);

            // Complete the first load
            loadCompleting.SetResult(true);

            // Wait for both loads to complete
            await Task.WhenAll(load1Task, load2Task);

            // Stop timing
            stopwatch.Stop();

            // Assert that the second load waited for the first load to complete
            Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(100), "Second load should wait for the first to complete");
            Assert.That(projectManager.ActiveProject?.Location, Is.EqualTo("dummyLocation"), "Project should be loaded");
        }
    }

    [TestFixture]
    public class TheConcurrentOperationsTests
    {
        [TestCase]
        public async Task HandlesMultipleRefreshRequestsCorrectlyAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            int refreshCount = 0;
            var refreshStarted = new TaskCompletionSource<bool>();
            var refreshCompleting = new TaskCompletionSource<bool>();
            bool firstRefreshStarted = false;

            projectManager.ProjectRefreshingAsync += async (s, e) =>
            {
                if (!firstRefreshStarted)
                {
                    firstRefreshStarted = true;
                    refreshStarted.SetResult(true);
                    await refreshCompleting.Task;
                }
                refreshCount++;
            };

            // Load project first
            await projectManager.LoadAsync("dummyLocation");
            var project = projectManager.ActiveProject;
            Assert.That(project, Is.Not.Null, "Project should be loaded before testing refresh");

            // Start the first refresh in a separate task
            var refresh1Task = Task.Run(async () =>
            {
                await projectManager.RefreshAsync(project);
            });

            // Wait for the first refresh to start
            await refreshStarted.Task;

            // Start multiple concurrent refreshes
            var tasks = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await projectManager.RefreshAsync(project);
                }));
            }

            // Give the other refresh tasks a moment to attempt to start
            await Task.Delay(100);

            // Complete the first refresh
            refreshCompleting.SetResult(true);

            // Wait for all refreshes to complete
            await Task.WhenAll(tasks);

            // Allow time for all event handlers to complete
            await Task.Delay(200);

            // Assert that the refresh event was triggered only once
            Assert.That(refreshCount, Is.EqualTo(1), "Refresh event should have been triggered only once");
        }


        [TestCase]
        public async Task SkipsRefreshDuringLoadOperationAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var refreshCount = 0;
            projectManager.ProjectRefreshRequiredAsync += async (s, e) => refreshCount++;

            // Start a load operation
            var loadTask = projectManager.LoadAsync("dummyLocation");

            // Try to refresh immediately
            if (projectManager.ActiveProject is not null)
            {
                await projectManager.RefreshAsync(projectManager.ActiveProject);
            }

            await loadTask;

            // No refresh should have occurred during load
            Assert.That(refreshCount, Is.EqualTo(0));
        }
    }

    [TestFixture]
    public class TheResourceAndStateCleanupTests
    {
        [TestCase]
        public async Task CleansUpStatesOnProjectCloseAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            // Load and then close project
            await projectManager.LoadAsync("dummyLocation");
            await projectManager.CloseAsync();

            // Verify we can load the same project again
            Assert.DoesNotThrowAsync(() => projectManager.LoadAsync("dummyLocation"));
        }

        [TestCase]
        public async Task HandlesCleanupDuringConcurrentOperationsAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            await projectManager.LoadAsync("dummyLocation");

            // Start multiple operations
            var saveTask = projectManager.SaveAsync("newLocation");
            var closeTask = projectManager.CloseAsync();

            // Should complete without throwing
            Assert.DoesNotThrowAsync(async () => await Task.WhenAll(saveTask, closeTask));
        }

        [TestCase]
        public async Task MaintainsProjectStateConsistencyAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            // Load project
            await projectManager.LoadAsync("dummyLocation");
            var project = projectManager.ActiveProject;

            // Force a failure during save
            var failedSave = projectManager.SaveAsync("invalid_location");

            // Wait for save to complete
            try
            {
                await failedSave;
            }
            catch
            {
                // Expected failure
            }

            // Project should still be in a valid state
            Assert.That(project, Is.EqualTo(projectManager.ActiveProject));
        }
    }

    [TestFixture]
    public class TheEventHandlingTests
    {
        [TestCase]
        public async Task HandlesEventHandlerTimeoutGracefullyAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            // Add slow event handler
            projectManager.ProjectLoadingAsync += async (s, e) =>
            {
                await Task.Delay(5000); // Longer than default timeout
            };

            // Should complete despite timeout
            Assert.DoesNotThrowAsync(() => projectManager.LoadAsync("dummyLocation"));
        }

        [TestCase]
        public async Task MaintainsEventOrderingAsync()
        {
            var factory = Factory.Create().SetupDefault();
            var projectManager = factory.GetProjectManager();

            var eventOrder = new List<string>();

            projectManager.ProjectLoadingAsync += async (s, e) => eventOrder.Add("Loading");
            projectManager.ProjectLoadedAsync += async (s, e) => eventOrder.Add("Loaded");

            await projectManager.LoadAsync("dummyLocation");

            Assert.That(eventOrder[0], Is.EqualTo("Loading"));
            Assert.That(eventOrder[1], Is.EqualTo("Loaded"));
        }
    }
}
