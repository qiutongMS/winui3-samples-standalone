using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.Devices;
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

namespace CameraAdvancedCapture;

public sealed partial class MainPage : Page
{
    private readonly SimpleOrientationSensor? _orientationSensor = SimpleOrientationSensor.GetDefault();
    private SimpleOrientation _deviceOrientation = SimpleOrientation.NotRotated;
    private DisplayOrientations _displayOrientation = DisplayOrientations.Landscape;

    private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");

    private StorageFolder? _captureFolder;

    private readonly DisplayRequest _displayRequest = new DisplayRequest();

    private MediaCapture? _mediaCapture;
    private bool _isInitialized;
    private bool _isPreviewing;

    private int _advancedCaptureMode = -1;

    private bool _mirroringPreview;
    private bool _externalCamera;

    private const double CERTAINTY_CAP = 0.7;

    private AdvancedPhotoCapture? _advancedCapture;
    private SceneAnalysisEffect? _sceneAnalysisEffect;
    private MediaPlayer? _mediaPlayer;

    public class AdvancedCaptureContext
    {
        public string CaptureFileName = string.Empty;
        public PhotoOrientation CaptureOrientation;
    }

    #region Constructor, lifecycle and navigation

    public MainPage()
    {
        this.InitializeComponent();

        NavigationCacheMode = NavigationCacheMode.Disabled;

        HdrImpactBar.Maximum = CERTAINTY_CAP;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        await SetupUiAsync();
        await InitializeCameraAsync();
    }

    protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        await CleanupCameraAsync();
        CleanupUi();
    }

    #endregion Constructor, lifecycle and navigation


    #region Event handlers

    private void OrientationSensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
    {
        if (args.Orientation != SimpleOrientation.Faceup && args.Orientation != SimpleOrientation.Facedown)
        {
            _deviceOrientation = args.Orientation;
            DispatcherQueue.TryEnqueue(() => UpdateControlOrientation());
        }
    }

    private void SceneAnalysisEffect_SceneAnalyzed(SceneAnalysisEffect sender, SceneAnalyzedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            HdrImpactBar.Value = Math.Min(CERTAINTY_CAP, args.ResultFrame.HighDynamicRange.Certainty);
            SceneTypeTextBlock.Text = "Scene: " + args.ResultFrame.AnalysisRecommendation;
        });
    }

    private async void AdvancedCapture_OptionalReferencePhotoCaptured(AdvancedPhotoCapture sender, OptionalReferencePhotoCapturedEventArgs args)
    {
        var context = args.Context as AdvancedCaptureContext;
        if (context == null) return;

        var referenceName = context.CaptureFileName.Replace(".jpg", "_Reference.jpg");

        using (var frame = args.Frame)
        {
            if (_captureFolder == null) return;
            var file = await _captureFolder.CreateFileAsync(referenceName, CreationCollisionOption.GenerateUniqueName);
            Debug.WriteLine("AdvancedCapture_OptionalReferencePhotoCaptured for " + context.CaptureFileName + ". Saving to " + file.Path);
            await ReencodeAndSavePhotoAsync(frame, file, context.CaptureOrientation);
        }
    }

    private void AdvancedCapture_AllPhotosCaptured(AdvancedPhotoCapture sender, object args)
    {
        Debug.WriteLine("AdvancedCapture_AllPhotosCaptured");
    }

    private async void PhotoButton_Click(object sender, RoutedEventArgs e)
    {
        await TakeAdvancedCapturePhotoAsync();
    }

    private async void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
    {
        Debug.WriteLine("MediaCapture_Failed: (0x{0:X}) {1}", errorEventArgs.Code, errorEventArgs.Message);
        await CleanupCameraAsync();
        DispatcherQueue.TryEnqueue(() => UpdateUi());
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
                cameraDevice = await FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Back);
            }
            catch (Exception ex)
            {
                ShowFallback(ex.Message);
                return;
            }

            if (cameraDevice == null)
            {
                Debug.WriteLine("No camera device found!");
                ShowFallback("No camera device was found on this machine.");
                return;
            }

            _mediaCapture = new MediaCapture();
            _mediaCapture.Failed += MediaCapture_Failed;

            var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };

            try
            {
                await _mediaCapture.InitializeAsync(settings);
                _isInitialized = true;
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("The app was denied access to the camera");
                ShowFallback("Camera access was denied. Please grant camera permission in Settings.");
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to initialize camera: " + ex.Message);
                ShowFallback(ex.Message);
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

                await StartPreviewAsync();

                if (_isPreviewing)
                {
                    await CreateSceneAnalysisEffectAsync();
                    await EnableAdvancedCaptureAsync();
                }
            }

            UpdateUi();
        }
    }

    private void ShowFallback(string errorMessage)
    {
        FallbackPanel.Visibility = Visibility.Visible;
        FallbackErrorText.Text = errorMessage;
        PreviewControl.Visibility = Visibility.Collapsed;
    }

    private async Task StartPreviewAsync()
    {
        _displayRequest.RequestActive();

        // Use MediaFrameSource to connect MediaCapture to MediaPlayerElement
        var frameSource = _mediaCapture!.FrameSources.Values.FirstOrDefault(
            s => s.Info.MediaStreamType == MediaStreamType.VideoPreview)
            ?? _mediaCapture.FrameSources.Values.FirstOrDefault(
            s => s.Info.MediaStreamType == MediaStreamType.VideoRecord);

        if (frameSource != null)
        {
            var mediaSource = MediaSource.CreateFromMediaFrameSource(frameSource);
            _mediaPlayer = new MediaPlayer { Source = mediaSource, AutoPlay = true };
            _mediaPlayer.IsVideoFrameServerEnabled = false;
            PreviewControl.SetMediaPlayer(_mediaPlayer);
        }
        else
        {
            // Fallback: start preview the traditional way (won't render in MediaPlayerElement, but keeps pipeline alive)
            await _mediaCapture.StartPreviewAsync();
        }

        PreviewControl.FlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        _isPreviewing = true;

        if (_isPreviewing)
        {
            await SetPreviewRotationAsync();
        }
    }

    private async Task SetPreviewRotationAsync()
    {
        if (_externalCamera) return;

        int rotationDegrees = ConvertDisplayOrientationToDegrees(_displayOrientation);

        if (_mirroringPreview)
        {
            rotationDegrees = (360 - rotationDegrees) % 360;
        }

        var props = _mediaCapture!.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
        props.Properties[RotationKey] = rotationDegrees;
        await _mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
    }

    private async Task StopPreviewAsync()
    {
        _isPreviewing = false;

        if (_mediaPlayer != null)
        {
            _mediaPlayer.Pause();
            PreviewControl.SetMediaPlayer(null);
            _mediaPlayer.Dispose();
            _mediaPlayer = null;
        }
        else
        {
            await _mediaCapture!.StopPreviewAsync();
        }

        DispatcherQueue.TryEnqueue(() =>
        {
            _displayRequest.RequestRelease();
        });
    }

    private async Task CreateSceneAnalysisEffectAsync()
    {
        var definition = new SceneAnalysisEffectDefinition();
        _sceneAnalysisEffect = (SceneAnalysisEffect)await _mediaCapture!.AddVideoEffectAsync(definition, MediaStreamType.VideoPreview);

        Debug.WriteLine("SA effect added to pipeline");

        _sceneAnalysisEffect.SceneAnalyzed += SceneAnalysisEffect_SceneAnalyzed;
        _sceneAnalysisEffect.HighDynamicRangeAnalyzer.Enabled = true;
    }

    private async Task CleanSceneAnalysisEffectAsync()
    {
        if (_sceneAnalysisEffect == null) return;

        _sceneAnalysisEffect.HighDynamicRangeAnalyzer.Enabled = false;
        _sceneAnalysisEffect.SceneAnalyzed -= SceneAnalysisEffect_SceneAnalyzed;

        await _mediaCapture!.RemoveEffectAsync(_sceneAnalysisEffect);

        Debug.WriteLine("SA effect removed from pipeline");
        _sceneAnalysisEffect = null;
    }

    private async Task EnableAdvancedCaptureAsync()
    {
        if (_advancedCapture != null) return;

        CycleAdvancedCaptureMode();

        _advancedCapture = await _mediaCapture!.PrepareAdvancedPhotoCaptureAsync(ImageEncodingProperties.CreateJpeg());

        Debug.WriteLine("Enabled Advanced Capture");

        _advancedCapture.AllPhotosCaptured += AdvancedCapture_AllPhotosCaptured;
        _advancedCapture.OptionalReferencePhotoCaptured += AdvancedCapture_OptionalReferencePhotoCaptured;
    }

    private void CycleAdvancedCaptureMode()
    {
        _advancedCaptureMode = (_advancedCaptureMode + 1) % _mediaCapture!.VideoDeviceController.AdvancedPhotoControl.SupportedModes.Count;

        var settings = new AdvancedPhotoCaptureSettings
        {
            Mode = _mediaCapture.VideoDeviceController.AdvancedPhotoControl.SupportedModes[_advancedCaptureMode]
        };

        _mediaCapture.VideoDeviceController.AdvancedPhotoControl.Configure(settings);
        ModeTextBlock.Text = _mediaCapture.VideoDeviceController.AdvancedPhotoControl.Mode.ToString();
    }

    private async Task DisableAdvancedCaptureAsync()
    {
        if (_advancedCapture == null) return;

        await _advancedCapture.FinishAsync();
        _advancedCapture = null;
        _advancedCaptureMode = -1;

        Debug.WriteLine("Disabled Advanced Capture");
    }

    private async Task TakeAdvancedCapturePhotoAsync()
    {
        PhotoButton.IsEnabled = false;
        CycleModeButton.IsEnabled = false;

        try
        {
            Debug.WriteLine("Taking Advanced Capture photo...");
            var photoOrientation = ConvertOrientationToPhotoOrientation(GetCameraOrientation());
            var fileName = String.Format("AdvancedCapturePhoto_{0}.jpg", DateTime.Now.ToString("HHmmss"));

            var context = new AdvancedCaptureContext { CaptureFileName = fileName, CaptureOrientation = photoOrientation };

            var capture = await _advancedCapture!.CaptureAsync(context);

            using (var frame = capture.Frame)
            {
                var file = await _captureFolder!.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
                Debug.WriteLine("Advanced Capture photo taken! Saving to " + file.Path);
                await ReencodeAndSavePhotoAsync(frame, file, photoOrientation);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception when taking an Advanced Capture photo: " + ex.ToString());
        }
        finally
        {
            UpdateUi();
        }
    }

    private async Task CleanupCameraAsync()
    {
        Debug.WriteLine("CleanupCameraAsync");

        if (_isInitialized)
        {
            if (_isPreviewing)
            {
                await StopPreviewAsync();
            }

            if (_advancedCapture != null)
            {
                await DisableAdvancedCaptureAsync();
            }

            if (_sceneAnalysisEffect != null)
            {
                await CleanSceneAnalysisEffectAsync();
            }

            _isInitialized = false;
        }

        if (_mediaCapture != null)
        {
            _mediaCapture.Failed -= MediaCapture_Failed;
            _mediaCapture.Dispose();
            _mediaCapture = null;
        }
    }

    #endregion MediaCapture methods


    #region Helper methods

    private async Task SetupUiAsync()
    {
        _displayOrientation = DisplayOrientations.Landscape;

        if (_orientationSensor != null)
        {
            _deviceOrientation = _orientationSensor.GetCurrentOrientation();
        }

        RegisterEventHandlers();

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

    private void CleanupUi()
    {
        UnregisterEventHandlers();
    }

    private void UpdateUi()
    {
        PhotoButton.IsEnabled = _isPreviewing;
        CycleModeButton.IsEnabled = _isPreviewing;
    }

    private void RegisterEventHandlers()
    {
        if (_orientationSensor != null)
        {
            _orientationSensor.OrientationChanged += OrientationSensor_OrientationChanged;
            UpdateControlOrientation();
        }

        CycleModeButton.Click += (sender, args) => CycleAdvancedCaptureMode();
    }

    private void UnregisterEventHandlers()
    {
        if (_orientationSensor != null)
        {
            _orientationSensor.OrientationChanged -= OrientationSensor_OrientationChanged;
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

    #endregion Helper methods


    #region Rotation helpers

    private SimpleOrientation GetCameraOrientation()
    {
        if (_externalCamera)
        {
            return SimpleOrientation.NotRotated;
        }

        var result = _deviceOrientation;

        if (_mirroringPreview)
        {
            switch (result)
            {
                case SimpleOrientation.Rotated90DegreesCounterclockwise:
                    return SimpleOrientation.Rotated270DegreesCounterclockwise;
                case SimpleOrientation.Rotated270DegreesCounterclockwise:
                    return SimpleOrientation.Rotated90DegreesCounterclockwise;
            }
        }

        return result;
    }

    private static int ConvertDeviceOrientationToDegrees(SimpleOrientation orientation)
    {
        switch (orientation)
        {
            case SimpleOrientation.Rotated90DegreesCounterclockwise:
                return 90;
            case SimpleOrientation.Rotated180DegreesCounterclockwise:
                return 180;
            case SimpleOrientation.Rotated270DegreesCounterclockwise:
                return 270;
            case SimpleOrientation.NotRotated:
            default:
                return 0;
        }
    }

    private static int ConvertDisplayOrientationToDegrees(DisplayOrientations orientation)
    {
        switch (orientation)
        {
            case DisplayOrientations.Portrait:
                return 90;
            case DisplayOrientations.LandscapeFlipped:
                return 180;
            case DisplayOrientations.PortraitFlipped:
                return 270;
            case DisplayOrientations.Landscape:
            default:
                return 0;
        }
    }

    private static PhotoOrientation ConvertOrientationToPhotoOrientation(SimpleOrientation orientation)
    {
        switch (orientation)
        {
            case SimpleOrientation.Rotated90DegreesCounterclockwise:
                return PhotoOrientation.Rotate90;
            case SimpleOrientation.Rotated180DegreesCounterclockwise:
                return PhotoOrientation.Rotate180;
            case SimpleOrientation.Rotated270DegreesCounterclockwise:
                return PhotoOrientation.Rotate270;
            case SimpleOrientation.NotRotated:
            default:
                return PhotoOrientation.Normal;
        }
    }

    private void UpdateControlOrientation()
    {
        int device = ConvertDeviceOrientationToDegrees(_deviceOrientation);
        int display = ConvertDisplayOrientationToDegrees(_displayOrientation);

        var angle = (360 + display + device) % 360;

        var transform = new RotateTransform { Angle = angle };

        PhotoButton.RenderTransform = transform;
        CycleModeButton.RenderTransform = transform;

        HdrImpactBar.FlowDirection = (angle == 180 || angle == 270) ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
    }

    #endregion Rotation helpers
}
