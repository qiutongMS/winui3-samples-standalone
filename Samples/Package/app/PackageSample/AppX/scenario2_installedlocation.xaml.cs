//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;

namespace PackageSample;

/// <summary>
/// Scenario page showing the package installed location.
/// </summary>
public sealed partial class Scenario2 : Page
{
    private readonly MainPage? _rootPage = MainPage.Current;

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
