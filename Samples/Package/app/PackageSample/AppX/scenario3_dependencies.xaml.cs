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
/// Scenario page showing the package dependencies.
/// </summary>
public sealed partial class Scenario3 : Page
{
    private readonly MainPage? _rootPage = MainPage.Current;

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
