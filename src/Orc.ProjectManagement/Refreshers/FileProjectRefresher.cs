namespace Orc.ProjectManagement;

using System.IO;

public class FileProjectRefresher : DirectoryProjectRefresher
{
    public FileProjectRefresher(string projectLocation) 
        : base(projectLocation, Path.GetDirectoryName(projectLocation) ?? string.Empty, Path.GetFileName(projectLocation))
    {
    }
}