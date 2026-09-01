// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/05/2006 by Dmitry Vorobyev
#if NETSTANDARD || NET

using System;
using System.IO;
using Aspose.Images.Filters;
using Aspose.Images.Pal.Graphics.Encoder.Tiff;
using BitMiracle.LibTiff.Classic;
using SkiaSharp;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// This class is used to store a temporary TIFF data between method calls when saving a multipage TIFF.
    /// </summary>
    public class TiffWriterPal : IDisposable
    {
        public void Dispose()
        {
            if (mTiffEncoder != null)
                mTiffEncoder.Dispose();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Saves an image in TIFF format to a stream as the first frame.
        ///
        /// Note: <see cref="pixelFormat"/> parameter is currently ignored.
        /// </summary>
        public void SaveFirstFrame(
            Stream stream,
            TiffCompressionCore compression,
            ImageBinarizationMethodCore binarizationMethod,
            byte threshold,
            System.Drawing.Imaging.PixelFormat pixelFormat,
            BitmapPal bitmap,
            bool isMultiframe)
        {
            mStream = stream;
            mCompression = compression;
            mBinarizationMethod = binarizationMethod;
            mThreshold = threshold;
            mFirstFrame = ConvertTo1BppIfRequired(bitmap, mCompression, binarizationMethod);

            TiffEncoderOptions options = CreateEncoderOptions(bitmap);
            mTiffEncoder = new TiffEncoder(stream, options);

            mTiffEncoder.Write(mFirstFrame);
        }

        /// <summary>
        /// Saves an image in TIFF format to a stream as an intermediate frame.
        /// </summary>
        public void SaveIntermediateFrame(BitmapPal frame)
        {
            SKBitmap frameToSave = ConvertTo1BppIfRequired(frame, mCompression, mBinarizationMethod);
            mTiffEncoder.Write(frameToSave);
        }

        public void FlushMultiframe()
        {
            // Not implemented yet.
        }

        private SKBitmap ConvertTo1BppIfRequired(BitmapPal bitmap, TiffCompressionCore compression, ImageBinarizationMethodCore binarizationMethod)
        {
            switch (compression)
            {
                case TiffCompressionCore.Ccitt3:
                case TiffCompressionCore.Ccitt4:
                case TiffCompressionCore.Rle:
                    bitmap = GetBitmapDependingFromMethod(binarizationMethod, bitmap);
                    break;
                default:
                {
                    // Other compression types doesn't require binarization.
                    break;
                }
            }
            return bitmap.GetNativeBitmap();
        }

        private TiffEncoderOptions CreateEncoderOptions(BitmapPal frame)
        {
            TiffEncoderOptions options = new TiffEncoderOptions();
            options.XResolution = (int)frame.HorizontalResolution;
            options.YResolution = (int)frame.VerticalResolution;
            options.Compression = GetEncoderValueForTiffCompression(mCompression);
            return options;
        }

        //JAVA-added: create bitmap with inverted color model
        // (0 bit for White and 1 bit for Black)
        private BitmapPal InvertColorModel(BitmapPal bitmap)
        {
            throw new NotImplementedException();
        }

        private static BitmapPal GetBitmapDependingFromMethod(ImageBinarizationMethodCore binarizationMethod, BitmapPal bitmap)
        {
            BitmapPal result;
            switch (binarizationMethod)
            {
                case ImageBinarizationMethodCore.Threshold:
                    using (bitmap)
                    {
                        // Color inversion is required.
                        SKBitmap bitonalBitmap = new BitonalConverter().ConvertToBitonal(bitmap.GetNativeBitmap(), true);
                        result = new BitmapPal(bitonalBitmap, bitmap.HorizontalResolution, bitmap.VerticalResolution);
                        break;
                    }
                case ImageBinarizationMethodCore.FloydSteinbergDithering:
                    using (bitmap)
                    {
                        SKBitmap bufferedImage = GetDitheredImage(bitmap.GetNativeBitmap());
                        result = new BitmapPal(bufferedImage);
                        break;
                    }
                default:
                    throw new ArgumentException("Parameter name: " + "binarizationMethod");
            }
            return result;
        }

        private static SKBitmap GetDitheredImage(SKBitmap bitmap)
        {
            // Source image has to be converted to grayscale first,
            // then dithering algorithm should be applied.
            // Grayscale accepts 24 or 32 bpp images only.
            // Convert to such format if needed.
            // Get grayscale image.
            using (bitmap)
            {
                SKBitmap grayScale = Grayscale.RMY().Apply(bitmap);

                // Binarize image using Floyd-Steinberg dithering algorithm.
                SKBitmap result = new FloydSteinbergColorDithering().Apply(grayScale);
                return result;
            }
        }

        private static Compression GetEncoderValueForTiffCompression(TiffCompressionCore compression)
        {
            switch (compression)
            {
                case TiffCompressionCore.None:
                    return Compression.NONE;
                case TiffCompressionCore.Rle:
                    return Compression.CCITTRLE;
                case TiffCompressionCore.Lzw:
                    return Compression.LZW;
                case TiffCompressionCore.Ccitt3:
                    return Compression.CCITTFAX3;
                case TiffCompressionCore.Ccitt4:
                    return Compression.CCITTFAX4;
                case TiffCompressionCore.Jpeg:
                    return Compression.JPEG;
                default:
                    throw new ArgumentException("compression: " + compression);
            }
        }

        private Stream mStream;
        private TiffCompressionCore mCompression;
        private ImageBinarizationMethodCore mBinarizationMethod;
        private byte mThreshold;
        private SKBitmap mFirstFrame;
        private TiffEncoder mTiffEncoder;
    }
}
#endif
