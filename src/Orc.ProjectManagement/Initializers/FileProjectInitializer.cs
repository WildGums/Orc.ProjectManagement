namespace Orc.ProjectManagement
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class FileProjectInitializer : IProjectInitializer
    {
        private readonly IInitialProjectLocationService _initialProjectLocationService;

        public FileProjectInitializer(IInitialProjectLocationService initialProjectLocationService)
        {
            ArgumentNullException.ThrowIfNull(initialProjectLocationService);

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

            return locations.ToArray();
        }
    }
}
