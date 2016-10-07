// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
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