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
        public virtual IEnumerable<string> GetInitialLocations()
        {
            return Enumerable.Empty<string>();
        }
    }
}