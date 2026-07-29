// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/07/2014 by Andrey Noskov

using System;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlNonVisualPropertiesWriter
    {
        internal static void WriteNvPr(string prefix, DmlNode node, DocxDocumentWriterBase writer)
        {
            if (node.NonVisualPr == null)
                return;

            if (prefix == "wpg" || prefix == "wpc" || prefix == "wps" || prefix == "wp")
            {
                // This element MUST NOT be present when the Wordprocessing Shape, 
                // Canvas or Group is contained directly by a graphicData.
                // Check this by checking parent.
                if (node.Parent != null && (node.Parent.NodeType == NodeType.GroupShape))
                    WriteNonVisualDrawingProperties(prefix, node.NonVisualPr.NvDrawingProperties, writer);

                WriteConnectorProperties(prefix, writer, node);
                return;
            }

            string tagName = string.Format("{0}:{1}", prefix, GetNvPrTagName(node.NonVisualPr.Holder));

            DocxBuilder builder = writer.CurrentBuilder;

            builder.StartElement(tagName);

            // This is strange behavior of MS Word. According to the spec 'cNvPr' is required,
            // but MS Word treat document with 'cNvPr' inside 'nvContentPartPr' as invalid and does not open it.
            // So simply do not write it for now.
            if (node.NonVisualPr.Holder != DmlNvHolder.ContentPart)
                WriteNonVisualDrawingProperties(prefix, node.NonVisualPr.NvDrawingProperties, writer);
            WriteConnectorProperties(prefix, writer, node);
            builder.EndElement(tagName);
        }

        private static void WriteNonVisualDrawingProperties(string prefix, DmlNvDrawingProperties nvPr, DocxDocumentWriterBase writer)
        {
            if (nvPr == null)
                return;
            
            DocxBuilder builder = writer.CurrentBuilder;

            string tagName = string.Format("{0}:cNvPr", prefix);
            builder.StartElement(tagName);

            // id and name are required attributes.
            builder.WriteAttributeString("id", nvPr.Id.ToString());
            builder.WriteAttributeString("name", nvPr.Name);
            builder.WriteAttribute("descr", nvPr.Description);
            builder.WriteAttributeIfTrue("hidden", nvPr.Hidden);
            DmlHlink hlinkClick = nvPr.HlinkClick;

            if (hlinkClick != null)
                DmlHlinkWriter.WriteHlink("a:hlinkClick", hlinkClick.Id, hlinkClick.TargetFrame, hlinkClick.Tooltip, hlinkClick.Extensions, writer);

            DmlExtensionListWriter.Write(nvPr.Extensions, writer);
            builder.EndElement(tagName);
        }

        private static void WriteConnectorProperties(string prefix, DocxDocumentWriterBase writer, DmlNode node)
        {
            DmlCnvPrBase cNvPr = node.NonVisualPr.CNvProperties;

            switch (cNvPr.Holder)
            {
                case DmlNvHolder.ConnectionShape:
                    WriteCnvPrConnectionShape(prefix, (DmlCnvPrConnectorShape)cNvPr, writer);
                    break;
                case DmlNvHolder.GraphicFrame:
                {
                    // Non visual properties "cNvFrPr" and "cNvGraphicFramePr" have the same schema definition
                    // but using different tag names.
                    string tag = "cNvGraphicFramePr";
                    if (node.DmlNodeType == DmlNodeType.GraphicFrame)
                        tag = ((DmlGroupShape)node).IsParentLockedCanvas ? "cNvGraphicFramePr" : "cNvFrPr";
    
                    WriteCnvPrGraphicFrame(prefix, tag, (DmlCnvPrGraphicFrame)cNvPr, writer);
                    break;
                }
                case DmlNvHolder.GroupShape:
                    WriteCnvPrGroupShape(prefix, (DmlCnvPrGroupShape)cNvPr, writer);
                    break;
                case DmlNvHolder.Picture:
                    WriteCnvPrPicture(prefix, (DmlCnvPrPicture)cNvPr, writer);
                    break;
                case DmlNvHolder.Shape:
                    WriteCnvPrShape(prefix, (DmlCnvPrShape)cNvPr, writer);
                    break;
                case DmlNvHolder.ContentPart:
                    WriteCnvPrContentPart(prefix, (DmlCnvPrContentPart)cNvPr, writer);
                    break;
                default:
                    throw new ArgumentException("Unexpected non-visual properties holder.");
            }
        }

        private static void WriteCnvPrContentPart(string prefix, DmlCnvPrContentPart cNvPr, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            string tagName = string.Format("{0}:cNvContentPartPr", prefix);

            builder.StartElement(tagName);
            WriteLocks(cNvPr, writer);
            if (!cNvPr.IsComment)
                builder.WriteAttribute("isComment", cNvPr.IsComment);

            builder.EndElement(tagName);
        }

        private static void WriteCnvPrConnectionShape(string prefix, DmlCnvPrConnectorShape cNvPr, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            // WORDSNET-26689 When writing the 21.3.2.15 'nvCxnSpPr' (Connector Non Visual Properties) element of the
            // http://purl.oclc.org/ooxml/drawingml/chartDrawing namespace (prefix 'cdr'), the element name is 'cNvCxnSpPr'.
            string connectorTagName = (isIsoStrict && (prefix != "cdr")) ? "cNvCnPr" : "cNvCxnSpPr";

            string tagName = (prefix == "wps") ? "wps:cNvCnPr" : string.Format("{0}:{1}", prefix, connectorTagName);

            builder.StartElement(tagName);
            WriteLocks(cNvPr, writer);

            DmlExtensionListWriter.Write(cNvPr.Extensions, writer);

            // WORDSNET-17029 Write namespaces to preserve links between shapes.
            if (cNvPr.ConnectionStart != null)
                WriteShapeConnectionsAttributes(builder, "a:stCxn", cNvPr.ConnectionStart);
            if (cNvPr.ConnectionEnd != null)
                WriteShapeConnectionsAttributes(builder, "a:endCxn", cNvPr.ConnectionEnd);
           
            builder.EndElement(tagName);
        }

        private static void WriteShapeConnectionsAttributes(DocxBuilder builder, string elementName, DmlConnection connection)
        {
            // WriteElementWithAttributes can not be used due to limitations related with "uint" while writing the attribute values.
            builder.StartElement(elementName);

            builder.WriteAttributeUInt("id", connection.Id);
            builder.WriteAttributeUInt("idx", connection.Index);

            builder.EndElement(elementName);
        }

        private static void WriteCnvPrGraphicFrame(string prefix, string tag, DmlCnvPrGraphicFrame cNvPr, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;
            
            string tagName = string.Format("{0}:{1}", prefix, tag);

            builder.StartElement(tagName);
            WriteLocks(cNvPr, writer);
            DmlExtensionListWriter.Write(cNvPr.Extensions, writer);
            builder.EndElement(tagName);
        }

        private static void WriteCnvPrGroupShape(string prefix, DmlCnvPrGroupShape cNvPr, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            string tagName = string.Format("{0}:cNvGrpSpPr", prefix);

            builder.StartElement(tagName);
            WriteLocks(cNvPr, writer);
            DmlExtensionListWriter.Write(cNvPr.Extensions, writer);
            builder.EndElement(tagName);
        }

        private static void WriteCnvPrPicture(string prefix, DmlCnvPrPicture cNvPr, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            string tagName = string.Format("{0}:cNvPicPr", prefix);

            builder.StartElement(tagName);
            WriteLocks(cNvPr, writer);
            DmlExtensionListWriter.Write(cNvPr.Extensions, writer);
            builder.EndElement(tagName);
        }

        private static void WriteCnvPrShape(string prefix, DmlCnvPrShape cNvPr, DocxDocumentWriterBase writer)
        {
            DocxBuilder builder = writer.CurrentBuilder;

            string tagName = string.Format("{0}:cNvSpPr", prefix);

            builder.StartElement(tagName);
            builder.WriteAttributeIfTrue("txBox", cNvPr.TextBox);
            WriteLocks(cNvPr, writer);
            DmlExtensionListWriter.Write(cNvPr.Extensions, writer);
            builder.EndElement(tagName);
        }

        private static void WriteLocks(DmlCnvPrBase cNvPr, DocxDocumentWriterBase writer)
        {
            if (cNvPr.Locks.Count == 0)
                return;

            string prefix = "a";

            DocxBuilder builder = writer.CurrentBuilder;

            string tagName = string.Format("{0}:{1}", prefix, GetLocksTagName(cNvPr.Holder));

            builder.StartElement(tagName);

            WriteLock(DmlLock.NoGroup, cNvPr.Locks, builder);
            WriteLock(DmlLock.NoSelect, cNvPr.Locks, builder);
            WriteLock(DmlLock.NoRotation, cNvPr.Locks, builder);
            WriteLock(DmlLock.NoChangeAspect, cNvPr.Locks, builder);
            WriteLock(DmlLock.NoMove, cNvPr.Locks, builder);
            WriteLock(DmlLock.NoResize, cNvPr.Locks, builder);
            WriteLock(DmlLock.NoEditPoints, cNvPr.Locks, builder);
            WriteLock(DmlLock.NoAdjustHandles, cNvPr.Locks, builder);
            WriteLock(DmlLock.NoChangeArrowheads, cNvPr.Locks, builder);
            WriteLock(DmlLock.NoChangeShapeType, cNvPr.Locks, builder);
            WriteLock(DmlLock.NoDrilldown, cNvPr.Locks, builder);
            WriteLock(DmlLock.NoUngroup, cNvPr.Locks, builder);
            WriteLock(DmlLock.NoCrop, cNvPr.Locks, builder);
            WriteLock(DmlLock.NoTextEdit, cNvPr.Locks, builder);

            DmlExtensionListWriter.Write(prefix, cNvPr.Locks.Extensions, writer);

            builder.EndElement(tagName);
        }

        private static void WriteLock(DmlLock dmlLock, DmlLocks locks, DocxBuilder builder)
        {
            object val = locks.GetLock(dmlLock);
            if(val == null)
                return;

            builder.WriteAttribute(DmlLockToAttrName(dmlLock), val);
        }

        private static string GetNvPrTagName(DmlNvHolder holder)
        {
            switch (holder)
            {
                case DmlNvHolder.ConnectionShape:
                    return "nvCxnSpPr";
                case DmlNvHolder.GraphicFrame:
                    return "nvGraphicFramePr";
                case DmlNvHolder.GroupShape:
                    return "nvGrpSpPr";
                case DmlNvHolder.Picture:
                    return "nvPicPr";
                case DmlNvHolder.Shape:
                    return "nvSpPr";
                case DmlNvHolder.ContentPart:
                    return "nvContentPartPr";
                default:
                    throw new ArgumentException("Unexpected non-visual properties holder.");
            }
        }

        private static string GetLocksTagName(DmlNvHolder holder)
        {
            switch (holder)
            {
                case DmlNvHolder.ConnectionShape:
                    return "cxnSpLocks";
                case DmlNvHolder.GraphicFrame:
                    return "graphicFrameLocks";
                case DmlNvHolder.GroupShape:
                    return "grpSpLocks";
                case DmlNvHolder.Picture:
                    return "picLocks";
                case DmlNvHolder.Shape:
                    return "spLocks";
                case DmlNvHolder.ContentPart:
                    return "cpLocks";
                default:
                    throw new ArgumentException("Unexpected non-visual properties holder.");
            }
        }

        private static string DmlLockToAttrName(DmlLock dmlLock)
        {
            switch (dmlLock)
            {
                case DmlLock.NoGroup:
                    return "noGrp";
                case DmlLock.NoSelect:
                    return "noSelect";
                case DmlLock.NoRotation:
                    return "noRot";
                case DmlLock.NoChangeAspect:
                    return "noChangeAspect";
                case DmlLock.NoMove:
                    return "noMove";
                case DmlLock.NoResize:
                    return "noResize";
                case DmlLock.NoEditPoints:
                    return "noEditPoints";
                case DmlLock.NoAdjustHandles:
                    return "noAdjustHandles";
                case DmlLock.NoChangeArrowheads:
                    return "noChangeArrowheads";
                case DmlLock.NoChangeShapeType:
                    return "noChangeShapeType";
                case DmlLock.NoDrilldown:
                    return "noDrilldown";
                case DmlLock.NoUngroup:
                    return "noUngrp";
                case DmlLock.NoCrop:
                    return "noCrop";
                case DmlLock.NoTextEdit:
                    return "noTextEdit";
                default:
                    throw new ArgumentException("Unexpected lock type.");
            }
        }
    }
}
