// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/08/2007 by Vladimir Averkin

using Aspose.Words.Fonts;
using Aspose.Words.Nrx;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Docx.Reader
{
    /// <summary>
    /// Provides static method for reading "Font Table" package part.
    /// </summary>
    internal static class DocxFontTableReader
    {
        /// <summary>
        /// Reads "Font Table" package part.
        /// </summary>
        internal static void Read(DocxDocumentReaderBase reader)
        {
            if (reader.LoadOptions.SkipFormatting)
                return;

            NrxXmlReader xmlReader = reader.SwitchToPartReaderByRelType(reader.RelTypes.FontTable);
            if (xmlReader == null)
                return;

            FontInfoCollection fontInfos = reader.Document.FontInfos;

            while (xmlReader.ReadChild("fonts"))    // w:fonts
            {
                switch (xmlReader.LocalName)
                {
                    case "font":    // w:font
                        fontInfos.Merge(ReadFont(reader));
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            reader.RestorePartReader();
        }

        /// <summary>
        /// Reads 'w:font' element.
        /// </summary>
        private static FontInfo ReadFont(DocxDocumentReaderBase reader)
        {
            NrxXmlReader xmlReader = reader.XmlReader;
            FontInfo fontInfo = new FontInfo(xmlReader.ReadAttribute("name", ""));

            while (xmlReader.ReadChild("font"))
            {
                switch (xmlReader.LocalName)
                {
                    case "altName":
                    {
                        // WORDSNET-27952 AltName specified as inner text rather than as val attribute.
                        string altName = xmlReader.ReadVal();
                        if (StringUtil.HasChars(altName))
                            fontInfo.AltName = altName;
                        break;
                    }
                    case "panose1":
                    {
                        // WORDSNET-20762 Panose can be null, need to check it.
                        string panose = xmlReader.ReadVal();
                        if (StringUtil.HasChars(panose))
                            fontInfo.Panose = NrxXmlUtil.HexToBytes(panose, FontInfo.PanoseLength);
                        break;
                    }
                    case "charset":
                    {
                        string charset = xmlReader.ReadVal();
                        if (StringUtil.HasChars(charset))
                        {
                            // WORDSNET-19353 0x prefix is not allowed. Make format parsable.
                            fontInfo.Charset = NrxXmlUtil.HexToInt(charset.Replace("x", ""));
                        }
                        else
                        {
                            // characterSet - IANA Name of Character Set. Name of the character set associated with the font.
                            // The values allowed by this attribute are defined by the names and aliases listed in the IANA registration table.
                            // If this value is specified, the value of the val attribute is ignored.
                            charset = xmlReader.ReadAttribute("characterSet", "");
                            if (StringUtil.HasChars(charset))
                            {
                                fontInfo.IanaCharset = charset;
                                reader.ComplianceInfo.MarkAsIsoTransitional();
                            }
                        }
                        break;
                    }
                    case "family":
                        fontInfo.Family = DocxEnum.DocxToFontFamily(xmlReader.ReadVal());
                        break;
                    case "notTrueType":
                        fontInfo.IsTrueType = !xmlReader.ReadBoolVal();
                        break;
                    case "pitch":
                        fontInfo.Pitch = StyleConvertUtil.XmlToFontPitch(xmlReader.ReadVal());
                        break;
                    case "sig":
                    {
                        fontInfo.Sig = new byte[FontInfo.SigLength];
                        while (xmlReader.MoveToNextAttribute())
                        {
                            switch (xmlReader.LocalName)
                            {
                                case "usb0":
                                    NrxXmlUtil.HexToBytesReversed(xmlReader.Value, fontInfo.Sig, 0);
                                    break;
                                case "usb1":
                                    NrxXmlUtil.HexToBytesReversed(xmlReader.Value, fontInfo.Sig, 4);
                                    break;
                                case "usb2":
                                    NrxXmlUtil.HexToBytesReversed(xmlReader.Value, fontInfo.Sig, 8);
                                    break;
                                case "usb3":
                                    NrxXmlUtil.HexToBytesReversed(xmlReader.Value, fontInfo.Sig, 12);
                                    break;
                                case "csb0":
                                    NrxXmlUtil.HexToBytesReversed(xmlReader.Value, fontInfo.Sig, 16);
                                    break;
                                case "csb1":
                                    NrxXmlUtil.HexToBytesReversed(xmlReader.Value, fontInfo.Sig, 20);
                                    break;
                                default:
                                    Debug.Fail(xmlReader.LocalName);
                                    break;
                            }
                        }
                        break;
                    }
                    case "embedRegular":
                        AddEmbeddedDataToFontInfo(reader, xmlReader, fontInfo, EmbeddedFontStyle.Regular);
                        break;
                    case "embedBold":
                        AddEmbeddedDataToFontInfo(reader, xmlReader, fontInfo, EmbeddedFontStyle.Bold);
                        break;
                    case "embedItalic":
                        AddEmbeddedDataToFontInfo(reader, xmlReader, fontInfo, EmbeddedFontStyle.Italic);
                        break;
                    case "embedBoldItalic":
                        AddEmbeddedDataToFontInfo(reader, xmlReader, fontInfo, EmbeddedFontStyle.BoldItalic);
                        break;
                    default:
                        xmlReader.IgnoreElement();
                        break;
                }
            }

            return fontInfo;
        }

        private static void AddEmbeddedDataToFontInfo(DocxDocumentReaderBase reader, NrxXmlReader xmlReader,
            FontInfo fontInfo, EmbeddedFontStyle embeddedFontStyle)
        {
            string relId = xmlReader.ReadId();
            string fontKey = xmlReader.ReadAttribute("fontKey", null);
            bool isSubsetted = xmlReader.ReadBoolAttribute("subsetted", false);

            byte[] data = reader.GetDetachedBinData(relId);
            if (fontKey != null)
                DocxFontObfuscator.DeObfuscate(data, fontKey);

            // DOCX can contain up to 4 identical font files of different styles (regular, bold, italic, bold-italic)
            // used for a font that has no special file for particular style. GetNonDuplicatedBytes function
            // saves space in the model for this case.
            data = reader.GetNonDuplicatedBytes(data);

            fontInfo.AddEmbeddedFont(data, EmbeddedFontFormat.OpenType, embeddedFontStyle, isSubsetted);
        }
    }
}
