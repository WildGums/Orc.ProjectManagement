// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Collections.Generic;

    public interface IProjectInitializer
    {
        IEnumerable<string> GetInitialLocations();
    }
}