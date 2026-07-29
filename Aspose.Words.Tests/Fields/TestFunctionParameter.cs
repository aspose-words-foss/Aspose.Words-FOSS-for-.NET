// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using Aspose.Common;
using Aspose.Words.Fields.Expressions;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    /// <summary>
    /// Tests <see cref="FunctionParameter"/> class.
    /// </summary>
    [TestFixture]
    public class TestFunctionParameter : TestFieldsBase
    {
        /// <summary>
        /// Tests parsing parameter value.
        /// </summary>
        [Test]
        public void TestParsingParameterValue()
        {
            SystemPal.SetCulture("en-US");

            ParameterPeculiarities expectedProperties = ParameterPeculiarities.Number |
                                                        ParameterPeculiarities.Negative |
                                                        ParameterPeculiarities.Fractional |
                                                        ParameterPeculiarities.CurrencyAtStart |
                                                        ParameterPeculiarities.SpaceBefore;
            TestParsingParameterValue(" -$5000.05", "-$5000.05", expectedProperties, 4);

            expectedProperties = ParameterPeculiarities.CurrencyAtStart | ParameterPeculiarities.Number |
                                 ParameterPeculiarities.Fractional | ParameterPeculiarities.SpaceAfter;
            TestParsingParameterValue("$.05 ", "$.05", expectedProperties, 0);

            expectedProperties = ParameterPeculiarities.Negative | ParameterPeculiarities.Number |
                                 ParameterPeculiarities.Fractional | ParameterPeculiarities.SpaceAfter;
            TestParsingParameterValue("-.05 ", "-.05", expectedProperties, 0);

            expectedProperties = ParameterPeculiarities.None;
            TestParsingParameterValue(".", ".", expectedProperties, 0);

            expectedProperties = ParameterPeculiarities.Number | ParameterPeculiarities.Fractional;
            TestParsingParameterValue("12345.543", "12345.543", expectedProperties, 5);

            expectedProperties = ParameterPeculiarities.Number | ParameterPeculiarities.SpaceAfter;
            TestParsingParameterValue("12 ", "12", expectedProperties, 2);

            expectedProperties = ParameterPeculiarities.None;
            TestParsingParameterValue("123.45.67", "123.45.67", expectedProperties, 0);

            expectedProperties = ParameterPeculiarities.Number | ParameterPeculiarities.Negative;
            TestParsingParameterValue("-1", "-1", expectedProperties, 1);
        }

        /// <summary>
        /// Tests parsing parameter value.
        /// </summary>
        /// <param name="value">Parameter value to parse.</param>
        /// <param name="expectedValue">An expected parameter value.</param>
        /// <param name="expectedProperties">An expected parameter properties.</param>
        /// <param name="expectedLength">An expected length of integer part of parameter.</param>
        private static void TestParsingParameterValue(string value, string expectedValue, ParameterPeculiarities expectedProperties, int expectedLength)
        {
            FunctionParameter target = new FunctionParameter(value);
            Assert.That(target.ParameterValue, Is.EqualTo(expectedValue));
            Assert.That(target.Peculiarities, Is.EqualTo(expectedProperties));
            Assert.That(target.LengthOfIntegerPart, Is.EqualTo(expectedLength));
            Assert.That(target.IsCurrencyAtEnd, Is.EqualTo((expectedProperties & ParameterPeculiarities.CurrencyAtEnd) == ParameterPeculiarities.CurrencyAtEnd));
            Assert.That(target.IsCurrencyAtStart, Is.EqualTo((expectedProperties & ParameterPeculiarities.CurrencyAtStart) == ParameterPeculiarities.CurrencyAtStart));
            Assert.That(target.IsFractional, Is.EqualTo((expectedProperties & ParameterPeculiarities.Fractional) == ParameterPeculiarities.Fractional));
            Assert.That(target.IsNegative, Is.EqualTo((expectedProperties & ParameterPeculiarities.Negative) == ParameterPeculiarities.Negative));
            Assert.That(target.IsNumber, Is.EqualTo((expectedProperties & ParameterPeculiarities.Number) == ParameterPeculiarities.Number));
            Assert.That(target.IsSpaceAfter, Is.EqualTo((expectedProperties & ParameterPeculiarities.SpaceAfter) == ParameterPeculiarities.SpaceAfter));
            Assert.That(target.IsSpaceBefore, Is.EqualTo((expectedProperties & ParameterPeculiarities.SpaceBefore) == ParameterPeculiarities.SpaceBefore));
        }
    }
}
