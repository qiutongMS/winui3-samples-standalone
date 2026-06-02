using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace BasicInput;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void NavView_Loaded(object sender, RoutedEventArgs e)
    {
        // Select the first item on load
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem selectedItem)
        {
            string? tag = selectedItem.Tag?.ToString();
            if (!string.IsNullOrEmpty(tag))
            {
                Type? pageType = Type.GetType(tag);
                if (pageType != null)
                {
                    ContentFrame.Navigate(pageType);
                }
            }
        }
    }
}
