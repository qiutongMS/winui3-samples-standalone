//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.System.Display;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

namespace CameraStarterKit;

public sealed partial class MainPage : Page
{
    private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");

    private StorageFolder? _captureFolder = null;
    private readonly DisplayRequest _displayRequest = new DisplayRequest();

    private MediaCapture? _mediaCapture;
    private bool _isInitialized;
    private bool _isPreviewing;
    private bool _isRecording;

    private bool _isActivePage;
    private bool _isUIActive;
    private Task _setupTask = Task.CompletedTask;

    private bool _mirroringPreview;
    private bool _externalCamera;

    private CameraRotationHelper? _rotationHelper;
    private readonly DispatcherQueue _dispatcherQueue;
    private MediaPlayer? _mediaPlayer;

    #region Constructor, lifecycle and navigation

    public MainPage()
    {
        this.InitializeComponent();
        _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        NavigationCacheMode = NavigationCacheMode.Disabled;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        _isActivePage = true;
        await SetUpBasedOnStateAsync();
    }

    protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        _isActivePage = false;
        await SetUpBasedOnStateAsync();
    }

    #endregion Constructor, lifecycle and navigation


    #region Event handlers

    private async void PhotoButton_Click(object sender, RoutedEventArgs e)
    {
        await TakePhotoAsync();
    }

    private async void VideoButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_isRecording)
        {
            await StartRecordingAsync();
        }
        else
        {
            await StopRecordingAsync();
        }

        UpdateCaptureControls();
    }

    private async void MediaCapture_RecordLimitationExceeded(MediaCapture sender)
    {
        await StopRecordingAsync();

        _dispatcherQueue.TryEnqueue(() => UpdateCaptureControls());
    }

    private async void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
    {
        Debug.WriteLine("MediaCapture_Failed: (0x{0:X}) {1}", errorEventArgs.Code, errorEventArgs.Message);

        await CleanupCameraAsync();

        _dispatcherQueue.TryEnqueue(() => UpdateCaptureControls());
    }

    #endregion Event handlers


    #region MediaCapture methods

    private async Task InitializeCameraAsync()
    {
        Debug.WriteLine("InitializeCameraAsync");

        if (_mediaCapture == null)
        {
            var cameraDevice = await FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Back);

            if (cameraDevice == null)
            {
                Debug.WriteLine("No camera device found!");
                ShowError("This sample requires a camera device that is not available on this machine.", null);
                return;
            }

            _mediaCapture = new MediaCapture();

            _mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;
            _mediaCapture.Failed += MediaCapture_Failed;

            var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

            try
            {
                await _mediaCapture.InitializeAsync(settings);
                _isInitialized = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine("The app was denied access to the camera");
                ShowError("The app was denied access to the camera.", ex.Message);
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception initializing camera: " + ex.ToString());
                ShowError("Failed to initialize camera.", ex.Message);
                return;
            }

            if (_isInitialized)
            {
                if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
                {
                    _externalCamera = true;
                }
                else
                {
                    _externalCamera = false;
                    _mirroringPreview = (cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
                }

                _rotationHelper = new CameraRotationHelper(cameraDevice.EnclosureLocation!);
                _rotationHelper.OrientationChanged += RotationHelper_OrientationChanged;

                await StartPreviewAsync();

                UpdateCaptureControls();
            }
        }
    }

    private async void RotationHelper_OrientationChanged(object? sender, bool updatePreview)
    {
        if (updatePreview)
        {
            await SetPreviewRotationAsync();
        }
        _dispatcherQueue.TryEnqueue(() => UpdateButtonOrientation());
    }

    private void UpdateButtonOrientation()
    {
        if (_rotationHelper == null) return;

        var angle = CameraRotationHelper.ConvertSimpleOrientationToClockwiseDegrees(_rotationHelper.GetUIOrientation());
        var transform = new RotateTransform { Angle = angle };

        PhotoButton.RenderTransform = transform;
        VideoButton.RenderTransform = transform;
    }

    private async Task StartPreviewAsync()
    {
        _displayRequest.RequestActive();

        // Use MediaFrameSource to feed preview into MediaPlayerElement
        var frameSource = _mediaCapture!.FrameSources.Values.FirstOrDefault(
            source => source.Info.MediaStreamType == MediaStreamType.VideoPreview)
            ?? _mediaCapture.FrameSources.Values.FirstOrDefault(
                source => source.Info.MediaStreamType == MediaStreamType.VideoRecord);

        if (frameSource != null)
        {
            _mediaPlayer = new MediaPlayer
            {
                AutoPlay = true,
                RealTimePlayback = true
            };
            _mediaPlayer.Source = MediaSource.CreateFromMediaFrameSource(frameSource);
            PreviewControl.SetMediaPlayer(_mediaPlayer);
        }

        if (_mirroringPreview)
        {
            PreviewControl.FlowDirection = FlowDirection.RightToLeft;
        }

        await _mediaCapture.StartPreviewAsync();
        _isPreviewing = true;

        if (_isPreviewing)
        {
            await SetPreviewRotationAsync();
        }
    }

    private async Task SetPreviewRotationAsync()
    {
        if (_externalCamera) return;
        if (_mediaCapture == null || _rotationHelper == null) return;

        var rotation = _rotationHelper.GetCameraPreviewOrientation();
        var props = _mediaCapture.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
        props.Properties.Add(RotationKey, CameraRotationHelper.ConvertSimpleOrientationToClockwiseDegrees(rotation));
        await _mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
    }

    private async Task StopPreviewAsync()
    {
        _isPreviewing = false;
        await _mediaCapture!.StopPreviewAsync();

        _dispatcherQueue.TryEnqueue(() =>
        {
            PreviewControl.SetMediaPlayer(null);
            if (_mediaPlayer != null)
            {
                _mediaPlayer.Dispose();
                _mediaPlayer = null;
            }
            _displayRequest.RequestRelease();
        });
    }

    private async Task TakePhotoAsync()
    {
        if (_mediaCapture == null || _captureFolder == null || _rotationHelper == null) return;

        VideoButton.IsEnabled = _mediaCapture.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported;
        VideoButton.Opacity = VideoButton.IsEnabled ? 1 : 0;

        var stream = new InMemoryRandomAccessStream();

        Debug.WriteLine("Taking photo...");
        await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

        try
        {
            var file = await _captureFolder.CreateFileAsync("SimplePhoto.jpg", CreationCollisionOption.GenerateUniqueName);
            Debug.WriteLine("Photo taken! Saving to " + file.Path);

            var photoOrientation = CameraRotationHelper.ConvertSimpleOrientationToPhotoOrientation(_rotationHelper.GetCameraCaptureOrientation());

            await ReencodeAndSavePhotoAsync(stream, file, photoOrientation);
            Debug.WriteLine("Photo saved!");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception when taking a photo: " + ex.ToString());
        }

        VideoButton.IsEnabled = true;
        VideoButton.Opacity = 1;
    }

    private async Task StartRecordingAsync()
    {
        if (_mediaCapture == null || _captureFolder == null || _rotationHelper == null) return;

        try
        {
            var videoFile = await _captureFolder.CreateFileAsync("SimpleVideo.mp4", CreationCollisionOption.GenerateUniqueName);

            var encodingProfile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);

            var rotationAngle = CameraRotationHelper.ConvertSimpleOrientationToClockwiseDegrees(_rotationHelper.GetCameraCaptureOrientation());
            encodingProfile.Video.Properties.Add(RotationKey, PropertyValue.CreateInt32(rotationAngle));

            Debug.WriteLine("Starting recording to " + videoFile.Path);

            await _mediaCapture.StartRecordToStorageFileAsync(encodingProfile, videoFile);
            _isRecording = true;

            Debug.WriteLine("Started recording!");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception when starting video recording: " + ex.ToString());
        }
    }

    private async Task StopRecordingAsync()
    {
        if (_mediaCapture == null) return;

        Debug.WriteLine("Stopping recording...");

        _isRecording = false;
        await _mediaCapture.StopRecordAsync();

        Debug.WriteLine("Stopped recording!");
    }

    private async Task CleanupCameraAsync()
    {
        Debug.WriteLine("CleanupCameraAsync");

        if (_isInitialized)
        {
            if (_isRecording)
            {
                await StopRecordingAsync();
            }

            if (_isPreviewing)
            {
                await StopPreviewAsync();
            }

            _isInitialized = false;
        }

        if (_mediaCapture != null)
        {
            _mediaCapture.RecordLimitationExceeded -= MediaCapture_RecordLimitationExceeded;
            _mediaCapture.Failed -= MediaCapture_Failed;
            _mediaCapture.Dispose();
            _mediaCapture = null;
        }

        if (_rotationHelper != null)
        {
            _rotationHelper.OrientationChanged -= RotationHelper_OrientationChanged;
            _rotationHelper = null;
        }
    }

    #endregion MediaCapture methods


    #region Helper functions

    private async Task SetUpBasedOnStateAsync()
    {
        while (!_setupTask.IsCompleted)
        {
            await _setupTask;
        }

        bool wantUIActive = _isActivePage;

        if (_isUIActive != wantUIActive)
        {
            _isUIActive = wantUIActive;

            Func<Task> setupAsync = async () =>
            {
                if (wantUIActive)
                {
                    await SetupUiAsync();
                    await InitializeCameraAsync();
                }
                else
                {
                    await CleanupCameraAsync();
                }
            };
            _setupTask = setupAsync();
        }

        await _setupTask;
    }

    private async Task SetupUiAsync()
    {
        var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
        _captureFolder = picturesLibrary.SaveFolder ?? ApplicationData.Current.LocalFolder;
    }

    private void UpdateCaptureControls()
    {
        PhotoButton.IsEnabled = _isPreviewing;
        VideoButton.IsEnabled = _isPreviewing;

        StartRecordingIcon.Visibility = _isRecording ? Visibility.Collapsed : Visibility.Visible;
        StopRecordingIcon.Visibility = _isRecording ? Visibility.Visible : Visibility.Collapsed;

        if (_isInitialized && _mediaCapture != null && !_mediaCapture.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
        {
            PhotoButton.IsEnabled = !_isRecording;
            PhotoButton.Opacity = PhotoButton.IsEnabled ? 1 : 0;
        }
    }

    private void ShowError(string message, string? detail)
    {
        _dispatcherQueue.TryEnqueue(() =>
        {
            ErrorPanel.Visibility = Visibility.Visible;
            ErrorMessageText.Text = message;
            ErrorDetailText.Text = detail ?? string.Empty;
            PhotoButton.IsEnabled = false;
            VideoButton.IsEnabled = false;
        });
    }

    private static async Task<DeviceInformation?> FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel desiredPanel)
    {
        var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

        DeviceInformation? desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);

        return desiredDevice ?? allVideoDevices.FirstOrDefault();
    }

    private static async Task ReencodeAndSavePhotoAsync(IRandomAccessStream stream, StorageFile file, PhotoOrientation photoOrientation)
    {
        using (var inputStream = stream)
        {
            var decoder = await BitmapDecoder.CreateAsync(inputStream);

            using (var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateForTranscodingAsync(outputStream, decoder);

                var properties = new BitmapPropertySet { { "System.Photo.Orientation", new BitmapTypedValue(photoOrientation, PropertyType.UInt16) } };

                await encoder.BitmapProperties.SetPropertiesAsync(properties);
                await encoder.FlushAsync();
            }
        }
    }

    #endregion Helper functions
}
