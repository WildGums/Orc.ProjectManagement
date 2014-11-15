// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectValidator.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;

    public interface IProjectValidator
    {
        Task<bool> CanStartLoadingProject(string location);
    }
}