namespace Orc.ProjectManagement.Example;

using System.Globalization;
using System.Windows;
using Catel.IoC;
using Catel.Logging;
using Catel.Reflection;
using Catel.Services;
using Catel.Windows;
using Orchestra;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    protected override void OnStartup(StartupEventArgs e)
    {
#if DEBUG
        LogManager.AddDebugListener(true);
#endif

        var languageService = ServiceLocator.Default.ResolveRequiredType<ILanguageService>();

        // Note: it's best to use .CurrentUICulture in actual apps since it will use the preferred language
        // of the user. But in order to demo multilingual features for devs (who mostly have en-US as .CurrentUICulture),
        // we use .CurrentCulture for the sake of the demo
        languageService.PreferredCulture = CultureInfo.CurrentCulture;
        languageService.FallbackCulture = new CultureInfo("en-US");

        Log.Info("Starting application");

        // To force the loading of all assemblies at startup, uncomment the lines below:

        //Log.Info("Preloading assemblies");
        //AppDomain.CurrentDomain.PreloadAssemblies();


        // Want to improve performance? Uncomment the lines below. Note though that this will disable
        // some features. 
        //
        // For more information, see https://catelproject.atlassian.net/wiki/display/CTL/Performance+considerations

        // Log.Info("Improving performance");
        // Catel.Data.ModelBase.DefaultSuspendValidationValue = true;
        // Catel.Windows.Controls.UserControl.DefaultCreateWarningAndErrorValidatorForViewModelValue = false;
        // Catel.Windows.Controls.UserControl.DefaultSkipSearchingForInfoBarMessageControlValue = true;


        // TODO: Register custom types in the ServiceLocator
        //Log.Info("Registering custom types");
        //var serviceLocator = ServiceLocator.Default;
        //serviceLocator.RegisterType<IMyInterface, IMyClass>();

        this.ApplyTheme();

        Log.Info("Calling base.OnStartup");

        base.OnStartup(e);
    }
}
