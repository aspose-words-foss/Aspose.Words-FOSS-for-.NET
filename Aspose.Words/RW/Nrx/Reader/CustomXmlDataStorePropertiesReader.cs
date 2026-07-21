// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2011 by Roman Korchagin

using System.IO;
using Aspose.Words.Markup;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Nrx.Reader
{
    internal static class CustomXmlDataStorePropertiesReader
    {
        /// <summary>
        /// Reads 22.5.2.1 datastoreItem (Custom XML Data Properties).
        /// </summary>
        internal static void Read(Stream stream, CustomXmlPart customXmlPart, IWarningCallback warningCallback)
        {
            // andrnosk: WORDSNET-6977 Check if stream is not empty.
            if (stream.Length == 0)
                return;

            try
            {
                NrxXmlReader xmlReader = new NrxXmlReader(stream);

                // Read attrs.
                while (xmlReader.MoveToNextAttribute())
                {
                    switch (xmlReader.LocalName)
                    {
                        case "itemID":
                            // The SPEC specifies this to be a GUID, but MS Word allows any string so we do the same.
                            customXmlPart.Id = xmlReader.Value;
                            break;
                        default:
                            // Do nothing.
                            break;
                    }
                }

                // Read elements.
                xmlReader.MoveToElement();
                while (xmlReader.ReadChild("datastoreItem"))
                {
                    switch (xmlReader.LocalName)
                    {
                        case "schemaRefs":
                            ReadSchemaRefs(xmlReader, customXmlPart);
                            break;
                        default:
                            xmlReader.IgnoreElement();
                            break;
                    }
                }
            }
            catch
            {
                // WORDSNET-10259 Resiliently catch any XML errors.
                WarningUtil.WarnUnexpected(warningCallback, WarningSource.Unknown, WarningStrings.CustomXmlError);
            }
        }

        /// <summary>
        /// Reads 22.5.2.3 schemaRefs (Set of Associated XML Schemas)
        /// </summary>
        private static void ReadSchemaRefs(NrxXmlReader xmlReader, CustomXmlPart customXmlPart)
        {
            while (xmlReader.ReadChild("schemaRefs"))
            {
                switch (xmlReader.LocalName)
                {
                    case "schemaRef":
                        ReadSchemaRef(xmlReader, customXmlPart);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads 22.5.2.2 schemaRef (Associated XML Schema)
        /// </summary>
        private static void ReadSchemaRef(NrxXmlReader xmlReader, CustomXmlPart customXmlPart)
        {
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "uri":
                        customXmlPart.Schemas.Add(xmlReader.Value);
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }
        }
    }
}
