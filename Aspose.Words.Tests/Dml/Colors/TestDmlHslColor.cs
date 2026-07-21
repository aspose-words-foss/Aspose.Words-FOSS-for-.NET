// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Colors
{
    [TestFixture]
    public class TestDmlHslColor
    {
        [Test]
        public void DrColor_ColorInitialized_DrColorIsCorrect()
        {
            DmlHslColor color = new DmlHslColor();
            //Arrange
            color.Hue = DmlAngle.FromDegrees(128.0);
            color.Luminance = 0.4;
            color.Saturation = 0.1;
            //Act
            DrColor result = color.CreateDrColor(null, null);
            //Assert
            Assert.That(result, Is.EqualTo(new DrColor(91,112,94)));
        }

        [Test]
        public void DrColor_ColorNotInitialized_BlackColorReturend()
        {
            DmlHslColor color = new DmlHslColor();
            //Act
            DrColor result = color.CreateDrColor(null, null);
            //Assert
            Assert.That(result, Is.EqualTo(DrColor.Black));
        }
    }
}