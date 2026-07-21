// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/12/2010 by Alexey Titov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml.Transforms;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Dml.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlTransformReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlTransformReader
    {
        [Test]
        public void BuildGroupTransform_AllPropertiesSpecified_AllPropertiesReaded()
        {
            // Arrange
            string xml = "<a:xfrm xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" "+
                            "flipH=\"1\" flipV=\"true\" rot=\"3600000\">" +
                            "<a:ext cx=\"1676400\" cy=\"1095375\" />" +
                            "<a:off x=\"6400\" y=\"109\" />" +
                            "<a:chExt cx=\"64001\" cy=\"1091\" />" +
                            "<a:chOff x=\"1\" y=\"2\" />" +
                         "</a:xfrm>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlGroupTransform result = DmlTransformReader.ReadGroupTransform(reader, null);
            // Assert
            Assert.That(result.Rotation.Value, Is.EqualTo(3600000.0));
            Assert.That(result.FlipOrientation, Is.EqualTo(FlipOrientation.Both));
            Assert.That(result.Width, Is.EqualTo(1676400.0));
            Assert.That(result.Height, Is.EqualTo(1095375.0));
            Assert.That(result.X, Is.EqualTo(6400.0));
            Assert.That(result.Y, Is.EqualTo(109.0));
            Assert.That(result.ChildWidth, Is.EqualTo(64001.0));
            Assert.That(result.ChildHeight, Is.EqualTo(1091.0));
            Assert.That(result.ChildX, Is.EqualTo(1.0));
            Assert.That(result.ChildY, Is.EqualTo(2.0));
        }

        [Test]
        public void BuildTransform_AllPropertiesSpecified_AllPropertiesReaded()
        {
            // Arrange
            string xml = "<a:xfrm xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" " +
                            "flipH=\"1\" flipV=\"true\" rot=\"3600000\">" +
                            "<a:ext cx=\"1676400\" cy=\"1095375\" />" +
                            "<a:off x=\"6400\" y=\"109\" />" +
                         "</a:xfrm>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlTransform result = DmlTransformReader.ReadTransform(reader, null);
            // Assert
            Assert.That(result.Rotation.Value, Is.EqualTo(3600000.0));
            Assert.That(result.FlipOrientation, Is.EqualTo(FlipOrientation.Both));
            Assert.That(result.Width, Is.EqualTo(1676400.0));
            Assert.That(result.Height, Is.EqualTo(1095375.0));
            Assert.That(result.X, Is.EqualTo(6400.0));
            Assert.That(result.Y, Is.EqualTo(109.0));
        }

    }
}