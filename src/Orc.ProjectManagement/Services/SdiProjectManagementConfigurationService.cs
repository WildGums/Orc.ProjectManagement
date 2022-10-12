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
