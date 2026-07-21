// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/05/2006 by Dmitry Vorobyev
#if !NETSTANDARD
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Aspose.Images.Filters;
using Aspose.JavaAttributes;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// This class is used to store a temporary TIFF data between method calls when saving a multipage TIFF.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    public sealed class TiffWriterPal : IDisposable
    {
        public void Dispose()
        {
            if (mIsFirstFrameDisposeRequired && (mFirstFrame != null))
            {
                mFirstFrame.Dispose();
                mFirstFrame = null;
            }
        }
        
        /// <summary>
        /// Saves an image in TIFF format to a stream as the first frame.
        /// </summary>
        public void SaveFirstFrame(
            Stream stream,
            TiffCompressionCore compression,
            ImageBinarizationMethodCore binarizationMethod,
            byte threshold,
            PixelFormat pixelFormat,
            BitmapPal bitmap,
            bool isMultiframe
            )
        {
            Bitmap nativeBitmap = bitmap.GetNativeBitmap();
            mCompression = compression;
            mbBinarizationMethod = binarizationMethod;
            mThreshold = threshold;
            mPixelFormat = pixelFormat;
            
            mFirstFrame = ConvertTo1BppIfRequired(nativeBitmap);
            ImageCodecInfo encoderInfo = ImageCodecInfoRegistry.GetEncoderInfo(ImageFormat.Tiff);
            mEncoderParameters = GetTiffEncoderParameters(compression, isMultiframe);
            mFirstFrame.Save(stream, encoderInfo, mEncoderParameters);

            // If new bitmap was created then it should be disposed.
            // If we use original bitmap then it will be disposed by other code
            mIsFirstFrameDisposeRequired = (mFirstFrame != nativeBitmap);
        }

        /// <summary>
        /// Saves an image in TIFF format to a stream as an intermediate frame.
        /// </summary>
        public void SaveIntermediateFrame(BitmapPal bitmap)
        {
            // RK It is questionable whether dispose for this object is needed. Most likely the first frame also requires dispose
            // and I hope it disposes all intermediate frames too. But I don't know for sure.
            Bitmap frameToSave = ConvertTo1BppIfRequired(bitmap.GetNativeBitmap());
            mEncoderParameters.Param[0] = new EncoderParameter(mEncoderParameters.Param[0].Encoder, (long)EncoderValue.FrameDimensionPage);
            mFirstFrame.SaveAdd(frameToSave, mEncoderParameters);
        }

        /// <summary>
        /// Finishes saving of a multiframe TIFF image.
        /// </summary>
        public void FlushMultiframe()
        {
            mEncoderParameters.Param[0] = new EncoderParameter(mEncoderParameters.Param[0].Encoder, (long)EncoderValue.Flush);
            mFirstFrame.SaveAdd(mEncoderParameters);
        }

        private Bitmap ConvertTo1BppIfRequired(Bitmap bitmap)
        {
            switch (mCompression)
            {
                case TiffCompressionCore.None:
                case TiffCompressionCore.Lzw:
                case TiffCompressionCore.Rle:
                    if (mPixelFormat == PixelFormat.Format1bppIndexed)
                        bitmap = new BitonalConverter().ConvertToBitonal(bitmap);
                    break;
                case TiffCompressionCore.Ccitt3:
                case TiffCompressionCore.Ccitt4:
                    bitmap = GetBitmapDependingFromMethod(bitmap, mbBinarizationMethod, mThreshold);
                    break;
                default:
                    // Other compression types doesn't require binarization.
                    break;
            }
            return bitmap;
        }

        private static Bitmap GetBitmapDependingFromMethod(Bitmap bitmap, ImageBinarizationMethodCore binarizationMethod, byte threshold)
        {
            switch (binarizationMethod)
            {
                case ImageBinarizationMethodCore.Threshold:
                    // WORDSNET-6015.
                    // BitonalConverter produces not very good results on some images.
                    // GDI+ seems to convert bitmap to bitonal itself when needed.
                    // Uncomment the lines below in the case of exceptions.
                    // WORDSNET-6374.
                    // Microsoft seems to change Tiff decoder in Windows 7.
                    // So if the OS is equal or greater than Windows 7 - let GDI+ convert to b&w if needed.
                    // In other cases - use BitonalConverter  for this.
                    if (!PlatformUtilPal.IsOsEqualOrGreaterThenWindows7() &&
                        (bitmap.PixelFormat != PixelFormat.Format1bppIndexed))
                    {
                        bitmap = new BitonalConverter().ConvertToBitonal(bitmap);
                    }
                    break;
                case ImageBinarizationMethodCore.FloydSteinbergDithering:
                    // WORDSNET-6236.
                    // New binarization method is introduced.
                    bitmap = GetDitheredImage(bitmap, threshold);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("binarizationMethod");
            }
            return bitmap;
        }

        private static Bitmap GetDitheredImage(Bitmap bitmap, byte threshold)
        {
            // Source image has to be converted to grayscale first, 
            // then dithering algorithm should be applied.
            // Grayscale accepts 24 or 32 bpp images only.
            // Convert to such format if needed.
            bitmap = ConvertTo32BppIfNeeded(bitmap);
            // Get grayscale image.
            bitmap = Grayscale.RMY().Apply(bitmap);
            // Binarize image using Floyd-Steinberg dithering algorithm.
            FloydSteinbergColorDithering floydSteinbergColorDithering = new FloydSteinbergColorDithering();
            floydSteinbergColorDithering.ThresholdValue = threshold;
            bitmap = floydSteinbergColorDithering.Apply(bitmap);
            return bitmap;
        }

        private static Bitmap ConvertTo32BppIfNeeded(Bitmap bitmap)
        {
            if (bitmap.PixelFormat == PixelFormat.Format24bppRgb || bitmap.PixelFormat == PixelFormat.Format32bppArgb ||
                bitmap.PixelFormat == PixelFormat.Format32bppRgb) return bitmap;

            Bitmap converted = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
            converted.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

            using (Graphics g = BitmapUtilPal.GraphicsFromImage(converted))
                g.DrawImageUnscaled(bitmap, 0, 0);

            return converted;
        }

        /// <summary>
        /// Gets initial TIFF encoder parameters given the desired compression and a value indicating if saved image
        /// is going to be multiframe. 
        /// </summary>
        private static EncoderParameters GetTiffEncoderParameters(TiffCompressionCore compression, bool isMultiframe)
        {
            EncoderParameter compressionParameter = new EncoderParameter(
                Encoder.Compression,
                (long)GetEncoderValueForTiffCompression(compression));

            if (isMultiframe)
            {
                EncoderParameters encoderParams = new EncoderParameters(2);
                encoderParams.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                encoderParams.Param[1] = compressionParameter;

                return encoderParams;
            }
            else
            {
                EncoderParameters encoderParams = new EncoderParameters();
                encoderParams.Param[0] = compressionParameter;

                return encoderParams;
            }
        }

        private static EncoderValue GetEncoderValueForTiffCompression(TiffCompressionCore compression)
        {
            switch (compression)
            {
                case TiffCompressionCore.None:
                    return EncoderValue.CompressionNone;
                case TiffCompressionCore.Rle:
                    return EncoderValue.CompressionRle;
                case TiffCompressionCore.Lzw:
                    return EncoderValue.CompressionLZW;
                case TiffCompressionCore.Ccitt3:
                    return EncoderValue.CompressionCCITT3;
                case TiffCompressionCore.Ccitt4:
                    return EncoderValue.CompressionCCITT4;
                default:
                    throw new ArgumentOutOfRangeException("compression");
            }
        }

        private TiffCompressionCore mCompression;
        private ImageBinarizationMethodCore mbBinarizationMethod;
        private byte mThreshold;
        private PixelFormat mPixelFormat;
        private Image mFirstFrame;
        private EncoderParameters mEncoderParameters;
        private bool mIsFirstFrameDisposeRequired;
    }
}
#endif
