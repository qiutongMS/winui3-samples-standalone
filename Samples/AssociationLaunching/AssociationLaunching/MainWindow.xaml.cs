using Microsoft.UI.Xaml;

namespace AssociationLaunching;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
    }
}
