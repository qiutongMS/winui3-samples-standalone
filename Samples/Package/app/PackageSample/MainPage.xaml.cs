using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace PackageSample;

/// <summary>
/// Main navigation page that hosts scenario pages in a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
    public static MainPage? Current;

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
            menuItems.Add(new NavigationViewItem
            {
                Content = $"{i++}) {s.Title}",
                Tag = s.ClassType
            });
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

        if (args.SelectedItem is NavigationViewItem item && item.Tag is Type pageType)
        {
            ScenarioFrame.Navigate(pageType);
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
        peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
    }
}

public enum NotifyType
{
    StatusMessage,
    ErrorMessage
}
