namespace Orc.ProjectManagement;

using System;

public class ProjectCloseEvent : IProjectEventType
{
    public ProjectCloseEvent(ProjectEventTypeStage stage, EventArgs eventArgs)
    {
        Stage = stage;
        EventArgs = eventArgs;
    }

    public ProjectEventTypeStage Stage { get; }
    public EventArgs EventArgs { get; }
}
