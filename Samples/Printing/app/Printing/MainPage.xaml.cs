using System;
using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PrintSample;

namespace SDKTemplate;

public partial class MainPage : Page
{
    public static MainPage Current { get; private set; } = null!;

    public const string FEATURE_NAME = "Printing C# Sample";

    List<Scenario> scenarios = new List<Scenario>
    {
        new Scenario() { Title="Basic", ClassType=typeof(Scenario1Basic)},
        new Scenario() { Title="Standard Options", ClassType=typeof(Scenario2StandardOptions)},
        new Scenario() { Title="Custom Options", ClassType=typeof(Scenario3CustomOptions)},
        new Scenario() { Title="Page Range", ClassType=typeof(Scenario4PageRange)},
        new Scenario() { Title="Photos", ClassType=typeof(Scenario5Photos)},
        new Scenario() { Title="Disable Preview", ClassType=typeof(Scenario6DisablePreview)}
    };

    public MainPage()
    {
        InitializeComponent();
        Current = this;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        var menuItems = new List<NavigationViewItem>();
        int i = 1;
        foreach (var s in scenarios)
        {
            var item = new NavigationViewItem
            {
                Content = $"{i++}) {s.Title}",
                Tag = s.ClassType
            };
            item.SetValue(Microsoft.UI.Xaml.Automation.AutomationProperties.AutomationIdProperty, $"NavItem_{s.ClassType.Name}");
            menuItems.Add(item);
        }

        NavView.MenuItemsSource = menuItems;

        if (menuItems.Count > 0)
        {
            NavView.SelectedItem = menuItems[0];
        }
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        NotifyUser(string.Empty, NotifyType.StatusMessage);

        if (args.SelectedItem is NavigationViewItem item && item.Tag is Type pageType)
        {
            ScenarioFrame.Navigate(pageType);
        }
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

        if (StatusBlock.Text != string.Empty)
        {
            StatusBorder.Visibility = Visibility.Visible;
            StatusPanel.Visibility = Visibility.Visible;
        }
        else
        {
            StatusBorder.Visibility = Visibility.Collapsed;
            StatusPanel.Visibility = Visibility.Collapsed;
        }

        var peer = FrameworkElementAutomationPeer.FromElement(StatusBlock);
        if (peer != null)
        {
            peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
        }
    }
}

public enum NotifyType
{
    StatusMessage,
    ErrorMessage
}

public class Scenario
{
    public string Title { get; set; } = string.Empty;
    public Type ClassType { get; set; } = typeof(MainPage);
}
