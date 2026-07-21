// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2025 by Alexander Zhiltsov

using System.IO;
using Aspose.IO;
using Aspose.OpcPackaging;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.RW.Dml.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Reads preset chart types and chart styles from the embedded resources.
    /// </summary>
    internal static class PresetChartReader
    {
        /// <summary>
        /// Reads chart of specific type from the Aspose.Words.Resources.Charts.ChartTypes.xml
        /// </summary>
        public static void ReadPresetChart(DmlChartSpace chartSpace, ChartType chartType, int documentVersion)
        {
            const string resourceName = "Aspose.Words.Resources.Charts.ChartTypes.xml";

            using (Stream stream = ResourceUtil.FetchResourceStream(resourceName))
            {
                stream.Position = 0;

                DocxXmlReader xmlReader = new DocxXmlReader(stream, new OoxmlComplianceInfo());

                DocxDocumentReaderBase reader = DocxReaderFactory.CreateDocumentReader(GetDocxDocumentReaderStub());

                // Some chart property defaults depend on a document version: need to set the version so that the defaults
                // are generated using the correct document version.
                if (documentVersion >= 0)
                    ((Document)reader.Document).BuiltInDocumentProperties.Version = documentVersion;

                reader.PushPartReader(xmlReader);

                string tagName = GetResourceTagName(chartType);

                while (reader.XmlReader.ReadChild("chartTypes"))
                {
                    if (reader.XmlReader.LocalName == tagName)
                    {
                        while (reader.XmlReader.ReadChild(tagName))
                            DmlChartReader.Read(reader, chartSpace);
                    }
                    else
                    {
                        reader.XmlReader.IgnoreElement();
                    }
                }
            }
        }

        /// <summary>
        /// Reads data of a styleX.xml part of a chart from the ChartStylesXXX.xml resource file as a
        /// <see cref="DmlChartStyle"/> instance.
        /// </summary>
        internal static DmlChartStyle ReadChartStyle(int styleId, Document document)
        {
            string resourceName = GetChartStyleResourceName(styleId);

            // This initializes a theme if it doesn't exist.
            if (document.Theme == null)
                Debug.Fail(null);

            using (Stream stream = ResourceUtil.FetchResourceStream(resourceName))
            {
                stream.Position = 0;

                DocxXmlReader xmlReader = new DocxXmlReader(stream, new OoxmlComplianceInfo());

                DocxDocumentReaderBase reader = DocxReaderFactory.CreateDocumentReader(GetDocxDocumentReaderStub());

                reader.PushPartReader(xmlReader);

                string stringId = styleId.ToString();

                while (reader.XmlReader.ReadChild("chartStyles"))
                {
                    Debug.Assert(reader.XmlReader.LocalName == "chartStyle");
                    if (reader.XmlReader.ReadAttribute("id", "") == stringId)
                        return DmlChartStyleReader.Read(reader);

                    reader.XmlReader.IgnoreElement();
                }
            }

            return null;
        }

        private static string GetChartStyleResourceName(int styleId)
        {
            Debug.Assert(styleId >= 1);
            int i = 1;
            while ((i < gChartStyleResourceFileStartId.Length) && (styleId >= gChartStyleResourceFileStartId[i]))
                i++;

            const string resourceNameTemplate = "Aspose.Words.Resources.Charts.ChartStyles{0}.xml";
            return string.Format(resourceNameTemplate, gChartStyleResourceFileStartId[i - 1]);
        }

        /// <summary>
        /// Gets a tag name that is used to store a chart definition of the specified type in the ChartTypes.xml
        /// resource file.
        /// </summary>
        private static string GetResourceTagName(ChartType chartType)
        {
            switch (chartType)
            {
                case ChartType.Area:
                case ChartType.AreaStacked:
                case ChartType.AreaPercentStacked:
                    return "areaChart";
                case ChartType.Area3D:
                case ChartType.Area3DStacked:
                case ChartType.Area3DPercentStacked:
                    return "area3DChart";
                case ChartType.Bar:
                case ChartType.BarStacked:
                case ChartType.BarPercentStacked:
                    return "barChart";
                case ChartType.Bar3D:
                case ChartType.Bar3DStacked:
                case ChartType.Bar3DPercentStacked:
                    return "bar3DChart";
                case ChartType.Bubble:
                case ChartType.Bubble3D:
                    return "bubbleChart";
                case ChartType.Column:
                case ChartType.ColumnStacked:
                case ChartType.ColumnPercentStacked:
                    return "colChart";
                case ChartType.Column3D:
                    return "col3DChart";
                case ChartType.Column3DClustered:
                case ChartType.Column3DStacked:
                case ChartType.Column3DPercentStacked:
                    return "col3DClusteredChart";
                case ChartType.Doughnut:
                    return "doughnutChart";
                case ChartType.Line:
                case ChartType.LineStacked:
                case ChartType.LinePercentStacked:
                    return "lineChart";
                case ChartType.Line3D:
                    return "line3DChart";
                case ChartType.Pie:
                    return "pieChart";
                case ChartType.PieOfBar:
                case ChartType.PieOfPie:
                    return "ofPieChart";
                case ChartType.Pie3D:
                    return "pie3DChart";
                case ChartType.Radar:
                    return "radarChart";
                case ChartType.Scatter:
                    return "scatterChart";
                case ChartType.Stock:
                    return "stockChart";
                case ChartType.Surface:
                    return "surfaceChart";
                case ChartType.Surface3D:
                    return "surface3DChart";
                case ChartType.Treemap:
                    return "treemap";
                case ChartType.Sunburst:
                    return "sunburst";
                case ChartType.Histogram:
                    return "histogram";
                case ChartType.Pareto:
                    return "pareto";
                case ChartType.BoxAndWhisker:
                    return "boxWhisker";
                case ChartType.Waterfall:
                    return "waterfall";
                case ChartType.Funnel:
                    return "funnel";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets a temporary document.xml part to use in a reader.
        /// </summary>
        private static OpcPackagePart GetDocxDocumentReaderStub()
        {
            OpcPackagePart documentPartStub = new OpcPackagePart("document.xml", "test");
            documentPartStub.Stream = new MemoryStream();
            StreamUtil.WriteAnsiStringToStream("<document />", documentPartStub.Stream);
            documentPartStub.Stream.Position = 0;
            return documentPartStub;
        }

        /// <summary>
        /// The predefined chart styles are stored in resources as several XML files to reduce reading time. Each file
        /// contains a certain range of styles identified by ID. The files are named ChartStyleXXX.xml, where XXX is
        /// the first style ID that is stored in the file. This array contains the IDs used in file names.
        /// </summary>
        private static readonly int[] gChartStyleResourceFileStartId =
        {
            1, 222, 245, 264, 284, 305, 326, 346, 385, 415
        };
    }
}
