// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/05/2007 by Vladimir Averkin

using System.Collections.Generic;
using Aspose.Collections.Generic;
using Aspose.Xml;

namespace Aspose.OpcPackaging
{
    /// <summary>
    /// Provides static method for writing "Content Types" package part.
    /// </summary>
    public static class OpcContentTypesWriter
    {
        /// <summary>
        /// Writes content types for the specified document.
        /// </summary>
        public static void Write(OpcPackage opcPackage, bool isPrettyFormat)
        {
            // This iterates over all parts and collects two lists to write: default and override content types.
            SortedStringListGeneric<string> defaultTypes = new SortedStringListGeneric<string>();
            SortedStringListGeneric<string> overrideTypes = new SortedStringListGeneric<string>();

            foreach (OpcPackagePart part in opcPackage.Parts)
            {
                switch (part.ContentType)
                {
                    case OpcContentType.Relationships:
                        // VA Content type for rels is added explicitly in the code below.
                        // RK I'm not exactly sure what is the reason, but lets leave it.
                        break;
                    case OpcContentType.ImageBmp:
                    case OpcContentType.ImageEmf:
                    case OpcContentType.ImageGif:
                    case OpcContentType.ImageJpeg:
                    case OpcContentType.ImagePictCompressed:
                    case OpcContentType.ImagePng:
                    case OpcContentType.ImageWmf:
                    case OpcContentType.ImageWebP:
                    case OpcContentType.Odttf:
                    case OpcContentType.DigitalSignatureOrigin:
                    case OpcContentType.DigitalSignatureCertificate:
                        defaultTypes[part.Extension] = part.ContentType;
                        break;
                    default:
                        // For all other parts, add them as overrides.
                        overrideTypes.Add(part.Name, part.ContentType);
                        break;
                }
            }
            
            // Create the content types part.
            OpcPackagePart contentTypesPart = new OpcPackagePart("/[Content_Types].xml", "");
            opcPackage.Parts.Add(contentTypesPart);

            AnyXmlBuilder builder = new AnyXmlBuilder(contentTypesPart.Stream, isPrettyFormat);
            builder.StartDocument("Types");
            builder.WriteAttributeString("xmlns", "http://schemas.openxmlformats.org/package/2006/content-types");
            
            // Write default content types.
            foreach (KeyValuePair<string, string> entry in defaultTypes)
                WriteDefaultElement(entry.Key, entry.Value, builder);

            WriteDefaultElement("rels", OpcContentType.Relationships, builder);
            WriteDefaultElement("xml", OpcContentType.Xml, builder);

            // Write overrides for content types.
            foreach (KeyValuePair<string, string> entry in overrideTypes)
                WriteOverrideElement(entry.Key, entry.Value, builder);

            builder.EndDocument();
        }

        private static void WriteDefaultElement(string extension, string contentType, AnyXmlBuilder builder)
        {
            // Example:
            //   <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml" />
            builder.StartElement("Default");
            builder.WriteAttributeString("Extension", extension);
            builder.WriteAttributeString("ContentType", contentType);
            builder.EndElement();
        }

        private static void WriteOverrideElement(string partName, string contentType, AnyXmlBuilder builder)
        {
            // Example:
            //   <Override PartName="/word/document.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml" />
            builder.StartElement("Override");
            builder.WriteAttributeString("PartName", partName);
            builder.WriteAttributeString("ContentType", contentType);
            builder.EndElement();
        }
    }
}
