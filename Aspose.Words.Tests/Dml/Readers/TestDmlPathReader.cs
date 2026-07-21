// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Common;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Path;
using Aspose.Words.Drawing.Core.Dml.Readers;
using Aspose.Words.Nrx;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlPathReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlPathReader
    {
        private object GetPathPart(string xml)
        {
            // Arrange
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlPath result = DmlPathReader.Read(reader, null);
            // Assert
            Assert.That(result.PathParts.Count, Is.EqualTo(1));

            return result.PathParts[0];
        }

        private void CheckMoveToPoint(string xml, string x, string y)
        {
            DmlMoveTo result = BuildMoveTo(xml);
            CheckPoint(result.Point, x, y);
        }

        private static void CheckPoint(DmlAdjustablePoint point, string x, string y)
        {
            Assert.That(point.X.String, Is.EqualTo(x));
            Assert.That(point.Y.String, Is.EqualTo(y));
        }

        private void CheckMoveToPoint(string xml, double x, double y)
        {
            DmlMoveTo result = BuildMoveTo(xml);
            CheckPoint(result.Point, FormatterPal.DoubleToStr(x), FormatterPal.DoubleToStr(y));
        }

        private DmlMoveTo BuildMoveTo(string xml)
        {
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlPath path = DmlPathReader.Read(reader, null);
            return (DmlMoveTo)path.PathParts[0];
        }

        [Test]
        public void Build_PathWithAttributes_AttributesReaded()
        {
            // Arrange
            string xml =
                "<a:path w=\"101\" h=\"102\" stroke=\"false\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "</a:path>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlPath result = DmlPathReader.Read(reader, null);
            // Assert
            Assert.That(result.Width, Is.EqualTo(101));
            Assert.That(result.Height, Is.EqualTo(102));
            Assert.That(result.Stroke, Is.EqualTo(false));
        }
        [Test]
        public void Build_BadXml_NullReturned()
        {
            // Arrange
            string xml =
                "<a:Badpath xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "</a:Badpath>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlPath result = DmlPathReader.Read(reader, null);
            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Build_LineToWithXandYDefined_PointReadedSuccesefuly()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:lnTo> " +
                "<a:pt x=\"77\" y=\"66\"/>" +
                "</a:lnTo> " +
                "</a:path>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlPath path = DmlPathReader.Read(reader, null);
            // Assert
            DmlLineTo result = (DmlLineTo)path.PathParts[0];
            Assert.That(result.Point.X.String, Is.EqualTo("77"));
            Assert.That(result.Point.Y.String, Is.EqualTo("66"));
        }

        [Test]
        public void Build_MoveToWithXandYDefined_PointReadedSuccesefuly()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:moveTo> " +
                "<a:pt x=\"77\" y=\"66\"/>" +
                "</a:moveTo> " +
                "</a:path>";
            CheckMoveToPoint(xml, 77, 66);
        }

        [Test]
        public void Build_MoveToWithXDefined_PointReadedSuccesefuly()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:moveTo> " +
                "<a:pt x=\"77\" />" +
                "</a:moveTo> " +
                "</a:path>";
            CheckMoveToPoint(xml, 77, 0);
        }

        [Test]
        public void Build_MoveToWithXYDefinedAsGuideName_PointReadedSuccesefuly()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:moveTo> " +
                "<a:pt x=\"abc\" y=\"def\" />" +
                "</a:moveTo> " +
                "</a:path>";
            CheckMoveToPoint(xml, "abc", "def");
        }

        [Test]
        public void Build_MoveToWithYDefined_PointReadedSuccesefuly()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:moveTo> " +
                "<a:pt y=\"77\" />" +
                "</a:moveTo> " +
                "</a:path>";
            CheckMoveToPoint(xml, 0, 77);
        }

        [Test]
        public void Build_PathDefined_WidthAndHeightSetted()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" w=\"2824222\" h=\"590309\">" +
                "</a:path>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlPath result = DmlPathReader.Read(reader, null);
            // Assert
            Assert.That(result.Width, Is.EqualTo(2824222.0));
            Assert.That(result.Height, Is.EqualTo(590309.0));
        }

        [Test]
        public void Build_PathWithArcTo_ArcToAdded()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:arcTo/> " +
                "</a:path>";
            // Assert
            Assert.That(GetPathPart(xml) is DmlArcTo, Is.True);
        }

        [Test]
        public void Build_PathWithArcTo_ArcToPropertiesDefined()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:arcTo hR=\"12556\" wR=\"trololo\" stAng=\"path\" swAng=\"34958\" /> " +
                "</a:path>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlPath path = DmlPathReader.Read(reader, null);
            DmlArcTo result = (DmlArcTo)path.PathParts[0];
            // Assert
            Assert.That(result.HeightRadius.String, Is.EqualTo("12556"));
            Assert.That(result.WidthRadius.String, Is.EqualTo("trololo"));
            Assert.That(result.SwingAngle.String, Is.EqualTo("34958"));
            Assert.That(result.StartAngle.String, Is.EqualTo("path"));
        }

        [Test]
        public void Build_PathWithClose_CloseAdded()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:close/> " +
                "</a:path>";
            // Assert
            Assert.That(GetPathPart(xml) is DmlClose, Is.True);
        }

        [Test]
        public void Build_PathWithCubicBezTo_CubicBezierToAdded()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:cubicBezTo/> " +
                "</a:path>";
            // Assert
            Assert.That(GetPathPart(xml) is DmlCubicBezierTo, Is.True);
        }

        [Test]
        public void Build_PathWithCubicBezToWithoutPoints_CubicBezToInitialized()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:cubicBezTo> " +
                "<a:pt x=\"77\" y=\"66\"/>" +
                "<a:pt x=\"88\" y=\"99\"/>" +
                "<a:pt x=\"1\" y=\"42\"/>" +
                "</a:cubicBezTo> " +
                "</a:path>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlPath path = DmlPathReader.Read(reader, null);
            DmlCubicBezierTo result = (DmlCubicBezierTo)path.PathParts[0];
            // Assert
            Assert.That(result.ControlPoint1.X.String, Is.EqualTo("77"));
            Assert.That(result.ControlPoint1.Y.String, Is.EqualTo("66"));
            Assert.That(result.ControlPoint2.X.String, Is.EqualTo("88"));
            Assert.That(result.ControlPoint2.Y.String, Is.EqualTo("99"));
            Assert.That(result.EndPoint.X.String, Is.EqualTo("1"));
            Assert.That(result.EndPoint.Y.String, Is.EqualTo("42"));
        }

        [Test]
        public void Build_PathWithLineTo_LineToAdded()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:lnTo/> " +
                "</a:path>";
            // Assert
            Assert.That(GetPathPart(xml) is DmlLineTo, Is.True);
        }

        [Test]
        public void Build_PathWithMoveTo_MoveToAdded()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:moveTo/> " +
                "</a:path>";
            // Assert
            Assert.That(GetPathPart(xml) is DmlMoveTo, Is.True);
        }

        [Test]
        public void Build_PathWithQuadBezTo_QuadraticBezierToAdded()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:quadBezTo/> " +
                "</a:path>";
            // Assert
            Assert.That(GetPathPart(xml) is DmlQuadraticBezierTo, Is.True);
        }

        [Test]
        public void Build_PathWithQuadBezToWithoutPoints_QuadBezToInidialized()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:quadBezTo> " +
                "<a:pt x=\"77\" y=\"66\"/>" +
                "<a:pt x=\"88\" y=\"99\"/>" +
                "</a:quadBezTo> " +
                "</a:path>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlPath path = DmlPathReader.Read(reader, null);
            DmlQuadraticBezierTo result = (DmlQuadraticBezierTo)path.PathParts[0];
            // Assert
            Assert.That(result.ControlPoint.X.String, Is.EqualTo("77"));
            Assert.That(result.ControlPoint.Y.String, Is.EqualTo("66"));
            Assert.That(result.EndPoint.X.String, Is.EqualTo("88"));
            Assert.That(result.EndPoint.Y.String, Is.EqualTo("99"));
        }

        [Test]
        public void Build_PathWithSeveralParts_AllPartsAdded()
        {
            // Arrange
            string xml =
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">" +
                "<a:moveTo/> " +
                "<a:lnTo/> " +
                "<a:lnTo/> " +
                "<a:close/> " +
                "</a:path>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlPath result = DmlPathReader.Read(reader, null);
            // Assert
            Assert.That(result.PathParts.Count, Is.EqualTo(4));
        }

        [Test]
        public void Build_FillNotSpecified_PropertyInitialized()
        {
            CheckFillProperty(
                "<a:path xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"/>", 
                DmlPathFillMode.Norm);
        }

        private void CheckFillProperty(string xml, DmlPathFillMode expected)
        {
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlPath result = DmlPathReader.Read(reader, null);
            // Assert
            Assert.That(result.FillMode, Is.EqualTo(expected));
        }

        [Test]
        public void Build_FillNoneSpecified_PropertyInitialized()
        {
            CheckFillProperty(
                "<a:path fill=\"none\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"/>", 
                DmlPathFillMode.None);
        }

        [Test]
        public void Build_FillNormSpecified_PropertyInitialized()
        {
            CheckFillProperty(
                "<a:path fill=\"norm\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"/>",
                DmlPathFillMode.Norm);
        }

        [Test]
        public void Build_FillDarkenSpecified_PropertyInitialized()
        {
            CheckFillProperty(
                "<a:path fill=\"darken\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"/>",
                DmlPathFillMode.Darken);
        }

        [Test]
        public void Build_FillDarkenLessSpecified_PropertyInitialized()
        {
            CheckFillProperty(
                "<a:path fill=\"darkenLess\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"/>",
                DmlPathFillMode.DarkenLess);
        }

        [Test]
        public void Build_FillLightenSpecified_PropertyInitialized()
        {
            CheckFillProperty(
                "<a:path fill=\"lighten\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"/>",
                DmlPathFillMode.Lighten);
        }

        [Test]
        public void Build_FillLightenLessSpecified_PropertyInitialized()
        {
            CheckFillProperty(
                "<a:path fill=\"lightenLess\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"/>",
                DmlPathFillMode.LightenLess);
        }

        [Test]
        public void Build_BadFillSpecified_PropertyInitialized()
        {
            CheckFillProperty(
                "<a:path fill=\"sadf\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"/>",
                DmlPathFillMode.Norm);
        }
    }
}