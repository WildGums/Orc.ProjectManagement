namespace Orc.ProjectManagement.Serialization;

using System;
using Microsoft.Extensions.DependencyInjection;

public class FixedProjectSerializerSelector<TReader, TWriter> : IProjectSerializerSelector
    where TReader : IProjectReader
    where TWriter : IProjectWriter
{
    private readonly IServiceProvider _serviceProvider;

    public FixedProjectSerializerSelector(IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        _serviceProvider = serviceProvider;
    }

    public IProjectReader GetReader(string location)
    {
        return ActivatorUtilities.CreateInstance<TReader>(_serviceProvider);
    }

    public IProjectWriter GetWriter(string location)
    {
        return ActivatorUtilities.CreateInstance<TWriter>(_serviceProvider);
    }
}
