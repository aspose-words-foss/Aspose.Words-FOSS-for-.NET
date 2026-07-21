// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/06/2012 by Alexey Noskov

using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;

namespace Aspose.Words.Drawing.Charts.Core
{
    internal class DmlBubbleChart : DmlCartesianPlaneChart
    {
        internal override DmlChartType ChartType
        {
            get { return DmlChartType.BubbleChart; }
        }

        /// <summary>
        /// Gets the type of series that such DML chart contains by default.
        /// </summary>
        internal override ChartSeriesType DefaultSeriesType
        {
            get { return Bubble3D ? ChartSeriesType.Bubble3D : ChartSeriesType.Bubble; }
        }

        /// <dev>
        /// This property is not written to a document, but is taken from a series.
        /// It is used to assign to a corresponding property of a new chart series.
        /// </dev>
        internal bool Bubble3D { get; set; }

        internal int BubbleScale
        {
            get { return (int)ChartPr.GetProperty(DmlChartAttrs.BubbleScale); }
            set { ChartPr.SetProperty(DmlChartAttrs.BubbleScale, value); }
        }

        internal bool ShowNegBubbles
        {
            get { return (bool)ChartPr.GetProperty(DmlChartAttrs.ShowNegBubbles); }
            set { ChartPr.SetProperty(DmlChartAttrs.ShowNegBubbles, value); }
        }

        internal SizeRepresents SizeRepresents
        {
            get { return (SizeRepresents)ChartPr.GetProperty(DmlChartAttrs.SizeRepresents); }
            set { ChartPr.SetProperty(DmlChartAttrs.SizeRepresents, value); }
        }

        /// <summary>
        /// Loop through all bubbles sizes and selects the maximum one to use it as <see cref="MaxBubbleSize"/>.
        /// </summary>
        private void RecalculateMaxBubbleSize()
        {
            mMaxBubbleSize = 0.0f;

            foreach (ChartSeries series in Series)
            {
                for (int i = 0; i < series.ValueCount; i++)
                {
                    DmlChartValue s = series.Size.GetValue(i);
                    if (!DmlChartValue.IsNullOrNaN(s))
                        mMaxBubbleSize = System.Math.Max(mMaxBubbleSize, s.FloatValue);
                }
            }
        }

        /// <summary>
        /// Returns the maximum bubble size of the current chart.
        /// </summary>
        internal float MaxBubbleSize
        {
            get
            {
                if (float.IsNaN(mMaxBubbleSize))
                    RecalculateMaxBubbleSize();

                return mMaxBubbleSize;
            }
        }

        private float mMaxBubbleSize = float.NaN;
    }
}
