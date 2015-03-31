// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectRefresherSelector.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public interface IProjectRefresherSelector
    {
        IProjectRefresher GetProjectRefresher(string location);
    }
}