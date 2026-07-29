// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/06/2012 by Alexey Noskov

using Aspose.Collections;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// Class is used to read series of charts.
    /// 5.7.2.168 ser (Bubble Chart Series)
    /// 5.7.2.169 ser (Line Chart Series)
    /// 5.7.2.170 ser (Pie Chart Series)
    /// 5.7.2.171 ser (Surface Chart Series)
    /// 5.7.2.172 ser (Scatter Chart Series)
    /// 5.7.2.173 ser (Radar Chart Series)
    /// 5.7.2.174 ser (Area Chart Series)
    /// 5.7.2.175 ser (Bar Chart Series)
    /// </summary>
    internal class DmlChartSeriesReader : DmlReaderBase
    {
        private DmlChartSeriesReader()
        {
        }

        internal static ChartSeries Build(DocxDocumentReaderBase reader, DmlChart chart, ObjToIntDictionary<ChartSeries> ownerMap)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            ChartSeries series = new ChartSeries(chart);
            string tagName = xmlReader.LocalName;

            if (xmlReader.NamespaceURI == DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChartEx, false))
            {
                while (xmlReader.MoveToNextAttribute())
                {
                    switch (xmlReader.LocalName)
                    {
                        case "layoutId":
                            series.LayoutType = DmlChartsEnum.DmlToSeriesLayout(xmlReader.Value);
                            break;
                        case "hidden":
                            series.Hidden = xmlReader.ValueAsBool;
                            break;
                        case "ownerIdx":
                        {
                            int index = xmlReader.ValueAsInt;
                            if (index < chart.Series.Count)
                                series.Owner = chart.Series[index];
                            else
                                ownerMap.Add(series, index);
                            break;
                        }
                        case "uniqueId":
                            series.UniqueId = xmlReader.Value;
                            break;
                        case "formatIdx":
                            series.FormatIndex = xmlReader.ValueAsInt;
                            break;
                        default:
                            WarnUnexpected(xmlReader);
                            break;
                    }
                }

                xmlReader.MoveToElement();
            }

            while (xmlReader.ReadChild(tagName))
            {
                switch (xmlReader.LocalName)
                {
                    case "idx":
                        series.Index = xmlReader.ReadIntAttribute(0);
                        break;
                    case "order":
                        series.Order = xmlReader.ReadIntAttribute(0);
                        break;
                    case "explosion":
                        series.DefaultDataPoint.PointPr.SetProperty(DmlChartDataPointAttr.Explosion, xmlReader.ReadIntAttribute(0));
                        break;
                    case "invertIfNegative":
                        series.DefaultDataPoint.PointPr.SetProperty(DmlChartDataPointAttr.InvertIfNegative, xmlReader.ReadBoolAttribute(true));
                        break;
                    case "bubble3D":
                        series.DefaultDataPoint.PointPr.SetProperty(DmlChartDataPointAttr.Bubble3D, xmlReader.ReadBoolAttribute(true));
                        break;
                    case "smooth":
                        series.Smooth = xmlReader.ReadBoolAttribute(true);
                        break;
                    case "shape":
                        series.Shape = DmlChartsEnum.DmlToShape(xmlReader.ReadAttribute(""));
                        break;
                    case "cat":
                    case "xVal":
                        DmlChartComplexTypesReader.ReadChartDataSource(reader, series.X.DataSource);
                        break;
                    case "val":
                    case "yVal":
                        DmlChartComplexTypesReader.ReadChartDataSource(reader, series.Y.DataSource);
                        break;
                    case "bubbleSize":
                        DmlChartComplexTypesReader.ReadChartDataSource(reader, series.Size.DataSource);
                        break;
                    case "errBars":
                        series.AddErrorBars(DmlChartComplexTypesReader.ReadChartErrorBars(reader));
                        break;
                    case "marker":
                    {
                        ChartMarker marker = new ChartMarker(chart);
                        DmlChartComplexTypesReader.ReadChartMarker(reader, marker);
                        series.DefaultDataPoint.PointPr.SetProperty(DmlChartDataPointAttr.Marker, marker);
                        break;
                    }
                    case "dLbls":
                    case "dataLabels": // in the 2.24.3.70 CT_Series type [MS-ODRAWXML]
                        series.DataLabels = DmlChartComplexTypesReader.ReadChartDataLabelCollection(reader, chart, series);
                        break;
                    case "dPt":
                    case "dataPt": // in the 2.24.3.70 CT_Series type [MS-ODRAWXML]
                        series.DataPoints.AddDataPoint(DmlChartComplexTypesReader.ReadChartDataPoint(reader, chart));
                        break;
                    case "pictureOptions":
                        series.DefaultDataPoint.PointPr.SetProperty(DmlChartDataPointAttr.PictureOptions, DmlChartComplexTypesReader.ReadChartPictureOptions(xmlReader));
                        break;
                    case "trendline":
                        series.AddTrendline(DmlChartComplexTypesReader.ReadChartTrendline(reader));
                        break;
                    case "tx":
                        series.Tx = DmlChartComplexTypesReader.ReadChartTx(reader);
                        break;
                    case "spPr":
                    {
                        DmlChartSpPr spPr = new DmlChartSpPr();
                        DmlChartComplexTypesReader.ReadChartSpPr(reader, spPr);
                        series.DefaultDataPoint.PointPr.SetProperty(DmlChartDataPointAttr.SpPr, spPr);
                        break;
                    }
                    case "valueColors": // in the 2.24.3.70 CT_Series type [MS-ODRAWXML]
                        series.ValueColors = DmlChartComplexTypesReader.ReadValueColors(reader);
                        break;
                    case "dataId": // in the 2.24.3.70 CT_Series type [MS-ODRAWXML]
                        series.DataId = xmlReader.ReadIntAttribute(-1);
                        break;
                    case "axisId": // in the 2.24.3.70 CT_Series type [MS-ODRAWXML]
                        series.AxisId = xmlReader.ReadIntAttribute(-1);
                        break;
                    case "layoutPr": // in the 2.24.3.70 CT_Series type [MS-ODRAWXML]
                        DmlChartComplexTypesReader.ReadLayoutProperties(reader, series.LayoutPr);
                        break;
                    case "extLst":
                        ((IDmlExtensionListSource)series).Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        WarnUnexpectedAndIgnoreElement(xmlReader);
                        break;
                }
            }

            NormalizeDataPointInvertIfNegative(series);

            series.EndReading();

            return series;
        }

        /// <summary>
        /// Normalizes the <see cref="ChartDataPoint.InvertIfNegative"/> property values of materialized data points of
        /// the series.
        /// </summary>
        private static void NormalizeDataPointInvertIfNegative(ChartSeries series)
        {
            // 1. The "invertIfNegative" property does not get value from the parent collection (series) in MS Word. Thus,
            // being defined for a data point collection (series), the same value has to be written for all materialized
            // data points that should have the same property value. So, let's collapse the value here so that changing
            // the property in the collection affects such data points too.
            // 2. If the property is not defined in a data point but is defined in the collection (series), and if the
            // value is different than the default value of the property, we need put the default value to the data point.

            DmlChartDataPointPr seriesPointPr = series.DefaultDataPoint.PointPr;
            if (!seriesPointPr.IsPropertySpecified(DmlChartDataPointAttr.InvertIfNegative))
                return;

            bool seriesValue = series.InvertIfNegative;
            // The default value depends on a document version.
            bool defaultValue = (bool)seriesPointPr.GetInheritedProperty(DmlChartDataPointAttr.InvertIfNegative);

            foreach (ChartDataPoint dataPoint in series.DataPoints.MaterializedDataPoints)
            {
                if (dataPoint.PointPr.IsPropertySpecified(DmlChartDataPointAttr.InvertIfNegative))
                {
                    if (dataPoint.InvertIfNegative == seriesValue)
                        dataPoint.PointPr.RemoveProperty(DmlChartDataPointAttr.InvertIfNegative);
                }
                else
                {
                    if (seriesValue != defaultValue)
                        dataPoint.InvertIfNegative = defaultValue;
                }
            }
        }
    }
}
