namespace Orc.ProjectManagement.Tests;

using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;

internal static class IServiceProviderExtensions
{
    public static Mock<T> ResolveMocked<T>(this IServiceProvider serviceProvider) 
        where T : class
    {
        var instance = serviceProvider.GetRequiredService<T>();

        return Mock.Get(instance);
    }
}
