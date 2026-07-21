// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/12/2008 by Roman Korchagin
// 25/05/2011 by Alexey Titov
#if !NETSTANDARD

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Aspose.Images.Pal;
using Aspose.IO;
using Aspose.JavaAttributes;

namespace Aspose.Images
{
    /// <summary>
    /// Idea taken from http://www.codeproject.com/KB/GDI-plus/BitonalImageConverter.aspx
    /// </summary>
    [JavaDelete("In Java we do conversion using Java's BufferedImage capabilities.")]
    [AndroidManual("Not needed in java. But it needs to be ported on Android manually.")]
    public class BitonalConverter
    {
        /// <summary>
        /// Create bitonal bitmap from the original bitmap. Image rows are aligned on byte boundary.
        /// </summary>
        public byte[] ConvertToBitonalBytes(Bitmap original)
        {
            BitmapBytes bitmapBytes = GetBitmapBytesArgb(original);
            int bytesPerLine = bitmapBytes.Width / 8 + (bitmapBytes.Width % 8 > 0 ? 1 : 0);
            return CreateBitonalImageBytes(bitmapBytes, bytesPerLine);
        }

        public Bitmap ConvertToBitonal(Bitmap original)
        {
            mInverseColors = true; //Inverse colors to correspond indexes in color palette.
            BitmapBytes bitmapBytes = GetBitmapBytesArgb(original);

            // Create destination bitmap
            Bitmap destination = new Bitmap(original.Width, original.Height, PixelFormat.Format1bppIndexed);

            // WORDSNET-7835 The problem occurred because Bitmap used default resolution.
            // Fixed by using resolution of the original Bitmap.
            destination.SetResolution(original.HorizontalResolution, original.VerticalResolution);

            // Lock destination bitmap in memory
            BitmapData destinationData = destination.LockBits(
                new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format1bppIndexed);

            // Create destination buffer
            byte[] destinationBuffer = CreateBitonalImageBytes(bitmapBytes, destinationData.Stride);

            // Copy binary image data to destination bitmap
            Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, destinationBuffer.Length);

            // Unlock destination bitmap
            destination.UnlockBits(destinationData);

            // Return
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
                // WORDSNET-5510
                // Order of components in 32-bit ARGB pixel is BGRA. So we should sum 1,2,3 components.
                int pixelTotal = 
                    bitmapBytes.Bytes[byteOffset] + 
                    bitmapBytes.Bytes[byteOffset + 1] +
                    bitmapBytes.Bytes[byteOffset + 2];

                bool shouldWriteOne = mInverseColors ? pixelTotal > Threshold : pixelTotal < Threshold;
                if (shouldWriteOne)
                    writer.WriteOneInCurrentBit();

                writer.MoveToNextBit();
                byteOffset += 4;
            }
        }

        internal static BitmapBytes GetBitmapBytesArgb(Bitmap original)
        {
            Bitmap source = null;
            try
            {
                // If original bitmap is not already in 32 BPP, ARGB format, then convert
                if (original.PixelFormat != PixelFormat.Format32bppArgb)
                {
                    source = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
                    source.SetResolution(original.HorizontalResolution, original.VerticalResolution);
                    using (Graphics g = BitmapUtilPal.GraphicsFromImage(source))
                        g.DrawImageUnscaled(original, 0, 0);
                }
                else
                    source = original;

                // Lock source bitmap in memory
                BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height),
                    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                // Copy image data to binary array
                BitmapBytes bitmapBytes = new BitmapBytes(sourceData);

                // Unlock source bitmap
                source.UnlockBits(sourceData);
                return bitmapBytes;
            }
            finally
            {
                // Dispose of source if not originally supplied bitmap
                if (source != original && source != null)
                    source.Dispose();
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int Threshold = 500;
        private bool mInverseColors;
    }
}
#endif
