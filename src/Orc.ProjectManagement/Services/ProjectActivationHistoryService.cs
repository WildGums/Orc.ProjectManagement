// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectActivationHistoryService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;

    public class ProjectActivationHistoryService : IProjectActivationHistoryService
    {
        #region Fields
        private readonly IList<IProject> _activationHistory = new List<IProject>();
        private IEnumerable<IProject> _projectsSource = Enumerable.Empty<IProject>();
        private readonly HashSet<string> _uniqueLocations = new HashSet<string>();
        #endregion

        #region Methods
        public void Remember(IProject project)
        {
            if (project is null || _isHistoryUsageSuspended)
            {
                return;
            }

            RemoveFromHistory(project);

            _activationHistory.Insert(0, project);
            _uniqueLocations.Add(project.Location);

            RenewHistory();
        }

        public void Forget(IProject project)
        {
            if (project is null)
            {
                return;
            }

            RemoveFromHistory(project);

            RenewHistory();
        }

        public IProject GetLastActiveProject()
        {
            if (_isHistoryUsageSuspended)
            {
                return null;
            }

            return _activationHistory.FirstOrDefault();
        }

        public IEnumerable<IProject> GetActivationHistory()
        {
            RenewHistory();

            return _activationHistory;
        }

        public void SetProjectsSource(IEnumerable<IProject> projects)
        {
            Argument.IsNotNull(() => projects);

            _projectsSource = projects;

            RenewHistory();
        }

        private bool _isHistoryUsageSuspended;

        public void SuspendUsingHistory()
        {
            _isHistoryUsageSuspended = true;
        }

        public void ContinueUsingHistory()
        {
            RenewHistory();

            _isHistoryUsageSuspended = false;
        }
        #endregion

        private void RemoveFromHistory(IProject project)
        {
            if (project is null)
            {
                return;
            }

            while (_activationHistory.Remove(project))
            {
                // Continue
            }

            _uniqueLocations.Remove(project.Location);
        }

        private void RenewHistory()
        {
            var source = _projectsSource.ToList();
            var history = _activationHistory.ToList();
            var toRemove = (from project in history
                            where !source.Contains(project)
                            select project).ToList();

            foreach (var project in toRemove)
            {
                RemoveFromHistory(project);
            }
            
            foreach (var project in _projectsSource)
            {
                if (_uniqueLocations.Add(project.Location))
                {
                    _activationHistory.Add(project);
                }
            }
        }
    }
}
