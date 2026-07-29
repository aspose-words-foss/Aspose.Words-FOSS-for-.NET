// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/05/2007 by Vladimir Averkin
using System;
using Aspose.Common;
using Aspose.Xml;

namespace Aspose.OpcPackaging
{
    /// <summary>
    /// Provides static method for building "Core File Properties" package part
    /// </summary>
    public static class OpcCorePropertiesWriter
    {
        /// <summary>
        /// Builds "Core File Properties" part for the specified document.
        /// </summary>
        public static void Write(
            AnyXmlBuilder builder,
            string title,
            string subject,
            string creator,
            string keywords,
            string description,
            string lastModifiedBy,
            string revision,
            DateTime lastPrinted,
            DateTime created,
            DateTime modified,
            string category,
            string contentStatus,
            string language,
            string docVersion
            )
        {
            builder.StartDocument("cp:coreProperties");

            builder.WriteAttributeString("xmlns:cp", "http://schemas.openxmlformats.org/package/2006/metadata/core-properties");
            builder.WriteAttributeString("xmlns:dc", "http://purl.org/dc/elements/1.1/");
            builder.WriteAttributeString("xmlns:dcterms", "http://purl.org/dc/terms/");
            builder.WriteAttributeString("xmlns:dcmitype", "http://purl.org/dc/dcmitype/");
            builder.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

            builder.WriteOptionalElement("dc:title", title);
            builder.WriteOptionalElement("dc:subject", subject);
            builder.WriteOptionalElement("dc:creator", creator);
            builder.WriteOptionalElement("cp:keywords", keywords);

            if (StringUtil.HasChars(description))
            {
                builder.StartElement("dc:description");
                builder.WriteStringConvertSpecialCharacters(description);
                builder.EndElement("dc:description");
            }

            builder.WriteOptionalElement("cp:lastModifiedBy", lastModifiedBy);
            builder.WriteElement("cp:revision", revision);
            builder.WriteOptionalElement("cp:lastPrinted", lastPrinted);
            WriteOptionalDate(builder, "created", created);
            WriteOptionalDate(builder, "modified", modified);
            builder.WriteOptionalElement("cp:category", category);
            builder.WriteOptionalElement("cp:contentStatus", contentStatus);
            builder.WriteOptionalElement("dc:language", language);
            builder.WriteOptionalElement("cp:version", docVersion);

            builder.EndDocument();
        }

        public static void WriteOptionalDate(AnyXmlBuilder builder, string name, DateTime value)
        {
            if (value.Year > 1)
            {
                builder.StartElement("dcterms:" + name);
                builder.WriteAttributeString("xsi:type", "dcterms:W3CDTF");
                builder.WriteString(FormatterPal.DateTimeToXmlUtc(value));
                builder.EndElement();
            }
        }
    }
}
