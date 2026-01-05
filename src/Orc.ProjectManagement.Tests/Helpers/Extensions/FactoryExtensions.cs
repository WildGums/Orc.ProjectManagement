namespace Orc.ProjectManagement.Tests;

using System;
using System.Threading;
using Catel;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        var serviceProvider = factory.ServiceProvider;
        if (serviceProvider.IsRegistered<IProjectManager>())
        {
            return serviceProvider.GetRequiredService<IProjectManager>();
        }

        var projectManager = factory.CreateInstance<ProjectManager>();
        return projectManager;
    }

    //public static Mock<IProjectManager> MockProjectManager(this Factory factory)
    //{
    //    ArgumentNullException.ThrowIfNull(factory);

    //    var mock = factory.ServiceProvider.ResolveMocked<IProjectManager>();
    //    return mock;
    //}

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

        var projectRefresher = factory.ServiceProvider.GetRequiredService<IProjectRefresher>();
        var mockOfProjectRefresher = Mock.Get(projectRefresher);

        var mockOfProjectWriter = factory.ServiceProvider.ResolveMocked<IProjectWriter>();

        mockOfProjectWriter.Setup(x => x.WriteAsync(It.IsAny<IProject>(), It.IsAny<string>())).Callback(() => Thread.Sleep(100)).CallBase().
            Callback<IProject, string>((project, location) => mockOfProjectRefresher.Raise(refresher => refresher.Updated += null, new ProjectEventArgs(project)));

        var mockOfProjectRefresherSelector = factory.ServiceProvider.ResolveMocked<IProjectRefresherSelector>();

        mockOfProjectRefresherSelector.Setup(x => x.GetProjectRefresher(It.IsAny<string>())).
            Returns(projectRefresher);
    }

    private static void SetupRegistrations(Factory factory, ProjectManagementType projectManagementType)
    {
        ArgumentNullException.ThrowIfNull(factory);

        //switch (projectManagementType)
        //{
        //    case ProjectManagementType.SingleDocument:
        //        factory.ServiceProvider.RegisterType<IProjectManagementConfigurationService, SdiProjectManagementConfigurationService>();
        //        break;

        //    case ProjectManagementType.MultipleDocuments:
        //        factory.ServiceProvider.RegisterType<IProjectManagementConfigurationService, MdiProjectManagementConfigurationService>();
        //        break;

        //    default:
        //        throw new ArgumentOutOfRangeException("projectManagementType", projectManagementType, null);
        //}

        // Why mock all these services??
        factory.MockAndRegisterIfNotRegistered<IProjectReader, MemoryProjectReader>();
        factory.MockAndRegisterIfNotRegistered<IProjectWriter, MemoryProjectWriter>();

        factory.MockAndRegisterIfNotRegistered<IProjectSerializerSelector, DefaultProjectSerializerSelector>(factory.ServiceCollection);

        factory.ServiceCollection.AddSingleton<IProjectValidator, EmptyProjectValidator>();
        factory.ServiceCollection.AddSingleton<IProjectUpgrader, EmptyProjectUpgrader>();
        factory.ServiceCollection.AddSingleton<IProjectStateService, ProjectStateService>();
        factory.ServiceCollection.AddSingleton<IProjectActivationHistoryService, ProjectActivationHistoryService>();

        factory.MockAndRegister<IProjectRefresher>();
        factory.MockAndRegister<IProjectRefresherSelector>();

        factory.ServiceCollection.AddSingleton<IProjectManagementInitializationService, ProjectManagementInitializationService>();

        factory.ServiceCollection.AddSingleton<IProjectInitializer, EmptyProjectInitializer>();

        factory.ServiceCollection.AddSingleton<IProjectManager, ProjectManager>();

        //var projectManager = factory.ServiceLocator.ResolveType<IProjectManager>() as ProjectManager;
        //factory.ServiceLocator.RegisterInstance<ProjectManager>(projectManager);
    }

    private static void MockAndRegister<T>(this Factory factory)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(factory);

        var serviceCollection = factory.ServiceCollection;

        serviceCollection.AddSingleton<T>(x =>
        {
            var mock = factory.Mock<T>();
            return mock.Object;
        });
    }

    private static void MockAndRegisterIfNotRegistered<T>(this Factory factory)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(factory);

        var serviceCollection = factory.ServiceCollection;

        serviceCollection.TryAddSingleton<T>(x =>
        {
            var mock = factory.Mock<T>();
            return mock.Object;
        });
    }

    private static void MockAndRegisterIfNotRegistered<TService, TServiceImplementation>(this Factory factory, params object[] args)
        where TService : class
        where TServiceImplementation : class, TService
    {
        ArgumentNullException.ThrowIfNull(factory);

        var serviceCollection = factory.ServiceCollection;

        serviceCollection.TryAddSingleton<TService>(x =>
        {
            var mock = factory.Mock<TServiceImplementation>(args);
            return mock.Object;
        });
    }
}
