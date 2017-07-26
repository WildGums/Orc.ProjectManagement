// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryExistsProjectValidator.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.IO;
    using System.Threading.Tasks;

    public class DirectoryExistsProjectValidator : ProjectValidatorBase
    {
        #region IProjectValidator Members
        public override async Task<bool> CanStartLoadingProjectAsync(string location)
        {
            return Directory.Exists(location);
        }
        #endregion
    }
}