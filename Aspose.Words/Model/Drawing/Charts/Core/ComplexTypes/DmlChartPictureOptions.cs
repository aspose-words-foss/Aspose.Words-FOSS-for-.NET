// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents 5.7.2.139 pictureOptions (Picture Options) element.
    /// This element specifies the picture to be used on the data point, series, wall, or floor.
    /// </summary>
    internal class DmlChartPictureOptions
    {
        internal DmlChartPictureOptions Clone()
        {
           return (DmlChartPictureOptions)MemberwiseClone();
        }

        internal bool ApplyToFront
        {
            get { return mApplyToFront; }
            set { mApplyToFront = value; }
        }

        internal bool ApplyToSides
        {
            get { return mApplyToSides; }
            set { mApplyToSides = value; }
        }

        internal bool ApplyToEnd
        {
            get { return mApplyToEnd; }
            set { mApplyToEnd = value; }
        }

        internal PictureFormat PictureFormat
        {
            get { return mPictureFormat; }
            set { mPictureFormat = value; }
        }

        /// <summary>
        /// Zero means that property is not set.
        /// </summary>
        internal double PictureStackUnit
        {
            get { return mPictureStackUnit; }
            set
            {
                // Value must be greater than zero.
                if (mPictureStackUnit > 0)
                    mPictureStackUnit = value;
            }
        }

        private bool mApplyToFront;
        private bool mApplyToSides;
        private bool mApplyToEnd;
        private PictureFormat mPictureFormat;
        private double mPictureStackUnit;
    }
}
