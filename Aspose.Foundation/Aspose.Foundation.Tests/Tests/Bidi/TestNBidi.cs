// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/04/2015 by Roman Korchagin

// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

using System.Text;
using Aspose.Bidi;
using Aspose.Common;
using NUnit.Framework;

namespace Aspose.Tests.Bidi
{
    [TestFixture]
    public class TestNBidi
    {
        [Test]
        public void Test01()
        {
            const string orig = "he said '\u05d6\u05d5\u05d4\u05d9 \u05de\u05db\u05d5\u05e0\u05d9\u05ea'";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("he said '\u05ea\u05d9\u05e0\u05d5\u05db\u05de \u05d9\u05d4\u05d5\u05d6'", res);
        }

        [Test]
        public void Test02()
        {
            const string orig = "\u05d4\u05d1\u05d8\u05d8\u05d5\u05ea \u05d2\u05d3\u05dc\u05d5\u05ea \u05d1\u05de\u05d9\u05e9\u05d5\u05e8\u05d9\u05dd, \u05d4\u05d0\u05d2\u05d5\u05d6\u05d9\u05dd \u05d1\u05d4\u05e8\u05d9\u05dd.";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings(".\u05dd\u05d9\u05e8\u05d4\u05d1 \u05dd\u05d9\u05d6\u05d5\u05d2\u05d0\u05d4 ,\u05dd\u05d9\u05e8\u05d5\u05e9\u05d9\u05de\u05d1 \u05ea\u05d5\u05dc\u05d3\u05d2 \u05ea\u05d5\u05d8\u05d8\u05d1\u05d4", res);
        }

        [Test]
        public void Test03()
        {
            const string orig = "\u05d4\u05d1\u05d8\u05d8\u05d5\u05ea \u05d2\u05d3\u05dc\u05d5\u05ea \u05d1\u05de\u05d9\u05e9\u05d5\u05e8\u05d9\u05dd, The nuts \u05d1\u05d4\u05e8\u05d9\u05dd.";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings(".\u05dd\u05d9\u05e8\u05d4\u05d1 The nuts ,\u05dd\u05d9\u05e8\u05d5\u05e9\u05d9\u05de\u05d1 \u05ea\u05d5\u05dc\u05d3\u05d2 \u05ea\u05d5\u05d8\u05d8\u05d1\u05d4", res);
        }

        [Test]
        public void Test04()
        {
            const string orig = "I am John, \u05d0\u05e0\u05d9 \u05d0\u05d5\u05d4\u05d1 Bananas.";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("I am John, \u05d1\u05d4\u05d5\u05d0 \u05d9\u05e0\u05d0 Bananas.", res);
        }

        [Test]
        public void Test05()
        {
            const string orig = "\u05d1-15 \u05d1\u05e0\u05d5\u05d1\u05de\u05d1\u05e8 \u05d0\u05de\u05e8\u05ea\u05d9 \u05dc-John: \u05dc\u05da \u05de\u05e4\u05d4!";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("!\u05d4\u05e4\u05de \u05da\u05dc :John-\u05dc \u05d9\u05ea\u05e8\u05de\u05d0 \u05e8\u05d1\u05de\u05d1\u05d5\u05e0\u05d1 15-\u05d1", res);
        }

        [Test]
        public void Test06_Mirroring_Hebrew()
        {
            const string orig = "\u05d1\u05d3\u05d9\u05e7\u05ea (\u05e1\u05d5\u05d2\u05e8\u05d9\u05d9\u05dd) \u05d1\u05e2\u05d1\u05e8\u05d9\u05ea";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("\u05ea\u05d9\u05e8\u05d1\u05e2\u05d1 (\u05dd\u05d9\u05d9\u05e8\u05d2\u05d5\u05e1) \u05ea\u05e7\u05d9\u05d3\u05d1", res);
        }

        [Test]
        public void Test06B_Mirroring_English()
        {
            const string orig = "test (mirror) behaviour";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("test (mirror) behaviour", res);
        }

