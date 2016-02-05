// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectRefresherSelector.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public interface IProjectRefresherSelector
    {
        IProjectRefresher GetProjectRefresher(string location);
    }
}