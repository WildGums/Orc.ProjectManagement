using Catel.IoC;
using Orc.ProjectManagement;
using Orc.ProjectManagement.Serialization;
using Orc.ProjectManagement.Services;

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

        serviceLocator.RegisterType<ICommandLineService, CommandLineService>();
        serviceLocator.RegisterType<IProjectManager, ProjectManager>();
        serviceLocator.RegisterType<IProjectInitializer, EmptyProjectInitializer>();
        serviceLocator.RegisterType<IProjectValidator, EmptyProjectValidator>();
        serviceLocator.RegisterType<IProjectRefresherSelector, DefaultProjectRefresherSelector>();
        serviceLocator.RegisterType<IProjectSerializerSelector, DefaultProjectSerializerSelector>(registerIfAlreadyRegistered:false);
    }
}