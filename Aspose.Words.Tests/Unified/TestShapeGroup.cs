// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for group shapes.
    /// </summary>
    [TestFixture]
    [NonParallelizable]
    public class TestShapeGroup : UnifiedTestsBase
    {
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestGroupShape(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Geometry\TestGroupShape", lf, sf);

            Assert.That(doc.GetChildNodes(NodeType.GroupShape, true).Count, Is.EqualTo(2));

            // Check the group.
            GroupShape group = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);
            Assert.That(group.IsGroup, Is.EqualTo(true));
            Assert.That(group.EditAs, Is.EqualTo(EditAs.Group));
            Assert.That(group.IsTopLevel, Is.EqualTo(true));
            CheckPosition(group, 30, 35, 94, 64);
            CheckCoordinateSpace(group, 2400, 2140, 1880, 1280);
            Assert.That(group.Count, Is.EqualTo(3));

            // This shape is in parent coordinate space.
            Shape rect = (Shape)group.FirstChild;
            Assert.That(rect.IsTopLevel, Is.EqualTo(false));
            Assert.That(rect.ShapeType, Is.EqualTo(ShapeType.Rectangle));
            CheckPosition(rect, 2400, 2140, 1080, 900);

            // I don't play with group CoordSize and CoordOrigin, it will work, but it will make
            // the shape selection look a bit ugly because the bounding rectangle should be synced.
            // I will leave this as is for now. Maybe later make nice public shape moving methods.

            // Widen the whole group.
            group.Width = 400;
            // Make the rectangle as high as the group itself.
            rect.Height = 1280;

            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Geometry\TestGroupShape Modified", lf, sf);
        }



        private static void CheckPosition(ShapeBase shape, double left, double top, double width, double height)
        {
            Assert.That(shape.Left, Is.EqualTo(left));
            Assert.That(shape.Top, Is.EqualTo(top));
            Assert.That(shape.Width, Is.EqualTo(width));
            Assert.That(shape.Height, Is.EqualTo(height));
        }

        private static void CheckCoordinateSpace(ShapeBase shape, int originX, int originY, int sizeX, int sizeY)
        {
            Assert.That(shape.CoordOrigin.X, Is.EqualTo(originX));
            Assert.That(shape.CoordOrigin.Y, Is.EqualTo(originY));
            Assert.That(shape.CoordSize.Width, Is.EqualTo(sizeX));
            Assert.That(shape.CoordSize.Height, Is.EqualTo(sizeY));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCanvas(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestCanvas", lf, sf);

            GroupShape group = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);
            Assert.That(group.EditAs, Is.EqualTo(EditAs.Canvas));

            Shape shape = (Shape)group.GetChild(NodeType.Shape, 0, true);
            // This shape is actually positioned 36 pt from top left corner of the canvas.
            // So these values below are somewhat ugly for the user to deal with, but no other ideas at this stage.
            Assert.That(shape.Left, Is.EqualTo(2446.0));
            Assert.That(shape.RelativeHorizontalPosition, Is.EqualTo(RelativeHorizontalPosition.Column));

            shape = (Shape)group.GetChild(NodeType.Shape, 1, true);
            // This shape is positioned at 0,0 from the Canvas center. Although MS Word allows to do that in
            // the user interface, after clicking OK in the dialog box it shows position from the top left corner.
            // So the shape is positioned 216 pt from the top left corner.
            Assert.That(shape.Left, Is.EqualTo(3046.0));
            Assert.That(shape.RelativeHorizontalPosition, Is.EqualTo(RelativeHorizontalPosition.Column));
        }

        /// <summary>
        /// WORDSNET-27303 Shape.Bounds returned by Aspose.Words is not correct for rotated shape in group shape.
        /// Now shape rotation is taken into account when calculating the bounds.
        /// </summary>
        [TestCase("Test27303.docx", 4.69, 2.80)]
        [TestCase("Test27303_30Rot.docx", 1.82, 7.23)]
        [TestCase("Test27303_45Rot.docx", 4.69, 2.80)]
        [TestCase("Test27303_135Rot.docx", 1.82, 7.23)]
        public void Test27303(string fileName, double expectedWidth, double expectedHeight)
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\" + fileName);
            Shape rotatedShape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            RectangleF bounds = rotatedShape.BoundsInPoints;

            Assert.That(ConvertUtilCore.PointToCm(bounds.Width), Is.EqualTo(expectedWidth).Within(0.005));
            Assert.That(ConvertUtilCore.PointToCm(bounds.Height), Is.EqualTo(expectedHeight).Within(0.005));
        }

        /// <summary>
        /// WORDSNET-27896 Coordinates of rotated shape are returned differently after 24.12 version
        /// Changed the way shape coordinates are calculated to support different scaling along the X and Y axes with
        /// rotated shapes.
        /// </summary>
        [TestCase("Test27896.docx", 4.69, 2.80)]
        [TestCase("Test27896NoGroupRotation.docx", 1.82, 7.23)]
        public void Test27896(string fileName, double expectedWidth, double expectedHeight)
        {
            Document doc = TestUtil.Open(@"Model\DrawingML\" + fileName);
            List<Shape> shapes = doc.GetChildNodes(NodeType.Shape, true).ToList<Shape>();

            // ShapeBase.BoundInPoints returns shape coordinates without rotation: the coordinates are checked taking this
            // into account. Also, shape's outline appears to be drawn over its rectangle, which increases the shape's
            // display bouds.
            // The coordinates are taken from a screenshot of Test27896NoGroupRotation.docx in MS Word.
            CheckShapeBounds(240, 130, 316, 306, shapes[0].BoundsInPoints, shapes[0].Rotation);
            CheckShapeBounds(444, 137, 609, 280, shapes[1].BoundsInPoints, shapes[1].Rotation);
            CheckShapeBounds(472, 333, 548, 522, shapes[2].BoundsInPoints, shapes[2].Rotation);
            CheckShapeBounds(190, 366, 354, 508, shapes[3].BoundsInPoints, shapes[3].Rotation);
        }

        /// <summary>
        /// WORDSNET-28627 Regression: Bounds are correct only after setting the width/height of the groupShape.
        /// Tests getting BoundsInPoints after VML shapes grouping.
        /// </summary>
        [Test]
        public void Test28627()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Creates new VML shape.
            Shape shape = new Shape(doc, ShapeType.Rectangle);
            shape.Width = 75.1;
            shape.Height = 56.4;

            GroupShape group = builder.InsertGroupShape(0d, 0d, 0d, 0d, shape);

            RectangleF boundsInPoints1 = shape.BoundsInPoints;

            Assert.That(boundsInPoints1.Left, Is.EqualTo(0).Within(0.1));
            Assert.That(boundsInPoints1.Top, Is.EqualTo(0).Within(0.1));
            Assert.That(boundsInPoints1.Width, Is.EqualTo(75.1).Within(0.1));
            Assert.That(boundsInPoints1.Height, Is.EqualTo(56.4).Within(0.1));

            group.Width = 100.0;
            group.Height = 100.0;

            RectangleF boundsInPoints2 = ((Shape)(group.FirstChild)).BoundsInPoints;

            Assert.That(boundsInPoints2.Left, Is.EqualTo(boundsInPoints1.Left).Within(0.1));
            Assert.That(boundsInPoints2.Top, Is.EqualTo(boundsInPoints1.Top).Within(0.1));
            Assert.That(boundsInPoints2.Width, Is.EqualTo(boundsInPoints1.Width).Within(0.1));
            Assert.That(boundsInPoints2.Height, Is.EqualTo(boundsInPoints1.Height).Within(0.1));
        }

        /// <summary>
        /// Checks shape bounds in pixels.
        /// </summary>
        private static void CheckShapeBounds(int pxX1, int pxY1, int pxX2, int pxY2, RectangleF bounds, double rotation)
        {
            bool swapXY = ((int)System.Math.Round(rotation / 90, MidpointRounding.AwayFromZero) % 2 == 1);
            RectangleF rect = swapXY ? ShapeBase.Rotate90Degrees(bounds) : bounds;
            const double accuracy = 1d; // Includes possible increase in the size of the shape due to the outline.

            Assert.That(ConvertUtil.PointToPixel(rect.X), Is.EqualTo(pxX1).Within(accuracy));
            Assert.That(ConvertUtil.PointToPixel(rect.Y), Is.EqualTo(pxY1).Within(accuracy));
            Assert.That(ConvertUtil.PointToPixel(rect.X + rect.Width), Is.EqualTo(pxX2).Within(accuracy));
            Assert.That(ConvertUtil.PointToPixel(rect.Y + rect.Height), Is.EqualTo(pxY2).Within(accuracy));
        }
    }
}
