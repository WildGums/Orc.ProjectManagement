// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectRefresher.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;

    public interface IProjectRefresher
    {
        string Location { get; }
        bool IsSubscribed { get; }

        event EventHandler<ProjectEventArgs> Updated;

        void Subscribe();
        void Unsubscribe();
    }
}