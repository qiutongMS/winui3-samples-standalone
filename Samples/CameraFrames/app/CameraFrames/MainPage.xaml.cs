using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace CameraFrames;

/// <summary>
/// The main content page with NavigationView shell for scenario navigation.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        // Select the first scenario by default
        NavView.SelectedItem = NavView.MenuItems[0];
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item)
        {
            var tag = item.Tag as string;
            switch (tag)
            {
                case "Scenario1":
                    ScenarioFrame.Navigate(typeof(SDKTemplate.Scenario1_DisplayDepthColorIR));
                    break;
                case "Scenario2":
                    ScenarioFrame.Navigate(typeof(SDKTemplate.Scenario2_FindAvailableSourceGroups));
                    break;
            }
        }
    }
}
