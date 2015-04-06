// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
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

            LoadProject = new TaskCommand(OnLoadProjectExecute);
            RefreshProject = new TaskCommand(OnRefreshProjectExecute, OnRefreshProjectCanExecute);
            SaveProject = new TaskCommand(OnSaveProjectExecute, OnSaveProjectCanExecute);
            SaveProjectAs = new TaskCommand(OnSaveProjectAsExecute, OnSaveProjectAsCanExecute);
            CloseProject = new Command(OnCloseProjectExecute, OnCloseProjectCanExecute);
            OpenFile = new Command(OnOpenFileExecute, OnOpenFileCanExecute);
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
        protected override async Task Initialize()
        {
            await base.Initialize();

            _projectManager.ProjectUpdated += OnProjectUpdated;

            ReloadProject();
        }

        protected override Task Close()
        {
            _projectManager.ProjectUpdated -= OnProjectUpdated;

            return base.Close();
        }

        private void OnProjectUpdated(object sender, ProjectUpdatedEventArgs e)
        {
            ReloadProject();
        }

        private void ReloadProject()
        {
            Project = _projectManager.GetProject<PersonProject>();
        }
        #endregion

        #region Commands
        public TaskCommand LoadProject { get; private set; }

        private async Task OnLoadProjectExecute()
        {
            _openFileService.InitialDirectory = Path.Combine(Environment.CurrentDirectory, "Data");
            _openFileService.Filter = TextFilter;
            if (_openFileService.DetermineFile())
            {
                await _projectManager.Load(_openFileService.FileName);
            }
        }

        public TaskCommand RefreshProject { get; private set; }

        private bool OnRefreshProjectCanExecute()
        {
            return _projectManager.Project != null;
        }

        private async Task OnRefreshProjectExecute()
        {
            await _projectManager.Refresh();
        }

        public TaskCommand SaveProject { get; private set; }

        private bool OnSaveProjectCanExecute()
        {
            return _projectManager.Project != null;
        }

        private async Task OnSaveProjectExecute()
        {
            await _projectManager.Save();
        }

        public TaskCommand SaveProjectAs { get; private set; }

        private bool OnSaveProjectAsCanExecute()
        {
            return _projectManager.Project != null;
        }

        private async Task OnSaveProjectAsExecute()
        {
            _saveFileService.Filter = TextFilter;
            if (_saveFileService.DetermineFile())
            {
                await _projectManager.Save(_openFileService.FileName);
            }
        }

        public Command CloseProject { get; private set; }

        private bool OnCloseProjectCanExecute()
        {
            return _projectManager.Project != null;
        }

        private void OnCloseProjectExecute()
        {
            _projectManager.Close();
        }

        public Command OpenFile { get; private set; }

        private bool OnOpenFileCanExecute()
        {
            return _projectManager.Project != null;
        }

        private void OnOpenFileExecute()
        {
            _processService.StartProcess(_projectManager.Location);
        }
        #endregion
    }
}