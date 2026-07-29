// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2023 by Vadim Saltykov

using System;
using System.Drawing;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Themes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing ForeThemeColor and BackThemeColor of Fill object.
    /// </summary>
    [TestFixture]
    public class TestFillThemeColors : TestFillFormatBase
    {
        /// <summary>
        /// Tests getting and setting all theme colors.
        /// Dml case.
        /// </summary>
        [Test]
        public void TestThemeColorsDml()
        {
            ThemeColor[] expThemeColors = new ThemeColor[]
            {
                ThemeColor.None, ThemeColor.Dark2, ThemeColor.Text1, ThemeColor.Light1, ThemeColor.Dark1,
                ThemeColor.Accent2, ThemeColor.Accent4, ThemeColor.Accent1, ThemeColor.Light2, ThemeColor.Accent3,
                ThemeColor.Background1, ThemeColor.FollowedHyperlink, ThemeColor.Accent5, ThemeColor.Accent6,
                ThemeColor.Hyperlink, ThemeColor.Text2
            };

            Document doc = Open("ThemeColors", ShapeMarkupLanguage.Dml);

            for (int i = 0; i < doc.FirstSection.Body.Shapes.Count; i++)
            {
                Shape shape = doc.FirstSection.Body.Shapes[i];

                Assert.That(shape.Fill.BackThemeColor, Is.EqualTo(expThemeColors[i]));
                Assert.That(shape.Fill.ForeThemeColor, Is.EqualTo(expThemeColors[i]));

                if (i < doc.FirstSection.Body.Shapes.Count - 1)
                {
                    shape.Fill.BackThemeColor = expThemeColors[i + 1];
                    shape.Fill.ForeThemeColor = expThemeColors[i + 1];
                    Assert.That(shape.Fill.BackThemeColor, Is.EqualTo(expThemeColors[i + 1]));
                    Assert.That(shape.Fill.ForeThemeColor, Is.EqualTo(expThemeColors[i + 1]));
                }
                else
                {
                    shape.Fill.BackThemeColor = ThemeColor.None;
                    shape.Fill.ForeThemeColor = ThemeColor.None;
                    Assert.That(shape.Fill.ForeThemeColor, Is.EqualTo(ThemeColor.None));
                    Assert.That(shape.Fill.ForeColor, Is.EqualTo(Color.FromArgb(255, 68, 84, 106)));
                    Assert.That(shape.Fill.BackThemeColor, Is.EqualTo(ThemeColor.None));
                    Assert.That(shape.Fill.BackColor, Is.EqualTo(Color.FromArgb(255, 68, 84, 106)));
                }
            }
        }



        /// <summary>
        /// Tests getting and setting ForeThemeColor for different fill types.
        /// Vml case.
        /// </summary>
        [TestCase("NoFill")]
        [TestCase("FillMixed")]
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("Patterned")]
        [TestCase("Solid")]
        [TestCase("TextSolid")]
        public void TestForeThemeColorVmlSupportedFills(string fileName)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Vml);

            Fill fill = (fileName.Contains("Text"))
                ? doc.FirstSection.Body.FirstParagraph.FirstRun.Font.Fill
                : doc.FirstSection.Body.Shapes[0].Fill;

            Assert.That(fill.ForeThemeColor, Is.EqualTo(ThemeColor.None));
            fill.ForeThemeColor = ThemeColor.Accent1;

            fill = (fileName.Contains("Text"))
                ? doc.FirstSection.Body.FirstParagraph.FirstRun.Font.Fill
                : doc.FirstSection.Body.Shapes[0].Fill;
            Assert.That(fill.ForeThemeColor, Is.EqualTo(ThemeColor.Accent1));
        }

        /// <summary>
        /// Tests getting and setting BackThemeColor for different fill types.
        /// Vml case.
        /// </summary>
        [TestCase("NoFill", ThemeColor.None)]
        [TestCase("FillMixed", ThemeColor.None)]
        [TestCase("OneColorGradient", ThemeColor.None)]
        [TestCase("TwoColorGradient", ThemeColor.Light1)]
        [TestCase("Patterned", ThemeColor.Light1)]
        [TestCase("Solid", ThemeColor.Light1)]
        public void TestBackThemeColorVmlSupportedFills(string fileName, ThemeColor expThemeColor)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Vml);

            Fill fill = doc.FirstSection.Body.Shapes[0].Fill;
            // For a fill other than NoFill in VML, there is no way to distinguish between ThemeColor.Light1 and ThemeColor.None.
            Assert.That(fill.BackThemeColor, Is.EqualTo(expThemeColor));
            fill.BackThemeColor = ThemeColor.Accent1;

            fill = doc.FirstSection.Body.Shapes[0].Fill;
            Assert.That(fill.BackThemeColor, Is.EqualTo(ThemeColor.Accent1));
        }

        /// <summary>
        /// Tests throwing an exception when trying to set ForeThemeColor value for certain Fill types.
        /// ForeThemeColor cannot be set for BlipFill and PresetFill fills.
        /// </summary>
        [TestCase("UserPicture", ShapeMarkupLanguage.Vml)]
        [TestCase("PresetTextured", ShapeMarkupLanguage.Vml)]
        [TestCase("UserPicture", ShapeMarkupLanguage.Dml)]
        [TestCase("PresetTextured", ShapeMarkupLanguage.Dml)]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Cannot set ForeThemeColor to this Fill.")]
        public void TestForeThemeColorUnsupportedFill(string fileName, ShapeMarkupLanguage shapeMarkupLanguage)
        {
            Document doc = Open(fileName, shapeMarkupLanguage);

            Fill fill = doc.FirstSection.Body.Shapes[0].Fill;

            Assert.That(fill.ForeThemeColor, Is.EqualTo(ThemeColor.None));
            // Expect InvalidOperationException here.
            fill.ForeThemeColor = ThemeColor.Accent1;
        }

        /// <summary>
        /// Tests throwing an exception when trying to set BackThemeColor value for certain Fill types.
        /// BackThemeColor cannot be set for BlipFill and PresetFill fills.
        /// </summary>
        [TestCase("UserPicture", ShapeMarkupLanguage.Vml)]
        [TestCase("PresetTextured", ShapeMarkupLanguage.Vml)]
        [TestCase("UserPicture", ShapeMarkupLanguage.Dml)]
        [TestCase("PresetTextured", ShapeMarkupLanguage.Dml)]
        [TestCase("TextSolid", ShapeMarkupLanguage.Dml)]
        [TestCase("TextSolid", ShapeMarkupLanguage.Vml)]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Cannot set BackThemeColor to this Fill.")]
        public void TestBackThemeColorUnsupportedFill(string fileName, ShapeMarkupLanguage shapeMarkupLanguage)
        {
            Document doc = Open(fileName, shapeMarkupLanguage );

            Fill fill = (fileName.Contains("TextSolid"))
                ? doc.FirstSection.Body.FirstParagraph.FirstRun.Font.Fill
                : doc.FirstSection.Body.Shapes[0].Fill;

            Assert.That(fill.BackThemeColor, Is.EqualTo(ThemeColor.None));
            // Expect InvalidOperationException here.
            fill.BackThemeColor = ThemeColor.Accent1;
        }

        private void CheckForeThemeColorDmlSupportedFill(string fileName, DmlFillType dmlFillType, bool fillChanging)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Dml);

            ShapeBase shapeNode = (dmlFillType == DmlFillType.GroupFill)
                ? (ShapeBase)doc.GetChild(NodeType.GroupShape, 0, true)
                : doc.FirstSection.Body.Shapes[0];
            DmlFill dmlFill = (DmlFill)shapeNode.FillCore;
            Fill fill = shapeNode.Fill;
            Assert.That(fill.ForeThemeColor, Is.EqualTo(ThemeColor.None));

            fill.ForeThemeColor = ThemeColor.Accent1;
            dmlFill = (DmlFill)shapeNode.FillCore;
            fill = shapeNode.Fill;
            Assert.That(fill.ForeThemeColor, Is.EqualTo(ThemeColor.Accent1));
            // Setting ForeThemeColor value should not change BackThemeColor values.
            Assert.That(fill.BackThemeColor, Is.EqualTo(ThemeColor.None));
            // Setting ForeThemeColor for NoFill, StyleFill and GroupFill changes the fill type to SolidFill.
            // The fill type does not change for other types.
            Assert.That(dmlFill.DmlFillType, Is.EqualTo((fillChanging) ? DmlFillType.SolidFill : dmlFillType));

            // Checks setting ThemeColor.None.
            DrColor expColor = dmlFill.ColorInternal;
            fill.ForeThemeColor = ThemeColor.None;
            dmlFill = (DmlFill)shapeNode.FillCore;
            fill = shapeNode.Fill;
            Assert.That(fill.ForeThemeColor, Is.EqualTo(ThemeColor.None));
            Assert.That(dmlFill.ColorInternal, Is.EqualTo(expColor));
        }

        private void CheckBackThemeColorDmlSupportedFill(string fileName, DmlFillType dmlFillType, bool fillChanging)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Dml);

            ShapeBase shapeNode = (dmlFillType == DmlFillType.GroupFill)
                ? (ShapeBase)doc.GetChild(NodeType.GroupShape, 0, true)
                : doc.FirstSection.Body.Shapes[0];

            DmlFill dmlFill = (DmlFill)shapeNode.FillCore;
            Fill fill = shapeNode.Fill;
            Assert.That(fill.BackThemeColor, Is.EqualTo(ThemeColor.None));

            fill.BackThemeColor = ThemeColor.Accent1;
            dmlFill = (DmlFill)shapeNode.FillCore;
            fill = shapeNode.Fill;
            Assert.That(fill.BackThemeColor, Is.EqualTo(ThemeColor.Accent1));

            // Setting BackThemeColor for GradientFill and PatternFill does not change the fill type and ForeThemeColor value.
            // PatternFill with ForeThemeColor equal to BackThemeColor is set for all other fill types.
            Assert.That(dmlFill.DmlFillType, Is.EqualTo((fillChanging) ? DmlFillType.PatternFill : dmlFillType));
            Assert.That(fill.ForeThemeColor, Is.EqualTo((fillChanging) ? fill.BackThemeColor : ThemeColor.None));

            // Checks setting ThemeColor.None.
            DrColor expColor = dmlFill.Color2Internal;
            fill.BackThemeColor = ThemeColor.None;
            dmlFill = (DmlFill)shapeNode.FillCore;
            fill = shapeNode.Fill;
            Assert.That(fill.BackThemeColor, Is.EqualTo(ThemeColor.None));
            Assert.That(dmlFill.Color2Internal, Is.EqualTo(expColor));
        }

        /// <summary>
        /// DefaultTheme contains the same color values for Dark1 and Text1, Dark2 and Text2,
        /// Background1 and Light1, and Background2 and Light2.
        /// Therefore, these ThemeColor-s for VML cannot be distinguished.
        /// </summary>
        private ThemeColor GetVmlRealThemeColor(ThemeColor desiredColor)
        {
            switch (desiredColor)
            {
                case ThemeColor.Text1:
                    return ThemeColor.Dark1;
                case ThemeColor.Text2:
                    return ThemeColor.Dark2;
                case ThemeColor.Background1:
                    return ThemeColor.Light1;
                case ThemeColor.Background2:
                    return ThemeColor.Light2;
                default:
                    return desiredColor;
            }
        }
    }
}
