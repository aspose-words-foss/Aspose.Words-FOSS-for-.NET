// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/09/2009 by Alexey Noskov

using System;
using System.IO;
using Aspose.Xml;

namespace Aspose.OpcPackaging
{
    /// <summary>
    /// Class reads/writes Flat Opc packages.
    /// Keeps all parts of the package in memory at all times.
    /// </summary>
    public class OpcFlatPackage : OpcPackageBase
    {
        /// <summary>
        /// Creates a new empty package.
        /// </summary>
        public OpcFlatPackage() { }

        /// <summary>
        /// Opens a package from a file and loads all parts into memory, reads content types and relationships.
        /// </summary>
        public OpcFlatPackage(string filename)
        {
            using (Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                LoadCore(stream);
        }

        /// <summary>
        /// Opens a package from a file and loads all parts into memory, reads content types and relationships.
        /// </summary>
        /// <param name="stream">It is the responsibility of the caller to close the stream.</param>
        public OpcFlatPackage(Stream stream)
        {
            LoadCore(stream);
        }

        /// <summary>
        /// Opens a package from a file and loads all parts into memory, reads content types and relationships.
        /// </summary>
        private void LoadCore(Stream stream)
        {
            LoadParts(stream);
            LoadRelationships();
        }

        /// <summary>
        /// Just loads all parts into the parts collection.
        /// When quits, content types are already resolved, but relationships are not read.
        /// </summary>
        private void LoadParts(Stream stream)
        {
            AnyXmlReader flatOpcReader = new AnyXmlReader(stream);
            while (flatOpcReader.ReadChild("package"))
            {
                // Child elements of package should be part elements.
                // If some other element is encountered, just ignore it.
                if (flatOpcReader.LocalName != "part")
                {
                    flatOpcReader.IgnoreElement();
                    continue;
                }

                OpcPackagePart part = ReadFlatOpcPackagePart(flatOpcReader);
                Parts.Add(part);
            }
        }

        /// <summary>
        /// Reads part from the Flat Opc package.
        /// </summary>
        private static OpcPackagePart ReadFlatOpcPackagePart(AnyXmlReader flatOpcReader)
        {
            // Read name and content type attributes.
            string partName = flatOpcReader.ReadAttribute("name", "");
            string contentType = flatOpcReader.ReadAttribute("contentType", "");

            // Read compression attribute, it is present only when part contains binary data.
            // It is not used yet. I am not sure whether it is needed at all.
            flatOpcReader.ReadAttribute("compression", "store");

            OpcPackagePart part = new OpcPackagePart(partName, contentType);

            // Read part data. It can be XML data or binary data.
            flatOpcReader.ReadChild("part");
            switch (flatOpcReader.LocalName)
            {
                case "xmlData":
                {
                    StreamWriter xmlDataWriter = new StreamWriter(part.Stream);
                    flatOpcReader.ReadChild("xmlData");
                    xmlDataWriter.Write(flatOpcReader.ReadOuterXml());
                    xmlDataWriter.Flush();
                    break;
                }
                case "binaryData":
                {
                    string base64 = flatOpcReader.ReadString();
                    byte[] binData;

                    try
                    {
                        binData = Convert.FromBase64String(base64);
                    }
                    catch (FormatException)
                    {
                        // WORDSNET-27250 Invalid Base64 string, invalid spaces and length.
                        base64 = base64.Replace(" ", "");

                        if (base64.Length % 4 == 1)
                            base64 = base64.Substring(0, base64.Length - 3);
                        else if (base64.Length % 4 != 0)
                            base64 = StringUtilPal.PadRight(base64, base64.Length + (4 - base64.Length % 4), '=');

                        binData = Convert.FromBase64String(base64);
                    }

                    part.Stream.Write(binData, 0, binData.Length);
                    break;
                }
                default:
                {
                    // Ignore.
                    break;
                }
            }

            part.Stream.Position = 0;

            return part;
        }

        public override void Save(Stream stream)
        {
            AnyXmlBuilder xmlBuilder = new AnyXmlBuilder(stream, true);

            xmlBuilder.StartDocument(true);
            xmlBuilder.WriteProcessingInstruction("mso-application", "progid=\"Word.Document\"");

            // Start document's root element - package.
            xmlBuilder.StartElement("pkg:package");
            xmlBuilder.WriteAttributeString("xmlns:pkg", "http://schemas.microsoft.com/office/2006/xmlPackage");

            foreach (OpcPackagePart part in Parts)
            {
                part.Stream.Position = 0;

                xmlBuilder.StartElement("pkg:part");
                xmlBuilder.WriteAttributeString("pkg:name", part.Name);
                xmlBuilder.WriteAttributeString("pkg:contentType", part.ContentType);

                // ContentType of package parts, which contain XML data normally ends with "+xml" or equals "application/xml".
                // So if ContentType does not end with "xml", we can assume this is binary data.
                if(!part.ContentType.EndsWith("xml", StringComparison.Ordinal))
                {
                    // If we write binaryData, we should also add compression attribute.
                    xmlBuilder.WriteAttributeString("pkg:compression", "store");

                    xmlBuilder.StartElement("pkg:binaryData");
                    xmlBuilder.WriteBase64(part.Stream);
                    xmlBuilder.EndElement();
                }
                else
                {
                    xmlBuilder.StartElement("pkg:xmlData");
                    xmlBuilder.WriteRaw(GetXmlString(part.Stream));
                    xmlBuilder.EndElement();
                }
                xmlBuilder.EndElement();
            }

            xmlBuilder.EndDocument();
        }

        /// <summary>
        /// Gets an XML string with no initial XML definition from the specified stream.
        /// </summary>
        private static string GetXmlString(Stream xmlStringStream)
        {
            const string opening = "<?xml";
            const string ending = "?>";
            const int notFound = -1;

            string xmlString  = new StreamReader(xmlStringStream).ReadToEnd();

            int openingPos = xmlString.IndexOf(opening, StringComparison.Ordinal);
            if (openingPos == notFound)
                return xmlString;

            int endingPos = xmlString.IndexOf(ending, openingPos + opening.Length, StringComparison.Ordinal);
            if (endingPos == notFound)
                return xmlString;

            // Remove the XML definition from the beginning of the XML string.
            // If the output XML document contains few XML definitions, it will be invalid.
            return xmlString.Substring(endingPos + ending.Length);
        }
    }
}
