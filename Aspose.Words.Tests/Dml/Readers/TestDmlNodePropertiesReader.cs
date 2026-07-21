// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Transforms;
using Aspose.Words.RW.Dml.Reader;
using Aspose.Words.Tests.Model;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlShapeReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlNodePropertiesReader
    {
        [Test]
        public void ReadVisualGroupShapeProperties_ExtentDefined_ExtentHasCorrectValues()
        {
            // Arrange
            string xml =
                "<a:grpSpPr bwMode=\"invGray\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:xfrm>" +
                        "<a:ext cx=\"1676400\" cy=\"1095375\" />" +
                    "</a:xfrm>" +
                "</a:grpSpPr>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            DmlGroupShape groupShape = new DmlGroupShape(DmlNodeType.GroupShape);
            // Act
            DmlNodePropertiesReader.ReadVisualGroupShapeProperties(reader, groupShape);
            // Assert
            Assert.That(groupShape.Transform.GetExtents().Width, Is.EqualTo(1676400f));
            Assert.That(groupShape.Transform.GetExtents().Height, Is.EqualTo(1095375f));
            Assert.That(groupShape.BWMode, Is.EqualTo(BWMode.InverseGray));
        }

        [Test]
        public void ReadShapeProperties_CustomGeometry_GeometrySetted()
        {
            string xml =
                "<a:spPr xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:custGeom>" +
                    "</a:custGeom>" +
                "</a:spPr>";
            BuildAndCheckGeometry(xml);
        }

        [Test]
        public void ReadShapeProperties_OutlineSpecified_PropertyInitialized()
        {
            // Arrange
            string xml =
                "<a:spPr xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:ln/>" +
                "</a:spPr>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            DmlShape shape = new DmlShape(DmlNodeType.Shape);
            // Act
            DmlNodePropertiesReader.ReadShapeProperties(reader, shape);
            // Assert
            Assert.That(shape.Outline, IsNot.Null());
        }

        [Test]
        public void ReadShapeProperties_PresetGeometry_GeometrySetted()
        {
            string xml =
                "<a:spPr xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:prstGeom>" +
                    "</a:prstGeom>" +
                "</a:spPr>";
            BuildAndCheckGeometry(xml);
        }

        [Test]
        public void ReadShapeProperties_XfrmDefined_TrasformPropertiesSetted()
        {
            // Arrange
            string xml =
                "<a:spPr xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:xfrm>" +
                        "<a:ext cx=\"12\" cy=\"32\"/>" +
                        "<a:off x=\"54\" y=\"72\"/>" +
                    "</a:xfrm>" +
                "</a:spPr>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            DmlShape shape = new DmlShape(DmlNodeType.Shape);
            // Act
            DmlNodePropertiesReader.ReadShapeProperties(reader, shape);
            // Assert
            DmlTransform transform = shape.Transform;
            Assert.That(transform.Width, Is.EqualTo(12));
            Assert.That(transform.Height, Is.EqualTo(32));
            Assert.That(transform.X, Is.EqualTo(54));
            Assert.That(transform.Y, Is.EqualTo(72));
        }

        private void BuildAndCheckGeometry(string xml)
        {
            // Arrange
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            DmlShape shape = new DmlShape(DmlNodeType.Shape);
            // Act
            DmlNodePropertiesReader.ReadShapeProperties(reader, shape);
            // Assert
            Assert.That(shape.Geometry, IsNot.Null());
            Assert.That(shape.Geometry.Guides.AdjustableValues.Count, Is.EqualTo(0));
            Assert.That(shape.Geometry.Guides.Guides.Count, Is.EqualTo(0));
            Assert.That(shape.Geometry.Paths.Count, Is.EqualTo(0));
            Assert.That(StringUtil.HasChars(shape.Geometry.PresetName), Is.False);
        }

        /// <summary>
        /// WORDSNET-11899 DmlNodePropertiesReader.ReadHLinkClick hanged if there was an unknown 
        /// attribute in the hlinkClick element. 
        /// WarnUnexcepted should be used instead of WarnUnexceptedAndIgnoreElement in attribute cycle.
        /// </summary>
        [Test]
        public void ReadNonVisualDrawingProperties_UnknownHLinkClickAttribute()
        {
            // Checks that reader does not hang
            // Arrange
            string xml =
                "<pic:pic xmlns:pic=\"http://schemas.openxmlformats.org/drawingml/2006/main\" " +
                    "xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" " +
                    "xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" >" +
                    "<pic:nvPicPr>" +
                        "<pic:cNvPr id=\"1026\" name=\"Picture 2\">" +
                            "<a:hlinkClick r:id=\"rId19\" action1=\"ppaction://hlinkfile\"/>" + // unknown attribute action1 
                        "</pic:cNvPr>" +
                    "</pic:nvPicPr>" +
                "</pic:pic>";
            TestMaxWarningCountCallback warnCounter = new TestMaxWarningCountCallback(1);
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml, warnCounter);
            reader.XmlReader.ReadChild("nvPicPr");
            reader.XmlReader.ReadChild("cNvPr");
            // Act
            DmlNodePropertiesReader.ReadNonVisualDrawingProperties(reader);
            // Assert
            Assert.That(warnCounter.Count, Is.EqualTo(1), "Number of unknown nodes");
        }

    }
}
