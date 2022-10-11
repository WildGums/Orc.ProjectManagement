namespace Orc.ProjectManagement
{
    using System;

    public interface IProjectRefresher
    {
        string Location { get; }
        bool IsSubscribed { get; }
        bool IsEnabled { get; set; }

        event EventHandler<ProjectEventArgs>? Updated;

        void Subscribe();
        void Unsubscribe();
    }
}
