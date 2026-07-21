// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2012 by Alexey Butalov
using System.IO;
using System.Text;
using Aspose.Words.RW.Txt.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Txt
{
    /// <summary>
    /// Tests how TxtParagraphReader detects paragraphs in various text. 
    /// </summary>
    [TestFixture]
    public class TestTxtParagraphReader
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-11467 Paragraph with big left indentation value is rendering incorrectly in PDF
        /// WORDSNET-11468 Page breaks are not preserved in PDF
        /// 
        /// Leading spaces and page break should be imported to model 
        /// </summary>
        [Test]
        public void TestJira11468_11468()
        {
            const string text = @"                                                                     PAGE 0 OF Y
PROTOCOL";
            TxtParagraphReader paragraphReader = CreateParagraphReader(text);
            TxtParagraph[] paragraphs = paragraphReader.Read();

            Assert.That(paragraphs.Length, Is.EqualTo(2));
            CheckParagraph(paragraphs[0], 69, 0, "PAGE 0 OF Y");
            CheckParagraph(paragraphs[1], 0, 0, "\fPROTOCOL");
        }

        [Test]
        public void TestReadParagraphs()
        {
            const string text = @"line1 line1 line1
line2
    line3 line3
 line4

                line6
line7";
            TxtParagraphReader paragraphReader = CreateParagraphReader(text);
            TxtParagraph[] paragraphs = paragraphReader.Read();

            Assert.That(paragraphs.Length, Is.EqualTo(7));
            CheckParagraph(paragraphs[0], 0, 0, "line1 line1 line1");
            CheckParagraph(paragraphs[1], 0, 0, "line2");
            CheckParagraph(paragraphs[2], 4, 0, "line3 line3");
            CheckParagraph(paragraphs[3], 1, 0, "line4");
            CheckParagraph(paragraphs[4], 0, 0, "");
            CheckParagraph(paragraphs[5], 16, 0, "line6");
            CheckParagraph(paragraphs[6], 0 ,0, "line7");
        }

        [Test]
        public void TestReadBullet1()
        {
            const string text = @"  * line1 line1
  line2
  * line3 line3";
            TxtParagraphReader paragraphReader = CreateParagraphReader(text);
            TxtParagraph[] paragraphs = paragraphReader.Read();

            Assert.That(paragraphs.Length, Is.EqualTo(3));
            CheckParagraph(paragraphs[0], 4, 0, "line1 line1");
            CheckNumbering(paragraphs[0].Numbering, 2, null, null, "*");
            CheckParagraph(paragraphs[1], 2, 0, "line2");
            CheckParagraph(paragraphs[2], 4, 0, "line3 line3");
            CheckNumbering(paragraphs[2].Numbering, 2, paragraphs[0], null, "*");
        }

        [Test]
        public void TestReadNumbering1()
        {
            const string text = @" 1. line1 line1 

 2. line2 line2";
            TxtParagraphReader paragraphReader = CreateParagraphReader(text);
            TxtParagraph[] paragraphs = paragraphReader.Read();

            Assert.That(paragraphs.Length, Is.EqualTo(3));
            CheckParagraph(paragraphs[0], 4, 0, "line1 line1");
            CheckNumbering(paragraphs[0].Numbering, 1, null, null, "1");
            CheckParagraph(paragraphs[1], 0, 0, "");
            CheckParagraph(paragraphs[2], 4, 0, "line2 line2");
            CheckNumbering(paragraphs[2].Numbering, 1, paragraphs[0], null, "2");
        }

        [Test]
        public void TestReadMultilineNumbering1()
        {
            const string text = @" 1. line1 line1 
    line2 line2";
            TxtParagraphReader paragraphReader = CreateParagraphReader(text);
            TxtParagraph[] paragraphs = paragraphReader.Read();

            Assert.That(paragraphs.Length, Is.EqualTo(2));
            CheckParagraph(paragraphs[0], 4, 0, "line1 line1");
            CheckNumbering(paragraphs[0].Numbering, 1, null, null, "1");
            CheckParagraph(paragraphs[1], 4, 0, "line2 line2");
        }

        [Test]
        public void TestReadNumberingMultilevel1()
        {
            const string text = @"1. line1 line1 
  1.1. line1.1
  1.2. line1.2
 2. line2 line2";
            TxtParagraphReader paragraphReader = CreateParagraphReader(text);
            TxtParagraph[] paragraphs = paragraphReader.Read();

            Assert.That(paragraphs.Length, Is.EqualTo(4));
            CheckParagraph(paragraphs[0], 3, 0, "line1 line1");
            CheckNumbering(paragraphs[0].Numbering, 0, null, null, "1");
            CheckParagraph(paragraphs[1], 7, 0, "line1.1");
            CheckNumbering(paragraphs[1].Numbering, 2, null, paragraphs[0], "1", "1");
            CheckParagraph(paragraphs[2], 7, 0, "line1.2");
            CheckNumbering(paragraphs[2].Numbering, 2, paragraphs[1], null, "1", "2");
            CheckParagraph(paragraphs[3], 4, 0, "line2 line2");
            CheckNumbering(paragraphs[3].Numbering, 1, paragraphs[0], null, "2");
        }



        /// <summary>
        /// WORDSNET-9968 The first asterisk in TXT file imported with a "Times New Roman" font.
        /// We should specify font properties for a ListLevel.
        /// </summary>
        [Test]
        public void TestJira9968()
        {
            Document doc = TestUtil.Open(@"ImportTxt\TestJira9968.txt");
            
            Font font = doc.Lists[0].ListLevels[0].Font;

            Assert.That("Courier New", Is.EqualTo(font.Name));
            Assert.That(10.5, Is.EqualTo(font.Size));
            Assert.That(10.5, Is.EqualTo(font.SizeBi));
        }

        private static void CheckNumbering(TxtNumbering numbering,
            int numberPosition,
            TxtParagraph prevNumberingParagraph,
            TxtParagraph parentNumberingParagraph,
            params string[] numbers)
        {
            Assert.That(numbering, IsNot.Null());
            Assert.That(numbering.NumberPosition, Is.EqualTo(numberPosition));
            Assert.That(numbering.PrevNumberingParagraph, Is.EqualTo(prevNumberingParagraph));
            Assert.That(numbering.ParentNumberingParagraph, Is.EqualTo(parentNumberingParagraph));
            if (numbers.Length != 0)
            {
                Assert.That(numbering.Numbers.Length, Is.EqualTo(numbers.Length));
                for (int i = 0; i < numbers.Length; i++)
                    Assert.That(numbering.Numbers[i], Is.EqualTo(numbers[i]));
            }
        }

        private static void CheckParagraph(TxtParagraph paragraph, int firstLineIndent, int leftIndent, string text)
        {
            Assert.That(paragraph, IsNot.Null());
            Assert.That(paragraph.FirstLineIndent, Is.EqualTo(firstLineIndent));
            Assert.That(paragraph.LeftIndent, Is.EqualTo(leftIndent));
            Assert.That(paragraph.GetText(), Is.EqualTo(text));
        }

        private static TxtParagraphReader CreateParagraphReader(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            MemoryStream stream = new MemoryStream(bytes);
            StreamReader streamReader = new StreamReader(stream, Encoding.Unicode);
            return new TxtParagraphReader(streamReader);
        }
    }
}
