// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/08/2007 by Vladimir Averkin

using Aspose.Common;
using Aspose.Words.Nrx;
using Aspose.Words.Properties;
using Aspose.Words.Saving;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for reading "Custom File Properties" package part
    /// </summary>
    internal static class DocxCustomPropertiesReader
    {
        /// <summary>
        /// Reads "Custom File Properties" part for the specified document.
        /// </summary>
        internal static void Read(DocxReader reader)
        {
            DocxXmlReader xmlReader = reader.CreatePackageChildPartReader(reader.RelTypes.CustomProperties);
            // WORDSNET-27423 MS Word allows opening and reading custom properties if the document has a strict compliance,
            // but the custom.xml file has a schema url from a transitional compliance.
            // Lets try to create a reader with url from transitional compliance, if the reader is null.
            if ((xmlReader == null) && (reader.Document.ComplianceInfo.Compliance == OoxmlComplianceCore.IsoStrict))
                xmlReader = reader.CreatePackageChildPartReader(DocxRelationshipTypes.GetType(DocxRelationshipType.CustomProperties, false));
            // If the reader still null, ignore the properties.
            if (xmlReader == null)
                return;

            CustomDocumentProperties props = reader.Document.CustomDocumentProperties;

            while (xmlReader.ReadChild("Properties"))
            {
                switch (xmlReader.LocalName)
                {
                    case "property":
                        ReadProperty(props, xmlReader);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        private static void ReadProperty(CustomDocumentProperties props, DocxXmlReader xmlReader)
        {
            // Example:
            //   <property fmtid="{D5CDD505-2E9C-101B-9397-08002B2CF9AE}" pid="4" name="_AuthorEmail">
            //       <vt:lpwstr>kpaul@boomerang.com</vt:lpwstr>
            //   </property>

            string name = null;
            string linkTarget = null;

            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "fmtid":
                        // Can be ignored.
                        break;
                    case "pid":
                        // RK It actually looks it is better to ignore property id and generate new one.
                        // This allows for "more" resiliency in case pid is not specified in DOCX.
                        break;
                    case "name":
                        name = xmlReader.Value;
                        break;
                    case "linkTarget":
                        linkTarget = xmlReader.Value;
                        break;
                    default:
                        // Do nothing.
                        break;
                }
            }

            // Property should have name.
            if (!StringUtil.HasChars(name))
                return;

            while (xmlReader.ReadChild("property"))
            {
                DocumentProperty prop = null;

                switch (xmlReader.LocalName)
                {
                    case "bool":        // vt:bool
                        props.AddSafe(name, xmlReader.ReadBoolString(false));
                        break;
                    case "i4":            // vt:i4
                        prop = props.AddSafe(name, xmlReader.ReadStringAsInt());
                        break;
                    case "r8":            // vt:r8
                        prop = props.AddSafe(name, FormatterPal.ParseDouble(xmlReader.ReadString()));
                        break;
                    case "lpwstr":        // vt:lpwstr
                    {
                        // andrnosk: WORDSNET-9275/9830/11735 x000D and x000B is the hexadecimal equivalent of 13, which is the ascii value of 
                        // a carriage return, MS Word does not show this.
                        prop = props.AddSafe(name, xmlReader.ReadStringConvertSpecialCharacters(true));
                        break;
                    }
                    case "filetime":    // vt:filetime
                        props.AddSafe(name, FormatterPal.XmlToDateTime(xmlReader.ReadString()));
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }

                if (StringUtil.HasChars(linkTarget) && (prop != null))
                    prop.LinkTarget = linkTarget;
            }
        }
    }
}
