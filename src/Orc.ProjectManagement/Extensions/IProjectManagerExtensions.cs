// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectManagerExtensions.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using Catel;

    public static class IProjectManagerExtensions
    {
        public static TProject GetProject<TProject>(this IProjectManager projectManager)
            where TProject : IProject
        {
            Argument.IsNotNull("projectManager", projectManager);

            return (TProject)projectManager.Project;
        }
    }
}