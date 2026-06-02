//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace SDKTemplate
{
    /// <summary>
    /// Scenario 1 Change camera preview settings
    /// </summary>
    public sealed partial class Scenario1_PreviewSettings : Page
    {
        private MainPage rootPage = MainPage.Current;
        private MediaCapturePreviewer _previewer = null!;

        public Scenario1_PreviewSettings()
        {
            this.InitializeComponent();
            _previewer = new MediaCapturePreviewer(PreviewControl, DispatcherQueue);
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            await _previewer.CleanupCameraAsync();
        }

        private async void InitializeCameraButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            rootPage.NotifyUser("", NotifyType.StatusMessage);

            button!.IsEnabled = false;
            await _previewer.InitializeCameraAsync();
            button.IsEnabled = true;

            if (_previewer.IsPreviewing)
            {
                button.Visibility = Visibility.Collapsed;
                PreviewControl.Visibility = Visibility.Visible;
                PopulateSettingsComboBox();
            }
        }

        private async void ComboBoxSettings_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (_previewer.IsPreviewing)
            {
                var selectedItem = (sender as ComboBox)?.SelectedItem as ComboBoxItem;
                if (selectedItem?.Tag is StreamResolution resolution)
                {
                    var encodingProperties = resolution.EncodingProperties;
                    await _previewer.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, encodingProperties);
                }
            }
        }

        private void PopulateSettingsComboBox()
        {
            IEnumerable<StreamResolution> allProperties = _previewer.MediaCapture!.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoPreview).Select(x => new StreamResolution(x));

            allProperties = allProperties.OrderByDescending(x => x.Height * x.Width).ThenByDescending(x => x.FrameRate);

            foreach (var property in allProperties)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();
                comboBoxItem.Content = property.GetFriendlyName();
                comboBoxItem.Tag = property;
                CameraSettings.Items.Add(comboBoxItem);
            }
        }
    }
}
