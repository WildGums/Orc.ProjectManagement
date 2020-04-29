// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Example.ViewModels
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Fody;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using Models;

    /// <summary>
    /// MainWindow view model.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        private const string TextFilter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IMessageService _messageService;
        private readonly IOpenFileService _openFileService;
        private readonly IProcessService _processService;
        private readonly IProjectManager _projectManager;
        private readonly ISaveFileService _saveFileService;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel(IProjectManager projectManager, IOpenFileService openFileService,
            ISaveFileService saveFileService, IProcessService processService, IMessageService messageService)
        {
            Argument.IsNotNull(() => projectManager);
            Argument.IsNotNull(() => openFileService);
            Argument.IsNotNull(() => saveFileService);
            Argument.IsNotNull(() => processService);

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
        #endregion

        #region Properties
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
        public PersonProject Project { get; private set; }
        #endregion

        #region Methods
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
        #endregion

        #region Commands
        public TaskCommand LoadProject { get; private set; }

        private async Task OnLoadProjectExecuteAsync()
        {
            _openFileService.InitialDirectory = Path.Combine(Environment.CurrentDirectory, "Data");
            _openFileService.Filter = TextFilter;

            if (await _openFileService.DetermineFileAsync())
            {
                await _projectManager.LoadAsync(_openFileService.FileName).ConfigureAwait(false);
            }
        }

        public TaskCommand RefreshProject { get; private set; }

        private bool OnRefreshProjectCanExecute()
        {
            return _projectManager.ActiveProject != null;
        }

        private async Task OnRefreshProjectExecuteAsync()
        {
            await _projectManager.RefreshAsync().ConfigureAwait(false);
        }

        public TaskCommand SaveProject { get; private set; }

        private bool OnSaveProjectCanExecute()
        {
            return _projectManager.ActiveProject != null;
        }

        private async Task OnSaveProjectExecuteAsync()
        {
            await _projectManager.SaveAsync().ConfigureAwait(false);
        }

        public TaskCommand SaveProjectAs { get; private set; }

        private bool OnSaveProjectAsCanExecute()
        {
            return _projectManager.ActiveProject != null;
        }

        private async Task OnSaveProjectAsExecuteAsync()
        {
            _saveFileService.Filter = TextFilter;

            if (await _saveFileService.DetermineFileAsync())
            {
                await _projectManager.SaveAsync(_saveFileService.FileName).ConfigureAwait(false);
            }
        }

        public Command CloseProject { get; private set; }

        private bool OnCloseProjectCanExecute()
        {
            return _projectManager.ActiveProject != null;
        }

        private void OnCloseProjectExecute()
        {
            _projectManager.CloseAsync();
        }

        public Command OpenFile { get; private set; }

        private bool OnOpenFileCanExecute()
        {
            return _projectManager.ActiveProject != null;
        }

        private void OnOpenFileExecute()
        {
            _processService.StartProcess(_projectManager.ActiveProject.Location);
        }
        #endregion
    }
}
