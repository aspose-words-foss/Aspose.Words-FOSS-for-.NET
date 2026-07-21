// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/09/2014 by Victor Chebotok

using Aspose.Bidi;
using NUnit.Framework;

namespace Aspose.Tests.Bidi
{
    /// <summary>
    /// Tests our implementation of the Unicode bidirectional algorithm.
    /// </summary>
    [TestFixture]
    public class TestBidiAlgorithm
    {
        [Test]
        public void TestIsRtlParagraphEmpty()
        {
            Assert.That(BidiAlgorithm.IsRtlParagraph(""), Is.EqualTo(false));
        }

        [Test]
        public void TestIsRtlParagraphLtr()
        {
            Assert.That(BidiAlgorithm.IsRtlParagraph("z\u05D0"), Is.EqualTo(false));
        }

        [Test]
        public void TestIsRtlParagraphRtl()
        {
            Assert.That(BidiAlgorithm.IsRtlParagraph("\u05D0Az"), Is.EqualTo(true));
        }

        [Test]
        public void TestIsRtlParagraphRtlArabicLetter()
        {
            Assert.That(BidiAlgorithm.IsRtlParagraph("\u0621z\u05D0A"), Is.EqualTo(true));
        }

        [Test]
        public void TestIsRtlParagraphNoStrongCharacter()
        {
            Assert.That(BidiAlgorithm.IsRtlParagraph("123"), Is.EqualTo(false));
        }

        [Test]
        public void TestBoundaryNeutralCharacterLtr()
        {
            BidiRun[] runs = BidiAlgorithm.Apply("abc\u00ADdef", false);

            Assert.That(runs.Length, Is.EqualTo(1));
            CheckBidiRun("abc\u00ADdef", false, 0, runs[0]);
        }

        [Test]
        public void TestBoundaryNeutralCharacterRtl()
        {
            BidiRun[] runs = BidiAlgorithm.Apply("\u0645\u06CC\u200C\u062E\u0648\u0627\u0647\u0645", false);

            Assert.That(runs.Length, Is.EqualTo(1));
            CheckBidiRun("\u0645\u06CC\u200C\u062E\u0648\u0627\u0647\u0645", true, 0, runs[0]);
        }

        [Test]
        public void Test1()
        {
            BidiSourceRun[] sourceRuns = new BidiSourceRun[]
            {
                new BidiSourceRun("User "),
                new BidiSourceRun("שנב"),
                new BidiSourceRun(" - 3.5 posts")
            };
            BidiRun[] runs = BidiAlgorithm.Apply(sourceRuns, false, false, true);

            Assert.That(runs.Length, Is.EqualTo(4));
            CheckBidiRun("User ", false, 0, runs[0]);
            CheckBidiRun("שנב", true, 1, runs[1]);
            CheckBidiRun(" - 3.5", true, 2, runs[2]);
            CheckBidiRun(" posts", false, 2, runs[3]);
        }
        
        [Test]
        public void Test2()
        {
            BidiSourceRun[] sourceRuns = new BidiSourceRun[]
            {
                new BidiSourceRun("1. שנב 123 456 גקכ.", BidiLevel.EmbedRtl)
            };
            BidiRun[] runs = BidiAlgorithm.Apply(sourceRuns, false, false, true);

            Assert.That(runs.Length, Is.EqualTo(1));
            CheckBidiRun("1. שנב 123 456 גקכ.", true, 0, runs[0]);
        }
        
        [Test]
        public void Test3()
        {
            BidiSourceRun[] sourceRuns = new BidiSourceRun[]
            {
                new BidiSourceRun("\u05D0\u05D1\u05D2  \uD840\uDC00\uD840\uDC01\uD840\uDC02 \u05D3\u05D4\u05D5")
            };
            BidiRun[] runs = BidiAlgorithm.Apply(sourceRuns, true, false, true);

            Assert.That(runs.Length, Is.EqualTo(3));
            CheckBidiRun("\u05D0\u05D1\u05D2  ", true, 0, runs[0]);
            CheckBidiRun("\uD840\uDC00\uD840\uDC01\uD840\uDC02", false, 0, runs[1]);
            CheckBidiRun(" \u05D3\u05D4\u05D5", true, 0, runs[2]);
        }
        
        [Test]
        public void Test4()
        {
            BidiSourceRun[] sourceRuns = new BidiSourceRun[]
            {
                new BidiSourceRun("5/11/2011", BidiLevel.EmbedLtr),
                new BidiSourceRun(" יעיבר םוי - ןורחא ןוכדע")
            };
            BidiRun[] runs = BidiAlgorithm.Apply(sourceRuns, true, true, true);

            Assert.That(runs.Length, Is.EqualTo(2));
            CheckBidiRunHebrewRtl("5/11/2011", 0, runs[0]);
            CheckBidiRun("עדכון אחרון - יום רביעי ", true, 1, runs[1]);
        }

