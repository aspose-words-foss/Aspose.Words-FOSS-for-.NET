// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for shape geometry attributes.
    /// </summary>
    [TestFixture]
    public class TestShapeGeometry : UnifiedTestsBase
    {
        /// <summary>
        /// Checks we can obtain standard shape type properties.
        /// </summary>
        [Test]
        public void TestShapeTypeGeometry()
        {
            // <v:shapetype id="_x0000_t94" coordsize="21600,21600" o:spt="94" 
            //  adj="16200,5400" 
            //  path="m@0,l @0@1, 0@1 @5,10800, 0@2 @0@2 @0,21600, 21600,10800xe">
            //  <v:stroke joinstyle="miter" />
            //  <v:formulas><v:f eqn="val #0" /><v:f eqn="val #1" /><v:f eqn="sum height 0 #1" />
            //      <v:f eqn="sum 10800 0 #1" /><v:f eqn="sum width 0 #0" /><v:f eqn="prod @4 @3 10800" />
            //      <v:f eqn="sum width 0 @5" />
            //  </v:formulas>
            //  <v:path o:connecttype="custom" o:connectlocs="@0,0;@5,10800;@0,21600;21600,10800" 
            //  o:connectangles="270,180,90,0" textboxrect="@5,@1,@6,@2" />
            //  <v:handles><v:h position="#0,#1" xrange="0,21600" yrange="0,10800" /></v:handles>
            // </v:shapetype>
            ShapePr shapePr = ShapeTypeLibrary.GetShapeTypePr(ShapeType.NotchedRightArrow);

            // Just check some of the parsed points.
            PathPoint[] points = (PathPoint[])shapePr[ShapeAttr.GeometryVertices];
            Assert.That(points.Length, Is.EqualTo(8));
            Assert.That(points[0].ToString(), Is.EqualTo("X:@0, Y:0"));
            Assert.That(points[1].ToString(), Is.EqualTo("X:@0, Y:@1"));

            // Check some of the parsed path infos.
            PathInfo[] pathInfos = (PathInfo[])shapePr[ShapeAttr.GeometrySegmentInfo];
            Assert.That(pathInfos.Length, Is.EqualTo(4));
            CheckPathInfo(pathInfos[0], PathType.MoveTo, 0);
            CheckPathInfo(pathInfos[1], PathType.LineTo, 7);
        }

        /// <summary>
        /// Check shape type definitions we obtain are cached and not created every time.
        /// </summary>
        [Test]
        public void TestShapeTypeCached()
        {
            ShapePr shapePrA = ShapeTypeLibrary.GetShapeTypePr(ShapeType.UturnArrow);
            ShapePr shapePrB = ShapeTypeLibrary.GetShapeTypePr(ShapeType.UturnArrow);
            Assert.That((object)shapePrB, Is.EqualTo((object)shapePrA)); // Casting for C++.
        }

        /// <summary>
        /// Check shape types that do not have shape type definitions.
        /// </summary>
        [Test]
        public void TestShapeTypeMissing()
        {
            Assert.That(ShapeTypeLibrary.GetShapeTypePr(ShapeType.CustomShape), Is.EqualTo(null));
        }

        /// <summary>
        /// This shape contains several bezier shapes and had a problem reading it in DOCX and WML.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPathBezier(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Geometry\TestPathBezier", lf, sf);

            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            // Check coord size.
            Assert.That(shape.CoordOrigin.X, Is.EqualTo(0));
            Assert.That(shape.CoordOrigin.Y, Is.EqualTo(0));
            Assert.That(shape.CoordSize.Width, Is.EqualTo(3840));
            Assert.That(shape.CoordSize.Height, Is.EqualTo(3630));

            // Check the path vertices.
            PathPoint[] points = (PathPoint[])shape.FetchShapeAttrInternal(ShapeAttr.GeometryVertices);
            Assert.That(points.Length, Is.EqualTo(28));
            Assert.That(points[0].X.Value, Is.EqualTo(420));
            Assert.That(points[0].Y.Value, Is.EqualTo(3060));
            // RK This is a start of a relative curve. The relative value is 300,-180, but it is translated
            // to this absolute value since the model, DOC and RTF do not support relative values in path.
            Assert.That(points[7].X.Value, Is.EqualTo(1080));
            Assert.That(points[7].Y.Value, Is.EqualTo(180));

            // Check the path commands.
            PathInfo[] pathInfos = (PathInfo[])shape.FetchShapeAttrInternal(ShapeAttr.GeometrySegmentInfo);
            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Doc2Wml:
                case UnifiedScenario.Rtf2RtfNoGold:
                {
                    // RK This is an array as read from DOC and RTF. 
                    Assert.That(pathInfos.Length, Is.EqualTo(22));
                    CheckPathInfo(pathInfos[0], PathType.MoveTo, 0);
                    CheckPathInfo(pathInfos[1], PathType.EscapeAutoCurve, 0);
                    CheckPathInfo(pathInfos[2], PathType.CurveTo, 1);
                    CheckPathInfo(pathInfos[20], PathType.Close, 1);
                    CheckPathInfo(pathInfos[21], PathType.End, 0);
                    break;
                }
                case UnifiedScenario.DocxDml2DocxDml:
                case UnifiedScenario.Docx2DocxNoGold:
                case UnifiedScenario.Wml2WmlNoGold:
                {
                    // RK This is an array as read from WML and DOCX. It is equivalent to the above, 
                    // just MS Word seems to combine commands slightly differently.
                    Assert.That(pathInfos.Length, Is.EqualTo(10));
                    CheckPathInfo(pathInfos[0], PathType.MoveTo, 0);
                    CheckPathInfo(pathInfos[1], PathType.CurveTo, 2);
                    CheckPathInfo(pathInfos[2], PathType.CurveTo, 1);
                    CheckPathInfo(pathInfos[8], PathType.Close, 1);
                    CheckPathInfo(pathInfos[9], PathType.End, 0);
                    break;
                }
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }

        private static void CheckPathInfo(PathInfo pathInfo, PathType expectedPathType, int expectedSegments)
        {
            Assert.That(pathInfo.PathType, Is.EqualTo(expectedPathType));
            Assert.That(pathInfo.SegmentCount, Is.EqualTo(expectedSegments));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestGeometry(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Shape\Geometry\TestGeometry", lf, sf);
        }

        
        /// <summary>
        /// This is a new shape inserted from ClipArt. In ClipArt is says format is WMF, but somehow 
        /// it gets converted by MS Word into a shape with geometry and adjustable handles.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestClipArt(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Geometry\TestClipArt", lf, sf);

            // Check coordorigin and size of one of the group shapes. They used to be lost overridden
            // by geometry coordinate space.
            GroupShape groupShape = (GroupShape)doc.GetChild(NodeType.GroupShape, 3, true);
            Assert.That(groupShape.CoordOrigin.X, Is.EqualTo(1824));
            Assert.That(groupShape.CoordOrigin.Y, Is.EqualTo(633));
            Assert.That(groupShape.CoordSize.Width, Is.EqualTo(2834));
            Assert.That(groupShape.CoordSize.Height, Is.EqualTo(2849));

            // Check the tree shape.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 6, true);
            Assert.That(shape.Name, Is.EqualTo("Tree"));
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.CustomShape));
            // It is important that this attribute is not missing for custom shapes, otherwise
            // MS Word does not draw such shapes when loaded from RTF.
            PathInfo[] pathInfos = (PathInfo[])shape.ShapePr.GetDirectAttr(ShapeAttr.GeometrySegmentInfo);
            Assert.That(pathInfos, IsNot.Null());
        }
        



        /// <summary>
        /// Arcsize for roundrect in DOC and RTF store arcsize as adj1, but VML stores as arcsize.
        /// In the model we store as adj1 and convert VML arcsize to adj1.
        /// In VML arcsize is 16.16 percentage value. In the model adj1 is in geometry units.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRoundRect(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Geometry\TestRoundRect", lf, sf);

            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.GetAdjust(1), Is.EqualTo(5513));

            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Assert.That(shape.GetAdjust(1), Is.EqualTo(0));
            
            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            Assert.That(shape.GetAdjust(1), Is.EqualTo(10800));
        }


        /// <summary>
        /// WORDSNET-13556 Exception is thrown in VmlShapeReader while reading VML shape
        /// Get shape properties from template when type is missing.
        /// </summary>
        [Test]
        public void TestJira13556()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Geometry\TestJira13556.xml");

            ShapePr shapePr = doc.FirstSection.Body.Shapes[0].ShapePr;
            Assert.That(shapePr[ShapeAttr.GeometryVertices], IsNot.Null());
            Assert.That(shapePr[ShapeAttr.GeometrySegmentInfo], IsNot.Null());
        }

        /// <summary>
        /// WORDSNET-16036 Coordinate space (the coordsize attribute) of a VML shape was not written,
        /// if only width or only height is defined directly.
        /// </summary>
        [Test]
        public void TestJira16036()
        {
            // FOSS: Doc reader removed; input converted to .docx.
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Geometry\TestJira16036",
                UnifiedScenario.Docx2Docx | UnifiedScenario.NoGold);
            Shape shape = doc.FirstSection.Body.Shapes[1];
            Assert.That(shape.CoordSizeWidth, Is.EqualTo(25519));
        }

        // FOSS: Test21857A / Test21857B removed. Their source was a Word 2003 WordML (.wml) file with a
        // shape whose width/height were absent, which the WML reader marked with the InvalidShapeSizeDefault
        // (50.25) sentinel. The WML reader is removed in FOSS; the Word .wml->.docx conversion writes an
        // explicit size for the missing dimension, so the sentinel condition can't be reproduced.
    }
}
