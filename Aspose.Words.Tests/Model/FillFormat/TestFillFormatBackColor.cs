// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/12/2019 by Ilya Navrotskiy

using System.Drawing;
using System.Globalization;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing BackColor property of the FillFormat object.
    /// </summary>
    [TestFixture]
    public class TestFillFormatBackColor : TestFillFormatBase
    {
        /// <summary>
        /// Tests BackColor of the various one-colored Font fills.
        /// </summary>
        /// <remarks> This is test for the fills that cannot have second color (back color) at all. </remarks>
        [TestCase("TextSolid")]
        [TestCase("TextNoFill")]
        public void TestFontOneColoredFill(string testName)
        {
            // DML
            TestFillBackColor(testName, ShapeMarkupLanguage.Dml, Color.Empty, Color.Peru, Color.Empty);

            // VML
            TestFillBackColor(testName, ShapeMarkupLanguage.Vml, Color.Empty, Color.Peru, Color.Empty);
        }

        /// <summary>
        /// Tests BackColor of the gradient Font fills.
        /// </summary>
        [TestCase("TextPresetGradient", 0xCC, 0xCC, 0xFF)]
        [TestCase("TextTwoColorGradient", 0x00, 0x00, 0x00)]
        [TestCase("TextOneColorGradient", 0x00, 0x80, 0x80)]
        public void TestFontGradientFill(string testName, int r, int g, int b)
        {
            Color expectedOriginalColor = Color.FromArgb(r, g, b);

            // DML
            TestFillBackColor(testName, ShapeMarkupLanguage.Dml, expectedOriginalColor, Color.Peru);

            // VML
            TestFillBackColor(testName, ShapeMarkupLanguage.Vml, Color.Empty, Color.Peru, Color.Empty);
        }

        /// <summary>
        /// Tests BackColor of the various Shape fills.
        /// </summary>
        /// <remarks> These fills have second (back color) and are able to change it. </remarks>
        [TestCase("OneColorGradient", 0x00, 0x3B, 0x3B)]
        [TestCase("TwoColorGradient", 0xFF, 0xFF, 0xFF)]
        [TestCase("PresetGradient", 0xCC, 0xCC, 0xFF)]
        [TestCase("Patterned", 0xFF, 0xFF, 0xFF)]
        public void TestShapeFill(string testName, int r, int g, int b)
        {
            Color expectedOriginalColor = Color.FromArgb(r, g, b);

            // Dml
            TestFillBackColor(testName, ShapeMarkupLanguage.Dml, expectedOriginalColor, Color.Black);
            TestFillBackColor(testName, ShapeMarkupLanguage.Dml, expectedOriginalColor, Color.White);
            TestFillBackColor(testName, ShapeMarkupLanguage.Dml, expectedOriginalColor, Color.Peru);

            // Vml
            TestFillBackColor(testName, ShapeMarkupLanguage.Vml, expectedOriginalColor, Color.Black);
            TestFillBackColor(testName, ShapeMarkupLanguage.Vml, expectedOriginalColor, Color.White);
            TestFillBackColor(testName, ShapeMarkupLanguage.Vml, expectedOriginalColor, Color.Peru);
        }

        /// <summary>
        /// Tests BackColor of the Shape with Solid fill.
        /// </summary>
        /// <remarks>
        /// Solid fill cannot have second color (back color). But in VBA it still can be applied to VML shape.
        /// </remarks>
        [Test]
        public void TestShapeSolidFill()
        {
            const string testName = "Solid";

            // DML
            TestFillBackColor(testName, ShapeMarkupLanguage.Dml, Color.Empty, Color.Peru, Color.Empty);

            // VML
            TestFillBackColor(testName, ShapeMarkupLanguage.Vml, Color.White, Color.Black);
            TestFillBackColor(testName, ShapeMarkupLanguage.Vml, Color.White, Color.White);
            TestFillBackColor(testName, ShapeMarkupLanguage.Vml, Color.White, Color.Peru);
        }

        /// <summary>
        /// Tests BackColor of the various Shape fills.
        /// </summary>
        /// <remarks>
        /// Setting BackColor in Word VBA for these fills for DML shapes does not actually change the color (and also
        /// does not throw). But for VML it does.
        /// </remarks>
        [TestCase("PresetTextured")]
        [TestCase("UserPicture")]
        [TestCase("UserTextured")]
        public void TestShapeFillEmptyA(string testName)
        {
            // DML
            TestFillBackColor(testName, ShapeMarkupLanguage.Dml, Color.Empty, Color.Peru, Color.Empty);

            // VML
            TestFillBackColor(testName, ShapeMarkupLanguage.Vml, Color.White, Color.Black);
            TestFillBackColor(testName, ShapeMarkupLanguage.Vml, Color.White, Color.Peru);
        }

        /// <summary>
        /// Tests BackColor of the 'NoFill' Shape fill.
        /// </summary>
        [Test]
        public void TestShapeNoFill()
        {
            Document doc = TestFillBackColor("NoFill", ShapeMarkupLanguage.Dml, Color.Empty, Color.Peru);
            Assert.That(GetFill(doc).FillType, Is.EqualTo(FillType.Patterned));

            doc = TestFillBackColor("NoFill", ShapeMarkupLanguage.Vml, Color.White, Color.Peru);
            Assert.That(GetFill(doc).FillType, Is.EqualTo(FillType.Solid));
        }

        /// <summary>
        /// Tests BackColor of the first fillable object in a document at a specified path.
        /// </summary>
        private static Document TestFillBackColor(string testFileNameWithoutExtension, ShapeMarkupLanguage markupLanguage,
            Color originalColor, Color desiredColor)
        {
            return TestFillBackColor(testFileNameWithoutExtension, markupLanguage, originalColor, desiredColor, desiredColor);
        }

        /// <summary>
        /// Tests BackColor of the first fillable object in a document at a specified path.
        /// </summary>
        private static Document TestFillBackColor(string testFileNameWithoutExtension, ShapeMarkupLanguage markupLanguage,
            Color originalColor, Color desiredColor, Color actualColor)
        {
            Document doc = Open(testFileNameWithoutExtension, markupLanguage);
            return TestFillBackColor(doc, testFileNameWithoutExtension, markupLanguage,
                originalColor, desiredColor, actualColor);
        }

        /// <summary>
        /// Tests BackColor of the first fillable object in a specified document.
        /// </summary>
        private static Document TestFillBackColor(Document doc, string testFileNameWithoutExtension,
            ShapeMarkupLanguage markupLanguage, Color originalColor, Color desiredColor, Color actualColor)
        {
            Fill fill = GetFill(doc);
            CheckColor(originalColor, fill.BackColor);

            // Set desired BackColor.
            fill.BackColor = desiredColor;
            CheckColor(actualColor, fill.BackColor);

            string ext = MarkupLanguageToExtension(markupLanguage);
            string outFileName = string.Format("FillFormat\\{0}_BackColor_{1}{2}",
                testFileNameWithoutExtension, desiredColor.ToArgb().ToString("X", CultureInfo.InvariantCulture), ext);

            // Roundtrip the document.
            doc = TestUtil.SaveOpen(doc, outFileName, CreateSaveOptions(markupLanguage), false);

            fill = GetFill(doc);
            CheckColor(actualColor, fill.BackColor);

            return doc;
        }
    }
}
