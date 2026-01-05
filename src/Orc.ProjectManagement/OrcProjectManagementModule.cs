namespace Orc.ProjectManagement
{
    using Catel.Services;
    using Catel.ThirdPartyNotices;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Orc.ProjectManagement.Serialization;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class OrcProjectManagementModule
    {
        public static IServiceCollection AddOrcProjectManagement(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IProjectManager, ProjectManager>();
            serviceCollection.TryAddSingleton<IProjectStateService, ProjectStateService>();
            serviceCollection.TryAddSingleton<IProjectInitializer, EmptyProjectInitializer>();
            serviceCollection.TryAddSingleton<IProjectValidator, EmptyProjectValidator>();
            serviceCollection.TryAddSingleton<IProjectUpgrader, EmptyProjectUpgrader>();
            serviceCollection.TryAddSingleton<IProjectRefresherSelector, DefaultProjectRefresherSelector>();
            serviceCollection.TryAddSingleton<IProjectActivationHistoryService, ProjectActivationHistoryService>();
            serviceCollection.TryAddSingleton<IInitialProjectLocationService, InitialProjectLocationService>();

            serviceCollection.TryAddSingleton<IProjectSerializerSelector, DefaultProjectSerializerSelector>();
            serviceCollection.TryAddSingleton<IProjectManagementConfigurationService, SdiProjectManagementConfigurationService>();
            serviceCollection.TryAddSingleton<IProjectManagementInitializationService, ProjectManagementInitializationService>();


            serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Orc.ProjectManagement", "Orc.ProjectManagement.Properties", "Resources"));

            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new LibraryThirdPartyNotice("Orc.ProjectManagement", "https://github.com/wildgums/orc.projectmanagement"));

            return serviceCollection;
        }
    }
}
