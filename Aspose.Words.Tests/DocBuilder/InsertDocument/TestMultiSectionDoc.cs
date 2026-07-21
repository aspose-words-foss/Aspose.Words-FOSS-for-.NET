// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/12/2014 by Ilya Navrotskiy

using NUnit.Framework;

namespace Aspose.Words.Tests.DocBuilder.InsertDocument
{
    /// <summary>
    /// WORDSNET-5251 Consider providing a one line method in the API for InsertDocument.
    /// Tests insertion of document with multiple sections.
    /// </summary>
    [TestFixture]
    public class TestMultiSectionDoc : TestInsertDocumentBase
    {
        /// <summary>
        /// Checks that paragraph before inserted content is a list item.
        /// </summary>
        [Test]
        public void TestListParaBefore()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                if (!IsBadLocation(location))
                {
                    TestInsertDocument(TestInsertSrcDocType.MultiSectionDoc, location);
                    switch (location)
                    {
                        case TestInsertDstDocLocation.BeginningOfListItem:
                        case TestInsertDstDocLocation.MiddleOfListItem:
                        case TestInsertDstDocLocation.EndOfListItem:
                        case TestInsertDstDocLocation.DocumentEnd:
                            Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                            Assert.That(ParaBeforeInsertedContent.IsListItem, Is.True);
                            break;
                        case TestInsertDstDocLocation.Middle:
                        case TestInsertDstDocLocation.MiddleWithSpaceAfter:
                        case TestInsertDstDocLocation.MiddleWithSpaceBefore:
                        case TestInsertDstDocLocation.InlineSdt:
                            Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                            Assert.That(ParaBeforeInsertedContent.IsListItem, Is.False);
                            break;
                        default:
                            Assert.That(ParaBeforeInsertedContent, Is.Null);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks that paragraph after inserted content is a list item and 
        /// its 'id' is the same as paragraph's list 'id' before inserted content.
        /// </summary>
        [Test]
        public void TestListParaAfter()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                if (!IsBadLocation(location))
                {
                    TestInsertDocument(TestInsertSrcDocType.MultiSectionDoc, location);
                    switch (location)
                    {
                        case TestInsertDstDocLocation.BeginningOfListItem:
                        case TestInsertDstDocLocation.MiddleOfListItem:
                        case TestInsertDstDocLocation.EndOfListItem:
                            Assert.That(ParaAfterInsertedContent.ListFormat.ListId, Is.EqualTo(ParaBeforeInsertedContent.ListFormat.ListId));
                            break;
                        case TestInsertDstDocLocation.InlineSdt:
                            Assert.That(ParaAfterInsertedContent, IsNot.Null());
                            Assert.That(ParaAfterInsertedContent.IsListItem, Is.True);
                            break;
                        default:
                            Assert.That(ParaAfterInsertedContent, IsNot.Null());
                            Assert.That(ParaAfterInsertedContent.IsListItem, Is.False);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks that inserted paragraphs are list items.
        /// </summary>
        [Test]
        public void TestListInsertedParagraphs()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                if (!IsBadLocation(location))
                {
                    TestInsertDocument(TestInsertSrcDocType.MultiSectionDoc, location);
                    switch (location)
                    {
                        case TestInsertDstDocLocation.BeginningOfListItem:
                            Assert.That(FirstInsertedPara.IsListItem, Is.True);
                            Assert.That((FirstInsertedParaInSecondSection != null) && (FirstInsertedParaInSecondSection.IsListItem), Is.True);
                            Assert.That((FirstInsertedParaInThirdSection != null) && (FirstInsertedParaInThirdSection.IsListItem), Is.True);
                            break;
                        default:
                            Assert.That(FirstInsertedPara.IsListItem, Is.False);
                            Assert.That((FirstInsertedParaInSecondSection != null) && (FirstInsertedParaInSecondSection.IsListItem), Is.False);
                            Assert.That((FirstInsertedParaInThirdSection != null) && (FirstInsertedParaInThirdSection.IsListItem), Is.False);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks styles of the inserted paragraphs.
        /// </summary>
        [Test]
        public void TestStyleInsertedParagraphs()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                if (!IsBadLocation(location))
                {
                    TestInsertDocument(TestInsertSrcDocType.MultiSectionDoc, location);
                    if (location != TestInsertDstDocLocation.InlineSdt)
                    {
                        Assert.That(FirstInsertedParaInSecondSection, IsNot.Null());
                        Assert.That(FirstInsertedParaInThirdSection, IsNot.Null());
                        Assert.That(FirstInsertedPara.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.User));
                        Assert.That(FirstInsertedParaInSecondSection.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.User));
                        Assert.That(FirstInsertedParaInThirdSection.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.User));
                    }
                }
            }
        }

        /// <summary>
        /// Checks style of the paragraph before inserted content.
        /// </summary>
        [Test]
        public void TestStyleParaBefore()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                if (!IsBadLocation(location))
                {
                    TestInsertDocument(TestInsertSrcDocType.MultiSectionDoc, location);
                    switch (location)
                    {
                        case TestInsertDstDocLocation.BeginningOfListItem:
                        case TestInsertDstDocLocation.MiddleOfListItem:
                        case TestInsertDstDocLocation.EndOfListItem:
                        case TestInsertDstDocLocation.DocumentEnd:
                            Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                            Assert.That(ParaBeforeInsertedContent.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.ListParagraph));
                            break;
                        case TestInsertDstDocLocation.DocumentStart:
                        case TestInsertDstDocLocation.BlockLevelSdt:
                            Assert.That(ParaBeforeInsertedContent, Is.Null);
                            break;
                        default:
                            Assert.That(ParaBeforeInsertedContent, IsNot.Null());
                            Assert.That(ParaBeforeInsertedContent.ParagraphStyle.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks that first inserted node was concatenated with the paragraph before it.
        /// </summary>
        [Test]
        public void TestCheckFirstParasConcatenated()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                if (!IsBadLocation(location))
                {
                    TestInsertDocument(TestInsertSrcDocType.MultiSectionDoc, location);

                    int firstInsertedParaTextIdx = FirstInsertedParaText.IndexOf("First inserted paragraph");
                    switch (location)
                    {
                        case TestInsertDstDocLocation.BeginningOfListItem:
                        case TestInsertDstDocLocation.DocumentStart:
                            Assert.That(firstInsertedParaTextIdx, Is.EqualTo(0));
                            break;
                        default:
                            Assert.That(firstInsertedParaTextIdx, Is.GreaterThan(1));
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Checks insertion into inline SDT with default options.
        /// </summary>
        [Test]
        public void TestInsertionIntoInlineSdtWithDefaultOptions()
        {
            TestInsertDocument(TestInsertSrcDocType.MultiSectionDoc, TestInsertDstDocLocation.InlineSdt);
            const string insertedText =
                "inline level First inserted paragraph\t\t This is content of the second section Last inserted paragraphSDT";

            Assert.That(FirstInsertedParaText.Contains(insertedText), Is.True);
        }

        /// <summary>
        /// Checks insertion into inline SDT with disabled 'AdjustSpacing' option.
        /// </summary>
        [Test]
        public void TestInsertionIntoInlineSdtWithAdjustSpacingOptionDisabled()
        {
            TestInsertDocument(TestInsertSrcDocType.MultiSectionDoc, TestInsertDstDocLocation.InlineSdt, new ImportFormatOptions());
            const string insertedText =
                "inline levelFirst inserted paragraph\t\t This is content of the second section Last inserted paragraphSDT";

            Assert.That(FirstInsertedParaText.Contains(insertedText), Is.True);
        }


        /// <summary>
        /// Checks that document has needed count of the sections and appropriate headers-footers.
        /// </summary>
        [Test]
        public void TestSectionsNumber()
        {
            foreach (TestInsertDstDocLocation location in DstDocPossibleLocations)
            {
                if (!IsBadLocation(location))
                {
                    TestInsertDocument(TestInsertSrcDocType.MultiSectionDoc, location);
                    switch (location)
                    {
                        case TestInsertDstDocLocation.InlineSdt:
                            Assert.That(FirstInsertedSection.HeadersFooters[HeaderFooterType.HeaderPrimary].GetText() == "This is header\r", Is.True);
                            Assert.That(FirstInsertedNode.FetchDocument().Sections.Count, Is.EqualTo(1));
                            break;
                        default:
                            Assert.That(FirstInsertedNode.FetchDocument().Sections.Count, Is.EqualTo(3));
                            Assert.That(SecondInsertedSection, IsNot.Null());
                            Assert.That(ThirdInsertedSection, IsNot.Null());
                            Assert.That(FirstInsertedSection.HeadersFooters[HeaderFooterType.HeaderFirst].GetText() == "Header section1\r", Is.True);
                            Assert.That(SecondInsertedSection.HeadersFooters[HeaderFooterType.HeaderPrimary].GetText() == "Source document header second section\r", Is.True);
                            Assert.That(ThirdInsertedSection.HeadersFooters[HeaderFooterType.HeaderPrimary].GetText() == "This is header\r", Is.True);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true, if document can not be inserted at the specified location.
        /// </summary>
        private static bool IsBadLocation(TestInsertDstDocLocation location)
        {
            return IsBadDstDocTestLocation(TestInsertSrcDocType.MultiSectionDoc, location);
        }
    }
}

