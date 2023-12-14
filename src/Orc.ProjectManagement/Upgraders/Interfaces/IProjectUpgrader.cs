namespace Orc.ProjectManagement;

using System.Threading.Tasks;

public interface IProjectUpgrader
{
    Task<bool> RequiresUpgradeAsync(string location);
    Task<string> UpgradeAsync(string location);
}