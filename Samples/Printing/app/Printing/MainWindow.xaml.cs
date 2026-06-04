using Microsoft.UI.Xaml;

namespace Printing;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon("Assets/AppIcon.ico");
    }

    public void NavigateToMain()
    {
        RootFrame.Navigate(typeof(SDKTemplate.MainPage));
    }
}
