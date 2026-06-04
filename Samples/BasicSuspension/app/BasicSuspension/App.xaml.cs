using SDKTemplate.Common;
using System;
using Windows.Storage;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace SDKTemplate
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private const string sessionStateFilename = "_sessionState.xml";
        private RootFrameNavigationHelper? rootFrameNavigationHelper;

        public static Window? MainWindow { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            SuspensionManager.KnownTypes.AddRange(new[] { typeof(ItemList), typeof(Item) });
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            var window = new MainWindow();
            MainWindow = window;

            var rootFrame = window.GetRootFrame();
            SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

            this.rootFrameNavigationHelper = new RootFrameNavigationHelper(rootFrame);

            rootFrame.NavigationFailed += OnNavigationFailed;

            // If this is not the first time the app is run, then restore from the previous session.
            StorageFile? file;
            try
            {
                file = await ApplicationData.Current.LocalFolder.GetFileAsync(sessionStateFilename);
            }
            catch (Exception)
            {
                file = null;
            }

            if (file != null)
            {
                await SuspensionManager.RestoreAsync();
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), args.Arguments);
            }

            window.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
