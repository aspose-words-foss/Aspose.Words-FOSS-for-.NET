// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/21/2014 by Alexey Noskov

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlChartCommonWriter
    {
        /// <summary>
        /// Writes shape properties.
        /// </summary>
        internal static void WriteShapeProperties(DmlChartSpPr spPr, IDmlShapeWriterContext writer, bool isChartEx)
        {
            WriteShapeProperties(spPr, writer, isChartEx, null);
        }

        /// <summary>
        /// Writes shape properties.
        /// </summary>
        internal static void WriteShapeProperties(DmlChartSpPr spPr, IDmlShapeWriterContext writer, bool isChartEx,
            string prefix)
        {
            if (spPr == null || spPr.IsEmpty)
                return;

            if (!StringUtil.HasChars(prefix))
                prefix = GetTagPrefix(isChartEx);

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName(prefix, "spPr"));

            // In chart blip fill must have 'a' prefix.
            if ((spPr.DirectFill != null) && (spPr.DirectFill.DmlFillType == DmlFillType.BlipFill))
                DmlFillWriter.WriteDmlBlipFill("a", (DmlBlipFill)spPr.DirectFill, writer);
            else
                DmlFillWriter.Write(spPr.DirectFill, writer, false);

            DmlOutlineWriter.Write("a:ln", spPr.DirectOutline, writer);
            DmlShapeEffectsWriter.Write(spPr.Effects, writer, false);
            Dml3DPropertiesWriter.WriteScene3D(spPr.Scene3DProp, writer, false);
            Dml3DPropertiesWriter.WriteShape3D(spPr.Shape3DProp, writer, false);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), spPr.Extensions, writer);

            builder.EndElement(GetTagName(prefix, "spPr"));
        }

        internal static void WriteTextProperties(DmlChartTxPr txPr, IDmlShapeWriterContext writer, bool isChartEx)
        {
            if (txPr == null || txPr.IsEmpty)
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("txPr", isChartEx));

            DmlTextShapeWriter.WriteDmlTextShapeBodyPr("a:bodyPr", txPr.BodyPr, writer);
            // According to the specs lstStyle element is not supported within chart, write it as empty tag.
            builder.WriteEmptyElement("a:lstStyle");
            DmlTextShapeWriter.WriteParagraph(txPr.FirstParagraph, writer);

            builder.EndElement(GetTagName("txPr", isChartEx));
        }

        internal static void WriteTitle(DmlChartTitle title, IDmlShapeWriterContext writer,
            bool isAutoTitleDeleted, bool isChartEx, bool isAxisTitle)
        {
            if ((title == null) || isAutoTitleDeleted)
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("title", isChartEx));

            // WORDSNET-21476 For the axis title, these attributes cause an error when MS Word 2019 tries to open the file.
            // It doesn't matter if the title text is set or not.
            if (isChartEx && (!isAxisTitle))
            {
                if (title.SidePosition != SidePosition.Top)
                    builder.WriteAttribute("pos", DmlChartsEnum.SidePositionToDml(title.SidePosition));
                if (title.PositionAlignment != PositionAlignment.Center)
                    builder.WriteAttribute("align", DmlChartsEnum.PositionAlignmentToDml(title.PositionAlignment));

                builder.WriteAttributeIfTrue(Overlay, title.Overlay);
            }

            WriteTx(title.Tx, writer, isChartEx);
            if (!isChartEx)
            {
                WriteManualLayout(GetTagPrefix(isChartEx), title.Layout, writer, isChartEx);
                builder.WriteElementWithAttributes(GetTagName(Overlay, isChartEx), "val", title.Overlay);
            }
            WriteShapeProperties(title.SpPr, writer, isChartEx);
            WriteTextProperties(title.TxPr, writer, isChartEx);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), ((IDmlExtensionListSource)title).Extensions, writer);

            builder.EndElement(GetTagName("title", isChartEx));
        }

        internal static void WriteTx(DmlChartTx tx, IDmlShapeWriterContext writer, bool isChartEx)
        {
            if (tx == null)
                return;

            if ((tx.TxType == DmlChartTxType.ChartText) && (tx.RichText.Paragraphs.Count == 0))
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("tx", isChartEx));

            if (tx.StrRef == null)
            {
                if (tx.TxType == DmlChartTxType.ChartText)
                {
                    WriteRich(tx, writer, isChartEx);
                }
                else
                {
                    if (isChartEx)
                    {
                        WriteTxData(tx, writer);
                    }
                    else
                    {
                        // "c:v" is required so write even if SourcePlainText is empty.
                        builder.WriteElement(GetTagName("v", isChartEx), tx.SourcePlainText);
                    }
                }
            }
            else
            {
                WriteValueRef(tx.StrRef, writer, false);
            }

            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), tx.Extensions, writer);

            builder.EndElement(GetTagName("tx", isChartEx));
        }

        /// <summary>
        /// Writes a rich text element (21.2.2.156 rich).
        /// </summary>
        private static void WriteRich(DmlChartTx tx, IDmlShapeWriterContext writer, bool isChartEx)
        {
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("rich", isChartEx));

            DmlTextShapeWriter.WriteDmlTextShapeBodyPr("a:bodyPr", tx.RichText.Properties, writer);
            // According to the specs lstStyle element is not supported within chart, write it as empty tag.
            builder.WriteEmptyElement("a:lstStyle");
            foreach (DmlParagraph paragraph in tx.RichText.Paragraphs)
                DmlTextShapeWriter.WriteParagraph(paragraph, writer);

            builder.EndElement(GetTagName("rich", isChartEx));
        }

        /// <summary>
        /// Writes the txData element of the 2.24.3.79 CT_TextData complex type [MS-ODRAWXML].
        /// </summary>
        private static void WriteTxData(DmlChartTx tx, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("txData", true));

            if (tx.Formula != null)
                WriteFormula(GetTagName("f", true), tx.Formula, builder);

            if (tx.PlainText != null)
            {
                builder.StartElement(GetTagName("v", true));
                builder.WriteString(tx.PlainText);
                builder.EndElement(GetTagName("v", true));
            }

            builder.EndElement(GetTagName("txData", true));
        }

        internal static void WriteDataSource(string tagName, DmlChartDataSource dataSource, IDmlShapeWriterContext writer,
            bool isChartEx)
        {
            DmlChartValueCollection values = dataSource.Values;
            if (values == null)
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(tagName);

            if (!dataSource.ValueRef.IsLiteralData)
            {
                // Writes data as xxxRef/xxxCache elements.
                WriteValueRef(dataSource.ValueRef, writer, isChartEx);
            }
            else
            {
                // This code is intended to write xxxLit element, not xxxCache. Otherwise wrong markup is written.

                if (!isChartEx && (values.ValueType == DmlChartValueType.MultiLvlString))
                {
                    // Need to write AlternateContent element.

                    builder.StartElement("mc:AlternateContent");
                    builder.WriteAttribute("xmlns:mc",
                        DocxNamespaces.GetNamespace(DocxNamespace.MarkupCompatibility, false));

                    builder.StartElement("mc:Choice");
                    builder.WriteAttribute("Requires", "c16ac");
                    builder.WriteAttribute("xmlns:c16ac", DocxNamespaces.GetNamespace(DocxNamespace.Chart16Ac, false));
                    WriteValueCollection(values, writer, false, false);
                    builder.EndElement("mc:Choice");

                    builder.StartElement("mc:Fallback");
                    DmlChartValueCollection fallbackValues = dataSource.GetOrGenerateFallbackValues();
                    WriteValueCollection(fallbackValues, writer, false, false);
                    builder.EndElement("mc:Fallback");

                    builder.EndElement("mc:AlternateContent");
                }
                else
                {
                    WriteValueCollection(values, writer, false, isChartEx);
                }
            }

            builder.EndElement(tagName);
        }

        internal static void WriteManualLayout(string prefix, DmlChartManualLayout layout, IDmlShapeWriterContext writer,
            bool isChartEx)
        {
            NrxXmlBuilder builder = writer.Builder;

            if (layout == null)
            {
                builder.WriteEmptyElement(string.Format("{0}:layout", prefix));
                return;
            }

            builder.StartElement(string.Format("{0}:layout", prefix));
            builder.StartElement(GetTagName("manualLayout", isChartEx));

            if (layout.LayoutTarget == LayoutTarget.Inner)
                builder.WriteElementWithAttributes(GetTagName("layoutTarget", isChartEx), "val", DmlChartsEnum.LayoutTargetToDml(layout.LayoutTarget));

            WriteLayoutMode(layout.LeftMode, builder, GetTagName("xMode", isChartEx));
            WriteLayoutMode(layout.TopMode, builder, GetTagName("yMode", isChartEx));
            WriteLayoutMode(layout.WidthMode, builder, GetTagName("wMode", isChartEx));
            WriteLayoutMode(layout.HeightMode, builder, GetTagName("hMode", isChartEx));

            if (layout.IsXSet || layout.IsYSet)
            {
                builder.WriteElementWithAttributes(GetTagName("x", isChartEx), "val",
                    FormatterPal.DoubleToStrRoundtrip(layout.Left));
                builder.WriteElementWithAttributes(GetTagName("y", isChartEx), "val",
                    FormatterPal.DoubleToStrRoundtrip(layout.Top));
            }

            if (layout.IsWidthSet)
                builder.WriteElementWithAttributes(GetTagName("w", isChartEx), "val",
                    FormatterPal.DoubleToStrRoundtrip(layout.Width));

            if (layout.IsHeightSet)
                builder.WriteElementWithAttributes(GetTagName("h", isChartEx), "val",
                    FormatterPal.DoubleToStrRoundtrip(layout.Height));

            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), layout.Extensions, writer);

            builder.EndElement(GetTagName("manualLayout", isChartEx));
            builder.EndElement(string.Format("{0}:layout", prefix));
        }

        /// <summary>
        /// Writes wrapped bool value.
        /// </summary>
        internal static void WriteWrappedBooleanIfTrue(string tagName, bool value, NrxXmlBuilder builder)
        {
            if (value)
                builder.WriteElementWithAttributes(tagName, "val", value);
        }

        /// <summary>
        /// Writes wrapped bool value.
        /// </summary>
        internal static void WriteWrappedDoubleIfNotZero(string tagName, double value, NrxXmlBuilder builder)
        {
            if (!MathUtil.IsZero(value))
                builder.WriteElementWithAttributes(tagName, "val", FormatterPal.DoubleToStr9Decimals(value));
        }

        internal static void WriteSurface(string tagName, DmlChartSurface surface, IDmlShapeWriterContext writer,
            bool isChartEx)
        {
            if (surface == null)
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(tagName);

            if (!isChartEx)
                builder.WriteElementWithAttributes(GetTagName("thickness", isChartEx), "val", surface.Thickness);
            WriteShapeProperties(surface.SpPr, writer, isChartEx);
            if (!isChartEx)
                WritePictureOptions(surface.PictureOptions, builder, isChartEx);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), surface.Extensions, writer);

            builder.EndElement(tagName);
        }

        internal static void WritePictureOptions(DmlChartPictureOptions pictOpt, NrxXmlBuilder builder, bool isChartEx)
        {
            if (pictOpt == null)
                return;

            builder.StartElement(GetTagName("pictureOptions", isChartEx));
            WriteWrappedBooleanIfTrue(GetTagName("applyToEnd", isChartEx), pictOpt.ApplyToEnd, builder);
            WriteWrappedBooleanIfTrue(GetTagName("applyToFront", isChartEx), pictOpt.ApplyToFront, builder);
            WriteWrappedBooleanIfTrue(GetTagName("applyToSides", isChartEx), pictOpt.ApplyToSides, builder);
            builder.WriteElementWithAttributes(GetTagName("pictureFormat", isChartEx), "val",
                DmlChartsEnum.PictureFormatToDml(pictOpt.PictureFormat));
            if (!MathUtil.IsZero(pictOpt.PictureStackUnit))
                builder.WriteElementWithAttributes(GetTagName("pictureStackUnit", isChartEx), "val",
                    pictOpt.PictureStackUnit);
            builder.EndElement(GetTagName("pictureOptions", isChartEx));
        }

        internal static void WriteLegend(ChartLegend legend, IDmlShapeWriterContext writer, bool isChartEx)
        {
            if ((legend == null) || !legend.IsVisible)
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("legend", isChartEx));

            if (isChartEx)
            {
                if (legend.SidePosition != ChartLegend.ChartExDefaultSidePosition)
                    builder.WriteAttribute("pos", DmlChartsEnum.SidePositionToDml(legend.SidePosition));
                if (legend.PositionAlignment != PositionAlignment.Center)
                    builder.WriteAttribute("align", DmlChartsEnum.PositionAlignmentToDml(legend.PositionAlignment));

                builder.WriteAttributeIfTrue(Overlay, legend.Overlay);
            }
            else
            {
                builder.WriteElementWithAttributes(GetTagName("legendPos", isChartEx), "val",
                    DmlChartsEnum.LegendPositionToDml(legend.Position));

                foreach (ChartLegendEntry entry in legend.LegendEntries.MaterializedLegendEntries)
                    WriteLegendEntry(entry, writer, isChartEx);

                WriteManualLayout(GetTagPrefix(isChartEx), legend.Layout, writer, isChartEx);
                builder.WriteElementWithAttributes(GetTagName(Overlay, isChartEx), "val", legend.Overlay);
            }
            WriteShapeProperties(legend.SpPr, writer, isChartEx);
            WriteTextProperties(legend.TxPr, writer, isChartEx);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), ((IDmlExtensionListSource)legend).Extensions, writer);

            builder.EndElement(GetTagName("legend", isChartEx));
        }

        internal static void WriteProtection(DmlChartProtection protection, NrxXmlBuilder builder, bool isChartEx)
        {
            if (protection == null || !protection.HasProtection)
                return;

            builder.StartElement(GetTagName("protection", isChartEx));
            WriteWrappedBooleanIfTrue(GetTagName("chartObject", isChartEx), protection.ChartObject, builder);
            WriteWrappedBooleanIfTrue(GetTagName("data", isChartEx), protection.Data, builder);
            WriteWrappedBooleanIfTrue(GetTagName("formatting", isChartEx), protection.Formatting, builder);
            WriteWrappedBooleanIfTrue(GetTagName("selection", isChartEx), protection.Selection, builder);
            WriteWrappedBooleanIfTrue(GetTagName("userInterface", isChartEx), protection.UserInterface, builder);
            builder.EndElement(GetTagName("protection", isChartEx));
        }

        internal static void WriteNumberFormat(DmlChartNumFormat format, NrxXmlBuilder builder, bool isChartEx)
        {
            if (format == null)
                return;

            builder.StartElement(GetTagName("numFmt", isChartEx));
            builder.WriteAttribute("formatCode",
                !string.IsNullOrEmpty(format.SourceFormatCode)
                    ? format.SourceFormatCode
                    : ChartNumberFormat.GeneralFormatCode);
            builder.WriteAttribute("sourceLinked", format.SourceLinked);
            builder.EndElement(GetTagName("numFmt", isChartEx));
        }

        internal static void WriteDataLabels(ChartDataLabelCollection dataLabels, IDmlShapeWriterContext writer,
            bool isChartEx)
        {
            Debug.Assert(dataLabels != null);
            DmlChartDataLabelPr labelPr = dataLabels.LabelPr;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName(isChartEx ? "dataLabels" : "dLbls", isChartEx));

            if (isChartEx)
            {
                object position = labelPr.GetDirectProperty(DmlChartDataLabelAttrs.DLblPos);
                if (position != null)
                    builder.WriteAttribute("pos", DmlChartsEnum.DataLabelPositionToDml((ChartDataLabelPosition)position));
            }

            List<ChartDataLabel> labels = dataLabels.GetLabelsToWrite();
            foreach (ChartDataLabel label in labels)
                WriteDataLabel(label, writer, isChartEx);

            WriteDataLabelProperties(GetTagPrefix(isChartEx), dataLabels.LabelPr, writer, isChartEx, false);

            if (isChartEx)
            {
                foreach (ChartDataLabel label in labels)
                {
                    if (label.IsHidden)
                        builder.WriteElementWithAttributes(GetTagName("dataLabelHidden", true), "idx", label.Index);
                }
            }

            builder.EndElement(GetTagName(isChartEx ? "dataLabels" : "dLbls", isChartEx));
        }

        /// <summary>
        /// Writes an element of the CT_ChartLines complex type [ISO/IEC 29500] or the 2.24.3.52 CT_Gridlines
        /// complex type [MS-ODRAWXML].
        /// </summary>
        internal static void WriteGridlines(string tagName, DmlChartGridlines gridlines, IDmlShapeWriterContext writer,
            bool isChartEx)
        {
            if (gridlines == null)
                return;

            writer.Builder.StartElement(GetTagName(tagName, isChartEx));

            WriteShapeProperties(gridlines.SpPr, writer, isChartEx);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), gridlines.Extensions, writer);

            writer.Builder.EndElement(GetTagName(tagName, isChartEx));
        }

        internal static void WriteWrappedSpPr(string wrapperTagName, DmlChartSpPr spPr, IDmlShapeWriterContext writer,
            bool allowEmpty, bool isChartEx)
        {
            if (spPr == null || (spPr.IsEmpty && !allowEmpty))
                return;

            writer.Builder.StartElement(wrapperTagName);
            WriteShapeProperties(spPr, writer, isChartEx);
            writer.Builder.EndElement(wrapperTagName);
        }

        internal static void WriteDataPoints(ChartSeries series, IDmlShapeWriterContext writer, bool isChartEx)
        {
            bool writeInvertIfNegative = ChartDataPointCollection.SupportsInvertIfNegative(series.Chart.ChartType);

            NrxXmlBuilder builder = writer.Builder;
            foreach (ChartDataPoint dpt in series.DataPoints.MaterializedDataPoints)
            {
                if (!dpt.HasNonDefaultFormatting)
                    continue;

                builder.StartElement(GetTagName(isChartEx ? "dataPt" : "dPt", isChartEx));
                DmlChartDataPointPr dmlChartDataPointPr = dpt.PointPr;
                if (isChartEx)
                {
                    builder.WriteAttribute("idx", dpt.Index);
                }
                else
                {
                    WriteDataPointAttr(dmlChartDataPointPr, DmlChartDataPointAttr.Index, writer, isChartEx);

                    // The attribute 'invertIfNegative' does not get a value from the parent collection (series) in
                    // MS Word. Thus if a value is not defined in a data point but is written for the collection
                    // (series), it should be written too. And because 'invertIfNegative' is always written for the
                    // collection (series) in some chart types (see DmlChartSeriesWriter), let's write it for all
                    // materialized data points in that charts too.
                    if (writeInvertIfNegative || dpt.PointPr.IsPropertySpecified(DmlChartDataPointAttr.InvertIfNegative))
                    {
                        builder.WriteElementWithAttributes(GetTagName("invertIfNegative", false),
                            "val", dpt.InvertIfNegative);
                    }

                    WriteDataPointAttr(dmlChartDataPointPr, DmlChartDataPointAttr.Bubble3D, writer, isChartEx);
                    WriteDataPointAttr(dmlChartDataPointPr, DmlChartDataPointAttr.Explosion, writer, isChartEx);
                    WriteDataPointAttr(dmlChartDataPointPr, DmlChartDataPointAttr.Marker, writer, isChartEx);
                    WriteDataPointAttr(dmlChartDataPointPr, DmlChartDataPointAttr.PictureOptions, writer, isChartEx);
                }
                WriteDataPointAttr(dmlChartDataPointPr, DmlChartDataPointAttr.SpPr, writer, isChartEx);
                WriteDataPointAttr(dmlChartDataPointPr, DmlChartDataPointAttr.Extensions, writer, isChartEx);
                builder.EndElement(GetTagName(isChartEx ? "dataPt" : "dPt", isChartEx));
            }
        }

        internal static void WriteMarker(ChartMarker marker, IDmlShapeWriterContext writer, bool isChartEx)
        {
            DmlChartMarkerPr dmlChartMarkerPr = marker.MarkerPr;
            if (dmlChartMarkerPr.Count == 0)
                return;

            writer.Builder.StartElement(GetTagName("marker", isChartEx));
            WriteMarkerAttr(dmlChartMarkerPr, DmlChartMarkerAttr.Symbol, writer, isChartEx);
            WriteMarkerAttr(dmlChartMarkerPr, DmlChartMarkerAttr.Size, writer, isChartEx);
            WriteMarkerAttr(dmlChartMarkerPr, DmlChartMarkerAttr.SpPr, writer, isChartEx);
            WriteMarkerAttr(dmlChartMarkerPr, DmlChartMarkerAttr.Extensions, writer, isChartEx);
            writer.Builder.EndElement(GetTagName("marker", isChartEx));
        }

        internal static void WriteErrorBars(DmlChartErrorBars errorBars, IDmlShapeWriterContext writer, bool isChartEx)
        {
            if (errorBars == null)
                return;

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(GetTagName("errBars", isChartEx));
            if (errorBars.ErrDir != ErrorBarDirection.Y)
                builder.WriteElementWithAttributes(GetTagName("errDir", isChartEx), "val", DmlChartsEnum.ErrorBarDirectionToDml(errorBars.ErrDir));
            builder.WriteElementWithAttributes(GetTagName("errBarType", isChartEx), "val", DmlChartsEnum.ErrorBarTypeToDml(errorBars.ErrBarType));
            builder.WriteElementWithAttributes(GetTagName("errValType", isChartEx), "val", DmlChartsEnum.ErrorValueTypeToDml(errorBars.ErrValType));
            builder.WriteElementWithAttributes(GetTagName("noEndCap", isChartEx), "val", errorBars.NoEndCap);
            WriteDataSource(GetTagName("plus", isChartEx), errorBars.Plus.DataSource, writer, isChartEx);
            WriteDataSource(GetTagName("minus", isChartEx), errorBars.Minus.DataSource, writer, isChartEx);
            if (!MathUtil.IsZero(errorBars.Val))
                builder.WriteElementWithAttributes(GetTagName("val", isChartEx), "val", errorBars.Val);
            WriteShapeProperties(errorBars.SpPr, writer, isChartEx);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), errorBars.Extensions, writer);
            builder.EndElement(GetTagName("errBars", isChartEx));
        }

        internal static void WriteTrendLines(IList<DmlChartTrendline> trendlines, IDmlShapeWriterContext writer,
            bool isChartEx)
        {
            NrxXmlBuilder builder = writer.Builder;

            foreach (DmlChartTrendline trendline in trendlines)
            {
                builder.StartElement(GetTagName("trendline", isChartEx));
                DmlChartTrendlinePr dmlChartTrendlinePr = trendline.TrendlinePr;
                WriteTrendlineArrt(dmlChartTrendlinePr, DmlChartTrendlineAttr.Name, writer, isChartEx);
                WriteTrendlineArrt(dmlChartTrendlinePr, DmlChartTrendlineAttr.SpPr, writer, isChartEx);
                WriteTrendlineArrt(dmlChartTrendlinePr, DmlChartTrendlineAttr.TrendlineType, writer, isChartEx);
                WriteTrendlineArrt(dmlChartTrendlinePr, DmlChartTrendlineAttr.Order, writer, isChartEx);
                WriteTrendlineArrt(dmlChartTrendlinePr, DmlChartTrendlineAttr.Period, writer, isChartEx);
                WriteTrendlineArrt(dmlChartTrendlinePr, DmlChartTrendlineAttr.Forward, writer, isChartEx);
                WriteTrendlineArrt(dmlChartTrendlinePr, DmlChartTrendlineAttr.Backward, writer, isChartEx);
                WriteTrendlineArrt(dmlChartTrendlinePr, DmlChartTrendlineAttr.Intercept, writer, isChartEx);
                WriteTrendlineArrt(dmlChartTrendlinePr, DmlChartTrendlineAttr.DispRSqr, writer, isChartEx);
                WriteTrendlineArrt(dmlChartTrendlinePr, DmlChartTrendlineAttr.DispEq, writer, isChartEx);
                WriteTrendlineArrt(dmlChartTrendlinePr, DmlChartTrendlineAttr.TrendlineLbl, writer, isChartEx);
                WriteTrendlineArrt(dmlChartTrendlinePr, DmlChartTrendlineAttr.Extensions, writer, isChartEx);
                builder.EndElement(GetTagName("trendline", isChartEx));
            }
        }

        internal static void WriteView3D(DmlChartFormat chartFormat, IDmlShapeWriterContext writer, bool isChartEx)
        {
            DmlChartView3D view3D = chartFormat.View3D;

            if (view3D == null)
                return;

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(GetTagName("view3D", isChartEx));

            builder.WriteElementWithAttributes(GetTagName("rotX", isChartEx), "val", view3D.RotX);
            WriteDoubleIfNotDefault(GetTagName("hPercent", isChartEx), view3D.HPercent, DmlChartView3D.DefaultHPercent, builder);

            // By specification default RotY is zero, however in MS Word if RotY is omitted,
            // it uses value 20 degrees as default for all charts except Pie3DChart, for Pie3DChart the default value is zero.
            int rotY = view3D.RotY;
            if ((chartFormat.PlotArea.FirstChart.ChartType == DmlChartType.Pie3DChart) && !view3D.HasRotY)
                rotY = 0;

            builder.WriteElementWithAttributes(GetTagName("rotY", isChartEx), "val", rotY);

            WriteDoubleIfNotDefault(GetTagName("depthPercent", isChartEx), view3D.DepthPercent, DmlChartView3D.DefaultDepthPercent, builder);
            builder.WriteElementWithAttributes(GetTagName("rAngAx", isChartEx), "val", view3D.RAngAx);

            WriteDoubleIfNotDefault(GetTagName("perspective", isChartEx), view3D.Perspective, DmlChartView3D.DefaultPerspective, builder);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), view3D.Extensions, writer);

            builder.EndElement(GetTagName("view3D", isChartEx));
        }

        internal static void WritePivotFmts(DmlChartPivotFormats pivotFmts, IDmlShapeWriterContext writer, bool isChartEx)
        {
            if (pivotFmts == null)
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("pivotFmts", isChartEx));

            foreach (DmlChartPivotFormat fmt in pivotFmts.Fmts.Values)
            {
                builder.StartElement(GetTagName("pivotFmt", isChartEx));
                builder.WriteElementWithAttributes(GetTagName("idx", isChartEx), "val", fmt.Index);
                WriteShapeProperties(fmt.SpPr, writer, isChartEx);
                WriteTextProperties(fmt.TxPr, writer, isChartEx);
                WriteMarker(fmt.Marker, writer, isChartEx);
                WriteDataLabel(fmt.DLbl, writer, isChartEx);
                DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), fmt.Extensions, writer);
                builder.EndElement(GetTagName("pivotFmt", isChartEx));
            }

            builder.EndElement(GetTagName("pivotFmts", isChartEx));
        }

        internal static void WritePivotSource(DmlChartPivotSource pivotSource, IDmlShapeWriterContext writer,
            bool isChartEx)
        {
            if (pivotSource == null)
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("pivotSource", isChartEx));
            builder.WriteElement(GetTagName("name", isChartEx), pivotSource.Name);
            builder.WriteElementWithAttributes(GetTagName("fmtId", isChartEx), "val", pivotSource.FmtId);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), pivotSource.Extensions, writer);
            builder.EndElement(GetTagName("pivotSource", isChartEx));
        }

        internal static void WriteDisplayUnits(AxisDisplayUnit dispUnits, IDmlShapeWriterContext writer, bool isChartEx)
        {
            if ((dispUnits == null) || !dispUnits.IsDefined)
                return;

            NrxXmlBuilder builder = writer.Builder;
            string elementName = isChartEx ? "units" : "dispUnits";

            builder.StartElement(GetTagName(elementName, isChartEx));

            if (dispUnits.Unit != AxisBuiltInUnit.None)
            {
                if (isChartEx)
                {
                    if (dispUnits.Unit != AxisBuiltInUnit.Custom)
                        builder.WriteAttribute("unit", DmlChartsEnum.BuiltInUnitToDml(dispUnits.Unit));
                }
                else
                {
                    if (dispUnits.Unit != AxisBuiltInUnit.Custom)
                    {
                        builder.WriteElementWithAttributes(GetTagName("builtInUnit", isChartEx), "val",
                            DmlChartsEnum.BuiltInUnitToDml(dispUnits.Unit));
                    }
                    else
                    {
                        builder.WriteElementWithAttributes(GetTagName("custUnit", isChartEx), "val",
                            FormatterPal.DoubleToStrRoundtrip(dispUnits.CustomUnit));
                    }
                }
            }

            WriteDisplayUnitsLabel(dispUnits.Label, writer, isChartEx);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx),
                ((IDmlExtensionListSource)dispUnits).Extensions, writer);

            builder.EndElement(GetTagName(elementName, isChartEx));
        }

        internal static void WriteScaling(AxisScaling scaling, IDmlShapeWriterContext writer, bool isChartEx)
        {
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("scaling", isChartEx));

            if (scaling.Type == AxisScaleType.Logarithmic)
                builder.WriteElementWithAttributes(GetTagName("logBase", isChartEx), "val",
                    FormatterPal.DoubleToStrRoundtrip(scaling.LogBase));

            builder.WriteElementWithAttributes(GetTagName("orientation", isChartEx), "val",
                DmlChartsEnum.AxisOrientationToDml(scaling.Orientation));

            if (!scaling.Maximum.IsAuto)
                builder.WriteElementWithAttributes(GetTagName("max", isChartEx), "val",
                    FormatterPal.DoubleToStrRoundtrip(scaling.Maximum.Value));

            if (!scaling.Minimum.IsAuto)
                builder.WriteElementWithAttributes(GetTagName("min", isChartEx), "val",
                    FormatterPal.DoubleToStrRoundtrip(scaling.Minimum.Value));

            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), ((IDmlExtensionListSource)scaling).Extensions, writer);

            builder.EndElement(GetTagName("scaling", isChartEx));
        }

        internal static void WriteBandFormats(DmlChartBandFormats bandFmts, IDmlShapeWriterContext writer, bool isChartEx)
        {
            if (bandFmts == null || bandFmts.Count == 0)
                return;

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(GetTagName("bandFmts", isChartEx));

            foreach (DmlChartBandFormat format in bandFmts.Formats)
            {
                builder.StartElement(GetTagName("bandFmt", isChartEx));
                builder.WriteElementWithAttributes(GetTagName("idx", isChartEx), "val", format.Index);
                WriteShapeProperties(format.SpPr, writer, isChartEx);
                builder.EndElement(GetTagName("bandFmt", isChartEx));
            }

            builder.EndElement(GetTagName("bandFmts", isChartEx));
        }

        internal static void WriteCustomSplit(int[] custSplit, NrxXmlBuilder builder, bool isChartEx)
        {
            builder.StartElement(GetTagName("custSplit", isChartEx));
            foreach (int split in custSplit)
                builder.WriteElementWithAttributes(GetTagName("secondPiePt", isChartEx), "val", split);
            builder.EndElement(GetTagName("custSplit", isChartEx));
        }

        internal static void WriteUpDownBars(DmlChartUpDownBars upDownBars, IDmlShapeWriterContext writer,
            bool isChartEx)
        {
            if (upDownBars == null || upDownBars.IsEmpty)
                return;

            writer.Builder.StartElement(GetTagName("upDownBars", isChartEx));
            writer.Builder.WriteElementWithAttributes(GetTagName("gapWidth", isChartEx), "val", upDownBars.GapWidth);
            WriteWrappedSpPr(GetTagName("upBars", isChartEx), upDownBars.UpBars, writer, upDownBars.HasUpBars, isChartEx);
            WriteWrappedSpPr(GetTagName("downBars", isChartEx), upDownBars.DownBars, writer, upDownBars.HasDownBars, isChartEx);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), upDownBars.Extensions, writer);
            writer.Builder.EndElement(GetTagName("upDownBars", isChartEx));
        }

        private static void WriteValueRef(DmlChartValueRef valueRef, IDmlShapeWriterContext writer, bool isChartEx)
        {
            NrxXmlBuilder builder = writer.Builder;

            string tagName = GetValueRefTagName(valueRef.Values.ValueType, isChartEx);

            builder.StartElement(tagName);
            if (isChartEx)
            {
                builder.WriteAttribute("type", DmlChartsEnum.DimensionTypeToDml(valueRef.DimensionType));

                WriteFormula(GetTagName("f", isChartEx), valueRef.Formula, builder);
                WriteFormula(GetTagName("nf", isChartEx), valueRef.NameFormula, builder);

                WriteChartValueLevels(valueRef.Values, builder, isChartEx);
            }
            else
            {
                builder.WriteElement(GetTagName("f", isChartEx), valueRef.Formula.Value);
                WriteValueCollection(valueRef.Values, writer, true, isChartEx);
                DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), valueRef.Extensions, writer);
            }

            builder.EndElement(tagName);
        }

        /// <summary>
        /// Writes an element of the 2.24.3.28 CT_Formula complex type [MS-ODRAWXML].
        /// </summary>
        private static void WriteFormula(string elementName, DmlChartFormula formula, NrxXmlBuilder builder)
        {
            if (formula == null || !StringUtil.HasChars(formula.Value))
                return;

            builder.StartElement(elementName);

            if (formula.Direction != FormulaDirection.Column)
                builder.WriteAttribute("dir", DmlChartsEnum.FormulaDirectionToDml(formula.Direction));
            builder.WriteString(formula.Value);

            builder.EndElement();
        }

        private static string GetValueRefTagName(DmlChartValueType refType, bool isChartEx)
        {
            switch (refType)
            {
                case DmlChartValueType.String:
                    return GetTagName("strRef", isChartEx);
                case DmlChartValueType.Numeric:
                    return GetTagName("numRef", isChartEx);
                case DmlChartValueType.MultiLvlString:
                    return GetTagName(isChartEx ? "strDim" : "multiLvlStrRef", isChartEx);
                case DmlChartValueType.MultiLvlNumeric:
                {
                    if (isChartEx)
                        return GetTagName("numDim", isChartEx);
                    else
                        throw new ArgumentException("Unexpected Dml Chart Value Reference Type");
                }
                default:
                    throw new ArgumentException("Unexpected Dml Chart Value Reference Type");
            }
        }

        internal static void WriteValueCollection(string tagName, DmlChartValueCollection values,
            IDmlShapeWriterContext writer, bool isChartEx)
        {
            if ((values == null) || (values.ValueCount <= 0) || (values.ValueType == DmlChartValueType.None))
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(tagName);

            if (values.ValueType == DmlChartValueType.Numeric && StringUtil.HasChars(values.FormatCode))
                builder.WriteElement(GetTagName("formatCode", isChartEx), values.FormatCode);

            builder.WriteElementWithAttributes(GetTagName("ptCount", isChartEx), "val", values.ValueCount);

            if (values.ValueType == DmlChartValueType.MultiLvlString)
            {
                WriteChartValueLevels(values, builder, false);
            }
            else
            {
                // Let's order by value index.
                for (int i = 0; i < values.ValueCount; i++)
                {
                    DmlChartValue value = values[i];
                    if (value != null)
                        WriteChartValuePoint(value, builder, isChartEx);
                }
            }

            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), values.Extensions, writer);

            builder.EndElement(tagName);
        }

        private static void WriteValueCollection(DmlChartValueCollection values, IDmlShapeWriterContext writer,
            bool isCache, bool isChartEx)
        {
            string tagName = GetValueCollectionTagName(values, isCache, isChartEx);
            WriteValueCollection(tagName, values, writer, isChartEx);
        }

        private static string GetValueCollectionTagName(DmlChartValueCollection values, bool isCache, bool isChartEx)
        {
            switch (values.ValueType)
            {
                case DmlChartValueType.String:
                    return GetTagName(isCache ? "strCache" : "strLit", isChartEx);
                case DmlChartValueType.Numeric:
                    return GetTagName(isCache ? "numCache" : "numLit", isChartEx);
                case DmlChartValueType.MultiLvlString:
                {
                    // There is a special namespace for the multiLvlStrLit element in pre-Word 2016 charts.
                    if (!isCache && !isChartEx)
                        return "c16ac:multiLvlStrLit";
                    else
                        return GetTagName("multiLvlStrCache", isChartEx);
                }
                default:
                    throw new ArgumentException("Unexpected Dml Chart Value Type");
            }
        }

        private static void WriteChartValueLevels(DmlChartValueCollection values, NrxXmlBuilder builder, bool isChartEx)
        {
            int levels = 0;
            if (isChartEx)
            {
                levels = values.LevelProperties.Count;
            }
            else
            {
                foreach (DmlChartValue value in values)
                    levels = System.Math.Max(((DmlChartMultiLvlStrValue)value).Levels.Count, levels);
            }

            for (int i = 0; i < levels; i++)
            {
                builder.StartElement(GetTagName("lvl", isChartEx));
                if (isChartEx)
                {
                    DmlChartDataLevelProperties properties = values.LevelProperties[i];
                    if (properties != null)
                    {
                        builder.WriteAttribute("ptCount", properties.ValueCount);
                        builder.WriteAttribute("formatCode", properties.FormatCode);
                        builder.WriteAttribute("name", properties.Name);
                    }
                }

                // Let's write sorted by index.
                for (int j = 0; j <= values.LastNonEmptyValueIndex; j++)
                {
                    DmlChartMultiLvlValue value = (DmlChartMultiLvlValue)values[j];
                    if (value == null)
                        continue;

                    // WORDSNET-11644 Levels can have a different number of <c:pt> elements.
                    if ((value.Levels.Count > i) && (value.Levels[i] != null))
                    {
                        builder.StartElement(GetTagName("pt", isChartEx));
                        builder.WriteAttribute("idx", value.Index);
                        if (isChartEx)
                        {
                            builder.WriteString(values.ValueType == DmlChartValueType.MultiLvlString
                                ? (string)value.Levels[i]
                                : FormatterPal.DoubleToStr((double)value.Levels[i]));
                        }
                        else
                        {
                            builder.WriteElement(GetTagName("v", isChartEx), (string)value.Levels[i]);
                        }
                        builder.EndElement(GetTagName("pt", isChartEx));
                    }
                }

                builder.EndElement(GetTagName("lvl", isChartEx));
            }
        }

        private static void WriteChartValuePoint(DmlChartValue val, NrxXmlBuilder builder, bool isChartEx)
        {
            if (val.ValueType == DmlChartValueType.None)
                return;

            builder.StartElement(GetTagName("pt", isChartEx));
            builder.WriteAttribute("idx", val.Index);

            if (val.ValueType == DmlChartValueType.String)
                builder.WriteElement(GetTagName("v", isChartEx), val.StringValue);

            if (val.ValueType == DmlChartValueType.Numeric)
            {
                DmlChartNumValue numValue = (DmlChartNumValue)val;
                if (StringUtil.HasChars(numValue.SourceFormatCode))
                    builder.WriteAttribute("formatCode", numValue.SourceFormatCode);
                // NaN value must be written as #N/A. See TestJira8965.
                builder.WriteElement(GetTagName("v", isChartEx),
                    double.IsNaN(numValue.Value) ? "#N/A" : FormatterPal.DoubleToStrRoundtrip(numValue.Value));
            }

            builder.EndElement(GetTagName("pt", isChartEx));
        }

        private static void WriteLayoutMode(LayoutMode layoutMode, NrxXmlBuilder builder, string tagName)
        {
            // Factor is default mode so do not write it.
            if (layoutMode == LayoutMode.Factor)
                return;

            builder.WriteElementWithAttributes(tagName, "val", DmlChartsEnum.LayoutModeToDml(layoutMode));
        }

        private static void WriteLegendEntry(ChartLegendEntry entry, IDmlShapeWriterContext writer, bool isChartEx)
        {
            if (!entry.HasNonDefaultFormatting)
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("legendEntry", isChartEx));
            builder.WriteElementWithAttributes(GetTagName("idx", isChartEx), "val", entry.Index);
            WriteWrappedBooleanIfTrue(GetTagName("delete", isChartEx), entry.IsHidden, builder);
            WriteTextProperties(entry.TxPr, writer, isChartEx);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), ((IDmlExtensionListSource)entry).Extensions, writer);
            builder.EndElement(GetTagName("legendEntry", isChartEx));
        }

        private static void WriteDataLabel(ChartDataLabel label, IDmlShapeWriterContext writer, bool isChartEx)
        {
            if ((label == null) || (isChartEx && label.IsHidden))
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName(isChartEx ? "dataLabel" : "dLbl", isChartEx));

            if (isChartEx)
            {
                builder.WriteAttribute("idx", label.Index);
                object position = label.LabelPr.GetDirectProperty(DmlChartDataLabelAttrs.DLblPos);
                if (position != null)
                    builder.WriteAttribute("pos", DmlChartsEnum.DataLabelPositionToDml((ChartDataLabelPosition)position));
            }

            WriteDataLabelProperties(GetTagPrefix(isChartEx), label.LabelPr, writer, isChartEx, label.IsHidden);

            builder.EndElement(GetTagName(isChartEx ? "dataLabel" : "dLbl", isChartEx));
        }

        private static void WriteDataLabelProperties(string prefix, DmlChartDataLabelPr labelPr,
            IDmlShapeWriterContext writer, bool isChartEx, bool isHidden)
        {
            if (!isChartEx)
            {
                WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.Idx, writer, isChartEx);
                WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.Delete, writer, isChartEx);
                WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.Layout, writer, isChartEx);
                // WORDSNET-20461 If the data label is hidden we must not write this node,
                // otherwise Word will notify that file is corrupted.
                // I didnt found anything related in the specs, and found the solution comparing Word's and AW's output.
                // Libre Office have no that problem, looks like it's a bug in the Word.
                if (!isHidden)
                    WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.Tx, writer, isChartEx);
            }
            WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.NumFmt, writer, isChartEx);
            WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.SpPr, writer, isChartEx);
            WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.TxPr, writer, isChartEx);
            if (isChartEx)
            {
                WriteDataLabelVisibility(prefix, labelPr, writer);
            }
            // WORDSNET-20461 The description is above.
            else if (!isHidden)
            {
                WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.DLblPos, writer, isChartEx);
                WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.ShowLegendKey, writer, isChartEx);
                WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.ShowVal, writer, isChartEx);
                WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.ShowCatName, writer, isChartEx);
                WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.ShowSerName, writer, isChartEx);
                WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.ShowPercent, writer, isChartEx);
                WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.ShowBubbleSize, writer, isChartEx);
            }
            WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.Separator, writer, isChartEx);
            if (!isChartEx)
            {
                WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.ShowLeaderLines, writer, isChartEx);
                WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.LeaderLines, writer, isChartEx);
            }
            WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.Extensions, writer, isChartEx);
        }

        /// <summary>
        /// Writes properties of the <see cref="DmlExtensionUri.DataLabels"/> extension.
        /// </summary>
        internal static void WriteDataLabelExtensionProperties(string prefix, DmlChartDataLabelPr labelPr,
            IDmlShapeWriterContext writer)
        {
            WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.Layout, writer, false);

            WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.ShapePr, writer, false);
            WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.TxPr, writer, false);

            WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.FieldTable, writer, false);
            WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.ShowDataLabelsRange, writer, false);
            WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.ShowLeaderLines, writer, false);
            WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.LeaderLines, writer, false);
            WriteDataLabelAttr(prefix, labelPr, DmlChartDataLabelAttrs.XForSave, writer, false);
        }

        /// <summary>
        /// Writes the visibility element of the 2.24.3.20 CT_DataLabelVisibilities complex type [MS-ODRAWXML].
        /// </summary>
        private static void WriteDataLabelVisibility(string prefix, DmlChartDataLabelPr labelPr,
            IDmlShapeWriterContext writer)
        {
            object showSeries = labelPr.GetDirectProperty(DmlChartDataLabelAttrs.ShowSerName);
            object showCategory = labelPr.GetDirectProperty(DmlChartDataLabelAttrs.ShowCatName);
            object showValue = labelPr.GetDirectProperty(DmlChartDataLabelAttrs.ShowVal);

            if (showSeries != null || showCategory != null || showValue != null)
            {
                writer.Builder.WriteElementWithAttributes(string.Format("{0}:visibility", prefix),
                    "seriesName", showSeries, "categoryName", showCategory, "value", showValue);
            }
        }

        private static void WriteDataLabelAttr(string prefix, DmlChartDataLabelPr labelPr, DmlChartDataLabelAttrs attr,
            IDmlShapeWriterContext writer, bool isChartEx)
        {
            NrxXmlBuilder builder = writer.Builder;
            object value = labelPr.GetDirectProperty(attr);
            if (value == null)
                return;

            switch (attr)
            {
                case DmlChartDataLabelAttrs.Delete:
                    builder.WriteElementWithAttributes(string.Format("{0}:delete", prefix), "val", value);
                    break;
                case DmlChartDataLabelAttrs.DLblPos:
                    builder.WriteElementWithAttributes(string.Format("{0}:dLblPos", prefix), "val",
                        DmlChartsEnum.DataLabelPositionToDml((ChartDataLabelPosition)value));
                    break;
                case DmlChartDataLabelAttrs.Idx:
                    builder.WriteElementWithAttributes(string.Format("{0}:idx", prefix), "val", value);
                    break;
                case DmlChartDataLabelAttrs.Layout:
                    WriteManualLayout(prefix, (DmlChartManualLayout)value, writer, isChartEx);
                    break;
                case DmlChartDataLabelAttrs.NumFmt:
                    WriteNumberFormat((DmlChartNumFormat)value, builder, isChartEx);
                    break;
                case DmlChartDataLabelAttrs.Separator:
                    builder.WriteElement(string.Format("{0}:separator", prefix), (string)value);
                    break;
                case DmlChartDataLabelAttrs.ShowBubbleSize:
                    builder.WriteElementWithAttributes(string.Format("{0}:showBubbleSize", prefix), "val", value);
                    break;
                case DmlChartDataLabelAttrs.ShowCatName:
                    builder.WriteElementWithAttributes(string.Format("{0}:showCatName", prefix), "val", value);
                    break;
                case DmlChartDataLabelAttrs.ShowLegendKey:
                    builder.WriteElementWithAttributes(string.Format("{0}:showLegendKey", prefix), "val", value);
                    break;
                case DmlChartDataLabelAttrs.ShowPercent:
                    builder.WriteElementWithAttributes(string.Format("{0}:showPercent", prefix), "val", value);
                    break;
                case DmlChartDataLabelAttrs.ShowSerName:
                    builder.WriteElementWithAttributes(string.Format("{0}:showSerName", prefix), "val", value);
                    break;
                case DmlChartDataLabelAttrs.ShowVal:
                    builder.WriteElementWithAttributes(string.Format("{0}:showVal", prefix), "val", value);
                    break;
                case DmlChartDataLabelAttrs.ShowLeaderLines:
                    builder.WriteElementWithAttributes(string.Format("{0}:showLeaderLines", prefix), "val", value);
                    break;
                case DmlChartDataLabelAttrs.ShowDataLabelsRange:
                    builder.WriteElementWithAttributes(string.Format("{0}:showDataLabelsRange", prefix), "val", value);
                    break;
                case DmlChartDataLabelAttrs.SpPr:
                    WriteShapeProperties((DmlChartSpPr)value, writer, isChartEx, prefix);
                    break;
                case DmlChartDataLabelAttrs.ShapePr:
                    DmlShapePropertiesWriter.Write(prefix, (DmlChartShapeProperties)value, writer);
                    break;
                case DmlChartDataLabelAttrs.Tx:
                    WriteTx((DmlChartTx)value, writer, isChartEx);
                    break;
                case DmlChartDataLabelAttrs.TxPr:
                    WriteTextProperties((DmlChartTxPr)value, writer, isChartEx);
                    break;
                case DmlChartDataLabelAttrs.LeaderLines:
                    WriteWrappedSpPr(string.Format("{0}:leaderLines", prefix), (DmlChartSpPr)value, writer, false,
                        isChartEx);
                    break;
                case DmlChartDataLabelAttrs.XForSave:
                    builder.WriteElementWithAttributes(string.Format("{0}:xForSave", prefix), "val", value);
                    break;
                case DmlChartDataLabelAttrs.FieldTable:
                {
                    // The property value is currently represented as list of raw-XML entries.
                    List<string> entries = (List<string>)value;
                    WriteDataLabelFieldTable(prefix, entries, writer.Builder);
                    break;
                }
                case DmlChartDataLabelAttrs.Extensions:
                    DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), (StringToObjDictionary<DmlExtension>)value, writer);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Writes the [MS-ODRAWXML] 2.6.1.4 dlblFieldTable element.
        /// </summary>
        private static void WriteDataLabelFieldTable(string prefix, List<string> entries, NrxXmlBuilder builder)
        {
            if ((entries == null) || (entries.Count == 0))
                return;

            string elementName = string.Format("{0}:dlblFieldTable", prefix);
            builder.StartElement(elementName);

            // The entries are currently represented as raw-XML strings.
            foreach (string entry in entries)
                builder.WriteRaw(entry);

            builder.EndElement(elementName);
        }

        private static void WriteDataPointAttr(DmlChartDataPointPr pr, DmlChartDataPointAttr attr,
            IDmlShapeWriterContext writer, bool isChartEx)
        {
            object val = pr.GetDirectProperty(attr);
            if (val == null)
                return;

            NrxXmlBuilder builder = writer.Builder;
            switch (attr)
            {
                case DmlChartDataPointAttr.Index:
                    builder.WriteElementWithAttributes(GetTagName("idx", isChartEx), "val", val);
                    break;
                case DmlChartDataPointAttr.Explosion:
                    builder.WriteElementWithAttributes(GetTagName("explosion", isChartEx), "val", val);
                    break;
                case DmlChartDataPointAttr.InvertIfNegative:
                    builder.WriteElementWithAttributes(GetTagName("invertIfNegative", isChartEx), "val", val);
                    break;
                case DmlChartDataPointAttr.Bubble3D:
                    builder.WriteElementWithAttributes(GetTagName("bubble3D", isChartEx), "val", val);
                    break;
                case DmlChartDataPointAttr.Marker:
                    WriteMarker((ChartMarker)val, writer, isChartEx);
                    break;
                case DmlChartDataPointAttr.SpPr:
                    WriteShapeProperties((DmlChartSpPr)val, writer, isChartEx);
                    break;
                case DmlChartDataPointAttr.PictureOptions:
                    WritePictureOptions((DmlChartPictureOptions)val, builder, isChartEx);
                    break;
                case DmlChartDataPointAttr.Extensions:
                    DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), (StringToObjDictionary<DmlExtension>)val, writer);
                    break;
                default:
                    break;
            }
        }

        private static void WriteMarkerAttr(DmlChartMarkerPr pr, DmlChartMarkerAttr attr, IDmlShapeWriterContext writer,
            bool isChartEx)
        {
            object val = pr.GetDirectProperty(attr);
            if (val == null)
                return;

            NrxXmlBuilder builder = writer.Builder;
            switch (attr)
            {
                case DmlChartMarkerAttr.Symbol:
                    builder.WriteElementWithAttributes(GetTagName("symbol", isChartEx), "val",
                        DmlChartsEnum.MarkerStyleToDml((MarkerSymbol)val));
                    break;
                case DmlChartMarkerAttr.Size:
                    builder.WriteElementWithAttributes(GetTagName("size", isChartEx), "val", val);
                    break;
                case DmlChartMarkerAttr.SpPr:
                    WriteShapeProperties((DmlChartSpPr)val, writer, isChartEx);
                    break;
                case DmlChartMarkerAttr.Extensions:
                    DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), (StringToObjDictionary<DmlExtension>)val, writer);
                    break;
                default:
                    break;
            }
        }

        private static void WriteTrendlineArrt(DmlChartTrendlinePr pr, DmlChartTrendlineAttr attr,
            IDmlShapeWriterContext writer, bool isChartEx)
        {
            object val = pr.GetDirectProperty(attr);
            if (val == null)
                return;

            NrxXmlBuilder builder = writer.Builder;
            switch (attr)
            {
                case DmlChartTrendlineAttr.Backward:
                    builder.WriteElementWithAttributes(GetTagName("backward", isChartEx), "val", val);
                    break;
                case DmlChartTrendlineAttr.DispEq:
                    builder.WriteElementWithAttributes(GetTagName("dispEq", isChartEx), "val", val);
                    break;
                case DmlChartTrendlineAttr.DispRSqr:
                    builder.WriteElementWithAttributes(GetTagName("dispRSqr", isChartEx), "val", val);
                    break;
                case DmlChartTrendlineAttr.Forward:
                    builder.WriteElementWithAttributes(GetTagName("forward", isChartEx), "val", val);
                    break;
                case DmlChartTrendlineAttr.Intercept:
                    builder.WriteElementWithAttributes(GetTagName("intercept", isChartEx), "val", val);
                    break;
                case DmlChartTrendlineAttr.Name:
                    builder.WriteElement(GetTagName("name", isChartEx), (string)val);
                    break;
                case DmlChartTrendlineAttr.Order:
                    builder.WriteElementWithAttributes(GetTagName("order", isChartEx), "val", val);
                    break;
                case DmlChartTrendlineAttr.Period:
                    builder.WriteElementWithAttributes(GetTagName("period", isChartEx), "val", val);
                    break;
                case DmlChartTrendlineAttr.SpPr:
                    WriteShapeProperties((DmlChartSpPr)val, writer, isChartEx);
                    break;
                case DmlChartTrendlineAttr.TrendlineLbl:
                    WriteTrendlineLbl((DmlChartTrendlineLabel)val, writer, isChartEx);
                    break;
                case DmlChartTrendlineAttr.TrendlineType:
                    builder.WriteElementWithAttributes(GetTagName("trendlineType", isChartEx), "val",
                        DmlChartsEnum.TrendlineTypeToDml((TrendlineType)val));
                    break;
                case DmlChartTrendlineAttr.Extensions:
                    DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), (StringToObjDictionary<DmlExtension>)val, writer);
                    break;
                default:
                    break;
            }
        }

        private static void WriteTrendlineLbl(DmlChartTrendlineLabel lbl, IDmlShapeWriterContext writer, bool isChartEx)
        {
            if (lbl == null)
                return;

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(GetTagName("trendlineLbl", isChartEx));
            WriteManualLayout(GetTagPrefix(isChartEx), lbl.Layout, writer, isChartEx);
            WriteNumberFormat(lbl.NumFmt, builder, isChartEx);
            WriteShapeProperties(lbl.SpPr, writer, isChartEx);
            WriteTx(lbl.Tx, writer, isChartEx);
            WriteTextProperties(lbl.TxPr, writer, isChartEx);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), lbl.Extensions, writer);
            builder.EndElement(GetTagName("trendlineLbl", isChartEx));
        }

        private static void WriteDoubleIfNotDefault(string tagName, double value, double defaultValue,
            NrxXmlBuilder builder)
        {
            if (!MathUtil.AreEqual(value, defaultValue))
                builder.WriteElementWithAttributes(tagName, "val", value);
        }

        private static void WriteDisplayUnitsLabel(DmlChartTitle dispUnitsLbl, IDmlShapeWriterContext writer,
            bool isChartEx)
        {
            if (dispUnitsLbl == null)
                return;

            NrxXmlBuilder builder = writer.Builder;
            string elementName = isChartEx ? "unitsLabel" : "dispUnitsLbl";

            builder.StartElement(GetTagName(elementName, isChartEx));

            if (!isChartEx)
                WriteManualLayout(GetTagPrefix(isChartEx), dispUnitsLbl.Layout, writer, isChartEx);

            WriteShapeProperties(dispUnitsLbl.SpPr, writer, isChartEx);
            WriteTextProperties(dispUnitsLbl.TxPr, writer, isChartEx);
            WriteTx(dispUnitsLbl.Tx, writer, isChartEx);

            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx),
                ((IDmlExtensionListSource)dispUnitsLbl).Extensions, writer);

            builder.EndElement(GetTagName(elementName, isChartEx));
        }

        /// <summary>
        /// Writes the data element of the 2.24.3.15 CT_Data complex type [MS-ODRAWXML].
        /// The element specifies the number dimension and string dimension for the specified chart data.
        /// </summary>
        internal static void WriteData(DmlChartData data, IDmlShapeWriterContext writer, bool isChartEx)
        {
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("data", isChartEx));
            builder.WriteAttribute("id", data.Id);

            foreach (DmlChartDataSource dataSource in data.DataSources)
                WriteValueRef(dataSource.ValueRef, writer, true);

            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), data.Extensions, writer);

            builder.EndElement(GetTagName("data", isChartEx));
        }

        /// <summary>
        /// Writes the fmtOvr element of the 2.24.3.26 CT_FormatOverride complex type [MS-ODRAWXML].
        /// </summary>
        internal static void WriteFormatOverride(DmlChartFormatOverride formatOverride, IDmlShapeWriterContext writer,
            bool isChartEx)
        {
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("fmtOvr", isChartEx));
            builder.WriteAttribute("idx", formatOverride.Index);

            if (formatOverride.SpPr != null)
                WriteShapeProperties(formatOverride.SpPr, writer, isChartEx);

            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), formatOverride.Extensions, writer);

            builder.EndElement(GetTagName("fmtOvr", isChartEx));
        }

        /// <summary>
        /// Writes an element of the CT_TickMark complex type [ISO/IEC 29500] or the 2.24.3.81 CT_TickMarks
        /// complex type [MS-ODRAWXML].
        /// </summary>
        internal static void WriteTickMarks(string tagName, AxisTickMark tickMark,
            StringToObjDictionary<DmlExtension> extensions,
            IDmlShapeWriterContext writer, bool isChartEx)
        {
            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName(tagName, isChartEx));

            builder.WriteAttribute(isChartEx ? "type" : "val", DmlChartsEnum.TickMarkToDml(tickMark));
            if (isChartEx)
                DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), extensions, writer);

            builder.EndElement(GetTagName(tagName, isChartEx));
        }

        /// <summary>
        /// Writes the valueColors element of the 2.24.3.86 CT_ValueColors complex type [MS-ODRAWXML].
        /// </summary>
        internal static void WriteValueColors(DmlChartValueColors colors, IDmlShapeWriterContext writer)
        {
            if ((colors == null) || colors.IsEmpty)
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("valueColors", true));

            WriteValueColor("minColor", colors.MinimumColor, writer);
            WriteValueColor("midColor", colors.MiddleColor, writer);
            WriteValueColor("maxColor", colors.MaximumColor, writer);

            builder.EndElement(GetTagName("valueColors", true));
        }

        /// <summary>
        /// Writes a sub-element of the valueColors element of the 2.24.3.86 CT_ValueColors complex type [MS-ODRAWXML].
        /// </summary>
        internal static void WriteValueColor(string tagName, DmlColor color, IDmlShapeWriterContext writer)
        {
            if (color == null)
                return;

            writer.Builder.StartElement(GetTagName(tagName, true));
            DmlColorWriter.Write(color, writer);
            writer.Builder.EndElement(GetTagName(tagName, true));
        }

        /// <summary>
        /// Writes the layoutPr element of the 2.24.3.72 CT_SeriesLayoutProperties complex type [MS-ODRAWXML].
        /// </summary>
        internal static void WriteLayoutProperties(DmlChartSeriesLayoutPr layoutPr, IDmlShapeWriterContext writer)
        {
            if (layoutPr.Count == 0)
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("layoutPr", true));

            object value = layoutPr.GetDirectProperty(DmlChartSeriesLayoutAttr.ParentLabelLayout);
            if (value != null)
            {
                builder.WriteElementWithAttributes(GetTagName("parentLabelLayout", true),
                    "val", DmlChartsEnum.ParentLabelLayoutToDml((ParentLabelLayout)value));
            }
            value = layoutPr.GetDirectProperty(DmlChartSeriesLayoutAttr.RegionLabelLayout);
            if (value != null)
            {
                builder.WriteElementWithAttributes(GetTagName("regionLabelLayout", true),
                    "val", DmlChartsEnum.RegionLabelLayoutToDml((RegionLabelLayout)value));
            }
            builder.WriteElementWithAttributes(GetTagName("visibility", true),
                "connectorLines", layoutPr.GetDirectProperty(DmlChartSeriesLayoutAttr.IsConnectorLinesVisible),
                "meanLine", layoutPr.GetDirectProperty(DmlChartSeriesLayoutAttr.IsMeanLineVisible),
                "meanMarker", layoutPr.GetDirectProperty(DmlChartSeriesLayoutAttr.IsMeanMarkerVisible),
                "nonoutliers", layoutPr.GetDirectProperty(DmlChartSeriesLayoutAttr.IsNonOutliersVisible),
                "outliers", layoutPr.GetDirectProperty(DmlChartSeriesLayoutAttr.IsOutliersVisible));
            value = layoutPr.GetDirectProperty(DmlChartSeriesLayoutAttr.IsAggregation);
            if (value != null)
                builder.WriteEmptyElement(GetTagName("aggregation", true));
            WriteBinning((DmlChartBinningPr)layoutPr.GetDirectProperty(DmlChartSeriesLayoutAttr.Binning), writer);
            value = layoutPr.GetDirectProperty(DmlChartSeriesLayoutAttr.QuartileMethod);
            if (value != null)
            {
                builder.WriteElementWithAttributes(GetTagName("statistics", true),
                    "quartileMethod", DmlChartsEnum.QuartileMethodToDml((QuartileMethod)value));
            }
            WriteSubtotals((List<int>)layoutPr.GetDirectProperty(DmlChartSeriesLayoutAttr.Subtotals), builder);
            DmlExtensionListWriter.Write(GetTagPrefix(true),
                (StringToObjDictionary<DmlExtension>)layoutPr.GetDirectProperty(DmlChartSeriesLayoutAttr.Extensions), writer);

            builder.EndElement(GetTagName("layoutPr", true));
        }

        /// <summary>
        /// Writes the binning element of the 2.24.3.7 CT_Binning complex type [MS-ODRAWXML].
        /// </summary>
        private static void WriteBinning(DmlChartBinningPr binningPr, IDmlShapeWriterContext writer)
        {
            if ((binningPr == null) || (binningPr.Count == 0))
                return;

            NrxXmlBuilder builder = writer.Builder;

            builder.StartElement(GetTagName("binning", true));

            object value = binningPr.GetDirectProperty(DmlChartBinningAttr.IntervalClosed);
            if (value != null)
            {
                builder.WriteAttribute("intervalClosed",
                    DmlChartsEnum.IntervalClosedSideToDml((IntervalClosedSide)value));
            }
            WriteDoubleOrAutomaticAttribute("underflow",
                (DoubleOrAutomatic)binningPr.GetDirectProperty(DmlChartBinningAttr.Underflow), builder);
            WriteDoubleOrAutomaticAttribute("overflow",
                (DoubleOrAutomatic)binningPr.GetDirectProperty(DmlChartBinningAttr.Overflow), builder);

            value = binningPr.GetDirectProperty(DmlChartBinningAttr.BinSize);
            if (value != null)
            {
                builder.WriteElementWithAttributes(GetTagName("binSize", true), "val", value);
            }
            else
            {
                value = binningPr.GetDirectProperty(DmlChartBinningAttr.BinCount);
                if (value != null)
                    builder.WriteElementWithAttributes(GetTagName("binCount", true), "val", value);
            }

            builder.EndElement(GetTagName("binning", true));
        }

        /// <summary>
        /// Writes attribute of the ST_DoubleOrAutomatic, ST_ValueAxisUnit, ST_GapWidthRatio simple types [MS-ODRAWXML].
        /// It may have either numeric value or the enumeration value 'auto'.
        /// </summary>
        internal static void WriteDoubleOrAutomaticAttribute(string name, DoubleOrAutomatic value, NrxXmlBuilder builder)
        {
            if (value != null && !value.IsNull)
            {
                if (value.IsAuto)
                    builder.WriteAttribute(name, "auto");
                else
                    builder.WriteAttribute(name, value.Value);
            }
        }

        /// <summary>
        /// Writes axis bound value.
        /// </summary>
        internal static void WriteAxisBound(string name, AxisBound value, NrxXmlBuilder builder)
        {
            if (value.IsAuto)
                builder.WriteAttribute(name, "auto");
            else
                builder.WriteAttribute(name, value.Value);
        }

        /// <summary>
        /// Writes the subtotals element of the 2.24.3.77 CT_Subtotals complex type [MS-ODRAWXML].
        /// </summary>
        private static void WriteSubtotals(List<int> subtotals, NrxXmlBuilder builder)
        {
            if ((subtotals == null) || (subtotals.Count == 0))
                return;

            builder.StartElement(GetTagName("subtotals", true));

            foreach (int index in subtotals)
                builder.WriteElementWithAttributes(GetTagName("idx", true), "val", index);

            builder.EndElement(GetTagName("subtotals", true));
        }

        /// <summary>
        /// Generates a tag name with a prefix depending on chart type.
        /// </summary>
        /// <param name="localName">Name without prefix.</param>
        /// <param name="isChartEx">A flag indicating that the chart is of the chartEx schema.</param>
        /// <returns>Tag name with prefix.</returns>
        internal static string GetTagName(string localName, bool isChartEx)
        {
            return GetTagName(GetTagPrefix(isChartEx), localName);
        }

        /// <summary>
        /// Generates a tag name with a specified prefix.
        /// </summary>
        /// <param name="prefix">A prefix that will be used for the tag.</param>
        /// <param name="localName">Name without prefix.</param>
        /// <returns>Tag name with prefix.</returns>
        internal static string GetTagName(string prefix, [CodePorting.Translator.Cs2Cpp.CppForceStringParam] string localName)
        {
            return string.Format("{0}:{1}", prefix, localName);
        }

        /// <summary>
        /// Gets a tag prefix depending on chart type.
        /// </summary>
        internal static string GetTagPrefix(bool isChartEx)
        {
            return isChartEx ? "cx" : "c";
        }

        private const string Overlay = "overlay";
    }
}
