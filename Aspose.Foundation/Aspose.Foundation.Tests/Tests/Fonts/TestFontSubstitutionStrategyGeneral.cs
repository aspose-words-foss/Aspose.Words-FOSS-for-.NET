// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2016 by Victor Sotnikov

using System;
using System.Collections.Generic;
using System.Drawing;
using Aspose.Fonts;
using Aspose.Fonts.TrueType;
using Aspose.TestFx;
using NUnit.Framework;

namespace Aspose.Tests.Fonts
{
    /// <summary>
    /// Tests for <see cref="FontSubstitutionStrategyGeneral"/>.
    /// </summary>
    [TestFixture]
    public class TestFontSubstitutionStrategyGeneral
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestFxUtil.SetUpTests();
            LoadTestFonts();
        }

        private static void LoadTestFonts()
        {
            // Load only a subset of system fonts to get consistent result.
            FontSourceBaseCore systemFontsSource = new SystemFontSourceCore();
            if (PlatformUtilPal.IsUnixLike())
                systemFontsSource = new FolderFontSourceCore(TestEnvironment.GetWinFontsFolder(), false);
            FontSourceBaseCore[] sources = { systemFontsSource }; // C++ workaround
            IEnumerable<FontSearchInfo> systemFonts = new ExternalFontCache((IFontSource[])sources).FontSearchInfos;
            List<FontSearchInfo> testFonts = new List<FontSearchInfo>();
            foreach (FontSearchInfo searchInfo in systemFonts)
                if (gTestFontNames.Contains(searchInfo.FontFamilyName))
                    testFonts.Add(searchInfo);

            gTestFonts = testFonts;
        }

        [Test, Ignore("PrintFontInformation")]
        public void PrintFontInformation()
        {
            FontSearchInfo info = FindSystemFont("Angsana New", FontStyle.Regular);
            Debug.WriteLine(info.FontFullName);
            Debug.WriteLine(string.Format("panose\t{0}", ArrayUtil.DumpArray(info.Panose.Values)));
            Debug.WriteLine(string.Format("ur1\t{0:X8}", info.UnicodeRanges.Range1));
            Debug.WriteLine(string.Format("ur2\t{0:X8}", info.UnicodeRanges.Range2));
            Debug.WriteLine(string.Format("ur3\t{0:X8}", info.UnicodeRanges.Range3));
            Debug.WriteLine(string.Format("ur4\t{0:X8}", info.UnicodeRanges.Range4));
            Debug.WriteLine(string.Format("cp1\t{0:X8}", info.CodepageRanges.Range1));
            Debug.WriteLine(string.Format("cp2\t{0:X8}", info.CodepageRanges.Range2));
        }

        private static FontSearchInfo FindSystemFont(string familyName, FontStyle style)
        {
            FontSourceBaseCore[] sources = { new SystemFontSourceCore() }; // C++ workaround
            IEnumerable<FontSearchInfo> systemFonts = new ExternalFontCache((IFontSource[])sources).FontSearchInfos;
            foreach (FontSearchInfo searchInfo in systemFonts)
                if (searchInfo.FontFamilyName == familyName && searchInfo.Style == style)
                    return searchInfo;

            return null;
        }

        [Test]
        [TestCase(FontFamilyCore.Auto, "Times New Roman")]
        [TestCase(FontFamilyCore.Roman, "Times New Roman")]
        [TestCase(FontFamilyCore.Swiss, "Arial")]
        [TestCase(FontFamilyCore.Modern, "Arial")]
        [TestCase(FontFamilyCore.Decorative, "Gabriola")]
        [TestCase(FontFamilyCore.Script, "Bradley Hand ITC")]
        public void TestFamily(FontFamilyCore family, string expectedResult)
        {
            CheckSubstitution(CreateSubstitutionInfo(family, FontPitchCore.Variable, 0), expectedResult);
        }

        [Test]
        [TestCase(FontFamilyCore.Auto, "MS Mincho")]
        [TestCase(FontFamilyCore.Roman, "MS Mincho")]
        [TestCase(FontFamilyCore.Swiss, "MS Gothic")]
        [TestCase(FontFamilyCore.Modern, "MS Gothic")]
        [TestCase(FontFamilyCore.Decorative, "Gabriola")]
        [TestCase(FontFamilyCore.Script, "Bradley Hand ITC")]
        public void TestFamilyFixedPitch(FontFamilyCore family, string expectedResult)
        {
            CheckSubstitution(CreateSubstitutionInfo(family, FontPitchCore.Fixed, 0), expectedResult);
        }

        [Test]
        [TestCase(FontFamilyCore.Auto, 0x02, "Symbol")]
        [TestCase(FontFamilyCore.Roman, 0xCC, "Times New Roman")]
        [TestCase(FontFamilyCore.Swiss, 0xCC, "Arial")]
        [TestCase(FontFamilyCore.Roman, 0x80, "MS Mincho")]
        [TestCase(FontFamilyCore.Swiss, 0x80, "MS Gothic")]
        public void TestCharset(FontFamilyCore family, int charset, string expectedResult)
        {
            CheckSubstitution(CreateSubstitutionInfo(family, FontPitchCore.Default, charset), expectedResult);
        }

        [Test]
        [TestCase("Arial", FontStyle.Regular)]
        [TestCase("Arial", FontStyle.Bold)]
        [TestCase("Arial", FontStyle.Italic)]
        [TestCase("Courier New", FontStyle.Regular)]
        [TestCase("Courier New", FontStyle.Bold)]
        [TestCase("Courier New", FontStyle.Italic)]
        [TestCase("Cambria", FontStyle.Regular)]
        [TestCase("Times New Roman", FontStyle.Regular)]
        [TestCase("Symbol", FontStyle.Regular)]
        [TestCase("Wingdings", FontStyle.Regular)]
        public void TestFontMatch(string familyName, FontStyle style)
        {
            CheckSubstitution(CreateSubstitutionInfo(FindFont(familyName, style)), familyName);
        }

        private static FontSearchInfo FindFont(string familyName, FontStyle style)
        {
            foreach (FontSearchInfo info in gTestFonts)
            {
                if (info.FontFamilyName == familyName && info.Style == style)
                    return info;
            }

            throw new InvalidOperationException("Font not found.");
        }

        private static FontSubstitutionInfo CreateSubstitutionInfo(FontFamilyCore family, FontPitchCore pitch, int charset)
        {
            return new FontSubstitutionInfo(FontPanose.Empty, FontUnicodeRanges.Empty, FontCodepageRanges.Empty, family, pitch,
                       charset, 0);
        }

        private static FontSubstitutionInfo CreateSubstitutionInfo(FontSearchInfo searchInfo)
        {
            return new FontSubstitutionInfo(searchInfo.Panose, searchInfo.UnicodeRanges, searchInfo.CodepageRanges,
                       FontFamilyCore.Auto, FontPitchCore.Default, 0, 0);
        }

        private void CheckSubstitution(FontSubstitutionInfo info, string expectedResult)
        {
            string result = mStrategy.GetSubstitution(info, gTestFonts);
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        private static IEnumerable<FontSearchInfo> gTestFonts;
        private readonly FontSubstitutionStrategyGeneral mStrategy = new FontSubstitutionStrategyGeneral();

        private static readonly List<string> gTestFontNames =
            new List<string>(new string[]
                          {
                              "Arial", "Bradley Hand ITC", "Calibri", "Cambria", "Cambria Math", "Courier New", "Gabriola",
                              "MS Gothic", "MS Mincho", "Segoe UI", "Symbol", "Tahoma", "Times New Roman", "Wingdings"
                          });

    }
}
