// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Configuration;
    using Catel.Logging;

    public class DirectoryProjectInitializer : IProjectInitializer
    {
        private readonly IInitialProjectLocationService _initialProjectLocationService;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationService _configurationService;

        public DirectoryProjectInitializer(IConfigurationService configurationService, IInitialProjectLocationService initialProjectLocationService)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => initialProjectLocationService);

            _configurationService = configurationService;
            _initialProjectLocationService = initialProjectLocationService;
        }

        public virtual async Task<IEnumerable<string>> GetInitialLocationsAsync()
        {
            var locations = new List<string>();

            var silence = true;
            var dataDirectory = _configurationService.GetRoamingValue<string>("DataLocation");
            if (string.IsNullOrWhiteSpace(dataDirectory))
            {
                // TODO: This code should be available in Catel 5.4 for all platforms
#if NET
                dataDirectory = Path.Combine(Catel.IO.Path.GetApplicationDataDirectory(), "data");

                Log.Debug("DataLocation is empty in configuration, determining the data directory automatically to '{0}'", dataDirectory);
#endif
            }

            var initialLocation = await _initialProjectLocationService.GetInitialProjectLocationAsync();
            if (!string.IsNullOrWhiteSpace(initialLocation))
            {
                dataDirectory = initialLocation;
                silence = false;
            }

            if (string.IsNullOrWhiteSpace(initialLocation))
            {
                return locations;
            }

            var fullPath = Path.GetFullPath(dataDirectory);
            if (!Directory.Exists(fullPath))
            {
                var message = string.Format("Cannot use the data directory '{0}', it does not exist", fullPath);

                if (!silence)
                {
                    Log.Warning(message);
                }
                else
                {
                    Log.Debug(message);
                }

                return null;
            }

            locations.Add(fullPath);

            return locations;
        }
    }
}