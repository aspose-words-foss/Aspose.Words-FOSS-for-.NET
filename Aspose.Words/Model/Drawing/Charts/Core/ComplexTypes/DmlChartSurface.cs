// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/07/2012 by Alexey Noskov

using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing.Charts.Core.ComplexTypes
{
    /// <summary>
    /// Represents CT_Surface complex type that specifies the back wall, side wall or floor of the chart.
    /// </summary>
    internal class DmlChartSurface : DmlExtensionListSource
    {
        internal DmlChartSurface Clone()
        {
            DmlChartSurface lhs = (DmlChartSurface)MemberwiseClone();
            if (mSpPr != null)
                lhs.mSpPr = mSpPr.Clone();

            if (mPictureOptions != null)
                lhs.mPictureOptions = mPictureOptions.Clone();

            lhs.Extensions = CloneExtensions();

            return lhs;
        }

        internal DmlChartPictureOptions PictureOptions
        {
            get { return mPictureOptions; }
            set { mPictureOptions = value; }
        }

        internal DmlChartSpPr SpPr
        {
            get
            {
                if (mSpPr == null)
                    mSpPr = new DmlChartSpPr();

                return mSpPr;
            }
        }

        internal int Thickness
        {
            get { return mThickness; }
            set { mThickness = value; }
        }

        private DmlChartPictureOptions mPictureOptions;
        private DmlChartSpPr mSpPr;
        private int mThickness;
    }
}
