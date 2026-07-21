// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2010 by Alexey Titov

using System;
using Aspose.Words.Drawing.Core.Dml.Guides;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Guides
{
    [TestFixture]
    public class TestDmlGuides
    {
        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void AddAdjustableValue_FormulaIsNull_ExceptionThrown()
        {
            DmlGuides guides = new DmlGuides();
            guides.AddAdjustableValue("aaa", null, false);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void AddAdjustableValue_NameIsNull_ExceptionThrown()
        {
            DmlGuides guides = new DmlGuides();
            guides.AddAdjustableValue(null, "val 100", false);
        }

        [Test]
        public void AddAdjustableValue_ValidArguments_ValidGuideCreatedAndAdded()
        {
            DmlGuides guides = new DmlGuides();
            // Act
            guides.AddAdjustableValue("gd", "val 100", false);
            // Assert
            DmlGuide guide = guides.AdjustableValues[0];
            Assert.That(guide.Name, Is.EqualTo("gd"));
            Assert.That(guide.Formula, IsNot.Null());
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void AddGuide_FormulaIsNull_ExceptionThrown()
        {
            DmlGuides guides = new DmlGuides();
            guides.AddGuide("aaa", null, false);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void AddGuide_NameIsNull_ExceptionThrown()
        {
            DmlGuides guides = new DmlGuides();
            guides.AddGuide(null, "val 100", false);
        }

        [Test]
        public void AddGuide_ValidArguments_ValidGuideCreatedAndAdded()
        {
            DmlGuides guides = new DmlGuides();
            // Act
            guides.AddGuide("gd", "val 100", false);
            // Assert
            DmlGuide guide = guides.Guides[0];
            Assert.That(guide.Name, Is.EqualTo("gd"));
            Assert.That(guide.Formula, IsNot.Null());
        }

        [Test]
        public void Calculate_GuideHasRelationOnAdjustableValue_CorrectValuesReturned()
        {
            DmlGuides guides = new DmlGuides();
            // Arrange
            guides.AddAdjustableValue("av1", "val 16", false);
            guides.AddGuide("g1", "sqrt av1", false);
            // Act
            guides.Calculate(0, 0);
            // Assert
            CheckGuideValue("g1", 4, guides);
            CheckGuideValue("av1", 16, guides);
        }

        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void Calculate_GuideHasRelationOnNextGuide_ExceptionThrown()
        {
            DmlGuides guides = new DmlGuides();
            // Arrange
            guides.AddGuide("g1", "sqrt g2", false);
            guides.AddGuide("g2", "sqrt g1", false);
            // Act
            guides.Calculate(0, 0);
            // Assert
            CheckGuideValue("g1", 4, guides);
        }

        [Test]
        public void Calculate_GuideHasRelationOnPrevGuide_CalcualtedSuccesefuly()
        {
            DmlGuides guides = new DmlGuides();
            // Arrange
            guides.AddGuide("g1", "sqrt 16", false);
            guides.AddGuide("g2", "min g1 5", false);
            // Act
            guides.Calculate(0, 0);
            // Assert
            CheckGuideValue("g2", 4, guides);
        }

        [Test]
        public void Calculate_WidthAndHeightDefined_StandartGuidesInitialized()
        {
            DmlGuides guides = new DmlGuides();
            // Act
            guides.Calculate(22, 50);
            // Assert
            CheckGuideValue("w", 22, guides);
            CheckGuideValue("h", 50, guides);
            CheckGuideValue("3cd4", 16200000.0, guides);
            CheckGuideValue("3cd8", 8100000.0, guides);
            CheckGuideValue("5cd8", 13500000.0, guides);
            CheckGuideValue("7cd8", 18900000.0, guides);
            CheckGuideValue("b", 50, guides);
            CheckGuideValue("cd2", 10800000.0, guides);
            CheckGuideValue("cd4", 5400000.0, guides);
            CheckGuideValue("cd8", 2700000.0, guides);
            CheckGuideValue("hc", 22/2.0, guides);
            CheckGuideValue("hd2", 50/2.0, guides);
            CheckGuideValue("hd3", 50/3.0, guides);
            CheckGuideValue("hd4", 50/4.0, guides);
            CheckGuideValue("hd5", 50/5.0, guides);
            CheckGuideValue("hd6", 50/6.0, guides);
            CheckGuideValue("hd8", 50/8.0, guides);
            CheckGuideValue("l", 0, guides);
            CheckGuideValue("ls", 50, guides);
            CheckGuideValue("r", 22, guides);
            CheckGuideValue("ss", 22, guides);
            CheckGuideValue("ssd2", 22/2.0, guides);
            CheckGuideValue("ssd4", 22/4.0, guides);
            CheckGuideValue("ssd6", 22/6.0, guides);
            CheckGuideValue("ssd8", 22/8.0, guides);
            CheckGuideValue("ssd16", 22/16.0, guides);
            CheckGuideValue("ssd32", 22/32.0, guides);
            CheckGuideValue("t", 0, guides);
            CheckGuideValue("vc", 50/2.0, guides);
            CheckGuideValue("wd2", 22/2.0, guides);
            CheckGuideValue("wd3", 22/3.0, guides);
            CheckGuideValue("wd4", 22/4.0, guides);
            CheckGuideValue("wd5", 22/5.0, guides);
            CheckGuideValue("wd6", 22/6.0, guides);
            CheckGuideValue("wd8", 22/8.0, guides);
            CheckGuideValue("wd10", 22/10.0, guides);
            CheckGuideValue("wd32", 22/32.0, guides);
        }

        private void CheckGuideValue(string guideName, double expectedValue, DmlGuides guides)
        {
            Assert.That(guides.GetValue(guideName), Is.EqualTo(expectedValue));
        }

        [Test]
        public void GetValue_AdjustableValueWithSpecifiedNameExists_ValueReturned()
        {
            DmlGuides guides = new DmlGuides();
            // Arange
            guides.AddAdjustableValue("aaa", "val 4", false);
            guides.Calculate(0, 0);
            // Act
            // Assert
            CheckGuideValue("aaa", 4, guides);
        }

        [Test]
        public void GetValue_GuideWithSpecifiedNameExists_ValueReturned()
        {
            DmlGuides guides = new DmlGuides();
            // Arange
            guides.AddGuide("aaa", "val 4", false);
            guides.Calculate(0, 0);
            // Act
            // Assert
            CheckGuideValue("aaa", 4, guides);
        }

        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void GetValue_ValueWithSpecifiedNotExists_ExceptionThrown()
        {
            DmlGuides guides = new DmlGuides();
            guides.GetValue("aaa");
        }
    }
}
