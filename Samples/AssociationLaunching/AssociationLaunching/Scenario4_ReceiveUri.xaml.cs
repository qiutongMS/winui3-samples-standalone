using Windows.ApplicationModel.Activation;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace AssociationLaunching;

public sealed partial class Scenario4_ReceiveUri : Page
{
    private readonly MainPage _rootPage = MainPage.Current;
    private readonly string Protocol = "alsdkcs";

    public Scenario4_ReceiveUri()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is IProtocolActivatedEventArgs args)
        {
            _rootPage.NotifyUser("Protocol activation received. The received URI is " + args.Uri.AbsoluteUri + ".", NotifyType.StatusMessage);
        }
    }
}