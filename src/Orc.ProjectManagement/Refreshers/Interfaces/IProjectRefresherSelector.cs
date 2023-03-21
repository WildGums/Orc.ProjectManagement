namespace Orc.ProjectManagement;

public interface IProjectRefresherSelector
{
    IProjectRefresher? GetProjectRefresher(string location);
}