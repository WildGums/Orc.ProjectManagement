namespace Orc.ProjectManagement.Serialization;

/// <summary>
/// The default project serializer selector which uses the service locator.
/// </summary>
public class DefaultProjectSerializerSelector : IProjectSerializerSelector
{
    private readonly IProjectReader _projectReader;
    private readonly IProjectWriter _projectWriter;

    public DefaultProjectSerializerSelector(IProjectReader projectReader, IProjectWriter projectWriter)
    {
        _projectReader = projectReader;
        _projectWriter = projectWriter;
    }

    public IProjectReader GetReader(string location)
    {
        return _projectReader;
    }

    public IProjectWriter GetWriter(string location)
    {
        return _projectWriter;
    }
}
