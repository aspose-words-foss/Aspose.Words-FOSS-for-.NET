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
        /// WORDSNET-22821 CJK metrics adjustments are wrongly used for Barcode font.
        /// </summary>
        [Test]
        public void Test22821()
        {
            TTFont font = TTFontBuilder.ReadOpenType(TestFxUtil.BuildTestFileName(@"TrueType\Test22821\IDAutomationC128S.ttf"));

            Assert.That(font.IsCjkMetrics, Is.False);
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
