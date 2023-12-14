namespace Orc.ProjectManagement.Test.Mocks;

using System.Threading.Tasks;

public class MemoryProjectWriter : ProjectWriterBase<Project>
{
    protected override Task<bool> WriteToLocationAsync(Project project, string location)
    {
        // no implementation required
        return Task.FromResult(true);
    }
}
