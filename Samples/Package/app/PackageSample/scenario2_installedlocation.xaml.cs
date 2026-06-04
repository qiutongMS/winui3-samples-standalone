using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel;

namespace SDKTemplate;

public sealed partial class Scenario2 : Page
{
    public Scenario2()
    {
        InitializeComponent();
    }

    private void GetInstalledLocation_Click(object sender, RoutedEventArgs e)
    {
        Windows.Storage.StorageFolder installedLocation = Package.Current.InstalledLocation;
        OutputTextBlock.Text = $"Installed Location: {installedLocation.Path}";
    }
}
