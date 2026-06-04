//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Windows.Graphics.Imaging;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

namespace SDKTemplate
{
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    class FrameRenderer
    {
        private Image _imageElement;
        private SoftwareBitmap _backBuffer;
        private bool _taskRunning = false;

        public FrameRenderer(Image imageElement)
        {
            _imageElement = imageElement;
            _imageElement.Source = new SoftwareBitmapSource();
        }

        public void ProcessFrame(MediaFrameReference frame)
        {
            var softwareBitmap = FrameRenderer.ConvertToDisplayableImage(frame?.VideoMediaFrame);

            RenderFrame(softwareBitmap);
        }

        public void RenderFrame(SoftwareBitmap softwareBitmap)
        {
            if (softwareBitmap != null)
            {
                // Swap the processed frame to _backBuffer and trigger UI thread to render it
                softwareBitmap = Interlocked.Exchange(ref _backBuffer, softwareBitmap);

                // UI thread always reset _backBuffer before using it. Unused bitmap should be disposed.
                softwareBitmap?.Dispose();

                // Changes to xaml ImageElement must happen in UI thread through DispatcherQueue
                _imageElement.DispatcherQueue.TryEnqueue(async () =>
                {
                    // Don't let two copies of this task run at the same time.
                    if (_taskRunning)
                    {
                        return;
                    }
                    _taskRunning = true;

                    // Keep draining frames from the backbuffer until the backbuffer is empty.
                    SoftwareBitmap latestBitmap;
                    while ((latestBitmap = Interlocked.Exchange(ref _backBuffer, null)) != null)
                    {
                        var imageSource = (SoftwareBitmapSource)_imageElement.Source;
                        await imageSource.SetBitmapAsync(latestBitmap);
                        latestBitmap.Dispose();
                    }

                    _taskRunning = false;
                });
            }
        }

        // Function delegate that transforms a scanline from an input image to an output image.
        private unsafe delegate void TransformScanline(int pixelWidth, byte* inputRowBytes, byte* outputRowBytes);

        /// <summary>
        /// Determines the subtype to request from the MediaFrameReader that will result in
        /// a frame that can be rendered by ConvertToDisplayableImage.
        /// </summary>
        /// <returns>Subtype string to request, or null if subtype is not renderable.</returns>
        public static string GetSubtypeForFrameReader(MediaFrameSourceKind kind, MediaFrameFormat format)
        {
            string subtype = format.Subtype;
            switch (kind)
            {
                // For color sources, we accept anything and request that it be converted to Bgra8.
                case MediaFrameSourceKind.Color:
                    return MediaEncodingSubtypes.Bgra8;

                // The only depth format we can render is D16.
                case MediaFrameSourceKind.Depth:
                    return String.Equals(subtype, MediaEncodingSubtypes.D16, StringComparison.OrdinalIgnoreCase) ? subtype : null;

                // The only infrared formats we can render are Nv12, L8, and L16.
                case MediaFrameSourceKind.Infrared:
                    return (String.Equals(subtype, MediaEncodingSubtypes.Nv12, StringComparison.OrdinalIgnoreCase) ||
                        String.Equals(subtype, MediaEncodingSubtypes.L8, StringComparison.OrdinalIgnoreCase) ||
                        String.Equals(subtype, MediaEncodingSubtypes.L16, StringComparison.OrdinalIgnoreCase)) ? subtype : null;

                // No other source kinds are supported by this class.
                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts a frame to a SoftwareBitmap of a valid format to display in an Image control.
        /// </summary>
        /// <param name="inputFrame">Frame to convert.</param>
        public static unsafe SoftwareBitmap ConvertToDisplayableImage(VideoMediaFrame inputFrame)
        {
            SoftwareBitmap result = null;
            using (var inputBitmap = inputFrame?.SoftwareBitmap)
            {
                if (inputBitmap != null)
                {
                    switch (inputFrame.FrameReference.SourceKind)
                    {
                        case MediaFrameSourceKind.Color:
                            // XAML requires Bgra8 with premultiplied alpha.
                            if (inputBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8)
                            {
                                System.Diagnostics.Debug.WriteLine("Color frame in unexpected format.");
                            }
                            else if (inputBitmap.BitmapAlphaMode == BitmapAlphaMode.Premultiplied)
                            {
                                // Already in the correct format.
                                result = SoftwareBitmap.Copy(inputBitmap);
                            }
                            else
                            {
                                // Convert to premultiplied alpha.
                                result = SoftwareBitmap.Convert(inputBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                            }
                            break;

                        case MediaFrameSourceKind.Depth:
                            // We requested D16 from the MediaFrameReader, so the frame should
                            // be in Gray16 format.
                            if (inputBitmap.BitmapPixelFormat == BitmapPixelFormat.Gray16)
                            {
                                // Use a special pseudo color to render 16 bits depth frame.
                                var depthScale = (float)inputFrame.DepthMediaFrame.DepthFormat.DepthScaleInMeters;
                                var minReliableDepth = inputFrame.DepthMediaFrame.MinReliableDepth;
                                var maxReliableDepth = inputFrame.DepthMediaFrame.MaxReliableDepth;
                                result = TransformBitmap(inputBitmap, (w, i, o) => PseudoColorHelper.PseudoColorForDepth(w, i, o, depthScale, minReliableDepth, maxReliableDepth));
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Depth frame in unexpected format.");
                            }
                            break;

                        case MediaFrameSourceKind.Infrared:
                            switch (inputBitmap.BitmapPixelFormat)
                            {
                                case BitmapPixelFormat.Gray16:
                                    // Use pseudo color to render 16 bits frames.
                                    result = TransformBitmap(inputBitmap, PseudoColorHelper.PseudoColorFor16BitInfrared);
                                    break;

                                case BitmapPixelFormat.Gray8:
                                    // Use pseudo color to render 8 bits frames.
                                    result = TransformBitmap(inputBitmap, PseudoColorHelper.PseudoColorFor8BitInfrared);
                                    break;

                                case BitmapPixelFormat.Nv12:
                                    result = SoftwareBitmap.Convert(inputBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                                    break;

                                default:
                                    System.Diagnostics.Debug.WriteLine("Infrared frame in unexpected format.");
                                    break;
                            }
                            break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Transform image into Bgra8 image using given transform method.
        /// </summary>
        private static unsafe SoftwareBitmap TransformBitmap(SoftwareBitmap softwareBitmap, TransformScanline transformScanline)
        {
            // XAML Image control only supports premultiplied Bgra8 format.
            var outputBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8,
                softwareBitmap.PixelWidth, softwareBitmap.PixelHeight, BitmapAlphaMode.Premultiplied);

            using (var input = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Read))
            using (var output = outputBitmap.LockBuffer(BitmapBufferAccessMode.Write))
            {
                // Get stride values to calculate buffer position for a given pixel x and y position.
                int inputStride = input.GetPlaneDescription(0).Stride;
                int outputStride = output.GetPlaneDescription(0).Stride;
                int pixelWidth = softwareBitmap.PixelWidth;
                int pixelHeight = softwareBitmap.PixelHeight;

                using (var outputReference = output.CreateReference())
                using (var inputReference = input.CreateReference())
                {
                    // Get input and output byte access buffers.
                    byte* inputBytes;
                    uint inputCapacity;
                    ((IMemoryBufferByteAccess)inputReference).GetBuffer(out inputBytes, out inputCapacity);
                    byte* outputBytes;
                    uint outputCapacity;
                    ((IMemoryBufferByteAccess)outputReference).GetBuffer(out outputBytes, out outputCapacity);

                    // Iterate over all pixels and store converted value.
                    for (int y = 0; y < pixelHeight; y++)
                    {
                        byte* inputRowBytes = inputBytes + y * inputStride;
                        byte* outputRowBytes = outputBytes + y * outputStride;

                        transformScanline(pixelWidth, inputRowBytes, outputRowBytes);
                    }
                }
            }
            return outputBitmap;
        }

        /// <summary>
        /// A helper class to manage look-up-table for pseudo-colors.
        /// </summary>
        private static class PseudoColorHelper
        {
            private const int TableSize = 1024;
            private static readonly uint[] PseudoColorTable;
            private static readonly uint[] InfraredRampTable;

            private static readonly Windows.UI.Color[] ColorRamp =
            {
                Windows.UI.Color.FromArgb(a:0xFF, r:0x7F, g:0x00, b:0x00),
                Windows.UI.Color.FromArgb(a:0xFF, r:0xFF, g:0x00, b:0x00),
                Windows.UI.Color.FromArgb(a:0xFF, r:0xFF, g:0x7F, b:0x00),
                Windows.UI.Color.FromArgb(a:0xFF, r:0xFF, g:0xFF, b:0x00),
                Windows.UI.Color.FromArgb(a:0xFF, r:0x7F, g:0xFF, b:0x7F),
                Windows.UI.Color.FromArgb(a:0xFF, r:0x00, g:0xFF, b:0xFF),
                Windows.UI.Color.FromArgb(a:0xFF, r:0x00, g:0x7F, b:0xFF),
                Windows.UI.Color.FromArgb(a:0xFF, r:0x00, g:0x00, b:0xFF),
                Windows.UI.Color.FromArgb(a:0xFF, r:0x00, g:0x00, b:0x7F),
            };

            static PseudoColorHelper()
            {
                PseudoColorTable = InitializePseudoColorLut();
                InfraredRampTable = InitializeInfraredRampLut();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static uint InfraredColor(float value)
            {
                int index = (int)(value * TableSize);
                index = index < 0 ? 0 : index > TableSize - 1 ? TableSize - 1 : index;
                return InfraredRampTable[index];
            }

            private static uint[] InitializeInfraredRampLut()
            {
                uint[] lut = new uint[TableSize];
                for (int i = 0; i < TableSize; i++)
                {
                    var value = (float)i / TableSize;
                    var alpha = (float)Math.Pow(1 - value, 12);
                    lut[i] = ColorRampInterpolation(alpha);
                }
                return lut;
            }

            private static uint[] InitializePseudoColorLut()
            {
                uint[] lut = new uint[TableSize];
                for (int i = 0; i < TableSize; i++)
                {
                    lut[i] = ColorRampInterpolation((float)i / TableSize);
                }
                return lut;
            }

            private static uint ColorRampInterpolation(float value)
            {
                int rampSteps = ColorRamp.Length - 1;
                float scaled = value * rampSteps;
                int integer = (int)scaled;
                int index =
                    integer < 0 ? 0 :
                    integer >= rampSteps - 1 ? rampSteps - 1 :
                    integer;
                Windows.UI.Color prev = ColorRamp[index];
                Windows.UI.Color next = ColorRamp[index + 1];

                uint alpha = (uint)((scaled - integer) * 255);
                uint beta = 255 - alpha;
                return
                    ((prev.A * beta + next.A * alpha) / 255) << 24 |
                    ((prev.R * beta + next.R * alpha) / 255) << 16 |
                    ((prev.G * beta + next.G * alpha) / 255) << 8 |
                    ((prev.B * beta + next.B * alpha) / 255);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static uint PseudoColor(float value)
            {
                int index = (int)(value * TableSize);
                index = index < 0 ? 0 : index > TableSize - 1 ? TableSize - 1 : index;
                return PseudoColorTable[index];
            }

            public static unsafe void PseudoColorForDepth(int pixelWidth, byte* inputRowBytes, byte* outputRowBytes, float depthScale, float minReliableDepth, float maxReliableDepth)
            {
                float minInMeters = minReliableDepth * depthScale;
                float maxInMeters = maxReliableDepth * depthScale;
                float one_min = 1.0f / minInMeters;
                float range = 1.0f / maxInMeters - one_min;

                ushort* inputRow = (ushort*)inputRowBytes;
                uint* outputRow = (uint*)outputRowBytes;
                for (int x = 0; x < pixelWidth; x++)
                {
                    var depth = inputRow[x] * depthScale;

                    if (depth == 0)
                    {
                        outputRow[x] = 0;
                    }
                    else
                    {
                        var a = (1.0f / depth - one_min) / range;
                        outputRow[x] = PseudoColor(a * a);
                    }
                }
            }

            public static unsafe void PseudoColorFor8BitInfrared(
                int pixelWidth, byte* inputRowBytes, byte* outputRowBytes)
            {
                byte* inputRow = inputRowBytes;
                uint* outputRow = (uint*)outputRowBytes;
                for (int x = 0; x < pixelWidth; x++)
                {
                    outputRow[x] = InfraredColor(inputRow[x] / (float)Byte.MaxValue);
                }
            }

            public static unsafe void PseudoColorFor16BitInfrared(int pixelWidth, byte* inputRowBytes, byte* outputRowBytes)
            {
                ushort* inputRow = (ushort*)inputRowBytes;
                uint* outputRow = (uint*)outputRowBytes;
                for (int x = 0; x < pixelWidth; x++)
                {
                    outputRow[x] = InfraredColor(inputRow[x] / (float)UInt16.MaxValue);
                }
            }
        }
    }
}
