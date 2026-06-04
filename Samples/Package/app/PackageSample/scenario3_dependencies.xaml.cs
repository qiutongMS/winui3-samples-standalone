using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel;

namespace SDKTemplate;

public sealed partial class Scenario3 : Page
{
    public Scenario3()
    {
        InitializeComponent();
    }

    private void GetDependencies_Click(object sender, RoutedEventArgs e)
    {
        IReadOnlyList<Package> dependencies = Package.Current.Dependencies;

        string output = $"Count: {dependencies.Count}";
        for (int i = 0; i < dependencies.Count; i++)
        {
            Package dependency = dependencies[i];
            output += $"\n[{i}]: {dependency.Id.FullName}";
        }

        OutputTextBlock.Text = output;
    }
}
