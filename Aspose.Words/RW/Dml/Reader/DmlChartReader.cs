// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/06/2012 by Alexey Noskov

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Class is used to read a chart part.
    /// This is root class for reading charts.
    /// </summary>
    internal class DmlChartReader : DmlReaderBase
    {
        private DmlChartReader(DocxDocumentReaderBase reader)
        {
            mDocumentReader = reader;
        }

        /// <summary>
        /// Builds <see cref="DmlChart"/>, xmlReader current node must be 5.7.2.26 chart (Reference to Chart Part).
        /// </summary>
        internal static Shape Read(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            if (xmlReader.LocalName != "chart")
                return null;

            string chartId = null;

            // Read id of the chart part and get it.
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "id":
                        chartId = xmlReader.Value;
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            if (!StringUtil.HasChars(chartId))
                return null;

            DocxXmlReader chartSpaceReader = reader.SwitchToPartReaderByRelId(chartId);

            Shape chartDml = new Shape(reader.Document, ShapeMarkupLanguage.Dml);
            reader.AddAndPushContainer(chartDml);

            bool isChartEx =
                chartSpaceReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartEx, false);
            DmlChartSpace chartSpace = new DmlChartSpace(isChartEx);
            chartDml.DmlNode = chartSpace;
            Read(reader, chartSpace);

            // Read theme override if exists.
            chartSpace.ThemeOverride= DocxThemeReader.ReadThemeOverride(reader);

            bool isIsoStrict = reader.ComplianceInfo.IsIsoStrict;

            if (reader.SwitchToPartReaderByRelType(chartSpaceReader.Part,
                    DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartColorStyle, isIsoStrict)) != null)
            {
                chartSpace.ColorStyle = DmlChartColorStyleReader.Read(reader);
                reader.RestorePartReader();
            }
            if (reader.SwitchToPartReaderByRelType(chartSpaceReader.Part,
                    DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartStyle, isIsoStrict)) != null)
            {
                chartSpace.DmlChartStyle = DmlChartStyleReader.Read(reader);
                reader.RestorePartReader();
            }

            reader.PopContainer(NodeType.Shape);
            reader.RestorePartReader();

            return chartDml;
        }

        /// <summary>
        /// Builds <see cref="DmlChartSpace"/>. xmlReader must read chart part.
        /// </summary>
        internal static void Read(DocxDocumentReaderBase reader, DmlChartSpace chartSpace)
        {
            BuildChartSpace(reader, chartSpace);
        }

        /// <summary>
        /// Method reads root element of chart part.
        /// </summary>
        private static void BuildChartSpace(DocxDocumentReaderBase reader, DmlChartSpace chartSpace)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            DmlChartReader chartReader = new DmlChartReader(reader);

            while (xmlReader.ReadChild("chartSpace"))
            {
                switch (xmlReader.LocalName)
                {
                    case "chart":
                        chartSpace.ChartFormat = chartReader.BuildChartFormat();
                        break;
                    case "date1904":
                        chartSpace.Date1904 = xmlReader.ReadBoolAttribute(false);
                        break;
                    case "externalData":
                        ReadExternalData(chartSpace, reader);
                        break;
                    case "lang":
                        chartSpace.Lang = xmlReader.ReadAttribute("");
                        break;
                    case "pivotSource":
                        chartSpace.PivotSource = DmlChartComplexTypesReader.ReadChartPivotSource(reader);
                        break;
                    case "protection":
                        chartSpace.Protection = DmlChartComplexTypesReader.ReadChartProtection(xmlReader);
                        break;
                    case "roundedCorners":
                        chartSpace.RoundedCorners = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "spPr":
                        DmlChartComplexTypesReader.ReadChartSpPr(reader, chartSpace.SpPr);
                        break;
                    case "style":
                        chartSpace.StyleIndex = xmlReader.ReadIntAttribute(0);
                        break;
                    case "txPr":
                        DmlChartComplexTypesReader.ReadChartTxPr(reader, chartSpace.TxPr);
                        break;
                    case "userShapes":
                    {
                        string userShapesId = xmlReader.ReadAttribute("id", "");
                        DmlChartShapesReader.Read(reader, userShapesId);
                        break;
                    }
                    case "AlternateContent":
                        // WORDSNET-8006 Try to read style from fallback.
                        // It seems MS Word 2013 always use AlternateContent to set style of the charts.
                        chartReader.TryReadFallBackStyle(chartSpace);
                        break;
                    case "clrMapOvr":
                        chartSpace.ColorMapOverride = ReadColorMapOverride(reader);
                        break;
                    case "extLst":
                        chartSpace.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    case "printSettings":
                        WarnNotSupportedAndIgnoreElement(xmlReader);
                        break;
                    case "chartData": // 2.24.3.10 CT_ChartData [MS-ODRAWXML]
                        ReadChartData(chartSpace, reader);
                        break;
                    case "fmtOvrs":   // 2.24.3.27 CT_FormatOverrides [MS-ODRAWXML]
                        ReadFormatOverrides(chartSpace.FormatOverrides, reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads elements of the 2.24.3.10 CT_ChartData complex type [MS-ODRAWXML].
        /// </summary>
        private static void ReadChartData(DmlChartSpace chartSpace, DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            xmlReader.MoveToElement();
            while (xmlReader.ReadChild("chartData"))
            {
                switch (xmlReader.LocalName)
                {
                    case "externalData":
                        ReadExternalData(chartSpace, reader);
                        break;
                    case "data":
                        ReadData(chartSpace, reader);
                        break;
                    case "extLst":
                        chartSpace.Data.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads elements of the 2.24.3.27 CT_FormatOverrides complex type [MS-ODRAWXML].
        /// </summary>
        private static void ReadFormatOverrides(IList<DmlChartFormatOverride> list, DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            xmlReader.MoveToElement();
            while (xmlReader.ReadChild("fmtOvrs"))
            {
                switch (xmlReader.LocalName)
                {
                    case "fmtOvr":
                        list.Add(DmlChartComplexTypesReader.ReadFormatOverride(reader));
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        private static DmlChartColorMapOverride ReadColorMapOverride(DocxDocumentReaderBase reader)
        {
            DmlChartColorMapOverride colorMapOverride = new DmlChartColorMapOverride();

            NrxXmlReader xmlReader = reader.XmlReader;
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "bg1":
                        colorMapOverride.Bg1 = xmlReader.Value;
                        break;
                    case "tx1": colorMapOverride.Tx1 = xmlReader.Value;
                        break;
                    case "bg2": colorMapOverride.Bg2 = xmlReader.Value;
                        break;
                    case "tx2": colorMapOverride.Tx2 = xmlReader.Value;
                        break;
                    case "accent1": colorMapOverride.Accent1 = xmlReader.Value;
                        break;
                    case "accent2": colorMapOverride.Accent2 = xmlReader.Value;
                        break;
                    case "accent3": colorMapOverride.Accent3 = xmlReader.Value;
                        break;
                    case "accent4": colorMapOverride.Accent4 = xmlReader.Value;
                        break;
                    case "accent5": colorMapOverride.Accent5 = xmlReader.Value;
                        break;
                    case "accent6": colorMapOverride.Accent6 = xmlReader.Value;
                        break;
                    case "hlink": colorMapOverride.Hlink = xmlReader.Value;
                        break;
                    case "folHlink": colorMapOverride.FolHlink = xmlReader.Value;
                        break;
                    default:
                        break;
                }
            }

            xmlReader.MoveToElement();

            while (xmlReader.ReadChild("clrMapOvr"))
                WarnNotSupportedAndIgnoreElement(xmlReader);

            return colorMapOverride;
        }

        private static void ReadExternalData(DmlChartSpace chartSpace, DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            string externalDataId = xmlReader.ReadAttribute("id", "");
            if (!StringUtil.HasChars(externalDataId))
                return;

            // Try reading external data as embedded object first.
            chartSpace.EmbeddedData = reader.GetEmbeddedObject(externalDataId);

            // If embedded data is null, suppose data is linked.
            if (chartSpace.EmbeddedData == null)
                chartSpace.LinkedData = reader.GetRelationshipTarget(externalDataId);

            // autoUpdate is attribute in the chartEx scheme.
            chartSpace.AutoUpdate = xmlReader.ReadBoolAttribute("autoUpdate", true);

            xmlReader.MoveToElement();
            while (xmlReader.ReadChild("externalData"))
            {
                switch (xmlReader.LocalName)
                {
                    case "autoUpdate":
                        chartSpace.AutoUpdate = xmlReader.ReadBoolAttribute(true);
                        break;
                    default:
                           WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads elements of the 2.24.3.15 CT_Data complex type [MS-ODRAWXML].
        /// </summary>
        private static void ReadData(DmlChartSpace chartSpace, DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;

            DmlChartData data = new DmlChartData();
            data.Id = xmlReader.ReadIntAttribute("id", -1);

            while (xmlReader.ReadChild("data"))
            {
                switch (xmlReader.LocalName)
                {
                    case "numDim":
                    case "strDim":
                    {
                        DmlChartDataSource dataSource = new DmlChartDataSource();
                        dataSource.ValueRef = DmlChartComplexTypesReader.ReadChartValRef(reader);
                        data.DataSources.Add(dataSource);
                        break;
                    }
                    case "extLst":
                        data.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            chartSpace.Data.Add(data);
        }

        /// <summary>
        /// Method tries to read style from fallback.
        /// IF there is no style set in fallback, simply does nothing.
        /// </summary>
        private void TryReadFallBackStyle(DmlChartSpace chartSpace)
        {
            while (XmlReader.ReadChild("AlternateContent"))
            {
                switch (XmlReader.LocalName)
                {
                    case "Fallback":
                    {
                        while (XmlReader.ReadChild("Fallback"))
                        {
                            switch (XmlReader.LocalName)
                            {
                                case "style":
                                    chartSpace.StyleIndex = XmlReader.ReadIntAttribute(0);
                                    break;
                                default:
                                    WarnNotSupportedAndIgnoreElement(XmlReader);
                                    break;
                            }
                        }
                        break;
                    }
                    case "Choice":
                        while (XmlReader.ReadChild("Choice"))
                        {
                            switch (XmlReader.LocalName)
                            {
                                case "style":
                                    chartSpace.Word2010Style = XmlReader.ReadIntAttribute(0);
                                    break;
                                default:
                                    WarnNotSupportedAndIgnoreElement(XmlReader);
                                    break;
                            }
                        }
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 'chart' element.
        /// </summary>
        private DmlChartFormat BuildChartFormat()
        {
            DmlChartFormat chartFormat = new DmlChartFormat();
            bool isChartEx =
                XmlReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartEx, false);

            while (XmlReader.ReadChild("chart"))
            {
                switch (XmlReader.LocalName)
                {
                    case "autoTitleDeleted":
                        chartFormat.AutoTitleDeleted = XmlReader.ReadBoolAttribute(true);
                        break;
                    case "backWall":
                        chartFormat.BackWall = DmlChartComplexTypesReader.ReadChartSurface(mDocumentReader);
                        break;
                    case "dispBlanksAs":
                        chartFormat.DispBlanksAs = DmlChartsEnum.DmlToDisplayBlanksAs(XmlReader.ReadAttribute(""));
                        break;
                    case "floor":
                        chartFormat.Floor = DmlChartComplexTypesReader.ReadChartSurface(mDocumentReader);
                        break;
                    case "legend":
                        chartFormat.Legend = DmlChartComplexTypesReader.ReadChartLegend(mDocumentReader, chartFormat);
                        break;
                    case "pivotFmts":
                        chartFormat.PivotFmts = DmlChartComplexTypesReader.ReadChartPivotFormats(mDocumentReader);
                        break;
                    case "plotArea":
                        chartFormat.PlotArea = BuildPlotArea(chartFormat);
                        break;
                    case "plotVisOnly":
                        chartFormat.PlotVisOnly = XmlReader.ReadBoolAttribute(true);
                        break;
                    case "showDLblsOverMax":
                        chartFormat.ShowDLblsOverMax = XmlReader.ReadBoolAttribute(true);
                        break;
                    case "sideWall":
                        chartFormat.SideWall = DmlChartComplexTypesReader.ReadChartSurface(mDocumentReader);
                        break;
                    case "title":
                        chartFormat.DCTitle = DmlChartComplexTypesReader.ReadChartTitle(chartFormat, mDocumentReader);
                        break;
                    case "view3D":
                        chartFormat.View3D = DmlChartComplexTypesReader.ReadChartView3D(mDocumentReader);
                        break;
                    case "extLst":
                        chartFormat.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            if (isChartEx && (chartFormat.DCTitle == null))
                chartFormat.AutoTitleDeleted = true;

            return chartFormat;
        }

        /// <summary>
        /// Reads 'plotArea' element.
        /// </summary>
        private DmlChartPlotArea BuildPlotArea(DmlChartFormat chartFormat)
        {
            DmlChartPlotArea plotArea = new DmlChartPlotArea(chartFormat);
            if (XmlReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartEx, false))
            {
                DmlChartExChart chart = new DmlChartExChart();
                // Need to assign a document: the defaults depend on the document version.
                chart.Document = mDocumentReader.Document;
                plotArea.AddChart(chart);
            }

            while (XmlReader.ReadChild("plotArea"))
            {
                switch (XmlReader.LocalName)
                {
                    case "areaChart":
                    case "area3DChart":
                    case "lineChart":
                    case "line3DChart":
                    case "stockChart":
                    case "radarChart":
                    case "scatterChart":
                    case "pieChart":
                    case "pie3DChart":
                    case "doughnutChart":
                    case "barChart":
                    case "bar3DChart":
                    case "ofPieChart":
                    case "surfaceChart":
                    case "surface3DChart":
                    case "bubbleChart":
                        plotArea.AddChart(BuildChart(mDocumentReader.Document));
                        break;
                    case "valAx":
                    case "catAx":
                    case "dateAx":
                    case "serAx":
                        plotArea.AddAxis(DmlChartAxisReader.Read(mDocumentReader, plotArea.FirstChart, false));
                        break;
                    case "layout":
                        plotArea.Layout = DmlChartComplexTypesReader.ReadChartLayout(mDocumentReader);
                        break;
                    case "dTable":
                        plotArea.DataTable = DmlChartComplexTypesReader.ReadChartDataTable(mDocumentReader, plotArea);
                        break;
                    case "spPr":
                        DmlChartComplexTypesReader.ReadChartSpPr(mDocumentReader, plotArea.SpPr);
                        break;
                    case "plotAreaRegion": // 2.24.3.65 CT_PlotAreaRegion [MS-ODRAWXML]
                        ReadPlotAreaRegion(plotArea);
                        break;
                    case "axis": // 2.24.3.3 CT_Axis [MS-ODRAWXML]
                        plotArea.AddAxis(DmlChartAxisReader.Read(mDocumentReader, plotArea.FirstChart, true));
                        break;
                    case "extLst":
                        plotArea.Extensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            // WORDSNET-23232 Chart data for the axis may be null when an axis follows before a chart node in the markup.
            // Let's set first chart from the plot area collection as works the logic above.
            foreach (ChartAxis axis in plotArea.Axes)
            {
                if (axis.Chart == null)
                    axis.SetChart(plotArea.FirstChart);
            }

            plotArea.InitAxisDirection();

            return plotArea;
        }

        /// <summary>
        /// Reads the plotAreaRegion element of the 2.24.3.65 CT_PlotAreaRegion complex type [MS-ODRAWXML].
        /// </summary>
        private void ReadPlotAreaRegion(DmlChartPlotArea plotArea)
        {
            DmlChart chart = plotArea.FirstChart;
            ObjToIntDictionary<ChartSeries> ownerMap = new ObjToIntDictionary<ChartSeries>();

            while (XmlReader.ReadChild("plotAreaRegion"))
            {
                switch (XmlReader.LocalName)
                {
                    case "plotSurface":
                        plotArea.Surface = DmlChartComplexTypesReader.ReadChartSurface(mDocumentReader);
                        break;
                    case "series":
                        chart.Series.AddForLoad(DmlChartSeriesReader.Build(mDocumentReader, chart, ownerMap));
                        break;
                    case "extLst":
                        plotArea.RegionExtensions = DmlExtensionListReader.Read(mDocumentReader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            ResolveSeriesOwners(chart.Series, ownerMap);
        }

        /// <summary>
        /// Resolves series owners by their indexes.
        /// </summary>
        /// <param name="seriesCollection">A series collection.</param>
        /// <param name="ownerMap">A map of owned series to its owner index.</param>
        private static void ResolveSeriesOwners(DmlChartSeriesCollection seriesCollection,
            ObjToIntDictionary<ChartSeries> ownerMap)
        {
            foreach (ChartSeries series in ownerMap.Keys)
            {
                int ownerIndex = ownerMap[series];
                series.Owner = seriesCollection[ownerIndex];
            }
        }

        private DmlChart BuildChart(DocumentBase doc)
        {
            string tagName = XmlReader.LocalName;
            DmlChartType chartType = GetChartType(tagName);
            DmlChart chart = DmlChartFactory.CreateChart(chartType);
            // WORDSNET-20873 The defaults depend on the document version.
            chart.Document = doc;

            while (XmlReader.ReadChild(tagName))
            {
                switch (XmlReader.LocalName)
                {
                    case "axId":
                        chart.AddAxId(XmlReader.ReadIntAttribute(0));
                        break;
                    case "dLbls":
                        chart.ChartPr.SetProperty(DmlChartAttrs.DLbls,
                            DmlChartComplexTypesReader.ReadChartDataLabelCollection(mDocumentReader, chart, null));
                        break;
                    case "dropLines":
                    {
                        chart.ChartPr.SetProperty(DmlChartAttrs.IsDropLinesVisible, true);
                        DmlChartComplexTypesReader.ReadChartSpPrContainer(mDocumentReader, (DmlChartSpPr)chart.ChartPr.GetProperty(DmlChartAttrs.DropLines));
                        break;
                    }
                    case "ser":
                    {
                        // WORDSNET-7285 Add series to the collection only if it is not empty.
                        // It seems, MS Word ignores series with no data.
                        ChartSeries series = DmlChartSeriesReader.Build(mDocumentReader, chart, null);
                        if (series.ValueCount > 0)
                            chart.Series.AddForLoad(series);
                        break;
                    }
                    case "varyColors":
                    chart.ChartPr.SetProperty(DmlChartAttrs.VaryColors, XmlReader.ReadBoolAttribute(true));
                        break;
                    case "grouping":
                        chart.ChartPr.Grouping = DmlChartsEnum.DmlToGrouping(XmlReader.ReadAttribute(""));
                        break;
                    case "gapDepth":
                        chart.ChartPr.SetProperty(DmlChartAttrs.GapDepth, XmlReader.ReadIntAttribute(150));
                        break;
                    case "barDir":
                        chart.ChartPr.SetProperty(DmlChartAttrs.BarDir, DmlChartsEnum.DmlToBarDirection(XmlReader.ReadAttribute("")));
                        break;
                    case "gapWidth":
                        chart.ChartPr.SetProperty(DmlChartAttrs.GapWidth, XmlReader.ReadIntAttribute(150));
                        break;
                    case "shape":
                        chart.ChartPr.SetProperty(DmlChartAttrs.Shape, DmlChartsEnum.DmlToShape(XmlReader.ReadAttribute("")));
                        break;
                    case "overlap":
                        chart.ChartPr.SetProperty(DmlChartAttrs.Overlap, XmlReader.ReadIntAttribute(0));
                        break;
                    case "serLines":
                        chart.ChartPr.SetProperty(DmlChartAttrs.ShowSerLine, true);
                        DmlChartComplexTypesReader.ReadChartSpPrContainer(mDocumentReader, (DmlChartSpPr)chart.ChartPr.GetProperty(DmlChartAttrs.SerLines));
                        break;
                    case "bubbleScale":
                        chart.ChartPr.SetProperty(DmlChartAttrs.BubbleScale, XmlReader.ReadIntAttribute(0));
                        break;
                    case "showNegBubbles":
                        chart.ChartPr.SetProperty(DmlChartAttrs.ShowNegBubbles, XmlReader.ReadBoolAttribute(true));
                        break;
                    case "sizeRepresents":
                        chart.ChartPr.SetProperty(DmlChartAttrs.SizeRepresents, DmlChartsEnum.DmlToSizeRepresents(XmlReader.ReadAttribute("")));
                        break;
                    case "firstSliceAng":
                        chart.ChartPr.SetProperty(DmlChartAttrs.FirstSliceAng, XmlReader.ReadIntAttribute(0));
                        break;
                    case "holeSize":
                        chart.ChartPr.SetProperty(DmlChartAttrs.HoleSize, XmlReader.ReadIntAttribute(10));
                        break;
                    case "hiLowLines":
                    {
                        chart.ChartPr.SetProperty(DmlChartAttrs.IsHiLowLinesVisible, true);
                        DmlChartComplexTypesReader.ReadChartSpPrContainer(mDocumentReader, (DmlChartSpPr)chart.ChartPr.GetProperty(DmlChartAttrs.HiLowLines));
                        break;
                    }
                    case "marker":
                    chart.ChartPr.SetProperty(DmlChartAttrs.ShowMarker, XmlReader.ReadBoolAttribute(true));
                        break;
                    case "smooth":
                        chart.ChartPr.SetProperty(DmlChartAttrs.Smooth, XmlReader.ReadBoolAttribute(true));
                        break;
                    case "upDownBars":
                    {
                        DmlChartUpDownBars upDownBars = new DmlChartUpDownBars();
                        DmlChartComplexTypesReader.ReadChartUpDownBars(mDocumentReader, upDownBars);
                        chart.ChartPr.SetProperty(DmlChartAttrs.UpDownBars, upDownBars);
                        break;
                    }
                    case "custSplit":
                        chart.ChartPr.SetProperty(DmlChartAttrs.CustSplit, DmlChartComplexTypesReader.ReadChartCustomSplit(XmlReader));
                        break;
                    case "ofPieType":
                        chart.ChartPr.OfPieType = DmlChartsEnum.DmlToOfPieType(XmlReader.ReadAttribute(""));
                        break;
                    case "secondPieSize":
                        chart.ChartPr.SetProperty(DmlChartAttrs.SecondPieSize, XmlReader.ReadIntAttribute(75));
                        break;
                    case "splitPos":
                        chart.ChartPr.SetProperty(DmlChartAttrs.SplitPos, XmlReader.ReadDoubleAttribute(0));
                        break;
                    case "splitType":
                        chart.ChartPr.SetProperty(DmlChartAttrs.SplitType, DmlChartsEnum.DmlToSplitType(XmlReader.ReadAttribute("")));
                        break;
                    case "radarStyle":
                        chart.ChartPr.SetProperty(DmlChartAttrs.RadarStyle, DmlChartsEnum.DmlToRadarStyle(XmlReader.ReadAttribute("")));
                        break;
                    case "scatterStyle":
                        chart.ChartPr.SetProperty(DmlChartAttrs.ScatterStyle, DmlChartsEnum.DmlToScatterStyle(XmlReader.ReadAttribute("")));
                        break;
                    case "bandFmts":
                        DmlChartComplexTypesReader.ReadChartBandFmts(mDocumentReader, (DmlChartBandFormats)chart.ChartPr.GetProperty(DmlChartAttrs.BandFmts));
                        break;
                    case "wireframe":
                        chart.ChartPr.SetProperty(DmlChartAttrs.Wireframe, XmlReader.ReadBoolAttribute(true));
                        break;
                    case "extLst":
                        chart.ChartPr.SetProperty(DmlChartAttrs.Extensions, DmlExtensionListReader.Read(mDocumentReader));
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(XmlReader);
                        break;
                }
            }

            if ((chart is DmlBubbleChart) && (chart.Series.Count > 0) && chart.Series[0].Bubble3D)
                ((DmlBubbleChart)chart).Bubble3D = true;

            return chart;
        }

        private static DmlChartType GetChartType(string tagName)
        {
            switch (tagName)
            {
                case "area3DChart":
                    return DmlChartType.Area3DChart;
                case "areaChart":
                    return DmlChartType.AreaChart;
                case "bar3DChart":
                    return DmlChartType.Bar3DChart;
                case "barChart":
                    return DmlChartType.BarChart;
                case "bubbleChart":
                    return DmlChartType.BubbleChart;
                case "doughnutChart":
                    return DmlChartType.DoughnutChart;
                case "line3DChart":
                    return DmlChartType.Line3DChart;
                case "lineChart":
                    return DmlChartType.LineChart;
                case "ofPieChart":
                    return DmlChartType.OfPieChart;
                case "pie3DChart":
                    return DmlChartType.Pie3DChart;
                case "pieChart":
                    return DmlChartType.PieChart;
                case "radarChart":
                    return DmlChartType.RadarChart;
                case "scatterChart":
                    return DmlChartType.ScatterChart;
                case "stockChart":
                    return DmlChartType.StockChart;
                case "surface3DChart":
                    return DmlChartType.Surface3DChart;
                case "surfaceChart":
                    return DmlChartType.SurfaceChart;
                default:
                    throw new ArgumentException("Unexpected tag name");
            }
        }

        private NrxXmlReader XmlReader
        {
            get { return mDocumentReader.XmlReader; }
        }

        private readonly DocxDocumentReaderBase mDocumentReader;
    }
}
