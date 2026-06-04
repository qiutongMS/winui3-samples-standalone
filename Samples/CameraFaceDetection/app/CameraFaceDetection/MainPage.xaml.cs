using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.Core;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.System.Display;
using Microsoft.UI;
using Microsoft.UI.Dispatching;

namespace CameraFaceDetection;

public sealed partial class MainPage : Page
{
    private StorageFolder? _captureFolder;
    private readonly DisplayRequest _displayRequest = new();

    private MediaCapture? _mediaCapture;
    private MediaPlayer? _mediaPlayer;
    private bool _isInitialized;
    private bool _isRecording;

    private bool _mirroringPreview;

    private FaceDetectionEffect? _faceDetectionEffect;

    public MainPage()
    {
        this.InitializeComponent();
        this.Loaded += MainPage_Loaded;
        this.Unloaded += MainPage_Unloaded;
    }

    private async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        await SetupUiAsync();
        await InitializeCameraAsync();
    }

    private async void MainPage_Unloaded(object sender, RoutedEventArgs e)
    {
        await CleanupCameraAsync();
    }

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

    private async void FaceDetectionButton_Click(object sender, RoutedEventArgs e)
    {
        if (_faceDetectionEffect == null || !_faceDetectionEffect.Enabled)
        {
            FacesCanvas.Children.Clear();
            await CreateFaceDetectionEffectAsync();
        }
        else
        {
            await CleanUpFaceDetectionEffectAsync();
        }

        UpdateCaptureControls();
    }

    private async void MediaCapture_RecordLimitationExceeded(MediaCapture sender)
    {
        await StopRecordingAsync();
        DispatcherQueue.TryEnqueue(() => UpdateCaptureControls());
    }

    private async void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
    {
        Debug.WriteLine("MediaCapture_Failed: (0x{0:X}) {1}", errorEventArgs.Code, errorEventArgs.Message);
        await CleanupCameraAsync();
        DispatcherQueue.TryEnqueue(() => UpdateCaptureControls());
    }

    private void FaceDetectionEffect_FaceDetected(FaceDetectionEffect sender, FaceDetectedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(() => HighlightDetectedFaces(args.ResultFrame.DetectedFaces));
    }

    #endregion Event handlers

    #region MediaCapture methods

    private async Task InitializeCameraAsync()
    {
        Debug.WriteLine("InitializeCameraAsync");

        if (_mediaCapture == null)
        {
            DeviceInformation? cameraDevice;
            try
            {
                cameraDevice = await FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Front);
            }
            catch (Exception ex)
            {
                ShowFallback(ex.Message);
                return;
            }

            if (cameraDevice == null)
            {
                ShowFallback("No camera device found.");
                return;
            }

            _mediaCapture = new MediaCapture();
            _mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;
            _mediaCapture.Failed += MediaCapture_Failed;

            var settings = new MediaCaptureInitializationSettings
            {
                VideoDeviceId = cameraDevice.Id,
                StreamingCaptureMode = StreamingCaptureMode.Video,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu
            };

            // Try to get a frame source group for this device to enable MediaPlayerElement preview
            try
            {
                var sourceGroups = await MediaFrameSourceGroup.FindAllAsync();
                var matchingGroup = sourceGroups.FirstOrDefault(g =>
                    g.SourceInfos.Any(si => si.DeviceInformation?.Id == cameraDevice.Id));
                if (matchingGroup != null)
                {
                    settings.SourceGroup = matchingGroup;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not find matching source group: " + ex.Message);
            }

            try
            {
                await _mediaCapture.InitializeAsync(settings);
                _isInitialized = true;
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowFallback("The app was denied access to the camera. " + ex.Message);
                return;
            }
            catch (Exception ex)
            {
                ShowFallback("Failed to initialize camera: " + ex.Message);
                return;
            }

            if (_isInitialized)
            {
                if (cameraDevice.EnclosureLocation == null || cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown)
                {
                    _mirroringPreview = false;
                }
                else
                {
                    _mirroringPreview = (cameraDevice.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
                }

                await StartPreviewAsync();
                UpdateCaptureControls();
            }
        }
    }

    private async Task StartPreviewAsync()
    {
        _displayRequest.RequestActive();

        // Use MediaFrameSource-based preview through MediaPlayerElement
        var frameSource = _mediaCapture!.FrameSources.Values.FirstOrDefault(
            source => source.Info.MediaStreamType == MediaStreamType.VideoPreview)
            ?? _mediaCapture.FrameSources.Values.FirstOrDefault(
                source => source.Info.MediaStreamType == MediaStreamType.VideoRecord);

        if (frameSource != null)
        {
            var mediaSource = MediaSource.CreateFromMediaFrameSource(frameSource);
            _mediaPlayer = new MediaPlayer
            {
                AutoPlay = true,
                RealTimePlayback = true
            };
            _mediaPlayer.Source = mediaSource;
            PreviewPlayerElement.SetMediaPlayer(_mediaPlayer);
        }
        else
        {
            // Fallback: start preview without display
            await _mediaCapture.StartPreviewAsync();
        }

        if (_mirroringPreview)
        {
            PreviewPlayerElement.RenderTransform = new ScaleTransform { ScaleX = -1 };
            PreviewPlayerElement.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
        }
    }

    private async Task StopPreviewAsync()
    {
        if (_mediaPlayer != null)
        {
            _mediaPlayer.Pause();
            _mediaPlayer.Source = null;
            PreviewPlayerElement.SetMediaPlayer(null);
            _mediaPlayer.Dispose();
            _mediaPlayer = null;
        }
        else if (_mediaCapture != null)
        {
            try
            {
                await _mediaCapture.StopPreviewAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("StopPreviewAsync exception: " + ex.Message);
            }
        }

        DispatcherQueue.TryEnqueue(() =>
        {
            _displayRequest.RequestRelease();
        });
    }

    private async Task CreateFaceDetectionEffectAsync()
    {
        var definition = new FaceDetectionEffectDefinition();
        definition.SynchronousDetectionEnabled = false;
        definition.DetectionMode = FaceDetectionMode.HighPerformance;

        _faceDetectionEffect = (FaceDetectionEffect)await _mediaCapture!.AddVideoEffectAsync(definition, MediaStreamType.VideoPreview);
        _faceDetectionEffect.FaceDetected += FaceDetectionEffect_FaceDetected;
        _faceDetectionEffect.DesiredDetectionInterval = TimeSpan.FromMilliseconds(33);
        _faceDetectionEffect.Enabled = true;
    }

    private async Task CleanUpFaceDetectionEffectAsync()
    {
        if (_faceDetectionEffect != null)
        {
            _faceDetectionEffect.Enabled = false;
            _faceDetectionEffect.FaceDetected -= FaceDetectionEffect_FaceDetected;
            await _mediaCapture!.RemoveEffectAsync(_faceDetectionEffect);
            _faceDetectionEffect = null;
        }
    }

    private async Task TakePhotoAsync()
    {
        if (_mediaCapture == null || _captureFolder == null) return;

        VideoButton.IsEnabled = _mediaCapture.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported;
        VideoButton.Opacity = VideoButton.IsEnabled ? 1 : 0;

        var stream = new InMemoryRandomAccessStream();

        Debug.WriteLine("Taking photo...");
        await _mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);

        try
        {
            var file = await _captureFolder.CreateFileAsync("SimplePhoto.jpg", CreationCollisionOption.GenerateUniqueName);
            Debug.WriteLine("Photo taken! Saving to " + file.Path);
            await ReencodeAndSavePhotoAsync(stream, file, PhotoOrientation.Normal);
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
        if (_mediaCapture == null || _captureFolder == null) return;

        try
        {
            var videoFile = await _captureFolder.CreateFileAsync("SimpleVideo.mp4", CreationCollisionOption.GenerateUniqueName);
            var encodingProfile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto);

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

            if (_faceDetectionEffect != null)
            {
                await CleanUpFaceDetectionEffectAsync();
            }

            await StopPreviewAsync();

            _isInitialized = false;
        }

        if (_mediaCapture != null)
        {
            _mediaCapture.RecordLimitationExceeded -= MediaCapture_RecordLimitationExceeded;
            _mediaCapture.Failed -= MediaCapture_Failed;
            _mediaCapture.Dispose();
            _mediaCapture = null;
        }
    }

    #endregion MediaCapture methods

    #region Helper functions

    private async Task SetupUiAsync()
    {
        try
        {
            var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            _captureFolder = picturesLibrary.SaveFolder ?? ApplicationData.Current.LocalFolder;
        }
        catch
        {
            _captureFolder = ApplicationData.Current.LocalFolder;
        }
    }

    private void UpdateCaptureControls()
    {
        PhotoButton.IsEnabled = _isInitialized;
        VideoButton.IsEnabled = _isInitialized;
        FaceDetectionButton.IsEnabled = _isInitialized;

        FaceDetectionDisabledIcon.Visibility = (_faceDetectionEffect == null || !_faceDetectionEffect.Enabled) ? Visibility.Visible : Visibility.Collapsed;
        FaceDetectionEnabledIcon.Visibility = (_faceDetectionEffect != null && _faceDetectionEffect.Enabled) ? Visibility.Visible : Visibility.Collapsed;

        FacesCanvas.Visibility = (_faceDetectionEffect != null && _faceDetectionEffect.Enabled) ? Visibility.Visible : Visibility.Collapsed;

        StartRecordingIcon.Visibility = _isRecording ? Visibility.Collapsed : Visibility.Visible;
        StopRecordingIcon.Visibility = _isRecording ? Visibility.Visible : Visibility.Collapsed;

        if (_isInitialized && !_mediaCapture!.MediaCaptureSettings.ConcurrentRecordAndPhotoSupported)
        {
            PhotoButton.IsEnabled = !_isRecording;
            PhotoButton.Opacity = PhotoButton.IsEnabled ? 1 : 0;
        }
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

    private void ShowFallback(string detail)
    {
        FallbackPanel.Visibility = Visibility.Visible;
        FallbackDetail.Text = detail;
    }

    #endregion Helper functions

    #region Face detection helpers

    private void HighlightDetectedFaces(IReadOnlyList<DetectedFace> faces)
    {
        FacesCanvas.Children.Clear();

        for (int i = 0; i < faces.Count; i++)
        {
            Rectangle faceBoundingBox = ConvertPreviewToUiRectangle(faces[i].FaceBox);
            faceBoundingBox.StrokeThickness = 2;
            faceBoundingBox.Stroke = (i == 0 ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.DeepSkyBlue));
            FacesCanvas.Children.Add(faceBoundingBox);
        }

        SetFacesCanvasRotation();
    }

    private Rectangle ConvertPreviewToUiRectangle(BitmapBounds faceBoxInPreviewCoordinates)
    {
        var result = new Rectangle();

        var frameSource = _mediaCapture?.FrameSources.Values.FirstOrDefault(
            source => source.Info.MediaStreamType == MediaStreamType.VideoPreview)
            ?? _mediaCapture?.FrameSources.Values.FirstOrDefault(
                source => source.Info.MediaStreamType == MediaStreamType.VideoRecord);

        if (frameSource == null) return result;

        var format = frameSource.CurrentFormat?.VideoFormat;
        if (format == null || format.Width == 0 || format.Height == 0) return result;

        double streamWidth = format.Width;
        double streamHeight = format.Height;

        var previewInUI = GetPreviewStreamRectInControl(streamWidth, streamHeight);

        result.Width = (faceBoxInPreviewCoordinates.Width / streamWidth) * previewInUI.Width;
        result.Height = (faceBoxInPreviewCoordinates.Height / streamHeight) * previewInUI.Height;

        var x = (faceBoxInPreviewCoordinates.X / streamWidth) * previewInUI.Width;
        var y = (faceBoxInPreviewCoordinates.Y / streamHeight) * previewInUI.Height;
        Canvas.SetLeft(result, x);
        Canvas.SetTop(result, y);

        return result;
    }

    private void SetFacesCanvasRotation()
    {
        var frameSource = _mediaCapture?.FrameSources.Values.FirstOrDefault(
            source => source.Info.MediaStreamType == MediaStreamType.VideoPreview)
            ?? _mediaCapture?.FrameSources.Values.FirstOrDefault(
                source => source.Info.MediaStreamType == MediaStreamType.VideoRecord);

        if (frameSource == null) return;

        var format = frameSource.CurrentFormat?.VideoFormat;
        if (format == null || format.Width == 0 || format.Height == 0) return;

        var previewArea = GetPreviewStreamRectInControl(format.Width, format.Height);

        FacesCanvas.Width = previewArea.Width;
        FacesCanvas.Height = previewArea.Height;

        Canvas.SetLeft(FacesCanvas, previewArea.X);
        Canvas.SetTop(FacesCanvas, previewArea.Y);

        FacesCanvas.FlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
    }

    private Rect GetPreviewStreamRectInControl(double streamWidth, double streamHeight)
    {
        var result = new Rect();

        double controlWidth = PreviewPlayerElement.ActualWidth;
        double controlHeight = PreviewPlayerElement.ActualHeight;

        if (controlWidth < 1 || controlHeight < 1 || streamWidth == 0 || streamHeight == 0)
        {
            return result;
        }

        result.Width = controlWidth;
        result.Height = controlHeight;

        if ((controlWidth / controlHeight > streamWidth / streamHeight))
        {
            var scale = controlHeight / streamHeight;
            var scaledWidth = streamWidth * scale;
            result.X = (controlWidth - scaledWidth) / 2.0;
            result.Width = scaledWidth;
        }
        else
        {
            var scale = controlWidth / streamWidth;
            var scaledHeight = streamHeight * scale;
            result.Y = (controlHeight - scaledHeight) / 2.0;
            result.Height = scaledHeight;
        }

        return result;
    }

    #endregion Face detection helpers
}
