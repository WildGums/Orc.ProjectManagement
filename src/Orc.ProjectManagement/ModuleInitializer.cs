using Catel.IoC;
using Catel.Services;
using Orc.ProjectManagement;
using Orc.ProjectManagement.Serialization;

/// <summary>
/// Used by the ModuleInit. All code inside the Initialize method is ran as soon as the assembly is loaded.
/// </summary>
public static class ModuleInitializer
{
    /// <summary>
    /// Initializes the module.
    /// </summary>
    public static void Initialize()
    {
        var serviceLocator = ServiceLocator.Default;

        serviceLocator.RegisterType<IProjectManager, ProjectManager>();
        serviceLocator.RegisterType<IProjectStateService, ProjectStateService>();
        serviceLocator.RegisterType<IProjectInitializer, EmptyProjectInitializer>();
        serviceLocator.RegisterType<IProjectValidator, EmptyProjectValidator>();
        serviceLocator.RegisterType<IProjectUpgrader, EmptyProjectUpgrader>();
        serviceLocator.RegisterType<IProjectRefresherSelector, DefaultProjectRefresherSelector>();
        serviceLocator.RegisterType<IProjectActivationHistoryService, ProjectActivationHistoryService>();
        serviceLocator.RegisterType<IInitialProjectLocationService, InitialProjectLocationService>();

        serviceLocator.RegisterTypeIfNotYetRegistered<IProjectSerializerSelector, DefaultProjectSerializerSelector>();
        serviceLocator.RegisterTypeIfNotYetRegistered<IProjectManagementConfigurationService, SdiProjectManagementConfigurationService>();
        serviceLocator.RegisterTypeIfNotYetRegistered<IProjectManagementInitializationService, ProjectManagementInitializationService>();

        var languageService = serviceLocator.ResolveType<ILanguageService>();
        languageService.RegisterLanguageSource(new LanguageResourceSource("Orc.ProjectManagement", "Orc.ProjectManagement.Properties", "Resources"));
    }
}