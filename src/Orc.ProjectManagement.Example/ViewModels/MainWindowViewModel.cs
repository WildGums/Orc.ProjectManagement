// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindowViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Example.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
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
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private const string TextFilter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

        private readonly IProjectManager _projectManager;
        private readonly IOpenFileService _openFileService;
        private readonly ISaveFileService _saveFileService;
        private readonly IProcessService _processService;
        private readonly IMessageService _messageService;

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

            LoadProject = new Command(OnLoadProjectExecute);
            RefreshProject = new Command(OnRefreshProjectExecute, OnRefreshProjectCanExecute);
            SaveProject = new Command(OnSaveProjectExecute, OnSaveProjectCanExecute);
            SaveProjectAs = new Command(OnSaveProjectAsExecute, OnSaveProjectAsCanExecute);
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

        #region Commands
        public Command LoadProject { get; private set; }

        private void OnLoadProjectExecute()
        {
            _openFileService.InitialDirectory = Path.Combine(Environment.CurrentDirectory, "Data");
            _openFileService.Filter = TextFilter;
            if (_openFileService.DetermineFile())
            {
                _projectManager.Load(_openFileService.FileName);
            }
        }

        public Command RefreshProject { get; private set; }

        private bool OnRefreshProjectCanExecute()
        {
            return _projectManager.Project != null;
        }

        private void OnRefreshProjectExecute()
        {
            _projectManager.Refresh();
        }

        public Command SaveProject { get; private set; }

        private bool OnSaveProjectCanExecute()
        {
            return _projectManager.Project != null;
        }

        private void OnSaveProjectExecute()
        {
            _projectManager.Save();
        }

        public Command SaveProjectAs { get; private set; }

        private bool OnSaveProjectAsCanExecute()
        {
            return _projectManager.Project != null;
        }

        private void OnSaveProjectAsExecute()
        {
            _saveFileService.Filter = TextFilter;
            if (_saveFileService.DetermineFile())
            {
                _projectManager.Save(_openFileService.FileName);
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
    }
}