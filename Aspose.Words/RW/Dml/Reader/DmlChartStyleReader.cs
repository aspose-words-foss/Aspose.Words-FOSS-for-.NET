// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/05/2016 by Alexander Zhiltsov

using System;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Words.Themes;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// This class allows reading a style part of a chart.
    /// </summary>
    internal class DmlChartStyleReader : DmlReaderBase
    {
        // Cannot create objects of this class.
        private DmlChartStyleReader()
        {
        }

        /// <summary>
        /// Reads a <see cref="DmlChartStyle"/> object from the element 2.8.1.1 chartStyle [MS-ODRAWXML] that is
        /// a root element of the word/charts/styleX.xml part.
        /// </summary>
        internal static DmlChartStyle Read(DocxDocumentReaderBase reader)
        {
            if (reader.XmlReader.LocalName != "chartStyle")
                return null;

            DmlChartStyle style = new DmlChartStyle();

            ReadAttributes(reader.XmlReader, style);
            ReadChildren(reader, style);

            return style;
        }

        /// <summary>
        /// Reads attributes of the root chartStyle element.
        /// </summary>
        private static void ReadAttributes(NrxXmlReader xmlReader, DmlChartStyle style)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "id":
                        style.Id = xmlReader.Value;
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }
            xmlReader.MoveToElement();
        }

        /// <summary>
        /// Reads child elements of the root chartStyle element.
        /// </summary>
        private static void ReadChildren(DocxDocumentReaderBase reader, DmlChartStyle style)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("chartStyle"))
            {
                switch (xmlReader.LocalName)
                {
                    case "dataPointMarkerLayout":
                        style.DataPointMarkerLayout = ReadMarkerLayout(xmlReader);
                        break;
                    case "extLst":
                        style.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        {
                            if (gElementToStyleItemMap.ContainsKey(xmlReader.LocalName))
                            {
                                style[(DmlChartStyleItem)gElementToStyleItemMap[xmlReader.LocalName]] =
                                    ReadStyleEntry(reader);
                            }
                            else
                            {
                                WarnUnexpectedAndIgnoreElement(xmlReader);
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Reads an element of the 2.8.3.7 CT_StyleEntry complex type.
        /// </summary>
        private static DmlChartStyleEntry ReadStyleEntry(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlChartStyleEntry styleEntry = new DmlChartStyleEntry();
            string tagName = xmlReader.LocalName;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "mods":
                        styleEntry.Modifiers = xmlReader.Value.Split(' ');
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }

            xmlReader.MoveToElement();
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "lnRef":
                        styleEntry.LineReference = ReadStyleReference(reader);
                        break;
                    case "lineWidthScale":
                        double value = FormatterPal.TryParseDoubleInvariant(xmlReader.ReadString());
                        if (!Double.IsNaN(value))
                            styleEntry.LineWidthScale = value;
                        break;
                    case "fillRef":
                        styleEntry.FillReference = ReadStyleReference(reader);
                        break;
                    case "effectRef":
                        styleEntry.EffectReference = ReadStyleReference(reader);
                        break;
                    case "fontRef":
                        styleEntry.FontReference = ReadFontReference(reader);
                        break;
                    case "spPr":
                        {
                            styleEntry.ShapePr = new DefaultShapeProperties();
                            DmlNodePropertiesReader.ReadShapeProperties(reader, styleEntry.ShapePr);
                            break;
                        }
                    case "defRPr":
                        {
                            styleEntry.DefaultRunPr = new DmlRunProperties();
                            DmlTextShapeReader.ReadRunProperties(styleEntry.DefaultRunPr, reader);
                            break;
                        }
                    case "bodyPr":
                        {
                            styleEntry.TextBodyPr = new DmlTextBodyProperties();
                            DmlTextShapeReader.ReadTextBodyProperties(reader, styleEntry.TextBodyPr, reader.ComplianceInfo);
                            break;
                        }
                    case "extLst":
                        styleEntry.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return styleEntry;
        }

        /// <summary>
        /// Reads an element of the 2.8.3.8 CT_StyleReference complex type.
        /// </summary>
        private static DmlChartStyleReference ReadStyleReference(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlChartStyleReference styleReference = new DmlChartStyleReference();

            styleReference.StyleMatrixIndex = xmlReader.ReadIntAttribute("idx", 0);
            styleReference.Modifiers = ReadReferenceModifiers(xmlReader);
            ReadReferenceColor(reader, styleReference);

            return styleReference;
        }

        /// <summary>
        /// Reads the mods attribute of the 2.8.4.12 ST_StyleReferenceModifierList type.
        /// </summary>
        private static string[] ReadReferenceModifiers(NrxXmlReader xmlReader)
        {
            string modifiers = xmlReader.ReadAttribute("mods", string.Empty);
            return StringUtil.HasChars(modifiers) ? modifiers.Split(' ') : null;
        }

        /// <summary>
        /// Reads color of the CT_StyleReference and CT_FontReference elements [MS-ODRAWXML].
        /// </summary>
        private static void ReadReferenceColor(DocxDocumentReaderBase reader, DmlStyleReferenceBase styleReference)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string currentTagName = xmlReader.LocalName;

            while (xmlReader.ReadChild(currentTagName))
            {
                if (xmlReader.LocalName == "styleClr")
                {
                    styleReference.Color = ReadStyleColor(reader);
                }
                else
                {
                    DmlColor color = DmlColorReader.Read(xmlReader, reader.ComplianceInfo);
                    // Reader returns null if it founds unknown tag. We filter null value to disable overwriting of
                    // already initialized Color property by null values.
                    if (color != null)
                        styleReference.Color = color;
                }
            }
        }

        /// <summary>
        /// Reads an element of the 2.8.3.6 CT_StyleColor complex type.
        /// </summary>
        private static DmlChartStyleColor ReadStyleColor(DocxDocumentReaderBase reader)
        {
            DmlChartStyleColor color = new DmlChartStyleColor();
            color.Value = reader.XmlReader.ReadAttribute(string.Empty);
            color.ColorModifiers = DmlColorReader.ReadModifiers(reader.XmlReader, reader.ComplianceInfo);
            return color;
        }

        /// <summary>
        /// Reads an element of the 2.8.3.4 CT_FontReference complex type.
        /// </summary>
        private static DmlChartFontReference ReadFontReference(DocxDocumentReaderBase reader)
        {
            DmlChartFontReference fontReference = new DmlChartFontReference();

            string idxValue = reader.XmlReader.ReadAttribute("idx", string.Empty);
            fontReference.FontCollectionIndex = DmlEnum.DmlToFontCollectionIndex(idxValue);

            fontReference.Modifiers = ReadReferenceModifiers(reader.XmlReader);
            ReadReferenceColor(reader, fontReference);

            return fontReference;
        }

        /// <summary>
        /// Reads an element of the 2.8.3.5 CT_MarkerLayout complex type.
        /// </summary>
        private static DmlChartMarkerLayout ReadMarkerLayout(NrxXmlReader xmlReader)
        {
            DmlChartMarkerLayout markerLayout = new DmlChartMarkerLayout();

            markerLayout.Symbol = xmlReader.ReadAttribute("symbol", string.Empty);
            markerLayout.Size = xmlReader.ReadIntAttribute("size", 0);

            return markerLayout;
        }

        /// <summary>
        /// Static ctor.
        /// </summary>
        static DmlChartStyleReader()
        {
            gElementToStyleItemMap = new StringToIntDictionary();

            gElementToStyleItemMap.Add("axisTitle", (int)DmlChartStyleItem.AxisTitle);
            gElementToStyleItemMap.Add("categoryAxis", (int)DmlChartStyleItem.CategoryAxis);
            gElementToStyleItemMap.Add("chartArea", (int)DmlChartStyleItem.ChartArea);
            gElementToStyleItemMap.Add("dataLabel", (int)DmlChartStyleItem.DataLabel);
            gElementToStyleItemMap.Add("dataLabelCallout", (int)DmlChartStyleItem.DataLabelCallout);
            gElementToStyleItemMap.Add("dataPoint", (int)DmlChartStyleItem.DataPoint);
            gElementToStyleItemMap.Add("dataPoint3D", (int)DmlChartStyleItem.DataPoint3D);
            gElementToStyleItemMap.Add("dataPointLine", (int)DmlChartStyleItem.DataPointLine);
            gElementToStyleItemMap.Add("dataPointMarker", (int)DmlChartStyleItem.DataPointMarker);
            gElementToStyleItemMap.Add("dataPointWireframe", (int)DmlChartStyleItem.DataPointWireframe);
            gElementToStyleItemMap.Add("dataTable", (int)DmlChartStyleItem.DataTable);
            gElementToStyleItemMap.Add("downBar", (int)DmlChartStyleItem.DownBar);
            gElementToStyleItemMap.Add("dropLine", (int)DmlChartStyleItem.DropLine);
            gElementToStyleItemMap.Add("errorBar", (int)DmlChartStyleItem.ErrorBar);
            gElementToStyleItemMap.Add("floor", (int)DmlChartStyleItem.Floor);
            gElementToStyleItemMap.Add("gridlineMajor", (int)DmlChartStyleItem.GridlineMajor);
            gElementToStyleItemMap.Add("gridlineMinor", (int)DmlChartStyleItem.GridlineMinor);
            gElementToStyleItemMap.Add("hiLoLine", (int)DmlChartStyleItem.HighLowLine);
            gElementToStyleItemMap.Add("leaderLine", (int)DmlChartStyleItem.LeaderLine);
            gElementToStyleItemMap.Add("legend", (int)DmlChartStyleItem.Legend);
            gElementToStyleItemMap.Add("plotArea", (int)DmlChartStyleItem.PlotArea);
            gElementToStyleItemMap.Add("plotArea3D", (int)DmlChartStyleItem.PlotArea3D);
            gElementToStyleItemMap.Add("seriesAxis", (int)DmlChartStyleItem.SeriesAxis);
            gElementToStyleItemMap.Add("seriesLine", (int)DmlChartStyleItem.SeriesLine);
            gElementToStyleItemMap.Add("title", (int)DmlChartStyleItem.Title);
            gElementToStyleItemMap.Add("trendline", (int)DmlChartStyleItem.TrendLine);
            gElementToStyleItemMap.Add("trendlineLabel", (int)DmlChartStyleItem.TrendlineLabel);
            gElementToStyleItemMap.Add("upBar", (int)DmlChartStyleItem.UpBar);
            gElementToStyleItemMap.Add("valueAxis", (int)DmlChartStyleItem.ValueAxis);
            gElementToStyleItemMap.Add("wall", (int)DmlChartStyleItem.Wall);

#if DEBUG
            Debug.Assert(gElementToStyleItemMap.Count == DmlChartStyle.AllStyleItems.Length,
                "Wrong length of the style item map.");
#endif
        }

        private static readonly StringToIntDictionary gElementToStyleItemMap;
    }
}
