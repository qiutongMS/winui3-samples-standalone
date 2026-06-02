using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;
using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace AssociationLaunching;

public sealed partial class Scenario1_LaunchFile : Page
{
    private readonly MainPage _rootPage = MainPage.Current;
    private readonly string _fileToLaunch = @"Assets\microsoft-sdk.png";

    public Scenario1_LaunchFile()
    {
        InitializeComponent();
        ViewPreference.ItemsSource = MainPage.ViewSizePreferences;
        ViewPreference.SelectedIndex = 0;
    }

    private async Task<StorageFile> GetFileToLaunchAsync()
    {
        var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(_fileToLaunch);
        file = await file.CopyAsync(ApplicationData.Current.TemporaryFolder, "filetolaunch.png", NameCollisionOption.ReplaceExisting);
        return file;
    }

    private async void LaunchFileDefault()
    {
        var file = await GetFileToLaunchAsync();
        bool success = await Launcher.LaunchFileAsync(file);
        if (success)
        {
            _rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
        }
        else
        {
            _rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
        }
    }

    private async void LaunchFileWithWarning()
    {
        var file = await GetFileToLaunchAsync();
        var options = new LauncherOptions() { TreatAsUntrusted = true };
        bool success = await Launcher.LaunchFileAsync(file, options);
        if (success)
        {
            _rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
        }
        else
        {
            _rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
        }
    }

    private async void LaunchFileOpenWith(object sender, RoutedEventArgs e)
    {
        var file = await GetFileToLaunchAsync();
        Point openWithPosition = MainPage.GetElementLocation(sender);
        var options = new LauncherOptions();
        options.DisplayApplicationPicker = true;
        options.UI.InvocationPoint = openWithPosition;
        options.UI.PreferredPlacement = Windows.UI.Popups.Placement.Below;

        bool success = await Launcher.LaunchFileAsync(file, options);
        if (success)
        {
            _rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
        }
        else
        {
            _rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
        }
    }

    private async void LaunchFileSplitScreen()
    {
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        openPicker.FileTypeFilter.Add("*");

        // Initialize the picker with the window handle (required for WinUI 3 desktop)
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);

        StorageFile? file = await openPicker.PickSingleFileAsync();
        if (file != null)
        {
            var options = new LauncherOptions() { DesiredRemainingView = (ViewSizePreference)ViewPreference.SelectedValue };
            bool success = await Launcher.LaunchFileAsync(file, options);
            if (success)
            {
                _rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
            }
            else
            {
                _rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
            }
        }
        else
        {
            _rootPage.NotifyUser("No file was picked.", NotifyType.ErrorMessage);
        }
    }

    private async void PickAndLaunchFile()
    {
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        openPicker.FileTypeFilter.Add("*");

        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);

        StorageFile? file = await openPicker.PickSingleFileAsync();
        if (file != null)
        {
            bool success = await Launcher.LaunchFileAsync(file);
            if (success)
            {
                _rootPage.NotifyUser("File launched: " + file.Name, NotifyType.StatusMessage);
            }
            else
            {
                _rootPage.NotifyUser("File launch failed.", NotifyType.ErrorMessage);
            }
        }
        else
        {
            _rootPage.NotifyUser("No file was picked.", NotifyType.ErrorMessage);
        }
    }
}
