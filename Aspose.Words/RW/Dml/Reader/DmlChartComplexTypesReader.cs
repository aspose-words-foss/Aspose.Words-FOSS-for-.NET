// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/06/2012 by Alexey Noskov

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Xml;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Class is used to read complex types upon reading charts.
    /// </summary>
    internal class DmlChartComplexTypesReader : DmlReaderBase
    {
        private DmlChartComplexTypesReader()
        {
        }

        /// <summary>
        /// Reads CT_TextBody complex type that defines specifies text formatting.
        /// </summary>
        internal static void ReadChartTxPr(DocxDocumentReaderBase reader, DmlChartTxPr txPr)
        {
            while (reader.XmlReader.ReadChild("txPr"))
            {
                switch (reader.XmlReader.LocalName)
                {
                    case "bodyPr":
                    {
                        txPr.BodyPr = new DmlTextBodyProperties();
                        DmlTextShapeReader.ReadTextBodyProperties(reader, txPr.BodyPr,
                            reader.ComplianceInfo);
                        break;
                    }
                    case "lstStyle":
                        // According to the specs this element is not supported within chart, but anyway lets read it.
                        DmlTextShapeReader.ReadListStyles(reader, txPr.LstStyle);
                        break;
                    case "p":
                        DmlParagraph paragraph = txPr.AddParagraph();
                        DmlTextShapeReader.ReadParagraph(paragraph, reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader.XmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads CT_ShapeProperties complex type that contains Shape properties for charts.
        /// 5.7.2.198 spPr (Shape Properties)
        /// </summary>
        internal static void ReadChartSpPr(DocxDocumentReaderBase reader, DmlChartSpPr spPr)
        {
            OoxmlComplianceInfo compliance = reader.ComplianceInfo;

            while (reader.XmlReader.ReadChild("spPr"))
            {
                switch (reader.XmlReader.LocalName)
                {
                    case "blipFill":
                    case "gradFill":
                    case "grpFill":
                    case "noFill":
                    case "pattFill":
                    case "solidFill":
                        spPr.Fill = DmlFillReader.Read(reader);
                        break;
                    case "ln":
                        spPr.Outline = DmlOutlineReader.Read(reader);
                        break;
                    case "effectLst":
                        spPr.Effects = DmlShapeEffectReader.ReadEffects(reader, false, false);
                        break;
                    case "effectDag":
                        spPr.Effects = DmlShapeEffectReader.ReadEffects(reader, false, true);
                        break;
                    case "scene3d":
                        spPr.Scene3DProp = DmlScene3DReader.ReadScene3DProperties(reader, compliance);
                        break;
                    case "sp3d":
                        spPr.Shape3DProp = DmlScene3DReader.ReadShape3DProperties(reader, compliance);
                        break;
                    case "extLst":
                        spPr.Extensions = DmlExtensionListReader.Read(reader);
                        break;

                    // According to the spec custGeom, prstGeom, scene3d, and xfrm are not supported in charts.
                    case "custGeom":
                    case "xfrm":
                    case "prstGeom":
                    // Other are not supported by our code, so ignore.
                    default:
                        WarnUnexpectedAndIgnoreElement(reader.XmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// There are several elements in charts that have only SpPr as child nodes and no attributes.
        /// Use this method to read such elements.
        /// </summary>
        internal static void ReadChartSpPrContainer(DocxDocumentReaderBase reader, DmlChartSpPr spPr)
        {
            string tagName = reader.XmlReader.LocalName;

            while (reader.XmlReader.ReadChild(tagName))
            {
                switch (reader.XmlReader.LocalName)
                {
                    case "spPr":
                        ReadChartSpPr(reader, spPr);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader.XmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads CT_Legend complex type that specifies the legend.
        /// </summary>
        internal static ChartLegend ReadChartLegend(DocxDocumentReaderBase reader, DmlChartFormat chartFormat)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            ChartLegend legend = new ChartLegend(chartFormat);
            legend.IsVisible = true;
            if (xmlReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartEx, false))
            {
                // Default position is different in Word 2016 charts.
                legend.SidePosition = ChartLegend.ChartExDefaultSidePosition;
            }

            // Read attributes of the 2.24.3.54 CT_Legend complex type [MS-ODRAWXML].
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "pos":
                        legend.SidePosition =
                            DmlChartsEnum.DmlToSidePosition(xmlReader.Value, ChartLegend.ChartExDefaultSidePosition);
                        break;
                    case "align":
                        legend.PositionAlignment = DmlChartsEnum.DmlToPositionAlignment(xmlReader.Value);
                        break;
                    case "overlay":
                        legend.Overlay = xmlReader.ValueAsBool;
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }

            xmlReader.MoveToElement();
            while (xmlReader.ReadChild("legend"))
            {
                switch (xmlReader.LocalName)
                {
                    case "layout":
                        legend.Layout = ReadChartLayout(reader);
                        break;
                    case "legendEntry":
                        legend.AddEntry(ReadChartLegendEntry(reader));
                        break;
                    case "legendPos":
                        legend.SetPositionInternal(DmlChartsEnum.DmlToLegendPosition(xmlReader.ReadAttribute("")));
                        break;
                    case "overlay":
                        legend.Overlay = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "spPr":
                        ReadChartSpPr(reader, legend.SpPr);
                        break;
                    case "txPr":
                        ReadChartTxPr(reader, legend.TxPr);
                        break;
                    case "extLst":
                        ((IDmlExtensionListSource)legend).Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return legend;
        }

        /// <summary>
        /// Reads CT_LegendEntry complex type that specifies a legend entry.
        /// 5.7.2.95 legendEntry (Legend Entry)
        /// </summary>
        private static ChartLegendEntry ReadChartLegendEntry(DocxDocumentReaderBase reader)
        {
            ChartLegendEntry entry = new ChartLegendEntry();
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("legendEntry"))
            {
                switch (xmlReader.LocalName)
                {
                    case "delete":
                        entry.IsHidden = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "idx":
                        entry.Index = xmlReader.ReadIntAttribute(0);
                        break;
                    case "txPr":
                        ReadChartTxPr(reader, entry.TxPr);
                        break;
                    case "extLst":
                        ((IDmlExtensionListSource)entry).Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return entry;
        }

        /// <summary>
        /// Reads CT_PivotFmts complex type that contains a collection of formatting bands for a surface
        /// chart indexed from low to high.
        /// 5.7.2.144 pivotFmts (Pivot Formats)
        /// </summary>
        internal static DmlChartPivotFormats ReadChartPivotFormats(DocxDocumentReaderBase reader)
        {
            DmlChartPivotFormats pivotFormats = new DmlChartPivotFormats();

            while (reader.XmlReader.ReadChild("pivotFmts"))
            {
                switch (reader.XmlReader.LocalName)
                {
                    case "pivotFmt":
                        pivotFormats.AddFmt(ReadChartPivotFormat(reader));
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader.XmlReader);
                        break;
                }
            }

            return pivotFormats;
        }

        /// <summary>
        /// Reads CT_PivotFmt complex type that contains a set of formatting to be applied to the chart
        /// that is based on a pivotTable.
        /// 5.7.2.143 pivotFmt (Pivot Format)
        /// </summary>
        private static DmlChartPivotFormat ReadChartPivotFormat(DocxDocumentReaderBase reader)
        {
            DmlChartPivotFormat pivotFormat = new DmlChartPivotFormat(reader.Document.GetThemeInternal());
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("pivotFmt"))
            {
                switch (xmlReader.LocalName)
                {
                    case "dLbl":
                        pivotFormat.DLbl = ReadChartDataLabel(reader, null, null);
                        break;
                    case "idx":
                        pivotFormat.Index = xmlReader.ReadIntAttribute(0);
                        break;
                    case "marker":
                        ReadChartMarker(reader, pivotFormat.Marker);
                        break;
                    case "spPr":
                        ReadChartSpPr(reader, pivotFormat.SpPr);
                        break;
                    case "txPr":
                        ReadChartTxPr(reader, pivotFormat.TxPr);
                        break;
                    case "extLst":
                        pivotFormat.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return pivotFormat;
        }

        /// <summary>
        /// Reads CT_View3D complex type that specifies the 3-D view of the chart.
        /// </summary>
        internal static DmlChartView3D ReadChartView3D(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            DmlChartView3D view3D = new DmlChartView3D();

            while (xmlReader.ReadChild("view3D"))
            {
                switch (xmlReader.LocalName)
                {
                    case "depthPercent":
                        view3D.DepthPercent = xmlReader.ReadIntAttribute(100);
                        break;
                    case "hPercent":
                        view3D.HPercent = xmlReader.ReadIntAttribute(100);
                        break;
                    case "perspective":
                        view3D.Perspective = xmlReader.ReadIntAttribute(30);
                        break;
                    case "rAngAx":
                        view3D.RAngAx = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "rotX":
                        view3D.RotX = xmlReader.ReadIntAttribute(0);
                        break;
                    case "rotY":
                        view3D.RotY = xmlReader.ReadIntAttribute(0);
                        break;
                    case "extLst":
                        view3D.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return view3D;
        }

        /// <summary>
        /// Reads CT_Protection complex type that specifies protection for the chart.
        /// 5.7.2.150 protection (Protection)
        /// </summary>
        internal static DmlChartProtection ReadChartProtection(NrxXmlReader reader)
        {
            DmlChartProtection protection = new DmlChartProtection();

            while (reader.ReadChild("protection"))
            {
                switch (reader.LocalName)
                {
                    case "chartObject":
                        protection.ChartObject = reader.ReadBoolAttribute(true);
                        break;
                    case "data":
                        protection.Data = reader.ReadBoolAttribute(true);
                        break;
                    case "formatting":
                        protection.Formatting = reader.ReadBoolAttribute(true);
                        break;
                    case "selection":
                        protection.Selection = reader.ReadBoolAttribute(true);
                        break;
                    case "userInterface":
                        protection.UserInterface = reader.ReadBoolAttribute(true);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }

            return protection;
        }

        /// <summary>
        /// Reads CT_PivotSource complex type that specifies the source pivot table for a pivot chart.
        /// 5.7.2.145 pivotSource (Pivot Source)
        /// </summary>
        internal static DmlChartPivotSource ReadChartPivotSource(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            DmlChartPivotSource pivotSource = new DmlChartPivotSource();

            while (xmlReader.ReadChild("pivotSource"))
            {
                switch (xmlReader.LocalName)
                {
                    case "fmtId":
                        pivotSource.FmtId = xmlReader.ReadIntAttribute(0);
                        break;
                    case "name":
                        pivotSource.Name = xmlReader.ReadString();
                        break;
                    case "extLst":
                        pivotSource.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return pivotSource;
        }

        /// <summary>
        /// Reads CT_UpDownBars complex type that specifies the up and down bars..
        /// 5.7.2.219 upDownBars (Up/Down Bars)
        /// </summary>
        internal static void ReadChartUpDownBars(DocxDocumentReaderBase reader, DmlChartUpDownBars upDownBars)
        {
            upDownBars.IsVisible = true;
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("upDownBars"))
            {
                switch (xmlReader.LocalName)
                {
                    case "downBars":
                    {
                        upDownBars.HasDownBars = true;
                        ReadChartSpPrContainer(reader, upDownBars.DownBars);
                        break;
                    }
                    case "gapWidth":
                        upDownBars.GapWidth = xmlReader.ReadIntAttribute(150);
                        break;
                    case "upBars":
                    {
                        upDownBars.HasUpBars = true;
                        ReadChartSpPrContainer(reader, upDownBars.UpBars);
                        break;
                    }
                    case "extLst":
                        upDownBars.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads CT_CustSplit complex type that contains the custom split information for a pie-of-pie or
        /// bar-of-pie chart with a custom split type.
        /// 5.7.2.35 custSplit (Custom Split)
        /// </summary>
        internal static int[] ReadChartCustomSplit(NrxXmlReader reader)
        {
            IntList splitsList = IntList.CreateAllocated();

            while (reader.ReadChild("custSplit"))
            {
                switch (reader.LocalName)
                {
                    case "secondPiePt":
                        splitsList.Add(reader.ReadIntAttribute(0));
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }

            return splitsList.ToArray();
        }

        /// <summary>
        /// Reads CT_BandFmts complex type that contains a collection of formatting bands for a surface chart
        /// indexed from low to high.
        /// 5.7.2.14 bandFmts (Band Formats)
        /// </summary>
        internal static void ReadChartBandFmts(DocxDocumentReaderBase reader, DmlChartBandFormats bandFmts)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("bandFmts"))
            {
                switch (xmlReader.LocalName)
                {
                    case "bandFmt":
                        ReadChartBandFmt(reader, bandFmts);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads CT_BandFmt complex type that specifies the formatting band of a surface chart.
        /// 5.7.2.13 bandFmt (Band Format)
        /// </summary>
        private static void ReadChartBandFmt(DocxDocumentReaderBase reader, DmlChartBandFormats bandFmts)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            int index = 0;
            DmlChartSpPr spPr = new DmlChartSpPr();

            while (xmlReader.ReadChild("bandFmt"))
            {
                switch (xmlReader.LocalName)
                {
                    case "idx":
                        index = xmlReader.ReadIntAttribute(0);
                        break;
                    case "spPr":
                        ReadChartSpPr(reader, spPr);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            bandFmts.AddFmt(index, spPr);
        }

        /// <summary>
        /// Reads CT_DTable complex type that specifies a data table.
        /// 5.7.2.54 dTable (Data Table)
        /// </summary>
        internal static ChartDataTable ReadChartDataTable(DocxDocumentReaderBase reader, DmlChartPlotArea plotArea)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            ChartDataTable dt = new ChartDataTable(plotArea);
            // If a data table element is present, the data table is shown.
            dt.SetShowInternal(true);

            while (xmlReader.ReadChild("dTable"))
            {
                switch (xmlReader.LocalName)
                {
                    case "showHorzBorder":
                        dt.HasHorizontalBorder = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "showKeys":
                        dt.HasLegendKeys = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "showOutline":
                        dt.HasOutlineBorder = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "showVertBorder":
                        dt.HasVerticalBorder = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "spPr":
                        ReadChartSpPr(reader, dt.SpPr);
                        break;
                    case "txPr":
                        ReadChartTxPr(reader, dt.TxPr);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return dt;
        }

        /// <summary>
        /// Reads CT_Surface complex type that specifies the back wall, side wall or floor of the chart.
        /// 5.7.2.11 backWall (Back Wall)
        /// 5.7.2.192 sideWall (Side Wall)
        /// 5.7.2.69 floor (Floor)
        /// </summary>
        internal static DmlChartSurface ReadChartSurface(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            string tagName = xmlReader.LocalName;
            DmlChartSurface surface = new DmlChartSurface();

            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "thickness":
                        surface.Thickness = xmlReader.ReadIntAttribute(0);
                        break;
                    case "pictureOptions":
                        surface.PictureOptions = ReadChartPictureOptions(xmlReader);
                        break;
                    case "spPr":
                        ReadChartSpPr(reader, surface.SpPr);
                        break;
                    case "extLst":
                        surface.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return surface;
        }

        /// <summary>
        /// Reads CT_Title complex type that specifies a title.
        /// 5.7.2.211 title (Title) ECMA-376 and 2.24.3.12 CT_ChartTitle [MS-ODRAWXML]
        /// </summary>
        internal static DmlChartTitle ReadChartTitle(IDmlChartTitleHolder chartTitleHolder, DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            DmlChartTitle title = new DmlChartTitle(chartTitleHolder);

            // Read attributes of the 2.24.3.12 CT_ChartTitle complex type [MS-ODRAWXML].
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "pos":
                        title.SidePosition = DmlChartsEnum.DmlToSidePosition(xmlReader.Value, SidePosition.Top);
                        break;
                    case "align":
                        title.PositionAlignment = DmlChartsEnum.DmlToPositionAlignment(xmlReader.Value);
                        break;
                    case "overlay":
                        title.Overlay = xmlReader.ValueAsBool;
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }

            xmlReader.MoveToElement();
            while (xmlReader.ReadChild("title"))
            {
                switch (xmlReader.LocalName)
                {
                    case "layout":
                        title.Layout = ReadChartLayout(reader);
                        break;
                    case "overlay":
                        title.Overlay = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "spPr":
                        title.SpPr = new DmlChartSpPr();
                        ReadChartSpPr(reader, title.SpPr);
                        break;
                    case "tx":
                        title.Tx = ReadChartTx(reader);
                        break;
                    case "txPr":
                        ReadChartTxPr(reader, title.TxPr);
                        break;
                    case "extLst":
                        ((IDmlExtensionListSource)title).Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return title;
        }

        /// <summary>
        /// Reads CT_NumFmt complex type that specifies number formatting for the parent element.
        /// 5.7.2.122 numFmt (Number Format).
        /// </summary>
        internal static DmlChartNumFormat ReadChartNumFormat(NrxXmlReader reader)
        {
            DmlChartNumFormat numFormat = new DmlChartNumFormat();

            // Read attributes.
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "formatCode":
                        numFormat.FormatCode = reader.Value;
                        break;
                    case "sourceLinked":
                        numFormat.SourceLinked = reader.ValueAsBool;
                        break;
                    default:
                        WarnUnexpected(reader);
                        break;
                }
            }

            return numFormat;
        }

        /// <summary>
        /// Reads CT_Scaling complex type that contains additional axis settings.
        /// 5.7.2.161 scaling (Scaling)
        /// </summary>
        internal static void ReadChartScaling(DocxDocumentReaderBase reader, AxisScaling scaling)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            // Read children
            while (xmlReader.ReadChild("scaling"))
            {
                switch (xmlReader.LocalName)
                {
                    case "logBase":
                    {
                        double value = xmlReader.ReadDoubleAttribute(double.NaN);
                        if (!double.IsNaN(value))
                            scaling.SetLogBaseWithoutCheck(value);
                        break;
                    }
                    case "max":
                        scaling.Maximum = new AxisBound(xmlReader.ReadDoubleAttribute(0));
                        break;
                    case "min":
                        scaling.Minimum = new AxisBound(xmlReader.ReadDoubleAttribute(0));
                        break;
                    case "orientation":
                        scaling.Orientation = DmlChartsEnum.DmlToAxisOrientation(xmlReader.ReadAttribute("minMax"));
                        break;
                    case "extLst":
                        ((IDmlExtensionListSource)scaling).Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads the catScaling element of the 2.24.3.8 CT_CategoryAxisScaling complex type [MS-ODRAWXML].
        /// It contains additional axis settings.
        /// </summary>
        internal static void ReadCategoryScaling(NrxXmlReader reader, AxisScaling scaling)
        {
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "gapWidth":
                        ReadDoubleOrAutomatic(reader, scaling.GapWidth);
                        break;
                    default:
                        WarnUnexpected(reader);
                        break;
                }
            }

            reader.MoveToElement();
        }

        /// <summary>
        /// Reads the valScaling element of the 2.24.3.82 CT_ValueAxisScaling complex type [MS-ODRAWXML].
        /// It contains additional axis settings.
        /// </summary>
        internal static void ReadValueScaling(NrxXmlReader reader, ChartAxis axis)
        {
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "max":
                        axis.Scaling.Maximum = ReadAxisBound(reader);
                        break;
                    case "min":
                        axis.Scaling.Minimum = ReadAxisBound(reader);
                        break;
                    case "majorUnit":
                        ReadDoubleOrAutomatic(reader, axis.MajorUnitPr);
                        break;
                    case "minorUnit":
                        ReadDoubleOrAutomatic(reader, axis.MinorUnitPr);
                        break;
                    default:
                        WarnUnexpected(reader);
                        break;
                }
            }

            reader.MoveToElement();
        }

        /// <summary>
        /// Reads value of the <see cref="DoubleOrAutomatic"/> type from elements of the ST_DoubleOrAutomatic,
        /// ST_ValueAxisUnit, ST_GapWidthRatio simple types [MS-ODRAWXML].
        /// </summary>
        private static void ReadDoubleOrAutomatic(NrxXmlReader reader, DoubleOrAutomatic instance)
        {
            if (reader.Value == "auto")
                instance.IsAuto = true;
            else
                instance.Value = FormatterPal.TryParseDoubleInvariant(reader.Value);
        }

        /// <summary>
        /// Reads value of the <see cref="AxisBound"/> type.
        /// </summary>
        private static AxisBound ReadAxisBound(NrxXmlReader reader)
        {
            if (reader.Value == "auto")
                return AxisBound.Auto;
            else
                return new AxisBound(FormatterPal.TryParseDoubleInvariant(reader.Value));
        }

        /// <summary>
        /// Reads CT_DispUnits complex type that specifies the scaling value of the display units for the value axis.
        /// 5.7.2.45 dispUnits (Display Units).
        /// </summary>
        internal static AxisDisplayUnit ReadChartDisplayUnits(IDmlChartTitleHolder chartTitleHolder,
            DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            AxisDisplayUnit displayUnits = new AxisDisplayUnit();
            string tagName = xmlReader.LocalName;
            bool isEmpty = true;
            bool isChartEx =
                xmlReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartEx, false);

            if (isChartEx)
            {
                string units = xmlReader.ReadAttribute("unit", "");
                if (StringUtil.HasChars(units))
                {
                    displayUnits.Unit = DmlChartsEnum.DmlToBuiltInUnit(units);
                    isEmpty = false;
                }
            }

            // Read children
            while (xmlReader.ReadChild(tagName))
            {
                isEmpty = false;

                switch (xmlReader.LocalName)
                {
                    case "builtInUnit":
                        displayUnits.Unit = DmlChartsEnum.DmlToBuiltInUnit(xmlReader.ReadAttribute("thousands"));
                        break;
                    case "custUnit":
                        displayUnits.CustomUnit = xmlReader.ReadDoubleAttribute(0);
                        break;
                    case "dispUnitsLbl":
                    case "unitsLabel": // 2.24.3.6 CT_AxisUnitsLabel [MS-ODRAWXML]
                        displayUnits.SetLabel(ReadChartDisplayUnitsLabel(chartTitleHolder, reader));
                        break;
                    case "extLst":
                        ((IDmlExtensionListSource)displayUnits).Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            // In case of empty 'dispUnits' element we return null since no processing of display units is required in this case.
            return isEmpty ? null : displayUnits;
        }


        /// <summary>
        /// Reads CT_DispUnitsLbl complex type that specifies the display unit label for the value axis in the specified chart.
        /// 5.7.2.46 dispUnitsLbl (Display Units Label).
        /// </summary>
        private static DmlChartDisplayUnitsLabel ReadChartDisplayUnitsLabel(IDmlChartTitleHolder chartTitleHolder,
            DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlChartDisplayUnitsLabel label = new DmlChartDisplayUnitsLabel(chartTitleHolder);
            string tagName = xmlReader.LocalName;

            // Read children
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "layout":
                        label.Layout = ReadChartLayout(reader);
                        break;
                    case "spPr":
                        ReadChartSpPr(reader, label.SpPr);
                        break;
                    case "tx":
                        label.Tx = ReadChartTx(reader);
                        break;
                    case "txPr":
                        ReadChartTxPr(reader, label.TxPr);
                        break;
                    case "extLst": // in 2.24.3.6 CT_AxisUnitsLabel complex type [MS-ODRAWXML]
                        ((IDmlExtensionListSource)label).Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return label;
        }

        /// <summary>
        /// Reads CT_Layout complex type that specifies how the chart element is placed on the chart.
        /// 5.7.2.88 layout (Layout).
        /// </summary>
        internal static DmlChartManualLayout ReadChartLayout(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            // There can be empty 'layout', in this case behavior should be the same as if 'layout' is not set at all.
            // So do not create an instance of DmlChartManualLayout if 'manualLayout' does not exist.
            DmlChartManualLayout layout = null;

            // Read children
            while (xmlReader.ReadChild("layout"))
            {
                switch (xmlReader.LocalName)
                {
                    case "manualLayout":
                        layout = new DmlChartManualLayout();
                        ReadChartManualLayout(reader, layout);
                        break;
                    case "extLst":
                        WarnNotSupportedAndIgnoreElement(xmlReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return layout;
        }

        /// <summary>
        /// Reads CT_ManualLayout complex type properties that specifies the exact position of a chart element.
        /// </summary>
        private static void ReadChartManualLayout(DocxDocumentReaderBase reader, DmlChartManualLayout layout)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            // Read children
            while (xmlReader.ReadChild("manualLayout"))
            {
                switch (xmlReader.LocalName)
                {
                    case "h":
                        layout.Height = xmlReader.ReadDoubleAttribute(0);
                        break;
                    case "w":
                        layout.Width = xmlReader.ReadDoubleAttribute(0);
                        break;
                    case "x":
                        layout.Left = xmlReader.ReadDoubleAttribute(0);
                        break;
                    case "y":
                        layout.Top = xmlReader.ReadDoubleAttribute(0);
                        break;
                    case "hMode":
                        layout.HeightMode = DmlChartsEnum.DmlToLayoutMode(xmlReader.ReadAttribute("factor"));
                        break;
                    case "wMode":
                        layout.WidthMode = DmlChartsEnum.DmlToLayoutMode(xmlReader.ReadAttribute("factor"));
                        break;
                    case "xMode":
                        layout.LeftMode = DmlChartsEnum.DmlToLayoutMode(xmlReader.ReadAttribute("factor"));
                        break;
                    case "yMode":
                        layout.TopMode = DmlChartsEnum.DmlToLayoutMode(xmlReader.ReadAttribute("factor"));
                        break;
                    case "layoutTarget":
                        layout.LayoutTarget = DmlChartsEnum.DmlToLayoutTarget(xmlReader.ReadAttribute("outer"));
                        break;
                    case "extLst":
                        layout.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads CT_SerTx or CT_Tx complex type that specifies text to use on a chart, including rich text formatting
        /// or text for a series name, without rich text formatting.
        /// 5.7.2.215 tx (Chart Text)
        /// 5.7.2.216 tx (Series Text)
        /// </summary>
        internal static DmlChartTx ReadChartTx(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlChartTx tx = new DmlChartTx();

            // Read children
            while (xmlReader.ReadChild("tx"))
            {
                switch (xmlReader.LocalName)
                {
                    case "rich":
                        tx.RichText = new DmlTextBody();
                        DmlTextShapeReader.ReadTextBody(reader, tx.RichText, "rich");
                        break;
                    case "v":
                        tx.PlainText = xmlReader.ReadString();
                        break;
                    case "strRef":
                        tx.StrRef = ReadChartValRef(reader);
                        break;
                    case "txData": // 2.24.3.79 CT_TextData [MS-ODRAWXML]
                        ReadTxData(reader, tx);
                        break;
                    case "extLst":
                        tx.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return tx;
        }

        /// <summary>
        /// Reads the txData element of the 2.24.3.79 CT_TextData complex type [MS-ODRAWXML].
        /// </summary>
        private static void ReadTxData(DocxDocumentReaderBase reader, DmlChartTx tx)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            // Read children
            while (xmlReader.ReadChild("txData"))
            {
                switch (xmlReader.LocalName)
                {
                    case "f":
                    {
                        tx.Formula = new DmlChartFormula();
                        ReadFormula(xmlReader, tx.Formula);
                        break;
                    }
                    case "v":
                        tx.PlainText = xmlReader.ReadString();
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads extension 2.6.1.3 datalabelsRange
        /// </summary>
        internal static DmlChartValueRef ReadChartDataLabelsRange(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            DmlChartValueRef valueRef = null;
            while (xmlReader.ReadChild("ext"))
            {
                switch (xmlReader.LocalName)
                {
                    case "datalabelsRange":
                        valueRef = ReadChartValRef(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return valueRef;
        }

        /// <summary>
        /// Reads uniqueId extension (http://schemas.microsoft.com/office/drawing/2014/chart).
        /// </summary>
        internal static Guid ReadUniqueIdExtension(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            Guid uniqueId = Guid.Empty;

            while (xmlReader.ReadChild("ext"))
            {
                switch (xmlReader.LocalName)
                {
                    case "uniqueId":
                    {
                        string id = xmlReader.ReadAttribute("val", "");
                        if (id != "")
                            uniqueId = new Guid(id);
                        break;
                    }
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return uniqueId;
        }

        /// <summary>
        /// Reads CT_StrRef or CT_NumRef complex type that specifies a reference to data for a single data
        /// label or title with a cache of the last values used.
        /// 5.7.2.202 strRef (String Reference)
        /// 5.7.2.124 1 numRef (Number Reference)
        /// The method also reads elements of the CT_NumericDimension and CT_StringDimension complex types [MS-ODRAWXML].
        /// </summary>
        internal static DmlChartValueRef ReadChartValRef(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            string tagName = xmlReader.LocalName;

            DmlChartValueType valueType = GetRefValueType(tagName);
            DmlChartValueRef dataRef = new DmlChartValueRef(valueType);
            int levelIndex = 0;

            dataRef.DimensionType = DmlChartsEnum.DmlToDimensionType(xmlReader.ReadAttribute("type", ""));

            // Read children
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "f":
                    {
                        // The dir attribute of the 2.24.3.28 CT_Formula complex type [MS-ODRAWXML].
                        ReadFormula(xmlReader, dataRef.Formula);
                        break;
                    }
                    case "nf":
                    {
                        // nf element of the CT_NumericDimension and CT_StringDimension complex types [MS-ODRAWXML]
                        ReadFormula(xmlReader, dataRef.NameFormula);
                        break;
                    }
                    case "strCache":
                    case "dlblRangeCache":
                        ReadChartValuesCache(reader, dataRef.Values);
                        break;
                    case "numCache":
                        ReadChartValuesCache(reader, dataRef.Values);
                        break;
                    case "multiLvlStrCache":
                        ReadChartMultiLvlValuesCache(reader, dataRef.Values);
                        break;
                    case "lvl":
                    {
                        // lvl element of the CT_NumericDimension and CT_StringDimension complex types [MS-ODRAWXML]
                        ReadLevel(xmlReader, dataRef.Values, tagName == "numDim", levelIndex);
                        levelIndex++;
                        break;
                    }
                    case "extLst":
                        dataRef.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return dataRef;
        }

        /// <summary>
        /// Reads the formula element (21.2.2.65 f [ISO/IEC 29500-1] or 2.24.3.28 CT_Formula [MS-ODRAWXML]).
        /// </summary>
        private static void ReadFormula(NrxXmlReader xmlReader, DmlChartFormula formula)
        {
            formula.Direction = DmlChartsEnum.DmlToFormulaDirection(xmlReader.ReadAttribute("dir", ""));
            formula.Value = xmlReader.ReadString();
        }

        private static DmlChartValueType GetRefValueType(string tagName)
        {
            switch (tagName)
            {
                case "multiLvlStrRef":
                case "strDim": // chartEx schema
                    return DmlChartValueType.MultiLvlString;
                case "numDim": // chartEx schema
                    return DmlChartValueType.MultiLvlNumeric;
                case "numRef":
                    return DmlChartValueType.Numeric;
                case "strRef":
                case "datalabelsRange":
                    return DmlChartValueType.String;
                default:
                    return DmlChartValueType.None;
            }
        }

        /// <summary>
        /// Reads CT_StrData complex type that specifies the last string data used for a chart.
        /// 5.7.2.200 strCache (String Cache)
        /// </summary>
        internal static void ReadChartMultiLvlValuesCache(DocxDocumentReaderBase reader, DmlChartValueCollection data)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string tagName = xmlReader.LocalName;
            int levelIndex = 0;

            // Read children
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "ptCount":
                        data.ValueCount = xmlReader.ReadIntAttribute(0);
                        break;
                    case "lvl":
                        ReadLevel(xmlReader, data, false, levelIndex);
                        levelIndex++;
                        break;
                    case "extLst":
                        data.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Read level of multi-level value cache.
        /// </summary>
        private static void ReadLevel(NrxXmlReader reader, DmlChartValueCollection data, bool isNumeric, int levelIndex)
        {
            if (reader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartEx, false))
            {
                // Read attributes of the CT_NumericLevel and CT_StringLevel complex types [MS-ODRAWXML].
                DmlChartDataLevelProperties levelProperties = new DmlChartDataLevelProperties();
                while (reader.MoveToNextAttribute())
                {
                    switch (reader.LocalName)
                    {
                        case "ptCount":
                            levelProperties.ValueCount = reader.ValueAsInt;
                            break;
                        case "formatCode":
                            levelProperties.FormatCode = reader.Value;
                            break;
                        case "name":
                            levelProperties.Name = reader.Value;
                            break;
                        default:
                            WarnUnexpected(reader);
                            break;
                    }
                }

                while (data.LevelProperties.Count < levelIndex)
                    data.AddLevelProperties(null);

                data.AddLevelProperties(levelProperties);
            }

            reader.MoveToElement();
            while (reader.ReadChild("lvl"))
            {
                switch (reader.LocalName)
                {
                    case "pt":
                    {
                        DmlChartValue val = ReadChartValPoint(reader,
                            isNumeric ? DmlChartValueType.Numeric : DmlChartValueType.String);
                        DmlChartMultiLvlValue multiLvlValue = (DmlChartMultiLvlValue)data[val.Index];

                        if (isNumeric)
                        {
                            if (multiLvlValue == null)
                            {
                                multiLvlValue = new DmlChartMultiLvlNumValue(val.Index, data.LevelProperties);
                                data.Add(multiLvlValue);
                            }

                            multiLvlValue.AddLevelValue(val.Value, levelIndex);
                        }
                        else
                        {
                            if (multiLvlValue == null)
                            {
                                multiLvlValue = new DmlChartMultiLvlStrValue(val.Index);
                                data.Add(multiLvlValue);
                            }

                            multiLvlValue.AddLevelValue(val.StringValue, levelIndex);
                        }
                        break;
                    }
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads CT_StrData complex type that specifies the last string data used for a chart.
        /// 5.7.2.200 strCache (String Cache)
        /// </summary>
        internal static void ReadChartValuesCache(DocxDocumentReaderBase reader, DmlChartValueCollection data)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            string tagName = xmlReader.LocalName;

            // Read children
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "formatCode":
                        data.FormatCode = xmlReader.ReadString();
                        break;
                    case "ptCount":
                    {
                        data.ValueCount = xmlReader.ReadIntAttribute(0);
                        break;
                    }
                    case "pt":
                        data.Add(ReadChartValPoint(xmlReader, data.ValueType));
                        break;
                    case "extLst":
                        data.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            // Inherit format code for numeric values from data collection.
            if (data.ValueType == DmlChartValueType.Numeric)
            {
                foreach (DmlChartValue value in data)
                {
                    DmlChartNumValue numValue = (DmlChartNumValue)value;
                    // WORDSNET-15228 Reset format code only if it is not specified explicitly.
                    // See TestJira14993.
                    if (!StringUtil.HasChars(numValue.FormatCode))
                        numValue.FormatCode = data.FormatCode;
                }
            }
        }

        /// <summary>
        /// Reads CT_StrVal complex type that specifies string data for a specific data point.
        /// 5.7.2.152 pt (String Point)
        /// </summary>
        internal static DmlChartValue ReadChartValPoint(NrxXmlReader reader, DmlChartValueType pointType)
        {
            int idx = 0;
            string strValue = null;
            string formatCode = null;
            bool isChartEx = reader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartEx, false);

            // Read attributes.
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "idx":
                        idx = reader.ValueAsInt;
                        break;
                    case "formatCode":
                        formatCode = reader.Value;
                        break;
                    default:
                        WarnUnexpected(reader);
                        break;
                }
            }

            reader.MoveToElement();
            if (isChartEx)
            {
                strValue = reader.ReadString();
            }
            else
            {
                // Read children
                while (reader.ReadChild("pt"))
                {
                    switch (reader.LocalName)
                    {
                        case "v":
                            strValue = reader.ReadString();
                            break;
                        default:
                            WarnUnexpectedAndIgnoreElement(reader);
                            break;
                    }
                }
            }

            if (pointType == DmlChartValueType.String)
            {
                return new DmlChartStrValue(idx, strValue);
            }
            else
            {
                // If NaN value encountered, it will be removed later.
                double val = FormatterPal.TryParseDoubleInvariant(strValue);
                return new DmlChartNumValue(idx, val, formatCode);
            }
        }

        /// <summary>
        /// Reads CT_DPt complex type that specifies a single data point.
        /// 5.7.2.52 dPt (Data Point)
        /// </summary>
        internal static ChartDataPoint ReadChartDataPoint(DocxDocumentReaderBase reader, DmlChart chart)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            ChartDataPoint dataPoint = new ChartDataPoint(chart);
            string tagName = xmlReader.LocalName;

            if (xmlReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartEx, false))
                dataPoint.PointPr.SetProperty(DmlChartDataPointAttr.Index, xmlReader.ReadIntAttribute("idx", 0));

            // Read children
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "bubble3D":
                        dataPoint.PointPr.SetProperty(DmlChartDataPointAttr.Bubble3D, xmlReader.ReadBoolAttribute(true));
                        break;
                    case "explosion":
                        dataPoint.PointPr.SetProperty(DmlChartDataPointAttr.Explosion, xmlReader.ReadIntAttribute(0));
                        break;
                    case "idx":
                        dataPoint.PointPr.SetProperty(DmlChartDataPointAttr.Index, xmlReader.ReadIntAttribute(0));
                        break;
                    case "invertIfNegative":
                        dataPoint.PointPr.SetProperty(DmlChartDataPointAttr.InvertIfNegative, xmlReader.ReadBoolAttribute(true));
                        break;
                    case "marker":
                        ChartMarker marker = new ChartMarker(chart);
                        ReadChartMarker(reader, marker);
                        dataPoint.PointPr.SetProperty(DmlChartDataPointAttr.Marker, marker);
                        break;
                    case "pictureOptions":
                        dataPoint.PointPr.SetProperty(DmlChartDataPointAttr.PictureOptions, ReadChartPictureOptions(xmlReader));
                        break;
                    case "spPr":
                        DmlChartSpPr spPr = new DmlChartSpPr();
                        ReadChartSpPr(reader, spPr);
                        dataPoint.PointPr.SetProperty(DmlChartDataPointAttr.SpPr, spPr);
                        break;
                    case "extLst":
                        dataPoint.PointPr.SetProperty(DmlChartDataPointAttr.Extensions, DmlExtensionListReader.Read(reader));
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return dataPoint;
        }

        /// <summary>
        /// Reads CT_PictureOptions complex type that specifies the picture to be used on the data point,
        /// series, wall, or floor.
        /// 5.7.2.139 pictureOptions (Picture Options)
        /// </summary>
        internal static DmlChartPictureOptions ReadChartPictureOptions(NrxXmlReader reader)
        {
            DmlChartPictureOptions pictureOptions = new DmlChartPictureOptions();

            // Read children
            while (reader.ReadChild("pictureOptions"))
            {
                switch (reader.LocalName)
                {
                    case "applyToEnd":
                        pictureOptions.ApplyToEnd = reader.ReadBoolAttribute(true);
                        break;
                    case "applyToFront":
                        pictureOptions.ApplyToFront = reader.ReadBoolAttribute(true);
                        break;
                    case "applyToSides":
                        pictureOptions.ApplyToSides = reader.ReadBoolAttribute(true);
                        break;
                    case "pictureFormat":
                        pictureOptions.PictureFormat = DmlChartsEnum.DmlToPictureFormat(reader.ReadAttribute("stack"));
                        break;
                    case "pictureStackUnit":
                        pictureOptions.PictureStackUnit = reader.ReadDoubleAttribute(0.0d);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(reader);
                        break;
                }
            }

            return pictureOptions;
        }

        /// <summary>
        /// Reads CT_Marker complex type that specifies a data marker.
        /// 5.7.2.106 marker (Marker)
        /// </summary>
        internal static void ReadChartMarker(DocxDocumentReaderBase reader, ChartMarker marker)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            // Read children
            while (xmlReader.ReadChild("marker"))
            {
                switch (xmlReader.LocalName)
                {
                    case "size":
                        marker.MarkerPr.SetProperty(DmlChartMarkerAttr.Size, xmlReader.ReadIntAttribute(7));
                        break;
                    case "spPr":
                        DmlChartSpPr spPr = new DmlChartSpPr();
                        ReadChartSpPr(reader, spPr);
                        marker.MarkerPr.SetProperty(DmlChartMarkerAttr.SpPr, spPr);
                        break;
                    case "symbol":
                        marker.MarkerPr.SetProperty(DmlChartMarkerAttr.Symbol, DmlChartsEnum.DmlToMarkerStyle(xmlReader.ReadAttribute("none")));
                        break;
                    case "extLst":
                        marker.MarkerPr.SetProperty(DmlChartMarkerAttr.Extensions, DmlExtensionListReader.Read(reader));
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads CT_DLbls complex type that serves as a root element
        /// that specifies the settings for the data labels for an entire series or the entire chart.
        /// 5.7.2.106 marker (Marker)
        /// </summary>
        internal static ChartDataLabelCollection ReadChartDataLabelCollection(DocxDocumentReaderBase reader,
            DmlChart chart, ChartSeries series)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            ChartDataLabelCollection labelCollection = new ChartDataLabelCollection(chart, series);

            if (series != null)
                series.SetHasDataLabels(true);

            DmlChartDataLabelPr labelPr = labelCollection.LabelPr;
            string tagName = xmlReader.LocalName;

            if (xmlReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartEx, false))
            {
                string position = xmlReader.ReadAttribute("pos", "");
                if (StringUtil.HasChars(position))
                    labelPr.SetProperty(DmlChartDataLabelAttrs.DLblPos, DmlChartsEnum.DmlToDataLabelPosition(position));
            }

            // Read children
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "delete":
                    case "dLblPos":
                    case "extLst":
                    case "numFmt":
                    case "separator":
                    case "showBubbleSize":
                    case "showCatName":
                    case "showLeaderLines":
                    case "showLegendKey":
                    case "showPercent":
                    case "showSerName":
                    case "showVal":
                    case "spPr":
                    case "txPr":
                    case "leaderLines":
                    case "visibility": // in the 2.24.3.19 CT_DataLabels complex type [MS-ODRAWXML]
                        ReadChartDataLabelOption(reader, labelPr, false);
                        break;
                    case "dLbl":
                    case "dataLabel": // in the 2.24.3.19 CT_DataLabels complex type [MS-ODRAWXML]
                        labelCollection.AddLabel(ReadChartDataLabel(reader, labelPr, series));
                        break;
                    case "dataLabelHidden": // in the 2.24.3.19 CT_DataLabels complex type [MS-ODRAWXML]
                        int index = xmlReader.ReadIntAttribute("idx", -1);
                        if (index >= 0)
                            labelCollection[index].IsHidden = true;
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            labelCollection.NormalizeAfterLoad();

            return labelCollection;
        }

        private static ChartDataLabel ReadChartDataLabel(DocxDocumentReaderBase reader, DmlChartDataLabelPr parentLabelPr,
            ChartSeries series)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            ChartDataLabel label = new ChartDataLabel(parentLabelPr, series);
            string tagName = xmlReader.LocalName;

            if (xmlReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartEx, false))
            {
                while (xmlReader.MoveToNextAttribute())
                {
                    switch (xmlReader.LocalName)
                    {
                        case "idx":
                            label.Index = xmlReader.ValueAsInt;
                            break;
                        case "pos":
                            label.LabelPr.SetProperty(DmlChartDataLabelAttrs.DLblPos,
                                DmlChartsEnum.DmlToDataLabelPosition(xmlReader.Value));
                            break;
                        default:
                            WarnUnexpected(xmlReader);
                            break;
                    }
                }

                xmlReader.MoveToElement();
            }

            while (xmlReader.ReadChild(tagName))
                ReadChartDataLabelOption(reader, label.LabelPr, false);

            return label;
        }

        /// <summary>
        /// Reads the extension of the dLbls/dLbl elements (2.2.1.2 and 2.2.1.3 of [MS-ODRAWXML]).
        /// </summary>
        internal static DmlChartDataLabelPr ReadChartExtDataLabelPr(DocxDocumentReaderBase reader)
        {
            DmlChartDataLabelPr pr = new DmlChartDataLabelPr(null, reader.Document, false);

            while (reader.XmlReader.ReadChild("ext"))
                ReadChartDataLabelOption(reader, pr, true);

            return pr;
        }

        private static void ReadChartDataLabelOption(DocxDocumentReaderBase reader, DmlChartDataLabelPr labelPr,
            bool isExtension)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            switch (xmlReader.LocalName)
            {
                case "delete":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.Delete, xmlReader.ReadBoolAttribute(true));
                    break;
                case "dLblPos":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.DLblPos,
                        DmlChartsEnum.DmlToDataLabelPosition(xmlReader.ReadAttribute("center")));
                    break;
                case "idx":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.Idx, xmlReader.ReadIntAttribute(0));
                    break;
                case "layout":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.Layout, ReadChartLayout(reader));
                    break;
                case "numFmt":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.NumFmt, ReadChartNumFormat(xmlReader));
                    break;
                case "separator":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.Separator, xmlReader.ReadString());
                    break;
                case "showBubbleSize":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.ShowBubbleSize, xmlReader.ReadBoolAttribute(true));
                    break;
                case "showCatName":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.ShowCatName, xmlReader.ReadBoolAttribute(true));
                    break;
                case "showLegendKey":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.ShowLegendKey, xmlReader.ReadBoolAttribute(true));
                    break;
                case "showPercent":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.ShowPercent, xmlReader.ReadBoolAttribute(true));
                    break;
                case "showSerName":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.ShowSerName, xmlReader.ReadBoolAttribute(true));
                    break;
                case "showVal":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.ShowVal, xmlReader.ReadBoolAttribute(true));
                    break;
                case "showLeaderLines":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.ShowLeaderLines, xmlReader.ReadBoolAttribute(true));
                    break;
                case "showDataLabelsRange":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.ShowDataLabelsRange, xmlReader.ReadBoolAttribute(true));
                    break;
                case "spPr":
                {
                    if (isExtension)
                    {
                        DmlChartShapeProperties shapeProperties = new DmlChartShapeProperties();
                        DmlNodePropertiesReader.ReadShapeProperties(reader, shapeProperties);
                        labelPr.SetProperty(DmlChartDataLabelAttrs.ShapePr, shapeProperties);
                    }
                    else
                    {
                        DmlChartSpPr spPr = new DmlChartSpPr();
                        ReadChartSpPr(reader, spPr);
                        labelPr.SetProperty(DmlChartDataLabelAttrs.SpPr, spPr);
                    }
                    break;
                }
                case "tx":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.Tx, ReadChartTx(reader));
                    break;
                case "txPr":
                    DmlChartTxPr txPr = new DmlChartTxPr();
                    ReadChartTxPr(reader, txPr);
                    labelPr.SetProperty(DmlChartDataLabelAttrs.TxPr, txPr);
                    break;
                case "leaderLines":
                    DmlChartSpPr leaderLines = new DmlChartSpPr();
                    ReadChartSpPrContainer(reader, leaderLines);
                    labelPr.SetProperty(DmlChartDataLabelAttrs.LeaderLines, leaderLines);
                    break;
                case "visibility": // in the 2.24.3.19 CT_DataLabels complex type [MS-ODRAWXML]
                {
                    labelPr.SetProperty(DmlChartDataLabelAttrs.ShowSerName,
                        xmlReader.ReadBoolAttribute("seriesName", false));
                    labelPr.SetProperty(DmlChartDataLabelAttrs.ShowCatName,
                        xmlReader.ReadBoolAttribute("categoryName", false));
                    labelPr.SetProperty(DmlChartDataLabelAttrs.ShowVal,
                        xmlReader.ReadBoolAttribute("value", true));
                    break;
                }
                case "xForSave":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.XForSave, xmlReader.ReadBoolAttribute(true));
                    break;
                case "dlblFieldTable":
                    ReadDataLabelFieldTable(xmlReader, labelPr);
                    break;
                case "extLst":
                    labelPr.SetProperty(DmlChartDataLabelAttrs.Extensions, DmlExtensionListReader.Read(reader));
                    break;
                default:
                    WarnUnexpectedAndIgnoreElement(xmlReader);
                    break;
            }
        }

        /// <summary>
        /// Reads the [MS-ODRAWXML] 2.6.1.4 dlblFieldTable element.
        /// </summary>
        private static void ReadDataLabelFieldTable(NrxXmlReader xmlReader, DmlChartDataLabelPr labelPr)
        {
            // The property contains a list of field table entries. Each entry corresponds to a text field
            // in a data label whose value is obtained from a formula reference.
            // MS Word has a bug that if there is a space between '</c15:dlblFieldTableCache>' and '</c15:dlblFTEntry>'
            // element close tags, it raises an error while opening the document. Thus, let's read and store entries as
            // plain XML for now.
            // Cannot use XmlUtilPal.ReadOuterXml for the entire dlblFieldTable element because it moves the reader to
            // the next element.

            const string tagName = "dlblFieldTable";
            Debug.Assert(xmlReader.LocalName == tagName);

            List<string> fieldTable = new List<string>();
            xmlReader.ReadChild(tagName);

            while (xmlReader.LocalName != tagName)
            {
                string xml = XmlUtilPal.ReadOuterXml(xmlReader.UnderlyingReader);
                fieldTable.Add(xml);
            }

            if (fieldTable.Count > 0)
                labelPr.SetProperty(DmlChartDataLabelAttrs.FieldTable, fieldTable);
        }

        /// <summary>
        /// Reads CT_ErrBars complex type that specifies error bars.
        /// 5.7.2.55 errBars (Error Bars)
        /// </summary>
        internal static DmlChartErrorBars ReadChartErrorBars(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlChartErrorBars errorBars = new DmlChartErrorBars();

            // Read children
            while (xmlReader.ReadChild("errBars"))
            {
                switch (xmlReader.LocalName)
                {
                    case "errBarType":
                        errorBars.ErrBarType = DmlChartsEnum.DmlToErrorBarType(xmlReader.ReadAttribute(""));
                        break;
                    case "errDir":
                        errorBars.ErrDir = DmlChartsEnum.DmlToErrorBarDirection(xmlReader.ReadAttribute(""));
                        break;
                    case "errValType":
                        errorBars.ErrValType = DmlChartsEnum.DmlToErrorValueType(xmlReader.ReadAttribute(""));
                        break;
                    case "minus":
                        ReadChartDataSource(reader, errorBars.Minus.DataSource);
                        break;
                    case "plus":
                        ReadChartDataSource(reader, errorBars.Plus.DataSource);
                        break;
                    case "noEndCap":
                        errorBars.NoEndCap = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "spPr":
                        ReadChartSpPr(reader, errorBars.SpPr);
                        break;
                    case "val":
                        errorBars.Val = xmlReader.ReadDoubleAttribute(0.0d);
                        break;
                    case "extLst":
                        errorBars.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return errorBars;
        }

        internal static void ReadChartDataSource(DocxDocumentReaderBase reader, DmlChartDataSource dataSource)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            string tagName = xmlReader.LocalName;

            // Read children
            while (xmlReader.ReadChild(tagName))
            {
                if (xmlReader.LocalName == "AlternateContent")
                {
                    ReadDataSourceAlternateContent(reader, dataSource);
                }
                else
                {
                    DmlChartValueRef valueRef = ReadChartDataSourceValueRef(reader);
                    if (valueRef != null)
                        dataSource.ValueRef = valueRef;
                }
            }
        }

        /// <summary>
        /// Reads an AlternateContent element that is used to represent multi-level string literal data in
        /// pre-Word 2016 charts.
        /// </summary>
        private static void ReadDataSourceAlternateContent(DocxDocumentReaderBase reader, DmlChartDataSource dataSource)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string tagName = xmlReader.LocalName;

            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "Choice":
                        ReadDataSourceAlternateContentChoice(reader, dataSource);
                        break;
                    case "Fallback":
                        ReadDataSourceAlternateContentFallback(reader, dataSource);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads a Choice part of an AlternateContent element that is used to represent multi-level string literal data
        /// in pre-Word 2016 charts.
        /// </summary>
        private static void ReadDataSourceAlternateContentChoice(DocxDocumentReaderBase reader,
            DmlChartDataSource dataSource)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("Choice"))
            {
                DmlChartValueRef valueRef = ReadChartDataSourceValueRef(reader);
                if (valueRef != null)
                    dataSource.ValueRef = valueRef;
            }
        }

        /// <summary>
        /// Reads a Fallback part of an AlternateContent element that is used to represent multi-level string literal data
        /// in pre-Word 2016 charts.
        /// </summary>
        private static void ReadDataSourceAlternateContentFallback(DocxDocumentReaderBase reader,
            DmlChartDataSource dataSource)
        {
            Debug.Assert(dataSource.Values != null);

            NrxXmlReader xmlReader = reader.XmlReader;

            while (xmlReader.ReadChild("Fallback"))
            {
                DmlChartValueRef valueRef = ReadChartDataSourceValueRef(reader);
                if (valueRef != null)
                    dataSource.FallbackValueRef = valueRef;
            }
        }

        /// <summary>
        /// Reads reference or literal data of a chart.
        /// </summary>
        private static DmlChartValueRef ReadChartDataSourceValueRef(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            switch (xmlReader.LocalName)
            {
                case "strLit":
                {
                    DmlChartValueCollection values = new DmlChartValueCollection(DmlChartValueType.String);
                    ReadChartValuesCache(reader, values);
                    return new DmlChartValueRef(values);
                }
                case "multiLvlStrLit":
                {
                    DmlChartValueCollection values = new DmlChartValueCollection(DmlChartValueType.MultiLvlString);
                    ReadChartMultiLvlValuesCache(reader, values);
                    return new DmlChartValueRef(values);
                }
                case "numLit":
                {
                    DmlChartValueCollection values = new DmlChartValueCollection(DmlChartValueType.Numeric);
                    ReadChartValuesCache(reader, values);
                    return new DmlChartValueRef(values);
                }
                case "strRef":
                case "numRef":
                case "multiLvlStrRef":
                case "datalabelsRange":
                    return ReadChartValRef(reader);
                default:
                    WarnUnexpectedAndIgnoreElement(xmlReader);
                    return null;
            }
        }

        /// <summary>
        /// Reads CT_Trendline complex type that specifies a trendline.
        /// 5.7.2.212 trendline (Trendlines)
        /// </summary>
        internal static DmlChartTrendline ReadChartTrendline(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlChartTrendline trendline = new DmlChartTrendline();

            // Read children
            while (xmlReader.ReadChild("trendline"))
            {
                switch (xmlReader.LocalName)
                {
                    case "backward":
                        trendline.Backward = xmlReader.ReadDoubleAttribute(0);
                        break;
                    case "dispEq":
                        trendline.DispEq = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "dispRSqr":
                        trendline.DispRSqr = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "forward":
                        trendline.Forward = xmlReader.ReadDoubleAttribute(0);
                        break;
                    case "intercept":
                        trendline.Intercept = xmlReader.ReadDoubleAttribute(0);
                        break;
                    case "name":
                        trendline.Name = xmlReader.ReadString();
                        break;
                    case "order":
                        trendline.Order = xmlReader.ReadIntAttribute(0);
                        break;
                    case "period":
                        trendline.Period = xmlReader.ReadIntAttribute(0);
                        break;
                    case "spPr":
                        ReadChartSpPr(reader, trendline.SpPr);
                        break;
                    case "trendlineLbl":
                        trendline.TrendlineLbl = ReadChartTrendlineLbl(reader);
                        break;
                    case "trendlineType":
                        trendline.TrendlineType = DmlChartsEnum.DmlToTrendlineType(xmlReader.ReadAttribute(""));
                        break;
                    case "extLst":
                        trendline.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return trendline;
        }

        /// <summary>
        /// Reads CT_TrendlineLbl complex type that specifies the label for the trendline.
        /// 5.7.2.213 trendlineLbl (Trendline Label).
        /// </summary>
        private static DmlChartTrendlineLabel ReadChartTrendlineLbl(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlChartTrendlineLabel label = new DmlChartTrendlineLabel();

            // Read children
            while (xmlReader.ReadChild("trendlineLbl"))
            {
                switch (xmlReader.LocalName)
                {
                    case "layout":
                        label.Layout = ReadChartLayout(reader);
                        break;
                    case "numFmt":
                        label.NumFmt = ReadChartNumFormat(xmlReader);
                        break;
                    case "spPr":
                        ReadChartSpPr(reader, label.SpPr);
                        break;
                    case "tx":
                        label.Tx = ReadChartTx(reader);
                        break;
                    case "txPr":
                        ReadChartTxPr(reader, label.TxPr);
                        break;
                    case "extLst":
                        label.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return label;
        }

        /// <summary>
        /// Reads elements of the 2.24.3.26 CT_FormatOverride complex type [MS-ODRAWXML].
        /// </summary>
        internal static DmlChartFormatOverride ReadFormatOverride(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            DmlChartFormatOverride formatOverrid = new DmlChartFormatOverride();
            formatOverrid.Index = xmlReader.ReadIntAttribute("idx", 0);

            // Read children
            while (xmlReader.ReadChild("fmtOvr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "spPr":
                    {
                        formatOverrid.SpPr = new DmlChartSpPr();
                        ReadChartSpPr(reader, formatOverrid.SpPr);
                        break;
                    }
                    case "extLst":
                        formatOverrid.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return formatOverrid;
        }

        /// <summary>
        /// Reads an element of the 2.24.3.81 CT_TickMarks complex type [MS-ODRAWXML].
        /// </summary>
        internal static void ReadTickMarks(DocxDocumentReaderBase reader, DmlChartAxisPr axisPr,
            DmlChartAxisAttrs typeAttribute, DmlChartAxisAttrs extensionAttribute)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string tagName = xmlReader.LocalName;

            axisPr.SetProperty(typeAttribute, DmlChartsEnum.DmlToTickMark(xmlReader.ReadAttribute("type", "")));

            // Read children
            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "extLst":
                        axisPr.SetProperty(extensionAttribute, DmlExtensionListReader.Read(reader));
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads the valueColors element of the 2.24.3.86 CT_ValueColors complex type [MS-ODRAWXML].
        /// </summary>
        internal static DmlChartValueColors ReadValueColors(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlChartValueColors colors = new DmlChartValueColors();

            // Read children
            while (xmlReader.ReadChild("valueColors"))
            {
                switch (xmlReader.LocalName)
                {
                    case "minColor":
                        colors.MinimumColor = DmlColorReader.ReadColor(xmlReader, reader.ComplianceInfo);
                        break;
                    case "midColor":
                        colors.MiddleColor = DmlColorReader.ReadColor(xmlReader, reader.ComplianceInfo);
                        break;
                    case "maxColor":
                        colors.MaximumColor = DmlColorReader.ReadColor(xmlReader, reader.ComplianceInfo);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return colors;
        }

        /// <summary>
        /// Reads the layoutPr element of the 2.24.3.72 CT_SeriesLayoutProperties complex type [MS-ODRAWXML].
        /// </summary>
        internal static void ReadLayoutProperties(DocxDocumentReaderBase reader, DmlChartSeriesLayoutPr layoutPr)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            // Read children
            while (xmlReader.ReadChild("layoutPr"))
            {
                switch (xmlReader.LocalName)
                {
                    case "parentLabelLayout":
                    {
                        layoutPr.SetProperty(DmlChartSeriesLayoutAttr.ParentLabelLayout,
                            DmlChartsEnum.DmlToParentLabelLayout(xmlReader.ReadAttribute("")));
                        break;
                    }
                    case "regionLabelLayout":
                    {
                        layoutPr.SetProperty(DmlChartSeriesLayoutAttr.RegionLabelLayout,
                            DmlChartsEnum.DmlToRegionLabelLayout(xmlReader.ReadAttribute("")));
                        break;
                    }
                    case "visibility":
                        ReadSeriesElementVisibilities(xmlReader, layoutPr);
                        break;
                    case "aggregation":
                    {
                        layoutPr.SetProperty(DmlChartSeriesLayoutAttr.IsAggregation,
                            true);
                        break;
                    }
                    case "binning":
                    {
                        layoutPr.SetProperty(DmlChartSeriesLayoutAttr.Binning,
                            ReadBinning(xmlReader));
                        break;
                    }
                    case "statistics":
                    {
                        layoutPr.SetProperty(DmlChartSeriesLayoutAttr.QuartileMethod,
                            DmlChartsEnum.DmlToQuartileMethod(xmlReader.ReadAttribute("quartileMethod", "")));
                        break;
                    }
                    case "subtotals":
                    {
                        layoutPr.SetProperty(DmlChartSeriesLayoutAttr.Subtotals,
                            ReadSubtotals(xmlReader));
                        break;
                    }
                    case "extLst":
                    {
                        layoutPr.SetProperty(DmlChartSeriesLayoutAttr.Extensions,
                            DmlExtensionListReader.Read(reader));
                        break;
                    }
                    case "geography":
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads an element of the 2.24.3.71 CT_SeriesElementVisibilities complex type [MS-ODRAWXML].
        /// </summary>
        private static void ReadSeriesElementVisibilities(NrxXmlReader xmlReader, DmlChartSeriesLayoutPr layoutPr)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "connectorLines":
                        layoutPr.SetProperty(DmlChartSeriesLayoutAttr.IsConnectorLinesVisible, xmlReader.ValueAsBool);
                        break;
                    case "meanLine":
                        layoutPr.SetProperty(DmlChartSeriesLayoutAttr.IsMeanLineVisible, xmlReader.ValueAsBool);
                        break;
                    case "meanMarker":
                        layoutPr.SetProperty(DmlChartSeriesLayoutAttr.IsMeanMarkerVisible, xmlReader.ValueAsBool);
                        break;
                    case "nonoutliers":
                        layoutPr.SetProperty(DmlChartSeriesLayoutAttr.IsNonOutliersVisible, xmlReader.ValueAsBool);
                        break;
                    case "outliers":
                        layoutPr.SetProperty(DmlChartSeriesLayoutAttr.IsOutliersVisible, xmlReader.ValueAsBool);
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }

            xmlReader.MoveToElement();
        }

        /// <summary>
        /// Reads the binning element of the 2.24.3.7 CT_Binning complex type [MS-ODRAWXML].
        /// </summary>
        private static DmlChartBinningPr ReadBinning(NrxXmlReader xmlReader)
        {
            DmlChartBinningPr binningPr = new DmlChartBinningPr();

            // Read attributes
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "intervalClosed":
                    {
                        binningPr.SetProperty(DmlChartBinningAttr.IntervalClosed,
                            DmlChartsEnum.DmlToIntervalClosedSide(xmlReader.Value));
                        break;
                    }
                    case "underflow":
                        ReadDoubleOrAutomatic(xmlReader,
                            (DoubleOrAutomatic)binningPr.GetProperty(DmlChartBinningAttr.Underflow));
                        break;
                    case "overflow":
                        ReadDoubleOrAutomatic(xmlReader,
                            (DoubleOrAutomatic)binningPr.GetProperty(DmlChartBinningAttr.Overflow));
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }
            xmlReader.MoveToElement();

            // Read children
            while (xmlReader.ReadChild("binning"))
            {
                switch (xmlReader.LocalName)
                {
                    case "binSize":
                        binningPr.SetProperty(DmlChartBinningAttr.BinSize, xmlReader.ReadDoubleAttribute(1));
                        break;
                    case "binCount":
                        binningPr.SetProperty(DmlChartBinningAttr.BinCount, xmlReader.ReadIntAttribute(6));
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return binningPr;
        }

        /// <summary>
        /// Reads the subtotals element of the 2.24.3.77 CT_Subtotals complex type [MS-ODRAWXML].
        /// </summary>
        private static List<int> ReadSubtotals(NrxXmlReader xmlReader)
        {
            List<int> subtotals = new List<int>();

            // Read children
            while (xmlReader.ReadChild("subtotals"))
            {
                switch (xmlReader.LocalName)
                {
                    case "idx":
                        subtotals.Add(xmlReader.ReadIntAttribute(0));
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            return subtotals;
        }
    }
}
