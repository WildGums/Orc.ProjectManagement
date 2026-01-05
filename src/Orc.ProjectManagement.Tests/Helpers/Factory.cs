namespace Orc.ProjectManagement.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Catel;
using Microsoft.Extensions.DependencyInjection;
using Moq;

internal sealed class Factory : IDisposable
{
    private bool _disposed;

    private IServiceProvider? _serviceProvider;

    private readonly IServiceCollection _serviceCollection;

    private Factory(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public IServiceProvider ServiceProvider
    {
        get
        {
            var serviceProvider = _serviceProvider;
            if (serviceProvider is null)
            {
#pragma warning disable IDISP003 // Dispose previous before re-assigning
                _serviceProvider = ServiceCollection.BuildServiceProvider();
#pragma warning restore IDISP003 // Dispose previous before re-assigning
            }

            return _serviceProvider;
        }
    }

    public IServiceCollection ServiceCollection
    {
        get
        {
            if (_serviceProvider is not null)
            {
                throw new NotSupportedException("The service provider has already been built, cannot customize services");
            }

            return _serviceCollection;
        }
    }

    public static Factory Create()
    {
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        return new Factory(serviceCollection);
    }

    public T CreateInstance<T>(params object[] args)
        where T : class
    {
        var instance = CreateWithFactory((c, p) => (T)c.Invoke(p), typeof(T), args);

        return instance;
    }

    public Mock<T> Mock<T>(params object[] args)
        where T : class
    {
        var mock = CreateWithFactory((c, p) => new Mock<T>(MockBehavior.Loose, p)
        {
            CallBase = true
        }, typeof(Mock<T>), args);

        return mock;
    }

    private T CreateWithFactory<T>(Func<ConstructorInfo, object[], T> factory, Type type, params object[] args)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(type);

        var constructors = FilterConstructors(type, args);

        T instance = null;

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

    private IEnumerable<object> PrepareParameters(ConstructorInfo constructor)
    {
        ArgumentNullException.ThrowIfNull(constructor);

        var parameterInfos = constructor.GetParameters();

        var serviceProvider = ServiceProvider;

        if (parameterInfos.Any(p => !serviceProvider.IsRegistered(p.ParameterType)))
        {
            return null;
        }

        return parameterInfos.Select(x => serviceProvider.GetRequiredService(x.ParameterType));
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

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        var serviceProvider = _serviceProvider as IDisposable;
        serviceProvider?.Dispose();

        _disposed = true;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
