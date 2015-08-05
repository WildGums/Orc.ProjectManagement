// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryProjectWriter.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Test.Mocks
{
    public class MemoryProjectWriter : ProjectWriterBase<Project>
    {
        protected override bool WriteToLocation(Project project, string location)
        {
            // no implementation required
            return true;
        }
    }
}