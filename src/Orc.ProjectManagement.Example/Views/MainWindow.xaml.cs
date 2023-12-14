namespace Orc.ProjectManagement.Example.Views;

using Catel.Logging;
using Catel.Windows;
using Orchestra.Logging;

public partial class MainWindow : DataWindow
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    public MainWindow()
        : base(DataWindowMode.Custom)
    {
        InitializeComponent();

        LogManager.AddListener(new TextBoxLogListener(outputTextBox));

        Log.Info("Welcome to the example of Orc.ProjectManagement. Use any of the buttons above to control the project. Log messages will appear here");
    }
}
