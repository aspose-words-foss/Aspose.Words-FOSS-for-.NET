// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/05/2021 by Vadim Saltykov

using System;
using System.Drawing;
using Aspose.Drawing;
using Aspose.JavaAttributes;
using Aspose.TestFx;
using Aspose.Words.Drawing;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing PresetTextured() method of Fill object.
    /// </summary>
    [TestFixture]
    public class TestFillPatterned : TestFillFormatBase
    {
        /// <summary>
        /// Test shows slightly confusing behavior related to background color fill and fill type change in DML.
        /// It is interesting that MS Word does the same.
        /// </summary>
        [Test]
        public void TestPatternedNotPatternFillDml()
        {
            Document doc = Open("Solid", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);
            fill.ForeColor = DrColor.Blue.ToNativeColor();
            fill.BackColor = DrColor.Red.ToNativeColor();
            fill.Patterned(PatternType.DarkDownwardDiagonal);
            Assert.That(DrColor.Blue.ToNativeColor(), Is.EqualTo(fill.ForeColor));
            // Solid fill has no background color and it's not preserved when the fill type is changed.
            // It is interesting that MS Word does the same
            Assert.That(DrColor.Red.ToNativeColor(), IsNot.EqualTo(fill.BackColor));

            // Let's try again.
            // Pattern fill preserves background color.
            fill.ForeColor = DrColor.Blue.ToNativeColor();
            fill.BackColor = DrColor.Red.ToNativeColor();
            fill.Patterned(PatternType.DarkHorizontal);
            Assert.That(DrColor.Blue.ToNativeColor(), Is.EqualTo(fill.ForeColor));
            Assert.That(DrColor.Red.ToNativeColor(), Is.EqualTo(fill.BackColor));
        }

        /// <summary>
        /// Checks if solid fill already preserves background color in VML.
        /// </summary>
        [Test]
        public void TestPatternedNotPatternFillVml()
        {
            Document doc = Open("Solid", ShapeMarkupLanguage.Vml);
            Fill fill = GetFill(doc);
            fill.ForeColor = DrColor.Blue.ToNativeColor();
            fill.BackColor = DrColor.Red.ToNativeColor();
            fill.Patterned(PatternType.DarkDownwardDiagonal);
            Assert.That(DrColor.Blue.ToNativeColor(), Is.EqualTo(fill.ForeColor));
            // Solid fill already preserves background color.
            // MS Word has the same behaviour.
            Assert.That(DrColor.Red.ToNativeColor(), Is.EqualTo(fill.BackColor));
        }

        /// <summary>
        /// Checks pattern setting to DML shape.
        /// </summary>
        [TestCase(PatternType.Percent5)]
        [TestCase(PatternType.Percent10)]
        [TestCase(PatternType.Percent25)]
        [TestCase(PatternType.Percent30)]
        [TestCase(PatternType.Percent40)]
        [TestCase(PatternType.Percent50)]
        [TestCase(PatternType.Percent60)]
        [TestCase(PatternType.Percent70)]
        [TestCase(PatternType.Percent80)]
        [TestCase(PatternType.Percent90)]
        [TestCase(PatternType.Cross)]
        [TestCase(PatternType.DarkDownwardDiagonal)]
        [TestCase(PatternType.DarkHorizontal)]
        [TestCase(PatternType.DarkUpwardDiagonal)]
        [TestCase(PatternType.DarkVertical)]
        [TestCase(PatternType.DashedDownwardDiagonal)]
        [TestCase(PatternType.DashedHorizontal)]
        [TestCase(PatternType.DashedUpwardDiagonal)]
        [TestCase(PatternType.DashedVertical)]
        [TestCase(PatternType.DiagonalBrick)]
        [TestCase(PatternType.DiagonalCross)]
        [TestCase(PatternType.Divot)]
        [TestCase(PatternType.DottedDiamond)]
        [TestCase(PatternType.DottedGrid)]
        [TestCase(PatternType.DownwardDiagonal)]
        [TestCase(PatternType.Horizontal)]
        [TestCase(PatternType.HorizontalBrick)]
        [TestCase(PatternType.LargeCheckerBoard)]
        [TestCase(PatternType.LargeConfetti)]
        [TestCase(PatternType.LargeGrid)]
        [TestCase(PatternType.LightDownwardDiagonal)]
        [TestCase(PatternType.LightHorizontal)]
        [TestCase(PatternType.LightUpwardDiagonal)]
        [TestCase(PatternType.LightVertical)]
        [TestCase(PatternType.NarrowHorizontal)]
        [TestCase(PatternType.NarrowVertical)]
        [TestCase(PatternType.OutlinedDiamond)]
        [TestCase(PatternType.Plaid)]
        [TestCase(PatternType.Shingle)]
        [TestCase(PatternType.SmallCheckerBoard)]
        [TestCase(PatternType.SmallConfetti)]
        [TestCase(PatternType.SmallGrid)]
        [TestCase(PatternType.SolidDiamond)]
        [TestCase(PatternType.Sphere)]
        [TestCase(PatternType.Trellis)]
        [TestCase(PatternType.UpwardDiagonal)]
        [TestCase(PatternType.Vertical)]
        [TestCase(PatternType.Wave)]
        [TestCase(PatternType.Weave)]
        [TestCase(PatternType.WideDownwardDiagonal)]
        [TestCase(PatternType.WideUpwardDiagonal)]
        [TestCase(PatternType.ZigZag)]
        public void TestShapePatternedDml(PatternType patternType)
        {
            Document doc = Open("NoFill", ShapeMarkupLanguage.Dml);

            Fill fill = GetFill(doc);
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));

            fill.Patterned(patternType);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Patterned));
            Assert.That(fill.Pattern, Is.EqualTo(patternType));
        }

        /// <summary>
        /// Checks pattern setting to DML shape.
        /// </summary>
        [TestCase(PatternType.Percent5)]
        [TestCase(PatternType.Percent10)]
        [TestCase(PatternType.Percent25)]
        [TestCase(PatternType.Percent30)]
        [TestCase(PatternType.Percent40)]
        [TestCase(PatternType.Percent50)]
        [TestCase(PatternType.Percent60)]
        [TestCase(PatternType.Percent70)]
        [TestCase(PatternType.Percent80)]
        [TestCase(PatternType.Percent90)]
        [TestCase(PatternType.Cross)]
        [TestCase(PatternType.DarkDownwardDiagonal)]
        [TestCase(PatternType.DarkHorizontal)]
        [TestCase(PatternType.DarkUpwardDiagonal)]
        [TestCase(PatternType.DarkVertical)]
        [TestCase(PatternType.DashedDownwardDiagonal)]
        [TestCase(PatternType.DashedHorizontal)]
        [TestCase(PatternType.DashedUpwardDiagonal)]
        [TestCase(PatternType.DashedVertical)]
        [TestCase(PatternType.DiagonalBrick)]
        [TestCase(PatternType.DiagonalCross)]
        [TestCase(PatternType.Divot)]
        [TestCase(PatternType.DottedDiamond)]
        [TestCase(PatternType.DottedGrid)]
        [TestCase(PatternType.DownwardDiagonal)]
        [TestCase(PatternType.Horizontal)]
        [TestCase(PatternType.HorizontalBrick)]
        [TestCase(PatternType.LargeCheckerBoard)]
        [TestCase(PatternType.LargeConfetti)]
        [TestCase(PatternType.LargeGrid)]
        [TestCase(PatternType.LightDownwardDiagonal)]
        [TestCase(PatternType.LightHorizontal)]
        [TestCase(PatternType.LightUpwardDiagonal)]
        [TestCase(PatternType.LightVertical)]
        [TestCase(PatternType.NarrowHorizontal)]
        [TestCase(PatternType.NarrowVertical)]
        [TestCase(PatternType.OutlinedDiamond)]
        [TestCase(PatternType.Plaid)]
        [TestCase(PatternType.Shingle)]
        [TestCase(PatternType.SmallCheckerBoard)]
        [TestCase(PatternType.SmallConfetti)]
        [TestCase(PatternType.SmallGrid)]
        [TestCase(PatternType.SolidDiamond)]
        [TestCase(PatternType.Sphere)]
        [TestCase(PatternType.Trellis)]
        [TestCase(PatternType.UpwardDiagonal)]
        [TestCase(PatternType.Vertical)]
        [TestCase(PatternType.Wave)]
        [TestCase(PatternType.Weave)]
        [TestCase(PatternType.WideDownwardDiagonal)]
        [TestCase(PatternType.WideUpwardDiagonal)]
        [TestCase(PatternType.ZigZag)]
        [JavaDelete("WORDSJAVA-2634")]
        public void TestShapePatternedVml(PatternType patternType)
        {
            Document doc = Open("NoFill", ShapeMarkupLanguage.Vml);

            Fill fill = GetFill(doc);
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));

            fill.Patterned(patternType);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Patterned));
            if (patternType == PatternType.Cross)
                Assert.That(fill.Pattern, Is.EqualTo(PatternType.LargeGrid));
            else 
               Assert.That(fill.Pattern, Is.EqualTo(patternType));
        }




        /// <summary>
        /// Checks setting patterned default colors.
        /// </summary>
        [Test]
        public void TestPatternedDefaultColorsDml()
        {
            Document doc = Open("NoFill", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);

            fill.Patterned(PatternType.DarkDownwardDiagonal);
            
            Assert.That(doc.Theme.Colors.Accent1, Is.EqualTo(fill.ForeColor));
            TestFxUtil.AreEqualArgb(fill.BackColor, Color.White);
        }

        /// <summary>
        /// Checks setting patterned default colors.
        /// </summary>
        [Test]
        public void TestPatternedDefaultColorsVml()
        {
            Document doc = Open("NoFill", ShapeMarkupLanguage.Vml);
            Fill fill = GetFill(doc);

            fill.Patterned(PatternType.DarkDownwardDiagonal);
           
            Assert.That(DrColor.Black.ToNativeColor(), Is.EqualTo(fill.ForeColor));
            TestFxUtil.AreEqualArgb(fill.BackColor, Color.White);
        }

        /// <summary>
        /// Checks applying pattern to different types of DML fill.
        /// </summary>
        [TestCase("Solid", 255, 0, 0)]
        [TestCase("OneColorGradient", 0, 128, 128)]
        [TestCase("TwoColorGradient", 128, 0, 0)]
        [TestCase("PresetGradient", 204, 204, 255)]
        [TestCase("PresetTextured", -1, -1, -1)]
        [TestCase("UserPicture", -1, -1, -1)]
        [TestCase("UserTextured", -1, -1, -1)]
        public void TestPatternedFillDml(string fileName, int r, int g, int b)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Dml);

            Fill fill = GetFill(doc);

            Assert.That(fill.Transparency, Is.EqualTo(0.37).Within(0.01));
            Color expectedColor = (r != -1)
                ? Color.FromArgb(r, g, b)
                : Color.Empty;
            Assert.That(fill.ForeColor, Is.EqualTo(expectedColor));

            fill.Patterned(PatternType.DarkDownwardDiagonal, Color.Blue, Color.Red);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Patterned));
            Assert.That(fill.Pattern, Is.EqualTo(PatternType.DarkDownwardDiagonal));
            Assert.That(fill.Transparency, Is.EqualTo(0.0).Within(0.01));
            Assert.That(fill.ForeColor, Is.EqualTo(Color.FromArgb(255, 0, 0, 255)));
            Assert.That(fill.BackColor, Is.EqualTo(Color.FromArgb(255, 255, 0, 0)));
        }

        /// <summary>
        /// Checks applying pattern to different types of VML fill.
        /// </summary>
        [TestCase("Solid", 255, 0, 0)]
        [TestCase("OneColorGradient", 0, 128, 128)]
        [TestCase("TwoColorGradient", 128, 0, 0)]
        [TestCase("PresetGradient", 204, 204, 255)]
        [TestCase("PresetTextured", 255, 255, 255)]
        [TestCase("UserPicture", 255, 255, 255)]
        [TestCase("UserTextured", 255, 255, 255)]
        public void TestTexturedSolidFillVml(string fileName, int r, int g, int b)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Vml);

            Fill fill = GetFill(doc);

            Assert.That(fill.Transparency, Is.EqualTo(0.37).Within(0.01));
            Assert.That(fill.ForeColor, Is.EqualTo(Color.FromArgb(r, g, b)));

            fill.Patterned(PatternType.DarkDownwardDiagonal, Color.Blue, Color.Red);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Patterned));
            Assert.That(fill.Pattern, Is.EqualTo(PatternType.DarkDownwardDiagonal));
            Assert.That(fill.Transparency, Is.EqualTo(0.37).Within(0.01));
            Assert.That(fill.ForeColor, Is.EqualTo(Color.FromArgb(255, 0, 0, 255)));
            Assert.That(fill.BackColor, Is.EqualTo(Color.FromArgb(255, 255, 0, 0)));
        }

        /// <summary>
        /// Checks applying pattern to Font fill.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Pattern cannot be applied to Font.")]
        public void TestTextPatterned()
        {
            Document doc = Open("TextNoFill", ShapeMarkupLanguage.Vml);

            Fill fill = GetFill(doc);
            fill.Patterned(PatternType.Percent10, Color.Blue, Color.Blue);
        }

        /// <summary>
        /// Checks obtaining pattern from GroupShape fill.
        /// </summary>
        [TestCase(ShapeMarkupLanguage.Vml)]
        [TestCase(ShapeMarkupLanguage.Dml)]
        public void TestPresetTextureMixed(ShapeMarkupLanguage markupLanguage)
        {
            Document doc = Open("FillMixed", markupLanguage);

            GroupShape groupShape = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);
            Fill fill = groupShape.Fill;

            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(fill.Pattern, Is.EqualTo(PatternType.None));
        }

        /// <summary>
        /// Checks obtaining pattern type from fill which is not FillType.Patterned.
        /// </summary>
        /// <remarks>
        /// Mimics MS Word behavior.
        /// </remarks>
        [TestCase("NoFill")]
        [TestCase("Solid")]
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("PresetTextured")]
        [TestCase("UserPicture")]
        [TestCase("UserTextured")]
        public void TestPresetTextureNoneAsDefault(string fileName)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Vml);

            Fill fill = GetFill(doc);

            Assert.That(fill.Pattern, Is.EqualTo(PatternType.None));
        }

        /// <summary>
        /// Checks setting PatternType.None.
        /// </summary>
        /// <remarks>
        /// Mimics MS Word behavior.
        /// </remarks>
        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "PatternType.None cannot be applied directly.")]
        public void TestPatternNone()
        {
            Document doc = Open("NoFill", ShapeMarkupLanguage.Vml);

            Fill fill = GetFill(doc);
            fill.Patterned(PatternType.None, Color.Blue, Color.Blue);
        }
    }
}
