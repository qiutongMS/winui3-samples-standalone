// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).

using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace BasicInput;

public sealed partial class Scenario1_InputEvents : Page
{
    public Scenario1_InputEvents()
    {
        this.InitializeComponent();

        // pointer press/release handlers
        pressedTarget.PointerPressed += new PointerEventHandler(target_PointerPressed);
        pressedTarget.PointerReleased += new PointerEventHandler(target_PointerReleased);

        // pointer enter/exit handlers
        enterExitTarget.PointerEntered += new PointerEventHandler(target_PointerEntered);
        enterExitTarget.PointerExited += new PointerEventHandler(target_PointerExited);

        // gesture handlers
        tapTarget.Tapped += new TappedEventHandler(target_Tapped);
        tapTarget.DoubleTapped += new DoubleTappedEventHandler(target_DoubleTapped);

        holdTarget.Holding += new HoldingEventHandler(target_Holding);
        holdTarget.RightTapped += new RightTappedEventHandler(target_RightTapped);
    }

    void target_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        pressedTarget.Background = new SolidColorBrush(Colors.RoyalBlue);
        pressedTargetText.Text = "Pointer Pressed";
    }

    void target_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        pressedTarget.Background = new SolidColorBrush(Colors.LightGray);
        pressedTargetText.Text = "Pointer Released";
    }

    void target_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        enterExitTarget.Background = new SolidColorBrush(Colors.RoyalBlue);
        enterExitTargetText.Text = "Pointer Entered";
    }

    void target_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        enterExitTarget.Background = new SolidColorBrush(Colors.LightGray);
        enterExitTargetText.Text = "Pointer Exited";
    }

    void target_Tapped(object sender, TappedRoutedEventArgs e)
    {
        tapTarget.Background = new SolidColorBrush(Colors.DeepSkyBlue);
        tapTargetText.Text = "Tapped";
    }

    void target_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        tapTarget.Background = new SolidColorBrush(Colors.RoyalBlue);
        tapTargetText.Text = "Double-Tapped";
    }

    void target_RightTapped(object sender, RightTappedRoutedEventArgs e)
    {
        holdTarget.Background = new SolidColorBrush(Colors.RoyalBlue);
        holdTargetText.Text = "Right Tapped";
    }

    void target_Holding(object sender, HoldingRoutedEventArgs e)
    {
        if (e.HoldingState == Microsoft.UI.Input.HoldingState.Started)
        {
            holdTarget.Background = new SolidColorBrush(Colors.DeepSkyBlue);
            holdTargetText.Text = "Holding";
        }
        else if (e.HoldingState == Microsoft.UI.Input.HoldingState.Completed)
        {
            holdTarget.Background = new SolidColorBrush(Colors.LightGray);
            holdTargetText.Text = "Held";
        }
        else
        {
            holdTarget.Background = new SolidColorBrush(Colors.LightGray);
            holdTargetText.Text = "Hold Canceled";
        }
    }
}
