namespace Orc.ProjectManagement;

using System;
using Catel.Data;

public class ProjectErrorEventArgs : ProjectLocationEventArgs
{
    public ProjectErrorEventArgs(string? location, Exception? exception = null, IValidationContext? validationContext = null)
        : base(location)
    {
        Exception = exception;
        ValidationContext = validationContext ?? new ValidationContext();
    }

    public ProjectErrorEventArgs(IProject? project, Exception? exception = null, IValidationContext? validationContext = null)
        : base(project?.Location)
    {
        Project = project;
        Exception = exception;
        ValidationContext = validationContext ?? new ValidationContext();
    }

    public IProject? Project { get; set; }
    public Exception? Exception { get; }
    public IValidationContext ValidationContext { get; }
}
