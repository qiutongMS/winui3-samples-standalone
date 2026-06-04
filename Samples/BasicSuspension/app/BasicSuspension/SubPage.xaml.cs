using SDKTemplate.Common;
using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace SDKTemplate
{
    public sealed partial class SubPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private int tapCount;

        public SubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += LoadState;
            this.navigationHelper.SaveState += SaveState;
        }

        #region NavigationHelper registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.TitleTextBlock.Text = e.Parameter?.ToString() ?? string.Empty;

            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void LoadState(object sender, LoadStateEventArgs e)
        {
            tapCount = 0;
            if (e.PageState != null && e.PageState.ContainsKey("tapCount"))
            {
                tapCount = (int)e.PageState["tapCount"];
            }
            TapCountRun.Text = tapCount.ToString();
        }

        private void SaveState(object sender, SaveStateEventArgs e)
        {
            e.PageState["tapCount"] = tapCount;
        }

        #endregion

        private void Page_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            tapCount++;
            TapCountRun.Text = tapCount.ToString();
        }
    }
}
