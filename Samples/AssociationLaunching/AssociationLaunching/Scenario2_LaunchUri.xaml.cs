using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace AssociationLaunching;

public sealed partial class Scenario2_LaunchUri : Page
{
    private readonly MainPage _rootPage = MainPage.Current;

    public Scenario2_LaunchUri()
    {
        InitializeComponent();
        ViewPreference.ItemsSource = MainPage.ViewSizePreferences;
        ViewPreference.SelectedIndex = 0;
    }

    private async void LaunchUriDefault()
    {
        var uri = new Uri(UriToLaunch.Text);
        bool success = await Launcher.LaunchUriAsync(uri);
        if (success)
        {
            _rootPage.NotifyUser("URI launched: " + uri.AbsoluteUri, NotifyType.StatusMessage);
        }
        else
        {
            _rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage);
        }
    }

    private async void LaunchUriWithWarning()
    {
        var uri = new Uri(UriToLaunch.Text);
        var options = new LauncherOptions() { TreatAsUntrusted = true };
        bool success = await Launcher.LaunchUriAsync(uri, options);
        if (success)
        {
            _rootPage.NotifyUser("URI launched: " + uri.AbsoluteUri, NotifyType.StatusMessage);
        }
        else
        {
            _rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage);
        }
    }

    private async void LaunchUriOpenWith(object sender, RoutedEventArgs e)
    {
        var uri = new Uri(UriToLaunch.Text);
        Point openWithPosition = MainPage.GetElementLocation(sender);
        var options = new LauncherOptions();
        options.DisplayApplicationPicker = true;
        options.UI.InvocationPoint = openWithPosition;
        options.UI.PreferredPlacement = Windows.UI.Popups.Placement.Below;

        bool success = await Launcher.LaunchUriAsync(uri, options);
        if (success)
        {
            _rootPage.NotifyUser("URI launched: " + uri.AbsoluteUri, NotifyType.StatusMessage);
        }
        else
        {
            _rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage);
        }
    }

    private async void LaunchUriSplitScreen()
    {
        var uri = new Uri(UriToLaunch.Text);
        var options = new LauncherOptions() { DesiredRemainingView = (ViewSizePreference)ViewPreference.SelectedValue };
        bool success = await Launcher.LaunchUriAsync(uri, options);
        if (success)
        {
            _rootPage.NotifyUser("URI launched: " + uri.AbsoluteUri, NotifyType.StatusMessage);
        }
        else
        {
            _rootPage.NotifyUser("URI launch failed.", NotifyType.ErrorMessage);
        }
    }
}