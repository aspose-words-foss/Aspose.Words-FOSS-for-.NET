// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/04/2015 by Roman Korchagin

// NBidi - a .Net implementation of the BIDI (Bi-Directional Text) algorithm.

using Aspose.Common;
using NUnit.Framework;

namespace Aspose.Tests.Bidi
{
    [TestFixture]
    public class TestNBidiArabicShaping
    {
        [Test]
        public void Test01ArabicShapingWithTashkeel()
        {
            //Tashkeel characters are a set of glyphs that controls how a word is pronounced, but not part of the word
            //Tashkeel characters appear above or below a character and should not affect joining (irrelevant to joining)
            //Tashkeel characters are:
            //Fathatan \u064B
            //Dammatan \u064c
            //Kasratan \u064D
            //Fatha \u064E
            //Damma \u064F
            //Kasra \u0650
            //Shadda \u0651
            //Sukun \u0652
            const string orig = "\u0639\u0631\u0628\u0650\u064A";
            string res = Aspose.Bidi.NBidi.LogicalToVisual(orig);
            const string exp = "\uFEF2\u0650\uFE91\uFEAE\uFECB";
            Assert.That(res.ToCharArray(), Is.EqualTo(exp.ToCharArray()), string.Format(
                "original: \"{0}\"\noriginal as chars: {1}\nresult as chars:   {2}\nexpected as chars: {3}",
                orig, AsCharArray(orig), AsCharArray(res), AsCharArray(exp)));
        }

        //It is required that squences of Lam and Alef characters, converted to a single presentation character Lamalef
        [Test]
        public void Test02ArabicShapingBasicLamAlef()
        {
            const string orig = "\u0628\u0644\u0627 \u0644\u0627";
            string res = Aspose.Bidi.NBidi.LogicalToVisual(orig);
            const string exp = "\uFEFB \uFEFC\uFE91";
            Assert.That(res.ToCharArray(), Is.EqualTo(exp.ToCharArray()), string.Format(
                "original: \"{0}\"\noriginal as chars: {1}\nresult as chars:   {2}\nexpected as chars: {3}",
                orig, AsCharArray(orig), AsCharArray(res), AsCharArray(exp)));
        }

        [Test]
        public void Test03ArabicShapingLamAlefAbove()
        {
            const string orig = "\u0628\u0644\u0623 \u0644\u0623";
            string res = Aspose.Bidi.NBidi.LogicalToVisual(orig);
            const string exp = "\uFEF7 \uFEF8\uFE91";
            Assert.That(res.ToCharArray(), Is.EqualTo(exp.ToCharArray()), string.Format(
                "original: \"{0}\"\noriginal as chars: {1}\nresult as chars:   {2}\nexpected as chars: {3}",
                orig, AsCharArray(orig), AsCharArray(res), AsCharArray(exp)));
        }

        [Test]
        public void Test04ArabicShapingWithLamAlefBelow()
        {
            const string orig = "\u0628\u0644\u0625 \u0644\u0625";
            string res = Aspose.Bidi.NBidi.LogicalToVisual(orig);
            const string exp = "\uFEF9 \uFEFA\uFE91";
            Assert.That(res.ToCharArray(), Is.EqualTo(exp.ToCharArray()), string.Format(
                "original: \"{0}\"\noriginal as chars: {1}\nresult as chars:   {2}\nexpected as chars: {3}",
                orig, AsCharArray(orig), AsCharArray(res), AsCharArray(exp)));
        }

        [Test]
        public void Test05ArabicShapingWithLamAlefMaddaAbove()
        {
            const string orig = "\u0628\u0644\u0622 \u0644\u0622";
            string res = Aspose.Bidi.NBidi.LogicalToVisual(orig);
            const string exp = "\uFEF5 \uFEF6\uFE91";
            Assert.That(res.ToCharArray(), Is.EqualTo(exp.ToCharArray()), string.Format(
                "original: \"{0}\"\noriginal as chars: {1}\nresult as chars:   {2}\nexpected as chars: {3}",
                orig, AsCharArray(orig), AsCharArray(res), AsCharArray(exp)));
        }
            
        [Test]
        public void Test06ArabicShapingTashkil()
        {
            //meem-initial madda ha-medial meem-medial dal-final
            const string orig = "\u0645\u064F\u062D\u0645\u062F";
            string res = Aspose.Bidi.NBidi.LogicalToVisual(orig);
            const string exp = "\uFEAA\uFEE4\uFEA4\u064F\uFEE3";
            Assert.That(res.ToCharArray(), Is.EqualTo(exp.ToCharArray()), string.Format(
                "original: \"{0}\"\noriginal as chars: {1}\nresult as chars:   {2}\nexpected as chars: {3}",
                orig, AsCharArray(orig), AsCharArray(res), AsCharArray(exp)));
        }

        [Test]
        public void Test07ArabicAlefLamKafIndexing()
        {
            //alef lam kaf
            const string orig = "\u0627\u0644\u0643";
            string res = Aspose.Bidi.NBidi.LogicalToVisual(orig);
            const string exp = "\uFEDA\uFEDF\uFE8D";

            Assert.That(res.ToCharArray(), Is.EqualTo(exp.ToCharArray()), string.Format(
                "original: \"{0}\"\noriginal as chars: {1}\nresult as chars:   {2}\nexpected as chars: {3}",
                orig, AsCharArray(orig), AsCharArray(res), AsCharArray(exp)));
        }

        [Test]
        public void Test08ArabicLamAlefKafIndexing()
        {
            //lam alef kaf
            const string orig = "\u0644\u0627\u0643";
            string res = Aspose.Bidi.NBidi.LogicalToVisual(orig);
            const string exp = "\uFED9\uFEFB";

            Assert.That(res.ToCharArray(), Is.EqualTo(exp.ToCharArray()), string.Format(
                "original: \"{0}\"\noriginal as chars: {1}\nresult as chars:   {2}\nexpected as chars: {3}",
                orig, AsCharArray(orig), AsCharArray(res), AsCharArray(exp)));
        }

        private static string AsCharArray(string str)
        {
            string testRes = string.Empty;
            for (int i = 0; i < str.Length; i++)
                testRes += @"\u" + FormatterPal.IntToStrX4(str[i]);

            return testRes;
        }
    }
}
