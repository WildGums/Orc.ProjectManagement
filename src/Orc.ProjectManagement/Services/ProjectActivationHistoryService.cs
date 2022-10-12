namespace Orc.ProjectManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ProjectActivationHistoryService : IProjectActivationHistoryService
    {
        private readonly IList<IProject> _activationHistory = new List<IProject>();
        private IEnumerable<IProject> _projectsSource = Enumerable.Empty<IProject>();
        private readonly HashSet<string> _uniqueLocations = new HashSet<string>();

        public void Remember(IProject project)
        {
            ArgumentNullException.ThrowIfNull(project);

            if (_isHistoryUsageSuspended)
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
            ArgumentNullException.ThrowIfNull(project);

            RemoveFromHistory(project);

            RenewHistory();
        }

        public IProject? GetLastActiveProject()
        {
            if (_isHistoryUsageSuspended)
            {
                return null;
            }

            return _activationHistory.FirstOrDefault();
        }

        public IProject[] GetActivationHistory()
        {
            RenewHistory();

            return _activationHistory.ToArray();
        }

        public void SetProjectsSource(IEnumerable<IProject> projects)
        {
            ArgumentNullException.ThrowIfNull(projects);

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

        private void RemoveFromHistory(IProject project)
        {
            ArgumentNullException.ThrowIfNull(project);
            
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
