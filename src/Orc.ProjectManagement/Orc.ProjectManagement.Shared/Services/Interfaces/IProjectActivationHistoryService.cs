// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectActivationHistoryService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Collections.Generic;

    public interface IProjectActivationHistoryService
    {
        void Remember(IProject project);
        void Forget(IProject project);
        IEnumerable<IProject> GetActivationHistory();
        IProject GetLastActiveProject();
        void SetProjectsSource(IEnumerable<IProject> projects);
        void SuspendUsingHistory();
        void ContinueUsingHistory();
    }
}