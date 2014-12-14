// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryProjectReader.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Test.Mocks
{
    using System.Threading.Tasks;

    public class MemoryProjectReader : ProjectReaderBase
    {
        protected override async Task<IProject> ReadFromLocation(string location)
        {
            return new Project(location);
        }
    }
}