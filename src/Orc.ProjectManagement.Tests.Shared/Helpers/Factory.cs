// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MockFactory.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Catel;
    using Catel.IoC;
    using Moq;

    internal class Factory
    {
        #region Constructors
        private Factory(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => serviceLocator);

            ServiceLocator = serviceLocator;
        }
        #endregion

        #region Properties
        public IServiceLocator ServiceLocator { get; private set; }
        #endregion

        public static Factory Create()
        {
            var serviceLocator = new ServiceLocator();

            return new Factory(serviceLocator);
        }

        public T CreateInstance<T>(params object[] args) where T : class
        {
            var instance = CreateWithFactory((c, p) => (T) c.Invoke(p), typeof (T), args);

            return instance;
        }

        public Mock<T> Mock<T>(params object[] args) where T : class
        {
            var mock = CreateWithFactory((c, p) => new Mock<T>(MockBehavior.Loose, p) {CallBase = true}, typeof (T), args);

            return mock;
        }

        private T CreateWithFactory<T>(Func<ConstructorInfo, object[], T> factory, Type type, params object[] args) where T : class
        {
            Argument.IsNotNull(() => factory);
            Argument.IsNotNull(() => type);

            var constructors = FIlterConstructors(type, args);

            T inatance = null;

            var index = 0;
            while (inatance == null && index < constructors.Count())
            {
                var constructor = constructors[index++];

                var parameters = PrepareParameters(constructor);
                if (parameters == null)
                {
                    continue;
                }

                inatance = factory(constructor, args.Concat(parameters.Skip(args.Length)).ToArray());
            }

            return inatance;
        }

        private IEnumerable<object> PrepareParameters(ConstructorInfo constructor)
        {
            Argument.IsNotNull(() => constructor);

            var parameterInfos = constructor.GetParameters();

            var serviceLocator = ServiceLocator;

            if (parameterInfos.Any(p => !serviceLocator.IsTypeRegistered(p.ParameterType)))
            {
                return null;
            }

            return parameterInfos.Select(x => serviceLocator.ResolveType(x.ParameterType));
        }

        private static ConstructorInfo[] FIlterConstructors(Type type, params object[] args)
        {
            Argument.IsNotNull(() => type);

            var constructors = (from constructor in type.GetConstructors()
                                let parameters = constructor.GetParameters()
                                where !parameters.Any() || parameters.Length >= args.Length
                                orderby parameters.Length
                                select constructor).ToArray();

            return constructors;
        }
    }
}