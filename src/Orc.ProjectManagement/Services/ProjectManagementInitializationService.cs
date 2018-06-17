// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagementInitializationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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
        private readonly ITypeFactory _typeFactory;

        public ProjectManagementInitializationService(IServiceLocator serviceLocator, IProjectManagementConfigurationService projectManagementConfigurationService,
            ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => serviceLocator);
            Argument.IsNotNull(() => projectManagementConfigurationService);
            Argument.IsNotNull(() => typeFactory);

            ServiceLocator = serviceLocator;
            _projectManagementConfigurationService = projectManagementConfigurationService;
            _typeFactory = typeFactory;
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
                    var closeBeforeLoadProjectWatcher = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<CloseBeforeLoadProjectWatcher>(projectManager);
                    serviceLocator.RegisterInstance(closeBeforeLoadProjectWatcher);
                    break;

                case ProjectManagementType.MultipleDocuments:
                    // Note: don't register and instantiate because IProjectManager is not yet registered here
                    var activationHistoryProjectWatcher = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<ActivationHistoryProjectWatcher>(projectManager);
                    serviceLocator.RegisterInstance(activationHistoryProjectWatcher);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(projectManagementType), projectManagementType, null);
            }
        }
    }
}
