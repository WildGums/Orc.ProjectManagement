namespace Orc.ProjectManagement;

using Catel;

public class InvalidProjectException : ProjectException
{
    public InvalidProjectException(IProject project)
        : base(project, $"Project '{ObjectToStringHelper.ToString(project)}' is invalid at this stage")
    {
    }
}
