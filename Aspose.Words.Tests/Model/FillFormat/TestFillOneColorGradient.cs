// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/09/2021 by Ilya Navrotskiy

using System;
using System.Drawing;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing OneColorGradient() method of Fill object.
    /// </summary>
    [TestFixture]
    public class TestFillOneColorGradient : TestFillFormatBase
    {
        /// <summary>
        /// Tests all one-color gradients in text from scratch without specifying colors.
        /// </summary>
        /// <remarks>
        /// Word does not allow to work with gradient stops for Font object in VBA and throws an exception.
        /// However, user can change colors in GUI. In AW setting gradient fill mimics Word behavior in VBA for shapes by
        /// changing gradient stops forcibly to the same colors as in shapes with corresponding gradient style and variant.
        /// </remarks>
        [TestCase(ShapeMarkupLanguage.Dml)]
        [TestCase(ShapeMarkupLanguage.Vml)]
        public void TestTextFromScratch(ShapeMarkupLanguage markupLanguage)
        {
            Document doc = CreateDocumentWithAllGradientsText(markupLanguage);

            string outFileName = string.Format("Model\\FillFormat\\OneColorGradient_{0}_Text.docx", markupLanguage.ToString());
            TestUtil.SaveCheckGold(doc, outFileName);
        }

        /// <summary>
        /// Tests all one-color gradients in text from scratch with specifying colors.
        /// </summary>
        [TestCase(ShapeMarkupLanguage.Dml)]
        [TestCase(ShapeMarkupLanguage.Vml)]
        public void TestTextFromScratchWithColors(ShapeMarkupLanguage markupLanguage)
        {
            Document doc = CreateDocumentWithAllGradientsText(markupLanguage, Color.Chocolate);

            string outFileName = string.Format("Model\\FillFormat\\OneColorGradient_{0}_Text_WithColors.docx", markupLanguage.ToString());
            TestUtil.SaveCheckGold(doc, outFileName);
        }

        /// <summary>
        /// Tests setting OneColorGradient for various DML Shape fills.
        /// </summary>
        [TestCase("OneColorGradient", 0x00, 0x80, 0x80)]
        [TestCase("TwoColorGradient", 0x80, 0x00, 0x00)]
        [TestCase("PresetGradient", 0xCC, 0xCC, 0xFF)]
        [TestCase("Patterned", 0x80, 0x00, 0x00)]
        /*-----------------------------------------------------------------*/
        // Actually, all BlipFills in VBA returns Color.White, but we returns Color.Empty and then convert it to Accent1.
        // We probable should override DmlBlipFill.DmlColorInternal to return DrColor.White to mimic Word VBA.
        [TestCase("PresetTextured", 0x44, 0x72, 0xC4)]
        [TestCase("UserPicture", 0x44, 0x72, 0xC4)]
        [TestCase("UserTextured", 0x44, 0x72, 0xC4)]
        /*-----------------------------------------------------------------*/
        [TestCase("Solid", 0xFF, 0x00, 0x00)]
        [TestCase("NoFill", 0x44, 0x72, 0xC4)]
        public void TestShapeFillsDml(string testName, int r, int g, int b)
        {
            Document doc = Open(testName, ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);

            Color expectedColor = Color.FromArgb(r, g, b);

            // Set some one-color gradient.
            const GradientStyle style = GradientStyle.Horizontal;
            const GradientVariant variant = GradientVariant.Variant3;
            const double degree = 0.87;
            fill.OneColorGradient(style, variant, degree);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.GradientStyle, Is.EqualTo(style));

            // This is One-color gradient, so ForeColor and BackColor should be the same.
            CheckColor(expectedColor, fill.ForeColor);
            CheckColor(expectedColor, fill.BackColor);
        }

        /// <summary>
        /// Tests setting OneColorGradient for various VML Shape fills.
        /// </summary>
        [TestCase("OneColorGradient", 0x00, 0x80, 0x80)]
        [TestCase("TwoColorGradient", 0x80, 0x00, 0x00)]
        [TestCase("PresetGradient", 0xCC, 0xCC, 0xFF)]
        [TestCase("Patterned", 0x80, 0x00, 0x00)]
        [TestCase("PresetTextured", 0xFF, 0xFF, 0xFF)]
        [TestCase("UserPicture", 0xFF, 0xFF, 0xFF)]
        [TestCase("UserTextured", 0xFF, 0xFF, 0xFF)]
        [TestCase("Solid", 0xFF, 0x00, 0x00)]
        [TestCase("NoFill", 0xFF, 0xFF, 0xFF)]
        public void TestShapeFillsVml(string testName, int r, int g, int b)
        {
            Document doc = Open(testName, ShapeMarkupLanguage.Vml);
            Fill fill = GetFill(doc);

            Color expectedColor = Color.FromArgb(r, g, b);

            // Set some One-color gradient.
            const GradientStyle style = GradientStyle.Horizontal;
            const GradientVariant variant = GradientVariant.Variant3;
            const double degree = 0.17;
            fill.OneColorGradient(style, variant, degree);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.GradientStyle, Is.EqualTo(style));
            Assert.That(fill.GradientVariant, Is.EqualTo(variant));

            // Word sets BackColor as lighten/darken percentage of ForeColor. We cannot set it the same in our model,
            // so we forced to calculate and set actual BackColor and technically it becomes a two-color gradient.
            // Therefore, despite visually fills look identical in AW and Word, we return other values for BackColor
            // than Word VBA. This should be investigated additionally, so let's check only ForeColor for a while.
            CheckColor(expectedColor, fill.ForeColor);
        }

        /// <summary>
        /// Tests setting OneColorGradient for various Font fills (DML).
        /// </summary>
        [TestCase("TextOneColorGradient", 0x00, 0x80, 0x80, /**/ 0x00, 0x80, 0x80)]
        [TestCase("TextTwoColorGradient", 0x80, 0x00, 0x00, /**/ 0x80, 0x00, 0x00)]
        [TestCase("TextPresetGradient", 0xCC, 0xCC, 0xFF, /**/ 0xCC, 0xCC, 0xFF)]
        [TestCase("TextSolid", 0xFF, 0x00, 0x00, /**/ 0xFF, 0x00, 0x00)]
        [TestCase("TextNoFill", 0x00, 0x00, 0x00, /**/ 0x00, 0x00, 0x00)]
        public void TestFontFillsDml(string testName,
            int foreR, int foreG, int foreB,
            int backR, int backG, int backB)
        {
            Document doc = Open(testName, ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);

            Color expectedForeColor = Color.FromArgb(foreR, foreG, foreB);
            Color expectedBackColor = Color.FromArgb(backR, backG, backB);

            // Set some one-color gradient.
            const GradientStyle style = GradientStyle.Horizontal;
            const GradientVariant variant = GradientVariant.Variant3;
            const double degree = 0.17;
            fill.OneColorGradient(style, variant, degree);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.GradientStyle, Is.EqualTo(style));
            Assert.That(fill.GradientVariant, Is.EqualTo(variant));

            CheckColor(expectedForeColor, fill.ForeColor);
            CheckColor(expectedBackColor, fill.BackColor);
        }

        /// <summary>
        /// Tests <see cref="Fill.GradientAngle"/> in text.
        /// </summary>
        [Test]
        public void TestGradientAngleText()
        {
            Document doc = CreateDocumentWithAllGradientsText(ShapeMarkupLanguage.Dml);
            CheckGradientAngleText(doc);
        }

        /// <summary>
        /// Returns gradient variant expected for a specified gradient style and variant for Shapes.
        /// </summary>
        private static int GetExpectedVariant(GradientStyle style, int variant, ShapeMarkupLanguage markupLanguage)
        {
            if ((style != GradientStyle.FromCenter) || (markupLanguage == ShapeMarkupLanguage.Vml))
                return variant;

            switch (variant)
            {
                // Variants 1 and 2 of a 'FromCenter' gradient in DML differ only in that the Fore and Back colors
                // are swapped. Thus, a variant 2 is recognized as a variant 1.
                case 2:
                    return 1;
                // The same is with variants 3 and 4.
                case 4:
                    return 3;
                default:
                    return variant;
            }
        }

        /// <summary>
        /// Returns gradient variant expected for a specified gradient style and variant for Text.
        /// </summary>
        private static int GetExpectedVariantText(GradientStyle style, int variant)
        {
            if (style != GradientStyle.FromCenter)
                return variant;

            switch (variant)
            {
                // Variants 1 and 2 of a 'FromCenter' gradient in DML differ only in that the Fore and Back colors
                // are swapped. Thus, a variant 2 is recognized as a variant 1.
                case 2:
                    return 1;
                // The same is with variants 3 and 4.
                case 4:
                    return 3;
                default:
                    return variant;
            }
        }

        /// <summary>
        /// Creates document with all gradients texts with default colors.
        /// </summary>
        private static Document CreateDocumentWithAllGradientsText(ShapeMarkupLanguage markupLanguage)
        {
            return CreateDocumentWithAllGradientsText(markupLanguage, Color.Empty);
        }

        /// <summary>
        /// Creates document with all gradients texts with the specified colors.
        /// </summary>
        private static Document CreateDocumentWithAllGradientsText(ShapeMarkupLanguage markupLanguage, Color color)
        {
            DocumentBuilder builder = CreateDocument(markupLanguage);
            builder.Document.FirstSection.PageSetup.Orientation = Orientation.Landscape;
            builder.Document.FirstSection.PageSetup.LeftMargin = 10;
            builder.Document.FirstSection.PageSetup.RightMargin = 10;

            double degree = 0.0;

            builder.Font.Size = 24;
            GradientStyle[] styles = new GradientStyle[]
            {
                GradientStyle.Horizontal, GradientStyle.Vertical, GradientStyle.DiagonalDown, GradientStyle.DiagonalUp,
                GradientStyle.FromCenter, GradientStyle.FromCorner
            };
            foreach (GradientStyle style in styles)
            {
                for (int variant = 1; variant <= 4; variant++)
                {
                    if (color == Color.Empty)
                        builder.Font.Fill.OneColorGradient(style, (GradientVariant)variant, degree);
                    else
                        builder.Font.Fill.OneColorGradient(color, style, (GradientVariant)variant, degree);

                    builder.Write(string.Format("{0}_{1}\t", style.ToString(), variant));

                    int expectedVariant = GetExpectedVariantText(style, variant);
                    string msg = string.Format("Style: {0}; Variant: {1}", style.ToString(), variant);
                    Run run = (Run)builder.CurrentParagraph.LastChild;
                    Assert.That(run.Font.Fill.FillType, Is.EqualTo(FillType.Gradient), msg);
                    Assert.That(run.Font.Fill.GradientStyle, Is.EqualTo(style), msg);
                    Assert.That(run.Font.Fill.GradientVariant, Is.EqualTo((GradientVariant)expectedVariant), msg);

                    degree += 0.04;
                }

                builder.Writeln();
            }

            return builder.Document;
        }
    }
}
