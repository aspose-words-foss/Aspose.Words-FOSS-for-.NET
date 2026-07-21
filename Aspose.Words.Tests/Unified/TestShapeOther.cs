// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Drawing;
using System.IO;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Loading;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for various shape features.
    /// </summary>
    [TestFixture]
    public class TestShapeOther : UnifiedTestsBase
    {
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefaultShapes(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Geometry\TestDefaultShapes", lf, sf);

            // Verify some properties of a default oval.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.Id, Is.EqualTo(1025));

            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.Ellipse));
            Assert.That(shape.ZOrder, Is.EqualTo(1));

            // Verify most of the properties of a default rectangle.
            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Assert.That(shape.Id, Is.EqualTo(1026));

            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.Rectangle));
            Assert.That(shape.ZOrder, Is.EqualTo(0));

            Assert.That(shape.GetText(), Is.EqualTo(""));
            Assert.That((BWMode)shape.FetchShapeAttrInternal(ShapeAttr.BWMode), Is.EqualTo(BWMode.Automatic));

            Assert.That(shape.Filled, Is.EqualTo(true));
            Assert.That(shape.FillColor.ToArgb(), Is.EqualTo(Color.White.ToArgb()));

            Assert.That(shape.Stroked, Is.EqualTo(true));
            Assert.That(shape.StrokeWeight, Is.EqualTo(0.75));
            Assert.That(shape.StrokeColor.ToArgb(), Is.EqualTo(Color.Black.ToArgb()));

            Assert.That(shape.RelativeHorizontalPosition, Is.EqualTo(RelativeHorizontalPosition.Column));
            Assert.That(shape.RelativeVerticalPosition, Is.EqualTo(RelativeVerticalPosition.Paragraph));
            Assert.That(shape.Left, Is.EqualTo(24.0));
            Assert.That(shape.Top, Is.EqualTo(18.0));
            Assert.That(shape.Width, Is.EqualTo(96.0));
            Assert.That(shape.Height, Is.EqualTo(45.0));

            Assert.That(shape.BehindText, Is.EqualTo(false));
            Assert.That(shape.WrapType, Is.EqualTo(WrapType.None));

            Assert.That(shape.AnchorLocked, Is.EqualTo(false));
            Assert.That(shape.AllowOverlap, Is.EqualTo(true));
            Assert.That(shape.IsLayoutInCell, Is.EqualTo(true));
        }

        /// <summary>
        /// Test generation of shape identifiers when creating or cloning shapes.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestShapeIdGenerate(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();

            // We generate unique shape identifiers for every new shape create.
            Shape shape1 = new Shape(doc, ShapeType.Rectangle);
            shape1.Width = 100;
            shape1.Height = 20;
            Assert.That(shape1.Id, Is.EqualTo(100001));

            GroupShape group1 = new GroupShape(doc);
            Assert.That(group1.Id, Is.EqualTo(100002));
            group1.AppendChild(shape1);
            group1.Width = 100;
            group1.Height = 20;
            group1.WrapType = WrapType.None;
            group1.CoordOrigin = Point.Empty;
            group1.CoordSize = new Size(100, 20);

            // Add the first group to the main text.
            doc.FirstSection.Body.FirstParagraph.AppendChild(group1);


            // We no longer generate unique shape identifiers when cloning existing shapes.
            GroupShape group2 = (GroupShape)group1.Clone(true);
            Assert.That(group2.Id, Is.EqualTo(100002));
            Shape shape2 = (Shape)group2.FirstChild;
            Assert.That(shape2.Id, Is.EqualTo(100001));

            // Add the second group to the header.
            Paragraph para = new Paragraph(doc);
            para.AppendChild(group2);
            HeaderFooter headerFooter = new HeaderFooter(doc, HeaderFooterType.HeaderPrimary);
            headerFooter.AppendChild(para);
            doc.FirstSection.AppendChild(headerFooter);


            // Save will generate new shape identifiers that are valid in MS Word.
            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Layout\TestShapeIdGenerate", lf, sf);

            // Check the proper identifiers for shapes in the main text were generated.
            group1 = (GroupShape)doc.FirstSection.Body.GetChild(NodeType.GroupShape, 0, true);
            Assert.That(group1.Id, Is.EqualTo(0x401));
            shape1 = (Shape)group1.FirstChild;
            Assert.That(shape1.Id, Is.EqualTo(0x402));

            // Check the proper identifiers for shapes in the header were generated.
            headerFooter = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary];
            group2 = (GroupShape)headerFooter.GetChild(NodeType.GroupShape, 0, true);
            Assert.That(group2.Id, Is.EqualTo(0x801));
            shape2 = (Shape)group2.FirstChild;
            Assert.That(shape2.Id, Is.EqualTo(0x802));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestImportShapes(LoadFormat lf, SaveFormat sf)
        {
            Document srcDoc = TestUtil.Open(@"Model\Shape\Layout\TestImportShapes", lf);

            // This is how the shapes are in the original document.
            GroupShape srcGroup = (GroupShape)srcDoc.GetChild(NodeType.GroupShape, 0, true);
            Assert.That(srcGroup.Id, Is.EqualTo(1028));
            Shape srcShape1 = (Shape)srcGroup.GetChild(NodeType.Shape, 1, false);
            Assert.That(srcShape1.Id, Is.EqualTo(1026));
            Assert.That(srcShape1.IsImage, Is.EqualTo(true));


            Document dstDoc = new Document();

            // Skip several shape ids in the destination document so it is easier to verify
            // shape ids of the imported shapes.
            for (int i = 0; i < 5; i++)
                dstDoc.GetNextShapeId();

            dstDoc.Sections.RemoveAt(0);
            dstDoc.Sections.Add(dstDoc.ImportNode(srcDoc.Sections[0], true));

            // The node importer used to generate shape id's, but not anymore.
            GroupShape dstGroup = (GroupShape)dstDoc.GetChild(NodeType.GroupShape, 0, true);
            Assert.That(dstGroup.Id, Is.EqualTo(1028));

            // The node importer used to generate shape id's, but not anymore.
            Shape dstShape0 = (Shape)dstGroup.GetChild(NodeType.Shape, 0, false);
            Assert.That(dstShape0.Id, Is.EqualTo(101379));
            Shape dstShape1 = (Shape)dstGroup.GetChild(NodeType.Shape, 1, false);
            Assert.That(dstShape1.Id, Is.EqualTo(101378));
            Assert.That(srcShape1.IsImage, Is.EqualTo(true));

            // The image bytes were not duplicated, this is correct.
            Assert.That(srcShape1.ImageData.ImageBytes == dstShape1.ImageData.ImageBytes, Is.True);

            TestUtil.SaveOpen(dstDoc, @"Model\Shape\Layout\TestImportShapes Modified", lf, sf);
        }

        /// <summary>
        /// WordArt is called GeoText in MS Word specification. In VML it is called TextPath.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestGeoText(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\WordArt\TestGeoText", lf, sf);

            // Pretty much default shape.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.TextPlainText));
            TextPath path = shape.TextPath;
            Assert.That(path.Text, Is.EqualTo("Your Text 1"));
            Assert.That(path.FitPath, Is.EqualTo(true));
            Assert.That(path.FitShape, Is.EqualTo(true));
            Assert.That(path.FontFamily, Is.EqualTo("Arial Black"));
            Assert.That(path.Size, Is.EqualTo(36.0));
            Assert.That(path.Bold, Is.EqualTo(false));
            Assert.That(path.Italic, Is.EqualTo(false));
            Assert.That(path.SmallCaps, Is.EqualTo(false));
            Assert.That(path.Shadow, Is.EqualTo(false));
            Assert.That(path.On, Is.EqualTo(true));
            Assert.That(path.Underline, Is.EqualTo(false));
            Assert.That(path.StrikeThrough, Is.EqualTo(false));
            Assert.That(path.Trim, Is.EqualTo(true));
            Assert.That(path.RotateLetters, Is.EqualTo(false));
            Assert.That(path.SameLetterHeights, Is.EqualTo(false));
            Assert.That(path.TextPathAlignment, Is.EqualTo(TextPathAlignment.Center));
            Assert.That(path.Kerning, Is.EqualTo(true));
            Assert.That(path.ReverseRows, Is.EqualTo(false));
            Assert.That(path.Spacing, Is.EqualTo(1.0));
            Assert.That(path.XScale, Is.EqualTo(false));

            // Changed some font properties.
            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            path = shape.TextPath;
            Assert.That(path.Text, Is.EqualTo("Your Text 2"));
            Assert.That(path.FontFamily, Is.EqualTo("Times New Roman"));
            Assert.That(path.Size, Is.EqualTo(20.0));
            Assert.That(path.Bold, Is.EqualTo(false));
            Assert.That(path.Italic, Is.EqualTo(true));

            // Changed more font properties.
            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            path = shape.TextPath;
            Assert.That(path.Text, Is.EqualTo("Your Text 3"));
            Assert.That(path.Bold, Is.EqualTo(true));
            Assert.That(path.Italic, Is.EqualTo(false));
            Assert.That(path.SameLetterHeights, Is.EqualTo(true));
            Assert.That(path.Kerning, Is.EqualTo(false));
            Assert.That(path.Spacing, Is.EqualTo(0.9).Within(0.01));
            Assert.That(shape.Fill.ImageBytes.Length, Is.EqualTo(3935));

            shape = (Shape)doc.GetChild(NodeType.Shape, 3, true);
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.TextCurveUp));
            path = shape.TextPath;
            Assert.That(path.Text, Is.EqualTo("Your Text 4"));
            Assert.That(path.Spacing, Is.EqualTo(1.5));

            shape = (Shape)doc.GetChild(NodeType.Shape, 4, true);
            path = shape.TextPath;
            Assert.That(path.FitShape, Is.EqualTo(false));
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTest3D(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Shape\Extrusion\Test3D", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTest3DEx(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Shape\Extrusion\Test3DEx", lf, sf);
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCallout(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Shape\Geometry\TestCallout", lf, sf);
        }




        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBackground(LoadFormat lf, SaveFormat sf)
        {
            Document srcDoc = TestUtil.OpenSaveOpen(@"Model\Shape\Background\TestBackground", lf, sf);

            Assert.That(srcDoc.BackgroundShape, IsNot.Null());
            Assert.That(srcDoc == srcDoc.BackgroundShape.Document, Is.True);
            Assert.That(srcDoc.BackgroundShape.Filled, Is.EqualTo(true));
            Assert.That(srcDoc.BackgroundShape.FillCore.FillType, Is.EqualTo(FillTypeCore.ShadeScale));
            Assert.That(srcDoc.BackgroundShape.FillCore.Angle, Is.EqualTo(-90.0));

            // Verify cloning of a document makes a background shape that belongs to the destination document.
            Document dstDoc = srcDoc.Clone();
            Assert.That(dstDoc.BackgroundShape, IsNot.Null());
            // Background shape was cloned.
            Assert.That(srcDoc.BackgroundShape != dstDoc.BackgroundShape, Is.True);
            // Background shape belongs to the new document.
            Assert.That(dstDoc == dstDoc.BackgroundShape.Document, Is.True);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBackgroundFill(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Background\TestBackgroundFill", lf, sf);

            Shape shape = doc.BackgroundShape;
            Assert.That(shape.Filled, Is.EqualTo(true));
            Assert.That(shape.FillColor.ToArgb(), Is.EqualTo(unchecked((int)0xFFFFFF00)));
            Assert.That(shape.Fill.FillType, Is.EqualTo(FillType.Solid));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBackgroundImage(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Background\TestBackgroundImage", lf, sf);

            Shape shape = doc.BackgroundShape;
            Assert.That(shape.Filled, Is.EqualTo(true));
            Assert.That(shape.Fill.FillType, Is.EqualTo(FillType.Textured));
            // The image bytes is in the fill, not in the shape itself.
            Assert.That(shape.ImageData.ImageBytes, Is.EqualTo(null));

            // RK Image fill is not supported in DOC yet.
            if (lf != LoadFormat.Doc)
                Assert.That(shape.Fill.ImageBytes != null, Is.True);
        }

        /// <summary>
        /// Test shapes with hyperlinks.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHyperlink(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestHyperlink", lf, sf);

            // This group is a canvas. The canvas itself is not hyperlinked.
            GroupShape group = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);
            Assert.That(group.HRef, Is.EqualTo(""));
            Assert.That(group.ScreenTip, Is.EqualTo(""));

            // The first shape inside the canvas is an empty picture frame (thanks to MS Word).
            // It is hyperlinked.
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.Image));
            Assert.That(shape.HRef, Is.EqualTo("mailto:romeok@rosvet.com"));
            Assert.That(shape.ScreenTip, Is.EqualTo(""));

            // Floating shape inside the canvas, hyperlinked, screen tip.
            shape = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.Ellipse));
            Assert.That(shape.HRef, Is.EqualTo("http://www.aspose.com/"));
            Assert.That(shape.ScreenTip, Is.EqualTo("Link to Aspose.com"));

            // Floating picture inside the canvas, local hyperlink.
            shape = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.Image));
            Assert.That(shape.HRef, Is.EqualTo("#bmk1"));
            Assert.That(shape.ScreenTip, Is.EqualTo(""));

            // Inline picture.
            // A HYPERLINK field in the DOC file that contains a shape.
            // In the model the field is removed and the shape itself is hyperlinked.
            shape = (Shape)doc.GetChild(NodeType.Shape, 3, true);
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.Image));
            Assert.That(shape.ImageData.IsLink, Is.EqualTo(false));
            Assert.That(shape.HRef, Is.EqualTo("http://www.aspose.com/Products/Default.aspx#Suites"));
            Assert.That(shape.ScreenTip, Is.EqualTo("Link to Suites."));

            // Inline shape.
            shape = (Shape)doc.GetChild(NodeType.Shape, 4, true);
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.Ellipse));
            Assert.That(shape.HRef, Is.EqualTo("http://www.microsoft.com/"));
            Assert.That(shape.ScreenTip, Is.EqualTo(""));
            Assert.That(shape.GetText(), Is.EqualTo("Test\r"));

            // Inline textbox. Has HYPERLINK field inside the textbox.
            shape = (Shape)doc.GetChild(NodeType.Shape, 5, true);
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.TextBox));
            Assert.That(shape.HRef, Is.EqualTo("http://www.yahoo.com/"));
            Assert.That(shape.ScreenTip, Is.EqualTo(""));
            Assert.That(shape.GetText(), Is.EqualTo("\x0013 HYPERLINK \"http://www.aspose.com/\" \x0014http://www.aspose.com/\x0015\r"));

            // Linked picture.
            shape = (Shape)doc.GetChild(NodeType.Shape, 6, true);
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.Image));
            Assert.That(shape.ImageData.IsLink, Is.EqualTo(true));
            Assert.That(shape.HRef, Is.EqualTo("http://www.test.com/"));
            Assert.That(shape.ScreenTip, Is.EqualTo(""));

            // There were several fields in the DOC for SHAPE, INCLUDEPICTURE and HYPERLINK,
            // but only one field remained in the model, it is for the hyperlink inside the textbox.
            Assert.That(doc.GetChildNodes(NodeType.FieldStart, true).Count, Is.EqualTo(1));
            Assert.That(doc.GetChildNodes(NodeType.FieldSeparator, true).Count, Is.EqualTo(1));
            Assert.That(doc.GetChildNodes(NodeType.FieldEnd, true).Count, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests hyperlink with and without a bookmark, with and without a frame, inline and float.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHyperlinkOptions(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestHyperlinkOptions", lf, sf);

            // Test hyperlinks on floating shapes.
            VerifyShapeHyperlink(doc, 0, "http://www.aspose.com/", "test title", "", false);
            VerifyShapeHyperlink(doc, 1, "http://www.aspose.com/Products/Default.aspx#Suites", "", "", false);

            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Rtf2RtfNoGold:
                    // When a floating image is hyperlinked in RTF, there is no way to store target frame.
                    VerifyShapeHyperlink(doc, 2, "http://www.aspose.com/Products/Default.aspx#Suites", "", "", false);
                    VerifyShapeHyperlink(doc, 3, "#bmk1", "", "", false);
                    break;
                default:
                    VerifyShapeHyperlink(doc, 2, "http://www.aspose.com/Products/Default.aspx#Suites", "", "_blank", false);
                    VerifyShapeHyperlink(doc, 3, "#bmk1", "", "_top", false);
                    break;
            }

            // In DOC and RTF, MS Word can store relative path, but in WML it stores absolute path.
            // It is ugly, but I leave it as is. I don't want to resolve urls myself.
            VerifyShapeHyperlink(doc, 4, "C:\\Program Files\\Automated QA\\AQtime 4\\EULA.rtf#test", "", "", false);

            // Test hyperlinks on inline shapes.
            VerifyShapeHyperlink(doc, 5, "mailto:test@test.com?subject=TestSubject", "tip", "", true);
            VerifyShapeHyperlink(doc, 6, "http://www.aspose.com/Products/Default.aspx#Suites", "", "", true);
            VerifyShapeHyperlink(doc, 7, "http://www.aspose.com/Products/Default.aspx#Suites", "", "_blank", true);
            VerifyShapeHyperlink(doc, 8, "#bmk1", "", "_top", true);
            VerifyShapeHyperlink(doc, 9, "../../../../Program%20Files/Automated%20QA/AQtime%204/EULA.rtf#test", "", "", true);
            VerifyShapeHyperlink(doc, 10, @"\\Vladimira\NetDir\SomeDirectory\Some Document.doc", "", "", true);
        }

        private static void VerifyShapeHyperlink(
            Document doc,
            int shapeIdx,
            string href,
            string title,
            string target,
            bool isInline)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, shapeIdx, true);
            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.Image));
            Assert.That(shape.HRef, Is.EqualTo(href));
            Assert.That(shape.ScreenTip, Is.EqualTo(title));
            Assert.That(shape.Target, Is.EqualTo(target));
            Assert.That(shape.IsInline, Is.EqualTo(isInline));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestShapesSimple(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Shape\Unsorted\TestShapesSimple", lf, sf);
        }


        /// <summary>
        /// WORDSNET-52 romeok - Duplicating floating images and shapes during mail merge problems.
        /// Shape ids were not recalculated and copying floating shapes created a mess of course.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCloneShape(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Unsorted\TestCloneShape", lf, sf);

            doc.Sections.Add(doc.Sections[0].Clone(true));

            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Unsorted\TestCloneShape Modified", lf, sf);

            // The dummy background shape was filtered out.
            // 0x401 - group, 0x402 - rect, 0x403 - freeform, 0x404 - oval with text.
            GroupShape group = (GroupShape)doc.GetChild(NodeType.GroupShape, 0, true);
            Assert.That(group.Id, Is.EqualTo(0x0401));

            Shape shape = (Shape)group.FirstChild;
            Assert.That(shape.Id, Is.EqualTo(0x0402));

            //0x405 - group, 0x406 - rect, 0x407 - freeform, 0x408 - oval with text.
            group = (GroupShape)doc.GetChild(NodeType.GroupShape, 1, true);
            Assert.That(group.Id, Is.EqualTo(0x0405));

            shape = (Shape)group.FirstChild;
            Assert.That(shape.Id, Is.EqualTo(0x0406));
        }


        /// <summary>
        /// WORDSNET-584 Inserted images make resulting document much larger.
        /// This was because all raster images
        /// were written as png. Now changed so it looks for the original image format. If the original
        /// image format is jpeg, writes as jpeg, otherwise writes as png.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefect584(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.InsertImage(TestUtil.BuildTestFileName(@"Model\Shape\Unsorted\TestDefect584.jpg"));

            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Unsorted\TestDefect584", lf, sf);
            string dstFileName = doc.OriginalFileName;

            int maxExpectedSize = 67 * 1024;
            // RK For RTF we need a bigger size because image data is written in hexadecimal form
            // and it is written twice (the second time for old readers).
            if (sf == SaveFormat.Rtf)
                maxExpectedSize = 190 * 1024;

            using (Stream stream = File.OpenRead(dstFileName))
                Assert.That(stream.Length < maxExpectedSize, Is.True);    //Without jpeg compression, the file was 428kb or so.
        }





        /// <summary>
        /// WORDSNET-1508 Over 1024 shapes results in invalid DOC file.
        /// </summary>
        /// <param name="lf"></param>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMany(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Layout\TestMany", lf, sf);

            Shape shape;

            Body body = doc.FirstSection.Body;
            shape = (Shape)body.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.Id, Is.EqualTo(0x0401));

            shape = (Shape)body.GetChild(NodeType.Shape, -1, true);
            Assert.That(shape.Id, Is.EqualTo(0x0840));

            HeaderFooter header = doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary];
            shape = (Shape)header.GetChild(NodeType.Shape, 0, true);
            Assert.That(shape.Id, Is.EqualTo(0x0c01));    // 0x0c00 is patriarch.

            shape = (Shape)header.GetChild(NodeType.Shape, -1, true);
            Assert.That(shape.Id, Is.EqualTo(0x1040));
        }






        /// <summary>
        /// Tests <see cref="SignatureLine"/> object properties.
        /// </summary>
        [Test]
        public void TestSignatureLineProperties()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Unsorted\TestSignatureLineProperties.docx");
            Shape shape1 = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            CheckSignatureLine(shape1.SignatureLine, "John Doe", "Manager", "john@doe.com",
                true, "", false, true);
            Shape shape2 = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            CheckSignatureLine(shape2.SignatureLine, "Tom Lane", "Developer", "tom@lane.com",
                false, "", true, false);
            Shape shape3 = (Shape)doc.GetChild(NodeType.Shape, 2, true);
            CheckSignatureLine(shape3.SignatureLine, "Sean Bean", "Manager", "sean@bean.com",
                false, "test", false, true);

            SignatureLine signatureLine1 = shape1.SignatureLine;
            signatureLine1.Signer = "Tom Lane";
            signatureLine1.SignerTitle = "Developer";
            signatureLine1.Email = "tom@lane.com";
            signatureLine1.DefaultInstructions = false;
            signatureLine1.Instructions = "test";
            signatureLine1.AllowComments = true;
            signatureLine1.ShowDate = false;

            doc = TestUtil.SaveOpen(doc, @"ImportDocx\TestSignatureLine.docx", null, false);
            shape1 = (Shape)doc.GetChild(NodeType.Shape, 0, true);
            CheckSignatureLine(shape1.SignatureLine, "Tom Lane", "Developer", "tom@lane.com",
                false, "test", true, false);
        }

        /// <summary>
        /// Checks signature line properties.
        /// </summary>
        private static void CheckSignatureLine(SignatureLine signatureLine, string expectedSigner,
            string expectedSigner2, string expectedEmail, bool expectedDefaultInstructions,
            string expectedInstructions, bool expectedAllowComments, bool expectedShowDate)
        {
            Assert.That(signatureLine, IsNot.Null(), "Wrong SignatureLine object.");
            Assert.That(signatureLine.Signer, Is.EqualTo(expectedSigner), "Wrong signer.");
            Assert.That(signatureLine.SignerTitle, Is.EqualTo(expectedSigner2), "Wrong signer title.");
            Assert.That(signatureLine.Email, Is.EqualTo(expectedEmail), "Wrong e-mail.");
            Assert.That(signatureLine.DefaultInstructions, Is.EqualTo(expectedDefaultInstructions), "Wrong default instructions flag.");
            Assert.That(signatureLine.Instructions, Is.EqualTo(expectedInstructions), "Wrong instructions.");
            Assert.That(signatureLine.AllowComments, Is.EqualTo(expectedAllowComments), "Wrong allow comments flag.");
            Assert.That(signatureLine.ShowDate, Is.EqualTo(expectedShowDate), "Wrong show date flag.");
        }



        /// <summary>
        /// WORDSNET 7021 Doc to PDF conversion issue with image rendering.
        /// The problem occurred because there is negative angle (-90).
        /// Fixed by processing this angle.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira7021(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Unsorted\TestJira7021", lf, sf);

            Node[] shapes = doc.GetChildNodes(NodeType.Shape, true).ToArray();

            foreach (Node shapeNode in shapes)
            {
                Shape shape = (Shape) shapeNode;//casting for java
                Assert.That(shape.Size.Height, Is.EqualTo(99.7).Within(0.9));
                Assert.That(shape.Size.Width, Is.EqualTo(200.4).Within(0.9));
            }
        }


        /// <summary>
        /// WORDSNET-12918 Add feature to get/set Alt Text Title property of Shape.
        /// We do not need regular unified scenarios here, we need to save Docx to Docx/Doc/Wml/Rtf/Odt,
        /// and check how the title was saved.
        /// </summary>
        [Test]
        // FOSS Doc/WordML/Rtf/Odt are removed save formats; keep the Docx roundtrip.
        [TestCase(LoadFormat.Docx, SaveFormat.Docx)]
        public void UnifiedTestJira12918(LoadFormat lf, SaveFormat sf)
        {
            SystemPal.SaveCulture();
            SystemPal.SaveUICulture();
            SystemPal.SetStandardCulture();
            SystemPal.SetStandardUICulture();
            try
            {
                Document doc = TestUtil.OpenSaveOpen(@"Model\Shape\Unsorted\TestJira12918", lf, sf);

                Node[] shapes = doc.GetChildNodes(NodeType.Shape, true).ToArray();
                Shape shape0 = (Shape)shapes[0];
                Shape shape1 = (Shape)shapes[1];
                Shape shape2 = (Shape)shapes[2];
                Shape shape3 = (Shape)shapes[3];
                Shape shape4 = (Shape)shapes[4];

                switch (sf)
                {
                    case SaveFormat.Doc:
                    case SaveFormat.WordML:
                    case SaveFormat.Rtf:
                    {
                        Assert.That(
                            shape0.AlternativeText,
                            Is.EqualTo("Title: TestShapeTitle - Description: TestShapeDescription"));
                        Assert.That(shape0.Title, Is.EqualTo(String.Empty));

                        Assert.That(
                            shape1.AlternativeText,
                            Is.EqualTo("Title: TestImageTitle - Description: TestImageDescription"));
                        Assert.That(shape1.Title, Is.EqualTo(String.Empty));

                        Assert.That(
                            shape2.AlternativeText,
                            Is.EqualTo("Title: TestChartTitle - Description: TestChartDescription"));
                        Assert.That(shape2.Title, Is.EqualTo(String.Empty));

                        Assert.That(shape3.AlternativeText, Is.EqualTo("TestImageDescription"));
                        Assert.That(shape3.Title, Is.EqualTo(String.Empty));

                        Assert.That(shape4.AlternativeText, Is.EqualTo("Title: TestImageTitle"));
                        Assert.That(shape4.Title, Is.EqualTo(String.Empty));
                        break;
                    }
                    case SaveFormat.Docx:
                    case SaveFormat.Odt:
                    {
                        Assert.That(shape0.AlternativeText, Is.EqualTo("TestShapeDescription"));
                        Assert.That(shape0.Title, Is.EqualTo("TestShapeTitle"));

                        Assert.That(shape1.AlternativeText, Is.EqualTo("TestImageDescription"));
                        Assert.That(shape1.Title, Is.EqualTo("TestImageTitle"));

                        Assert.That(shape2.AlternativeText, Is.EqualTo("TestChartDescription"));
                        Assert.That(shape2.Title, Is.EqualTo("TestChartTitle"));

                        Assert.That(shape3.AlternativeText, Is.EqualTo("TestImageDescription"));
                        Assert.That(shape3.Title, Is.EqualTo(String.Empty));

                        Assert.That(shape4.AlternativeText, Is.EqualTo(String.Empty));
                        Assert.That(shape4.Title, Is.EqualTo("TestImageTitle"));
                        break;
                    }
                    default:
                        throw new InvalidOperationException("Unknown file format.");
                }
            }
            finally
            {
                SystemPal.RestoreCulture();
                SystemPal.RestoreUICulture();
            }
        }



        [Test]
        public void TestJira6729()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Unsorted\TestJira6729.docx");
            Shape dml = (Shape)doc.GetChild(NodeType.Shape, 1, true);
            dml.Font.Hidden = true;
            TestUtil.SaveOpen(doc, @"Model\Shape\Unsorted\TestJira6729.docx");
        }



        /// <summary>
        /// WORDSNET-12060 Watermark (shape node) is lost after re-saving Docx.
        /// Watermark shape has fraction in shape type.
        /// </summary>
        [Test]
        public void TestJira12060()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Unsorted\TestJira12060.docx");
            Shape shape = (Shape)doc.FirstSection.HeadersFooters[HeaderFooterType.HeaderPrimary].GetChild(NodeType.Shape, 0, true);

            Assert.That(shape.ShapeType, Is.EqualTo(ShapeType.TextPlainText));
        }

        /// <summary>
        /// WORDSNET-12866 Provide API to set the Aspect Ratio to Scale.
        /// Made the ShapeBase.AspectRatioLocked property public.
        /// </summary>
        [Test]
        public void TestJira12866()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Unsorted\TestJira12866.docx");

            ShapeBase groupShape = GetShapeBase(doc, NodeType.GroupShape, 0);
            Shape childShape0 = (Shape)groupShape.GetChildNodes(NodeType.Any, false)[0];
            Shape childShape1 = (Shape)groupShape.GetChildNodes(NodeType.Any, false)[1];

            ShapeBase shape2 = GetShapeBase(doc, NodeType.Shape, 2);
            ShapeBase shape3 = GetShapeBase(doc, NodeType.Shape, 3);
            ShapeBase shape4 = GetShapeBase(doc, NodeType.Shape, 4);

            Assert.That(groupShape.AspectRatioLocked, Is.False, "Group Shape");
            // It is possible to get/set AspectRatioLocked for child shape (mimic MS Word),
            // but please note it has effect only for top level shapes.
            Assert.That(childShape0.AspectRatioLocked, Is.True);
            Assert.That(childShape1.AspectRatioLocked, Is.False);

            Assert.That(shape2.AspectRatioLocked, Is.False);
            Assert.That(shape3.AspectRatioLocked, Is.True);
            Assert.That(shape4.AspectRatioLocked, Is.False);

            // Reset AspectRatioLocked.
            groupShape.AspectRatioLocked = true;
            childShape0.AspectRatioLocked = false;
            childShape1.AspectRatioLocked = true;

            shape2.AspectRatioLocked = true;
            shape3.AspectRatioLocked = false;
            shape4.AspectRatioLocked = true;

            doc = TestUtil.SaveOpen(doc, @"Model\Shape\Unsorted\TestJira12866", UnifiedScenario.Docx2DocxNoGold);

            groupShape = GetShapeBase(doc, NodeType.GroupShape, 0);
            childShape0 = (Shape)groupShape.GetChildNodes(NodeType.Any, false)[0];
            childShape1 = (Shape)groupShape.GetChildNodes(NodeType.Any, false)[1];

            shape2 = GetShapeBase(doc, NodeType.Shape, 2);
            shape3 = GetShapeBase(doc, NodeType.Shape, 3);
            shape4 = GetShapeBase(doc, NodeType.Shape, 4);

            Assert.That(groupShape.AspectRatioLocked, Is.True, "Group Shape");
            Assert.That(childShape0.AspectRatioLocked, Is.False);
            Assert.That(childShape1.AspectRatioLocked, Is.True);

            Assert.That(shape2.AspectRatioLocked, Is.True);
            Assert.That(shape3.AspectRatioLocked, Is.False);
            Assert.That(shape4.AspectRatioLocked, Is.True);
        }

        /// <summary>
        /// WORDSNET-14006 Aspose.Words.FileCorruptedException is thrown while loading Docx.
        /// Shape's element 'xdr:txBody' has an undeclared namespace. MS Word ignores such shapes.
        /// </summary>
        [Test]
        public void TestJira14006()
        {
            SystemPal.SaveCulture();
            SystemPal.SaveUICulture();
            try
            {
                // Warning message can be localized.
                SystemPal.SetStandardCulture();
                SystemPal.SetStandardUICulture();

                LoadOptions lo = new LoadOptions();
                lo.WarningCallback = new WarningInfoCollection();
                lo.LoadFormat = LoadFormat.Docx;
                // Customer complaints about the document cannot be read due to filename extension.
                // Despite this is not actually a reason of the problem, lets open document with '.zip' extension.
                Document doc = TestUtil.Open(@"Model\Shape\Unsorted\TestJira14006.zip", lo);

                // Check warning is issued.
                WarningInfoCollection warnings = (WarningInfoCollection) doc.WarningCallback;
                WarningInfo warningInfo = warnings[1];
                Assert.That(warningInfo.WarningType, Is.EqualTo(WarningType.UnexpectedContent));
                Assert.That(warningInfo.Source, Is.EqualTo(WarningSource.DrawingML));
                // .Net exception:        "Shape is ignored due to: 'xdr' is an undeclared namespace."
                // NetStandard exception: "Shape is ignored due to: 'xdr' is an undeclared prefix"
                // Java one:              "Undeclared namespace prefix "xdr"..."
                Assert.That(warningInfo.Description.Contains("ndeclared"), Is.True);
                Assert.That(warningInfo.Description.Contains("xdr"), Is.True);

                NodeCollection shapes = doc.GetChildNodes(NodeType.Shape, true);

                // Check all shapes were read.
                Assert.That(shapes.Count, Is.EqualTo(2));

                // Check charts were read successfully.
                Assert.That((((Shape) shapes[1]).Chart.ChartSpace.Charts.Count), Is.EqualTo(2));

                // Check that group shape with shapes that have bad namespaces was not read.
                ShapeBase groupShape = GetShapeBase(doc, NodeType.GroupShape, 0);
                Assert.That(groupShape, Is.Null);
            }
            finally
            {
                SystemPal.RestoreCulture();
                SystemPal.RestoreUICulture();
            }
        }

        /// <summary>
        /// WORDSNET-14012 Generating of the shapes identifiers.
        /// Pre-increment have to be used inside GetNextShapeId to avoid duplicate shape identifiers.
        /// </summary>
        [Test]
        public void TestJira14012()
        {
            Document doc = new Document();
            doc.SetNextShapeId(50);

            // GetNextShapeId have to return 50 + 1. In case of using post-increment it returns already used value (50).
            Assert.That(doc.GetNextShapeId(), Is.EqualTo(51));
        }

        private ShapeBase GetShapeBase(Document doc, NodeType shapeType, int index)
        {
            return (ShapeBase)doc.GetChild(shapeType, index, true);
        }

        [Test]
        public void TestAspectRatioLocked()
        {
            Document doc = new Document();

            DocumentBuilder builder = new DocumentBuilder(doc);

            const double frameHeight = 100;
            const double frameWidth = 300;
            Shape img = builder.InsertImage(TestUtil.BuildTestFileName(@"DocBuilder\Image\TestPng.png"));

            // Set AspectRatioLocked equals true and try to reset shape height.
            img.AspectRatioLocked = true;

            img.Height = frameHeight;
            if (img.Width > frameWidth)
                img.Width = frameWidth;

            Assert.That(img.Height, Is.EqualTo(100));
            Assert.That(img.Width, Is.EqualTo(100));
        }


        // FOSS TestJira14927 removed: it verifies the Fill type / Recolor flags written during the DML->VML
        // conversion when saving DOCX to WML, a removed format.



        /// <summary>
        /// WORDSNET-26193 Make ShapeBase.Hidden property public.
        /// Tests public property Shape.Hidden.
        /// </summary>
        [Test]
        public void Test26193()
        {
            const string testFileName = @"Model\Shape\Unsorted\Test26193";
            Document doc = TestUtil.Open(testFileName, LoadFormat.Docx);

            Shape shape1 = doc.FirstSection.Body.Shapes[0];
            Shape shape2 = doc.FirstSection.Body.Shapes[1];

            Assert.That(shape1.Hidden, Is.True);
            Assert.That(shape2.Hidden, Is.False);

            shape1.Hidden = false;
            shape2.Hidden = true;

            doc = TestUtil.SaveOpen(doc, testFileName, UnifiedScenario.Docx2DocxNoGold);

            Assert.That(doc.FirstSection.Body.Shapes[0].Hidden, Is.False);
            Assert.That(doc.FirstSection.Body.Shapes[1].Hidden, Is.True);
        }
    }
}
