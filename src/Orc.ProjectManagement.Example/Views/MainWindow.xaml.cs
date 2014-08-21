// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.ProjectManagement.Example.Views
{
    using Catel.Logging;
    using Catel.Windows;
    using Logging;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : DataWindow
    {
        #region Constants
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
            : base(DataWindowMode.Custom)
        {
            InitializeComponent();

            LogManager.AddListener(new TextBoxLogListener(outputTextBox));

            Log.Info("Welcome to the example of Orc.ProjectManagement. Use any of the buttons above to control the project. Log messages will appear here");
        }
        #endregion
    }
}