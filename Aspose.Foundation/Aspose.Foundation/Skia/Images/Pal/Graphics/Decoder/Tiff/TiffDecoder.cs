// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/01/2024 by Denis Panov

#if NETSTANDARD
using System;
using System.IO;
using System.Runtime.InteropServices;
using Aspose.Skia.Images.Pal.Graphics;
using BitMiracle.LibTiff.Classic;
using SkiaSharp;
using LibTiff = BitMiracle.LibTiff.Classic.Tiff;

namespace Aspose.Images.Pal.Graphics.Decoder.Tiff
{
    /// <summary>
    ///  Class is used to read data from TIFF image.
    /// </summary>
    internal class TiffDecoder : IDisposable
    {
        public TiffDecoder(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("Stream not set!");

            // The BitMiracle.LibTiff library can output error messages to the console.
            // The current code attempts to avoid such behavior, but it is better to suppress the default error handler
            // (null cannot be used because it would use the default handler in that case).
            LibTiff.SetErrorHandler(new TiffDummyErrorHandler());

            // Needs to use our custom TiffStream implementation to prevent BitMiracle.LibTiff from closing the stream
            // when the work is completed.
            mTiff = LibTiff.ClientOpen("in-memory", "r", null, new TiffCustomStream(stream));
            mPageCount = mTiff.NumberOfDirectories();
        }

        public SKBitmap GetNativeBitmap()
        {
            return GetNativeBitmap(0);
        }

        public SKBitmap GetNativeBitmap(short pageIndex)
        {
            string errorMsg;
            if (mTiff.RGBAImageOK(out errorMsg))
            {
                mTiff.SetDirectory(pageIndex);

                int width = mTiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int height = mTiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                Orientation orientation = (Orientation)mTiff.GetFieldDefaulted(TiffTag.ORIENTATION)[0].ToInt();
                ExtraSample extraSample = (ExtraSample)mTiff.GetFieldDefaulted(TiffTag.EXTRASAMPLES)[0].ToInt();

                SKBitmap bitmap = new SKBitmap();
                SKImageInfo info = new SKImageInfo(width, height, SKColorType.Rgba8888, extraSample == ExtraSample.UNASSALPHA
                    ? SKAlphaType.Opaque
                    : SKAlphaType.Premul);

                int[] raster = new int[width * height];
                GCHandle ptr = GCHandle.Alloc(raster, GCHandleType.Pinned);
                bitmap.InstallPixels(info, ptr.AddrOfPinnedObject(), info.RowBytes, (addr, ctx) => ptr.Free());

                TiffRgbaImage tiffRgbaImage = TiffRgbaImage.Create(mTiff, false, out errorMsg);
                if (tiffRgbaImage != null)
                {
                    tiffRgbaImage.ReqOrientation = orientation;
                    if (tiffRgbaImage.GetRaster(raster, (height - tiffRgbaImage.Height) * width, width, tiffRgbaImage.Height))
                        return bitmap;
                }
            }

            throw new ArgumentException(
                string.Format("Exeption during Tiff decoding: {0}", errorMsg ?? "Not a valid TIFF image."));
        }

        public void Dispose()
        {
            if (mTiff != null)
                mTiff.Dispose();
        }

        public short PageCount
        {
            get { return mPageCount; }
        }

        private readonly short mPageCount;
        private readonly LibTiff mTiff;
    }
}
#endif
