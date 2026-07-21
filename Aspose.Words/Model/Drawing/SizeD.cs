// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/10/2017 by Andrey Noskov

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Stores shape size as double precision floating-point numbers.
    /// </summary>
    internal class SizeD
    {
        internal SizeD(double width, double height)
        {
            Width = width;
            Height = height;
        }

        internal SizeD()
        {
        }

        /// <summary>
        /// Returns true, if width or height of this SizeD is greater
        /// than width or height of the specified SizeD.
        /// </summary>
        internal bool IsExceeds(SizeD size)
        {
            return ((mWidth > size.Width) || (mHeight > size.Height));
        }

        /// <summary>
        /// Gets a value indicating whether this SizeD has zero width and height.
        /// </summary>
        internal bool IsEmpty
        {
            get { return MathUtil.IsZero(mWidth) && MathUtil.IsZero(mHeight); }
        }

        /// <summary>
        /// Gets or sets the horizontal component of size.
        /// </summary>
        internal double Width
        {
            get { return mWidth; }
            set { mWidth = value; }
        }

        /// <summary>
        /// Gets or sets the vertical component of size.
        /// </summary>
        internal double Height
        {
            get { return mHeight; }
            set { mHeight = value; }
        }

        private double mWidth;
        private double mHeight;
    }
}
