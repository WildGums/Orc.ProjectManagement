namespace Orc.ProjectManagement.Tests;

using Catel;
using Microsoft.Extensions.DependencyInjection;
using Orc.ProjectManagement;

internal static class ServiceCollectionHelper
{
    public static IServiceCollection CreateServiceCollection()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLogging();
        serviceCollection.AddCatelCore();
        serviceCollection.AddOrcProjectManagement();

        return serviceCollection;
    }
}
