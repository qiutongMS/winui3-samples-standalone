using System;
using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace SDKTemplate;

public sealed partial class MainPage : Page
{
    public static MainPage Current { get; private set; } = null!;

    public MainPage()
    {
        InitializeComponent();
        Current = this;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (NavView.MenuItems.Count > 0)
        {
            NavView.SelectedItem = NavView.MenuItems[0];
        }
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        NotifyUser(string.Empty, NotifyType.StatusMessage);

        if (args.SelectedItem is NavigationViewItem item && item.Tag is string tag)
        {
            Type? pageType = Type.GetType(tag);
            if (pageType != null)
            {
                ScenarioFrame.Navigate(pageType);
            }
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

        StatusBorder.Visibility = string.IsNullOrEmpty(strMessage) ? Visibility.Collapsed : Visibility.Visible;
        StatusPanel.Visibility = string.IsNullOrEmpty(strMessage) ? Visibility.Collapsed : Visibility.Visible;
    }
}

public enum NotifyType
{
    StatusMessage,
    ErrorMessage
}
