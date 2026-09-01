// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/05/2020 by Alexey Noskov
#if NETSTANDARD || NET

using SkiaSharp;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// In SkiaSharp graphics quality options are applied to each SKPaint object not to SKCanvas (analog of Graphics),
    /// this makes impossible to set anti-aliasing in one place in the code. This class is container for graphics quality options
    /// and allows to apply them to SKPaint object.
    /// </summary>
    public class GraphicsQualityOptions
    {
        public static void ApplyDefault(SKPaint paint)
        {
            paint.IsAntialias = DefaultAntiAliasing;
        }

        public void Apply(SKPaint paint, bool isText)
        {
            paint.IsAntialias = isText ? mTextAntiAliasing : mGraphicsAntiAliasing;
        }

        public SKFilterMode FilterMode
        {
            get { return mFilterMode; }
            set { mFilterMode = value; }
        }

        public SKMipmapMode MipmapMode
        {
            get { return mMipmapMode; }
            set { mMipmapMode = value; }
        }

        public bool TextAntiAliasing
        {
            get { return mTextAntiAliasing; }
            set { mTextAntiAliasing = value; }
        }

        public bool GraphicsAntiAliasing
        {
            get { return mGraphicsAntiAliasing; }
            set { mGraphicsAntiAliasing = value; }
        }

        public SKSamplingOptions GetSamplingOptions()
        {
            return new SKSamplingOptions(mFilterMode, mMipmapMode);
        }

        public static SKSamplingOptions DefaultSamplingOptions
        {
            get { return gDefaultSamplingOptions; }
        }

        private SKFilterMode mFilterMode = DefaultFilterMode;
        private SKMipmapMode mMipmapMode = DefaultMipmapMode;
        private bool mTextAntiAliasing = DefaultAntiAliasing;
        private bool mGraphicsAntiAliasing = DefaultAntiAliasing;

        private const SKFilterMode DefaultFilterMode = SKFilterMode.Linear;
        private const SKMipmapMode DefaultMipmapMode = SKMipmapMode.Linear;
        private const bool DefaultAntiAliasing = true;
        private static readonly SKSamplingOptions gDefaultSamplingOptions = new SKSamplingOptions(DefaultFilterMode, DefaultMipmapMode);
    }
}

#endif
