using Catel.IoC;
using Orc.ProjectManagement;
using Orc.ProjectManagement.Example.ProjectManagement;
using Orc.ProjectManagement.Example.Services;

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

        serviceLocator.RegisterType<IProjectReader, PersonProjectReader>();
        serviceLocator.RegisterType<IProjectWriter, PersonProjectWriter>();
        serviceLocator.RegisterType<IProjectInitializer, PersonProjectInitializer>();
        serviceLocator.RegisterType<IProjectRefresher, FileProjectRefresher>(RegistrationType.Transient);

        serviceLocator.RegisterTypeAndInstantiate<RefreshProjectWatcher>();
    }
}