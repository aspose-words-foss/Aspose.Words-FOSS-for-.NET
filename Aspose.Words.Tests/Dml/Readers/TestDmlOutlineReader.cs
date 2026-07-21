// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/01/2011 by Alexey Titov

using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.RW.Dml.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlOutlineReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlOutlineReader
    {
        [Test]
        public void Build_EmptyTag_DefaultValuesAreCorrect()
        {
            // Arrange
            string xml = "<ln/>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlOutline result = DmlOutlineReader.Read(reader);
            // Assert
            Assert.That(result.StrokeAlignment, Is.False);
            Assert.That(result.CompoundLineType, Is.EqualTo(ShapeLineStyle.Single));
            Assert.That(result.LineEndingCapType, Is.EqualTo(EndCap.Flat));
            Assert.That(result.WidthInEmus, Is.EqualTo(0.0));
            Assert.That(((DmlPresetDash)result.Dash).Preset, Is.EqualTo(DashStyle.Solid));
            Assert.That(result.Fill.DmlFillType, Is.EqualTo(DmlFillType.NoFill));
            Assert.That(result.HeadLineEndStyle, Is.EqualTo(new DmlHeadLineEndStyle()));
            Assert.That(result.TailLineEndStyle, Is.EqualTo(new DmlTailLineEndStyle()));
        }

        [Test]
        public void Build_Attributes_PropertiesAreInitialized()
        {
            // Arrange
            string xml = "<ln algn=\"in\" cap=\"flat\" cmpd=\"tri\" w=\"12345\">" +
                         "</ln>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlOutline result = DmlOutlineReader.Read(reader);
            // Assert
            Assert.That(result.StrokeAlignment, Is.True);
            Assert.That(result.CompoundLineType, Is.EqualTo(ShapeLineStyle.Triple));
            Assert.That(result.LineEndingCapType, Is.EqualTo(EndCap.Flat));
            Assert.That(result.WidthInEmus, Is.EqualTo(12345.0));
        }

        [Test]
        public void Build_SolidFillSpecified_PropertyInitialized()
        {
            // Arrange
            string xml = "<ln>" +
                            "<solidFill><schemeClr val=\"accent1\" /></solidFill>" +
                         "</ln>";
            CheckFillProperty(xml, DmlFillType.SolidFill);
        }

        [Test]
        public void Build_NoFillSpecified_PropertyInitialized()
        {
            // Arrange
            string xml = "<ln>" +
                            "<noFill/>" +
                         "</ln>";
            CheckFillProperty(xml, DmlFillType.NoFill);
        }

        [Test]
        public void Build_PatternFillSpecified_PropertyInitialized()
        {
            // Arrange
            string xml = "<ln>" +
                            "<pattFill/>" +
                         "</ln>";
            CheckFillProperty(xml, DmlFillType.PatternFill);
        }

        [Test]
        public void Build_GradFillSpecified_PropertyInitialized()
        {
            // Arrange
            string xml = "<ln>" +
                            "<gradFill/>" +
                         "</ln>";
            CheckFillProperty(xml, DmlFillType.GradientFill);
        }


        [Test]
        public void Build_PresetDashSpecified_PropertyInitialized()
        {
            // Arrange
            string xml = "<ln>" +
                            "<prstDash val=\"dot\"/>" +
                         "</ln>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlOutline result = DmlOutlineReader.Read(reader);
            // Assert
            Assert.That(((DmlPresetDash)result.Dash).Preset, Is.EqualTo(DashStyle.Dot));
        }

        [Test]
        public void Build_CustomDashSpecified_PropertyInitialized()
        {
            // Arrange
            string xml = "<ln>" +
                            "<custDash>" +
                                "<ds/>" +
                                "<ds d=\"100\" sp=\"10000\"/>" +
                            "</custDash>" +
                         "</ln>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlOutline result = DmlOutlineReader.Read(reader);
            DmlCustomDash dash = (DmlCustomDash)result.Dash;
            // Assert
            Assert.That(dash.DashStops.Count, Is.EqualTo(2));
            Assert.That(DmlPercentageUtil.ToDmlPercent(dash.DashStops[1].DashLength), Is.EqualTo(100.0));
            Assert.That(DmlPercentageUtil.ToDmlPercent(dash.DashStops[1].SpaceLength), Is.EqualTo(10000.0));
        }

        [Test]
        public void Build_TailEndSpecified_PropertyInitialized()
        {
            // Arrange
            string xml = "<ln>" +
                            "<tailEnd len=\"med\" type=\"diamond\" w=\"lg\"/>" +
                         "</ln>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlOutline result = DmlOutlineReader.Read(reader);
            // Assert
            Assert.That(result.TailLineEndStyle.Length, Is.EqualTo(ArrowLength.Medium));
            Assert.That(result.TailLineEndStyle.Width, Is.EqualTo(ArrowWidth.Wide));
            Assert.That(result.TailLineEndStyle.Type, Is.EqualTo(ArrowType.Diamond));
        }

        [Test]
        public void Build_HeadEndSpecified_PropertyInitialized()
        {
            // Arrange
            string xml = "<ln>" +
                            "<headEnd  len=\"sm\" type=\"arrow\" w=\"med\"/>" +
                         "</ln>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlOutline result = DmlOutlineReader.Read(reader);
            // Assert
            Assert.That(result.HeadLineEndStyle.Length, Is.EqualTo(ArrowLength.Short));
            Assert.That(result.HeadLineEndStyle.Width, Is.EqualTo(ArrowWidth.Medium));
            Assert.That(result.HeadLineEndStyle.Type, Is.EqualTo(ArrowType.Open));
        }

        private void CheckFillProperty(string xml, DmlFillType expectedFillType)
        {
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlOutline result = DmlOutlineReader.Read(reader);
            // Assert
            Assert.That(result.Fill, IsNot.Null());
            Assert.That(result.Fill.DmlFillType, Is.EqualTo(expectedFillType));
        }
    }
}