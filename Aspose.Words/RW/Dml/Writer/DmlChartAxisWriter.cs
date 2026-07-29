// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/22/2014 by Alexey Noskov

using System;
using Aspose.Common;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Class used to write DrawingML chart axis.
    /// </summary>
    internal static class DmlChartAxisWriter
    {
        internal static void Write(ChartAxis axis, DocxDocumentWriterBase writer)
        {
            switch (axis.Type)
            {
                case ChartAxisType.Category:
                {
                    if (axis.IsDateCategoryAxis)
                        WriteDateAxis(axis, writer);
                    else
                        WriteCategoryAxis(axis, writer);
                    break;
                }
                case ChartAxisType.Series:
                    WriteSeriesAxis(axis, writer);
                    break;
                case ChartAxisType.Value:
                    WriteValueAxis(axis, writer);
                    break;
                default:
                    throw new ArgumentException("Unexpected axis type.");
            }
        }

        internal static void WriteChartExAxis(ChartAxis axis, DocxDocumentWriterBase writer)
        {
            DmlChartExChart chartEx = (DmlChartExChart)axis.Chart;

            if (chartEx.IsStubAxis(axis.AxId))
                return;

            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(GetTagName("axis", true));

            builder.WriteAttribute("id", axis.AxId);
            if (axis.Hidden)
                builder.WriteAttribute("hidden", axis.Hidden);

            if (axis.IsCategory)
                WriteCategoryScaling(axis, builder);
            else
                WriteValueScaling(axis, builder);

            DmlChartCommonWriter.WriteDisplayUnits(axis.DisplayUnit, writer, true);

            WriteAxisCommon(axis, writer, true);

            DmlExtensionListWriter.Write(GetTagPrefix(true), ((IDmlExtensionListSource)axis).Extensions, writer);

            builder.EndElement(GetTagName("axis", true));
        }

        /// <summary>
        /// Writes category axis scaling properties (the catScaling element of the 2.24.3.8 CT_CategoryAxisScaling
        /// complex type [MS-ODRAWXML]).
        /// </summary>
        private static void WriteCategoryScaling(ChartAxis axis, DocxBuilder builder)
        {
            builder.StartElement(GetTagName("catScaling", true));

            DmlChartCommonWriter.WriteDoubleOrAutomaticAttribute("gapWidth", axis.Scaling.GapWidth, builder);

            builder.EndElement(GetTagName("catScaling", true));
        }

        /// <summary>
        /// Writes category axis scaling properties (the valScaling element of the 2.24.3.82 CT_ValueAxisScaling
        /// complex type [MS-ODRAWXML]).
        /// </summary>
        private static void WriteValueScaling(ChartAxis axis, DocxBuilder builder)
        {
            builder.StartElement(GetTagName("valScaling", true));

            if (axis.Scaling.MaximumIsDefined)
                DmlChartCommonWriter.WriteAxisBound("max", axis.Scaling.Maximum, builder);
            if (axis.Scaling.MinimumIsDefined)
                DmlChartCommonWriter.WriteAxisBound("min", axis.Scaling.Minimum, builder);
            DmlChartCommonWriter.WriteDoubleOrAutomaticAttribute("majorUnit", axis.MajorUnitPr, builder);
            DmlChartCommonWriter.WriteDoubleOrAutomaticAttribute("minorUnit", axis.MinorUnitPr, builder);

            builder.EndElement(GetTagName("valScaling", true));
        }

        private static void WriteCategoryAxis(ChartAxis axis, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(GetTagName("catAx", false));

            WriteAxisCommon(axis, writer, false);

            builder.WriteElementWithAttributes(GetTagName("auto", false), "val",
                axis.CategoryType == AxisCategoryType.Automatic);
            if (axis.LblAlgn != LabelAlignment.Default)
                builder.WriteElementWithAttributes(GetTagName("lblAlgn", false), "val", 
                    DmlChartsEnum.LabelAlignmentToDml(axis.LblAlgn));
            builder.WriteElementWithAttributes(GetTagName("lblOffset", false), "val", axis.TickLabels.Offset);
            if (!axis.TickLabels.IsAutoSpacing)
                builder.WriteElementWithAttributes(GetTagName("tickLblSkip", false), "val", axis.TickLabels.Spacing);
            if (!axis.TickMarkSpacingIsAuto)
                builder.WriteElementWithAttributes(GetTagName("tickMarkSkip", false), "val", axis.TickMarkSpacing);
            builder.WriteElementWithAttributes(GetTagName("noMultiLvlLbl", false), "val", axis.NoMultiLvlLbl);

            DmlExtensionListWriter.Write(GetTagPrefix(false), ((IDmlExtensionListSource)axis).Extensions, writer);

            builder.EndElement(GetTagName("catAx", false));
        }

        private static void WriteDateAxis(ChartAxis axis, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(GetTagName("dateAx", false));

            WriteAxisCommon(axis, writer, false);

            DmlChartCommonWriter.WriteWrappedBooleanIfTrue(GetTagName("auto", false), 
                axis.CategoryType == AxisCategoryType.Automatic, builder);
            builder.WriteElementWithAttributes(GetTagName("lblOffset", false), "val", axis.TickLabels.Offset);
            builder.WriteElementWithAttributes(GetTagName("baseTimeUnit", false), "val", 
                DmlChartsEnum.TimeUnitToDml(axis.BaseTimeUnit));
            if (!axis.MajorUnitIsAuto)
                builder.WriteElementWithAttributes(GetTagName("majorUnit", false), "val", 
                    FormatterPal.DoubleToStrRoundtrip(axis.MajorUnit));
            builder.WriteElementWithAttributes(GetTagName("majorTimeUnit", false), "val", 
                DmlChartsEnum.TimeUnitToDml(axis.MajorUnitScale));
            if (!axis.MinorUnitIsAuto)
                builder.WriteElementWithAttributes(GetTagName("minorUnit", false), "val", 
                    FormatterPal.DoubleToStrRoundtrip(axis.MinorUnit));
            builder.WriteElementWithAttributes(GetTagName("minorTimeUnit", false), "val", 
                DmlChartsEnum.TimeUnitToDml(axis.MinorUnitScale));

            DmlExtensionListWriter.Write(GetTagPrefix(false), ((IDmlExtensionListSource)axis).Extensions, writer);

            builder.EndElement(GetTagName("dateAx", false));
        }

        private static void WriteSeriesAxis(ChartAxis axis, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(GetTagName("serAx", false));

            WriteAxisCommon(axis, writer, false);

            if (!axis.TickLabels.IsAutoSpacing)
                builder.WriteElementWithAttributes(GetTagName("tickLblSkip", false), "val", axis.TickLabels.Spacing);
            if (!axis.TickMarkSpacingIsAuto)
                builder.WriteElementWithAttributes(GetTagName("tickMarkSkip", false), "val", axis.TickMarkSpacing);

            DmlExtensionListWriter.Write(GetTagPrefix(false), ((IDmlExtensionListSource)axis).Extensions, writer);

            builder.EndElement(GetTagName("serAx", false));
        }

        private static void WriteValueAxis(ChartAxis axis, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(GetTagName("valAx", false));

            WriteAxisCommon(axis, writer, false);

            builder.WriteElementWithAttributes(GetTagName("crossBetween", false), "val", 
                DmlChartsEnum.CrossBetweenToDml(axis.CrossBetween));
           
            if (!axis.MajorUnitIsAuto)
                builder.WriteElementWithAttributes(GetTagName("majorUnit", false), "val", 
                    FormatterPal.DoubleToStrRoundtrip(axis.MajorUnit));

            if (!axis.MinorUnitIsAuto)
                builder.WriteElementWithAttributes(GetTagName("minorUnit", false), "val", 
                    FormatterPal.DoubleToStrRoundtrip(axis.MinorUnit));

            DmlChartCommonWriter.WriteDisplayUnits(axis.DisplayUnit, writer, false);

            DmlExtensionListWriter.Write(GetTagPrefix(false), ((IDmlExtensionListSource)axis).Extensions, writer);

            builder.EndElement(GetTagName("valAx", false));
        }

        private static void WriteAxisCommon(ChartAxis axis, DocxDocumentWriterBase writer, bool isChartEx)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            if (!isChartEx)
            {
                builder.WriteElementWithAttributes(GetTagName("axId", isChartEx), "val", axis.AxId);
                DmlChartCommonWriter.WriteScaling(axis.Scaling, writer, isChartEx);
                builder.WriteElementWithAttributes(GetTagName("delete", isChartEx), "val", axis.Hidden);
                builder.WriteElementWithAttributes(GetTagName("axPos", isChartEx), "val", DmlChartsEnum.AxisPositionToDml(axis.AxPos));
            }
            DmlChartCommonWriter.WriteGridlines("majorGridlines", axis.MajorGridlines, writer, isChartEx);
            DmlChartCommonWriter.WriteGridlines("minorGridlines", axis.MinorGridlines, writer, isChartEx);
            DmlChartCommonWriter.WriteTitle(axis.TitleInternal, writer, ((IDmlChartTitleHolder)axis).TitleDeleted, isChartEx, true);
            DmlChartCommonWriter.WriteNumberFormat(axis.NumFmt, builder, isChartEx);

            DmlChartCommonWriter.WriteTickMarks(isChartEx ? "majorTickMarks" : "majorTickMark", 
                axis.MajorTickMark, axis.MajorTickMarkExtensions, writer, isChartEx);
            DmlChartCommonWriter.WriteTickMarks(isChartEx ? "minorTickMarks" : "minorTickMark", 
                axis.MinorTickMark, axis.MinorTickMarkExtensions, writer, isChartEx);

            if (isChartEx)
            {
                if (axis.AreTickLabelsVisible || 
                    (axis.TickLabelExtensions != null && axis.TickLabelExtensions.Count > 0))
                {
                    builder.StartElement(GetTagName("tickLabels", isChartEx));
                    DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), axis.TickLabelExtensions, writer);
                    builder.EndElement(GetTagName("tickLabels", isChartEx));
                }
            }
            else
            {
                object tickLblPos = axis.ChartAxisPr.GetDirectProperty(DmlChartAxisAttrs.TickLblPos);
                if (tickLblPos != null)
                    builder.WriteElementWithAttributes(GetTagName("tickLblPos", isChartEx), "val",
                        DmlChartsEnum.TickLabelPositionToDml((AxisTickLabelPosition)tickLblPos));
            }
          
            DmlChartCommonWriter.WriteShapeProperties(axis.SpPr, writer, isChartEx);
            DmlChartTxPr axisTxPr = (DmlChartTxPr)axis.ChartAxisPr.GetProperty(DmlChartAxisAttrs.TxPr);
            DmlChartCommonWriter.WriteTextProperties(axisTxPr, writer, isChartEx);

            if (!isChartEx)
            {
                builder.WriteElementWithAttributes(GetTagName("crossAx", isChartEx), "val", axis.CrossAx);

                if (axis.Crosses == AxisCrosses.Custom)
                {
                    builder.WriteElementWithAttributes(GetTagName("crossesAt", isChartEx), "val",
                        FormatterPal.DoubleToStr(axis.CrossesAt));
                }
                else
                {
                    builder.WriteElementWithAttributes(GetTagName("crosses", isChartEx), "val",
                        DmlChartsEnum.CrossesToDml(axis.Crosses));
                }
            }
        }

        /// <summary>
        /// Generates a tag name with a prefix depending on chart type.
        /// </summary>
        /// <param name="localName">Name without prefix.</param>
        /// <param name="isChartEx">A flag indicating that the chart is of the chartEx schema.</param>
        /// <returns>Tag name with prefix.</returns>
        private static string GetTagName(string localName, bool isChartEx)
        {
            return DmlChartCommonWriter.GetTagName(localName, isChartEx);
        }

        /// <summary>
        /// Gets a tag prefix depending on chart type.
        /// </summary>
        private static string GetTagPrefix(bool isChartEx)
        {
            return DmlChartCommonWriter.GetTagPrefix(isChartEx);
        }
    }
}
