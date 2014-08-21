// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Project.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using Catel;
    using Catel.Data;

    public abstract class ProjectBase : ModelBase, IProject
    {
        public ProjectBase(string location)
            : this(location, location) { }

        protected ProjectBase(string location, string title)
        {
            Location = location;
            Title = title;

            Id = UniqueIdentifierHelper.GetUniqueIdentifier(GetType());
        }

        public int Id { get; private set; }

        public virtual string Location { get; set; }

        public virtual string Title { get; private set; }

        public virtual void ClearIsDirty()
        {
            IsDirty = false;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}