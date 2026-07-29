// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/08/2017 by Vyacheslav Durin
// Translation from Java by Alexey Noskov

#if NETSTANDARD

using System;
using System.Drawing;
using System.IO;
using Aspose.Drawing;
using Aspose.Drawing.Fonts;
using Aspose.Images.Pal.Graphics.Decoder.Tiff;
using Aspose.Warnings;
using SkiaSharp;

namespace Aspose.Images.Pal
{
    internal class BitmapCorePal
    {
        private BitmapCorePal()
        {
        }

        internal static DrPixels ParseGrayScale(SKBitmap bitmap)
        {
            int bitsPerPixel = 8;
            return new DrPixels(bitmap.Bytes, new byte[0], false, ColorModel.GrayScale, bitsPerPixel, null);
        }

        /// <summary>
        /// Convert GDI+ BitmapData into a binary array.
        /// Can only parse 8bit per channel RGB or ARGB data.
        /// </summary>
        internal static DrPixels ParseRgb(SKBitmap bitmap, bool extractOnlyAlphaChannel)
        {
            bool hasAlphaChannel = ImageHasAlphaChannel(bitmap);

            // If the is no alpha in the image and we need only alpha then we could stop here.
            if (!hasAlphaChannel && extractOnlyAlphaChannel)
                return new DrPixels(ArrayUtil.EmptyByteArray, ArrayUtil.EmptyByteArray, false, ColorModel.Rgb, 8, null);

            // We want exactly 3 color component RGB in this array.
            byte[] colorValues = new byte[extractOnlyAlphaChannel ? 0 : bitmap.Width * bitmap.Height * 3];
            byte[] alphaValues = new byte[(hasAlphaChannel) ? bitmap.Width * bitmap.Height : 0];

            bool hasTransparentPixels = false;
            int colorValueIdx = 0;
            int alphaValueIdx = 0;

            SKColor[] pixels = bitmap.Pixels;
            for (int i = 0; i < pixels.Length; i++)
            {
                SKColor pixel = pixels[i];

                byte r = pixel.Red;
                byte g = pixel.Green;
                byte b = pixel.Blue;

                if (hasAlphaChannel)
                {
                    byte alphaValue = pixel.Alpha;
                    alphaValues[alphaValueIdx++] = alphaValue;

                    if ((alphaValue & 0xFF) < 0xFF)
                        hasTransparentPixels = true;
                }

                if (!extractOnlyAlphaChannel)
                {
                    colorValues[colorValueIdx++] = r;
                    colorValues[colorValueIdx++] = g;
                    colorValues[colorValueIdx++] = b;
                }
            }

            return new DrPixels(colorValues, alphaValues, hasTransparentPixels, ColorModel.Rgb, 8, null);
        }

