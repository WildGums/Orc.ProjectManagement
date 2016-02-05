// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyProjectValidator.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public class EmptyProjectValidator : ProjectValidatorBase
    {
        public override bool CanStartLoadingProject(string location)
        {
            return true;
        }
    }
}