// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
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