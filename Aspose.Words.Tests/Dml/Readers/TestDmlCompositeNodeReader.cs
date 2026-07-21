// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.RW.Dml.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlCompositeNodeReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlCompositeNodeReader
    {
        [Test]
        public void Build_BadXml_NullReturned()
        {
            // Arrange
            string xml =
                "<lc:badLockedCanvas xmlns:lc=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">" +
                "</lc:badLockedCanvas>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            ShapeBase result = DmlCompositeNodeReader.ReadLockedCanvas(reader);
            // Assert
            Assert.That(result, Is.Null);
        }



        [Test]
        public void Build_ShapeDefined_ShapeCreated()
        {
            string xml =
                "<lc:lockedCanvas xmlns:lc=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">" +
                "<a:sp xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"/>" +
                "</lc:lockedCanvas>";
            BuildAndCheckThatShapeCreated(xml);
        }

        [Test]
        public void Build_GroupShapeDefined_ShapeCreated()
        {
            string xml =
                "<lc:lockedCanvas xmlns:lc=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">" +
                    "<a:grpSp xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"/>" +
                "</lc:lockedCanvas>";
            BuildAndCheckThatShapeCreated(xml);
        }

        [Test]
        public void Build_GroupTreeDefined_NestedShapesCreated()
        {
            // Arrange
            string xml =
                "<lc:lockedCanvas xmlns:lc=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">" +
                    "<a:grpSp xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                        "<a:grpSp >" +
                            "<a:grpSp />" +
                        "</a:grpSp>" +
                    "</a:grpSp>" +
                "</lc:lockedCanvas>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            ShapeBase drawingML;
            // Act
            drawingML = DmlCompositeNodeReader.ReadLockedCanvas(reader);
            // Assert
            Assert.That(drawingML.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            GroupShape groupDml = (GroupShape)drawingML.GetChildNodes(NodeType.Any, false)[0];
            Assert.That(groupDml.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            GroupShape nestedGroupDml = (GroupShape)groupDml.GetChildNodes(NodeType.Any, false)[0];
            Assert.That(nestedGroupDml.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
        }


        [Test]
        public void Build_CxnShapeDefined_ShapeCreated()
        {
            string xml =
                "<lc:lockedCanvas xmlns:lc=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">" +
                "<a:cxnSp xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"/>" +
                "</lc:lockedCanvas>";
            BuildAndCheckThatShapeCreated(xml);
        }

        [Test]
        public void Build_PictureDefined_ShapeCreated()
        {
            string xml =
                "<lc:lockedCanvas xmlns:lc=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">" +
                    "<a:pic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"/>" +
                "</lc:lockedCanvas>";
            DmlNode node = BuildAndCheckThatShapeCreated(xml);
            Assert.That(node is DmlLockedCanvas, Is.True);
        }

        private DmlNode BuildAndCheckThatShapeCreated(string xml)
        {
            Shape drawingML = new Shape(new Document(), ShapeMarkupLanguage.Dml);
            // Arrange
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            reader.AddAndPushContainer(drawingML);
            // Act
            DmlCompositeNodeReader.ReadLockedCanvas(reader);
            // Assert
            Assert.That(drawingML.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(drawingML.GetChildNodes(NodeType.Any, false)[0], IsNot.Null());
            return ((GroupShape)drawingML.GetChildNodes(NodeType.Any, false)[0]).DmlNode;
        }

        [Test]
        public void Build_TagIsEmpty_ResultIsInitializedWithDefaultValues()
        {
            // Arrange
            string xml =
                "<lc:lockedCanvas xmlns:lc=\"http://schemas.openxmlformats.org/drawingml/2006/lockedCanvas\">" +
                "</lc:lockedCanvas>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            ShapeBase result = DmlCompositeNodeReader.ReadLockedCanvas(reader);
            // Assert
            Assert.That(result.DmlNode, IsNot.Null());
            Assert.That(result.DmlNode.Transform, IsNot.Null());
            Assert.That(result.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(0));
        }
    }
}