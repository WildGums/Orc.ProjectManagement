// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectReaderService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public interface IProjectReader
    {
        IProject Read(string location);
    }
}