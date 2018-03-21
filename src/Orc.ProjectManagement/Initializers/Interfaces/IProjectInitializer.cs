// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IProjectInitializer
    {
        Task<IEnumerable<string>> GetInitialLocationsAsync();
    }
}