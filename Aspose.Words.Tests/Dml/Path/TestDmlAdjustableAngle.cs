// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/10/2010 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Path;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Path
{
    [TestFixture]
    public class TestDmlAdjustableAngle
    {
        [Test]
        public void Constructor_Double_ValueSetted()
        {
            //Arrange
            //Act
            DmlAdjustableAngle angle = new DmlAdjustableAngle(123);
            //Assert
            Assert.That(angle.GetValue(new DmlGuideValueProviderStub()), Is.EqualTo(123.0));
        }

        [Test]
        public void Constructor_GuideName_GuideNameSetted()
        {
            DmlGuideValueProviderStub guideValueProvider = new DmlGuideValueProviderStub();
            //Arrange
            string value = "guide";
            guideValueProvider.Add(value, 999);
            //Act
            DmlAdjustableAngle angle = new DmlAdjustableAngle(value);
            //Assert
            Assert.That(angle.GetValue(guideValueProvider), Is.EqualTo(999.0));
        }

        [Test]
        public void Constructor_Number_ValueSetted()
        {
            //Arrange
            string value = "570";
            //Act
            DmlAdjustableAngle angle = new DmlAdjustableAngle(value);
            //Assert
            Assert.That(angle.GetValue(new DmlGuideValueProviderStub()), Is.EqualTo(570.0));
        }
        
    }
}