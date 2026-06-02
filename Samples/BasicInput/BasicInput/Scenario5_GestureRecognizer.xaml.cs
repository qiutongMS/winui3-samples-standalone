// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).

using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace BasicInput;

public sealed partial class Scenario5_GestureRecognizer : Page
{
    private GestureRecognizer _recognizer = null!;
    private ManipulationInputProcessor _manipulationProcessor = null!;

    public Scenario5_GestureRecognizer()
    {
        this.InitializeComponent();

        InitOptions();

        _recognizer = new GestureRecognizer();
        _manipulationProcessor = new ManipulationInputProcessor(_recognizer, manipulateMe, mainCanvas);
    }

    private void InitOptions()
    {
        movementAxis.SelectedIndex = 0;
        InertiaSwitch.IsOn = true;
    }

    private void movementAxis_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (_manipulationProcessor == null)
        {
            return;
        }

        ComboBoxItem selectedItem = (ComboBoxItem)((ComboBox)sender).SelectedItem;
        switch (selectedItem.Content.ToString())
        {
            case "X only":
                _manipulationProcessor.LockToXAxis();
                break;
            case "Y only":
                _manipulationProcessor.LockToYAxis();
                break;
            default:
                _manipulationProcessor.MoveOnXAndYAxes();
                break;
        }
    }

    private void InertiaSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        if (_manipulationProcessor == null)
        {
            return;
        }

        _manipulationProcessor.UseInertia(InertiaSwitch.IsOn);
    }

    private void resetButton_Pressed(object sender, RoutedEventArgs e)
    {
        InitOptions();
        _manipulationProcessor.Reset();
    }
}

internal class ManipulationInputProcessor
{
    private readonly GestureRecognizer _recognizer;
    private readonly UIElement _element;
    private readonly UIElement _reference;
    private TransformGroup _cumulativeTransform = null!;
    private MatrixTransform _previousTransform = null!;
    private CompositeTransform _deltaTransform = null!;

    public ManipulationInputProcessor(GestureRecognizer gestureRecognizer, UIElement target, UIElement referenceFrame)
    {
        _recognizer = gestureRecognizer;
        _element = target;
        _reference = referenceFrame;

        InitializeTransforms();

        _recognizer.GestureSettings = GenerateDefaultSettings();

        _element.PointerPressed += OnPointerPressed;
        _element.PointerMoved += OnPointerMoved;
        _element.PointerReleased += OnPointerReleased;
        _element.PointerCanceled += OnPointerCanceled;

        _recognizer.ManipulationStarted += OnManipulationStarted;
        _recognizer.ManipulationUpdated += OnManipulationUpdated;
        _recognizer.ManipulationCompleted += OnManipulationCompleted;
        _recognizer.ManipulationInertiaStarting += OnManipulationInertiaStarting;
    }

    public void InitializeTransforms()
    {
        _cumulativeTransform = new TransformGroup();
        _deltaTransform = new CompositeTransform();
        _previousTransform = new MatrixTransform() { Matrix = Matrix.Identity };

        _cumulativeTransform.Children.Add(_previousTransform);
        _cumulativeTransform.Children.Add(_deltaTransform);

        _element.RenderTransform = _cumulativeTransform;
    }

    private static GestureSettings GenerateDefaultSettings()
    {
        return GestureSettings.ManipulationTranslateX |
            GestureSettings.ManipulationTranslateY |
            GestureSettings.ManipulationRotate |
            GestureSettings.ManipulationTranslateInertia |
            GestureSettings.ManipulationRotateInertia;
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
    {
        _element.CapturePointer(args.Pointer);
        _recognizer.ProcessDownEvent(args.GetCurrentPoint(_reference));
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs args)
    {
        _recognizer.ProcessMoveEvents(args.GetIntermediatePoints(_reference));
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs args)
    {
        _recognizer.ProcessUpEvent(args.GetCurrentPoint(_reference));
        _element.ReleasePointerCapture(args.Pointer);
    }

    private void OnPointerCanceled(object sender, PointerRoutedEventArgs args)
    {
        _recognizer.CompleteGesture();
        _element.ReleasePointerCapture(args.Pointer);
    }

    private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
    {
        Border b = (Border)_element;
        b.Background = new SolidColorBrush(Colors.DeepSkyBlue);
    }

    private void OnManipulationUpdated(object sender, ManipulationUpdatedEventArgs e)
    {
        _previousTransform.Matrix = _cumulativeTransform.Value;

        Point center = new Point(e.Position.X, e.Position.Y);
        _deltaTransform.CenterX = center.X;
        _deltaTransform.CenterY = center.Y;

        _deltaTransform.Rotation = e.Delta.Rotation;
        _deltaTransform.TranslateX = e.Delta.Translation.X;
        _deltaTransform.TranslateY = e.Delta.Translation.Y;
    }

    private void OnManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
    {
        Border b = (Border)_element;
        b.Background = new SolidColorBrush(Colors.RoyalBlue);
    }

    private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
    {
        Border b = (Border)_element;
        b.Background = new SolidColorBrush(Colors.LightGray);
    }

    public void LockToXAxis()
    {
        _recognizer.CompleteGesture();
        _recognizer.GestureSettings |= GestureSettings.ManipulationTranslateY | GestureSettings.ManipulationTranslateX;
        _recognizer.GestureSettings ^= GestureSettings.ManipulationTranslateY;
    }

    public void LockToYAxis()
    {
        _recognizer.CompleteGesture();
        _recognizer.GestureSettings |= GestureSettings.ManipulationTranslateY | GestureSettings.ManipulationTranslateX;
        _recognizer.GestureSettings ^= GestureSettings.ManipulationTranslateX;
    }

    public void MoveOnXAndYAxes()
    {
        _recognizer.CompleteGesture();
        _recognizer.GestureSettings |= GestureSettings.ManipulationTranslateX | GestureSettings.ManipulationTranslateY;
    }

    public void UseInertia(bool inertia)
    {
        if (!inertia)
        {
            _recognizer.CompleteGesture();
            _recognizer.GestureSettings ^= GestureSettings.ManipulationTranslateInertia | GestureSettings.ManipulationRotateInertia;
        }
        else
        {
            _recognizer.GestureSettings |= GestureSettings.ManipulationTranslateInertia | GestureSettings.ManipulationRotateInertia;
        }
    }

    public void Reset()
    {
        _element.RenderTransform = null;
        _recognizer.CompleteGesture();
        InitializeTransforms();
        _recognizer.GestureSettings = GenerateDefaultSettings();
    }
}
