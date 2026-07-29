// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/09/2011 by Alexey Kachalov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Effects;
using Aspose.Words.RW.Dml.Reader;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlEffectReader"/>.
    /// </summary>
    [TestFixture]
    public class TestDmlEffectReader
    {
        [Test]
        public void Build_XmlWithBiLevelEffect_EffectIsBuiltCorrectly()
        {
            // Arrange.
            const string xml = "<biLevel thresh=\"50000\" />";
            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            // Act.
            DmlBiLevelEffect effect = (DmlBiLevelEffect)DmlEffectReader.Read(reader);

            // Assert.
            Assert.That(DmlPercentageUtil.ToDmlPercent(effect.Threshold), Is.EqualTo(50000));
        }

        [Test]
        public void Build_XmlWithColorChangeEffectWithConsiderAlphaValuesByDefault_EffectIsBuiltCorrectly()
        {
            // Arrange.
            const string xml = "<clrChange>" +
                               "<clrFrom>" +
                               "<srgbClr />" +
                               "</clrFrom>" +
                               "<clrTo>" +
                               "<scrgbClr />" +
                               "</clrTo>" +
                               "</clrChange>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            // Act.
            DmlColorChangeEffect effect = (DmlColorChangeEffect)DmlEffectReader.Read(reader);

            // Assert.
            Assert.That(effect.SourceColor.ColorType, Is.EqualTo(DmlColorType.HexRgbColor));
            Assert.That(effect.DestinationColor.ColorType, Is.EqualTo(DmlColorType.PercentageRgbColor));
            Assert.That(effect.ConsiderAlphaValues, Is.EqualTo(true));
        }

        [Test]
        public void Build_XmlWithColorChangeEffectWithConsiderAlphaValuesFalse_EffectIsBuiltCorrectly()
        {
            // Arrange.
            const string xml = "<clrChange useA=\"false\">" +
                               "<clrFrom>" +
                               "</clrFrom>" +
                               "<clrTo>" +
                               "</clrTo>" +
                               "</clrChange>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            // Act.
            DmlColorChangeEffect effect = (DmlColorChangeEffect)DmlEffectReader.Read(reader);

            // Assert.
            Assert.That(effect.ConsiderAlphaValues, Is.EqualTo(false));
        }

        [Test]
        public void Build_XmlWithGrayScaleEffect_EffectIsBuiltCorrectly()
        {
            // Arrange.
            const string xml = "<grayscl />";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            // Act.
            DmlEffect effect = DmlEffectReader.Read(reader);

            // Assert.
            Assert.That(effect, Is.InstanceOf(typeof(DmlGrayScaleEffect)));
        }

        [Test]
        public void Build_XmlWithLuminanceEffect_EffectIsBuiltCorrectly()
        {
            // Arrange.
            const string xml = "<lum bright=\"30000\" contrast=\"-30000\">";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            // Act.
            DmlLuminanceEffect effect = (DmlLuminanceEffect)DmlEffectReader.Read(reader);

            // Assert.
            Assert.That(DmlPercentageUtil.ToDmlPercent((effect.Brightness*2) - 1), Is.EqualTo(30000D).Within(0.01));
            Assert.That(DmlPercentageUtil.ToDmlPercent((effect.Contrast*2) - 1), Is.EqualTo(-30000D).Within(0.01));
        }

        [Test]
        public void Build_XmlWithDuotoneEffect_EffectIsBuiltCorrectly()
        {
            // Arrange.
            const string xml = "<duotone>" +
                               "<srgbClr/>" +
                               "<srgbClr/>" +
                               "</duotone>";

            DocxDocumentReaderStub reader = new DocxDocumentReaderStub(xml);

            // Act.
            DmlDuotoneEffect effect = (DmlDuotoneEffect)DmlEffectReader.Read(reader);

            // Assert.
            // Assert.
            Assert.That(effect.Color1.ColorType, Is.EqualTo(DmlColorType.HexRgbColor));
            Assert.That(effect.Color2.ColorType, Is.EqualTo(DmlColorType.HexRgbColor));
        }
    }
}
