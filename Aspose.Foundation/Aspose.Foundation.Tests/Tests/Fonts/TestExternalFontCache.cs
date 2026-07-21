// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2013 by Konstantin Kornilov

using System;
using System.Drawing;
using System.IO;
using Aspose.Fonts;
using Aspose.Fonts.TrueType;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.Tests.Fonts
{
    /// <summary>
    /// Test <see cref="ExternalFontCache"/> class.
    /// </summary>
    [TestFixture]
    public class TestExternalFontCache
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestFxUtil.SetUpTests();
            Directory.CreateDirectory(TestFxUtil.GetInTestOutPath("TrueType"));
        }

        /// <summary>
        /// Check that family properly finds and reads all fonts of the family.
        /// </summary>
        [Test]
        public void TestReadFontFamily()
        {
            // Check the Arial family.
            VerifyFontFamilyLoading("Arial", "Arial", "Arial Bold", "Arial Italic", "Arial Bold Italic");

            // Check the Arial Black family.
            VerifyFontFamilyLoading("Arial Black", "Arial Black", "Arial Black", "Arial Black", "Arial Black");

            // Check the MS UI Gothic family.
            VerifyFontFamilyLoading("MS UI Gothic", "MS UI Gothic", "MS UI Gothic", "MS UI Gothic", "MS UI Gothic");

            // Check the MS PGothic family.
            VerifyFontFamilyLoading("MS PGothic", "MS PGothic", "MS PGothic", "MS PGothic", "MS PGothic");
        }

        private static void VerifyFontFamilyLoading(
            string familyName,
            string regularName,
            string boldName,
            string italicName,
            string boldItalicName)
        {
            Assert.That(TestFontProvider.StaticCache.GetFont(familyName, FontStyle.Regular).FullFontName, Is.EqualTo(regularName));
            Assert.That(TestFontProvider.StaticCache.GetFont(familyName, FontStyle.Bold).FullFontName, Is.EqualTo(boldName));
            Assert.That(TestFontProvider.StaticCache.GetFont(familyName, FontStyle.Italic).FullFontName, Is.EqualTo(italicName));
            Assert.That(TestFontProvider.StaticCache.GetFont(familyName, FontStyle.Bold | FontStyle.Italic).FullFontName, Is.EqualTo(boldItalicName));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        [JavaAttributes.JavaDelete]
        public void TestFontsSourcesNull()
        {
            new ExternalFontCache((IFontSource[])null); // Casting for C++.
        }

        [Test]
        public void TestFontDirectoryNotExist()
        {
            FontSourceBaseCore[] sources = {
                new FolderFontSourceCore(@"C:\ABC\Fonts", false),
                new FolderFontSourceCore(TestFxUtil.BuildTestFileName(@"TrueType"), false)
            };
            ExternalFontCache testInstance = new ExternalFontCache((IFontSource[])sources); // Casting for C++.
            TTFont font = testInstance.GetFont("Water on the Oil", FontStyle.Regular);
            Assert.That(font.FullFontName, Is.EqualTo("Water on the Oil"));
        }

        [Test]
        public void TestFontDirectorySet()
        {
            IFontSource[] sources = { new SystemFontSourceCore() };
            ExternalFontCache testInstance = new ExternalFontCache(sources);

            Assert.That(testInstance.GetFont("Water on the Oil", FontStyle.Regular), Is.EqualTo(null));

            FontSourceBaseCore[] sources1 =
            {
                new FolderFontSourceCore(TestFxUtil.BuildTestFileName(@"TrueType"), false)
            };
            testInstance = new ExternalFontCache((IFontSource[])sources1); // Casting for C++

            Assert.That(testInstance.GetFont("Water on the Oil", FontStyle.Regular).FullFontName, Is.EqualTo("Water on the Oil"));
        }

        /// <summary>
        /// Check that loading font by family name and style works (as opposed to loading by file name).
        /// Make sure it works both when an exact style match is found and when not found.
        /// </summary>
        [Test]
        public void TestLoadFontByName()
        {
            // This is an exact match since Arial Italic exists in Windows.
            TTFont font = TestFontProvider.StaticCache.GetFont("Arial", FontStyle.Italic);
            Assert.That(font.PostscriptName, Is.EqualTo("Arial-ItalicMT"));
            Assert.That(font.Style, Is.EqualTo(FontStyle.Italic));

            // Requesting Bold when it does not exist will return Regular.
            font = TestFontProvider.StaticCache.GetFont("Arial Black", FontStyle.Bold);
            Assert.That(font.PostscriptName, Is.EqualTo("Arial-Black"));

            // Only regular font exists and it is returned.
            font = TestFontProvider.StaticCache.GetFont("Impact", FontStyle.Bold | FontStyle.Italic);
            Assert.That(font.PostscriptName, Is.EqualTo("Impact"));

            // Test the same scenarios for a font from TTC.

            // This is an exact match since MS UI Gothic exists in Windows.
            font = TestFontProvider.StaticCache.GetFont("MS UI Gothic", FontStyle.Regular);
            Assert.That(font.PostscriptName, Is.EqualTo("MS-UIGothic"));
            Assert.That(font.Style, Is.EqualTo(FontStyle.Regular));

            // This is a fuzzy match. Requesting Bold when it does not exist will return Regular.
            font = TestFontProvider.StaticCache.GetFont("MS UI Gothic", FontStyle.Bold);
            Assert.That(font.PostscriptName, Is.EqualTo("MS-UIGothic"));
        }

        /// <summary>
        /// Test that we tries to trim the font family name when fetching.
        /// </summary>
        [Test]
        public void TestFontFamilyNameTrimming()
        {
            TTFont font = TestFontProvider.StaticCache.GetFont(" Arial", FontStyle.Regular);
            Assert.That(font.FamilyName, Is.EqualTo("Arial"));

            font = TestFontProvider.StaticCache.GetFont("Arial ", FontStyle.Regular);
            Assert.That(font.FamilyName, Is.EqualTo("Arial"));

            font = TestFontProvider.StaticCache.GetFont(" Arial ", FontStyle.Regular);
            Assert.That(font.FamilyName, Is.EqualTo("Arial"));
        }

        /// <summary>
        /// WORDSNET 100040 - Font names for different platforms was handled incorrectly.
        /// Test case:
        /// HelveticaLTStd-Roman.otf font has "Helvetica LT Std" Windows and Mac family name.
        /// HelveticaLTStd-UltraComp.otf font has "Helvetica LT Std UltCompressed" Windows family name and "Helvetica LT Std" Mac family name.
        /// Check that HelveticaLTStd-Roman.otf is found by "Helvetica LT Std" family name.
        /// </summary>
        [Test]
        public void TestJira10040A()
        {
            FontSourceBaseCore[] sources =
            {
                new FileFontSourceCore(
                    TestFxUtil.BuildTestFileName(@"TrueType\TestJira10040\HelveticaLTStd-Roman.otf")),
                new FileFontSourceCore(
                    TestFxUtil.BuildTestFileName(@"TrueType\TestJira10040\HelveticaLTStd-UltraComp.otf"))
            };
            ExternalFontCache testInstance = new ExternalFontCache((IFontSource[])sources); // Casting for C++

            TTFont font = testInstance.GetFont("Helvetica LT Std", FontStyle.Regular);

            Assert.That(font.FullFontName, Is.EqualTo("HelveticaLTStd-Roman"));
        }

        /// <summary>
        /// WORDSNET 100040 - Font names for different platforms was handled incorrectly.
        /// Test case:
        /// HelveticaLTStd-UltraComp.otf font has "Helvetica LT Std UltCompressed" Windows family name and "Helvetica LT Std" Mac family name.
        /// Check that font is found by Windows family name and not by Mac family name.
        /// </summary>
        [Test]
        public void TestJira10040B()
        {
            FontSourceBaseCore[] sources =
            {
                new FileFontSourceCore(
                    TestFxUtil.BuildTestFileName(@"TrueType\TestJira10040\HelveticaLTStd-UltraComp.otf"))
            };
            ExternalFontCache testInstance = new ExternalFontCache((IFontSource[])sources); // Casting for C++

            TTFont fontByWindowsFamilyName = testInstance.GetFont("Helvetica LT Std UltCompressed",
                                                                  FontStyle.Regular);
            TTFont fontByMacFamilyName = testInstance.GetFont("Helvetica LT Std", FontStyle.Regular);

            Assert.That(fontByWindowsFamilyName.FullFontName, Is.EqualTo("HelveticaLTStd-UltraComp"));
            Assert.That(fontByMacFamilyName, Is.Null);
        }

        [Test]
        public void TestFontNameCaseInsensitive()
        {
            // RK GDI+ font name seems to be case insensitive.
            TTFont font = TestFontProvider.StaticCache.GetFont("ARIAL", FontStyle.Regular);
            Assert.That(font.FamilyName, Is.EqualTo("Arial"));
        }

        [Test]
        public void TestFontStyleNotAvailable()
        {
            FontSourceBaseCore[] sources = {
                new FileFontSourceCore(TestFxUtil.BuildTestFileName(@"Rendering\MTCORSVA.TTF"))
            };
            ExternalFontCache testInstance = new ExternalFontCache((IFontSource[])sources); // Casting for C++

            // There is only Italic of this font available.
            TTFont font = testInstance.GetFont("Monotype Corsiva", FontStyle.Regular);
            Assert.That(font.FamilyName, Is.EqualTo("Monotype Corsiva"));
            Assert.That(font.Style, Is.EqualTo(FontStyle.Italic));
        }

        /// <summary>
        /// Test that fonts with '@' prefix is trimmed when fetching fonts.
        /// </summary>
        [Test]
        public void TestAtSignFontName()
        {
            TTFont font = TestFontProvider.StaticCache.GetFont("@Arial Unicode MS", FontStyle.Regular);
            Assert.That(font.FamilyName, Is.EqualTo("Arial Unicode MS"));
        }

        /// <summary>
        /// Test how font with invalid font metrics is handled by ExternalFontCache.
        /// </summary>
        [Test]
        public void TestInvalidFontMetrics()
        {
            FontSourceBaseCore[] sources = {
                new FileFontSourceCore(TestFxUtil.BuildTestFileName(@"TrueType\InvalidFontMetrics.ttf")),
            };
            ExternalFontCache testInstance = new ExternalFontCache((IFontSource[])sources); // Casting for C++

            Assert.That(testInstance.GetFont("CCLSFK+TT16Dt00", FontStyle.Regular), Is.Null);
        }

        /// <summary>
        /// WORDSNET-12916 Chinese font with PRC encoding.
        /// </summary>
        [Test]
        public void TestJira12916()
        {
            FontSourceBaseCore[] sources = {
                new FileFontSourceCore(TestFxUtil.BuildTestFileName(@"TrueType\TestJira12916\GBInnMing.TTF")),
            };
            ExternalFontCache testInstance = new ExternalFontCache((IFontSource[])sources); // Casting for C++

            // Check that name records are parsed correctly.
            TTFont font = testInstance.GetFont("恅隋苤梓冼潠", FontStyle.Regular);
            Assert.That(font.FamilyName, Is.EqualTo("恅隋苤梓冼潠"));
            Assert.That(font.FullFontName, Is.EqualTo("恅隋苤梓冼潠"));
            Assert.That(font.PostscriptName, Is.EqualTo("GBInnMing-Bold"));
            Assert.That(font.SubFamilyName, Is.EqualTo("Regular"));

            // Check that font is found by second name in PRC encoding.
            font = testInstance.GetFont("文鼎小标宋简", FontStyle.Regular);
            Assert.That(font.FullFontName, Is.EqualTo("恅隋苤梓冼潠"));
        }

        /// <summary>
        /// WORDSNET-13270 Chinese font with PRC encoding.
        /// </summary>
        [Test]
        public void TestJira13270()
        {
            FontSourceBaseCore[] sources = {
                new FileFontSourceCore(TestFxUtil.BuildTestFileName(@"TrueType\TestJira12916\GBInnMing-Bold.TTF")),
            };
            ExternalFontCache testInstance = new ExternalFontCache((IFontSource[])sources); // Casting for C++

            // Check that name records are parsed correctly.
            TTFont font = testInstance.GetFont("长城小标宋体", FontStyle.Regular);
            Assert.That(font.FamilyName, Is.EqualTo("长城小标宋体"));
            Assert.That(font.FullFontName, Is.EqualTo("长城小标宋体"));
            Assert.That(font.PostscriptName, Is.EqualTo("GBInnMing-Bold"));
            Assert.That(font.SubFamilyName, Is.EqualTo("Regular"));
        }

        /// <summary>
        /// WORDSNET-20026 Chinese font with PRC encoding.
        /// </summary>
        [Test]
        public void Test20026()
        {
            FontSourceBaseCore[] sources = {
                new FileFontSourceCore(TestFxUtil.BuildTestFileName(@"TrueType\Test20026\font.TTF")),
            };
            ExternalFontCache testInstance = new ExternalFontCache((IFontSource[])sources); // Casting for C++

            // Check that name records are parsed correctly.
            TTFont font = testInstance.GetFont("微软简标宋", FontStyle.Regular);
            Assert.That(font.FamilyName, Is.EqualTo("CTBiaoSongSJ"));
            Assert.That(font.FullFontName, Is.EqualTo("CTBiaoSongSJ"));
            Assert.That(font.PostscriptName, Is.EqualTo("CTBiaoSongSJ"));
            Assert.That(font.SubFamilyName, Is.EqualTo("Regular"));
        }

        /// <summary>
        /// WORDSNET-24634 Inaccurate style selection when Regular is not available.
        /// </summary>
        [Test]
        public void Test24634()
        {
            FontSourceBaseCore[] sources = {
                new FolderFontSourceCore(TestFxUtil.BuildTestFileName(@"TrueType\Test24634"), false),
            };
            ExternalFontCache testInstance = new ExternalFontCache((IFontSource[])sources); // Casting for C++

            TTFont font = testInstance.GetFont("Cytiva Aktiv", FontStyle.Regular);
            Assert.That(font.FamilyName, Is.EqualTo("Cytiva Aktiv"));
            Assert.That(font.FullFontName, Is.EqualTo("Cytiva Aktiv Italic"));
            Assert.That(font.Style, Is.EqualTo(FontStyle.Italic));
        }

        /// <summary>
        /// WORDSNET-25845 Italic style is incorrectly resolved from family with only Bold and BoldItalic styles.
        /// </summary>
        [Test]
        public void Test25845()
        {
            FontSourceBaseCore[] sources = {
                new FolderFontSourceCore(TestFxUtil.BuildTestFileName(@"TrueType\Test25845"), false),
            };
            ExternalFontCache testInstance = new ExternalFontCache((IFontSource[])sources); // Casting for C++

            TTFont regular = testInstance.GetFont("Nunito ExtraBold", FontStyle.Regular);
            TTFont bold = testInstance.GetFont("Nunito ExtraBold", FontStyle.Bold);
            TTFont italic = testInstance.GetFont("Nunito ExtraBold", FontStyle.Italic);
            TTFont boldItalic = testInstance.GetFont("Nunito ExtraBold", FontStyle.Bold | FontStyle.Italic);

            Assert.That(regular.FullFontName, Is.EqualTo("Nunito ExtraBold"));
            Assert.That(bold.FullFontName, Is.EqualTo("Nunito ExtraBold"));
            Assert.That(italic.FullFontName, Is.EqualTo("Nunito ExtraBold Italic"));
            Assert.That(boldItalic.FullFontName, Is.EqualTo("Nunito ExtraBold Italic"));
        }

        /// <summary>
        /// WORDSNET-28879 MW select alphabetically first file among fully suited fonts.
        /// </summary>
        [Test]
        public void Test28879()
        {
            FontSourceBaseCore[] sources = {
                new FolderFontSourceCore(TestFxUtil.BuildTestFileName(@"TrueType\Test28879"), false),
            };
            ExternalFontCache testInstance = new ExternalFontCache((IFontSource[])sources); // Casting for C++

            TTFont regular = testInstance.GetFont("Roboto Lt", FontStyle.Regular);

            Assert.That(regular.FullFontName, Is.EqualTo("Roboto Light"));
        }
    }
}
