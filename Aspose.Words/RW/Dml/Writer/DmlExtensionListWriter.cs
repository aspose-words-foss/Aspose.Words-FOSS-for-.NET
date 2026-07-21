// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/07/2014 by Andrey Noskov

using System;
using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Writer for DrawingML extensions.
    /// </summary>
    internal static class DmlExtensionListWriter
    {
        /// <summary>
        /// Writes extensions with prefix 'a'.
        /// </summary>
        internal static void Write(StringToObjDictionary<DmlExtension> extensions, IDmlShapeWriterContext writer)
        {
            Write("a", extensions, writer);
        }

        /// <summary>
        /// Writes extensions with the specified prefix.
        /// </summary>
        internal static void Write(string prefix, StringToObjDictionary<DmlExtension> extensions,
            IDmlShapeWriterContext writer)
        {
            Write(prefix, null, extensions, writer);
        }

        /// <summary>
        /// Writes extensions with the specified prefix and namespace.
        /// </summary>
        internal static void Write(string prefix, string elementNamespace,
            StringToObjDictionary<DmlExtension> extensions, IDmlShapeWriterContext writer)
        {
            if ((extensions == null) || (extensions.Count == 0))
                return;

            string rootTagName = string.Concat(prefix, DmlExtensionName.Separator, DmlExtensionName.ExtLst);

            NrxXmlBuilder builder = writer.Builder;
            builder.StartElement(rootTagName);

            if (StringUtil.HasChars(elementNamespace))
                builder.WriteAttribute(string.Concat(DmlExtensionName.Xmlns, DmlExtensionName.Separator, prefix), elementNamespace);

            foreach (DmlExtension extension in extensions.Values)
            {
                if (extension == null)
                    continue;

                switch (extension.Uri)
                {
                    case DmlExtensionUri.NonVisualPr:
                        WriteNvPrExt(writer, extension);
                        break;
                    case DmlExtensionUri.HiddenFill:
                        WriteFillExt(writer, extension);
                        break;
                    case DmlExtensionUri.UseLocalDpi:
                        WriteUseLocalDpiExt(writer, extension);
                        break;
                    case DmlExtensionUri.HiddenLine:
                        WriteHiddenLineExt(writer, extension);
                        break;
                    case DmlExtensionUri.DataLabels:
                        WriteDatalabelsExtension(extension, writer, prefix);
                        break;
                    case DmlExtensionUri.Filtering:
                        WriteFilteringExtension(extension, writer, prefix);
                        break;
                    case DmlExtensionUri.UniqueId:
                        WriteUniqueIdExtension(extension, writer, prefix);
                        break;
                    case DmlExtensionUri.VideoPr:
                        WriteVideoPrExtension(extension, writer);
                        break;
                    case DmlExtensionUri.DataModelExt:
                        WriteDataModelExtension(extension, writer);
                        break;
                    case DmlExtensionUri.SvgBlip:
                        WriteSvgBlip(extension, writer, prefix);
                        break;
                    case DmlExtensionUri.Decorative:
                        WriteDecorativeExtension(writer, extension);
                        break;
                    case DmlExtensionUri.InvertSolidFillFmt:
                        WriteInvertSolidFillFmtExt(writer, extension, prefix);
                        break;
                    default:
                    {
                        // If this extension is unknown type or not parsed, we should write it as raw data to preserve it.
                        WriteRaw(extension, writer);
                        break;
                    }
                }
            }

            builder.EndElement(rootTagName);
        }

        /// <summary>
        /// Writes data model extension.
        /// 2.10.1.1 dataModelExt [MS-ODRAWXML].
        /// </summary>
        private static void WriteDataModelExtension(DmlExtension ext, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;

            const string prefix = "a";
            const string prefixDsp = "dsp";
            const string extName = DmlExtensionName.DataModelExt;
            string prefixDspWithExtName = string.Concat(prefixDsp, DmlExtensionName.Separator, extName);
            string xmlmsWithPrefix = string.Concat(DmlExtensionName.Xmlns, DmlExtensionName.Separator, prefix);
            string prefixExt = string.Concat(prefix, DmlExtensionName.Separator, DmlExtensionName.Ext);
            string xmlmsWithprefixDsp = string.Concat(DmlExtensionName.Xmlns, DmlExtensionName.Separator, prefixDsp);

            builder.WriteElementWithAttributesStart(prefixExt, DmlExtensionName.Uri, ext.Uri);

            // MSW always writes not strict namespaces for this extension.
            builder.WriteAttribute(xmlmsWithPrefix, DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, false));
            builder.StartElement(prefixDspWithExtName);
            builder.WriteAttributeString("minVer", DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLDiagram, false));
            builder.WriteAttributeString("relId", ext.DrawingRelId);
            builder.WriteAttribute(xmlmsWithprefixDsp, DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLDiagram2008, false));
            builder.EndElement(prefixDspWithExtName);
            builder.EndElement(prefixExt);
        }

        /// <summary>
        /// Writes extension of web video properties.
        /// 2.20.1.1 webVideoPr [MS-ODRAWXML].
        /// </summary>
        private static void WriteVideoPrExtension(DmlExtension ext, IDmlShapeWriterContext writer)
        {
            NrxXmlBuilder builder = writer.Builder;

            if (ext.WebVideoPr != null)
            {
                const string prefix = "a";
                const string prefixWp15 = "wp15";
                string prefixWp15WithExtName = string.Concat(prefixWp15, DmlExtensionName.Separator, DmlExtensionName.WebVideoPr);
                string xmlmsWithPrefixWp15 = string.Concat(DmlExtensionName.Xmlns, DmlExtensionName.Separator, prefixWp15);
                string prefixExt = string.Concat(prefix, DmlExtensionName.Separator, DmlExtensionName.Ext);

                builder.WriteElementWithAttributesStart(prefixExt, DmlExtensionName.Uri, ext.Uri);

                // Write webVideoPr element that specifies the properties for displaying an online video to the user.
                builder.WriteElementWithAttributesStart(prefixWp15WithExtName, xmlmsWithPrefixWp15,
                    DmlExtensionsNamespace.WordprocessingDrawing2012);

                DmlWebVideoProperties webVideoPr = ext.WebVideoPr;

                builder.WriteAttributeString(DmlExtensionName.EmbeddedHtml, webVideoPr.EmbedHtml);
                builder.WriteAttributeString("h", FormatterPal.DoubleToStr(webVideoPr.FrameHeight));
                builder.WriteAttributeString("w", FormatterPal.DoubleToStr(webVideoPr.FrameWidth));

                builder.EndElement(prefixWp15WithExtName);
                builder.EndElement(prefixExt);
            }
            else
            {
                WriteRaw(ext, writer);
            }
        }

        private static void WriteFilteringExtension(DmlExtension ext, IDmlShapeWriterContext writer, string prefix)
        {
            NrxXmlBuilder builder = writer.Builder;

            if (ext.DataLabelsRangeData.ValueRef == null)
            {
                if (!string.IsNullOrEmpty(ext.XmlDoc))
                    WriteRaw(ext, writer);

                return;
            }

            const string prefixC15 = "c15";
            const string extName = "datalabelsRange";
            const string fElement = "f";
            const string dlblRnangeChache = "dlblRangeCache";
            string prefixC15WithExtName = string.Concat(prefixC15, DmlExtensionName.Separator, extName);
            string xmlmsWithPrefixC15 = string.Concat(DmlExtensionName.Xmlns, DmlExtensionName.Separator, prefixC15);
            string prefixExt = string.Concat(prefix, DmlExtensionName.Separator, DmlExtensionName.Ext);
            string prefixC15F = string.Concat(prefixC15, DmlExtensionName.Separator, fElement);
            string prefixDlblRnangeChache = string.Concat(prefixC15, DmlExtensionName.Separator, dlblRnangeChache);


            builder.WriteElementWithAttributesStart(prefixExt, DmlExtensionName.Uri, ext.Uri);
            builder.WriteAttribute(xmlmsWithPrefixC15, DmlExtensionsNamespace.Chart);
            builder.StartElement(prefixC15WithExtName);
            builder.WriteElement(prefixC15F, ext.DataLabelsRangeData.ValueRef.Formula.Value);
            DmlChartCommonWriter.WriteValueCollection(prefixDlblRnangeChache, ext.DataLabelsRangeData.ValueRef.Values,
                writer, false);

            builder.EndElement(prefixC15WithExtName);
            builder.EndElement(prefixExt);
        }

        /// <summary>
        /// Writes uniqueId (<see cref="DmlExtensionUri.UniqueId"/>) extension.
        /// </summary>
        private static void WriteUniqueIdExtension(DmlExtension ext, IDmlShapeWriterContext writer, string prefix)
        {
            NrxXmlBuilder builder = writer.Builder;

            if (ext.DataLabelId == Guid.Empty)
            {
                if (!string.IsNullOrEmpty(ext.XmlDoc))
                    WriteRaw(ext, writer);

                return;
            }

            const string prefixC16 = "c16";
            string prefixС16WithExtName = string.Concat(prefixC16, DmlExtensionName.Separator, DmlExtensionName.UniqueId);
            string xmlmsWithPrefixС16 = string.Concat(DmlExtensionName.Xmlns, DmlExtensionName.Separator, prefixC16);
            string prefixExt = string.Concat(prefix, DmlExtensionName.Separator, DmlExtensionName.Ext);

            builder.WriteElementWithAttributesStart(prefixExt, DmlExtensionName.Uri, ext.Uri);
            builder.WriteAttribute(xmlmsWithPrefixС16, DmlExtensionsNamespace.Chart2014);
            string labelId = ext.DataLabelId.ToString("B").ToUpperInvariant();
            builder.WriteElementWithAttributes(prefixС16WithExtName, DmlExtensionName.Val, labelId);
            builder.EndElement(prefixExt);
        }

        /// <summary>
        /// Writes raw xml, creates correct relation ids for embedded images.
        /// </summary>
        private static void WriteRaw(DmlExtension ext, IDmlShapeWriterContext writer)
        {
            string text = ext.GetXmlToWrite(writer.Compliance);

            foreach (KeyValuePair<string, byte[]> pair in ext.EmbeddedImages)
            {
                string oldRelId = string.Format(" r:embed=\"{0}\"", pair.Key);
                string newRelId;
                if (pair.Value == null)
                {
                    // The case of the non-valid image data. Remove this relation.
                    newRelId = "";
                }
                else
                {
                    // Write the corresponding image, and replace the old Id with the correct one.
                    newRelId = string.Format(" r:embed=\"{0}\"", writer.WriteImageBinData(pair.Value));
                }

                text = text.Replace(oldRelId, newRelId);
            }

            writer.Builder.WriteRaw(text);
        }

        /// <summary>
        /// Writes svgBlip drawing extension.
        /// </summary>
        private static void WriteSvgBlip(DmlExtension ext, IDmlShapeWriterContext writer, string prefix)
        {
            NrxXmlBuilder builder = writer.Builder;
            const string prefixAsvg = "asvg";
            string prefixAsvgWithExtName = string.Concat(prefixAsvg, DmlExtensionName.Separator, DmlExtensionName.SvgBlip);
            string xmlmsWithPrefixAsvg = string.Concat(DmlExtensionName.Xmlns, DmlExtensionName.Separator, prefixAsvg);
            string prefixExt = string.Concat(prefix, DmlExtensionName.Separator, DmlExtensionName.Ext);

            builder.WriteElementWithAttributesStart(prefixExt, DmlExtensionName.Uri, ext.Uri);
            builder.StartElement(prefixAsvgWithExtName);
            builder.WriteAttribute(xmlmsWithPrefixAsvg, DmlExtensionsNamespace.Svg);
            DmlFillWriter.WriteBlipImage(ext.SvgBlip, writer);
            builder.EndElement(prefixAsvgWithExtName);
            builder.EndElement(prefixExt);
        }

        /// <summary>
        /// Writes extension of known type, i.e. that was successfully parsed.
        /// </summary>
        private static void WriteDatalabelsExtension(DmlExtension ext, IDmlShapeWriterContext writer, string prefix)
        {
            NrxXmlBuilder builder = writer.Builder;
            string prefixWithExt = string.Concat(prefix, DmlExtensionName.Separator, DmlExtensionName.Ext);
            const string prefixC15 = "c15";
            string xmlnsWithPrefixC15 = string.Concat(DmlExtensionName.Xmlns, DmlExtensionName.Separator, prefixC15);
            builder.WriteElementWithAttributesStart(prefixWithExt, DmlExtensionName.Uri, ext.Uri);
            builder.WriteAttribute(xmlnsWithPrefixC15, DmlExtensionsNamespace.Chart);
            DmlChartCommonWriter.WriteDataLabelExtensionProperties(prefixC15, ext.DataLabelPr, writer);
            builder.EndElement(prefixWithExt);
        }

        /// <summary>
        /// Writes "HiddenLine" extension content.
        /// </summary>
        /// <param name="writer">Document writer.</param>
        /// <param name="ext">Object which holds properties of the "hidden line" extension.</param>
        private static void WriteHiddenLineExt(IDmlShapeWriterContext writer, DmlExtension ext)
        {
            Debug.Assert((writer != null) && (ext != null));
            const string prefix = "a14";
            string prefixWithExtName = string.Concat(prefix, DmlExtensionName.Separator, DmlExtensionName.HiddenLine);
            StartExtension(writer, ext);
            DmlOutlineWriter.Write(prefixWithExtName, ext.OutlinePr, writer);
            EndExtension(writer);
        }

        /// <summary>
        /// Writes "InvertSolidFillFmt" extension content.
        /// </summary>
        private static void WriteInvertSolidFillFmtExt(IDmlShapeWriterContext writer, DmlExtension ext, string prefix)
        {
            Debug.Assert((writer != null) && (ext != null));

            // The name spaces <a:> and <c:> are declared when the chart space are created.
            const string prefixC14 = "c14";
            string nameSpaceС14 = DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLChart2007, false);
            string xmlnsWithPrefC14 = string.Concat(DmlExtensionName.Xmlns, DmlExtensionName.Separator, prefixC14);
            string prefixC14WithExtName = string.Concat(prefixC14, DmlExtensionName.Separator, DmlExtensionName.InvertSolidFillFmt);
            string prefixExt = string.Concat(prefix, DmlExtensionName.Separator, DmlExtensionName.Ext);

            NrxXmlBuilder builder = writer.Builder;
            builder.WriteElementWithAttributesStart(prefixExt, DmlExtensionName.Uri, ext.Uri);
            builder.WriteAttribute(xmlnsWithPrefC14, nameSpaceС14);
            builder.StartElement(prefixC14WithExtName);
            DmlChartCommonWriter.WriteShapeProperties(ext.DmlChartSpPr, writer, false, prefixC14);
            builder.EndElement(prefixC14WithExtName);
            builder.EndElement(prefixExt);
        }

        private static void WriteUseLocalDpiExt(IDmlShapeWriterContext writer, DmlExtension ext)
        {
            WriteBoolExtension(writer, ext, "a14", DmlExtensionName.UseLocalDpi, ext.UseLocalDpi,
                DmlExtensionsNamespace.DrawingMain);
        }

        /// <summary>
        /// Writes Decorative extension.
        /// </summary>
        private static void WriteDecorativeExtension(IDmlShapeWriterContext writer, DmlExtension ext)
        {
            WriteBoolExtension(writer, ext, "adec", DmlExtensionName.Decorative, ext.Decorative,
                DmlExtensionsNamespace.Decorative);
        }

        /// <summary>
        /// Writes bool type extension.
        /// </summary>
        private static void WriteBoolExtension(IDmlShapeWriterContext writer,
            DmlExtension ext,
            string prefix,
            string extName,
            bool extValue,
            string nameSpace)
        {
            Debug.Assert((writer != null) && (ext != null));

            NrxXmlBuilder builder = writer.Builder;
            StartExtension(writer, ext);
            string prefixWithExtName = string.Concat(prefix, DmlExtensionName.Separator, extName);
            string xmlmsWithPrefix = string.Concat(DmlExtensionName.Xmlns, DmlExtensionName.Separator, prefix);
            builder.WriteElementWithAttributesStart(prefixWithExtName, xmlmsWithPrefix, nameSpace);
            builder.WriteAttribute(DmlExtensionName.Val, extValue);
            builder.EndElement(prefixWithExtName);
            EndExtension(writer);
        }

        /// <summary>
        /// Begins the extension XML element.
        /// </summary>
        /// <param name="writer">Document writer.</param>
        /// <param name="ext">Extension for writing.</param>
        private static void StartExtension(IDmlShapeWriterContext writer, DmlExtension ext)
        {
            Debug.Assert((writer != null) && (ext != null));
            string prefixExt = string.Concat(DmlExtensionName.APrefix, DmlExtensionName.Separator, DmlExtensionName.Ext);
            string xmlmsWithPrefix = string.Concat(DmlExtensionName.Xmlns, DmlExtensionName.Separator, DmlExtensionName.APrefix);
            bool isStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;
            string nameSpace = DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, isStrict);
            writer.Builder.WriteElementWithAttributesStart(prefixExt, DmlExtensionName.Uri, ext.Uri, xmlmsWithPrefix,
                nameSpace);
        }

        /// <summary>
        /// Closes the extension XML element.
        /// </summary>
        /// <param name="writer">Document writer.</param>
        private static void EndExtension(IDmlShapeWriterContext writer)
        {
            Debug.Assert(writer != null);
            string prefixWithExt = string.Concat(DmlExtensionName.APrefix, DmlExtensionName.Separator, DmlExtensionName.Ext);
            writer.Builder.EndElement(prefixWithExt);
        }

        /// <summary>
        /// Writes extension with fill properties to output document.
        /// </summary>
        /// <param name="writer">Document writer.</param>
        /// <param name="ext">Extension for writing.</param>
        private static void WriteFillExt(IDmlShapeWriterContext writer, DmlExtension ext)
        {
            Debug.Assert((writer != null) && (ext != null));

            if (ext.FillPr == null)
                return;

            const string prefix = "a14";
            string prefWithExtName = string.Concat(prefix, DmlExtensionName.Separator, DmlExtensionName.HiddenFill);
            string xmlnsWithPrefix = string.Concat(DmlExtensionName.Xmlns, DmlExtensionName.Separator, prefix);

            DmlFill fill = ext.FillPr;
            NrxXmlBuilder builder = writer.Builder;
            StartExtension(writer, ext);
            builder.WriteElementWithAttributesStart(prefWithExtName, xmlnsWithPrefix, DmlExtensionsNamespace.DrawingMain);
            DmlFillWriter.Write(fill, writer, false);
            builder.EndElement(prefWithExtName);
            EndExtension(writer);
        }

        /// <summary>
        /// Writes extension with non-visual drawing properties to output document.
        /// </summary>
        /// <param name="writer">Document writer.</param>
        /// <param name="ext">Extension for writing.</param>
        private static void WriteNvPrExt(IDmlShapeWriterContext writer, DmlExtension ext)
        {
            Debug.Assert((writer != null) && (ext != null));

            if (ext.NvPr == null)
                return;

            const string prefix = "dgm14";
            string prefWithExtName = string.Concat(prefix, DmlExtensionName.Separator, DmlExtensionName.CNvPr);
            string xmlnsWithPrefix = string.Concat(DmlExtensionName.Xmlns, DmlExtensionName.Separator, prefix);
            DmlNvDrawingProperties nvPr = ext.NvPr;
            StartExtension(writer, ext);

            NrxXmlBuilder builder = writer.Builder;
            builder.WriteElementWithAttributesStart(prefWithExtName, xmlnsWithPrefix, DmlExtensionsNamespace.Diagram2010);
            builder.WriteAttributeString("id", nvPr.Id.ToString());
            builder.WriteAttributeString("name", nvPr.Name);

            DmlHlink hlinkClick = nvPr.HlinkClick;

            if (hlinkClick != null)
                DmlHlinkWriter.WriteHlink("a:hlinkClick", hlinkClick.Id, hlinkClick.TargetFrame, hlinkClick.Tooltip,
                    hlinkClick.Extensions, writer);

            DmlHlink hlinkHover = nvPr.HlinkHover;

            if (hlinkHover != null)
                DmlHlinkWriter.WriteHlink("a:hlinkHover", hlinkHover.Id, hlinkHover.TargetFrame, hlinkHover.Tooltip,
                    hlinkHover.Extensions, writer);

            builder.EndElement(prefWithExtName);
            EndExtension(writer);
        }
    }
}
