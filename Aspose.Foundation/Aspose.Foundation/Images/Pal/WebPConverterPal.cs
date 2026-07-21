// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/03/2023 by Denis Panov

#if !NETSTANDARD
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using Aspose.JavaAttributes;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// This class is used to read and write WebP under .NetFramework v4.6.2 or greater assembly.
    /// </summary>
    [JavaManual]
    [AndroidDelete]
    internal static class WebPConverterPal
    {
        static WebPConverterPal()
        {
            LoadSkiaSharp();
        }

        private static void LoadSkiaSharp()
        {
#if NET462_OR_GREATER
            try
            {
                skiaSharpAssembly = Assembly.Load("SkiaSharp");

                skCodecType = skiaSharpAssembly.GetType("SkiaSharp.SKCodec");
                skBitmapType = skiaSharpAssembly.GetType("SkiaSharp.SKBitmap");
                skImageInfoType = skiaSharpAssembly.GetType("SkiaSharp.SKImageInfo");
                skCodecResultType = skiaSharpAssembly.GetType("SkiaSharp.SKCodecResult");
                skCodecOptionsType = skiaSharpAssembly.GetType("SkiaSharp.SKCodecOptions");
                skImageInfo = skiaSharpAssembly.GetType("SkiaSharp.SKImageInfo");
                skEncodedImageFormat = skiaSharpAssembly.GetType("SkiaSharp.SKEncodedImageFormat");
            }
            catch
            {
                throw new CantCreateBitmapException("SkiaSharp cannot be loaded. Make sure SkiaSharp (version 3.119.0 or higher) is referenced in your project and placed next to Aspose.Words.dll");
            }
#endif
        }

        internal static Bitmap ReadBySkia(Stream stream)
        {
#if NET462_OR_GREATER
            object codec = null;
            try
            {
                using (MemoryStream inputStream = new MemoryStream())
                {
                    stream.CopyTo(inputStream);
                    inputStream.Seek(0, SeekOrigin.Begin);

                    codec = CreateSKCodec(inputStream);
                    return SKCodecToBitmap(codec);
                }
            }
            finally
            {
                DisposeSkiaObject(codec);
            }
#else
            throw new CantCreateBitmapException("WebP format images are supported starting from Aspose.Words for .NetFramework v4.6.2. Additionally, ensure that SkiaSharp (version 3.119.0 or higher) is installed and referenced in your project.");
#endif
        }


        internal static void WriteBySkia(Bitmap image, Stream stream)
        {
#if NET462_OR_GREATER
            object skBitmap = null;
            try
            {
                skBitmap = BitmapToSKBitmap(image);
                EncodeSKBitmap(skBitmap, stream);
            }
            finally
            {
                DisposeSkiaObject(skBitmap);
            }
#else
            throw new CantCreateBitmapException("WebP format images are supported starting from Aspose.Words for .NetFramework v4.6.2. Additionally, ensure that SkiaSharp (version 3.119.0 or higher) is installed and referenced in your project.");
#endif
        }

#if NET462_OR_GREATER
        private static void DisposeSkiaObject(object skiaObj)
        {
            IDisposable disposable = skiaObj as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        private static object CreateSKCodec(Stream stream)
        {
            MethodInfo createMethod = skCodecType.GetMethod("Create",  new Type[] { typeof(Stream) });
            return createMethod.Invoke(null, new object[] { stream });
        }

        private static Bitmap SKCodecToBitmap(object codec)
        {
            PropertyInfo infoProperty = skCodecType.GetProperty("Info");
            object imageInfo = infoProperty.GetValue(codec, null);

            PropertyInfo widthProperty = skImageInfo.GetProperty("Width");
            PropertyInfo heightProperty = skImageInfo.GetProperty("Height");
            int width = (int)widthProperty.GetValue(imageInfo, null);
            int height = (int)heightProperty.GetValue(imageInfo, null);

            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppPArgb);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            try
            {
                MethodInfo getPixelsMethod = skCodecType.GetMethod("GetPixels", new Type[] { skImageInfoType, typeof(IntPtr), skCodecOptionsType });
                object codecOptions = Activator.CreateInstance(skCodecOptionsType, 0);
                object outImageInfo = Activator.CreateInstance(skImageInfoType, width, height);
                object result = getPixelsMethod.Invoke(codec, new object[] { outImageInfo, data.Scan0, codecOptions });
                int success = (int)Enum.Parse(skCodecResultType, "Success");

                if ((int)result != success)
                {
                    bitmap.Dispose();
                    bitmap = null;
                }
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
            return bitmap;
        }

        private static object BitmapToSKBitmap(Bitmap bitmap)
        {
            object imageInfo = Activator.CreateInstance(skImageInfoType, bitmap.Width, bitmap.Height);
            object skiaBitmap = Activator.CreateInstance(skBitmapType, imageInfo);
            try
            {
                PropertyInfo rowBytesProperty = skImageInfo.GetProperty("RowBytes");
                int rowBytes = (int)rowBytesProperty.GetValue(imageInfo, null);

                MethodInfo getPixelsMethod = skBitmapType.GetMethod("GetPixels", Type.EmptyTypes);
                IntPtr pixels = (IntPtr)getPixelsMethod.Invoke(skiaBitmap, null);

                using (Bitmap tempBitmap = new Bitmap(bitmap.Width, bitmap.Height, rowBytes, PixelFormat.Format32bppPArgb, pixels))
                using (Graphics gr = Graphics.FromImage(tempBitmap))
                {
                    gr.Clear(System.Drawing.Color.Transparent);
                    gr.DrawImageUnscaled(bitmap, 0, 0);
                }
            }
            catch
            {
                DisposeSkiaObject(skiaBitmap);
                throw;
            }
            return skiaBitmap;
        }

        private static void EncodeSKBitmap(object skBitmap, Stream stream)
        {
            object imageFormat = Enum.Parse(skEncodedImageFormat, "Webp");
            MethodInfo encodeMethod = skBitmapType.GetMethod("Encode", new Type[] { typeof(Stream), skEncodedImageFormat, typeof(int) });
            encodeMethod.Invoke(skBitmap, new object[] { stream, imageFormat, 100 });
        }

        private static Assembly skiaSharpAssembly;
        private static Type skCodecType;
        private static Type skBitmapType;
        private static Type skImageInfoType;
        private static Type skCodecResultType;
        private static Type skCodecOptionsType;
        private static Type skImageInfo;
        private static Type skEncodedImageFormat;
#endif
    }
}
#endif
