using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.System.Display;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;

namespace CameraGetPreviewFrame;

[ComImport]
[Guid("5b0d3235-4dba-4d44-865e-8f1d0e4fd04d")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
unsafe interface IMemoryBufferByteAccess
{
    void GetBuffer(out byte* buffer, out uint capacity);
}

public sealed partial class MainPage : Page
{
    private static readonly Guid RotationKey = new Guid("C380465D-2271-428C-9B83-ECEA3B4A85C1");

    private StorageFolder? _captureFolder;
    private readonly DisplayRequest _displayRequest = new DisplayRequest();

    private MediaCapture? _mediaCapture;
    private bool _isInitialized;
    private bool _isPreviewing;
    private static readonly SemaphoreSlim _mediaCaptureLifeLock = new SemaphoreSlim(1);

    private bool _mirroringPreview;
    private bool _externalCamera;

    private CancellationTokenSource? _previewCancellation;

    public MainPage()
    {
        this.InitializeComponent();
        NavigationCacheMode = NavigationCacheMode.Required;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        await InitializeCameraAsync();
    }

    protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        await CleanupCameraAsync();
    }

    #region Event handlers

    private async void GetPreviewFrameButton_Click(object sender, RoutedEventArgs e)
    {
        if (!_isPreviewing) return;

        if ((ShowFrameCheckBox.IsChecked == true) || (SaveFrameCheckBox.IsChecked == true))
        {
            await GetPreviewFrameAsSoftwareBitmapAsync();
        }
        else
        {
            await GetPreviewFrameAsD3DSurfaceAsync();
        }
    }

    private async void MediaCapture_Failed(MediaCapture sender, MediaCaptureFailedEventArgs errorEventArgs)
    {
        Debug.WriteLine("MediaCapture_Failed: (0x{0:X}) {1}", errorEventArgs.Code, errorEventArgs.Message);

        await CleanupCameraAsync();

        DispatcherQueue.TryEnqueue(() => GetPreviewFrameButton.IsEnabled = _isPreviewing);
    }

    #endregion Event handlers

    #region MediaCapture methods

    private async Task InitializeCameraAsync()
    {
        Debug.WriteLine("InitializeCameraAsync");

        await _mediaCaptureLifeLock.WaitAsync();

        if (_mediaCapture == null)
        {
            var cameraDevice = await FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Back);

            if (cameraDevice == null)
            {
                Debug.WriteLine("No camera device found!");
                _mediaCaptureLifeLock.Release();
                DispatcherQueue.TryEnqueue(() =>
                {
                    NoCameraMessage.Text = "This sample requires a camera device that is not available on this machine.";
                    NoCameraMessage.Visibility = Visibility.Visible;
                });
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
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine("The app was denied access to the camera");
                _mediaCaptureLifeLock.Release();
                DispatcherQueue.TryEnqueue(() =>
                {
                    NoCameraMessage.Text = $"Camera access was denied.\n{ex.Message}";
                    NoCameraMessage.Visibility = Visibility.Visible;
                });
                return;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Camera initialization failed: {ex.Message}");
                _mediaCaptureLifeLock.Release();
                DispatcherQueue.TryEnqueue(() =>
                {
                    NoCameraMessage.Text = $"This sample requires a camera device that is not available on this machine.\n{ex.Message}";
                    NoCameraMessage.Visibility = Visibility.Visible;
                });
                return;
            }
            finally
            {
                if (!_isInitialized)
                {
                    // Only release if we haven't already (early returns above release before returning)
                }
                else
                {
                    _mediaCaptureLifeLock.Release();
                }
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
        }
        else
        {
            _mediaCaptureLifeLock.Release();
        }
    }

    private async Task StartPreviewAsync()
    {
        Debug.WriteLine("StartPreviewAsync");

        _displayRequest.RequestActive();

        // WinUI 3 does not have CaptureElement; mirror the preview via FlowDirection on the Image
        PreviewImage.FlowDirection = _mirroringPreview ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        await _mediaCapture!.StartPreviewAsync();
        _isPreviewing = true;

        if (_isPreviewing && !_externalCamera)
        {
            await SetPreviewRotationAsync();
        }

        GetPreviewFrameButton.IsEnabled = _isPreviewing;

        // Start continuous frame-grabbing loop to render preview to Image control
        _previewCancellation = new CancellationTokenSource();
        _ = RenderPreviewFramesAsync(_previewCancellation.Token);
    }

    private async Task RenderPreviewFramesAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _isPreviewing)
        {
            try
            {
                var previewProperties = _mediaCapture!.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;
                if (previewProperties == null) break;

                var videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)previewProperties.Width, (int)previewProperties.Height);

                using (var currentFrame = await _mediaCapture.GetPreviewFrameAsync(videoFrame))
                {
                    var previewBitmap = currentFrame.SoftwareBitmap;
                    var sbSource = new SoftwareBitmapSource();
                    await sbSource.SetBitmapAsync(previewBitmap);
                    PreviewImage.Source = sbSource;
                }

                // ~30 fps target
                await Task.Delay(33, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Preview frame render error: {ex.Message}");
                break;
            }
        }
    }

    private async Task SetPreviewRotationAsync()
    {
        if (_externalCamera) return;

        // Desktop apps are typically landscape; set 0 degrees rotation
        int rotationDegrees = 0;

        var props = _mediaCapture!.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview);
        props.Properties.Add(RotationKey, rotationDegrees);
        await _mediaCapture.SetEncodingPropertiesAsync(MediaStreamType.VideoPreview, props, null);
    }

    private async Task StopPreviewAsync()
    {
        _isPreviewing = false;

        // Cancel the preview rendering loop
        _previewCancellation?.Cancel();
        _previewCancellation?.Dispose();
        _previewCancellation = null;

        await _mediaCapture!.StopPreviewAsync();

        DispatcherQueue.TryEnqueue(() =>
        {
            PreviewImage.Source = null;
            _displayRequest.RequestRelease();
            GetPreviewFrameButton.IsEnabled = _isPreviewing;
        });
    }

    private async Task GetPreviewFrameAsSoftwareBitmapAsync()
    {
        var previewProperties = _mediaCapture!.VideoDeviceController.GetMediaStreamProperties(MediaStreamType.VideoPreview) as VideoEncodingProperties;

        var videoFrame = new VideoFrame(BitmapPixelFormat.Bgra8, (int)previewProperties!.Width, (int)previewProperties.Height);

        using (var currentFrame = await _mediaCapture.GetPreviewFrameAsync(videoFrame))
        {
            SoftwareBitmap previewFrame = currentFrame.SoftwareBitmap;

            FrameInfoTextBlock.Text = String.Format("{0}x{1} {2}", previewFrame.PixelWidth, previewFrame.PixelHeight, previewFrame.BitmapPixelFormat);

            if (GreenEffectCheckBox.IsChecked == true)
            {
                ApplyGreenFilter(previewFrame);
            }

            if (ShowFrameCheckBox.IsChecked == true)
            {
                var sbSource = new SoftwareBitmapSource();
                await sbSource.SetBitmapAsync(previewFrame);
                PreviewFrameImage.Source = sbSource;
            }

            if (SaveFrameCheckBox.IsChecked == true)
            {
                var file = await _captureFolder!.CreateFileAsync("PreviewFrame.jpg", CreationCollisionOption.GenerateUniqueName);
                Debug.WriteLine("Saving preview frame to " + file.Path);
                await SaveSoftwareBitmapAsync(previewFrame, file);
            }
        }
    }

    private async Task GetPreviewFrameAsD3DSurfaceAsync()
    {
        using (var currentFrame = await _mediaCapture!.GetPreviewFrameAsync())
        {
            if (currentFrame.Direct3DSurface != null)
            {
                var surface = currentFrame.Direct3DSurface;
                FrameInfoTextBlock.Text = String.Format("{0}x{1} {2}", surface.Description.Width, surface.Description.Height, surface.Description.Format);
            }
            else
            {
                SoftwareBitmap previewFrame = currentFrame.SoftwareBitmap;
                FrameInfoTextBlock.Text = String.Format("{0}x{1} {2}", previewFrame.PixelWidth, previewFrame.PixelHeight, previewFrame.BitmapPixelFormat);
            }

            PreviewFrameImage.Source = null;
        }
    }

    private async Task CleanupCameraAsync()
    {
        await _mediaCaptureLifeLock.WaitAsync();

        try
        {
            if (_isInitialized)
            {
                if (_isPreviewing)
                {
                    await StopPreviewAsync();
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
        finally
        {
            _mediaCaptureLifeLock.Release();
        }
    }

    #endregion MediaCapture methods

    #region Helper functions

    private static async Task<DeviceInformation?> FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel desiredPanel)
    {
        var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

        DeviceInformation? desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);

        return desiredDevice ?? allVideoDevices.FirstOrDefault();
    }

    private static async Task SaveSoftwareBitmapAsync(SoftwareBitmap bitmap, StorageFile file)
    {
        using (var outputStream = await file.OpenAsync(FileAccessMode.ReadWrite))
        {
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, outputStream);
            encoder.SetSoftwareBitmap(bitmap);
            await encoder.FlushAsync();
        }
    }

    private unsafe void ApplyGreenFilter(SoftwareBitmap bitmap)
    {
        if (bitmap.BitmapPixelFormat == BitmapPixelFormat.Bgra8)
        {
            const int BYTES_PER_PIXEL = 4;

            using (var buffer = bitmap.LockBuffer(BitmapBufferAccessMode.ReadWrite))
            using (var reference = buffer.CreateReference())
            {
                if (reference is IMemoryBufferByteAccess)
                {
                    byte* data;
                    uint capacity;
                    ((IMemoryBufferByteAccess)reference).GetBuffer(out data, out capacity);

                    var desc = buffer.GetPlaneDescription(0);

                    for (uint row = 0; row < desc.Height; row++)
                    {
                        for (uint col = 0; col < desc.Width; col++)
                        {
                            var currPixel = desc.StartIndex + desc.Stride * row + BYTES_PER_PIXEL * col;

                            var b = data[currPixel + 0];
                            var g = data[currPixel + 1];
                            var r = data[currPixel + 2];

                            data[currPixel + 0] = b;
                            data[currPixel + 1] = (byte)Math.Min(g + 80, 255);
                            data[currPixel + 2] = r;
                        }
                    }
                }
            }
        }
    }

    #endregion Helper functions
}