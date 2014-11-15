// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyProjectValidator.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;

    public class EmptyProjectValidator : IProjectValidator
    {
        public async Task<bool> CanStartLoadingProject(string location)
        {
            return true;
        }
    }
}