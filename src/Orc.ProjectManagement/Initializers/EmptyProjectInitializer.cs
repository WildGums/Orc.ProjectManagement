namespace Orc.ProjectManagement;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EmptyProjectInitializer : IProjectInitializer
{
    public virtual Task<IEnumerable<string>> GetInitialLocationsAsync()
    {
        return Task.FromResult<IEnumerable<string>>(Array.Empty<string>());
    }
}