// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyProjectValidator.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;
    using Catel.Threading;

    public class EmptyProjectValidator : ProjectValidatorBase
    {
        public override Task<bool> CanStartLoadingProjectAsync(string location)
        {
            return TaskHelper<bool>.FromResult(true);
        }
    }
}