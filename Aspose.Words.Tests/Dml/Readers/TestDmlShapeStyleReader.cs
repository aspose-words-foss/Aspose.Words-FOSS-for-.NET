// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/02/2011 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Dml.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlShapeStyleReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlShapeStyleReader
    {
        [Test]
        public void Build_LineReference_Builded()
        {
            // Arrange
            string xml = "<style>" + 
                            "<lnRef idx=\"45\">" +
                                "<prstClr />" +
                            "</lnRef>" +
                         "</style>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlShapeStyle result = DmlShapeStyleReader.Read(reader, null);
            // Assert
            Assert.That(result.LineReference.StyleMatrixIndex, Is.EqualTo(45));
            Assert.That(result.LineReference.Color.ColorType, Is.EqualTo(DmlColorType.PresetColor));
        }

        [Test]
        public void Build_FillReference_Builded()
        {
            // Arrange
            string xml = "<style>" +
                            "<fillRef idx=\"45\">" +
                                "<prstClr />" +
                            "</fillRef>" +
                         "</style>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlShapeStyle result = DmlShapeStyleReader.Read(reader, null);
            // Assert
            Assert.That(result.FillReference.StyleMatrixIndex, Is.EqualTo(45));
            Assert.That(result.FillReference.Color.ColorType, Is.EqualTo(DmlColorType.PresetColor));
        }

        [Test]
        public void Build_EffectReference_Builded()
        {
            // Arrange
            string xml = "<style>" +
                            "<effectRef idx=\"45\">" +
                                "<prstClr />" +
                            "</effectRef>" +
                         "</style>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlShapeStyle result = DmlShapeStyleReader.Read(reader, null);
            // Assert
            Assert.That(result.EffectReference.StyleMatrixIndex, Is.EqualTo(45));
            Assert.That(result.EffectReference.Color.ColorType, Is.EqualTo(DmlColorType.PresetColor));
        }

        [Test]
        public void Build_FontReference_Builded()
        {
            // Arrange
            string xml = "<style>" +
                            "<fontRef idx=\"minor\">" +
                                "<prstClr />" +
                            "</fontRef>" +
                         "</style>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlShapeStyle result = DmlShapeStyleReader.Read(reader, null);
            // Assert
            Assert.That(result.FontReference.FontCollectionIndex, Is.EqualTo(DmlFontCollectionIndex.Minor));
            Assert.That(result.FontReference.Color.ColorType, Is.EqualTo(DmlColorType.PresetColor));
        }
    }
}