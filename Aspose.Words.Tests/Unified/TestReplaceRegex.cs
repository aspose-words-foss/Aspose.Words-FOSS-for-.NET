// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/05/2020 by Konstantin Sidorenko

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Drawing;
using Aspose.JavaAttributes;
using Aspose.Words.Notes;
using Aspose.Words.Replacing;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using CodePorting.Translator.Cs2Cpp;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    [TestFixture]
    public class TestReplaceRegex
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests that meta character works with Regex.
        /// </summary>
        [Test]
        public void TestRegexMetaCharacters()
        {
            const string testName = @"Model\Replace2\TestRegexMetaCharacters.docx";
            Document doc = TestUtil.Open(testName);
            int count = docRangeReplace(doc, "[&p]{2}", "[two~breaks]");
            Assert.That(count, Is.EqualTo(1));
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(3));
        }

        /// <summary>
        /// Tests all nodes replacement with single character.
        /// </summary>
        [Test]
        public void TestAllNodes()
        {
            const string testName = @"Model\Replace2\TestAllNodes.docx";
            Document doc = TestUtil.Open(testName);

            docRangeReplace(doc, ".", "A");
        }

        /// <summary>
        /// Tests case when we cannot remove paragraph break.
        /// </summary>
        [Test]
        public void TestIrreplaceableParagraphBreak()
        {
            const string testName = @"Model\Replace2\TestIrreplaceableParagraphBreak.docx";

            Document doc = TestUtil.Open(testName);

            int count = docRangeReplace(doc, "&p", "");

            // We returns two replacement while actually did only one. Word does the same.
            Assert.That(count, Is.EqualTo(3));

            // First paragraph remains untouched. It cannot be removed.
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests number regex replacement.
        /// </summary>
        [Test]
        public void TestNumberRegex()
        {
            const string testName = @"Model\Replace2\TestNumberRegex.docx";
            Document doc = TestUtil.Open(testName);

            FindReplaceOptions options = new FindReplaceOptions();
            options.ApplyFont.Color = Color.DarkBlue;
            options.ApplyFont.Bold = true;
            options.ApplyFont.Size = 14;

            int count = docRangeReplace(doc, @"\d+", "*", options);

            Assert.That(count, Is.EqualTo(7));
        }

        [Test]
        public void TestNumberHighlightCallback()
        {
            const string testName = @"Model\Replace2\TestNumberHighlightCallback.docx";
            Document doc = TestUtil.Open(testName);

            FindReplaceOptions options = new FindReplaceOptions();
            options.ApplyFont.Bold = true;
            options.ApplyFont.Color = Color.DarkOrange;
            options.ReplacingCallback = new NumberHighlighter();

            int count = docRangeReplace(doc, "[0-9]+", "Whatever", options);

            Assert.That(count, Is.EqualTo(5));
            Assert.That(doc.GetText(), Is.EqualTo("There are three numbers: 35478.0, 999283.0 and 000.0. \rAll four numbers should be conditionally formatted.\f"));
        }

        private class NumberHighlighter : IReplacingCallback
        {
            public ReplaceAction Replacing(ReplacingArgs args)
            {
                if (args.Match.Length > 1)
                {
                    args.Replacement = string.Format("{0}.0", args.Match.Value);
                }
                else
                {
                    args.Replacement = (args.Match.Value == "3") ? "three" : "four";
                }

                return ReplaceAction.Replace;
            }
        }

        /// <summary>
        /// Tests that callback is able to insert paragraph breaks.
        /// </summary>
        [Test]
        public void TestSentenceCounterCallback()
        {
            const string testName = @"Model\Replace2\TestSentenceCounterCallback.docx";
            Document doc = TestUtil.Open(testName);

            FindReplaceOptions options = new FindReplaceOptions();
            options.ReplacingCallback = new SentenceCounter();

            int count = docRangeReplace(doc, "[\\w\\s]+\\.", "Whatever", options);

            Assert.That(count, Is.EqualTo(3));

            ParagraphCollection pc = doc.FirstSection.Body.Paragraphs;

            Assert.That(pc.Count, Is.EqualTo(4));
            Assert.That(pc[0].GetText().StartsWith("1)"), Is.True);
            Assert.That(pc[1].GetText().StartsWith("2)"), Is.True);
            Assert.That(pc[2].GetText().StartsWith("3)"), Is.True);
        }

        /// <summary>
        /// The same as above but tests backward direction.
        /// </summary>
        [Test]
        public void TestSentenceCounterCallbackBackward()
        {
            const string testName = @"Model\Replace2\TestSentenceCounterCallback.docx";
            Document doc = TestUtil.Open(testName);

            FindReplaceOptions options = new FindReplaceOptions();
            options.Direction = FindReplaceDirection.Backward;
            options.ReplacingCallback = new SentenceCounter();

            int count = docRangeReplace(doc, "[\\w\\s]+\\.", "Whatever", options);

            Assert.That(count, Is.EqualTo(3));

            ParagraphCollection pc = doc.FirstSection.Body.Paragraphs;

            Assert.That(pc.Count, Is.EqualTo(4));
            Assert.That(pc[0].GetText().StartsWith("3)"), Is.True);
            Assert.That(pc[1].GetText().StartsWith("2)"), Is.True);
            Assert.That(pc[2].GetText().StartsWith("1)"), Is.True);
        }

        private class SentenceCounter : IReplacingCallback
        {
            public ReplaceAction Replacing(ReplacingArgs args)
            {
                args.Replacement = string.Format("{0}) {1}&p", mCount++, args.Match.Value.Trim());

                return ReplaceAction.Replace;
            }

            private int mCount = 1;
        }

        /// <summary>
        /// Tests that DocumentBuilder can write safely during replace operation.
        /// </summary>
        [Test]
        public void TestBuilderIsolation()
        {
            const string testName = @"Model\Replace2\TestBuilderIsolation.docx";
            Document doc = TestUtil.Open(testName);

            FindReplaceOptions options = new FindReplaceOptions();
            options.ReplacingCallback = new BuilderWriter();

            int count = docRangeReplace(doc, "[0-9]+", "[was-number]", options);

            // Only 2 numbers are replaced. Written by DocumentBuilder is isolated.
            Assert.That(count, Is.EqualTo(2));
        }

        private class BuilderWriter : IReplacingCallback
        {
            public ReplaceAction Replacing(ReplacingArgs args)
            {
                DocumentBuilder builder = new DocumentBuilder(args.MatchNode.FetchDocument());
                builder.MoveToDocumentStart();
                builder.Font.Color = Color.Red;
                builder.Writeln(string.Format("DocumentBuilder writes: {0}...", mCount++));


                builder.MoveToDocumentEnd();
                builder.Font.Color = Color.Green;
                builder.Writeln(string.Format("DocumentBuilder writes: {0}...", mCount++));

                return ReplaceAction.Replace;
            }

            private int mCount = 1;
        }

        /// <summary>
        /// WORDSNET-13138 Document.Range.Replace throws exception when isForward is set to true
        /// </summary>
        [Test]
        public void TestJira13138()
        {
            const string testName = @"Model\Replace2\TestJira13138.docx";

            // Test both directions.
            foreach (FindReplaceDirection direction in new FindReplaceDirection[] { FindReplaceDirection.Forward, FindReplaceDirection.Backward })
            {
                Document doc = TestUtil.Open(testName);

                int count = docRangeReplace(doc, "Test", "", new FindReplaceOptions(direction, new Callback13138()));
                Assert.That(count, Is.EqualTo(1));
                Assert.That(doc.GetText(), Is.EqualTo("\r\r\r\r\r\rTest value\f"));
            }
        }

        private class Callback13138 : IReplacingCallback
        {
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
            {
                DocumentBuilder builder = new DocumentBuilder(e.MatchNode.Document as Document);
                builder.MoveTo((Run)e.MatchNode);
                builder.Write("Test value");
                e.Replacement = "";
                return ReplaceAction.Replace;
            }
        }

        /// <summary>
        /// WORDSNET-12954 Document.Range.Replace is throwing exception when isForward is set to true
        /// </summary>
        [Test]
        public void TestJira12954()
        {
            const string testName = @"Model\Replace2\TestJira12954.docx";

            // Test both directions.
            foreach (FindReplaceDirection direction in gDirections)
            {
                Document doc = TestUtil.Open(testName);

                FindReplaceOptions options = new FindReplaceOptions(direction, new Callback12954());

                int count = docRangeReplace(doc, "%([A-Z0-9_]{3,30})%", "", options);
                Assert.That(count, Is.EqualTo(1));

                Assert.That(doc.Range.Bookmarks.Count, Is.EqualTo(1));
                Assert.That(doc.Range.Bookmarks[0].Name, Is.EqualTo("%MODEL%"));
            }
        }

        private class Callback12954 : IReplacingCallback
        {
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
            {
                DocumentBuilder builder = new DocumentBuilder(e.MatchNode.Document as Document);
                builder.MoveTo((Run)e.MatchNode);
                string bookmarkName = e.Match.Value;
                builder.StartBookmark(bookmarkName);
                builder.EndBookmark(bookmarkName);
                e.Replacement = "";
                return ReplaceAction.Replace;
            }
        }

        /// <summary>
        /// WORDSNET-13046 Range.Replace throws System.NotSupportedException
        /// </summary>
        [Test]
        public void TestJira13046()
        {
            Document doc = TestUtil.Open(@"Model\Replace2\TestJira13046.docx");

            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(6));

            int count = docRangeReplace(doc, "[a-z][^\\.;]*(Bente)[^\\.;]*", "HELLO"); // Fails

            Assert.That(count, Is.EqualTo(1));
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(4));
        }

        /// <summary>
        /// WORDSNET-12078 Range.Replace method throws System.NotSupportedException
        /// </summary>
        [Test]
        public void TestJira12078()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Hello This a test String. Guidance \n dnvnhjlmnmvhctd  ajjdkb ;.nnsh EndGuidance.;;';lp Hello Tis a test String. Guidance dnvnhjlmnmvhctd  ajjdkb ;.nnsh EndGuidance");

            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(3));

            const string strPattern = "(?s)Guidance.*?EndGuidance";

            int count = docRangeReplace(doc, strPattern, "");

            Assert.That(count, Is.EqualTo(2));
            Assert.That(doc.FirstSection.Body.Paragraphs.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// Tests that callback returned action is correctly processed.
        /// </summary>
        [Test]
        public void TestSkipAllCallback()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Hello world.");

            int count = docRangeReplace(doc, ".", "*", new FindReplaceOptions(new SkipAllCallback()));

            Assert.That(count, Is.EqualTo(0));
            Assert.That(doc.GetText(), Is.EqualTo("Hello world.\r\f"));
        }

        private class SkipAllCallback : IReplacingCallback
        {
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
            {
                return ReplaceAction.Skip;
            }
        }

        /// <summary>
        /// Tests that callback returned action is correctly processed.
        /// </summary>
        [Test]
        public void TestEmptyCallback()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("Hello world.");

            int count = docRangeReplace(doc, ".", "*", new FindReplaceOptions(new EmptyCallback()));

            Assert.That(count, Is.EqualTo(14));
            Assert.That(doc.GetText(), Is.EqualTo("**************\f"));
        }

        private class EmptyCallback : IReplacingCallback
        {
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
            {
                return ReplaceAction.Replace;
            }
        }

        /// <summary>
        /// Tests arguments passed to callback.
        /// </summary>
        [Test]
        public void TestTestingCallback()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("123 or 456.");

            FindReplaceOptions options = new FindReplaceOptions();
            options.ReplacingCallback = new TestingCallback();

            int count = docRangeReplace(doc, "[0-9]+", "*", options);

            Assert.That(count, Is.EqualTo(2));
            Assert.That(doc.GetText(), Is.EqualTo("* or *.\r\f"));
        }

        private class TestingCallback : IReplacingCallback
        {
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
            {
                Assert.That(e.Replacement, Is.EqualTo("*"));
                Assert.That((e.Match.Value == "123") || (e.Match.Value == "456"), Is.True);
                Assert.That((e.MatchOffset == 0) || (e.MatchOffset == 4), Is.True);

                return ReplaceAction.Replace;
            }
        }

        /// <summary>
        /// Tests Wiki demonstration sample.
        /// </summary>
        /// <remarks>
        /// Demonstrates callback usage.
        /// Numbers matched by regular expression are converted to HEX presentation and highlighted.
        /// </remarks>
        [Test]
        public void TestWiki02()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Name = "Arial";
            builder.Write("There are few numbers that should be converted to HEX and highlighted: 123, 456, 789 and 17379.");

            FindReplaceOptions options = new FindReplaceOptions();
            options.ApplyFont.HighlightColor = Color.DarkOrange;
            options.ReplacingCallback = new NumberHexer();

            int count = docRangeReplace(doc, "[0-9]+", "", options);
            Assert.That(count, Is.EqualTo(4));
        }

        private class NumberHexer : IReplacingCallback
        {
            public ReplaceAction Replacing(ReplacingArgs args)
            {
                int number = Convert.ToInt32(args.Match.Value);
                args.Replacement = string.Format("0x{0:X}", number);

                return ReplaceAction.Replace;
            }
        }

        /// <summary>
        /// Tests code sample from public API comment.
        /// </summary>
        [Test]
        public void TestCodeSample_2()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Writeln("a1, b2, c3");

            // Replaces each number with paragraph break.
            docRangeReplace(doc, @"\d+", "&p");

            Assert.That(doc.GetText(), Is.EqualTo("a\r, b\r, c\r\r\f"));
        }

        /// <summary>
        /// Relates to WORDSNET-14046
        /// Tests that content cell break character replacement is correctly handled.
        /// </summary>
        [Test]
        public void TestJira15046B()
        {
            Document doc = TestUtil.Open(@"Model\Replace2\TestJira15046A.docx");

            // No replacement should be made.
            int count = docRangeReplace(doc, ".", "XX");
            Assert.That(count, Is.EqualTo(5));
        }

        /// <summary>
        /// WORDSNET-15177 Range.Replace method does not replace text in footnote
        /// Footnotes were excluded from find/replace processing.
        /// </summary>
        [Test]
        public void TestJira15177()
        {
            Document doc = TestUtil.Open(@"Model\Replace2\TestJira15177.docx");

            FindReplaceOptions options = new FindReplaceOptions();
            options.ApplyFont.HighlightColor = Color.Yellow;

            options.ReplacingCallback = new ReplaceEvaluatorFindAndHighlight();
            options.Direction = FindReplaceDirection.Backward;
            // We want the "your document" phrase to be highlighted.
            docRangeReplace(doc, "(?i)[a-z]+", "", options);

            Footnote footnote = (Footnote)doc.FirstSection.Body.GetChild(NodeType.Footnote, 0, true);

            Run run = footnote.FirstParagraph.Runs[1];
            Assert.That(run.Text, Is.EqualTo("Lorem"));
            Assert.That(((DrColor)run.RunPr[FontAttr.HighlightColor]).ToArgb(), Is.EqualTo(DrColor.Yellow.ToArgb()));
        }

        private class ReplaceEvaluatorFindAndHighlight : IReplacingCallback
        {
            public ReplaceAction Replacing(ReplacingArgs args)
            {
                if (args.Match.Value.Contains("o"))
                {
                    args.Replacement = args.Match.Value;
                    return ReplaceAction.Replace;
                }
                else
                {
                    return ReplaceAction.Skip;
                }
            }
        }

        /// <summary>
        /// WORDSNET-15624 Tests that headers/footers are processed in order of their visual
        /// representation in a document.
        /// </summary>
        [Test]
        public void TestHeaderFooterOrder()
        {
            Document doc = TestUtil.Open(@"Model\Replace2\TestJira15624.docx");

            FindReplaceOptions options = new FindReplaceOptions();
            MatchLogger logger = new MatchLogger();
            options.ReplacingCallback = logger;

            docRangeReplace(doc, "(header|footer)",  "", options);

            Assert.That(logger.Text, Is.EqualTo("First header" + Environment.NewLine +
                            "First footer" + Environment.NewLine +
                            "Second header" + Environment.NewLine +
                            "Second footer" + Environment.NewLine +
                            "Third header" + Environment.NewLine +
                            "Third footer" + Environment.NewLine));
        }


        /// <summary>
        /// WORDSNET-16217 FindReplaceOptions.FindWholeWordsOnly does not replace standalone word.
        /// Added a word's boundaries determination.
        /// </summary>
        [Test]
        public void TestJira16217()
        {
            Document doc = new Document();
            DocumentBuilder docBuilder = new DocumentBuilder(doc);

            docBuilder.Writeln("$IMPORT_TEXT");
            docBuilder.Writeln("$IMPORT_TEXTAREA");

            FindReplaceOptions findReplaceOptions = new FindReplaceOptions();
            findReplaceOptions.FindWholeWordsOnly = true;

            docRangeReplace(doc, @"\$IMPORT_TEXT", "testing", findReplaceOptions);
            docRangeReplace(doc, @"\$IMPORT_TEXTAREA", "textarea", findReplaceOptions);

            NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);
            Assert.That(((Run)runs[0]).Text, Is.EqualTo("testing"));
            Assert.That(((Run)runs[1]).Text, Is.EqualTo("textarea"));
        }

        private class MatchLogger : IReplacingCallback
        {
            public ReplaceAction Replacing(ReplacingArgs e)
            {
                mText.AppendLine(e.MatchNode.GetText());
                return ReplaceAction.Skip;
            }

            internal string Text
            {
                get { return mText.ToString(); }
            }

            private readonly StringBuilder mText = new StringBuilder();
        }

        [Test]
        public void TestReplaceDirection()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceDirection.docx");

            mCounter = 0;
            //Backward
            Node para = doc.SelectSingleNode("//Paragraph[1]");
            para.Range.Replace(new Regex("A"), "", new FindReplaceOptions(FindReplaceDirection.Backward, new ReplaceEvaluator1(this)));
            Assert.That(para.GetText(), Is.EqualTo("A4 A3 A2 A1 A0\r"));

            mCounter = 0;
            //Forward
            para = doc.SelectSingleNode("//Paragraph[2]");
            para.Range.Replace(new Regex("A"), "", new FindReplaceOptions(FindReplaceDirection.Forward, new ReplaceEvaluator1(this)));
            Assert.That(para.GetText(), Is.EqualTo("A0 A1 A2 A3 A4\r"));
        }

        private class ReplaceEvaluator1 : IReplacingCallback
        {
            public ReplaceEvaluator1(TestReplaceRegex context)
            {
                mContext = context;
            }

            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
            {
                //Also shows how to add something to the text, not just replace.
                e.Replacement = e.Match.Value + mContext.mCounter.ToString();
                mContext.mCounter++;
                return ReplaceAction.Replace;
            }

            private readonly TestReplaceRegex mContext;
        }

        [Test]
        public void TestReplaceAcrossParagraph()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceAcrossParent.docx");
            //Note the way of capturing a paragraph break without specifying it explicitly.
            int replacementCount = docRangeReplace(doc, "Para1.Para2", "");
            Assert.That(replacementCount, Is.EqualTo(1));
        }

        public void TestReplaceAcrossParagraphUnescaped()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceAcrossParent.docx");
            //There is a way to pass unescaped \r, it is also caught.
            int replacementCount = docRangeReplace(doc, "Para1\rPara2", "");
            Assert.That(replacementCount, Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-614 This used to throw object reference not set.
        /// </summary>
        [Test]
        public void TestDefect614()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestDefect614.docx");
            Cell cell = (Cell)doc.GetChild(NodeType.Cell, 0, true);
            cell.Range.Replace(new Regex(".*[^\x07]"), "");
            Assert.That(cell.Range.Text, Is.EqualTo("\x0007"));
        }

        /// <summary>
        /// Replacing a particular string with a table.
        /// </summary>
        [Test]
        public void TestReplaceWithTable()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestReplaceWithTable.docx");

            //Note I have to pass "isForward = false", otherwise such a complex replace will not work.
            docRangeReplace(doc, "Tag", "", new FindReplaceOptions(FindReplaceDirection.Backward, new HandleReplaceWithTable()));

            TestUtil.Save(doc, @"Model\Replace\TestReplaceWithTable.docx");

            Assert.That(doc.Range.Text, Is.EqualTo("Hello\x0007\x0007" +
                "Other text\r" +
                "Hello\x0007\x0007" +
                "\x000c"));
        }

        private class HandleReplaceWithTable : IReplacingCallback
        {
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
            {
                //The node that contains text should be a Run.
                Run run = (Run)e.MatchNode;

                //We can only insert a table at the "block" level, that is among paragraphs and tables.
                //Step up to the paragraph node that contains the run with the match text.
                Paragraph para = run.ParentParagraph;

                //This is the table we are going to insert.
                //Note you can use DocumentBuilder instead of building node by node,
                //it could be easier to build a table with DocumentBuilder.
                Table newTable = CreateTestTable(para.Document);

                //Ask the parent of the paragraph to insert the new table after this paragraph.
                para.InsertNext(newTable);

                //In fact I want the old paragraph to be removed.
                para.Remove();

                //Let the replace continue.
                e.Replacement = "";
                return ReplaceAction.Replace;
            }
        }

        private static Table CreateTestTable(DocumentBase doc)
        {
            //Create a table with one cell.
            Table table = new Table(doc);
            table.EnsureMinimum();

            Cell cell = table.FirstRow.FirstCell;
            cell.CellFormat.Width = 144;

            //Add some text.
            Run run = new Run(doc, "Hello");
            cell.FirstParagraph.AppendChild(run);

            return table;
        }

        /// <summary>
        /// Checks several replacements in one run when forward direction is used.
        /// </summary>
        [Test]
        public void TestTwoReplacementInOneRun()
        {
            DocumentBuilder builder = new DocumentBuilder();
            Document doc = builder.Document;
            builder.Write("ol");
            builder.Writeln("d old old");

            NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);
            Debug.Assert(runs.Count == 2);
            Debug.Assert(((Run)runs[0]).Text == "ol");

            FindReplaceOptions options = new FindReplaceOptions()
            {
                Direction = FindReplaceDirection.Forward,
                ReplacingCallback = null
            };
            FindReplace findReplace = new FindReplace(doc, new Regex("old"), "new text", options);
            findReplace.Replace();

            string expectedText = "new text new text new text";
            Assert.That(doc.GetText().Substring(0, expectedText.Length), Is.EqualTo(expectedText));
        }

        /// <summary>
        /// WORDSNET-12530 ArgumentOutOfRangeException may occur during replacement when forward
        /// direction is used and length of replcement string is different than length of searching string.
        /// </summary>
        [Test]
        public void TestJira12530()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestJira12530.docx");

            docRangeReplace(doc, @"(?i)<\*.+?(Counter)\*>", "", new FindReplaceOptions(FindReplaceDirection.Forward, new TestJira12530ReplacingCallback()));

            string expectedText = "1     \r2     \r3     \r4     \r5     \r6     \r7     \r8     \f";
            Assert.That(doc.GetText(), Is.EqualTo(expectedText));
        }

        private class TestJira12530ReplacingCallback : IReplacingCallback
        {
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
            {
                switch (e.Match.ToString())
                {
                    case "<*NumberCounter*>":
                    {
                        e.Replacement = mSectionNumberCounter.ToString();
                        mSectionNumberCounter++;
                        return ReplaceAction.Replace;
                    }
                    default:
                    {
                        return ReplaceAction.Skip;
                    }
                }
            }

            private int mSectionNumberCounter = 1;
        }



        /// <summary>
        /// Relates to WORDSNET-18986 Checks for replacement in SmartTag with para.
        /// </summary>
        [Test]
        public void Test18986ReplaceSmartTagWithPara()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test18986ReplaceSmartTagWithPara.docx");

            int replaceCount = docRangeReplace(doc, @"\[(.*?)\]", @"Before&pAfter", new FindReplaceOptions());
            Assert.That(replaceCount, Is.EqualTo(4));
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras.Count, Is.EqualTo(6));

            Assert.That(paras[0].GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(paras[0].FirstRun.Text, Is.EqualTo("Before"));

            Assert.That(paras[0].GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(paras[1].FirstRun.Text, Is.EqualTo("After"));

            Assert.That(paras[2].GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(paras[2].FirstRun.Text, Is.EqualTo("Before"));

            Assert.That(paras[3].GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(4));
            Assert.That(paras[3].Runs[0].Text, Is.EqualTo("After"));
            Assert.That(paras[3].Runs[1].ParentNode.NodeType, Is.EqualTo(NodeType.SmartTag));
            Assert.That(paras[3].Runs[1].Text, Is.EqualTo("MustBeInSmartTag"));
            Assert.That(paras[3].Runs[2].Text, Is.EqualTo("WP_SMT_SMT_WR2"));
            Assert.That(paras[3].Runs[3].Text, Is.EqualTo("Before"));

            Assert.That(paras[4].GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
            Assert.That(paras[4].Runs[0].Text, Is.EqualTo("After"));
            Assert.That(paras[4].Runs[1].Text, Is.EqualTo("Before"));

            Assert.That(paras[5].GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(paras[5].FirstRun.Text, Is.EqualTo("After"));
        }

        /// <summary>
        /// Relates to WORDSNET-18986 Checks for replacement in SmartTag.
        /// </summary>
        [Test]
        public void Test18986ReplaceSmartTagA()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test18986ReplaceSmartTag.docx");

            int replaceCount = docRangeReplace(doc, @"\[(.*?)\]", @"Test", new FindReplaceOptions());
            Assert.That(replaceCount, Is.EqualTo(8));

            NodeCollection smartTags = doc.FirstSection.Body.GetChildNodes(NodeType.SmartTag, true);
            Assert.That(smartTags.Count, Is.EqualTo(2));
        }

        /// <summary>
        /// WORDSNET-19357 Difference in Order between the now and old deprecated Replace methods.
        /// The new Replace method searches for text in text boxes after the first search-pass in a document -
        /// mimics Word. Added option.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void Test19357(bool useLegacyOrder)
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test19357.docx");

            Test19357ReplacingCallback callback = new Test19357ReplacingCallback();
            FindReplaceOptions options = new FindReplaceOptions();
            options.ReplacingCallback = callback;

            // Checks the default value.
            Assert.That(options.UseLegacyOrder, Is.False);
            options.UseLegacyOrder = useLegacyOrder;

            docRangeReplace(doc, @"\[(.*?)\]", "", options);

            if (useLegacyOrder)
            {
                Assert.That(callback.Matches[0], Is.EqualTo("[tag 1]"));
                Assert.That(callback.Matches[1], Is.EqualTo("[tag 2]"));
                Assert.That(callback.Matches[2], Is.EqualTo("[tag 3]"));
            }
            else
            {
                Assert.That(callback.Matches[0], Is.EqualTo("[tag 1]"));
                Assert.That(callback.Matches[1], Is.EqualTo("[tag 3]"));
                Assert.That(callback.Matches[2], Is.EqualTo("[tag 2]"));
            }
        }

        private class Test19357ReplacingCallback : IReplacingCallback
        {
            ReplaceAction IReplacingCallback.Replacing(ReplacingArgs e)
            {
                mMatches.Add(e.Match.Value);
                return ReplaceAction.Replace;
            }

            public List<string> Matches { get { return mMatches; } }

            private readonly List<string> mMatches = new List<string>();
        }

        /// <summary>
        /// WORDSNET-19913 Range.Replace with substitution.
        /// The new <see cref="FindReplaceOptions.UseSubstitutions"/> is introduced.
        /// We should calculate substituted result from the replacing string before passing it
        /// to the replacing routine for every match when the option is enabled.
        /// </summary>
        [TestCase(true, "Paul take money from Jason\f")]
        [TestCase(false, "$2 take money from $1\f")]
        public void Test19913(bool isRecognizeSubstitutions, string expectedContent)
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test19913.docx");

            FindReplaceOptions options = new FindReplaceOptions();
            options.UseSubstitutions = isRecognizeSubstitutions;
            docRangeReplace(doc, @"([A-z]+) give money to ([A-z]+)", @"$2 take money from $1", options);

            Assert.That(doc.FirstSection.Body.FirstParagraph.GetText(), Is.EqualTo(expectedContent));
        }

        /// <summary>
        /// WORDSNET-19912 FindReplaceOptions new properties.
        /// Implemented new option <see cref="FindReplaceOptions.IgnoreDeleted"/>.
        /// </summary>
        [TestCase(false, "d*l*t*d")]
        [TestCase(true, "deleted")]
        public void Test19912DeleteRevision(bool isIgnoreDeleted, string expectedDeletedText)
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test19912Revisions.docx");

            FindReplaceOptions options = new FindReplaceOptions();
            options.IgnoreDeleted = isIgnoreDeleted;

            docRangeReplace(doc, "e", "*", options);

            string expectedText = string.Format("t*xt {0} ins*rt*d t*xt\f", expectedDeletedText);
            Assert.That(doc.FirstSection.Body.FirstParagraph.GetText(), Is.EqualTo(expectedText));
        }

        /// <summary>
        /// WORDSNET-19912 FindReplaceOptions new properties.
        /// Implemented new option <see cref="FindReplaceOptions.IgnoreInserted"/>.
        /// </summary>
        [TestCase(false, "ins*rt*d t*xt")]
        [TestCase(true, "inserted text")]
        public void Test19912InsertRevision(bool isIgnoreInserted, string expectedInsertedText)
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test19912Revisions.docx");

            FindReplaceOptions options = new FindReplaceOptions();
            options.IgnoreInserted = isIgnoreInserted;

            docRangeReplace(doc, "e", "*", options);

            string expectedText = string.Format("t*xt d*l*t*d {0}\f", expectedInsertedText);
            Assert.That(doc.FirstSection.Body.FirstParagraph.GetText(), Is.EqualTo(expectedText));
        }


        /// <summary>
        /// WORDSJAVA-2546 com.aspose.ms.System.Text.RegularExpressions.Match.result() returns
        /// correct value plus some extra symbols when regular expressions with substitutions are used.
        /// </summary>
        [Test]
        public void TestJava2546()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("world");
            builder.Writeln("world");

            FindReplaceOptions options = new FindReplaceOptions();
            options.UseSubstitutions = true;

            doc.Range.Replace(new Regex("(world)"), "Hello $1!", options);

            Assert.That(doc.GetText().Trim(), Is.EqualTo("Hello world!\rHello world!"));
        }

        /// <summary>
        /// WORDSJAVA-2455 "useSubstitutions" test doesn't replace text
        /// </summary>
        [Test]
        public void TestJava2455()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Writeln("John sold a car to Paul.");
            builder.Writeln("Jane sold a house to Joe.");

            FindReplaceOptions options = new FindReplaceOptions();
            options.UseSubstitutions = true;

            docRangeReplace(doc, "([A-z]+) sold a ([A-z]+) to ([A-z]+)", "$3 bought a $2 from $1", options);

            Assert.That(doc.GetText().Trim(), Is.EqualTo("Paul bought a car from John.\rJoe bought a house from Jane."));
        }

        /// <summary>
        /// WORDSNET-22179 Range.Replace hangs after upgrading to 21.4.
        /// The issue is caused by WORDSJAVA-2455, where we have started to get matches from both escaped
        /// and unescaped patterns. Solved by reverting WORDSJAVA-2455, as string patterns are not regexes
        /// and therefore should be always escaped.
        /// </summary>
        [Test]
        public void Test22179()
        {
            Test22179ThreadWorker worker = new Test22179ThreadWorker();
            TestUtil.IsHanging(worker, 5);
        }

        /// <summary>
        /// WORDSNET-22200 Regression: Range.Replace is causing memory leak.
        /// The issue duplicates WORDSNET-22179
        /// </summary>
        [Test]
        public void Test22200()
        {
            Test22200ThreadWorker worker = new Test22200ThreadWorker();
            TestUtil.IsHanging(worker, 5);
        }

        /// <summary>
        /// WORDSNET-22632 Find and Replace does not work with delete revisions.
        /// Before the fix we ignored whole deleted/inserted nodes, so that if the node is composite,
        /// then all its child nodes were also ignored, even they have no any revisions.
        /// The behavior is changed to not skip child nodes in such cases for now.
        /// </summary>
        [Test]
        public void Test22632DeleteRevision()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test22632Del.docx");
            Assert.That(doc.FirstSection.Body.FirstParagraph.IsDeleteRevision, Is.True);
            FindReplaceOptions options = new FindReplaceOptions { IgnoreDeleted = true };
            Test22632Core(doc, options);
        }

        /// <summary>
        /// Relates to WORDSNET-22632 Checks insertion revision.
        /// </summary>
        [Test]
        public void Test22632InsertRevision()
        {
            Document doc = TestUtil.Open(@"Model\Replace\Test22632Ins.docx");
            Assert.That(doc.FirstSection.Body.FirstParagraph.IsInsertRevision, Is.True);

            FindReplaceOptions options = new FindReplaceOptions { IgnoreInserted = true };
            Test22632Core(doc, options);
        }


        /// <summary>
        /// WORDSNET-23428 Range.Replace does not throw RegexMatchTimeoutException.
        /// We lost the <see cref="Regex.MatchTimeout"/> property when re-created private
        /// regex object in constructor of <see cref="FindReplace"/>.
        /// </summary>
        [Test]
        [CppSkipEntity("C++ doesn't support RegexMatchTimeoutException")]
        [JavaDelete("Java doesn't support RegexMatchTimeoutException")]
        [NetStandardIgnore("In .NET Standard test forces test runner to stop. Skip Test for now.")]
        public void Test23428()
        {
            Test23428ThreadWorker worker = new Test23428ThreadWorker();
            TestUtil.IsHanging(worker, 5);

            Assert.That(worker.ExceptionType, Is.EqualTo(typeof(RegexMatchTimeoutException)));
        }



        /// <summary>
        /// WORDSJAVA-2910 - Incorrect behavior of regular expression while searching a sentence ending by a dot.
        /// </summary>
        [Test]
        public void TestJava2910()
        {
            Document doc = TestUtil.Open(@"Model\Replace\TestJava2910.docx");

            FindReplaceOptions boldFontOptions = new FindReplaceOptions();
            boldFontOptions.ApplyFont.Bold = true;
            boldFontOptions.UseSubstitutions = true;

            const string pattern = @"^[^\.]+?\.";
            const string replacement = "$0";

            docRangeReplace(doc, pattern, replacement, boldFontOptions);

            TestUtil.SaveCheckGold(doc, @"Model\Replace\TestJava2910.docx");
        }

        /// <summary>
        /// Tests SpecialChar update when replacing in LegacyMode.
        /// </summary>
        [Test]
        public void TestReplaceCommentLegacy()
        {
            Document doc = TestUtil.Open(@"Model\Replace2\TestCommentText.docx");

            FindReplaceOptions options = new FindReplaceOptions();
            options.LegacyMode = true;

            // Checks System.InvalidCastException here.
            int updateCount = doc.Range.Replace(new Regex("[a-zA-Z\x05]"), "*", options);
            Assert.That(updateCount, Is.EqualTo(16));
        }

        /// <summary>
        /// A worker thread for <see cref="Test23428"/>.
        /// </summary>
        [CppSkipEntity("C++ doesn't support RegexMatchTimeoutException")]
        [JavaDelete("Java doesn't support RegexMatchTimeoutException")]
        private class Test23428ThreadWorker : ThreadPal
        {
            [ExpectedException(typeof(RegexMatchTimeoutException))]
            protected override void DoWork()
            {
                try
                {
                    Document doc = TestUtil.Open(@"Model\Replace\Test23428.docx");

                    const string pattern =
                        @"<<MAIN(?'ItemProperties'[:](<<(?<LEVEL>)|>>(?<-LEVEL>)|(?!<<|>>).*?)+(?(LEVEL)(?!)))?>>.*?<<MAIN_END>>";
                    // Customer's scenario to limit regex execution by passing time span into ctor of regex.
                    Regex rgx = new Regex(pattern, RegexOptions.None, TimeSpan.FromSeconds(1));
                    doc.Range.Replace(rgx, string.Empty);
                }
                catch (RegexMatchTimeoutException)
                {
                    ExceptionType = typeof(RegexMatchTimeoutException);
                    throw;
                }
            }

            internal Type ExceptionType { get; private set; }
        }

        /// <summary>
        /// The core test method for WORDSNET-22632
        /// </summary>
        private static void Test22632Core(Document doc, FindReplaceOptions options)
        {
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Run run = para.FirstRun;
            Assert.That(run.Text, Is.EqualTo("First line has a "));
            Assert.That(run.IsDeleteRevision, Is.False);
            Assert.That(run.IsInsertRevision, Is.False);

            docRangeReplace(doc, "First", "REPLACED", options);

            Assert.That(para.GetText(), Is.EqualTo("REPLACED line has a insertion\r"));
        }

        /// <summary>
        /// A worker thread for Test22200.
        /// </summary>
        private class Test22200ThreadWorker : ThreadPal
        {
            protected override void DoWork()
            {
                Document doc = TestUtil.Open(@"Model\Replace\Test22200.docx");

                // This is scenario of the customer. I did not optimize anything in this code intentionally.
                doc.Range.Replace("[projectname]", "sdfsdf");
                doc.Range.Replace("[chargeCodes]", "sdfsdf");
                doc.Range.Replace("[resort]", "sdfsdf");
                doc.Range.Replace("[service]", "sdfsdf");
                doc.Range.Replace("[businesssponsor]", "sdfsdf");
                doc.Range.Replace("[tpManager]", "sdfsdf");
                doc.Range.Replace("[projectManager]", "sdfsdf");
                doc.Range.Replace("[changeIDs]", "sdfsdf / asdasd / asdasd /a dasd a/Asda s/Asda / asda sda/ asdasd");
                doc.Range.Replace("[proposalFolderLink]", "https://news.google.com/topstories?hl=de&gl=DE&ceid=DE:de");
                doc.Range.Replace("[serviceRequestLink]", "https://news.google.com/topstories?hl=de&gl=DE&ceid=DE:de");
                doc.Range.Replace("[projectSpaceLink]", "https://news.google.com/topstories?hl=de&gl=DE&ceid=DE:de");

                doc = TestUtil.SaveOpen(doc, @"Model\Replace\Test22200.docx", new OoxmlSaveOptions(), false);

                // Check all values were successfully replaced.
                string docText = doc.GetText();
                Assert.That(docText.Contains("[projectname]"), Is.False);
                Assert.That(docText.Contains("[resort]"), Is.False);
                Assert.That(docText.Contains("[service]"), Is.False);
                Assert.That(docText.Contains("[businesssponsor]"), Is.False);
                Assert.That(docText.Contains("[tpManager]"), Is.False);
                Assert.That(docText.Contains("[projectManager]"), Is.False);
                Assert.That(docText.Contains("[changeIDs]"), Is.False);
                Assert.That(docText.Contains("[proposalFolderLink]"), Is.False);
                Assert.That(docText.Contains("[serviceRequestLink]"), Is.False);
                Assert.That(docText.Contains("[projectSpaceLink]"), Is.False);
            }
        }

        /// <summary>
        /// A worker thread for Test22179.
        /// </summary>
        private class Test22179ThreadWorker : ThreadPal
        {
            protected override void DoWork()
            {
                string[] patterns = new[]
                {
                    "[Patient Name]", "[Date of birth]", "[Email]", "[X Number]", "[Telephone]", "[Enquiry Timestamp]",
                    "[Financial Class]", "[Status]", "[Comment]", "[Source]"
                };
                string[] replacement = new[]
                {
                    "Mrs TestDB  TestAttachments ", " ", "658018@testtest.com", "3g3g", "75983410", "28/01/2021 19:47",
                    "Insured", "Open", "She says she isn't feeling well and vwould like to see a Dr.", "Self Referral"
                };

                Assert.That(replacement.Length, Is.EqualTo(patterns.Length));

                Document doc = TestUtil.Open(@"Model\Replace\Test22179.docx");

                int count = 0;
                for (int i = 0; i < patterns.Length; i++)
                {
                    count += doc.Range.Replace(patterns[i], replacement[i]);
                }

                Assert.That(count, Is.EqualTo(0));
            }
        }

        private static int docRangeReplace(Document doc, string regexStr, string replacement)
        {
            return docRangeReplace(doc, regexStr, replacement, new FindReplaceOptions());
        }

        [JavaThrows(true)]
        private static int docRangeReplace(Document doc, string regexStr, string replacement, FindReplaceOptions options)
        {
#if JAVA
            java.util.regex.Pattern regex = java.util.regex.Pattern.compile(regexStr);
            return doc.getRange().replace(regex, replacement, options);
#else
            Regex regex = new Regex(regexStr);
            return doc.Range.Replace(regex, replacement, options);
#endif
        }

        private int mCounter;
        private static readonly FindReplaceDirection[] gDirections = { FindReplaceDirection.Forward, FindReplaceDirection.Backward };
    }
}
