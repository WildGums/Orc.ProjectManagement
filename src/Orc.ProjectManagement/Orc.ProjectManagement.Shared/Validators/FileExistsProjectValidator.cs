// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileExistsProjectValidator.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.IO;

    public class FileExistsProjectValidator : ProjectValidatorBase
    {
        #region IProjectValidator Members
        public override bool CanStartLoadingProject(string location)
        {
            return File.Exists(location);
        }
        #endregion
    }
}