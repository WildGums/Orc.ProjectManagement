namespace Orc.ProjectManagement
{
    using System;

    public interface IProject
    {
        int Id { get; }

        string Location { get; set; }

        string Title { get; }

        bool IsDirty { get; }

        /// <summary>
        /// Gets or sets the date this project is created on. By default this is the creation date/time of
        /// the project instance (so it will be renewed every time the project has been read from 
        /// the source). This property can be overridden to implement a custom creation date of the project.
        /// </summary>
        /// <value>
        /// The date/time this project has been created on.
        /// </value>
        DateTime CreatedOn { get; }

        void ClearIsDirty();

        void MarkAsDirty();
    }
}
