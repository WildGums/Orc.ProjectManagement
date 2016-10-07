// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectConfigurationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public class MdiProjectManagementConfigurationService : ProjectManagementConfigurationService
    {
        public override ProjectManagementType GetProjectManagementType()
        {
            return ProjectManagementType.MultipleDocuments;
        }
    }
}