        internal static DrPixels ParseIndexed(SKBitmap bitmap)
        {
            // alexnosk: I am not sure about this, but seems it should work as expected.
            int bitsPerComponent = 8;

            byte[] colorValues = new byte[bitmap.Width * bitmap.Height];

            bool hasAlphaValues = ImageHasAlphaChannel(bitmap);
            byte[] alphaValues = new byte[(hasAlphaValues) ? bitmap.Width * bitmap.Height : 0];
            int alphaIdx = 0;
            bool hasTransparentPixels = false;

            int colorValueIdx = 0;

            bool isAlphaOnly = (bitmap.ColorType == SKColorType.Alpha8);

            // alexnosk: Wrap SKBitmap into BitmapPal to use fix with SKBitmap.GetPixel bug.
            BitmapPal bitmapPal = new BitmapPal(bitmap);
            //Actual parsing of the data
            try
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    // If we need to get alpha channel, we have to parse image data.
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        colorValues[colorValueIdx++] = isAlphaOnly ? (byte)0 : bitmap.Bytes[y * bitmap.Width + x];

                        if (hasAlphaValues)
                        {
                            int pixel = bitmapPal.GetPixel(x, y);
                            DrColor pixelColor = new DrColor(pixel);
                            byte alphaValue = (byte)pixelColor.A;
                            alphaValues[alphaIdx++] = alphaValue;

                            if (alphaValue < 0xFF)
                                hasTransparentPixels = true;
                        }
                    }
                }
            }
            catch
            {
                // Silent the exception.
            }
            return new DrPixels(colorValues, alphaValues, hasTransparentPixels, ColorModel.Indexed, bitsPerComponent, null);
        }

        /// <summary>
        /// Checks whether image has alpha channel.
        /// </summary>
        internal static bool ImageHasAlphaChannel(SKBitmap src)
        {
            return src.AlphaType != SKAlphaType.Opaque && src.AlphaType != SKAlphaType.Unknown;
        }

        /// <summary>
        /// Draws this bitmap onto another bitmap. Allows to positively crop and to scale the image.
        /// </summary>
        internal static void DrawImage(SKBitmap srcBitmap, Rectangle srcRect, SKBitmap dstBitmap, Rectangle dstRect)
        {
            DrawImage(srcBitmap, srcRect, dstBitmap, dstRect, null);
        }

        /// <summary>
        /// Draws this bitmap onto another bitmap. Allows to positively crop and to scale the image. The bitmap can be
        /// drawn on a colored canvas.
        /// </summary>
        internal static void DrawImage(SKBitmap srcBitmap, Rectangle srcRect, SKBitmap dstBitmap, Rectangle dstRect,
            DrColor backgroundColor)
        {
            SKRect dest = new SKRect(dstRect.Left, dstRect.Top, dstRect.Right, dstRect.Bottom);
            SKRect src = new SKRect(srcRect.Left, srcRect.Top, srcRect.Right, srcRect.Bottom);

            if (dstBitmap == null)
                dstBitmap = new SKBitmap(dstRect.Width, dstRect.Height, srcBitmap.ColorType, srcBitmap.AlphaType);

            using (SKCanvas canvas = new SKCanvas(dstBitmap))
            using (SKPaint paint = new SKPaint())
            {
                if ((backgroundColor != null) && (backgroundColor != DrColor.Empty))
                    canvas.Clear(new SKColor((uint)backgroundColor.ToArgb()));

                GraphicsQualityOptions.ApplyDefault(paint);
                canvas.DrawBitmap(srcBitmap, src, dest, paint);
            }
        }


        // From what I can understand it is hard to convert CMYK to RGB exactly, one needs a device profile for it.
        // I tried using Java's ColorSpace methods to perform the conversion but it results in plain incorrect picture.
        // So I'm just using a simple formula here that seems to work better.
        internal static SKColor[] ToSkRgbaPixels(byte[] bytes, int stride, int width, int height)
        {
            int comp = stride / width;
            SKColor[] pixels = new SKColor[width * height];

            int i = 0;
            for (int y = 0; y < height; y++)
            {
                // FIX WORDSNET-19062 - The problem occured because stride of the image is compnetsPerpixel*width+1,
                // in the code we expected stride=compnetsPerpixel*width. Made code resilient.
                i = y * stride;
                for (int x = 0; x < width; x++)
                {
                    byte a = (byte)(comp > 3 ? bytes[i + 3] : 255);
                    byte r = (byte)(bytes[i + 2] & 0xFF);
                    byte g = (byte)(bytes[i + 1] & 0xFF);
                    byte b = (byte)(bytes[i + 0] & 0xFF);
                    i += comp;

                    pixels[y * width + x] = new SKColor(r, g, b, a);
                }
            }
            return pixels;
        }

        /// <summary>
        /// http://today.java.net/pub/a/today/2007/04/03/perils-of-image-getscaledinstance.html
        ///
        /// Convenience method that returns a scaled instance of the provided { @code BufferedImage }.
        ///
        /// If downscaling ratio is more than twice of original size a multi-step scaling technique
        /// will be used because usual one-step technique provides too bad results in this case.
        ///
        /// Disposes the original bitmap. The caller should dispose the returned bitmap only.
        /// </summary>
        /// <param name="image">The original image to be scaled</param>
        /// <param name="targetWidth">The desired width of the scaled instance, in pixels</param>
        /// <param name="targetHeight">The desired height of the scaled instance, in pixels</param>
        /// <returns>A scaled version of the original.</returns>
        internal static SKBitmap GetScaledInstance(SKBitmap image, int targetWidth, int targetHeight)
        {
            targetWidth = Math.Abs(targetWidth);
            targetHeight = Math.Abs(targetHeight);
            // Do not "scale" image to zero as .Net Graphics.DrawImage() does.
            if (targetWidth == 0 || targetHeight == 0)
                return image;

            int srcWidth = image.Width;
            int srcHeight = image.Height;

            if ((srcWidth == targetWidth) && (srcHeight == targetHeight))
                return image;

            // The original image should be disposed.
            using (image)
            {
                SKBitmap result = new SKBitmap(targetWidth, targetHeight, image.ColorType, image.AlphaType);
                image.ScalePixels(result, GraphicsQualityOptions.DefaultSamplingOptions);

                return result;
            }
        }

        /// <summary>
        /// Converts PixelFormat to the corresponding Bitmap.Config.
        /// </summary>
        internal static SKColorType GetBitmapType(System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return SKColorType.Rgb565;

                //Specifies that the format is 32 bits per pixel; 8 bits each are used for the alpha, red, green, and blue components.
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return BitmapPal.DefaultColorType;

                //Specifies that the format is 32 bits per pixel; 8 bits each are used for the alpha, red, green, and blue
                //components. The red, green, and blue components are premultiplied, according to the alpha component.
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    return BitmapPal.DefaultColorType;

                //return BufferedImage.TYPE_BYTE_INDEXED;
                default:
                    return BitmapPal.DefaultColorType;
            }
        }

        /// <summary>
        /// Gets ColorModel by specified Bitmap.Config.
        /// </summary>
        internal static ColorModel GetColorModel(SKColorType pixelFormat)
        {
            switch (pixelFormat)
            {
                case SKColorType.Bgra8888:
                case SKColorType.RgbaF16:
                case SKColorType.Rgba8888:
                case SKColorType.Argb4444:
                case SKColorType.Rgb565:
                    return ColorModel.Rgb;
                case SKColorType.Alpha8:
                    return ColorModel.Indexed;
                case SKColorType.Gray8:
                    return ColorModel.GrayScale;
                case SKColorType.Unknown:
                default:
                    throw new InvalidOperationException("Unknown bitmap type.");
            }
        }

        /// <summary>
        /// The key point to load raw image bytes stream into java Bitmap.
        /// Converts CMYK and YCCK to RGB during load.
        /// </summary>
        internal static SKBitmap ImageFromStream(Stream stream, FileFormat imageType)
        {
            bool isDownSampled;
            return ImageFromStream(stream, imageType, true, out isDownSampled);
        }

        /// <summary>
        /// The key point to load raw image bytes stream into java Bitmap.
        /// Converts CMYK and YCCK to RGB during load.
        /// </summary>
        internal static SKBitmap ImageFromStream(Stream stream, FileFormat imageType, bool emulateRasterOperations, out bool isDownSampled)
        {
            // Throw this to make our image behave more like GDI+ images.
            if (imageType == FileFormat.Unknown)
                throw new ArgumentException("The image format is unsupported.");

            // By default false.
            isDownSampled = false;

            stream.Position = 0;

            // Metafile rasterization requires the rendering engine that is not part of the FOSS build.
            if (ImageUtil.IsMetafile(imageType))
                throw new CantCreateBitmapException("Metafile rasterization is not supported. FileFormat=" + imageType);

            if (imageType == FileFormat.Tiff)
                return LoadTiff(stream);

            return LoadImageBySkia(stream, imageType);
        }

        /// <summary>
        /// Returns no image (red cross) bitmap. Each time when called creates new instance of bitmap.
        /// </summary>
        public static SKBitmap GetNoImageBitmap()
        {
            return SKBitmap.Decode(ImageUtil.GetNoImageBytes());
        }

        private static SKBitmap LoadTiff(Stream stream)
        {
            long pos = 0;
            SKBitmap result = null;
            try
            {
                pos = stream.Position;
                stream.Position = 0;
                using (TiffDecoder decoder = new TiffDecoder(stream))
                    result = decoder.GetNativeBitmap(); // first frame only
            }
            catch
            {
                result = GetNoImageBitmap();
            }
            finally
            {
                stream.Position = pos;
            }

            return result;
        }

        /// <summary>
        /// Image is loaded by Android BitmapFactory.
        /// </summary>
        private static SKBitmap LoadImageBySkia(Stream stream, FileFormat imageType)
        {
            if (stream == null || stream.Length == 0)
                return GetNoImageBitmap();

            SKBitmap result;
            try
            {
                // FIX WORDSNET-22958, WORDSNET-22928, WORDSNET-22881, WORDSNET-22859, WORDSNET-22909 - Workaround of the  of SkiaSharp bug.
                // suggested here https://github.com/mono/SkiaSharp/issues/1846
                using (SKData data = SKData.Create(stream))
                    result = (imageType == FileFormat.Jpeg)
                        ? LoadImageWithCodec(data)
                        : SKBitmap.Decode(data);
            }
            catch
            {
                result = GetNoImageBitmap();
            }

            // If the image fails to load, return the default "no image" bitmap.
            return result == null ? GetNoImageBitmap() : result;
        }

        private static SKBitmap LoadImageWithCodec(SKData data)
        {
            SKBitmap result;

            // WORDSNET-27379, WORDSNET-27289
            // SKBitmap.Decode() doesn't process the JPG color profiles for the sake of backward compatibility.
            // Use the SKCodec to process the color profiles instead.
            // Currently this is only used for Jpeg images. I did this to minimize changes in golds.
            // But in general, this approach is needed for all images that may have color profiles.
            using (SKCodec codec = SKCodec.Create(data))
            {
                // Set the destination ColorSpace (i.e. sRGB).
                SKImageInfo info = new SKImageInfo(codec.Info.Width, codec.Info.Height,
                    codec.Info.ColorType, codec.Info.AlphaType,
                    SKColorSpace.CreateSrgb());

                // Skia creates a new bitmap, converting the source ColorSpace (e.g. CMYK) to the
                // destination ColorSpace (sRGB).
                result = SKBitmap.Decode(codec, info);
            }

            return result;
        }

    }
}
#endif
