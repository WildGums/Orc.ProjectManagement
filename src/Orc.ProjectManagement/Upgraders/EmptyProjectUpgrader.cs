namespace Orc.ProjectManagement;

using System.Threading.Tasks;

public class EmptyProjectUpgrader : IProjectUpgrader
{
    public virtual Task<bool> RequiresUpgradeAsync(string location)
    {
        return Task.FromResult(false);
    }

    public virtual async Task<string> UpgradeAsync(string location)
    {
        return location;
    }
}