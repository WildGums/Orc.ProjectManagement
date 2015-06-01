// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Collections.Generic;
    using System.Linq;

    public class EmptyProjectInitializer : IProjectInitializer
    {
        [ObsoleteEx(ReplacementTypeOrMember = "GetInitialLocations", RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
        public virtual string GetInitialLocation()
        {
            return GetInitialLocations().FirstOrDefault();
        }

        public virtual IEnumerable<string> GetInitialLocations()
        {
            return Enumerable.Empty<string>();
        }
    }
}