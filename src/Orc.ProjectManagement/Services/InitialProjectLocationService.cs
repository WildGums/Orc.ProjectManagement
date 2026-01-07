namespace Orc.ProjectManagement;

using System;
using System.Threading.Tasks;
using Catel.Logging;
using Microsoft.Extensions.Logging;

public class InitialProjectLocationService : IInitialProjectLocationService
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(InitialProjectLocationService));

    public virtual async Task<string?> GetInitialProjectLocationAsync()
    {
        throw Logger.LogErrorAndCreateException<NotImplementedException>($"To use the initial project location service, implement it yourself, for example by returning the first argument from the command line arguments");
    }
}
