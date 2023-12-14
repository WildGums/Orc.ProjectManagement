namespace Orc.ProjectManagement;

using System.Threading.Tasks;

public interface IInitialProjectLocationService
{
    Task<string> GetInitialProjectLocationAsync();
}