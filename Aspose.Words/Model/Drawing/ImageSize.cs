// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/10/2006 by Dmitry Vorobyev

using Aspose.Images;


namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Contains information about image size and resolution.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-images/">Working with Images</a> documentation article.</para>
    /// </summary>
    /// <seealso cref="ImageData.ImageSize"/>
    public class ImageSize
    {
        /// <summary>
        /// Initializes width and height to the given values in pixels. Initializes resolution to 96 dpi.
        /// </summary>
        /// <param name="widthPixels">Width in pixels.</param>
        /// <param name="heightPixels">Height in pixels.</param>
        public ImageSize(int widthPixels, int heightPixels) : this(widthPixels, heightPixels, 96, 96)
        {
        }

        /// <summary>
        /// Initializes width, height and resolution to the given values.
        /// </summary>
        /// <param name="widthPixels">Width in pixels.</param>
        /// <param name="heightPixels">Height in pixels.</param>
        /// <param name="horizontalResolution">Horizontal resolution in DPI.</param>
        /// <param name="verticalResolution">Vertical resolution in DPI.</param>
        public ImageSize(int widthPixels, int heightPixels, double horizontalResolution, double verticalResolution)
        {
            mWidthPixels = widthPixels;
            mHeightPixels = heightPixels;
            mHorizontalResolution = horizontalResolution;
            mVerticalResolution = verticalResolution;
        }

        /// <summary>
        /// Creates an object from an internal image size object.
        /// </summary>
        internal ImageSize(ImageSizeCore core)
        {
            mWidthPixels = core.Width;
            mHeightPixels = core.Height;
            mHorizontalResolution = core.HorizontalResolution;
            mVerticalResolution = core.VerticalResolution;
        }

        /// <summary>
        /// Returns <c>true</c> if width and height are greater than zero.
        /// </summary>
        internal bool IsValid
        {
            get { return (WidthPixels > 0) && (HeightPixels > 0); }
        }


        /// <summary>
        /// Gets the width of the image in pixels.
        /// </summary>
        public int WidthPixels
        {
            get { return mWidthPixels; }
        }

        /// <summary>
        /// Gets the height of the image in pixels.
        /// </summary>
        public int HeightPixels
        {
            get { return mHeightPixels; }
        }

        /// <summary>
        /// Gets the horizontal resolution in DPI.
        /// </summary>
        public double HorizontalResolution
        {
            get { return mHorizontalResolution; }
        }

        /// <summary>
        /// Gets the vertical resolution in DPI.
        /// </summary>
        public double VerticalResolution
        {
            get { return mVerticalResolution; }
        }
        
        /// <summary>
        /// Gets the width of the image in points. 1 point is 1/72 inch.
        /// </summary>
        public double WidthPoints
        {
            get { return ConvertUtil.PixelToPoint(mWidthPixels, mHorizontalResolution); }
        }

        /// <summary>
        /// Gets the height of the image in points. 1 point is 1/72 inch.
        /// </summary>
        public double HeightPoints
        {
            get { return ConvertUtil.PixelToPoint(mHeightPixels, mVerticalResolution); }
        }

        internal int WidthTwips
        {
            get { return ConvertUtilCore.PixelToTwip(mWidthPixels, mHorizontalResolution); }
        }

        internal int HeightTwips
        {
            get { return ConvertUtilCore.PixelToTwip(mHeightPixels, mVerticalResolution); }
        }

        private readonly int mWidthPixels;
        private readonly int mHeightPixels;
        private readonly double mHorizontalResolution;
        private readonly double mVerticalResolution;
    }
}
