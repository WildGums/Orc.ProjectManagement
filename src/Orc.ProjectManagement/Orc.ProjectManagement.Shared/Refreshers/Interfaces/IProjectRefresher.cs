// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectRefresher.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
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