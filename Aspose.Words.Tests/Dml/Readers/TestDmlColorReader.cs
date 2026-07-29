// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2010 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Dml.Reader;
using Aspose.Words.Themes;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Readers
{
    /// <summary>
    /// Tests for <see cref="DmlColorReader"/>. 
    /// </summary>
    [TestFixture]
    public class TestDmlColorReader
    {
        [Test]
        public void Build_HslColor_ColorBuilt()
        {
            // Arrange
            string xml = "<hslClr hue=\"14400000\" sat=\"100.000%\" lum=\"50.000%\"/>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlHslColor result = (DmlHslColor)DmlColorReader.Read(reader, null);
            // Assert
            Assert.That(result.Hue, Is.EqualTo(new DmlAngle(14400000)));
            Assert.That(result.Saturation, Is.EqualTo(DmlPercentageUtil.FromPercent("100.000%")));
            Assert.That(result.Luminance, Is.EqualTo(DmlPercentageUtil.FromPercent("50.000%")));
        }

        [Test]
        public void Build_PresetColor_ColorBuilt()
        {
            // Arrange
            string xml = "<prstClr val=\"black\"/>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlPresetColor result = (DmlPresetColor)DmlColorReader.Read(reader, null);
            // Assert
            Assert.That(result.Value, Is.EqualTo("black"));
        }

        [Test]
        public void Build_PercentageRgbColor_ColorBuilt()
        {
            // Arrange
            string xml = "<scrgbClr r=\"50%\" g=\"-51%\" b=\"1%\"/>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlPercentageRgbColor result = (DmlPercentageRgbColor)DmlColorReader.Read(reader, null);
            // Assert
            Assert.That(result.R, Is.EqualTo(DmlPercentageUtil.FromPercent("50%")));
            Assert.That(result.G, Is.EqualTo(DmlPercentageUtil.FromPercent("-51%")));
            Assert.That(result.B, Is.EqualTo(DmlPercentageUtil.FromPercent("1%")));
        }

        [Test]
        public void Build_HexRgbColor_ColorBuilded()
        {
            // Arrange
            string xml = "<srgbClr val=\"10ABCD\"/>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlHexRgbColor result = (DmlHexRgbColor)DmlColorReader.Read(reader, null);
            // Assert
            Assert.That(result.Value, Is.EqualTo("10ABCD"));
        }

        [Test]
        public void Build_SchemeColor_ColorBuilt()
        {
            // Arrange
            string xml = "<schemeClr val=\"lt1\"/>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlSchemeColor result = (DmlSchemeColor)DmlColorReader.Read(reader, null);
            // Assert
            Assert.That(result.Value, Is.EqualTo(ThemeColor.Light1));
        }

        [Test]
        public void Build_SchemeColorWithPlaceholderValue_ColorBuilt()
        {
            // Arrange
            string xml = "<schemeClr val=\"phClr\"/>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlColor result = DmlColorReader.Read(reader, null);
            // Assert
            Assert.That(result is DmlPlaceholderColor, Is.True);
        }

        [Test]
        public void Build_SystemColor_ColorBuilt()
        {
            // Arrange
            string xml = "<sysClr val=\"windowText\" lastClr=\"FF0011\"/>";
            NrxXmlReader reader = new NrxXmlReader(xml, null);
            // Act
            DmlSystemColor result = (DmlSystemColor)DmlColorReader.Read(reader, null);
            // Assert
            Assert.That(result.Value, Is.EqualTo("windowText"));
            Assert.That(result.LastColor, Is.EqualTo("FF0011"));
        }

        [Test]
        public void ReadColor_XmlContainsSchemeColorAndSystemColor_TheLastColorIsRead()
        {
            // Arrange.
            string xml =
                "<clrFrom>" +
                    "<schemeClr />" +
                    "<sysClr />" +
                "</clrFrom>";

            NrxXmlReader reader = new NrxXmlReader(xml, null);

            // Act.
            DmlColor color = DmlColorReader.ReadColor(reader, null);

            // Assert.
            Assert.That(color, Is.InstanceOf(typeof(DmlSystemColor)));
        }
    }
}
