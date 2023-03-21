namespace Orc.ProjectManagement.Tests;

using System;
using System.Threading;
using Catel;
using Catel.IoC;
using Moq;
using Serialization;
using Test.Mocks;

internal static class FactoryExtensions
{
    public static IProject CreateProject(this Factory factory, string location)
    {
        ArgumentNullException.ThrowIfNull(factory);
        Argument.IsNotNullOrEmpty(() => location);

        return new Project(location);
    }

    public static IProjectManager GetProjectManager(this Factory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var serviceLocator = factory.ServiceLocator;
        if (serviceLocator.IsTypeRegistered<IProjectManager>())
        {
            return serviceLocator.ResolveType<IProjectManager>();
        }

        var projectManager = factory.CreateInstance<ProjectManager>();
        return projectManager;
    }

    public static Mock<IProjectManager> MockProjectManager(this Factory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var mock = factory.ServiceLocator.ResolveMocked<IProjectManager>();
        return mock;
    }


    public static Factory SetupDefault(this Factory factory, ProjectManagementType projectManagementType = ProjectManagementType.SingleDocument)
    {
        ArgumentNullException.ThrowIfNull(factory);

        SetupRegistrations(factory, projectManagementType);

        SetupBehavior(factory);

        return factory;
    }

    private static void SetupBehavior(Factory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var projectRefresher = factory.ServiceLocator.ResolveType<IProjectRefresher>();
        var mockOfProjectRefresher = Mock.Get(projectRefresher);

        var mockOfProjectWriter = factory.ServiceLocator.ResolveMocked<IProjectWriter>();

        mockOfProjectWriter.Setup(x => x.WriteAsync(It.IsAny<IProject>(), It.IsAny<string>())).Callback(() => Thread.Sleep(100)).CallBase().
            Callback<IProject, string>((project, _) => mockOfProjectRefresher.Raise(refresher => refresher.Updated += null, new ProjectEventArgs(project)));

        var mockOfProjectRefresherSelector = factory.ServiceLocator.ResolveMocked<IProjectRefresherSelector>();

        mockOfProjectRefresherSelector.Setup(x => x.GetProjectRefresher(It.IsAny<string>())).
            Returns(projectRefresher);
    }

    private static void SetupRegistrations(Factory factory, ProjectManagementType projectManagementType)
    {
        ArgumentNullException.ThrowIfNull(factory);

        switch (projectManagementType)
        {
            case ProjectManagementType.SingleDocument:
                factory.ServiceLocator.RegisterType<IProjectManagementConfigurationService, SdiProjectManagementConfigurationService>();
                break;

            case ProjectManagementType.MultipleDocuments:
                factory.ServiceLocator.RegisterType<IProjectManagementConfigurationService, MdiProjectManagementConfigurationService>();
                break;

            default:
                throw new ArgumentOutOfRangeException("projectManagementType", projectManagementType, null);
        }

        // Why mock all these services??
        factory.MockAndRegisterIfNotRegistered<IProjectReader, MemoryProjectReader>();
        factory.MockAndRegisterIfNotRegistered<IProjectWriter, MemoryProjectWriter>();

        factory.MockAndRegisterIfNotRegistered<IProjectSerializerSelector, DefaultProjectSerializerSelector>(factory.ServiceLocator);

        factory.ServiceLocator.RegisterType<IProjectValidator, EmptyProjectValidator>();
        factory.ServiceLocator.RegisterType<IProjectUpgrader, EmptyProjectUpgrader>();
        factory.ServiceLocator.RegisterType<IProjectStateService, ProjectStateService>();
        factory.ServiceLocator.RegisterType<IProjectActivationHistoryService, ProjectActivationHistoryService>();

        factory.MockAndRegisterIfNotRegistered<IProjectRefresher>();
        factory.MockAndRegisterIfNotRegistered<IProjectRefresherSelector>();

        factory.ServiceLocator.RegisterType<IProjectManagementInitializationService, ProjectManagementInitializationService>();

        factory.ServiceLocator.RegisterType<IProjectInitializer, EmptyProjectInitializer>();

        factory.ServiceLocator.RegisterType<IProjectManager, ProjectManager>();

        //var projectManager = factory.ServiceLocator.ResolveType<IProjectManager>() as ProjectManager;
        //factory.ServiceLocator.RegisterInstance<ProjectManager>(projectManager);
    }

    private static void MockAndRegisterIfNotRegistered<T>(this Factory factory)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(factory);

        var serviceLocator = factory.ServiceLocator;
        if (serviceLocator.IsTypeRegistered<T>())
        {
            return;
        }

        var mock = new Mock<T>(MockBehavior.Loose);

        serviceLocator.RegisterInstance(typeof (T), mock.Object);
    }

    private static void MockAndRegisterIfNotRegistered<TService, TServiceImplementation>(this Factory factory, params object[] args)
        where TService : class
        where TServiceImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(factory);

        var serviceLocator = factory.ServiceLocator;
        if (serviceLocator.IsTypeRegistered<TService>())
        {
            return;
        }

        var mock = factory.Mock<TServiceImplementation>(args);
        var instance = mock.Object;
        serviceLocator.RegisterInstance(typeof(TService), instance);
    }
}
