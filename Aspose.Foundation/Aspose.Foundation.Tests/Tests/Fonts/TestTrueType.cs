// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/08/2013 by Roman Korchagin

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Aspose.Collections.Generic;
using Aspose.Fonts;
using Aspose.Fonts.EmbeddedOpenType;
using Aspose.Fonts.Sfnt;
using Aspose.Fonts.TrueType;
using Aspose.IO;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.Tests.Fonts
{
    /// <summary>
    /// Test reading of TrueType fonts.
    /// </summary>
    [TestFixture]
    public class TestTrueType
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestFxUtil.SetUpTests();
            Directory.CreateDirectory(TestFxUtil.GetInTestOutPath("TrueType"));
        }

#if !NETSTANDARD // SystemPal.GetWindowsFontsFolder() is not available when run on device in Xamarin.Android. Use Platform to exclude this test.
        /// <summary>
        /// Check that we can read font okay and font properties are set as expected.
        /// </summary>
        [Test]
        public void TestFontProperties()
        {
            string fontDirectoryPath = TestEnvironment.GetWinFontsFolder();
            string arialPath = Path.Combine(fontDirectoryPath, "arialbd.ttf");
            string ttcPath = Path.Combine(fontDirectoryPath, "cambria.ttc");

            TTFont ttfFont = TTFontBuilder.ReadOpenType(arialPath);
            Assert.That(((FileFontData)ttfFont.Data).FileName.ToLower(), Is.EqualTo(arialPath.ToLower()));
            Assert.That(ttfFont.FamilyName, Is.EqualTo("Arial"));
            Assert.That(ttfFont.SubFamilyName, Is.EqualTo("Bold"));            // "Regular" for normal font
            Assert.That(ttfFont.FullFontName, Is.EqualTo("Arial Bold"));        // "Arial" for normal font.
            Assert.That(ttfFont.PostscriptName, Is.EqualTo("Arial-BoldMT"));    // "ArialMT" for normal font.
            Assert.That(ttfFont.Style, Is.EqualTo(FontStyle.Bold));
            Assert.That(ttfFont.StrikeoutSize, Is.EqualTo(102));
            Assert.That(ttfFont.StrikeoutPosition, Is.EqualTo(530));
            Assert.That(ttfFont.SuperscriptSize, Is.EqualTo(1331));
            Assert.That(ttfFont.SuperscriptOffset, Is.EqualTo(977));
            Assert.That(ttfFont.SubscriptSize, Is.EqualTo(1331));
            Assert.That(ttfFont.SubscriptOffset, Is.EqualTo(283));
            Assert.That(ttfFont.EmHeight, Is.EqualTo(2048));
            Assert.That(ttfFont.Ascent, Is.EqualTo(1854));
            Assert.That(ttfFont.Descent, Is.EqualTo(434));
            Assert.That(ttfFont.LineSpacing, Is.EqualTo(2355));

            TTFont ttcFont = TTFontBuilder.ReadTtc(ttcPath, "Cambria");
            Assert.That(((FileFontData)ttcFont.Data).FileName.ToLower(), Is.EqualTo(ttcPath.ToLower()));
            Assert.That(ttcFont.FamilyName, Is.EqualTo("Cambria"));
            Assert.That(ttcFont.SubFamilyName, Is.EqualTo("Regular"));
            Assert.That(ttcFont.FullFontName, Is.EqualTo("Cambria"));
            Assert.That(ttcFont.PostscriptName, Is.EqualTo("Cambria"));
            Assert.That(ttcFont.Style, Is.EqualTo(FontStyle.Regular));
            Assert.That(ttcFont.StrikeoutSize, Is.EqualTo(102));
            Assert.That(ttcFont.StrikeoutPosition, Is.EqualTo(510));
            Assert.That(ttcFont.SuperscriptSize, Is.EqualTo(1331));
            Assert.That(ttcFont.SuperscriptOffset, Is.EqualTo(503));
            Assert.That(ttcFont.SubscriptSize, Is.EqualTo(1331));
            Assert.That(ttcFont.SubscriptOffset, Is.EqualTo(156));
            Assert.That(ttcFont.EmHeight, Is.EqualTo(2048));
            Assert.That(ttcFont.Ascent, Is.EqualTo(1946));
            Assert.That(ttcFont.Descent, Is.EqualTo(455));
            Assert.That(ttcFont.LineSpacing, Is.EqualTo(2401));

#if !JAVA // This is just an additional check to make sure we have the same values as GDI+.
            TTFont[] fonts = new TTFont[2];
            fonts[0] = ttfFont;
            fonts[1] = ttcFont;
            string[] family = new string[] { "Arial", "Cambria" };
            for (int i = 0; i < fonts.Length; i++)
            {
                FontFamily gdiFamily = new FontFamily(family[i]);
                int emHeight = gdiFamily.GetEmHeight(FontStyle.Regular);
                int gdiAscent = gdiFamily.GetCellAscent(FontStyle.Regular);
                int gdiDescent = gdiFamily.GetCellDescent(FontStyle.Regular);
                int gdiLineSpacing = gdiFamily.GetLineSpacing(FontStyle.Regular);

                Assert.That(fonts[i].EmHeight, Is.EqualTo(emHeight));
                Assert.That(fonts[i].Ascent, Is.EqualTo(gdiAscent));
                Assert.That(fonts[i].Descent, Is.EqualTo(gdiDescent));
                Assert.That(fonts[i].LineSpacing, Is.EqualTo(gdiLineSpacing));
            }
#endif
        }
