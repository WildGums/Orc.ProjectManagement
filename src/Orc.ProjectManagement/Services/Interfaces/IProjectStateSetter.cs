namespace Orc.ProjectManagement
{
    internal interface IProjectStateSetter
    {
        void SetProjectLoading(string location, bool value);
        void SetProjectSaving(string location, bool value);
        void SetProjectClosing(string location, bool value);
        void SetProjectActivating(string? location, bool value);
        void SetProjectDeactivating(string? location, bool value);
        void SetProjectRefreshing(string location, bool value, bool isActiveProject = true);
    }
}
