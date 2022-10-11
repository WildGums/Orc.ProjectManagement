namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;

    public interface IProjectWriter
    {
        Task<bool> WriteAsync(IProject project, string location);
    }
}
