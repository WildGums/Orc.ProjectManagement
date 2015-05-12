// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectVisualizer.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;

    public interface IProjectVisualizer
    {
        #region Methods
        Task EnsureReadyForVisualize(IProject project);
        #endregion

        void ExecuteWhenReady(IProject project, Action action);
    }
}