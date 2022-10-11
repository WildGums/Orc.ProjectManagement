namespace Orc.ProjectManagement
{
    using Catel;

    public class InvalidProjectException : ProjectException
    {
        public InvalidProjectException(IProject project)
            : base(project, string.Format("Project '{0}' is invalid at this stage", ObjectToStringHelper.ToString(project)))
        {
        }
    }
}
