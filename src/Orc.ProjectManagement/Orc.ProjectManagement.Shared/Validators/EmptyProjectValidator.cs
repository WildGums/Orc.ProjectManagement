// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyProjectValidator.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
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