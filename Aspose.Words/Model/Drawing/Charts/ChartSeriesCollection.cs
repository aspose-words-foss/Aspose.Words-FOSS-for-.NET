// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/13/2015 by Andrey Noskov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Represents collection of a <see cref="ChartSeries"/>.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-charts/">Working with Charts</a> documentation article.</para>
    /// </summary>
    public class ChartSeriesCollection : IEnumerable<ChartSeries>
    {
        internal ChartSeriesCollection(IChartSeriesSource chartSeriesSource)
        {
            mChartSeriesSource = chartSeriesSource;
        }

        /// <summary>
        /// Returns a <see cref="ChartSeries"/> at the specified index.
        /// </summary>
        /// <include file='..\..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the collection.</param>
        public ChartSeries this[int index]
        {
            get { return Series[index]; }
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<ChartSeries> GetEnumerator()
        {
            return Series.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Removes a <see cref="ChartSeries"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="ChartSeries"/> to remove.</param>
        public void RemoveAt(int index)
        {
            mChartSeriesSource.RemoveAt(index);
        }

        /// <summary>
        /// Removes all <see cref="ChartSeries"/> from this collection.
        /// </summary>
        public void Clear()
        {
            mChartSeriesSource.Clear();
        }

        /// <summary>
        /// Adds new <see cref="ChartSeries"/> to this collection.
        /// Use this method to add series to any type of Bar, Column, Line and Surface charts.
        /// </summary>
        /// <returns>Recently added <see cref="ChartSeries"/> object.</returns>
        public ChartSeries Add(string seriesName, string[] categories, double[] values)
        {
            return ApplyStyle(Add(DestinationChart, seriesName, categories, values));
        }

        /// <summary>
        /// Adds new <see cref="ChartSeries"/> to this collection.
        /// Use this method to add series to Waterfall charts.
        /// </summary>
        /// <remarks>
        /// For chart types other than Waterfall, <paramref name="isSubtotal"/> values are ignored.
        /// </remarks>
        /// <param name="seriesName">A name of the series to be added.</param>
        /// <param name="categories">Category names for the X axis.</param>
        /// <param name="values">Y-axis values.</param>
        /// <param name="isSubtotal">Values indicating whether the corresponding Y value is a subtotal.</param>
        /// <returns>Recently added <see cref="ChartSeries"/> object.</returns>
        public ChartSeries Add(string seriesName, string[] categories, double[] values, bool[] isSubtotal)
        {
            return ApplyStyle(Add(DestinationChart, seriesName, categories, values, isSubtotal));
        }

        /// <summary>
        /// Adds new <see cref="ChartSeries"/> to this collection.
        /// Use this method to add series to any type of Scatter charts.
        /// </summary>
        /// <returns>Recently added <see cref="ChartSeries"/> object.</returns>
        public ChartSeries Add(string seriesName, double[] xValues, double[] yValues)
        {
            return ApplyStyle(Add(DestinationChart, seriesName, xValues, yValues));
        }

        /// <summary>
        /// Adds new <see cref="ChartSeries"/> to this collection.
        /// Use this method to add series to any type of Area, Radar and Stock charts.
        /// </summary>
        public ChartSeries Add(string seriesName, DateTime[] dates, double[] values)
        {
            return ApplyStyle(Add(DestinationChart, seriesName, dates, values));
        }

        /// <summary>
        /// Adds new <see cref="ChartSeries"/> to this collection.
        /// Use this method to add series to any type of Bubble charts.
        /// </summary>
        /// <returns>Recently added <see cref="ChartSeries"/> object.</returns>
        public ChartSeries Add(string seriesName, double[] xValues, double[] yValues, double[] bubbleSizes)
        {
            return ApplyStyle(Add(DestinationChart, seriesName, xValues, yValues, bubbleSizes));
        }

        /// <summary>
        /// Adds new <see cref="ChartSeries"/> to this collection.
        /// Use this method to add series that have multi-level data categories.
        /// </summary>
        /// <returns>Recently added <see cref="ChartSeries"/> object.</returns>
        public ChartSeries Add(string seriesName, ChartMultilevelValue[] categories, double[] values)
        {
            return ApplyStyle(Add(DestinationChart, seriesName, categories, values));
        }

        /// <summary>
        /// Adds new <see cref="ChartSeries"/> to this collection.
        /// Use this method to add series to Histogram charts.
        /// </summary>
        /// <remarks>
        /// For chart types other than Histogram, this method adds a series with empty Y values.
        /// </remarks>
        /// <returns>Recently added <see cref="ChartSeries"/> object.</returns>
        public ChartSeries Add(string seriesName, double[] xValues)
        {
            return ApplyStyle(Add(DestinationChart, seriesName, xValues));
        }

        /// <summary>
        /// Adds a new <see cref="ChartSeries"/> to this collection as a child of the specified DML chart.
        /// </summary>
        internal ChartSeries Add(DmlChart dmlChart, string seriesName, string[] categories, double[] values)
        {
            if ((categories == null) || (values == null))
                throw new ArgumentException("Data arrays must not be null.");

            if ((categories.Length <= 0) || (values.Length <= 0))
                throw new ArgumentException("Data arrays must not be empty.");

            if (categories.Length != values.Length)
                throw new ArgumentException("Data arrays must be of the same size.");

            ChartSeries series = AddCore(dmlChart, seriesName, values);

            series.SetXValues(CollectStringValues(categories, dmlChart.IsChartEx));

            return series;
        }

        /// <summary>
        /// Adds a new <see cref="ChartSeries"/> to this collection as a child of the specified DML chart.
        /// </summary>
        internal ChartSeries Add(DmlChart dmlChart, string seriesName, double[] xValues, double[] yValues)
        {
            if ((xValues == null) || (yValues == null))
                throw new ArgumentException("Data arrays must not be null.");

            if ((xValues.Length <= 0) || (yValues.Length <= 0))
                throw new ArgumentException("Data arrays must not be empty.");

            if (xValues.Length != yValues.Length)
                throw new ArgumentException("Data arrays must be of the same size.");

            ChartSeries series = AddCore(dmlChart, seriesName, yValues);

            series.SetXValues(CollectNumValues(xValues, dmlChart.IsChartEx));

            return series;
        }

        /// <summary>
        /// Adds a new <see cref="ChartSeries"/> to this collection as a child of the specified DML chart.
        /// </summary>
        private ChartSeries Add(DmlChart dmlChart, string seriesName, DateTime[] dates, double[] values)
        {
            if ((dates == null) || (values == null))
                throw new ArgumentException("Data arrays must not be null.");

            if ((dates.Length <= 0) || (values.Length <= 0))
                throw new ArgumentException("Data arrays must not be empty.");

            if (dates.Length != values.Length)
                throw new ArgumentException("Data arrays must be of the same size.");

            ChartSeries series = AddCore(dmlChart, seriesName, values);

            series.SetXValues(CollectDateValues(dates, dmlChart.IsChartEx));

            // WORDSNET-16119 Mark the category axis as date/time. (Except radar chart like MS Word does.)
            if (!dmlChart.IsRadarChart)
                dmlChart.SetXAxisDateCategoryAttribute(true);

            return series;
        }

        /// <summary>
        /// Adds a new <see cref="ChartSeries"/> to this collection as a child of the specified DML chart.
        /// </summary>
        internal ChartSeries Add(DmlChart dmlChart, string seriesName, double[] xValues, double[] yValues,
            double[] bubbleSizes)
        {
            if ((xValues == null) || (yValues == null))
                throw new ArgumentException("Data arrays must not be null.");

            if ((xValues.Length <= 0) || (yValues.Length <= 0))
                throw new ArgumentException("Data arrays must not be empty.");

            if (xValues.Length != yValues.Length)
                throw new ArgumentException("Data arrays must be of the same size.");

            if (dmlChart.ChartType != DmlChartType.BubbleChart)
                dmlChart.Warn(WarningType.DataLoss, WarningStrings.CannotSetBubbleSize);

            ChartSeries series = AddCore(dmlChart, seriesName, yValues);

            series.DefaultDataPoint.PointPr.SetProperty(
                DmlChartDataPointAttr.Bubble3D,
                (series.Chart is DmlBubbleChart) && ((DmlBubbleChart)series.Chart).Bubble3D);

            series.SetXValues(CollectNumValues(xValues, dmlChart.IsChartEx));

            if (bubbleSizes != null)
                series.Size.Values = CollectNumValues(bubbleSizes, dmlChart.IsChartEx);

            return series;
        }

        /// <summary>
        /// Adds a new <see cref="ChartSeries"/> to this collection as a child of the specified DML chart.
        /// </summary>
        private ChartSeries Add(DmlChart dmlChart, string seriesName, ChartMultilevelValue[] categories, double[] values)
        {
            if ((categories == null) || (values == null))
                throw new ArgumentException("Data arrays must not be null.");

            if ((categories.Length == 0) || (values.Length == 0))
                throw new ArgumentException("Data arrays must not be empty.");

            if (categories.Length != values.Length)
                throw new ArgumentException("Data arrays must be of the same size.");

            ChartSeries series = AddCore(dmlChart, seriesName, values);

            series.SetXValues(CollectMultilevelValues(categories));

            return series;
        }

        /// <summary>
        /// Adds a new <see cref="ChartSeries"/> to this collection as a child of the specified DML chart.
        /// </summary>
        private ChartSeries Add(DmlChart dmlChart, string seriesName, double[] xValues)
        {
            if (xValues == null)
                throw new ArgumentException("Data array must not be null.");

            if (xValues.Length == 0)
                throw new ArgumentException("Data array must not be empty.");

            // Need to provide Y values for non-Histogram charts, otherwise there is an exception when writing the document.
            double[] yValues = (dmlChart.SeriesType != ChartSeriesType.Histogram)
                ? GenerateArrayOfNaN(xValues.Length)
                : null;

            ChartSeries series = AddCore(dmlChart, seriesName, yValues);

            series.SetXValues(CollectNumValues(xValues, dmlChart.IsChartEx));

            return series;
        }

        /// <summary>
        /// Generates an array of <see cref="double.NaN"/> values.
        /// </summary>
        private static double[] GenerateArrayOfNaN(int length)
        {
            double[] data = new double[length];

            for (int i = 0; i < length; i++)
                data[i] = double.NaN;

            return data;
        }

        /// <summary>
        /// Adds a new <see cref="ChartSeries"/> to this collection as a child of the specified DML chart.
        /// </summary>
        private ChartSeries Add(DmlChart dmlChart, string seriesName, string[] categories, double[] yValues,
            bool[] isSubtotal)
        {
            if ((categories == null) || (yValues == null) || (isSubtotal == null))
                throw new ArgumentException("Data arrays must not be null.");

            if ((categories.Length == 0) || (yValues.Length == 0) || (isSubtotal.Length == 0))
                throw new ArgumentException("Data arrays must not be empty.");

            if ((categories.Length != yValues.Length) || (categories.Length != isSubtotal.Length))
                throw new ArgumentException("Data arrays must be of the same size.");

            ChartSeries series = AddCore(dmlChart, seriesName, yValues);

            series.SetXValues(CollectStringValues(categories, dmlChart.IsChartEx));

            if (dmlChart.SeriesType == ChartSeriesType.Waterfall)
                FillSubTotalIndexes(series, isSubtotal);

            return series;
        }

        /// <summary>
        /// Fills the <see cref="DmlChartSeriesLayoutPr.SubTotals"/> property of the specified series with indexes of
        /// subtotal data points.
        /// </summary>
        private static void FillSubTotalIndexes(ChartSeries series, bool[] isSubtotal)
        {
            series.LayoutPr.SubTotals = new List<int>();

            for (int i = 0; i < isSubtotal.Length; i++)
            {
                if (isSubtotal[i])
                    series.LayoutPr.SubTotals.Add(i);
            }
        }

        private ChartSeries AddCore(DmlChart dmlChart, string seriesName, double[] values)
        {
            ChartSeries series = new ChartSeries(dmlChart);
            series.Name = seriesName;
            series.ConfigureChartExLayout(dmlChart.SeriesType);
            series.CreateChartExDataStorage();

            SetSeriesTypedProperties(series);

            if (values != null)
                series.SetYValues(CollectNumValues(values, dmlChart.IsChartEx));

            // After adding a series, an existing embedded/linked XLSX document does not contain the added data, we need
            // to remove it, so that when the document is opened, MS Word will generate a new embedded file that includes
            // the entire chart data.
            DmlChartSpace chartSpace = mChartSeriesSource.DestinationChart.ChartSpace;
            chartSpace.RemoveExternalDataLinkage();

            DmlChartPlotArea plotArea = chartSpace.ChartFormat.PlotArea;
            plotArea.AddSeries(series);

            if (series.SeriesType == ChartSeriesType.Pareto)
                AddParetoLineSeries(dmlChart, series);

            return series;
        }

        /// <summary>
        /// Adds a pareto line series to the DML chart using the specified chart series as an owner.
        /// </summary>
        private static void AddParetoLineSeries(DmlChart dmlChart, ChartSeries ownerSeries)
        {
            ChartSeries paretoLine = new ChartSeries(dmlChart);
            paretoLine.LayoutType = SeriesLayout.ParetoLine;
            paretoLine.Owner = ownerSeries;
            dmlChart.PlotArea.AddSeries(paretoLine);

            ChartAxis paretoLineAxis = dmlChart.PlotArea.GetOrCreateSecondaryYAxis();
            AxisScaling scaling = new AxisScaling();
            scaling.Minimum = new AxisBound(0);
            scaling.Maximum = new AxisBound(1);
            paretoLineAxis.ChartAxisPr.SetProperty(DmlChartAxisAttrs.Scaling, scaling);
            paretoLineAxis.DisplayUnit.Unit = AxisBuiltInUnit.Percentage;
            paretoLineAxis.HasMajorGridlines = false;

            paretoLine.AxisId = paretoLineAxis.AxId;
        }

        private static void SetSeriesTypedProperties(ChartSeries series)
        {
            // This is required because if SpPr is not set explicitly, the default properties are returned.
            // If customer changes properties, all series with default properties are changed, that is incorrect,
            // only concrete series must be changed.
            series.DefaultDataPoint.PointPr.SetProperty(DmlChartDataPointAttr.SpPr, new DmlChartSpPr());

            // MS Word initializes invertIfNegative to false by default. And the property has this value in created
            // charts. Let's do the same.
            if (ChartDataPointCollection.SupportsInvertIfNegative(series.Chart.ChartType))
                series.InvertIfNegative = false;

            switch (series.Chart.ChartType)
            {
                case DmlChartType.LineChart:
                case DmlChartType.RadarChart:
                {
                    SetMarkerDefaults(series, MarkerSymbol.None);
                    break;
                }
                case DmlChartType.StockChart:
                {
                    SetMarkerDefaults(series, MarkerSymbol.None);
                    SetScatterSpPrDefaults(series);
                    break;
                }
                case DmlChartType.ScatterChart:
                {
                    SetMarkerDefaults(series, MarkerSymbol.Circle);
                    SetScatterSpPrDefaults(series);
                    break;
                }
                case DmlChartType.PieChart:
                case DmlChartType.Pie3DChart:
                case DmlChartType.OfPieChart:
                case DmlChartType.DoughnutChart:
                {
                    SetPieChartSpPrDefaults(series);
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        private static void SetPieChartSpPrDefaults(ChartSeries series)
        {
            DmlChartSpPr spPr = new DmlChartSpPr();
            spPr.Outline = new DmlOutline();
            DmlSchemeColor schemeColor = new DmlSchemeColor(ThemeColor.Light1);
            spPr.Outline.Fill = new DmlSolidFill(schemeColor);
            spPr.Outline.WidthInEmus = 19050;
            series.DefaultDataPoint.PointPr.SetProperty(DmlChartDataPointAttr.SpPr, spPr);
        }

        private static void SetScatterSpPrDefaults(ChartSeries series)
        {
            DmlChartSpPr spPr = new DmlChartSpPr();
            spPr.Outline = new DmlOutline();
            spPr.Outline.Fill = new DmlNoFill();
            spPr.Outline.EndCap = EndCap.Round;
            spPr.Outline.WidthInEmus = 19050;
            series.DefaultDataPoint.PointPr.SetProperty(DmlChartDataPointAttr.SpPr, spPr);
        }

        private static void SetMarkerDefaults(ChartSeries series, MarkerSymbol markerStyle)
        {
            ChartMarker marker = new ChartMarker(series.Chart);
            marker.MarkerPr.SetProperty(DmlChartMarkerAttr.Symbol, markerStyle);
            series.DefaultDataPoint.PointPr.SetProperty(DmlChartDataPointAttr.Marker, marker);
        }

        private static DmlChartValueCollection CollectNumValues(double[] values, bool asMultiLevelChartValue)
        {
            return CollectNumValues(values, asMultiLevelChartValue, ChartNumberFormat.GeneralFormatCode);
        }

        private static DmlChartValueCollection CollectNumValues(double[] values,
            bool asMultiLevelChartValue, string formatCode)
        {
            int i = 0;
            DmlChartValueCollection dmlChartValues = new DmlChartValueCollection(
                asMultiLevelChartValue ? DmlChartValueType.MultiLvlNumeric : DmlChartValueType.Numeric);

            dmlChartValues.FormatCode = formatCode;

            foreach (double value in values)
            {
                // Consider double.NaN value as empty values in chart data.
                // On the one hand the same have to be done for chart DateValues (CollectDateValues method).
                // But on the other hand, DateValues are used as X-values and if one of them is empty all corresponding Y-values
                // of series will be ignored (MSW behavior). And also since DateTime is a value type you cannot assign null to it.
                if (!double.IsNaN(value))
                {
                    // Do not add to the value collection data points with double.NaN values.

                    if (asMultiLevelChartValue)
                    {
                        DmlChartMultiLvlNumValue chartValue = new DmlChartMultiLvlNumValue(i, null);
                        chartValue.Levels.Add(value);
                        dmlChartValues.Add(chartValue);
                    }
                    else
                    {
                        dmlChartValues.Add(new DmlChartNumValue(i, value));
                    }
                }

                i++;
            }

            dmlChartValues.ValueCount = values.Length;
            return dmlChartValues;
        }

        private static DmlChartValueCollection CollectDateValues(DateTime[] dates, bool asMultiLevelChartValue)
        {
            return CollectNumValues(
                ToWordNumericDateArray(dates),
                asMultiLevelChartValue,
                ChartNumberFormat.DefaultDateFormatCode);
        }

        private static DmlChartValueCollection CollectStringValues(string[] categories, bool asMultiLevelChartValue)
        {
            int i = 0;
            DmlChartValueCollection dmlChartValues = new DmlChartValueCollection(
                asMultiLevelChartValue ? DmlChartValueType.MultiLvlString : DmlChartValueType.String);

            foreach (string category in categories)
            {
                if (asMultiLevelChartValue)
                {
                    DmlChartMultiLvlStrValue chartValue = new DmlChartMultiLvlStrValue(i);
                    chartValue.Levels.Add(category);
                    dmlChartValues.Add(chartValue);
                }
                else
                {
                    dmlChartValues.Add(new DmlChartStrValue(i, category));
                }

                i++;
            }

            dmlChartValues.ValueCount = categories.Length;

            return dmlChartValues;
        }

        /// <summary>
        /// Collects the specified multi-level values in a created <see cref="DmlChartValueCollection"/>.
        /// </summary>
        private static DmlChartValueCollection CollectMultilevelValues(ChartMultilevelValue[] values)
        {
            DmlChartValueCollection dmlChartValues = new DmlChartValueCollection(DmlChartValueType.MultiLvlString);

            int levelCount = GetLevelCount(values);
            dmlChartValues.SetValueCountForAllLevels(values.Length, levelCount);

            int i = 0;

            foreach (ChartMultilevelValue multilevelValue in values)
            {
                DmlChartMultiLvlStrValue value = new DmlChartMultiLvlStrValue(i++);

                if (levelCount == 3)
                    value.Levels.Add(multilevelValue.Level3);
                if (levelCount >= 2)
                    value.Levels.Add(multilevelValue.Level2);
                value.Levels.Add(multilevelValue.Level1);

                dmlChartValues.Add(value);
            }

            return dmlChartValues;
        }

        /// <summary>
        /// Gets the maximum number of levels in the specified multi-level values.
        /// </summary>
        private static int GetLevelCount(ChartMultilevelValue[] values)
        {
            int levelCount = 1;

            foreach (ChartMultilevelValue multilevelValue in values)
            {
                if (StringUtil.HasChars(multilevelValue.Level3))
                    return 3;

                if (StringUtil.HasChars(multilevelValue.Level2))
                    levelCount = 2;
            }

            return levelCount;
        }

        /// <summary>
        /// Converts the specified datetime values to Word's numeric datetime representations.
        /// </summary>
        private static double[] ToWordNumericDateArray(DateTime[] dates)
        {
            double[] doubles = new double[dates.Length];

            for (int i = 0; i < dates.Length; i++)
                doubles[i] = DmlChartUtil.GetDoubleFromDate(dates[i]);

            return doubles;
        }

        /// <summary>
        /// Applies the current chart style to the specified series and returns it.
        /// </summary>
        private ChartSeries ApplyStyle(ChartSeries series)
        {
            mChartSeriesSource.DestinationChart.ChartSpace.ApplyChartStyle(series);
            return series;
        }

        /// <summary>
        /// Returns the number of <see cref="ChartSeries"/> in this collection.
        /// </summary>
        public int Count
        {
            get { return Series.Count; }
        }

        /// <summary>
        /// Gets a list of series of the plot area.
        /// </summary>
        private IList<ChartSeries> Series
        {
            get { return mChartSeriesSource.SeriesList; }
        }

        /// <summary>
        /// Gets the DML chart to which adding series are placed.
        /// Currently series are added to the first DML chart of the plot area only.
        /// </summary>
        private DmlChart DestinationChart
        {
            get { return mChartSeriesSource.DestinationChart; }
        }

        private readonly IChartSeriesSource mChartSeriesSource;
    }
}
