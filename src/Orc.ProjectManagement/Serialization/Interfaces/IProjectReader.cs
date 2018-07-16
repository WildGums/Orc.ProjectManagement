// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectReaderService.cs" company="WildGums">
//   Copyright (c) 2008 - 2014 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Threading.Tasks;

    public interface IProjectReader
    {
        Task<IProject> ReadAsync(string location);
    }
}