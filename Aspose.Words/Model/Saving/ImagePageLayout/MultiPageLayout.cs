// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/05/2025 by Dmitry Burov

using System;
using System.Drawing;
using Aspose.Drawing;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Defines a layout for rendering multiple pages into a single output.
    /// </summary>
    /// <remarks>
    /// Use one of the static factory methods to create a layout configuration.
    /// </remarks>
    public sealed class MultiPageLayout
    {
        /// <summary>
        /// Private ctor.
        /// </summary>
        private MultiPageLayout(MultiPageLayoutMode mode, int columns, float horizontalGap, float verticalGap)
        {
            if (horizontalGap < 0)
                throw new ArgumentOutOfRangeException("horizontalGap", "Horizontal gap must be non-negative.");

            if (verticalGap < 0)
                throw new ArgumentOutOfRangeException("verticalGap", "Vertical gap must be non-negative.");

            mMode = mode;
            mColumns = columns;
            mHorizontalGap = horizontalGap;
            mVerticalGap = verticalGap;

            // Set the default values.
            BorderWidth = 0;
            BorderColor = Color.Empty;
            BackColor = Color.Empty;
        }

        /// <summary>
        /// Creates a layout where all specified pages are rendered vertically one below the other in a single output.
        /// </summary>
        /// <param name="verticalGap">The vertical gap between pages in points.</param>
        public static MultiPageLayout Vertical(float verticalGap)
        {
            return new MultiPageLayout(MultiPageLayoutMode.Vertical, 1, 0, verticalGap);
        }

        /// <summary>
        /// Creates a layout in which all specified pages are rendered horizontally side by side,
        /// left to right, in a single output.
        /// </summary>
        /// <param name="horizontalGap">The horizontal gap between pages in points.</param>
        public static MultiPageLayout Horizontal(float horizontalGap)
        {
            return new MultiPageLayout(MultiPageLayoutMode.Horizontal, int.MaxValue, horizontalGap, 0);
        }

        /// <summary>
        /// Creates a layout in which pages are rendered left-to-right, top-to-bottom, in a grid with the
        /// specified number of columns.
        /// </summary>
        /// <param name="columns">The number of columns in the layout. Must be greater than zero.</param>
        /// <param name="horizontalGap">The horizontal gap between columns in points.</param>
        /// <param name="verticalGap">The vertical gap between rows in points.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="columns"/> is less than or equal to zero.</exception>
        public static MultiPageLayout Grid(int columns, float horizontalGap, float verticalGap)
        {
            if (columns <= 0)
                throw new ArgumentOutOfRangeException("columns", "Columns must be greater than zero.");

            return new MultiPageLayout(MultiPageLayoutMode.Grid, columns, horizontalGap, verticalGap);
        }

        /// <summary>
        /// Creates a layout where each page is rendered as a separate frame in a multi-frame TIFF image.
        /// Applicable only to TIFF image formats.
        /// </summary>
        public static MultiPageLayout TiffFrames()
        {
            return new MultiPageLayout(MultiPageLayoutMode.TiffFrames, 0, 0, 0);
        }

        /// <summary>
        /// Creates a layout that renders only the first of specified pages.
        /// </summary>
        public static MultiPageLayout SinglePage()
        {
            return new MultiPageLayout(MultiPageLayoutMode.SinglePage, 0, 0, 0);
        }

        /// <summary>
        /// Gets or sets the background color of the output.
        /// The default is <see cref="Color.Empty"/>.
        /// </summary>
        public Color BackColor
        {
            get { return mBackColor.ToNativeColor(); }
            set { mBackColor = DrColor.FromNativeColor(value); }
        }

        /// <summary>
        /// Gets or sets the color of the pages border.
        /// The default is <see cref="Color.Empty"/>.
        /// </summary>
        public Color BorderColor
        {
            get { return mBorderColor.ToNativeColor(); }
            set { mBorderColor = DrColor.FromNativeColor(value); }
        }

        /// <summary>
        /// Gets or sets the width of the pages border.
        /// The default is 0.
        /// </summary>
        public float BorderWidth
        {
            get { return mBorderWidth; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Border width must be non-negative.");

                mBorderWidth = value;
            }
        }

        /// <summary>
        /// Gets a layout mode for rendering multiple pages into a single output.
        /// </summary>
        internal MultiPageLayoutMode Mode
        {
            get { return mMode; }
        }

        /// <summary>
        /// Gets the number of columns in a grid layout.
        /// </summary>
        /// <remarks>
        /// This property is used only if the layout mode is <c>Grid</c>.
        /// </remarks>
        internal int Columns
        {
            get { return mColumns; }
        }

        /// <summary>
        /// Gets the horizontal gap between pages in points.
        /// </summary>
        internal float HorizontalGap
        {
            get { return mHorizontalGap; }
        }

        /// <summary>
        /// Gets the vertical gap between pages in points.
        /// </summary>
        internal float VerticalGap
        {
            get { return mVerticalGap; }
        }

        internal DrColor BackColorCore
        {
            get { return mBackColor; }
        }

        internal DrColor BorderColorCore
        {
            get { return mBorderColor; }
        }

        internal bool HasBorder
        {
            get { return (mBorderColor != DrColor.Empty) && (mBorderWidth > 0); }
        }

        private float mBorderWidth;
        private DrColor mBorderColor;
        private DrColor mBackColor;
        private readonly float mVerticalGap;
        private readonly float mHorizontalGap;
        private readonly int mColumns;
        private readonly MultiPageLayoutMode mMode;
    }
}
