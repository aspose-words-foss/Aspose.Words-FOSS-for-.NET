// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/05/2016 by Alexander Zhiltsov

using System.Collections.Generic;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;

namespace Aspose.Words.RW.Dml.Reader
{
    /// <summary>
    /// This class allows reading a color style part of a chart.
    /// </summary>
    internal class DmlChartColorStyleReader : DmlReaderBase
    {
        // Cannot create objects of this class.
        private DmlChartColorStyleReader()
        {
        }

        /// <summary>
        /// Reads a <see cref="DmlChartColorStyle"/> object from the element 2.8.1.2 colorStyle [MS-ODRAWXML] that is
        /// a root element of the word/charts/colorsX.xml part.
        /// </summary>
        internal static DmlChartColorStyle Read(DocxDocumentReaderBase reader)
        {
            if (reader.XmlReader.LocalName != "colorStyle")
                return null;

            DmlChartColorStyle colorStyle = new DmlChartColorStyle();

            ReadAttributes(reader.XmlReader, colorStyle);
            ReadChildren(reader, colorStyle);

            return colorStyle;
        }

        /// <summary>
        /// Reads attributes of the root colorStyle element.
        /// </summary>
        private static void ReadAttributes(NrxXmlReader xmlReader, DmlChartColorStyle colorStyle)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "meth":
                        colorStyle.Method = xmlReader.Value;
                        break;
                    case "id":
                        colorStyle.Id = xmlReader.Value;
                        break;
                    default:
                        WarnUnexpected(xmlReader);
                        break;
                }
            }
            xmlReader.MoveToElement();
        }

        /// <summary>
        /// Reads child elements of the root colorStyle element.
        /// </summary>
        private static void ReadChildren(DocxDocumentReaderBase reader, DmlChartColorStyle colorStyle)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            List<DmlColor> colors = new List<DmlColor>();
            List<DmlChartColorStyleVariation> variations = new List<DmlChartColorStyleVariation>();

            while (xmlReader.ReadChild("colorStyle"))
            {
                switch (xmlReader.LocalName)
                {
                    case "variation":
                        DmlChartColorStyleVariation variation = new DmlChartColorStyleVariation();
                        variation.ColorModifiers = DmlColorReader.ReadModifiers(xmlReader, reader.ComplianceInfo);
                        variations.Add(variation);
                        break;
                    case "extLst":
                        colorStyle.Extensions = DmlExtensionListReader.Read(reader);
                        break;
                    default:
                        DmlColor color = DmlColorReader.Read(xmlReader, reader.ComplianceInfo);
                        if (color != null)
                            colors.Add(color);
                        break;
                }
            }

            if (colors.Count > 0)
            {
                colorStyle.Colors = new DmlColor[colors.Count];
                for (int i = 0; i < colors.Count; i++)
                    colorStyle.Colors[i] = colors[i];
            }

            if (variations.Count > 0)
            {
                colorStyle.Variations = new DmlChartColorStyleVariation[variations.Count];
                for (int i = 0; i < variations.Count; i++)
                    colorStyle.Variations[i] = variations[i];
            }
        }
    }
}