        [Test]
        public void Test07_Single_Paragraph_Separator_Multiple_Lines()
        {
            string orig = "he said '\u05d6\u05d5\u05d4\u05d9 \u05de\u05db\u05d5\u05e0\u05d9\u05ea'";
            orig += '\n'; // Add a single paragraph separator
            orig += "\u05d4\u05d1\u05d8\u05d8\u05d5\u05ea \u05d2\u05d3\u05dc\u05d5\u05ea \u05d1\u05de\u05d9\u05e9\u05d5\u05e8\u05d9\u05dd, \u05d4\u05d0\u05d2\u05d5\u05d6\u05d9\u05dd \u05d1\u05d4\u05e8\u05d9\u05dd.";
            // Note the string does not end with a paragraph separator.

            string expected = "he said '\u05ea\u05d9\u05e0\u05d5\u05db\u05de \u05d9\u05d4\u05d5\u05d6'";
            expected += '\n';
            expected += ".\u05dd\u05d9\u05e8\u05d4\u05d1 \u05dd\u05d9\u05d6\u05d5\u05d2\u05d0\u05d4 ,\u05dd\u05d9\u05e8\u05d5\u05e9\u05d9\u05de\u05d1 \u05ea\u05d5\u05dc\u05d3\u05d2 \u05ea\u05d5\u05d8\u05d8\u05d1\u05d4";
            string res = NBidi.LogicalToVisual(orig);

            CompareStrings(expected, res);
        }

        [Test]
        public void Test08_Single_Paragraph_Separator_Multiple_Lines()
        {
            string orig = "he said '\u05d6\u05d5\u05d4\u05d9 \u05de\u05db\u05d5\u05e0\u05d9\u05ea'";
            orig += '\r'; // Add a single paragraph separator
            orig += "\u05d4\u05d1\u05d8\u05d8\u05d5\u05ea \u05d2\u05d3\u05dc\u05d5\u05ea \u05d1\u05de\u05d9\u05e9\u05d5\u05e8\u05d9\u05dd, \u05d4\u05d0\u05d2\u05d5\u05d6\u05d9\u05dd \u05d1\u05d4\u05e8\u05d9\u05dd.";
            // Note the string does not end with a paragraph separator.

            string expected = "he said '\u05ea\u05d9\u05e0\u05d5\u05db\u05de \u05d9\u05d4\u05d5\u05d6'";
            expected += '\r';
            expected += ".\u05dd\u05d9\u05e8\u05d4\u05d1 \u05dd\u05d9\u05d6\u05d5\u05d2\u05d0\u05d4 ,\u05dd\u05d9\u05e8\u05d5\u05e9\u05d9\u05de\u05d1 \u05ea\u05d5\u05dc\u05d3\u05d2 \u05ea\u05d5\u05d8\u05d8\u05d1\u05d4";
            string res = NBidi.LogicalToVisual(orig);

            CompareStrings(expected, res);
        }

        [Test]
        public void Test09_Single_Paragraph_Separator()
        {
            string orig = "he said '\u05d6\u05d5\u05d4\u05d9 \u05de\u05db\u05d5\u05e0\u05d9\u05ea'";
            orig += '\n'; // Add a single paragraph separator

            string expected = "he said '\u05ea\u05d9\u05e0\u05d5\u05db\u05de \u05d9\u05d4\u05d5\u05d6'";
            expected += '\n';
            string res = NBidi.LogicalToVisual(orig);

            CompareStrings(expected, res);
        }

        [Test]
        public void Test10_Single_Paragraph_Separator_English_Only()
        {
            string orig = "he said 'This is a car'";
            orig += '\n'; // Add a single paragraph separator

            string expected = "he said 'This is a car'";
            expected += '\n';
            string res = NBidi.LogicalToVisual(orig);

            CompareStrings(expected, res);
        }

        [Test]
        public void Test11_Single_Paragraph_Separator_Hebrew_Only()
        {
            string orig = "\u05d4\u05d1\u05d8\u05d8\u05d5\u05ea \u05d2\u05d3\u05dc\u05d5\u05ea \u05d1\u05de\u05d9\u05e9\u05d5\u05e8\u05d9\u05dd, \u05d4\u05d0\u05d2\u05d5\u05d6\u05d9\u05dd \u05d1\u05d4\u05e8\u05d9\u05dd.";
            orig += '\r'; // Add a single paragraph separator

            string expected = ".\u05dd\u05d9\u05e8\u05d4\u05d1 \u05dd\u05d9\u05d6\u05d5\u05d2\u05d0\u05d4 ,\u05dd\u05d9\u05e8\u05d5\u05e9\u05d9\u05de\u05d1 \u05ea\u05d5\u05dc\u05d3\u05d2 \u05ea\u05d5\u05d8\u05d8\u05d1\u05d4";
            expected += '\r';

            string res = NBidi.LogicalToVisual(orig);

            CompareStrings(expected, res);
        }

