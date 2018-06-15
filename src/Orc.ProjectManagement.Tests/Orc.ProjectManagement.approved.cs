[assembly: System.Resources.NeutralResourcesLanguageAttribute("en-US")]
[assembly: System.Runtime.Versioning.TargetFrameworkAttribute(".NETFramework,Version=v4.6", FrameworkDisplayName=".NET Framework 4.6")]
public class static ModuleInitializer
{
    public static void Initialize() { }
}
namespace Orc.ProjectManagement
{
    public class CloseBeforeLoadProjectWatcher : Orc.ProjectManagement.ProjectWatcherBase
    {
        public CloseBeforeLoadProjectWatcher(Orc.ProjectManagement.IProjectManager projectManager) { }
        protected override System.Threading.Tasks.Task OnLoadingAsync(Orc.ProjectManagement.ProjectCancelEventArgs e) { }
    }
    public class DefaultProjectRefresherSelector : Orc.ProjectManagement.IProjectRefresherSelector
    {
        public DefaultProjectRefresherSelector(Catel.IoC.IServiceLocator serviceLocator, Catel.IoC.ITypeFactory typeFactory) { }
        public Orc.ProjectManagement.IProjectRefresher GetProjectRefresher(string location) { }
    }
    public class DirectoryExistsProjectValidator : Orc.ProjectManagement.ProjectValidatorBase
    {
        public DirectoryExistsProjectValidator() { }
        public override System.Threading.Tasks.Task<bool> CanStartLoadingProjectAsync(string location) { }
    }
    public class DirectoryProjectInitializer : Orc.ProjectManagement.IProjectInitializer
    {
        public DirectoryProjectInitializer(Catel.Configuration.IConfigurationService configurationService, Orc.ProjectManagement.IInitialProjectLocationService initialProjectLocationService) { }
        public virtual System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<string>> GetInitialLocationsAsync() { }
    }
    public class DirectoryProjectRefresher : Orc.ProjectManagement.ProjectRefresherBase
    {
        public DirectoryProjectRefresher(string projectLocation, string directoryToWatch) { }
        public DirectoryProjectRefresher(string projectLocation, string directoryToWatch, string fileFilter) { }
        public string FileFilter { get; }
        protected override void SubscribeToLocation(string location) { }
        protected override void UnsubscribeFromLocation(string location) { }
    }
    public class EmptyProjectInitializer : Orc.ProjectManagement.IProjectInitializer
    {
        public EmptyProjectInitializer() { }
        public virtual System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<string>> GetInitialLocationsAsync() { }
    }
    public class EmptyProjectUpgrader : Orc.ProjectManagement.IProjectUpgrader
    {
        public EmptyProjectUpgrader() { }
        public virtual System.Threading.Tasks.Task<bool> RequiresUpgradeAsync(string location) { }
        public virtual System.Threading.Tasks.Task<string> UpgradeAsync(string location) { }
    }
    public class EmptyProjectValidator : Orc.ProjectManagement.ProjectValidatorBase
    {
        public EmptyProjectValidator() { }
        public override System.Threading.Tasks.Task<bool> CanStartLoadingProjectAsync(string location) { }
    }
    public class FileExistsProjectValidator : Orc.ProjectManagement.ProjectValidatorBase
    {
        public FileExistsProjectValidator() { }
        public override System.Threading.Tasks.Task<bool> CanStartLoadingProjectAsync(string location) { }
    }
    public class FileProjectInitializer : Orc.ProjectManagement.IProjectInitializer
    {
        public FileProjectInitializer(Orc.ProjectManagement.IInitialProjectLocationService initialProjectLocationService) { }
        public virtual System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<string>> GetInitialLocationsAsync() { }
    }
    public class FileProjectRefresher : Orc.ProjectManagement.DirectoryProjectRefresher
    {
        public FileProjectRefresher(string projectLocation) { }
    }
    public interface IInitialProjectLocationService
    {
        System.Threading.Tasks.Task<string> GetInitialProjectLocationAsync();
    }
    public class InitialProjectLocationService : Orc.ProjectManagement.IInitialProjectLocationService
    {
        public InitialProjectLocationService() { }
        public virtual System.Threading.Tasks.Task<string> GetInitialProjectLocationAsync() { }
    }
    public class InvalidProjectException : Orc.ProjectManagement.ProjectException
    {
        public InvalidProjectException(Orc.ProjectManagement.IProject project) { }
    }
    public interface IProject
    {
        System.DateTime CreatedOn { get; }
        int Id { get; }
        bool IsDirty { get; }
        string Location { get; set; }
        string Title { get; }
        void ClearIsDirty();
        void MarkAsDirty();
    }
    public interface IProjectActivationHistoryService
    {
        void ContinueUsingHistory();
        void Forget(Orc.ProjectManagement.IProject project);
        System.Collections.Generic.IEnumerable<Orc.ProjectManagement.IProject> GetActivationHistory();
        Orc.ProjectManagement.IProject GetLastActiveProject();
        void Remember(Orc.ProjectManagement.IProject project);
        void SetProjectsSource(System.Collections.Generic.IEnumerable<Orc.ProjectManagement.IProject> projects);
        void SuspendUsingHistory();
    }
    public interface IProjectInitializer
    {
        System.Threading.Tasks.Task<System.Collections.Generic.IEnumerable<string>> GetInitialLocationsAsync();
    }
    public interface IProjectManagementConfigurationService
    {
        Orc.ProjectManagement.ProjectManagementType GetProjectManagementType();
    }
    public interface IProjectManagementInitializationService
    {
        void Initialize(Orc.ProjectManagement.IProjectManager projectManager);
    }
    public interface IProjectManager
    {
        Orc.ProjectManagement.IProject ActiveProject { get; }
        bool IsLoading { get; }
        Orc.ProjectManagement.ProjectManagementType ProjectManagementType { get; }
        System.Collections.Generic.IEnumerable<Orc.ProjectManagement.IProject> Projects { get; }
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectUpdatedEventArgs> ProjectActivatedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectUpdatingCancelEventArgs> ProjectActivationAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectActivationCanceledAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectErrorEventArgs> ProjectActivationFailedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectClosedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectCancelEventArgs> ProjectClosingAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectClosingCanceledAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectLoadedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectCancelEventArgs> ProjectLoadingAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectLoadingCanceledAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectErrorEventArgs> ProjectLoadingFailedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectRefreshedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectCancelEventArgs> ProjectRefreshingAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectRefreshingCanceledAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectErrorEventArgs> ProjectRefreshingFailedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectRefreshRequiredAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectSavedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectCancelEventArgs> ProjectSavingAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectSavingCanceledAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectErrorEventArgs> ProjectSavingFailedAsync;
        System.Threading.Tasks.Task<bool> CloseAsync();
        System.Threading.Tasks.Task<bool> CloseAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task InitializeAsync();
        System.Threading.Tasks.Task<bool> LoadAsync(string location);
        System.Threading.Tasks.Task<bool> LoadInactiveAsync(string location);
        System.Threading.Tasks.Task<bool> RefreshAsync();
        System.Threading.Tasks.Task<bool> RefreshAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task<bool> SaveAsync(string location = null);
        System.Threading.Tasks.Task<bool> SaveAsync(Orc.ProjectManagement.IProject project, string location = null);
        System.Threading.Tasks.Task<bool> SetActiveProjectAsync(Orc.ProjectManagement.IProject project);
    }
    public class static IProjectManagerExtensions
    {
        public static TProject GetActiveProject<TProject>(this Orc.ProjectManagement.IProjectManager projectManager)
            where TProject : Orc.ProjectManagement.IProject { }
        public static string GetActiveProjectLocation(this Orc.ProjectManagement.IProjectManager projectManager) { }
    }
    public interface IProjectManagerWorkflowItem
    {
        System.Threading.Tasks.Task ActivatedAsync(Orc.ProjectManagement.IProject oldProject, Orc.ProjectManagement.IProject newProject);
        System.Threading.Tasks.Task<bool> ActivationAsync(Orc.ProjectManagement.IProject oldProject, Orc.ProjectManagement.IProject newProject, bool isRefresh);
        System.Threading.Tasks.Task ActivationCanceledAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task ActivationFailedAsync(Orc.ProjectManagement.IProject project, System.Exception exception, Catel.Data.IValidationContext validationContext);
        System.Threading.Tasks.Task ClosedAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task<bool> ClosingAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task ClosingCanceledAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task LoadedAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task<bool> LoadingAsync(string location);
        System.Threading.Tasks.Task LoadingCanceledAsync(string location);
        System.Threading.Tasks.Task LoadingFailedAsync(string location, System.Exception exception, Catel.Data.IValidationContext validationContext);
        System.Threading.Tasks.Task RefreshedAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task<bool> RefreshingAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task RefreshingCanceledAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task RefreshingFailedAsync(Orc.ProjectManagement.IProject project, System.Exception exception, Catel.Data.IValidationContext validationContext);
        System.Threading.Tasks.Task SavedAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task<bool> SavingAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task SavingCanceledAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task SavingFailedAsync(Orc.ProjectManagement.IProject project, System.Exception exception, Catel.Data.IValidationContext validationContext);
    }
    public interface IProjectReader
    {
        System.Threading.Tasks.Task<Orc.ProjectManagement.IProject> ReadAsync(string location);
    }
    public interface IProjectRefresher
    {
        bool IsEnabled { get; set; }
        bool IsSubscribed { get; }
        string Location { get; }
        public event System.EventHandler<Orc.ProjectManagement.ProjectEventArgs> Updated;
        void Subscribe();
        void Unsubscribe();
    }
    public interface IProjectRefresherSelector
    {
        Orc.ProjectManagement.IProjectRefresher GetProjectRefresher(string location);
    }
    public interface IProjectSerializerSelector
    {
        Orc.ProjectManagement.IProjectReader GetReader(string location);
        Orc.ProjectManagement.IProjectWriter GetWriter(string location);
    }
    public interface IProjectStateService
    {
        bool IsRefreshingActiveProject { get; }
        public event System.EventHandler<System.EventArgs> IsRefreshingActiveProjectUpdated;
        public event System.EventHandler<Orc.ProjectManagement.ProjectStateEventArgs> ProjectStateUpdated;
        Orc.ProjectManagement.ProjectState GetProjectState(Orc.ProjectManagement.IProject project);
    }
    public interface IProjectUpgrader
    {
        System.Threading.Tasks.Task<bool> RequiresUpgradeAsync(string location);
        System.Threading.Tasks.Task<string> UpgradeAsync(string location);
    }
    public interface IProjectValidator
    {
        System.Threading.Tasks.Task<bool> CanStartLoadingProjectAsync(string location);
        System.Threading.Tasks.Task<Catel.Data.IValidationContext> ValidateProjectAsync(Orc.ProjectManagement.IProject project);
        System.Threading.Tasks.Task<Catel.Data.IValidationContext> ValidateProjectBeforeLoadingAsync(string location);
    }
    public class static IProjectValidatorExtensions { }
    public interface IProjectWriter
    {
        System.Threading.Tasks.Task<bool> WriteAsync(Orc.ProjectManagement.IProject project, string location);
    }
    public class MdiProjectManagementConfigurationService : Orc.ProjectManagement.ProjectManagementConfigurationService
    {
        public MdiProjectManagementConfigurationService() { }
        public override Orc.ProjectManagement.ProjectManagementType GetProjectManagementType() { }
    }
    public class ProjectActivationHistoryService : Orc.ProjectManagement.IProjectActivationHistoryService
    {
        public ProjectActivationHistoryService() { }
        public void ContinueUsingHistory() { }
        public void Forget(Orc.ProjectManagement.IProject project) { }
        public System.Collections.Generic.IEnumerable<Orc.ProjectManagement.IProject> GetActivationHistory() { }
        public Orc.ProjectManagement.IProject GetLastActiveProject() { }
        public void Remember(Orc.ProjectManagement.IProject project) { }
        public void SetProjectsSource(System.Collections.Generic.IEnumerable<Orc.ProjectManagement.IProject> projects) { }
        public void SuspendUsingHistory() { }
    }
    public abstract class ProjectBase : Catel.Data.ModelBase, Orc.ProjectManagement.IProject
    {
        public static readonly Catel.Data.PropertyData CreatedOnProperty;
        public static readonly Catel.Data.PropertyData IdProperty;
        public static readonly Catel.Data.PropertyData LocationProperty;
        public static readonly Catel.Data.PropertyData TitleProperty;
        public ProjectBase(string location) { }
        protected ProjectBase(string location, string title) { }
        public virtual System.DateTime CreatedOn { get; set; }
        public int Id { get; }
        public virtual string Location { get; set; }
        public string Title { get; }
        public virtual void ClearIsDirty() { }
        public void MarkAsDirty() { }
        protected override bool ShouldPropertyChangeUpdateIsDirty(string propertyName) { }
        public override string ToString() { }
    }
    public class ProjectCancelEventArgs : System.ComponentModel.CancelEventArgs
    {
        public ProjectCancelEventArgs(string location, bool cancel = False) { }
        public ProjectCancelEventArgs(Orc.ProjectManagement.IProject project, bool cancel = False) { }
        public string Location { get; }
        public Orc.ProjectManagement.IProject Project { get; }
    }
    public class ProjectErrorEventArgs : Orc.ProjectManagement.ProjectEventArgs
    {
        public ProjectErrorEventArgs(string location, System.Exception exception = null, Catel.Data.IValidationContext validationContext = null) { }
        public ProjectErrorEventArgs(Orc.ProjectManagement.IProject project, System.Exception exception = null, Catel.Data.IValidationContext validationContext = null) { }
        public System.Exception Exception { get; }
        public Catel.Data.IValidationContext ValidationContext { get; }
    }
    public class ProjectEventArgs : System.EventArgs
    {
        public ProjectEventArgs(Orc.ProjectManagement.IProject project) { }
        public ProjectEventArgs(string location) { }
        public string Location { get; }
        public Orc.ProjectManagement.IProject Project { get; }
    }
    public class ProjectException : System.Exception
    {
        public ProjectException(string location, string message) { }
        public ProjectException(Orc.ProjectManagement.IProject project, string message) { }
        public ProjectException(Orc.ProjectManagement.IProject project, string message, System.Exception innerException) { }
        public string Location { get; }
        public Orc.ProjectManagement.IProject Project { get; }
    }
    public class static ProjectExtensions { }
    public class ProjectFileSystemEventArgs : Orc.ProjectManagement.ProjectEventArgs
    {
        public ProjectFileSystemEventArgs(Orc.ProjectManagement.IProject project, params string[] fileNames) { }
        public ProjectFileSystemEventArgs(string location, params string[] fileNames) { }
        public string[] FileNames { get; }
    }
    public abstract class ProjectManagementConfigurationService : Orc.ProjectManagement.IProjectManagementConfigurationService
    {
        protected ProjectManagementConfigurationService() { }
        public abstract Orc.ProjectManagement.ProjectManagementType GetProjectManagementType();
    }
    public class ProjectManagementInitializationService : Orc.ProjectManagement.IProjectManagementInitializationService
    {
        public ProjectManagementInitializationService(Catel.IoC.IServiceLocator serviceLocator, Orc.ProjectManagement.IProjectManagementConfigurationService projectManagementConfigurationService, Catel.IoC.ITypeFactory typeFactory) { }
        protected Catel.IoC.IServiceLocator ServiceLocator { get; }
        public virtual void Initialize(Orc.ProjectManagement.IProjectManager projectManager) { }
        protected virtual void Initialize(Orc.ProjectManagement.IProjectManager projectManager, Orc.ProjectManagement.ProjectManagementType projectManagementType) { }
    }
    public enum ProjectManagementType
    {
        SingleDocument = 0,
        MultipleDocuments = 1,
    }
    public class ProjectManager : Catel.IoC.INeedCustomInitialization, Orc.ProjectManagement.IProjectManager
    {
        public ProjectManager(Orc.ProjectManagement.IProjectValidator projectValidator, Orc.ProjectManagement.IProjectUpgrader projectUpgrader, Orc.ProjectManagement.IProjectRefresherSelector projectRefresherSelector, Orc.ProjectManagement.IProjectSerializerSelector projectSerializerSelector, Orc.ProjectManagement.IProjectInitializer projectInitializer, Orc.ProjectManagement.IProjectManagementConfigurationService projectManagementConfigurationService, Orc.ProjectManagement.IProjectManagementInitializationService projectManagementInitializationService, Orc.ProjectManagement.IProjectStateService projectStateService) { }
        public virtual Orc.ProjectManagement.IProject ActiveProject { get; set; }
        public bool IsLoading { get; }
        public Orc.ProjectManagement.ProjectManagementType ProjectManagementType { get; }
        public virtual System.Collections.Generic.IEnumerable<Orc.ProjectManagement.IProject> Projects { get; }
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectUpdatedEventArgs> ProjectActivatedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectUpdatingCancelEventArgs> ProjectActivationAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectActivationCanceledAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectErrorEventArgs> ProjectActivationFailedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectClosedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectCancelEventArgs> ProjectClosingAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectClosingCanceledAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectLoadedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectCancelEventArgs> ProjectLoadingAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectLoadingCanceledAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectErrorEventArgs> ProjectLoadingFailedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectRefreshedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectCancelEventArgs> ProjectRefreshingAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectRefreshingCanceledAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectErrorEventArgs> ProjectRefreshingFailedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectRefreshRequiredAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectSavedAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectCancelEventArgs> ProjectSavingAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectEventArgs> ProjectSavingCanceledAsync;
        public event Catel.AsyncEventHandler<Orc.ProjectManagement.ProjectErrorEventArgs> ProjectSavingFailedAsync;
        public System.Threading.Tasks.Task<bool> CloseAsync() { }
        public System.Threading.Tasks.Task<bool> CloseAsync(Orc.ProjectManagement.IProject project) { }
        public System.Threading.Tasks.Task InitializeAsync() { }
        public System.Threading.Tasks.Task<bool> LoadAsync(string location) { }
        public System.Threading.Tasks.Task<bool> LoadInactiveAsync(string location) { }
        protected virtual System.Threading.Tasks.Task<Orc.ProjectManagement.IProject> ReadProjectAsync(string location) { }
        public System.Threading.Tasks.Task<bool> RefreshAsync() { }
        public System.Threading.Tasks.Task<bool> RefreshAsync(Orc.ProjectManagement.IProject project) { }
        public System.Threading.Tasks.Task<bool> SaveAsync(string location = null) { }
        public System.Threading.Tasks.Task<bool> SaveAsync(Orc.ProjectManagement.IProject project, string location = null) { }
        public System.Threading.Tasks.Task<bool> SetActiveProjectAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task<bool> WriteProjectAsync(Orc.ProjectManagement.IProject project, string location) { }
    }
    public abstract class ProjectManagerWorkflowItemBase : Orc.ProjectManagement.IProjectManagerWorkflowItem
    {
        protected ProjectManagerWorkflowItemBase() { }
        public virtual System.Threading.Tasks.Task ActivatedAsync(Orc.ProjectManagement.IProject oldProject, Orc.ProjectManagement.IProject newProject) { }
        public virtual System.Threading.Tasks.Task<bool> ActivationAsync(Orc.ProjectManagement.IProject oldProject, Orc.ProjectManagement.IProject newProject, bool isRefresh) { }
        public virtual System.Threading.Tasks.Task ActivationCanceledAsync(Orc.ProjectManagement.IProject project) { }
        public virtual System.Threading.Tasks.Task ActivationFailedAsync(Orc.ProjectManagement.IProject project, System.Exception exception, Catel.Data.IValidationContext validationContext) { }
        public virtual System.Threading.Tasks.Task ClosedAsync(Orc.ProjectManagement.IProject project) { }
        public virtual System.Threading.Tasks.Task<bool> ClosingAsync(Orc.ProjectManagement.IProject project) { }
        public virtual System.Threading.Tasks.Task ClosingCanceledAsync(Orc.ProjectManagement.IProject project) { }
        public virtual System.Threading.Tasks.Task LoadedAsync(Orc.ProjectManagement.IProject project) { }
        public virtual System.Threading.Tasks.Task<bool> LoadingAsync(string location) { }
        public virtual System.Threading.Tasks.Task LoadingCanceledAsync(string location) { }
        public virtual System.Threading.Tasks.Task LoadingFailedAsync(string location, System.Exception exception, Catel.Data.IValidationContext validationContext) { }
        public virtual System.Threading.Tasks.Task RefreshedAsync(Orc.ProjectManagement.IProject project) { }
        public virtual System.Threading.Tasks.Task<bool> RefreshingAsync(Orc.ProjectManagement.IProject project) { }
        public virtual System.Threading.Tasks.Task RefreshingCanceledAsync(Orc.ProjectManagement.IProject project) { }
        public virtual System.Threading.Tasks.Task RefreshingFailedAsync(Orc.ProjectManagement.IProject project, System.Exception exception, Catel.Data.IValidationContext validationContext) { }
        public virtual System.Threading.Tasks.Task SavedAsync(Orc.ProjectManagement.IProject project) { }
        public virtual System.Threading.Tasks.Task<bool> SavingAsync(Orc.ProjectManagement.IProject project) { }
        public virtual System.Threading.Tasks.Task SavingCanceledAsync(Orc.ProjectManagement.IProject project) { }
        public virtual System.Threading.Tasks.Task SavingFailedAsync(Orc.ProjectManagement.IProject project, System.Exception exception, Catel.Data.IValidationContext validationContext) { }
    }
    public abstract class ProjectManagerWorkflowProjectWatcherBase : Orc.ProjectManagement.ProjectWatcherBase
    {
        protected System.Collections.Generic.List<Orc.ProjectManagement.IProjectManagerWorkflowItem> ActivationSequence;
        protected System.Collections.Generic.List<Orc.ProjectManagement.IProjectManagerWorkflowItem> ClosingSequence;
        protected System.Collections.Generic.List<Orc.ProjectManagement.IProjectManagerWorkflowItem> LoadingSequence;
        protected System.Collections.Generic.List<Orc.ProjectManagement.IProjectManagerWorkflowItem> RefreshingSequence;
        protected System.Collections.Generic.List<Orc.ProjectManagement.IProjectManagerWorkflowItem> SavingSequence;
        protected ProjectManagerWorkflowProjectWatcherBase(Orc.ProjectManagement.IProjectManager projectManager) { }
        protected abstract void ArrangeWorkflowItems();
        public virtual void Initialize() { }
        protected virtual System.Threading.Tasks.Task OnActivatedAsync(Orc.ProjectManagement.IProject oldProject, Orc.ProjectManagement.IProject newProject) { }
        protected virtual System.Threading.Tasks.Task OnActivationAsync(Orc.ProjectManagement.ProjectUpdatingCancelEventArgs e) { }
        protected virtual System.Threading.Tasks.Task OnActivationCanceledAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnActivationFailedAsync(Orc.ProjectManagement.IProject project, System.Exception exception) { }
        protected virtual System.Threading.Tasks.Task OnClosedAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnClosingAsync(Orc.ProjectManagement.ProjectCancelEventArgs e) { }
        protected virtual System.Threading.Tasks.Task OnClosingCanceledAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnLoadedAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnLoadingAsync(Orc.ProjectManagement.ProjectCancelEventArgs e) { }
        protected virtual System.Threading.Tasks.Task OnLoadingCanceledAsync(string location) { }
        protected virtual System.Threading.Tasks.Task OnLoadingFailedAsync(string location, System.Exception exception, Catel.Data.IValidationContext validationContext) { }
        protected virtual System.Threading.Tasks.Task OnRefreshedAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnRefreshingAsync(Orc.ProjectManagement.ProjectCancelEventArgs e) { }
        protected virtual System.Threading.Tasks.Task OnRefreshingCanceledAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnRefreshingFailedAsync(Orc.ProjectManagement.IProject project, System.Exception exception, Catel.Data.IValidationContext validationContext) { }
        protected virtual System.Threading.Tasks.Task OnSavedAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnSavingAsync(Orc.ProjectManagement.ProjectCancelEventArgs e) { }
        protected virtual System.Threading.Tasks.Task OnSavingCanceledAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnSavingFailedAsync(Orc.ProjectManagement.IProject project, System.Exception exception) { }
    }
    public abstract class ProjectReaderBase : Orc.ProjectManagement.IProjectReader
    {
        protected ProjectReaderBase() { }
        public System.Threading.Tasks.Task<Orc.ProjectManagement.IProject> ReadAsync(string location) { }
        protected abstract System.Threading.Tasks.Task<Orc.ProjectManagement.IProject> ReadFromLocationAsync(string location);
    }
    public abstract class ProjectRefresherBase : Orc.ProjectManagement.IProjectRefresher
    {
        protected ProjectRefresherBase(string projectLocation) { }
        protected ProjectRefresherBase(string projectLocation, string locationToWatch) { }
        public bool IsEnabled { get; set; }
        public bool IsSubscribed { get; }
        public bool IsSuspended { get; }
        public string Location { get; }
        public string ProjectLocation { get; }
        protected Orc.ProjectManagement.IProjectManager ProjectManager { get; }
        public event System.EventHandler<Orc.ProjectManagement.ProjectEventArgs> Updated;
        protected void RaiseUpdated(string fileName) { }
        public void Subscribe() { }
        protected abstract void SubscribeToLocation(string location);
        public void Unsubscribe() { }
        protected abstract void UnsubscribeFromLocation(string location);
    }
    public class ProjectState
    {
        public ProjectState() { }
        public ProjectState(Orc.ProjectManagement.ProjectState projectState) { }
        public bool IsActivating { get; set; }
        public bool IsClosing { get; set; }
        public bool IsDeactivating { get; set; }
        public bool IsLoading { get; set; }
        public bool IsRefreshing { get; set; }
        public bool IsSaving { get; set; }
        public string Location { get; set; }
    }
    public class ProjectStateEventArgs : System.EventArgs
    {
        public ProjectStateEventArgs(Orc.ProjectManagement.ProjectState projectState) { }
        public Orc.ProjectManagement.ProjectState ProjectState { get; }
    }
    public class ProjectStateService : Orc.ProjectManagement.IProjectStateService
    {
        public ProjectStateService() { }
        public bool IsRefreshingActiveProject { get; }
        public event System.EventHandler<System.EventArgs> IsRefreshingActiveProjectUpdated;
        public event System.EventHandler<Orc.ProjectManagement.ProjectStateEventArgs> ProjectStateUpdated;
        public Orc.ProjectManagement.ProjectState GetProjectState(Orc.ProjectManagement.IProject project) { }
        public void SetProjectActivating(string location, bool value) { }
        public void SetProjectClosing(string location, bool value) { }
        public void SetProjectDeactivating(string location, bool value) { }
        public void SetProjectLoading(string location, bool value) { }
        public void SetProjectRefreshing(string location, bool value, bool isActiveProject = True) { }
        public void SetProjectSaving(string location, bool value) { }
    }
    public class ProjectUpdatedEventArgs : System.EventArgs
    {
        public ProjectUpdatedEventArgs(Orc.ProjectManagement.IProject oldProject, Orc.ProjectManagement.IProject newProject) { }
        public bool IsRefresh { get; }
        public Orc.ProjectManagement.IProject NewProject { get; }
        public Orc.ProjectManagement.IProject OldProject { get; }
    }
    public class ProjectUpdatingCancelEventArgs : System.ComponentModel.CancelEventArgs
    {
        public ProjectUpdatingCancelEventArgs(Orc.ProjectManagement.IProject oldProject, Orc.ProjectManagement.IProject newProject, bool cancel = False) { }
        public ProjectUpdatingCancelEventArgs(string oldProjectLocation, string newProjectLocation, bool cancel = False) { }
        public bool IsRefresh { get; }
        public Orc.ProjectManagement.IProject NewProject { get; }
        public string NewProjectLocation { get; }
        public Orc.ProjectManagement.IProject OldProject { get; }
        public string OldProjectLocation { get; }
    }
    public abstract class ProjectValidatorBase : Orc.ProjectManagement.IProjectValidator
    {
        protected ProjectValidatorBase() { }
        public virtual System.Threading.Tasks.Task<bool> CanStartLoadingProjectAsync(string location) { }
        public virtual System.Threading.Tasks.Task<Catel.Data.IValidationContext> ValidateProjectAsync(Orc.ProjectManagement.IProject project) { }
        public virtual System.Threading.Tasks.Task<Catel.Data.IValidationContext> ValidateProjectBeforeLoadingAsync(string location) { }
    }
    public abstract class ProjectWatcherBase
    {
        protected ProjectWatcherBase(Orc.ProjectManagement.IProjectManager projectManager) { }
        protected bool IsActivating { get; }
        protected bool IsClosing { get; }
        protected bool IsLoading { get; }
        protected bool IsRefreshing { get; }
        protected bool IsSaving { get; }
        protected Orc.ProjectManagement.IProjectManager ProjectManager { get; }
        protected virtual System.Threading.Tasks.Task OnActivatedAsync(Orc.ProjectManagement.IProject oldProject, Orc.ProjectManagement.IProject newProject) { }
        protected virtual System.Threading.Tasks.Task OnActivationAsync(Orc.ProjectManagement.ProjectUpdatingCancelEventArgs e) { }
        protected virtual System.Threading.Tasks.Task OnActivationCanceledAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnActivationFailedAsync(Orc.ProjectManagement.IProject project, System.Exception exception) { }
        protected virtual System.Threading.Tasks.Task OnClosedAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnClosingAsync(Orc.ProjectManagement.ProjectCancelEventArgs e) { }
        protected virtual System.Threading.Tasks.Task OnClosingCanceledAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnLoadedAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnLoadingAsync(Orc.ProjectManagement.ProjectCancelEventArgs e) { }
        protected virtual System.Threading.Tasks.Task OnLoadingCanceledAsync(string location) { }
        protected virtual System.Threading.Tasks.Task OnLoadingFailedAsync(string location, System.Exception exception, Catel.Data.IValidationContext validationContext) { }
        protected virtual System.Threading.Tasks.Task OnRefreshedAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnRefreshingAsync(Orc.ProjectManagement.ProjectCancelEventArgs e) { }
        protected virtual System.Threading.Tasks.Task OnRefreshingCanceledAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnRefreshingFailedAsync(Orc.ProjectManagement.IProject project, System.Exception exception, Catel.Data.IValidationContext validationContext) { }
        protected virtual System.Threading.Tasks.Task OnRefreshingInternalAsync(Orc.ProjectManagement.ProjectCancelEventArgs e) { }
        protected virtual System.Threading.Tasks.Task OnRefreshRequiredAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnSavedAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnSavingAsync(Orc.ProjectManagement.ProjectCancelEventArgs e) { }
        protected virtual System.Threading.Tasks.Task OnSavingCanceledAsync(Orc.ProjectManagement.IProject project) { }
        protected virtual System.Threading.Tasks.Task OnSavingFailedAsync(Orc.ProjectManagement.IProject project, System.Exception exception) { }
    }
    public abstract class ProjectWriterBase<TProject> : Orc.ProjectManagement.IProjectWriter
        where TProject : Orc.ProjectManagement.IProject
    {
        protected ProjectWriterBase() { }
        public System.Threading.Tasks.Task<bool> WriteAsync(Orc.ProjectManagement.IProject project, string location) { }
        protected abstract System.Threading.Tasks.Task<bool> WriteToLocationAsync(TProject project, string location);
    }
    public class SdiProjectManagementConfigurationService : Orc.ProjectManagement.ProjectManagementConfigurationService
    {
        public SdiProjectManagementConfigurationService() { }
        public override Orc.ProjectManagement.ProjectManagementType GetProjectManagementType() { }
    }
    public class SdiProjectManagementException : System.Exception
    {
        public SdiProjectManagementException() { }
        public SdiProjectManagementException(string messageFormat, params object[] args) { }
        public SdiProjectManagementException(string message) { }
    }
}
namespace Orc.ProjectManagement.Serialization
{
    public class DefaultProjectSerializerSelector : Orc.ProjectManagement.IProjectSerializerSelector
    {
        public DefaultProjectSerializerSelector(Catel.IoC.IServiceLocator serviceLocator) { }
        public Orc.ProjectManagement.IProjectReader GetReader(string location) { }
        public Orc.ProjectManagement.IProjectWriter GetWriter(string location) { }
    }
    public class FixedProjectSerializerSelector<TReader, TWriter> : Orc.ProjectManagement.IProjectSerializerSelector
        where TReader : Orc.ProjectManagement.IProjectReader
        where TWriter : Orc.ProjectManagement.IProjectWriter
    {
        public FixedProjectSerializerSelector() { }
        public Orc.ProjectManagement.IProjectReader GetReader(string location) { }
        public Orc.ProjectManagement.IProjectWriter GetWriter(string location) { }
    }
}