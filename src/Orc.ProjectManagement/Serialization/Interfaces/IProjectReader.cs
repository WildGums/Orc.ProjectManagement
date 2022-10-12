namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;

    public interface IProjectReader
    {
        Task<IProject?> ReadAsync(string location);
    }
}
