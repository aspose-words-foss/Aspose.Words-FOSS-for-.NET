// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.RW.Dml.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlShapeReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlShapeReader
    {
        [Test]
        public void Build_BadXml_NullReturned()
        {
            // Arrange
            string xml =
                "<a:Badsp xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "</a:Badsp>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            Shape result = DmlShapeReader.Read(reader);
            // Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public void Build_StyleSpecified_PropertyInitialized()
        {
            // Arrange
            string xml =
                "<a:sp xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:style>" +
                    "</a:style>" +
                "</a:sp>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            Shape result = DmlShapeReader.Read(reader);
            // Assert
            Assert.That(result, IsNot.Null());
            Assert.That(result.DmlNode, IsNot.Null());
            Assert.That(result.DmlNode as DmlShape, IsNot.Null());
            Assert.That((result.DmlNode as DmlShape).Style, IsNot.Null());
        }
    }
}