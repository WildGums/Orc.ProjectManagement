// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagementInitializationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement
{
    using System;
    using Catel;
    using Catel.IoC;
    using Catel.Logging;

    public class ProjectManagementInitializationService : IProjectManagementInitializationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IProjectManagementConfigurationService _projectManagementConfigurationService;

        public ProjectManagementInitializationService(IServiceLocator serviceLocator, IProjectManagementConfigurationService projectManagementConfigurationService)
        {
            Argument.IsNotNull(() => serviceLocator);
            Argument.IsNotNull(() => projectManagementConfigurationService);

            ServiceLocator = serviceLocator;
            _projectManagementConfigurationService = projectManagementConfigurationService;
        }

        protected IServiceLocator ServiceLocator { get; private set; }

        public virtual void Initialize(IProjectManager projectManager)
        {
            Argument.IsNotNull(() => projectManager);

            var projectManagementType = _projectManagementConfigurationService.GetProjectManagementType();

            Log.Debug("Initializing project management for '{0}'", projectManagementType);

            Initialize(projectManager, projectManagementType);
        }

        protected virtual void Initialize(IProjectManager projectManager, ProjectManagementType projectManagementType)
        {
            var serviceLocator = ServiceLocator;

            switch (projectManagementType)
            {
                case ProjectManagementType.SingleDocument:
                    // Note: don't register and instantiate because IProjectManager is not yet registered here
                    var closeBeforeLoadProjectWatcher = new CloseBeforeLoadProjectWatcher(projectManager);
                    serviceLocator.RegisterInstance(closeBeforeLoadProjectWatcher);
                    break;

                case ProjectManagementType.MultipleDocuments:
                    // Note: don't register and instantiate because IProjectManager is not yet registered here
                    var projectActivationHistoryService = new ProjectActivationHistoryService(projectManager);
                    serviceLocator.RegisterInstance(typeof(IProjectActivationHistoryService), projectActivationHistoryService);

                    var activationHistoryProjectWatcher = new ActivationHistoryProjectWatcher(projectManager, projectActivationHistoryService);
                    serviceLocator.RegisterInstance(activationHistoryProjectWatcher);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("projectManagementType", projectManagementType, null);
            }
        }
    }
}