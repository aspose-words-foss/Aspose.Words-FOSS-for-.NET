// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/12/2019 by Ilya Navrotskiy

using System.Drawing;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing FillFormat properties using DocumentBuilder.
    /// </summary>
    [TestFixture]
    public class TestFillFormatBuilder : TestFillFormatBase
    {
           /// <summary>
        /// Tests FillFormat properties of the Font.
        /// </summary>
        [TestCase(ShapeMarkupLanguage.Dml)]
        [TestCase(ShapeMarkupLanguage.Vml)]
        public void TestFont(ShapeMarkupLanguage markupLanguage)
        {
            Color foreColor = Color.Chocolate;
            Color backColor = Color.BlueViolet;
            const double transparency = 0.43;

            DocumentBuilder builder = CreateDocument(markupLanguage);

            Fill fill = builder.Font.Fill;
            fill.ForeColor = foreColor;
            fill.BackColor = backColor;
            fill.Transparency = transparency;

            builder.Writeln("Text");

            string ext = MarkupLanguageToExtension(markupLanguage);
            string outFileName = string.Format("FillFormat\\TextBuilder{0}{1}", markupLanguage, ext);

            // Roundtrip the document.
            Document doc = TestUtil.SaveOpen(builder.Document, outFileName, CreateSaveOptions(markupLanguage), false);

            fill = GetFill(doc);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(fill.Visible, Is.EqualTo(true));
            Assert.That(fill.BackColor, Is.EqualTo(Color.Empty));

            Color expectedForeColor = (markupLanguage == ShapeMarkupLanguage.Dml) ? foreColor : Color.Empty;
            CheckColor(expectedForeColor, fill.ForeColor);

            double expectedTransparency = (markupLanguage == ShapeMarkupLanguage.Dml) ? transparency : 0.0;
            Assert.That(fill.Transparency, Is.EqualTo(expectedTransparency).Within(0.005));
        }

        /// <summary>
        /// Tests default FillFormat properties of the Font.
        /// </summary>
        [TestCase(ShapeMarkupLanguage.Dml)]
        [TestCase(ShapeMarkupLanguage.Vml)]
        public void TestFontDefault(ShapeMarkupLanguage markupLanguage)
        {
            DocumentBuilder builder = CreateDocument(markupLanguage);

            builder.Writeln("Text");

            Fill fill = GetFill(builder.Document);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(fill.ForeColor, Is.EqualTo(Color.Empty));
            Assert.That(fill.BackColor, Is.EqualTo(Color.Empty));
            Assert.That(fill.Transparency, Is.EqualTo(0.0).Within(0.001));
            Assert.That(fill.Visible, Is.EqualTo(true));
        }
    }
}
