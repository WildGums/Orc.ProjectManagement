// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectManager.cs" company="Simulation Modelling Services">
//   Copyright (c) 2008 - 2014 Simulation Modelling Services. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using System.Threading.Tasks;

    public interface IProjectManager
    {
        IProject Project { get; }
        string Location { get; }

        event EventHandler<ProjectEventArgs> ProjectLoading;
        event EventHandler<ProjectEventArgs> ProjectLoaded;

        event EventHandler<ProjectEventArgs> ProjectSaving;
        event EventHandler<ProjectEventArgs> ProjectSaved;

        event EventHandler<ProjectUpdatedEventArgs> ProjectUpdated;

        event EventHandler<ProjectEventArgs> ProjectClosing;
        event EventHandler<ProjectEventArgs> ProjectClosed;

        Task Initialize();
        Task Refresh();
        Task Load(string location);
        Task Save(string location = null);
        void Close();
    }
}