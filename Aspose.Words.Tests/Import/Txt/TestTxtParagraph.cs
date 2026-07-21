// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2012 by Alexey Butalov
using Aspose.Words.RW.Txt.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Txt
{
    /// <summary>
    /// Unit tests for TxtParagraph 
    /// </summary>
    [TestFixture]
    public class TestTxtParagraph
    {
        [Test]
        public void TestAppendText()
        {
            TxtParagraph txtParagraph = new TxtParagraph();
            Assert.That(txtParagraph.LinesCount, Is.EqualTo(0));
            Assert.That(txtParagraph.GetText(), Is.EqualTo(""));

            txtParagraph.AppendText("line1");
            Assert.That(txtParagraph.LinesCount, Is.EqualTo(1));
            Assert.That(txtParagraph.GetText(), Is.EqualTo("line1"));

            txtParagraph.AppendText("line2");
            Assert.That(txtParagraph.LinesCount, Is.EqualTo(2));
            Assert.That(txtParagraph.GetText(), Is.EqualTo("line1 line2"));
        }
    }
}