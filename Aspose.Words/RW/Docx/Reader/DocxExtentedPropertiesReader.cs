// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/08/2007 by Vladimir Averkin

using Aspose.Common;
using Aspose.Words.Nrx;
using Aspose.Words.Properties;
using Aspose.Xml;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for reading "Extented File Properties" package part
    /// </summary>
    internal static class DocxExtentedPropertiesReader
    {
        /// <summary>
        /// Reads "Extented File Properties" part for the specified document.
        /// </summary>
        internal static void Read(DocxReader reader)
        {
            DocxXmlReader xmlReader = reader.CreatePackageChildPartReader(reader.RelTypes.ExtendedProperties);
            if (xmlReader == null)
                return;

            BuiltInDocumentProperties props = reader.Document.BuiltInDocumentProperties;

            while (xmlReader.ReadChild("Properties"))
            {
                switch (xmlReader.LocalName)
                {
                    case "Template":
                        props.Template = xmlReader.ReadString();
                        break;
                    case "TotalTime":
                        props.SetTotalEditingTimeSafe(xmlReader.ReadStringAsInt());
                        break;
                    case "Pages":
                        props.Pages = xmlReader.ReadStringAsInt();
                        break;
                    case "Words":
                        props.Words = xmlReader.ReadStringAsInt();
                        break;
                    case "Characters":
                        props.Characters = xmlReader.ReadStringAsInt();
                        break;
                    case "Application":
                        props.NameOfApplication = xmlReader.ReadString();
                        break;
                    case "DocSecurity":
                        props.Security = (DocumentSecurity)xmlReader.ReadStringAsInt();
                        break;
                    case "Lines":
                        props.Lines = xmlReader.ReadStringAsInt();
                        break;
                    case "Paragraphs":
                        props.Paragraphs = xmlReader.ReadStringAsInt();
                        break;
                    case "HeadingPairs":
                        props.HeadingPairs = ReadHeadingPairs(xmlReader);
                        break;
                    case "TitlesOfParts":
                        props.TitlesOfParts = ReadTitlesOfParts(xmlReader);
                        break;
                    case "Manager":
                        props.Manager = xmlReader.ReadString();
                        break;
                    case "Company":
                        props.Company = xmlReader.ReadString();
                        break;
                    case "LinksUpToDate":
                        // Not in model. Ignored.
                        break;
                    case "CharactersWithSpaces":
                        props.CharactersWithSpaces = xmlReader.ReadStringAsInt();
                        break;
                    case "HLinks":
                        // Not in model. Ignored.
                        break;
                    case "HyperlinkBase":
                        props.HyperlinkBase = xmlReader.ReadString();
                        break;
                    case "AppVersion":
                        props.Version = NrxXmlUtil.XmlToVersionNumber(xmlReader.ReadString());
                        break;
                    case "DigSig":
                        // Ignore because this is only used in the binary Excel documents.
                        break;
                    case PropertyName.ScaleCrop:
                    case PropertyName.SharedDoc:
                    case PropertyName.HyperlinksChanged:
                        // WORDSNET-27690 Extended document properties are written as int rather than as bool.
                        string val = xmlReader.ReadString();
                        if(FormatterPal.IsBoolTrueFalse(val))
                            props.SetProperty(xmlReader.LocalName, FormatterPal.ParseBoolTrueFalse(val));
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }
        }

        /// <summary>
        /// Reads a heading pairs property from an OOXML document.
        /// If there is any problem, return a zero length array, not null.
        /// </summary>
        private static object[] ReadHeadingPairs(DocxXmlReader xmlReader)
        {
            while (xmlReader.ReadChild("HeadingPairs"))
            {
                switch (xmlReader.LocalName)
                {
                    case "vector":
                        return ReadVector(xmlReader);
                    default:
                        break;
                }
            }
            return new object[0];
        }

        /// <summary>
        /// Reads a titles of parts property from an OOXML document.
        /// If there is any problem, return a zero length array, not null.
        /// </summary>
        private static string[] ReadTitlesOfParts(DocxXmlReader xmlReader)
        {
            while (xmlReader.ReadChild("TitlesOfParts"))
            {
                switch (xmlReader.LocalName)
                {
                    case "vector":
                    {
                        // Convert from an object array to a string array.
                        object[] objects = ReadVector(xmlReader);
                        string[] result = new string[objects.Length];
#if CPLUSPLUS
                        // CPLUSPLUSS can't unbox object[] to string via Array.CopyTo
                        for (int idx = 0; idx < objects.Length; ++idx)
                        {
                            result[idx] = (string)objects[idx];
                        }
#else
                        objects.CopyTo(result, 0);
#endif
                        return result;
                    }
                    default:
                        break;
                }
            }
            return new string[0];
        }

        /// <summary>
        /// Reads a vt:vector from an OOXML document.
        /// Returns an object array of zero length if cannot parse.
        /// </summary>
        private static object[] ReadVector(DocxXmlReader xmlReader)
        {
            int length = 0;
            while (xmlReader.MoveToNextAttribute())
            {
                switch (xmlReader.LocalName)
                {
                    case "size":
                        length = xmlReader.ValueAsInt;
                        break;
                    case "baseType":
                        // Do nothing. We read any as array objects.
                        break;
                    default:
                        break;
                }
            }

            object[] result = new object[length];
            int index = 0;
            while (xmlReader.ReadChildWithTextValues("vector", AnyXmlTextHandlingConsts.TextAndSignificant))
            {
                switch (xmlReader.LocalName)
                {
                    case "variant":
                        result[index] = ReadVariant(xmlReader);
                        index++;
                        break;
                    case "lpstr":
                        result[index] = xmlReader.ReadString();
                        index++;
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        /// <summary>
        /// Reads a vt:variant from an OOXML document. At the moment reads only limited number of types.
        /// Returns null if cannot parse the value.
        /// </summary>
        private static object ReadVariant(DocxXmlReader xmlReader)
        {
            while (xmlReader.ReadChild("variant"))
            {
                switch (xmlReader.LocalName)
                {
                    case "lpstr":
                        return xmlReader.ReadString();
                    case "i4":
                        return xmlReader.ReadStringAsInt();
                    default:
                        break;
                }
            }
            return null;
        }
    }
}
