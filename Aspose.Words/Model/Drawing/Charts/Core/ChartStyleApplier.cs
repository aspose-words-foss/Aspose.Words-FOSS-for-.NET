// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/01/2025 by Alexander Zhiltsov

using System;
using System.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Themes;

namespace Aspose.Words.Drawing.Charts.Core
{
    /// <summary>
    /// Applies the assigned chart style to the chart.
    /// </summary>
    internal class ChartStyleApplier
    {
        /// <summary>
        /// Ctor with a chart space provided.
        /// </summary>
        internal ChartStyleApplier(DmlChartSpace chartSpace)
        {
            mChartSpace = chartSpace;
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the entire chart.
        /// </summary>
        internal void ApplyStyle()
        {
            foreach (DmlChart chart in mChartSpace.Charts)
                DefineStyleDependentProperties(chart);

            ApplyChartStyleToChartElements();
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the specified series.
        /// </summary>
        internal void ApplyStyle(ChartSeries series)
        {
            ApplyStyleToSeries(series, series.Index);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the specified data labels.
        /// </summary>
        internal void ApplyStyle(ChartDataLabelCollection dataLabels)
        {
            FixDataLabelPosition(dataLabels);

            DmlChartStyle style = mChartSpace.DmlChartStyle;
            DmlChart chart = dataLabels.Chart;
            DmlChartStyleEntry labelsStyleEntry = style[DmlChartStyleItem.DataLabel];

            if (chart.IsPieChart && (chart.ChartType != DmlChartType.DoughnutChart))
            {
                foreach (ChartDataLabel dataLabel in dataLabels)
                    ApplyStyleEntryToDataLabel(dataLabel, labelsStyleEntry);
            }
            else
            {
                ApplyStyleEntry(dataLabels.SpPr, dataLabels.TxPr, labelsStyleEntry);
            }

            //DmlChartStyleItem.DataLabelCallout is not currently supported.

            DmlChartSpPr leaderLinesSpPr =
                (DmlChartSpPr)dataLabels.LabelPr.GetProperty(DmlChartDataLabelAttrs.LeaderLines);
            if (leaderLinesSpPr != null)
                ApplyStyleEntry(leaderLinesSpPr, null, style[DmlChartStyleItem.LeaderLine]);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the specified axis.
        /// </summary>
        internal void ApplyStyle(ChartAxis axis)
        {
            if (axis == null)
                return;

            DmlChartStyle style = mChartSpace.DmlChartStyle;

            ApplyStyleToAxisTitle(axis);

            DmlChartStyleItem styleItem = GetAxisStyleItem(axis);
            ApplyStyleEntry(axis.SpPr, axis.TxPr, style[styleItem]);

            if (axis.MajorGridlines != null)
                ApplyStyleEntry(axis.MajorGridlines.SpPr, null, style[DmlChartStyleItem.GridlineMajor]);

            if (axis.MinorGridlines != null)
                ApplyStyleEntry(axis.MinorGridlines.SpPr, null, style[DmlChartStyleItem.GridlineMinor]);
        }

        /// <summary>
        /// Applies the specified chart style item to the text properties.
        /// </summary>
        internal void ApplyStyle(DmlChartTxPr txPr, DmlChartStyleItem styleItem)
        {
            ApplyStyleEntryToTxPr(txPr, mChartSpace.DmlChartStyle[styleItem], 0);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the specified legend.
        /// </summary>
        internal void ApplyStyle(ChartLegend legend)
        {
            if (legend == null)
                return;

            DmlChartStyleEntry styleEntry = mChartSpace.DmlChartStyle[DmlChartStyleItem.Legend];
            ApplyStyleEntry(legend.SpPr, legend.TxPr, styleEntry);

            foreach (ChartLegendEntry entry in legend.LegendEntries.MaterializedLegendEntries)
                ApplyStyleEntry(null, entry.TxPr, styleEntry, entry.Index);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the specified data table.
        /// </summary>
        internal void ApplyStyle(ChartDataTable dataTable)
        {
            if (dataTable != null)
                ApplyStyleEntry(dataTable.SpPr, dataTable.TxPr, mChartSpace.DmlChartStyle[DmlChartStyleItem.DataTable]);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the specified marker.
        /// </summary>
        internal void ApplyStyle(ChartMarker marker, int chartElementIndex)
        {
            if (marker.Symbol == MarkerSymbol.None)
                return;

            DmlChartStyle style = mChartSpace.DmlChartStyle;
            DmlChartMarkerPr markerPr = marker.MarkerPr;
            DmlChartSpPr spPr = (DmlChartSpPr)markerPr.GetDirectProperty(DmlChartMarkerAttr.SpPr);
            if (spPr == null)
            {
                // Set cloned inherited value.
                spPr = (DmlChartSpPr)markerPr.GetProperty(DmlChartMarkerAttr.SpPr);
                spPr = (spPr != null) ? spPr.Clone() : new DmlChartSpPr();
                markerPr.SetProperty(DmlChartMarkerAttr.SpPr, spPr);
            }

            ApplyStyleEntry(spPr, null, style[DmlChartStyleItem.DataPointMarker], chartElementIndex);

            if (style.DataPointMarkerLayout.Size != 0)
                marker.Size = style.DataPointMarkerLayout.Size;
            else
                marker.MarkerPr.RemoveProperty(DmlChartMarkerAttr.Size);

            if (style.DataPointMarkerLayout.Symbol != string.Empty)
            {
                MarkerSymbol symbol = (style.DataPointMarkerLayout.MarkerSymbol != MarkerSymbol.Default)
                    ? style.DataPointMarkerLayout.MarkerSymbol
                    : gMarkerAutoSymbols[chartElementIndex % gMarkerAutoSymbols.Length];

                marker.SetSymbolInternal(symbol);
            }
            else
            {
                marker.MarkerPr.RemoveProperty(DmlChartMarkerAttr.Symbol);
            }
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the chart title.
        /// </summary>
        internal void ApplyStyleToChartTitle()
        {
            DmlChartTitle title = ChartFormat.DCTitle;
            if (title != null)
                ApplyStyleEntry(title.SpPr, title.TxPr, mChartSpace.DmlChartStyle[DmlChartStyleItem.Title]);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the title of the specified axis.
        /// </summary>
        internal void ApplyStyleToAxisTitle(ChartAxis axis)
        {
            DmlChartTitle title = axis.TitleInternal;
            if (title != null)
                ApplyStyleEntry(title.SpPr, title.TxPr, mChartSpace.DmlChartStyle[DmlChartStyleItem.AxisTitle]);
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to all elements of the chart.
        /// </summary>
        private void ApplyChartStyleToChartElements()
        {
            DmlChartStyle style = mChartSpace.DmlChartStyle;
            DmlChartPlotArea plotArea = ChartFormat.PlotArea;

            ApplyStyleEntry(mChartSpace.SpPr, mChartSpace.TxPr, style[DmlChartStyleItem.ChartArea]);

            DmlChartStyleItem plotAreaItem = ((plotArea.FirstChart != null) && plotArea.FirstChart.Is3D)
                ? DmlChartStyleItem.PlotArea3D
                : DmlChartStyleItem.PlotArea;
            ApplyStyleEntry(plotArea.SpPr, null, style[plotAreaItem]);

            ApplyStyleToChartTitle();
            ApplyStyle(plotArea.DataTable);

            DmlChartSurface floor = ChartFormat.Floor;
            if (floor != null)
                ApplyStyleEntry(floor.SpPr, null, style[DmlChartStyleItem.Floor]);

            DmlChartSurface backWall = ChartFormat.BackWall;
            if (backWall != null)
                ApplyStyleEntry(backWall.SpPr, null, style[DmlChartStyleItem.Wall]);

            DmlChartSurface sideWall = ChartFormat.SideWall;
            if (sideWall != null)
                ApplyStyleEntry(sideWall.SpPr, null, style[DmlChartStyleItem.Wall]);

            ApplyStyle(ChartFormat.Legend);

            foreach (ChartAxis axis in mChartSpace.ChartFormat.PlotArea.Axes)
                ApplyStyle(axis);

            foreach (DmlChart chart in mChartSpace.Charts)
            {
                DmlChartPr chartPr = chart.ChartPr;
                DmlChartUpDownBars upDownBars = (DmlChartUpDownBars)chartPr.GetDirectProperty(DmlChartAttrs.UpDownBars);
                if (upDownBars != null)
                {
                    ApplyStyleEntry(upDownBars.DownBars, null, style[DmlChartStyleItem.DownBar]);
                    ApplyStyleEntry(upDownBars.UpBars, null, style[DmlChartStyleItem.UpBar]);
                }

                DmlChartSpPr dropLineSpPr =
                    GetChartSpPr(chartPr, DmlChartAttrs.IsDropLinesVisible, DmlChartAttrs.DropLines);
                if (dropLineSpPr != null)
                    ApplyStyleEntry(dropLineSpPr, null, style[DmlChartStyleItem.DropLine]);

                DmlChartSpPr seriesLinesSpPr =
                    GetChartSpPr(chartPr, DmlChartAttrs.ShowSerLine, DmlChartAttrs.SerLines);
                if (seriesLinesSpPr != null)
                    ApplyStyleEntry(seriesLinesSpPr, null, style[DmlChartStyleItem.SeriesLine]);

                DmlChartSpPr hiLowLinesSpPr =
                    GetChartSpPr(chartPr, DmlChartAttrs.IsHiLowLinesVisible, DmlChartAttrs.HiLowLines);
                if (hiLowLinesSpPr != null)
                    ApplyStyleEntry(hiLowLinesSpPr, null, style[DmlChartStyleItem.HighLowLine]);

                for (int i = 0; i < chart.Series.Count; i++)
                    ApplyStyleToSeries(chart.Series[i], i);

                DmlSurfaceChart surfaceChart = chart as DmlSurfaceChart;
                if (surfaceChart != null)
                {
                    for (int i = 0; i < surfaceChart.BandFmts.Count; i++)
                    {
                        DmlChartSpPr bandSpPr = surfaceChart.BandFmts.GetFmt(i);
                        ApplyStyleEntry(bandSpPr, null, style[DmlChartStyleItem.DataPoint], i);
                    }
                }
            }
        }

        /// <summary>
        /// Applies the chart style assigned to the chart space to the specified series using the specified index to get
        /// style element color.
        /// </summary>
        private void ApplyStyleToSeries(ChartSeries series, int chartElementIndex)
        {
            MarkMarkersForShowingIfRequired(series);

            DmlChartStyle style = mChartSpace.DmlChartStyle;
            DmlChart chart = series.Chart;

            if (series.HasDataLabels)
            {
                ChartDataLabelCollection dataLabels = series.DataLabels;
                ApplyStyle(dataLabels);
            }

            if (series.XErrorBars != null)
                ApplyStyleEntry(series.XErrorBars.SpPr, null, style[DmlChartStyleItem.ErrorBar]);

            if (series.YErrorBars != null)
                ApplyStyleEntry(series.YErrorBars.SpPr, null, style[DmlChartStyleItem.ErrorBar]);

            DmlChartStyleItem dataPointItem = GetDataPointStyleItem(chart);
            if (chart.IsPieChart)
            {
                for (int j = 0; j < series.DataPoints.Count; j++)
                {
                    ChartDataPoint dataPoint = series.DataPoints[j];
                    ((IChartFormatSource)dataPoint).MaterializeSpPr();
                    ApplyStyleEntry(dataPoint.SpPr, null, style[dataPointItem], j);
                }
            }
            else
            {
                DmlChartSpPr spPr = series.DefaultDataPoint.SpPr;
                bool noLine =
                    NeedPreserveDataPointNoLine(chart.ChartType) &&
                    (spPr != null) &&
                    ((spPr.Outline.Fill == null) || (spPr.Outline.Fill.DmlFillType == DmlFillType.NoFill));

                ApplyStyleEntry(series.DefaultDataPoint.SpPr, null, style[dataPointItem], chartElementIndex);

                if (noLine)
                    spPr.Outline.Fill = new DmlNoFill();
            }

            ApplyStyle(series.DefaultDataPoint.Marker, chartElementIndex);

            foreach (DmlChartTrendline trendLine in series.Trendlines)
            {
                ApplyStyleEntry(trendLine.SpPr, null, style[DmlChartStyleItem.TrendLine]);

                DmlChartTrendlineLabel trendLineLabel = trendLine.TrendlineLbl;
                if (trendLineLabel != null)
                    ApplyStyleEntry(trendLineLabel.SpPr, trendLineLabel.TxPr, style[DmlChartStyleItem.TrendlineLabel]);
            }
        }

        /// <summary>
        /// Applies the specified chart style entry to the data label.
        /// </summary>
        private void ApplyStyleEntryToDataLabel(ChartDataLabel dataLabel, DmlChartStyleEntry styleEntry)
        {
            DmlChartDataLabelPr labelPr = dataLabel.LabelPr;

            DmlChartSpPr spPr = (DmlChartSpPr)labelPr.GetDirectProperty(DmlChartDataLabelAttrs.SpPr);
            if (spPr == null)
            {
                // Set cloned inherited value.
                spPr = (DmlChartSpPr)labelPr.GetProperty(DmlChartDataLabelAttrs.SpPr);
                spPr = (spPr != null) ? spPr.Clone() : new DmlChartSpPr();
                labelPr.SetProperty(DmlChartDataLabelAttrs.SpPr, spPr);
            }

            DmlChartTxPr txPr = (DmlChartTxPr)labelPr.GetDirectProperty(DmlChartDataLabelAttrs.TxPr);
            if (txPr == null)
            {
                // Set cloned inherited value.
                txPr = (DmlChartTxPr)labelPr.GetProperty(DmlChartDataLabelAttrs.TxPr);
                txPr = (txPr != null) ? txPr.Clone() : new DmlChartTxPr();
                labelPr.SetProperty(DmlChartDataLabelAttrs.TxPr, txPr);
            }

            ApplyStyleEntry(spPr, txPr, styleEntry, dataLabel.Index);
        }

        /// <summary>
        /// Gets a flag indicating whether if a series of the specified chart type has no line between data points before
        /// applying a style, there should be no line after applying the style.
        /// </summary>
        private static bool NeedPreserveDataPointNoLine(DmlChartType chartType)
        {
            switch (chartType)
            {
                // Data points on stock and scatter charts should not be connected by lines.
                case DmlChartType.ScatterChart:
                case DmlChartType.StockChart:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets a style item that should be applied to data points of the specified chart.
        /// </summary>
        private static DmlChartStyleItem GetDataPointStyleItem(DmlChart chart)
        {
            // For information about which data point style items apply to which chart types, see 2.8.3.1
            // CT_ChartStyle [MS-ODRAWXML].

            if ((chart.ChartType == DmlChartType.SurfaceChart) && ((DmlSurfaceChart)chart).Wireframe)
                return DmlChartStyleItem.DataPointWireframe;

            if (chart.Is3D)
                return DmlChartStyleItem.DataPoint3D;

            if ((chart.ChartType == DmlChartType.LineChart) ||
                (chart.ChartType == DmlChartType.RadarChart) ||
                (chart.ChartType == DmlChartType.ScatterChart))
            {
                return DmlChartStyleItem.DataPointLine;
            }

            return DmlChartStyleItem.DataPoint;
        }

        /// <summary>
        /// Gets a style item that should be applied to the specified axis.
        /// </summary>
        private static DmlChartStyleItem GetAxisStyleItem(ChartAxis axis)
        {
            DmlChartStyleItem styleItem;
            switch (axis.Type)
            {
                case ChartAxisType.Series:
                    styleItem = DmlChartStyleItem.SeriesAxis;
                    break;
                case ChartAxisType.Value:
                    styleItem = DmlChartStyleItem.ValueAxis;
                    break;
                case ChartAxisType.Category:
                default:
                    styleItem = DmlChartStyleItem.CategoryAxis;
                    break;
            }

            return styleItem;
        }

        /// <summary>
        /// Gets a chart attribute of the <see cref="DmlChartSpPr"/> type if the corresponding visibility attribute is
        /// <b>true</b>.
        /// </summary>
        private static DmlChartSpPr GetChartSpPr(DmlChartPr chartPr, DmlChartAttrs visibilityAttr, DmlChartAttrs spPrAttr)
        {
            return (bool)chartPr.GetProperty(visibilityAttr)
                ? (DmlChartSpPr)chartPr.GetDirectProperty(spPrAttr)
                : null;
        }

        /// <summary>
        /// Applies the chart style entry to the specified shape and text properties.
        /// </summary>
        private void ApplyStyleEntry(DmlChartSpPr spPr, DmlChartTxPr txPr, DmlChartStyleEntry styleEntry)
        {
            ApplyStyleEntry(spPr, txPr, styleEntry, 0);
        }

        /// <summary>
        /// Applies the chart style entry to the specified shape and text properties using the specified index to get
        /// element color.
        /// </summary>
        private void ApplyStyleEntry(DmlChartSpPr spPr, DmlChartTxPr txPr, DmlChartStyleEntry styleEntry,
            int chartElementIndex)
        {
            if (mChartSpace.IsChartEx)
            {
                // Word 2016 chart automatically gets style from ChartStyle.
                if (spPr != null)
                    spPr.Clear();
                if (txPr != null)
                    txPr.Clear();
                return;
            }

            ApplyStyleEntryToSpPr(spPr, styleEntry, chartElementIndex);
            ApplyStyleEntryToTxPr(txPr, styleEntry, chartElementIndex);
        }

        /// <summary>
        /// Applies the chart style entry to the specified shape properties.
        /// </summary>
        private void ApplyStyleEntryToSpPr(DmlChartSpPr spPr, DmlChartStyleEntry styleEntry, int chartElementIndex)
        {
            if (spPr == null)
                return;

            // styleEntry.Modifiers are currently not supported.

            DefaultShapeProperties shapePr = styleEntry.ShapePr;

            if ((shapePr != null) && (shapePr.Fill != null))
                spPr.Fill = shapePr.Fill.Clone();
            else if (styleEntry.FillReference.StyleMatrixIndex > 0)
                spPr.Fill = Theme.GetFillStyle(styleEntry.FillReference.StyleMatrixIndex - 1);
            else
                spPr.Fill = new DmlNoFill();

            ApplyStyleColor(spPr.Fill, styleEntry.FillReference.Color, chartElementIndex);

            if ((shapePr != null) && (shapePr.Outline != null))
                spPr.Outline = shapePr.Outline.Clone();
            else if (styleEntry.LineReference.StyleMatrixIndex > 0)
                spPr.Outline = Theme.GetLineStyle(styleEntry.LineReference.StyleMatrixIndex - 1);
            else
                spPr.Outline = CreateEmptyDmlOutline();

            if ((spPr.Outline != null) && (styleEntry.LineReference.Color != null))
                ApplyStyleColor(spPr.Outline.Fill, styleEntry.LineReference.Color, chartElementIndex);

            EffectStyle themeEffectStyle = (styleEntry.EffectReference.StyleMatrixIndex > 0)
                ? Theme.GetEffectStyle(styleEntry.EffectReference.StyleMatrixIndex - 1)
                : null;
            if ((shapePr != null) && (shapePr.Effects != null))
            {
                spPr.Effects = shapePr.Effects.Clone();
            }
            else if (themeEffectStyle != null)
            {
                spPr.Effects = themeEffectStyle.Effects.Clone();
                spPr.Effects.IsTheme = false;
            }
            else
            {
                spPr.Effects = null;
            }

            if ((spPr.Effects != null) && (styleEntry.EffectReference.Color != null))
            {
                DmlColor color = ResolveStyleColor(styleEntry.EffectReference.Color, chartElementIndex);
                foreach (DmlShapeEffect effect in spPr.Effects)
                    effect.Color = AddColorModifiers(color.Clone(), effect.Color.ColorModifiers);
            }

            if ((shapePr != null) && (shapePr.Scene3DProperties != null))
                spPr.Scene3DProp = shapePr.Scene3DProperties.Clone();
            else if (themeEffectStyle != null)
                spPr.Scene3DProp = themeEffectStyle.Scene3DProperties;
            else
                spPr.Scene3DProp = null;

            if ((shapePr != null) && (shapePr.Shape3DProperties != null))
                spPr.Shape3DProp = shapePr.Shape3DProperties.Clone();
            else if (themeEffectStyle != null)
                spPr.Shape3DProp = themeEffectStyle.Shape3DProperties;
            else
                spPr.Shape3DProp = null;

            if ((shapePr != null) && (shapePr.SpPrExtensions != null))
                spPr.Extensions = DmlExtensionListSource.CloneExtensions(shapePr.SpPrExtensions);
        }

        /// <summary>
        /// Applies the specified color to the fill.
        /// </summary>
        private void ApplyStyleColor(DmlFill fill, DmlColor color, int chartElementIndex)
        {
            if ((fill == null) || (color == null))
                return;

            color = ResolveStyleColor(color, chartElementIndex);

            if ((fill.DmlColorInternal != null) && (fill.DmlColorInternal.ColorType == DmlColorType.PlaceholderColor))
                fill.DmlColorInternal = AddColorModifiers(color.Clone(), fill.DmlColorInternal.ColorModifiers);

            if ((fill.DmlColor2Internal != null) && (fill.DmlColor2Internal.ColorType == DmlColorType.PlaceholderColor))
                fill.DmlColor2Internal = AddColorModifiers(color.Clone(), fill.DmlColor2Internal.ColorModifiers);

            if (fill.DmlFillType == DmlFillType.GradientFill)
            {
                DmlGradientFill gradientFill = (DmlGradientFill)fill;
                foreach (DmlGradientStop gradientStop in gradientFill.GradientStops)
                {
                    if (gradientStop.Color.ColorType == DmlColorType.PlaceholderColor)
                        gradientStop.Color = AddColorModifiers(color.Clone(), gradientStop.Color.ColorModifiers);
                }
            }
        }

        /// <summary>
        /// If the passed color is a <see cref="DmlChartStyleColor"/> instance, gets the color from the chart's colors
        /// part using the specified index.
        /// </summary>
        private DmlColor ResolveStyleColor(DmlColor color, int chartElementIndex)
        {
            if (color.ColorType != DmlColorType.ChartStyleColor)
                return color;

            DmlChartStyleColor styleColor = (DmlChartStyleColor)color;
            int colorIndex = styleColor.IsAuto ? chartElementIndex : styleColor.ColorIndex;
            return mChartSpace.ColorStyle.GetColor(colorIndex);
        }

        /// <summary>
        /// Adds the modifiers to the specified color.
        /// </summary>
        private static DmlColor AddColorModifiers(DmlColor color, IEnumerable<IDmlColorModifier> colorModifiers)
        {
            color = color.Clone();

            foreach (IDmlColorModifier colorModifier in colorModifiers)
            {
                // It seems MS Word allows multiple modifiers of the same type and applies them all.
                color.ColorModifiers.Add(colorModifier.Clone());
            }

            return color;
        }

        /// <summary>
        /// Creates a <see cref="DmlOutline"/> instance with no fill.
        /// </summary>
        private static DmlOutline CreateEmptyDmlOutline()
        {
            DmlOutline outline = new DmlOutline();
            outline.Fill = new DmlNoFill();
            return outline;
        }

        /// <summary>
        /// Applies the chart style entry to the specified text properties.
        /// </summary>
        private void ApplyStyleEntryToTxPr(DmlChartTxPr txPr, DmlChartStyleEntry styleEntry, int chartElementIndex)
        {
            if (txPr == null)
                return;

            txPr.BodyPr = (styleEntry.TextBodyPr != null) ? styleEntry.TextBodyPr.Clone() : null;

            DmlChartFontReference fontReference = styleEntry.FontReference;
            DmlFontCollectionIndex fontIndex = fontReference.FontCollectionIndex;
            DmlFill fill =  (fontReference.Color != null)
                ? new DmlSolidFill(ResolveStyleColor(fontReference.Color, chartElementIndex))
                : null;

            DmlRunProperties defaultRunPr = styleEntry.DefaultRunPr;
            if (defaultRunPr != null)
            {
                defaultRunPr = defaultRunPr.Clone();
                SetFonts(defaultRunPr, fontIndex);
                defaultRunPr.Fill = fill;

                // It seems MS Word expands non-defined properties using defaults to not inherit from chart space or
                // from chart element specific defaults. Do the same.
                ExpandProperties(defaultRunPr);
            }

            SetParagraphRunPr(txPr.FirstParagraph, defaultRunPr);

            foreach (DmlParagraph para in txPr.Paragraphs)
                SetParagraphRunPr(para, defaultRunPr);
        }

        /// <summary>
        /// Sets a clone of the specified run properties to the DML paragraph.
        /// </summary>
        private static void SetParagraphRunPr(DmlParagraph para, DmlRunProperties runPr)
        {
            para.Properties.DefaultRunProperties = (runPr != null) ? runPr.Clone() : null;
            if ((runPr != null) && !runPr.IsEmpty)
                para.Properties.HasDefaultRunProperties = true;
        }

        /// <summary>
        /// If some of the <see cref="gExpandedRunProperties"/> properties do not exist in the specified run properties,
        /// gets property values from the parent properties/defaults and adds directly to the run properties.
        /// </summary>
        private static void ExpandProperties(DmlRunProperties runPr)
        {
            foreach (DmlRunPropertiesIds property in gExpandedRunProperties)
            {
                if (!runPr.IsPropertySpecified(property))
                    runPr.SetProperty(property, runPr.GetProperty(property));
            }
        }

        /// <summary>
        /// Sets the specified theme font to the run properties.
        /// </summary>
        private static void SetFonts(DmlRunProperties runPr, DmlFontCollectionIndex themeFontIndex)
        {
            switch (themeFontIndex)
            {
                case DmlFontCollectionIndex.Major:
                {
                    runPr.LatinFont = new DmlFont("+mj-lt");
                    runPr.EastAsianFont = new DmlFont("+mj-ea");
                    runPr.ComplexScriptFont = new DmlFont("+mj-cs");
                    break;
                }
                case DmlFontCollectionIndex.Minor:
                {
                    runPr.LatinFont = new DmlFont("+mn-lt");
                    runPr.EastAsianFont = new DmlFont("+mn-ea");
                    runPr.ComplexScriptFont = new DmlFont("+mn-cs");
                    break;
                }
                case DmlFontCollectionIndex.None:
                default:
                {
                    runPr.LatinFont = null;
                    runPr.EastAsianFont = null;
                    runPr.ComplexScriptFont = null;
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, mostly the visibility of some chart elements, so
        /// that a Normal style chart after applying the style looks the same as after applying the same style to a Normal
        /// style chart in MS Word.
        /// </summary>
        /// <remarks>
        /// A chart style definition (an instance of the <see cref="DmlChartColorStyle"/> class) contains only definitions
        /// of the text, fill, outline properties of chart elements, as well as symbol and size of data point markers.
        /// This method sets all other properties of chart elements that have different values compared to the Normal
        /// style when the corresponding style is set in MS Word.
        /// </remarks>
        private void DefineStyleDependentProperties(DmlChart chart)
        {
            switch (Style)
            {
                case ChartStyle.Normal:
                    // No changes.
                    break;
                case ChartStyle.Muted:
                    DefineMutedStyleDependentProperties(chart);
                    break;
                case ChartStyle.Saturated:
                    DefineSaturatedStyleDependentProperties(chart);
                    break;
                case ChartStyle.Shaded:
                    DefineShadedStyleDependentProperties(chart);
                    break;
                case ChartStyle.Flat:
                    DefineFlatStyleDependentProperties(chart);
                    break;
                case ChartStyle.Shadowed:
                    DefineShadowedStyleDependentProperties(chart);
                    break;
                case ChartStyle.Gradient:
                    DefineGradientStyleDependentProperties(chart);
                    break;
                case ChartStyle.Original:
                    DefineOriginalStyleDependentProperties(chart);
                    break;
                case ChartStyle.Transparent1:
                    DefineTransparent1StyleDependentProperties(chart);
                    break;
                case ChartStyle.Transparent2:
                    DefineTransparent2StyleDependentProperties(chart);
                    break;
                case ChartStyle.Outline:
                    DefineOutlineStyleDependentProperties(chart);
                    break;
                case ChartStyle.OutlineBlack:
                    DefineOutlineBlackStyleDependentProperties(chart);
                    break;
                case ChartStyle.Black:
                    DefineBlackStyleDependentProperties(chart);
                    break;
                case ChartStyle.Grey:
                    DefineGreyStyleDependentProperties(chart);
                    break;
                case ChartStyle.Blue:
                    DefineBlueStyleDependentProperties(chart);
                    break;
                case ChartStyle.ShadedPlot:
                    DefineShadedPlotStyleDependentProperties(chart);
                    break;
                default:
                    throw new InvalidOperationException("Unknown chart style.");
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.Muted"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineMutedStyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            switch (chart.ChartType)
            {
                case DmlChartType.BarChart:
                {
                    DmlBarChart barChart = (DmlBarChart)chart;
                    bool isBar = (barChart.BarDir == BarDirection.Bar);

                    if ((chart.ChartPr.Grouping == Grouping.Stacked) ||
                        (chart.ChartPr.Grouping == Grouping.PercentStacked))
                    {
                        barChart.GapWidth = 150;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        barChart.GapWidth = 100;
                        barChart.Overlap = isBar ? 0 : -24;
                    }

                    break;
                }
                case DmlChartType.LineChart:
                {
                    chart.ChartPr.SetProperty(DmlChartAttrs.IsDropLinesVisible, true);
                    break;
                }
                case DmlChartType.RadarChart:
                {
                    Legend.SetPositionInternal(LegendPosition.Bottom);
                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    if (chart.Series.Count == 0)
                        break;

                    switch (chart.Series[0].SeriesType)
                    {
                        case ChartSeriesType.Treemap:
                        case ChartSeriesType.Sunburst:
                        {
                            Legend.SetPositionInternal(LegendPosition.Bottom);
                            break;
                        }
                        case ChartSeriesType.BoxAndWhisker:
                        {
                            Legend.SetPositionInternal(LegendPosition.Bottom);

                            foreach (ChartSeries series in chart.Series)
                                series.LayoutPr.IsMeanLineVisible = true;

                            break;
                        }
                        case ChartSeriesType.Waterfall:
                        {
                            Legend.SetPositionInternal(LegendPosition.Bottom);
                            ShowDataLabels(chart, ChartDataLabelPosition.InsideEnd);
                            break;
                        }
                        case ChartSeriesType.Funnel:
                        {
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.5);
                            break;
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.Saturated"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineSaturatedStyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            // The styles look similar, but with slight differences.
            if (chart.ChartType != DmlChartType.LineChart)
                DefineMutedStyleDependentProperties(chart);
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.Shaded"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineShadedStyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            ChartSeriesType seriesType = (chart.Series.Count > 0)
                ? chart.Series[0].SeriesType
                : (ChartSeriesType)(-1);

            if ((chart.ChartType != DmlChartType.BubbleChart) &&
                (seriesType != ChartSeriesType.Histogram) &&
                (seriesType != ChartSeriesType.Pareto) &&
                (seriesType != ChartSeriesType.Funnel))
            {
                Legend.SetPositionInternal(LegendPosition.Top);
            }

            Grouping grouping = chart.ChartPr.Grouping;
            bool isStacked = (grouping == Grouping.Stacked) || (grouping == Grouping.PercentStacked);

            switch (chart.ChartType)
            {
                case DmlChartType.BarChart:
                {
                    DmlBarChart barChart = (DmlBarChart)chart;
                    bool isBar = (barChart.BarDir == BarDirection.Bar);

                    if (isStacked)
                    {
                        barChart.GapWidth = 150;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        barChart.GapWidth = isBar ? 227 : 164;
                        barChart.Overlap = isBar ? -48 : -22;
                    }

                    break;
                }
                case DmlChartType.Bar3DChart:
                {
                    DmlBarChart barChart = (DmlBarChart)chart;
                    barChart.GapWidth = isStacked ? 150 : 160;

                    if (!isStacked)
                    {
                        barChart.ChartPr.SetProperty(DmlChartAttrs.GapDepth, 0);
                        ChartFormat.View3D.RotX = 10;
                        ChartFormat.View3D.RotY = 0;
                        ChartFormat.View3D.RAngAx = false;
                    }

                    break;
                }
                case DmlChartType.DoughnutChart:
                {
                    ((DmlDoughnutChart)chart).HoleSize = 70;
                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    switch (seriesType)
                    {
                        case ChartSeriesType.BoxAndWhisker:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(1.1);
                            break;
                        case ChartSeriesType.Funnel:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.2);
                            break;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.Flat"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineFlatStyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            ChartSeriesType seriesType = (chart.Series.Count > 0)
                ? chart.Series[0].SeriesType
                : (ChartSeriesType)(-1);

            if (!chart.Is3D &&
                (chart.ChartType != DmlChartType.BubbleChart) &&
                (seriesType != ChartSeriesType.Histogram) &&
                (seriesType != ChartSeriesType.Pareto))
            {
                Legend.SetPositionInternal(LegendPosition.Top);
            }

            Grouping grouping = chart.ChartPr.Grouping;
            bool isStacked = (grouping == Grouping.Stacked) || (grouping == Grouping.PercentStacked);

            if (chart.HasAxis)
                chart.AxisY.HasMinorGridlines = true;

            switch (chart.ChartType)
            {
                case DmlChartType.BarChart:
                {
                    DmlBarChart barChart = (DmlBarChart)chart;

                    if (isStacked)
                    {
                        barChart.GapWidth = 150;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        barChart.GapWidth = (barChart.BarDir == BarDirection.Bar) ? 269 : 199;
                        barChart.Overlap = 0;
                    }

                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    switch (seriesType)
                    {
                        case ChartSeriesType.BoxAndWhisker:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(1.5);
                            break;
                        case ChartSeriesType.Waterfall:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.5);
                            break;
                        case ChartSeriesType.Funnel:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.03);
                            break;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.Shadowed"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineShadowedStyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            switch (chart.ChartType)
            {
                case DmlChartType.BarChart:
                {
                    DmlBarChart barChart = (DmlBarChart)chart;
                    bool isBar = (barChart.BarDir == BarDirection.Bar);

                    if ((chart.ChartPr.Grouping == Grouping.Stacked) ||
                        (chart.ChartPr.Grouping == Grouping.PercentStacked))
                    {
                        barChart.GapWidth = 150;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        barChart.GapWidth = isBar ? 115 : 100;
                        barChart.Overlap = isBar ? -20 : -24;
                    }

                    break;
                }
                case DmlChartType.RadarChart:
                {
                    Legend.SetPositionInternal(LegendPosition.Top);
                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    if (chart.Series.Count == 0)
                        break;

                    switch (chart.Series[0].SeriesType)
                    {
                        case ChartSeriesType.BoxAndWhisker:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(1);
                            break;
                        case ChartSeriesType.Funnel:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.5);
                            break;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.Gradient"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineGradientStyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            bool isStacked =
                (chart.ChartPr.Grouping == Grouping.Stacked) || (chart.ChartPr.Grouping == Grouping.PercentStacked);

            switch (chart.ChartType)
            {
                case DmlChartType.BarChart:
                {
                    DmlBarChart barChart = (DmlBarChart)chart;
                    bool isBar = (barChart.BarDir == BarDirection.Bar);

                    if (isStacked)
                    {
                        barChart.GapWidth = 225;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        barChart.GapWidth = isBar ? 326 : 355;
                        barChart.Overlap = isBar ? -58 : -70;
                    }

                    break;
                }
                case DmlChartType.Bar3DChart:
                {
                    DmlBar3DChart bar3DChart = (DmlBar3DChart)chart;
                    if ((bar3DChart.BarDir == BarDirection.Column) || isStacked)
                        bar3DChart.GapWidth = 225;

                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    if (chart.Series.Count == 0)
                        break;

                    switch (chart.Series[0].SeriesType)
                    {
                        case ChartSeriesType.BoxAndWhisker:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(1.75);
                            break;
                        case ChartSeriesType.Waterfall:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(1.5);
                            break;
                        case ChartSeriesType.Funnel:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(1.5);
                            break;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.Original"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineOriginalStyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            if (chart.HasAxis)
            {
                chart.AxisX.HasMajorGridlines = true;
                chart.AxisY.HasMajorGridlines = (chart.ChartType == DmlChartType.ScatterChart);
                chart.AxisY.HasMinorGridlines = false;
            }

            Grouping grouping = chart.ChartPr.Grouping;
            bool isStacked = (grouping == Grouping.Stacked) || (grouping == Grouping.PercentStacked);

            switch (chart.ChartType)
            {
                case DmlChartType.BarChart:
                case DmlChartType.Bar3DChart:
                {
                    Legend.SetPositionInternal(LegendPosition.Top);

                    DmlBarChart barChart = (DmlBarChart)chart;
                    if (chart.Is3D)
                    {
                        if (isStacked)
                            ShowDataLabels(chart);
                    }
                    else
                    {
                        ShowDataLabels(
                            chart,
                            isStacked ? ChartDataLabelPosition.Center : ChartDataLabelPosition.OutsideEnd);
                    }

                    if (!chart.Is3D || isStacked)
                    {
                        foreach (ChartSeries series in chart.Series)
                        {
                            DmlChartSpPr leaderLines = new DmlChartSpPr();
                            series.DataLabels.LabelPr.SetExtensionProperty(DmlChartDataLabelAttrs.LeaderLines, leaderLines);
                            series.DataLabels.LabelPr.SetExtensionProperty(DmlChartDataLabelAttrs.ShowLeaderLines, true);
                        }

                        chart.AxisY.SetHiddenInternal(true);
                    }

                    bool isBar = (barChart.BarDir == BarDirection.Bar);
                    if (isStacked)
                    {
                        barChart.GapWidth = 79;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        if (chart.ChartType == DmlChartType.BarChart)
                        {
                            barChart.GapWidth = isBar ? 390 : 444;
                            barChart.Overlap = isBar ? -75 : -90;
                        }
                        else
                        {
                            barChart.GapWidth = isBar ? 285 : 300;
                        }
                    }

                    break;
                }
                case DmlChartType.LineChart:
                {
                    Legend.SetPositionInternal(LegendPosition.Top);
                    break;
                }
                case DmlChartType.DoughnutChart:
                case DmlChartType.PieChart:
                case DmlChartType.Pie3DChart:
                case DmlChartType.OfPieChart:
                {
                    Legend.SetPositionInternal(LegendPosition.None);

                    if (chart.ChartType == DmlChartType.DoughnutChart)
                    {
                        ShowDataLabels(chart);
                        ((DmlDoughnutChart)chart).HoleSize = 40;
                    }
                    else
                    {
                        ShowDataLabels(chart, ChartDataLabelPosition.OutsideEnd);
                    }

                    foreach (ChartSeries series in chart.Series)
                    {
                        series.DataLabels.ShowValue = false;
                        series.DataLabels.ShowCategoryName = true;
                        series.DataLabels.ShowLeaderLines = true;
                        series.DataLabels.LabelPr.SetProperty(DmlChartDataLabelAttrs.LeaderLines, new DmlChartSpPr());
                    }

                    break;
                }
                case DmlChartType.RadarChart:
                {
                    Legend.SetPositionInternal(LegendPosition.Top);
                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    if (chart.Series.Count == 0)
                        break;

                    switch (chart.Series[0].SeriesType)
                    {
                        case ChartSeriesType.Treemap:
                        case ChartSeriesType.Sunburst:
                        {
                            Legend.SetPositionInternal(LegendPosition.None);
                            ShowDataLabels(chart);
                            break;
                        }
                        case ChartSeriesType.BoxAndWhisker:
                        {
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(1);

                            foreach (ChartSeries series in chart.Series)
                                series.LayoutPr.IsMeanLineVisible = true;

                            break;
                        }
                        case ChartSeriesType.Funnel:
                        {
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.5);
                            break;
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.Transparent1"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineTransparent1StyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            Grouping grouping = chart.ChartPr.Grouping;
            bool isStacked = (grouping == Grouping.Stacked) || (grouping == Grouping.PercentStacked);

            switch (chart.ChartType)
            {
                case DmlChartType.BarChart:
                case DmlChartType.Bar3DChart:
                {
                    DmlBarChart barChart = (DmlBarChart)chart;
                    bool isBar = (barChart.BarDir == BarDirection.Bar);
                    if (isStacked)
                    {
                        barChart.GapWidth = 80;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        if (chart.ChartType == DmlChartType.BarChart)
                        {
                            barChart.GapWidth = isBar ? 90 : 80;
                            barChart.Overlap = isBar ? 10 : 25;
                        }
                        else
                        {
                            barChart.GapWidth = 150;
                        }
                    }

                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    if (chart.Series.Count == 0)
                        break;

                    switch (chart.Series[0].SeriesType)
                    {
                        case ChartSeriesType.BoxAndWhisker:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.25);
                            break;
                        case ChartSeriesType.Waterfall:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.25);
                            break;
                        case ChartSeriesType.Funnel:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.15);
                            break;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.Transparent2"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineTransparent2StyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            Grouping grouping = chart.ChartPr.Grouping;
            bool isStacked = (grouping == Grouping.Stacked) || (grouping == Grouping.PercentStacked);

            switch (chart.ChartType)
            {
                case DmlChartType.BarChart:
                case DmlChartType.Bar3DChart:
                {
                    DmlBarChart barChart = (DmlBarChart)chart;
                    if (isStacked)
                    {
                        barChart.GapWidth = 100;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        if (chart.ChartType == DmlChartType.BarChart)
                        {
                            barChart.GapWidth = 100;
                            barChart.Overlap = 0;
                        }
                        else
                        {
                            barChart.GapWidth = 150;
                        }
                    }

                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    if (chart.Series.Count == 0)
                        break;

                    switch (chart.Series[0].SeriesType)
                    {
                        case ChartSeriesType.BoxAndWhisker:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.5);
                            break;
                        case ChartSeriesType.Waterfall:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.5);
                            break;
                        case ChartSeriesType.Funnel:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.5);
                            break;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.Outline"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineOutlineStyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            ChartSeriesType seriesType = (chart.Series.Count > 0)
                ? chart.Series[0].SeriesType
                : (ChartSeriesType)(-1);

            if ((chart.ChartType != DmlChartType.BubbleChart) &&
                (seriesType != ChartSeriesType.Histogram) &&
                (seriesType != ChartSeriesType.Pareto) &&
                (seriesType != ChartSeriesType.Funnel))
            {
                Legend.SetPositionInternal(LegendPosition.Top);
            }

            if (chart.HasAxis)
            {
                chart.AxisY.HasMajorGridlines = false;
                chart.AxisY.HasMinorGridlines = false;
            }

            Grouping grouping = chart.ChartPr.Grouping;
            bool isStacked = (grouping == Grouping.Stacked) || (grouping == Grouping.PercentStacked);

            switch (chart.ChartType)
            {
                case DmlChartType.BarChart:
                case DmlChartType.Bar3DChart:
                {
                    DmlBarChart barChart = (DmlBarChart)chart;
                    bool isBar = (barChart.BarDir == BarDirection.Bar);
                    if (isStacked)
                    {
                        barChart.GapWidth = 180;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        if (chart.ChartType == DmlChartType.BarChart)
                        {
                            barChart.GapWidth = isBar ? 227 : 164;
                            barChart.Overlap = isBar ? -48 : -35;
                        }
                        else
                        {
                            barChart.GapWidth = 180;
                        }
                    }

                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    if (chart.Series.Count == 0)
                        break;

                    switch (chart.Series[0].SeriesType)
                    {
                        case ChartSeriesType.BoxAndWhisker:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(1.25);
                            break;
                        case ChartSeriesType.Waterfall:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(1.25);
                            break;
                        case ChartSeriesType.Funnel:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.7);
                            break;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.OutlineBlack"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineOutlineBlackStyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            ChartSeriesType seriesType = (chart.Series.Count > 0)
                ? chart.Series[0].SeriesType
                : (ChartSeriesType)(-1);

            if ((chart.ChartType != DmlChartType.BubbleChart) &&
                (chart.ChartType != DmlChartType.ScatterChart) &&
                (seriesType != ChartSeriesType.Histogram) &&
                (seriesType != ChartSeriesType.Pareto) &&
                (seriesType != ChartSeriesType.Funnel))
            {
                Legend.SetPositionInternal(LegendPosition.Top);
            }

            if (chart.HasAxis)
            {
                chart.AxisX.HasMajorGridlines = !chart.Is3D;
                chart.AxisY.HasMajorGridlines = !chart.Is3D;
            }

            Grouping grouping = chart.ChartPr.Grouping;
            bool isStacked = (grouping == Grouping.Stacked) || (grouping == Grouping.PercentStacked);

            switch (chart.ChartType)
            {
                case DmlChartType.BarChart:
                case DmlChartType.Bar3DChart:
                {
                    DmlBarChart barChart = (DmlBarChart)chart;
                    bool isBar = (barChart.BarDir == BarDirection.Bar);
                    if (isStacked)
                    {
                        barChart.GapWidth = 190;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        if (chart.ChartType == DmlChartType.BarChart)
                        {
                            barChart.GapWidth = isBar ? 182 : 315;
                            barChart.Overlap = isBar ? -50 : -40;
                        }
                        else
                        {
                            barChart.GapWidth = 190;
                        }
                    }

                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    if (chart.Series.Count == 0)
                        break;

                    switch (chart.Series[0].SeriesType)
                    {
                        case ChartSeriesType.BoxAndWhisker:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(1);
                            break;
                        case ChartSeriesType.Waterfall:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(1);
                            break;
                        case ChartSeriesType.Funnel:
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.6);
                            break;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.Black"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineBlackStyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            switch (chart.ChartType)
            {
                case DmlChartType.BarChart:
                {
                    DmlBarChart barChart = (DmlBarChart)chart;

                    if ((chart.ChartPr.Grouping == Grouping.Stacked) ||
                        (chart.ChartPr.Grouping == Grouping.PercentStacked))
                    {
                        barChart.GapWidth = 150;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        if (barChart.BarDir == BarDirection.Bar)
                        {
                            ((DmlBarChart)chart).GapWidth = 115;
                            ((DmlBarChart)chart).Overlap = -20;
                        }
                        else
                        {
                            ((DmlBarChart)chart).GapWidth = 100;
                            ((DmlBarChart)chart).Overlap = -24;
                        }
                    }

                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    if (chart.Series.Count == 0)
                        break;

                    switch (chart.Series[0].SeriesType)
                    {
                        case ChartSeriesType.Treemap:
                        case ChartSeriesType.Sunburst:
                        {
                            Legend.SetPositionInternal(LegendPosition.Bottom);
                            break;
                        }
                        case ChartSeriesType.BoxAndWhisker:
                        {
                            Legend.SetPositionInternal(LegendPosition.Bottom);
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(1.5);

                            foreach (ChartSeries series in chart.Series)
                                series.LayoutPr.IsMeanLineVisible = true;

                            break;
                        }
                        case ChartSeriesType.Waterfall:
                        {
                            ShowDataLabels(chart, ChartDataLabelPosition.InsideEnd);
                            break;
                        }
                        case ChartSeriesType.Funnel:
                        {
                            // Show legend at top if it is not shown.
                            if (ChartFormat.Legend == null)
                                Legend.SetPositionInternal(LegendPosition.Top);

                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.5);
                            break;
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.Grey"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineGreyStyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            if (chart.HasAxis)
            {
                chart.AxisX.HasMajorGridlines = false; //(chart.ChartType == DmlChartType.ScatterChart)
                chart.AxisY.HasMajorGridlines = (chart.ChartType != DmlChartType.BubbleChart);
                chart.AxisY.HasMinorGridlines = false;
            }

            Grouping grouping = chart.ChartPr.Grouping;
            bool isStacked = (grouping == Grouping.Stacked) || (grouping == Grouping.PercentStacked);

            switch (chart.ChartType)
            {
                case DmlChartType.BarChart:
                case DmlChartType.Bar3DChart:
                {
                    DmlBarChart barChart = (DmlBarChart)chart;
                    if (!chart.Is3D)
                    {
                        ShowDataLabels(
                            chart,
                            isStacked ? ChartDataLabelPosition.Center : ChartDataLabelPosition.InsideEnd);
                    }

                    if (!chart.Is3D || isStacked)
                    {
                        foreach (ChartSeries series in chart.Series)
                        {
                            DmlChartSpPr leaderLines = new DmlChartSpPr();
                            series.DataLabels.LabelPr.SetExtensionProperty(DmlChartDataLabelAttrs.LeaderLines, leaderLines);
                            series.DataLabels.LabelPr.SetExtensionProperty(DmlChartDataLabelAttrs.ShowLeaderLines, true);
                        }

                        if ((barChart.BarDir == BarDirection.Column) || isStacked)
                            chart.AxisY.SetHiddenInternal(true);
                    }
                    else
                    {
                        ChartFormat.View3D.RotX = 0;
                        ChartFormat.View3D.RotY = 0;
                        ChartFormat.View3D.RAngAx = false;
                        ChartFormat.View3D.DepthPercent = 60;
                        ChartFormat.View3D.Perspective = 100;
                    }

                    if (isStacked)
                    {
                        barChart.GapWidth = 150;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        barChart.GapWidth = 65;
                        if (chart.ChartType == DmlChartType.BarChart)
                            barChart.Overlap = 0;
                    }

                    break;
                }
                case DmlChartType.BubbleChart:
                {
                    ShowDataLabels(chart, ChartDataLabelPosition.Center);
                    foreach (ChartSeries series in chart.Series)
                        series.DataLabels.ShowBubbleSize = false;

                    break;
                }
                case DmlChartType.LineChart:
                {
                    ShowDataLabels(chart, ChartDataLabelPosition.Center);
                    chart.AxisY.SetHiddenInternal(true);
                    break;
                }
                case DmlChartType.DoughnutChart:
                case DmlChartType.PieChart:
                case DmlChartType.Pie3DChart:
                case DmlChartType.OfPieChart:
                {
                    if (chart.ChartType == DmlChartType.DoughnutChart)
                    {
                        ShowDataLabels(chart);
                        ((DmlDoughnutChart)chart).HoleSize = 50;
                    }
                    else
                    {
                        ShowDataLabels(chart, ChartDataLabelPosition.Center);
                    }

                    foreach (ChartSeries series in chart.Series)
                    {
                        series.DataLabels.ShowValue = false;
                        series.DataLabels.ShowPercentage = true;
                        series.DataLabels.ShowLeaderLines = true;
                        series.DataLabels.LabelPr.SetProperty(DmlChartDataLabelAttrs.LeaderLines, new DmlChartSpPr());
                    }

                    Legend.SetPositionInternal(LegendPosition.Right);

                    if (chart.Is3D)
                    {
                        ChartFormat.View3D.RotX = 50;
                        ChartFormat.View3D.DepthPercent = 100;
                    }

                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    if (chart.Series.Count == 0)
                        break;

                    switch (chart.Series[0].SeriesType)
                    {
                        case ChartSeriesType.Treemap:
                        case ChartSeriesType.Sunburst:
                        {
                            ShowDataLabels(chart);
                            Legend.SetPositionInternal(LegendPosition.Right);
                            break;
                        }
                        case ChartSeriesType.Histogram:
                        case ChartSeriesType.Pareto:
                        case ChartSeriesType.ParetoLine:
                        {
                            foreach (ChartSeries series in chart.Series)
                            {
                                if (series.HasDataLabels || (series.SeriesType == ChartSeriesType.ParetoLine))
                                    continue;

                                series.SetHasDataLabels(true);
                                ChartDataLabelCollection dataLabels = series.DataLabels;
                                dataLabels.ShowValue = true;
                                dataLabels.Position = ChartDataLabelPosition.InsideEnd;
                            }

                            chart.AxisY.SetHiddenInternal(true);

                            break;
                        }
                        case ChartSeriesType.BoxAndWhisker:
                        {
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(1.5);
                            Legend.SetPositionInternal(LegendPosition.Bottom);
                            break;
                        }
                        case ChartSeriesType.Funnel:
                        {
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.15);
                            break;
                        }
                        case ChartSeriesType.Waterfall:
                        {
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.25);
                            chart.AxisY.SetHiddenInternal(true);
                            break;
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.Blue"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineBlueStyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            DmlChartType chartType = chart.ChartType;
            Grouping grouping = chart.ChartPr.Grouping;
            bool isStacked = (grouping == Grouping.Stacked) || (grouping == Grouping.PercentStacked);

            switch (chartType)
            {
                case DmlChartType.Area3DChart:
                {
                    if (!isStacked)
                        Legend.SetPositionInternal(LegendPosition.None);

                    break;
                }
                case DmlChartType.BarChart:
                case DmlChartType.Bar3DChart:
                {
                    DmlBarChart barChart = (DmlBarChart)chart;
                    if (chart.Is3D && (grouping == Grouping.Standard))
                        Legend.SetPositionInternal(LegendPosition.None);

                    if (isStacked)
                    {
                        barChart.GapWidth = 150;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        barChart.GapWidth = 70;
                        if (chartType == DmlChartType.BarChart)
                            barChart.Overlap = -20;
                    }

                    break;
                }
                case DmlChartType.BubbleChart:
                {
                    ShowDataLabels(chart, ChartDataLabelPosition.Center);
                    foreach (ChartSeries series in chart.Series)
                        series.DataLabels.ShowBubbleSize = false;

                    break;
                }
                case DmlChartType.Line3DChart:
                {
                    Legend.SetPositionInternal(LegendPosition.None);
                    break;
                }
                case DmlChartType.DoughnutChart:
                case DmlChartType.PieChart:
                case DmlChartType.Pie3DChart:
                case DmlChartType.OfPieChart:
                {
                    if (chart.ChartType == DmlChartType.DoughnutChart)
                    {
                        ShowDataLabels(chart);
                        ((DmlDoughnutChart)chart).HoleSize = 50;
                    }
                    else
                    {
                        ShowDataLabels(chart, ChartDataLabelPosition.InsideEnd);
                    }

                    foreach (ChartSeries series in chart.Series)
                    {
                        series.DataLabels.ShowValue = false;
                        series.DataLabels.ShowPercentage = true;
                        series.DataLabels.ShowCategoryName = true;
                        series.DataLabels.ShowLeaderLines = true;
                        series.DataLabels.Separator = "\r";
                        series.DataLabels.LabelPr.SetProperty(DmlChartDataLabelAttrs.LeaderLines, new DmlChartSpPr());
                    }

                    Legend.SetPositionInternal(LegendPosition.None);
                    break;
                }
                case DmlChartType.StockChart:
                {
                    if (chart.HasAxis)
                    {
                        chart.AxisY.HasMajorGridlines = false;
                        chart.AxisY.HasMinorGridlines = false;
                    }

                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    if (chart.Series.Count == 0)
                        break;

                    switch (chart.Series[0].SeriesType)
                    {
                        case ChartSeriesType.Treemap:
                        case ChartSeriesType.Sunburst:
                        {
                            Legend.SetPositionInternal(LegendPosition.None);
                            break;
                        }
                        case ChartSeriesType.Funnel:
                        {
                            Legend.SetPositionInternal(LegendPosition.Top);
                            chart.AxisX.Scaling.GapWidth = DoubleOrAutomatic.FromDouble(0.5);

                            foreach (ChartSeries series in chart.Series)
                                series.SetHasDataLabels(false);

                            break;
                        }
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Sets the required properties of the chart and its elements, so that a Normal style chart after applying the
        /// <see cref="ChartStyle.ShadedPlot"/> style looks the same as the corresponding style chart in MS Word.
        /// </summary>
        private void DefineShadedPlotStyleDependentProperties(DmlChart chart)
        {
            if (chart.Is3D)
                ChartFormat.View3D.DepthPercent = 100;

            DmlChartType chartType = chart.ChartType;
            Grouping grouping = chart.ChartPr.Grouping;
            bool isStacked = (grouping == Grouping.Stacked) || (grouping == Grouping.PercentStacked);

            switch (chartType)
            {
                case DmlChartType.Area3DChart:
                {
                    if (!isStacked && chart.HasAxis)
                    {
                        chart.AxisZ.HasMajorGridlines = true;
                        chart.AxisZ.HasMinorGridlines = false;
                    }

                    break;
                }
                case DmlChartType.BarChart:
                case DmlChartType.Bar3DChart:
                {
                    if (chart.HasAxis)
                    {
                        chart.AxisX.HasMajorGridlines = true;
                        chart.AxisX.HasMinorGridlines = false;
                    }

                    DmlBarChart barChart = (DmlBarChart)chart;
                    if (chart.Is3D && (grouping == Grouping.Standard))
                        Legend.SetPositionInternal(LegendPosition.None);

                    bool isBar = (barChart.BarDir == BarDirection.Bar);
                    if (isStacked)
                    {
                        barChart.GapWidth = 150;
                        barChart.Overlap = 100;
                    }
                    else
                    {
                        if (chart.ChartType == DmlChartType.BarChart)
                        {
                            barChart.GapWidth = isBar ? 247 : 267;
                            barChart.Overlap = isBar ? 0 : -43;
                        }
                        else
                        {
                            barChart.GapWidth = 150;
                        }
                    }

                    break;
                }
                case DmlChartType.BubbleChart:
                {
                    ShowDataLabels(chart, ChartDataLabelPosition.Center);
                    foreach (ChartSeries series in chart.Series)
                        series.DataLabels.ShowBubbleSize = false;

                    break;
                }
                case DmlChartType.DoughnutChart:
                case DmlChartType.PieChart:
                case DmlChartType.Pie3DChart:
                case DmlChartType.OfPieChart:
                {
                    if (chartType == DmlChartType.DoughnutChart)
                        ((DmlDoughnutChart)chart).HoleSize = 70;

                    Legend.SetPositionInternal(LegendPosition.Right);
                    break;
                }
                case DmlChartType.ChartExChart:
                {
                    if (chart.Series.Count == 0)
                        break;

                    switch (chart.Series[0].SeriesType)
                    {
                        case ChartSeriesType.Treemap:
                        case ChartSeriesType.Sunburst:
                            Legend.SetPositionInternal(LegendPosition.Right);
                            break;
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Changes the position of data labels if the color applied to them when the style is applied blends with
        /// the background at their current position.
        /// </summary>
        private void FixDataLabelPosition(ChartDataLabelCollection dataLabels)
        {
            ChartSeriesType seriesType = dataLabels.Series.SeriesType;
            ChartDataLabelPosition position = dataLabels.Position;

            switch (Style)
            {
                case ChartStyle.Blue:
                {
                    if ((seriesType == ChartSeriesType.PieOfBar) &&
                        ((position == ChartDataLabelPosition.OutsideEnd) ||
                         (position == ChartDataLabelPosition.BestFit)))
                    {
                        dataLabels.Position = ChartDataLabelPosition.InsideEnd;
                    }

                    break;
                }
                case ChartStyle.Grey:
                {
                    switch (seriesType)
                    {
                        case ChartSeriesType.Bar:
                        case ChartSeriesType.Column:
                        {
                            if (position == ChartDataLabelPosition.OutsideEnd)
                                dataLabels.Position = ChartDataLabelPosition.InsideEnd;

                            break;
                        }
                        case ChartSeriesType.Bubble:
                        case ChartSeriesType.Bubble3D:
                        case ChartSeriesType.Line:
                        case ChartSeriesType.LineStacked:
                        case ChartSeriesType.LinePercentStacked:
                        {
                            dataLabels.Position = ChartDataLabelPosition.Center;
                            break;
                        }
                    }

                    break;
                }
                case ChartStyle.Original:
                {
                    if (dataLabels.Series.IsPieChartSeries && (seriesType != ChartSeriesType.Doughnut))
                        dataLabels.Position = ChartDataLabelPosition.OutsideEnd;

                    break;
                }
            }
        }

        /// <summary>
        /// Displays  the data labels of all series of the specified chart.
        /// </summary>
        private static void ShowDataLabels(DmlChart chart)
        {
            foreach (ChartSeries series in chart.Series)
            {
                if (series.HasDataLabels)
                    continue;

                series.SetHasDataLabels(true);
                ChartDataLabelCollection dataLabels = series.DataLabels;
                dataLabels.ShowValue = true;
                dataLabels.ShowLegendKey = false;
                dataLabels.ShowCategoryName = false;
                dataLabels.ShowSeriesName = false;
                dataLabels.ShowPercentage = false;
                dataLabels.ShowLeaderLines = false;
            }

            chart.DataLabels.ShowValue = true;
        }

        /// <summary>
        /// Displays the data labels of all series of the chart at the specified position.
        /// </summary>
        private static void ShowDataLabels(DmlChart chart, ChartDataLabelPosition position)
        {
            ShowDataLabels(chart);

            foreach (ChartSeries series in chart.Series)
                series.DataLabels.Position = position;
        }

        /// <summary>
        /// If the specified series is a series of a line or radar chart, displays the markers for the appropriate
        /// chart styles.
        /// </summary>
        private void MarkMarkersForShowingIfRequired(ChartSeries series)
        {
            DmlChartType chartType = series.Chart.ChartType;
            if ((chartType != DmlChartType.LineChart) && (chartType != DmlChartType.RadarChart))
                return;

            switch (Style)
            {
                case ChartStyle.Original:
                case ChartStyle.Grey:
                case ChartStyle.Blue:
                {
                    if ((chartType == DmlChartType.LineChart) ||
                        ((chartType == DmlChartType.RadarChart) && (Style == ChartStyle.Blue)))
                    {
                        series.DefaultDataPoint.Marker.SetSymbolInternal(MarkerSymbol.Default);
                    }

                    break;
                }
            }
        }

        /// <summary>
        /// Gets the document theme, initializing it if it does not exist.
        /// </summary>
        private IThemeProvider Theme
        {
            get
            {
                if (mChartSpace.ThemeOverride != null)
                    return mChartSpace.ThemeOverride;

                // This initializes a theme if not defined yet.
                return ((Document)mChartSpace.Dml.Document).Theme;
            }
        }

        /// <summary>
        /// Gets the chart legend.
        /// </summary>
        private ChartLegend Legend
        {
            get
            {
                if (ChartFormat.Legend == null)
                    ChartFormat.Legend = new ChartLegend(ChartFormat);

                return ChartFormat.Legend;
            }
        }

        /// <summary>
        /// Gets the chart format.
        /// </summary>
        private DmlChartFormat ChartFormat
        {
            get { return mChartSpace.ChartFormat; }
        }

        /// <summary>
        /// Gets the applying chart style.
        /// </summary>
        private ChartStyle Style
        {
            get { return ChartStyleResolver.GetChartStyle(mChartSpace.DmlChartStyle.IntId); }
        }

        private readonly DmlChartSpace mChartSpace;

        private static readonly DmlRunPropertiesIds[] gExpandedRunProperties = new DmlRunPropertiesIds[]
        {
            DmlRunPropertiesIds.FontSize, DmlRunPropertiesIds.Bold, DmlRunPropertiesIds.Italics,
            DmlRunPropertiesIds.Underline, DmlRunPropertiesIds.Strikethrough, DmlRunPropertiesIds.Kerning,
            DmlRunPropertiesIds.Baseline
        };

        private static readonly MarkerSymbol[] gMarkerAutoSymbols = new MarkerSymbol[]
        {
            MarkerSymbol.Diamond, MarkerSymbol.Square, MarkerSymbol.Triangle, MarkerSymbol.X, MarkerSymbol.Star,
            MarkerSymbol.Circle, MarkerSymbol.Plus, MarkerSymbol.Dot, MarkerSymbol.Dash
        };
    }
}
