// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/12/2019 by Ilya Navrotskiy

using System.Drawing;
using System.Globalization;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing ForeColor property of the FillFormat object.
    /// </summary>
    [TestFixture]
    public class TestFillFormatForeColor : TestFillFormatBase
    {
        /// <summary>
        /// Tests ForeColor of the various Font fills.
        /// </summary>
        [TestCase("TextSolid", 0xFF, 0x00, 0x00)]
        [TestCase("TextOneColorGradient", 0x00, 0x80, 0x80)]
        [TestCase("TextTwoColorGradient", 0x80, 0x00, 0x00)]
        [TestCase("TextPresetGradient", 0xCC, 0xCC, 0xFF)]
        public void TestFontFillForeColor(string testName, int r, int g, int b)
        {
            Color expectedOriginalColor = Color.FromArgb(r, g, b);

            // DML
            TestFillForeColor(testName, ShapeMarkupLanguage.Dml, expectedOriginalColor, Color.Black);
            TestFillForeColor(testName, ShapeMarkupLanguage.Dml, expectedOriginalColor, Color.White);
            TestFillForeColor(testName, ShapeMarkupLanguage.Dml, expectedOriginalColor, Color.Peru);

            // FOSS There is no fill for font in VML.
        }

        /// <summary>
        /// Tests ForeColor of the various Shape fills.
        /// </summary>
        [TestCase("Solid", 0xFF, 0x00, 0x00)]
        [TestCase("OneColorGradient", 0x00, 0x80, 0x80)]
        [TestCase("TwoColorGradient", 0x80, 0x00, 0x00)]
        [TestCase("PresetGradient", 0xCC, 0xCC, 0xFF)]
        [TestCase("Patterned", 0x80, 0x00, 0x00)]
        public void TestShapeFillForeColorRGB(string testName, int r, int g, int b)
        {
            Color expectedOriginalColor = Color.FromArgb(r, g, b);

            // DML
            TestFillForeColor(testName, ShapeMarkupLanguage.Dml, expectedOriginalColor, Color.Black);
            TestFillForeColor(testName, ShapeMarkupLanguage.Dml, expectedOriginalColor, Color.White);
            TestFillForeColor(testName, ShapeMarkupLanguage.Dml, expectedOriginalColor, Color.Peru);

            // VML
            TestFillForeColor(testName, ShapeMarkupLanguage.Vml, expectedOriginalColor, Color.Black);
            TestFillForeColor(testName, ShapeMarkupLanguage.Vml, expectedOriginalColor, Color.White);
            TestFillForeColor(testName, ShapeMarkupLanguage.Vml, expectedOriginalColor, Color.Peru);
        }

        /// <summary>
        /// Tests ForeColor of the various Shape fills.
        /// </summary>
        /// <remarks>
        /// Setting ForeColor in Word VBA for these fills for DML shapes does not actually change the color (and also
        /// does not throw). But for VML it does.
        /// </remarks>
        [TestCase("PresetTextured")]
        [TestCase("UserPicture")]
        [TestCase("UserTextured")]
        public void TestShapeFillForeColor(string testName)
        {
            // DML
            TestFillForeColor(testName, ShapeMarkupLanguage.Dml, Color.Empty, Color.Peru, Color.Empty);

            // VML
            TestFillForeColor(testName, ShapeMarkupLanguage.Vml, Color.White, Color.Black);
            TestFillForeColor(testName, ShapeMarkupLanguage.Vml, Color.White, Color.White);
            TestFillForeColor(testName, ShapeMarkupLanguage.Vml, Color.White, Color.Peru);
        }

        /// <summary>
        /// Tests ForeColor of the 'NoFill' for Font fill.
        /// </summary>
        [Test]
        public void TestFontNoFill()
        {
            // DML
            Document doc = TestFillForeColor("TextNoFill", ShapeMarkupLanguage.Dml, Color.Empty, Color.Peru);
            Assert.That(GetFill(doc).FillType, Is.EqualTo(FillType.Solid));

            // FOSS There is no fill for font in VML.
            // VML
        }

        /// <summary>
        /// Tests ForeColor of the 'NoFill' for Shape fill.
        /// </summary>
        [Test]
        public void TestShapeNoFill()
        {
            // DML
            Document doc = TestFillForeColor("NoFill", ShapeMarkupLanguage.Dml, Color.Empty, Color.Peru);
            Assert.That(GetFill(doc).FillType, Is.EqualTo(FillType.Solid));

            // VML
            doc = TestFillForeColor("NoFill", ShapeMarkupLanguage.Vml, Color.White, Color.Peru);
            Assert.That(GetFill(doc).FillType, Is.EqualTo(FillType.Solid));
        }

        /// <summary>
        /// Tests ForeColor of the first fillable object in a document at a specified path.
        /// </summary>
        private static Document TestFillForeColor(string testFileNameWithoutExtension, ShapeMarkupLanguage markupLanguage,
            Color expectedOriginalValue, Color desiredValue)
        {
            return TestFillForeColor(testFileNameWithoutExtension, markupLanguage,
                expectedOriginalValue, desiredValue, desiredValue);
        }

        /// <summary>
        /// Tests ForeColor of the first fillable object in a document at a specified path.
        /// </summary>
        private static Document TestFillForeColor(string testFileNameWithoutExtension, ShapeMarkupLanguage markupLanguage,
            Color expectedOriginalValue, Color desiredValue, Color actualValue)
        {
            Document doc = Open(testFileNameWithoutExtension, markupLanguage);
            return TestFillForeColor(doc, testFileNameWithoutExtension, markupLanguage,
                expectedOriginalValue, desiredValue, actualValue);
        }

        /// <summary>
        /// Tests ForeColor of the first fillable object in a specified document.
        /// </summary>
        private static Document TestFillForeColor(Document doc, string testFileNameWithoutExtension,
            ShapeMarkupLanguage markupLanguage, Color expectedOriginalValue, Color desiredValue, Color actualValue)
        {
            Fill fill = GetFill(doc);
            CheckColor(expectedOriginalValue, fill.ForeColor);

            // Set desired value.
            fill.ForeColor = desiredValue;
            CheckColor(actualValue, fill.ForeColor);

            string ext = MarkupLanguageToExtension(markupLanguage);
            string outFileName = string.Format("FillFormat\\{0}_ForeColor_{1}{2}",
                testFileNameWithoutExtension, desiredValue.ToArgb().ToString("X", CultureInfo.InvariantCulture), ext);

            // Roundtrip the document.
            doc = TestUtil.SaveOpen(doc, outFileName, CreateSaveOptions(markupLanguage), false);

            Node node = GetFirstFillable(doc);
            fill = (node.NodeType == NodeType.Run) ? ((Run)node).Font.Fill : ((Shape)node).Fill;

            // There is no fill for font in VML. So after export to VML-based format (like DOC),
            // the color is reset to its original value (Empty).
            Color expectedColor = ((markupLanguage == ShapeMarkupLanguage.Vml) && (node.NodeType == NodeType.Run))
                ? expectedOriginalValue
                : actualValue;
            CheckColor(expectedColor, fill.ForeColor);

            return doc;
        }
    }
}
