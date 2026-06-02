using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.ViewManagement.Core;
using WinRT;
using WinRT.Interop;

namespace TouchKeyboard;

/// <summary>
/// Sample page to show the Emoji keyboard.
/// </summary>
public sealed partial class Scenario4_ShowView : Page
{
    public Scenario4_ShowView()
    {
        InitializeComponent();
    }

    private void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        var window = MainWindow.Current;
        if (window is null)
        {
            return;
        }

        var hwnd = WindowNative.GetWindowHandle(window);
        var coreInputView = GetCoreInputViewForWindow(hwnd);
        coreInputView?.TryShow(CoreInputViewKind.Emoji);
    }

    private static CoreInputView? GetCoreInputViewForWindow(nint hwnd)
    {
        var iid = typeof(CoreInputView).GUID;
        var hr = RoGetActivationFactory(
            "Windows.UI.ViewManagement.Core.CoreInputView",
            ref iid,
            out var factory);

        if (hr != 0 || factory is null)
        {
            return null;
        }

        // Use the ICoreInputViewInterop COM interface
        var interop = factory as ICoreInputViewInterop;
        if (interop is null)
        {
            return null;
        }

        var coreInputViewIid = typeof(CoreInputView).GUID;
        interop.GetForWindow(hwnd, ref coreInputViewIid, out var result);
        return result as CoreInputView;
    }

    [DllImport("combase.dll", PreserveSig = true)]
    private static extern int RoGetActivationFactory(
        [MarshalAs(UnmanagedType.HString)] string activatableClassId,
        ref Guid iid,
        out object factory);

    [ComImport]
    [Guid("0576AB31-A310-4C40-B31F-CECDAE573C13")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface ICoreInputViewInterop
    {
        void GetForWindow(nint appWindow, ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object coreInputView);
    }
}
