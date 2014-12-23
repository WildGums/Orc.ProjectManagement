// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRefresher.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;

    public interface IRefresher
    {
        string Location { get; }
        bool IsSubscribed { get; }

        event EventHandler<EventArgs> Updated;

        void Subscribe();
        void Unsubscribe();
    }
}