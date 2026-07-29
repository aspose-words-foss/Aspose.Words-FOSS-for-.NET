// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Loading;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    [TestFixture]
    [NonParallelizable]
    public class TestSection
    {
        [SetUp]
        public void SetUp()
        {
            SystemPal.SaveCulture();
            TestUtil.SetUpTests();
            mOldSectTestMode = SectPr.TestMode;
        }

        [TearDown]
        public void TearDown()
        {
            SystemPal.RestoreCulture();
            SectPr.TestMode = mOldSectTestMode;
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "Cannot insert a node of this type at this location.")]
        public void TestTwoBodies()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Section sect = doc.AppendChild(new Section(doc));
            sect.AppendChild(new Body(doc));
            sect.AppendChild(new Body(doc));
        }

        [Test]
        public void TestTwoDifferentHeaders()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Section sect = doc.AppendChild(new Section(doc));
            //Adding headers of different type is okay.
            sect.AppendChild(new HeaderFooter(doc, HeaderFooterType.FooterPrimary));
            sect.AppendChild(new HeaderFooter(doc, HeaderFooterType.HeaderPrimary));
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "Cannot insert a node of this type at this location.")]
        public void TestTwoSameHeaders()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Section sect = doc.AppendChild(new Section(doc));
            sect.AppendChild(new HeaderFooter(doc, HeaderFooterType.FooterPrimary));
            sect.AppendChild(new HeaderFooter(doc, HeaderFooterType.FooterPrimary));
        }

        /// <summary>
        /// Test that shortcuts to body and headers footers work.
        /// </summary>
        [Test]
        public void TestShortcuts()
        {
            Document doc = new Document(DocumentCtorMode.EmptyDocumentNode);
            Section sect = doc.AppendChild(new Section(doc));

            Assert.That(sect.Body, Is.Null);
            Assert.That(sect.HeadersFooters[HeaderFooterType.FooterFirst], Is.Null);

            sect.AppendChild(new HeaderFooter(doc, HeaderFooterType.FooterFirst));
            sect.AppendChild(new Body(doc));

            Assert.That(sect.Body, IsNot.Null());
            Assert.That(sect.HeadersFooters[HeaderFooterType.FooterFirst], IsNot.Null());
            Assert.That(sect.HeadersFooters[HeaderFooterType.FooterPrimary], Is.Null);
        }


        /// <summary>
        /// WORDSNET-13826 System.InvalidCastException is thrown while loading document
        /// under different Culture.
        /// Threads can have different culture and AW overwrites defaults for section attributes every time
        /// when current culture was changed. Moreover AW does not implement it in thread safe manner.
        /// Such behavior causes different exceptions.
        /// </summary>
        [Test]
        public void TestJira13826()
        {
            SectPr.TestMode = false;
            string[] langs = new string[] {"fr-FR", "en-GB", "de-DE", "en-US"};

            // Prepare cache.
            IntToObjDictionary<SectPr> cache = SectPr.DefaultsCache;
            cache.Clear();

            // 1. Check that every thread with custom culture can have own collection of the section attributes.
            // Each collection cached only one time.
            AppendSectDefaults(langs, null);
            AppendSectDefaults(langs, cache.Values.GetEnumerator());

            Assert.That(cache.Count, Is.EqualTo(4));

            // 2. Check that every collection contains culture specific values.
            IntToObjDictionary<SectPr>.Enumerator enumerator = cache.GetEnumerator();

            while (enumerator.MoveNext())
            {
                SectPr currAttrs = enumerator.CurrentValue;
                int space = LocaleDefaults.GetSpaceBetweenColumns(enumerator.CurrentKey);
                Assert.That(currAttrs.GetDirectAttr(SectAttr.ColumnsSpacing), Is.EqualTo(space));
                Assert.That(currAttrs.GetDirectAttr(SectAttr.SectionStart), Is.EqualTo(SectionStart.NewPage));
            }

            // 3. Check that attempt to get default values of the section properties returns collection from cache.
            TestSectPr sectPr = new TestSectPr();
            List<SectPr> expectedColls = new List<SectPr>(langs.Length);

            foreach (string lang in langs)
            {
                SystemPal.SetCulture(lang);
                expectedColls.Add(sectPr.GetSectDefault());
            }

            AppendSectDefaults(langs, expectedColls.GetEnumerator());
        }

        /// <summary>
        /// MS Word uses different defaults for page margins, header/footer distance and column
        /// spacing. The defaults depend on the language during generation of the Normal template.
        /// Aspose.Words mimics MS Word behavior with using current culture settings.
        /// </summary>
        [TestCase("en-US", 1440, 1440, 1440, 1440, 720, 720, 720)]
        [TestCase("de-DE", 1417, 1417, 1417, 1134, 708, 708, 708)]
        [TestCase("it-IT", 1134, 1134, 1417, 1134, 708, 708, 708)]
        [TestCase(null, 1440, 1440, 1440, 1440, 708, 708, 708)]
        [NonParallelizable]
        public void TestDefaultMargins(string cultureName, int expectedLeftMargin, int expectedRightMargin,
            int expectedTopMargin, int expectedBottomMargin, int expectedHeaderDistance, int expectedFooterDistance,
            int expectedColumnSpacing)
        {
            SectPr.TestMode = false;

            if (StringUtil.HasChars(cultureName))
                SystemPal.SetCulture(cultureName);
            else
                SystemPal.SetStandardCulture();

            LocaleDefaults.Margins expectedMargins = new LocaleDefaults.Margins(expectedLeftMargin,
                expectedRightMargin, expectedTopMargin, expectedBottomMargin);

            // New document
            Document newDoc = new Document();
            CheckMargins(newDoc.FirstSection, expectedMargins, expectedHeaderDistance, expectedFooterDistance,
                expectedColumnSpacing);

            // FOSS: the "loaded document" leg used an inline WordML 2003 stream; the WML reader is
            // removed, so the locale-default margins are verified on the new document only.
        }

        /// <summary>
        /// WORDSNET-14959 Different page margins are reported by AW model and MS Word.
        /// Checks whether page margins is obtained by LCID specified in the registry instead of the one set in the current locale.
        /// </summary>
        // FOSS: .doc, .rtf and .wml cases removed (those readers are gone); .docx case retained.
        public void Test14959(string testFile)
        {
            if (!PlatformUtilPal.IsWindows())
                return;

            int sysLcid = SystemPal.GetSystemDefaultLcid();
            LoadOptions lo = Path.GetExtension(testFile).Equals(".rtf")
                ? new RtfLoadOptions()
                : new LoadOptions();
            lo.UseSystemLcid = true;
            Document doc = TestUtil.Open(testFile, lo);

            // Checks the defaults of the first section corresponding to the system LCID set in DocumentPostLoader.
            CheckMargins(sysLcid, doc.Sections[0]);

            // Checks the defaults of the second section corresponding to the system LCID set in a document reader-s.
            CheckMargins(sysLcid, doc.Sections[1]);

            // LoadOptions.UseSystemLcid only works during the document loading stage. Subsequently, the model works with the current locale.
            Section sect = new Section(doc);
            doc.Sections.Add(sect);
            // Checks the defaults of the last section corresponding to the test LCID set by default.
            CheckMargins(TestLcid, doc.LastSection);
        }

        /// <summary>
        /// WORDSNET-25620 Empty documents created by Aspose.Words have different page setup
        /// Checks whether SectPr.SetLocaleDefaultsForNewDocument() is called to load from an empty stream.
        /// </summary>
        [Test]
        public void Test25620()
        {
            SectPr.TestMode = false;

            try
            {
                Document doc1 = new Document();
                doc1 = TestUtil.SaveOpen(
                    doc1,
                    @"Model\Document\Test25620_Blank",
                    UnifiedScenario.Docx2Docx | UnifiedScenario.NoGold);
                SectPr sectPr1 = doc1.FirstSection.SectPr;

                Document doc2 = new Document(new MemoryStream(), new LoadOptions());
                doc2 = TestUtil.SaveOpen(
                    doc2,
                    @"Model\Document\Test25620_Empty",
                    UnifiedScenario.Docx2Docx | UnifiedScenario.NoGold);
                SectPr sectPr2 = doc2.FirstSection.SectPr;

                Assert.That(sectPr2.PageWidth, Is.EqualTo(sectPr1.PageWidth));
                Assert.That(sectPr2.PageHeight, Is.EqualTo(sectPr1.PageHeight));
                Assert.That(sectPr2.LeftMargin, Is.EqualTo(sectPr1.LeftMargin));
                Assert.That(sectPr2.RightMargin, Is.EqualTo(sectPr1.RightMargin));
                Assert.That(sectPr2.TopMargin, Is.EqualTo(sectPr1.TopMargin));
                Assert.That(sectPr2.BottomMargin, Is.EqualTo(sectPr1.BottomMargin));
                Assert.That(sectPr2.HeaderDistance, Is.EqualTo(sectPr1.HeaderDistance));
                Assert.That(sectPr2.ColumnsSpacing, Is.EqualTo(sectPr1.ColumnsSpacing));
            }
            finally
            {
                SectPr.TestMode = true;
            }
        }

        /// <summary>
        /// WORDSNET-27383 Footer is changed after open/save document.
        /// If headers/footers are defined in a sectPr element as a direct child of a body, they apply to all
        /// subsequent sections, even if those have other headers/footers specified.
        /// </summary>
        [Test]
        public void Test27383()
        {
            const string fileName = @"Model\Section\Test27383.docx";
            Document doc = TestUtil.Open(fileName);

            for (int i = 1; i < doc.Sections.Count; i++)
            {
                // All sections inherit headers/footers from the first section.
                Assert.That(doc.Sections[i].HeadersFooters.Count, Is.EqualTo(0));
            }

            // The last section inherits properties from the first section.
            SectPr firstSectPr = doc.FirstSection.SectPr;
            SectPr lastSectPr = doc.LastSection.SectPr;
            Assert.That(lastSectPr.LeftMargin, Is.EqualTo(firstSectPr.LeftMargin));

            TestUtil.SaveCheckGoldExportOnly(doc, fileName);
        }

        /// <summary>
        /// Fills collections with default values to section properties cache using specified cultures.
        /// Also compares added collections with specified enumeration.
        /// </summary>
        /// <param name="langs">Names of the cultures.</param>
        /// <param name="expectedColls">Expected collections to compare.</param>
        private static void AppendSectDefaults(string[] langs, IEnumerator<SectPr> expectedColls)
        {
            for (int j = 0; j < langs.Length; ++j)
            {
                SystemPal.SetCulture(langs[j]);
                SectPr.FetchDefaultAttr(SectAttr.LeftMargin);

                if (expectedColls != null)
                {
                    Assert.That(expectedColls.MoveNext(), Is.True);
                    CultureInfo ci = SystemPal.GetCurrentCulture();
                    // Explicit cast to AttrCollection resolve overload ambiguity in C++
                    Assert.That((AttrCollection)expectedColls.Current, Is.EqualTo(SectPr.DefaultsCache[ci.LCID]));
                }
            }
        }

        /// <summary>
        /// Checks the margin values for conformity with the specified LCID.
        /// </summary>
        private void CheckMargins(int lcid, Section section)
        {
            const int testHeaderFooterDistance = 720;
            const int testColumnSpacing = 720;
            LocaleDefaults.Margins testDefaultMargins =
                new LocaleDefaults.Margins(1800, 1800, 1440, 1440);
            LocaleDefaults.Margins expectedMargins = lcid == TestLcid
                ? testDefaultMargins
                : LocaleDefaults.GetPageMargins(lcid);
            int expectedDistance = lcid == TestLcid
                ? testHeaderFooterDistance
                : LocaleDefaults.GetHeaderFooterDistance(lcid);
            int expectedColumnSpacing = lcid == TestLcid
                ? testColumnSpacing
                : LocaleDefaults.GetSpaceBetweenColumns(lcid);

            CheckMargins(section, expectedMargins, expectedDistance, expectedDistance,
                expectedColumnSpacing);
        }

        /// <summary>
        /// Checks page margins, header/footer distance, column spacing of the specified sectPr.
        /// </summary>
        private void CheckMargins(Section section, LocaleDefaults.Margins expectedMargins,
            int expectedHeaderDistance, int expectedFooterDistance, int expectedColumnSpacing)
        {
            SectPr sectPr = section.SectPr;

            Assert.That(sectPr.LeftMargin, Is.EqualTo(expectedMargins.Left), "Left margin is wrong.");
            Assert.That(sectPr.RightMargin, Is.EqualTo(expectedMargins.Right), "Right margin is wrong.");
            Assert.That(sectPr.TopMargin, Is.EqualTo(expectedMargins.Top), "Top margin is wrong.");
            Assert.That(sectPr.BottomMargin, Is.EqualTo(expectedMargins.Bottom), "Bottom margin is wrong.");

            Assert.That(sectPr.HeaderDistance, Is.EqualTo(expectedHeaderDistance), "Header distance is wrong.");
            Assert.That(sectPr.FooterDistance, Is.EqualTo(expectedFooterDistance), "Footer distance is wrong.");

            Assert.That(sectPr.ColumnsSpacing, Is.EqualTo(expectedColumnSpacing), "Space between columns is wrong.");
        }

        private bool mOldSectTestMode;

        private const int TestLcid = -1;

        /// <summary>
        /// Class is used for test purposes. Implements methods for access to nonpublic members.
        /// </summary>
        private class TestSectPr : SectPr
        {
            internal SectPr GetSectDefault()
            {
                return (SectPr)GetDefaults();
            }
        }
    }
}
