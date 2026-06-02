using System;
using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace SDKTemplate;

public sealed partial class MainPage : Page
{
    public static MainPage Current = null!;

    public MainPage()
    {
        InitializeComponent();
        Current = this;
        SampleTitle.Text = FEATURE_NAME;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        var menuItems = new List<NavigationViewItem>();
        int i = 1;
        foreach (Scenario s in scenarios)
        {
            var item = new NavigationViewItem
            {
                Content = $"{i}) {s.Title}",
                Tag = s
            };
            AutomationProperties.SetAutomationId(item, $"Scenario{i}");
            menuItems.Add(item);
            i++;
        }

        NavView.MenuItems.Clear();
        foreach (var item in menuItems)
        {
            NavView.MenuItems.Add(item);
        }

        if (menuItems.Count > 0)
        {
            NavView.SelectedItem = menuItems[0];
        }
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        NotifyUser(string.Empty, NotifyType.StatusMessage);

        if (args.SelectedItem is NavigationViewItem item && item.Tag is Scenario s)
        {
            ScenarioFrame.Navigate(s.ClassType);
        }
    }

    public List<Scenario> Scenarios
    {
        get { return this.scenarios; }
    }

    public void NotifyUser(string strMessage, NotifyType type)
    {
        if (DispatcherQueue.HasThreadAccess)
        {
            UpdateStatus(strMessage, type);
        }
        else
        {
            DispatcherQueue.TryEnqueue(() => UpdateStatus(strMessage, type));
        }
    }

    private void UpdateStatus(string strMessage, NotifyType type)
    {
        switch (type)
        {
            case NotifyType.StatusMessage:
                StatusBorder.Background = new SolidColorBrush(Colors.Green);
                break;
            case NotifyType.ErrorMessage:
                StatusBorder.Background = new SolidColorBrush(Colors.Red);
                break;
        }

        StatusBlock.Text = strMessage;

        StatusBorder.Visibility = (StatusBlock.Text != string.Empty) ? Visibility.Visible : Visibility.Collapsed;
        StatusPanel.Visibility = (StatusBlock.Text != string.Empty) ? Visibility.Visible : Visibility.Collapsed;
    }
}
