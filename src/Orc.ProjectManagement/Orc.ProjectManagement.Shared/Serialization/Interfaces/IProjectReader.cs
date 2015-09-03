// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectReaderService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
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