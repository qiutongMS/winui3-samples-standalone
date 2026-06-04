using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel;

namespace SDKTemplate;

public sealed partial class Scenario1 : Page
{
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

        string output = string.Format("Name: \"{0}\"\n" +
                                      "Version: {1}\n" +
                                      "Architecture: {2}\n" +
                                      "ResourceId: \"{3}\"\n" +
                                      "Publisher: \"{4}\"\n" +
                                      "PublisherId: \"{5}\"\n" +
                                      "FullName: \"{6}\"\n" +
                                      "FamilyName: \"{7}\"\n" +
                                      "IsFramework: {8}\n" +
                                      "IsResourcePackage: {9}\n" +
                                      "IsBundle: {10}\n" +
                                      "IsDevelopmentMode: {11}\n" +
                                      "DisplayName: \"{12}\"\n" +
                                      "PublisherDisplayName: \"{13}\"\n" +
                                      "Description: \"{14}\"\n" +
                                      "Logo: \"{15}\"\n",
                                      packageId.Name,
                                      VersionString(packageId.Version),
                                      ArchitectureString(packageId.Architecture),
                                      packageId.ResourceId,
                                      packageId.Publisher,
                                      packageId.PublisherId,
                                      packageId.FullName,
                                      packageId.FamilyName,
                                      package.IsFramework,
                                      package.IsResourcePackage,
                                      package.IsBundle,
                                      package.IsDevelopmentMode,
                                      package.DisplayName,
                                      package.PublisherDisplayName,
                                      package.Description,
                                      package.Logo.AbsoluteUri);

        OutputTextBlock.Text = output;
    }
}
