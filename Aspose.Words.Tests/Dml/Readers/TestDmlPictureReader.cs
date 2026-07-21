// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.RW.Dml.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlShapeReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlPictureReader
    {
        [Test]
        public void Build_BadXml_NullReturned()
        {
            // Arrange
            string xml =
                "<a:Badpic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "</a:Badpic>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            Shape result = DmlPictureReader.Read(reader);
            // Assert
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public void Build_StyleSpecified_PropertyInitialized()
        {
            // Arrange
            string xml =
                "<a:pic xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                    "<a:style>" +
                    "</a:style>" +
                "</a:pic>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            Shape result = DmlPictureReader.Read(reader);
            // Assert
            Assert.That(result, IsNot.Null());
            Assert.That(result.DmlNode, IsNot.Null());
            Assert.That(result.DmlNode as DmlPicture, IsNot.Null());
            Assert.That((result.DmlNode as DmlPicture).Style, IsNot.Null());
        }

        [Test]
        public void Build_BlipFillSpecified_PropertyInitialized()
        {
            // Arrange
            string xml =
                "<pic >" +
                    "<blipFill>" +
                    "</blipFill>" +
                "</pic>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            Shape result = DmlPictureReader.Read(reader);
            // Assert
            Assert.That(result, IsNot.Null());
            Assert.That(result.DmlNode, IsNot.Null());

            DmlPicture dmlPicture = result.DmlNode as DmlPicture;
            Assert.That(dmlPicture, IsNot.Null());
            Assert.That(dmlPicture.BlipFill, IsNot.Null());
            Assert.That(dmlPicture.BlipFill.DmlFillType, Is.EqualTo(DmlFillType.BlipFill));
            Assert.That(dmlPicture.BlipFill.Blip, IsNot.Null());
            Assert.That(dmlPicture.BlipFill.Blip.EmbedImage, Is.Null);
            Assert.That(StringUtil.HasChars(dmlPicture.BlipFill.Blip.ImageLink), Is.False);
        }
    }
}
