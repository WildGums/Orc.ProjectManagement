namespace Orc.ProjectManagement;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EmptyProjectInitializer : IProjectInitializer
{
    public virtual Task<IReadOnlyList<string>> GetInitialLocationsAsync()
    {
        return Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
    }
}
