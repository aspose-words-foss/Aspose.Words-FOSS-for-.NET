// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/06/2012 by Alexey Butalov

using Aspose.Words.Loading;
using Aspose.Words.RW.Txt.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Txt
{
    /// <summary>
    /// Unit tests for TxtNumbering class
    /// </summary>
    [TestFixture]
    public class TestTxtNumbering
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            mArabicDotNumberingStyle = new ArabicDotTxtNumberingStyle();
            mAsteriskNumberingStyle = new AsteriskTxtNumberingStyle();
        }

        [Test]
        public void TestIsFirstChildForTrue1()
        {
            TxtNumbering parentNumbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "2", "33" }, "2.33.");
            TxtNumbering numbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "2", "33", "1" }, "2.33.1.");

            Assert.That(numbering.IsFirstChildFor(parentNumbering), Is.True);
        }

        [Test]
        public void TestIsFirstChildForTrue2()
        {
            TxtNumbering parentNumbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "1" }, "1.");
            TxtNumbering numbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "1", "1" }, "1.1.");

            Assert.That(numbering.IsFirstChildFor(parentNumbering), Is.True);
        }

        [Test]
        public void TestIsFirstChildForFalse1()
        {
            TxtNumbering parentNumbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "1" }, "1.");
            TxtNumbering numbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "1", "2" }, "1.2.");

            Assert.That(numbering.IsFirstChildFor(parentNumbering), Is.False);
        }

        [Test]
        public void TestIsFirstChildForFalse2()
        {
            TxtNumbering parentNumbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "1" }, "1.");
            TxtNumbering numbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "1", "2", "1" }, "1.2.1.");

            Assert.That(numbering.IsFirstChildFor(parentNumbering), Is.False);
        }

        [Test]
        public void TestIsFirstChildForFalse3()
        {
            TxtNumbering parentNumbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "1" }, "1.");
            TxtNumbering numbering = new TxtNumbering(
                mAsteriskNumberingStyle, new string[] { "*" }, "*");

            Assert.That(numbering.IsFirstChildFor(parentNumbering), Is.False);
        }

        [Test]
        public void TestIsPrevSiblingForTrue1()
        {
            TxtNumbering nextNumbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "2", "33" }, "2.33.");
            TxtNumbering numbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "2", "32" }, "2.32.");

            Assert.That(numbering.IsPrevSiblingFor(nextNumbering), Is.True);
        }

        [Test]
        public void TestIsPrevSiblingForTrue2()
        {
            TxtNumbering nextNumbering = new TxtNumbering(
                mAsteriskNumberingStyle, new string[] { "*" }, "*");
            TxtNumbering numbering = new TxtNumbering(
                mAsteriskNumberingStyle, new string[] { "*" }, "*");

            Assert.That(numbering.IsPrevSiblingFor(nextNumbering), Is.True);
        }

        [Test]
        public void TestIsPrevSiblingForFalse1()
        {
            TxtNumbering nextNumbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "2", "33" }, "2.33.");
            TxtNumbering numbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "2", "33" }, "2.33.");

            Assert.That(numbering.IsPrevSiblingFor(nextNumbering), Is.False);
        }

        [Test]
        public void TestIsPrevSiblingForFalse2()
        {
            TxtNumbering nextNumbering = new TxtNumbering(
                mArabicDotNumberingStyle, new string[] { "2", "33" }, "2.33.");
            TxtNumbering numbering = new TxtNumbering(
                mAsteriskNumberingStyle, new string[] { "*" }, "*");

            Assert.That(numbering.IsPrevSiblingFor(nextNumbering), Is.False);
        }

        /// <summary>
        /// WORDSNET-28829 Document.GetText() returns the text without numbered lists.
        /// The issue is 'Not a Bug'. Added the verification test.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void Test28829(bool autoNumberingDetection)
        {
            TxtLoadOptions loadOptions = new TxtLoadOptions();
            loadOptions.AutoNumberingDetection = autoNumberingDetection;

            Document doc = TestUtil.Open(@"ImportTxt\Test28829.txt", loadOptions);

            Paragraph para = TestUtil.GetParagraphWithText(doc, "...Boil some water...");
            Assert.That(para.IsListItem, Is.EqualTo(autoNumberingDetection));

            string text = doc.GetText();
            // When automatic numbering detection is disabled, list label is just plain text within a Run node.
            string expectedText = string.Format("\r{0}Boil some water.", autoNumberingDetection ? "" : "2. ");
            Assert.That(text, Contains.Substring(expectedText));
        }

#if ANDROID
        static
#endif
        private ArabicDotTxtNumberingStyle mArabicDotNumberingStyle;

#if ANDROID
        static
#endif
        private AsteriskTxtNumberingStyle mAsteriskNumberingStyle;
    }
}
