// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Colors;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Colors
{
    [TestFixture]
    public class TestDmlPresetColor
    {
        [Test]
        public void DrColor_CorrectValue_CorrectDrColor()
        {
            DmlPresetColor color = new DmlPresetColor();
            //Arrange
            color.Value = "red";
            //Act
            DrColor result = color.CreateDrColor(null, null);
            //Assert
            Assert.That(result, Is.EqualTo(DrColor.Red));
        }

        [Test]
        public void DrColor_NotInitialized_EmptyColor()
        {
            DmlPresetColor color = new DmlPresetColor();
            //Act
            DrColor result = color.CreateDrColor(null, null);
            //Assert
            Assert.That(result, Is.EqualTo(DrColor.Empty));
        }
    }
}