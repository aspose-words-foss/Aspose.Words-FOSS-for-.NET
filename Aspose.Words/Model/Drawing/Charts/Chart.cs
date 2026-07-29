// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/13/2015 by Andrey Noskov

using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Provides access to the chart shape properties.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// There are text properties at the chart (chart space) level, but when a chart is created in MS Word, child elements
    /// of a chart override text properties. Therefore implementing a Chart.Font property does not provide much benefit,
    /// actually it would affect only data labels shown in AW, whose text properties are then empty. It looks like Word
    /// VBA has no the Chart.Font property. Moreover, MS Word ignores any text properties defined in a chart space in
    /// Word 2016 charts. So it seems there is no need to implement this property in AW too.
    /// </dev>
    public class Chart : IChartFormatSource
    {
        internal Chart(DmlChartSpace dmlChartSpace)
        {
            mChartSpace = dmlChartSpace;
        }

        /// <summary>
        /// Provides access to series collection.
        /// </summary>
        public ChartSeriesCollection Series
        {
            get
            {
                if (mSeriesCollection == null)
                    mSeriesCollection = new ChartSeriesCollection(PlotArea);

                return mSeriesCollection;
            }
        }

        /// <summary>
        /// Provides access to a series group collection of this chart.
        /// </summary>
        public ChartSeriesGroupCollection SeriesGroups
        {
            get
            {
                if (mSeriesGroups == null)
                    mSeriesGroups = new ChartSeriesGroupCollection(mChartSpace);

                return mSeriesGroups;
            }
        }

        /// <summary>
        /// Provides access to the chart title properties.
        /// </summary>
        public ChartTitle Title
        {
            get
            {
                if (mTitle == null)
                    mTitle = new ChartTitle(ChartFormat);

                return mTitle;
            }
        }

        /// <summary>
        /// Provides access to the chart legend properties.
        /// </summary>
        public ChartLegend Legend
        {
            get
            {
                if (ChartFormat.Legend == null)
                    ChartFormat.Legend = new ChartLegend(ChartFormat);

                return ChartFormat.Legend;
            }
        }

        /// <summary>
        /// Provides access to properties of a data table of this chart.
        /// The data table can be shown using the <see cref="ChartDataTable.Show"/> property.
        /// </summary>
        public ChartDataTable DataTable
        {
            get
            {
                if (PlotArea.DataTable == null)
                    PlotArea.DataTable = ChartDataTable.CreateDataTableWithDefaultFormat(PlotArea);

                return PlotArea.DataTable;
            }
        }

        /// <summary>
        /// Provides access to properties of the primary X axis of the chart.
        /// </summary>
        public ChartAxis AxisX
        {
            get { return PlotArea.PrimaryXAxis; }
        }

        /// <summary>
        /// Provides access to properties of the primary Y axis of the chart.
        /// </summary>
        public ChartAxis AxisY
        {
            get { return PlotArea.PrimaryYAxis; }
        }

        /// <summary>
        /// Provides access to properties of the Z axis of the chart.
        /// </summary>
        public ChartAxis AxisZ
        {
            get { return PlotArea.ZAxis; }
        }

        /// <summary>
        /// Gets a collection of all axes of this chart.
        /// </summary>
        public ChartAxisCollection Axes
        {
            get
            {
                if (mAxes == null)
                    mAxes = new ChartAxisCollection(mChartSpace);

                return mAxes;
            }
        }

        /// <summary>
        /// Gets the path and name of an xls/xlsx file this chart is linked to.
        /// </summary>
        public string SourceFullName
        {
            get { return mChartSpace.LinkedData; }
            set { mChartSpace.LinkedData = value; }
        }

        /// <summary>
        /// Provides access to fill and line formatting of the chart.
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
        /// Gets or sets the style of the chart.
        /// </summary>
        public ChartStyle Style
        {
            get
            {
                return (ChartSpace.DmlChartStyle != null)
                    ? ChartStyleResolver.GetChartStyle(ChartSpace.DmlChartStyle.IntId)
                    : ChartStyle.Normal;
            }
            set { ChartSpace.SetChartStyle(value); }
        }

        /// <summary>
        /// Returns the document to which this chart belongs.
        /// </summary>
        internal DocumentBase Document
        {
            get { return mChartSpace.Dml.Document; }
        }

        /// <summary>
        /// Returns a chart space instance that contains all information about the chart.
        /// </summary>
        internal DmlChartSpace ChartSpace
        {
            get { return mChartSpace; }
        }

        /// <summary>
        /// Returns a chart format instance that contains chart properties.
        /// </summary>
        private DmlChartFormat ChartFormat
        {
            get { return mChartSpace.ChartFormat; }
        }

        /// <summary>
        /// Returns a plot area of this chart.
        /// </summary>
        private DmlChartPlotArea PlotArea
        {
            get { return ChartFormat.PlotArea; }
        }

        #region IChartFormatSource implementation

        void IChartFormatSource.MaterializeSpPr()
        {
            // Do nothing for a chart.
        }

        bool IChartFormatSource.IsFillSupported
        {
            get { return true; }
        }

        DmlFill IChartFormatSource.Fill
        {
            get { return ChartSpace.SpPr.Fill; }
            set { ChartSpace.SpPr.Fill = value; }
        }

        DmlOutline IChartFormatSource.Outline
        {
            get { return ChartSpace.SpPr.Outline; }
            set { ChartSpace.SpPr.Outline = value; }
        }

        ChartShapeType IChartFormatSource.ShapeType
        {
            get
            {
                // Not supported by a chart.
                return ChartShapeType.Default;
            }
            set
            {
                // Not supported by a chart.
            }
        }

        IThemeProvider IChartFormatSource.ThemeProvider
        {
            get { return Document.GetThemeInternal(); }
        }

        bool IChartFormatSource.IsFormatDefined
        {
            get { return !ChartSpace.SpPr.IsEmpty; }
        }

        #endregion

        private ChartSeriesCollection mSeriesCollection;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DmlChartSpace mChartSpace;
        private ChartAxisCollection mAxes;
        private ChartTitle mTitle;
        private ChartFormat mFormat;
        private ChartSeriesGroupCollection mSeriesGroups;
    }
}
