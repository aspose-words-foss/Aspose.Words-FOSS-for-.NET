// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/02/2015 by Andrey Noskov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Settings;
using Aspose.Words.Themes;

namespace Aspose.Words.RW.Docx.Reader
{
    internal class ChartInserter : IChartInserter
    {
        /// <summary>
        /// Chart insertion with options.
        /// </summary>
        public Shape InsertChart(
            ChartType chartType,
            ChartStyle chartStyle,
            RelativeHorizontalPosition horzPos,
            double left,
            RelativeVerticalPosition vertPos,
            double top,
            double width,
            double height,
            WrapType wrapType,
            DocumentBuilder documentBuilder)
        {
            Document document = documentBuilder.Document;
            Shape shape = new Shape(document, ShapeMarkupLanguage.Dml);

            DmlChartSpace chartSpace = CreateChartSpace(chartType, document);
            shape.DmlNode = chartSpace;
            shape.RunPr = documentBuilder.GetRunPrCopy();
            shape.RelativeHorizontalPosition = horzPos;
            shape.Left = left;
            shape.RelativeVerticalPosition = vertPos;
            shape.Top = top;
            shape.WrapType = wrapType;

            CompatibilityOptions compatibility = document.CompatibilityOptions;
            if (IsChartExChart(chartType) ||
                (chartStyle != ChartStyle.Normal) ||
                (compatibility.MswVersion >= MsWordVersionCore.Word2013))
            {
                chartSpace.ColorStyle = GenerateChartColorStyle();
                chartSpace.SetChartStyle(chartStyle, chartType, (chartStyle != ChartStyle.Normal));

                // WORDSNET-27030 It is needed to optimize the document to version 2013, otherwise MS Word will not allow
                // to insert Word 2016 charts into the document, even though such a chart is already present in it.
                if (!compatibility.IsOptimized && (compatibility.MswVersion < MsWordVersionCore.Word2013))
                {
                    compatibility.OptimizeFor(MsWordVersion.Word2013);

                    document.Warn(
                        WarningType.MinorFormattingLoss,
                        WarningSource.Unknown,
                        WarningStrings.OptimizedForWord2013);
                }
            }

            // WORDSNET-15923 If width or height is negative or zero chart has to be scaled to 100% of its container.
            // Preserve width/height ratio.
            if (MathUtil.IsLessOrEqual(width,0) || MathUtil.IsLessOrEqual(height,0))
            {
                SizeD containerSize = ShapeSizeValidationHelper.GetContainerSize(shape, documentBuilder.CurrentParagraph);
                SizeD scaledSize = ShapeSizeValidationHelper.ScaleToFitContainer(new SizeD(0, 0), containerSize, 0);

                width = scaledSize.Width;
                height = scaledSize.Height;
            }

            shape.SetWidthSafe(width);
            shape.SetHeightSafe(height);

            // We presume we are inserting a top level shape, it has to be inserted into the tree
            // for the shape size validation below to work properly.
            documentBuilder.InsertNode(shape);

            return shape;
        }

        /// <summary>
        /// Creates a chart space for the specified chart type.
        /// </summary>
        public DmlChartSpace CreateChartSpace(ChartType chartType, Document document)
        {
            bool isChartEx = IsChartExChart(chartType);
            if (isChartEx)
            {
                if (document.ComplianceInfo == null)
                    document.ComplianceInfo = new OoxmlComplianceInfo();

                document.ComplianceInfo.MarkAsIsoTransitional();
            }

            DmlChartSpace chartSpace = new DmlChartSpace(isChartEx);
            ReadPresetChart(chartSpace, chartType, document.BuiltInDocumentProperties.Version);

            DmlChart chart = chartSpace.FirstChart;
            SetChartTypedProperties(chartType, chart);

            return chartSpace;
        }

        /// <summary>
        /// Reads chart of specific type from the Aspose.Words.Resources.Charts.ChartTypes.xml
        /// </summary>
        public void ReadPresetChart(DmlChartSpace chartSpace, ChartType chartType, int documentVersion)
        {
            PresetChartReader.ReadPresetChart(chartSpace, chartType, documentVersion);
        }

