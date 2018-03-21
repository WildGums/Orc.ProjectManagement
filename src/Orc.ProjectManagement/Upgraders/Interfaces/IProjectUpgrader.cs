// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectUpgrader.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;

    public interface IProjectUpgrader
    {
        Task<bool> RequiresUpgradeAsync(string location);
        Task<string> UpgradeAsync(string location);
    }
}