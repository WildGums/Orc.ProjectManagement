namespace Orc.ProjectManagement;

using System.ComponentModel;
using Catel;

public class ProjectUpdatingCancelEventArgs : CancelEventArgs
{
    private readonly string? _oldProjectLocation;
    private readonly string? _newProjectLocation;

    public ProjectUpdatingCancelEventArgs(IProject? oldProject, IProject? newProject, bool cancel = false)
        : base(cancel)
    {
        OldProject = oldProject;
        NewProject = newProject;
    }

    public ProjectUpdatingCancelEventArgs(string oldProjectLocation, string newProjectLocation, bool cancel = false)
        : base(cancel)
    {
        _oldProjectLocation = oldProjectLocation;
        _newProjectLocation = newProjectLocation;
    }

    public IProject? OldProject { get; }

    public string? OldProjectLocation
    {
        get { return OldProject?.Location ?? _oldProjectLocation; }
    }

    public IProject? NewProject { get; }

    public string? NewProjectLocation
    {
        get { return NewProject?.Location ?? _newProjectLocation; }
    }

    public bool IsRefresh
    {
        get
        {
            var oldProjectLocation = OldProjectLocation;
            var newProjectLocation = NewProjectLocation;

            if (string.IsNullOrWhiteSpace(oldProjectLocation) || string.IsNullOrWhiteSpace(newProjectLocation))
            {
                return false;
            }

            return ObjectHelper.AreEqual(oldProjectLocation, newProjectLocation);
        }
    }
}
