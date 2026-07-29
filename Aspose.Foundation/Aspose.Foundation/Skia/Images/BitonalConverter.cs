// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/08/2017 by Alexey Noskov

#if NETSTANDARD

using SkiaSharp;
using Aspose.IO;
using System;
using Aspose.Images.Pal;
using System.Runtime.InteropServices;

namespace Aspose.Images
{
    public class BitonalConverter
    {
        /// <summary>
        /// Create bitonal bitmap from the original bitmap. Image rows are aligned on byte boundary.
        /// </summary>
        public byte[] ConvertToBitonalBytes(SKBitmap original)
        {
            BitmapBytes bitmapBytes = GetBitmapBytesArgb(original);
            int bytesPerLine = bitmapBytes.Width / 8 + (bitmapBytes.Width % 8 > 0 ? 1 : 0);
            return CreateBitonalImageBytes(bitmapBytes, bytesPerLine);
        }

        public SKBitmap ConvertToBitonal(SKBitmap original)
        {
            return ConvertToBitonal(original, false);
        }

        public SKBitmap ConvertToBitonal(SKBitmap original, bool invert)
        {
            mInverseColors = invert;
            // This is workaround, it is required to implement real conversion to 1bpp. See WORDSNET-16706.
            byte[] oneBppBytes = ConvertToBitonalBytes(original);
            return CreateBitmapFrom1bppBytes(oneBppBytes, original.Width, original.Height);
        }

        /// <summary>
        ///  Creates a bitonal image from image data.
        /// </summary>
        /// <remarks>
        /// Using the <see cref="SKBitmap.SetPixels(IntPtr)"> method may cause memory leaks, so it is preferable to use
        /// the <see cref="SKBitmap.InstallPixels(SKImageInfo, IntPtr, int, SKBitmapReleaseDelegate)"> method.
        /// InstallPixels also optionally takes a delegate that will be invoked on the supplied pixel array pointer when
        /// the bitmap is disposed of. This can be used to do things like unpin arrays, release GC handles,
        /// free memory on the non-GC heap and so on. This lets you pass ownership of a memory buffer to an SkBitmap instance,
        /// and not have to worry about keeping it alive or cleaning it up yourself.
        /// </remarks>
        public static SKBitmap CreateBitmapFrom1bppBytes(byte[] oneBppBytes, int width, int height)
        {
            int bytesPerLine = width / 8 + (width % 8 > 0 ? 1 : 0);

            // In SkiaSharp there is no way to create 1bpp image, so use 8bit gray-scale instead.
            SKImageInfo info = new SKImageInfo(width, height, SKColorType.Gray8, SKAlphaType.Opaque);
            SKBitmap destination = new SKBitmap(info);

            // 1bpp data cannot be directly written to Gray8 image, it is required to convert them to gray.
            byte[] grayData = new byte[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;
                    int byteIndex = y * bytesPerLine + x / 8;
                    int bitIndex = x % 8;

                    byte b = oneBppBytes[byteIndex];

                    grayData[index] = ((b >> (7 - bitIndex) & 0x01) > 0) ? (byte)0 : (byte)255;
                }
            }

            GCHandle dataPin = GCHandle.Alloc(grayData, GCHandleType.Pinned);
            destination.InstallPixels(info, dataPin.AddrOfPinnedObject(), info.RowBytes, (addr, ctx) => dataPin.Free());

            return destination;
        }

        private byte[] CreateBitonalImageBytes(BitmapBytes bitmapBytes, int bytesPerLine)
        {
            int imageSize = bytesPerLine * bitmapBytes.Height; // Align image rows on bytes
            byte[] result = new byte[imageSize];
            BitWriter writer = new BitWriter(result);

            // Iterate lines
            for (int y = 0; y < bitmapBytes.Height; y++)
            {
                int sourceIndex = y * bitmapBytes.Stride;
                writer.ByteIndex = y * bytesPerLine;
                ConvertBitmapLine(bitmapBytes, writer, sourceIndex);
            }

            writer.Flush();
            return result;
        }

        private void ConvertBitmapLine(BitmapBytes bitmapBytes, BitWriter writer, int byteOffset)
        {
            // Iterate pixels in line
            for (int x = 0; x < bitmapBytes.Width; x++)
            {
                // Compute pixel brightness (i.e. total of Red, Green, and Blue values)
                // Order of components in 32-bit ARGB pixel is BGRA. So we should sum 1,2,3 components.
                // WORDSNET-16704 - In SkiaSharp compute avarage value of color components and copmpare with 127 threshold,
                // this gives result closer to what .NET System.Drawing produces.
                int pixelTotal =
                    (bitmapBytes.Bytes[byteOffset + 0] +
                    bitmapBytes.Bytes[byteOffset + 1] +
                    bitmapBytes.Bytes[byteOffset + 2]) / 3;

                bool shouldWriteOne = mInverseColors ? pixelTotal >= Threshold : pixelTotal < Threshold;
                if (shouldWriteOne)
                    writer.WriteOneInCurrentBit();

                writer.MoveToNextBit();
                byteOffset += 4;
            }
        }

        private static BitmapBytes GetBitmapBytesArgb(SKBitmap original)
        {
            // If original bitmap is not already in 32 BPP, ARGB format, then convert
            if (original.ColorType != BitmapPal.DefaultColorType)
            {
                using (SKBitmap source = original.Copy(BitmapPal.DefaultColorType)) // must be mutable
                {
                    return new BitmapBytes(source);
                }
            }
            else
            {
                return new BitmapBytes(original);
            }
        }

        private static readonly int Threshold = 127;
        private bool mInverseColors;
    }
}
#endif
