// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/08/2010 by Roman Korchagin

#if !NETSTANDARD
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.IO;
using Aspose.JavaAttributes;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// Port this class manually to Java.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    [System.Security.SecuritySafeCritical] // RK This is required for .NET 4.0. Without it, throws "Security transparent method calls a security critical method."
    public class BitmapPal : ImagePal
    {
        /// <summary>
        /// Creates a bitmap of the specified width and height in pixels.
        /// The resolution is 96dpi.
        /// The color model is 32bpp ARGB.
        /// </summary>
        public BitmapPal(int width, int height) :
            this(width, height, ImageConstants.StandardResolution, ImageConstants.StandardResolution)
        {
        }

        /// <summary>
        /// Creates a bitmap of the specified width and height in pixels and resolution in the 32bpp ARGB
        /// pixel format and color model.
        /// </summary>
        public BitmapPal(int width, int height, float hRes, float vRes) :
            this(width, height, hRes, vRes, PixelFormat.Format32bppArgb)
        {
        }

        public BitmapPal(int width, int height, float hRes, float vRes, PixelFormat pixelFormat)
        {
            // In general, we want to honor the image color model, but most likely the caller wants to create a
            // Graphics object on the bitmap and .NET Graphics can only be created on a bitmap with a non-indexed format.
            // So we use 32bpp ARGB when the source pixel format is indexed.
            if (GetColorModel(pixelFormat) == ColorModel.Indexed)
                pixelFormat = PixelFormat.Format32bppArgb;

            Bitmap bmp;
            try
            {
                bmp = new Bitmap(width, height, pixelFormat);
            }
            catch (Exception originalException)
            {
                throw new CantCreateBitmapException(originalException);
            }

            bmp.SetResolution(hRes, vRes);
            SetBitmapInternal(bmp);
        }

        /// <summary>
        /// Loads a bitmap from image bytes.
        /// What formats are to be supported? Need to list to make sure it works both in .NET and JAVA.
        /// </summary>
        public BitmapPal(byte[] imageBytes)
        {
            BitmapPalCore(new MemoryStream(imageBytes));
        }

        /// <summary>
        /// Loads a bitmap from MemoryStream.
        /// </summary>
        public BitmapPal(MemoryStream imageStream)
        {
            BitmapPalCore(CompressedData.GetStream(imageStream));
        }

        /// <summary>
        /// Disposes image stream.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (mImageStream != null)
                mImageStream.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Creates a bitmap from a stream.
        /// </summary>
        private void BitmapPalCore(Stream stream)
        {
            mImageStream = stream;
            try
            {
                SetBitmapInternal(ImageUtil.TryReadAsBitmap(mImageStream));
            }
            catch (Exception originalException)
            {
                throw new CantCreateBitmapException(originalException);
            }

            mImageStream.Position = 0;
            SetIsOriginalResolutionZero(mImageStream);
        }

        /// <summary>
        /// Creates instance of <see cref="BitmapPal"/> class from a specified Bitmap.
        /// </summary>
        public BitmapPal(Bitmap bitmap)
        {
            SetBitmapInternal(bitmap);
        }

        /// <summary>
        /// Sets a specified Bitmap as source.
        /// </summary>
        /// <param name="bitmap">The Bitmap to set as source.</param>
        private void SetBitmapInternal(Bitmap bitmap)
        {
            Bitmap = bitmap;

            // WORDSNET-6728 Conversation was moved here from SaveJpeg method.
            // WORDSNET-23947 MS Word seems to convert CMYK bitmaps to RGB when writing to PDF so we do the same.
            if (IsCmykPixelFormat())
                ConvertTo24BppRgb(null);

            ImageType = ImageFormatToImageType(Bitmap.RawFormat);
        }

        /// <summary>
        /// Gets or sets internal native bitmap.
        /// </summary>
        private Bitmap Bitmap
        {
            get { return (Bitmap)NativeImage; }
            set { SetNativeImage(value); }
        }

        public PixelFormat PixelFormat
        {
            get
            {
                return Bitmap.PixelFormat;
            }
        }

        /// <summary>
        /// Temporary platform-specific non-optimized implementation of ImageUtil.HasAlphaChannel(byte[]).
        /// </summary>
        public static bool ImageHasAlphaChannel(byte[] imageBytes)
        {
            using (Image image = Image.FromStream(new MemoryStream(imageBytes)))
                return Image.IsAlphaPixelFormat(image.PixelFormat);
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
            return GetColorModel(GetParsingPixelFormat(Bitmap.PixelFormat));
        }

        /// <summary>
        /// Gets the native object that this wrapper encapsulates. Should be the native type in each platform implementation.
        /// </summary>
        public Bitmap GetNativeBitmap()
        {
            return Bitmap;
        }

        /// <summary>
        /// Draws this bitmap onto another bitmap. Allows to positively crop and to scale the image.
        /// </summary>
        public void DrawImage(Rectangle srcRect, BitmapPal dstBitmap, Rectangle dstRect)
        {
            DrawImage(srcRect, dstBitmap, dstRect, null);
        }

        /// <summary>
        /// Draws this bitmap onto another bitmap. Allows to positively crop and to scale the image. The bitmap can be
        /// drawn on a colored canvas.
        /// </summary>
        private void DrawImage(Rectangle srcRect, BitmapPal dstBitmap, Rectangle dstRect, DrColor backgroundColor)
        {
            using (Graphics gr = Graphics.FromImage(dstBitmap.Bitmap))
            {
                if ((backgroundColor != null) && (backgroundColor != DrColor.Empty))
                    gr.Clear(backgroundColor.ToNativeColor());

                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.DrawImage(Bitmap, dstRect, srcRect, GraphicsUnit.Pixel);
            }
        }

        /// <summary>
        /// Positively crops (only making the image smaller) the image and returns the new image.
        /// The new image has the same resolution as the original image.
        /// </summary>
        public BitmapPal PositivelyCrop(Rectangle srcRect)
        {
            // WORDSNET-27377
            // Image manipulation using the System.Drawing.Graphics object destroys the transparency of GIF images.
            // Previously we used  GDI+ Graphics.DrawImage for cropping here. The problem is that GDI+ cannot create
            // Graphics object for indexed image format Bitmap (e.g this happens when the Bitmap is loaded from GIF).
            // So we had to create the destination bitmap with 32-bit ARGB format. When using Graphics.DrawImage
            // the transparency information is converted into alpha channel values which produce the black background
            // when writing to indexed image formats.
            // Fortunately, there is a more direct way to positively crop an image without using the Graphics object.
            Bitmap croppedImage = Bitmap.Clone(srcRect, Bitmap.PixelFormat);

            // WORDSNET-14121 Set image type of the resulting bitmap to the original bitmap's image type.
            BitmapPal result = new BitmapPal(croppedImage);
            result.ImageType = ImageType;

            return result;
        }

        /// <summary>
        /// Scales the specified crop rectangle of the bitmap to specified size.
        /// </summary>
        public BitmapPal ScaleCore(Rectangle srcRect, Size dstSize, float hRes, float vRes, InterpolationMode interpolation)
        {
            // kvk: GDI+ DrawImage method has a bug in the interpolation algorithm.
            // When image is stretched/shrinked DrawImage takes the pixels outside of the cropping rectangle
            // to calculate the border pixels of the resulting image. In case when cropping rectangle equals
            // to the image bounds, pixels outside of the cropping rectangle are considered fully transparent.
            // So in this case the border of the resulting image becomes partially transparent even for fully
            // opaque images. Here is more info about this bug http://www.codeproject.com/Articles/14884/BorderBug.

            // When rendering to PDF (for which this method is used now) transparency is highly undesirable.
            // So to fix the problem use PixelOffsetMode.HighQuality and WrapMode.TileFlipXY.
            // PixelOffsetMode.HighQuality gives better border quality.
            // WrapMode.TileFlipXY prevents transparency of the border.

            // Probably this hack could be used in BitmapPal.DrawImage method which may scale the images too.
            // But I'm not sure that it doesn't have side effects. And there was no complains about the border problem.
            // So use it only for PDF rendering for now.
            BitmapPal dstBitmap = new BitmapPal(dstSize.Width, dstSize.Height, hRes, vRes, Bitmap.PixelFormat);

            using (Graphics gr = Graphics.FromImage(dstBitmap.Bitmap))
            {
                gr.InterpolationMode = interpolation;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetWrapMode(WrapMode.TileFlipXY);
                    gr.DrawImage(Bitmap, dstBitmap.GetBounds(), srcRect.X, srcRect.Y, srcRect.Width,
                                 srcRect.Height, GraphicsUnit.Pixel, attributes);
                }
            }

            return dstBitmap;
        }

        /// <summary>
        /// Returns image bounds.
        /// </summary>
        public Rectangle GetBounds()
        {
            return new Rectangle(0, 0, Bitmap.Width, Bitmap.Height);
        }

        /// <summary>
        /// Saves the image.
        /// </summary>
        public void Save(Stream stream, FileFormat imageType)
        {
            switch (imageType)
            {
                case FileFormat.Tiff:
                    // 128 - Default value threshold for ImageBinarizationMethodCore.FloydSteinbergDithering
                    SaveTiff(stream, TiffCompressionCore.Lzw, ImageBinarizationMethodCore.Threshold, 128, PixelFormat);
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
                case FileFormat.Gif:
                    SaveGif(stream);
                    break;
                case FileFormat.WebP:
                    SaveWebP(stream);
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
            SaveJpegCore(Bitmap, stream, jpegQuality);
        }

        /// <summary>
        /// Saves the image in GIF format into specified stream.
        /// </summary>
        public void SaveGif(Stream stream)
        {
            SetStandardResolutionIfOriginalResolutionWasZero();
            SaveGifCore(Bitmap, stream);
        }

        public void SavePng(Stream stream)
        {
            SetStandardResolutionIfOriginalResolutionWasZero();

            if (PlatformUtilPal.GetPlatform() == Platform.Windows)
            {
                SavePngCore(Bitmap, stream);
            }
            else
            {
                // WORDSNET-12001 MONO. Add image resolution to PNG data manually. (Mono doesn't do it.)
                Stream tmp = new MemoryStream();
                SavePngCore(Bitmap, tmp);
                ImageUtil.SavePngResolution(tmp, stream, HorizontalResolution, VerticalResolution);
            }
        }

        public void SaveBmp(Stream stream)
        {
            SetStandardResolutionIfOriginalResolutionWasZero();
            SaveBmpCore(Bitmap, stream);
        }

        /// <summary>
        /// Saves a single frame TIFF.
        /// </summary>
        public void SaveTiff(
            Stream stream,
            TiffCompressionCore compression,
            ImageBinarizationMethodCore binarizationMethod,
            byte threshold,
            PixelFormat pixelFormat)
        {
            SetStandardResolutionIfOriginalResolutionWasZero();
            using (TiffWriterPal tiffWriter = new TiffWriterPal())
                tiffWriter.SaveFirstFrame(stream, compression, binarizationMethod, threshold, pixelFormat, this, false);
        }

        /// <summary>
        /// Saves the image in WebP format into specified stream.
        /// </summary>
        public void SaveWebP(Stream stream)
        {
            SetStandardResolutionIfOriginalResolutionWasZero();
            SaveWebPCore(Bitmap, stream);
        }

        private static void SaveJpegCore(Image image, Stream stream, int jpegQuality)
        {
            Debug.Assert((jpegQuality >= 0) && (jpegQuality <= 100));

            ImageCodecInfo encoderInfo = ImageCodecInfoRegistry.GetEncoderInfo(ImageFormat.Jpeg);

            using (EncoderParameters encoderParams = new EncoderParameters())
            {
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, jpegQuality);

                // WORDSNET-17404
                // Sometimes an exception occurs:
                // System.Runtime.InteropServices.ExternalException: A generic error occurred in GDI+.
                // It occurs, for example, when the image was loaded from a stream and the stream has already been
                // closed/disposed. At this case image.PropertyItems becomes empty after closing the stream, image
                // metadata is lost.
                // Create a new object in this case solves the exception.
                try
                {
                    image.Save(stream, encoderInfo, encoderParams);
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                    stream.Position = 0;
                    using (Bitmap tempBitmap = new Bitmap(image))
                    {
                        // Let's restore original dpi if 'new Bitmap(image)' changed it to local system one.
                        if (tempBitmap.HorizontalResolution != image.HorizontalResolution ||
                            tempBitmap.VerticalResolution != image.VerticalResolution)
                            tempBitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                        tempBitmap.Save(stream, encoderInfo, encoderParams);
                    }
                }
            }
        }

        private static void SaveGifCore(Image image, Stream stream)
        {
            image.Save(stream, ImageCodecInfoRegistry.GetEncoderInfo(ImageFormat.Gif), null);
        }

        private static void SavePngCore(Image image, Stream stream)
        {
            image.Save(stream, ImageCodecInfoRegistry.GetEncoderInfo(ImageFormat.Png), null);
        }

        private static void SaveBmpCore(Image image, Stream stream)
        {
            image.Save(stream, ImageCodecInfoRegistry.GetEncoderInfo(ImageFormat.Bmp), null);
        }

        private static void SaveWebPCore(Bitmap image, Stream stream)
        {
            WebPConverterPal.WriteBySkia(image, stream);
        }

        /// <summary>
        /// Returns true if specified pixel format supports alpha channel.
        /// </summary>
        private static bool DoesPixelFormatSupportsAlphaChannel(PixelFormat pixelFormat)
        {
            return ((pixelFormat & PixelFormat.Alpha) != 0);
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
                byte[] colorValues = bitonalConverter.ConvertToBitonalBytes(Bitmap);
                return new DrPixels(colorValues, null, false, ColorModel.GrayScale, 1, null);
            }
            else
            {
                BitmapData bitmapData = null;
                try
                {
                    // We might select a pixel format different from the original to make it simpler for us to parse
                    // the pixel data and LockBits will convert data into that format for us.
                    PixelFormat parsingPixelFormat = GetParsingPixelFormat(Bitmap.PixelFormat);

                    // Useful info here http://www.bobpowell.net/lockingbits.htm
                    bitmapData = Bitmap.LockBits(new Rectangle(0, 0, Bitmap.Width, Bitmap.Height),
                        ImageLockMode.ReadOnly, parsingPixelFormat);

                    switch (GetColorModel(parsingPixelFormat))
                    {
                        case ColorModel.Rgb:
                            return ParseRgb(bitmapData, false);
                        case ColorModel.Indexed:
                            return ParseIndexed(bitmapData, Bitmap.Palette);
                        case ColorModel.GrayScale:
                            throw new InvalidOperationException("Have not seen any gray scale images yet.");
                        default:
                            throw new InvalidOperationException("Unknown color space.");
                    }
                }
                finally
                {
                    if (bitmapData != null)
                        Bitmap.UnlockBits(bitmapData);
                }
            }
        }

        /// <summary>
        /// Converts a pixel format into a simpler format that will be used when <see cref="GetPixels"/> is called.
        /// </summary>
        private static PixelFormat GetParsingPixelFormat(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                case PixelFormat.Format4bppIndexed:
                case PixelFormat.Format8bppIndexed:
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                    // These formats we can convert to pixel data.
                    return pixelFormat;

                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                case PixelFormat.Format48bppRgb:
                    // These formats are harder to parse. These formats do not have alpha, so 24bppRgb is good.
                    return PixelFormat.Format24bppRgb;

                case PixelFormat.Format16bppGrayScale:
                    // I have not seen such bitmaps yet, but I don't want to deal with parsing them so I expect
                    // that LockBits will convert such bitmap to 24bit RGB. This changes the color model by the way.
                    return PixelFormat.Format24bppRgb;

                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:
                default:
                    // These formats are harder to parse. These formats have alpha (or we don't know) so 32bppArgb is good.
                    // Images of some other formats such as 24bit CMYK go into here (they don't seem to have defined
                    // PixelFormat values).
                    return PixelFormat.Format32bppArgb;
            }
        }

        private static ColorModel GetColorModel(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                case PixelFormat.Format4bppIndexed:
                case PixelFormat.Format8bppIndexed:
                    return ColorModel.Indexed;
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format16bppRgb555:
                case PixelFormat.Format16bppRgb565:
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format48bppRgb:
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:
                    return ColorModel.Rgb;
                case PixelFormat.Format16bppGrayScale:  // I'm not really sure what this one is about.
                    return ColorModel.GrayScale;
                default:
                    throw new InvalidOperationException("Unknown pixel format.");
            }
        }

        /// <summary>
        /// Convert GDI+ BitmapData into a binary array.
        /// Can only parse 8bit per channel RGB or ARGB data.
        /// </summary>
        private static DrPixels ParseRgb(
            BitmapData bitmapData,
            bool extractOnlyAlphaChannel)
        {
            switch (bitmapData.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                case PixelFormat.Format32bppRgb:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb: // RK I am not sure about this one.
                    // Ok, we can parse these formats.
                    break;
                default:
                    throw new InvalidOperationException("Cannot parse this pixel format.");
            }

            if (bitmapData.Stride < 0)
                throw new InvalidOperationException("A bitmap has a negative stride and this is not yet supported.");

            bool hasAlphaChannel = DoesPixelFormatSupportsAlphaChannel(bitmapData.PixelFormat);

            // If the is no alpha in the image and we need only alpha then we could stop here.
            if (!hasAlphaChannel && extractOnlyAlphaChannel)
                return new DrPixels(ArrayUtil.EmptyByteArray, ArrayUtil.EmptyByteArray, false, ColorModel.Rgb, 8, null);

            // We want exactly 3 color component RGB in this array.
            byte[] colorValues = new byte[extractOnlyAlphaChannel ? 0 : bitmapData.Width * bitmapData.Height * 3];
            byte[] alphaValues = new byte[hasAlphaChannel ? bitmapData.Width * bitmapData.Height : 0];

            bool hasTransparentPixels = false;
            int colorValueIdx = 0;
            int alphaValueIdx = 0;

            // To save memory we process one stride at a time.
            byte[] strideBytes = new byte[bitmapData.Stride];
            long startOfStride = bitmapData.Scan0.ToInt64();

            bool containsUnusedByte = (bitmapData.PixelFormat == PixelFormat.Format32bppRgb);

            int bitmapHeight = bitmapData.Height;
            int bitmapWidth = bitmapData.Width;
            for (int y = 0; y < bitmapHeight; y++)
            {
                Marshal.Copy(new IntPtr(startOfStride), strideBytes, 0, strideBytes.Length);

                int srcIdx = 0;

                for (int x = 0; x < bitmapWidth; x++)
                {
                    // GDI+ returns BGR data, not RGB.
                    byte b = strideBytes[srcIdx++];
                    byte g = strideBytes[srcIdx++];
                    byte r = strideBytes[srcIdx++];

                    if (hasAlphaChannel)
                    {
                        byte alphaValue = strideBytes[srcIdx++];
                        alphaValues[alphaValueIdx++] = alphaValue;

                        if (alphaValue < 0xFF)
                            hasTransparentPixels = true;
                    }
                    else if (containsUnusedByte)
                    {
                        // skip the unused bytes if they exist in the bitmapBytes array.
                        srcIdx++;
                    }

                    if (!extractOnlyAlphaChannel)
                    {
                        colorValues[colorValueIdx + 2] = b;
                        colorValues[colorValueIdx + 1] = g;
                        colorValues[colorValueIdx + 0] = r;
                        colorValueIdx += 3;
                    }
                }

                startOfStride += bitmapData.Stride;
            }

            return new DrPixels(colorValues, alphaValues, hasTransparentPixels, ColorModel.Rgb, 8, null);
        }

        private static DrPixels ParseIndexed(BitmapData bitmapData, ColorPalette palette)
        {
            int bitsPerComponent;
            switch (bitmapData.PixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    bitsPerComponent = 1;
                    break;
                case PixelFormat.Format4bppIndexed:
                    bitsPerComponent = 4;
                    break;
                case PixelFormat.Format8bppIndexed:
                    bitsPerComponent = 8;
                    break;
                default:
                    throw new InvalidOperationException("Cannot parse this pixel format.");
            }

            if (bitmapData.Stride < 0)
                throw new InvalidOperationException("A bitmap has a negative stride and this is not yet supported.");

            int bytesPerLine = (int)Math.Ceiling((float)bitmapData.Width * bitsPerComponent / 8);

            byte[] colorValues = new byte[bytesPerLine * bitmapData.Height];

            bool hasAlphaValues = HasPaletteAlphaValues(palette);
            byte[] alphaValues = new byte[(hasAlphaValues) ? bitmapData.Width * bitmapData.Height : 0];
            int alphaIdx = 0;
            bool hasTransparentPixels = false;

            int colorValueIdx = 0;

            long startOfStride = bitmapData.Scan0.ToInt64();

            for (int y = 0; y < bitmapData.Height; y++)
            {
                // The stride in the bitmap can be longer than the actual number of bytes we need.
                Marshal.Copy(new IntPtr(startOfStride), colorValues, colorValueIdx, bytesPerLine);

                if (hasAlphaValues)
                {
                    // If we need to get alpha channel, we have to parse image data.
                    for (int i = 0; i < bitmapData.Width; i++)
                    {
                        DrColor pixelColor =
                            GetPixelFromIndexedImage(colorValues, bitsPerComponent, palette, colorValueIdx, i);

                        byte alphaValue = (byte)pixelColor.A;
                        alphaValues[alphaIdx++] = alphaValue;

                        if (alphaValue < 0xFF)
                            hasTransparentPixels = true;
                    }
                }

                colorValueIdx += bytesPerLine;
                startOfStride += bitmapData.Stride;
            }

            return new DrPixels(colorValues, alphaValues, hasTransparentPixels, ColorModel.Indexed, bitsPerComponent, PaletteToColorTable(palette));
        }

        private static DrColor[] PaletteToColorTable(ColorPalette palette)
        {
            Color[] nativeColors = palette.Entries;
            DrColor[] result = new DrColor[nativeColors.Length];
            for (int i = 0; i < nativeColors.Length; i++)
                result[i] = DrColor.FromNativeColor(nativeColors[i]);

            return result;
        }

        private static bool HasPaletteAlphaValues(ColorPalette palette)
        {
            foreach (Color color in palette.Entries)
                if (color.A != 0xFF)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns pixel from indexed image.
        /// </summary>
        /// <param name="imageData">Indexed image data.</param>
        /// <param name="bitsPerComponent">Number of bits per color component.</param>
        /// <param name="palette">Palette.</param>
        /// <param name="rowStartIndex">Start index in the image data of the required row.</param>
        /// <param name="column">Required column index.</param>
        /// <returns>Pixel color.</returns>
        private static DrColor GetPixelFromIndexedImage(
            byte[] imageData,
            int bitsPerComponent,
            ColorPalette palette,
            int rowStartIndex,
            int column)
        {
            int byteInRow = (int)Math.Floor((float)column * bitsPerComponent / 8);
            byte pixelByte = imageData[rowStartIndex + byteInRow];

            int paletteIndex = DrColorUtil.GetPaletteIndex(pixelByte, column, bitsPerComponent);

            return DrColor.FromNativeColor(palette.Entries[paletteIndex]);
        }

        /// <summary>
        /// Loads a native image from stream.
        /// </summary>
        public static Image LoadNativeImage(Stream stream)
        {
            return Image.FromStream(stream);
        }

        /// <summary>
        /// Loads a native image from file.
        /// </summary>
        public static Image LoadNativeImage(string fileName)
        {
            // 'Image.FromFile' is preferable here than 'using (FileStream stream = ...) Image.FromStream(stream))'
            // because an Image needs a stream to remain open (see the comment in SaveJpegCore for more info).
            return Image.FromFile(fileName);
        }

        /// <summary>
        /// Saves an image into a stream in a format that will satisfy MS Word 97.
        ///
        /// In .NET the image parameter is a GDI+ Image, but in Java the image parameter is BufferedImage.
        ///
        /// This is a static method (not an instance method) because in .NET it accepts a <see cref="Image"/> parameter
        /// that can either be a metafile or bitmap and saves it accordingly.
        /// </summary>
        /// <param name="image">The image to save.</param>
        /// <param name="stream">The destination stream.</param>
        public static void SaveNativeImageForWord97(Image image, Stream stream)
        {
            switch (ImageFormatToImageType(image.RawFormat))
            {
                case FileFormat.Wmf:
                case FileFormat.Emf:
                {
                    SaveMetafile((Metafile)image, stream);
                    break;
                }
                case FileFormat.Jpeg:
                {
                    BitmapPal bitmap = new BitmapPal((Bitmap)image);
                    bitmap.SaveJpeg(stream, 80);
                    // This is required to avoid disposing the user's image object.
                    bitmap.Bitmap = null;
                    break;
                }

                // This image format is supported by MS Word 97, just save in this format.
                case FileFormat.Png:
                // WORDSNET-3702 "There is not enough memory or disk space to display or print the picture".
                // RK We used to store BMP images as BMP (DIB) in a word document, but it seems MS Word
                // fails on some DIB formats (on all 32bit DIBs etc). MS Word itself converts BMP to PNG
                // when you insert a BMP. Therefore, we now do the same.
                case FileFormat.Bmp:
                // Gif and Tiff are not natively supported by MS Word 97 and therefore we convert them to PNG.
                case FileFormat.Gif:
                case FileFormat.Tiff:
                // Also, any other image type that we might encounter, try to store as PNG in our model.
                default:
                {
                    BitmapPal bitmap = new BitmapPal((Bitmap)image);
                    bitmap.SavePng(stream);
                    // This is required to avoid disposing the user's image object.
                    bitmap.Bitmap = null;
                    break;
                }
            }
        }

        /// <summary>
        /// This is needed to save a WMF or EMF metafile object as an EMF file again. Seems no easier way in .NET.
        /// Note this makes the destination EMF resolution 96dpi. I don't like this (it would be better
        /// to keep the dpi of the original metafile), but I don't know how to do this in .NET.
        ///
        /// AL: To set resolution simply change the hdc resolution. RK ??
        /// </summary>
        private static void SaveMetafile(Metafile srcMetafile, Stream dstStream)
        {
            // Create an empty bitmap so we can create a Graphics on it.
            using (BitmapPal dummyBitmap = new BitmapPal(1, 1))
            {
                // Set Resolution from original metafile
                dummyBitmap.GetNativeBitmap().SetResolution(srcMetafile.HorizontalResolution, srcMetafile.VerticalResolution);
                // Obtain an HDC to draw a metafile on.
                using (Graphics srcGraphics = BitmapUtilPal.GraphicsFromImage(dummyBitmap.GetNativeBitmap()))
                {
                    IntPtr srcHdc = srcGraphics.GetHdc();
                    try
                    {
                        // Create a recordable metafile and release HDC.
                        // Using EmfPlusDual seems to produce better images, for example gradient shading works well.
                        // The metafile is actually written when it is disposed.
                        using (Metafile dstMetafile = new Metafile(dstStream, srcHdc, EmfType.EmfPlusDual))
                        {
                            // Create a graphics and play the existing metafile on to it.
                            using (Graphics dstGraphics = BitmapUtilPal.GraphicsFromImage(dstMetafile))
                            {
                                Rectangle srcBounds = srcMetafile.GetMetafileHeader().Bounds;
                                dstGraphics.DrawImageUnscaled(srcMetafile, srcBounds);
                            }

                            dstStream.Flush();
                        }
                    }
                    finally
                    {
                        srcGraphics.ReleaseHdc(srcHdc);
                    }
                }
            }
        }

        /// <summary>
        /// Converts the bitmap to 24bppRgb pixel format.
        /// </summary>
        /// <param name="isDispose">Indicates either to Dispose old underlying <see cref="Bitmap"/>
        /// after it was converted. </param>
        /// <param name="backgroundColor">Canvas color to re-draw the image on.</param>
        private void ConvertTo24BppRgb(DrColor backgroundColor)
        {
            BitmapPal rgbBitmap = new BitmapPal(
                Width, Height, HorizontalResolution, VerticalResolution, PixelFormat.Format24bppRgb);

            Rectangle rect = GetBounds();
            DrawImage(rect, rgbBitmap, rect, backgroundColor);
            Bitmap = rgbBitmap.Bitmap;
            rgbBitmap.Bitmap = null;
        }

        /// <summary>
        /// Converts the bitmap to 32BppArgb pixel format.
        /// </summary>
        /// <remarks>
        /// If the bitmap is in 32BppArgb pixel format, returns this one, otherwise creates a new image.
        /// </remarks>
        internal BitmapPal ConvertTo32BppArgb()
        {
            BitmapPal converted;

            // If original bitmap is not already in 32 BPP ARGB format, then convert
            if (Bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                converted = new BitmapPal(
                    Width,
                    Height,
                    HorizontalResolution,
                    VerticalResolution,
                    PixelFormat.Format32bppArgb);

                Rectangle rect = GetBounds();

                DrawImage(rect, converted, rect);
            }
            else
                converted = this;

            return converted;
        }

        public bool IsCmykPixelFormat()
        {
            return IsCmykPixelFormat(Bitmap);
        }

        public static bool IsCmykPixelFormat(Bitmap bmp)
        {
            // WORDSNET-6409.
            // Systems under Windows 7 don't recognize PixelFormat right for Cmyk images.
            // In such cases we must use Image.Flags to determinate the Cmyk color space.
            ImageFlags flags = (ImageFlags)bmp.Flags;
            if (((flags & ImageFlags.ColorSpaceCmyk) == ImageFlags.ColorSpaceCmyk) ||
                ((flags & ImageFlags.ColorSpaceYcck) == ImageFlags.ColorSpaceYcck))
            {
                return true;
            }

            return ((int)bmp.PixelFormat == CmykPixelFormat);
        }

        /// <summary>
        /// Converts from .NET ImageFormat into our ImageType.
        /// </summary>
        private static FileFormat ImageFormatToImageType(ImageFormat imageFormat)
        {
            if (imageFormat.Equals(ImageFormat.Jpeg))
                return FileFormat.Jpeg;
            else if (imageFormat.Equals(ImageFormat.Png))
                return FileFormat.Png;
            else if (imageFormat.Equals(ImageFormat.Emf))
                return FileFormat.Emf;
            else if (imageFormat.Equals(ImageFormat.Wmf))
                return FileFormat.Wmf;
            else if (imageFormat.Equals(ImageFormat.Bmp))
                return FileFormat.Bmp;
            else if (imageFormat.Equals(ImageFormat.Gif))
                return FileFormat.Gif;
            else if (imageFormat.Equals(ImageFormat.Tiff))
                return FileFormat.Tiff;
            else
                return FileFormat.Unknown;
        }

        public BitmapDataPal LockBits()
        {
            Rectangle rect = new Rectangle(0, 0, Bitmap.Width, Bitmap.Height);
            BitmapData data = Bitmap.LockBits(rect, ImageLockMode.ReadWrite, Bitmap.PixelFormat);
            return new BitmapDataPal(Bitmap, data);
        }


        private const int CmykPixelFormat = 0x0000200f;

        /// <summary>
        /// The source stream with image data. We must keep the stream open for the lifetime of the Bitmap.
        /// </summary>
        private Stream mImageStream;
    }
}
#endif
