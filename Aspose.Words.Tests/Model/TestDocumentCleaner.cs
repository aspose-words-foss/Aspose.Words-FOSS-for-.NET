// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/05/2017 by Alexey Morozov

using System;
using Aspose.Common;
using Aspose.Words.Lists;
using Aspose.Words.Notes;
using Aspose.Words.Tables;
using Aspose.Words.Validation;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests DocumentCleaner feature.
    /// </summary>
    [TestFixture]
    public class TestDocumentCleaner
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [SetUp]
        public void Init()
        {
            RandomUtil.Reset();
        }



        /// <summary>
        /// Tests that Click-and-Type and default table styles are preserved
        /// in a document after removing unused resources.
        /// </summary>
        [Test]
        public void TestPreservingStylesAfterRemovingUnusedResources()
        {
            Document doc = new Document();

            Style style = doc.Styles.Add(StyleType.Paragraph, "MyClickAndType");
            style.BaseStyleName = "Heading 3";
            doc.DocPr.ClickTypeParaStyleIstd = style.Istd;

            style = doc.Styles.Add(StyleType.Paragraph, "MyTable");
            style.BaseStyleName = "Heading 4";
            doc.DocPr.DefaultTableStyleIstd = style.Istd;

            doc.Cleanup();

            Assert.That(doc.Styles.GetByName("MyClickAndType", false), IsNot.Null());
            Assert.That(doc.Styles.GetByName("MyTable", false), IsNot.Null());
        }


        // FOSS: TestStyleIstdNormalization (WORDSNET-19345) removed. Its inputs were binary .doc files
        // authored with non-contiguous (missing) style ISTDs, which is the precondition the test asserts
        // (Debug.Assert(MaxIstd < oldMaxIstd)). The .doc reader is removed; the Word .doc->.docx conversion
        // renumbers styles contiguously, eliminating the ISTD gaps, so the scenario cannot be reproduced.


        /// <summary>
        /// WORDSNET-20094 Implemented ability (optional) to remove duplicate styles.
        /// </summary>
        [Test]
        public void TestRemovingDuplicateStyles()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestRemovingDuplicateStyles.docx");
            StyleCollection styles = doc.Styles;
            Paragraph para3 = doc.FirstSection.Body.Paragraphs[2];
            Paragraph para4 = doc.FirstSection.Body.Paragraphs[3];
            Debug.Assert(styles.Count == 12);
            Debug.Assert(para3.ParagraphStyle.Name == "Style3");
            Debug.Assert(para4.ParagraphStyle.Name == "Style4");

            CleanupOptions options = new CleanupOptions();
            options.UnusedStyles = false;
            options.UnusedLists = false;
            options.DuplicateStyle = true;

            doc.Cleanup(options);

            Assert.That(styles.Count, Is.EqualTo(8));
            Assert.That(styles["Style3"], Is.Null);
            Assert.That(styles["Style4"], Is.Null);
            Assert.That(para3.ParagraphStyle.Name, Is.EqualTo("Style1"));
            Assert.That(para4.ParagraphStyle.Name, Is.EqualTo("Style2"));
        }


        /// <summary>
        /// WORDSNET-19312 Tests removing unused and duplicate styles in the customer's document.
        /// </summary>
        [Test]
        public void Test19312()
        {
            Document doc = TestUtil.Open(@"Model\Style\Test19312.docx");
            StyleCollection styles = doc.Styles;
            StyleCollection glossaryStyles = doc.GlossaryDocument.Styles;

            Debug.Assert(styles.Count == 58);
            Debug.Assert(glossaryStyles.Count == 2502);

            CleanupOptions options = new CleanupOptions();
            options.DuplicateStyle = true;

            doc.Cleanup(options);

            Assert.That(styles.Count, Is.EqualTo(52));
            Assert.That(glossaryStyles.Count, Is.EqualTo(7));
        }

        /// <summary>
        /// WORDSNET-20094 Tests removing duplicate table styles.
        /// </summary>
        [Test]
        public void TestRemovingDuplicateTableStyles()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestComparingTableStyles.docx");
            StyleCollection styles = doc.Styles;
            Table table2 = doc.FirstSection.Body.Tables[1];
            Debug.Assert(styles.Count == 8);
            Debug.Assert(table2.StyleName == "TableStyle2");

            CleanupOptions options = new CleanupOptions();
            options.UnusedStyles = false;
            options.UnusedLists = false;
            options.DuplicateStyle = true;

            doc.Cleanup(options);

            Assert.That(styles.Count, Is.EqualTo(7));
            Assert.That(styles["TableStyle2"], Is.Null);
            Assert.That(table2.Style.Name, Is.EqualTo("TableStyle1"));
        }

        /// <summary>
        /// WORDSNET-21597 Unused styles are not cleaned from DOCX.
        /// Added public property UnusedBuiltinStyles.
        /// </summary>
        [Test]
        public void Test21977()
        {
            Document doc = TestUtil.Open(@"Model\Style\Test21977.docx");

            Assert.That(doc.Styles.Count, Is.EqualTo(64));

            CleanupOptions cleanupOptions = new CleanupOptions();
            cleanupOptions.DuplicateStyle = true;
            cleanupOptions.UnusedStyles = true;
            cleanupOptions.UnusedLists = true;
            cleanupOptions.UnusedBuiltinStyles = true;
            doc.Cleanup(cleanupOptions);

            Assert.That(doc.Styles.Count, Is.EqualTo(9));
        }
    }
}
