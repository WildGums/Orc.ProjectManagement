namespace Orc.ProjectManagement.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Catel.IoC;
using Moq;

internal class Factory
{
    private Factory(IServiceLocator serviceLocator)
    {
        ArgumentNullException.ThrowIfNull(serviceLocator);

        ServiceLocator = serviceLocator;
    }

#pragma warning disable IDISP006 // Implement IDisposable.
    public IServiceLocator ServiceLocator { get; }
#pragma warning restore IDISP006 // Implement IDisposable.

    public static Factory Create()
    {
#pragma warning disable IDISP001 // Dispose created.
        var serviceLocator = new ServiceLocator();
#pragma warning restore IDISP001 // Dispose created.

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

    private T? CreateWithFactory<T>(Func<ConstructorInfo, object[], T> factory, Type type, params object[] args) where T : class
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(type);

        var constructors = FilterConstructors(type, args);

        T? instance = null;

        var index = 0;
        while (instance is null && index < constructors.Count())
        {
            var constructor = constructors[index++];

            var parameters = PrepareParameters(constructor);
            if (parameters is null)
            {
                continue;
            }

            instance = factory(constructor, args.Concat(parameters.Skip(args.Length)).ToArray());
        }

        return instance;
    }

    private IEnumerable<object>? PrepareParameters(ConstructorInfo constructor)
    {
        ArgumentNullException.ThrowIfNull(constructor);

        var parameterInfos = constructor.GetParameters();

        var serviceLocator = ServiceLocator;

        return parameterInfos.All(p => serviceLocator.IsTypeRegistered(p.ParameterType))
            ? parameterInfos.Select(x => serviceLocator.ResolveType(x.ParameterType)) 
            : null;
    }

    private static ConstructorInfo[] FilterConstructors(Type type, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(type);

        var constructors = (from constructor in type.GetConstructors()
            let parameters = constructor.GetParameters()
            where !parameters.Any() || parameters.Length >= args.Length
            orderby parameters.Length
            select constructor).ToArray();

        return constructors;
    }
}
