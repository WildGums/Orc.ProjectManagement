// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectValidator.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Test.Mocks
{
    using System;
    using System.Threading.Tasks;

    public class ProjectValidator : IProjectValidator
    {
        #region IProjectValidator Members
        public async Task<bool> CanStartLoadingProject(string location)
        {
            return string.Equals(location, "cannotload", StringComparison.InvariantCultureIgnoreCase);
        }
        #endregion
    }
}