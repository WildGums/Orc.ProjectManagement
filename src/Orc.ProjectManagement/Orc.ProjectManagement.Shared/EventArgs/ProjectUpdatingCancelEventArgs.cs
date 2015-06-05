// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectUpdatingCancelEventArgs.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.ComponentModel;
    using Catel;

    public class ProjectUpdatingCancelEventArgs : CancelEventArgs
    {
        private readonly string _oldProjectLocation;
        private readonly string _newProjectLocation;

        public ProjectUpdatingCancelEventArgs(IProject oldProject, IProject newProject, bool cancel = false)
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

        public IProject OldProject { get; private set; }

        public string OldProjectLocation
        {
            get { return OldProject == null ? _oldProjectLocation : OldProject.Location; }
        }

        public IProject NewProject { get; private set; }

        public string NewProjectLocation
        {
            get { return NewProject == null ? _newProjectLocation : NewProject.Location; }
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
}