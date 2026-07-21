// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/15/2014 by Alexey Noskov

using System;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlGroupShapeWriter
    {
        internal static void WriteStart(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            DmlNode dmlNode = dml.DmlNode;
            DmlNodeType grpSpType = dmlNode.DmlNodeType;

            if (grpSpType != DmlNodeType.GroupShape && 
                grpSpType != DmlNodeType.WordprocessingGroupShape)
                throw new ArgumentException("Unexpected group shape type.");

            string prefix = "a";
            string tag = "grpSp";
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            ShapeBase topLevelParent = GetTopLevelParent(dml);            
            DmlNodeType topLevelParentType = (topLevelParent != null) ? topLevelParent.DmlNode.DmlNodeType : grpSpType;
                            
            if (grpSpType != DmlNodeType.GroupShape)                
            {
                tag = "wgp";
                prefix = isIsoStrict ? "wp" : "wpg";
            }

            if ((topLevelParentType == DmlNodeType.WordprocessingCanvas) ||
                (topLevelParentType == DmlNodeType.WordprocessingGroupShape))         
                prefix = isIsoStrict ? "wp" : "wpg";
     
            // Use the same writer for writing legacy grpSp and wgp.
            // Determine root tag name by group shape type.
            string rootTagName = string.Format("{0}:{1}", prefix, tag);            
            DmlGroupShape groupShape = (DmlGroupShape)dmlNode;
            NrxXmlBuilder builder = writer.CurrentBuilder;          

            builder.StartElement(rootTagName);

            if (grpSpType == DmlNodeType.WordprocessingGroupShape)
                builder.WriteAttribute("xmlns:wpg",
                    DmlNamespaceUtil.GetNamespace(DmlNodeType.WordprocessingGroupShape, isIsoStrict));

            DmlNonVisualPropertiesWriter.WriteNvPr(prefix, groupShape, writer);
            WriteGroupShapeProperties(prefix, groupShape, writer);
        }

        internal static void WriteEnd(DocxDocumentWriterBase writer)
        {
            writer.CurrentBuilder.EndElement(); // Closes either "a:grpSp" or "wpg:wgp".
        }

        /// <summary>
        /// Returns top-level parent drawingML.
        /// </summary>
        /// <param name="dml"></param>
        /// <returns></returns>
        private static ShapeBase GetTopLevelParent(ShapeBase dml)
        {
            if (dml.IsTopLevel)
                return dml;

            if ((dml.ParentNode.NodeType != NodeType.Shape) && (dml.ParentNode.NodeType != NodeType.GroupShape))
                return null;

            return GetTopLevelParent((ShapeBase)dml.ParentNode);
        }

        internal static void WriteGroupShapeProperties(string prefix, DmlCompositeNode dml, DocxDocumentWriterBase writer)
        {
            NrxXmlBuilder builder = writer.CurrentBuilder;

            string rootTagName = string.Format("{0}:grpSpPr", prefix);

            builder.StartElement(rootTagName);

            if ((dml.DmlNodeType == DmlNodeType.WordprocessingGroupShape) || 
                (dml.DmlNodeType == DmlNodeType.GroupShape))
            {
                builder.WriteAttributeIfNotDefault("bwMode", DmlEnum.BWModeToDml(((DmlGroupShape)dml).BWMode),
                    DmlEnum.BWModeToDml(BWMode.Default));
            }
                             
            DmlXfrmWriter.Write(dml.TransformInternal, writer);
            DmlFillWriter.Write(dml.Fill, writer, false);
            DmlShapeEffectsWriter.Write(dml.Effects, writer, false);
            Dml3DPropertiesWriter.WriteScene3D(dml.Scene3DProperties, writer, false);

            DmlExtensionListWriter.Write(dml.Extensions, writer);

            builder.EndElement(rootTagName);
        }
    }
}
