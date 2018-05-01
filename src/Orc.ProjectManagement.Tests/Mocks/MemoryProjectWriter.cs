// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MemoryProjectWriter.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Test.Mocks
{
    using System.Threading.Tasks;
    using Catel.Threading;

    public class MemoryProjectWriter : ProjectWriterBase<Project>
    {
        protected override Task<bool> WriteToLocationAsync(Project project, string location)
        {
            // no implementation required
            return TaskHelper<bool>.FromResult(true);
        }
    }
}