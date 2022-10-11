namespace Orc.ProjectManagement
{
    using System.Collections.Generic;

    public interface IProjectActivationHistoryService
    {
        void Remember(IProject project);
        void Forget(IProject project);
        IProject[] GetActivationHistory();
        IProject? GetLastActiveProject();
        void SetProjectsSource(IEnumerable<IProject> projects);
        void SuspendUsingHistory();
        void ContinueUsingHistory();
    }
}
