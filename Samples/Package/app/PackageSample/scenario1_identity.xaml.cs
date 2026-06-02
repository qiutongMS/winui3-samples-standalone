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
/// Scenario page showing the current app package identity.
/// </summary>
public sealed partial class Scenario1 : Page
{
    private readonly MainPage? _rootPage = MainPage.Current;

    public Scenario1()
    {
        InitializeComponent();
    }

    private static string VersionString(PackageVersion version)
    {
        return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    private static string ArchitectureString(Windows.System.ProcessorArchitecture architecture)
    {
        return architecture switch
        {
            Windows.System.ProcessorArchitecture.X86 => "x86",
            Windows.System.ProcessorArchitecture.Arm => "arm",
            Windows.System.ProcessorArchitecture.X64 => "x64",
            Windows.System.ProcessorArchitecture.Neutral => "neutral",
            Windows.System.ProcessorArchitecture.Unknown => "unknown",
            _ => "???"
        };
    }

    private void GetPackage_Click(object sender, RoutedEventArgs e)
    {
        Package package = Package.Current;
        PackageId packageId = package.Id;

        string output = $"Name: \"{packageId.Name}\"\n" +
                        $"Version: {VersionString(packageId.Version)}\n" +
                        $"Architecture: {ArchitectureString(packageId.Architecture)}\n" +
                        $"ResourceId: \"{packageId.ResourceId}\"\n" +
                        $"Publisher: \"{packageId.Publisher}\"\n" +
                        $"PublisherId: \"{packageId.PublisherId}\"\n" +
                        $"FullName: \"{packageId.FullName}\"\n" +
                        $"FamilyName: \"{packageId.FamilyName}\"\n" +
                        $"IsFramework: {package.IsFramework}\n" +
                        $"IsResourcePackage: {package.IsResourcePackage}\n" +
                        $"IsBundle: {package.IsBundle}\n" +
                        $"IsDevelopmentMode: {package.IsDevelopmentMode}\n" +
                        $"DisplayName: \"{package.DisplayName}\"\n" +
                        $"PublisherDisplayName: \"{package.PublisherDisplayName}\"\n" +
                        $"Description: \"{package.Description}\"\n" +
                        $"Logo: \"{package.Logo.AbsoluteUri}\"\n";

        OutputTextBlock.Text = output;
    }
}
