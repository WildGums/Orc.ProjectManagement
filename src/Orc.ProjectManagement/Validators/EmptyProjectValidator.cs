namespace Orc.ProjectManagement;

using System.Threading.Tasks;

public class EmptyProjectValidator : ProjectValidatorBase
{
    public override Task<bool> CanStartLoadingProjectAsync(string location)
    {
        return Task.FromResult(true);
    }
}