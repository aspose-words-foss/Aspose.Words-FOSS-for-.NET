// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/15/2014 by Alexey Noskov

using System;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Dml.Writer
{
    /// <summary>
    /// Instance class for writing DrawingML picture.
    /// ISO_IEC_29500-1 20.2 (DrawingML - Picture)
    /// </summary>
    internal static class DmlPictureWriter
    {
        internal static void Write(ShapeBase dml, DocxDocumentWriterBase writer)
        {
            DmlNode dmlNode = dml.DmlNode;

            if (dmlNode.DmlNodeType != DmlNodeType.Picture)
                throw new ArgumentException("Unexpected Dml node type.");

            string prefix = "pic";

            ShapeBase topLevelParentDml = dml.GetTopLevelParentShape();
            if ((topLevelParentDml != null) && (topLevelParentDml.DmlNode.DmlNodeType == DmlNodeType.LockedCanvas))
                prefix = "a";

            string tagName = string.Format("{0}:pic", prefix);
            DmlPicture dmlPicture = (DmlPicture)dmlNode;
            NrxXmlBuilder builder = writer.CurrentBuilder;
            bool isIsoStrict = writer.Compliance == OoxmlComplianceCore.IsoStrict;

            builder.StartElement(tagName);
            builder.WriteAttribute("xmlns:pic", DmlNamespaceUtil.GetNamespace(dmlPicture.DmlNodeType, isIsoStrict));

            DmlNonVisualPropertiesWriter.WriteNvPr(prefix, dmlPicture, writer);
            DmlFillWriter.WriteDmlBlipFill(prefix, dmlPicture.BlipFill, writer);
            DmlShapePropertiesWriter.Write(prefix, dmlPicture, writer);

            // Write extLst (Extension List with Modification Flag) §4.2.4.
            DmlExtensionListWriter.Write(dmlPicture.Extensions, writer);

            DmlShapeStyleWriter.Write(dmlPicture, writer);

            builder.EndElement(tagName);
        }
    }
}