        [Test]
        public void Test12_Multiple_Paragraph_Separators()
        {
            string orig = "he said '\u05d6\u05d5\u05d4\u05d9 \u05de\u05db\u05d5\u05e0\u05d9\u05ea'";
            orig += '\n'; // Add a single paragraph separator
            orig += "\u05d4\u05d1\u05d8\u05d8\u05d5\u05ea \u05d2\u05d3\u05dc\u05d5\u05ea \u05d1\u05de\u05d9\u05e9\u05d5\u05e8\u05d9\u05dd, \u05d4\u05d0\u05d2\u05d5\u05d6\u05d9\u05dd \u05d1\u05d4\u05e8\u05d9\u05dd.";
            orig += '\r'; // Add a different single paragraph separator
            orig += "\u05d4\u05d1\u05d8\u05d8\u05d5\u05ea \u05d2\u05d3\u05dc\u05d5\u05ea \u05d1\u05de\u05d9\u05e9\u05d5\u05e8\u05d9\u05dd, The nuts \u05d1\u05d4\u05e8\u05d9\u05dd.";
            // Note the string does not end with a paragraph separator.

            string expected = "he said '\u05ea\u05d9\u05e0\u05d5\u05db\u05de \u05d9\u05d4\u05d5\u05d6'";
            expected += '\n';
            expected += ".\u05dd\u05d9\u05e8\u05d4\u05d1 \u05dd\u05d9\u05d6\u05d5\u05d2\u05d0\u05d4 ,\u05dd\u05d9\u05e8\u05d5\u05e9\u05d9\u05de\u05d1 \u05ea\u05d5\u05dc\u05d3\u05d2 \u05ea\u05d5\u05d8\u05d8\u05d1\u05d4";
            expected += '\r';
            expected += ".\u05dd\u05d9\u05e8\u05d4\u05d1 The nuts ,\u05dd\u05d9\u05e8\u05d5\u05e9\u05d9\u05de\u05d1 \u05ea\u05d5\u05dc\u05d3\u05d2 \u05ea\u05d5\u05d8\u05d8\u05d1\u05d4";
            string res = NBidi.LogicalToVisual(orig);

            CompareStrings(expected, res);
        }

        [Test]
        public void Test13_Composition()
        {
            const string orig = "\u0627\u0653";
            string res = NBidi.LogicalToVisual(orig);
            Assert.That(res, Is.EqualTo("\uFE81"));
        }

        [Test]
        public void Test14_ArabicShaping()
        {
            const string orig = "\u0647\u0647\u0647\u0647 \u0647";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("\uFEE9 \uFEEA\uFEEC\uFEEC\uFEEB", res);
        }

        [Test]
        public void Test15_ArabicShaping_Joiners()
        {
            const string orig = "\u0647 \u200D\u0647 \u0647\u200D \u200D\u0647\u200D";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("\u200D\uFEEC\u200D\u0020\u200D\uFEEB\u0020\uFEEA\u200D\u0020\uFEE9", res);
        }

        [Test]
        public void Test16_LRO() //TODO - should the control characters remain in place after bidi?
        {
            const string orig = "\u202D\u05D0\u05D1\u05D2\u202C";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("\u05D0\u05D1\u05D2", res);
        }

        [Test]
        public void Test17_RLO()
        {
            const string orig = "\u202Eabc\u202C";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("cba", res);
        }

        [Test]
        public void Test18_LRE()
        {
            string orig = "\u05E2\u05D1\u05E8\u05D9\u05EA " + BidiChars.LRE + "1 2 3" + BidiChars.PDF + " \u05D1\u05D3\u05D9\u05E7\u05D4";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("\u05D4\u05E7\u05D9\u05D3\u05D1 1 2 3 \u05EA\u05D9\u05E8\u05D1\u05E2", res);
        }

        [Test]
        public void Test19_RLE()
        {
            const string orig = "English \u202B1 2 3\u202C Test";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("English 3 2 1 Test", res);
        }

        [Test]
        public void Test20_Newlines_1()
        {
            string orig = BidiChars.RLM + "Hello World \u05E9\u05DC\u05D5\u05DD1\n\u05E9\u05DC\u05D5\u05DD2 \u05E9\u05DC\u05D5\u05DD3 \u05E9\u05DC\u05D5\u05DD4\n\u05E9\u05DC\u05D5\u05DD5 \u05E9\u05DC\u05D5\u05DD6 \u05E9\u05DC\u05D5\u05DD7";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("1\u05DD\u05D5\u05DC\u05E9 Hello World\n4\u05DD\u05D5\u05DC\u05E9 3\u05DD\u05D5\u05DC\u05E9 2\u05DD\u05D5\u05DC\u05E9\n7\u05DD\u05D5\u05DC\u05E9 6\u05DD\u05D5\u05DC\u05E9 5\u05DD\u05D5\u05DC\u05E9", res);
        }

