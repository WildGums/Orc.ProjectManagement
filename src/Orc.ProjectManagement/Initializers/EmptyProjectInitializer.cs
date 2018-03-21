// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInitializer.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel.Collections;
    using Catel.Threading;

    public class EmptyProjectInitializer : IProjectInitializer
    {
        public virtual Task<IEnumerable<string>> GetInitialLocationsAsync()
        {
            return TaskHelper<IEnumerable<string>>.FromResult(ArrayShim.Empty<string>());
        }
    }
}