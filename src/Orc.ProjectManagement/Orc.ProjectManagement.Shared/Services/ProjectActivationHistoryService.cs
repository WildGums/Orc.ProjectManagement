// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectActivationHistoryService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;

    internal class ProjectActivationHistoryService : IProjectActivationHistoryService
    {
        #region Fields
        private readonly IList<IProject> _activationHistory = new List<IProject>();
        private IEnumerable<IProject> _projectsSoutce = Enumerable.Empty<IProject>();
        private readonly HashSet<string> _uniqueLocations = new HashSet<string>();
        #endregion

        #region Methods
        public void Remember(IProject project)
        {
            if (project == null)
            {
                return;
            }

            RemoveFromHistory(project);

            _activationHistory.Insert(0, project);
            _uniqueLocations.Add(project.Location);

            AppendHistory();
        }

        public void Forget(IProject project)
        {
            if (project == null)
            {
                return;
            }

            RemoveFromHistory(project);

            AppendHistory();
        }

        public IProject GetLastActiveProject()
        {
            return _activationHistory.FirstOrDefault();
        }

        public IEnumerable<IProject> GetActivationHistory()
        {
            AppendHistory();

            return _activationHistory;
        }

        public void SetProjectsSource(IEnumerable<IProject> projects)
        {
            Argument.IsNotNull(() => projects);

            _projectsSoutce = projects;

            AppendHistory();
        }
        #endregion

        private void RemoveFromHistory(IProject project)
        {
            Argument.IsNotNull(() => project);

            while (_activationHistory.Remove(project))
            {
            }

            _uniqueLocations.Remove(project.Location);
        }

        private void AppendHistory()
        {
            foreach (var project in _projectsSoutce)
            {
                if (_uniqueLocations.Add(project.Location))
                {
                    _activationHistory.Add(project);
                }
            }
        }
    }
}