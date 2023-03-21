namespace Orc.ProjectManagement;

using System;
using System.Threading.Tasks;
using Catel;
using Catel.Logging;
using MethodTimer;

public abstract class ProjectWriterBase<TProject> : IProjectWriter
    where TProject : IProject
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    [Time]
    public async Task<bool> WriteAsync(IProject project, string location)
    {
        ArgumentNullException.ThrowIfNull(project);
        Argument.IsNotNullOrWhitespace(() => location);

        Log.Debug("Writing all data to '{0}'", location);

        if (!await WriteToLocationAsync((TProject) project, location).ConfigureAwait(false))
        {
            Log.Warning("Failed to write all data to '{0}'", location);
            return false;
        }

        project.Location = location;
        project.ClearIsDirty();

        Log.Info("Wrote all data to '{0}'", location);

        return true;
    }

    protected abstract Task<bool> WriteToLocationAsync(TProject project, string location);
}