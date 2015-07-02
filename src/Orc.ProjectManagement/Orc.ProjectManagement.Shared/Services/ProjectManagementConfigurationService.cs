// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectConfigurationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public abstract class ProjectManagementConfigurationService : IProjectManagementConfigurationService
    {
        public abstract ProjectManagementType GetProjectManagementType();
    }
}