// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/21/2014 by Alexey Noskov

using System.Collections.Generic;
using System.Drawing;
using Aspose.Common;
using Aspose.IO;
using Aspose.OpcPackaging;
using Aspose.Ss;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;
using Aspose.Words.Themes;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Instance class for writing DrawingML chart spaces.
    /// </summary>
    internal static class DmlChartSpaceWriter
    {
        internal static void Write(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            DmlChartSpace chartSpace = dml.DmlNode as DmlChartSpace;
            if (chartSpace == null)
                return;

            DocxBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            bool isChartEx = chartSpace.IsChartEx;

            builder.StartDocument(GetTagName("chartSpace", isChartEx));
            builder.WriteAttribute("xmlns:" + GetTagPrefix(isChartEx), DocxNamespaces.GetNamespace(
                chartSpace.IsChartEx ? DocxNamespace.DrawingMLChartEx : DocxNamespace.DrawingMLChart, isIsoStrict));
            builder.WriteAttribute("xmlns:a", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, isIsoStrict));
            builder.WriteAttribute("xmlns:r", DocxNamespaces.GetNamespace(DocxNamespace.Relationships, isIsoStrict));

            if (isChartEx)
            {
                WriteChartData(chartSpace, writer);
            }
            else
            {
                builder.WriteElementWithAttributes(GetTagName("date1904", isChartEx), "val", chartSpace.Date1904);
                builder.WriteElementWithAttributes(GetTagName("lang", isChartEx), "val", chartSpace.Lang);
                builder.WriteElementWithAttributes(GetTagName("roundedCorners", isChartEx), "val", chartSpace.RoundedCorners);
                WriteStyle(chartSpace, builder);
                WriteColormapOverride(chartSpace.ColorMapOverride, writer, isChartEx);
                DmlChartCommonWriter.WritePivotSource(chartSpace.PivotSource, writer, isChartEx);
                DmlChartCommonWriter.WriteProtection(chartSpace.Protection, builder, isChartEx);
            }
            
            WriteChart(chartSpace.ChartFormat, writer, isChartEx);
            DmlChartCommonWriter.WriteShapeProperties(chartSpace.SpPr, writer, isChartEx);
            DmlChartCommonWriter.WriteTextProperties(chartSpace.TxPr, writer, isChartEx);

            if (isChartEx)
            {
                WriteColormapOverride(chartSpace.ColorMapOverride, writer, isChartEx);
                WriteFormatOverrides(chartSpace, writer);
            }
            else
            {
                WriteExternalData(chartSpace, writer);
            }

            WriteUserShapes(dml, writer, isChartEx);
            WriteThemeOverride(chartSpace.ThemeOverride, writer);
            WriteColorStyle(chartSpace.ColorStyle, writer);
            WriteStyle(chartSpace.DmlChartStyle, writer);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), chartSpace.Extensions, writer);

            builder.EndDocument();
        }

        private static void WriteStyle(DmlChartStyle style, DocxDocumentWriterBase writer)
        {
            if (style == null)
                return;

            string contentType = DocxContentType.ChartStyle;
            string relType = DocxRelationshipTypes.GetType(DocxRelationshipType.ChartStyle, 
                writer.Compliance == OoxmlComplianceCore.IsoStrict);
            string partName = string.Format("chart/style{0}.xml", writer.GetNextEmbeddedPartNumber(relType));
            string dummyRelId;
            DocxBuilder builder = writer.CreateChildPartAndBuilder(writer.CurrentBuilder.Part, partName, contentType,
                relType, out dummyRelId);

            writer.PushBuilder(builder);
            DmlChartStyleWriter.Write(style, writer);
            writer.PopBuilder();
        }

        private static void WriteColorStyle(DmlChartColorStyle colorStyle, DocxDocumentWriterBase writer)
        {
            if (colorStyle == null)
                return;

            string contentType = DocxContentType.ChartColorStyle;
            string relType = DocxRelationshipTypes.GetType(DocxRelationshipType.ChartColorStyle,
                writer.Compliance == OoxmlComplianceCore.IsoStrict);
            string partName = string.Format("chart/colors{0}.xml", writer.GetNextEmbeddedPartNumber(relType));
            string dummyRelId;
            DocxBuilder builder = writer.CreateChildPartAndBuilder(writer.CurrentBuilder.Part, partName, contentType,
                relType, out dummyRelId);

            writer.PushBuilder(builder);
            DmlChartColorStyleWriter.Write(colorStyle, writer);
            writer.PopBuilder();
        }

        private static void WriteStyle(DmlChartSpace chartSpace, DocxBuilder builder)
        {
            bool isChartEx = chartSpace.IsChartEx; 
            if (chartSpace.Word2010Style < 0)
            {
                // 2 is default style, MS Word does not write it.
                if (chartSpace.StyleIndex != 2)
                    builder.WriteElementWithAttributes(GetTagName("style", isChartEx), "val", chartSpace.StyleIndex);
            }
            else
            {
                bool isIsoStrict = builder.OoxmlCompliance == OoxmlComplianceCore.IsoStrict;

                // In MS Word 2013 AlternateContent is written for style.
                builder.StartElement("mc:AlternateContent");
                builder.WriteAttribute("xmlns:mc", 
                    DocxNamespaces.GetNamespace(DocxNamespace.MarkupCompatibility, isIsoStrict));

                builder.StartElement("mc:Choice");
                builder.WriteAttribute("Requires", "c14");
                builder.WriteAttribute("xmlns:c14", 
                    DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChart2007, isIsoStrict));
                builder.WriteElementWithAttributes("c14:style", "val", chartSpace.Word2010Style);
                builder.EndElement("mc:Choice");

                builder.StartElement("mc:Fallback");
                builder.WriteElementWithAttributes(GetTagName("style", isChartEx), "val", chartSpace.StyleIndex);
                builder.EndElement("mc:Fallback");

                builder.EndElement("mc:AlternateContent");
            }
        }

        private static void WriteColormapOverride(DmlChartColorMapOverride colorMapOverride, 
            DocxDocumentWriterBase writer, bool isChartEx)
        {
            if(colorMapOverride == null)
                return;

            DocxBuilder builder = writer.CurrentBuilder;
            builder.WriteElementWithAttributes(GetTagName("clrMapOvr", isChartEx),
                "bg1", colorMapOverride.Bg1,
                "tx1", colorMapOverride.Tx1,
                "bg2", colorMapOverride.Bg2,
                "tx2", colorMapOverride.Tx2,
                "accent1", colorMapOverride.Accent1,
                "accent2", colorMapOverride.Accent2,
                "accent3", colorMapOverride.Accent3,
                "accent4", colorMapOverride.Accent4,
                "accent5", colorMapOverride.Accent5,
                "accent6", colorMapOverride.Accent6,
                "hlink", colorMapOverride.Hlink,
                "folHlink", colorMapOverride.FolHlink);
        }

        private static void WriteThemeOverride(Theme themeOverride, DocxDocumentWriterBase writer)
        {
            if (themeOverride == null)
                return;

            DocxDocumentWriter.WriteTheme(themeOverride, writer, true);
        }

        private static void WriteChart(DmlChartFormat chartFormat, DocxDocumentWriterBase writer, bool isChartEx)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(GetTagName("chart", isChartEx));
            DmlChartCommonWriter.WriteTitle(chartFormat.DCTitle, writer, chartFormat.AutoTitleDeleted, isChartEx, false);
            if (!isChartEx)
            {
                builder.WriteElementWithAttributes(GetTagName("autoTitleDeleted", isChartEx), "val", chartFormat.AutoTitleDeleted);
                DmlChartCommonWriter.WritePivotFmts(chartFormat.PivotFmts, writer, isChartEx);
                DmlChartCommonWriter.WriteView3D(chartFormat, writer, isChartEx);
                DmlChartCommonWriter.WriteSurface(GetTagName("floor", isChartEx), chartFormat.Floor, writer, isChartEx);
                DmlChartCommonWriter.WriteSurface(GetTagName("sideWall", isChartEx), chartFormat.SideWall, writer, isChartEx);
                DmlChartCommonWriter.WriteSurface(GetTagName("backWall", isChartEx), chartFormat.BackWall, writer, isChartEx);
            }

            WritePlotArea(chartFormat.PlotArea, writer, isChartEx);
            DmlChartCommonWriter.WriteLegend(chartFormat.Legend, writer, isChartEx);

            if (!isChartEx)
            {
                DmlChartCommonWriter.WriteWrappedBooleanIfTrue(GetTagName("plotVisOnly", isChartEx), chartFormat.PlotVisOnly, builder);
                builder.WriteElementWithAttributes(GetTagName("dispBlanksAs", isChartEx), "val", DmlChartsEnum.DisplayBlanksAsToDml(chartFormat.DispBlanksAs));
                builder.WriteElementWithAttributes(GetTagName("showDLblsOverMax", isChartEx), "val", chartFormat.ShowDLblsOverMax);
            }

            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), chartFormat.Extensions, writer);
            
            builder.EndElement(GetTagName("chart", isChartEx));
        }

        private static void WritePlotArea(DmlChartPlotArea plotArea, DocxDocumentWriterBase writer, bool isChartEx)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(GetTagName("plotArea", isChartEx));
            if (!isChartEx)
            {
                DmlChartCommonWriter.WriteManualLayout(GetTagPrefix(isChartEx), plotArea.Layout, writer, isChartEx);

                foreach (DmlChart chart in plotArea.Charts)
                    DmlChartObjectWriter.Write(chart, writer, isChartEx);
            }
            else
            {
                WritePlotAreaRegion(plotArea, writer);
            }

            // It looks like MS Word cannot open a document with a non-combo pre-Word 2016 chart that has secondary axes:
            // do not write them.
            bool removeUnusedAxes = !isChartEx && (plotArea.Charts.Count == 1);

            foreach (ChartAxis axis in plotArea.Axes)
            {
                if (removeUnusedAxes && !IsAxisUsed(axis, plotArea.Charts))
                    continue;

                if (isChartEx)
                    DmlChartAxisWriter.WriteChartExAxis(axis, writer);
                else
                    DmlChartAxisWriter.Write(axis, writer);
            }

            if (!isChartEx)
                WriteDataTable(plotArea.DataTable, writer, isChartEx);

            DmlChartCommonWriter.WriteShapeProperties(plotArea.SpPr, writer, isChartEx);

            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), plotArea.Extensions, writer);

            builder.EndElement(GetTagName("plotArea", isChartEx));
        }

        /// <summary>
        /// Returns a flag indicating whether the specified axis is used by DML charts of the chart being written.
        /// </summary>
        private static bool IsAxisUsed(ChartAxis axis, IList<DmlChart> charts)
        {
            foreach (DmlChart chart in charts)
            {
                IDmlChart2D chart2D = chart as IDmlChart2D;
                if ((chart2D != null) && ((chart2D.AxIdX == axis.AxId) || (chart2D.AxIdY == axis.AxId)))
                    return true;

                IDmlChart3D chart3D = chart as IDmlChart3D;
                if ((chart3D != null) && (chart3D.AxIdZ == axis.AxId))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Writes the plotAreaRegion of 2.24.3.65 CT_PlotAreaRegion complex type [MS-ODRAWXML].
        /// </summary>
        private static void WritePlotAreaRegion(DmlChartPlotArea plotArea, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(GetTagName("plotAreaRegion", true));

            DmlChartCommonWriter.WriteSurface(GetTagName("plotSurface", true), plotArea.Surface, writer, true);

            // plotArea.FirstChart: plot area of a chartEx chart may contain only one chart.
            DmlChartSeriesWriter.Write(plotArea.FirstChart, writer); 

            DmlExtensionListWriter.Write(GetTagPrefix(true), plotArea.RegionExtensions, writer);

            builder.EndElement(GetTagName("plotAreaRegion", true));
        }

        private static void WriteDataTable(ChartDataTable dt, DocxDocumentWriterBase writer, bool isChartEx)
        {
            if ((dt == null) || !dt.Show)
                return;

            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(GetTagName("dTable", isChartEx));
            builder.WriteElementWithAttributes(GetTagName("showHorzBorder", isChartEx), "val", dt.HasHorizontalBorder);
            builder.WriteElementWithAttributes(GetTagName("showVertBorder", isChartEx), "val", dt.HasVerticalBorder);
            builder.WriteElementWithAttributes(GetTagName("showOutline", isChartEx), "val", dt.HasOutlineBorder);
            builder.WriteElementWithAttributes(GetTagName("showKeys", isChartEx), "val", dt.HasLegendKeys);
            DmlChartCommonWriter.WriteShapeProperties(dt.SpPr, writer, isChartEx);
            DmlChartCommonWriter.WriteTextProperties(dt.TxPr, writer, isChartEx);

            builder.EndElement(GetTagName("dTable", isChartEx));
        }

        private static void WriteExternalData(DmlChartSpace chartSpace, DocxDocumentWriterBase writer)
        {
            // For now write only Ooxml embedded objects here. I never saw other types of data for charts.
            // So I think this is acceptable.
            string relId = null;
            bool isChartEx = chartSpace.IsChartEx;
            DocxBuilder builder = writer.CurrentBuilder;

            if (chartSpace.EmbeddedData != null)
            {
                OoxmlObject ooxmlObject = chartSpace.EmbeddedData as OoxmlObject;
                bool isOoxmlObject = (ooxmlObject != null);

                string contentType = isOoxmlObject ? ooxmlObject.ContentType : DocxContentType.OleObject;
                string relType = isOoxmlObject ? writer.RelTypes.Package : writer.RelTypes.OleObject;              
 
                string partNameFormat = isOoxmlObject 
                    ? "/word/embeddings/Microsoft_Excel_Worksheet{0}{1}" 
                    : "/word/embeddings/OleObject{0}{1}";

                // WORDSNET-14967 Preserve extension of the embedded object while re-saving the document.
                string extension = DocxEnum.ContentTypeToExtension(contentType);

                // Sometimes content type for embedded data is octet stream, but MS Word still writes xlsx extension.
                if (contentType == DocxContentType.OctetStream)
                    extension = DocxEnum.ContentTypeToExtension(DocxContentType.Xlsx);

                string partName = string.Format(partNameFormat, writer.GetNextEmbeddedPartNumber(relType), extension);                
                OpcPackagePart part = writer.Package.CreateChildPart(builder.Part, partName, contentType, relType, out relId);

                if (isOoxmlObject)
                {
                    ooxmlObject.Stream.Position = 0;
                    StreamUtil.CopyStream(ooxmlObject.Stream, part.Stream);
                }
                else
                {
                    FileSystem fs = new FileSystem(((OleObject)chartSpace.EmbeddedData).Data);
                    fs.Save(part.Stream);
                }
            }
            else if (StringUtil.HasChars(chartSpace.LinkedData))
            {
                relId = writer.AddExternalOleObjectRelationship(chartSpace.LinkedData);
            }

            if (StringUtil.HasChars(relId))
            {
                builder.StartElement(GetTagName("externalData", isChartEx));
                // MS Word writes the id attribute with the Transitional OOXML relationships namespace on saving
                // a chartEx chart to Strict OOXML: let's do the same.
                if ((writer.Compliance == OoxmlComplianceCore.IsoStrict) && isChartEx)
                {
                    builder.WriteAttribute("xmlns:p5", DocxNamespaces.GetNamespace(DocxNamespace.Relationships, false));
                    builder.WriteAttribute("p5:id", relId);
                }
                else
                {
                    builder.WriteAttribute("r:id", relId);
                }
                if (!chartSpace.AutoUpdate)
                {
                    if (isChartEx)
                        builder.WriteAttribute(GetTagName("autoUpdate", true), chartSpace.AutoUpdate);
                    else
                        builder.WriteElementWithAttributes(GetTagName("autoUpdate", false), "val", chartSpace.AutoUpdate);
                }
                builder.EndElement(GetTagName("externalData", isChartEx));
            }
        }

        private static void WriteUserShapes(ShapeBase dml, DocxDocumentWriterBase writer, bool isChartEx)
        {
            if (dml.GetChildNodes(NodeType.Any, false).Count == 0)
                return;

            string chartDrawingRelType = DocxRelationshipTypes.GetType(DocxRelationshipType.ChartDrawing,
                writer.Compliance == OoxmlComplianceCore.IsoStrict);
            int chartDrawingPartNumber = writer.GetNextEmbeddedPartNumber(chartDrawingRelType);
            string chartDrawingPartName = string.Format("/word/drawings/drawing{0}.xml", chartDrawingPartNumber);

            string relId;
            DocxBuilder userShapesBuilder = writer.CreateChildPartAndBuilder(
                writer.CurrentBuilder.Part,
                chartDrawingPartName, 
                DocxContentType.ChartDrawing, 
                chartDrawingRelType, 
                out relId);

            writer.CurrentBuilder.WriteElementWithAttributes(GetTagName("userShapes", isChartEx), "r:id", relId);
            
            writer.PushBuilder(userShapesBuilder);
            WriteUserShapesCore(dml, writer);
            writer.PopBuilder();
        }

        private static void WriteUserShapesCore(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartDocument("c:userShapes");
            builder.WriteAttribute("xmlns:c", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChart, isIsoStrict));
            builder.WriteAttribute("xmlns:a", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, isIsoStrict));
            foreach (ShapeBase shape in dml.GetChildNodes(NodeType.Any, false))
                WriteUserShapeRelative(shape, writer);

            // Here we must actually write user shapes.
            builder.EndDocument();
        }

        private static void WriteUserShapeRelative(ShapeBase shape, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            string tagName = (shape.AnchorType == DmlChartUserShapeAnchorType.Absolute)
                ? "cdr:absSizeAnchor"
                : "cdr:relSizeAnchor";

            builder.StartElement(tagName);
            builder.WriteAttribute("xmlns:cdr", 
                DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartDrawing, isIsoStrict));

            WritePoint("cdr:from", shape.From, builder);
            WritePoint("cdr:to", shape.To, builder);

            WriteShapeCore(shape, writer);

            builder.EndElement(tagName);
        }

        private static void WriteShapeCore(ShapeBase shape, DocxDocumentWriterBase writer)
        {
            DmlNode dmlNode = shape.DmlNode;
            switch (dmlNode.DmlNodeType)
            {
                case DmlNodeType.ConnectorShape:
                    WriteConnectorShape((DmlShape)dmlNode, writer);
                    break;
                case DmlNodeType.Shape:
                    WriteShape((DmlShape)dmlNode, writer);
                    break;
                case DmlNodeType.Picture:
                    WritePictureShape((DmlPicture)dmlNode, writer);
                    break;
                case DmlNodeType.GroupShape:
                    WriteGroupShape(shape, writer);
                    break;
                default:
                    break;
            }
        }

        private static void WriteConnectorShape(DmlShape cxnShape, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("cdr:cxnSp");
            DmlNonVisualPropertiesWriter.WriteNvPr("cdr", cxnShape, writer);
            DmlShapePropertiesWriter.Write("cdr", cxnShape, writer);
            DmlShapeStyleWriter.Write("cdr", cxnShape.Style, writer);
            builder.EndElement("cdr:cxnSp");
        }

        private static void WriteShape(DmlShape shape, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("cdr:sp");
            DmlNonVisualPropertiesWriter.WriteNvPr("cdr", shape, writer);
            DmlShapePropertiesWriter.Write("cdr", shape, writer);
            DmlShapeStyleWriter.Write("cdr", shape.Style, writer);

            if (shape.TextShape != null)
                DmlTextShapeWriter.WriteDmlShapeTextBody("cdr:txBody", shape.TextShape.TextBody, writer);
            
            builder.EndElement("cdr:sp");
        }

        private static void WritePictureShape(DmlPicture pic, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("cdr:pic");
            DmlNonVisualPropertiesWriter.WriteNvPr("cdr", pic, writer);
            DmlFillWriter.WriteDmlBlipFill("cdr", pic.BlipFill, writer);
            DmlShapePropertiesWriter.Write("cdr", pic, writer);
            DmlShapeStyleWriter.Write("cdr", pic.Style, writer);
            builder.EndElement("cdr:pic");
        }

        private static void WriteGroupShape(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            DmlNode grpSp = dml.DmlNode;
            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement("cdr:grpSp");
            DmlNonVisualPropertiesWriter.WriteNvPr("cdr", grpSp, writer);
            DmlGroupShapeWriter.WriteGroupShapeProperties("cdr", (DmlCompositeNode)grpSp, writer);

            foreach (ShapeBase node in dml.GetChildNodes(NodeType.Any, false))
                WriteShapeCore(node, writer);
            
            builder.EndElement("cdr:grpSp");
        }

        private static void WritePoint(string tagName, PointF point, DocxBuilder builder)
        {
            builder.StartElement(tagName);
            builder.WriteElement("cdr:x", FormatterPal.DoubleToStrNDecimals(point.X, 5));
            builder.WriteElement("cdr:y", FormatterPal.DoubleToStrNDecimals(point.Y, 5));
            builder.EndElement(tagName);
        }

        /// <summary>
        /// Writes the chartData element of the 2.24.3.10 CT_ChartData complex type [MS-ODRAWXML].
        /// The element specifies the data for the chart.
        /// </summary>
        internal static void WriteChartData(DmlChartSpace chartSpace, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            bool isChartEx = chartSpace.IsChartEx;

            builder.StartElement(GetTagName("chartData", isChartEx));

            WriteExternalData(chartSpace, writer);
            foreach (DmlChartData dataItem in chartSpace.Data)
                DmlChartCommonWriter.WriteData(dataItem, writer, isChartEx);
            DmlExtensionListWriter.Write(GetTagPrefix(isChartEx), chartSpace.Data.Extensions, writer);

            builder.EndElement(GetTagName("chartData", isChartEx));
        }

        /// <summary>
        /// Writes the fmtOvrs element of the 2.24.3.27 CT_FormatOverrides complex type [MS-ODRAWXML].
        /// </summary>
        internal static void WriteFormatOverrides(DmlChartSpace chartSpace, DocxDocumentWriterBase writer)
        {
            if (chartSpace.FormatOverrides.Count == 0)
                return;

            DocxBuilder builder = writer.CurrentBuilder;
            bool isChartEx = chartSpace.IsChartEx;

            builder.StartElement(GetTagName("fmtOvrs", isChartEx));

            foreach (DmlChartFormatOverride item in chartSpace.FormatOverrides)
                DmlChartCommonWriter.WriteFormatOverride(item, writer, isChartEx);

            builder.EndElement(GetTagName("fmtOvrs", isChartEx));
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
