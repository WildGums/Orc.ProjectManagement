// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectConfigurationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public abstract class ProjectManagementConfigurationService : IProjectManagementConfigurationService
    {
        public abstract ProjectManagementType GetProjectManagementType();
    }
}