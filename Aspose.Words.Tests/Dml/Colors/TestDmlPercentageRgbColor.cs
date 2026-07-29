// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Colors
{
    [TestFixture]
    public class TestDmlPercentageRgbColor
    {
        [Test]
        public void DrColor_CorrectValue_CorrectDrColor()
        {
            DmlPercentageRgbColor color = new DmlPercentageRgbColor();
            //Arrange
            color.R = DmlPercentageUtil.FromPercent("10.0%");
            color.G = DmlPercentageUtil.FromPercent("20.0%");
            color.B = DmlPercentageUtil.FromPercent("30.0%");
            //Act
            DrColor result = color.CreateDrColor(null, null);
            //Assert
            Assert.That(result, Is.EqualTo(new DrColor(0x1A,0x33,0x4c)));
        }

        [Test]
        public void DrColor_NotInitialized_BlackColor()
        {
            DmlPercentageRgbColor color = new DmlPercentageRgbColor();
            //Act
            DrColor result = color.CreateDrColor(null, null);
            //Assert
            Assert.That(result, Is.EqualTo(DrColor.Black));
        }
    }
}