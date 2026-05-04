namespace Orc.ProjectManagement;

using System.Threading.Tasks;
using Catel;
using Catel.Logging;
using MethodTimer;
using Microsoft.Extensions.Logging;

public abstract class ProjectReaderBase : IProjectReader
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ProjectReaderBase));

    [Time]
    public async Task<IProject?> ReadAsync(string location)
    {
        Argument.IsNotNullOrWhitespace(() => location);

        Logger.LogDebug("Reading data from '{Location}'", location);

        var project = await ReadFromLocationAsync(location).ConfigureAwait(false);
        if (project is null)
        {
            Logger.LogInformation("Project reader returned no project");
        }
        else
        {
            project.ClearIsDirty();

            Logger.LogInformation("Read data from '{Location}'", location);
        }

        return project;
    }

    protected abstract Task<IProject?> ReadFromLocationAsync(string location);
}
