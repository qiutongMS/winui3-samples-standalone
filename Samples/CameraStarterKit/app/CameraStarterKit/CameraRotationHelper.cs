using System;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Storage.FileProperties;

namespace CameraStarterKit;

class CameraRotationHelper
{
    private readonly EnclosureLocation _cameraEnclosureLocation;
    private readonly SimpleOrientationSensor? _orientationSensor = SimpleOrientationSensor.GetDefault();

    public event EventHandler<bool>? OrientationChanged;

    public CameraRotationHelper(EnclosureLocation cameraEnclosureLocation)
    {
        _cameraEnclosureLocation = cameraEnclosureLocation;
        if (!IsEnclosureLocationExternal(_cameraEnclosureLocation) && _orientationSensor != null)
        {
            _orientationSensor.OrientationChanged += SimpleOrientationSensor_OrientationChanged;
        }
    }

    public static bool IsEnclosureLocationExternal(EnclosureLocation? enclosureLocation)
    {
        return (enclosureLocation == null || enclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Unknown);
    }

    public SimpleOrientation GetUIOrientation()
    {
        if (IsEnclosureLocationExternal(_cameraEnclosureLocation))
        {
            return SimpleOrientation.NotRotated;
        }

        var deviceOrientation = _orientationSensor?.GetCurrentOrientation() ?? SimpleOrientation.NotRotated;
        // On desktop, display is always landscape (NotRotated)
        return SubtractOrientations(SimpleOrientation.NotRotated, deviceOrientation);
    }

    public SimpleOrientation GetCameraCaptureOrientation()
    {
        if (IsEnclosureLocationExternal(_cameraEnclosureLocation))
        {
            return SimpleOrientation.NotRotated;
        }

        var deviceOrientation = _orientationSensor?.GetCurrentOrientation() ?? SimpleOrientation.NotRotated;
        var result = SubtractOrientations(deviceOrientation, GetCameraOrientationRelativeToNativeOrientation());

        if (ShouldMirrorPreview())
        {
            result = MirrorOrientation(result);
        }
        return result;
    }

    public SimpleOrientation GetCameraPreviewOrientation()
    {
        if (IsEnclosureLocationExternal(_cameraEnclosureLocation))
        {
            return SimpleOrientation.NotRotated;
        }

        // On desktop, display orientation is always landscape (NotRotated)
        var result = SimpleOrientation.NotRotated;
        result = SubtractOrientations(result, GetCameraOrientationRelativeToNativeOrientation());

        if (ShouldMirrorPreview())
        {
            result = MirrorOrientation(result);
        }
        return result;
    }

    public static PhotoOrientation ConvertSimpleOrientationToPhotoOrientation(SimpleOrientation orientation)
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

    public static int ConvertSimpleOrientationToClockwiseDegrees(SimpleOrientation orientation)
    {
        switch (orientation)
        {
            case SimpleOrientation.Rotated90DegreesCounterclockwise:
                return 270;
            case SimpleOrientation.Rotated180DegreesCounterclockwise:
                return 180;
            case SimpleOrientation.Rotated270DegreesCounterclockwise:
                return 90;
            case SimpleOrientation.NotRotated:
            default:
                return 0;
        }
    }

    private static SimpleOrientation MirrorOrientation(SimpleOrientation orientation)
    {
        switch (orientation)
        {
            case SimpleOrientation.Rotated90DegreesCounterclockwise:
                return SimpleOrientation.Rotated270DegreesCounterclockwise;
            case SimpleOrientation.Rotated270DegreesCounterclockwise:
                return SimpleOrientation.Rotated90DegreesCounterclockwise;
        }
        return orientation;
    }

    private static SimpleOrientation AddOrientations(SimpleOrientation a, SimpleOrientation b)
    {
        var aRot = ConvertSimpleOrientationToClockwiseDegrees(a);
        var bRot = ConvertSimpleOrientationToClockwiseDegrees(b);
        var result = (aRot + bRot) % 360;
        return ConvertClockwiseDegreesToSimpleOrientation(result);
    }

    private static SimpleOrientation SubtractOrientations(SimpleOrientation a, SimpleOrientation b)
    {
        var aRot = ConvertSimpleOrientationToClockwiseDegrees(a);
        var bRot = ConvertSimpleOrientationToClockwiseDegrees(b);
        var result = (360 + (aRot - bRot)) % 360;
        return ConvertClockwiseDegreesToSimpleOrientation(result);
    }

    private static SimpleOrientation ConvertClockwiseDegreesToSimpleOrientation(int orientation)
    {
        switch (orientation)
        {
            case 270:
                return SimpleOrientation.Rotated90DegreesCounterclockwise;
            case 180:
                return SimpleOrientation.Rotated180DegreesCounterclockwise;
            case 90:
                return SimpleOrientation.Rotated270DegreesCounterclockwise;
            case 0:
            default:
                return SimpleOrientation.NotRotated;
        }
    }

    private void SimpleOrientationSensor_OrientationChanged(SimpleOrientationSensor sender, SimpleOrientationSensorOrientationChangedEventArgs args)
    {
        if (args.Orientation != SimpleOrientation.Faceup && args.Orientation != SimpleOrientation.Facedown)
        {
            OrientationChanged?.Invoke(this, false);
        }
    }

    private bool ShouldMirrorPreview()
    {
        return (_cameraEnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
    }

    private SimpleOrientation GetCameraOrientationRelativeToNativeOrientation()
    {
        var enclosureAngle = ConvertClockwiseDegreesToSimpleOrientation((int)_cameraEnclosureLocation.RotationAngleInDegreesClockwise);
        return enclosureAngle;
    }
}
