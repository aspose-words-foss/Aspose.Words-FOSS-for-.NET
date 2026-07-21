// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/08/2007 by Vladimir Averkin

using System.Text;
using Aspose.Common;
using Aspose.Words.Properties;
using Aspose.Xml;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for reading the "Core File Properties" package part.
    /// </summary>
    internal static class DocxCorePropertiesReader
    {
        /// <summary>
        /// Reads "Core File Properties" part for the specified document.
        /// </summary>
        internal static void Read(DocxReader reader)
        {
            DocxXmlReader xmlReader = reader.CreatePackageChildPartReader(reader.RelTypes.CoreProperties);
            if (xmlReader == null)
                return;

            BuiltInDocumentProperties props = reader.Document.BuiltInDocumentProperties;

            while (xmlReader.ReadChild("coreProperties"))    //cp:coreProperties
            {
                switch (xmlReader.LocalName)
                {
                    case "title": // dc:title
                        props.Title = xmlReader.ReadStringConvertSpecialCharacters(false);
                        break;
                    case "subject": // dc:subject
                    {
                        // FOSS
                        props.Subject = xmlReader.ReadString();
                        break;
                    }
                    case "creator": // dc:creator
                        props.Author = xmlReader.ReadString();
                        break;
                    case "keywords": // cp:keywords
                        ParseKeywords(props, xmlReader);
                        break;
                    case "description": // dc:description
                        props.Comments = xmlReader.ReadStringConvertSpecialCharacters(false);
                        break;
                    case "lastModifiedBy":    // cp:lastModifiedBy
                        props.LastSavedBy = xmlReader.ReadString();
                        break;
                    case "revision":        // cp:revision
                        props.RevisionNumber = xmlReader.ReadStringAsInt();
                        break;
                    case "lastPrinted":        // cp:lastPrinted
                        props.LastPrinted = FormatterPal.XmlToDateTime(xmlReader.ReadString());
                        break;
                    case "created":            // dcterms:created
                        props.CreatedTime = FormatterPal.XmlToDateTime(xmlReader.ReadString());
                        break;
                    case "modified":        // dcterms:modified
                        props.LastSavedTime = FormatterPal.XmlToDateTime(xmlReader.ReadString());
                        break;
                    case "category":        // cp:category
                        props.Category = xmlReader.ReadString();
                        break;
                    case "contentStatus":    // cp:contentStatus
                        props.ContentStatus = xmlReader.ReadString();
                        break;
                    case "contentType":        // cp:contentType
                        props.ContentType = xmlReader.ReadString();
                        break;
                    case "language":        // dc:language
                        props[PropertyName.Language].Value = xmlReader.ReadString();
                        break;
                    case "version":         // cp:version
                        props[PropertyName.DocVersion].Value = xmlReader.ReadString();
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ParseKeywords(BuiltInDocumentProperties props, DocxXmlReader xmlReader)
        {
            StringBuilder keywords = new StringBuilder();
            // Read pre-ISO29500 style keywords represented as <keywords>key1, key2, key3</keywords>
            // as well as ISO29500 style keywords:
            // <keywords><value>key2</value><value>key3</value></keywords>
            while (xmlReader.ReadChildWithTextValues("keywords", AnyXmlTextHandlingConsts.TextAndSignificant))
            {
                switch(xmlReader.LocalName)
                {
                    case "value":
                        {
                            // process ISO29500 styled keywords e.g. <value>key1</value>
                            xmlReader.ComplianceInfo.MarkAsIsoTransitional();
                            keywords.Append(xmlReader.ReadString());
                            keywords.Append(", ");
                            break;
                        }
                    default: // assume we deal with pre-ISO29500 keywords which are plain text.
                        props.Keywords = xmlReader.Value;
                        break;
                }
            }

            if (keywords.Length != 0) // although this case seems extra-care, combine new and old style keywords
            {
                props.Keywords = (props.Keywords.Length > 0)
                    ? keywords.ToString() + props.Keywords
                    : keywords.ToString(0, keywords.Length - 2);
            }
        }
    }
}
