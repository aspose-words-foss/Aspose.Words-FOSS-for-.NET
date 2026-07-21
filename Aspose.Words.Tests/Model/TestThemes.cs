// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/06/2015 by Alexey Morozov

using System;
using System.Drawing;
using System.IO;
using System.Xml;
using Aspose.Collections;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Nrx;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Tests.Export.Docx;
using Aspose.Words.Themes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    [TestFixture]
    public class TestThemes
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests how theme fonts are accessed public.
        /// </summary>
        [Test]
        public void TestThemeFonts()
        {
            const string testName = @"Model\Theme\TestThemeFonts.docx";
            Document doc = TestUtil.Open(testName);

            // Check few fonts.
            Theme theme = doc.Theme;
            Assert.That(theme.MajorFonts.Latin, Is.EqualTo("Cooper Black"));
            Assert.That(theme.MinorFonts.Latin, Is.EqualTo("Consolas"));
            Assert.That(theme.MinorFonts.EastAsian, Is.EqualTo(""));

            // Set to new values.
            theme.MajorFonts.Latin = "Times New Roman";
            theme.MajorFonts.EastAsian = null;
            theme.MinorFonts.Latin = "Arial";
            theme.MinorFonts.EastAsian = "MS Mincho";

            doc = TestUtil.SaveOpen(doc, testName, UnifiedScenario.Docx2DocxNoGold);

            theme = doc.Theme;
            Assert.That(theme.MajorFonts.Latin, Is.EqualTo("Times New Roman"));
            Assert.That(theme.MajorFonts.EastAsian, Is.EqualTo(""));
            Assert.That(theme.MinorFonts.Latin, Is.EqualTo("Arial"));
            Assert.That(theme.MinorFonts.EastAsian, Is.EqualTo("MS Mincho"));
        }

        /// <summary>
        /// Tests how theme fonts are accessed public.
        /// </summary>
        [Test]
        public void TestThemeSupplementalFonts()
        {
            const string testName = @"Model\Theme\TestThemeSupplimentalFonts.docx";
            Document doc = TestUtil.Open(testName);

            Theme theme = doc.Theme;
            Assert.That(doc.Theme.MajorFonts.SupplementalFonts.Count, Is.EqualTo(30));
            Assert.That(doc.Theme.MinorFonts.SupplementalFonts.Count, Is.EqualTo(30));

            // Modify theme.
            theme.MajorFonts.Latin = "Courier";

            doc = TestUtil.SaveOpen(doc, testName, UnifiedScenario.Docx2DocxNoGold);

            // Check that supplemental font collection is preserved.
            theme = doc.Theme;
            Assert.That(theme.MajorFonts.SupplementalFonts.Count, Is.EqualTo(30));
            Assert.That(doc.Theme.MinorFonts.SupplementalFonts.Count, Is.EqualTo(30));
        }


        /// <summary>
        /// Tests how theme colors are accessed public.
        /// </summary>
        [Test]
        public void TestThemeColors()
        {
            const string testName = @"Model\Theme\TestThemeColors.docx";
            Document doc = TestUtil.Open(testName);

            SetAllColors(doc.Theme);
            CheckAllColors(doc.Theme);

            doc = TestUtil.SaveOpen(doc, testName, UnifiedScenario.Docx2DocxNoGold);

            CheckAllColors(doc.Theme);
        }

        /// <summary>
        /// Tests how theme accessed when document has no theme initially.
        /// </summary>
        [Test]
        public void TestNoTheme()
        {
            // FOSS: the Doc reader is removed, so the original themeless TestNoTheme.doc can no longer
            // be loaded. Reproduce the same "document with no internal theme" state directly instead;
            // the tested behavior (theme is lazily created from the built-in resource) is unchanged.
            Document doc = new Document();
            doc.SetThemeInternal(null);

            // Although we have no theme internally it is created on first public access.
            Assert.That(doc.GetThemeInternal(), Is.Null);
            Assert.That(doc.Theme, IsNot.Null());

            Theme theme = doc.Theme;
            // Check that theme is created only once.
            Assert.That(ReferenceEquals(theme, doc.Theme), Is.True);

            Assert.That(theme.MajorFonts.Latin, Is.EqualTo("Cambria"));
            Assert.That(theme.MinorFonts.Latin, Is.EqualTo("Calibri"));

            SetAllColors(theme);
            CheckAllColors(theme);
        }


        /// <summary>
        /// Tests that extension properties are read/written correctly.
        /// </summary>
        [Test]
        public void TestExtension()
        {
            const string documentName = @"Model\Theme\TestThemeColors.docx";
            Document doc = TestUtil.Open(documentName);

            CheckThemeFamilyExtension(((IDmlExtensionListSource)doc.Theme).Extensions, false);

            // We need to specify ISO Strict compliance here to regenerate theme stream on writing.
            doc = TestUtil.SaveOpen(doc, documentName,
                OoxmlSaveOptions.DocxWithCompliance(OoxmlComplianceCore.IsoStrict), false);

            CheckThemeFamilyExtension(((IDmlExtensionListSource)doc.Theme).Extensions, true);
        }

        /// <summary>
        /// Checks properties of the theme family extension in the specified extension list.
        /// </summary>
        private static void CheckThemeFamilyExtension(StringToObjDictionary<DmlExtension> extensionList, bool expectedIsoStrictUrl)
        {
            Assert.That(extensionList, IsNot.Null());

            Assert.That(extensionList["{05A4C25C-085E-4340-85A3-A5531E510DB2}"].Uri, Is.EqualTo("{05A4C25C-085E-4340-85A3-A5531E510DB2}"));

            const string extensionXml =
                "<a:ext uri=\"{{05A4C25C-085E-4340-85A3-A5531E510DB2}}\" xmlns:a=\"{0}\">" +
                    "<thm15:themeFamily id=\"{{62F939B6-93AF-4DB8-9C6B-D6C7DFDC589F}}\" name=\"Office Theme\" " +
                        "vid=\"{{4A3C46E8-61CC-4603-A589-7422A47A8E4A}}\" " +
                        "xmlns:thm15=\"http://schemas.microsoft.com/office/thememl/2012/main\" />" +
                "</a:ext>";
            Assert.That(extensionList["{05A4C25C-085E-4340-85A3-A5531E510DB2}"].XmlDoc, Is.EqualTo(string.Format(extensionXml,
                    DocxNamespaces.GetNamespace(DocxNamespace.DrawingMLMain, expectedIsoStrictUrl))));
        }

        /// <summary>
        /// Tests that several extensions are read/written correctly.
        /// </summary>
        [Test]
        public void TestReadWriteTwoExtensions()
        {
            const string documentName = @"Model\Theme\TestTwoExtensions_ManuallyGenerated.docx";
            Document doc = TestUtil.Open(documentName);

            CheckExtExtension(((IDmlExtensionListSource)doc.Theme).Extensions);

            // We need to specify ISO Strict compliance here to regenerate theme stream on writing.
            doc = TestUtil.SaveOpen(doc, documentName,
                OoxmlSaveOptions.DocxWithCompliance(OoxmlComplianceCore.IsoStrict), false);

            CheckExtExtension(((IDmlExtensionListSource)doc.Theme).Extensions);
        }

        // FOSS: VerifyRtfThemeExport removed — the RTF reader/writer are removed, so an RTF
        // theme import/export round-trip cannot be performed.

        /// <summary>
        /// Checks additional extension in theme extension list. The extension is not real; it is added for test only.
        /// </summary>
        private static void CheckExtExtension(StringToObjDictionary<DmlExtension> extensionList)
        {
            Assert.That(extensionList, IsNot.Null());
            Assert.That(extensionList.Count, Is.EqualTo(2));
            // The extension is not real; it is added for test only.
            Assert.That(extensionList["{05A4C25C-085E-4340-85A3-A5531E510DB3}"].Uri, Is.EqualTo("{05A4C25C-085E-4340-85A3-A5531E510DB3}"));
        }






        // FOSS: TestBadThemeFontDescription and TestRtfTheme removed — both load .rtf inputs to
        // verify RTF-specific theme import (RTF carries no theme data, so MSW resolves fonts/colors
        // from the default theme). The RTF reader is removed, so these scenarios cannot exist.


        /// <summary>
        /// Additional test for WORDSNET-15143
        /// </summary>
        [Test]
        public void TestJira15146()
        {
            const string testName = @"Model\Theme\TestJira15146";

            Document doc = TestUtil.Open(testName, LoadFormat.Docx);
            Table table = doc.FirstSection.Body.Tables[0];

            table.FirstRow.FirstCell.CellFormat.Borders.Bottom.Color = Color.Red;

            doc = TestUtil.SaveOpen(doc, testName, UnifiedScenario.Docx2DocxNoGold);

            table = doc.FirstSection.Body.Tables[0];
            Assert.That(table.FirstRow.FirstCell.CellFormat.Borders.Bottom.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
        }


        /// <summary>
        /// WORDSNET-15818 Traditional Chinese fonts should be used for certain Chinese locales instead of simplified fonts.
        /// </summary>
        [TestCase(Language.ChineseChina, "宋体")]
        [TestCase(Language.ChineseSingapore, "宋体")]
        [TestCase(Language.ChineseHongKong, "新細明體")]
        [TestCase(Language.ChineseMacao, "新細明體")]
        [TestCase(Language.ChineseTaiwan, "新細明體")]
        public void TestJira15818(Language chineseLanguage, string expectedFontName)
        {
            Document doc = new Document();
            doc.Theme.ThemeFontLanguages.EastAsia = chineseLanguage;
            Assert.That(((IThemeProvider)doc.Theme).GetFontName(ThemeFontCore.MajorEastAsia), Is.EqualTo(expectedFontName));
            Assert.That(((IThemeProvider)doc.Theme).GetFontName(ThemeFontCore.MinorEastAsia), Is.EqualTo(expectedFontName));
        }


        // FOSS: TestJira6840 (MHTML) and TestJira16563 (HTML) removed — both verify that the
        // MHTML/HTML writer emits the *actual* system-color value rather than the theme's cached
        // LastColor. That behavior lives in the removed MHTML/HTML export path; a DOCX round-trip
        // preserves the sysClr/lastClr as-is (LastColor stays the source value, not the resolved
        // one), so the invariant cannot be reproduced without those writers.


        /// <summary>
        /// WORDSNET-17840 Border color not modified when changing Accent color.
        /// Theme should be applied to the paragraph attributes of the style.
        /// </summary>
        [Test]
        public void TestJira17840()
        {
            Document doc = TestUtil.Open(@"Model\Theme\TestJira17840.docx");
            doc.Theme.Colors.Accent1 = Color.Yellow;

            doc = TestUtil.SaveOpen(doc, @"Model\Theme\TestJira17840.docx", null, false);

            Style style = doc.Styles["Heading 1"];
            DrColor color = style.ParaPr.BorderBottom.ColorInternal;
            Assert.That(color, Is.EqualTo(DrColor.Yellow));
        }

        /// <summary>
        /// WORDSNET-21183 Set custom font style as “linked” to the Theme’s font does not work.
        /// Implemented new public properties that allow to work with themed fonts.
        /// </summary>
        /// <remarks>This test checks customer's scenario.</remarks>
        [Test]
        public void Test21183()
        {
            Document doc = TestUtil.Open(@"Model\Theme\Test21183.docx");
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();
            builder.Writeln();

            // Create some style with theme font properties.
            Style style = doc.Styles.Add(StyleType.Paragraph, "From Code");
            style.Font.ThemeFont = ThemeFont.Major;
            style.Font.ThemeColor = ThemeColor.Accent5;
            builder.ParagraphFormat.StyleName = "From Code";
            builder.Writeln("From Code: This is in \"From Code\" style created");

            doc = TestUtil.SaveOpen(doc, @"Model\Theme\Test21183.docx");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            // Check Theme's font properties of the run with built-in style created using Word.
            Run run = paras[0].FirstRun;
            Assert.That(run.Text, Is.EqualTo("This is in “Heading 1” style"));
            Assert.That(run.Font.Color, Is.EqualTo(Color.FromArgb(0x2F, 0x54, 0x96)));
            Assert.That(run.Font.ThemeColor, Is.EqualTo(ThemeColor.Accent1));
            Assert.That(run.Font.TintAndShade, Is.EqualTo(-0.25).Within(0.01));
            Assert.That(run.Font.ThemeFont, Is.EqualTo(ThemeFont.Major));
            CheckThemeFontNames(run.Font, ThemeFont.Major);

            // Check Theme's font properties of the run with custom style created using Word.
            run = paras[1].FirstRun;
            Assert.That(run.Text, Is.EqualTo("This is in “Custom Style” style created in MS Word"));
            Assert.That(run.Font.Color, Is.EqualTo(doc.Theme.Colors.Accent1));
            Assert.That(run.Font.ThemeColor, Is.EqualTo(ThemeColor.Accent1));
            CheckThemeFontNames(run.Font, ThemeFont.Minor);

            // Check Theme's font properties of the run with style created from the scratch in code.
            run = paras[2].FirstRun;
            Assert.That(run.Text, Is.EqualTo("From Code: This is in \"From Code\" style created"));
            Assert.That(run.Font.Color, Is.EqualTo(doc.Theme.Colors.Accent5));
            Assert.That(run.Font.ThemeColor, Is.EqualTo(ThemeColor.Accent5));
            CheckThemeFontNames(run.Font, ThemeFont.Major);
        }

        /// <summary>
        /// Relates to  WORDSNET-21183
        /// Tests changing non-theme font name to the themed one and vice versa by applying theme font 'None'.
        /// </summary>
        [Test]
        public void TestThemeFontNameA()
        {
            Document doc = CreateNewDocumentWithThemes();
            Font font = doc.Styles["Normal"].Font;

            // Set non-theme font name.
            font.ThemeFont = ThemeFont.None;
            doc = TestUtil.SaveOpen(doc, @"Model\Theme\TestThemeFontNameA.docx", new OoxmlSaveOptions(), false);

            font = doc.Styles["Normal"].Font;
            CheckThemeFontNames(font, ThemeFont.None);
            CheckResolvedFontNames(font, "Algerian", "Aharoni", "Algerian", "Andalus");
        }

        /// <summary>
        /// Relates to  WORDSNET-21183
        /// Checks changing non-theme font name to themed one and vice versa by applying some simple font name.
        /// </summary>
        [Test]
        public void TestThemeFontNameB()
        {
            Document doc = CreateNewDocumentWithThemes();
            Font font = doc.Styles["Normal"].Font;

            // Set non-theme font name.
            font.Name = "Arial";
            doc = TestUtil.SaveOpen(doc, @"Model\Theme\TestThemeFontNameB.docx", new OoxmlSaveOptions(), false);

            font = doc.Styles["Normal"].Font;
            CheckThemeFontNames(font, ThemeFont.None);
            CheckResolvedFontNames(font, "Arial", "Arial", "Arial", "Arial");
        }

        /// <summary>
        /// Relates to  WORDSNET-21183
        /// Tests changing non-theme color to the theme color and vice versa by applying theme font color 'None'.
        /// </summary>
        [Test]
        public void TestThemeFontColorA()
        {
            Document doc = new Document();

            // Check a simple RGB color.
            Font font = doc.Styles["Normal"].Font;
            Assert.That(font.ThemeColor, Is.EqualTo(ThemeColor.None));
            Assert.That(font.Color, Is.EqualTo(Color.Empty));

            // Apply theme color.
            font.ThemeColor = ThemeColor.Accent2;
            doc = TestUtil.SaveOpen(doc, @"Model\Theme\TestThemeFontColorA_Accent2.docx", new OoxmlSaveOptions(), false);
            font = doc.Styles["Normal"].Font;
            Assert.That(font.ThemeColor, Is.EqualTo(ThemeColor.Accent2));
            Assert.That(font.Color, Is.EqualTo(Color.Empty));

            // Set theme color back to 'None'.
            font.ThemeColor = ThemeColor.None;
            doc = TestUtil.SaveOpen(doc, @"Model\Theme\TestThemeFontColorA_None.docx", new OoxmlSaveOptions(), false);
            font = doc.Styles["Normal"].Font;
            Assert.That(font.ThemeColor, Is.EqualTo(ThemeColor.None));
            Assert.That(font.Color, Is.EqualTo(Color.Empty));
        }

        /// <summary>
        /// Relates to  WORDSNET-21183
        /// Tests changing non-theme color to the theme color and vice versa by applying some simple color.
        /// </summary>
        [Test]
        public void TestThemeFontColorB()
        {
            Document doc = new Document();

            // Check a simple RGB color.
            Font font = doc.Styles["Normal"].Font;
            Assert.That(font.ThemeColor, Is.EqualTo(ThemeColor.None));
            Assert.That(font.Color, Is.EqualTo(Color.Empty));

            // Apply theme color.
            font.ThemeColor = ThemeColor.Accent2;
            doc = TestUtil.SaveOpen(doc, @"Model\Theme\TestThemeFontColorB_Accent2.docx", new OoxmlSaveOptions(), false);
            font = doc.Styles["Normal"].Font;
            Assert.That(font.ThemeColor, Is.EqualTo(ThemeColor.Accent2));
            Assert.That(font.Color, Is.EqualTo(Color.Empty));

            // Set simple RGB color.
            font.Color = Color.Blue;
            doc = TestUtil.SaveOpen(doc, @"Model\Theme\TestThemeFontColorB_RGB_Blue.docx", new OoxmlSaveOptions(), false);
            font = doc.Styles["Normal"].Font;
            Assert.That(font.ThemeColor, Is.EqualTo(ThemeColor.None));
            Assert.That(font.Color.ToArgb(), Is.EqualTo(Color.Blue.ToArgb()));
        }

        /// <summary>
        /// Relates to  WORDSNET-21183
        /// Tests various combinations of TintAndShade.
        /// </summary>
        [TestCase (0.4)]
        [TestCase (-0.25)]
        [TestCase (1.0)]
        [TestCase (-1.0)]
        [TestCase (0.0)]
        public void TestTintAndShade(double tintAndShade)
        {
            const ThemeColor desiredThemeColor = ThemeColor.Accent6;

            Document doc = new Document();
            Font font = doc.Styles["Normal"].Font;

            font.ThemeColor = desiredThemeColor;
            font.TintAndShade = tintAndShade;

            string outFileName = string.Format(@"Model\Theme\TestTintAndShade_{0}.docx", tintAndShade);
            doc = TestUtil.SaveOpen(doc, outFileName, new OoxmlSaveOptions(), false);

            font = doc.Styles["Normal"].Font;
            Assert.That(font.TintAndShade, Is.EqualTo(tintAndShade).Within(0.01));
            Assert.That(font.ThemeColor, Is.EqualTo(desiredThemeColor));
            Assert.That(font.Color, Is.EqualTo(Color.Empty));
        }

        /// <summary>
        /// Relates to  WORDSNET-21183
        /// Tests that exception is thrown when applying value to TintAndShade out of [-1, 1] range.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestTintAndShadeOutOfRange()
        {
            Document doc = new Document();
            Font font = doc.Styles["Normal"].Font;
            font.ThemeColor = ThemeColor.Accent2;
            // Apply TintAndShade value out of the allowed range [-1, 1].
            font.TintAndShade = 2;
        }

        /// <summary>
        /// Relates to  WORDSNET-21183
        /// Tests that exception is thrown when applying value to TintAndShade for non-theme font color.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestTintAndShadeNonThemeColor()
        {
            Document doc = new Document();
            Font font = doc.Styles["Normal"].Font;
            // Apply TintAndShade to the non-theme font color.
            font.TintAndShade = 0.5;
        }

        /// <summary>
        /// WORDSNET-24441 Add Border.ThemeColor.
        /// Tests the public API of ThemeColor and TintAndShade properties in Border.
        /// Style border case.
        /// </summary>
        [Test]
        public void Test24441_Style()
        {
            Document doc = TestUtil.Open(@"Model\Theme\Test24441.docx");

            BorderCollection heading1Borders = doc.Styles[StyleIdentifier.Heading1].ParagraphFormat.Borders;
            CheckBorderColors(heading1Borders.Bottom, ThemeColor.Accent1, Color.Empty);
            CheckBorderColors(heading1Borders.Top, ThemeColor.None, Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
            heading1Borders.Bottom.ThemeColor = ThemeColor.Accent2;
            heading1Borders.Top.ThemeColor = ThemeColor.Accent1;
            heading1Borders.Top.TintAndShade = 0.5;

            doc = TestUtil.SaveOpen(doc, @"Model\Theme\Test24441_Style.docx", new OoxmlSaveOptions(), false);
            heading1Borders = doc.Styles[StyleIdentifier.Heading1].ParagraphFormat.Borders;

            CheckBorderColors(heading1Borders.Bottom, ThemeColor.Accent2, Color.Empty);
            CheckBorderColors(heading1Borders.Top, ThemeColor.Accent1, Color.Empty);
            Assert.That(heading1Borders.Top.TintAndShade, Is.EqualTo(0.5).Within(0.01));
        }

        /// <summary>
        /// Relates to WORDSNET-24441
        /// Tests the public API of ThemeColor and TintAndShade properties in Border.
        /// Paragraph border case.
        /// </summary>
        [Test]
        public void Test24441_Paragraph()
        {
            Document doc = TestUtil.Open(@"Model\Theme\Test24441.docx");

            BorderCollection paraBorders = doc.FirstSection.Body.Paragraphs[1].ParagraphFormat.Borders;
            CheckBorderColors(paraBorders.Bottom, ThemeColor.Accent5, Color.FromArgb(0xFF, 0x5B, 0x9B, 0xD5));
            CheckBorderColors(paraBorders.Top, ThemeColor.Accent5, Color.FromArgb(0xFF, 0x5B, 0x9B, 0xD5));
            paraBorders.Bottom.ThemeColor = ThemeColor.Accent1;
            paraBorders.Top.Color = Color.Red;
            paraBorders.Bottom.TintAndShade = -0.5;

            doc = TestUtil.SaveOpen(doc, @"Model\Theme\Test24441_Paragraph.docx", new OoxmlSaveOptions(), false);
            paraBorders = doc.FirstSection.Body.Paragraphs[1].ParagraphFormat.Borders;

            CheckBorderColors(paraBorders.Bottom, ThemeColor.Accent1, Color.Empty);
            CheckBorderColors(paraBorders.Top, ThemeColor.None, Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
            Assert.That(paraBorders.Bottom.TintAndShade, Is.EqualTo(-0.5).Within(0.01));
        }

        /// <summary>
        /// Relates to WORDSNET-24441
        /// Tests the public API of ThemeColor and TintAndShade properties in Border.
        /// Text border case.
        /// </summary>
        [Test]
        public void Test24441_Text()
        {
            Document doc = TestUtil.Open(@"Model\Theme\Test24441.docx");

            Border textBorder = doc.FirstSection.Body.Paragraphs[3].FirstRun.Font.Border;
            CheckBorderColors(textBorder, ThemeColor.Accent3, Color.Empty);
            textBorder.ThemeColor = ThemeColor.Accent1;
            textBorder.TintAndShade = -0.25;

            doc = TestUtil.SaveOpen(doc, @"Model\Theme\Test24441_Text.docx", new OoxmlSaveOptions(), false);
            textBorder = doc.FirstSection.Body.Paragraphs[3].FirstRun.Font.Border;

            CheckBorderColors(textBorder, ThemeColor.Accent1, Color.Empty);
            Assert.That(textBorder.TintAndShade, Is.EqualTo(-0.25).Within(0.01));
        }

        /// <summary>
        /// Relates to WORDSNET-24441
        /// Tests the public API of ThemeColor and TintAndShade properties in Border.
        /// Table border case.
        /// </summary>
        [Test]
        public void Test24441_Table()
        {
            Document doc = TestUtil.Open(@"Model\Theme\Test24441.docx");

            BorderCollection tableBorders = doc.FirstSection.Body.Tables[0].FirstRow.RowFormat.Borders;
            CheckBorderColors(tableBorders.Bottom, ThemeColor.Accent6, Color.FromArgb(0xFF, 0x70, 0xAD, 0x47));
            CheckBorderColors(tableBorders.Top, ThemeColor.Accent6, Color.FromArgb(0xFF, 0x70, 0xAD, 0x47));
            tableBorders.Bottom.ThemeColor = ThemeColor.Accent3;
            tableBorders.Top.Color = Color.Red;
            tableBorders.Bottom.TintAndShade = 0.25;
            // The setter resets the border color value.
            CheckBorderColors(tableBorders.Bottom, ThemeColor.Accent3, Color.Empty);

            doc = TestUtil.SaveOpen(doc, @"Model\Theme\Test24441_Table.docx", new OoxmlSaveOptions(), false);
            tableBorders = doc.FirstSection.Body.Tables[0].FirstRow.RowFormat.Borders;
            // The writer restores the border color by the appropriate theme color.
            CheckBorderColors(tableBorders.Bottom, ThemeColor.Accent3, Color.FromArgb(0xFF, 0xBB, 0xBB, 0xBB));
            CheckBorderColors(tableBorders.Top, ThemeColor.None, Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
            Assert.That(tableBorders.Bottom.TintAndShade, Is.EqualTo(0.25).Within(0.01));
        }

        /// <summary>
        /// Relates to  WORDSNET-24441
        /// Tests the public API of ThemeColor and TintAndShade properties in Border.
        /// Cell border case.
        /// </summary>
        [Test]
        public void Test24441_Cell()
        {
            Document doc = TestUtil.Open(@"Model\Theme\Test24441.docx");

            BorderCollection cellBorders = doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.CellFormat.Borders;
            CheckBorderColors(cellBorders.Bottom, ThemeColor.Accent4, Color.Empty);
            CheckBorderColors(cellBorders.Right, ThemeColor.Accent4, Color.Empty);
            cellBorders.Bottom.ThemeColor = ThemeColor.Accent1;
            cellBorders.Top.Color = Color.Red;
            cellBorders.Bottom.TintAndShade = 0.125;
            // Setter resets the border color value.
            CheckBorderColors(cellBorders.Bottom, ThemeColor.Accent1, Color.Empty);

            doc = TestUtil.SaveOpen(doc, @"Model\Theme\Test24441_Cell.docx", new OoxmlSaveOptions(), false);
            cellBorders = doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.CellFormat.Borders;
            // Writer restores the border color by the appropriate theme color.
            CheckBorderColors(cellBorders.Bottom, ThemeColor.Accent1, Color.FromArgb(0xFF, 0x5B, 0x83, 0xCB));
            CheckBorderColors(cellBorders.Top, ThemeColor.None, Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));
            Assert.That(cellBorders.Bottom.TintAndShade, Is.EqualTo(0.125).Within(0.01));
        }

        /// <summary>
        /// Relates to  WORDSNET-24441
        /// Tests that exception is thrown when applying value to TintAndShade out of [-1, 1] range.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestBorderTintAndShadeOutOfRange()
        {
            Document doc = new Document();
            Border border = doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Borders.Vertical;
            border.ThemeColor = ThemeColor.Accent2;
            // Apply TintAndShade value out of the allowed range [-1, 1].
            border.TintAndShade = 2;
        }

        /// <summary>
        /// Relates to  WORDSNET-24441
        /// Tests that exception is thrown when applying value to TintAndShade for non-theme font color.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void TestBorderTintAndShadeNonThemeColor()
        {
            Document doc = new Document();
            Border border = doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Borders.Vertical;
            // Apply TintAndShade to the non-theme font color.
            border.TintAndShade = 0.5;
        }

        /// <summary>
        /// WORDSNET-24456 Add Shading.ThemeColor.
        /// Tests the public API of ForegroundThemeColor, BackgroundThemeColor, ForegroundTintAndShade
        /// and BackgroundTintAndShade properties in Shading.
        /// Paragraph shading case.
        /// </summary>
        [Test]
        public void Test24456_Paragraph()
        {
            Document doc = TestUtil.Open(@"Model\Theme\Test24456_Para.docx");

            Shading shadingPara0 = doc.FirstSection.Body.Paragraphs[0].ParagraphFormat.Shading;
            Shading shadingPara1 = doc.FirstSection.Body.Paragraphs[1].ParagraphFormat.Shading;
            CheckShadingColors(shadingPara0, ThemeColor.None, ThemeColor.None,
                0, 0, Color.FromArgb(0xFF, 0x00, 0x00, 0xFF),
                Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF));
            CheckShadingColors(shadingPara1, ThemeColor.None, ThemeColor.None,
                0, 0, Color.FromArgb(0xFF, 0x00, 0x80, 0x00),
                Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));

            shadingPara0.ForegroundPatternThemeColor = ThemeColor.Accent1;
            shadingPara0.BackgroundPatternThemeColor = ThemeColor.Accent2;
            shadingPara1.ForegroundPatternThemeColor = ThemeColor.Dark1;
            shadingPara1.BackgroundPatternThemeColor = ThemeColor.Dark2;
            shadingPara1.ForegroundTintAndShade = 0.5;
            shadingPara1.BackgroundTintAndShade = -0.5;

            doc = TestUtil.SaveOpen(doc, @"Model\Theme\Test24456_Para.docx", new OoxmlSaveOptions(), false);
            shadingPara0 = doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Shading;
            shadingPara1 = doc.FirstSection.Body.Paragraphs[1].ParagraphFormat.Shading;

            CheckShadingColors(shadingPara0, ThemeColor.Accent1, ThemeColor.Accent2,
                0, 0, Color.Empty, Color.Empty);
            CheckShadingColors(shadingPara1, ThemeColor.Dark1, ThemeColor.Dark2,
                0.5, -0.5, Color.Empty, Color.Empty);
        }

        /// <summary>
        /// Relates to WORDSNET-24456
        /// Tests the public API of ForegroundThemeColor, BackgroundThemeColor, ForegroundTintAndShade
        /// and BackgroundTintAndShade properties in Shading.
        /// Text shading case.
        /// </summary>
        [Test]
        public void Test24456_Text()
        {
            Document doc = TestUtil.Open(@"Model\Theme\Test24456_Text.docx");

            Shading shadingFont = doc.FirstSection.Body.FirstParagraph.FirstRun.Font.Shading;
            CheckShadingColors(shadingFont, ThemeColor.None, ThemeColor.None, 0,
                0, Color.FromArgb(0xFF, 0x00, 0x00, 0xFF),
                Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));

            shadingFont.ForegroundPatternThemeColor = ThemeColor.Accent1;
            shadingFont.BackgroundPatternThemeColor = ThemeColor.Accent2;
            shadingFont.ForegroundTintAndShade = 0.5;
            shadingFont.BackgroundTintAndShade = -0.5;

            doc = TestUtil.SaveOpen(doc, @"Model\Theme\Test24456_Tara.docx", new OoxmlSaveOptions(), false);
            shadingFont = doc.FirstSection.Body.FirstParagraph.FirstRun.Font.Shading;

            CheckShadingColors(shadingFont, ThemeColor.Accent1, ThemeColor.Accent2,
                0.5, -0.5, Color.Empty, Color.Empty);
        }

        /// <summary>
        /// Relates to WORDSNET-24456
        /// Tests the public API of ForegroundThemeColor, BackgroundThemeColor, ForegroundTintAndShade
        /// and BackgroundTintAndShade properties in Shading.
        /// Table style shading case.
        /// </summary>
        [Test]
        public void Test24456_TableStyle()
        {
            Document doc = TestUtil.Open(@"Model\Theme\Test24456_TableStyle.docx");

            Shading shadingTable0 = ((TableStyle)doc.FirstSection.Body.Tables[0].Style).Shading;
            Shading shadingTable1 = ((TableStyle)doc.FirstSection.Body.Tables[1].Style).Shading;
            CheckShadingColors(shadingTable0, ThemeColor.None, ThemeColor.None,
                0, 0, Color.FromArgb(0xFF, 0x00, 0x00, 0xFF),
                Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF));
            CheckShadingColors(shadingTable1, ThemeColor.None, ThemeColor.None,
                0, 0, Color.FromArgb(0xFF, 0x00, 0x80, 0x00),
                Color.FromArgb(0xFF, 0xFF, 0x00, 0x00));

            shadingTable0.ForegroundPatternThemeColor = ThemeColor.Accent1;
            shadingTable0.BackgroundPatternThemeColor = ThemeColor.Accent2;
            shadingTable1.ForegroundPatternThemeColor = ThemeColor.Dark1;
            shadingTable1.BackgroundPatternThemeColor = ThemeColor.Dark2;
            shadingTable1.ForegroundTintAndShade = 0.5;
            shadingTable1.BackgroundTintAndShade = -0.5;

            doc = TestUtil.SaveOpen(doc, @"Model\Theme\Test24456_TableStyle.docx", new OoxmlSaveOptions(), false);
            shadingTable0 = ((TableStyle)doc.FirstSection.Body.Tables[0].Style).Shading;
            shadingTable1 = ((TableStyle)doc.FirstSection.Body.Tables[1].Style).Shading;

            CheckShadingColors(shadingTable0, ThemeColor.Accent1, ThemeColor.Accent2,
                0, 0, Color.Empty, Color.Empty);
            CheckShadingColors(shadingTable1, ThemeColor.Dark1, ThemeColor.Dark2,
                0.5, -0.5, Color.Empty, Color.Empty);
        }

        /// <summary>
        /// Relates to WORDSNET-24456
        /// Tests the public API of ForegroundThemeColor, BackgroundThemeColor, ForegroundTintAndShade
        /// and BackgroundTintAndShade properties in Shading.
        /// Table cell shading case.
        /// </summary>
        [Test]
        public void Test24456_Cell()
        {
            Document doc = TestUtil.Open(@"Model\Theme\Test24456_Cell.docx");

            Shading shadingCell = doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.CellFormat.Shading;
            CheckShadingColors(shadingCell, ThemeColor.None, ThemeColor.None,
                0, 0, Color.FromArgb(0xFF, 0xFF, 0x00, 0x00),
                Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF));

            shadingCell.ForegroundPatternThemeColor = ThemeColor.Dark1;
            shadingCell.BackgroundPatternThemeColor = ThemeColor.Dark2;
            shadingCell.ForegroundTintAndShade = 0.5;
            shadingCell.BackgroundTintAndShade = -0.5;

            doc = TestUtil.SaveOpen(doc, @"Model\Theme\Test24456_Cell.docx", new OoxmlSaveOptions(), false);
            shadingCell = doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.CellFormat.Shading;
            // Writer restores the border color by the appropriate theme color.
            CheckShadingColors(shadingCell, ThemeColor.Dark1, ThemeColor.Dark2, 0.5,
                -0.5, Color.FromArgb(0xFF, 0x80, 0x80, 0x80),
                Color.FromArgb(0xFF, 0x21, 0x29, 0x34));
        }

        /// <summary>
        /// Relates to WORDSNET-24456
        /// Tests that exception is thrown when applying value to ForegroundTintAndShade or BackgroundTintAndShade out of [-1, 1] range.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestShadingTintAndShadeOutOfRange(bool testForeground)
        {
            Document doc = new Document();
            Shading shading = doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Shading;
            shading.BackgroundPatternThemeColor = ThemeColor.Accent1;
            shading.ForegroundPatternThemeColor = ThemeColor.Accent1;
            // Apply ForegroundTintAndShade or BackgroundTintAndShade value out of the allowed range [-1, 1].
            if (testForeground)
                shading.ForegroundTintAndShade = 2;
            else
                shading.BackgroundTintAndShade = 2;
        }

        /// <summary>
        /// Relates to WORDSNET-24456
        /// Tests that exception is thrown when applying value to ForegroundTintAndShade or BackgroundTintAndShade for non-theme color.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestShadingTintAndShadeNonThemeColor(bool testForeground)
        {
            Document doc = new Document();
            Shading shading = doc.FirstSection.Body.FirstParagraph.ParagraphFormat.Shading;
            // Apply TintAndShade to the non-theme color.
            if (testForeground)
                shading.ForegroundTintAndShade = 0.5;
            else
                shading.BackgroundTintAndShade = 0.5;
        }


        /// <summary>
        /// Checks border colors.
        /// </summary>
        private static void CheckBorderColors(Border border, ThemeColor expectedThemeColor, Color expectedColor)
        {
            Assert.That(border.ThemeColor, Is.EqualTo(expectedThemeColor));
            Assert.That(border.Color, Is.EqualTo(expectedColor));
        }

        /// <summary>
        /// Checks shading colors.
        /// </summary>
        private static void CheckShadingColors(Shading shading, ThemeColor expectedForegroundThemeColor, ThemeColor expectedBackgroundThemeColor,
            double expectedForegroundTintAndShade, double expectedBackgroundTintAndShade,
            Color expectedForegroundPatternColor, Color expectedBackgroundPatternColor)
        {
            Assert.That(shading.ForegroundPatternThemeColor, Is.EqualTo(expectedForegroundThemeColor));
            Assert.That(shading.BackgroundPatternThemeColor, Is.EqualTo(expectedBackgroundThemeColor));
            Assert.That(shading.ForegroundTintAndShade, Is.EqualTo(expectedForegroundTintAndShade).Within(0.01));
            Assert.That(shading.BackgroundTintAndShade, Is.EqualTo(expectedBackgroundTintAndShade).Within(0.01));
            Assert.That(shading.ForegroundPatternColor, Is.EqualTo(expectedForegroundPatternColor));
            Assert.That(shading.BackgroundPatternColor, Is.EqualTo(expectedBackgroundPatternColor));
        }

        /// <summary>
        /// Sets all theme color to certain values.
        /// </summary>
        private static void SetAllColors(Theme theme)
        {
            ThemeColors colors = theme.Colors;

            colors.Accent1 = Color.Red;
            colors.Accent2 = Color.Green;
            colors.Accent3 = Color.Blue;
            colors.Accent4 = Color.Yellow;
            colors.Accent5 = Color.Gold;
            colors.Accent6 = Color.DeepPink;
            colors.Dark1 = Color.DarkOrchid;
            colors.Dark2 = Color.DarkOrange;
            colors.Light1 = Color.LightCoral;
            colors.Light2 = Color.LightSkyBlue;
            colors.FollowedHyperlink = Color.DarkRed;
            colors.Hyperlink = Color.DarkMagenta;
        }

        /// <summary>
        /// Checks that all theme colors equal to certain values.
        /// </summary>
        /// <param name="theme"></param>
        private static void CheckAllColors(Theme theme)
        {
            ThemeColors colors = theme.Colors;

            Assert.That(colors.Accent1.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            Assert.That(colors.Accent2.ToArgb(), Is.EqualTo(Color.Green.ToArgb()));
            Assert.That(colors.Accent3.ToArgb(), Is.EqualTo(Color.Blue.ToArgb()));
            Assert.That(colors.Accent4.ToArgb(), Is.EqualTo(Color.Yellow.ToArgb()));
            Assert.That(colors.Accent5.ToArgb(), Is.EqualTo(Color.Gold.ToArgb()));
            Assert.That(colors.Accent6.ToArgb(), Is.EqualTo(Color.DeepPink.ToArgb()));
            Assert.That(colors.Dark1.ToArgb(), Is.EqualTo(Color.DarkOrchid.ToArgb()));
            Assert.That(colors.Dark2.ToArgb(), Is.EqualTo(Color.DarkOrange.ToArgb()));
            Assert.That(colors.Light1.ToArgb(), Is.EqualTo(Color.LightCoral.ToArgb()));
            Assert.That(colors.Light2.ToArgb(), Is.EqualTo(Color.LightSkyBlue.ToArgb()));
            Assert.That(colors.FollowedHyperlink.ToArgb(), Is.EqualTo(Color.DarkRed.ToArgb()));
            Assert.That(colors.Hyperlink.ToArgb(), Is.EqualTo(Color.DarkMagenta.ToArgb()));
        }

        /// <summary>
        /// Creates a new document and applies theme font names to the Normal style.
        /// </summary>
        private static Document CreateNewDocumentWithThemes()
        {
            Document doc = new Document();
            doc.Theme.MinorFonts.Latin = "Algerian";
            doc.Theme.MinorFonts.EastAsian = "Aharoni";
            doc.Theme.MinorFonts.ComplexScript = "Andalus";

            // Normal style is not themed originally. Check it.
            Font font = doc.Styles["Normal"].Font;
            CheckThemeFontNames(font, ThemeFont.None, ThemeFont.None, ThemeFont.None, ThemeFont.None);
            CheckResolvedFontNames(font, "Times New Roman", "Times New Roman", "Times New Roman", "Times New Roman");

            // Apply theme font names to the Normal style.
            SetThemeFontNames(font, ThemeFont.Minor, ThemeFont.Minor, ThemeFont.Minor, ThemeFont.Minor);

            // Round-trip the document.
            using (MemoryStream stream = new MemoryStream())
            {
                doc.Save(stream, SaveFormat.Docx);
                doc = new Document(stream);
            }

            font = doc.Styles["Normal"].Font;

            CheckThemeFontNames(font, ThemeFont.Minor);
            CheckResolvedFontNames(font, "Algerian", "Aharoni", "Algerian", "Andalus");

            return doc;
        }

        /// <summary>
        /// Checks theme's font names.
        /// </summary>
        private static void CheckThemeFontNames(Font font, ThemeFont expectedFont)
        {
            Assert.That(font.ThemeFontAscii, Is.EqualTo(expectedFont));
            Assert.That(font.ThemeFontFarEast, Is.EqualTo(expectedFont));
            Assert.That(font.ThemeFontOther, Is.EqualTo(expectedFont));
            Assert.That(font.ThemeFontBi, Is.EqualTo(expectedFont));
        }

        /// <summary>
        /// Checks theme's font names.
        /// </summary>
        private static void CheckThemeFontNames(
            Font font,
            ThemeFont expectedAscii, ThemeFont expectedFarEast, ThemeFont expectedOther, ThemeFont expectedBi)
        {
            Assert.That(font.ThemeFontAscii, Is.EqualTo(expectedAscii));
            Assert.That(font.ThemeFontFarEast, Is.EqualTo(expectedFarEast));
            Assert.That(font.ThemeFontOther, Is.EqualTo(expectedOther));
            Assert.That(font.ThemeFontBi, Is.EqualTo(expectedBi));
        }

        /// <summary>
        /// Checks theme's font names.
        /// </summary>
        private static void SetThemeFontNames(
            Font font,
            ThemeFont themeAscii, ThemeFont themeFarEast, ThemeFont themeOther, ThemeFont themeBi)
        {
            font.ThemeFontAscii = themeAscii;
            font.ThemeFontFarEast = themeFarEast;
            font.ThemeFontOther = themeOther;
            font.ThemeFontBi = themeBi;
        }

        /// <summary>
        /// Checks resolved font names.
        /// </summary>
        private static void CheckResolvedFontNames(
            Font font,
            string expectedAscii, string expectedFarEast, string expectedOther, string expectedBi)
        {
            Assert.That(font.NameAscii, Is.EqualTo(expectedAscii));
            Assert.That(font.NameFarEast, Is.EqualTo(expectedFarEast));
            Assert.That(font.NameOther, Is.EqualTo(expectedOther));
            Assert.That(font.NameBi, Is.EqualTo(expectedBi));
        }
    }
}
