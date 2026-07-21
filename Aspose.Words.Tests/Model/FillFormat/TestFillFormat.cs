// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/02/2021 by Ilya Navrotskiy

using System;
using System.Drawing;
using System.IO;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing various properties of the new Fill (VBA FillFormat) object requested by the customers.
    /// </summary>
    [TestFixture]
    public class TestFillFormat : TestFillFormatBase
    {
        /// <summary>
        /// WORDSNET-7788 Support Font.Fill property and provide public members in API.
        /// The new <see cref="Fill"/> (FillFormat) type and its related properties are exposed publicly.
        /// </summary>
        [Test]
        public void Test7788()
        {
            Document doc = TestUtil.Open(@"Model\FillFormat\Test7788.docx");
            RunCollection runs = doc.FirstSection.Body.FirstParagraph.Runs;

            // This is the customer's scenario to check the transparency of the font.
            Assert.That(runs[0].Font.Fill.Transparency, Is.EqualTo(0));
            Assert.That(runs[1].Font.Fill.Transparency, Is.EqualTo(1));
            Assert.That(runs[2].Font.Fill.Transparency, Is.EqualTo(0));

            // Check all related types and their properties are exposed publicly.
#if !JAVA && !CPLUSPLUS
            Type fill = typeof(Fill);
            Assert.That(fill.IsPublic, Is.True);
            Assert.That(fill.GetMember("ForeColor"), IsNot.Empty());
            Assert.That(fill.GetMember("BackColor"), IsNot.Empty());
            Assert.That(fill.GetMember("Visible"), IsNot.Empty());
            Assert.That(fill.GetMember("Transparency"), IsNot.Empty());
            Assert.That(fill.GetMember("RotateWithObject"), IsNot.Empty());
            Assert.That(fill.GetMember("FillType"), IsNot.Empty());

            Assert.That(fill.GetMember("Color"), IsNot.Empty());
            Assert.That(fill.GetMember("Opacity"), IsNot.Empty());
            Assert.That(fill.GetMember("ImageBytes"), IsNot.Empty());

            Type fillType = typeof(FillType);
            Assert.That(fillType.IsPublic, Is.True);

            Type font = typeof(Font);
            Assert.That(font.GetMember("Fill"), IsNot.Empty());
#endif
        }

        /// <summary>
        /// WORDSNET-18317 The gradient fill support for WordArt shapes.
        /// Fixed per related WORDSNET-22003
        /// </summary>
        [Test]
        public void Test18317()
        {
            Document doc = TestUtil.Open(@"Model\FillFormat\Test18317.docx");

            Shape shape = doc.FirstSection.Body.Shapes[0];
            Assert.That(shape.IsTextBox, Is.True);

            Fill fill = shape.FirstParagraph.FirstRun.Font.Fill;
            Assert.That(fill.ForeColor, Is.EqualTo(Color.FromArgb(0xFF, 0x20, 0x58, 0x67)));
            Assert.That(fill.BackColor, Is.EqualTo(Color.FromArgb(0xFF, 0x92, 0xCC, 0xDC)));
        }


        /// <summary>
        /// WORDSNET-21594 Provide API to Add Remove Gradient Color Fill for Text.
        /// Fixed per related WORDSNET-22003
        /// </summary>
        [Test]
        public void Test21594()
        {
            DocumentBuilder builder = new DocumentBuilder();

            // Add gradient fill.
            builder.Font.Fill.TwoColorGradient(Color.Red, Color.Blue, GradientStyle.Horizontal, GradientVariant.Variant2);
            builder.Write("Some text");

            // Roundtrip and check.
            OoxmlComplianceInfo.MarkAsHasDrawingExtensions(builder.Document);
            Document doc = TestUtil.SaveOpen(builder.Document, @"Model\FillFormat\Test21594", UnifiedScenario.Docx2DocxNoGold);
            Fill fill = doc.FirstSection.Body.FirstParagraph.FirstRun.Font.Fill;
            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.ForeColor, Is.EqualTo(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00)));
            Assert.That(fill.BackColor, Is.EqualTo(Color.FromArgb(0xFF, 0x00, 0x00, 0xFF)));

            // Remove fill.
            fill.Solid();
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(fill.ForeColor, Is.EqualTo(Color.FromArgb(0xFF, 0x00, 0x00, 0x00)));
            Assert.That(fill.BackColor, Is.EqualTo(Color.Empty));
        }

          /// <summary>
        /// WORDSNET-16733 Add feature to get the gradient fill properties of a shape.
        /// Fixed per related WORDSNET-7788, WORDSNET-22003, WORDSNET-22808
        /// </summary>
        [Test]
        public void Test16733()
        {
            Document doc = TestUtil.Open(@"Model\FillFormat\Test16733.docx");

            Fill fill = doc.FirstSection.Body.Shapes[0].Fill;
            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.GradientStyle, Is.EqualTo(GradientStyle.Horizontal));
            Assert.That(fill.GradientVariant, Is.EqualTo(GradientVariant.Variant1));
            Assert.That(fill.GradientAngle, Is.EqualTo(90.0).Within(0.001));
            Assert.That(fill.GradientStops.Count, Is.EqualTo(4));
            CheckGradientStop(fill.GradientStops[1], Color.FromArgb(0xA9, 0xBE, 0xE4), 0.74, 0.0);

            fill = TestUtil.GetRunWithText(doc, "...Text...").Font.Fill;
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(fill.ForeColor, Is.EqualTo(Color.Empty));
        }

        /// <summary>
        /// WORDSNET-18648 Text gradient support.
        /// Fixed per related WORDSNET-7788, WORDSNET-22003, WORDSNET-22808
        /// </summary>
        [Test]
        public void Test18648()
        {
            Document doc = TestUtil.Open(@"Model\FillFormat\Test18648.docx");

            Fill fill = TestUtil.GetRunWithText(doc, "...First example...").Font.Fill;
            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.GradientStyle, Is.EqualTo(GradientStyle.Horizontal));
            Assert.That(fill.GradientVariant, Is.EqualTo(GradientVariant.Variant1));
            Assert.That(fill.GradientAngle, Is.EqualTo(90.0).Within(0.001));
            Assert.That(fill.GradientStops.Count, Is.EqualTo(7));

            CheckGradientStop(fill.GradientStops[0], Color.FromArgb(0xA6, 0x03, 0xAB), 0.00);
            CheckGradientStop(fill.GradientStops[1], Color.FromArgb(0x08, 0x19, 0xFB), 0.21);
            CheckGradientStop(fill.GradientStops[2], Color.FromArgb(0x1A, 0x8D, 0x48), 0.35);
            CheckGradientStop(fill.GradientStops[3], Color.FromArgb(0xFF, 0xFF, 0x00), 0.52);
            CheckGradientStop(fill.GradientStops[4], Color.FromArgb(0xEE, 0x3F, 0x17), 0.73);
            CheckGradientStop(fill.GradientStops[5], Color.FromArgb(0xE8, 0x17, 0x66), 0.88);
            CheckGradientStop(fill.GradientStops[6], Color.FromArgb(0xA6, 0x03, 0xAB), 1.00);

            // Let's, for example, interchange gradient stops 1 and 5.
            // For this purpose, first remove the gradient stops.
            GradientStop gradientStopA = fill.GradientStops[1];
            GradientStop gradientStopB = fill.GradientStops[5];
            gradientStopA.Remove();
            gradientStopB.Remove();
            // And then insert removed gradient stops to the new indexes.
            fill.GradientStops.Insert(1, gradientStopB);
            fill.GradientStops.Insert(5, gradientStopA);

            // Now, let's apply new gradient stop to one of the indexes.
            fill.GradientStops[4] = new GradientStop(Color.Aqua, fill.GradientStops[4].Position, 0.25);

            // At last, let's move one of the gradient stops at the new position.
            fill.GradientStops[3].Position += 0.1;

            // Roundtrip and check stops.
            doc = TestUtil.SaveOpen(doc, @"Model\FillFormat\Test18648", UnifiedScenario.Docx2DocxNoGold);
            fill = TestUtil.GetRunWithText(doc, "...First example...").Font.Fill;
            CheckGradientStop(fill.GradientStops[0], Color.FromArgb(0xA6, 0x03, 0xAB), 0.00);
            CheckGradientStop(fill.GradientStops[1], Color.FromArgb(0xE8, 0x17, 0x66), 0.88);
            CheckGradientStop(fill.GradientStops[2], Color.FromArgb(0x1A, 0x8D, 0x48), 0.35);
            CheckGradientStop(fill.GradientStops[3], Color.FromArgb(0xFF, 0xFF, 0x00), 0.62);
            CheckGradientStop(fill.GradientStops[4], Color.Aqua, 0.73, 0.25);
            CheckGradientStop(fill.GradientStops[5], Color.FromArgb(0x08, 0x19, 0xFB), 0.21);
            CheckGradientStop(fill.GradientStops[6], Color.FromArgb(0xA6, 0x03, 0xAB), 1.00);
        }




        /// <summary>
        /// WORDSNET-18739 Gather data about gradient fills.
        /// Fixed per WORDSNET-7788, WORDSNET-22003, WORDSNET-22808
        /// Also added implementation of <see cref="IDmlCommonShapePrSource"/> to <see cref="DmlLockedCanvas"/>.
        /// </summary>
        [Test]
        public void Test18739()
        {
            Document doc = TestUtil.Open(@"Model\FillFormat\Test18739.docx");
            NodeCollection shapes = doc.GetChildNodes(NodeType.GroupShape, true);

            // Check first shape.
            Fill fill = ((GroupShape)shapes[0]).Fill;
            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.GradientStyle, Is.EqualTo(GradientStyle.DiagonalDown));
            Assert.That(fill.GradientVariant, Is.EqualTo(GradientVariant.Variant1));
            Assert.That(fill.GradientAngle, Is.EqualTo(135.0).Within(0.001));
            Assert.That(fill.GradientStops.Count, Is.EqualTo(4));
            CheckGradientStop(fill.GradientStops[0], Color.FromArgb(0xFF, 0x00, 0x00), 0.3);
            CheckGradientStop(fill.GradientStops[1], Color.FromArgb(0x1F, 0x49, 0x7D), 0.0);
            CheckGradientStop(fill.GradientStops[2], Color.FromArgb(0xFF, 0xFF, 0x00), 0.8);
            CheckGradientStop(fill.GradientStops[3], Color.FromArgb(0x00, 0x00, 0x00), 1.0);

            // Check second shape.
            fill = ((GroupShape)shapes[1]).Fill;
            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.GradientStyle, Is.EqualTo(GradientStyle.FromCenter));
            Assert.That(fill.GradientVariant, Is.EqualTo(GradientVariant.Variant1));
            Assert.That(fill.GradientStops.Count, Is.EqualTo(5));
            CheckGradientStop(fill.GradientStops[0], Color.FromArgb(0xFF, 0x00, 0x00), 0.65);
            CheckGradientStop(fill.GradientStops[1], Color.FromArgb(0x00, 0x00, 0x00), 0.3);
            CheckGradientStop(fill.GradientStops[2], Color.FromArgb(0x1F, 0x49, 0x7D), 0.0);
            CheckGradientStop(fill.GradientStops[3], Color.FromArgb(0xFF, 0xFF, 0xFF), 0.8);
            CheckGradientStop(fill.GradientStops[4], Color.FromArgb(0x00, 0xB0, 0x50), 1.0);

            // Check third shape.
            fill = ((GroupShape)shapes[2]).Fill;
            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.GradientStyle, Is.EqualTo(GradientStyle.FromCorner));
            Assert.That(fill.GradientVariant, Is.EqualTo(GradientVariant.Variant4));
            Assert.That(fill.GradientStops.Count, Is.EqualTo(6));
            CheckGradientStop(fill.GradientStops[0], Color.FromArgb(0xFF, 0x00, 0x00), 0.5);
            CheckGradientStop(fill.GradientStops[1], Color.FromArgb(0x00, 0x00, 0x00), 0.25);
            CheckGradientStop(fill.GradientStops[2], Color.FromArgb(0x9A, 0xB5, 0xE3), 0.0);
            CheckGradientStop(fill.GradientStops[3], Color.FromArgb(0x00, 0xB0, 0x50), 0.9);
            CheckGradientStop(fill.GradientStops[4], Color.FromArgb(0xF7, 0x96, 0x46), 0.8);
            CheckGradientStop(fill.GradientStops[5], Color.FromArgb(0x70, 0x30, 0xA0), 1.0);

            // Check fourth shape.
            fill = ((GroupShape)shapes[3]).Fill;
            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.GradientStyle, Is.EqualTo(GradientStyle.FromCenter));
            Assert.That(fill.GradientVariant, Is.EqualTo(GradientVariant.Variant1));
            Assert.That(fill.GradientStops.Count, Is.EqualTo(5));
            CheckGradientStop(fill.GradientStops[0], Color.FromArgb(0xFF, 0x00, 0x00), 0.65);
            CheckGradientStop(fill.GradientStops[1], Color.FromArgb(0x00, 0x00, 0x00), 0.3);
            CheckGradientStop(fill.GradientStops[2], Color.FromArgb(0x1F, 0x49, 0x7D), 0.0);
            CheckGradientStop(fill.GradientStops[3], Color.FromArgb(0xFF, 0xFF, 0xFF), 0.8);
            CheckGradientStop(fill.GradientStops[4], Color.FromArgb(0x00, 0xB0, 0x50), 1.0);

            // Check fifth shape.
            fill = ((GroupShape)shapes[4]).Fill;
            Assert.That(fill.FillType, Is.EqualTo(FillType.Gradient));
            Assert.That(fill.GradientStyle, Is.EqualTo(GradientStyle.FromCenter));
            Assert.That(fill.GradientVariant, Is.EqualTo(GradientVariant.Variant1));
            Assert.That(fill.GradientStops.Count, Is.EqualTo(5));
            // Strangely enough, Word shows 0.72 in both UI and VBA.
            // But the corresponding element is as follows: <a:gs pos="70000">.
            CheckGradientStop(fill.GradientStops[0], Color.FromArgb(0xFF, 0x00, 0x00), 0.70);
            CheckGradientStop(fill.GradientStops[1], Color.FromArgb(0x49, 0x44, 0x29), 0.4);
            CheckGradientStop(fill.GradientStops[2], Color.FromArgb(0x00, 0xB0, 0x50), 0.2);
            CheckGradientStop(fill.GradientStops[3], Color.FromArgb(0x1F, 0x49, 0x7D), 0.0);
            CheckGradientStop(fill.GradientStops[4], Color.FromArgb(0xFF, 0xFF, 0x00), 1.0);
        }



        /// <summary>
        /// Ability to get foreground colour without modifiers in Fill and Stroke classes.
        /// Implemented <see cref="Fill.BaseForeColor"/> and <see cref="Stroke.BaseForeColor"/> properties.
        /// </summary>
        [Test]
        public void Test24933()
        {
            Document doc = TestUtil.Open(@"Model\FillFormat\Test24933.docx");
            Shape shape = doc.FirstSection.Body.Shapes[0];

            Assert.That(shape.Fill.ForeColor, Is.EqualTo(Color.FromArgb(0xFE, 0x5D, 0x5D)));
            Assert.That(shape.Fill.BaseForeColor, Is.EqualTo(Color.FromArgb(0xFF, Color.Red)));

            Assert.That(shape.Stroke.ForeColor, Is.EqualTo(Color.FromArgb(0x0D, 0xFF, 0x0D)));
            Assert.That(shape.Stroke.BaseForeColor, Is.EqualTo(Color.FromArgb(0xFF, Color.Green)));

            Assert.That(shape.Stroke.Fill.ForeColor, Is.EqualTo(Color.FromArgb(0x0D, 0xFF, 0x0D)));
            Assert.That(shape.Stroke.Fill.BaseForeColor, Is.EqualTo(Color.FromArgb(0xFF, Color.Green)));
        }

        /// <summary>
        /// Checks properties of a specified Fill object.
        /// </summary>
        private static void CheckFill(Fill fill, Color expectedForeColor, bool expectedVisible, double expectedOpacity)
        {
            // Fill.Color takes in account Opacity.
            CheckColor(Color.FromArgb(MathUtil.DoubleToInt(expectedOpacity * 255d), expectedForeColor), fill.Color);
            CheckColor(expectedForeColor, fill.ForeColor);

            Assert.That(fill.Visible, Is.EqualTo(expectedVisible));

            Assert.That(fill.Opacity, Is.EqualTo(expectedOpacity).Within(0.01));
        }

        /// <summary>
        /// Verifies that image set correctly to the shape with a specified index in test document.
        /// </summary>
        private static Shape CheckSetImage(string testFileName, string imageFileName, int shapeIndex)
        {
            const string dir = @"Model\FillFormat\";

            string imagePath = TestUtil.BuildTestFileName(Path.Combine(dir, imageFileName));
            byte[] imageBytes = File.ReadAllBytes(imagePath);

            string testName = Path.Combine(dir, testFileName);
            Document doc = TestUtil.Open(testName);
            Shape shape = doc.FirstSection.Body.Shapes[shapeIndex];

            shape.Fill.SetImage(imagePath);

            // Roundtrip and check fill image.
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(testFileName), false);

            shape = doc.FirstSection.Body.Shapes[shapeIndex];
            Assert.That(ArrayUtil.IsArrayEqual(imageBytes, shape.Fill.ImageBytes), Is.True);

            return shape;
        }
    }
}
