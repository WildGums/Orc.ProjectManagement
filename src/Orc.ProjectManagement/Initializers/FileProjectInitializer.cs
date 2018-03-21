// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Services;

    public class FileProjectInitializer : IProjectInitializer
    {
        private readonly IInitialProjectLocationService _initialProjectLocationService;

        public FileProjectInitializer(IInitialProjectLocationService initialProjectLocationService)
        {
            Argument.IsNotNull(() => initialProjectLocationService);

            _initialProjectLocationService = initialProjectLocationService;
        }

        public virtual async Task<IEnumerable<string>> GetInitialLocationsAsync()
        {
            var locations = new List<string>();

            var initialLocation = await _initialProjectLocationService.GetInitialProjectLocationAsync();
            if (!string.IsNullOrWhiteSpace(initialLocation))
            {
                locations.Add(initialLocation);
            }

            return locations;
        }
    }
}