// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2010 by Alexey Titov
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.RW.Dml.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlFillReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlFillReader
    {
        [Test]
        public void Build_BlipFillWithAttributes_CorrespondingPropertiesInitialized()
        {
            // Arrange
            string xml = "<blipFill dpi=\"120\" rotWithShape=\"true\"/>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlBlipFill result = (DmlBlipFill)DmlFillReader.Read(reader);
            // Assert
            Assert.That(result.DotsPerInch, Is.EqualTo(120));
            Assert.That(result.RotateWithShape, Is.EqualTo(true));
        }

        [Test]
        public void Build_BlipFillWithStretch_StretchInitialized()
        {
            // Arrange
            string xml = "<blipFill>" +
                            "<stretch>" +
                                "<fillRect l=\"50000\" t=\"60000\" r=\"70000\" b=\"80000\" />" +
                            "</stretch>" +
                         "</blipFill>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlBlipFill result = (DmlBlipFill)DmlFillReader.Read(reader);
            // Assert
            DmlBlipFillStretch dmlBlipFillStretch = ((DmlBlipFillStretch)result.BlipFillMode);
            Assert.That(dmlBlipFillStretch.FillRectangle, Is.EqualTo(new DmlPercentageOffsetRectangle(0.5, 0.6, 0.7, 0.8)));
        }

        [Test]
        public void Build_BlipFillWithSourceRectangle_SourceRectangleInitialized()
        {
            // Arrange
            string xml = "<blipFill>" +
                             "<srcRect l=\"50000\" t=\"60000\" r=\"70000\" b=\"80000\" />" +
                         "</blipFill>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlBlipFill result = (DmlBlipFill)DmlFillReader.Read(reader);
            // Assert
            Assert.That(result.SourceRectangle, Is.EqualTo(new DmlPercentageOffsetRectangle(0.5, 0.6, 0.7, 0.8)));
        }

        [Test]
        public void Build_BlipFillWithTile_TileInitialized()
        {
            // Arrange
            string xml = "<blipFill>" +
                             "<tile algn=\"ctr\" flip=\"y\" sx=\"50000\" sy=\"100000\" tx=\"100\" ty=\"200\"/>" +
                         "</blipFill>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlBlipFill result = (DmlBlipFill)DmlFillReader.Read(reader);
            // Assert
            DmlBlipFillTile tile = (DmlBlipFillTile)result.BlipFillMode;
            Assert.That(tile.Alignment, Is.EqualTo(DmlRectangleAlignment.Center));
            Assert.That(tile.TileFlipMode, Is.EqualTo(DmlTileFlipMode.Vertical));
            Assert.That(tile.HorizontalRatio, Is.EqualTo(0.5));
            Assert.That(tile.VerticalRatio, Is.EqualTo(1.0));
            Assert.That(tile.HorizontalOffset, Is.EqualTo(100.0));
            Assert.That(tile.VerticalOffset, Is.EqualTo(200.0));
        }

        [Test]
        public void Build_BlipFillWithBilp_BlipInitialized()
        {
            // Arrange
            string xml = "<blipFill>" +
                            "<blip embed=\"rId2\" link=\"http://url\" cstate=\"hqprint\"/>" +
                         "</blipFill>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlBlipFill result = (DmlBlipFill)DmlFillReader.Read(reader);
            // Assert
            Assert.That(result.Blip.EmbedImage, Is.EqualTo(new byte[0]));
            Assert.That(result.Blip.ImageLink, Is.EqualTo(string.Format("reltaget/http://url")));
            Assert.That(result.Blip.CompressionState, Is.EqualTo(DmlCompressionState.HighQualityPrint));
            Assert.That(result.Blip.Effects.Count, Is.EqualTo(0));
        }

        [Test]
        public void Build_GradFill_DmlGradientFillBuilded()
        {
            // Arrange
            string xml = "<gradFill rotWithShape=\"1\" flip=\"xy\">" +
                            "<tileRect l=\"50000\" t=\"60000\" r=\"70000\" b=\"80000\"/>" +
                         "</gradFill>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlGradientFill result = (DmlGradientFill)DmlFillReader.Read(reader);
            // Assert
            Assert.That(result.RotateWithShape, Is.True);
            Assert.That(result.TileFlipMode, Is.EqualTo(DmlTileFlipMode.HorizontalAndVertical));
            Assert.That(result.TileRectangle, Is.EqualTo(new DmlPercentageOffsetRectangle(0.5, 0.6, 0.7, 0.8)));
        }

        [Test]
        public void Build_GradFillWithGradientStop_DmlGradientFillBuilded()
        {
            // Arrange
            string xml = "<gradFill>" +
                              "<gsLst>" +
                                    "<gs pos=\"0\">" +
                                        "<schemeClr val=\"phClr\"/>" +
                                    "</gs>" +
                              "</gsLst>" +
                         "</gradFill>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlGradientFill result = (DmlGradientFill)DmlFillReader.Read(reader);
            // Assert
            Assert.That(result.GradientStops.Count, Is.EqualTo(1));
            DmlGradientStop gradientStop = result.GradientStops[0];
            Assert.That(gradientStop.Position, Is.EqualTo(0.0));
            Assert.That(gradientStop.Color is DmlPlaceholderColor, Is.True);
        }

        [Test]
        public void Build_GradFillWithLinearGradient_DmlGradientFillBuilded()
        {
            // Arrange
            string xml = "<gradFill>" +
                              "<lin ang=\"16200000\" scaled=\"1\"/> " +
                         "</gradFill>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlGradientFill result = (DmlGradientFill)DmlFillReader.Read(reader);
            // Assert
            DmlLinearGradient gradient = (DmlLinearGradient)result.Gradient;
            Assert.That(gradient.Angle.Value, Is.EqualTo(16200000.0));
            Assert.That(gradient.IsScaled, Is.True);
        }

        [Test]
        public void Build_GradFillWithPathGradient_DmlGradientFillBuilded()
        {
            // Arrange
            string xml = "<gradFill>" +
                              "<path path=\"circle\">" +
                                    "<fillToRect l=\"50000\" t=\"60000\" r=\"70000\" b=\"80000\"/>" +
                              "</path>" +
                         "</gradFill>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlGradientFill result = (DmlGradientFill)DmlFillReader.Read(reader);
            // Assert
            DmlPathGradient gradient = (DmlPathGradient)result.Gradient;
            Assert.That(gradient.FillToRectangle, Is.EqualTo(new DmlPercentageOffsetRectangle(0.5,0.6,0.7,0.8)));
            Assert.That(gradient.Path, Is.EqualTo(DmlPathShadeType.Circle));
        }

        [Test]
        public void Build_GrpFill_DmlGroupFillBuilded()
        {
            // Arrange
            string xml = "<grpFill>" +
                         "</grpFill>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlGroupFill result = (DmlGroupFill)DmlFillReader.Read(reader);
            // Assert
            Assert.That(result, IsNot.Null());
        }

        [Test]
        public void Build_NoFill_DmlNoFillBuilded()
        {
            // Arrange
            string xml = "<noFill>" +
                         "</noFill>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlNoFill result = (DmlNoFill)DmlFillReader.Read(reader);
            // Assert
            Assert.That(result, IsNot.Null());
        }

        [Test]
        public void Build_PattFill_DmlPatternFillBuilded()
        {
            // Arrange
            string xml = "<pattFill prst=\"pct70\">" +
                            "<bgClr><sysClr val=\"windowText\"/></bgClr>" +
                            "<fgClr><schemeClr val=\"lt1\"/></fgClr>" +
                         "</pattFill>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlPatternFill result = (DmlPatternFill)DmlFillReader.Read(reader);
            // Assert
            Assert.That(result.FillPresetPattern, Is.EqualTo(PatternType.Percent70));
            Assert.That(result.BackgroundColor is DmlSystemColor, Is.True);
            Assert.That(result.ForegroundColor is DmlSchemeColor, Is.True);
        }

        [Test]
        public void Build_SolidFill_DmlSolidFillBuilded()
        {
            // Arrange
            string xml = "<solidFill>" +
                            "<srgbClr>" +
                            "</srgbClr>" + 
                         "</solidFill>";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);
            // Act
            DmlSolidFill result = (DmlSolidFill)DmlFillReader.Read(reader);
            // Assert
            Assert.That(result.Color.ColorType, Is.EqualTo(DmlColorType.HexRgbColor));
        }
    }
}
