// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectConfigurationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public class SdiProjectManagementConfigurationService : ProjectManagementConfigurationService
    {
        public override ProjectManagementType GetProjectManagementType()
        {
            return ProjectManagementType.SingleDocument;
        }
    }
}