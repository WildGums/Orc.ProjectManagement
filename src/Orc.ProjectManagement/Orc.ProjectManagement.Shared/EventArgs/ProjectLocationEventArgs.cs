// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectLocationEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;

    public class ProjectLocationEventArgs : EventArgs
    {
        #region Constructors
        public ProjectLocationEventArgs(string location)
        {
            Location = location;
        }
        #endregion

        #region Properties
        public string Location { get; private set; }
        #endregion
    }
}