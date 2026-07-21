// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Colors
{
    [TestFixture]
    public class TestDmlHexRgbColor
    {
        [Test]
        public void DrColor_CorrectValue_CorrectDrColor()
        {
            DmlHexRgbColor color = new DmlHexRgbColor();
            //Arrange
            color.Value = "F1CAB8";
            //Act
            DrColor result = color.CreateDrColor(null, null);
            //Assert
            Assert.That(result, Is.EqualTo(new DrColor(0xF1,0xCA,0xB8)));
        }

        [Test]
        public void DrColor_Null_EmptyColor()
        {
            DmlHexRgbColor color = new DmlHexRgbColor();
            //Arrange
            color.Value = null;
            //Act
            DrColor result = color.CreateDrColor(null, null);
            //Assert
            Assert.That(result, Is.EqualTo(DrColor.Empty));
        }

        [Test]
        public void DrColor_EmptyString_EmptyColor()
        {
            DmlHexRgbColor color = new DmlHexRgbColor();
            //Arrange
            color.Value = "";
            //Act
            DrColor result = color.CreateDrColor(null, null);
            //Assert
            Assert.That(result, Is.EqualTo(DrColor.Empty));
        }
    }
}