        [Test]
        public void Test21_Newlines_2()
        {
            const string orig = "Hello World\n\u05E9\u05DC\u05D5\u05DD1 \u05E9\u05DC\u05D5\u05DD2\n\u05E9\u05DC\u05D5\u05DD3 \u05E9\u05DC\u05D5\u05DD4\n\u05E9\u05DC\u05D5\u05DD5 \u05E9\u05DC\u05D5\u05DD6\n\u05E9\u05DC\u05D5\u05DD7\n";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("Hello World\n2\u05DD\u05D5\u05DC\u05E9 1\u05DD\u05D5\u05DC\u05E9\n4\u05DD\u05D5\u05DC\u05E9 3\u05DD\u05D5\u05DC\u05E9\n6\u05DD\u05D5\u05DC\u05E9 5\u05DD\u05D5\u05DC\u05E9\n7\u05DD\u05D5\u05DC\u05E9\n", res);
        }

        [Test]
        public void Test22_Indexes_1_NoComposition()
        {
            const string orig = "Hello World \u05E9\u05DC\u05D5\u05DD \u05E2\u05D5\u05DC\u05DD";
            string res = NBidi.LogicalToVisual(orig);
            //HELLO WORLD slom olam
            //012345678901234567890
            CompareStrings("Hello World \u05DD\u05DC\u05D5\u05E2 \u05DD\u05D5\u05DC\u05E9", res);
        }

        [Test]
        public void Test23_Indexes_2_NoComposition_RLM()
        {
            string orig = BidiChars.RLM + "Hello World \u05E9\u05DC\u05D5\u05DD \u05E2\u05D5\u05DC\u05DD";
            string res = NBidi.LogicalToVisual(orig);
            // HELLO WORLD slom olam
            //0123456789012345678901
            CompareStrings("\u05DD\u05DC\u05D5\u05E2 \u05DD\u05D5\u05DC\u05E9 Hello World", res);
        }

        [Test]
        public void Test23_Indexes_3_Composition()
        {
            const string orig = "Hello World \u0628\u0644\u0622 \u0644\u0627\u0653";
            string res = NBidi.LogicalToVisual(orig);
            //HELLO WORLD abc def
            //0123456789012345678

            // abc -> xy
            // def -> z

            //HELLO WORLD z-- y-x
            //0123456789012--34-5

            CompareStrings("Hello World \uFEF5 \uFEF6\uFE91", res);
        }

        [Test]
        public void Test24_Decomposition_and_Recomposition()
        {
            const string orig = "\u200F\uFE9F\uFE77\uFE82\uFEE7\uFE7B\uFEF0";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("\uFEF0\u0650\u0640\uFEE7\uFE82\u064E\u0640\uFE9F", res);
        }

        [Test]
        public void Test25_Numbers_and_Hebrew()
        {
            const string orig = "123.45 \u05E9\u05DC\u05D5\u05DD \u05E2\u05D5\u05DC\u05DD 678.90";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("678.90 \u05DD\u05DC\u05D5\u05E2 \u05DD\u05D5\u05DC\u05E9 123.45", res);
        }

        [Test]
        public void Test26_RTL_Numbers()
        {
            const string orig = "\u05E2\u05D1\u05E8\u05D9\u05EA 1 2 3 \u05D1\u05D3\u05D9\u05E7\u05D4";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("\u05D4\u05E7\u05D9\u05D3\u05D1 3 2 1 \u05EA\u05D9\u05E8\u05D1\u05E2", res);
        }

        [Test]
        public void Test27_LRE_Letters_Only()
        {
            string orig = "\u05E2\u05D1\u05E8\u05D9\u05EA " + BidiChars.LRE + "A B C" + BidiChars.PDF + " \u05D1\u05D3\u05D9\u05E7\u05D4";
            string res = NBidi.LogicalToVisual(orig);
            CompareStrings("\u05D4\u05E7\u05D9\u05D3\u05D1 A B C \u05EA\u05D9\u05E8\u05D1\u05E2", res);
        }

        private static void CompareStrings(string expected, string actual)
        {
            Assert.That(AsCharArray(actual), Is.EqualTo(AsCharArray(expected)));
        }

        private static string AsCharArray(string str)
        {
            StringBuilder result = new StringBuilder();
            foreach (char c in str)
            {
                if (result.Length > 0)
                {
                    result.Append(' ');
                }
                result.Append(FormatterPal.IntToStrX4(c));
            }
            return result.ToString();
        }
    }
}
