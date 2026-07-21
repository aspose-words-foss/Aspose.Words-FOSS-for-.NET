// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/26/2014 by Alexey Noskov

using Aspose.OpcPackaging;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    internal static class DmlContentPartWriter
    {
        internal static void Write(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            DmlContentPart contentPart = (DmlContentPart)dml.DmlNode;

            DocxBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartElement("w14:contentPart");
            builder.WriteAttribute("bwMode", DmlEnum.BWModeToDml(contentPart.BWMode));
            string relId = WriteContentPart(contentPart, writer);
            builder.WriteAttribute("r:id", relId);
            builder.WriteAttribute("xmlns:r", DocxNamespaces.GetNamespace(DocxNamespace.Relationships, isIsoStrict));
            builder.WriteAttribute("xmlns:w14", DocxNamespaces.GetNamespace(DocxNamespace.W14Markup, isIsoStrict));

            DmlNonVisualPropertiesWriter.WriteNvPr("w14", contentPart, writer);
            DmlXfrmWriter.Write("w14:xfrm", contentPart.Transform, writer);

            DmlExtensionListWriter.Write("w14", contentPart.Extensions, writer);

            builder.EndElement("w14:contentPart");
        }

        private static string WriteContentPart(DmlContentPart contentPart, DocxDocumentWriterBase writer)
        {
            string contentType = contentPart.ContentPart.ContentType;
            string relType = DocxRelationshipTypes.GetType(DocxRelationshipType.CustomXml,
                writer.Compliance == OoxmlComplianceCore.IsoStrict);
            string partName = string.Format("ink/ink{0}.xml", writer.GetNextEmbeddedPartNumber(relType));
            string relId;
            OpcPackagePart newContentPart = writer.Package.CreateChildPart(writer.CurrentBuilder.Part, partName, contentType, relType, out relId);
            newContentPart.Stream = contentPart.ContentPart.Stream;
            DocxDocumentWriterBase.WriteUnparsedRels(contentPart.ContentPart, contentPart.RelatedParts, newContentPart, writer);

            return relId;
        }
    }
}
