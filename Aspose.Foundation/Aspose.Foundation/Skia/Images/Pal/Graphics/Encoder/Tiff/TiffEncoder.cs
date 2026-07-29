// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/01/2024 by Denis Panov

#if NETSTANDARD

using System;
using System.IO;
using System.Runtime.InteropServices;
using Aspose.Skia.Images.Pal.Graphics;
using BitMiracle.LibTiff.Classic;
using SkiaSharp;
using LibTiff = BitMiracle.LibTiff.Classic.Tiff;

namespace Aspose.Images.Pal.Graphics.Encoder.Tiff
{
    /// <summary>
    /// Class is used to write data to TIFF image.
    /// </summary>
    internal class TiffEncoder : IDisposable
    {
        public TiffEncoder(Stream output, TiffEncoderOptions options)
        {
            if (output == null)
                throw new ArgumentNullException("Output not set");

            if (options == null)
                throw new ArgumentNullException("options == null");

            mOptions = options;
            mPageNumber = 0;

            // The BitMiracle.LibTiff library can output error messages to the console.
            // The current code attempts to avoid such behavior, but it is better to suppress the default error handler
            // (null cannot be used because it would use the default handler in that case).
            LibTiff.SetErrorHandler(new TiffDummyErrorHandler());

            // Needs to use our custom TiffStream implementation to prevent BitMiracle.LibTiff from closing the stream
            // when the work is completed.
            mTiff = LibTiff.ClientOpen("in-memory", "w", null, new TiffCustomStream(output));
        }

        public void Write(SKBitmap image)
        {
            if (image == null)
                throw new ArgumentNullException("image == null");

            if (mPageNumber != 0)
            {
                mTiff.WriteDirectory();
                mTiff.CreateDirectory();
            }

            mTiff.SetDirectory(mPageNumber);

            mTiff.SetField(TiffTag.IMAGEWIDTH, image.Width);
            mTiff.SetField(TiffTag.IMAGELENGTH, image.Height);

            // These parameters are optional, but they may be required by some TIFF readers:
            // mTiff.SetField(TiffTag.FILLORDER, FillOrder.MSB2LSB);
            // mTiff.SetField(TiffTag.ORIENTATION, Orientation.TOPLEFT);
            // mTiff.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);

            mTiff.SetField(TiffTag.PHOTOMETRIC, mOptions.Photometric);
            mTiff.SetField(TiffTag.COMPRESSION, mOptions.Compression);
            mTiff.SetField(TiffTag.XRESOLUTION, mOptions.XResolution);
            mTiff.SetField(TiffTag.YRESOLUTION, mOptions.YResolution);

            byte[] data;
            int stride;
            switch (mOptions.Compression)
            {
                case Compression.NONE:
                case Compression.LZW:
                    mTiff.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                    mTiff.SetField(TiffTag.BITSPERSAMPLE, 8);
                    mTiff.SetField(TiffTag.SAMPLESPERPIXEL, 4);

                    stride = image.Width * 4;
                    data = ConvertToRgba(image);
                    break;
                case Compression.CCITTFAX3:
                case Compression.CCITTFAX4:
                case Compression.CCITTRLE:
                    Debug.Assert(image.ColorType == SKColorType.Gray8);

                    if (mOptions.Compression == Compression.CCITTFAX4)
                        mTiff.SetField(TiffTag.GROUP4OPTIONS, 0);

                    if (mOptions.Compression == Compression.CCITTFAX3)
                        mTiff.SetField(TiffTag.GROUP3OPTIONS, 1);

                    mTiff.SetField(TiffTag.SAMPLESPERPIXEL, 1);

                    stride = (image.Width + 7) / 8;
                    data = ConvertTo1Bpp(image);
                    break;
                //TODO: Compression.JPEG is currently not used, but the following draft code has been prepared.
                //case Compression.JPEG:
                //    mTiff.SetField(TiffTag.ROWSPERSTRIP, image.Height);
                //    mTiff.SetField(TiffTag.SAMPLESPERPIXEL, 3);
                //    Need to implement a method similar to an ConvertToRgba method, but the output color must be 24 bit.
                //    SKColorType does not have such format, so it will be necessary to remove alpha channel manually.
                //    stride = image.Width * 3;
                //    data = ConvertToRgb(image);
                //    break;
                default:
                    throw new ArgumentException("Tiff compression not supported.");
            }


            for (int i = 0, offset = 0; i < image.Height; i++)
            {
                bool success = mTiff.WriteScanline(data, offset, i, 0);
                if (!success)
                    throw new ArgumentException("LibTiff write error!");

                offset += stride;
            }

            mPageNumber++;
        }

        private static byte[] ConvertToRgba(SKBitmap image)
        {
            if (image.ColorType == SKColorType.Rgba8888)
                return CreateImageData(image.GetPixels(), image.Info.BytesSize);

            if (image.ColorType == SKColorType.Bgra8888)
            {
                // BGRA -> RGBA
                IntPtr pixels = image.GetPixels();
                SKSwizzle.SwapRedBlue(pixels, image.Width * image.Height);
                return CreateImageData(pixels, image.Info.BytesSize);
            }

            // This part of the method will be rarely executed, because Skia always loads images as Bgra8888, Gray8 or Alpha8.
            // So it could be invoked only for Grayscale image encoded as Tiff RGB image.
            // Perhaps in the future, this part could be optimized by using custom color channel conversion.
            using (SKBitmap tmp = new SKBitmap(image.Width, image.Height, SKColorType.Rgba8888, image.AlphaType))
            {
                if (image.CopyTo(tmp, SKColorType.Rgba8888))
                    return CreateImageData(tmp.GetPixels(), tmp.Info.BytesSize);
                else
                    throw new ArgumentOutOfRangeException("Not supported ColorType.");
            }
        }

        private static byte[] CreateImageData(IntPtr ptr, int size)
        {
            byte[] data = new byte[size];
            Marshal.Copy(ptr, data, 0, size);
            return data;
        }

        private static byte[] ConvertTo1Bpp(SKBitmap image)
        {
            int width  = image.Width;
            int height = image.Height;
            int stride = (image.Width + 7) / 8;
            IntPtr pixels = image.GetPixels();

            // This method converts a byte array to a bit array, and
            // byte array must contain only values 0 and 255(so the image is already Bitonal).
            byte[] result = new byte[stride * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Calculate the index in the pixels array and get the grayscale value
                    int grayIndex = y * width + x; 
                    byte grayValue = Marshal.ReadByte(pixels, grayIndex);

                    // Calculate the index in the result array
                    int resultIndex = y * stride + x / 8;
                    // Create a mask for setting the bit
                    byte mask = (byte)(0x80 >> (x % 8)); 

                    // Set the bit if the grayscale value is byte.MaxValue
                    if (grayValue == byte.MaxValue)
                        result[resultIndex] |= mask;
                }
            }

            return result;
        }

        public void Dispose()
        {
            if (mTiff != null)
                mTiff.Dispose();
        }

        private LibTiff mTiff;
        private TiffEncoderOptions mOptions;
        private short mPageNumber;
    }
}
#endif
