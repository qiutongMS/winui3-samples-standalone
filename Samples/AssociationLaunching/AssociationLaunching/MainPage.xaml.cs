using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Automation.Peers;
using Windows.Foundation;
using Windows.UI.ViewManagement;

namespace AssociationLaunching;

public sealed partial class MainPage : Page
{
    public static MainPage Current { get; private set; } = null!;

    public static List<ViewSizePreference> ViewSizePreferences { get; } = new()
    {
        ViewSizePreference.Default,
        ViewSizePreference.UseLess,
        ViewSizePreference.UseHalf,
        ViewSizePreference.UseMore,
        ViewSizePreference.UseMinimum,
        ViewSizePreference.UseNone,
    };

    private readonly List<ScenarioInfo> _scenarios = new()
    {
        new ScenarioInfo("Launching a file", typeof(Scenario1_LaunchFile)),
        new ScenarioInfo("Launching a URI", typeof(Scenario2_LaunchUri)),
        new ScenarioInfo("Receiving a file", typeof(Scenario3_ReceiveFile)),
        new ScenarioInfo("Receiving a URI", typeof(Scenario4_ReceiveUri)),
    };

    public MainPage()
    {
        Current = this;
        InitializeComponent();

        // Select the first scenario by default
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    public void NavigateToPageWithParameter(int pageIndex, object parameter)
    {
        NavView.SelectedItem = NavView.MenuItems[pageIndex];
        ScenarioFrame.Navigate(_scenarios[pageIndex].ClassType, parameter);
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        NotifyUser(string.Empty, NotifyType.StatusMessage);

        if (args.SelectedItem is NavigationViewItem item)
        {
            var tag = item.Tag?.ToString();
            var index = tag switch
            {
                "Scenario1" => 0,
                "Scenario2" => 1,
                "Scenario3" => 2,
                "Scenario4" => 3,
                _ => -1
            };

            if (index >= 0)
            {
                ScenarioFrame.Navigate(_scenarios[index].ClassType);
            }
        }
    }

    public static Point GetElementLocation(object e)
    {
        var element = (FrameworkElement)e;
        Microsoft.UI.Xaml.Media.GeneralTransform buttonTransform = element.TransformToVisual(null);
        Point desiredLocation = buttonTransform.TransformPoint(new Point());
        desiredLocation.Y += element.ActualHeight;
        return desiredLocation;
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
                StatusBorder.Background = new SolidColorBrush(Microsoft.UI.Colors.Green);
                break;
            case NotifyType.ErrorMessage:
                StatusBorder.Background = new SolidColorBrush(Microsoft.UI.Colors.Red);
                break;
        }

        StatusBlock.Text = strMessage;
        StatusBorder.Visibility = string.IsNullOrEmpty(strMessage) ? Visibility.Collapsed : Visibility.Visible;
        StatusPanel.Visibility = string.IsNullOrEmpty(strMessage) ? Visibility.Collapsed : Visibility.Visible;

        var peer = FrameworkElementAutomationPeer.FromElement(StatusBlock);
        peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
    }
}

public enum NotifyType
{
    StatusMessage,
    ErrorMessage
}

public class ScenarioInfo
{
    public string Title { get; }
    public Type ClassType { get; }

    public ScenarioInfo(string title, Type classType)
    {
        Title = title;
        ClassType = classType;
    }

    public override string ToString() => Title;
}
