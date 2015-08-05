// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectWriterService.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    public interface IProjectWriter
    {
        bool Write(IProject project, string location);
    }
}