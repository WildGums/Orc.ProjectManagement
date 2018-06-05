// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectRefresher.cs" company="WildGums">
//   Copyright (c) 2008 - 2018 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;

    public interface IProjectRefresher
    {
        #region Properties
        string Location { get; }
        bool IsSubscribed { get; }
        bool IsEnabled { get; set; }
        #endregion

        #region Methods
        event EventHandler<ProjectEventArgs> Updated;

        void Subscribe();
        void Unsubscribe();
        #endregion
    }
}
