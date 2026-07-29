// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2014 by Konstantin Sidorenko

using System.Collections.Generic;
using Aspose.Bidi;
using Aspose.Common;
using NUnit.Framework;

namespace Aspose.Tests.Bidi
{
    [TestFixture]
    public class TestNBidiSimple
    {
        [Test]
        public void TestRtlRuns()
        {
            string org = "1[Row1 'السلام عليكم' Col1]";
            //rtl text marker
            const char rle = BidiChars.RLE;
            org = rle + org;
            IList<BidiParagraph> pars = BidiParagraph.SplitStringToParagraphs(org);
            CheckBidiRuns(pars, "1", "[", "Row1", " 'السلام عليكم' ", "Col1", "]");
        }

        [Test]
        public void TestLtrRuns()
        {
            const string org = "1[Row1 'السلام عليكم' Col1]";
            IList<BidiParagraph> pars = BidiParagraph.SplitStringToParagraphs(org);
            CheckBidiRuns(pars, "1[Row1 '", "السلام عليكم", "' Col1]");
        }

        private static void CheckBidiRuns(IList<BidiParagraph> pars, params string[] expectedTexts)
        {
            int index = 0;
            foreach (BidiRun run in pars[0].BidiRuns)
                Assert.That(run.Text, Is.EqualTo(expectedTexts[index++]));

            Assert.That(index, Is.EqualTo(expectedTexts.Length));
        }

        [Test]
        public void TestSplitToPars()
        {
            //single separator
            string org = "par1\n"
                         + "par2\r"
                         + "\r" //empty paragraph 
                         + "\r" //empty paragraph 
                         + "par3\n"
                         + "par4\n";
            IList<BidiParagraph> pars = BidiParagraph.SplitStringToParagraphs(org);
            Assert.That(pars.Count, Is.EqualTo(6));
            // String finished without paragraph separator
            org = "par1\r\n"
                         + "par2\r\n"
                         + "par3]r\n"
                         + "par4";//finished without paragraph separator
            pars = BidiParagraph.SplitStringToParagraphs(org);
            Assert.That(pars.Count, Is.EqualTo(4));
            //combo 
            org = "par1\r\n"
                         + "par2\r"
                         + "\r\n" //empty paragraph 
                         + "\n" //empty paragraph 
                         + "par3\n"
                         + "par4\r\n";
            pars = BidiParagraph.SplitStringToParagraphs(org);
            Assert.That(pars.Count, Is.EqualTo(6));
        }

        [Test]
        public void TestComplex()
        {
            const string span1 = "1[F].";
            const string span2 = "2{S}.";
            const string span3 = "3(T).";
            //Create string which emulate the following HTML:
            //<p><span dir="RTL">1[F].</span><span>2{S}.</span><span dir="RTL">3(T).</span></p>
            string orig = BidiChars.RLE + span1 + BidiChars.PDF + span2 + BidiChars.RLE + span3 + BidiChars.PDF;
            
            IList<BidiParagraph> pars = BidiParagraph.SplitStringToParagraphs(orig);
            ICollection<BidiRun> br = pars[0].BidiRuns;
            Assert.That(10, Is.EqualTo(br.Count));
            
            string res = NBidi.LogicalToVisual(orig);
            string exp = "2.[F]1{S}..(T)3";
            Assert.That(res, Is.EqualTo(exp), string.Format(
                    "original: \"{0}\"\noriginal as chars: {1}\nresult as chars:   {2}\nexpected as chars: {3}", 
                    orig, AsCharArray(orig), AsCharArray(res), AsCharArray(exp)));
        }

        [Category("CppSlowTest")]
        private static string AsCharArray(string str)
        {
            string testRes = string.Empty;
            for (int i = 0; i < str.Length; i++)
                testRes += @"\u" + FormatterPal.IntToStrX4(str[i]);
            return testRes;
        }
    }
}
