// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileExistsProjectValidator.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.IO;
    using System.Threading.Tasks;
    using Catel.Threading;

    public class FileExistsProjectValidator : ProjectValidatorBase
    {
        #region IProjectValidator Members
        public override Task<bool> CanStartLoadingProjectAsync(string location)
        {
            var canStart = File.Exists(location);
            return TaskHelper<bool>.FromResult(canStart);
        }
        #endregion
    }
}