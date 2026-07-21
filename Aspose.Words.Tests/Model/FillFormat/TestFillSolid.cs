// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/03/2021 by Ilya Navrotskiy

using System.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml.Fills;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing Solid() method of Fill object.
    /// </summary>
    [TestFixture]
    public class TestFillSolid : TestFillFormatBase
    {
        /// <summary>
        /// Tests setting Solid for various Shape fills (DML and VML).
        /// </summary>
        [TestCase("OneColorGradient", 0x00, 0x80, 0x80)]
        [TestCase("TwoColorGradient", 0x80, 0x00, 0x00)]
        [TestCase("PresetGradient", 0xCC, 0xCC, 0xFF)]
        [TestCase("Patterned", 0x80, 0x00, 0x00)]
        [TestCase("PresetTextured", 0xFF, 0xFF, 0xFF)]
        [TestCase("UserPicture", 0xFF, 0xFF, 0xFF)]
        [TestCase("UserTextured", 0xFF, 0xFF, 0xFF)]
        public void TestShape(string testName, int r, int g, int b)
        {
            Color expectedColor = Color.FromArgb(r, g, b);

            // DML
            // Word VBA changes transparency in DML to fully opaque color.
            double expectedTransparency = 0.0;
            CheckSetSolid(testName, ShapeMarkupLanguage.Dml, expectedColor, expectedTransparency);
            // VML
            // Word VBA changes only type of the fill in VML (i.e. preserves original transparency and color)
            expectedTransparency = 0.37;
            CheckSetSolid(testName, ShapeMarkupLanguage.Vml, expectedColor, expectedTransparency);
        }

        /// <summary>
        /// Tests setting Solid for a shape that has Solid fill already.
        /// </summary>
        [TestCase("Solid")]
        public void TestShapeSolid(string testName)
        {
            // Word VBA does not change anything either in DML or VML when the fill has Solid fill already.
            Color expectedColor = Color.FromArgb(0xFF, 0x00, 0x00);
            const double expectedTransparency = 0.37;
            // DML
            CheckSetSolid(testName, ShapeMarkupLanguage.Dml, expectedColor, expectedTransparency);
            // VML
            CheckSetSolid(testName, ShapeMarkupLanguage.Vml, expectedColor, expectedTransparency);
        }

        /// <summary>
        /// Tests setting Solid for a DML shape that has NoFill fill.
        /// </summary>
        [TestCase("NoFill")]
        public void TestDmlShapeNoFill(string testName)
        {
            Document doc = Open(testName, ShapeMarkupLanguage.Dml);
            Shape shape = (Shape)GetFirstFillable(doc);
            Fill fill = shape.Fill;

            fill.Solid();

            // Word VBA does not change fill type for NoFill in DML
            Assert.That(((DmlFill)shape.FillCore).DmlFillType, Is.EqualTo(DmlFillType.NoFill));
        }

        /// <summary>
        /// Tests setting Solid for a VML shape that has NoFill fill.
        /// </summary>
        [TestCase("NoFill")]
        public void TestVmlShapeNoFill(string testName)
        {
            // Word VBA sets in VML fully opaque white color.
            CheckSetSolid(testName, ShapeMarkupLanguage.Vml, Color.White, 0.0);
        }

        /// <summary>
        /// Tests setting Solid for a fill of DML Font object.
        /// </summary>
        [TestCase("TextOneColorGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextPresetGradient")]
        [TestCase("TextNoFill")]
        public void TestDmlFont(string testName)
        {
            // Word VBA for DML Font resets fill to fully opaque black color.
            CheckSetSolid(testName, ShapeMarkupLanguage.Dml, Color.Black, 0.0);
        }

        /// <summary>
        /// Tests setting Solid for a fill of DML Font object that was Solid originally.
        /// </summary>
        [TestCase("TextSolid")]
        public void TestDmlFontSolid(string testName)
        {
            // Word VBA does not change anything if the fill was Solid originally.
            CheckSetSolid(testName, ShapeMarkupLanguage.Dml, Color.Red, 0.39);
        }

        /// <summary>
        /// Tests setting Solid for a fill of VML (document opened in compatibility mode) Font object.
        /// </summary>
        [TestCase("TextOneColorGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextPresetGradient")]
        [TestCase("TextNoFill")]
        [TestCase("TextSolid")]
        public void TestVmlFont(string testName)
        {
            // Word VBA throws when call to Solid() for fill of VML Font object.
            // But we agreed to change the fill to DmlSolidFill implicitly.
            CheckSetSolid(testName, ShapeMarkupLanguage.Vml, Color.Black, 0.0);
        }

        /// <summary>
        /// Tests setting Solid with a specified desired color to a Shape.
        /// </summary>
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("Patterned")]
        [TestCase("PresetTextured")]
        [TestCase("UserPicture")]
        [TestCase("UserTextured")]
        public void TestShapeSolidWithColor(string testName)
        {
            // DML
            CheckSetSolid(testName, ShapeMarkupLanguage.Dml, Color.Aqua, Color.Aqua, 0.0);
            // VML
            CheckSetSolid(testName, ShapeMarkupLanguage.Vml, Color.Aqua, Color.Aqua, 0.37);
        }

        /// <summary>
        /// Tests setting Solid with a specified desired color to a Shape with Solid already.
        /// </summary>
        [TestCase("Solid")]
        public void TestSolidShapeSetSolidWithColor(string testName)
        {
            // DML
            // Note, Word VBA does not reset transparency to fully opaque, but we do it for a while in this case.
            CheckSetSolid(testName, ShapeMarkupLanguage.Dml, Color.Aqua, Color.Aqua, 0.0);
            // VML
            CheckSetSolid(testName, ShapeMarkupLanguage.Vml, Color.Aqua, Color.Aqua, 0.37);
        }

        /// <summary>
        /// Tests setting Solid with a specified desired color to a Shape that has a NoFill fill.
        /// </summary>
        [TestCase("NoFill")]
        public void TestSolidWithColorVml(string testName)
        {
            // DML
            CheckSetSolid(testName, ShapeMarkupLanguage.Dml, Color.Aqua, Color.Aqua, 0.0);
            // VML
            CheckSetSolid(testName, ShapeMarkupLanguage.Vml, Color.Aqua, Color.Aqua, 0.0);
        }

        /// <summary>
        /// Tests setting Solid with a specified desired color to a Font.
        /// </summary>
        [TestCase("TextOneColorGradient")]
        [TestCase("TextTwoColorGradient")]
        [TestCase("TextPresetGradient")]
        [TestCase("TextSolid")]
        [TestCase("TextNoFill")]
        public void TestFontSolidWithColor(string testName)
        {
            // DML
            CheckSetSolid(testName, ShapeMarkupLanguage.Dml, Color.Aqua, Color.Aqua, 0.0);
            // VML
            CheckSetSolid(testName, ShapeMarkupLanguage.Vml, Color.Aqua, Color.Aqua, 0.0);
        }

        /// <summary>
        /// Checks setting Solid fill for a test file with a specified name.
        /// </summary>
        private static void CheckSetSolid(string testName,
            ShapeMarkupLanguage markupLanguage, Color expectedColor, double expectedTransparency)
        {
            Document doc = Open(testName, markupLanguage);
            Fill fill = GetFill(doc);

            fill.Solid();

            CheckSolidFill(fill, expectedColor, expectedTransparency);
        }

        /// <summary>
        /// Checks setting Solid fill for a test file with a specified name.
        /// </summary>
        private static void CheckSetSolid(string testName, ShapeMarkupLanguage markupLanguage,
            Color desiredColor, Color expectedColor, double expectedTransparency)
        {
            Document doc = Open(testName, markupLanguage);
            Fill fill = GetFill(doc);

            fill.Solid(desiredColor);

            CheckSolidFill(fill, expectedColor, expectedTransparency);
        }

        /// <summary>
        /// Checks a specified Solid fill.
        /// </summary>
        private static void CheckSolidFill(Fill solidFill, Color expectedColor, double expectedTransparency)
        {
            Assert.That(solidFill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(solidFill.Transparency, Is.EqualTo(expectedTransparency).Within(0.01));
            CheckColor(expectedColor, solidFill.ForeColor);
        }
    }
}
