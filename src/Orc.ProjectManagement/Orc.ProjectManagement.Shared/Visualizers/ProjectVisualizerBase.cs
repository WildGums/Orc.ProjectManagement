// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectVisualizerBase.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;

    public abstract class ProjectVisualizerBase : IProjectVisualizer
    {
        #region Methods
        public async Task EnsureReadyForVisualize(IProject project)
        {
            Argument.IsNotNull(() => project);

            await Task.Factory.StartNew(() =>
            {
                while (!IsReadyForVisualize(project))
                {
                    Thread.Sleep(100);
                }
            });
        }

        public void ExecuteWhenReady(IProject project, Action action)
        {
            Task.Factory.StartNew(() =>
            {
                while (!IsReadyForVisualize(project))
                {
                    Thread.Sleep(100);
                }

                action();
            });
        }

        protected abstract bool IsReadyForVisualize(IProject project);
        #endregion
    }
}