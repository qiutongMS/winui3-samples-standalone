using System;
using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
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
        // Populate the NavigationView from the scenario list
        int i = 1;
        foreach (Scenario s in scenarios)
        {
            var item = new NavigationViewItem
            {
                Content = $"{i++}) {s.Title}",
                Tag = s
            };
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetAutomationId(item, $"Scenario{i - 1}");
            NavView.MenuItems.Add(item);
        }

        if (NavView.MenuItems.Count > 0)
        {
            NavView.SelectedItem = NavView.MenuItems[0];
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

    public List<Scenario> Scenarios => scenarios;

    /// <summary>
    /// Display a message to the user.
    /// This method may be called from any thread.
    /// </summary>
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

        var peer = FrameworkElementAutomationPeer.FromElement(StatusBlock);
        if (peer != null)
        {
            peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
        }
    }
}
