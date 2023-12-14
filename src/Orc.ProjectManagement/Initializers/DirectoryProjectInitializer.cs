namespace Orc.ProjectManagement;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Catel.Configuration;
using Catel.Logging;

public class DirectoryProjectInitializer : IProjectInitializer
{
    private readonly IInitialProjectLocationService _initialProjectLocationService;
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    private readonly IConfigurationService _configurationService;

    public DirectoryProjectInitializer(IConfigurationService configurationService, IInitialProjectLocationService initialProjectLocationService)
    {
        ArgumentNullException.ThrowIfNull(configurationService);
        ArgumentNullException.ThrowIfNull(initialProjectLocationService);

        _configurationService = configurationService;
        _initialProjectLocationService = initialProjectLocationService;
    }

    public virtual async Task<IEnumerable<string>> GetInitialLocationsAsync()
    {
        var locations = new List<string>();
        var dataDirectory = _configurationService.GetRoamingValue<string>("DataLocation");
        if (string.IsNullOrWhiteSpace(dataDirectory))
        {
            dataDirectory = Path.Combine(Catel.IO.Path.GetApplicationDataDirectory(), "data");

            Log.Debug("DataLocation is empty in configuration, determining the data directory automatically to '{0}'", dataDirectory);
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
            Log.Debug("Cannot use the data directory '{0}', it does not exist", fullPath);
            return Array.Empty<string>();
        }

        locations.Add(fullPath);

        return locations;
    }
}