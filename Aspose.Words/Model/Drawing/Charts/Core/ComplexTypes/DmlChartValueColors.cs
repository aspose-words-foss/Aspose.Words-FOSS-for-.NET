// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/03/2017 by Alexander Zhiltsov

using Aspose.Words.Drawing.Core.Dml.Colors;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// This class represents the 2.24.3.86 CT_ValueColors complex type [MS-ODRAWXML]. It specifies
    /// the color of individual stops composing a gradient for representing data values as color.
    /// </summary>
    internal class DmlChartValueColors
    {
        /// <summary>
        /// Returns a copy of this instance.
        /// </summary>
        internal DmlChartValueColors Clone()
        {
            DmlChartValueColors lhs = (DmlChartValueColors)MemberwiseClone();

            if (mMinimumColor != null)
                lhs.mMinimumColor = mMinimumColor.Clone();
            if (mMiddleColor != null)
                lhs.mMiddleColor = mMiddleColor.Clone();
            if (mMaximumColor != null)
                lhs.mMaximumColor = mMaximumColor.Clone();

            return lhs;
        }

        /// <summary>
        /// Gets or sets the color of the lowest position of the gradient.
        /// </summary>
        internal DmlColor MinimumColor
        {
            get { return mMinimumColor; }
            set { mMinimumColor = value; }
        }

        /// <summary>
        /// Gets or sets the color of the position between the lowest and highest positions of the gradient.
        /// </summary>
        internal DmlColor MiddleColor
        {
            get { return mMiddleColor; }
            set { mMiddleColor = value; }
        }

        /// <summary>
        /// Gets or sets the color of the highest position of the gradient.
        /// </summary>
        internal DmlColor MaximumColor
        {
            get { return mMaximumColor; }
            set { mMaximumColor = value; }
        }

        /// <summary>
        /// Returns true if this instance of <see cref="DmlChartValueColors"/> is empty (all the colors are <c>null</c>).
        /// </summary>
        public bool IsEmpty
        {
            get { return (mMinimumColor == null) && (mMiddleColor == null) && (mMaximumColor == null); }
        }

        private DmlColor mMinimumColor;
        private DmlColor mMiddleColor;
        private DmlColor mMaximumColor;
    }
}
