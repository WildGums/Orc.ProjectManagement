namespace Orc.ProjectManagement;

using System;

public class ProjectSaveEvent : IProjectEventType
{
    public ProjectSaveEvent(ProjectEventTypeStage stage, EventArgs eventArgs)
    {
        Stage = stage;
        EventArgs = eventArgs;
    }

    public ProjectEventTypeStage Stage { get; }
    public EventArgs EventArgs { get; }
}
