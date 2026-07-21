// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/05/2010 by Roman Korchagin

using Aspose.Common;
using Aspose.IO;
using Aspose.OpcPackaging;
using Aspose.Words.Markup;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Reads the Custom XML Data Storage and Custom XML Data Storage Properties Parts.
    /// </summary>
    internal static class DocxCustomXmlPartReader
    {
        internal static void Read(DocxDocumentReader reader, IWarningCallback warningCallback)
        {


            // The main document part can have an implicit relationship to zero or more Custom XML Data Storage parts.
            OpcPackagePart documentPart = reader.DocumentPart;

            foreach (OpcRelationship rel in documentPart.Rels)
            {
                if (rel.Type == reader.RelTypes.CustomXml)
                    ReadCustomXmlPart(reader, documentPart.GetRelatedPartName(rel), warningCallback);
            }
        }

        /// <summary>
        /// Reads a Custom XML Data Storage Part and its properties and adds to the document.
        /// </summary>
        private static void ReadCustomXmlPart(DocxDocumentReader reader, string customXmlPartName, IWarningCallback warningCallback)
        {
            OpcPackagePart opcDataPart = reader.FetchPartByName(customXmlPartName);

            // Do not read DrawingML related XmlPart (ink) because it will be read
            // upon DrawingML reading (see DmlDrawingReader.ReadRId method)
            // The relationship type of the explicit relationship specified for this part is CustomXml
            // and that is why we have to avoid reading this as customXmlPart here.
            // Next condition includes this one, but leave it here for better code readability.
            if (opcDataPart.ContentType == DocxContentType.InkML)
                return;

            // WORDSNET-15911 All Custom XML Parts with content type which is not equals “application/xml”
            // have to be ignored according to MS Word behavior.
            if (opcDataPart.ContentType != OpcContentType.Xml)
                return;

            // We don't need to preserve the content type because it is always application/xml.
            // We don't need to preserve the name because we auto generate it on write.
            // But we preserve the data of course.
            CustomXmlPart customXmlPart = new CustomXmlPart();
            customXmlPart.Data = StreamUtil.CopyStreamToByteArray(opcDataPart.Stream);

            // Read the related Custom XML Data Storage Properties Part.
            OpcPackagePart opcPropsPart = reader.GetPartByRelationshipType(opcDataPart, reader.RelTypes.CustomXmlProperties);

            // If Custom XML Data does not have a corresponding Custom XML Data Properties part it is an invalid document I suppose.
            // In this case let's just skip this data part.
            // AM. We should read such parts anyway. See WORDSNET-3913
            if (opcPropsPart == null)
            {
                // andrnosk: WORDSNET-6369 Make sure warningCallback is not null.
                if (warningCallback != null)
                    // Issue warning and read part without properties.
                    warningCallback.Warning(new WarningInfo(WarningType.DataLoss, WarningSource.Docx, string.Format("Custom XML part properties are missed for {0}.", customXmlPartName)));

                // WORDSNET-28700 Assign unique Id for orphan parts.
                customXmlPart.Id = RandomUtil.NewGuid().ToString();
            }
            else
            {
                // Read properties.
                CustomXmlDataStorePropertiesReader.Read(opcPropsPart.Stream, customXmlPart, warningCallback);
            }

            // Finally add the custom xml part to the model.
            reader.Document.CustomXmlParts.Add(customXmlPart);
        }
    }
}