        /// <summary>
        /// Generates a <see cref="DmlChartColorStyle"/> instance that stores data of a colorsX.xml part of a chart.
        /// </summary>
        private static DmlChartColorStyle GenerateChartColorStyle()
        {
            DmlChartColorStyle colorStyle = new DmlChartColorStyle();
            colorStyle.Id = "10";
            colorStyle.Method = "cycle";

            colorStyle.Colors = new DmlColor[]
            {
                new DmlSchemeColor(ThemeColor.Accent1),
                new DmlSchemeColor(ThemeColor.Accent2),
                new DmlSchemeColor(ThemeColor.Accent3),
                new DmlSchemeColor(ThemeColor.Accent4),
                new DmlSchemeColor(ThemeColor.Accent5),
                new DmlSchemeColor(ThemeColor.Accent6)
            };

            colorStyle.Variations = new DmlChartColorStyleVariation[]
            {
                new DmlChartColorStyleVariation(),
                new DmlChartColorStyleVariation(new DmlLuminanceModulation(0.6)),
                new DmlChartColorStyleVariation(new DmlLuminanceModulation(0.8), new DmlLuminanceOffset(0.2)),
                new DmlChartColorStyleVariation(new DmlLuminanceModulation(0.8)),
                new DmlChartColorStyleVariation(new DmlLuminanceModulation(0.6), new DmlLuminanceOffset(0.4)),
                new DmlChartColorStyleVariation(new DmlLuminanceModulation(0.5)),
                new DmlChartColorStyleVariation(new DmlLuminanceModulation(0.7), new DmlLuminanceOffset(0.3)),
                new DmlChartColorStyleVariation(new DmlLuminanceModulation(0.7)),
                new DmlChartColorStyleVariation(new DmlLuminanceModulation(0.5), new DmlLuminanceOffset(0.5))
            };

            return colorStyle;
        }

        /// <summary>
        /// Sets chart typed properties.
        /// </summary>
        private static void SetChartTypedProperties(ChartType chartType, DmlChart chart)
        {
            DmlChartPr chartPr = chart.ChartPr;
            switch (chartType)
            {
                case ChartType.Area:
                    break;
                case ChartType.AreaStacked:
                    chartPr.Grouping = Grouping.Stacked;
                    break;
                case ChartType.AreaPercentStacked:
                    chartPr.Grouping = Grouping.PercentStacked;
                    break;
                case ChartType.Area3D:
                    break;
                case ChartType.Area3DStacked:
                    chartPr.Grouping = Grouping.Stacked;
                    break;
                case ChartType.Area3DPercentStacked:
                    chartPr.Grouping = Grouping.PercentStacked;
                    break;
                case ChartType.Bar:
                    break;
                case ChartType.BarStacked:
                    chartPr.Grouping = Grouping.Stacked;
                    chartPr.SetProperty(DmlChartAttrs.Overlap, 100);
                    break;
                case ChartType.BarPercentStacked:
                    chartPr.Grouping = Grouping.PercentStacked;
                    chartPr.SetProperty(DmlChartAttrs.Overlap, 100);
                    break;
                case ChartType.Bar3D:
                    break;
                case ChartType.Bar3DStacked:
                    chartPr.Grouping = Grouping.Stacked;
                    break;
                case ChartType.Bar3DPercentStacked:
                    chartPr.Grouping = Grouping.PercentStacked;
                    break;
                case ChartType.Bubble:
                    break;
                case ChartType.Bubble3D:
                    ((DmlBubbleChart)chart).Bubble3D = true;
                    foreach (ChartSeries series in chart.Series)
                        series.Bubble3D = true;
                    break;
                case ChartType.Column:
                    break;
                case ChartType.ColumnStacked:
                    chartPr.Grouping = Grouping.Stacked;
                    chartPr.SetProperty(DmlChartAttrs.Overlap, 100);
                    break;
                case ChartType.ColumnPercentStacked:
                    chartPr.Grouping = Grouping.PercentStacked;
                    chartPr.SetProperty(DmlChartAttrs.Overlap, 100);
                    break;
                case ChartType.Column3D:
                    break;
                case ChartType.Column3DClustered:
                    break;
                case ChartType.Column3DStacked:
                    chartPr.Grouping = Grouping.Stacked;
                    break;
                case ChartType.Column3DPercentStacked:
                    chartPr.Grouping = Grouping.PercentStacked;
                    break;
                case ChartType.Doughnut:
                    break;
                case ChartType.Line:
                    break;
                case ChartType.LineStacked:
                    chartPr.Grouping = Grouping.Stacked;
                    break;
                case ChartType.LinePercentStacked:
                    chartPr.Grouping = Grouping.PercentStacked;
                    break;
                case ChartType.Line3D:
                    break;
                case ChartType.Pie:
                    break;
                case ChartType.PieOfBar:
                    chartPr.OfPieType = OfPieType.Bar;
                    break;
                case ChartType.PieOfPie:
                    chartPr.OfPieType = OfPieType.Pie;
                    break;
                case ChartType.Pie3D:
                    break;
                case ChartType.Radar:
                    break;
                case ChartType.Scatter:
                    break;
                case ChartType.Stock:
                    break;
                case ChartType.Surface:
                    break;
                case ChartType.Surface3D:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the specified chart type relates to a Word 2016 chart.
        /// </summary>
        private static bool IsChartExChart(ChartType chartType)
        {
            switch (chartType)
            {
                case ChartType.Treemap:
                case ChartType.Sunburst:
                case ChartType.Histogram:
                case ChartType.Pareto:
                case ChartType.BoxAndWhisker:
                case ChartType.Waterfall:
                case ChartType.Funnel:
                    return true;
                default:
                    return false;
            }
        }
    }
}
