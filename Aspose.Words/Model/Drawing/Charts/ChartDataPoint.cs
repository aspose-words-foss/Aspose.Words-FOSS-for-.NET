// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2012 by Alexey Noskov

using System;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Allows to specify formatting of a single data point on the chart.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// On a series, the <see cref="ChartDataPoint"/> object is a member of the <see cref="ChartDataPointCollection"/>. 
    /// The <see cref="ChartDataPointCollection"/> contains a <see cref="ChartDataPoint"/> object for each point. 
    /// </remarks>
    public class ChartDataPoint : IChartDataPoint, IChartFormatSource
    {
        // Represents 5.7.2.52 dPt (Data Point) element.

        internal ChartDataPoint(DmlChart chart)
        {
            mChart = chart;
            mPointPr = new DmlChartDataPointPr((chart != null) ? chart.Document : null);
        }

        /// <summary>
        /// Clears format of this data point. The properties are set to the default values defined in the parent series.
        /// </summary>
        public void ClearFormat()
        {
            int index = Index;

            mPointPr.Clear();

            Index = index;

            if (mMarker != null)
            {
                mMarker.MarkerPr.Clear();
                mPointPr.SetProperty(DmlChartDataPointAttr.Marker, mMarker);
            }
        }

        /// <summary>
        /// Copies format from the specified source data point.
        /// </summary>
        /// <remarks>
        /// The format of this data point is cleared, and only the changed properties are copied from the source data point.
        /// </remarks>
        /// <dev>
        /// This method is intended to copy format from a data point of the same <see cref="ChartDataPointCollection"/>.
        /// </dev>
        internal void CopyFormatFrom(ChartDataPoint dataPoint)
        {
            if (this == dataPoint)
                return;

            // Preserve the index.
            int index = Index;
            mPointPr = dataPoint.PointPr.Clone();
            Index = index;

            // If SpPr and Marker are present then they are contained in mPointPr and hence are cloned 
            // together with mPointPr, so we should reference the clones from mPointPr.
            mSpPr = (DmlChartSpPr)mPointPr.GetDirectProperty(DmlChartDataPointAttr.SpPr);
            mMarker = (ChartMarker)mPointPr.GetDirectProperty(DmlChartDataPointAttr.Marker);

            mFormat = null;
        }

        /// <summary>
        /// Merges changed properties from the specified source data point.
        /// </summary>
        internal void MergeFormatFrom(ChartDataPoint dataPoint)
        {
            PointPr.CopyDirectPropertiesFrom(dataPoint.PointPr);

            // If SpPr and/or Marker are affected, need to get the new values.
            mSpPr = (DmlChartSpPr)mPointPr.GetDirectProperty(DmlChartDataPointAttr.SpPr);
            mMarker = (ChartMarker)mPointPr.GetDirectProperty(DmlChartDataPointAttr.Marker);
        }

        internal void SetParent(ChartDataPoint parent)
        {
            mPointPr.SetParent(parent.PointPr);
        }

        internal ChartDataPoint Clone()
        {
            ChartDataPoint lhs = (ChartDataPoint)MemberwiseClone();
            if (mPointPr != null)
                lhs.mPointPr = mPointPr.Clone();

            // If mSpPr and mMarker are present then they are contained in mPointPr and hence are cloned 
            // together with mPointPr, so we should reference the clones from lhs.mPointPr for lhs.
            if (mSpPr != null)
                lhs.mSpPr = (DmlChartSpPr)lhs.mPointPr.GetProperty(DmlChartDataPointAttr.SpPr);

            if (mMarker != null)
                lhs.mMarker = (ChartMarker)lhs.mPointPr.GetProperty(DmlChartDataPointAttr.Marker);

            lhs.mFormat = null;

            return lhs;
        }

        /// <summary>
        /// Sets the parent chart of this data point.
        /// </summary>
        internal void SetChart(DmlChart chart)
        {
            mChart = chart;
        }

        /// <summary>
        /// Sets <see cref="DmlChartSpPr"/> for this data point.
        /// </summary>
        internal void SetSpPr(DmlChartSpPr spPr)
        {
            mSpPr = spPr;
            mPointPr.SetProperty(DmlChartDataPointAttr.SpPr, mSpPr);
        }

        private void SetMarker(ChartMarker marker)
        {
            mMarker = marker;
            mPointPr.SetProperty(DmlChartDataPointAttr.Marker, mMarker);
        }

        #region IChartFormatSource implementation

        void IChartFormatSource.MaterializeSpPr()
        {
            // This un-links data point SpPr from SpPr of series default data point. Do it only on changing properties,
            // not on reading, for the behavior to not depend on a fact whether a customer has read ChartFormat properties
            // or hasn't.

            if (mPointPr.IsPropertySpecified(DmlChartDataPointAttr.SpPr))
                return;
            
            DmlChartSpPr parentSpPr = (DmlChartSpPr)mPointPr.GetProperty(DmlChartDataPointAttr.SpPr);
            DmlChartSpPr spPr = (parentSpPr != null) ? parentSpPr.Clone() : new DmlChartSpPr();
            SetSpPr(spPr);
        }

        bool IChartFormatSource.IsFillSupported
        {
            get { return true; }
        }

        DmlFill IChartFormatSource.Fill
        {
            get { return SpPr.Fill; }
            set { SpPr.Fill = value; }
        }

        DmlOutline IChartFormatSource.Outline
        {
            get { return SpPr.Outline; }
            set { SpPr.Outline = value; }
        }

        ChartShapeType IChartFormatSource.ShapeType
        {
            get
            {
                return ChartShapeType.Default;
            }
            set
            {
                if (value != ChartShapeType.Default)
                    throw new InvalidOperationException("Cannot change the shape type of this chart element.");
            }
        }

        IThemeProvider IChartFormatSource.ThemeProvider
        {
            get { return mChart.Document.GetThemeInternal(); }
        }

        bool IChartFormatSource.IsFormatDefined
        {
            get { return mPointPr.IsPropertySpecified(DmlChartDataPointAttr.SpPr) && !SpPr.IsEmpty; }
        }

        #endregion

        /// <summary>
        /// Index of the data point this object applies formatting to.
        /// </summary>
        public int Index
        {
            get { return (int)mPointPr.GetProperty(DmlChartDataPointAttr.Index); }
            internal set { mPointPr.SetProperty(DmlChartDataPointAttr.Index, value); }
        }

        /// <summary>
        /// Specifies the amount the data point shall be moved from the center of the pie.
        /// Can be negative, negative means that property is not set and no explosion should be applied.
        /// Applies only to Pie charts.
        /// </summary>
        public int Explosion
        {
            get { return (int)mPointPr.GetProperty(DmlChartDataPointAttr.Explosion); }
            set
            {
                if (mChart.IsPieChart)
                    mPointPr.SetProperty(DmlChartDataPointAttr.Explosion, value);
                else
                    mChart.Warn(WarningType.MinorFormattingLoss, "Explosion is not supported by this type of chart, value will not be set.");
            }
        }

        /// <summary>
        /// Specifies whether the parent element shall inverts its colors if the value is negative.
        /// </summary>
        public bool InvertIfNegative
        {
            get { return (bool)mPointPr.GetProperty(DmlChartDataPointAttr.InvertIfNegative); }
            set { mPointPr.SetProperty(DmlChartDataPointAttr.InvertIfNegative, value); }
        }

        /// <summary>
        /// Specifies whether the bubbles in Bubble chart should have a 3-D effect applied to them.
        /// </summary>
        public bool Bubble3D
        {
            get { return (bool)mPointPr.GetProperty(DmlChartDataPointAttr.Bubble3D); }
            set
            {
                if (mChart.ChartType == DmlChartType.BubbleChart)
                    mPointPr.SetProperty(DmlChartDataPointAttr.Bubble3D, value);
                else
                    mChart.Warn(WarningType.MinorFormattingLoss, "Bubble3D is not supported by this type of chart, value will not be set.");
            }
        }

        /// <summary>
        /// Provides access to fill and line formatting of this data point.
        /// </summary>
        public ChartFormat Format
        {
            get
            {
                if (mFormat == null)
                    mFormat = new ChartFormat(this);

                return mFormat;
            }
        }

        /// <summary>
        /// Specifies a data marker. Marker is automatically created when requested.
        /// </summary>
        internal bool HasMarker
        {
            get { return (mPointPr.IsPropertySpecified(DmlChartDataPointAttr.Marker)); }
        }

        /// <summary>
        /// Specifies the formatting for the series.
        /// </summary>
        internal DmlChartSpPr SpPr
        {
            get
            {
                // This kind of lazy initialization is used for java.
                // For some reason in multi-threads environment SpPr and Marker from defaults are changed and tests fails.
                // So removed SpPr and Marker from default and init them here on demand.
                if (mSpPr == null)
                    mSpPr = (DmlChartSpPr)mPointPr.GetProperty(DmlChartDataPointAttr.SpPr);

                if (mSpPr == null)
                    SetSpPr(new DmlChartSpPr());

                return mSpPr;
            }
        }

        /// <summary>
        /// Specifies chart data marker.
        /// </summary>
        public ChartMarker Marker
        {
            get
            {
                if (mMarker == null)
                    mMarker = (ChartMarker)mPointPr.GetDirectProperty(DmlChartDataPointAttr.Marker);

                if (mMarker == null)
                    SetMarker(new ChartMarker(mChart));

                return mMarker;
            }
        }

        /// <summary>
        /// Specifies the picture to be used on the series.
        /// </summary>
        internal DmlChartPictureOptions PictureOptions
        {
            get { return (DmlChartPictureOptions)mPointPr.GetProperty(DmlChartDataPointAttr.PictureOptions); }
        }

        internal DmlChartDataPointPr PointPr
        {
            get { return mPointPr; }
        }

        internal DmlChart Chart
        {
            get { return mChart; }
        }

        /// <summary>
        /// Returns a flag indicating whether any properties of this data point have different values than the
        /// corresponding properties of the parent collection.
        /// </summary>
        internal bool HasNonDefaultFormatting
        {
            get { return mPointPr.HasNonDefaultFormatting; }
        }

        private DmlChartDataPointPr mPointPr;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private DmlChart mChart;

        private DmlChartSpPr mSpPr;
        private ChartMarker mMarker;
        private ChartFormat mFormat;
    }
}
