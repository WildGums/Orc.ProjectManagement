// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileExistsProjectValidator.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
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