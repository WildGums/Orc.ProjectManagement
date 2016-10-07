// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyProjectUpgrader.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;
    using Catel.Threading;

    public class EmptyProjectUpgrader : IProjectUpgrader
    {
        public virtual Task<bool> RequiresUpgradeAsync(string location)
        {
            return TaskHelper<bool>.FromResult(false);
        }

        public virtual async Task<string> UpgradeAsync(string location)
        {
            return location;
        }
    }
}