namespace Orc.ProjectManagement;

public abstract class ProjectManagementConfigurationService : IProjectManagementConfigurationService
{
    public abstract ProjectManagementType GetProjectManagementType();
}