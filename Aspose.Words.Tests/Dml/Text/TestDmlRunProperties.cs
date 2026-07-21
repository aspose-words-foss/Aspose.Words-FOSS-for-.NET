// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using System.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Text;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Text
{
    [TestFixture]
    public class TestDmlRunProperties
    {
        [Test]
        public void GetFontStyle_BoldProps_BoldFontStyleReturned()
        {
            // Arrange
            DmlRunProperties props = new DmlRunProperties();
            props.Bold = true;
            // Act
            CheckFontStyle(FontStyle.Bold, props);
        }

        [Test]
        public void GetFontStyle_DefaultProps_RegularFontStyleReturned()
        {
            // Arrange
            DmlRunProperties props = new DmlRunProperties();
            // Act
            CheckFontStyle(FontStyle.Regular, props);
        }

        [Test]
        public void GetFontStyle_ItalicProps_ItalicsFontStyleReturned()
        {
            // Arrange
            DmlRunProperties props = new DmlRunProperties();
            props.Italics = true;
            // Act
            CheckFontStyle(FontStyle.Italic, props);
        }

        [Test]
        public void GetFontStyle_TextStrike_StrikeFontStyleReturned()
        {
            // Arrange
            DmlRunProperties props = new DmlRunProperties();
            props.Strikethrough = DmlTextStrike.Single;
            // Act
            CheckFontStyle(FontStyle.Strikeout, props);

            // currently we will render double strike as single strike. Later can be reworked
            // Arrange
            props.Strikethrough = DmlTextStrike.Double;
            // Act
            CheckFontStyle(FontStyle.Strikeout, props);
        }

        [Test]
        public void GetFontStyle_UnderlineProps_UnderlineFontStyleReturned()
        {
            // Arrange
            DmlRunProperties props = new DmlRunProperties();
            props.Underline = Underline.WavyDouble;
            // Act
            CheckFontStyle(FontStyle.Underline, props);
        }

        /// <summary>
        /// WORDSNET-14139 Some DML run properties are not supported.
        /// Read/write was implemented.
        /// </summary>
        [Test]
        public void TestJira14139()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\DrawingML\TestJira14139", UnifiedScenario.Docx2DocxNoGold);
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            DmlTextShape result = ((DmlShape)shape.DmlNode).TextShape;

            DmlParagraph paragraph = (DmlParagraph)result.TextBody.Paragraphs[0];
            DmlRun run = (DmlRun)paragraph.Elements[2];
            DmlRunProperties props = run.RunProperties;
            Assert.That(props.UnderlineFill is DmlSolidFill, Is.True);
            Assert.That(props.UnderlineStroke.Fill is DmlSolidFill, Is.True);
            Assert.That(props.UnderlineFillTx, Is.False);
            Assert.That(props.UnderlineStrokeTx, Is.False);

            Assert.That(props.RightToLeftFlowDirection, Is.True);

            run = (DmlRun)paragraph.Elements[1];
            props = run.RunProperties;
            Assert.That(props.UnderlineFill == null, Is.True);
            Assert.That(props.UnderlineStroke.Fill is DmlNoFill, Is.True);
            Assert.That(props.UnderlineFillTx, Is.True);
            Assert.That(props.UnderlineStrokeTx, Is.True);

            Assert.That(props.RightToLeftFlowDirection, Is.False);
        }

        /// <summary>
        /// WORDSNET-15502 Corrupt output DOCX document containing SmartArt.
        /// On writing DmlFont, we must write "typeface" attribute despite the value is empty.
        /// </summary>
        [Test]
        public void TestJira15502()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\DrawingML\TestJira15502", UnifiedScenario.Docx2DocxNoGold);

            // The model of the document remains the same after the fix, so the only way to verify the fix
            // is to check content of the problematic package. 
            TestUtil.IsWrittenToOpcPackage(doc.OriginalFileName, @"/word/diagrams/data1.xml", "a:ea typeface");
        }

        private static void CheckFontStyle(FontStyle expectedFontStyle, DmlRunProperties props)
        {
            FontStyle style = props.GetFontStyle();
            // Assert
            Assert.That(style, Is.EqualTo(expectedFontStyle));
        }
    }
}
