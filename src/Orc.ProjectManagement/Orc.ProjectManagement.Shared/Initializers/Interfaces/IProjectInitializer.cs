// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectInitializer.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    [ObsoleteEx(RemoveInVersion = "1.1.0", TreatAsErrorFromVersion = "1.0.0")]
    public interface IProjectInitializer
    {        
        string GetInitialLocation();
    }
}