        [Test]
        public void Test5()
        {
            BidiSourceRun[] sourceRuns = new BidiSourceRun[]
            {
                new BidiSourceRun("123:456")
            };
            BidiRun[] runs = BidiAlgorithm.Apply(sourceRuns, true, true, true);

            Assert.That(runs.Length, Is.EqualTo(1));
            CheckBidiRun("123:456", false, 0, runs[0]);
        }

        [Test]
        public void Test6()
        {
            BidiSourceRun[] sourceRuns = new BidiSourceRun[]
            {
                new BidiSourceRun(":" + "ךיראת"),
                new BidiSourceRun("23/04/2014", BidiLevel.EmbedLtr)
            };
            BidiRun[] runs = BidiAlgorithm.Apply(sourceRuns, true, true, true);

            Assert.That(runs.Length, Is.EqualTo(2));
            CheckBidiRun("תאריך" + ":", true, 0, runs[0]);
            CheckBidiRunHebrewRtl("23/04/2014", 1, runs[1]);
        }

        [Test]
        public void Test7()
        {
            BidiSourceRun[] sourceRuns = new BidiSourceRun[]
            {
                new BidiSourceRun("LTR Najib said \"السلام عليكم\" "),
                new BidiSourceRun("(as-salaam alaykum] to me.", BidiLevel.EmbedRtl)
            };
            BidiRun[] runs = BidiAlgorithm.Apply(sourceRuns, false, false, true);

            Assert.That(runs.Length, Is.EqualTo(5));
            CheckBidiRun("LTR Najib said \"", false, 0, runs[0]);
            CheckBidiRun(".", true, 1, runs[1]);
            CheckBidiRun("as-salaam alaykum] to me", false, 1, runs[2]);
            CheckBidiRun("السلام عليكم\" ", true, 0, runs[3]);
            CheckBidiRun("(", true, 1, runs[4]);
        }

        [Test]
        public void Test8()
        {
            BidiSourceRun[] sourceRuns = new BidiSourceRun[]
            {
                new BidiSourceRun("1[First].", BidiLevel.EmbedRtl),
                new BidiSourceRun("2{Second}!"),
                new BidiSourceRun("3(Third)?", BidiLevel.EmbedRtl)
            };
            BidiRun[] runs = BidiAlgorithm.Apply(sourceRuns, false, false, true);

            Assert.That(runs.Length, Is.EqualTo(8));
            CheckBidiRun("].",          true,   0, runs[0]);
            CheckBidiRun("2",           true,   1, runs[1]);
            CheckBidiRun("First",       false,  0, runs[2]);
            CheckBidiRun("1[",          true,   0, runs[3]);
            CheckBidiRun("{Second}!",   false,  1, runs[4]);
            CheckBidiRun(")?",          true,   2, runs[5]);
            CheckBidiRun("Third",       false,  2, runs[6]);
            CheckBidiRun("3(",          true,   2, runs[7]);
        }

        /// <summary>
        /// WORDSNET-12119 Adjacent runs having same direction were reordered during conversion to MS Word order.
        /// </summary>
        [Test]
        public void TestJira12119()
        {
            // The source runs correspond to the following HTML code:
            // <p dir='rtl'><span dir='ltr'>server</span><span dir='rtl'>A</span><span> 8</span></p>
            BidiSourceRun[] sourceRuns = new BidiSourceRun[]
            {
                new BidiSourceRun("server", BidiLevel.EmbedLtr),
                new BidiSourceRun("A", BidiLevel.EmbedRtl),
                new BidiSourceRun(" 8")
            };
            BidiRun[] runs = BidiAlgorithm.Apply(sourceRuns, true, false, true);

            // The order of runs 'server' and 'A' shouldn't change.
            Assert.That(runs.Length, Is.EqualTo(3));
            CheckBidiRun("server", false, 0, runs[0]);
            CheckBidiRun("A", false, 1, runs[1]);
            CheckBidiRun(" 8", true, 2, runs[2]);
        }

