// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectLocationCancelEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.ComponentModel;

    public class ProjectLocationCancelEventArgs : CancelEventArgs
    {
        #region Constructors
        public ProjectLocationCancelEventArgs(string location, bool cancel = false)
            : base(cancel)
        {
            Location = location;
        }
        #endregion

        #region Properties
        public string Location { get; private set; }
        #endregion
    }
}