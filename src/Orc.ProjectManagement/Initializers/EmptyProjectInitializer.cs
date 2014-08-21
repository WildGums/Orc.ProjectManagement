// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public class EmptyProjectInitializer : IProjectInitializer
    {
        public virtual string GetInitialLocation()
        {
            return null;
        }
    }
}