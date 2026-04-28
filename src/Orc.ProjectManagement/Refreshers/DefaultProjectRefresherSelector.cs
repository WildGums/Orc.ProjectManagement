namespace Orc.ProjectManagement;

using System;
using Catel;
using Catel.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class DefaultProjectRefresherSelector : IProjectRefresherSelector
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultProjectRefresherSelector(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IProjectRefresher? GetProjectRefresher(string location)
    {
        var serviceDescriptors = _serviceProvider.GetServiceDescriptors<IProjectRefresher>();
        if (serviceDescriptors.Count == 0)
        {
            return null;
        }

        foreach (var serviceDescriptor in serviceDescriptors)
        {
            if (serviceDescriptor.Lifetime != ServiceLifetime.Transient)
            {
                continue;
            }

            return ActivatorUtilities.CreateInstance(_serviceProvider, serviceDescriptor.ImplementationType!, location) as IProjectRefresher;
        }

        return null;
    }
}
