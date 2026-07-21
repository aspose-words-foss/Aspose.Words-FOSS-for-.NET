// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/03/2014 by Alexey Morozov
using Aspose.Drawing;
using NUnit.Framework;

namespace Aspose.Tests.Base
{
    [TestFixture]
    public class TestHslColor
    {
        [Test]
        public void TestHslColorOfficeAlgorithm()
        {
            // Values are taken from spec.
            DrColor rgbColor = DrColor.FromArgb(0x4f, 0x81, 0xbd);

            // Verify RGB to HSL conversion.
            HSLColor hslColor = HSLColor.OfficeFromDrColor(rgbColor);
            Assert.That(hslColor.Hue, Is.EqualTo(0.5916).Within(0.001));
            Assert.That(hslColor.Sat, Is.EqualTo(0.4541).Within(0.001));
            Assert.That(hslColor.Lum, Is.EqualTo(0.5250).Within(0.001));

            // Verify back HSL to RGB conversion.
            rgbColor = hslColor.OfficeToDrColor();
            Assert.That(rgbColor.R, Is.EqualTo(0x4f));
            Assert.That(rgbColor.G, Is.EqualTo(0x81));
            Assert.That(rgbColor.B, Is.EqualTo(0xbd));
        }
    }
}