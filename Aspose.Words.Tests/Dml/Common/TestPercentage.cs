// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/12/2010 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Saving;
using NUnit.Framework;

namespace Aspose.Words.Tests.Dml.Common
{
    [TestFixture]
    public class TestPercentage
    {
      
        [Test]
        public void Constructor_NonValidString_FractionIs0()
        {
            // Act
            double result = DmlPercentageUtil.FromPercent("100");
            // Assert
            Assert.That(result, Is.EqualTo(0.0));
        }

        [Test]
        public void Constructor_ValidString_FractionIsCorret()
        {
            // Act
            double result = DmlPercentageUtil.FromPercent("-101.25%");
            // Assert
            Assert.That(result, Is.EqualTo(-1.0125));
        }

        /// <summary>
        /// Checks DmlPercentageUtil.FromPercentOrDmlPercent(string, double)
        /// </summary>
        [TestCase(null, 1.23, 1.23, false)]
        [TestCase("", 1.23, 1.23, false)]
        [TestCase("0", 1.23, 0, false)]
        [TestCase("0%", 1.23, 0, true)]
        [TestCase("123400", 0, 1.234, false)]
        [TestCase("123.4%", 0, 1.234, true)]
        [TestCase("-999900", 0, -9.999, false)]
        [TestCase("-999.9%", 0, -9.999, true)]
        public void TestConversionFromPercentOrDmlPercent(string value, double defaultValue, double expectedValue, 
            bool expectedIsoTransitional)
        {
            OoxmlComplianceInfo complianceInfo = new OoxmlComplianceInfo();
            double result = DmlPercentageUtil.FromPercentOrDmlPercent(value, defaultValue, complianceInfo);
            Assert.That(result, Is.EqualTo(expectedValue), "The conversion result is wrong.");
            Assert.That(complianceInfo.Compliance == OoxmlComplianceCore.IsoTransitional, Is.EqualTo(expectedIsoTransitional), "Wrong value of the ISO Transitional flag.");
        }

        /// <summary>
        /// Checks DmlPercentageUtil.ToPercentOrDmlPercent
        /// </summary>
        [TestCase(0.99999, false, "99999")]
        [TestCase(0.99999, true, "99.999%")]
        [TestCase(-0.01, false, "-1000")]
        [TestCase(-0.01, true, "-1%")]
        [TestCase(0, false, "0")]
        [TestCase(0, true, "0%")]
        public void TestConversionToPercentOrDmlPercent(double fraction, bool isIsoStrict, string expectedValue)
        {
            string result = DmlPercentageUtil.ToPercentOrDmlPercent(fraction, isIsoStrict);
            Assert.That(result, Is.EqualTo(expectedValue), "The conversion result is wrong.");
        }
    }
}