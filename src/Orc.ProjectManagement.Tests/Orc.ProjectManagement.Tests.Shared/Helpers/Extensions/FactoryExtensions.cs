// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FactoryExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Tests
{
    using Catel;
    using Catel.IoC;
    using Moq;
    using Serialization;
    using Test.Mocks;

    internal static class FactoryExtensions
    {
        public static IProjectManager GetProjectManager(this Factory factory)
        {
            Argument.IsNotNull(() => factory);

            var projectManager = factory.CreateInstance<ProjectManager>();

            return projectManager;
        }

        public static void MockAndRegisterIfNotRegistered<T>(this Factory factory)
            where T : class
        {
            Argument.IsNotNull(() => factory);

            var serviceLocator = factory.ServiceLocator;

            if (serviceLocator.IsTypeRegistered<T>())
            {
                return;
            }

            var mock = new Mock<T>(MockBehavior.Loose);

            serviceLocator.RegisterInstance(typeof(T), mock.Object);
        }

        public static void MockAndRegisterIfNotRegistered<TService, TServiceImplementation>(this Factory factory, params object[] args)
            where TService : class
            where TServiceImplementation : class, TService
        {
            Argument.IsNotNull(() => factory);

            var serviceLocator = factory.ServiceLocator;

            if (serviceLocator.IsTypeRegistered<TService>())
            {
                return;
            }

            var mock = factory.Mock<TServiceImplementation>(args);
            var instance = mock.Object;

            serviceLocator.RegisterInstance(typeof(TService), instance);
        }

        public static Factory SetupDefault(this Factory factory)
        {
            Argument.IsNotNull(() => factory);

            factory.MockAndRegisterIfNotRegistered<IProjectReader, MemoryProjectReader>();
            factory.MockAndRegisterIfNotRegistered<IProjectWriter, MemoryProjectWriter>();

            factory.MockAndRegisterIfNotRegistered<IProjectSerializerSelector, DefaultProjectSerializerSelector>(factory.ServiceLocator);

            factory.MockAndRegisterIfNotRegistered<IProjectValidator, EmptyProjectValidator>();
            factory.MockAndRegisterIfNotRegistered<IProjectRefresher>();
            factory.MockAndRegisterIfNotRegistered<IProjectRefresherSelector>();

            factory.MockAndRegisterIfNotRegistered<IProjectInitializer, EmptyProjectInitializer>();

            return factory;
        }
    }
}