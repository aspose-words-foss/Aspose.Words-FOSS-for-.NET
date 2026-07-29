// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/11/2023 by Vadim Saltykov

using System;
using System.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Themes;
using NUnit.Framework;


namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing various properties of the Stroke (VBA LineFormat) object.
    /// </summary>
    [TestFixture]
    public class TestStrokeFormat : TestFillFormatBase
    {
        /// <summary>
        /// Tests getting and setting theme colors for different fill types.
        /// Dml case.
        /// </summary>
        [TestCase("SolidOutline", false)]
        [TestCase("PatternedOutline", true)]
        [TestCase("Gradient1Outline", true)]
        [TestCase("Gradient2Outline", true)]
        public void TestStrokeThemeColorDml(string fileName, bool backColorSupported)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Dml);
            Stroke stroke = doc.FirstSection.Body.Shapes[0].Stroke;
            Assert.That(stroke.ForeThemeColor, Is.EqualTo(ThemeColor.None));
            Assert.That(stroke.BackThemeColor, Is.EqualTo(ThemeColor.None));
            stroke.ForeThemeColor = ThemeColor.Accent3;
            stroke.BackThemeColor = ThemeColor.Accent2;

            doc = TestUtil.SaveOpen(doc, @"Model\FillFormat\TestStrokeThemeColorDml", UnifiedScenario.Docx2DocxNoGold);
            stroke = doc.FirstSection.Body.Shapes[0].Stroke;
            Assert.That(stroke.ForeThemeColor, Is.EqualTo(ThemeColor.Accent3));
            if (backColorSupported)
                Assert.That(stroke.BackThemeColor, Is.EqualTo(ThemeColor.Accent2));
            stroke.ForeThemeColor = ThemeColor.None;
            stroke.BackThemeColor = ThemeColor.None;

            doc = TestUtil.SaveOpen(doc, @"Model\FillFormat\TestStrokeThemeColorDml", UnifiedScenario.Docx2DocxNoGold);
            stroke = doc.FirstSection.Body.Shapes[0].Stroke;
            Assert.That(stroke.ForeThemeColor, Is.EqualTo(ThemeColor.None));
            Assert.That(stroke.ForeColor, Is.EqualTo(Color.FromArgb(255, 165, 165, 165)));
            Assert.That(stroke.BackThemeColor, Is.EqualTo(ThemeColor.None));
            if (backColorSupported)
                Assert.That(stroke.BackColor, Is.EqualTo(Color.FromArgb(255, 237, 125, 49)));
        }

        /// <summary>
        /// Tests getting and setting Tint and Shade for different fill types.
        /// Dml case.
        /// </summary>
        [TestCase("SolidOutline", false)]
        [TestCase("PatternedOutline", true)]
        [TestCase("Gradient1Outline", true)]
        [TestCase("Gradient2Outline", true)]
        public void TestStrokeTintAndShadeDml(string fileName, bool backModifierSupported)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Dml);
            Stroke stroke = doc.FirstSection.Body.Shapes[0].Stroke;
            Assert.That(stroke.ForeTintAndShade, Is.EqualTo(0));
            Assert.That(stroke.BackTintAndShade, Is.EqualTo(0));
            stroke.ForeTintAndShade = 0.6;
            stroke.BackTintAndShade = 0.3;

            doc = TestUtil.SaveOpen(doc, @"Model\FillFormat\TestStrokeTintAndShadeDml", UnifiedScenario.Docx2DocxNoGold);
            stroke = doc.FirstSection.Body.Shapes[0].Stroke;
            Assert.That(stroke.ForeTintAndShade, Is.EqualTo(0.6).Within(0.01));
            if (backModifierSupported)
                Assert.That(stroke.BackTintAndShade, Is.EqualTo(0.3).Within(0.01));
            stroke.ForeTintAndShade = -0.3;
            stroke.BackTintAndShade = -0.6;

            doc = TestUtil.SaveOpen(doc, @"Model\FillFormat\TestStrokeTintAndShadeDml", UnifiedScenario.Docx2DocxNoGold);
            stroke = doc.FirstSection.Body.Shapes[0].Stroke;
            Assert.That(stroke.ForeTintAndShade, Is.EqualTo(-0.3).Within(0.01));
            if (backModifierSupported)
                Assert.That(stroke.BackTintAndShade, Is.EqualTo(-0.6).Within(0.01));
            stroke.ForeTintAndShade = 0;
            stroke.BackTintAndShade = 0;

            doc = TestUtil.SaveOpen(doc, @"Model\FillFormat\TestStrokeTintAndShadeDml", UnifiedScenario.Docx2DocxNoGold);
            stroke = doc.FirstSection.Body.Shapes[0].Stroke;
            Assert.That(stroke.ForeTintAndShade, Is.EqualTo(0));
            Assert.That(stroke.BackTintAndShade, Is.EqualTo(0));
        }

        /// <summary>
        /// Tests that exception is thrown when applying value to ForeTintAndShade out of [-1, 1] range.
        /// </summary>
        [TestCase(ShapeMarkupLanguage.Dml, -1.5)]
        [TestCase(ShapeMarkupLanguage.Dml, 1.5)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestForeTintAndShadeOutOfRange(ShapeMarkupLanguage shapeMarkupLanguage, double tintAndShade)
        {
            Document doc = Open("TintShadeOutline", shapeMarkupLanguage);

            Shape shape = doc.FirstSection.Body.Shapes[0];
            // Expect ArgumentOutOfRangeException here.
            shape.Stroke.ForeTintAndShade = tintAndShade;
        }

        /// <summary>
        /// Tests that exception is thrown when applying value to BackTintAndShade out of [-1, 1] range.
        /// </summary>
        [TestCase(ShapeMarkupLanguage.Dml, -1.5)]
        [TestCase(ShapeMarkupLanguage.Dml, 1.5)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestBackTintAndShadeOutOfRange(ShapeMarkupLanguage shapeMarkupLanguage, double tintAndShade)
        {
            Document doc = Open("TintShadeOutline", shapeMarkupLanguage);

            Shape shape = doc.FirstSection.Body.Shapes[0];
            // Expect ArgumentOutOfRangeException here.
            shape.Stroke.BackTintAndShade = tintAndShade;
        }
    }
}
