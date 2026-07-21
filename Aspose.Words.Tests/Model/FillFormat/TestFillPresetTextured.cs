// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/04/2021 by Vadim Saltykov

using System;
using System.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml.Fills;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model.FillFormat
{
    /// <summary>
    /// The class for testing PresetTextured() method of Fill object.
    /// </summary>
    [TestFixture]
    public class TestFillPresetTextured : TestFillFormatBase
    {
        /// <summary>
        /// Checks attribute values when applying different PresetTexture to VML shape.
        /// </summary>
        /// <remarks>
        /// Mimics MS Word behavior.
        /// </remarks>
        [TestCase(PresetTexture.BlueTissuePaper, "Blue tissue paper", 204, 236, 255)]
        [TestCase(PresetTexture.Bouquet, "Bouquet", 204, 204, 255)]
        [TestCase(PresetTexture.BrownMarble, "Brown marble", 102, 51, 0)]
        [TestCase(PresetTexture.Canvas, "Canvas", 255, 255, 204)]
        [TestCase(PresetTexture.Cork, "Cork", 153, 102, 0)]
        [TestCase(PresetTexture.Denim, "Denim", 102, 153, 255)]
        [TestCase(PresetTexture.FishFossil, "Fish fossil", 255, 204, 153)]
        [TestCase(PresetTexture.Oak, "Oak", 255, 204, 153)]
        [TestCase(PresetTexture.PaperBag, "Paper bag", 255, 204, 153)]
        [TestCase(PresetTexture.Papyrus, "Papyrus", 255, 204, 153)]
        [TestCase(PresetTexture.Granite, "Granite", 221, 221, 221)]
        [TestCase(PresetTexture.GreenMarble, "Green marble", 0, 102, 0)]
        [TestCase(PresetTexture.MediumWood, "Medium wood", 153, 102, 51)]
        [TestCase(PresetTexture.Newsprint, "Newsprint", 248, 248, 248)]
        [TestCase(PresetTexture.Parchment, "Parchment", 255, 255, 204)]
        [TestCase(PresetTexture.PinkTissuePaper, "Pink tissue paper", 255, 204, 204)]
        [TestCase(PresetTexture.PurpleMesh, "Purple mesh", 153, 0, 204)]
        [TestCase(PresetTexture.RecycledPaper, "Recycled paper", -1, -1, -1)]
        [TestCase(PresetTexture.WhiteMarble, "White marble", -1, -1, -1)]
        [TestCase(PresetTexture.Sand, "Sand", 192, 192, 192)]
        [TestCase(PresetTexture.Stationery, "Stationery", 255, 255, 204)]
        [TestCase(PresetTexture.Walnut, "Walnut", 102, 51, 0)]
        [TestCase(PresetTexture.WaterDroplets, "Water droplets", 204, 255, 255)]
        [TestCase(PresetTexture.WovenMat, "Woven mat", 255, 204, 102)]
        public void TestShapePresetTexturedVml(PresetTexture presetTexture, string textureTitle, int r, int g, int b)
        {
            Document doc = Open("NoFill", ShapeMarkupLanguage.Vml);

            Shape shape = (Shape)GetFirstFillable(doc);
            Fill fill = shape.Fill;

            fill.PresetTextured(presetTexture);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Textured));
            Assert.That(fill.PresetTexture, Is.EqualTo(presetTexture));
            if (r == -1)
                Assert.That(shape.ShapePr[ShapeAttr.FillColor], Is.Null);
            else
                Assert.That(fill.ForeColor, Is.EqualTo(Color.FromArgb(r, g, b)));
            Assert.That(shape.ShapePr[ShapeAttr.FillBlipName], Is.EqualTo(textureTitle));
        }

        /// <summary>
        /// Checks texture setting when applying different PresetTexture to DML shape.
        /// </summary>
        [TestCase(PresetTexture.BlueTissuePaper)]
        [TestCase(PresetTexture.Bouquet)]
        [TestCase(PresetTexture.BrownMarble)]
        [TestCase(PresetTexture.Canvas)]
        [TestCase(PresetTexture.Cork)]
        [TestCase(PresetTexture.Denim)]
        [TestCase(PresetTexture.FishFossil)]
        [TestCase(PresetTexture.Granite)]
        [TestCase(PresetTexture.GreenMarble)]
        [TestCase(PresetTexture.MediumWood)]
        [TestCase(PresetTexture.Newsprint)]
        [TestCase(PresetTexture.Oak)]
        [TestCase(PresetTexture.PaperBag)]
        [TestCase(PresetTexture.Papyrus)]
        [TestCase(PresetTexture.Parchment)]
        [TestCase(PresetTexture.PurpleMesh)]
        [TestCase(PresetTexture.RecycledPaper)]
        [TestCase(PresetTexture.Sand)]
        [TestCase(PresetTexture.Stationery)]
        [TestCase(PresetTexture.Walnut)]
        [TestCase(PresetTexture.WaterDroplets)]
        [TestCase(PresetTexture.WhiteMarble)]
        [TestCase(PresetTexture.WovenMat)]
        public void TestShapePresetTexturedDml(PresetTexture presetTexture)
        {
            Document doc = Open("NoFill", ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(doc);
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));

            fill.PresetTextured(presetTexture);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Textured));
            Assert.That(fill.PresetTexture, Is.EqualTo(presetTexture));
        }

        /// <summary>
        /// Checks determining texture from source documents.
        /// </summary>
        [TestCase("BlueTissuePaper", PresetTexture.BlueTissuePaper)]
        [TestCase("Bouquet", PresetTexture.Bouquet)]
        [TestCase("BrownMarble", PresetTexture.BrownMarble)]
        [TestCase("Canvas", PresetTexture.Canvas)]
        [TestCase("Cork", PresetTexture.Cork)]
        [TestCase("Denim", PresetTexture.Denim)]
        [TestCase("FishFossil", PresetTexture.FishFossil)]
        [TestCase("Oak", PresetTexture.Oak)]
        [TestCase("PaperBag", PresetTexture.PaperBag)]
        [TestCase("Papyrus", PresetTexture.Papyrus)]
        [TestCase("Granite", PresetTexture.Granite)]
        [TestCase("GreenMarble", PresetTexture.GreenMarble)]
        [TestCase("MediumWood", PresetTexture.MediumWood)]
        [TestCase("Newsprint", PresetTexture.Newsprint)]
        [TestCase("Parchment", PresetTexture.Parchment)]
        [TestCase("PinkTissuePaper", PresetTexture.PinkTissuePaper)]
        [TestCase("PurpleMesh", PresetTexture.PurpleMesh)]
        [TestCase("RecycledPaper", PresetTexture.RecycledPaper)]
        [TestCase("WhiteMarble", PresetTexture.WhiteMarble)]
        [TestCase("Sand", PresetTexture.Sand)]
        [TestCase("Stationery", PresetTexture.Stationery)]
        [TestCase("Walnut", PresetTexture.Walnut)]
        [TestCase("WaterDroplets", PresetTexture.WaterDroplets)]
        [TestCase("WovenMat", PresetTexture.WovenMat)]
        public void TestShapeTexture(string fileName, PresetTexture presetTexture)
        {
            Document dmlDoc = Open(fileName, ShapeMarkupLanguage.Dml);
            Fill fill = GetFill(dmlDoc);
            Assert.That(fill.FillType, Is.EqualTo(FillType.Textured));
            Assert.That(fill.PresetTexture, Is.EqualTo(presetTexture));

            Document vmlDoc = Open(fileName, ShapeMarkupLanguage.Vml);
            fill = GetFill(vmlDoc);
            Assert.That(fill.FillType, Is.EqualTo(FillType.Textured));
            Assert.That(fill.PresetTexture, Is.EqualTo(presetTexture));
        }

        /// <summary>
        /// Checks applying texture to different types of Dml fill.
        /// </summary>
        [TestCase("Solid")]
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("Patterned")]
        [TestCase("PresetTextured")]
        [TestCase("UserPicture")]
        [TestCase("UserTextured")]
        public void TestTexturedSolidFill(string fileName)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Dml);

            Shape shape = (Shape)GetFirstFillable(doc);
            Fill fill = shape.Fill;
            Assert.That(fill.Transparency, Is.EqualTo(0.37));

            fill.PresetTextured(PresetTexture.FishFossil);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Textured));
            Assert.That(fill.PresetTexture, Is.EqualTo(PresetTexture.FishFossil));
            Assert.That(fill.Transparency, Is.EqualTo(0));

            DmlBlipFill dmlBlipFill = (DmlBlipFill)shape.DmlShape.Fill;
            DmlBlipFillTile dmlBlipFillTile = (DmlBlipFillTile)dmlBlipFill.BlipFillMode;
            Assert.That(dmlBlipFillTile.Alignment, Is.EqualTo(DmlRectangleAlignment.TopLeft));
            Assert.That(dmlBlipFillTile.TileFlipMode, Is.EqualTo(DmlTileFlipMode.None));
            Assert.That(dmlBlipFillTile.HorizontalOffset, Is.EqualTo(0));
            Assert.That(dmlBlipFillTile.VerticalOffset, Is.EqualTo(0));
            Assert.That(dmlBlipFillTile.HorizontalRatio, Is.EqualTo(1.0));
            Assert.That(dmlBlipFillTile.VerticalRatio, Is.EqualTo(1.0));
        }

        /// <summary>
        /// Checks applying texture to different types of Vml fill.
        /// </summary>
        [TestCase("Solid", 255, 0, 0)]
        [TestCase("OneColorGradient", 0, 128, 128)]
        [TestCase("TwoColorGradient", 128, 0, 0)]
        [TestCase("PresetGradient", 204, 204, 255)]
        [TestCase("Patterned", 128, 0, 0)]
        [TestCase("PresetTextured", 255, 255, 255)]
        [TestCase("UserPicture", 255, 255, 255)]
        [TestCase("UserTextured", 255, 255, 255)]
        public void TestTexturedSolidFillVml(string fileName, int r, int g, int b)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Vml);

            Fill fill = GetFill(doc);

            Assert.That(fill.Transparency, Is.EqualTo(0.37).Within(0.01));
            Assert.That(fill.ForeColor, Is.EqualTo(Color.FromArgb(r, g, b)));

            fill.PresetTextured(PresetTexture.FishFossil);

            Assert.That(fill.FillType, Is.EqualTo(FillType.Textured));
            Assert.That(fill.PresetTexture, Is.EqualTo(PresetTexture.FishFossil));
            Assert.That(fill.Transparency, Is.EqualTo(0.37).Within(0.01));
            Assert.That(fill.ForeColor, Is.EqualTo(Color.FromArgb(255, 204, 153)));
        }

        /// <summary>
        /// Checks applying texture to Font fill.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Preset texture cannot be applied to Font.")]
        public void TestTextPresetTextured()
        {
            Document doc = Open("TextNoFill", ShapeMarkupLanguage.Vml);

            Fill fill = GetFill(doc);
            fill.PresetTextured(PresetTexture.FishFossil);
        }

        /// <summary>
        /// Checks obtaining texture from Font fill.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Preset texture cannot be applied to Font.")]
        public void TestTextGetPresetTexture()
        {
            Document doc = Open("TextNoFill", ShapeMarkupLanguage.Vml);

            Fill fill = GetFill(doc);
            Assert.Fail(fill.PresetTexture.ToString());
        }

        /// <summary>
        /// Checks obtaining texture from GroupShape fill.
        /// </summary>
        [TestCase(ShapeMarkupLanguage.Vml)]
        [TestCase(ShapeMarkupLanguage.Dml)]
        public void TestPresetTextureMixed(ShapeMarkupLanguage markupLanguage)
        {
            Document doc = Open("FillMixed", markupLanguage);

            GroupShape groupShape = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);
            Fill fill = groupShape.Fill;

            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));
            Assert.That(fill.PresetTexture, Is.EqualTo(PresetTexture.None));
        }

        /// <summary>
        /// Checks obtaining texture from fill which is not FillType.Textured.
        /// </summary>
        /// <remarks>
        /// Mimics MS Word behavior.
        /// </remarks>
        [TestCase("Solid")]
        [TestCase("OneColorGradient")]
        [TestCase("TwoColorGradient")]
        [TestCase("PresetGradient")]
        [TestCase("Patterned")]
        [TestCase("UserPicture")]
        [TestCase("UserTextured")]
        public void TestPresetTextureNoneAsDefault(string fileName)
        {
            Document doc = Open(fileName, ShapeMarkupLanguage.Vml);

            Fill fill = GetFill(doc);

            Assert.That(fill.PresetTexture, Is.EqualTo(PresetTexture.None));
        }

        /// <summary>
        /// Checks setting PresetTexture.None.
        /// </summary>
        /// <remarks>
        /// Mimics MS Word behavior.
        /// </remarks>
        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "PresetTexture.None cannot be applied directly.")]
        public void TestPresetTexturedMixed()
        {
            Document doc = Open("NoFill", ShapeMarkupLanguage.Vml);

            Fill fill = GetFill(doc);
            fill.PresetTextured(PresetTexture.None);
        }
    }
}
