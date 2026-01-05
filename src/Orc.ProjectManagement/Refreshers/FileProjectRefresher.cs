namespace Orc.ProjectManagement;

using System;
using System.IO;

public class FileProjectRefresher : DirectoryProjectRefresher
{
    public FileProjectRefresher(string projectLocation, IServiceProvider serviceProvider) 
        : base(projectLocation, Path.GetDirectoryName(projectLocation) ?? string.Empty, 
            Path.GetFileName(projectLocation), serviceProvider)
    {
    }
}
