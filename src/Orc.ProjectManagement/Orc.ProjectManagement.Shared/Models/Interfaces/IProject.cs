// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProject.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using Catel.Data;

    public interface IProject : IModel
    {
        int Id { get; }

        string Location { get; set; }

        string Title { get; }

        void ClearIsDirty();

        void MarkAsDirty();
    }
}