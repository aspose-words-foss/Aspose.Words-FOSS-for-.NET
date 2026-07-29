// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2025 by Alexey Noskov
#if NETSTANDARD

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Images.Pal.Graphics.Decoder.Tiff;
using SkiaSharp;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// Class provides a method to extract all frames of TIFF as a separate images.
    /// </summary>
    public static class TiffUtilPal
    {
        /// <summary>
        /// Saves each frame of TIFF as PNG and returns a list of result images.
        /// </summary>
        public static List<byte[]> GetFrames(byte[] tiff)
        {
            List<byte[]> frames = new List<byte[]>();

            using (MemoryStream tiffStream = new MemoryStream(tiff))
            {
                TiffDecoder decoder = new TiffDecoder(tiffStream);
                for (short i = 0; i < decoder.PageCount; i++)
                {
                    using (SKBitmap frameBitmap = decoder.GetNativeBitmap(i))
                    using (MemoryStream frameStream = new MemoryStream())
                    {
                        frameBitmap.Encode(frameStream, SKEncodedImageFormat.Png, 100);
                        frames.Add(frameStream.ToArray());
                    }
                }
            }

            return frames;
        }
    }
}
#endif
