// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectManagementInitializationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public interface IProjectManagementInitializationService
    {
        void Initialize(IProjectManager projectManager);
    }
}