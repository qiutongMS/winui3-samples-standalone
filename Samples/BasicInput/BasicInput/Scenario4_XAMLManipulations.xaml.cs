// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;

namespace BasicInput;

public sealed partial class Scenario4_XAMLManipulations : Page
{
    private TransformGroup _transforms = null!;
    private MatrixTransform _previousTransform = null!;
    private CompositeTransform _deltaTransform = null!;
    private bool _forceManipulationsToEnd;

    public Scenario4_XAMLManipulations()
    {
        this.InitializeComponent();
        _forceManipulationsToEnd = false;

        InitOptions();
        InitManipulationTransforms();

        manipulateMe.ManipulationStarted += new ManipulationStartedEventHandler(ManipulateMe_ManipulationStarted);
        manipulateMe.ManipulationDelta += new ManipulationDeltaEventHandler(ManipulateMe_ManipulationDelta);
        manipulateMe.ManipulationCompleted += new ManipulationCompletedEventHandler(ManipulateMe_ManipulationCompleted);
        manipulateMe.ManipulationInertiaStarting += new ManipulationInertiaStartingEventHandler(ManipulateMe_ManipulationInertiaStarting);

        manipulateMe.ManipulationMode =
            ManipulationModes.TranslateX |
            ManipulationModes.TranslateY |
            ManipulationModes.Rotate |
            ManipulationModes.TranslateInertia |
            ManipulationModes.RotateInertia;
    }

    private void InitManipulationTransforms()
    {
        _transforms = new TransformGroup();
        _previousTransform = new MatrixTransform() { Matrix = Matrix.Identity };
        _deltaTransform = new CompositeTransform();

        _transforms.Children.Add(_previousTransform);
        _transforms.Children.Add(_deltaTransform);

        manipulateMe.RenderTransform = _transforms;
    }

    private void ManipulateMe_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        _forceManipulationsToEnd = false;
        manipulateMe.Background = new SolidColorBrush(Colors.DeepSkyBlue);
    }

    private void ManipulateMe_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (_forceManipulationsToEnd)
        {
            e.Complete();
            return;
        }

        _previousTransform.Matrix = _transforms.Value;

        Point center = _previousTransform.TransformPoint(new Point(e.Position.X, e.Position.Y));
        _deltaTransform.CenterX = center.X;
        _deltaTransform.CenterY = center.Y;

        _deltaTransform.Rotation = e.Delta.Rotation;
        _deltaTransform.TranslateX = e.Delta.Translation.X;
        _deltaTransform.TranslateY = e.Delta.Translation.Y;
    }

    private void ManipulateMe_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
    {
        manipulateMe.Background = new SolidColorBrush(Colors.RoyalBlue);
    }

    private void ManipulateMe_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        manipulateMe.Background = new SolidColorBrush(Colors.LightGray);
    }

    private void movementAxis_Changed(object sender, SelectionChangedEventArgs e)
    {
        manipulateMe.ManipulationMode |= ManipulationModes.TranslateX | ManipulationModes.TranslateY;

        ComboBoxItem selectedItem = (ComboBoxItem)((ComboBox)sender).SelectedItem;
        switch (selectedItem.Content.ToString())
        {
            case "X only":
                manipulateMe.ManipulationMode ^= ManipulationModes.TranslateY;
                break;
            case "Y only":
                manipulateMe.ManipulationMode ^= ManipulationModes.TranslateX;
                break;
        }
    }

    private void InertiaSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        if (manipulateMe != null)
        {
            manipulateMe.ManipulationMode ^= ManipulationModes.TranslateInertia | ManipulationModes.RotateInertia;
        }
    }

    private void InitOptions()
    {
        movementAxis.SelectedIndex = 0;
        inertiaSwitch.IsOn = true;
    }

    private void resetButton_Pressed(object sender, RoutedEventArgs e)
    {
        _forceManipulationsToEnd = true;
        manipulateMe.RenderTransform = null;
        movementAxis.SelectedIndex = 0;
        InitOptions();
        InitManipulationTransforms();
    }
}
