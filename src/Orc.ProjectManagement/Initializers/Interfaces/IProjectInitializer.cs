namespace Orc.ProjectManagement;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IProjectInitializer
{
    Task<IReadOnlyList<string>> GetInitialLocationsAsync();
}
