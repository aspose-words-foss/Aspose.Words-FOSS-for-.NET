// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/02/2023 by Vadim Saltykov

using System;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Themes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing ForeTintAndShade and BackTintAndShade of Fill object.
    /// </summary>
    [TestFixture]
    public class TestFillTintAndShade : TestFillFormatBase
    {
        /// <summary>
        /// Tests getting and setting TintAndShade for both the theme and RGB colors.
        /// Dml case.
        /// </summary>
        [Test]
        public void TestTintAndShadeDml()
        {
            Document doc = Open("TintAndShade", ShapeMarkupLanguage.Dml);

            Shape shapeSimple = doc.FirstSection.Body.Shapes[0];
            Shape shapeTheme = doc.FirstSection.Body.Shapes[1];

            Assert.That(shapeSimple.Fill.ForeTintAndShade, Is.EqualTo(0.6).Within(0.01));
            Assert.That(shapeSimple.Fill.BackTintAndShade, Is.EqualTo(0.6).Within(0.01));
            Assert.That(shapeTheme.Fill.ForeTintAndShade, Is.EqualTo(0.6).Within(0.01));
            Assert.That(shapeTheme.Fill.BackTintAndShade, Is.EqualTo(0.6).Within(0.01));

            CheckTintAndShadeDml(doc, 0.8);
            CheckTintAndShadeDml(doc, -0.6);
            CheckTintAndShadeDml(doc, 1);
            CheckTintAndShadeDml(doc, -1);
            CheckTintAndShadeDml(doc, 0);
        }

        /// <summary>
        /// Tests getting and setting TintAndShade for different fill types.
        /// </summary>
        [TestCase("OneColorGradient", ShapeMarkupLanguage.Vml)]
        [TestCase("TwoColorGradient", ShapeMarkupLanguage.Vml)]
        [TestCase("Patterned", ShapeMarkupLanguage.Vml)]
        [TestCase("Solid", ShapeMarkupLanguage.Vml)]
        [TestCase("TextSolid", ShapeMarkupLanguage.Vml)]
        [TestCase("OneColorGradient", ShapeMarkupLanguage.Dml)]
        [TestCase("TwoColorGradient", ShapeMarkupLanguage.Dml)]
        [TestCase("Patterned", ShapeMarkupLanguage.Dml)]
        [TestCase("Solid", ShapeMarkupLanguage.Dml)]
        [TestCase("TextSolid", ShapeMarkupLanguage.Dml)]
        public void TestForeTintAndShadeFillTypes(string fileName, ShapeMarkupLanguage markupLanguage)
        {
            Document doc = Open(fileName, markupLanguage);

            Fill fill = (fileName.Contains("Text"))
                ? doc.FirstSection.Body.FirstParagraph.FirstRun.Font.Fill
                : doc.FirstSection.Body.Shapes[0].Fill;

            if (fileName.Contains("Text"))
                fill.ForeThemeColor = ThemeColor.Accent1;

            Assert.That(fill.ForeTintAndShade, Is.EqualTo(0));
            // Checks Tint.
            fill.ForeTintAndShade = 0.6;
            Assert.That(fill.ForeTintAndShade, Is.EqualTo(0.6));
            // Checks Shade.
            fill.ForeTintAndShade = -0.6;
            Assert.That(fill.ForeTintAndShade, Is.EqualTo(-0.6));
        }

        /// <summary>
        /// Tests that exception is thrown when applying value to TintAndShade out of [-1, 1] range.
        /// </summary>
        [TestCase(ShapeMarkupLanguage.Dml, -1.5)]
        [TestCase(ShapeMarkupLanguage.Dml, 1.5)]
        [TestCase(ShapeMarkupLanguage.Vml, -1.5)]
        [TestCase(ShapeMarkupLanguage.Vml, 1.5)]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestForeTintAndShadeOutOfRange(ShapeMarkupLanguage shapeMarkupLanguage, double tintAndShade)
        {
            Document doc = Open("ThemeColors", shapeMarkupLanguage);

            Shape shape = doc.FirstSection.Body.Shapes[0];
            // Expect ArgumentOutOfRangeException here.
            shape.Fill.ForeTintAndShade = tintAndShade;
        }

        /// <summary>
        /// Tests throwing an exception when trying to set ForeTintAndShade value for certain Fill types.
        /// </summary>
        [TestCase("NoFill", ShapeMarkupLanguage.Dml)]
        [TestCase("NoFill", ShapeMarkupLanguage.Vml)]
        [TestCase("UserPicture", ShapeMarkupLanguage.Dml)]
        [TestCase("UserPicture", ShapeMarkupLanguage.Vml)]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Cannot apply ForeTintAndShade to this Fill.")]
        public void TestForeTintAndShadeUnsupportedFillTypes(string fileName, ShapeMarkupLanguage shapeMarkupLanguage)
        {
            Document doc = Open(fileName, shapeMarkupLanguage);

            Shape shape = doc.FirstSection.Body.Shapes[0];
            Assert.That(shape.Fill.ForeTintAndShade, Is.EqualTo(0));
            // Expect InvalidOperationException here.
            shape.Fill.ForeTintAndShade = 0.5;
        }

        /// <summary>
        /// Tests throwing an exception when trying to set BackTintAndShade value for certain Fill types.
        /// </summary>
        [TestCase("NoFill", ShapeMarkupLanguage.Dml)]
        [TestCase("NoFill", ShapeMarkupLanguage.Vml)]
        [TestCase("UserPicture", ShapeMarkupLanguage.Dml)]
        [TestCase("UserPicture", ShapeMarkupLanguage.Vml)]
        [TestCase("TextSolid", ShapeMarkupLanguage.Dml)]
        [TestCase("TextSolid", ShapeMarkupLanguage.Vml)]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Cannot apply BackTintAndShade to this Fill.")]
        public void TestBackTintAndShadeUnsupportedFillTypes(string fileName, ShapeMarkupLanguage shapeMarkupLanguage)
        {
            Document doc = Open(fileName, shapeMarkupLanguage);

            Fill fill = (fileName.Contains("Text"))
                ? doc.FirstSection.Body.FirstParagraph.FirstRun.Font.Fill
                : doc.FirstSection.Body.Shapes[0].Fill;

            Assert.That(fill.BackTintAndShade, Is.EqualTo(0.0d));

            // Expect InvalidOperationException here.
            fill.BackTintAndShade = 0.5;
        }

        private void CheckTintAndShadeVml(Document doc, string fileName, UnifiedScenario scenario,
            double expTintAndShade)
        {
            Fill fill = doc.FirstSection.Body.Shapes[0].Fill;

            fill.ForeTintAndShade = expTintAndShade;
            fill.BackTintAndShade = expTintAndShade;

            doc = TestUtil.SaveOpen(doc, fileName, scenario);

            // MS Word does not write zero modifiers (TintAndShade == 1) to the output Wml file.
            if (scenario != UnifiedScenario.Wml2WmlNoGold || !expTintAndShade.Equals(1))
            {
                fill = doc.FirstSection.Body.Shapes[0].Fill;

                Assert.That(fill.ForeTintAndShade, Is.EqualTo(expTintAndShade).Within(0.01));
                Assert.That(fill.BackTintAndShade, Is.EqualTo(expTintAndShade).Within(0.01));
            }
        }

        private void CheckTintAndShadeDml(Document doc, double expTintAndShade)
        {
            Shape shapeSimple = doc.FirstSection.Body.Shapes[0];
            Shape shapeTheme = doc.FirstSection.Body.Shapes[1];

            shapeSimple.Fill.ForeTintAndShade = expTintAndShade;
            shapeSimple.Fill.BackTintAndShade = expTintAndShade;
            shapeTheme.Fill.ForeTintAndShade = expTintAndShade;
            shapeTheme.Fill.BackTintAndShade = expTintAndShade;

            doc = TestUtil.SaveOpen(doc, @"Model\FillFormat\TintAndShade", UnifiedScenario.Docx2DocxNoGold);
            shapeSimple = doc.FirstSection.Body.Shapes[0];
            shapeTheme = doc.FirstSection.Body.Shapes[1];

            Assert.That(shapeSimple.Fill.ForeTintAndShade, Is.EqualTo(expTintAndShade).Within(0.01));
            Assert.That(shapeSimple.Fill.BackTintAndShade, Is.EqualTo(expTintAndShade).Within(0.01));
            Assert.That(shapeTheme.Fill.ForeTintAndShade, Is.EqualTo(expTintAndShade).Within(0.01));
            Assert.That(shapeTheme.Fill.BackTintAndShade, Is.EqualTo(expTintAndShade).Within(0.01));

            // Checks whether DmlShade and DmlTint modifiers are removed.
            if (expTintAndShade == 0)
            {
                Assert.That(((DmlFill)shapeSimple.FillCore).DmlColorInternal.ColorModifiers.Count, Is.EqualTo(0));
                Assert.That(((DmlFill)shapeTheme.FillCore).DmlColorInternal.ColorModifiers.Count, Is.EqualTo(0));
            }
        }
    }
}