#endif

        /// <summary>
        /// Check we can read a font okay and the information about glyphs is as expected.
        /// </summary>
        [Test]
        public void TestGlyphs()
        {
            TTFont f = TestFontProvider.StaticInstance.FetchTTFont("Arial", FontStyle.Bold);

            // This number can change from time to time with Windows updates.
            // In this font there are X glyphs, but only Y characters, this is okay since
            // some glyphs are composite and consist of several glyphs that are not characters independently.
            Assert.That(f.Glyphs.Glyphs.Count, Is.EqualTo(4651));

            // This glyph is mapped using one way in cmap format 4.
            TTGlyph g = f.Glyphs.FetchGlyphByCharCode(0x20);
            Assert.That(g.GlyphIndex, Is.EqualTo(3));
            Assert.That(g.AdvanceWidth, Is.EqualTo(569));

            // This glyph (non breaking space) is mapped another way in cmap format 4.
            g = f.Glyphs.FetchGlyphByCharCode(0xA0);
            Assert.That(g.GlyphIndex, Is.EqualTo(3));
            Assert.That(g.AdvanceWidth, Is.EqualTo(569));

            // Just check some more glyphs.
            g = f.Glyphs.FetchGlyphByCharCode(0xA1);
            Assert.That(g.GlyphIndex, Is.EqualTo(163));
            Assert.That(g.AdvanceWidth, Is.EqualTo(682));

            // This is mapped to the "missing glyph".
            g = f.Glyphs.FetchGlyphByCharCode(0xD078);
            Assert.That(g.GlyphIndex, Is.EqualTo(0));
        }


        /// <summary>
        /// WORDSNET-8260, 9334, 9423 "EndOfStreamException" exception occurs during converting document to PDF.
        /// The problem occurred because font is True Type Version 0 (1) and Font Metrics were read improperly.
        /// Changed code to read metrics properly from old versions of True Type.
        /// </summary>
        [Test]
        public void TestReadMetricsOldVersion()
        {
            // Read True Type Version 0
            TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\Expo1.TTF"));
            // Read True Type Version 1
            TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\USPS_BAR.TTF"));
        }

        /// <summary>
        /// WORDSNET-9205 "IndexOutOfRangeException" exception occurs during converting document to PDF.
        /// The problem occurred because the latest character (65535) contains invalid mapping.
        /// Made code resilient.
        /// </summary>
        [Test]
        public void TestDefect9205()
        {
            TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\NEWTOW__.TTF"));
        }

        /// <summary>
        /// WORDSNET-15433 “System.ArgumentException: Item has already been added. Key in dictionary: '”
        /// occurs during rendering to PDF and XPS.
        /// The problem occurred because CMAP table of the font contained 4 [0;0] segments that leads to an attempt to
        /// add few glyphs with the same key into a hashtable. Check for the key is added.
        /// </summary>
        [Test]
        public void TestDefect15433()
        {
            TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\CHARLOSC.TTF"));
        }

        /// <summary>
        /// WORDSNET-25211 Code39 font is not embedded into PDF.
        /// Code39 font doesn't contains postscript name and exception is thrown upon loading the font.
        /// </summary>
        [Test]
        public void TestDefect25211()
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\code39.ttf"));
            Assert.That(font.PostscriptName, Is.EqualTo("Code 39"));
        }

        /// <summary>
        /// WORDSNET-24711 Distance between text lines is too small during rendering.
        /// Font with incorrect hhea table returns wrong LineSpacing value.
        /// </summary>
        [Test]
        public void TestDefect24711()
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\lte50329.ttf"));

            // These values are obtained from GDI+.
            Assert.That(font.Ascent, Is.EqualTo(935));
            Assert.That(font.Descent, Is.EqualTo(250));
            Assert.That(font.LineSpacing, Is.EqualTo(1185));
            Assert.That(font.EmHeight, Is.EqualTo(1000));
        }

        /// <summary>
        /// There was an error in reading TrueType rev 1.5 font metrics.
        /// </summary>
        [Test]
        public void TestTrueTypeRev15()
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\Ira.ttf"));

            // These values are obtained from GDI+.
            Assert.That(font.Ascent, Is.EqualTo(1000));
            Assert.That(font.Descent, Is.EqualTo(824));
            Assert.That(font.LineSpacing, Is.EqualTo(2024));
            Assert.That(font.EmHeight, Is.EqualTo(1000));
        }

        /// <summary>
        /// Buffet Script font contains glyphs with negative advance widths.
        /// OpenType spec says that advance width should be positive but MS Word allows negative values.
        /// So we should do the same.
        /// </summary>
        [Test]
        public void TestNegativeAdvanceWidth()
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\Buffet Script.otf"));

            Assert.That(font.Glyphs.GetGlyphByCharCode(0xA8).AdvanceWidth, Is.LessThan(0));
            Assert.That(font.Glyphs.GetGlyphByCharCode(0xB4).AdvanceWidth, Is.LessThan(0));
        }

        /// <summary>
        /// Test that TTFont supports Unicode Supplementary Planes characters.
        /// </summary>
        [Test]
        public void TestUnicodeSupplementaryPlanes()
        {
            TTFont font = TestFontProvider.StaticInstance.FetchTTFont("MS UI Gothic", FontStyle.Regular);
            Assert.That(font.Glyphs.GetGlyphByCharCode(0x25c4b), IsNot.EqualTo(null));
            Assert.That(font.Glyphs.GetGlyphByCharCode(0x27da0), IsNot.EqualTo(null));

            font = TestFontProvider.StaticInstance.FetchTTFont("PMingLiU-ExtB", FontStyle.Regular);
            Assert.That(font.Glyphs.GetGlyphByCharCode(0x25aaf), IsNot.EqualTo(null));

            font = TestFontProvider.StaticInstance.FetchTTFont("Cambria Math", FontStyle.Regular);
            Assert.That(font.Glyphs.GetGlyphByCharCode(0x1d465), IsNot.EqualTo(null));
            Assert.That(font.Glyphs.GetGlyphByCharCode(0x1d44f), IsNot.EqualTo(null));
            Assert.That(font.Glyphs.GetGlyphByCharCode(0x1d44e), IsNot.EqualTo(null));
            Assert.That(font.Glyphs.GetGlyphByCharCode(0x1d450), IsNot.EqualTo(null));
        }

        /// <summary>
        /// Test that Macintosh TrueType fonts are handled correctly.
        /// </summary>
        [Test]
        public void TestMacintoshTrueType()
        {
            TTFont font = TTFontBuilder.ReadTtc(TestFxUtil.BuildTestFileName(@"TrueType\MacTrueType\Cochin.ttc"), "Cochin");
            Assert.That(font.FamilyName, Is.EqualTo("Cochin"));
            Assert.That(font.PostscriptName, Is.EqualTo("Cochin"));

            font = TTFontBuilder.ReadTtc(TestFxUtil.BuildTestFileName(@"TrueType\MacTrueType\Futura.ttc"), "Futura Medium");
            Assert.That(font.FamilyName, Is.EqualTo("Futura"));
            Assert.That(font.PostscriptName, Is.EqualTo("Futura-Medium"));

            font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\MacTrueType\HelveticaNeue.ttf"));
            Assert.That(font.FamilyName, Is.EqualTo("Helvetica Neue"));
            Assert.That(font.PostscriptName, Is.EqualTo("HelveticaNeue"));

            font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\MacTrueType\HelveticaNeueBold.ttf"));
            Assert.That(font.FamilyName, Is.EqualTo("Helvetica Neue"));
            Assert.That(font.PostscriptName, Is.EqualTo("HelveticaNeue-Bold"));
        }

        /// <summary>
        /// Custom font defines negative usWinDescent values.
        /// OpenType spec says that it should be positive but MS Word allows negative values.
        /// </summary>
        [Test]
        public void TestNegativeUsWinDescent()
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\Shapes1.ttf"));

            Assert.That(font.Descent, Is.LessThan(0));
        }

        /// <summary>
        /// Test that bitmap font is not recognized as a valid font.
        /// </summary>
        [Test]
        public void TestBitmapFont()
        {
            TTFontFiler filer = new TTFontFiler();
            List<FontSearchInfo> searchInfos = new List<FontSearchInfo>();
            filer.ExtractFontSearchInfo(searchInfos,
                new FileFontData(TestFxUtil.BuildTestFileName(@"TrueType\NotoColorEmoji.ttf")),
                0);

            Assert.That(searchInfos.Count, Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-22821 CJK metrics adjustments are wrongly used for Barcode font.
        /// </summary>
        [Test]
        public void Test22821()
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\Test22821\IDAutomationC128S.ttf"));

            Assert.That(font.IsCjkMetrics, Is.False);
        }

        /// <summary>
        /// WORDSNET-12365 Font with regular style from fsSelection and usWeightClass=700 is considered bold by MW.
        /// </summary>
        [Test]
        public void Test12365()
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\Test12365\Novin-Bold.ttf"));

            Assert.That(font.IsBold, Is.True);
        }

        /// <summary>
        /// WORDSNET-24139 Customer font has invalid 'vhea' version.
        /// </summary>
        [Test]
        public void Test24139()
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\Test24139\CN-CD128.TTF"));

            Assert.That(font, IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-25217 Customer font has invalid 'vhea' version.
        /// </summary>
        [Test]
        public void Test25217()
        {
            TTFont font = TTFontBuilder.ReadTtc(TestFxUtil.BuildTestFileName(@"TrueType\Test25217\kai08mz.ttc"), "AR StdKaiZuinn Md");

            Assert.That(font, IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-25754 Customer font has malformed 'vmtx' table.
        /// </summary>
        [Test]
        public void Test25754()
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\Test25754\BMWTypeGlobalPro-Regular.ttf"));

            Assert.That(font, IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-26737 Exception parsing 'post' table.
        /// </summary>
        [Test]
        public void Test26737()
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\Test26737\AGLTSYM1.ttf"));

            Assert.That(font, IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-27389 Chinese fonts with Big5 encoding in cmap and name tables.
        /// </summary>
        [TestCase("Test27389.ttf", "文鼎中粗隸")]
        [TestCase("Test27393.ttf", "金梅海報書法字形")]
        public void Test27389(string fontFile, string fontName)
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\Test27389\" + fontFile));

            Assert.That(font.FamilyName, Is.EqualTo(fontName));
        }

        /// <summary>
        /// WORDSNET-28017 Error in parsing COLR table.
        /// </summary>
        [Test]
        public void Test28017()
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\Test28017\SEGUIEMJv151.TTF"));

            Assert.That(font, IsNot.Null());
            Assert.That(font.IsColored, Is.True);
        }

        /// <summary>
        /// WORDSNET-28625 Error in parsing corrupted cmap table.
        /// </summary>
        [Test]
        public void Test28625()
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\Test28625\PHTK53SF.TTF"));

            Assert.That(font.FamilyName, Is.EqualTo("書法中楷加框（破音三）"));
        }
    }
}
