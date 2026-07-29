// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    internal class DmlPointsTextSpacing : DmlTextSpacing
    {
        internal DmlPointsTextSpacing()
            : this(new DmlTextPoints())
        {
        }

        internal DmlPointsTextSpacing(DmlTextPoints value)
        {
            mValue = value;
        }

        /// <summary>
        /// Clones this instance of text spacing.
        /// </summary>
        internal override DmlTextSpacing Clone()
        {
            DmlPointsTextSpacing lhs = (DmlPointsTextSpacing)MemberwiseClone();

            if (mValue != null)
                lhs.mValue = mValue.Clone();

            return lhs;
        }

        /// <summary>
        /// Specifies the size of the white space in point size. Whole points are specified in increments of
        /// 100 starting with 100 being a point size of 1. For instance a font point size of 12 would be 1200
        /// and a font point size of 12.5 would be 1250.
        /// </summary>
        internal DmlTextPoints Value
        {
            get { return mValue; }
        }

        private DmlTextPoints mValue = new DmlTextPoints(); // For Java
    }
}
