// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/10/2006 by Dmitry Vorobyev

using System.Drawing;

namespace Aspose.Images
{
    /// <summary>
    /// Contains information about image size and resolution.
    /// </summary>
    public class ImageSizeCore
    {
        /// <summary>
        /// Creates zero width and height object, but with 96 dpi resolution.
        /// </summary>
        public static ImageSizeCore CreateEmpty()
        {
            return new ImageSizeCore(0, 0, 0, 0, 0, 0); // RK The resolution should be zero here, it is "validated" in the ctor.
        }

        /// <summary>
        /// Creates an image size object when you know resolution.
        /// But you still can pass resolution zero, it will default to 96 dpi.
        /// </summary>
        /// <param name="width">Pixels.</param>
        /// <param name="height">Pixels.</param>
        /// <param name="hRes">DPI</param>
        /// <param name="vRes">DPI</param>
        /// <returns></returns>
        public static ImageSizeCore CreateWithResolution(
            int width,
            int height,
            double hRes,
            double vRes)
        {
            return new ImageSizeCore(0, 0, width, height, hRes, vRes);
        }

        /// <summary>
        /// Creates an image size object when you know resolution.
        /// But you still can pass resolution zero, it will default to 96 dpi.
        /// </summary>
        /// <param name="width">Pixels.</param>
        /// <param name="height">Pixels.</param>
        /// <param name="res">Horizontal and vertical DPI.</param>
        /// <returns></returns>
        public static ImageSizeCore CreateWithResolution(
            int width,
            int height,
            double res)
        {
            double hRes = res;
            double vRes = res;
            return new ImageSizeCore(0, 0, width, height, hRes, vRes);
        }

        /// <summary>
        /// Creates an image size object when you know physical size (in EMUs).
        /// </summary>
        /// <param name="left">Pixels</param>
        /// <param name="top">Pixels</param>
        /// <param name="right">Pixels</param>
        /// <param name="bottom">Pixels</param>
        /// <param name="widthEmus">EMUs</param>
        /// <param name="heightEmus">EMUs</param>
        /// <returns></returns>
        public static ImageSizeCore CreateWithDimensions(
            int left,
            int top,
            int right,
            int bottom,
            int widthEmus,
            int heightEmus)
        {
            int width = right - left;
            int height = bottom - top;
            double hRes = (widthEmus != 0) ? (width / ConvertUtilCore.EmuToInch(widthEmus)) : 0;
            double vRes = (heightEmus != 0) ? (height / ConvertUtilCore.EmuToInch(heightEmus)) : 0;
            return new ImageSizeCore(left, top, width, height, hRes, vRes);
        }

        /// <summary>
        /// Use static ctors instead.
        /// </summary>
        private ImageSizeCore(int left, int top, int width, int height, double hRes, double vRes)
        {
            mLeft = left;
            mTop = top;
            mWidth = width;
            mHeight = height;
            mHorizontalResolution = hRes;
            mVerticalResolution = vRes;

            if ((hRes == 0) || (vRes == 0))
            {
                mIsOriginalResolutionZero = true;
                mHorizontalResolution = ImageConstants.StandardResolution;
                mVerticalResolution = ImageConstants.StandardResolution;
            }
        }

        /// <summary>
        /// Left bound of the image, pixels. Normally used for metafiles only.
        /// </summary>
        public int Left
        {
            get { return mLeft; }
        }

        /// <summary>
        /// Top bound of the image, pixels. Normally used for metafiles only.
        /// </summary>
        public int Top
        {
            get { return mTop; }
        }

        /// <summary>
        /// Right bound of the image, pixels. Normally used for metafiles only.
        /// </summary>
        public int Right
        {
            get { return Left + Width; }
        }

        /// <summary>
        /// Bottom bound of the image, pixels. Normally used for metafiles only.
        /// </summary>
        public int Bottom
        {
            get { return Top + Height; }
        }

        /// <summary>
        /// Width of the image in pixels.
        /// </summary>
        public int Width
        {
            get { return mWidth; }
        }

        /// <summary>
        /// Height of the image in pixels.
        /// </summary>
        public int Height
        {
            get { return mHeight; }
        }

        /// <summary>
        /// Horizontal resolution in DPI.
        /// </summary>
        public double HorizontalResolution
        {
            get { return mHorizontalResolution; }
        }

        /// <summary>
        /// Vertical resolution in DPI.
        /// </summary>
        public double VerticalResolution
        {
            get { return mVerticalResolution; }
        }

        /// <summary>
        /// Returns true if the resolution was not specified in the image.
        ///
        /// We never returns zero resolution from <see cref="HorizontalResolution"/> and <see cref="VerticalResolution"/>
        /// to simplify mathematical calculations based on them for the clients, we default to 96 dpi.
        /// But sometimes it is important to know if the resolution was actually specified or not in the image.
        /// </summary>
        public bool IsOriginalResolutionZero
        {
            get { return mIsOriginalResolutionZero; }
        }

        /// <summary>
        /// Width of the image in points. 1 point is 1/72 inch.
        /// </summary>
        public double WidthPoints
        {
            get { return ConvertUtilCore.PixelToPoint(mWidth, mHorizontalResolution); }
        }

        /// <summary>
        /// Height of the image in points. 1 point is 1/72 inch.
        /// </summary>
        public double HeightPoints
        {
            get { return ConvertUtilCore.PixelToPoint(mHeight, mVerticalResolution); }
        }

        /// <summary>
        /// Gets width of the metafile in EMUs.
        /// </summary>
        public int WidthEmus
        {
            get { return ConvertUtilCore.PointToEmu(WidthPoints); }
        }

        /// <summary>
        /// Gets height of the metafile in EMUs.
        /// </summary>
        public int HeightEmus
        {
            get { return ConvertUtilCore.PointToEmu(HeightPoints); }
        }

        public int WidthTwips
        {
            get { return ConvertUtilCore.PixelToTwip(Width, HorizontalResolution); }
        }

        public int HeightTwips
        {
            get { return ConvertUtilCore.PixelToTwip(Height, VerticalResolution); }
        }

        public Size Size
        {
            get { return new Size(mWidth, mHeight); }
        }

        private readonly int mLeft;
        private readonly int mTop;
        private readonly int mWidth;
        private readonly int mHeight;
        private double mHorizontalResolution;       // Can't make readonly because of Java.
        private double mVerticalResolution;         // Can't make readonly because of Java.
        private bool mIsOriginalResolutionZero;     // Can't make readonly because of Java.
    }
}
