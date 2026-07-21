// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Validation;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for shape position and wrapping properties.
    /// </summary>
    [TestFixture]
    public class TestShapePosition : UnifiedTestsBase
    {
        [Test]
        public void TestDefaultDimensions()
        {
            Document doc = new Document();
            Shape shape = new Shape(doc, ShapeType.Ellipse);
            Assert.That(shape.Bounds, Is.EqualTo(RectangleF.Empty));
            Assert.That(shape.BoundsInPoints, Is.EqualTo(RectangleF.Empty));
            Assert.That(shape.CoordOrigin, Is.EqualTo(Point.Empty));
            Assert.That(shape.CoordSize, Is.EqualTo(new Size(21600, 21600)));
        }

        [Test]
        public void TestNegativeLocation()
        {
            Document doc = new Document();
            Shape shape = new Shape(doc, ShapeType.Ellipse);
            shape.Left = -10;
            shape.Top = -20;
            Assert.That(shape.Bounds.Location, Is.EqualTo(new PointF(-10, -20)));
        }

        [Test]
        public void TestNegativeWidth()
        {
            try
            {
                Document doc = new Document();
                Shape shape = new Shape(doc, ShapeType.Ellipse);
                shape.Width = -10;
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                // RK We do not use NUnit's ExpectedEsception and ExpectedMessage because 
                // MatchType is ignored when the test is run from ReSharper.
                Assert.That(e.Message.IndexOf("Shape width cannot be less than 0.") >= 0, Is.True);
            }
        }

        [Test]
        public void TestNegativeHeight()
        {
            try
            {
                Document doc = new Document();
                Shape shape = new Shape(doc, ShapeType.Ellipse);
                shape.Height = -10;
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.That(e.Message.IndexOf("Shape height cannot be less than 0.") >= 0, Is.True);
            }
        }

        /// <summary>
        /// Top level shapes have limit on the shape size.
        /// </summary>
        [Test]
        public void TestHugeTopLevelWidth()
        {
            try
            {
                Document doc = new Document();

                Shape shape = new Shape(doc, ShapeType.Ellipse);
                doc.FirstSection.Body.FirstParagraph.AppendChild(shape);

                Assert.That(shape.IsTopLevel, Is.EqualTo(true));
                // This line throws and it is expected behavior.
                shape.Width = 1585;
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.That(e.Message.IndexOf("Shape width cannot be greater than 1584 points.") >= 0, Is.True);
            }
        }

        /// <summary>
        /// Child shapes have no limit on the shape size.
        /// </summary>
        [Test]
        public void TestHugeChildWidth()
        {
            Document doc = new Document();
            
            GroupShape topLevel = new GroupShape(doc);
            doc.FirstSection.Body.FirstParagraph.AppendChild(topLevel);

            Shape child = new Shape(doc, ShapeType.Rectangle);
            topLevel.AppendChild(child);

            Assert.That(child.IsTopLevel, Is.EqualTo(false));
            child.Width = 1585;
            Assert.That(child.Width, Is.EqualTo(1585));
        }

        [Test]
        public void TestSetZeroCoordSize()
        {
            try
            {
                Document doc = new Document();
                GroupShape group = new GroupShape(doc);
                group.CoordSize = new Size(100, -1);
                Assert.Fail();
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.That(e.Message.IndexOf("Local coordinate space size cannot be less than zero.") >= 0, Is.True);
            }
        }

        [Test]
        public void TestSetCoordSize()
        {
            Document doc = new Document();
            GroupShape group = new GroupShape(doc);
            
            // Set a valid value and check.
            group.CoordSize = new Size(50, 20);
            Assert.That(new Size(50, 20), Is.EqualTo(group.CoordSize));

            // Set an invalid value safely, it sets to default.
            group.SetCoordSizeSafe(new Size(-1, -1));
            Assert.That(new Size(1000, 1000), Is.EqualTo(group.CoordSize));

            // Set zero coordsize.
            group.SetCoordSizeSafe(new Size(0, 0));
            Assert.That(new Size(0, 0), Is.EqualTo(group.CoordSize));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTopLevelBounds(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();

            Shape shape = new Shape(doc, ShapeType.Ellipse);
            doc.FirstSection.Body.FirstParagraph.AppendChild(shape);
            shape.Left = 40;
            shape.Top = 50;
            shape.Width = 200;
            shape.Height = 100;

            Assert.That(shape.Bounds, Is.EqualTo(new RectangleF(40, 50, 200, 100)));
            Assert.That(shape.BoundsInPoints, Is.EqualTo(new RectangleF(40, 50, 200, 100)));

            TestUtil.SaveOpen(doc, @"Model\Shape\Layout\TestTopLevelBounds", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestChildBounds(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();

            GroupShape group = new GroupShape(doc);
            doc.FirstSection.Body.FirstParagraph.AppendChild(group);
            group.Bounds = new RectangleF(40, 50, 200, 100);
            group.CoordSize = new Size(1000, 2000);
            group.CoordOrigin = new Point(-500, -1000);

            Shape shape = new Shape(doc, ShapeType.Ellipse);
            shape.Bounds = new RectangleF(0, 0, 100, 100);
            group.AppendChild(shape);

            RectangleF rect = shape.BoundsInPoints;
            // The group's coordinates are set up in such a way that the child's 0,0 
            // will be at the center of the parent.
            Assert.That((double)rect.X, Is.EqualTo(40.0 + 200.0 / 2.0));
            Assert.That((double)rect.Y, Is.EqualTo(50.0 + 100.0 / 2.0));

            // Width of the shape is 100 out of 1000 and height of the shape is 100 out of 2000.
            // Multiple by points to come to the points.
            Assert.That((double)rect.Width, Is.EqualTo(100.0 / 1000.0 * 200.0));
            Assert.That((double)rect.Height, Is.EqualTo(100.0 / 2000.0 * 100.0));

            TestUtil.SaveOpen(doc, @"Model\Shape\Layout\TestChildBounds", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestChildBoundsNoCoordSize(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();

            // Create a group shape that does not have CoordSize specified.
            GroupShape group = new GroupShape(doc);
            doc.FirstSection.Body.FirstParagraph.AppendChild(group);
            group.Bounds = new RectangleF(40, 50, 200, 100);

            Shape shape = new Shape(doc, ShapeType.Ellipse);
            shape.Bounds = new RectangleF(0, 0, 100, 100);
            group.AppendChild(shape);

            RectangleF rect = shape.BoundsInPoints;
            Assert.That((double)rect.X, Is.EqualTo(40.0));
            Assert.That((double)rect.Y, Is.EqualTo(50.0));
            // Default coord size is 1000.
            Assert.That((double)rect.Width, Is.EqualTo(100.0 / 1000.0 * 200.0));
            Assert.That((double)rect.Height, Is.EqualTo(100.0 / 1000.0 * 100.0));

            TestUtil.SaveOpen(doc, @"Model\Shape\Layout\TestChildBoundsNoCoordSize", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestOuterLeftTopWidthHeight(LoadFormat lf, SaveFormat sf)
        {
            // Verify open save works.
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestOuterLeftTopWidthHeight", lf, sf);
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.Left, Is.EqualTo(50.0));
            Assert.That(shape.Top, Is.EqualTo(40.0));
            Assert.That(shape.Width, Is.EqualTo(80.0));
            Assert.That(shape.Height, Is.EqualTo(35.0));

            // Modify and verify save open works.
            shape.Left = 10;
            shape.Top = 20;
            shape.Width = 200;
            shape.Height = 5;

            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Layout\TestOuterLeftTopWidthHeight Modified", lf, sf);
            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.Left, Is.EqualTo(10.0));
            Assert.That(shape.Top, Is.EqualTo(20.0));
            Assert.That(shape.Width, Is.EqualTo(200.0));
            Assert.That(shape.Height, Is.EqualTo(5.0));
        }

        /// <summary>
        /// Check that the Absolute positioning options in the Layout / Advanced Layout / Picture Position work.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPositionAbsolute(LoadFormat lf, SaveFormat sf)
        {
            // Verify open save works.
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestPositionAbsolute", lf, sf);
            CheckPositionAbsolute(doc, 0, RelativeHorizontalPosition.Page, RelativeVerticalPosition.Page);
            CheckPositionAbsolute(doc, 1, RelativeHorizontalPosition.Margin, RelativeVerticalPosition.Margin);
            CheckPositionAbsolute(doc, 2, RelativeHorizontalPosition.Column, RelativeVerticalPosition.Paragraph);
            CheckPositionAbsolute(doc, 3, RelativeHorizontalPosition.Character, RelativeVerticalPosition.Line);

            // These are shapes created in Word 2007 using new position options.
            // When MS Word saves this to WordML or DOC it converts them to Page relative.
            // When MS Word saves this to DOCX it only saves them in DrawingML.
            // I don't yet know how to support these in the model and how to down convert so leave till later.
            // SonarIgnoreStart
            // CheckPositionAbsolute(doc, 4, RelativeHorizontalPosition.LeftMargin, RelativeVerticalPosition.TopMargin);
            // CheckPositionAbsolute(doc, 5, RelativeHorizontalPosition.RightMargin, RelativeVerticalPosition.BottomMargin);
            // CheckPositionAbsolute(doc, 6, RelativeHorizontalPosition.InsideMargin, RelativeVerticalPosition.InsideMargin);
            // CheckPositionAbsolute(doc, 7, RelativeHorizontalPosition.OutsideMargin, RelativeVerticalPosition.OutsideMargin);
            // SonarIgnoreEnd

            // Modify and very save open works.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            shape.RelativeHorizontalPosition = RelativeHorizontalPosition.Margin;
            shape.RelativeVerticalPosition = RelativeVerticalPosition.Margin;
            shape.Top = -20;
            shape.Left = -20;

            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Layout\TestPositionAbsolute Modified", lf, sf);
            CheckPositionAbsolute(doc, 0, RelativeHorizontalPosition.Margin, RelativeVerticalPosition.Margin);
        }

        private static void CheckPositionAbsolute(
            Document doc, 
            int objectIdx, 
            RelativeHorizontalPosition hPos, 
            RelativeVerticalPosition vPos)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, objectIdx, true);
            Assert.That(shape.RelativeHorizontalPosition, Is.EqualTo(hPos));
            Assert.That(shape.RelativeVerticalPosition, Is.EqualTo(vPos));
        }

        /// <summary>
        /// Check that the Alignment positioning options in the Layout / Advanced Layout / Picture Position work.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPositionAlignment(LoadFormat lf, SaveFormat sf)
        {
            // Verify open save works.
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestPositionAlignment", lf, sf);
            CheckPositionAlignment(doc, 0, HorizontalAlignment.Left, VerticalAlignment.Top);
            CheckPositionAlignment(doc, 1, HorizontalAlignment.Center, VerticalAlignment.Center);
            CheckPositionAlignment(doc, 2, HorizontalAlignment.Right, VerticalAlignment.Bottom);
            CheckPositionAlignment(doc, 3, HorizontalAlignment.Inside, VerticalAlignment.Inside);
            CheckPositionAlignment(doc, 4, HorizontalAlignment.Outside, VerticalAlignment.Outside);
            CheckPositionAlignment(doc, 5, HorizontalAlignment.None, VerticalAlignment.None);

            // Modify and very save open works.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            shape.HorizontalAlignment = HorizontalAlignment.Center;
            shape.VerticalAlignment = VerticalAlignment.Inside;

            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Layout\TestPositionAlignment Modified", lf, sf);
            CheckPositionAlignment(doc, 0, HorizontalAlignment.Center, VerticalAlignment.Inside);
        }

        private static void CheckPositionAlignment(
            Document doc, 
            int objectIdx, 
            HorizontalAlignment hAlign, 
            VerticalAlignment vAlign)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, objectIdx, true);
            Assert.That(shape.HorizontalAlignment, Is.EqualTo(hAlign));
            Assert.That(shape.VerticalAlignment, Is.EqualTo(vAlign));
        }

        /// <summary>
        /// Check that the Book Layout positioning options in the Layout / Advanced Layout / Picture Position work.
        /// </summary>
        [Test]
        public void TestPositionBookLayout()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestPositionBookLayout.docx");
            
            // This positioning is actually just normal HorizontalAlignment.Inside and HoirzontalAlignment.Outside Relative to Margin or Page.
            CheckPositionBookLayout(doc, 0, HorizontalAlignment.Inside, RelativeHorizontalPosition.Margin);
            CheckPositionBookLayout(doc, 1, HorizontalAlignment.Outside, RelativeHorizontalPosition.Margin);
            CheckPositionBookLayout(doc, 2, HorizontalAlignment.Inside, RelativeHorizontalPosition.Page);
            CheckPositionBookLayout(doc, 3, HorizontalAlignment.Outside, RelativeHorizontalPosition.Page);
        }

        private static void CheckPositionBookLayout(
            Document doc,
            int objectIdx,
            HorizontalAlignment hAlign,
            RelativeHorizontalPosition hPos)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, objectIdx, true);
            Assert.That(shape.HorizontalAlignment, Is.EqualTo(hAlign));
            Assert.That(shape.RelativeHorizontalPosition, Is.EqualTo(hPos));
        }

        /// <summary>
        /// Check that the Relative positioning options in the Layout / Advanced Layout / Picture Position work.
        /// WORDSNET-24133 /shape percent position/ Sidebar missing after opening and saving with Aspose.
        /// </summary>
        [Test]
        public void TestPositionRelative()
        {
            // FOSS Rtf/Wml are removed; verify the Docx roundtrip (relative positioning fully supported).
            TestPositionRelative(UnifiedScenario.Docx2DocxNoGold, true);
        }

        private static void TestPositionRelative(UnifiedScenario scenario, bool hasRelative)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestPositionRelative", scenario);

            // Green rectangles.
            CheckPositionRelative(doc, 0, 0.0, 0.0, 0, 0, RelativeHorizontalPosition.Margin, RelativeVerticalPosition.Margin, hasRelative);
            CheckPositionRelative(doc, 1, 126.0, 144.0, 500, 500, RelativeHorizontalPosition.Margin, RelativeVerticalPosition.Margin, hasRelative);
            CheckPositionRelative(doc, 2, 252.0, 288.0, 1000, 1000, RelativeHorizontalPosition.Margin, RelativeVerticalPosition.Margin, hasRelative);

            // Violet rectangles
            CheckPositionRelative(doc, 3, 0.0, 0.0, 0, 0, RelativeHorizontalPosition.Page, RelativeVerticalPosition.Page, hasRelative);
            CheckPositionRelative(doc, 4, 144.0, 172.8, 400, 400, RelativeHorizontalPosition.Page, RelativeVerticalPosition.Page, hasRelative);
            CheckPositionRelative(doc, 5, 324.0, 388.8, 900, 900, RelativeHorizontalPosition.Page, RelativeVerticalPosition.Page, hasRelative);

            // Orange rectangles
            CheckPositionRelative(doc, 6, -5.4, -3.6, -100, -50, RelativeHorizontalPosition.LeftMargin, RelativeVerticalPosition.BottomMargin, hasRelative);
            CheckPositionRelative(doc, 7, 0.0, 0.0, 0, 0, RelativeHorizontalPosition.RightMargin, RelativeVerticalPosition.TopMargin, hasRelative);

            // Gray rectangles.
            CheckPositionRelative(doc, 8, -27.0, 79.2, -500, 1100, RelativeHorizontalPosition.InsideMargin, RelativeVerticalPosition.InsideMargin, hasRelative);
            CheckPositionRelative(doc, 9, 0.0, 122.4, 0, 1700, RelativeHorizontalPosition.OutsideMargin, RelativeVerticalPosition.OutsideMargin, hasRelative);
        }

        private static void CheckPositionRelative(
            Document doc,
            int objectIdx,
            double left,
            double top,
            int leftPercent,
            int topPercent,
            RelativeHorizontalPosition hPos,
            RelativeVerticalPosition vPos,
            bool hasRelative)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, objectIdx, true);

            // alexnosk: Use small delta to compare size. Delta is half of twip converted to point.
            // When we write to DOC or RTF, value is converted to twip and precision of the value might be decreased.
            // When we convert points to twips we round resulting value and half of twip is maximum value we can lose. 
            const double delta = 0.05;

            // These values are calculated by document validator.
            Assert.That(shape.Left, Is.EqualTo(left).Within(delta));
            Assert.That(shape.Top, Is.EqualTo(top).Within(delta));

            if (hasRelative)
            {
                Assert.That(shape.LeftPercent, Is.EqualTo(leftPercent));
                Assert.That(shape.TopPercent, Is.EqualTo(topPercent));
            }
            else
            {
                Assert.That(shape.LeftPercent, Is.Null);
                Assert.That(shape.TopPercent, Is.Null);
            }

            Assert.That(shape.RelativeHorizontalPosition, Is.EqualTo(hPos));
            Assert.That(shape.RelativeVerticalPosition, Is.EqualTo(vPos));
        }

        /// <summary>
        /// Check that the relative size works.
        /// </summary>
        [Test]
        public void TestSizeRelative()
        {
            // FOSS Rtf/Wml are removed; verify the Docx roundtrip (relative size fully supported).
            TestSizeRelative(UnifiedScenario.Docx2DocxNoGold, true);
        }

        private static void TestSizeRelative(UnifiedScenario scenario, bool hasRelative)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestSizeRelative", scenario);

            CheckSizeRelative(doc, 0, 126.0, 43.2, 500, 150, RelativeHorizontalSize.Margin, RelativeVerticalSize.Margin, hasRelative);
            CheckSizeRelative(doc, 1, 108.0, 43.2, 300, 100, RelativeHorizontalSize.Page, RelativeVerticalSize.Page, hasRelative);
            CheckSizeRelative(doc, 2, 108, 36.0, 2000, 500, RelativeHorizontalSize.LeftMargin, RelativeVerticalSize.TopMargin, hasRelative);
            CheckSizeRelative(doc, 3, 54.0, 72.0, 1000, 1000, RelativeHorizontalSize.RightMargin, RelativeVerticalSize.BottomMargin, hasRelative);
            CheckSizeRelative(doc, 4, 54.0, 72.0, 1000, 1000, RelativeHorizontalSize.InnerMargin, RelativeVerticalSize.InnerMargin, hasRelative);
            CheckSizeRelative(doc, 5, 54.0, 72.0, 1000, 1000, RelativeHorizontalSize.OuterMargin, RelativeVerticalSize.OuterMargin, hasRelative);
        }

        private static void CheckSizeRelative(
            Document doc,
            int objectIdx,
            double width,
            double height,
            int widthPercent,
            int heightPercent,
            RelativeHorizontalSize relWidth,
            RelativeVerticalSize relHeight,
            bool hasRelative)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, objectIdx, true);

            // alexnosk: Use small delta to compare size. Delta is half of twip converted to point.
            // When we write to DOC or RTF, value is converted to twip and precision of the value might be decreased.
            // When we convert points to twips we round resulting value and half of twip is maximum value we can lose. 
            const double delta = 0.05;

            // These values are calculated by document validator.
            Assert.That(shape.Width, Is.EqualTo(width).Within(delta));
            Assert.That(shape.Height, Is.EqualTo(height).Within(delta));

            if (hasRelative)
            {
                Assert.That(shape.WidthPercent, Is.EqualTo(widthPercent));
                Assert.That(shape.HeightPercent, Is.EqualTo(heightPercent));

                Assert.That(shape.RelativeHorizontalSize, Is.EqualTo(relWidth));
                Assert.That(shape.RelativeVerticalSize, Is.EqualTo(relHeight));
            }
            else
            {
                Assert.That(shape.WidthPercent, Is.Null);
                Assert.That(shape.HeightPercent, Is.Null);

                Assert.That(shape.RelativeHorizontalSize, Is.EqualTo(RelativeHorizontalSize.Page));
                Assert.That(shape.RelativeVerticalSize, Is.EqualTo(RelativeVerticalSize.Page));
            }
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestWrapType(LoadFormat lf, SaveFormat sf)
        {
            // Verify open save works.
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestWrapType", lf, sf);
            
            CheckWrapType(doc, 0, WrapType.Square, false);
            CheckWrapType(doc, 1, WrapType.Tight, true);    // It seems for this types of wrapping MS Word sets shape "behind text" flag on.
            CheckWrapType(doc, 2, WrapType.Through, false);    
            CheckWrapType(doc, 3, WrapType.TopBottom, false);
            CheckWrapType(doc, 4, WrapType.None, true);        // This shape is Behind text.
            CheckWrapType(doc, 5, WrapType.None, false);    // This shape is in Front of text.
            CheckWrapType(doc, 6, WrapType.Inline, false);

            // Modify and very save open works.
            // Convert floating shape into inline.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            // Pretty cool, we can now convert non inline shapes into inline and back easily!
            shape.WrapType = WrapType.Inline;    
            // MS Word seems to be doing this too when changing from floating to inline, 
            // but I don't think we should do this automatically.
            shape.AnchorLocked = true;
            shape.Locks.RotationLocked = true;
            shape.Locks.PositionLocked = true;

            // Make the shape that was below text to float above text.
            shape = (Shape)doc.GetChild(NodeType.Shape, 4, true);
            shape.BehindText = false;

            // Make the shape that was above text to be behind text.
            shape = (Shape)doc.GetChild(NodeType.Shape, 5, true);
            shape.BehindText = true;

            // Convert inline shape to floating.
            shape = (Shape)doc.GetChild(NodeType.Shape, 6, true);
            shape.WrapType = WrapType.Square;
            shape.Left = 50;
            shape.Top = 20;
            // MS Word seems to be doing this too when changing from inline shape to floating, 
            // but I don't think we should do this automatically.
            shape.RelativeHorizontalPosition = RelativeHorizontalPosition.Column;
            shape.RelativeVerticalPosition = RelativeVerticalPosition.Paragraph;
            shape.AnchorLocked = false;
            shape.Locks.RotationLocked = false;
            shape.Locks.PositionLocked = false;

            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Layout\TestWrapType Modified", lf, sf);
            CheckWrapType(doc, 0, WrapType.Inline, false);
            CheckWrapType(doc, 4, WrapType.None, false);    
            CheckWrapType(doc, 5, WrapType.None, true);    
            CheckWrapType(doc, 6, WrapType.Square, false);
        }

        private static void CheckWrapType(Document doc, int objectIdx, WrapType wrapType, bool isBehindText)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, objectIdx, true);
            Assert.That(shape.WrapType, Is.EqualTo(wrapType));
            Assert.That(shape.BehindText, Is.EqualTo(isBehindText));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestInlineShape(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestInlineShape", lf, sf);
            
            // This checks that inline shapes are represented just like floating shapes.
            // The SHAPE field that is needed for inline shapes is synthesized in the DOC file.
            // So you need to open the file in MS Word various versions to check its all okay there.
            Assert.That(doc.GetText(), Is.EqualTo("The quick dog.\r\rHello inline shapes.\x000c"));

            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.IsInline, Is.EqualTo(true));
            Assert.That(shape.WrapType, Is.EqualTo(WrapType.Inline));
            Assert.That(shape.Left, Is.EqualTo(0.0));
            Assert.That(shape.Top, Is.EqualTo(0.0));

            // Try to modify the inline shape, but it does not have effect in the DOC file because its inline.
            shape.Left = 10;
            shape.Top = 10;
            shape.RelativeHorizontalPosition = RelativeHorizontalPosition.Page;
            shape.RelativeVerticalPosition = RelativeVerticalPosition.Page;

            TestUtil.SaveOpen(doc, @"Model\Shape\Layout\TestInlineShape Modified", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestWrapSide(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestWrapSide", lf, sf);
            CheckWrapSide(doc, 0, WrapSide.Both, 1, 2, 3, 4);
            CheckWrapSide(doc, 1, WrapSide.Left, 0, 0, 9, 9);
            CheckWrapSide(doc, 2, WrapSide.Right, 0, 0, 9, 9);
            CheckWrapSide(doc, 3, WrapSide.Largest, 0, 0, 9, 9);
        }

        private static void CheckWrapSide(
            Document doc, 
            int objectIdx, 
            WrapSide wrapSide,
            double distanceTop,
            double distanceBottom,
            double distanceLeft,
            double distanceRight)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, objectIdx, true);
            Assert.That(shape.WrapSide, Is.EqualTo(wrapSide));
            Assert.That(shape.DistanceTop, Is.EqualTo(distanceTop));
            Assert.That(shape.DistanceBottom, Is.EqualTo(distanceBottom));
            Assert.That(shape.DistanceLeft, Is.EqualTo(distanceLeft));
            Assert.That(shape.DistanceRight, Is.EqualTo(distanceRight));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPositionOptions(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestPositionOptions", lf, sf);
            CheckPositionOptions(doc, 0, false, true, true);    // These are default settings.
            CheckPositionOptions(doc, 1, true, true, true);        // Test AnchorLocked
            CheckPositionOptions(doc, 2, false, false, true);    // Test AllowOverlap false
            CheckPositionOptions(doc, 3, false, true, false);    // Test AllowInCell false
        }

        private static void CheckPositionOptions(
            Document doc,
            int objectIdx,
            bool lockAnchor,
            bool allowOverlap,
            bool allowInCell)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, objectIdx, true);
            Assert.That(shape.AnchorLocked, Is.EqualTo(lockAnchor));
            Assert.That(shape.AllowOverlap, Is.EqualTo(allowOverlap));
            Assert.That(shape.IsLayoutInCell, Is.EqualTo(allowInCell));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRotation(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Geometry\TestRotation", lf, sf);

            // These are top level shapes.
            CheckRotation(doc, 0, 14, FlipOrientation.None, 102.55, 38.3);
            CheckRotation(doc, 1, 54, FlipOrientation.None, 102.55, 38.3);
            CheckRotation(doc, 2, 0, FlipOrientation.Horizontal, 102.55, 38.3);
            CheckRotation(doc, 3, 0, FlipOrientation.Vertical, 102.55, 38.3);
            CheckRotation(doc, 4, 0, FlipOrientation.Both, 102.55, 38.3);
           
            // These are shapes in a group.
            CheckRotation(doc, 5, 14, FlipOrientation.None, 2051, 766);
            CheckRotation(doc, 6, 0, FlipOrientation.Horizontal, 2051, 766);
            CheckRotation(doc, 7, 0, FlipOrientation.Vertical, 2051, 766);
            CheckRotation(doc, 8, 0, FlipOrientation.Both, 2051, 766);
            CheckRotation(doc, 9, 54, FlipOrientation.None, 2051, 766);

            // Check the group itself.
            GroupShape group = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);
            Assert.That(group.Rotation, Is.EqualTo(45).Within(1.0));
            Assert.That(group.Width, Is.EqualTo(108.35));
            Assert.That(group.Height, Is.EqualTo(291.9));

            // Modify one of the shapes.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            shape.Rotation = 54;

            TestUtil.SaveOpen(doc, @"Model\Shape\Geometry\TestRotation Modified", lf, sf);
        }

        private static void CheckRotation(Document doc, int shapeIdx, double angle, FlipOrientation orientation, double width, double height)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, shapeIdx, true);
            Assert.That(shape.Rotation, Is.EqualTo(angle).Within(1.0));
            Assert.That(shape.FlipOrientation, Is.EqualTo(orientation));
            Assert.That(shape.Width, Is.EqualTo(width));
            Assert.That(shape.Height, Is.EqualTo(height));
        }

        /// <summary>
        /// Test that ZOrder works when shapes are anchored to different paragraphs.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestZOrderAnchorsDifferent(LoadFormat lf, SaveFormat sf)
        {
            // Verify open save works.
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestZOrderAnchorsDifferent", lf, sf);
            CheckZOrder(doc, 0, 0, ShapeType.Rectangle, false);
            CheckZOrder(doc, 1, 1, ShapeType.Ellipse, false);
            CheckZOrder(doc, 2, 2, ShapeType.Triangle, false);

            // Modify. This brings the rectangle to the top.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            shape.ZOrder = 4;

            // Verify modified ZOrder works.
            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Layout\TestZOrderAnchorsDifferent Modified", lf, sf);

            // Now the order is bottom Ellipse, Triangle, Rectangle top.
            CheckZOrder(doc, 0, 2, ShapeType.Rectangle, false); 
            CheckZOrder(doc, 1, 0, ShapeType.Ellipse, false);   
            CheckZOrder(doc, 2, 1, ShapeType.Triangle, false);  
        }

        /// <summary>
        /// Test that ZOrder works when shapes are anchored to the same paragraph.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestZOrderAnchorsSame(LoadFormat lf, SaveFormat sf)
        {
            // Verify open save works.
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestZOrderAnchorsSame", lf, sf);
            // Although drawing objects themselves appear in the document text in reverse order,
            // their Z order is still correct.
            CheckZOrder(doc, 0, 2, ShapeType.Triangle, false);
            CheckZOrder(doc, 1, 1, ShapeType.Ellipse, false);
            CheckZOrder(doc, 2, 0, ShapeType.Rectangle, false);

            // Modify. This brings the triangle to the bottom.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            shape.ZOrder = -1;    

            // Verify modified ZOrder works.
            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Layout\TestZOrderAnchorsSame Modified", lf, sf);
            
            // Now the order is bottom Triangle, Rectangle, Ellipse top.
            CheckZOrder(doc, 0, 0, ShapeType.Triangle, false); 
            CheckZOrder(doc, 1, 2, ShapeType.Ellipse, false);  
            CheckZOrder(doc, 2, 1, ShapeType.Rectangle, false);
        }

        /// <summary>
        /// Test that ZOrder works for shapes behind and in front of text.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestZOrderBehind(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestZOrderBehind", lf, sf);
            // Current order: BehindText(Diamond, Circle), InFrontOfText(Rectangle, Line)
            CheckZOrder(doc, 0, 2, ShapeType.Rectangle, false);
            CheckZOrder(doc, 1, 1, ShapeType.FlowChartSummingJunction, true);
            CheckZOrder(doc, 2, 0, ShapeType.Diamond, true);
            CheckZOrder(doc, 3, 3, ShapeType.StraightConnector1, false);

            // New order I want: BehindText(Line, Diamond), InFrontOfText(Rectangle, Circle)

            // Modify line
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 3, true);
            shape.BehindText = true;
            shape.ZOrder = 10;

            // Modify diamond
            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            shape.BehindText = true;
            shape.ZOrder = 20;

            // Modify rectangle
            shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            shape.BehindText = false;
            shape.ZOrder = 30;

            // Modify circle
            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            shape.BehindText = false;
            shape.ZOrder = 40;

            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Layout\TestZOrderBehind Modified", lf, sf);

            CheckZOrder(doc, 0, 2, ShapeType.Rectangle, false);
            CheckZOrder(doc, 1, 3, ShapeType.FlowChartSummingJunction, false); 
            CheckZOrder(doc, 2, 1, ShapeType.Diamond, true);  
            CheckZOrder(doc, 3, 0, ShapeType.StraightConnector1, true); 
        }

        private static void CheckZOrder(Document doc, int objectIdx, int zOrder, ShapeType shapeType, bool isBehind)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, objectIdx, true);
            Assert.That(shape.ShapeType, Is.EqualTo(shapeType));
            Assert.That(shape.ZOrder, Is.EqualTo(zOrder));
            Assert.That(shape.BehindText, Is.EqualTo(isBehind));
        }



        /// <summary>
        /// WORDSNET-16546 Implemented the public property <see cref="ShapeBase.IsLayoutInCell"/>.
        /// </summary>
        [Test]
        public void TestJira16546()
        {
            const string fileName = @"Model\Shape\Unsorted\TestJira16546";
            Document doc = TestUtil.Open(fileName, LoadFormat.Docx); // FOSS .doc -> .docx
            Shape shape = doc.FirstSection.Body.Shapes[0];
            Assert.That(shape.IsLayoutInCell, Is.True);

            shape.IsLayoutInCell = false;
            TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2DocxNoGold);
            shape = doc.FirstSection.Body.Shapes[0];
            Assert.That(shape.IsLayoutInCell, Is.False);

            shape.IsLayoutInCell = true;
            TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2DocxNoGold);
            shape = doc.FirstSection.Body.Shapes[0];
            Assert.That(shape.IsLayoutInCell, Is.True);
        }




    }
}
