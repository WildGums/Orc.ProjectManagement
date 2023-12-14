namespace Orc.ProjectManagement;

public class MdiProjectManagementConfigurationService : ProjectManagementConfigurationService
{
    public override ProjectManagementType GetProjectManagementType()
    {
        return ProjectManagementType.MultipleDocuments;
    }
}