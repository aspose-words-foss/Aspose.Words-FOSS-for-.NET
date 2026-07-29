// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/06/2023 by Alexander Zhiltsov

using System;
using Aspose.Words.Drawing.Charts.Core;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Provides access to the axis title properties.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with
    /// Charts</a> documentation article.</para>
    /// </summary>
    public class ChartAxisTitle
    {
        /// <summary>
        /// Customers cannot create instances of this class.
        /// </summary>
        internal ChartAxisTitle(IDmlChartTitleHolder chartTitleHolder)
        {
            mTitleHolder = chartTitleHolder;
        }

        /// <summary>
        /// Gets or sets the text of the axis title.
        /// If <c>null</c> or empty value is specified, auto generated title will be shown.
        /// </summary>
        /// <remarks>Use <see cref="Show"/> option if you need to show the title.</remarks>
        public string Text
        {
            get { return DmlTitle.Text; }
            set { DmlTitle.Text = value; }
        }

        /// <summary>
        /// Determines whether other chart elements shall be allowed to overlap the title.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool Overlay
        {
            get { return DmlTitle.Overlay; }
            set { DmlTitle.Overlay = value; }
        }

        /// <summary>
        /// Determines whether the title shall be shown for the axis.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool Show
        {
            get { return !mTitleHolder.TitleDeleted; }
            set
            {
                if (Show == value)
                    return;

                // It is needed to create a title instance when showing the title, otherwise it will become invisible when
                // calling DmlTitle.get.
                if (value && (mTitleHolder.DCTitle == null))
                    mTitleHolder.DCTitle = new DmlChartTitle(mTitleHolder);

                // Sets default text format for the showing title.
                if (value)
                    DmlTitle.EnsureInitialized();

                mTitleHolder.TitleDeleted = !value;
            }
        }

        /// <summary>
        /// Provides access to the font formatting of the axis title.
        /// </summary>
        public Font Font
        {
            get { return DmlTitle.Font; }
        }

        /// <summary>
        /// Provides access to fill and line formatting of the axis title.
        /// </summary>
        public ChartFormat Format
        {
            get { return DmlTitle.Format; }
        }

        /// <summary>
        /// Gets or sets the orientation of the axis title text.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="ShapeTextOrientation.Horizontal"/>.
        /// </remarks>
        public ShapeTextOrientation Orientation
        {
            get { return DmlTitle.Orientation; }
            set
            {
                if (IsChartEx)
                    throw new InvalidOperationException(DmlChartUtil.ChartExUnsupportedProperty);

                DmlTitle.Orientation = value;
            }
        }

        /// <summary>
        /// Gets or sets the rotation of the axis title in degrees.
        /// </summary>
        /// <remarks>
        /// The range of acceptable values is from -180 to 180 inclusive.
        /// </remarks>
        public int Rotation
        {
            get { return DmlTitle.Rotation; }
            set
            {
                if (IsChartEx)
                    throw new InvalidOperationException(DmlChartUtil.ChartExUnsupportedProperty);

                ArgumentUtil.CheckRangeInclusive(value, -180, 180, "value");
                DmlTitle.Rotation = value;
            }
        }

        /// <summary>
        /// Gets or creates the internal <see cref="DmlChartTitle"/> instance to get/set property values.
        /// </summary>
        private DmlChartTitle DmlTitle
        {
            get
            {
                if (mTitleHolder.DCTitle == null)
                {
                    mTitleHolder.DCTitle = new DmlChartTitle(mTitleHolder);
                    // The title is hidden by default.
                    Show = false;
                }

                return mTitleHolder.DCTitle;
            }
        }

        /// <summary>
        /// Indicates that this chart is an extension of the ISO/IEC 29000 format that is defined by
        /// the http://schemas.microsoft.com/office/drawing/2015/9/8/chartex schema of [MS-ODRAWXML].
        /// The extension is introduced in MS Word 2016.
        /// </summary>
        private bool IsChartEx
        {
            get { return mTitleHolder.ChartSpace.IsChartEx; }
        }

        private readonly IDmlChartTitleHolder mTitleHolder;
    }
}
