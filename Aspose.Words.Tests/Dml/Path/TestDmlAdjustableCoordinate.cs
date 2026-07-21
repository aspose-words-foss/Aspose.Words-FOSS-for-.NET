// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Path;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Path
{
    [TestFixture]
    public class TestDmlAdjustableCoordinate
    {
        [Test]
        public void Constructor_Double_ValueSetted()
        {
            //Arrange
            //Act
            DmlAdjustableCoordinate coordinate = new DmlAdjustableCoordinate(123);
            //Assert
            Assert.That(coordinate.GetValue(new DmlGuideValueProviderStub()), Is.EqualTo(123.0));
        }

        [Test]
        public void Constructor_GuideName_GuideNameSetted()
        {
            DmlGuideValueProviderStub guideValueProvider = new DmlGuideValueProviderStub();
            //Arrange
            string value = "guide";
            guideValueProvider.Add(value, 345);
            //Act
            DmlAdjustableCoordinate coordinate = new DmlAdjustableCoordinate(value);
            //Assert
            Assert.That(coordinate.GetValue(guideValueProvider), Is.EqualTo(345.0));
        }

        [Test]
        public void Constructor_Number_ValueSetted()
        {
            //Arrange
            string value = "570";
            //Act
            DmlAdjustableCoordinate coordinate = new DmlAdjustableCoordinate(value);
            //Assert
            Assert.That(coordinate.GetValue(new DmlGuideValueProviderStub()), Is.EqualTo(570.0));
        }

      
    }
}