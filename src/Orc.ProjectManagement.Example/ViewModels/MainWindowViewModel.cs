namespace Orc.ProjectManagement.Example.ViewModels
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Catel.Fody;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Models;

    public class MainWindowViewModel : ViewModelBase
    {
        private const string TextFilter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IMessageService _messageService;
        private readonly IOpenFileService _openFileService;
        private readonly IProcessService _processService;
        private readonly IProjectManager _projectManager;
        private readonly ISaveFileService _saveFileService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel(IProjectManager projectManager, IOpenFileService openFileService,
            ISaveFileService saveFileService, IProcessService processService, IMessageService messageService)
        {
            ArgumentNullException.ThrowIfNull(projectManager);
            ArgumentNullException.ThrowIfNull(openFileService);
            ArgumentNullException.ThrowIfNull(saveFileService);
            ArgumentNullException.ThrowIfNull(processService);

            _projectManager = projectManager;
            _openFileService = openFileService;
            _saveFileService = saveFileService;
            _processService = processService;
            _messageService = messageService;

            LoadProject = new TaskCommand(OnLoadProjectExecuteAsync);
            RefreshProject = new TaskCommand(OnRefreshProjectExecuteAsync, OnRefreshProjectCanExecute);
            SaveProject = new TaskCommand(OnSaveProjectExecuteAsync, OnSaveProjectCanExecute);
            SaveProjectAs = new TaskCommand(OnSaveProjectAsExecuteAsync, OnSaveProjectAsCanExecute);
            CloseProject = new Command(OnCloseProjectExecute, OnCloseProjectCanExecute);
            OpenFile = new Command(OnOpenFileExecute, OnOpenFileCanExecute);

            Title = "Orc.ProjectManagement example";
        }

        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        public override string Title
        {
            get { return "Orc.ProjectManagement.Example"; }
        }

        [Model]
        [Expose("FirstName")]
        [Expose("MiddleName")]
        [Expose("LastName")]
        public PersonProject? Project { get; private set; }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync().ConfigureAwait(false);

            _projectManager.ProjectActivatedAsync += OnProjectActivatedAsync;

            ReloadProject();
        }

        protected override Task CloseAsync()
        {
            _projectManager.ProjectActivatedAsync -= OnProjectActivatedAsync;

            return base.CloseAsync();
        }

        private async Task OnProjectActivatedAsync(object sender, ProjectUpdatedEventArgs e)
        {
            ReloadProject();
        }

        private void ReloadProject()
        {
            Project = _projectManager.GetActiveProject<PersonProject>();
        }

        public TaskCommand LoadProject { get; private set; }

        private async Task OnLoadProjectExecuteAsync()
        {
            var result = await _openFileService.DetermineFileAsync(new DetermineOpenFileContext
            {
                InitialDirectory = Path.Combine(Environment.CurrentDirectory, "Data"),
                Filter = TextFilter
            });

            if (result.Result)
            {
                await _projectManager.LoadAsync(result.FileName).ConfigureAwait(false);
            }
        }

        public TaskCommand RefreshProject { get; private set; }

        private bool OnRefreshProjectCanExecute()
        {
            return _projectManager.ActiveProject is not null;
        }

        private async Task OnRefreshProjectExecuteAsync()
        {
            await _projectManager.RefreshAsync().ConfigureAwait(false);
        }

        public TaskCommand SaveProject { get; private set; }

        private bool OnSaveProjectCanExecute()
        {
            return _projectManager.ActiveProject is not null;
        }

        private async Task OnSaveProjectExecuteAsync()
        {
            await _projectManager.SaveAsync().ConfigureAwait(false);
        }

        public TaskCommand SaveProjectAs { get; private set; }

        private bool OnSaveProjectAsCanExecute()
        {
            return _projectManager.ActiveProject is not null;
        }

        private async Task OnSaveProjectAsExecuteAsync()
        {
            var result = await _saveFileService.DetermineFileAsync(new DetermineSaveFileContext
            {
                Filter = TextFilter
            });

            if (result.Result)
            {
                await _projectManager.SaveAsync(result.FileName).ConfigureAwait(false);
            }
        }

        public Command CloseProject { get; private set; }

        private bool OnCloseProjectCanExecute()
        {
            return _projectManager.ActiveProject is not null;
        }

        private void OnCloseProjectExecute()
        {
            _projectManager.CloseAsync();
        }

        public Command OpenFile { get; private set; }

        private bool OnOpenFileCanExecute()
        {
            return _projectManager.ActiveProject is not null;
        }

        private void OnOpenFileExecute()
        {
            _processService.StartProcess(_projectManager.ActiveProject.Location);
        }
    }
}
