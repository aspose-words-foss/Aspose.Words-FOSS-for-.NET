// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.Text;
using Aspose.Numbering;
using Aspose.Words.Lists;
using Aspose.Words.Markup;
using Aspose.Words.Saving;
using NUnit.Framework;
using List = Aspose.Words.Lists.List;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Tests concerning numbering which are independent on file formats though they may read files to build valid model.
    /// </summary>
    [TestFixture]
    public class TestNumbering
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [Test]
        public void TestKanjiNumbering()
        {
            Document doc = TestUtil.Open(@"Model\List\TestKanjiNumbering.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[0].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.Kanji));

            Assert.That(ListLabel(paras, 0), Is.EqualTo("〇.")); // 0
            Assert.That(ListLabel(paras, 1), Is.EqualTo("一.")); // 1
            Assert.That(ListLabel(paras, 2), Is.EqualTo("二.")); // 2
            Assert.That(ListLabel(paras, 3), Is.EqualTo("三.")); // 3
            Assert.That(ListLabel(paras, 4), Is.EqualTo("四.")); // 4
            Assert.That(ListLabel(paras, 5), Is.EqualTo("五.")); // 5
            Assert.That(ListLabel(paras, 6), Is.EqualTo("六.")); // 6
            Assert.That(ListLabel(paras, 7), Is.EqualTo("七.")); // 7
            Assert.That(ListLabel(paras, 8), Is.EqualTo("八.")); // 8
            Assert.That(ListLabel(paras, 9), Is.EqualTo("九.")); // 9
            Assert.That(ListLabel(paras, 10), Is.EqualTo("一〇.")); // 10
            Assert.That(ListLabel(paras, 11), Is.EqualTo("一一.")); // 11
            Assert.That(ListLabel(paras, 12), Is.EqualTo("二一.")); // 21
            Assert.That(ListLabel(paras, 13), Is.EqualTo("一二一.")); // 121
            Assert.That(ListLabel(paras, 14), Is.EqualTo("二二一.")); // 221
            Assert.That(ListLabel(paras, 15), Is.EqualTo("一二三四.")); // 1234
            Assert.That(ListLabel(paras, 16), Is.EqualTo("二三四五.")); // 2345
            Assert.That(ListLabel(paras, 17), Is.EqualTo("一二三四五.")); // 12345
            Assert.That(ListLabel(paras, 18), Is.EqualTo("二三四五六.")); // 23456
            Assert.That(ListLabel(paras, 19), Is.EqualTo("三二七六七.")); // 32767
        }

        [Test]
        public void TestBigKanjiNumbering()
        {
            const int mrd = 10000;
            Assert.That(JapaneseNumber.ToJapaneseCounting(983 * mrd + 6703), Is.EqualTo("九百八十三万六千七百三"));
            Assert.That(JapaneseNumber.ToJapaneseCounting(20 * mrd * mrd + 3652 * mrd + 1801), Is.EqualTo("二十億三千六百五十二万千八百一"));
        }

        [Test]
        public void TestKanjiDigitNumbering()
        {
            Document doc = TestUtil.Open(@"Model\List\TestKanjiDigitNumbering.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[0].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.KanjiDigit));

            Assert.That(ListLabel(paras, 0), Is.EqualTo("〇.")); // 0
            Assert.That(ListLabel(paras, 1), Is.EqualTo("一.")); // 1
            Assert.That(ListLabel(paras, 2), Is.EqualTo("二.")); // 2
            Assert.That(ListLabel(paras, 3), Is.EqualTo("三.")); // 3
            Assert.That(ListLabel(paras, 4), Is.EqualTo("四.")); // 4
            Assert.That(ListLabel(paras, 5), Is.EqualTo("五.")); // 5
            Assert.That(ListLabel(paras, 6), Is.EqualTo("六.")); // 6
            Assert.That(ListLabel(paras, 7), Is.EqualTo("七.")); // 7
            Assert.That(ListLabel(paras, 8), Is.EqualTo("八.")); // 8
            Assert.That(ListLabel(paras, 9), Is.EqualTo("九.")); // 9
            Assert.That(ListLabel(paras, 10), Is.EqualTo("十.")); // 10
            Assert.That(ListLabel(paras, 11), Is.EqualTo("十一.")); // 11
            Assert.That(ListLabel(paras, 12), Is.EqualTo("二十一.")); // 21
            Assert.That(ListLabel(paras, 13), Is.EqualTo("百二十一.")); // 121
            Assert.That(ListLabel(paras, 14), Is.EqualTo("二百二十一.")); // 221
            Assert.That(ListLabel(paras, 15), Is.EqualTo("千二百三十四.")); // 1234
            Assert.That(ListLabel(paras, 16), Is.EqualTo("二千三百四十五.")); // 2345
            Assert.That(ListLabel(paras, 17), Is.EqualTo("一万二千三百四十五.")); // 12345
            Assert.That(ListLabel(paras, 18), Is.EqualTo("二万三千四百五十六.")); // 23456
            Assert.That(ListLabel(paras, 19), Is.EqualTo("三万二千七百六十七.")); // 32767
        }


        /// <summary>
        /// WORDSNET-4628 <see cref="NumberStyle.ArabicFullWidth"/> numbering style was not supported.
        /// </summary>
        [Test]
        public void TestArabicFullWidthNumbering()
        {
            Document doc = TestUtil.Open("Model/List/TestArabicFullWidthNumbering.docx");
            doc.UpdateListLabels();

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paragraphs, 0), Is.EqualTo("０．"));
            Assert.That(ListLabel(paragraphs, 1), Is.EqualTo("１．"));
            Assert.That(ListLabel(paragraphs, 2), Is.EqualTo("２．"));
            Assert.That(ListLabel(paragraphs, 3), Is.EqualTo("３．"));
            Assert.That(ListLabel(paragraphs, 4), Is.EqualTo("４．"));
            Assert.That(ListLabel(paragraphs, 5), Is.EqualTo("５．"));
            Assert.That(ListLabel(paragraphs, 6), Is.EqualTo("６．"));
            Assert.That(ListLabel(paragraphs, 7), Is.EqualTo("７．"));
            Assert.That(ListLabel(paragraphs, 8), Is.EqualTo("８．"));
            Assert.That(ListLabel(paragraphs, 9), Is.EqualTo("９．"));
            Assert.That(ListLabel(paragraphs, 10), Is.EqualTo("１０．"));
            Assert.That(ListLabel(paragraphs, 11), Is.EqualTo("３２７６７．"));
        }

        /// <summary>
        /// WORDSNET-4628 <see cref="NumberStyle.Aiueo"/> numbering style was not supported.
        /// WORDSNET-26576 <see cref="NumberStyle.AiueoHalfWidth"/> numbering style was not supported.
        /// </summary>
        [TestCase("TestAiueoNumbering",  false)]
        [TestCase("TestAiueoHalfwidthNumbering", true)]
        public void TestAiueoNumbering(string document, bool isHalfWidth)
        {
            Document doc = TestUtil.Open("Model/List/" + document + ".docx");
            doc.UpdateListLabels();

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            string AiueoNumbers = "0アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲンアイウエアソ";
            string AiueoHalfWidthNumbers = "0ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜｦﾝｱｲｳｴｱｿ";
            string numbers = isHalfWidth ? AiueoHalfWidthNumbers : AiueoNumbers;

            const int NumberOfParagraphs = 53;
            for (int i =0; i < NumberOfParagraphs; i++)
                Assert.That(ListLabel(paragraphs, i), Is.EqualTo("(" + numbers[i] + ")"));
        }

        /// <summary>
        /// WORDSNET-4628 <see cref="NumberStyle.NumberInCircle"/> numbering style was not supported.
        /// </summary>
        [Test]
        public void TestNumberInCircleNumbering()
        {
            Document doc = TestUtil.Open("Model/List/TestNumberInCircleNumbering.docx");
            doc.UpdateListLabels();

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paragraphs, 0), Is.EqualTo("①"));
            Assert.That(ListLabel(paragraphs, 1), Is.EqualTo("②"));
            Assert.That(ListLabel(paragraphs, 2), Is.EqualTo("③"));
            Assert.That(ListLabel(paragraphs, 3), Is.EqualTo("④"));
            Assert.That(ListLabel(paragraphs, 4), Is.EqualTo("⑤"));
            Assert.That(ListLabel(paragraphs, 5), Is.EqualTo("⑥"));
            Assert.That(ListLabel(paragraphs, 6), Is.EqualTo("⑦"));
            Assert.That(ListLabel(paragraphs, 7), Is.EqualTo("⑧"));
            Assert.That(ListLabel(paragraphs, 8), Is.EqualTo("⑨"));
            Assert.That(ListLabel(paragraphs, 9), Is.EqualTo("⑩"));
            Assert.That(ListLabel(paragraphs, 10), Is.EqualTo("⑪"));
            Assert.That(ListLabel(paragraphs, 11), Is.EqualTo("⑫"));
            Assert.That(ListLabel(paragraphs, 12), Is.EqualTo("⑬"));
            Assert.That(ListLabel(paragraphs, 13), Is.EqualTo("⑭"));
            Assert.That(ListLabel(paragraphs, 14), Is.EqualTo("⑮"));
            Assert.That(ListLabel(paragraphs, 15), Is.EqualTo("⑯"));
            Assert.That(ListLabel(paragraphs, 16), Is.EqualTo("⑰"));
            Assert.That(ListLabel(paragraphs, 17), Is.EqualTo("⑱"));
            Assert.That(ListLabel(paragraphs, 18), Is.EqualTo("⑲"));
            Assert.That(ListLabel(paragraphs, 19), Is.EqualTo("⑳"));
            Assert.That(ListLabel(paragraphs, 20), Is.EqualTo("21"));
        }

        /// <summary>
        /// WORDSNET-4628 <see cref="NumberStyle.Iroha"/> numbering style was not supported.
        /// </summary>
        [Test]
        public void TestIrohaNumbering()
        {
            Document doc = TestUtil.Open("Model/List/TestIrohaNumbering.docx");
            doc.UpdateListLabels();

            ParagraphCollection paragraphs = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paragraphs, 0), Is.EqualTo("0)")); // 0
            Assert.That(ListLabel(paragraphs, 1), Is.EqualTo("イ)")); // 1
            Assert.That(ListLabel(paragraphs, 2), Is.EqualTo("ロ)")); // ...
            Assert.That(ListLabel(paragraphs, 3), Is.EqualTo("ハ)"));
            Assert.That(ListLabel(paragraphs, 4), Is.EqualTo("ニ)"));
            Assert.That(ListLabel(paragraphs, 5), Is.EqualTo("ホ)"));
            Assert.That(ListLabel(paragraphs, 6), Is.EqualTo("ヘ)"));
            Assert.That(ListLabel(paragraphs, 7), Is.EqualTo("ト)"));
            Assert.That(ListLabel(paragraphs, 8), Is.EqualTo("チ)"));
            Assert.That(ListLabel(paragraphs, 9), Is.EqualTo("リ)"));
            Assert.That(ListLabel(paragraphs, 10), Is.EqualTo("ヌ)"));
            Assert.That(ListLabel(paragraphs, 11), Is.EqualTo("ル)"));
            Assert.That(ListLabel(paragraphs, 12), Is.EqualTo("ヲ)"));
            Assert.That(ListLabel(paragraphs, 13), Is.EqualTo("ワ)"));
            Assert.That(ListLabel(paragraphs, 14), Is.EqualTo("カ)"));
            Assert.That(ListLabel(paragraphs, 15), Is.EqualTo("ヨ)"));
            Assert.That(ListLabel(paragraphs, 16), Is.EqualTo("タ)"));
            Assert.That(ListLabel(paragraphs, 17), Is.EqualTo("レ)"));
            Assert.That(ListLabel(paragraphs, 18), Is.EqualTo("ソ)"));
            Assert.That(ListLabel(paragraphs, 19), Is.EqualTo("ツ)"));
            Assert.That(ListLabel(paragraphs, 20), Is.EqualTo("ネ)"));
            Assert.That(ListLabel(paragraphs, 21), Is.EqualTo("ナ)"));
            Assert.That(ListLabel(paragraphs, 22), Is.EqualTo("ラ)"));
            Assert.That(ListLabel(paragraphs, 23), Is.EqualTo("ム)"));
            Assert.That(ListLabel(paragraphs, 24), Is.EqualTo("ウ)"));
            Assert.That(ListLabel(paragraphs, 25), Is.EqualTo("ヰ)"));
            Assert.That(ListLabel(paragraphs, 26), Is.EqualTo("ノ)"));
            Assert.That(ListLabel(paragraphs, 27), Is.EqualTo("オ)"));
            Assert.That(ListLabel(paragraphs, 28), Is.EqualTo("ク)"));
            Assert.That(ListLabel(paragraphs, 29), Is.EqualTo("ヤ)"));
            Assert.That(ListLabel(paragraphs, 30), Is.EqualTo("マ)"));
            Assert.That(ListLabel(paragraphs, 31), Is.EqualTo("ケ)"));
            Assert.That(ListLabel(paragraphs, 32), Is.EqualTo("フ)"));
            Assert.That(ListLabel(paragraphs, 33), Is.EqualTo("コ)"));
            Assert.That(ListLabel(paragraphs, 34), Is.EqualTo("エ)"));
            Assert.That(ListLabel(paragraphs, 35), Is.EqualTo("テ)"));
            Assert.That(ListLabel(paragraphs, 36), Is.EqualTo("ア)"));
            Assert.That(ListLabel(paragraphs, 37), Is.EqualTo("サ)"));
            Assert.That(ListLabel(paragraphs, 38), Is.EqualTo("キ)"));
            Assert.That(ListLabel(paragraphs, 39), Is.EqualTo("ユ)"));
            Assert.That(ListLabel(paragraphs, 40), Is.EqualTo("メ)"));
            Assert.That(ListLabel(paragraphs, 41), Is.EqualTo("ミ)"));
            Assert.That(ListLabel(paragraphs, 42), Is.EqualTo("シ)"));
            Assert.That(ListLabel(paragraphs, 43), Is.EqualTo("ヱ)"));
            Assert.That(ListLabel(paragraphs, 44), Is.EqualTo("ヒ)"));
            Assert.That(ListLabel(paragraphs, 45), Is.EqualTo("モ)"));
            Assert.That(ListLabel(paragraphs, 46), Is.EqualTo("セ)"));
            Assert.That(ListLabel(paragraphs, 47), Is.EqualTo("ス)"));
            Assert.That(ListLabel(paragraphs, 48), Is.EqualTo("ン)"));
            Assert.That(ListLabel(paragraphs, 49), Is.EqualTo("イ)")); // 49
            Assert.That(ListLabel(paragraphs, 50), Is.EqualTo("ロ)")); // 50
            Assert.That(ListLabel(paragraphs, 51), Is.EqualTo("イ)")); // 481
            Assert.That(ListLabel(paragraphs, 52), Is.EqualTo("ケ)")); // 32767
        }

        [Test]
        public void TestRomanNumbering()
        {
            Document doc = TestUtil.Open("Model/List/TestRomanNumbering.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            int index = 0;

            Assert.That(ListLabel(paras, index++), Is.EqualTo("i."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("ii."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("iii."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("iv."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("v."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("vi."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("vii."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("viii."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("ix."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("x."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("xi."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("xx."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("xxx."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("xl."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("xlix."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("l."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("lx."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("lxx."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("lxxx."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("xc."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("xcix."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("c."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("cc."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("ccc."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("cd."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("d."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("dc."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("dcc."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("dcclxvii."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("dccc."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("cm."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("m."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("mmmmmmmmmcmxcix."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("mmmmmmmmmm."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("mmmmmmmmmmmmmmmmmmmm."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("mmmmmmmmmmmmmmmmmmmmmmmmmmmmmm."));
            Assert.That(ListLabel(paras, index), Is.EqualTo("mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmdcclxvii."));
        }

        [Test]
        public void TestKanjiWithLeadingZeroes()
        {
            Assert.That(JapaneseNumber.ToJapaneseDigital(12, 5), Is.EqualTo("〇〇〇一二"));
        }


        /// <summary>
        /// Added support for <see cref="NumberStyle.TradChinNum2"/> numbering style.
        /// </summary>
        [Test]
        public void TestIdeographLegalTraditionalNumbering()
        {
            Document doc = TestUtil.Open(@"Model\List\TestIdeographLegalTraditional.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[0].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.TradChinNum2));

            Assert.That(ListLabel(paras, 0), Is.EqualTo("壹、")); // 1
            Assert.That(ListLabel(paras, 1), Is.EqualTo("貳、")); // 2
            Assert.That(ListLabel(paras, 2), Is.EqualTo("參、")); // 3
            Assert.That(ListLabel(paras, 3), Is.EqualTo("肆、")); // 4
            Assert.That(ListLabel(paras, 4), Is.EqualTo("伍、")); // 5
            Assert.That(ListLabel(paras, 5), Is.EqualTo("陸、")); // 6
            Assert.That(ListLabel(paras, 6), Is.EqualTo("柒、")); // 7
            Assert.That(ListLabel(paras, 7), Is.EqualTo("捌、")); // 8
            Assert.That(ListLabel(paras, 8), Is.EqualTo("玖、")); // 9
            Assert.That(ListLabel(paras, 9), Is.EqualTo("壹拾、")); // 10
            Assert.That(ListLabel(paras, 10), Is.EqualTo("壹拾壹、")); // 11
            Assert.That(ListLabel(paras, 11), Is.EqualTo("貳拾、")); // 20
            Assert.That(ListLabel(paras, 12), Is.EqualTo("貳拾壹、")); // 21
            Assert.That(ListLabel(paras, 13), Is.EqualTo("伍拾、")); // 50
            Assert.That(ListLabel(paras, 14), Is.EqualTo("壹佰、")); // 100
            Assert.That(ListLabel(paras, 15), Is.EqualTo("壹佰零壹、")); // 101
            Assert.That(ListLabel(paras, 16), Is.EqualTo("壹佰壹拾壹、")); // 111
            Assert.That(ListLabel(paras, 17), Is.EqualTo("壹佰貳拾壹、")); // 121
            Assert.That(ListLabel(paras, 18), Is.EqualTo("伍佰、")); // 500
            Assert.That(ListLabel(paras, 19), Is.EqualTo("壹仟、")); // 1000
            Assert.That(ListLabel(paras, 20), Is.EqualTo("壹仟零壹、")); // 1001
            Assert.That(ListLabel(paras, 21), Is.EqualTo("壹萬、")); // 10000
            Assert.That(ListLabel(paras, 22), Is.EqualTo("壹萬零壹、")); // 10001
            Assert.That(ListLabel(paras, 23), Is.EqualTo("壹萬零壹拾壹、")); // 10011
            Assert.That(ListLabel(paras, 24), Is.EqualTo("壹萬零壹佰零壹、")); // 10101
            Assert.That(ListLabel(paras, 25), Is.EqualTo("壹萬零壹佰壹拾壹、")); // 10111
            Assert.That(ListLabel(paras, 26), Is.EqualTo("壹萬壹仟壹佰壹拾壹、")); // 11111
            Assert.That(ListLabel(paras, 27), Is.EqualTo("壹萬參仟肆佰伍拾陸、")); // 13456
            Assert.That(ListLabel(paras, 28), Is.EqualTo("貳萬柒仟捌佰玖拾、")); // 27890
            Assert.That(ListLabel(paras, 29), Is.EqualTo("參萬貳仟柒佰陸拾柒、")); // 32767
            Assert.That(ListLabel(paras, 30), Is.EqualTo("參萬貳仟柒佰陸拾捌、")); // 32768
        }

        /// <summary>
        /// Added support for <see cref="NumberStyle.TradChinNum3"/> numbering style.
        /// </summary>
        [Test]
        public void TestTaiwaneseCountingThousandNumbering()
        {
            Document doc = TestUtil.Open(@"Model\List\TestTaiwaneseCountingThousand.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[0].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.TradChinNum3));

            Assert.That(ListLabel(paras, 0), Is.EqualTo("一、")); // 1
            Assert.That(ListLabel(paras, 1), Is.EqualTo("二、")); // 2
            Assert.That(ListLabel(paras, 2), Is.EqualTo("三、")); // 3
            Assert.That(ListLabel(paras, 3), Is.EqualTo("四、")); // 4
            Assert.That(ListLabel(paras, 4), Is.EqualTo("五、")); // 5
            Assert.That(ListLabel(paras, 5), Is.EqualTo("六、")); // 6
            Assert.That(ListLabel(paras, 6), Is.EqualTo("七、")); // 7
            Assert.That(ListLabel(paras, 7), Is.EqualTo("八、")); // 8
            Assert.That(ListLabel(paras, 8), Is.EqualTo("九、")); // 9
            Assert.That(ListLabel(paras, 9), Is.EqualTo("十、")); // 10
            Assert.That(ListLabel(paras, 10), Is.EqualTo("十一、")); // 11
            Assert.That(ListLabel(paras, 11), Is.EqualTo("二十、")); // 20
            Assert.That(ListLabel(paras, 12), Is.EqualTo("二十一、")); // 21
            Assert.That(ListLabel(paras, 13), Is.EqualTo("五十、")); // 50
            Assert.That(ListLabel(paras, 14), Is.EqualTo("一百、")); // 100
            Assert.That(ListLabel(paras, 15), Is.EqualTo("一百零一、")); // 101
            Assert.That(ListLabel(paras, 16), Is.EqualTo("一百一十一、")); // 111
            Assert.That(ListLabel(paras, 17), Is.EqualTo("一百二十一、")); // 121
            Assert.That(ListLabel(paras, 18), Is.EqualTo("五百、")); // 121
            Assert.That(ListLabel(paras, 19), Is.EqualTo("一千、")); // 1000
            Assert.That(ListLabel(paras, 20), Is.EqualTo("一千零一、")); // 1001
            Assert.That(ListLabel(paras, 21), Is.EqualTo("五千、")); // 5000
            Assert.That(ListLabel(paras, 22), Is.EqualTo("一萬、")); // 10000
            Assert.That(ListLabel(paras, 23), Is.EqualTo("一萬零一、")); // 10001
            Assert.That(ListLabel(paras, 24), Is.EqualTo("一萬零五、")); // 10005
            Assert.That(ListLabel(paras, 25), Is.EqualTo("一萬零一十一、")); // 10011
            Assert.That(ListLabel(paras, 26), Is.EqualTo("一萬零一百零一、")); // 10101
            Assert.That(ListLabel(paras, 27), Is.EqualTo("一萬零一百一十一、")); // 10111
            Assert.That(ListLabel(paras, 28), Is.EqualTo("一萬一千一百一十一、")); // 11111
            Assert.That(ListLabel(paras, 29), Is.EqualTo("一萬三千四百五十六、")); // 13456
            Assert.That(ListLabel(paras, 30), Is.EqualTo("二萬七千八百九十、")); // 27890
            Assert.That(ListLabel(paras, 31), Is.EqualTo("三萬二千七百六十七、")); // 32767
            Assert.That(ListLabel(paras, 32), Is.EqualTo("三萬二千七百六十八、")); // 32768
        }

        /// <summary>
        /// Added support for <see cref="NumberStyle.SimpChinNum3"/> numbering style.
        /// </summary>
        [Test]
        public void TestChineseCountingThousandNumbering()
        {
            Document doc = TestUtil.Open(@"Model\List\TestChineseCountingThousand.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[0].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.SimpChinNum3));

            Assert.That(ListLabel(paras, 0), Is.EqualTo("一、")); // 1
            Assert.That(ListLabel(paras, 1), Is.EqualTo("二、")); // 2
            Assert.That(ListLabel(paras, 2), Is.EqualTo("三、")); // 3
            Assert.That(ListLabel(paras, 3), Is.EqualTo("四、")); // 4
            Assert.That(ListLabel(paras, 4), Is.EqualTo("五、")); // 5
            Assert.That(ListLabel(paras, 5), Is.EqualTo("六、")); // 6
            Assert.That(ListLabel(paras, 6), Is.EqualTo("七、")); // 7
            Assert.That(ListLabel(paras, 7), Is.EqualTo("八、")); // 8
            Assert.That(ListLabel(paras, 8), Is.EqualTo("九、")); // 9
            Assert.That(ListLabel(paras, 9), Is.EqualTo("十、")); // 10
            Assert.That(ListLabel(paras, 10), Is.EqualTo("十一、")); // 11
            Assert.That(ListLabel(paras, 11), Is.EqualTo("二十、")); // 20
            Assert.That(ListLabel(paras, 12), Is.EqualTo("二十一、")); // 21
            Assert.That(ListLabel(paras, 13), Is.EqualTo("五十、")); // 50
            Assert.That(ListLabel(paras, 14), Is.EqualTo("一百、")); // 100
            Assert.That(ListLabel(paras, 15), Is.EqualTo("一百〇一、")); // 101
            Assert.That(ListLabel(paras, 16), Is.EqualTo("一百〇五、")); // 105
            Assert.That(ListLabel(paras, 17), Is.EqualTo("一百一十一、")); // 111
            Assert.That(ListLabel(paras, 18), Is.EqualTo("一百二十一、")); // 121
            Assert.That(ListLabel(paras, 19), Is.EqualTo("五百、")); // 500
            Assert.That(ListLabel(paras, 20), Is.EqualTo("一千、")); // 1000
            Assert.That(ListLabel(paras, 21), Is.EqualTo("一千〇一、")); // 1001
            Assert.That(ListLabel(paras, 22), Is.EqualTo("一千〇五、")); // 1005
            Assert.That(ListLabel(paras, 23), Is.EqualTo("五千、")); // 5000
            Assert.That(ListLabel(paras, 24), Is.EqualTo("一万、")); // 10000
            Assert.That(ListLabel(paras, 25), Is.EqualTo("一万〇一、")); // 10001
            Assert.That(ListLabel(paras, 26), Is.EqualTo("一万〇五、")); // 10005
            Assert.That(ListLabel(paras, 27), Is.EqualTo("一万〇一十一、")); // 10011
            Assert.That(ListLabel(paras, 28), Is.EqualTo("一万〇一百〇一、")); // 10101
            Assert.That(ListLabel(paras, 29), Is.EqualTo("一万〇一百一十一、")); // 10111
            Assert.That(ListLabel(paras, 30), Is.EqualTo("一万一千一百一十一、")); // 11111
            Assert.That(ListLabel(paras, 31), Is.EqualTo("一万三千四百五十六、")); // 13456
            Assert.That(ListLabel(paras, 32), Is.EqualTo("二万七千八百九十、")); // 27890
            Assert.That(ListLabel(paras, 33), Is.EqualTo("三万二千七百六十七、")); // 32767
            Assert.That(ListLabel(paras, 34), Is.EqualTo("三万二千七百六十八、")); // 32768
        }

        /// <summary>
        /// Added support for <see cref="NumberStyle.SimpChinNum1"/> numbering style.
        /// </summary>
        [Test]
        public void TestChineseCountingNumbering()
        {
            Document doc = TestUtil.Open(@"Model\List\TestChineseCounting.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[0].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.SimpChinNum1));

            Assert.That(ListLabel(paras, 0), Is.EqualTo("一、")); // 1
            Assert.That(ListLabel(paras, 1), Is.EqualTo("二、")); // 2
            Assert.That(ListLabel(paras, 2), Is.EqualTo("三、")); // 3
            Assert.That(ListLabel(paras, 3), Is.EqualTo("四、")); // 4
            Assert.That(ListLabel(paras, 4), Is.EqualTo("五、")); // 5
            Assert.That(ListLabel(paras, 5), Is.EqualTo("六、")); // 6
            Assert.That(ListLabel(paras, 6), Is.EqualTo("七、")); // 7
            Assert.That(ListLabel(paras, 7), Is.EqualTo("八、")); // 8
            Assert.That(ListLabel(paras, 8), Is.EqualTo("九、")); // 9
            Assert.That(ListLabel(paras, 9), Is.EqualTo("十、")); // 10
            Assert.That(ListLabel(paras, 10), Is.EqualTo("十一、")); // 11
            Assert.That(ListLabel(paras, 11), Is.EqualTo("二十、")); // 20
            Assert.That(ListLabel(paras, 12), Is.EqualTo("二十一、")); // 21
            Assert.That(ListLabel(paras, 13), Is.EqualTo("五十、")); // 50
            Assert.That(ListLabel(paras, 14), Is.EqualTo("一○○、")); // 100
            Assert.That(ListLabel(paras, 15), Is.EqualTo("一○一、")); // 101
            Assert.That(ListLabel(paras, 16), Is.EqualTo("一○五、")); // 105
            Assert.That(ListLabel(paras, 17), Is.EqualTo("一一一、")); // 111
            Assert.That(ListLabel(paras, 18), Is.EqualTo("一二一、")); // 121
            Assert.That(ListLabel(paras, 19), Is.EqualTo("五○○、")); // 500
            Assert.That(ListLabel(paras, 20), Is.EqualTo("一○○○、")); // 1000
            Assert.That(ListLabel(paras, 21), Is.EqualTo("一○○一、")); // 1001
            Assert.That(ListLabel(paras, 22), Is.EqualTo("一○○五、")); // 1005
            Assert.That(ListLabel(paras, 23), Is.EqualTo("五○○○、")); // 5000
            Assert.That(ListLabel(paras, 24), Is.EqualTo("一○○○○、")); // 10000
            Assert.That(ListLabel(paras, 25), Is.EqualTo("一○○○一、")); // 10001
            Assert.That(ListLabel(paras, 26), Is.EqualTo("一○○○五、")); // 10005
            Assert.That(ListLabel(paras, 27), Is.EqualTo("一○○一一、")); // 10011
            Assert.That(ListLabel(paras, 28), Is.EqualTo("一○一○一、")); // 10101
            Assert.That(ListLabel(paras, 29), Is.EqualTo("一○一一一、")); // 10111
            Assert.That(ListLabel(paras, 30), Is.EqualTo("一一一一一、")); // 11111
            Assert.That(ListLabel(paras, 31), Is.EqualTo("一三四五六、")); // 13456
            Assert.That(ListLabel(paras, 32), Is.EqualTo("二七八九○、")); // 27890
            Assert.That(ListLabel(paras, 33), Is.EqualTo("三二七六七、")); // 32767
            Assert.That(ListLabel(paras, 34), Is.EqualTo("三二七六八、")); // 32768
        }

        /// <summary>
        /// Added support for <see cref="NumberStyle.SimpChinNum2"/> numbering style.
        /// WORDSNET-15957 Rendering issue of Chinese number list
        /// </summary>
        [Test]
        [TestCase(1, "壹")]
        [TestCase(2, "贰")]
        [TestCase(3, "叁")]
        [TestCase(4, "肆")]
        [TestCase(5, "伍")]
        [TestCase(6, "陆")]
        [TestCase(7, "柒")]
        [TestCase(8, "捌")]
        [TestCase(9, "玖")]
        [TestCase(10, "壹拾")]
        [TestCase(11, "壹拾壹")]
        [TestCase(34, "叁拾肆")]
        [TestCase(50, "伍拾")]
        [TestCase(100, "壹佰")]
        [TestCase(102, "壹佰零贰")]
        [TestCase(110, "壹佰壹拾")]
        [TestCase(1000, "壹仟")]
        [TestCase(2060, "贰仟零陆拾")]
        [TestCase(2093, "贰仟零玖拾叁")]
        [TestCase(3500, "叁仟伍佰")]
        [TestCase(3604, "叁仟陆佰零肆")]
        [TestCase(4987, "肆仟玖佰捌拾柒")]
        [TestCase(5000, "伍仟")]
        [TestCase(10000, "壹万")]
        [TestCase(10008, "壹万零捌")]
        [TestCase(10309, "壹万零叁佰零玖")]
        [TestCase(30000, "叁万")]
        [TestCase(45000, "肆万伍仟")]
        [TestCase(56001, "伍万陆仟零壹")]
        [TestCase(67890, "陆万柒仟捌佰玖拾")]
        [TestCase(500000, "伍拾万")]
        [TestCase(1000200, "壹佰万零贰佰")]
        [TestCase(50000003, "伍仟万零叁")]
        public void TestChineseSimplifiedNumbering(int value, string expected)
        {
            string actual = NumberConverter.NumberToLocalizedString(value, NumberStyle.SimpChinNum2, false);
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// Added support for <see cref="NumberStyle.Zodiac1"/> numbering style.
        /// </summary>
        [Test]
        public void TestZodiac1Numbering()
        {
            Document doc = TestUtil.Open(@"Model\List\TestZodiac1.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[1].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.Zodiac1));

            Assert.That(ListLabel(paras, 1), Is.EqualTo("甲")); // 1
            Assert.That(ListLabel(paras, 2), Is.EqualTo("乙")); // 2
            Assert.That(ListLabel(paras, 3), Is.EqualTo("丙")); // 3
            Assert.That(ListLabel(paras, 4), Is.EqualTo("丁")); // 4
            Assert.That(ListLabel(paras, 5), Is.EqualTo("戊")); // 5
            Assert.That(ListLabel(paras, 6), Is.EqualTo("己")); // 6
            Assert.That(ListLabel(paras, 7), Is.EqualTo("庚")); // 7
            Assert.That(ListLabel(paras, 8), Is.EqualTo("辛")); // 8
            Assert.That(ListLabel(paras, 9), Is.EqualTo("壬")); // 9
            Assert.That(ListLabel(paras, 10), Is.EqualTo("癸")); // 10
            Assert.That(ListLabel(paras, 11), Is.EqualTo("11")); // 11
            Assert.That(ListLabel(paras, 12), Is.EqualTo("12")); // 12
            Assert.That(ListLabel(paras, 100), Is.EqualTo("100")); // 100
        }

        /// <summary>
        /// Added support for <see cref="NumberStyle.Zodiac2"/> numbering style.
        /// </summary>
        [Test]
        public void TestZodiac2Numbering()
        {
            Document doc = TestUtil.Open(@"Model\List\TestZodiac2.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[1].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.Zodiac2));

            Assert.That(ListLabel(paras, 1), Is.EqualTo("子")); // 1
            Assert.That(ListLabel(paras, 2), Is.EqualTo("丑")); // 2
            Assert.That(ListLabel(paras, 3), Is.EqualTo("寅")); // 3
            Assert.That(ListLabel(paras, 4), Is.EqualTo("卯")); // 4
            Assert.That(ListLabel(paras, 5), Is.EqualTo("辰")); // 5
            Assert.That(ListLabel(paras, 6), Is.EqualTo("巳")); // 6
            Assert.That(ListLabel(paras, 7), Is.EqualTo("午")); // 7
            Assert.That(ListLabel(paras, 8), Is.EqualTo("未")); // 8
            Assert.That(ListLabel(paras, 9), Is.EqualTo("申")); // 9
            Assert.That(ListLabel(paras, 10), Is.EqualTo("酉")); // 10
            Assert.That(ListLabel(paras, 11), Is.EqualTo("戍")); // 11
            Assert.That(ListLabel(paras, 12), Is.EqualTo("亥")); // 12
            Assert.That(ListLabel(paras, 13), Is.EqualTo("13")); // 13
            Assert.That(ListLabel(paras, 14), Is.EqualTo("14")); // 14
            Assert.That(ListLabel(paras, 100), Is.EqualTo("100")); // 100
        }

        /// <summary>
        /// Added support for <see cref="NumberStyle.Zodiac3"/> numbering style.
        /// </summary>
        [Test]
        public void TestZodiac3Numbering()
        {
            Document doc = TestUtil.Open(@"Model\List\TestZodiac3.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[1].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.Zodiac3));

            Assert.That(ListLabel(paras, 1), Is.EqualTo("甲子")); // 1
            Assert.That(ListLabel(paras, 2), Is.EqualTo("乙丑")); // 2
            Assert.That(ListLabel(paras, 3), Is.EqualTo("丙寅")); // 3
            Assert.That(ListLabel(paras, 4), Is.EqualTo("丁卯")); // 4
            Assert.That(ListLabel(paras, 5), Is.EqualTo("戊辰")); // 5
            Assert.That(ListLabel(paras, 6), Is.EqualTo("己巳")); // 6
            Assert.That(ListLabel(paras, 7), Is.EqualTo("庚午")); // 7
            Assert.That(ListLabel(paras, 8), Is.EqualTo("辛未")); // 8
            Assert.That(ListLabel(paras, 9), Is.EqualTo("壬申")); // 9
            Assert.That(ListLabel(paras, 10), Is.EqualTo("癸酉")); // 10
            Assert.That(ListLabel(paras, 11), Is.EqualTo("甲戍")); // 11
            Assert.That(ListLabel(paras, 12), Is.EqualTo("乙亥")); // 12
            Assert.That(ListLabel(paras, 13), Is.EqualTo("丙子")); // 13
            Assert.That(ListLabel(paras, 14), Is.EqualTo("丁丑")); // 14
            Assert.That(ListLabel(paras, 15), Is.EqualTo("戊寅")); // 15
            Assert.That(ListLabel(paras, 16), Is.EqualTo("己卯")); // 16
            Assert.That(ListLabel(paras, 17), Is.EqualTo("庚辰")); // 17
            Assert.That(ListLabel(paras, 20), Is.EqualTo("癸未")); // 20
            Assert.That(ListLabel(paras, 24), Is.EqualTo("丁亥")); // 24
            Assert.That(ListLabel(paras, 25), Is.EqualTo("戊子")); // 25
            Assert.That(ListLabel(paras, 30), Is.EqualTo("癸巳")); // 30
            Assert.That(ListLabel(paras, 31), Is.EqualTo("甲午")); // 31
            Assert.That(ListLabel(paras, 36), Is.EqualTo("己亥")); // 36
            Assert.That(ListLabel(paras, 37), Is.EqualTo("庚子")); // 37
            Assert.That(ListLabel(paras, 40), Is.EqualTo("癸卯")); // 40
            Assert.That(ListLabel(paras, 50), Is.EqualTo("癸丑")); // 50
            Assert.That(ListLabel(paras, 63), Is.EqualTo("丙寅")); // 63
            Assert.That(ListLabel(paras, 64), Is.EqualTo("丁卯")); // 64
            Assert.That(ListLabel(paras, 65), Is.EqualTo("戊辰")); // 65
            Assert.That(ListLabel(paras, 77), Is.EqualTo("庚辰")); // 77
            Assert.That(ListLabel(paras, 88), Is.EqualTo("辛卯")); // 88
            Assert.That(ListLabel(paras, 99), Is.EqualTo("壬寅")); // 99
            Assert.That(ListLabel(paras, 100), Is.EqualTo("癸卯")); // 100
        }

        /// <summary>
        /// Added support for <see cref="NumberStyle.GB2"/> numbering style.
        /// </summary>
        [Test]
        public void TestGb2Numbering()
        {
            Document doc = TestUtil.Open(@"Model\List\TestGb2.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[1].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.GB2));

            Assert.That(ListLabel(paras, 1), Is.EqualTo("⑴")); // 1
            Assert.That(ListLabel(paras, 2), Is.EqualTo("⑵")); // 2
            Assert.That(ListLabel(paras, 3), Is.EqualTo("⑶")); // 3
            Assert.That(ListLabel(paras, 4), Is.EqualTo("⑷")); // 4
            Assert.That(ListLabel(paras, 5), Is.EqualTo("⑸")); // 5
            Assert.That(ListLabel(paras, 6), Is.EqualTo("⑹")); // 6
            Assert.That(ListLabel(paras, 7), Is.EqualTo("⑺")); // 7
            Assert.That(ListLabel(paras, 8), Is.EqualTo("⑻")); // 8
            Assert.That(ListLabel(paras, 9), Is.EqualTo("⑼")); // 9
            Assert.That(ListLabel(paras, 10), Is.EqualTo("⑽")); // 10
            Assert.That(ListLabel(paras, 11), Is.EqualTo("⑾")); // 11
            Assert.That(ListLabel(paras, 12), Is.EqualTo("⑿")); // 12
            Assert.That(ListLabel(paras, 13), Is.EqualTo("⒀")); // 13
            Assert.That(ListLabel(paras, 14), Is.EqualTo("⒁")); // 14
            Assert.That(ListLabel(paras, 15), Is.EqualTo("⒂")); // 15
            Assert.That(ListLabel(paras, 16), Is.EqualTo("⒃")); // 16
            Assert.That(ListLabel(paras, 17), Is.EqualTo("⒄")); // 17
            Assert.That(ListLabel(paras, 18), Is.EqualTo("⒅")); // 18
            Assert.That(ListLabel(paras, 19), Is.EqualTo("⒆")); // 19
            Assert.That(ListLabel(paras, 20), Is.EqualTo("⒇")); // 20
            Assert.That(ListLabel(paras, 21), Is.EqualTo("21")); // 21
        }

        /// <summary>
        /// Added support for <see cref="NumberStyle.Custom"/> numbering style.
        /// Tests "001, 002, 003, ..." custom style.
        /// </summary>
        [Test]
        public void TestCustomLeadingZero2()
        {
            Document doc = TestUtil.Open(@"Model\List\TestCustomLeadingZero2.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[1].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.Custom));
            Assert.That(paras[1].ListFormat.ListLevel.CustomNumberStyle, Is.EqualTo("001, 002, 003, ..."));

            Assert.That(ListLabel(paras, 1), Is.EqualTo("001."));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("002."));
            Assert.That(ListLabel(paras, 3), Is.EqualTo("009."));
            Assert.That(ListLabel(paras, 4), Is.EqualTo("010."));
            Assert.That(ListLabel(paras, 5), Is.EqualTo("011."));
            Assert.That(ListLabel(paras, 6), Is.EqualTo("099."));
            Assert.That(ListLabel(paras, 7), Is.EqualTo("100."));
            Assert.That(ListLabel(paras, 8), Is.EqualTo("101."));
        }

        /// <summary>
        /// Added support for <see cref="NumberStyle.Custom"/> numbering style.
        /// Tests "0001, 0002, 0003, ..." custom style.
        /// </summary>
        [Test]
        public void TestCustomLeadingZero3()
        {
            Document doc = TestUtil.Open(@"Model\List\TestCustomLeadingZero3.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[1].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.Custom));
            Assert.That(paras[1].ListFormat.ListLevel.CustomNumberStyle, Is.EqualTo("0001, 0002, 0003, ..."));

            Assert.That(ListLabel(paras, 1), Is.EqualTo("0001."));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("0002."));
            Assert.That(ListLabel(paras, 3), Is.EqualTo("0009."));
            Assert.That(ListLabel(paras, 4), Is.EqualTo("0010."));
            Assert.That(ListLabel(paras, 5), Is.EqualTo("0011."));
            Assert.That(ListLabel(paras, 6), Is.EqualTo("0099."));
            Assert.That(ListLabel(paras, 7), Is.EqualTo("0100."));
            Assert.That(ListLabel(paras, 8), Is.EqualTo("0101."));
            Assert.That(ListLabel(paras, 9), Is.EqualTo("0999."));
            Assert.That(ListLabel(paras, 10), Is.EqualTo("1000."));
            Assert.That(ListLabel(paras, 11), Is.EqualTo("1001."));
        }

        /// <summary>
        /// Added support for <see cref="NumberStyle.Custom"/> numbering style.
        /// Tests "00001, 00002, 00003, ..." custom style.
        /// </summary>
        [Test]
        public void TestCustomLeadingZero4()
        {
            Document doc = TestUtil.Open(@"Model\List\TestCustomLeadingZero4.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[1].ListFormat.ListLevel.NumberStyle, Is.EqualTo(NumberStyle.Custom));
            Assert.That(paras[1].ListFormat.ListLevel.CustomNumberStyle, Is.EqualTo("00001, 00002, 00003, ..."));

            Assert.That(ListLabel(paras, 1), Is.EqualTo("00001."));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("00002."));
            Assert.That(ListLabel(paras, 3), Is.EqualTo("00009."));
            Assert.That(ListLabel(paras, 4), Is.EqualTo("00010."));
            Assert.That(ListLabel(paras, 5), Is.EqualTo("00011."));
            Assert.That(ListLabel(paras, 6), Is.EqualTo("00099."));
            Assert.That(ListLabel(paras, 7), Is.EqualTo("00100."));
            Assert.That(ListLabel(paras, 8), Is.EqualTo("00101."));
            Assert.That(ListLabel(paras, 9), Is.EqualTo("00999."));
            Assert.That(ListLabel(paras, 10), Is.EqualTo("01000."));
            Assert.That(ListLabel(paras, 11), Is.EqualTo("01001."));
            Assert.That(ListLabel(paras, 12), Is.EqualTo("09999."));
            Assert.That(ListLabel(paras, 13), Is.EqualTo("10000."));
            Assert.That(ListLabel(paras, 14), Is.EqualTo("10001."));
        }

        /// <summary>
        /// WORDSNET-5914 IndexOutOfRangeException was thrown on saving Rtf or call of doc.UpdateListLabels.
        /// </summary>
        [Test]
        public void TestJira5914()
        {
            Document doc = TestUtil.Open("Model/List/TestJira5914.docx");

            // FOSS: Rtf writer removed; UpdateListLabels alone reproduces the WORDSNET-5914 fix (no crash).
            doc.UpdateListLabels();
        }

        [Test]
        public void TestHexNumbering()
        {
            Document doc = TestUtil.Open("Model/List/TestHexNumbering.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            int index = 0;

            Assert.That(ListLabel(paras, index++), Is.EqualTo("1."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("2."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("3."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("4."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("5."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("6."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("7."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("8."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("9."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("A."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("B."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("C."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("D."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("E."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("F."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("10."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("11."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("12."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("13."));
            Assert.That(ListLabel(paras, index++), Is.EqualTo("14."));

            Assert.That(ListLabel(paras, 20), Is.EqualTo("7FFD."));
            Assert.That(ListLabel(paras, 21), Is.EqualTo("7FFE."));
            Assert.That(ListLabel(paras, 22), Is.EqualTo("7FFF."));
        }

        /// <summary>
        /// WORDSNET-7760 Support revisions in list numbers.
        /// andrnosk: If LayoutOptionsCore.RevisionOptions.IsShowOriginalRevision is not set then ParaPr.FormatRevision.RevPr
        /// is used instead of ParaPr upon list labels generating.
        /// </summary>
        [Test]
        public void TestJira7760()
        {
            Document doc = TestUtil.Open("Model/List/TestJira7760.docx");

            doc.UpdateListLabels();

            ParagraphCollection pc = doc.FirstSection.Body.Paragraphs;

            Assert.That(pc[0].ParaPr.HasFormatRevision, Is.False);
            Assert.That(pc[1].ParaPr.HasFormatRevision, Is.True);

            // Start number was changed.
            CheckListRevision(pc[1], "2.", "4.");
            CheckListRevision(pc[2], "3.", "5.");
            CheckListRevision(pc[3], "4.", "6.");

            // List was removed.
            CheckListRevision(pc[5], "1.0", "");
            CheckListRevision(pc[6], "2.0", "");

            CheckListRevision(pc[13], "\xf0b7", "3.");

            CheckListRevision(pc[16], "II.", "2.");
            CheckListRevision(pc[17], "III.", "3.");

            CheckListRevision(pc[30], "1.0", "1.0");
            CheckListRevision(pc[31], "2.0", "a.");
            CheckListRevision(pc[32], "3.0", "2.0");
            CheckListRevision(pc[33], "4.0", "3.0");

            CheckListRevision(pc[35], "1.", "1.");
            CheckListRevision(pc[36], "2.", "1.1.");
            CheckListRevision(pc[37], "3.", "1.1.1.");
            CheckListRevision(pc[38], "4.", "2.");

            CheckListRevision(pc[40], "1.", "I.");
            CheckListRevision(pc[41], "1.1.", "a.");
            CheckListRevision(pc[42], "1.1.1.", "i.");
            CheckListRevision(pc[43], "1.1.1.1.", "1.");
        }

        /// <summary>
        /// WORDSNET-7972 Model incorrectly computes Istd of the final revision in lists.
        /// </summary>
        [Test]
        public void TestJira7760Istd()
        {
            Document doc = TestUtil.Open("Model/List/TestJira7760.docx");

            Paragraph p0 = (Paragraph)doc.GetChild(NodeType.Paragraph, 0, true);
            Paragraph p1 = (Paragraph)doc.GetChild(NodeType.Paragraph, 1, true);

            Assert.That(p0.ParaPr.FormatRevision, Is.Null);
            Assert.That(p1.ParaPr.FormatRevision, IsNot.Null());

            // Calculate AfterChanges.
            ParaPr afterChanges = p1.ParaPr.Clone();
            afterChanges.AcceptFormatRevision();

            Assert.That(afterChanges.Istd, Is.EqualTo(p1.ParaPr.Istd));
        }



        /// <summary>
        /// WORDSNET-6440 List numbering of Bullets is incorrect when converting to fixed page formats
        /// Implemented converting to "Ideograph Legal Traditional" and "Taiwanese Counting Thousand" numbers formats
        /// </summary>
        [Test]
        public void TestJira6440()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira6440.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paras, 0), Is.EqualTo("壹、"));
            Assert.That(ListLabel(paras, 1), Is.EqualTo("一、"));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("二、"));
            Assert.That(ListLabel(paras, 3), Is.EqualTo("三、"));
            Assert.That(ListLabel(paras, 4), Is.EqualTo("貳、"));
            Assert.That(ListLabel(paras, 6), Is.EqualTo("參、"));
        }

        /// <summary>
        /// WORDSNET-9150 Doc to Pdf conversion issue with Chinese bullet numbers
        /// Implemented converting to "Chinese Counting Thousand" numbers format
        /// </summary>
        [Test]
        public void TestJira9150()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira9150.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paras, 0), Is.EqualTo("(一)"));
            Assert.That(ListLabel(paras, 1), Is.EqualTo("(二)"));
        }


        /// <summary>
        /// WORDSNET-11880 Added support for the Arabic Alpha and the Arabic Abjad numbering styles.
        /// </summary>
        [Test]
        public void TestJira11880_ArabicAlphaAndAbjadNumbering()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira11880_ArabicAlphaAndAbjadNumbering.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            // Test Arabic Alpha numbering
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < 1 + 28; i++)
            {
                string label = ListLabel(paras, i);
                int len = label.Length - 1;

                sb.Append(label.Substring(0, len));
            }

            Assert.That("أبتثجحخدذرزسشصضطظعغفقكلمنهوي", Is.EqualTo(sb.ToString()));
            Assert.That(ListLabel(paras, 29), Is.EqualTo("أأ."));
            Assert.That(ListLabel(paras, 57), Is.EqualTo("أأأ."));
            Assert.That(ListLabel(paras, 85), Is.EqualTo("أأأأ."));
            Assert.That(ListLabel(paras, 141), Is.EqualTo("أأأأأأ."));
            Assert.That(ListLabel(paras, 169), Is.EqualTo("أأأأأأأ."));
            Assert.That(ListLabel(paras, 392), Is.EqualTo("يييييييييييييي."));
            Assert.That(ListLabel(paras, 393), Is.EqualTo("أ."));

            // Test Arabic Abjad numbering
            sb = new StringBuilder();
            for (int i = 402; i < 402 + 28; i++)
            {
                string label = ListLabel(paras, i);
                int len = label.Length - 1;

                sb.Append(label.Substring(0, len));
            }

            Assert.That("أبجدهوزحطيكلمنسعفصقرشتثخذضغظ", Is.EqualTo(sb.ToString()));
        }



        /// <summary>
        /// WORDSNET-12648 Hebrew list labels are not rendered correctly in output PDF.
        /// Numbers 15 and 16 have special representation in Hebrew1.
        /// </summary>
        [Test]
        public void TestJira12648()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira12648.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paras, 14), Is.EqualTo("טו."));
            Assert.That(ListLabel(paras, 15), Is.EqualTo("טז."));
        }



        /// <summary>
        /// Relates to WORDSNET-10153 Tests that label is changed even if paragraph has no revisions.
        /// </summary>
        [Test]
        public void TestJira10153B()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestJira10153B.docx");
            doc.UpdateListLabels();

            ParagraphCollection pc = doc.FirstSection.Body.Paragraphs;
            Assert.That(pc[0].IsDeleteRevision, Is.True);
            CheckListRevision(pc[0], 1, 1, "1.", "");

            // Second paragraph has no revisions at all but label is changed since above paragraph is deleted.
            Assert.That(pc[1].HasRevisions, Is.False);
            CheckListRevision(pc[1], 1, 1, "2.", "1.");
        }

        /// <summary>
        /// Relates to WORDSNET-10153 Tests that ListId is fetched properly.
        /// </summary>
        [Test]
        public void TestJira10153C()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestJira10153C.docx");

            doc.UpdateListLabels();

            ParagraphCollection pc = doc.FirstSection.Body.Paragraphs;
            CheckListRevision(pc[0], 0, 2, "", "I.");
            CheckListRevision(pc[1], 0, 1, "", "1.");
            CheckListRevision(pc[2], 0, 2, "", "II.");
        }

        /// <summary>
        /// Relates to WORDSNET-10153 Tests various list revision cases.
        /// </summary>
        [Test]
        public void TestJira10153D()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestJira10153D.xml");

            doc.UpdateListLabels();

            ParagraphCollection pc = doc.FirstSection.Body.Paragraphs;
            CheckListRevision(pc[1], "1.", "1.");
            CheckListRevision(pc[2], "", "2.");
            CheckListRevision(pc[3], "2.", "");
            CheckListRevision(pc[4], "", "a)");
            CheckListRevision(pc[5], "a)", "b)");
            CheckListRevision(pc[6], "b)", "");
        }

        /// <summary>
        /// Relates to WORDSNET-10153 Tests that left indent is fetched properly.
        /// </summary>
        [Test]
        public void TestJira10153E()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestJira10153E.docx");

            Paragraph para = doc.FirstSection.Body.Paragraphs[0];

            Assert.That(para.FetchParaAttr(ParaAttr.LeftIndent, RevisionsView.Original), Is.EqualTo(0));
            Assert.That(para.FetchParaAttr(ParaAttr.LeftIndent, RevisionsView.Final), Is.EqualTo(720));
        }


        /// <summary>
        /// Relates to WORDSNET-10153 Tests bulleted list revision.
        /// </summary>
        [Test]
        public void TestJira9354()
        {
            Document doc = TestUtil.Open(@"Model\Revision\TestJira9354.docx");
            doc.UpdateListLabels();

            Paragraph p = doc.FirstSection.Body.Paragraphs[5];
            CheckListRevision(p, "", "\xf0b7");
        }


        /// <summary>
        /// WORDSNET-13685 Docx to Pdf conversion issue with ListLabel.
        /// Should increment skipped levels when not yet processed any level of list
        /// instead of it is level '0'.
        /// </summary>
        [Test]
        public void TestJira13685()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira13685.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            for (int i = 2; i < 8; i++)
                Assert.That(paras[i].ListLabel.LabelString.StartsWith("2.1"), Is.True);
        }


        /// <summary>
        /// WORDSNET-14331 ListLabel.LabelString does not return correct value for number style GB2.
        /// Implemented GB2 numbering.
        /// </summary>
        [Test]
        public void TestJira14331()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira14331.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paras, 1), Is.EqualTo("(1)"));
            Assert.That(ListLabel(paras, 5), Is.EqualTo("⑴"));
        }

        /// <summary>
        /// WORDSNET-14571 Unable to save a Word document as a PDF - document has special number formatting.
        /// Implemented custom numbering style.
        /// </summary>
        [Test]
        public void TestJira14571()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira14571.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            for (int i = 0; i < 8; i++)
            {
                string expectedListLabel = string.Format("[000{0}]", i + 1);
                Assert.That(ListLabel(paras, i), Is.EqualTo(expectedListLabel));
            }
        }

        /// <summary>
        /// WORDSNET-14640 System.ArgumentOutOfRangeException is thrown while saving document to Png.
        /// This issue duplicates <see cref="TestJira14571"/>.
        /// </summary>
        [Test]
        public void TestJira14640()
        {
            Document doc = TestUtil.Open(@"Model\List\TestJira14640.docx");
            doc.UpdateListLabels();

            Paragraph para = (Paragraph)doc.FirstSection.Body.GetChild(NodeType.Paragraph, 3, false);
            Assert.That(para.ListLabel.LabelString, Is.EqualTo("[0001]"));
        }



        [Test]
        public void TestListFormatFinalRevision()
        {
            Document document = TestUtil.Open(@"Model\List\TestListFormatFinalRevision.docx");

            ListFormat listFormat = document.FirstSection.Body.FirstParagraph.ListFormat;

            Assert.That(listFormat.ListId, Is.EqualTo(0));
            Assert.That(listFormat.ListIdFinal, Is.EqualTo(1));
            Assert.That(listFormat.ListLevelNumber, Is.EqualTo(0));
            Assert.That(listFormat.ListLevelNumberFinal, Is.EqualTo(0));
            Assert.That(listFormat.List, Is.Null);
            Assert.That(listFormat.ListFinal, IsNot.Null());
            Assert.That(listFormat.ListLevel, Is.Null);
            Assert.That(listFormat.ListLevelFinal, IsNot.Null());
        }



        /// <summary>
        /// WORDSNET-18309 Incorrect list numbers in output PDF.
        /// Document contains several nested fields. We must update paragraph list labels only for
        /// paragraphs at the topmost field result.
        /// </summary>
        [Test]
        public void TestJira18309()
        {
            // Document contains "IF" field, which contains "QUOTE" field with list paragraphs.
            // Paragraphs from "QUOTE" fieldcode and fieldresult should not be considered during list labels update.
            Document doc = TestUtil.Open(@"Model\List\TestJira18309.docx");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            // Verify the root issue.
            doc.UpdateListLabels();

            Assert.That(ListLabel(paras, 8), Is.EqualTo("1."));
            Assert.That(paras[8].GetText(), Is.EqualTo("paragraph 5\r"));

            Assert.That(ListLabel(paras, 9), Is.EqualTo("2."));
            Assert.That(paras[9].GetText(), Is.EqualTo("paragraph 6\r"));

            Assert.That(ListLabel(paras, 15), Is.EqualTo(""));
            Assert.That(ListLabel(paras, 16), Is.EqualTo(""));

            // Verify customer's use case.
            doc.UpdateFields();
            doc.UpdateListLabels();

            Assert.That(ListLabel(paras, 8), Is.EqualTo(""));
            Assert.That(ListLabel(paras, 9), Is.EqualTo(""));

            Assert.That(ListLabel(paras, 15), Is.EqualTo("1."));
            Assert.That(paras[15].GetText(), Is.EqualTo("paragraph 7\r"));

            Assert.That(ListLabel(paras, 16), Is.EqualTo("2."));
            Assert.That(paras[16].GetText(), Is.EqualTo("paragraph 8\r"));
        }

        /// <summary>
        /// WORDSNET-18365 Vietnamese text becomes number in PDF - PAGE field.
        /// Added support for Vietnamese numbering.
        /// </summary>
        [Test]
        #region Cases
        [TestCase(0, "không")]
        [TestCase(1, "một")]
        [TestCase(2, "hai")]
        [TestCase(3, "ba")]
        [TestCase(4, "bốn")]
        [TestCase(5, "năm")]
        [TestCase(6, "sáu")]
        [TestCase(7, "bảy")]
        [TestCase(8, "tám")]
        [TestCase(9, "chín")]
        [TestCase(10, "mười")]
        [TestCase(11, "mười một")]
        [TestCase(12, "mười hai")]
        [TestCase(13, "mười ba")]
        [TestCase(14, "mười bốn")]
        [TestCase(15, "mười lăm")]
        [TestCase(16, "mười sáu")]
        [TestCase(17, "mười bảy")]
        [TestCase(18, "mười tám")]
        [TestCase(19, "mười chín")]
        [TestCase(20, "hai mươi")]
        [TestCase(21, "hai mươi mốt")]
        [TestCase(22, "hai mươi hai")]
        [TestCase(23, "hai mươi ba")]
        [TestCase(24, "hai mươi bốn")]
        [TestCase(25, "hai mươi lăm")]
        [TestCase(26, "hai mươi sáu")]
        [TestCase(27, "hai mươi bảy")]
        [TestCase(28, "hai mươi tám")]
        [TestCase(29, "hai mươi chín")]
        [TestCase(30, "ba mươi")]
        [TestCase(31, "ba mươi mốt")]
        [TestCase(32, "ba mươi hai")]
        [TestCase(33, "ba mươi ba")]
        [TestCase(34, "ba mươi bốn")]
        [TestCase(35, "ba mươi lăm")]
        [TestCase(36, "ba mươi sáu")]
        [TestCase(37, "ba mươi bảy")]
        [TestCase(38, "ba mươi tám")]
        [TestCase(39, "ba mươi chín")]
        [TestCase(40, "bốn mươi")]
        [TestCase(41, "bốn mươi mốt")]
        [TestCase(42, "bốn mươi hai")]
        [TestCase(43, "bốn mươi ba")]
        [TestCase(44, "bốn mươi bốn")]
        [TestCase(45, "bốn mươi lăm")]
        [TestCase(46, "bốn mươi sáu")]
        [TestCase(47, "bốn mươi bảy")]
        [TestCase(48, "bốn mươi tám")]
        [TestCase(49, "bốn mươi chín")]
        [TestCase(50, "năm mươi")]
        [TestCase(51, "năm mươi mốt")]
        [TestCase(52, "năm mươi hai")]
        [TestCase(53, "năm mươi ba")]
        [TestCase(54, "năm mươi bốn")]
        [TestCase(55, "năm mươi lăm")]
        [TestCase(56, "năm mươi sáu")]
        [TestCase(57, "năm mươi bảy")]
        [TestCase(58, "năm mươi tám")]
        [TestCase(59, "năm mươi chín")]
        [TestCase(60, "sáu mươi")]
        [TestCase(61, "sáu mươi mốt")]
        [TestCase(62, "sáu mươi hai")]
        [TestCase(63, "sáu mươi ba")]
        [TestCase(64, "sáu mươi bốn")]
        [TestCase(65, "sáu mươi lăm")]
        [TestCase(66, "sáu mươi sáu")]
        [TestCase(67, "sáu mươi bảy")]
        [TestCase(68, "sáu mươi tám")]
        [TestCase(69, "sáu mươi chín")]
        [TestCase(70, "bảy mươi")]
        [TestCase(71, "bảy mươi mốt")]
        [TestCase(72, "bảy mươi hai")]
        [TestCase(73, "bảy mươi ba")]
        [TestCase(74, "bảy mươi bốn")]
        [TestCase(75, "bảy mươi lăm")]
        [TestCase(76, "bảy mươi sáu")]
        [TestCase(77, "bảy mươi bảy")]
        [TestCase(78, "bảy mươi tám")]
        [TestCase(79, "bảy mươi chín")]
        [TestCase(80, "tám mươi")]
        [TestCase(81, "tám mươi mốt")]
        [TestCase(82, "tám mươi hai")]
        [TestCase(83, "tám mươi ba")]
        [TestCase(84, "tám mươi bốn")]
        [TestCase(85, "tám mươi lăm")]
        [TestCase(86, "tám mươi sáu")]
        [TestCase(87, "tám mươi bảy")]
        [TestCase(88, "tám mươi tám")]
        [TestCase(89, "tám mươi chín")]
        [TestCase(90, "chín mươi")]
        [TestCase(91, "chín mươi mốt")]
        [TestCase(92, "chín mươi hai")]
        [TestCase(93, "chín mươi ba")]
        [TestCase(94, "chín mươi bốn")]
        [TestCase(95, "chín mươi lăm")]
        [TestCase(96, "chín mươi sáu")]
        [TestCase(97, "chín mươi bảy")]
        [TestCase(98, "chín mươi tám")]
        [TestCase(99, "chín mươi chín")]
        [TestCase(100, "một trăm")]
        [TestCase(101, "một trăm lẻ một")]
        [TestCase(102, "một trăm lẻ hai")]
        [TestCase(103, "một trăm lẻ ba")]
        [TestCase(104, "một trăm lẻ bốn")]
        [TestCase(105, "một trăm lẻ năm")]
        [TestCase(106, "một trăm lẻ sáu")]
        [TestCase(107, "một trăm lẻ bảy")]
        [TestCase(108, "một trăm lẻ tám")]
        [TestCase(109, "một trăm lẻ chín")]
        [TestCase(110, "một trăm mười")]
        [TestCase(111, "một trăm mười một")]
        [TestCase(112, "một trăm mười hai")]
        [TestCase(113, "một trăm mười ba")]
        [TestCase(114, "một trăm mười bốn")]
        [TestCase(115, "một trăm mười lăm")]
        [TestCase(116, "một trăm mười sáu")]
        [TestCase(117, "một trăm mười bảy")]
        [TestCase(118, "một trăm mười tám")]
        [TestCase(119, "một trăm mười chín")]
        [TestCase(170, "một trăm bảy mươi")]
        [TestCase(180, "một trăm tám mươi")]
        [TestCase(185, "một trăm tám mươi lăm")]
        [TestCase(193, "một trăm chín mươi ba")]
        [TestCase(200, "hai trăm")]
        [TestCase(222, "hai trăm hai mươi hai")]
        [TestCase(300, "ba trăm")]
        [TestCase(400, "bốn trăm")]
        [TestCase(500, "năm trăm")]
        [TestCase(555, "năm trăm năm mươi lăm")]
        [TestCase(600, "sáu trăm")]
        [TestCase(700, "bảy trăm")]
        [TestCase(768, "bảy trăm sáu mươi tám")]
        [TestCase(800, "tám trăm")]
        [TestCase(900, "chín trăm")]
        [TestCase(999, "chín trăm chín mươi chín")]
        [TestCase(1000, "một ngàn")]
        [TestCase(1001, "không")]
        [TestCase(1002, "không")]
        [TestCase(1003, "không")]
        #endregion
        public void TestVietnameseNumbering(int value, string expected)
        {
            string actual = NumberConverter.NumberToLocalizedString(value, NumberStyle.VietCardinalText, false);
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// WORDSNET-19971 Incorrect list numbers with custom numbering format and greek numerals.
        /// Added support for Greek numbering.
        /// </summary>
        [Test]
        #region Cases
        [TestCase(0, "")]
        [TestCase(10000, "α")]
        [TestCase(10006, "ζ")]
        [TestCase(-1, "")]
        [TestCase(-100, "")]
        [TestCase(25, "κε")]
        [TestCase(27, "κζ")]
        [TestCase(56, "νστ")]
        [TestCase(132, "ρλβ")]
        [TestCase(1027, ",ακζ")]
        [TestCase(1, "α")]
        [TestCase(2, "β")]
        [TestCase(3, "γ")]
        [TestCase(4, "δ")]
        [TestCase(5, "ε")]
        [TestCase(6, "στ")]
        [TestCase(7, "ζ")]
        [TestCase(8, "η")]
        [TestCase(9, "θ")]
        [TestCase(10, "ι")]
        [TestCase(20, "κ")]
        [TestCase(30, "λ")]
        [TestCase(40, "μ")]
        [TestCase(50, "ν")]
        [TestCase(60, "ξ")]
        [TestCase(70, "ο")]
        [TestCase(80, "π")]
        [TestCase(90, "ϟ")]
        [TestCase(100, "ρ")]
        [TestCase(200, "σ")]
        [TestCase(300, "τ")]
        [TestCase(400, "υ")]
        [TestCase(500, "φ")]
        [TestCase(600, "χ")]
        [TestCase(700, "ψ")]
        [TestCase(800, "ω")]
        [TestCase(900, "ϡ")]
        [TestCase(1000, ",α")]
        [TestCase(2000, ",β")]
        [TestCase(3000, ",γ")]
        [TestCase(4000, ",δ")]
        [TestCase(5000, ",ε")]
        [TestCase(6000, ",στ")]
        [TestCase(7000, ",ζ")]
        [TestCase(8000, ",η")]
        [TestCase(9000, ",θ")]
        #endregion
        public void TestGreekNumbering(int value, string expected)
        {
            string actual = NumberConverter.NumberToLocalizedString(value, NumberStyle.LowercaseLetter, "α, β, γ, ...", false, 0);
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// WORDSNET-19971 Incorrect list numbers in output PDF.
        /// Document contains custom numbering format with greek numerals.
        /// Support for greek numerals has been added.
        /// </summary>
        [Test]
        public void Test19971()
        {
            Document doc = TestUtil.Open(@"Model\List\Test19971.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paras, 0), Is.EqualTo("Α."));
            Assert.That(ListLabel(paras, 1), Is.EqualTo("Β."));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("Γ."));
            Assert.That(ListLabel(paras, 3), Is.EqualTo("Δ."));
            Assert.That(ListLabel(paras, 4), Is.EqualTo("Ε."));
            Assert.That(ListLabel(paras, 5), Is.EqualTo("ΣΤ."));
            Assert.That(ListLabel(paras, 6), Is.EqualTo("Ζ."));
            Assert.That(ListLabel(paras, 7), Is.EqualTo("Η."));

        }


        /// <summary>
        /// Related with WORDSNET-22207
        /// Checks how starting number changes when next level of text line is less than previous.
        /// </summary>
        [Test]
        public void Test22207A()
        {
            Document doc = TestUtil.Open(@"Model\List\Test22207A.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paras, 0), Is.EqualTo("32.64.128.256"));
            Assert.That(ListLabel(paras, 1), Is.EqualTo("32.64.21"));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("32.11"));
            Assert.That(ListLabel(paras, 3), Is.EqualTo("1."));
        }

        /// <summary>
        /// Related with WORDSNET-22207
        /// Checks how starting number changes when next level of text line is more than previous for 2 units.
        /// </summary>
        [Test]
        public void Test22207B()
        {
            Document doc = TestUtil.Open(@"Model\List\Test22207B.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paras, 0), Is.EqualTo("32."));
            Assert.That(ListLabel(paras, 1), Is.EqualTo("32.10.20"));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("32.11"));
        }

        /// <summary>
        /// Related with WORDSNET-22207
        /// Checks how starting number changes when next level of text line is less than previous and
        /// then more than previous for 2 units.
        /// </summary>
        [Test]
        public void Test22207C()
        {
            Document doc = TestUtil.Open(@"Model\List\Test22207C.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paras, 0), Is.EqualTo("32.64"));
            Assert.That(ListLabel(paras, 1), Is.EqualTo("6."));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("6.10.20"));
        }

        /// <summary>
        /// Related with WORDSNET-22207
        /// Checks how starting number changes when next level of text line is equal to previous.
        /// </summary>
        [Test]
        public void Test22207D()
        {
            Document doc = TestUtil.Open(@"Model\List\Test22207D.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paras, 0), Is.EqualTo("32.64"));
            Assert.That(ListLabel(paras, 1), Is.EqualTo("32.65"));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("32.65.20"));
        }

        /// <summary>
        /// Related with WORDSNET-22207
        /// Checks that starting number for initial list level is changed when next line has less level than initial
        /// for this list.
        /// </summary>
        [Test]
        public void Test22207E()
        {
            Document doc = TestUtil.Open(@"Model\List\Test22207E.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paras, 2), Is.EqualTo("32.65.20"));
            Assert.That(ListLabel(paras, 3), Is.EqualTo("32.66"));
            Assert.That(ListLabel(paras, 4), Is.EqualTo("6."));
            Assert.That(ListLabel(paras, 5), Is.EqualTo("6.10.20.30"));
        }

        /// <summary>
        /// WORDSNET-22478 Thai characters are converted to numbers in PDF.
        /// "ThaiArabic" number style is not taken in attention while updating list labels and page numbers.
        /// </summary>
        [Test]
        public void Test22478()
        {
            Document doc = TestUtil.Open(@"Model\List\Test22478.docx");
            doc.UpdateListLabels();
            doc.UpdateFields();

            // FOSS: the header page number comes from a PAGE field, whose value is layout-dependent and
            // cannot be computed with the rendering/layout engine removed; the list-label checks below
            // (the WORDSNET-22478 subject) are model-level and remain.
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras[0].ListLabel.LabelString, Is.EqualTo("๙๙๙๘."));
            Assert.That(paras[1].ListLabel.LabelString, Is.EqualTo("๙๙๙๙."));
            Assert.That(paras[2].ListLabel.LabelString, Is.EqualTo("๑๐๐๐๐."));
            Assert.That(paras[3].ListLabel.LabelString, Is.EqualTo("๑๐๐๐๑."));
            Assert.That(paras[4].ListLabel.LabelString, Is.EqualTo("๑."));
            Assert.That(paras[5].ListLabel.LabelString, Is.EqualTo("๒."));
            Assert.That(paras[6].ListLabel.LabelString, Is.EqualTo("๓."));
            Assert.That(paras[7].ListLabel.LabelString, Is.EqualTo("๔."));
        }

        /// <summary>
        /// WORDSNET-23547 New OpenXML File Format attribute for bulleted and numbered lists
        /// </summary>
        [Test]
        public void Test23547()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\List\Test23547", UnifiedScenario.Docx2DocxNoGold);
            Assert.That(doc.Lists[0].DurableId, Is.EqualTo(1314290636));
        }

        /// <summary>
        /// WORDSNET-23735 Wrong list numbering due to loss and non-use of DurableId attribute values
        /// The test checks that when a list containing <see cref="Lists.List.DurableId"/> property value is imported to
        /// a document, required compliance is set to the document, and the DurableId value is preserved when resaving.
        /// </summary>
        [Test]
        public void Test23735Saving()
        {
            const string fileName = @"Model\List\Test23735.docx";
            Document sourceDoc = TestUtil.Open(fileName);
            StructuredDocumentTag sdt = (StructuredDocumentTag)sourceDoc.GetChild(NodeType.StructuredDocumentTag, 0, true);

            Document destinationDoc = new Document();
            Body body = destinationDoc.FirstSection.Body;
            body.RemoveAllChildren();

            foreach (Node node in sdt.GetChildNodes(NodeType.Any, false))
            {
                Node importNode = destinationDoc.ImportNode(node, true, ImportFormatMode.KeepSourceFormatting);
                body.AppendChild(importNode);
            }

            destinationDoc = TestUtil.SaveOpen(destinationDoc, @"Model\List\Test23735Saving.docx", null, false);

            Paragraph listParagraph = destinationDoc.FirstSection.Body.FirstParagraph;
            Assert.That(listParagraph.ListFormat.List.DurableId, Is.EqualTo(24866079));
        }

        /// <summary>
        /// WORDSNET-23735 Wrong list numbering due to loss and non-use of DurableId attribute values
        /// The test checks that when a list is being imported during updating an XML-mapped SDT, corresponding list
        /// is searched with using <see cref="Lists.List.DurableId"/> property values.
        /// </summary>
        [Test]
        public void Test23735Updating()
        {
            const string fileName = @"Model\List\Test23735Updating.docx";
            Document doc = TestUtil.Open(fileName);
            foreach (StructuredDocumentTag sdt in doc.GetChildNodes(NodeType.StructuredDocumentTag, true))
            {
                if (sdt.XmlMapping.IsMapped)
                    sdt.XmlMapping.UpdateContent();
            }

            TestUtil.Save(doc, fileName, null, true, GoldLevel.ExportOnly);
        }


        /// <summary>
        /// WORDSNET-23889 Wrong list numbering in SDT bound to custom XML part
        /// </summary>
        [Test]
        public void Test23889()
        {
            Document doc = TestUtil.Open(@"Model\List\Test23889.docx");
            int listCount = doc.Lists.Count;
            int listDefCount = doc.Lists.ListDefCount;

            Node[] sdts = doc.GetChildNodes(NodeType.StructuredDocumentTag, true).ToArray();
            foreach (Node sdt in sdts)
                ((StructuredDocumentTag)sdt).XmlMapping.UpdateContent();

            // Check that paragraph list numbers are correct.
            ListLabelUpdater.UpdateListLabels(doc);
            StructuredDocumentTag sdt2 = (StructuredDocumentTag)sdts[1];
            Paragraph paragraph3 = (Paragraph)sdt2.GetChildNodes(NodeType.Any, false)[2];
            Assert.That(paragraph3.ListLabel.LabelValue, Is.EqualTo(2));

            // Check that no new lists are created during updating the SDTs.
            Assert.That(doc.Lists.Count, Is.EqualTo(listCount));
            Assert.That(doc.Lists.ListDefCount, Is.EqualTo(listDefCount));

            // Checks that duplicate DurableId values are removed.
            List newList = doc.Lists.AddCopy(doc.Lists[doc.Lists.Count - 1]);
            Assert.That(newList.DurableId, IsNot.EqualTo(0));
            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);
            Assert.That(newList.DurableId, Is.EqualTo(0));
        }

        /// <summary>
        /// WORDSNET-24169 Wrong list numbering in SDT bound to custom XML part
        /// When importing nodes, Aspose.Words does not use an existing list at some cases but creates a new one.
        /// Implemented that when <see cref="ImportFormatOptions"/> is not set to <see cref="NodeImporter"/>, an
        /// existing list is used when importing nodes.
        /// </summary>
        [Test]
        public void Test24169()
        {
            Document doc = TestUtil.Open(@"Model\List\Test24169.docx");
            CompositeNode sdt = (CompositeNode)doc.GetChild(NodeType.StructuredDocumentTag, 1, true);

            Document destinationDocument = new Document();
            destinationDocument.CopyStylesFromTemplate(doc);
            destinationDocument.FirstSection.Body.RemoveAllChildren();
            int listDefCount = destinationDocument.Lists.ListDefCount;

            foreach (Node node in sdt.GetChildNodes(NodeType.Any, false))
            {
                Node importNode = destinationDocument.ImportNode(node, true, ImportFormatMode.KeepSourceFormatting);
                destinationDocument.LastSection.Body.AppendChild(importNode);
            }

            // Check that paragraph list numbers are correct.
            ListLabelUpdater.UpdateListLabels(destinationDocument);
            ParagraphCollection paragraphs = destinationDocument.FirstSection.Body.Paragraphs;
            Paragraph paragraph2 = paragraphs[1];
            Paragraph paragraph3 = paragraphs[2];
            Assert.That(paragraph2.ListLabel.LabelValue, Is.EqualTo(1));
            Assert.That(paragraph3.ListLabel.LabelValue, Is.EqualTo(2));

            // Check that lists of the numbering items have the same list definition.
            Assert.That(paragraph2.ListFormat.List.ListId == paragraph3.ListFormat.List.ListId, Is.False);
            Assert.That(paragraph2.ListFormat.List.ListDefId == paragraph3.ListFormat.List.ListDefId, Is.True);

            // Check that no new list definitions are created during importing paragraphs.
            Assert.That(destinationDocument.Lists.ListDefCount, Is.EqualTo(listDefCount));
        }

        /// <summary>
        /// WORDSNET-24444 List list labels are incorrect after rendering document.
        /// The Word does not increment the counter for a missing in the content levels (when level has increased by more
        /// than one) in the case when previously occurred level in the content between "lvlRestart" and current levels.
        /// Mimic the Word to fix the issue.
        /// </summary>
        [Test]
        public void Test24444()
        {
            Document doc = TestUtil.Open(@"Model\List\Test24444.docx");
            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            const string lvl7Label = "1 1 {0} 1 a i 0 1";
            const string lvl6Label = "1 1 {0} 1 a i 1";

            Assert.That(ListLabel(paras, 2), Is.EqualTo(string.Format(lvl7Label, 1)));
            Assert.That(ListLabel(paras, 3), Is.EqualTo(string.Format(lvl6Label, 1)));
            Assert.That(ListLabel(paras, 8), Is.EqualTo(string.Format(lvl7Label, 2)));
            Assert.That(ListLabel(paras, 9), Is.EqualTo(string.Format(lvl6Label, 2)));
            Assert.That(ListLabel(paras, 14), Is.EqualTo(string.Format(lvl7Label, 3)));
            Assert.That(ListLabel(paras, 15), Is.EqualTo(string.Format(lvl6Label, 3)));
            Assert.That(ListLabel(paras, 20), Is.EqualTo(string.Format(lvl7Label, 4)));
            Assert.That(ListLabel(paras, 21), Is.EqualTo(string.Format(lvl6Label, 4)));

            Assert.That(ListLabel(paras, 25), Is.EqualTo("1 1 5 1 a i 1 1"));
            Assert.That(ListLabel(paras, 26), Is.EqualTo("1 1 5 1 a i 2"));
        }



        /// <summary>
        /// WORDSNET-26909 Invalid numbering culture in docx-to-PDF conversion.
        /// The ganada (Korean Ganada Numbering) is not implemented in <see cref="NumberConverterCore.NumberToString"/>.
        /// </summary>
        [Test]
        public void Test26909()
        {
            Document doc = TestUtil.Open(@"Model\List\Test26909.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paras, 0), Is.EqualTo("가."));
            Assert.That(ListLabel(paras, 1), Is.EqualTo("나."));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("다."));
            Assert.That(ListLabel(paras, 3), Is.EqualTo("라."));
            Assert.That(ListLabel(paras, 4), Is.EqualTo("마."));
            Assert.That(ListLabel(paras, 5), Is.EqualTo("바."));
            Assert.That(ListLabel(paras, 6), Is.EqualTo("사."));
            Assert.That(ListLabel(paras, 7), Is.EqualTo("아."));
            Assert.That(ListLabel(paras, 8), Is.EqualTo("자."));
            Assert.That(ListLabel(paras, 9), Is.EqualTo("차."));
            Assert.That(ListLabel(paras, 10), Is.EqualTo("카."));
            Assert.That(ListLabel(paras, 11), Is.EqualTo("타."));
            Assert.That(ListLabel(paras, 12), Is.EqualTo("파."));
            Assert.That(ListLabel(paras, 13), Is.EqualTo("하."));

            // Only the first 14 numbers are present in the Ganada and the rest are repeated in a loop.
            Assert.That(ListLabel(paras, 14), Is.EqualTo("가."));
            Assert.That(ListLabel(paras, 15), Is.EqualTo("나."));
            Assert.That(ListLabel(paras, 16), Is.EqualTo("다."));
            Assert.That(ListLabel(paras, 17), Is.EqualTo("라."));
            Assert.That(ListLabel(paras, 18), Is.EqualTo("마."));
            Assert.That(ListLabel(paras, 19), Is.EqualTo("바."));
            Assert.That(ListLabel(paras, 20), Is.EqualTo("사."));
            Assert.That(ListLabel(paras, 21), Is.EqualTo("아."));
            Assert.That(ListLabel(paras, 22), Is.EqualTo("자."));
            Assert.That(ListLabel(paras, 23), Is.EqualTo("차."));
            Assert.That(ListLabel(paras, 24), Is.EqualTo("카."));
            Assert.That(ListLabel(paras, 25), Is.EqualTo("타."));
            Assert.That(ListLabel(paras, 26), Is.EqualTo("파."));
            Assert.That(ListLabel(paras, 27), Is.EqualTo("하."));

            Assert.That(ListLabel(paras, 28), Is.EqualTo("가."));
            Assert.That(ListLabel(paras, 29), Is.EqualTo("나."));
            Assert.That(ListLabel(paras, 30), Is.EqualTo("다."));
            Assert.That(ListLabel(paras, 31), Is.EqualTo("라."));
            Assert.That(ListLabel(paras, 32), Is.EqualTo("마."));
            Assert.That(ListLabel(paras, 33), Is.EqualTo("바."));
            Assert.That(ListLabel(paras, 34), Is.EqualTo("사."));

            // Check 0 fallbacks to arabic representation.
            Assert.That(ListLabel(paras, 35), Is.EqualTo("0."));
        }

        /// <summary>
        /// Added support for <see cref="NumberStyle.Custom"/> numbering style.
        /// Tests ListLevel.CustomNumberStyle property setter.
        /// </summary>
        [Test]
        public void TestCustomNumberStyleFormatSetter()
        {
            Document doc = TestUtil.Open(@"Model\List\TestCustomLeadingZero4.docx");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            doc.UpdateListLabels();

            Assert.That(ListLabel(paras, 1), Is.EqualTo("00001."));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("00002."));

            // Change and check again.
            paras[1].ListFormat.ListLevel.CustomNumberStyleFormat = "001, 002, 003, ...";

            doc.UpdateListLabels();

            Assert.That(ListLabel(paras, 1), Is.EqualTo("001."));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("002."));

            // Check roundtrip.
            doc = TestUtil.SaveOpen(doc, @"Model\List\TestCustomNumberStyleFormatSetter.docx", new OoxmlSaveOptions(), false);
            Assert.That(doc.FirstSection.Body.Paragraphs[1].ListFormat.ListLevel.CustomNumberStyleFormat, Is.EqualTo("001, 002, 003, ..."));
        }

        [Test, ExpectedException(typeof(System.ArgumentNullException))]
        public void TestCustomNumberStyleFormatSetterNull()
        {
            Document doc = TestUtil.Open(@"Model\List\TestCustomLeadingZero4.docx");
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            // Try set null.
            paras[1].ListFormat.ListLevel.CustomNumberStyleFormat = null;
        }

        /// <summary>
        /// WORDSNET-27187 Chinese becomes Arabic numbering after conversion to HTML.
        /// The koreanDigital2 numbering format is not implemented in <see cref="NumberConverterCore.NumberToString"/>.
        /// </summary>
        [Test]
        public void Test27187()
        {
            Document doc = TestUtil.Open(@"Model\List\Test27187.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            // 0 - 21
            Assert.That(ListLabel(paras, 0), Is.EqualTo("零."));
            Assert.That(ListLabel(paras, 1), Is.EqualTo("一."));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("二."));
            Assert.That(ListLabel(paras, 3), Is.EqualTo("三."));
            Assert.That(ListLabel(paras, 4), Is.EqualTo("四."));
            Assert.That(ListLabel(paras, 5), Is.EqualTo("五."));
            Assert.That(ListLabel(paras, 6), Is.EqualTo("六."));
            Assert.That(ListLabel(paras, 7), Is.EqualTo("七."));
            Assert.That(ListLabel(paras, 8), Is.EqualTo("八."));
            Assert.That(ListLabel(paras, 9), Is.EqualTo("九."));
            Assert.That(ListLabel(paras, 10), Is.EqualTo("一零."));
            Assert.That(ListLabel(paras, 11), Is.EqualTo("一一."));
            Assert.That(ListLabel(paras, 12), Is.EqualTo("一二."));
            Assert.That(ListLabel(paras, 13), Is.EqualTo("一三."));
            Assert.That(ListLabel(paras, 14), Is.EqualTo("一四."));
            Assert.That(ListLabel(paras, 15), Is.EqualTo("一五."));
            Assert.That(ListLabel(paras, 16), Is.EqualTo("一六."));
            Assert.That(ListLabel(paras, 17), Is.EqualTo("一七."));
            Assert.That(ListLabel(paras, 18), Is.EqualTo("一八."));
            Assert.That(ListLabel(paras, 19), Is.EqualTo("一九."));
            Assert.That(ListLabel(paras, 20), Is.EqualTo("二零."));
            Assert.That(ListLabel(paras, 21), Is.EqualTo("二一."));

            Assert.That(ListLabel(paras, 22), Is.EqualTo("九九.")); //99
            Assert.That(ListLabel(paras, 23), Is.EqualTo("一零零.")); // 100

            Assert.That(ListLabel(paras, 24), Is.EqualTo("一零零零.")); // 1000
            Assert.That(ListLabel(paras, 25), Is.EqualTo("一零零一.")); // 1001
            Assert.That(ListLabel(paras, 26), Is.EqualTo("一零零二.")); // 1002
        }

        /// <summary>
        /// WORDSNET-27573 Korean Numbering is changed to Arabic after rendering.
        /// The chosung (Korean Chosung Numbering) is not implemented in <see cref="NumberConverterCore.NumberToString"/>.
        /// </summary>
        [Test]
        public void Test27573Chosung()
        {
            Document doc = TestUtil.Open(@"Model\List\Test27573Chosung.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(ListLabel(paras, 0), Is.EqualTo("ㄱ."));
            Assert.That(ListLabel(paras, 1), Is.EqualTo("ㄴ."));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("ㄷ."));
            Assert.That(ListLabel(paras, 3), Is.EqualTo("ㄹ."));
            Assert.That(ListLabel(paras, 4), Is.EqualTo("ㅁ."));
            Assert.That(ListLabel(paras, 5), Is.EqualTo("ㅂ."));
            Assert.That(ListLabel(paras, 6), Is.EqualTo("ㅅ."));
            Assert.That(ListLabel(paras, 7), Is.EqualTo("ㅇ."));
            Assert.That(ListLabel(paras, 8), Is.EqualTo("ㅈ."));
            Assert.That(ListLabel(paras, 9), Is.EqualTo("ㅊ."));
            Assert.That(ListLabel(paras, 10), Is.EqualTo("ㅋ."));
            Assert.That(ListLabel(paras, 11), Is.EqualTo("ㅌ."));
            Assert.That(ListLabel(paras, 12), Is.EqualTo("ㅍ."));
            Assert.That(ListLabel(paras, 13), Is.EqualTo("ㅎ."));

            // Only the first 14 numbers are present in the Chosung and the rest are repeated in a loop.
            Assert.That(ListLabel(paras, 14), Is.EqualTo("ㄱ."));
            Assert.That(ListLabel(paras, 15), Is.EqualTo("ㄴ."));
            Assert.That(ListLabel(paras, 16), Is.EqualTo("ㄷ."));
            Assert.That(ListLabel(paras, 17), Is.EqualTo("ㄹ."));
            Assert.That(ListLabel(paras, 18), Is.EqualTo("ㅁ."));
            Assert.That(ListLabel(paras, 19), Is.EqualTo("ㅂ."));
            Assert.That(ListLabel(paras, 20), Is.EqualTo("ㅅ."));
            Assert.That(ListLabel(paras, 21), Is.EqualTo("ㅇ."));
            Assert.That(ListLabel(paras, 22), Is.EqualTo("ㅈ."));
            Assert.That(ListLabel(paras, 23), Is.EqualTo("ㅊ."));
            Assert.That(ListLabel(paras, 24), Is.EqualTo("ㅋ."));
            Assert.That(ListLabel(paras, 25), Is.EqualTo("ㅌ."));
            Assert.That(ListLabel(paras, 26), Is.EqualTo("ㅍ."));
            Assert.That(ListLabel(paras, 27), Is.EqualTo("ㅎ."));

            Assert.That(ListLabel(paras, 28), Is.EqualTo("ㄱ."));
            Assert.That(ListLabel(paras, 29), Is.EqualTo("ㄴ."));
            Assert.That(ListLabel(paras, 30), Is.EqualTo("ㄷ."));
            Assert.That(ListLabel(paras, 31), Is.EqualTo("ㄹ."));
            Assert.That(ListLabel(paras, 32), Is.EqualTo("ㅁ."));
            Assert.That(ListLabel(paras, 33), Is.EqualTo("ㅂ."));
            Assert.That(ListLabel(paras, 34), Is.EqualTo("ㅅ."));
        }


        /// <summary>
        /// WORDSNET-28954 Numeral format is changed after rendering document.
        /// The taiwaneseCounting (Taiwanese Counting System) is not implemented in <see cref="NumberConverterCore.NumberToString"/>.
        /// </summary>
        [Test]
        public void Test28954()
        {
            Document doc = TestUtil.Open(@"Model\List\Test28954taiwaneseCounting.docx");

            doc.UpdateListLabels();

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            // 1-9
            Assert.That(ListLabel(paras, 0), Is.EqualTo("一."));
            Assert.That(ListLabel(paras, 1), Is.EqualTo("二."));
            Assert.That(ListLabel(paras, 2), Is.EqualTo("三."));
            Assert.That(ListLabel(paras, 3), Is.EqualTo("四."));
            Assert.That(ListLabel(paras, 4), Is.EqualTo("五."));
            Assert.That(ListLabel(paras, 5), Is.EqualTo("六."));
            Assert.That(ListLabel(paras, 6), Is.EqualTo("七."));
            Assert.That(ListLabel(paras, 7), Is.EqualTo("八."));
            Assert.That(ListLabel(paras, 8), Is.EqualTo("九."));

            // 10-15
            Assert.That(ListLabel(paras, 9), Is.EqualTo("十."));
            Assert.That(ListLabel(paras, 10), Is.EqualTo("十一."));
            Assert.That(ListLabel(paras, 11), Is.EqualTo("十二."));
            Assert.That(ListLabel(paras, 12), Is.EqualTo("十三."));
            Assert.That(ListLabel(paras, 13), Is.EqualTo("十四."));
            Assert.That(ListLabel(paras, 14), Is.EqualTo("十五."));

            // 20-22
            Assert.That(ListLabel(paras, 15), Is.EqualTo("二十."));
            Assert.That(ListLabel(paras, 16), Is.EqualTo("二十一."));
            Assert.That(ListLabel(paras, 17), Is.EqualTo("二十二."));

            // 30-31
            Assert.That(ListLabel(paras, 18), Is.EqualTo("三十."));
            Assert.That(ListLabel(paras, 19), Is.EqualTo("三十一."));

            // 40
            Assert.That(ListLabel(paras, 20), Is.EqualTo("四十."));
            // 50
            Assert.That(ListLabel(paras, 21), Is.EqualTo("五十."));
            // 60
            Assert.That(ListLabel(paras, 22), Is.EqualTo("六十."));
            // 70
            Assert.That(ListLabel(paras, 23), Is.EqualTo("七十."));
            // 80
            Assert.That(ListLabel(paras, 24), Is.EqualTo("八十."));
            // 90
            Assert.That(ListLabel(paras, 25), Is.EqualTo("九十."));

            // 99-112
            Assert.That(ListLabel(paras, 26), Is.EqualTo("九十九."));
            Assert.That(ListLabel(paras, 27), Is.EqualTo("一○○."));
            Assert.That(ListLabel(paras, 28), Is.EqualTo("一○一."));
            Assert.That(ListLabel(paras, 29), Is.EqualTo("一○二."));
            Assert.That(ListLabel(paras, 30), Is.EqualTo("一○三."));
            Assert.That(ListLabel(paras, 31), Is.EqualTo("一○四."));
            Assert.That(ListLabel(paras, 32), Is.EqualTo("一○五."));
            Assert.That(ListLabel(paras, 33), Is.EqualTo("一○六."));
            Assert.That(ListLabel(paras, 34), Is.EqualTo("一○七."));
            Assert.That(ListLabel(paras, 35), Is.EqualTo("一○八."));
            Assert.That(ListLabel(paras, 36), Is.EqualTo("一○九."));
            Assert.That(ListLabel(paras, 37), Is.EqualTo("一一○."));
            Assert.That(ListLabel(paras, 38), Is.EqualTo("一一一."));
            Assert.That(ListLabel(paras, 39), Is.EqualTo("一一二."));

            // 120-121
            Assert.That(ListLabel(paras, 40), Is.EqualTo("一二○."));
            Assert.That(ListLabel(paras, 41), Is.EqualTo("一二一."));

            // 200-202
            Assert.That(ListLabel(paras, 42), Is.EqualTo("二○○."));
            Assert.That(ListLabel(paras, 43), Is.EqualTo("二○一."));
            Assert.That(ListLabel(paras, 44), Is.EqualTo("二○二."));

            // 999-1001
            Assert.That(ListLabel(paras, 45), Is.EqualTo("九九九."));
            Assert.That(ListLabel(paras, 46), Is.EqualTo("一○○○."));
            Assert.That(ListLabel(paras, 47), Is.EqualTo("一○○一."));

            // 2000
            Assert.That(ListLabel(paras, 48), Is.EqualTo("二○○○."));

            // 10,000-10,001
            Assert.That(ListLabel(paras, 49), Is.EqualTo("一○○○○."));
            Assert.That(ListLabel(paras, 50), Is.EqualTo("一○○○一."));

            // 11,111
            Assert.That(ListLabel(paras, 51), Is.EqualTo("一一一一一."));

            // 32767
            Assert.That(ListLabel(paras, 52), Is.EqualTo("三二七六七."));
        }

        private static void CheckListRevision(Paragraph p, int originalListId, int finalListId, string originalLabel, string finalLabel)
        {
            Assert.That(p.FetchParaAttr(ParaAttr.ListId, RevisionsView.Original), Is.EqualTo(originalListId));
            Assert.That(p.FetchParaAttr(ParaAttr.ListId, RevisionsView.Final), Is.EqualTo(finalListId));

            CheckListRevision(p, originalLabel, finalLabel);
        }

        private static void CheckListRevision(Paragraph p, string originalLabel, string finalLabel)
        {
            Assert.That(p.ListLabel.LabelStringOriginal, Is.EqualTo(originalLabel));
            Assert.That(p.ListLabel.LabelStringFinal, Is.EqualTo(finalLabel));
        }

        /// <summary>
        /// Returns list label for the paragraph with the specified index from the collection.
        /// </summary>
        private static string ListLabel(ParagraphCollection paras, int paraIndex)
        {
            return paras[paraIndex].ListLabel.LabelString;
        }
    }
}
