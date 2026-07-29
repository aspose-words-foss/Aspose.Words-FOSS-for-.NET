// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/08/2010 by Roman Korchagin

#if NETSTANDARD

using System;
using System.Drawing;
using System.IO;
using Aspose.Drawing;
using Aspose.Images.Pal.Graphics;
using SkiaSharp;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// Port this class manually to Java.
    /// </summary>
    [System.Security.SecuritySafeCritical]
    // RK This is required for .NET 4.0. Without it, throws "Security transparent method calls a security critical method."
    public class BitmapPal : IDisposable
    {
        /// <summary>
        /// Creates a bitmap of the specified width and height in pixels.
        /// The resolution is 96dpi.
        /// The color model is 32bpp ARGB.
        /// </summary>
        public BitmapPal(int width, int height)
            : this(width, height, ImageConstants.StandardResolution, ImageConstants.StandardResolution)
        {
        }

        /// <summary>
        /// Creates a bitmap of the specified width and height in pixels and resolution in the 32bpp ARGB
        /// pixel format and color model.
        /// </summary>
        public BitmapPal(int width, int height, float hRes, float vRes)
            : this(width, height, hRes, vRes, DefaultColorType)
        {
        }

        /// <summary>
        /// Creates a bitmap of the specified width and height in pixels and resolution in the specified pixel format.
        /// </summary>
        public BitmapPal(int width, int height, float hRes, float vRes,
            System.Drawing.Imaging.PixelFormat pixelFormatCore)
            : this(width, height, hRes, vRes, BitmapCorePal.GetBitmapType(pixelFormatCore))
        {
        }

        public BitmapPal(int width, int height, System.Drawing.Imaging.PixelFormat pixelFormat, byte[] bytes, DrColor[] palette)
        {
            try
            {
                // Stride is number of bytes per row. Calculate it by dividing bytes length to height.
                int stride = (bytes.Length / height);

                SKBitmap result = new SKBitmap(width, height, DefaultColorType, SKAlphaType.Premul);
                SKColor[] pixels = BitmapCorePal.ToSkRgbaPixels(bytes, stride, width, height);
                result.Pixels = pixels;

                //result.SetPixels(pixels, 0, stride, 0, 0, width, height);
                ImageSizeCore size = ImageSizeCore.CreateWithResolution(width, height, ImageConstants.StandardResolution,
                    ImageConstants.StandardResolution);
                SetBitmap(result, FileFormat.Bmp, size);
            }
            catch (Exception originalException)
            {
                throw new CantCreateBitmapException(originalException);
            }
        }

        /// <summary>
        /// Loads a bitmap from image bytes.
        /// </summary>
        public BitmapPal(byte[] imageBytes) : this(new MemoryStream(imageBytes))
        {
        }

        /// <summary>
        /// Loads a bitmap from stream. The stream should be disposed by caller.
        /// </summary>
        public BitmapPal(Stream imageStream) : this(imageStream, true)
        {
        }

        public BitmapPal(Stream imageStream, bool emulateRasterOperations)
        {
            FileFormat imageType = ImageUtil.GetImageType(imageStream);
            if (imageType == FileFormat.Unknown && ImageUtil.IsDib(imageStream))
            {
                BinaryReader reader = new BinaryReader(imageStream);
                byte[] imageBytes = ImageUtil.PrependBmpHeader(reader, (int)imageStream.Length);
                imageType = ImageUtil.GetImageType(imageBytes);
                // Dispose the original stream and reset it to memory stream with bmp header prepended.
                imageStream.Dispose();
                imageStream = new MemoryStream(imageBytes);
            }

            if (!ImageUtil.IsImage(imageType))
                throw new CantCreateBitmapException("Parameter is not valid. FileFormat=" + imageType);

            // Check whether the original image is CMYK or YCCK image.
            // Required to determine whether the image can be used as is in PDF. See WORDSNET-22504.
            mWasCmykOrYcck = ImageUtil.IsCmykOrYCCK(imageStream);

            ImageSizeCore imageSize = ImageUtil.GetImageSize(imageStream, imageType);
            bool isDownSampled;
            SKBitmap bufferedImage = BitmapCorePal.ImageFromStream(imageStream, imageType, emulateRasterOperations, out isDownSampled);
            // If image was downsampled imagesize should be updated accordingly.
            if (isDownSampled)
            {
                // For metafiles ise zero resolution to set isOriginalresolutionZero flag.
                double resolution = ImageUtil.IsMetafile(imageType) ? 0.0d : ImageConstants.StandardResolution;
                imageSize = ImageSizeCore.CreateWithResolution(bufferedImage.Width, bufferedImage.Height, resolution, resolution);
            }

            SetBitmapInternal(bufferedImage, imageType, imageSize);
            SetIsOriginalResolutionZero(imageSize, imageType);
        }

        public BitmapPal(SKBitmap image)
            : this(image, ImageConstants.StandardResolution, ImageConstants.StandardResolution)
        {
        }

        public BitmapPal(SKBitmap image, float hRes, float vRes)
        {
            SetBitmapInternal(image, FileFormat.Bmp, ImageSizeCore.CreateWithResolution(image.Width, image.Height, hRes, vRes));
        }

        /// <summary>
        /// Creates a bitmap of the specified width and height in pixels and resolution in the specified native pixel format.
        /// </summary>
        private BitmapPal(int width, int height, float hRes, float vRes, SKColorType pixelFormat)
        {
            SKBitmap image;
            try
            {
                image = new SKBitmap(width, height, pixelFormat, SKAlphaType.Premul);
                using (SKCanvas c = new SKCanvas(image))
                    c.Clear(); // Clear with transparent white.
            }
            catch (Exception originalException)
            {
                throw new CantCreateBitmapException(originalException);
            }

            SetBitmapInternal(image, FileFormat.Bmp, ImageSizeCore.CreateWithResolution(width, height, hRes, vRes));
        }

        /// <summary>
        /// This implementation is a streamlined version of the FxCop rule
        /// http://msdn.microsoft.com/en-us/library/ms244737(VS.80).aspx
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Added to make code compliant
        protected virtual void Dispose(bool disposing)
        {
            Close();
        }

        /// <summary>
        /// The same as Dispose.
        /// </summary>
        public void Close()
        {
            if (mBitmap != null)
            {
                // According to Android documentation Bitmap.Recycle() should be enough to dispose the object,
                // calling Dispose() method is not required.
                mBitmap.Dispose();
                mBitmap = null;
            }
        }

        /// <summary>
        /// Returns the color model that will be used when <see cref="GetPixels"/> is called.
        /// The parsing color model is normally the same as the underlying bitmap color model.
        ///
        /// But it might be different. For example on .NET if we encounter a 16bit gray scale image,
        /// then we will request its data as 24bit RGB for pixel parsing.
        /// </summary>
        public ColorModel GetParsingColorModel()
        {
            return BitmapCorePal.GetColorModel(mBitmap.ColorType);
        }

        /// <summary>
        /// Gets the native object that this wrapper encapsulates. Should be the native type in each platform implementation.
        /// </summary>
        public SKBitmap GetNativeBitmap()
        {
            return mBitmap;
        }

        public void SetPixel(int x, int y, DrColor color)
        {
            SetPixel(x, y, color.ToArgb());
        }

        public void SetPixel(int x, int y, int color)
        {
            SetPixel(x, y, new SKColor((uint)color));
        }

        public void SetPixel(int x, int y, SKColor color)
        {
            mBitmap.SetPixel(x, y, color); //color.toArgb());
        }

        public int GetPixel(int x, int y)
        {
            // With SkiaSharp 1.6.1 workaround for Android is not required anymore.
            SKColor p = mBitmap.GetPixel(x, y);
            return new DrColor(p.Alpha, p.Red, p.Green, p.Blue).ToArgb();
        }

        /// <summary>
        /// Draws this bitmap onto another bitmap. Allows to positively crop and to scale the image.
        /// </summary>
        public void DrawImage(Rectangle srcRect, BitmapPal dstBitmap, Rectangle dstRect)
        {
            BitmapCorePal.DrawImage(mBitmap, srcRect, dstBitmap.mBitmap, dstRect);
        }

        /// <summary>
        /// Positively crops (only making the image smaller) the image and returns the new image.
        /// The new image has the same resolution as the original image.
        /// </summary>
        public BitmapPal PositivelyCrop(Rectangle srcRect)
        {
            // This is fix for TestJira14121. It seems crop is not required so return simple copy of current bitmap.
            if (srcRect.Location.IsEmpty &&
                MathUtil.AreEqual(mBitmap.Width, srcRect.Width) &&
                MathUtil.AreEqual(mBitmap.Height, srcRect.Height))
                return new BitmapPal(mBitmap.Copy());

            ConvertToDefaultColorType();

            // Define the crop area.
            SKRectI cropArea = new SKRectI(srcRect.Left, srcRect.Top, srcRect.Right, srcRect.Bottom);

            // Create a new bitmap to store the cropped image.
            SKBitmap croppedBitmap = new SKBitmap(cropArea.Width, cropArea.Height);

            // Extract the subset (cropped region).
            bool result = mBitmap.ExtractSubset(croppedBitmap, cropArea);

            // WORDSNET-27428
            // Due to limitations of SkiaSharp, it is not possible to preserve the original image type (GIF)
            // during conversion. As a result, the original larger image is rendered into a smaller rectangle,
            // which results in vertical compression.
            // Also GIF images lose transparency for some reason. Use PNG instead.
            BitmapPal dstBitmap = new BitmapPal(croppedBitmap);
            dstBitmap.ImageType = (ImageType == FileFormat.Gif)
                ? FileFormat.Png
                : ImageType; // Preserve the original image type.

            return dstBitmap;
        }

        /// <summary>
        /// Scales the specified crop rectangle of the bitmap to specified size.
        /// </summary>
        public BitmapPal Scale(Rectangle cropRect, Size dstSize, float hRes, float vRes)
        {
            Rectangle srcRect = Rectangle.Intersect(GetBounds(), cropRect);

            SKBitmap bmp;
            using (SKImage image = SKImage.FromBitmap(mBitmap))
            using (SKImage croppedImage = image.Subset(new SKRectI(srcRect.Left, srcRect.Top, srcRect.Right, srcRect.Bottom)))
                bmp = SKBitmap.FromImage(croppedImage);

            bmp = BitmapCorePal.GetScaledInstance(bmp, dstSize.Width, dstSize.Height);
            return new BitmapPal(bmp, hRes, vRes);
        }

        /// <summary>
        /// Returns image bounds.
        /// </summary>
        public Rectangle GetBounds()
        {
            return new Rectangle(0, 0, mBitmap.Width, mBitmap.Height);
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        public void Save(string fileName, FileFormat imageType)
        {
            using (Stream stream = File.Create(fileName))
            {
                Save(stream, imageType);
            }
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        public void Save(Stream stream, FileFormat imageType)
        {
            switch (imageType)
            {
                case FileFormat.Tiff:
                    // alexnosk: Originally Jpeg compression was used here, but it is not supported yet, so use Lzw.
                    // 128 - Default value threshold for ImageBinarizationMethodCore.FloydSteinbergDithering
                    SaveTiff(stream, TiffCompressionCore.Lzw, ImageBinarizationMethodCore.Threshold, 128);
                    break;
                case FileFormat.Png:
                    SavePng(stream);
                    break;
                case FileFormat.Jpeg:
                    // GDI+ has default quality close to this value. I am setting it explicitly here because
                    // I hope this will help to make some Java JPEGs more similar to .NET.
                    SaveJpeg(stream, 80);
                    break;
                case FileFormat.Bmp:
                    SaveBmp(stream);
                    break;
                default:
                    throw new InvalidOperationException("Cannot save in this image format.");
            }
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="jpegQuality">Valid range is 0-100.</param>
        public void SaveJpeg(Stream stream, int jpegQuality)
        {
            SetStandardResolutionIfOriginalResolutionWasZero();
            ImageWriterHelper.SaveJpegCore(this, stream, jpegQuality);
        }

        public void SavePng(Stream stream)
        {
            SetStandardResolutionIfOriginalResolutionWasZero();
            ImageWriterHelper.SavePngCore(this, stream);
        }

        public void SaveWebP(Stream stream)
        {
            SetStandardResolutionIfOriginalResolutionWasZero();
            ImageWriterHelper.SaveWebPCore(this, stream);
        }

        public void SaveBmp(Stream stream)
        {
            ImageWriterHelper.SaveBmpCore(this, stream);
        }

        /// <summary>
        /// Saves a single frame TIFF.
        /// </summary>
        public void SaveTiff(Stream stream, TiffCompressionCore compression, ImageBinarizationMethodCore binarizationMethod, byte threshold)
        {
            SaveTiff(stream, compression, binarizationMethod, threshold, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        /// <summary>
        /// Saves a single frame TIFF.
        ///
        /// Note <see cref="pixelFormat"/> parameter is currently ignored.
        /// </summary>
        public void SaveTiff(Stream stream, TiffCompressionCore compression, ImageBinarizationMethodCore binarizationMethod, byte threshold, System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            ImageWriterHelper.SaveTiffCore(this, stream, compression, binarizationMethod, threshold);
        }

        public void SaveGif(Stream stream)
        {
            ImageWriterHelper.SaveGifCore(this, stream);
        }

        /// <summary>
        /// Saves an image into a stream in a format that will satisfy MS Word 97.
        ///
        /// In .NET the image parameter is a GDI+ Image, but in Java the image parameter is Bitmap.
        ///
        /// This is a static method (not an instance method) because in .NET it accepts a <see cref="SKBitmap"/> parameter
        /// that can either be a metafile or bitmap and saves it accordingly.
        /// </summary>
        public static void SaveNativeImageForWord97(SKBitmap image, Stream stream)
        {
            SKBitmap img = image;

            // In .NET this code worked by using the original image format obtained from the image object.
            // In Java I don't think we can get the original image format and therefore let's just always save as PNG.
            BitmapPal bitmap = new BitmapPal(img);
            bitmap.SavePng(stream);
        }

        /// <summary>
        /// RK This Java implementation just returns the original image bytes.
        ///
        /// Fix rendering for certain BMP files when running on Mono.
        /// Safe to call for all image bytes, only does its job when needed.
        ///
        /// AM This is "hack" for libgdiplus bmpcodec bug.
        /// libgdiplus render certain BMP files Y-flipped and X-shifted.
        /// I think that we shouldn't do it in every renderer because it need the same code and
        /// will require more changes when this bug in libgdiplus will fixed and this workaround should be removed.
        ///
        /// Do Y-Flip and X-shift for certain 16bpp BMP formats which determined by file header information.
        /// </summary>
        /// <param name="imageBytes">Original raw image bytes</param>
        /// <returns>Fixed raw image bytes</returns>
        public static byte[] MonoBmpFix(byte[] imageBytes)
        {
            return imageBytes;
        }

        /// <summary>
        /// Extracts all the pixel color values into byte arrays and returns in a huge memory object.
        /// Also uses chroma key in alpha values calculation.
        /// </summary>
        /// <param name="isConvertTo1Bpp">If true, causes the image pixels to be returned as 1bpp black and white.</param>
        public DrPixels GetPixels(bool isConvertTo1Bpp)
        {
            if (isConvertTo1Bpp)
            {
                BitonalConverter bitonalConverter = new BitonalConverter();
                byte[] colorValues = bitonalConverter.ConvertToBitonalBytes(mBitmap);
                return new DrPixels(colorValues, null, false, ColorModel.GrayScale, 1, null);
            }
            else
            {
                switch (BitmapCorePal.GetColorModel(mBitmap.ColorType))
                {
                    case ColorModel.Rgb:
                        return BitmapCorePal.ParseRgb(mBitmap, false);
                    case ColorModel.Indexed:
                        return BitmapCorePal.ParseIndexed(mBitmap);
                    case ColorModel.GrayScale:
                        return BitmapCorePal.ParseGrayScale(mBitmap);
                    default:
                        throw new InvalidOperationException("Unknown color space.");
                }
            }
        }

        /// <summary>
        /// Converts the bitmap to the 32BppArgb pixel format if needed and returns its pixels.
        /// </summary>
        public DrPixels Get32BppArgbPixels()
        {
            using (BitmapPal converted = ConvertTo32BppArgb())
            {
                return converted.GetPixels(false);
            }
        }

        /// <summary>
        /// Removes image transparency by drawing the image on a canvas of the specified color.
        /// </summary>
        public void RemoveTransparency(DrColor backgroundColor)
        {
            // Let's use white if an empty color is specified, otherwise a black background is got.
            if ((backgroundColor == null) || (backgroundColor == DrColor.Empty))
                backgroundColor = DrColor.White;

            BitmapPal bitmap = new BitmapPal(Width, Height, HorizontalResolution, VerticalResolution, mBitmap.ColorType);

            Rectangle rect = GetBounds();
            BitmapCorePal.DrawImage(mBitmap, rect, bitmap.mBitmap, rect, backgroundColor);

            mBitmap.Dispose();
            mBitmap = bitmap.mBitmap;
            bitmap.mBitmap = null;
        }

        /// <summary>
        /// Converts the underlying native bitmap to default color type.
        /// </summary>
        public void ConvertToDefaultColorType()
        {
            // Convert bitmap to Rgba8888, because BitmapDataPal can work only with this color type at the moment.
            mBitmap = ConvertToDefaultColorType(mBitmap);
        }

        /// <summary>
        /// Converts the underlying native bitmap to 1bpp.
        /// SkiaSharp does not allow to create 1bpp images, so this method converts to Gray8.
        /// </summary>
        public void ConvertToBitonal()
        {
            BitonalConverter bitonalConverter = new BitonalConverter();
            mBitmap = bitonalConverter.ConvertToBitonal(mBitmap);
        }

        /// <summary>
        /// Used by PdfImage to check whether original image bytes can be saved without transformations. If this
        /// returns true - original bytes should be converted before save since pdf doesn't understand cmyks.
        /// </summary>
        public bool IsCmykPixelFormat()
        {
            // Determined upon loading the image.
            return mWasCmykOrYcck;
        }

        /// <summary>
        /// Returns true if bitmap data is in 32bpp pixel format.
        /// </summary>
        public bool Is32BppArgbPixelFormat()
        {
            return mBitmap.ColorType == SKColorType.Bgra8888;
        }

        /// <summary>
        /// Locks bits of this bitmap and returns the corresponding bitmap data.
        /// Note: currently works only for Rgba images.
        /// </summary>
        public BitmapDataPal LockBits()
        {
            // Convert bitmap to Rgba8888, because BitmapDataPal can work only with this color type at the moment.
            mBitmap = ConvertToDefaultColorType(mBitmap);
            return new BitmapDataPal(mBitmap);
        }

        /// <summary>
        /// Converts the specified bitmap to the specified color type.
        /// Disposes the original bitmap.
        /// </summary>
        internal static SKBitmap ConvertToDefaultColorType(SKBitmap src)
        {
            if ((src.ColorType == DefaultColorType) && !src.Info.IsOpaque)
                return src;

            using (src)
            {
                return CopyToDefaultColorType(src);
            }
        }

        /// <summary>
        /// Converts the specified bitmap to the specified color type.
        /// Does not dispose the original image. Normally you should use <see cref="ConvertToDefaultColorType"/>.
        /// </summary>
        private static SKBitmap CopyToDefaultColorType(SKBitmap src)
        {
            if ((src.ColorType == DefaultColorType) && !src.Info.IsOpaque)
                return src.Copy();

            SKBitmap res = null;
            if (src.CanCopyTo(DefaultColorType) && !src.Info.IsOpaque)
                res = src.Copy(DefaultColorType);

            if (res != null)
                return res;

            SKBitmap result = new SKBitmap(src.Width, src.Height, DefaultColorType, SKAlphaType.Unpremul);
            using (SKCanvas g = new SKCanvas(result))
            using (SKPaint p = new SKPaint())
            {
                GraphicsQualityOptions.ApplyDefault(p);
                g.DrawBitmap(src, 0, 0, p);
            }

            return result;
        }

        /// <summary>
        /// Loads a native image from a file.
        /// </summary>
        public static SKBitmap LoadNativeImage(string fileName)
        {
            using (Stream stream = File.OpenRead(fileName))
                return LoadNativeImage(stream);
        }

        /// <summary>
        /// Loads a native image from stream.
        /// </summary>
        public static SKBitmap LoadNativeImage(Stream stream)
        {
            BitmapPal bitmap = new BitmapPal(stream);
            return bitmap.GetNativeBitmap();
        }

        /// <summary>
        /// Returns true if image has alpha channel.
        /// </summary>
        public static bool ImageHasAlphaChannel(byte[] imageBytes)
        {
            if (ImageUtil.IsMetafile(ImageUtil.GetImageType(imageBytes)))
                return true;

            using (SKBitmap bmp = SKBitmap.Decode(imageBytes))
            {
                return BitmapCorePal.ImageHasAlphaChannel(bmp);
            }
        }

        /// <summary>
        /// Converts the bitmap to 32BppArgb pixel format.
        /// <remarks>
        /// If the bitmap is in 32BppArgb pixel format, returns this one, otherwise creates a new image.
        /// </remarks>
        /// </summary>
        internal BitmapPal ConvertTo32BppArgb()
        {
            //PixelFormat.FORMAT_32_BPP_ARGB => Bitmap.Bitmap.Config.ARGB_8888
            // Convert only if original bitmap is not already in 32 BPP ARGB format.
            if (mBitmap.ColorType == DefaultColorType)
                return this;

            // Not required to dispose original bitmap after conversion, because method create new BitmapPal.
            SKBitmap newBitmap = ConvertTo(mBitmap, GetBounds(), DefaultColorType, false);
            BitmapPal converted = new BitmapPal(newBitmap);

            return converted;
        }

        /// <summary>
        /// Converts this bitmap into a different bpp / color bitmap using Java's conversion.
        /// </summary>
        private SKBitmap ConvertTo(SKBitmap bitmap, Rectangle bounds, SKColorType colorType, bool disposeOriginal)
        {
            SKBitmap newBitmap = new SKBitmap(bitmap.Width, bitmap.Height, colorType, SKAlphaType.Premul);
            BitmapCorePal.DrawImage(bitmap, bounds, newBitmap, bounds);

            // Dispose original bitmap if required.
            if (disposeOriginal)
                bitmap.Dispose();

            return newBitmap;
        }

        /// <summary>
        /// RK It can happen when the resolution returned from the original image bytes is different
        /// from the resolution returned by the GDI+ for an image that was loaded from those bytes.
        ///
        /// This could happen for example, when the resolution was not specified in the original image bytes
        /// and we default to 96dpi. But creating a GDI+ image from such bytes will set image resolution to
        /// the current desktop resolution and it could be 120dpi for example. This will cause XPS and possibly
        /// other tests to fail because the output will be dependent on the desktop resolution.
        ///
        /// So here we detect that situation and change the bitmap resolution to what we expect it to be,
        /// but to properly set the bitmap resolution we actually have to create a new bitmap.
        /// This method disposes the original bitmap.
        /// </summary>
        private void SetStandardResolutionIfOriginalResolutionWasZero()
        {
            if (mIsOriginalResolutionZero)
            {
                mImageSize = ImageSizeCore.CreateWithResolution(mBitmap.Width, mBitmap.Height,
                    ImageConstants.StandardResolution, ImageConstants.StandardResolution);
                mIsOriginalResolutionZero = false;
            }
        }

        private void SetBitmapInternal(SKBitmap bitmap, FileFormat imageType, ImageSizeCore imageSize)
        {
            if (bitmap.Width < imageSize.Width || bitmap.Height < imageSize.Height)
                bitmap = ResizeCanvas(bitmap, imageSize);

            SetBitmap(bitmap, imageType, imageSize);
        }

        // JAVA-added: FIX WORDSJAVA-932: resize canvas to match size specified in image header
        private static SKBitmap ResizeCanvas(SKBitmap input, ImageSizeCore imageSize)
        {
            int wOut = imageSize.Width;
            int hOut = imageSize.Height;
            int wIn = input.Width;
            int hIn = input.Height;
            int left = wOut - wIn / 2;
            int top = hOut - hIn / 2;
            if (left > 0)
                left--;

            SKBitmap output = new SKBitmap(wOut, hOut, DefaultColorType, SKAlphaType.Premul);
            using (SKCanvas g = new SKCanvas(output))
            {
                // Clear graphics before drawing.
                g.Clear();
                using (SKPaint paint = new SKPaint())
                {
                    GraphicsQualityOptions.ApplyDefault(paint);
                    paint.BlendMode = SKBlendMode.SrcOver;
                    g.DrawBitmap(input, left, top, paint);
                }
            }
            return output;
        }

        private void SetBitmap(SKBitmap bitmap, FileFormat imageType, ImageSizeCore imageSize)
        {
            mBitmap = bitmap;
            mImageType = imageType;
            mImageSize = imageSize;
        }

        //JAVA-changed: we already have imageSize and imageType - no need to calculate them from imageBytes.
        private void SetIsOriginalResolutionZero(ImageSizeCore imageSize, FileFormat imageType)
        {
            mIsOriginalResolutionZero = ImageUtil.IsMetafile(imageType) || imageSize.IsOriginalResolutionZero;
        }

        /// <summary>
        /// Width in pixels.
        /// </summary>
        public int Width
        {
            get { return mBitmap.Width; }
        }

        /// <summary>
        /// Height in pixels.
        /// </summary>
        public int Height
        {
            get { return mBitmap.Height; }
        }

        /// <summary>
        /// Returns the resolution that is resilient to Windows desktop resolution.
        /// </summary>
        public float HorizontalResolution
        {
            get
            {
                return mIsOriginalResolutionZero
                    ? ImageConstants.StandardResolution
                    : (float)mImageSize.HorizontalResolution;
            }
        }

        /// <summary>
        /// Returns the resolution that is resilient to Windows desktop resolution.
        /// </summary>
        public float VerticalResolution
        {
            get
            {
                return mIsOriginalResolutionZero
                    ? ImageConstants.StandardResolution
                    : (float)mImageSize.VerticalResolution;
            }
        }

        /// <summary>
        /// Returns the original format of the image.
        /// </summary>
        public FileFormat ImageType
        {
            get { return mImageType; }
            private set { mImageType = value; }
        }

        /// <summary>
        /// Returns true if image has alpha channel.
        /// </summary>
        public bool HasAlphaChannel
        {
            get { return BitmapCorePal.ImageHasAlphaChannel(mBitmap); }
        }

        static BitmapPal()
        {
            // FIX WORDSNET-17239 - Some unix like OSs uses Rgba8888 color type some Bgra8888, Create an empty bitmap and let SkiSharp to detect proper color type for us.
            using (SKBitmap bmp = new SKBitmap(10, 10))
                DefaultColorType = bmp.ColorType;
        }

        public static readonly SKColorType DefaultColorType = SKColorType.Rgba8888;

        private SKBitmap mBitmap;
        private FileFormat mImageType;
        private bool mIsOriginalResolutionZero;
        private ImageSizeCore mImageSize;

        private bool mWasCmykOrYcck;
    }
}
#endif
