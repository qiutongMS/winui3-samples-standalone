using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;

namespace AssociationLaunching;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static Window MainWindow { get; private set; } = null!;

    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();
        MainWindow.Activate();

        // Handle activation (file, protocol, or normal launch)
        var activatedArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
        HandleActivation(activatedArgs);
    }

    public static void HandleActivation(AppActivationArguments activatedArgs)
    {
        if (MainWindow.Content is not MainPage mainPage)
        {
            return;
        }

        switch (activatedArgs.Kind)
        {
            case ExtendedActivationKind.File:
                mainPage.NavigateToPageWithParameter(2, activatedArgs.Data);
                break;
            case ExtendedActivationKind.Protocol:
                mainPage.NavigateToPageWithParameter(3, activatedArgs.Data);
                break;
        }
    }
}
