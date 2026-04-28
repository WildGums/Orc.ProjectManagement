namespace Orc.ProjectManagement;

using System;
using Catel.IoC;
using Catel.Logging;
using Microsoft.Extensions.Logging;

public class ProjectManagementInitializationService : IProjectManagementInitializationService
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ProjectManagementInitializationService));

    protected readonly IServiceProvider _serviceProvider;
    private readonly IProjectManagementConfigurationService _projectManagementConfigurationService;

    public ProjectManagementInitializationService(IServiceProvider serviceProvider, 
        IProjectManagementConfigurationService projectManagementConfigurationService)
    {
        _serviceProvider = serviceProvider;
        _projectManagementConfigurationService = projectManagementConfigurationService;
    }

    public virtual void Initialize(IProjectManager projectManager)
    {
        ArgumentNullException.ThrowIfNull(projectManager);

        var projectManagementType = _projectManagementConfigurationService.GetProjectManagementType();

        Logger.LogDebug("Initializing project management for '{0}'", projectManagementType);

        Initialize(projectManager, projectManagementType);
    }

    protected virtual void Initialize(IProjectManager projectManager, ProjectManagementType projectManagementType)
    {
        throw new NotImplementedException("Must be implemented if used");

        //switch (projectManagementType)
        //{
        //    case ProjectManagementType.SingleDocument:
        //        // Note: don't register and instantiate because IProjectManager is not yet registered here
        //        var closeBeforeLoadProjectWatcher = _typeFactory.CreateRequiredInstanceWithParametersAndAutoCompletion<CloseBeforeLoadProjectWatcher>(projectManager);
        //        ServiceLocator.RegisterInstance(closeBeforeLoadProjectWatcher);
        //        break;

        //    case ProjectManagementType.MultipleDocuments:
        //        // Note: don't register and instantiate because IProjectManager is not yet registered here
        //        var activationHistoryProjectWatcher = _typeFactory.CreateRequiredInstanceWithParametersAndAutoCompletion<ActivationHistoryProjectWatcher>(projectManager);
        //        ServiceLocator.RegisterInstance(activationHistoryProjectWatcher);
        //        break;

        //    default:
        //        throw Logger.LogErrorAndCreateException(_ => new ArgumentOutOfRangeException(nameof(projectManagementType), projectManagementType, null), string.Empty);
        //}
    }
}
