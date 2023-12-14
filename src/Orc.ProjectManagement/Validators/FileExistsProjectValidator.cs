namespace Orc.ProjectManagement;

using System.IO;
using System.Threading.Tasks;

public class FileExistsProjectValidator : ProjectValidatorBase
{
    public override Task<bool> CanStartLoadingProjectAsync(string location)
    {
        var canStart = File.Exists(location);
        return Task.FromResult(canStart);
    }
}