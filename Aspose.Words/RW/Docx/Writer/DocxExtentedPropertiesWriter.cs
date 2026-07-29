// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/05/2007 by Vladimir Averkin
using System;
using Aspose.Words.Nrx;
using Aspose.Words.Properties;
using Aspose.Words.RW.Nrx.Writer;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Provides static method for writing "Extended File Properties" package part
    /// </summary>
    internal static class DocxExtentedPropertiesWriter
    {
        /// <summary>
        /// Writes "Extended File Properties" part for the specified document.
        /// </summary>
        internal static void Write(DocxWriter writer)
        {
            DocxBuilder builder = writer.CreatePartAndBuilder(@"/docProps/app.xml", DocxContentType.ExtendedProperties, writer.RelTypes.ExtendedProperties);
            builder.StartExtendedPropertiesDocumentPart();

            Document doc = writer.Document;
            BuiltInDocumentProperties props = doc.BuiltInDocumentProperties;

            builder.WriteOptionalElement("Template", props.Template);

            builder.WriteElement("TotalTime", props.TotalEditingTime);
            builder.WriteElement("Pages", props.Pages);
            builder.WriteElement("Words", props.Words);
            builder.WriteElement("Characters", props.Characters);
            builder.WriteElement("Application", "Microsoft Office Word");
            builder.WriteElement("DocSecurity", (int)props.Security);
            builder.WriteElement("Lines", props.Lines);
            builder.WriteElement("Paragraphs", props.Paragraphs);
            builder.WriteElement("ScaleCrop", "false");
            WriteHeadingPairs(builder, props.HeadingPairs);
            WriteTitlesOfParts(builder, props.TitlesOfParts);
            builder.WriteOptionalElement("Manager", props.Manager);
            builder.WriteElement("Company", props.Company);
            builder.WriteElement("LinksUpToDate", "false");
            builder.WriteElement("CharactersWithSpaces", props.CharactersWithSpaces);
            builder.WriteElement("SharedDoc", "false");
            //"HLinks" we do not support.
            builder.WriteOptionalElement("HyperlinkBase", props.HyperlinkBase);
            builder.WriteElement("HyperlinksChanged", "false");

            // High 16 bits contains version number.
            MsWordVersionCore version = (MsWordVersionCore)(props.Version >> 16);

            // WORDSNET-8723 MS Word 2007 SP2 does not like earlier versions written. If they fix it,
            // it could be a good idea to change this code back to writing props.Version.
            // WORDSNET-15407 Do not change AppVersion for MSW version higher then Word2007.
            builder.WriteElement("AppVersion", (version <= MsWordVersionCore.Word2007)
                ? "12.0000"
                : NrxXmlUtil.VersionNumberToXml(props.Version));

            builder.EndDocument();
        }

        private static void WriteHeadingPairs(DocxBuilder builder, object[] headingPairs)
        {
            if (headingPairs.Length == 0)
                return;

            builder.StartElement("HeadingPairs");
            builder.StartElement("vt:vector");
            builder.WriteAttribute("size", headingPairs.Length);
            builder.WriteAttribute("baseType", "variant");

            foreach (object o in headingPairs)
            {
                builder.StartElement("vt:variant");

                if (o is string)
                    builder.WriteElement("vt:lpstr", (string)o);
                else if (o is int)
                    builder.WriteElement("vt:i4", (int)o);
                else
                    throw new InvalidOperationException("Unexpected value type in heading pairs.");

                builder.EndElement();   // vt:variant
            }

            builder.EndElement();   // vt:vector
            builder.EndElement();   // HeadingPairs
        }

        private static void WriteTitlesOfParts(DocxBuilder builder, string[] titlesOfParts)
        {
            if (titlesOfParts.Length == 0)
                return;

            builder.StartElement("TitlesOfParts");
            builder.StartElement("vt:vector");
            builder.WriteAttribute("size", titlesOfParts.Length);
            builder.WriteAttribute("baseType", "lpstr");

            foreach (string s in titlesOfParts)
                builder.WriteElement("vt:lpstr", s);

            builder.EndElement();   // vt:vector
            builder.EndElement();   // TitlesOfParts
        }
    }
}
