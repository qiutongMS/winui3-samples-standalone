// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).

using System.Collections.Generic;
using System.Text;
using Microsoft.UI;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace BasicInput;

public sealed partial class Scenario2_PointerPointProperties : Page
{
    private readonly Dictionary<uint, StackPanel> _popups;

    public Scenario2_PointerPointProperties()
    {
        this.InitializeComponent();

        _popups = new Dictionary<uint, StackPanel>();

        mainCanvas.PointerPressed += new PointerEventHandler(Pointer_Pressed);
        mainCanvas.PointerMoved += new PointerEventHandler(Pointer_Moved);
        mainCanvas.PointerReleased += new PointerEventHandler(Pointer_Released);
        mainCanvas.PointerExited += new PointerEventHandler(Pointer_Released);
        mainCanvas.PointerEntered += new PointerEventHandler(Pointer_Entered);
        mainCanvas.PointerWheelChanged += new PointerEventHandler(Pointer_Wheel_Changed);
    }

    private void Pointer_Pressed(object sender, PointerRoutedEventArgs e)
    {
        PointerPoint currentPoint = e.GetCurrentPoint(mainCanvas);
        CreateOrUpdatePropertyPopUp(currentPoint);
    }

    private void Pointer_Entered(object sender, PointerRoutedEventArgs e)
    {
        PointerPoint currentPoint = e.GetCurrentPoint(mainCanvas);
        if (currentPoint.IsInContact)
        {
            CreateOrUpdatePropertyPopUp(currentPoint);
        }
    }

    private void Pointer_Moved(object sender, PointerRoutedEventArgs e)
    {
        PointerPoint currentPoint = e.GetCurrentPoint(mainCanvas);
        if (currentPoint.IsInContact)
        {
            CreateOrUpdatePropertyPopUp(currentPoint);
        }
    }

    private void Pointer_Released(object sender, PointerRoutedEventArgs e)
    {
        PointerPoint currentPoint = e.GetCurrentPoint(mainCanvas);
        HidePropertyPopUp(currentPoint);
    }

    private void Pointer_Wheel_Changed(object sender, PointerRoutedEventArgs e)
    {
        PointerPoint currentPoint = e.GetCurrentPoint(mainCanvas);
        if (currentPoint.IsInContact)
        {
            CreateOrUpdatePropertyPopUp(currentPoint);
        }
    }

    private void CreateOrUpdatePropertyPopUp(PointerPoint currentPoint)
    {
        string pointerProperties = GetPointerProperties(currentPoint);
        string deviceSpecificProperties = GetDeviceSpecificProperties(currentPoint);
        RenderPropertyPopUp(pointerProperties, deviceSpecificProperties, currentPoint);
    }

    private static string GetPointerProperties(PointerPoint currentPoint)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("ID: " + currentPoint.PointerId);
        sb.Append("\nX: " + currentPoint.Position.X);
        sb.Append("\nY: " + currentPoint.Position.Y);
        sb.Append("\nContact: " + currentPoint.IsInContact + "\n");
        return sb.ToString();
    }

    private static string GetDeviceSpecificProperties(PointerPoint currentPoint)
    {
        string deviceSpecificProperties = "";
        switch (currentPoint.PointerDeviceType)
        {
            case PointerDeviceType.Mouse:
                deviceSpecificProperties = GetMouseProperties(currentPoint);
                break;
            case PointerDeviceType.Pen:
                deviceSpecificProperties = GetPenProperties(currentPoint);
                break;
            case PointerDeviceType.Touch:
                deviceSpecificProperties = GetTouchProperties(currentPoint);
                break;
        }
        return deviceSpecificProperties;
    }

    private static string GetMouseProperties(PointerPoint currentPoint)
    {
        PointerPointProperties pointerProperties = currentPoint.Properties;
        StringBuilder sb = new StringBuilder();
        sb.Append("Left button: " + pointerProperties.IsLeftButtonPressed);
        sb.Append("\nRight button: " + pointerProperties.IsRightButtonPressed);
        sb.Append("\nMiddle button: " + pointerProperties.IsMiddleButtonPressed);
        sb.Append("\nX1 button: " + pointerProperties.IsXButton1Pressed);
        sb.Append("\nX2 button: " + pointerProperties.IsXButton2Pressed);
        sb.Append("\nMouse wheel delta: " + pointerProperties.MouseWheelDelta);
        return sb.ToString();
    }

    private static string GetTouchProperties(PointerPoint currentPoint)
    {
        PointerPointProperties pointerProperties = currentPoint.Properties;
        StringBuilder sb = new StringBuilder();
        sb.Append("Contact Rect X: " + pointerProperties.ContactRect.X);
        sb.Append("\nContact Rect Y: " + pointerProperties.ContactRect.Y);
        sb.Append("\nContact Rect Width: " + pointerProperties.ContactRect.Width);
        sb.Append("\nContact Rect Height: " + pointerProperties.ContactRect.Height);
        return sb.ToString();
    }

    private static string GetPenProperties(PointerPoint currentPoint)
    {
        PointerPointProperties pointerProperties = currentPoint.Properties;
        StringBuilder sb = new StringBuilder();
        sb.Append("Barrel button: " + pointerProperties.IsBarrelButtonPressed);
        sb.Append("\nEraser: " + pointerProperties.IsEraser);
        sb.Append("\nPressure: " + pointerProperties.Pressure);
        return sb.ToString();
    }

    private void RenderPropertyPopUp(string pointerProperties, string deviceSpecificProperties, PointerPoint currentPoint)
    {
        if (_popups.ContainsKey(currentPoint.PointerId))
        {
            TextBlock pointerText = (TextBlock)_popups[currentPoint.PointerId].Children[0];
            pointerText.Text = pointerProperties;
            TextBlock deviceSpecificText = (TextBlock)_popups[currentPoint.PointerId].Children[1];
            deviceSpecificText.Text = deviceSpecificProperties;
        }
        else
        {
            StackPanel pointerPanel = new StackPanel();
            TextBlock pointerText = new TextBlock();
            pointerText.Text = pointerProperties;
            pointerPanel.Children.Add(pointerText);

            TextBlock deviceSpecificText = new TextBlock();
            deviceSpecificText.Text = deviceSpecificProperties;

            switch (currentPoint.PointerDeviceType)
            {
                case PointerDeviceType.Mouse:
                    deviceSpecificText.Foreground = new SolidColorBrush(Colors.Red);
                    break;
                case PointerDeviceType.Touch:
                    deviceSpecificText.Foreground = new SolidColorBrush(Colors.Green);
                    break;
                case PointerDeviceType.Pen:
                    deviceSpecificText.Foreground = new SolidColorBrush(Colors.Yellow);
                    break;
            }
            pointerPanel.Children.Add(deviceSpecificText);
            _popups[currentPoint.PointerId] = pointerPanel;
            mainCanvas.Children.Add(_popups[currentPoint.PointerId]);
        }
        TranslateTransform transform = new TranslateTransform();
        transform.X = currentPoint.Position.X + 24;
        transform.Y = currentPoint.Position.Y + 24;
        _popups[currentPoint.PointerId].RenderTransform = transform;
    }

    private void HidePropertyPopUp(PointerPoint currentPoint)
    {
        if (_popups.ContainsKey(currentPoint.PointerId))
        {
            mainCanvas.Children.Remove(_popups[currentPoint.PointerId]);
            _popups.Remove(currentPoint.PointerId);
        }
    }
}
