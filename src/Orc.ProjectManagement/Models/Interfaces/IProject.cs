// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProject.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
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
    }
}