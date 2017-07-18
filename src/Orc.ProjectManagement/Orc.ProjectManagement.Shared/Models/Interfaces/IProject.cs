// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProject.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public interface IProject
    {
        int Id { get; }

        string Location { get; set; }

        string Title { get; }

        void ClearIsDirty();

        void MarkAsDirty();
    }
}