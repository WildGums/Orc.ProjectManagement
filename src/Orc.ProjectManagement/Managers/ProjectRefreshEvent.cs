namespace Orc.ProjectManagement;

using System;

public class ProjectRefreshEvent : IProjectEventType
{
    public ProjectRefreshEvent(ProjectEventTypeStage stage, EventArgs eventArgs)
    {
        Stage = stage;
        EventArgs = eventArgs;
    }

    public ProjectEventTypeStage Stage { get; }
    public EventArgs EventArgs { get; }
}
