namespace Orc.ProjectManagement;

using Catel;
using Catel.Data;
using System;

public abstract class ProjectBase : ModelBase, IProject
{
    protected ProjectBase(string location)
        : this(location, location) { }

    protected ProjectBase(string location, string title)
    {
        Location = location;
        Title = title;

        Id = UniqueIdentifierHelper.GetUniqueIdentifier(GetType());
        CreatedOn = DateTime.Now;
    }

    protected override bool ShouldPropertyChangeUpdateIsDirty(string propertyName)
    {
        return false;
    }

    public int Id { get; private set; }

    /// <summary>
    /// Gets or sets the date this project is created on. By default this is the creation date/time of
    /// the project instance (so it will be renewed every time the project has been read from 
    /// the source). This property can be overridden to implement a custom creation date of the project.
    /// </summary>
    /// <value>
    /// The date/time this project has been created on.
    /// </value>
    public virtual DateTime CreatedOn { get; protected set; }

    public virtual string Location { get; set; }

    public virtual string Title { get; private set; }

    public virtual void ClearIsDirty()
    {
        IsDirty = false;
    }

    public void MarkAsDirty()
    {
        IsDirty = true;
    }

    public override string ToString()
    {
        return Title;
    }
}