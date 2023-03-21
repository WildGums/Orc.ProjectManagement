namespace Orc.ProjectManagement.Test.Mocks;

using System;
using System.Threading.Tasks;

public class MemoryProjectReader : ProjectReaderBase
{
    protected override async Task<IProject> ReadFromLocationAsync(string location)
    {
        if (string.Equals(location, "cannotload"))
        {
            throw new Exception("expected exception");
        }

        return new Project(location);
    }
}
