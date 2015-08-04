// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryProjectWriter.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Test.Mocks
{
    using System.Threading.Tasks;

    public class MemoryProjectWriter : ProjectWriterBase<Project>
    {
        protected override async Task WriteToLocationAsync(Project project, string location)
        {
            // no implementation required
        }
    }
}