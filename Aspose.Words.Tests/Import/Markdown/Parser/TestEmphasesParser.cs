// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/09/2019 by Ilya Navrotskiy

using Aspose.Words.RW.Markdown;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Markdown.Parser
{
    /// <summary>
    /// Tests parsing of markdown Emphases feature.
    /// </summary>
    public class TestEmphasesParser : TestMarkdownParserBase
    {
        /// <summary>
        /// Tests a flanking type of delimiter run.
        /// </summary>
        [Test]
        public void TestDelimiterFlanking()
        {
            CheckDelimiterRun("***abc", FlankingType.Left, "***");
            CheckDelimiterRun("  _abc", FlankingType.Left, "_");
            CheckDelimiterRun("**\"abc\"", FlankingType.Left, "**");
            CheckDelimiterRun(" _\"abc\"", FlankingType.Left, "_");

            CheckDelimiterRun("abc***", FlankingType.Right, "***");
            CheckDelimiterRun("abc_", FlankingType.Right, "_");
            CheckDelimiterRun("\"abc\"**", FlankingType.Right, "**");
            CheckDelimiterRun("\"abc\"_", FlankingType.Right, "_");

            CheckDelimiterRun("abc***def", FlankingType.Both, "***");
            CheckDelimiterRun("\"abc\"_\"def\"", FlankingType.Both, "_");

            CheckDelimiterRun("abc *** def", FlankingType.None, "***");
            CheckDelimiterRun("a _ b", FlankingType.None, "_");
        }

        /// <summary>
        /// Tests simple emphasis.
        /// </summary>
        [Test]
        public void TestEmphasisA()
        {
            MarkdownDocument doc = Open("TestEmphasisA.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo bar*")});
        }

        /// <summary>
        /// Tests this is not emphasis, because the opening * is followed by whitespace,
        /// and hence not part of a left-flanking delimiter run.
        /// </summary>
        [Test]
        public void TestEmphasisB()
        {
            MarkdownDocument doc = Open("TestEmphasisB.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "a * foo bar*")});
        }

        /// <summary>
        /// Tests this is not emphasis, because the opening * is preceded by an alphanumeric and followed by punctuation,
        /// and hence not part of a left-flanking delimiter run
        /// </summary>
        [Test]
        public void TestEmphasisC()
        {
            MarkdownDocument doc = Open("TestEmphasisC.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "a*\"foo\"*");
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "a*\"foo\"*")});
        }

        /// <summary>
        /// Tests intraword emphasis with * is permitted.
        /// </summary>
        [Test]
        public void TestEmphasisD()
        {
            MarkdownDocument doc = Open("TestEmphasisD.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckParagraph(doc[0], "foo*bar*");
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "foo"), new ExpectedInline(BlockType.ItalicInline, "*bar*")});
        }

        /// <summary>
        /// Tests intraword emphasis with * is permitted.
        /// </summary>
        [Test]
        public void TestEmphasisE()
        {
            MarkdownDocument doc = Open("TestEmphasisE.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0],
                new [] {new ExpectedInline(BlockType.Inline, "5"), new ExpectedInline(BlockType.ItalicInline, "*6*"), new ExpectedInline(BlockType.Inline, "78")});
        }

        /// <summary>
        /// Tests simple emphasis with underscore.
        /// </summary>
        [Test]
        public void TestEmphasisF()
        {
            MarkdownDocument doc = Open("TestEmphasisF.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo bar*")});
        }

        /// <summary>
        /// Tests this is not emphasis, because the opening _ is followed by whitespace.
        /// </summary>
        [Test]
        public void TestEmphasisG()
        {
            MarkdownDocument doc = Open("TestEmphasisG.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "_ foo bar_")});
        }

        /// <summary>
        /// Tests this is not emphasis, because the opening _ is preceded by an alphanumeric and followed by punctuation.
        /// </summary>
        [Test]
        public void TestEmphasisH()
        {
            MarkdownDocument doc = Open("TestEmphasisH.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "a_\"foo\"_")});
        }

        /// <summary>
        /// Tests that emphasis with _ is not allowed inside words.
        /// </summary>
        [Test]
        public void TestEmphasisI()
        {
            MarkdownDocument doc = Open("TestEmphasisI.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "foo_bar_")});
        }

        /// <summary>
        /// Tests that emphasis with _ is not allowed inside words.
        /// </summary>
        [Test]
        public void TestEmphasisJ()
        {
            MarkdownDocument doc = Open("TestEmphasisJ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "5_6_78")});
        }

        /// <summary>
        /// Tests that here _ does not generate emphasis, because the first delimiter run is right-flanking and
        /// the second left-flanking.
        /// </summary>
        [Test]
        public void TestEmphasisK()
        {
            MarkdownDocument doc = Open("TestEmphasisK.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "aa_\"bb\"_cc")});
        }

        /// <summary>
        /// Tests that this is emphasis, even though the opening delimiter is both left- and right-flanking,
        /// because it is preceded by punctuation.
        /// </summary>
        [Test]
        public void TestEmphasisL()
        {
            MarkdownDocument doc = Open("TestEmphasisL.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "foo-"), new ExpectedInline(BlockType.ItalicInline, "*(bar)*")});
        }

        /// <summary>
        /// Tests this is not emphasis, because the closing delimiter does not match the opening delimiter.
        /// </summary>
        [Test]
        public void TestEmphasisM()
        {
            MarkdownDocument doc = Open("TestEmphasisM.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "_foo*")});
        }

        /// <summary>
        /// Tests this is not emphasis, because the closing * is preceded by whitespace.
        /// </summary>
        [Test]
        public void TestEmphasisN()
        {
            MarkdownDocument doc = Open("TestEmphasisN.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "*foo bar *")});
        }

        /// <summary>
        /// Tests a newline also counts as whitespace.
        /// </summary>
        [Test]
        public void TestEmphasisO()
        {
            MarkdownDocument doc = Open("TestEmphasisO.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "*foo bar"+SoftBreak+"*")});
        }

        /// <summary>
        /// Tests this is not emphasis, because the second * is preceded by punctuation and followed by an
        /// alphanumeric (hence it is not part of a right-flanking delimiter run).
        /// </summary>
        [Test]
        public void TestEmphasisP()
        {
            MarkdownDocument doc = Open("TestEmphasisP.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "*(*foo)")});
        }


        /// <summary>
        /// Tests that intraword emphasis with * is allowed.
        /// </summary>
        [Test]
        public void TestEmphasisR()
        {
            MarkdownDocument doc = Open("TestEmphasisR.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo*"), new ExpectedInline(BlockType.Inline, "bar")});
        }

        /// <summary>
        /// Tests this is not emphasis, because the closing _ is preceded by whitespace.
        /// </summary>
        [Test]
        public void TestEmphasisS()
        {
            MarkdownDocument doc = Open("TestEmphasisS.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "_foo bar _")});
        }

        /// <summary>
        /// Tests this is not emphasis, because the second _ is preceded by punctuation and followed by an alphanumeric.
        /// </summary>
        [Test]
        public void TestEmphasisT()
        {
            MarkdownDocument doc = Open("TestEmphasisT.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "_(_foo)")});
        }


        /// <summary>
        /// Tests that intraword emphasis is disallowed for _.
        /// </summary>
        [Test]
        public void TestEmphasisV()
        {
            MarkdownDocument doc = Open("TestEmphasisV.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0],
                new []
                {
                    new ExpectedInline(BlockType.Inline, "_foo_bar"+SoftBreak+"_пристаням_стремятся"+SoftBreak),
                    new ExpectedInline(BlockType.ItalicInline, "*foo_bar_baz*")
                });
        }

        /// <summary>
        /// Tests that this is emphasis, even though the closing delimiter is both left- and right-flanking,
        /// because it is followed by punctuation.
        /// </summary>
        [Test]
        public void TestEmphasisW()
        {
            MarkdownDocument doc = Open("TestEmphasisW.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.ItalicInline, "*(bar)*"), new ExpectedInline(BlockType.Inline, ".")});
        }

        /// <summary>
        /// Tests strong emphasis.
        /// </summary>
        [Test]
        public void TestEmphasisX()
        {
            MarkdownDocument doc = Open("TestEmphasisX.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.BoldInline, "**foo bar**")});
        }

        /// <summary>
        /// Tests this is not strong emphasis, because the opening delimiter is followed by whitespace.
        /// </summary>
        [Test]
        public void TestEmphasisY()
        {
            MarkdownDocument doc = Open("TestEmphasisY.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "** foo bar**")});
        }

        /// <summary>
        /// Tests this is not strong emphasis, because the opening ** is preceded by an alphanumeric and followed
        /// by punctuation, and hence not part of a left-flanking delimiter run.
        /// </summary>
        [Test]
        public void TestEmphasisZ()
        {
            MarkdownDocument doc = Open("TestEmphasisZ.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "a**\"foo\"**")});
        }

        /// <summary>
        /// Tests that intraword strong emphasis with ** is permitted.
        /// </summary>
        [Test]
        public void TestEmphasisZ1()
        {
            MarkdownDocument doc = Open("TestEmphasisZ1.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "foo"), new ExpectedInline(BlockType.BoldInline, "**bar**")});
        }

        /// <summary>
        /// Tests this is not strong emphasis, because the opening delimiter is followed by whitespace.
        /// </summary>
        [Test]
        public void TestEmphasisZ2()
        {
            MarkdownDocument doc = Open("TestEmphasisZ2.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "__ foo bar__")});
        }

        /// <summary>
        /// Tests that a newline counts as whitespace.
        /// </summary>
        [Test]
        public void TestEmphasisZ3()
        {
            MarkdownDocument doc = Open("TestEmphasisZ3.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "__"+SoftBreak+"foo bar__")});
        }

        /// <summary>
        /// Tests that intraword strong emphasis is forbidden with __.
        /// </summary>
        [Test]
        public void TestEmphasisZ4()
        {
            MarkdownDocument doc = Open("TestEmphasisZ4.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "a__\"foo\"__")});
        }

        /// <summary>
        /// Tests this is not strong emphasis, because the opening __ is preceded by an alphanumeric
        /// and followed by punctuation.
        /// </summary>
        [Test]
        public void TestEmphasisZ5()
        {
            MarkdownDocument doc = Open("TestEmphasisZ5.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "foo__bar__"+SoftBreak+"5__6__78"+SoftBreak+"пристаням__стремятся__")});
        }

        /// <summary>
        /// Tests nested strong emphasis.
        /// </summary>
        [Test]
        public void TestEmphasisZ6()
        {
            MarkdownDocument doc = Open("TestEmphasisZ6.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            ParagraphBlock paragraphBlock = (ParagraphBlock)doc[0];
            CheckInlines(paragraphBlock, new [] {new ExpectedInline(BlockType.BoldInline, "**foo, **bar**, baz**")});

            CheckInlines(paragraphBlock[0],
                new [] {new ExpectedInline(BlockType.Inline, "foo, "), new ExpectedInline(BlockType.BoldInline, "**bar**"), new ExpectedInline(BlockType.Inline, ", baz")});
        }

        /// <summary>
        /// Tests that this is strong emphasis, even though the opening delimiter is both left- and right-flanking,
        /// because it is preceded by punctuation.
        /// </summary>
        [Test]
        public void TestEmphasisZ7()
        {
            MarkdownDocument doc = Open("TestEmphasisZ7.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "foo-"), new ExpectedInline(BlockType.BoldInline, "**(bar)**")});
        }

        /// <summary>
        /// Tests that this is not strong emphasis, because the closing delimiter is preceded by whitespace.
        /// Nor can it be interpreted as an emphasized *foo bar *, because a literal * character cannot occur at the
        /// beginning or end of *-delimited emphasis or **-delimited strong emphasis, unless it is backslash-escaped.
        /// </summary>
        [Test]
        public void TestEmphasisZ8()
        {
            MarkdownDocument doc = Open("TestEmphasisZ8.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "**foo bar **")});
        }

        /// <summary>
        /// Tests that this is not strong emphasis, because the second ** is preceded by punctuation
        /// and followed by an alphanumeric.
        /// </summary>
        [Test]
        public void TestEmphasisZ9()
        {
            MarkdownDocument doc = Open("TestEmphasisZ9.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "**(**foo)")});
        }

        /// <summary>
        /// Tests nested emphases.
        /// </summary>
        [Test]
        public void TestEmphasisZ10()
        {
            MarkdownDocument doc = Open("TestEmphasisZ10.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.ItalicInline, "*(**foo**)*")});
            CheckInlines(doc[1],
                new []
                {
                    new ExpectedInline(BlockType.BoldInline, "**Gomphocarpus (*Gomphocarpus physocarpus*, syn."+SoftBreak+"*Asclepias physocarpa*)**")
                });
            CheckInlines(doc[2], new [] {new ExpectedInline(BlockType.BoldInline, "**foo \"*bar*\" foo**")});
        }

        /// <summary>
        /// Tests intraword strong emphasis.
        /// </summary>
        [Test]
        public void TestEmphasisZ11()
        {
            MarkdownDocument doc = Open("TestEmphasisZ11.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.BoldInline, "**foo**"), new ExpectedInline(BlockType.Inline, "bar")});
        }

        /// <summary>
        /// Tests that this is not strong emphasis, because the closing delimiter is preceded by whitespace.
        /// </summary>
        [Test]
        public void TestEmphasisZ12()
        {
            MarkdownDocument doc = Open("TestEmphasisZ12.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "__foo bar __")});
        }

        /// <summary>
        /// Tests this is not strong emphasis, because the second __ is preceded by punctuation and followed by an alphanumeric.
        /// </summary>
        [Test]
        public void TestEmphasisZ13()
        {
            MarkdownDocument doc = Open("TestEmphasisZ13.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "__(__foo)")});
        }

        /// <summary>
        /// Tests nested emphases.
        /// </summary>
        [Test]
        public void TestEmphasisZ14()
        {
            MarkdownDocument doc = Open("TestEmphasisZ14.md");

            Assert.That(doc.Count, Is.EqualTo(1));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.ItalicInline, "*(**foo**)*")});
        }

        /// <summary>
        /// Tests that intraword strong emphasis is forbidden with __.
        /// </summary>
        [Test]
        public void TestEmphasisZ15()
        {
            MarkdownDocument doc = Open("TestEmphasisZ15.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "__foo__bar")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.Inline, "__пристаням__стремятся")});
            CheckInlines(doc[2], new [] {new ExpectedInline(BlockType.BoldInline, "**foo__bar__baz**")});
        }

        /// <summary>
        /// Tests that any nonempty sequence of inline elements can be the contents of an emphasized span.
        /// </summary>
        [Test]
        public void TestEmphasisZ16()
        {
            MarkdownDocument doc = Open("TestEmphasisZ16.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo bar/url*")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo"+SoftBreak+"bar*")});
        }

        /// <summary>
        /// Tests that emphasis and strong emphasis can be nested inside emphasis.
        /// </summary>
        [Test]
        public void TestEmphasisZ17()
        {
            MarkdownDocument doc = Open("TestEmphasisZ17.md");

            Assert.That(doc.Count, Is.EqualTo(7));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo **bar** baz*")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo *bar* baz*")});
            CheckInlines(doc[2], new [] {new ExpectedInline(BlockType.ItalicInline, "**foo* bar*")});
            CheckInlines(doc[3], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo *bar**")});
            CheckInlines(doc[4], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo **bar** baz*")});
            CheckInlines(doc[5], new [] {new ExpectedInline(BlockType.ItalicInline, "***foo** bar*")});
            CheckInlines(doc[6], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo **bar***")});
        }

        /// <summary>
        /// Tests that a delimiter that can both open and close cannot form emphasis if the sum of the lengths
        /// of the delimiter runs containing the opening and closing delimiters is a multiple of 3.
        /// </summary>
        [Test]
        public void TestEmphasisZ18()
        {
            MarkdownDocument doc = Open("TestEmphasisZ18.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo**bar**baz*")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo**bar***")});
        }

        /// <summary>
        /// Tests that indefinite levels of nesting are possible.
        /// </summary>
        [Test]
        public void TestEmphasisZ19()
        {
            MarkdownDocument doc = Open("TestEmphasisZ19.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo **bar *baz* bim** bop*")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo *bar*/url*")});
        }

        /// <summary>
        /// Tests that there can be no empty emphasis or strong emphasis.
        /// </summary>
        [Test]
        public void TestEmphasisZ20()
        {
            MarkdownDocument doc = Open("TestEmphasisZ20.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckParagraph(doc[0], "** is not an empty emphasis");
            CheckParagraph(doc[1], "**** is not an empty strong emphasis");
        }

        /// <summary>
        /// Tests that any nonempty sequence of inline elements can be the contents of an strongly emphasized span.
        /// </summary>
        [Test]
        public void TestEmphasisZ21()
        {
            MarkdownDocument doc = Open("TestEmphasisZ21.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.BoldInline, "**foo bar/url**")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.BoldInline, "**foo"+SoftBreak+"bar**")});
        }

        /// <summary>
        /// Tests that emphasis and strong emphasis can be nested inside strong emphasis.
        /// </summary>
        [Test]
        public void TestEmphasisZ22()
        {
            MarkdownDocument doc = Open("TestEmphasisZ22.md");

            Assert.That(doc.Count, Is.EqualTo(8));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.BoldInline, "**foo *bar* baz**")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.BoldInline, "**foo **bar** baz**")});
            CheckInlines(doc[2], new [] {new ExpectedInline(BlockType.BoldInline, "****foo** bar**")});
            CheckInlines(doc[3], new [] {new ExpectedInline(BlockType.BoldInline, "**foo **bar****")});
            CheckInlines(doc[4], new [] {new ExpectedInline(BlockType.BoldInline, "**foo *bar* baz**")});
            CheckInlines(doc[5], new [] {new ExpectedInline(BlockType.BoldInline, "**foo*bar*baz**")});
            CheckInlines(doc[6], new [] {new ExpectedInline(BlockType.BoldInline, "***foo* bar**")});
            CheckInlines(doc[7], new [] {new ExpectedInline(BlockType.BoldInline, "**foo *bar***")});
        }

        /// <summary>
        /// Tests that when delimiters do not match evenly, the excess literal * characters will appear outside
        /// of the emphasis, rather than inside it.
        /// </summary>
        [Test]
        public void TestEmphasisZ23()
        {
            MarkdownDocument doc = Open("TestEmphasisZ23.md");

            Assert.That(doc.Count, Is.EqualTo(6));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "*"), new ExpectedInline(BlockType.ItalicInline, "*foo*")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo*"), new ExpectedInline(BlockType.Inline, "*")});
            CheckInlines(doc[2], new [] {new ExpectedInline(BlockType.Inline, "*"), new ExpectedInline(BlockType.BoldInline, "**foo**")});
            CheckInlines(doc[3], new [] {new ExpectedInline(BlockType.Inline, "***"), new ExpectedInline(BlockType.ItalicInline, "*foo*")});
            CheckInlines(doc[4], new [] {new ExpectedInline(BlockType.BoldInline, "**foo**"), new ExpectedInline(BlockType.Inline, "*")});
            CheckInlines(doc[5], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo*"), new ExpectedInline(BlockType.Inline, "***")});
        }

        /// <summary>
        /// Tests that if you want emphasis nested directly inside emphasis, you must use different delimiters.
        /// </summary>
        [Test]
        public void TestEmphasisZ24()
        {
            MarkdownDocument doc = Open("TestEmphasisZ24.md");

            Assert.That(doc.Count, Is.EqualTo(4));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.BoldInline, "**foo**")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.ItalicInline, "**foo**")});
            CheckInlines(doc[2], new [] {new ExpectedInline(BlockType.BoldInline, "**foo**")});
            CheckInlines(doc[3], new [] {new ExpectedInline(BlockType.ItalicInline, "**foo**")});
        }

        /// <summary>
        /// Tests that strong emphasis within strong emphasis is possible without switching delimiters.
        /// </summary>
        [Test]
        public void TestEmphasisZ25()
        {
            MarkdownDocument doc = Open("TestEmphasisZ25.md");

            Assert.That(doc.Count, Is.EqualTo(3));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.BoldInline, "****foo****")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.BoldInline, "****foo****")});
            CheckInlines(doc[2], new [] {new ExpectedInline(BlockType.BoldInline, "******foo******")});
        }

        /// <summary>
        /// Tests that an interpretation <em><strong>...</strong></em> is always preferred to <strong><em>...</em></strong>.
        /// </summary>
        [Test]
        public void TestEmphasisZ26()
        {
            MarkdownDocument doc = Open("TestEmphasisZ26.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.ItalicInline, "***foo***")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.ItalicInline, "*****foo*****")});
        }

        /// <summary>
        /// Tests that when two potential emphasis or strong emphasis spans overlap, so that the second begins before the
        /// first ends and ends after the first ends, the first takes precedence.
        /// </summary>
        [Test]
        public void TestEmphasisZ27()
        {
            MarkdownDocument doc = Open("TestEmphasisZ27.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo _bar*"), new ExpectedInline(BlockType.Inline, " baz_")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.ItalicInline, "*foo **bar *baz bim** bam*")});
        }

        /// <summary>
        /// Tests that when there are two potential emphasis or strong emphasis spans with the same closing delimiter,
        /// the shorter one (the one that opens later) takes precedence.
        /// </summary>
        [Test]
        public void TestEmphasisZ28()
        {
            MarkdownDocument doc = Open("TestEmphasisZ28.md");

            Assert.That(doc.Count, Is.EqualTo(2));
            CheckInlines(doc[0], new [] {new ExpectedInline(BlockType.Inline, "**foo "), new ExpectedInline(BlockType.BoldInline, "**bar baz**")});
            CheckInlines(doc[1], new [] {new ExpectedInline(BlockType.Inline, "*foo "), new ExpectedInline(BlockType.ItalicInline, "*bar baz*")});
        }

        /// <summary>
        /// Tests that inline code spans, links, images, and HTML tags group more tightly than emphasis.
        /// So, when there is a choice between an interpretation that contains one of these elements and
        /// one that does not, the former always wins.
        /// </summary>
        /// <remarks>
        /// BUT for a while these are emphases.
        /// </remarks>
        [Test]
        public void TestEmphasisZ29()
        {
            MarkdownDocument doc = Open("TestEmphasisZ29.md");

            Assert.That(doc.Count, Is.EqualTo(9));
            CheckInlines(doc[0], new[] {
                new ExpectedInline(BlockType.Inline, "*"),
                new ExpectedInline(BlockType.LinkText, "bar*"),
                new ExpectedInline(BlockType.LinkDestination, "/url")});
            CheckInlines(doc[1], new[] {
                new ExpectedInline(BlockType.Inline, "_foo "),
                new ExpectedInline(BlockType.LinkText, "bar_"),
                new ExpectedInline(BlockType.LinkDestination, "/url")});
            CheckInlines(doc[2], new [] {
                new ExpectedInline(BlockType.ItalicInline, "*<img src=\"foo\" title=\"*"),
                new ExpectedInline(BlockType.Inline, "\"/>")});
            CheckInlines(doc[3], new [] {
                new ExpectedInline(BlockType.BoldInline, "**<a href=\"**"),
                new ExpectedInline(BlockType.Inline, "\">")});
            CheckInlines(doc[4], new [] {
                new ExpectedInline(BlockType.BoldInline, "**<a href=\"**"),
                new ExpectedInline(BlockType.Inline, "\">")});

            CheckInlines(doc[5], new [] { new ExpectedInline(BlockType.ItalicInline, "*a `*`*")});
            ItalicInlineBlock italicInline = (ItalicInlineBlock)((ParagraphBlock)doc[5])[0];
            CheckInlines(italicInline, new [] {
                new ExpectedInline(BlockType.Inline, "a "),
                new ExpectedInline(BlockType.InlineCode, "`*`")});

            CheckInlines(doc[6], new [] {new ExpectedInline(BlockType.ItalicInline, "*a `_`*")});
            italicInline = (ItalicInlineBlock)((ParagraphBlock)doc[6])[0];
            CheckInlines(italicInline, new [] {
                new ExpectedInline(BlockType.Inline, "a "),
                new ExpectedInline(BlockType.InlineCode, "`_`")});

            CheckInlines(doc[7], new[] {
                new ExpectedInline(BlockType.Inline, "**a"),
                new ExpectedInline(BlockType.Autolink, "<http://foo.bar/?q=**>") });
            CheckInlines(doc[8], new[] {
                new ExpectedInline(BlockType.Inline, "__a"),
                new ExpectedInline(BlockType.Autolink, "<http://foo.bar/?q=__>") });
        }

        /// <summary>
        /// Returns markdown document read from a specified path.
        /// </summary>
        internal override MarkdownDocument Open(string fileName)
        {
            return base.Open(string.Format(@"ImportMarkdown\Inlines\Emphasis\{0}", fileName));
        }

        /// <summary>
        /// Verifies delimiter run in a specified text.
        /// </summary>
        private static void CheckDelimiterRun(string text, FlankingType expectedFlankingType, string expectedOpening)
        {
            DelimiterRun delimiterRun = new DelimiterRun();
            delimiterRun.TryParse(text, 0);

            Assert.That(delimiterRun.Delimiters[0].FlankingType, Is.EqualTo(expectedFlankingType));
            Assert.That(delimiterRun.Opening, Is.EqualTo(expectedOpening));
        }
    }
}