        /// <summary>
        /// WORDSNET-13787 Numbers containing certain characters ('+', '-', and '/') get reordered if imported
        /// into an RTL run. In order to prevent this behavior we have to import such runs with 'he-IL' locale.
        /// This test checks a number in Hebrew RTL text.
        /// </summary>
        [Test]
        public void TestJira13787Hebrew()
        {
            // Because of Hebrew text, numbers are classified as European numbers (AN) and European separators (ES) between them
            // become parts of that numbers and receive the LTR direction.
            BidiSourceRun[] sourceRuns = new BidiSourceRun[]
            {
                new BidiSourceRun("שנב 1+2-3/4:5.6,7 גקכ")
            };
            BidiRun[] runs = BidiAlgorithm.Apply(sourceRuns, true, false, true);

            Assert.That(runs.Length, Is.EqualTo(3));
            CheckBidiRun("שנב" + " ", true, 0, runs[0]);
            // This part is RTL and it contains '+', '-', and '/' characters between european number (EN) characters
            // so it requires 'he-IL' locale to be rendered properly.
            CheckBidiRunHebrewRtl("1+2-3/4:5.6,7", 0, runs[1]);
            CheckBidiRun(" " + "גקכ", true, 0, runs[2]);
        }

        /// <summary>
        /// WORDSNET-13787 Numbers containing certain characters ('+', '-', and '/') get reordered if imported
        /// into an RTL run. In order to prevent this behavior we have to import such runs with 'he-IL' locale.
        /// This test checks a number in English LTR text.
        /// </summary>
        [Test]
        public void TestJira13787English()
        {
            BidiSourceRun[] sourceRuns = new BidiSourceRun[]
            {
                new BidiSourceRun("abc 1+2-3/4:5.6,7 def")
            };
            BidiRun[] runs = BidiAlgorithm.Apply(sourceRuns, true, false, true);

            Assert.That(runs.Length, Is.EqualTo(1));
            // The whole text is LTR. No need to offset the number in any way.
            CheckBidiRun("abc 1+2-3/4:5.6,7 def", false, 0, runs[0]);
        }

        /// <summary>
        /// WORDSNET-13787 Numbers containing certain characters ('+', '-', and '/') get reordered if imported
        /// into an RTL run. In order to prevent this behavior we have to import such runs with 'he-IL' locale.
        /// This test checks a number in Arabic RTL text.
        /// </summary>
        [Test]
        public void TestJira13787Arabic()
        {
            // Because of Arabic text, numbers are classified as Arabic numbers (AN) and European separators (ES) between them
            // are treated as neutral characters (ON), which receive the RTL direction.
            BidiSourceRun[] sourceRuns = new BidiSourceRun[]
            {
                new BidiSourceRun("شلاؤ 1+2-3/4:5.6,7 يثب")
            };
            BidiRun[] runs = BidiAlgorithm.Apply(sourceRuns, true, false, true);

            Assert.That(runs.Length, Is.EqualTo(3));
            CheckBidiRun("شلاؤ" + " 1+2-", true, 0, runs[0]);
            // This part is RTL and it contains a slash character so it requires 'he-IL' locale to be rendered properly.
            CheckBidiRunHebrewRtl("3/4:5.6,7", 0, runs[1]);
            CheckBidiRun(" " + "يثب", true, 0, runs[2]);
        }

        /// <summary>
        /// WORDSNET-18763 The W4 rule of the Unicode Bidi Algorithm was not applied to ES and CS characters in certain cases
        /// because of a bug.
        /// </summary>
        [Test]
        public void Test18763()
        {
            // U+202C "POP DIRECTIONAL FORMATTING" characters around the last separator are added to check how the W4 rule
            // of the algorithm works with BN characters.
            BidiRun[] runs = BidiAlgorithm.Apply("\u05D0:\u00A01/2\u202C\u202C+\u202C\u202C3", true);

            Assert.That(runs.Length, Is.EqualTo(2));
            // These two separators must be left unchanged.
            CheckBidiRun("\u05D0:\u00A0", true, 0, runs[0]);
            // These separators must become a part of the number. The PDF characters are removed by the algorithm.
            CheckBidiRun("1/2+3", false, 0, runs[1]);
        }

        private static void CheckBidiRun(string expectedText, bool expectedRtl, int expectedSourceRunIndex, BidiRun run)
        {
            Assert.That(run.Text, Is.EqualTo(expectedText));
            Assert.That(run.Rtl, Is.EqualTo(expectedRtl));
            Assert.That(run.SourceRunIndex, Is.EqualTo(expectedSourceRunIndex));
            Assert.That(run.RequiresHebrewLocaleBi, Is.EqualTo(false));
        }

        private static void CheckBidiRunHebrewRtl(string expectedText, int expectedSourceRunIndex, BidiRun run)
        {
            Assert.That(run.Text, Is.EqualTo(expectedText));
            Assert.That(run.Rtl, Is.EqualTo(true));
            Assert.That(run.SourceRunIndex, Is.EqualTo(expectedSourceRunIndex));
            Assert.That(run.RequiresHebrewLocaleBi, Is.EqualTo(true));
        }
    }
}
