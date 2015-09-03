// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectManagerExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using Catel;

    public static class IProjectManagerExtensions
    {
        #region Methods
        public static TProject GetActiveProject<TProject>(this IProjectManager projectManager)
            where TProject : IProject
        {
            Argument.IsNotNull(() => projectManager);

            return (TProject)projectManager.ActiveProject;
        }

        public static string GetActiveProjectLocation(this IProjectManager projectManager)
        {
            Argument.IsNotNull(() => projectManager);

            var activeProject = projectManager.ActiveProject;
            return activeProject == null ? null : activeProject.Location;
        }
        #endregion
    }
}