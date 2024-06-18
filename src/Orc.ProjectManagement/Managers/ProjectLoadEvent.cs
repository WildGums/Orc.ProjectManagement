namespace Orc.ProjectManagement;

using System;

public class ProjectLoadEvent : IProjectEventType
{
    public ProjectLoadEvent(ProjectEventTypeStage stage, EventArgs eventArgs)
    {
        Stage = stage;
        EventArgs = eventArgs;
    }

    public ProjectEventTypeStage Stage { get; }
    public EventArgs EventArgs { get; }
}
