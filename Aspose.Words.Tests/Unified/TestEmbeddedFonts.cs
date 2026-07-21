// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.IO;
using Aspose.Words.Fonts;
using Aspose.Words.Saving;
using Aspose.Words.Tests.Fonts;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// <para>Tests for fonts embedded in documents.</para>
    /// </summary>
    /// <remarks>
    /// FOSS: DOCX font-embedding OUTPUT was removed — DocxFontTableWriter strips all embedded fonts
    /// on save (FontInfos.CloneWithoutEmbeddedFonts), and DOC/RTF/HTML load+save are gone. The many
    /// tests that asserted embedded fonts are preserved / created / subsetted (incl. all DOC/RTF/HTML
    /// and font-merge cases) were removed accordingly. What remains verifies the surviving behavior:
    /// fonts are NOT embedded (embedding disabled or removed) and long font-name handling.
    /// </remarks>
    [TestFixture]
    public class TestEmbeddedFonts : UnifiedTestsBase
    {

        /// <summary>
        /// Relates to WORDSNET-21120
        /// Checks when <see cref="SaveOptions.AllowEmbeddingPostScriptFonts"/> option is on, but font embedding is disabled.
        /// </summary>
        // FOSS: reduced to Docx — Doc/Rtf save were removed.
        [TestCase(SaveFormat.Docx)]
        public void Test21120B(SaveFormat saveFormat)
        {
            Document doc = CreateDocumentTest21120();

            // Disable fonts embedding.
            doc.FontInfos.EmbedTrueTypeFonts = false;
            doc.FontInfos.EmbedSystemFonts = false;

            // Enable embedding PostScript fonts.
            SaveOptions saveOptions = SaveOptions.CreateSaveOptions(saveFormat);
            saveOptions.AllowEmbeddingPostScriptFonts = true;

            // Roundtrip the document with embedding problematic fonts.
            string outFileName = string.Format(@"Model\FontEmbedding\Test21120\Test21120_NoEmbedding{0}",
                FileFormatUtil.SaveFormatToExtension(saveFormat));
            doc = TestUtil.SaveOpen(doc, outFileName, saveOptions, false);

            // Check PostScript fonts are not embedded, as embedding of fonts is completely disabled in FontInfos.
            FontInfo fontInfo = doc.FontInfos["AllensHand"];
            Assert.That((fontInfo == null) || !fontInfo.HasEmbeddedFonts, Is.True);

            fontInfo = doc.FontInfos["Allura"];
            Assert.That((fontInfo == null) || !fontInfo.HasEmbeddedFonts, Is.True);

            // Check TrueType fonts are not embedded.
            fontInfo = doc.FontInfos["Long Cang"];
            Assert.That((fontInfo == null) || !fontInfo.HasEmbeddedFonts, Is.True);

            fontInfo = doc.FontInfos["Pacifico"];
            Assert.That((fontInfo == null) || !fontInfo.HasEmbeddedFonts, Is.True);
        }


        /// <summary>
        /// Tests embedded fonts removal.
        /// </summary>
        // FOSS: reduced to Docx2Docx — Doc/Rtf load+save were removed.
        [Test]
        [TestCase(LoadFormat.Docx, SaveFormat.Docx)]
        public void UnifiedTestRemoveEmbeddedFonts(LoadFormat lf, SaveFormat sf)
        {
            const string srcFileName = @"Model\FontEmbedding\TestEmbeddedAllStyles";
            Document doc = TestUtil.Open(srcFileName, lf);

            doc.FontInfos.EmbedTrueTypeFonts = false;

            string outFileName = TestUtil.BuildOutFileName(srcFileName, "", sf);
            doc = TestUtil.SaveOpen(doc, outFileName, SaveOptions.CreateSaveOptions(sf), false);

            Assert.That(doc.FontInfos.HasEmbeddedFonts(), Is.False);
            Assert.That(doc.FontInfos.EmbedTrueTypeFonts, Is.False);
            Assert.That(doc.DocPr.EmbedTrueTypeFonts, Is.False);
        }

        /// <summary>
        /// Creates test document for WORDSNET-21120
        /// </summary>
        private static Document CreateDocumentTest21120()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Create content with problematic fonts.
            builder.Font.Name = "AllensHand";
            builder.Writeln("AllensHand font.");

            builder.Font.Name = "Long Cang";
            builder.Writeln("Long Cang font.");

            builder.Font.Name = "Pacifico";
            builder.Writeln("Pacifico font.");

            builder.Font.Name = "Allura";
            builder.Write("Allura font.");

            // FOSS: font sources / substitution were removed; the test only asserts that fonts are
            // NOT embedded, which holds regardless of whether the source fonts are loaded.
            return doc;
        }
    }
}
