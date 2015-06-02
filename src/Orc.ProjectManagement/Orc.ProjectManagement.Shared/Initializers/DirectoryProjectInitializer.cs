// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.Configuration;
    using Catel.Logging;
    using Catel.Services;
    using Services;

    public class DirectoryProjectInitializer : IProjectInitializer
    {
        private readonly IStartUpInfoProvider _startUpInfoProvider;
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IConfigurationService _configurationService;

        public DirectoryProjectInitializer(IConfigurationService configurationService, IStartUpInfoProvider startUpInfoProvider)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => startUpInfoProvider);
            
            _configurationService = configurationService;
            _startUpInfoProvider = startUpInfoProvider;
        }

        [ObsoleteEx(ReplacementTypeOrMember = "GetInitialLocations", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public virtual string GetInitialLocation()
        {
            return GetInitialLocations().FirstOrDefault();
        }

        public virtual IEnumerable<string> GetInitialLocations()
        {
            var dataDirectory = _configurationService.GetValue<string>("DataLocation");
            if (string.IsNullOrWhiteSpace(dataDirectory))
            {
                dataDirectory = Path.Combine(Catel.IO.Path.GetApplicationDataDirectory(), "data");

                Log.Info("DataLocation is empty in configuration, determining the data directory automatically to '{0}'", dataDirectory);
            }

            if (_startUpInfoProvider.Arguments.Length > 0)
            {
                dataDirectory = _startUpInfoProvider.Arguments[0];
            }

            var fullPath = Path.GetFullPath(dataDirectory);
            if (!Directory.Exists(fullPath))
            {
                Log.Warning("Cannot use the data directory '{0}', it does not exist", fullPath);
                yield break;
            }

            yield return fullPath;
        }
    }
}