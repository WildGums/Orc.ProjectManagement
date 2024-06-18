namespace Orc.ProjectManagement;

using System;

public class ProjectActivationEvent : IProjectEventType
{
    public ProjectActivationEvent(ProjectEventTypeStage stage, EventArgs eventArgs)
    {
        Stage = stage;
        EventArgs = eventArgs;
    }

    public ProjectEventTypeStage Stage { get; }
    public EventArgs EventArgs { get; }
}
