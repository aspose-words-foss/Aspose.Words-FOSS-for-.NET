// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2012 by Andrey Noskov

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;
using Aspose.Common;
using Aspose.Images.Pal;
using Aspose.IO;
using Aspose.OpcPackaging;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Charts.Core;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Diagrams;
using Aspose.Words.Drawing.Core.Dml.Effects;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.NonVisualProperties;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Path;
using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Text;
using Aspose.Words.Drawing.Core.Dml.Transforms;
using Aspose.Words.Nrx;
using Aspose.Words.Replacing;
using Aspose.Words.RW.Dml.Reader;
using Aspose.Words.RW.Dml.Writer;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Tests.Dml;
using Aspose.Words.Tests.DocBuilder;
using Aspose.Words.Tests.Export.Docx;
using Aspose.Words.Tests.Unified;
using Aspose.Words.Themes;
using Aspose.Words.Validation;
using NUnit.Framework;

#if NETSTANDARD
using Image = SkiaSharp.SKBitmap;
#endif

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Basic DrawingML functionality tests.
    /// </summary>
    [TestFixture]
    public class TestDrawingML
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [SetUp]
        public void Init()
        {
            RandomUtil.Reset();
        }

        [Test]
        public void TestDmlPictureAlternativeText()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestDmlPictureAlternativeText.docx");
            Shape node = (Shape)doc.SelectSingleNode("//Shape[1]");
            Assert.That(node.AlternativeText, Is.EqualTo("ALTERNATIVE TEXT"));
            node.AlternativeText = "ALTERNATIVE TEXT CHANGED";

            doc = TestUtil.SaveOpen(doc, @"Model\DrawingML\TestDmlPictureAlternativeText.docx");
            node = (Shape)doc.SelectSingleNode("//Shape[1]");
            Assert.That(node.AlternativeText, Is.EqualTo("ALTERNATIVE TEXT CHANGED"));
        }





        /// <summary>
        /// WORDSNET-6958 Make sure charts are rendered upon converting to flow formats.
        /// </summary>
        // FOSS: TestJira6958 removed — it verified DrawingML image export to Doc/WordML/Rtf/Dot, all removed.




        /// <summary>
        /// WORDSNET-5608 DrawingML does not get rendered to flow formats other than DOCX.
        /// The problem occurred because DocumentValidator skipped rendering lockedCanvas DrawingMLs.
        /// Fixed by adding the appropriate condition.
        /// </summary>
        // FOSS: TestJira5083 removed — it verified DrawingML renders to non-DOCX flow formats (Doc/WordML/Rtf/Dot), all removed.


        /// <summary>
        /// WORDSNET-9666 Linked image does not update in output documents.
        /// The problem occurs upon converting DrawingML to Shape.
        /// Fixed by checking if shape has both embedded and linked images set, we have to try loading linked image first.
        /// </summary>
        // FOSS: TestJira9666 removed — it verified linked-image update on DrawingML export to WordML/Rtf, both removed.

        /// <summary>
        /// WORDSNET-8911Implement interface IShapeAttrSource in DrawingML.
        /// Test FetchInheritedShapeAttr method works correctly for DML/VML shapes.
        /// </summary>
        [Test]
        public void TestJira8911()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestJira8911.docx");

            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Shape drawingMl = (Shape)doc.GetChild(NodeType.Shape, 1, true);

            Assert.That(drawingMl.FetchInheritedShapeAttrInternal(ShapeAttr.LineDashStyle), Is.EqualTo(shape.FetchInheritedShapeAttrInternal(ShapeAttr.LineDashStyle)));
            Assert.That(drawingMl.FetchInheritedShapeAttrInternal(ShapeAttr.LineDashData), Is.EqualTo(shape.FetchInheritedShapeAttrInternal(ShapeAttr.LineDashData)));
            Assert.That(drawingMl.FetchInheritedShapeAttrInternal(ShapeAttr.LineStyle), Is.EqualTo(shape.FetchInheritedShapeAttrInternal(ShapeAttr.LineStyle)));
            Assert.That(drawingMl.FetchInheritedShapeAttrInternal(ShapeAttr.LineEndCapStyle), Is.EqualTo(shape.FetchInheritedShapeAttrInternal(ShapeAttr.LineEndCapStyle)));
            Assert.That(drawingMl.FetchInheritedShapeAttrInternal(ShapeAttr.LineStartArrow), Is.EqualTo(shape.FetchInheritedShapeAttrInternal(ShapeAttr.LineStartArrow)));
            Assert.That(drawingMl.FetchInheritedShapeAttrInternal(ShapeAttr.LineStartArrowLength), Is.EqualTo(shape.FetchInheritedShapeAttrInternal(ShapeAttr.LineStartArrowLength)));
            Assert.That(drawingMl.FetchInheritedShapeAttrInternal(ShapeAttr.LineStartArrowWidth), Is.EqualTo(shape.FetchInheritedShapeAttrInternal(ShapeAttr.LineStartArrowWidth)));
            Assert.That(drawingMl.FetchInheritedShapeAttrInternal(ShapeAttr.LineEndArrow), Is.EqualTo(shape.FetchInheritedShapeAttrInternal(ShapeAttr.LineEndArrow)));
            Assert.That(drawingMl.FetchInheritedShapeAttrInternal(ShapeAttr.LineEndArrowLength), Is.EqualTo(shape.FetchInheritedShapeAttrInternal(ShapeAttr.LineEndArrowLength)));
            Assert.That(drawingMl.FetchInheritedShapeAttrInternal(ShapeAttr.LineEndArrowWidth), Is.EqualTo(shape.FetchInheritedShapeAttrInternal(ShapeAttr.LineEndArrowWidth)));
            Assert.That(drawingMl.FetchInheritedShapeAttrInternal(ShapeAttr.LineInsetPen), Is.EqualTo(shape.FetchInheritedShapeAttrInternal(ShapeAttr.LineInsetPen)));

            // It seems default LineWidth is different for DML and VML shapes.
            // 9525EMUs = 0.75pt, 12700Emus = 1pt.
            Assert.That(shape.FetchInheritedShapeAttrInternal(ShapeAttr.LineWidth), Is.EqualTo(9525));
            Assert.That(drawingMl.FetchInheritedShapeAttrInternal(ShapeAttr.LineWidth), Is.EqualTo(9525));

            // LineWidth is specified directly inside shape that is why use FetchShapeAttr.
            Assert.That(drawingMl.FetchInheritedShapeAttrInternal(ShapeAttr.LineWidth), Is.EqualTo(shape.FetchShapeAttrInternal(ShapeAttr.LineWidth)));
        }




        [Test]
        public void TestDmlShapeType()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestDmlShapeType.docx");
            Node[] shapes = doc.GetChildNodes(NodeType.Shape, true).ToArray();

            foreach (Node shapeNode in shapes)
            {
                // JAVA-added explicit casting to Shape.
                Shape shape = (Shape)shapeNode;

                if (shape.FallbackShape != null)
                {
                    if (DmlEnum.HasVmlPresetGeometry(shape.ShapeType))
                        Assert.That(shape.ShapeType, Is.EqualTo(shape.FallbackShape.ShapeType));

                    Assert.That(shape.IsImage, Is.False);
                }
                else
                {
                    Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.Image));
                    Assert.That(shape.IsImage, Is.True);
                }

                Assert.That(shape.IsHorizontalRule, Is.False);
                Assert.That(shape.IsHorizontalRule, Is.False);
                Assert.That(shape.IsTopLevel, Is.True);
            }
        }



        [Test]
        public void TestDmlIsGroup()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestDmlIsGroup.docx");

            Node[] groupShapes = doc.GetChildNodes(NodeType.GroupShape, true).ToArray();

            Assert.That(groupShapes.Length, Is.EqualTo(3));

            GroupShape wpGroupShape = (GroupShape)groupShapes[0];
            Assert.That(wpGroupShape.ShapeType, Is.EqualTo(ShapeType.Group));
            Assert.That(wpGroupShape.IsGroup, Is.True);
            Assert.That(wpGroupShape.IsTopLevel, Is.True);
            Assert.That(wpGroupShape.NodeType, Is.EqualTo(NodeType.GroupShape));
            Assert.That(wpGroupShape.DmlNode.DmlNodeType, Is.EqualTo(DmlNodeType.WordprocessingGroupShape));

            wpGroupShape = (GroupShape)groupShapes[1];
            Assert.That(wpGroupShape.ShapeType, Is.EqualTo(ShapeType.Group));
            Assert.That(wpGroupShape.IsTopLevel, Is.True);
            Assert.That(wpGroupShape.IsGroup, Is.True);
            Assert.That(wpGroupShape.NodeType, Is.EqualTo(NodeType.GroupShape));
            Assert.That(wpGroupShape.DmlNode.DmlNodeType, Is.EqualTo(DmlNodeType.WordprocessingCanvas));

            Node[] shapes = doc.GetChildNodes(NodeType.Shape, true).ToArray();
            Assert.That(shapes.Length, Is.EqualTo(6));

            Shape shape = (Shape)shapes[0];
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.TextBox));

            //This shape is inside group.
            Assert.That(shape.IsTopLevel, Is.False);

            Assert.That(shape.IsGroup, Is.False);
            Assert.That(shape.NodeType, Is.EqualTo(NodeType.Shape));
            Assert.That(shape.DmlNode.DmlNodeType, Is.EqualTo(DmlNodeType.WordprocessingShape));

            shapes = doc.GetChildNodes(NodeType.Shape, false).ToArray();
            Assert.That(shapes.Length, Is.EqualTo(0));
        }

        [Test]
        public void TestDmlCoordSize()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestDmlIsGroup.docx");

            GroupShape groupShape = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);

            Assert.That(groupShape.ShapeType, Is.EqualTo(ShapeType.Group));
            Assert.That(groupShape.Width, Is.EqualTo(132).Within(0.1));
            Assert.That(groupShape.Height, Is.EqualTo(63).Within(0.1));

            Assert.That(groupShape.CoordSizeWidth, Is.EqualTo(1152525));
            Assert.That(groupShape.CoordSizeHeight, Is.EqualTo(581025));

            // Set double CoordSize.
            groupShape.CoordSize = new Size(groupShape.CoordSizeWidth * 2, groupShape.CoordSizeHeight * 2);

            doc = TestUtil.SaveOpen(doc, @"Model\DrawingML\TestDmlCoordSize.docx");

            groupShape = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);

            Assert.That(groupShape.Width, Is.EqualTo(132).Within(0.1));
            Assert.That(groupShape.Height, Is.EqualTo(63).Within(0.1));

            Assert.That(groupShape.CoordSizeWidth, Is.EqualTo(2305050));
            Assert.That(groupShape.CoordSizeHeight, Is.EqualTo(1162050));
        }

        [Test]
        public void TestDmlCoordOrigin()
        {
            const string fileName = @"Model\DrawingML\TestDmlIsGroup.docx";
            Document doc = TestUtil.Open(fileName);

            GroupShape groupShape = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);

            Assert.That(groupShape.ShapeType, Is.EqualTo(ShapeType.Group));
            Assert.That(groupShape.Width, Is.EqualTo(132).Within(0.1));
            Assert.That(groupShape.Height, Is.EqualTo(63).Within(0.1));

            Assert.That(groupShape.CoordOrigin.X, Is.EqualTo(0));
            Assert.That(groupShape.CoordOrigin.Y, Is.EqualTo(0));

            // Set half CoordSize as CoordOrigin.
            groupShape.CoordOrigin = new Point(groupShape.CoordSizeWidth / 2, groupShape.CoordSizeHeight / 2);

            doc = TestUtil.SaveOpen(doc, @"Model\DrawingML\TestDmlCoordOrigin.docx");

            groupShape = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);

            Assert.That(groupShape.Width, Is.EqualTo(132).Within(0.1));
            Assert.That(groupShape.Height, Is.EqualTo(63).Within(0.1));

            Assert.That(groupShape.CoordOriginX, Is.EqualTo(groupShape.CoordSizeWidth / 2));
            Assert.That(groupShape.CoordOriginY, Is.EqualTo(groupShape.CoordSizeHeight / 2));
        }

        [Test]
        public void TestDmlFlipOrientation()
        {
            const string fileName = @"Model\DrawingML\TestDmlFlipOrientation.docx";
            Document doc = TestUtil.Open(fileName);

            Node[] shapes = doc.GetChildNodes(NodeType.Shape, true).ToArray();
            ((Shape)shapes[0]).FlipOrientation = FlipOrientation.Vertical;
            ((Shape)shapes[1]).FlipOrientation = FlipOrientation.Both;
            ((Shape)shapes[2]).FlipOrientation = FlipOrientation.Vertical;
            ((Shape)shapes[4]).FlipOrientation = FlipOrientation.Horizontal;

            doc = TestUtil.SaveOpen(doc, @"Model\DrawingML\TestDmlFlipOrientation.docx");

            shapes = doc.GetChildNodes(NodeType.Shape, true).ToArray();
            Assert.That(((Shape)shapes[0]).FlipOrientation, Is.EqualTo(FlipOrientation.Vertical));
            Assert.That(((Shape)shapes[1]).FlipOrientation, Is.EqualTo(FlipOrientation.Both));
            Assert.That(((Shape)shapes[2]).FlipOrientation, Is.EqualTo(FlipOrientation.Vertical));
            Assert.That(((Shape)shapes[3]).FlipOrientation, Is.EqualTo(FlipOrientation.None));
            Assert.That(((Shape)shapes[4]).FlipOrientation, Is.EqualTo(FlipOrientation.Horizontal));
        }

        /// <summary>
        /// Test how Rotation works for Dml shape.
        /// </summary>
        [Test]
        public void TestDmlRotation()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestDmlFlipOrientation.docx");

            Node[] shapes = doc.GetChildNodes(NodeType.Shape, true).ToArray();

            ((Shape)shapes[0]).Rotation = 30;
            ((Shape)shapes[1]).Rotation = 50;
            ((Shape)shapes[2]).Rotation = 70;
            ((Shape)shapes[4]).Rotation = 90;

            doc = TestUtil.SaveOpen(doc, @"Model\DrawingML\TestDmlRotation.docx");

            shapes = doc.GetChildNodes(NodeType.Shape, true).ToArray();
            Assert.That(((Shape)shapes[0]).Rotation, Is.EqualTo(30));
            Assert.That(((Shape)shapes[1]).Rotation, Is.EqualTo(50));
            Assert.That(((Shape)shapes[2]).Rotation, Is.EqualTo(70));
            Assert.That(((Shape)shapes[3]).Rotation, Is.EqualTo(0));
            Assert.That(((Shape)shapes[4]).Rotation, Is.EqualTo(90));
        }

        /// <summary>
        /// Test ShadowEnabled and ExtrusionEnabled properties.
        /// </summary>
        [Test]
        public void TestDmlShadow()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestDmlShadow.docx");
            Node[] shapes = doc.GetChildNodes(NodeType.Shape, true).ToArray();

            Assert.That(((Shape)shapes[0]).ShadowEnabled, Is.False);
            Assert.That(((Shape)shapes[0]).ExtrusionEnabled, Is.True);

            Assert.That(((Shape)shapes[1]).ExtrusionEnabled, Is.False);
            Assert.That(((Shape)shapes[1]).ShadowEnabled, Is.False);

            Assert.That(((Shape)shapes[2]).ExtrusionEnabled, Is.True);

            Assert.That(((Shape)shapes[3]).ExtrusionEnabled, Is.True);

            Assert.That(((Shape)shapes[4]).ExtrusionEnabled, Is.True);
            Assert.That(((Shape)shapes[4]).ShadowEnabled, Is.True);
        }

        /// <summary>
        /// WORDSNET-10837 Test Dml shape fill Color, Opacity and On properties.
        /// </summary>
        [Test]
        public void TestDmlShapeFillColor()
        {
            const string pathName = @"Model\DrawingML\TestDmlShapeFillColor.docx";
            Document sourceDoc = TestUtil.Open(pathName);
            Shape dmlShape = (Shape)sourceDoc.GetChild(NodeType.Shape, 0, true);
            Fill fill = dmlShape.Fill;

            Assert.That(fill.Visible, Is.True);
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid));

            int expected = unchecked((int)0x8000b050);
            Assert.That(fill.Color.ToArgb(), Is.EqualTo(expected));
            Assert.That(dmlShape.FillColor.ToArgb(), Is.EqualTo(expected));

            Assert.That(fill.Opacity, Is.EqualTo(0.5).Within(0.01));

            // Set other solid color.
            fill.ForeColor = Color.Red;

            Assert.That(fill.ForeColor.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            Assert.That(fill.Opacity, Is.EqualTo(1.0).Within(0.01));

            // Set other opacity.
            fill.Opacity = 0.7;

            Assert.That(fill.Opacity, Is.EqualTo(0.7).Within(0.01));
            Assert.That(fill.Visible, Is.True);
            Assert.That(dmlShape.Filled, Is.True);

            Document resultDoc = TestUtil.SaveOpen(sourceDoc, pathName);
            dmlShape = (Shape)resultDoc.GetChild(NodeType.Shape, 0, true);

            Assert.That(dmlShape.Filled, Is.True);
            expected = unchecked((int)0xb2ff0000);
            Assert.That(dmlShape.FillColor.ToArgb(), Is.EqualTo(expected));
            Assert.That(fill.Opacity, Is.EqualTo(0.7).Within(0.01));
        }

        /// <summary>
        /// WORDSNET-12672 On loading the normAutofit element, fontScale="92500" attribute is not loaded
        /// correctly: the font scale becomes 0% that is not allowed in MS Word.
        /// </summary>
        [Test]
        public void TestJira12672()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestJira12672.docx");
            DmlTextBody shapeTextBody = GetChildShapeTextBody(doc, 0);
            DmlNormalAutoFitMode autoFit = (DmlNormalAutoFitMode)shapeTextBody.Properties.AutoFitMode;

            Assert.That(autoFit.FontScale, Is.EqualTo(0.925), "Loaded font scale value is wrong.");
        }

        [Test]
        public void TestDmlShapePatternFill()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestDmlShapeFillColor.docx");
            Shape dmlShape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Fill fill = dmlShape.Fill;
            Assert.That(fill.Visible, Is.True);
            Assert.That(fill.FillType, Is.EqualTo(FillType.Patterned));

            Assert.That(fill.ForeColor.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
            Assert.That(fill.BackColor.ToArgb(), Is.EqualTo(Color.Yellow.ToArgb()));

            // Set other colors.
            fill.ForeColor = Color.Green;
            fill.BackColor = Color.Black;

            Assert.That(fill.ForeColor.ToArgb(), Is.EqualTo(Color.Green.ToArgb()));
            Assert.That(fill.BackColor.ToArgb(), Is.EqualTo(Color.Black.ToArgb()));

            // Set other opacity.
            fill.Opacity = 0.3;
            dmlShape.FillCore.Opacity2 = 0.4;

            Assert.That(fill.Opacity, Is.EqualTo(0.3).Within(0.01));
            Assert.That(dmlShape.FillCore.Opacity2, Is.EqualTo(0.4).Within(0.01));
        }

        [Test]
        public void TestDmlShapeBlipFill()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestDmlShapeFillColor.docx");
            Shape dmlShape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            Fill fill = dmlShape.Fill;
            Assert.That(fill.Visible, Is.True);
            Assert.That(fill.FillType, Is.EqualTo(FillType.Picture));

            Assert.That(fill.ImageBytes.Length, Is.EqualTo(9509));
            byte[] imageBytes = StreamUtil.CopyFileToByteArray(TestUtil.BuildTestFileName(@"DocBuilder\Image\TestPng.png"));

            // Set new blip fill image.
            dmlShape.FillCore.SetImageBytes(imageBytes);
            Assert.That(fill.ImageBytes.Length, Is.EqualTo(29414));

            fill.Visible = false;
            Assert.That(fill.FillType, Is.EqualTo(FillType.Solid)); // We do not have NoFill type in Vml.
        }

        [Test]
        public void TestDmlOutlineClone()
        {
            const string pathName = @"Model\DrawingML\TestDmlOutline.docx";
            Document sourceDoc = TestUtil.Open(pathName);

            NodeCollection shapes = sourceDoc.GetChildNodes(NodeType.Shape, true);

            Shape shape0 = (Shape)shapes[0];
            Stroke beforeSave = shape0.Stroke;

            foreach (Shape shape in shapes)
            {
                Stroke stroke = shape.Stroke;
                Shape clonedShape = (Shape)shape.Clone(true);
                Stroke clonedStroke = clonedShape.Stroke;

                // Compare stroke params after cloning shape.
                CompareStroke(stroke, clonedStroke);
            }

            Document resultDoc = TestUtil.SaveOpen(sourceDoc, pathName);
            shapes = resultDoc.GetChildNodes(NodeType.Shape, true);
            shape0 = (Shape)shapes[0];

            Stroke afterSave = shape0.Stroke;

            // Compare stroke params after save.
            CompareStroke(beforeSave, afterSave);
        }

        [Test]
        public void TestDmlOutlineOn()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestDmlOutline.docx");

            NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);

            Shape shape0 = (Shape)shapes[0];
            Assert.That(shape0.Stroke.On, Is.True);
            shape0.Stroke.On = false;

            Shape shape1 = (Shape)shapes[1];
            Assert.That(shape1.Stroke.On, Is.True);
            shape1.Stroke.On = false;

            Shape shape2 = (Shape)shapes[2];
            Assert.That(shape2.Stroke.On, Is.True);
            shape2.Stroke.On = false;

            Shape shape3 = (Shape)shapes[3];
            Assert.That(shape3.Stroke.On, Is.False);
            shape3.Stroke.On = true;

            TestUtil.SaveOpen(doc, @"Model\DrawingML\TestDmlOutlineOn.docx");
        }

        [Test]
        public void TestDmlOutlineParams()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestDmlOutline.docx");

            NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);

            foreach (Shape shape in shapes)
            {
                Stroke stroke = shape.Stroke;
                stroke.On = true;
                stroke.Weight = 5;
                stroke.Color = Color.Red;
                stroke.Color2 = Color.Blue;
                stroke.DashStyle = DashStyle.ShortDashDotDot;
                stroke.Opacity = 0.3;
                stroke.JoinStyle = JoinStyle.Miter;
                stroke.EndCap = EndCap.Square;
                stroke.LineStyle = ShapeLineStyle.Triple;

                stroke.StartArrowType = ArrowType.Diamond;
                stroke.EndArrowType = ArrowType.Diamond;

                stroke.StartArrowWidth = ArrowWidth.Wide;
                stroke.EndArrowWidth = ArrowWidth.Narrow;

                stroke.StartArrowLength = ArrowLength.Long;
                stroke.EndArrowLength = ArrowLength.Short;
            }

            TestUtil.SaveOpen(doc, @"Model\DrawingML\TestDmlOutlineParams.docx");
        }

        /// <summary>
        /// WORDSNET-10987 Expose Font property for DrawingML same as Shape.Font.
        /// </summary>
        [Test]
        public void TestJira10987()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestJira10987.docx");

            // Get Dml shape and check its font attrs.
            Shape dml = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Font dmlFont = dml.Font;
            Assert.That(dmlFont.Parent is IShapeAttrSource, Is.True);
            Assert.That(dmlFont.Name, Is.EqualTo("Blackoak Std"));
            Assert.That(dmlFont.Color.ToArgb(), Is.EqualTo(-16732080));
            Assert.That(dmlFont.Bold, Is.True);
            Assert.That(dmlFont.Underline, Is.EqualTo(Underline.Single));

            // Get Vml shape and check its font attrs.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Font shapeFont = shape.Font;
            Assert.That(shapeFont.Name, Is.EqualTo("Adobe Fan Heiti Std B"));
            Assert.That(shapeFont.Color.ToArgb(), Is.EqualTo(-16732080));
            Assert.That(shapeFont.Bold, Is.True);
            Assert.That(shapeFont.Underline, Is.EqualTo(Underline.Single));
        }





        /// <summary>
        /// WORDSNET-11673 Stroke.Color throws System.NullReferenceException.
        /// andrnosk: The problem occurs because we don't have default DmlOutline fill.
        /// Fixed by specifying DmlNoFill as default DmlOutline fill. <see cref="DmlOutlinePropertiesDefaults"/>
        /// </summary>
        [Test]
        public void TestJira11673()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertImage(TestUtil.BuildTestFileName(@"DocBuilder\Image\TestPng.png"));
            Stroke stroke = shape.Stroke;
            stroke.On = true;
            stroke.Color = Color.Green;

            doc = TestUtil.SaveOpen(doc, @"Model\DrawingML\TestJira11673.docx", UnifiedScenario.Docx2DocxNoGold);
            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.Stroke.Color.ToArgb(), Is.EqualTo(Color.Green.ToArgb()));
        }



        /// <summary>
        /// WORDSNET-11789 Crop of images seems to be broken since 15.2.
        /// andrnosk: The problem occurs because starting from 15.2 AW inserts images as Dml.
        /// Fixed by creating <see cref="VmlToDmlAttrUtil"/> class to correctly process ImageData attrs.
        /// </summary>
        [Test]
        public void TestJira11789()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertParagraph();

            Shape shape = builder.InsertImage(TestUtil.BuildTestFileName(@"DocBuilder\Image\TestJira11789.png"));
            ImageData imageData = shape.ImageData;

            Assert.That(imageData.CropTop, Is.EqualTo(0));
            Assert.That(imageData.CropBottom, Is.EqualTo(0));
            Assert.That(imageData.CropLeft, Is.EqualTo(0));
            Assert.That(imageData.CropRight, Is.EqualTo(0));

            imageData.CropTop = 0.2;
            imageData.CropBottom = 0.2;
            imageData.CropLeft = 0.2;
            imageData.CropRight = 0.2;

            doc = TestUtil.SaveOpen(doc, @"Model\DrawingML\TestJira11789.docx");
            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            imageData = shape.ImageData;

            Assert.That(imageData.CropTop, Is.EqualTo(0.2));
            Assert.That(imageData.CropBottom, Is.EqualTo(0.2));
            Assert.That(imageData.CropLeft, Is.EqualTo(0.2));
            Assert.That(imageData.CropRight, Is.EqualTo(0.2));
        }

        /// <summary>
        /// WORDSNET-11864 Shape fill color is not set correctly using Color.FromArgb
        /// andrnosk: The problem occurs because of converting System.Color to DmlPercentageRgbColor,
        /// fixed by converting System.Color to DmlHexRgbColor.
        /// </summary>
        [Test]
        public void TestJira11864()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            Shape shape = builder.InsertImage(TestUtil.BuildTestFileName(@"DocBuilder\Image\TestPng.png"));
            Color color = Color.FromArgb(102, 29, 98);
            shape.Fill.Color = color;

            doc = TestUtil.SaveOpen(doc, @"Model\DrawingML\TestJira11864.docx");

            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            Assert.That(shape.Fill.Color, Is.EqualTo(color));
        }

        /// <summary>
        /// WORDSNET-11856 Shape.Name returns empty string for shapes inside GroupShape.
        /// andrnosk: The problem occurs because inside DML shape located in group, Name and Description stored
        /// in wps:cNvPr (Non-Visual Drawing Properties) instead of wp:docPr.
        /// </summary>
        [Test]
        public void TestJira11856()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestJira11856.docx");

            GroupShape groupShape = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);

            Assert.That(groupShape.Name, Is.EqualTo("Group 2"));
            Assert.That(groupShape.AlternativeText, Is.EqualTo("Main Group"));

            Node[] shapes = doc.GetChildNodes(NodeType.Shape, true).ToArray();

            Shape shape1 = (Shape)shapes[0];
            Shape shape2 = (Shape)shapes[1];

            Assert.That(shape1.Name, Is.EqualTo("Pie 1"));
            Assert.That(shape1.AlternativeText, Is.EqualTo("Drawing Shape"));

            Assert.That(shape2.Name, Is.EqualTo("Text Box 2"));
            Assert.That(shape2.AlternativeText, Is.EqualTo("Text Box"));

            groupShape.Name = "NewGroupName";
            groupShape.AlternativeText = "NewGroupAlternativeText";

            int i = 0;
            foreach (Node shapeNode in shapes)
            {
                Shape shape = (Shape)shapeNode;
                shape.Name = string.Format("NewName{0}", i++);
                shape.AlternativeText = string.Format("NewAlternativeText{0}", i++);
            }

            doc = TestUtil.SaveOpen(doc, @"Model\DrawingML\TestJira11856.docx", UnifiedScenario.Docm2DocmNoGold);

            groupShape = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);

            Assert.That(groupShape.Name, Is.EqualTo("NewGroupName"));
            Assert.That(groupShape.AlternativeText, Is.EqualTo("NewGroupAlternativeText"));

            shapes = doc.GetChildNodes(NodeType.Shape, true).ToArray();

            shape1 = (Shape)shapes[0];
            shape2 = (Shape)shapes[1];

            Assert.That(shape1.Name, Is.EqualTo("NewName0"));
            Assert.That(shape1.AlternativeText, Is.EqualTo("NewAlternativeText1"));

            Assert.That(shape2.Name, Is.EqualTo("NewName2"));
            Assert.That(shape2.AlternativeText, Is.EqualTo("NewAlternativeText3"));
        }

        /// <summary>
        /// WORDSNET-11777 Image is rotated if we convert DOCX to other formats.
        /// andrnosk: The problem occurs because VML does not support 3D effects, so to correctly convert this document to WML
        /// or any other formats we have to render this shape.
        /// </summary>
        // FOSS: TestJira11777 removed — it converted a 3D DML shape to WordML by rendering it to a shape; WordML save
        // and shape rendering are both removed.


        /// <summary>
        /// WORDSNET-12083 Hyperlinks of GroupShape are lost after conversion from Docx to Doc/Pdf
        /// Copy hyperlink from DML when it is replaced by Fallback.
        /// </summary>
        // FOSS: TestJira12083 removed — it relied on the DML->VML Fallback path (triggered by validating to a
        // fixed/non-DML format) to copy a group-shape hyperlink; that fallback/rendering path is removed.

        /// <summary>
        /// WORDSNET-12025 ImageData.GrayScale not taking any effect.
        /// The problem occurs because DmlShapePrFiller and VmlToDmlAttrUtil do not procces Dml shape GrayScale attr.
        /// </summary>
        [Test]
        public void TestJira12025()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertImage(TestUtil.BuildTestFileName(@"DocBuilder\Image\TestPng.png"));

            Assert.That(shape.ImageData.GrayScale, Is.False);

            shape.ImageData.GrayScale = true;
            doc = TestUtil.SaveOpen(doc, @"Model\DrawingML\TestJira12025", UnifiedScenario.Docx2DocxNoGold);
            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            Assert.That(shape.ImageData.GrayScale, Is.True);

            shape.ImageData.GrayScale = false;
            Assert.That(shape.ImageData.GrayScale, Is.False);
        }

        [Test]
        public void TestLuminanceEffect()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertImage(TestUtil.BuildTestFileName(@"DocBuilder\Image\TestPng.png"));
            ImageData imageData = shape.ImageData;

            Assert.That(imageData.Brightness, Is.EqualTo(0.5));
            Assert.That(imageData.Contrast, Is.EqualTo(0.5));

            imageData.Brightness = 0.9;
            imageData.Contrast = 0.9;

            Assert.That(imageData.Brightness, Is.EqualTo(0.9).Within(0.01));
            Assert.That(imageData.Contrast, Is.EqualTo(0.9).Within(0.01));

            doc = TestUtil.SaveOpen(doc, @"Model\DrawingML\TestLuminanceEffect", UnifiedScenario.Docx2DocxNoGold);
            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            imageData = shape.ImageData;

            Assert.That(imageData.Brightness, Is.EqualTo(0.9).Within(0.01));
            Assert.That(imageData.Contrast, Is.EqualTo(0.9).Within(0.01));

            imageData.Brightness = 0.1;
            imageData.Contrast = 0.1;

            Assert.That(imageData.Brightness, Is.EqualTo(0.1).Within(0.01));
            Assert.That(imageData.Contrast, Is.EqualTo(0.1).Within(0.01));
        }

        [Test]
        public void TestBiLevelEffect()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            Shape shape = builder.InsertImage(TestUtil.BuildTestFileName(@"DocBuilder\Image\TestPng.png"));

            Assert.That(shape.ImageData.BiLevel, Is.False);

            shape.ImageData.BiLevel = true;
            doc = TestUtil.SaveOpen(doc, @"Model\DrawingML\TestBiLevelEffect", UnifiedScenario.Docx2DocxNoGold);
            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            Assert.That(shape.ImageData.BiLevel, Is.True);

            shape.ImageData.BiLevel = false;
            Assert.That(shape.ImageData.BiLevel, Is.False);
        }



        /// <summary>
        /// WORDSNET-12752 Unable to open cloned document in MS Office.
        /// The problem occurs because there is grpSp (Group shape) with cNvPr (Non-Visual Drawing Properties) without Id.
        /// The input doc does not have cNvPr inside grpSp at all. Fixed mechanism of writing cNvPr.
        /// </summary>
        [Test]
        public void TestJira12752()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestJira12752.docx");
            Document docClone = (Document)doc.Clone(true);
            doc = TestUtil.SaveOpen(docClone, @"Model\DrawingML\TestJira12752", UnifiedScenario.Docx2DocxNoGold);
            GroupShape groupShape = (GroupShape)doc.GetChild(NodeType.GroupShape, 1, true);

            Assert.That(groupShape.DmlNode.NonVisualPr.NvDrawingProperties, Is.Null);
        }

        // FOSS: TestJira12918 removed — it verified the older-format (WordML) merge of Title+AltText into
        // "Title: {0} - Description: {1}"; DOCX keeps Title and AlternativeText as separate native fields,
        // and the WordML save that produced the merge is removed.




        /// <summary>
        /// WORDSNET-13508 Document.Save throws System.NullReferenceException while re-saving Docx.
        /// Some effects implementations have not default values for properties, but the specification describes it.
        /// Attempt to access to properties members of the effect failed with exception because properties values
        /// are not initialized.
        /// </summary>
        [Test]
        public void TestJira13508()
        {
            // Check that optional properties with reference type and default values are initialized.
            DmlShapePresetShadowEffect ePS = new DmlShapePresetShadowEffect();
            Assert.That(ePS.Direction.Value, Is.EqualTo(0.0d));

            DmlShapeReflectionEffect eR = new DmlShapeReflectionEffect();
            Assert.That(eR.VerticalSkew.Value, Is.EqualTo(0.0d));
            Assert.That(eR.HorizontalSkew.Value, Is.EqualTo(0.0d));
            Assert.That(eR.Direction.Value, Is.EqualTo(0.0d));
            Assert.That(eR.FadeDirection.Value, Is.EqualTo(5400000.0d));

            DmlShapeOuterShadowEffect eOS = new DmlShapeOuterShadowEffect();
            Assert.That(eOS.Direction.Value, Is.EqualTo(0.0d));
            Assert.That(eOS.VerticalSkew.Value, Is.EqualTo(0.0d));
            Assert.That(eOS.HorizontalSkew.Value, Is.EqualTo(0.0d));

            DmlShapeInnerShadowEffect eIS = new DmlShapeInnerShadowEffect();
            Assert.That(eIS.Direction.Value, Is.EqualTo(0.0d));

            DmlShapeFillOverlayEffect eFO = new DmlShapeFillOverlayEffect();
            Assert.That(eFO.FillOverlay, IsNot.Null());
        }


        // FOSS: TestJira13314 removed — it converted DOCX->DOC to force DML->VML conversion in the shape
        // validator and checked the resulting warning; DOC save and that conversion path are removed.

        // FOSS: TestJira13423 removed — it loads a binary .doc (removed) and asserts all shapes are VML, then
        // checks VML<->DML markup across Word-version optimization (needs the removed DML<->VML conversion).


        private void CompareStroke(Stroke stroke, Stroke strokeToCompare)
        {
            Assert.That(strokeToCompare, IsNot.SameAs(stroke));

            Assert.That(strokeToCompare.On, Is.EqualTo(stroke.On));
            Assert.That(strokeToCompare.Weight, Is.EqualTo(stroke.Weight));
            Assert.That(strokeToCompare.Color, Is.EqualTo(stroke.Color));
            Assert.That(strokeToCompare.Color2, Is.EqualTo(stroke.Color2));
            Assert.That(strokeToCompare.DashStyle, Is.EqualTo(stroke.DashStyle));
            Assert.That(strokeToCompare.JoinStyle, Is.EqualTo(stroke.JoinStyle));
            Assert.That(strokeToCompare.EndCap, Is.EqualTo(stroke.EndCap));
            Assert.That(strokeToCompare.LineStyle, Is.EqualTo(stroke.LineStyle));
            Assert.That(strokeToCompare.StartArrowType, Is.EqualTo(stroke.StartArrowType));
            Assert.That(strokeToCompare.EndArrowType, Is.EqualTo(stroke.EndArrowType));
            Assert.That(strokeToCompare.StartArrowWidth, Is.EqualTo(stroke.StartArrowWidth));
            Assert.That(strokeToCompare.StartArrowLength, Is.EqualTo(stroke.StartArrowLength));
            Assert.That(strokeToCompare.EndArrowWidth, Is.EqualTo(stroke.EndArrowWidth));
            Assert.That(strokeToCompare.EndArrowLength, Is.EqualTo(stroke.EndArrowLength));
            Assert.That(strokeToCompare.Opacity, Is.EqualTo(stroke.Opacity));
            Assert.That(strokeToCompare.ImageBytes, Is.EqualTo(stroke.ImageBytes));
        }


        /// <summary>
        /// WORDSNET-14145 Unsupported DML element cxnLst.
        /// Implement reading of the connection sites to the model and writing to output file.
        /// </summary>
        [Test]
        public void TestJira14145()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\DrawingML\Shapes\TestJira14145", UnifiedScenario.Docx2DocxNoGold);
            GroupShape grShape = (GroupShape)doc.FirstSection.Body.FirstParagraph.GetChild(NodeType.GroupShape, 0, false);
            DmlShape shape = (DmlShape)((Shape)grShape.FirstChild).DmlNode;

            IList<DmlConnectionSite> sites = shape.Geometry.ConnectionSites;
            Assert.That(sites.Count, Is.EqualTo(4));

            DmlConnectionSite site = sites[0];
            Assert.That(site.Angle.String, Is.EqualTo("myAngle"));
            Assert.That(site.Coordinates.X.String, Is.EqualTo("myXGuide"));
            Assert.That(site.Coordinates.Y.String, Is.EqualTo("507786"));

            site = sites[1];
            Assert.That(site.Angle.String, Is.EqualTo("16200000"));
            Assert.That(site.Coordinates.X.String, Is.EqualTo("connsiteX0"));
            Assert.That(site.Coordinates.Y.String, Is.EqualTo("connsiteY0"));

            site = sites[2];
            Assert.That(site.Angle.String, Is.EqualTo("0"));
            Assert.That(site.Coordinates.X.String, Is.EqualTo("connsiteX1"));
            Assert.That(site.Coordinates.Y.String, Is.EqualTo("connsiteY1"));

            site = sites[3];
            Assert.That(site.Angle.String, Is.EqualTo("0"));
            Assert.That(site.Coordinates.X.String, Is.EqualTo("connsiteX2"));
            Assert.That(site.Coordinates.Y.String, Is.EqualTo("connsiteY2"));
        }

        /// <summary>
        /// WORDSNET-14146 Unsupported attributes of the path element (DML).
        /// The extrusionOk (3D Extrusion Allowed) and fill (Path Fill) attributes of the
        /// 20.1.9.15 path (Shape Path) element are lost after re-saving a document.
        /// </summary>
        [Test]
        public void TestJira14146()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\DrawingML\Shapes\TestJira14146", UnifiedScenario.Docx2DocxNoGold);
            Paragraph par = doc.FirstSection.Body.FirstParagraph;

            DmlShape fShape = (DmlShape)((Shape)par.GetChild(NodeType.Shape, 0, false)).DmlNode;
            DmlShape sShape = (DmlShape)((Shape)par.GetChild(NodeType.Shape, 1, false)).DmlNode;

            DmlPath path = fShape.Geometry.Paths[0];
            Assert.That(path.ExtrusionOk, Is.False);

            path = sShape.Geometry.Paths[0];
            Assert.That(path.ExtrusionOk, Is.True);
        }

        /// <summary>
        /// WORDSNET-14147 Unsupported elements of chart shape properties.
        /// Currently does not support the following subelements of the 21.2.2.197 spPr (Shape Properties) element:
        /// scene3d, sp3d, extLst (e.g. hiddenFill extension).
        /// </summary
        [Test]
        public void TestJira14147()
        {
            const string path = @"Model\DrawingML\Shapes\TestJira14147";

            Document doc = TestUtil.OpenSaveOpen(path, UnifiedScenario.Docx2DocxNoGold);

            Shape chart = (Shape)doc.FirstSection.Body.FirstParagraph.GetChild(NodeType.Shape, 0, false);
            ChartDataPointCollection dpCol = chart.Chart.Series[0].DataPoints;

            DmlChartSpPr pr = dpCol[0].SpPr;
            Assert.That(pr.Extensions.Count, Is.EqualTo(2));
            Assert.That(pr.Scene3DProp, Is.Null);
            Assert.That(pr.Shape3DProp, IsNot.Null());

            pr = dpCol[1].SpPr;
            DmlScene3DProperties scene3D = pr.Scene3DProp;
            DmlShape3DProperties shape3D = pr.Shape3DProp;

            Assert.That(pr.Extensions, Is.Null);
            Assert.That(scene3D.Camera.PresetCameraType, Is.EqualTo(DmlPresetCameraType.OrthographicFront));
            Assert.That(scene3D.LightRig.LightRigType, Is.EqualTo(DmlLightRigType.ThreePt));
            Assert.That(shape3D.PresetMaterial, Is.EqualTo(DmlPresetMaterialType.TranslucentPowder));
            Assert.That(shape3D.BevelTop.BevelPresetType, Is.EqualTo(DmlBevelPresetType.ArtDeco));
            Assert.That(shape3D.ContourColor.ColorType, Is.EqualTo(DmlColorType.SchemeColor));
            Assert.That(shape3D.ContourWidth, Is.EqualTo(25400));

            pr = dpCol[2].SpPr;
            Assert.That(pr.Extensions, Is.Null);
            Assert.That(pr.Scene3DProp, Is.Null);
            Assert.That(pr.Shape3DProp, IsNot.Null());

            pr = dpCol[3].SpPr;
            Assert.That(pr.Extensions, Is.Null);
            Assert.That(pr.Scene3DProp, Is.Null);
            Assert.That(pr.Shape3DProp, IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-14128 Unsupported [MS-ODRAWXML] element 20.1.2.2.18 graphicFrame (Graphic Frame).
        /// Currently does not implement reading and writing graphic frame element which
        /// is included into DrawingML shape.
        /// </summary>
        [Test]
        public void TestJira14128()
        {
            const string path = @"Model\DrawingML\Shapes\TestJira14128";
            const string strictPath = @"Model\DrawingML\Shapes\TestJira14128_Strict";

            Document doc = TestUtil.OpenSaveOpen(path, UnifiedScenario.Docx2DocxNoGold);

            GroupShape group = (GroupShape)doc.FirstSection.Body.FirstParagraph.GetChild(NodeType.GroupShape, 0, false);
            NodeCollection groups = group.GetChildNodes(NodeType.GroupShape, false);

            // Get the graphic frame from group on the second level of nesting.
            group = (GroupShape)((GroupShape)groups[0]).GetChild(NodeType.GroupShape, 0, false);
            DmlGroupShape dmlGroup = (DmlGroupShape)group.DmlNode;
            CheckGraphicFramePr(dmlGroup, 8, 20326, 9509, 21031, 15972);
            // Check that frame contains the chart.
            Assert.That(((Shape)group.FirstChild).DmlNode.DmlNodeType, Is.EqualTo(DmlNodeType.Chart));

            group = (GroupShape)groups[1];
            dmlGroup = (DmlGroupShape)group.DmlNode;
            CheckGraphicFramePr(dmlGroup, 9, -60, 15422, 20847, 13594);
            // Check that frame contains the chart.
            Assert.That(((Shape)group.FirstChild).DmlNode.DmlNodeType, Is.EqualTo(DmlNodeType.Chart));

            // Roundtrip for strict format and compare with gold.
            doc = TestUtil.Open(strictPath, LoadFormat.Docx);
            OoxmlSaveOptions so = new OoxmlSaveOptions(SaveFormat.Docx);
            so.SetTestMode();
            so.Compliance = OoxmlCompliance.Iso29500_2008_Strict;
            TestUtil.SaveOpen(doc, strictPath, UnifiedScenario.Docx2Docx, so);
        }

        /// <summary>
        /// WORDSNET-14128 for details see test "TestJira14128".
        /// Test checks reading and writing extensions of the graphic frame.
        /// </summary>
        [Test]
        public void TestJira14128ExtensionReading()
        {
            // Prepare input data.
            string xml =
            "<wp:wgp" +
            " xmlns:wp=\"http://purl.oclc.org/ooxml/drawingml/wordprocessingDrawing\"" +
            " xmlns:a=\"http://purl.oclc.org/ooxml/drawingml/main\">" +
                "<wp:graphicFrame>" +
                    "<wp:cNvPr id=\"8\" name=\"Diagram 6\">" +
                        "<a:extLst>" +
                           "<a:ext uri=\"{77A0092B-C50C-777-A947-70E740481C1C}\">" +
                           "</a:ext>" +
                        "</a:extLst>" +
                    "</wp:cNvPr>" +
                    "<wp:cNvFrPr>" +
                        "<a:graphicFrameLocks/>" +
                        "<a:extLst>" +
                               "<a:ext uri=\"{78A0092B-C50C-777-A947-70E740481C1C}\">" +
                               "</a:ext>" +
                        "</a:extLst>" +
                    "</wp:cNvFrPr>" +
                    "<a:extLst>" +
                        "<a:ext uri=\"{79A0092B-C50C-777-A947-70E740481C1C}\">" +
                        "</a:ext>" +
                    "</a:extLst>" +
                "</wp:graphicFrame>" +
            "</wp:wgp>";
            // Perform reading (check that graphic frame extensions can be read).
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            GroupShape stubGroup = DmlCompositeNodeReader.ReadGroup(reader);

            // Prepare document for export.
            const string path = @"Model\DrawingML\Shapes\TestJira14128";
            Document doc = TestUtil.Open(path, LoadFormat.Docx);

            GroupShape group, groupForReplace;
            group = (GroupShape)doc.FirstSection.Body.FirstParagraph.GetChild(NodeType.GroupShape, 0, false);
            NodeCollection groups = group.GetChildNodes(NodeType.GroupShape, false);
            groupForReplace = (GroupShape)groups[1];

            // Replace graphic frame properties with properties which were read from the xml above.
            group = (GroupShape)stubGroup.FirstChild;
            groupForReplace.DmlNode.NonVisualPr = group.DmlNode.NonVisualPr;
            ((DmlGroupShape)groupForReplace.DmlNode).Extensions = ((DmlGroupShape)group.DmlNode).Extensions;

            // Perform export (writing) of the prepared document.
            DocxExportContext exportHelper = new DocxExportContext(doc, OoxmlComplianceCore.IsoStrict);

            // Get attributes by name, in C++ namespace attribute is returned as the first.
            string attrName = "uri";
            string graphicFrameExt = exportHelper.GetXmlNode("//wp:graphicFrame/a:extLst").FirstChild.Attributes[attrName].Value;
            string cNvPrExt = exportHelper.GetXmlNode("//wp:graphicFrame/wp:cNvPr/a:extLst").FirstChild.Attributes[attrName].Value;
            string cNvFrPrExt = exportHelper.GetXmlNode("//wp:graphicFrame/wp:cNvFrPr/a:extLst").FirstChild.Attributes[attrName].Value;

            Assert.That(StringUtil.Contains(cNvPrExt, "77A0092B", true), Is.True);
            Assert.That(StringUtil.Contains(cNvFrPrExt, "78A0092B", true), Is.True);
            Assert.That(StringUtil.Contains(graphicFrameExt, "79A0092B", true), Is.True);
        }

        // FOSS: TestJira13972 removed — it checked shape BWMode after a DOCX->DOC roundtrip; DOC save is removed.


        /// <summary>
        /// Tests checks behavior of the <see cref="DocxDocumentWriterBase.GetChoiceRequires"/> method for DML shape
        /// with type <see cref="DmlNodeType.GraphicFrame"/>.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Unexpected DrawingML type.")]
        public void TestGraphicFrameChoiceRequires()
        {
            GroupShape gf = new GroupShape(new Document(), ShapeMarkupLanguage.Dml);
            gf.DmlNode = new DmlGroupShape(DmlNodeType.GraphicFrame);

            DocxDocumentWriterBase.GetChoiceRequires(gf);
        }

        /// <summary>
        /// Test checks behavior of the <see cref="DocxDocumentWriterBase.GetChoiceRequires"/> method for DML shape
        /// with type <see cref="DmlNodeType.GraphicFrame"/>.
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Unexpected DrawingML type.")]
        public void TestGetNamespaceGraphicFrame()
        {
            DmlNamespaceUtil.GetNamespace(DmlNodeType.GraphicFrame, false);
        }

        /// <summary>
        /// Test checks ability to insert new DML nodes into shape when destination or new node
        /// is graphic frame.
        /// </summary>
        [Test]
        public void TestGraphicFrameCanInsertIntoShape()
        {
            Document doc = new Document();

            Shape destShape = new Shape(doc, ShapeMarkupLanguage.Dml);
            Shape newShape = new Shape(doc, ShapeMarkupLanguage.Dml);

            GroupShape grFrame = new GroupShape(doc, ShapeMarkupLanguage.Dml);
            grFrame.DmlNode = new DmlGroupShape(DmlNodeType.GraphicFrame);

            // 1. Case when destination DML shape is not composite node and new shape is graphic frame.
            destShape.DmlNode = new DmlShape(DmlNodeType.WordprocessingShape);
            Assert.That(destShape.CanInsert(grFrame), Is.False);

            // 2. Case when destination shape contains reference to outer xml (chart) and new shape is
            // graphic frame.
            destShape.DmlNode = new DmlChartSpace();
            Assert.That(destShape.CanInsert(grFrame), Is.True);

            // 3. Case when destination shape is graphic frame and new node does not contain
            // reference to outer xml.
            destShape.DmlNode = new DmlGroupShape(DmlNodeType.GraphicFrame);
            newShape.DmlNode = new DmlShape(DmlNodeType.WordprocessingShape);
            Assert.That(destShape.CanInsert(newShape), Is.False);

            // 4. Case when destination shape is graphic frame and new node is chart
            newShape.DmlNode = new DmlChartSpace();
            Assert.That(destShape.CanInsert(newShape), Is.True);

            // 5. Case when destination shape is graphic frame and new node is diagram.
            newShape.DmlNode = new DmlDiagram();
            Assert.That(destShape.CanInsert(newShape), Is.True);

            // 6. Case when destination shape is group shape and new child node is graphic frame.
            destShape.DmlNode = new DmlGroupShape(DmlNodeType.WordprocessingGroupShape);
            Assert.That(destShape.CanInsert(grFrame), Is.True);

            // 7. Case when destination shape is nested group shape and new child node is graphic frame.
            destShape.DmlNode = new DmlGroupShape(DmlNodeType.GroupShape);
            Assert.That(destShape.CanInsert(grFrame), Is.True);

            // 7. Case when destination shape is nested group shape and new child node is graphic frame.
            destShape.DmlNode = new DmlGroupShape(DmlNodeType.GroupShape);
            Assert.That(destShape.CanInsert(grFrame), Is.True);

            // 8. Case when destination shape is canvas shape and new child node is graphic frame.
            destShape.DmlNode = new DmlLockedCanvas(DmlNodeType.WordprocessingCanvas);
            Assert.That(destShape.CanInsert(grFrame), Is.False);

            // 9. Case when destination shape is locked canvas shape and new child node is graphic frame.
            destShape.DmlNode = new DmlLockedCanvas(DmlNodeType.LockedCanvas);
            Assert.That(destShape.CanInsert(grFrame), Is.False);
        }

        /// <summary>
        /// Test checks ability to insert new DML nodes into shape when destination or new node
        /// is graphic frame.
        /// </summary>
        [Test]
        public void TestGrFrameCanInsertIntoGroupShape()
        {
            Document doc = new Document();

            GroupShape destShape = new GroupShape(doc, ShapeMarkupLanguage.Dml);
            GroupShape newShape = new GroupShape(doc, ShapeMarkupLanguage.Dml);

            // 1. Case when destination shape is group shape and new shape is graphic frame.
            destShape.DmlNode = new DmlGroupShape(DmlNodeType.WordprocessingGroupShape);
            newShape.DmlNode = new DmlGroupShape(DmlNodeType.GraphicFrame);
            Assert.That(destShape.CanInsert(newShape), Is.True);

            // 2. Case when destination shape is nested group shape and new shape is graphic frame.
            destShape.DmlNode = new DmlGroupShape(DmlNodeType.GroupShape);
            Assert.That(destShape.CanInsert(newShape), Is.True);

            // 3. Case when destination shape is chart and new shape is graphic frame.
            destShape.DmlNode = new DmlChartSpace();
            Assert.That(destShape.CanInsert(newShape), Is.True);

            // 4. Case when destination shape is canvas and new shape is graphic frame.
            destShape.DmlNode = new DmlLockedCanvas(DmlNodeType.WordprocessingCanvas);
            Assert.That(destShape.CanInsert(newShape), Is.False);

            // 5. Case when destination shape is nested canvas and new shape is graphic frame.
            // AM. It seems that GroupShape can be child node of LockedCanvas. See WORDSNET-15844
            destShape.DmlNode = new DmlLockedCanvas(DmlNodeType.LockedCanvas);
            Assert.That(destShape.CanInsert(newShape), Is.True);

            // 6. Case when destination shape is graphic frame and new shape is chart.
            destShape.DmlNode = new DmlGroupShape(DmlNodeType.GraphicFrame);
            newShape.DmlNode = new DmlChartSpace();
            Assert.That(destShape.CanInsert(newShape), Is.True);

            // 7. Case when destination shape is graphic frame and new shape is diagram.
            newShape.DmlNode = new DmlDiagram();
            Assert.That(destShape.CanInsert(newShape), Is.True);

            // 8. Case when destination shape is graphic frame and new shape is graphic frame.
            newShape.DmlNode = new DmlGroupShape(DmlNodeType.GraphicFrame);
            Assert.That(destShape.CanInsert(newShape), Is.False);

            // 9. Case when destination shape is graphic frame and new shape is canvas.
            newShape.DmlNode = new DmlLockedCanvas(DmlNodeType.WordprocessingCanvas);
            Assert.That(destShape.CanInsert(newShape), Is.False);

            // 9. Case when destination shape is graphic frame and new shape is group shape.
            newShape.DmlNode = new DmlGroupShape(DmlNodeType.WordprocessingGroupShape);
            Assert.That(destShape.CanInsert(newShape), Is.False);

            // 10. Case when destination shape is graphic frame and new shape is not group shape.
            newShape.DmlNode = new DmlShape(DmlNodeType.WordprocessingShape);
            Assert.That(destShape.CanInsert(newShape), Is.False);
        }

        /// <summary>
        /// WORDSNET-14547 NullReferenceException occurs when calling Shape.AlternativeText.
        /// Non-visual properties for diagrams and charts are not initialized. Some public properties of the
        /// DML shape takes from non-visual properties and it is cause "NullReferenceException".
        /// </summary>
        [Test]
        public void TestJira14547()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestJira14547", LoadFormat.Docx);
            NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);

            foreach (Shape shape in shapes)
            {
                // Ignore shapes exclude diagram and chart.
                if (shape.DmlNode.DmlNodeType == DmlNodeType.WordprocessingShape)
                    continue;

                // Check reading.
                Assert.That(!StringUtil.HasChars(shape.AlternativeText), Is.True);
                Assert.That(!StringUtil.HasChars(shape.Title), Is.True);
                Assert.That(!StringUtil.HasChars(shape.Name), Is.True);
                Assert.That(shape.AspectRatioLocked, Is.False);

                // Check writing.
                shape.AspectRatioLocked = true;
                shape.AlternativeText = "1";
                shape.Title = "1";
                shape.Name = "1";
            }
        }

        /// <summary>
        /// Test concerned with issue WORDSNET-14542 and checks processing of the user shapes
        /// on writing output document. This shapes have to be skipped while writing chart.
        /// </summary>
        [Test]
        public void TestGrFrameHasChartWithUserShapes()
        {
            const string fileName = @"Model\DrawingML\TestGrFrameHasChartWithUserShapes";

            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);

            // Check that top level group has not fallback shape.
            GroupShape group = (GroupShape)doc.FirstSection.Body.FirstParagraph.GetChildNodes(NodeType.Any, false)[1];
            Assert.That(group.FallbackShape, Is.Null);

            // Use gold file to check generated fallback and output XML.
            doc = TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2Docx);

            group = (GroupShape)doc.FirstSection.Body.FirstParagraph.GetChildNodes(NodeType.Any, false)[1];
            GroupShape grFrame = (GroupShape)group.FirstChild;

            Assert.That(grFrame.DmlNode.DmlNodeType, Is.EqualTo(DmlNodeType.GraphicFrame));
            Assert.That(grFrame.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));

            // Check that chart has one user shape.
            Assert.That(((Shape)grFrame.FirstChild).GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Checks graphic frame properties.
        /// </summary>
        /// <param name="dmlGroup">Graphic frame shape for check.</param>
        /// <param name="id">Unique identifier for the current DrawingML object within the document.</param>
        /// <param name="xOffset">Coordinate on the x-axis.</param>
        /// <param name="yOffset">Coordinate on the y-axis.</param>
        /// <param name="width">Width of the extents rectangle in EMUs.</param>
        /// <param name="length">Length of the extents rectangle in EMUs.</param>
        private static void CheckGraphicFramePr(
            DmlGroupShape dmlGroup,
            int id,
            double xOffset,
            double yOffset,
            double width,
            double length)
        {
            Assert.That(dmlGroup.DmlNodeType, Is.EqualTo(DmlNodeType.GraphicFrame));

            DmlTransform transform = dmlGroup.TransformInternal;

            DmlNvPrGraphicFrame grFrPr = (DmlNvPrGraphicFrame)dmlGroup.NonVisualPr;
            DmlCnvPrGraphicFrame cnvFrPr = (DmlCnvPrGraphicFrame)grFrPr.CNvProperties;
            DmlNvDrawingProperties nvPr = (DmlNvDrawingProperties)grFrPr.NvDrawingProperties;

            Assert.That(grFrPr.Holder, Is.EqualTo(DmlNvHolder.GraphicFrame));

            Assert.That(MathUtil.AreEqual(xOffset, transform.XOffset), Is.True);
            Assert.That(MathUtil.AreEqual(yOffset, transform.YOffset), Is.True);
            Assert.That(MathUtil.AreEqual(width, transform.Width), Is.True);
            Assert.That(MathUtil.AreEqual(length, transform.Height), Is.True);

            Assert.That(cnvFrPr.Locks, IsNot.Null());
            Assert.That(cnvFrPr.Holder, Is.EqualTo(DmlNvHolder.GraphicFrame));

            Assert.That(nvPr.Id, Is.EqualTo(id));
            Assert.That(!string.IsNullOrEmpty(nvPr.Name), Is.True);
        }

        /// <summary>
        /// Test checks reading and writing of the hidden line extension.
        /// </summary>
        [Test]
        public void TestReadingWritingHiddenLineExtension()
        {
            const string path = @"Model\DrawingML\Shapes\TestReadingWritingHiddenLineExt";

            Document doc = TestUtil.OpenSaveOpen(path, UnifiedScenario.Docx2Docx);
            NodeCollection docShapes = doc.FirstSection.Body.GetChildNodes(NodeType.Shape, true);

            DmlTailLineEndStyle tailEndStyle = new DmlTailLineEndStyle();
            tailEndStyle.Length = ArrowLength.Long;
            tailEndStyle.Width = ArrowWidth.Wide;
            tailEndStyle.Type = ArrowType.Arrow;

            DmlHeadLineEndStyle headEndStyle = new DmlHeadLineEndStyle();
            headEndStyle.Length = ArrowLength.Medium;
            headEndStyle.Width = ArrowWidth.Medium;
            headEndStyle.Type = ArrowType.Oval;

            CheckHiddenLineExtension(docShapes, null, 0, DmlDashType.PresetDash, DashStyle.Dash,
                "1F4D78", 69850, 8.0, null, null);
            CheckHiddenLineExtension(docShapes, null, 1, DmlDashType.PresetDash, DashStyle.ShortDot,
                "7030A0", 41275, 8.0, tailEndStyle, headEndStyle);
            CheckHiddenLineExtension(docShapes, null, 2, DmlDashType.PresetDash, DashStyle.Default,
                 "000000", 9525, 8.0, null, null);
            CheckHiddenLineExtension(docShapes, null, 3, DmlDashType.PresetDash, DashStyle.Default,
                 "FF0000", 101600, 8.0, null, null);
            CheckHiddenLineExtension(docShapes, null, 4, DmlDashType.PresetDash, DashStyle.LongDash,
                 "000000", 98425, 8.0, null, null);
            CheckHiddenLineExtension(docShapes, null, 5, DmlDashType.PresetDash, DashStyle.Default,
                 "92D050", 34925, 8.0, null, null);

            // Check that "fallback" of the shape contains expected extension.
            Shape shape = (Shape)doc.FirstSection.HeadersFooters[1].FirstParagraph.FirstChild;
            DmlExtension ext = ((DmlShape)shape.FallbackShape.DmlNode).SpPrExtensions[DmlExtensionUri.HiddenLine];

            CheckHiddenLineExtension(null, ext, 0, DmlDashType.PresetDash, DashStyle.Default,
                "92D050", 34925, 0.0, null, null);
        }




        /// <summary>
        /// WORDSNET-16362 Shape is not resized properly.
        /// MSW automation resets relative size while absolute width changing. Do the same to fix the problem.
        /// </summary>
        [Test]
        public void TestJira16362()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\Shapes\TestJira16362.docx");
            Shape shape = doc.FirstSection.Body.Shapes[0];

            // Check original relative size.
            CheckRelativeSize(shape, 0, 200);

            shape.TextBox.FitShapeToText = false;
            shape.AspectRatioLocked = false;
            shape.Height = 200;

            // Relative height has to be reset.
            CheckRelativeSize(shape, 0, 0);

            shape.Width = 400;
            shape.Rotation = 30;

            doc = TestUtil.SaveOpen(doc, @"Model\DrawingML\Shapes\TestJira16362", UnifiedScenario.Docx2DocxNoGold);
            CheckRelativeSize(doc.FirstSection.Body.Shapes[0], 0, 0);
        }

        /// <summary>
        /// Relates to WORDSNET-16362
        /// Checks how changes of the absolute sizes values are affected to relative sizes.
        /// </summary>
        [Test]
        public void TestJira16362RelSizes()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\Shapes\TestJira16362RelSizes.docx");
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;

            // Document contains two shapes VML and DML. Check behavior for both types.
            foreach (Shape shape in shapes)
            {
                // If there is not relative width attributes, then after changing of the absolute value nothing to change.
                shape.ShapePr.Remove(ShapeAttr.RelativeWidth);
                shape.ShapePr.Remove(ShapeAttr.WidthPercent);
                shape.Width = 400;
                CheckRelativeSize(shape, null, 200);

                // Initialize relative width and check current values.
                shape.ShapePr[ShapeAttr.RelativeWidth] = RelativeHorizontalSize.Margin;
                shape.ShapePr[ShapeAttr.WidthPercent] = 200;
                CheckRelativeSize(shape, 200, 200);

                // Check that relative width will be reset.
                shape.Width = 400;
                CheckRelativeSize(shape, 0, 200);

                // Check that relative height will be reset.
                shape.Height = 200;
                CheckRelativeSize(shape, 0, 0);

                // If there is not relative height attributes, then after changing of the absolute value nothing to change.
                shape.ShapePr.Remove(ShapeAttr.RelativeHeight);
                shape.ShapePr.Remove(ShapeAttr.HeightPercent);
                shape.Height = 400;
                CheckRelativeSize(shape, 0, null);
            }

            Assert.That(shapes[0].MarkupLanguage, Is.EqualTo(ShapeMarkupLanguage.Dml));
            Assert.That(shapes[1].MarkupLanguage, Is.EqualTo(ShapeMarkupLanguage.Vml));
        }



        // FOSS: TestJira16359Vml removed — it loads and round-trips a WordML (.wml) VML shape; WordML load+save removed.


        /// <summary>
        /// Related to WORDSNET-16976
        /// Issue with hidden property, which returns invalid value for nested shapes.
        /// </summary>
        [Test]
        public void TestJira16976Hidden()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestJira16976Hidden.docx");
            Shape shape = (Shape)doc.FirstSection.Body.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.Hidden, Is.True);

            shape = (Shape)doc.FirstSection.Body.GetChild(NodeType.Shape, 1, true);
            Assert.That(shape.Hidden, Is.False);

            // Check, that setter works without errors.
            shape.Hidden = true;
            Assert.That(shape.Hidden, Is.True);
        }


        /// <summary>
        /// Relates to WORDSNET-13967
        /// Checks, that shape constructor raises exception for SingleCornerSnipped shape type.
        /// </summary>
        [Test, ExpectedException(typeof(NotSupportedException), ExpectedMessage = "Cannot create shapes of this type.")]
        public void TestJira13967SetShapeType2()
        {
            new Shape(new Document(), ShapeType.SingleCornerSnipped);
        }


        /// <summary>
        /// Realted to  WORDSNET-17530
        /// Checks case when document contains textbox.
        /// </summary>
        [Test]
        public void TestJira17530Txbx()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestJira17530Txbx.docx");
            Assert.That(doc.FirstSection.Body.Shapes[0].ShapeType, Is.EqualTo(ShapeType.TextBox));
        }

        /// <summary>
        /// WORDSNET-17672 Document.UpdatePageLayout throws System.NullReferenceException.
        /// AW does not read "nvGraphicFramePr" element it is caused the issue. Read the non visual
        /// graphic frame properties to fix the problem.
        /// </summary>
        [Test]
        public void TestJira17672()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestJira17672.docx");
            GroupShape lockedCanvas = (GroupShape)doc.FirstSection.Body.GetChild(NodeType.GroupShape, 0, true);
            Assert.That(lockedCanvas.DmlNode.DmlNodeType, Is.EqualTo(DmlNodeType.LockedCanvas));

            GroupShape graphicFrame = (GroupShape)lockedCanvas.FirstChild;
            DmlGroupShape dmlGraphicFrame = (DmlGroupShape)graphicFrame.DmlNode;
            Assert.That(dmlGraphicFrame.DmlNodeType, Is.EqualTo(DmlNodeType.GraphicFrame));
            Assert.That(dmlGraphicFrame.IsParentLockedCanvas, Is.True);

            Assert.That(dmlGraphicFrame.NonVisualPr, IsNot.Null());
            Assert.That(dmlGraphicFrame.NonVisualPr.NvDrawingProperties.Name, Is.EqualTo("6 Gráfico"));

            // Check client case.
            // No update page layout in FOSS.

            // Check resiliency which was added for "hidden" attribute.
            dmlGraphicFrame.NonVisualPr = null;
            Assert.That(dmlGraphicFrame.Hidden, Is.False);
        }

        /// <summary>
        /// Related to WORDSNET-17672
        /// Checks sequence of child nodes of the graphic frame placed into the locked canvas.
        /// </summary>
        [Test]
        public void TestJira17672MarkUp()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\TestJira17672.docx");
            DocxExportContext exportContext = new DocxExportContext(doc);

            XmlNode graphicFrameElement = exportContext.GetXmlNode(@"descendant::a:graphicFrame");
            XmlNodeList xmlNodes = graphicFrameElement.ChildNodes;
            Assert.That(xmlNodes[0].Name, Is.EqualTo("a:nvGraphicFramePr"));
            // The sequence of these nodes is important. Otherwise Word can not open document.
            // Also there are cases when sequence must be reversed (TestJira14128, TestGrFrameHasChartWithUserShapes).
            Assert.That(xmlNodes[1].Name, Is.EqualTo("a:graphic"));
            Assert.That(xmlNodes[2].Name, Is.EqualTo("a:xfrm"));
        }








        /// <summary>
        /// Related with WORDSNET-21498
        /// The case when link is reset for a group.
        /// <see cref="DmlNvPrBase.NvDrawingProperties"/> is not changed at a this case.
        /// </summary>
        [Test]
        public void Test21498Group()
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\Test21498Group", LoadFormat.Docx);
            GroupShape group = (GroupShape)doc.FirstSection.Body.GetChild(NodeType.GroupShape, 0, true);

            string url = "https://www.aspose.com/";

            Assert.That(group.ShapePr[ShapeAttr.HyperlinkAddress], Is.EqualTo(url));
            Assert.That(group.DmlNode.NonVisualPr.NvDrawingProperties, Is.Null);

            // Act.
            group.HRef = string.Empty;

            // Assert.
            Assert.That(group.ShapePr[ShapeAttr.HyperlinkAddress], Is.Null);
            Assert.That(group.DmlNode.NonVisualPr.NvDrawingProperties, Is.Null);
        }

        /// <summary>
        /// Related with WORDSNET-21498
        /// The case when link is reset for a rectangle.
        /// <see cref="DmlNvPrBase.NvDrawingProperties"/> is not changed at a this case.
        /// </summary>
        [Test]
        public void Test21498Rect()
        {
            Shape shape = TestUtil.Open(@"Model\DrawingML\Test21498Rect", LoadFormat.Docx).FirstSection.Body.Shapes[0];

            Assert.That(shape.ShapePr[ShapeAttr.HyperlinkAddress], Is.Null);
            Assert.That(shape.DmlNode.NonVisualPr.NvDrawingProperties, Is.Null);
            string url = "https://www.google.com/";

            // Act.
            shape.HRef = url;

            // Assert.
            Assert.That(shape.ShapePr[ShapeAttr.HyperlinkAddress], Is.EqualTo(url));
            Assert.That(shape.DmlNode.NonVisualPr.NvDrawingProperties, Is.Null);
        }


        /// <summary>
        /// Relates to WORDSNET-21597
        /// Tests whether Decorative is preserved during the roundtrip.
        /// </summary>
        // FOSS: only the OOXML roundtrip survives — Doc/Rtf/Wml load+save were removed.
        [TestCase(UnifiedScenario.Docx2DocxNoGold)]
        public void Test21597(UnifiedScenario scenario)
        {
            const string fileName = @"Model\DrawingML\Test21597";

            Document doc;
            if (scenario == UnifiedScenario.Rtf2Docx)
            {
                doc = TestUtil.Open(fileName, LoadFormat.Rtf);
                OoxmlSaveOptions options = new OoxmlSaveOptions(SaveFormat.Docx);
                options.Compliance = OoxmlCompliance.Iso29500_2008_Strict;
                doc = TestUtil.SaveOpen(doc, fileName, options, false);
            }
            else
                doc = TestUtil.OpenSaveOpen(fileName, scenario | UnifiedScenario.NoGold);

            Shape shape1 = doc.FirstSection.Body.Shapes[0];
            Shape shape2 = doc.FirstSection.Body.Shapes[1];

            Assert.That(shape1.GraphicData.Decorative, Is.True);
            Assert.That(shape2.GraphicData.Decorative, Is.False);
        }

        /// <summary>
        /// WORDSNET-21649 Bookmarks are lost after initiating LayoutEnumerator.
        /// Removes fallback if alternate content for choice and fallback is different.
        /// </summary>
        [Test]
        public void Test21649()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Textbox\Test21649.docx");

            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);   // FOSS: was a raw validator run with null SaveOptions (now asserts).

            int bookMarkStarts = doc.GetChildNodes(NodeType.BookmarkStart, true).Count;
            int bookMarkEnds = doc.GetChildNodes(NodeType.BookmarkEnd, true).Count;

            Assert.That(bookMarkStarts, Is.EqualTo(77));
            Assert.That(bookMarkEnds, Is.EqualTo(77));
        }



        /// <summary>
        /// Gets <see cref="DmlDiagramDataModel"/> instance of a diagram of a child shape of the specified node.
        /// </summary>
        private static DmlDiagramDataModel GetDiagramDataModel(CompositeNode parent, int diagramShapeIndex)
        {
            Shape shape = (Shape)parent.GetChild(NodeType.Shape, diagramShapeIndex, true);
            return ((DmlDiagram)shape.DmlNode).Data;
        }

        /// <summary>
        /// Gets <see cref="DmlTextBody"/> instance of a child shape of the specified node.
        /// </summary>
        private static DmlTextBody GetChildShapeTextBody(CompositeNode parent, int shapeIndex)
        {
            Shape shape = (Shape)parent.GetChild(NodeType.Shape, shapeIndex, true);
            return shape.DmlShape.TextShape.TextBody;
        }

        /// <summary>
        /// Checks space after the specified DML paragraph.
        /// </summary>
        private static void CheckSpaceAfter(DmlParagraph paragraph, int expectedValue, string expectedParagraphText)
        {
            // Check text to make sure this is the expected paragraph.
            Assert.That(paragraph.GetRunText(), Is.EqualTo(expectedParagraphText));

            DmlPointsTextSpacing spaceAfter =
                (DmlPointsTextSpacing)paragraph.Properties.GetDirectProperty(DmlParagraphPropertiesIds.SpaceAfter);

            Assert.That(spaceAfter, IsNot.Null());
            Assert.That(spaceAfter.Value.Value, Is.EqualTo(expectedValue));
        }

        /// <summary>
        /// Sequentially sets passed shape types for specified shapes.
        /// </summary>
        private static void CheckShapeTypeSetter(Shape shape, ShapeType[] shapeTypes, Dictionary<ShapeType, object> exclusions)
        {
            foreach (ShapeType shapeType in shapeTypes)
            {
                try
                {
                    shape.SetShapeType(shapeType);
                    Assert.That(shape.ShapePr[ShapeAttr.ShapeType], Is.EqualTo(shapeType));
                }
                catch (InvalidOperationException)
                {
                    if ((exclusions != null) && !exclusions.ContainsKey(shapeType))
                        throw;
                }
            }
        }

        /// <summary>
        /// Checks opacity value for specified shape, takes in attention the precision while converting double to fixed.
        /// </summary>
        private static void CheckVmlOpacity(Shape shape, double opacity)
        {
            int fixedOpacity = ConvertUtilCore.DoubleToFixed(opacity);
            Assert.That(MathUtil.AreEqual(ConvertUtilCore.FixedToDouble(fixedOpacity), shape.Fill.Opacity), Is.True);
        }

        /// <summary>
        /// Checks shape relative sizes values according to specified values.
        /// </summary>
        private static void CheckRelativeSize(ShapeBase shape, object relWidth, object relHeight)
        {
            if (relWidth != null)
            {
                Assert.That(shape.ShapePr[ShapeAttr.RelativeWidth], Is.EqualTo(RelativeHorizontalSize.Margin));
                Assert.That((int)shape.WidthPercent, Is.EqualTo((int)relWidth));
            }
            else
            {
                Assert.That(shape.WidthPercent, Is.Null);
                Assert.That(shape.ShapePr[ShapeAttr.RelativeWidth], Is.Null);
            }

            if (relHeight != null)
            {
                Assert.That(shape.ShapePr[ShapeAttr.RelativeHeight], Is.EqualTo(RelativeVerticalSize.Margin));
                Assert.That((int)shape.HeightPercent, Is.EqualTo((int)relHeight));
            }
            else
            {
                Assert.That(shape.HeightPercent, Is.Null);
                Assert.That(shape.ShapePr[ShapeAttr.RelativeHeight], Is.Null);
            }
        }

        /// <summary>
        /// Checks content of the hidden line extension  according to specified arguments.
        /// Method can check extension of the shape which stores in the collection or
        /// extension can be passed explicitly.
        /// </summary>
        /// <param name="shapes">Collection of the shapes which holds extensions.</param>
        /// <param name="ext">Extension for check.</param>
        /// <param name="shapeIndex">Index of the shape in the collection.</param>
        /// <param name="DmlDashType">Dashing scheme.</param>
        /// <param name="DashStyle">Dashed line style.</param>
        /// <param name="color">Value of the "val" attribute of the color.</param>
        /// <param name="width">Width of the outline.</param>
        /// <param name="lineMiterLimit">Value of the miter for line.</param>
        /// <param name="tailExpEndStyle">Tail line end style.</param>
        /// <param name="headExpEndStyle">Line Head/End Style.</param>
        private static void CheckHiddenLineExtension(
            NodeCollection shapes,
            DmlExtension extension,
            int shapeIndex,
            DmlDashType dashType,
            DashStyle dashStyle,
            string color,
            double width,
            double lineMiterLimit,
            DmlTailLineEndStyle tailExpEndStyle,
            DmlHeadLineEndStyle headExpEndStyle)
        {
            DmlExtension ext = (extension == null) ?
             ((DmlShape)(((Shape)shapes[shapeIndex]).DmlNode)).SpPrExtensions[DmlExtensionUri.HiddenLine] : extension;

            Assert.That(DmlExtensionUri.HiddenLine, Is.EqualTo(ext.Uri));

            if (!MathUtil.AreEqual(0.0, width))
                Assert.That(MathUtil.AreEqual(width, ext.OutlinePr.WidthInEmus), Is.True);

            DmlFill fill = ext.OutlinePr.Fill;

            Assert.That(fill.DmlFillType, Is.EqualTo(DmlFillType.SolidFill));
            Assert.That(((DmlHexRgbColor)fill.DmlColorInternal).Value, Is.EqualTo(color));

            Assert.That(ext.OutlinePr.DashStyle, Is.EqualTo(dashStyle));
            Assert.That(ext.OutlinePr.Dash.DashType, Is.EqualTo(dashType));
            Assert.That(MathUtil.AreEqual(lineMiterLimit, ext.OutlinePr.LineMiterLimit), Is.True);

            DmlTailLineEndStyle tailEndStyle = ext.OutlinePr.TailLineEndStyle;

            if (tailExpEndStyle != null)
            {
                Assert.That(tailEndStyle.Length, Is.EqualTo(tailExpEndStyle.Length));
                Assert.That(tailEndStyle.Width, Is.EqualTo(tailExpEndStyle.Width));
                Assert.That(tailEndStyle.Type, Is.EqualTo(tailExpEndStyle.Type));
            }

            DmlHeadLineEndStyle headEndStyle = ext.OutlinePr.HeadLineEndStyle;

            // TODO looks like a misake, maybe headEndStyle should be used below?
            if (headExpEndStyle != null)
            {
                Assert.That(headExpEndStyle.Length, Is.EqualTo(headExpEndStyle.Length));
                Assert.That(headExpEndStyle.Width, Is.EqualTo(headExpEndStyle.Width));
                Assert.That(headExpEndStyle.Type, Is.EqualTo(headExpEndStyle.Type));
            }
        }
    }
}
