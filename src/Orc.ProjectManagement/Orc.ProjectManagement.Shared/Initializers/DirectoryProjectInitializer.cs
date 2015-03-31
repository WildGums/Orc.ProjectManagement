// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.IO;
    using Catel;
    using Catel.Configuration;
    using Catel.Logging;
    using Services;

    public class DirectoryProjectInitializer : IProjectInitializer
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationService _configurationService;
        private readonly ICommandLineService _commandLineService;

        public DirectoryProjectInitializer(IConfigurationService configurationService, ICommandLineService commandLineService)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => commandLineService);

            _configurationService = configurationService;
            _commandLineService = commandLineService;
        }

        public virtual string GetInitialLocation()
        {
            var dataDirectory = _configurationService.GetValue<string>("DataLocation");
            if (string.IsNullOrWhiteSpace(dataDirectory))
            {
                dataDirectory = Path.Combine(Catel.IO.Path.GetApplicationDataDirectory(), "data");

                Log.Info("DataLocation is empty in configuration, determining the data directory automatically to '{0}'", dataDirectory);
            }

            if (_commandLineService.Arguments.Length > 0)
            {
                dataDirectory = _commandLineService.Arguments[0];
            }

            var fullPath = Path.GetFullPath(dataDirectory);
            if (!Directory.Exists(fullPath))
            {
                Log.Warning("Cannot use the data directory '{0}', it does not exist", fullPath);
                return null;
            }

            return fullPath;
        }
    }
}