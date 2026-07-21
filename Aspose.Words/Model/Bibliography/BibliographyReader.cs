// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2024 by Edward Voronov

using System.IO;
using Aspose.Words.Markup;
using Aspose.Xml;

namespace Aspose.Words.Bibliography
{
    /// <summary>
    /// <see cref="Source"/> xml deserializer.
    /// </summary>
    internal static class BibliographyReader
    {
        internal static CustomXmlPart Read(Bibliography bibliography, Document document)
        {
            CustomXmlPart result = null;

            foreach (CustomXmlPart customXmlPart in document.CustomXmlParts)
            {
                if (customXmlPart.Data.Length == 0)
                    continue;

                if (ReadXmlPartSafe(bibliography, customXmlPart))
                    result = customXmlPart;
            }

            return result;
        }

        private static bool ReadXmlPartSafe(Bibliography bibliography, CustomXmlPart customXmlPart)
        {
            try
            {
                return ReadXmlPartData(bibliography, customXmlPart.Data);
            }
            catch
            {
                return false;
            }
        }

        private static bool ReadXmlPartData(Bibliography bibliography, byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                AnyXmlReader reader = new AnyXmlReader(stream, true);
                return ReadXml(bibliography, reader);
            }
        }

        private static bool ReadXml(Bibliography bibliography, AnyXmlReader reader)
        {
            if (reader.LocalName != "Sources")
                return false;

            ReadBibliography(bibliography, reader);
            ReadSources(bibliography, reader);

            return true;
        }

        private static void ReadBibliography(Bibliography bibliography, AnyXmlReader reader)
        {
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "SelectedStyle":
                        bibliography.BibliographyStyle = reader.Value;
                        break;
                    case "StyleName":
                        bibliography.StyleName = reader.Value;
                        break;
                    case "Version":
                        bibliography.Version = reader.Value;
                        break;
                    default:
                        break;
                }
            }
        }

        private static void ReadSources(Bibliography bibliography, AnyXmlReader reader)
        {
            // Subsequent sources overrides any previous one.
            bibliography.Sources.Clear();

            if (!reader.ReadChild("Sources"))
                return;

            while (!reader.IsEndElement("Sources"))
            {
                switch (reader.LocalName)
                {
                    case "Source":
                        string xml = reader.ReadOuterXml();
                        Source source = new Source(xml);
                        bibliography.Sources.Add(source);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
