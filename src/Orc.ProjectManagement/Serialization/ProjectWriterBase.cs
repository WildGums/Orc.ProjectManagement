namespace Orc.ProjectManagement;

using System;
using System.Threading.Tasks;
using Catel;
using Catel.Logging;
using MethodTimer;
using Microsoft.Extensions.Logging;

public abstract class ProjectWriterBase<TProject> : IProjectWriter
    where TProject : IProject
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ProjectWriterBase<TProject>));

    [Time]
    public async Task<bool> WriteAsync(IProject project, string location)
    {
        ArgumentNullException.ThrowIfNull(project);
        Argument.IsNotNullOrWhitespace(() => location);

        Logger.LogDebug("Writing all data to '{Location}'", location);

        if (!await WriteToLocationAsync((TProject) project, location).ConfigureAwait(false))
        {
            Logger.LogWarning("Failed to write all data to '{Location}'", location);
            return false;
        }

        project.Location = location;
        project.ClearIsDirty();

        Logger.LogInformation("Wrote all data to '{Location}'", location);

        return true;
    }

    protected abstract Task<bool> WriteToLocationAsync(TProject project, string location);
}
