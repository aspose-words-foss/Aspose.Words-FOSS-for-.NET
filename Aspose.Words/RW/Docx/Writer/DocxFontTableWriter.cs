// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/07/2007 by Vladimir Averkin

using System;
using Aspose.Common;
using Aspose.OpcPackaging;
using Aspose.Words.Fonts;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Nrx.Writer;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Styles;

namespace Aspose.Words.RW.Docx.Writer
{
    /// <summary>
    /// Provides static method for writing "Font Table" package part
    /// </summary>
    internal class DocxFontTableWriter
    {
        /// <summary>
        /// Writes "Font Table" document part for the specified document.
        /// </summary>
        internal static void Write(DocxDocumentWriterBase writer)
        {
            new DocxFontTableWriter(writer).Write();
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        private DocxFontTableWriter(DocxDocumentWriterBase writer)
        {
            mWriter = writer;
            mBuilder = writer.CreateChildPartAndBuilder("fontTable.xml", DocxContentType.FontTable, writer.RelTypes.FontTable);

            DocumentBase doc = mWriter.Document;
            DocPr docPr = doc.DocPr;
            bool embedTrueTypeFonts = docPr.EmbedTrueTypeFonts;
            bool embedSystemFonts = docPr.EmbedSystemFonts;
            bool saveSubsetFonts = docPr.SaveSubsetFonts;
            // FOSS Removed subsetting of embedded fonts.
            mFontInfos = doc.FontInfos.CloneWithoutEmbeddedFonts();

            if (!embedTrueTypeFonts && (embedSystemFonts || saveSubsetFonts))
                writer.Warn(WarningType.FontEmbedding, WarningStrings.FontsNotEmbeddedDueToEmbeddingDisabled);
        }

        /// <summary>
        /// Writes "Font Table" document part using <see cref="mWriter"/>, <see cref="mBuilder"/> and <see cref="mFontInfos"/>
        /// initialized in ctor.
        /// </summary>
        private void Write()
        {
            mBuilder.StartFontTableDocumentPart();

            // Write w:font elements.
            foreach (FontInfo fontInfo in mFontInfos)
                WriteFontInfo(fontInfo);

            mBuilder.EndDocument(); //w:fonts
        }

        /// <summary>
        /// Writes information related to one font from <see cref="FontInfoCollection"/> of the document.
        /// </summary>
        private void WriteFontInfo(FontInfo fontInfo)
        {
            Debug.Assert(fontInfo != null);

            mBuilder.StartElement("w:font");

            string fontName = fontInfo.Name;

            // WORDSNET-13825 Try to fix font name if it is invalid.
            if (mBuilder.HasInvalidChars(fontName) && (!mBuilder.HasInvalidChars(fontInfo.AltName)))
                fontName = fontInfo.AltName;

            // WORDSNET-19382 Truncate font name to maximum allowed length.
            if (fontName.Length > FontInfo.MaxFontNameLength)
                fontName = fontName.Substring(0, FontInfo.MaxFontNameLength);

            mBuilder.WriteAttributeString("w:name", fontName);

            NrxFontWriter.WriteAltName(fontInfo, mBuilder, SaveFormat.Docx);

            if (fontInfo.Panose != null)
                mBuilder.WriteVal("w:panose1", StringUtil.BytesToHex(fontInfo.Panose));

            if (mBuilder.OoxmlCompliance == OoxmlComplianceCore.IsoStrict)
                mBuilder.WriteElementWithAttributes("w:charset", "w:characterSet", fontInfo.IanaCharset);
            else
                mBuilder.WriteVal("w:charset", FormatterPal.IntToStrX2(fontInfo.Charset));
            mBuilder.WriteVal("w:family", DocxEnum.FontFamilyToDocx(fontInfo.Family));

            if (!fontInfo.IsTrueType)
                mBuilder.WriteEmptyElement("w:notTrueType");

            mBuilder.WriteVal("w:pitch", StyleConvertUtil.FontPitchToXml(fontInfo.Pitch));

            if (fontInfo.Sig != null)
                WriteSigElement(fontInfo.Sig);

            EmbeddedFont[] embeddedFonts = fontInfo.GetEmbeddedFonts();
            if (embeddedFonts != null)
                WriteEmbeddedFonts(embeddedFonts);

            mBuilder.EndElement(); // font
        }

        /// <summary>
        /// Writes <see cref="FontInfo.Sig"/> of specific font to font table.
        /// </summary>
        /// <param name="sig"></param>
        private void WriteSigElement(byte[] sig)
        {
            Debug.Assert(sig != null);

            mBuilder.StartElement("w:sig");
            mBuilder.WriteAttributeString("w:usb0", NrxXmlUtil.IntToHex(sig, 0));
            mBuilder.WriteAttributeString("w:usb1", NrxXmlUtil.IntToHex(sig, 4));
            mBuilder.WriteAttributeString("w:usb2", NrxXmlUtil.IntToHex(sig, 8));
            mBuilder.WriteAttributeString("w:usb3", NrxXmlUtil.IntToHex(sig, 12));
            mBuilder.WriteAttributeString("w:csb0", NrxXmlUtil.IntToHex(sig, 16));
            mBuilder.WriteAttributeString("w:csb1", NrxXmlUtil.IntToHex(sig, 20));
            mBuilder.EndElement();    // sig
        }

        /// <summary>
        /// Writes up to four embedded fonts of different <see cref="EmbeddedFontStyle"/>s for one font name.
        /// </summary>
        private void WriteEmbeddedFonts(EmbeddedFont[] embeddedFonts)
        {
            Debug.Assert(embeddedFonts != null);

            foreach (EmbeddedFont embeddedFont in embeddedFonts)
            {
                if (embeddedFont == null)
                    continue;

                // DOCX supports 'OpenType' font embedding format.
                EmbeddedFont openTypeFont = embeddedFont.GetAsOpenType();
                if (openTypeFont == null)
                    continue;

                string embeddedFontName = "fonts/font" + FormatterPal.IntToStr(mEmbeddedFontOrdinal++) + ".odttf";
                WriteEmbeddedFont(openTypeFont, embeddedFontName);
            }
        }

        /// <summary>
        /// Writes embedded font file to DOCX package and record to font table of the package.
        /// </summary>
        /// <param name="embeddedFont"></param>
        /// <param name="embeddedFontName"></param>
        private void WriteEmbeddedFont(EmbeddedFont embeddedFont, string embeddedFontName)
        {
            Debug.Assert(embeddedFont.Format == EmbeddedFontFormat.OpenType);

            string relId;
            OpcPackagePart embeddedFontPart = mWriter.Package.CreateChildPart(
                mBuilder.Part,
                embeddedFontName,
                OpcContentType.Odttf,
                mWriter.RelTypes.Font, out relId);

            string fontKey = DocxFontObfuscator.WriteObfuscatedToStream(embeddedFont, embeddedFontPart.Stream);

            mBuilder.StartElement(GetEmbeddedFontElementName(embeddedFont.Style));
            mBuilder.WriteAttributeString("r:id", relId);
            mBuilder.WriteAttributeIfTrue("w:subsetted", embeddedFont.IsSubsetted);
            mBuilder.WriteAttributeString("w:fontKey", fontKey);
            mBuilder.EndElement();
        }

        /// <summary>
        /// Converts <see cref="EmbeddedFontStyle"/> to name of element in font table.
        /// </summary>
        private static string GetEmbeddedFontElementName(EmbeddedFontStyle fontStyle)
        {
            switch (fontStyle)
            {
                case EmbeddedFontStyle.Regular:
                    return "w:embedRegular";
                case EmbeddedFontStyle.Bold:
                    return "w:embedBold";
                case EmbeddedFontStyle.Italic:
                    return "w:embedItalic";
                case EmbeddedFontStyle.BoldItalic:
                    return "w:embedBoldItalic";
                default:
                    throw new ArgumentException("fontStyle");
            }
        }

        private readonly DocxDocumentWriterBase mWriter;
        private readonly DocxBuilder mBuilder;
        private readonly FontInfoCollection mFontInfos;

        /// <summary>
        /// It's used to create unique embedded font names in package.
        /// </summary>
        private int mEmbeddedFontOrdinal = 1;
    }
}
