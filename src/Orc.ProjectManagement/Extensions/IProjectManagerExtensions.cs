// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectManagerExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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
            return activeProject is null ? null : activeProject.Location;
        }
        #endregion
    }
}