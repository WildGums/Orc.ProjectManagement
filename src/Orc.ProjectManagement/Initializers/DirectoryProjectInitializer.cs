namespace Orc.ProjectManagement;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Catel.Configuration;
using Catel.Logging;
using Catel.Services;
using Microsoft.Extensions.Logging;

public class DirectoryProjectInitializer : IProjectInitializer
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(DirectoryProjectInitializer));

    private readonly IInitialProjectLocationService _initialProjectLocationService;
    private readonly IAppDataService _appDataService;

    private readonly IConfigurationService _configurationService;

    public DirectoryProjectInitializer(IConfigurationService configurationService, IInitialProjectLocationService initialProjectLocationService, IAppDataService appDataService)
    {
        ArgumentNullException.ThrowIfNull(configurationService);
        ArgumentNullException.ThrowIfNull(initialProjectLocationService);
        ArgumentNullException.ThrowIfNull(appDataService);

        _configurationService = configurationService;
        _initialProjectLocationService = initialProjectLocationService;
        _appDataService = appDataService;
    }

    public virtual async Task<IReadOnlyList<string>> GetInitialLocationsAsync()
    {
        var locations = new List<string>();
        var dataDirectory = _configurationService.GetRoamingValue<string>("DataLocation");
        if (string.IsNullOrWhiteSpace(dataDirectory))
        {
            dataDirectory = Path.Combine(Catel.IO.Path.GetApplicationDataDirectory(), "data");

            Logger.LogDebug("DataLocation is empty in configuration, determining the data directory automatically to '{DataDirectory}'", dataDirectory);
        }

        var initialLocation = await _initialProjectLocationService.GetInitialProjectLocationAsync();
        if (!string.IsNullOrWhiteSpace(initialLocation))
        {
            dataDirectory = initialLocation;
        }

        if (string.IsNullOrWhiteSpace(initialLocation))
        {
            return locations;
        }

        var fullPath = Path.GetFullPath(dataDirectory);
        if (!Directory.Exists(fullPath))
        {
            Logger.LogDebug("Cannot use the data directory '{FullPath}', it does not exist", fullPath);
            return Array.Empty<string>();
        }

        locations.Add(fullPath);

        return locations;
    }
}
