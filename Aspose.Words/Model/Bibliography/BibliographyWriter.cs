// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2024 by Edward Voronov

using System.IO;
using Aspose.IO;
using Aspose.Words.Markup;
using Aspose.Xml;

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// <see cref="Source"/> xml serializer.
    /// </summary>
    internal static class BibliographyWriter
    {
        internal static CustomXmlPart Write(Bibliography bibliography, Document document, CustomXmlPart customXmlPart)
        {
            customXmlPart = EnsureCustomXmlPart(customXmlPart, document);

            using (MemoryStream stream = new MemoryStream())
            {
                AnyXmlBuilder builder = new AnyXmlBuilder(stream, true);

                Write(bibliography, builder);

                customXmlPart.Data = StreamUtil.CopyStreamToByteArray(stream);
            }

            return customXmlPart;
        }

        private static CustomXmlPart EnsureCustomXmlPart(CustomXmlPart customXmlPart, Document document)
        {
            if ((customXmlPart != null) && document.CustomXmlParts.Contains(customXmlPart))
                return customXmlPart;

            customXmlPart = new CustomXmlPart();
            document.CustomXmlParts.Add(customXmlPart);

            return customXmlPart;
        }

        private static void Write(Bibliography bibliography, AnyXmlBuilder builder)
        {
            builder.StartDocument("b:Sources", false);

            builder.WriteAttributeString("xmlns:b", Bibliography.XmlNameSpace);
            builder.WriteAttributeString("xmlns", Bibliography.XmlNameSpace);
            builder.WriteAttributeString("SelectedStyle", bibliography.BibliographyStyle);
            builder.WriteAttributeString("StyleName", bibliography.StyleName);
            builder.WriteAttributeString("Version", bibliography.Version);

            foreach (Source source in bibliography.Sources)
                SourceWriter.Write(source, builder);

            builder.EndDocument();
        }
    }
}
