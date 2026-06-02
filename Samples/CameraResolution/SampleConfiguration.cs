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
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "Camera resolution C# sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Change camera preview settings", ClassType=typeof(Scenario1_PreviewSettings)},
            new Scenario() { Title="Change preview and photo settings", ClassType=typeof(Scenario2_PhotoSettings)},
            new Scenario() { Title="Match aspect ratios", ClassType=typeof(Scenario3_AspectRatio)}
        };
    }

    public class Scenario
    {
        public string Title { get; set; } = string.Empty;
        public Type ClassType { get; set; } = typeof(MainPage);
    }

    public class MediaCapturePreviewer
    {
        DispatcherQueue _dispatcherQueue;
        MediaPlayerElement _previewControl;
        MediaPlayer? _mediaPlayer;

        public MediaCapturePreviewer(MediaPlayerElement previewControl, DispatcherQueue dispatcherQueue)
        {
            _previewControl = previewControl;
            _dispatcherQueue = dispatcherQueue;
        }

        public bool IsPreviewing { get; private set; }
        public bool IsRecording { get; set; }
        public MediaCapture? MediaCapture { get; private set; }

        /// <summary>
        /// Sets encoding properties on a camera stream. Stops and restarts preview.
        /// </summary>
        public async Task SetMediaStreamPropertiesAsync(MediaStreamType streamType, IMediaEncodingProperties encodingProperties)
        {
            // Stop the preview
            StopPreview();

            // Apply desired stream properties
            await MediaCapture!.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoPreview, encodingProperties);

            // Restart the preview
            StartPreview();
        }

        /// <summary>
        /// Initializes the MediaCapture, starts preview.
        /// </summary>
        public async Task InitializeCameraAsync()
        {
            MediaCapture = new MediaCapture();
            MediaCapture.Failed += MediaCapture_Failed;

            try
            {
                var settings = new MediaCaptureInitializationSettings
                {
                    StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo,
                    SharingMode = MediaCaptureSharingMode.ExclusiveControl
                };
                await MediaCapture.InitializeAsync(settings);
                StartPreview();
                IsPreviewing = true;
            }
            catch (UnauthorizedAccessException)
            {
                MainPage.Current.NotifyUser("The app was denied access to the camera", NotifyType.ErrorMessage);
                await CleanupCameraAsync();
            }
        }

        private void StartPreview()
        {
            if (MediaCapture == null) return;

            var frameSource = MediaCapture.FrameSources.Values
                .FirstOrDefault(source => source.Info.MediaStreamType == MediaStreamType.VideoPreview)
                ?? MediaCapture.FrameSources.Values.FirstOrDefault();

            if (frameSource != null)
            {
                _mediaPlayer = new MediaPlayer
                {
                    RealTimePlayback = true,
                    AutoPlay = true
                };
                _mediaPlayer.Source = MediaSource.CreateFromMediaFrameSource(frameSource);
                _previewControl.SetMediaPlayer(_mediaPlayer);
            }
        }

        private void StopPreview()
        {
            _previewControl.SetMediaPlayer(null);
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Pause();
                _mediaPlayer.Source = null;
                _mediaPlayer.Dispose();
                _mediaPlayer = null;
            }
        }

        public async Task CleanupCameraAsync()
        {
            if (IsRecording)
            {
                await MediaCapture!.StopRecordAsync();
                IsRecording = false;
            }

            StopPreview();
            IsPreviewing = false;

            if (MediaCapture != null)
            {
                MediaCapture.Dispose();
                MediaCapture = null;
            }
        }

        private void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs e)
        {
            _dispatcherQueue.TryEnqueue(async () =>
            {
                MainPage.Current.NotifyUser("Preview stopped: " + e.Message, NotifyType.ErrorMessage);
                IsRecording = false;
                IsPreviewing = false;
                await CleanupCameraAsync();
            });
        }
    }

    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    }
}
