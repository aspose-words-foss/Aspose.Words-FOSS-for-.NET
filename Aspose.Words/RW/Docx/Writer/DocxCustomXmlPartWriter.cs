// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/05/2010 by Roman Korchagin
using System.IO;
using Aspose.OpcPackaging;
using Aspose.Words.Markup;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Writes the Custom XML Data Storage and Custom XML Data Storage Properties Parts.
    /// </summary>
    internal static class DocxCustomXmlPartWriter
    {
        internal static void Write(DocxDocumentWriter writer)
        {
            int customXmlPartId = 1;
            int customXmlPropsPartId = 1;
            foreach (CustomXmlPart customXmlPart in writer.Document.CustomXmlParts)
            {
                // Add Custom XML Data Storage Part as a child of the Main Document Part.
                OpcPackagePart opcDataPart = writer.Package.CreateChildPart(
                    writer.DocumentPart,
                    string.Format("/customXml/item{0}.xml", customXmlPartId++),
                    OpcContentType.Xml,
                    writer.RelTypes.CustomXml);

                opcDataPart.Stream = new MemoryStream(customXmlPart.Data);

                // Add Custom XML Data Storage Properties Parts as children of the Custom XML Data Storage Part.
                string dummyRelId;
                DocxBuilder builder = writer.CreateChildPartAndBuilder(
                    opcDataPart, 
                    string.Format("/customXml/itemProps{0}.xml", customXmlPropsPartId++),
                    DocxContentType.CustomXmlProperties,
                    writer.RelTypes.CustomXmlProperties,
                    out dummyRelId);

                CustomXmlDataStorePropertiesWriter.Write(customXmlPart, builder, 
                    builder.OoxmlCompliance == OoxmlComplianceCore.IsoStrict);
            }
        }

    }
}
