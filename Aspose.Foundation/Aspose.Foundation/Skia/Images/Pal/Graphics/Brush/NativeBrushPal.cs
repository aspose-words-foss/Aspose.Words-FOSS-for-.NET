// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/08/2017 by Alexey Noskov

#if NETSTANDARD

using System;
using Aspose.Drawing;
using SkiaSharp;

namespace Aspose.Images.Pal.Graphics.Brush
{
    public class NativeBrushPal : IDisposable
    {
        protected NativeBrushPal()
        {
            mPaint = new SKPaint();
            GraphicsQualityOptions.ApplyDefault(mPaint);

            // This is brush, so style is always Fill.
            mPaint.Style = SKPaintStyle.Fill;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Dispose shader.
            if (mShader != null)
                mShader.Dispose();

            // Dispose bitmap. No need to check if bitmap is recycled, this check is made inside its Dispose method. 
            if (mBitmap != null)
                mBitmap.Dispose();

            // Dispose Paint.
            if (mPaint != null)
                mPaint.Dispose();
        }

        protected void SetShader(SKShader shader)
        {
            mShader = shader;
            mPaint.Shader = shader;
        }

        protected static byte[] ToByteArray(SKBitmap bmp)
        {
            using (SKDynamicMemoryWStream skStream = new SKDynamicMemoryWStream())
            {
                bmp.Encode(skStream, SKEncodedImageFormat.Png, 100);
                using (SKData data = skStream.DetachAsData())
                    return data.ToArray();
            }
        }

        internal static SKColor DrColorToSKColor(DrColor c)
        {
            SKColor skColor = new SKColor((uint)c.ToArgb());
            return skColor;
        }

        internal static SKColor[] DrColorsToSKColors(DrColor[] colors)
        {
            SKColor[] skColors = new SKColor[colors.Length];
            for (int i = 0; i < colors.Length; i++)
                skColors[i] = DrColorToSKColor(colors[i]);

            return skColors;
        }

        internal SKPaint Paint
        {
            get { return mPaint; }
        }

        internal SKColor Color
        {
            get { return mPaint.Color; }
            set { mPaint.Color = value; }
        }

        internal SKBitmap Bitmap
        {
            get { return mBitmap; }
        }

        internal SKShader Shader
        {
            get { return mShader; }
        }

        protected SKBitmap mBitmap;
        private SKShader mShader;
        private readonly SKPaint mPaint;
    }
}
#endif
