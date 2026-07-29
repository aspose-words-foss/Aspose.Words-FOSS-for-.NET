// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for shapes fill and line properties.
    /// </summary>
    [TestFixture]
    public class TestShapeFillAndLine : UnifiedTestsBase
    {
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFillStyle(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Fill\TestFillStyle", lf, sf);

            // Default fill
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.Filled, Is.EqualTo(true));
            Assert.That(shape.FillColor.ToArgb(), Is.EqualTo(Color.White.ToArgb()));
            Assert.That(shape.Fill.Visible, Is.EqualTo(true));
            Assert.That(shape.Fill.ForeColor.ToArgb(), Is.EqualTo(Color.White.ToArgb()));
            Assert.That(shape.Fill.BackColor.ToArgb(), Is.EqualTo(Color.White.ToArgb()));
            Assert.That(shape.Fill.Opacity, Is.EqualTo(1.0));
            Assert.That(shape.FillCore.Opacity2, Is.EqualTo(1.0));
            Assert.That(shape.Fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(shape.FillCore.Focus, Is.EqualTo(0));
            Assert.That(shape.FillCore.Angle, Is.EqualTo(0.0));
            Assert.That(shape.FillCore.FocusLeft, Is.EqualTo(0.0));
            Assert.That(shape.FillCore.FocusTop, Is.EqualTo(0.0));
            Assert.That(shape.FillCore.GradientColors, Is.EqualTo(null));

            // Red transparent fill
            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Assert.That(shape.FillColor.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            Assert.That(shape.Fill.Opacity, Is.EqualTo(0.6).Within(0.01));

            // No fill
            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            Assert.That(shape.Filled, Is.EqualTo(false));
            Assert.That(shape.FillColor.ToArgb(), Is.EqualTo(Color.White.ToArgb()));
            Assert.That(shape.Fill.FillType, Is.EqualTo(FillType.Solid));

            // One color horizontal gradient.
            shape = (Shape)doc.GetChild(NodeType.Shape, 3, true);
            // TODO 2 No idea how to represent different gradient types in enum.
            Assert.That(shape.FillCore.FillType, Is.EqualTo(FillTypeCore.ShadeScale));
            Assert.That(shape.FillCore.Focus, Is.EqualTo(100));
            Assert.That(shape.Fill.ForeColor.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            // TODO 2 No idea what this color means. In WordML: "fill darken(51)". 0x33 means 51, 0x00100 means darken.
            Assert.That(shape.FillCore.Color2Internal.ToNativeColor().ToArgb(), Is.EqualTo(unchecked((int)0xeff00133)));

            // One color vertical gradient. Different darkness and focus. In MS Word focus is called "variant".
            shape = (Shape)doc.GetChild(NodeType.Shape, 4, true);
            Assert.That(shape.FillCore.FillType, Is.EqualTo(FillTypeCore.ShadeScale));
            Assert.That(shape.FillCore.Focus, Is.EqualTo(50));
            Assert.That(shape.Fill.ForeColor.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            // TODO 2 No idea what this color means. In WordML: "fill lighten(102)". 0x66 means 102, 0x00200 means lighten.
            Assert.That(shape.FillCore.Color2Internal.ToNativeColor().ToArgb(), Is.EqualTo(unchecked((int)0xeff00266)));
            Assert.That(shape.FillCore.Angle, Is.EqualTo(-90.0));

            // One color gradient from corner.
            shape = (Shape)doc.GetChild(NodeType.Shape, 5, true);
            Assert.That(shape.FillCore.FillType, Is.EqualTo(FillTypeCore.ShadeCenter));
            Assert.That(shape.FillCore.FocusLeft, Is.EqualTo(0.0));
            Assert.That(shape.FillCore.FocusTop, Is.EqualTo(1.0));

            // One color gradient from center. Also a range of transparency.
            shape = (Shape)doc.GetChild(NodeType.Shape, 6, true);
            Assert.That(shape.FillCore.FillType, Is.EqualTo(FillTypeCore.ShadeShape));
            Assert.That(shape.FillCore.FocusLeft, Is.EqualTo(0.5));
            Assert.That(shape.FillCore.FocusTop, Is.EqualTo(0.5));
            Assert.That(shape.Fill.Opacity, Is.EqualTo(0.9).Within(0.01));
            Assert.That(shape.FillCore.Opacity2, Is.EqualTo(0.3).Within(0.01));

            // Two color diagonal gradient.
            shape = (Shape)doc.GetChild(NodeType.Shape, 8, true);
            Assert.That(shape.FillCore.FillType, Is.EqualTo(FillTypeCore.ShadeScale));
            Assert.That(shape.Fill.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            Assert.That(shape.FillCore.Color2Internal.ToNativeColor().ToArgb(), Is.EqualTo(Color.Yellow.ToArgb()));

            // Rainbow predefined gradient. Fill rotated with the shape.
            shape = (Shape)doc.GetChild(NodeType.Shape, 9, true);
            Assert.That(shape.FillCore.FillType, Is.EqualTo(FillTypeCore.ShadeScale));
            GradientColor[] colors = shape.FillCore.GradientColors;
            Assert.That(colors.Length, Is.EqualTo(7));
            Assert.That(colors[1].Color.ToArgb(), Is.EqualTo(unchecked((int)0xff0819fb)));
            Assert.That(colors[1].Start, Is.EqualTo(13763));

            // Texture fill.
            shape = (Shape)doc.GetChild(NodeType.Shape, 11, true);
            Assert.That(shape.Fill.FillType, Is.EqualTo(FillType.Textured));
            Assert.That(shape.Fill.ImageBytes, IsNot.Null());
            // This is PresetTexture.FishFossil, but we only store this attribute in DOC and RTF.

            // Pattern fill.
            shape = (Shape)doc.GetChild(NodeType.Shape, 12, true);
            Assert.That(shape.Fill.FillType, Is.EqualTo(FillType.Patterned));
            // This is PresetTexture.HorizontalBrick, but we only store this attribute in DOC and RTF.

            // Picture fill.
            shape = (Shape)doc.GetChild(NodeType.Shape, 13, true);
            Assert.That(shape.Fill.FillType, Is.EqualTo(FillType.Picture));
            // This is PresetTexture.Custom.
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFillPattern(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Fill\TestFillPattern", lf, sf);

            // WORDSNET-1118 Pattern or texture fills are not saved to DOC.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.Fill.ImageBytes, IsNot.Null());
            // This is PresetTexture.Newspint, but we only store this attribute in DOC and RTF.
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineStyle(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Stroke\TestLineStyle", lf, sf);

            // Default line
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.Stroked, Is.EqualTo(true));
            Assert.That(shape.StrokeColor.ToArgb(), Is.EqualTo(Color.Black.ToArgb()));
            Assert.That(shape.StrokeWeight, Is.EqualTo(0.75));
            Assert.That(shape.Stroke.LineStyle, Is.EqualTo(ShapeLineStyle.Single));
            Assert.That(shape.Stroke.LineFillType, Is.EqualTo(LineFillType.Solid));
            Assert.That(shape.Stroke.Opacity, Is.EqualTo(1.0));
            Assert.That(shape.Stroke.DashStyle, Is.EqualTo(DashStyle.Solid));
            Assert.That(shape.Stroke.StartArrowType, Is.EqualTo(ArrowType.None));
            Assert.That(shape.Stroke.EndArrowType, Is.EqualTo(ArrowType.None));
            Assert.That(shape.Stroke.StartArrowLength, Is.EqualTo(ArrowLength.Medium));
            Assert.That(shape.Stroke.StartArrowWidth, Is.EqualTo(ArrowWidth.Medium));
            Assert.That(shape.Stroke.EndArrowLength, Is.EqualTo(ArrowLength.Medium));
            Assert.That(shape.Stroke.EndArrowWidth, Is.EqualTo(ArrowWidth.Medium));
            Assert.That(shape.Stroke.JoinStyle, Is.EqualTo(JoinStyle.Miter));
            Assert.That(shape.Stroke.ImageBytes, Is.Null);

            // Invisible line
            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Assert.That(shape.Stroked, Is.EqualTo(false));
            Assert.That(shape.StrokeColor.ToArgb(), Is.EqualTo(Color.Black.ToArgb()));
            Assert.That(shape.StrokeWeight, Is.EqualTo(0.75));
            Assert.That(shape.Stroke.LineStyle, Is.EqualTo(ShapeLineStyle.Single));

            // Thick red
            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            Assert.That(shape.StrokeColor.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            Assert.That(shape.StrokeWeight, Is.EqualTo(6.0));

            // Textured
            shape = (Shape)doc.GetChild(NodeType.Shape, 3, true);
            Assert.That(shape.Stroke.LineFillType, Is.EqualTo(LineFillType.Pattern));
            Assert.That(shape.Stroke.Color.ToArgb(), Is.EqualTo(unchecked((int)0xff800000)));
            Assert.That(shape.Stroke.Color2.ToArgb(), Is.EqualTo(Color.Yellow.ToArgb()));
            Assert.That(shape.Stroke.ImageBytes, IsNot.Null());
            // This is PresetTexture.Weave, but we only store this attribute in DOC and RTF.

            // Transparent
            shape = (Shape)doc.GetChild(NodeType.Shape, 4, true);
            Assert.That(shape.Stroke.Opacity, Is.EqualTo(0.32).Within(0.01));

            // Dotted
            shape = (Shape)doc.GetChild(NodeType.Shape, 5, true);
            Assert.That(shape.Stroke.DashStyle, Is.EqualTo(DashStyle.ShortDot));

            // Double
            shape = (Shape)doc.GetChild(NodeType.Shape, 12, true);
            Assert.That(shape.Stroke.LineStyle, Is.EqualTo(ShapeLineStyle.Double));

            // Arrow
            shape = (Shape)doc.GetChild(NodeType.Shape, 19, true);
            Assert.That(shape.Stroke.StartArrowType, Is.EqualTo(ArrowType.Arrow));
            Assert.That(shape.Stroke.EndArrowType, Is.EqualTo(ArrowType.Open));
            Assert.That(shape.Stroke.StartArrowLength, Is.EqualTo(ArrowLength.Long));
            Assert.That(shape.Stroke.StartArrowWidth, Is.EqualTo(ArrowWidth.Wide));
            Assert.That(shape.Stroke.EndArrowLength, Is.EqualTo(ArrowLength.Medium));
            Assert.That(shape.Stroke.EndArrowWidth, Is.EqualTo(ArrowWidth.Medium));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLinePattern(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Stroke\TestLinePattern", lf, sf);

            // WORDSNET-1118 Pattern or texture fills are not saved to DOC.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.Stroke.ImageBytes, IsNot.Null());
            // This is PresetTexture.HorizontalBrick, but we only store this attribute in DOC and RTF.
        }


        /// <summary>
        /// Test horizontal line. This is inserted in MS Word in Format / Borders and Shading dialog box.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHR(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Geometry\TestHR", lf, sf);

            // HR with image
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.Image));
            Assert.That(shape.IsInline, Is.EqualTo(true));
            Assert.That(shape.Width, Is.EqualTo(396));
            Assert.That(shape.Height, Is.EqualTo(22.5));
            Assert.That(shape.IsHorizontalRule, Is.EqualTo(true));
            Assert.That(shape.HorizontalRule.On, Is.EqualTo(true));
            Assert.That(shape.HorizontalRule.Percent, Is.EqualTo(0));
            Assert.That(shape.HorizontalRule.Align, Is.EqualTo(HorizontalRuleAlignment.Center));
            Assert.That(shape.HorizontalRule.Standard, Is.EqualTo(false));

            // Standard HR
            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.Rectangle));
            Assert.That(shape.IsInline, Is.EqualTo(true));

            switch (lf)
            {
                case LoadFormat.Doc:
                case LoadFormat.Rtf:
                    Assert.That(shape.Width, Is.EqualTo(432));
                    break;
                case LoadFormat.WordML:
                case LoadFormat.Docx:
                    // RK For DOCX and WML MS Word writes 0 here.
                    // Maybe it means that shape width should equal to page width.
                    Assert.That(shape.Width, Is.EqualTo(0));
                    break;
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }

            Assert.That(shape.Height, Is.EqualTo(1.5));
            Assert.That(shape.IsHorizontalRule, Is.EqualTo(true));
            Assert.That(shape.HorizontalRule.On, Is.EqualTo(true));
            Assert.That(shape.HorizontalRule.Percent, Is.EqualTo(100));
            Assert.That(shape.HorizontalRule.NoShade, Is.EqualTo(false));
            Assert.That(shape.HorizontalRule.Standard, Is.EqualTo(true));
        }


    }
}
