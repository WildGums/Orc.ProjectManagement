namespace Orc.ProjectManagement;

using System.IO;
using System.Threading.Tasks;

public class DirectoryExistsProjectValidator : ProjectValidatorBase
{
    public override async Task<bool> CanStartLoadingProjectAsync(string location)
    {
        return Directory.Exists(location);
    }
}