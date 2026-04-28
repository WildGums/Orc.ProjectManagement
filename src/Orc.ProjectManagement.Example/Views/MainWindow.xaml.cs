namespace Orc.ProjectManagement.Example.Views;

using Catel.Logging;
using Catel.Windows;
using Microsoft.Extensions.Logging;

public partial class MainWindow : DataWindow
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(MainWindow));

    partial void OnInitializedComponent()
    {
        Logger.LogInformation("Welcome to the example of Orc.ProjectManagement. Use any of the buttons above to control the project. Log messages will appear here");
    }
}
