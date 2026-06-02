// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).

using System.Text;
using Microsoft.UI.Xaml.Controls;
using Windows.Devices.Input;

namespace BasicInput;

public sealed partial class Scenario3_DeviceCapabilities : Page
{
    public Scenario3_DeviceCapabilities()
    {
        this.InitializeComponent();

        // Retrieve information about whether or not a keyboard is present
        KeyboardCapabilities kbdCapabilities = new KeyboardCapabilities();
        keyboardText.Text = "Keyboard present = " + kbdCapabilities.KeyboardPresent.ToString();

        // Retrieve information about the capabilities of the device's mouse
        MouseCapabilities mouseCapabilities = new MouseCapabilities();
        StringBuilder sb = new StringBuilder();
        sb.Append("Mouse present = " + mouseCapabilities.MousePresent.ToString() + "\n");
        sb.Append("Number of buttons = " + mouseCapabilities.NumberOfButtons.ToString() + "\n");
        sb.Append("Vertical wheel present = " + mouseCapabilities.VerticalWheelPresent.ToString() + "\n");
        sb.Append("Horizontal wheel present = " + mouseCapabilities.HorizontalWheelPresent.ToString() + "\n");
        sb.Append("Buttons swapped = " + mouseCapabilities.SwapButtons.ToString());
        mouseText.Text = sb.ToString();

        // Retrieve information about the capabilities of the device's touch
        TouchCapabilities touchCapabilities = new TouchCapabilities();
        sb = new StringBuilder();
        sb.Append("Touch present = " + touchCapabilities.TouchPresent.ToString() + "\n");
        sb.Append("Touch contacts supported = " + touchCapabilities.Contacts.ToString());
        touchText.Text = sb.ToString();
    }
}
