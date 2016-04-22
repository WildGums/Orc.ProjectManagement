// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectStateEventArgs.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using Catel;

    public class ProjectStateEventArgs : EventArgs
    {
        public ProjectStateEventArgs(ProjectState projectState)
        {
            Argument.IsNotNull(() => projectState);

            ProjectState = projectState;
        }

        public ProjectState ProjectState { get; private set; }
    }
}