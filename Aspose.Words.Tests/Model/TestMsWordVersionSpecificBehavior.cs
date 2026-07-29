// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/02/2014 by Denis Darkin

using Aspose.Words.Loading;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests different aspects of Document.OptimizeFor method, which should instruct Model or DOCX export on how to process documents in order
    /// to mimic all MSW version-specific behavior known to date.
    /// </summary>
    [TestFixture]
    public class TestMsWordVersionSpecificBehavior
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-4777 MS Word 2010 shows "Compatibility mode"on the title bar 
        /// during opening the document saved by Aspose.Words with Iso29500_2008_Transitional.
        /// Compatibility setting string should be written to suppress warning.
        /// </summary>
        [Test]
        public void TestDefect4777()
        {
            // There was error in StylePaneFormatFilterSettings - flags set worked incorrectly. Test it's OK now.
            StylePaneFormatFilterSettings settings = new StylePaneFormatFilterSettings();
            settings.AllStyles = true;
            Assert.That(settings.AllStyles, Is.True);
            settings.AllStyles = false;
            Assert.That(settings.AllStyles, Is.False);

            // Customer test case.
            Document doc = new Document();
            OoxmlSaveOptions opt = new OoxmlSaveOptions(SaveFormat.Docx);
            opt.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;
            opt.SetTestMode();
            doc.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2010);
            // Verify against gold file.
            TestUtil.SaveOpen(doc, @"Model\MswVersionSpecific\TestDefect4777.docx", opt);
        }

        [Test]
        public void TestBuiltInStylesOptimized()
        {
            Document doc = new Document();
            doc.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2000);
            Assert.That(doc.Styles["Heading 1"].RunPr.Count, Is.EqualTo(8));

            doc = new Document();
            doc.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2010);
            Assert.That(doc.Styles["Heading 1"].RunPr.Count, Is.EqualTo(10));
        }

        /// <summary>
        /// WORDSNET-7748 Export layout options as per Microsoft Word 2010 standard.
        /// The problem occurs because upon creating new document from scratch, AW loads "Aspose.Words.Resources.Blank.doc" 
        /// and of course Layout Options will be set for MS Word 2003.
        /// andrnosk: Fixed by adding an ability to set needed set of default options for specific formats. 
        /// </summary>
        [Test]
        public void TestJira7748InitWord2010Options()
        {
            Document doc = new Document();

            // Initialize compatibility options to Word 2010 default values.
            doc.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2010);

            OoxmlSaveOptions opt = new OoxmlSaveOptions(SaveFormat.Docx);
            opt.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;

            doc = TestUtil.SaveOpen(doc, @"Model\MswVersionSpecific\TestJira7748A.docx", opt);
            Assert.That(doc.CompatibilityOptions.CustomCompatibilitySettings["compatibilityMode"].Value, Is.EqualTo("14"));
            Assert.That(doc.CompatibilityOptions.CustomCompatibilitySettings.Count, Is.EqualTo(5));
        }

        [Test]
        public void TestJira7748InitWord2013Options()
        {
            Document doc = new Document();

            // Initialize compatibility options to Word 2013 default values.
            doc.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2013);

            OoxmlSaveOptions opt = new OoxmlSaveOptions(SaveFormat.Docx);
            opt.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;

            doc = TestUtil.SaveOpen(doc, @"Model\MswVersionSpecific\TestJira7748B.docx", opt);
            Assert.That(doc.CompatibilityOptions.CustomCompatibilitySettings["compatibilityMode"].Value, Is.EqualTo("15"));
            Assert.That(doc.CompatibilityOptions.CustomCompatibilitySettings.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// Tests default values of compatibility options for MS Word 2016.
        /// </summary>
        [Test]
        public void TestOptimizingForWord2016()
        {
            Document doc = new Document();

            doc.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2016);

            CustomCompatibilitySettingCollection options = doc.CompatibilityOptions.CustomCompatibilitySettings;
            Assert.That(options.Count, Is.EqualTo(5));
            Assert.That(options["compatibilityMode"].Value, Is.EqualTo("15"));
            Assert.That(options["overrideTableStyleFontSizeAndJustification"].Value, Is.EqualTo("1"));
            Assert.That(options["enableOpenTypeFeatures"].Value, Is.EqualTo("1"));
            Assert.That(options["doNotFlipMirrorIndents"].Value, Is.EqualTo("1"));
            Assert.That(options["differentiateMultirowTableHeaders"].Value, Is.EqualTo("1"));
        }

        /// <summary>
        /// Tests changing of document properties after running OptimizeFor.
        /// </summary>
        [TestCase(MsWordVersion.Word2007, false, null, false, 0, null)]
        [TestCase(MsWordVersion.Word2010, false, null, true, 96, "62376D8B")]
        [TestCase(MsWordVersion.Word2013, true, TestGuid, true, 96, "62376D8B")]
        [TestCase(MsWordVersion.Word2016, true, TestGuid, true, 96, "62376D8B")]
        public void TestDocumentProperties(MsWordVersion version, bool expectedChartTrackingRefBased,
            string expectedDocumentSetId, bool expectedDiscardImageEditingData, int expectedDefaultImageDpi,
            string expectedDocId)
        {
            Document doc = new Document();

            DocPr docPr = doc.DocPr;
            docPr.ChartTrackingRefBased = true;
            docPr.DocumentSetId = TestGuid;
            docPr.DiscardImageEditingData = true;
            docPr.DefaultImageDpi = 96;
            docPr.DocId = expectedDocId;

            doc.CompatibilityOptions.OptimizeFor(version);

            OoxmlSaveOptions opt = new OoxmlSaveOptions(SaveFormat.Docx);
            opt.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;
            opt.SetTestMode();
            opt.WriteW14DocId = true;

            doc = TestUtil.SaveOpen(doc, @"Model\MswVersionSpecific\TestDocumentProperties.docx", opt, false);

            docPr = doc.DocPr;
            Assert.That(docPr.ChartTrackingRefBased, Is.EqualTo(expectedChartTrackingRefBased));
            Assert.That(docPr.DocumentSetId, Is.EqualTo(expectedDocumentSetId));
            Assert.That(docPr.DiscardImageEditingData, Is.EqualTo(expectedDiscardImageEditingData));
            Assert.That(docPr.DefaultImageDpi, Is.EqualTo(expectedDefaultImageDpi));
            Assert.That(docPr.DocId, Is.EqualTo(expectedDocId));
        }

        /// <summary>
        /// WORDSNET-19908 Verifies MSWord 2013 setting as a default value.
        /// Checks DefaultParaPr values setting for versions MSWord2007 and MSWord2013.
        /// </summary>
        [Test]
        public void TestJira19908()
        {
            // Loads a document created in WPS office with the missing DefaultParaPr section.
            Document doc = TestUtil.Open(@"Model\Document\TestJira19908.docx");
            ParaPr paraPr = doc.Styles.DefaultParaPr;
            // Checks DefaultParaPr values for MSWord 2013.
            Assert.That(paraPr.SpaceAfter, Is.EqualTo(160));
            Assert.That(paraPr.LineSpacing, Is.EqualTo(259));

            LoadOptions lo = new LoadOptions();
            lo.MswVersion = MsWordVersion.Word2007;
            // Explicitly sets MSWord 2007.
            doc = TestUtil.Open(@"Model\Document\TestJira19908.docx", lo);
            paraPr = doc.Styles.DefaultParaPr;
            // Checks DefaultParaPr values for MSWord 2007.
            Assert.That(paraPr.SpaceAfter, Is.EqualTo(200));
            Assert.That(paraPr.LineSpacing, Is.EqualTo(276));
        }

        /// <summary>
        /// Test GUID for the <see cref="TestDocumentProperties"/> test.
        /// </summary>
        private const string TestGuid = "{E461F57B-44F5-469F-AE1F-68CCE877F320}";
    }
}
