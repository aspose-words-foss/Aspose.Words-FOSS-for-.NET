// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/09/2012 by Ilya Egorov

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Represents charts rendered on a Cartesian plane.
    /// </summary>
    internal abstract class DmlCartesianPlaneChart : DmlChart, IDmlChart2D
    {
        /// <summary>
        /// Clones this DML chart.
        /// </summary>
        internal override DmlChart Clone()
        {
            DmlCartesianPlaneChart lhs = (DmlCartesianPlaneChart)base.Clone();
            lhs.mAxX = null;
            lhs.mAxY = null;

            return lhs;
        }

        /// <summary>
        /// Returns id of the X-axis.
        /// </summary>
        public int AxIdX
        {
            get { return (int)ChartPr.GetProperty(DmlChartAttrs.AxIdX); }
        }

        /// <summary>
        /// Returns id of the Y-axis.
        /// </summary>
        public int AxIdY
        {
            get { return (int)ChartPr.GetProperty(DmlChartAttrs.AxIdY); }
        }

        public virtual ChartAxis AxX
        {
            get
            {
                if (mAxX == null)
                    mAxX = PlotArea.GetCrossAxis(AxIdY, AxIdX);

                return mAxX;
            }

            set { mAxX = value; }
        }

        public virtual ChartAxis AxY
        {
            get
            {
                if (mAxY == null)
                    mAxY = PlotArea.GetAxis(AxIdY);

                return mAxY;
            }

            set { mAxY = value; }
        }

        private ChartAxis mAxX;
        private ChartAxis mAxY;
    }
}
