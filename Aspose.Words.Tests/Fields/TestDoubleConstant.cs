// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/11/2012 by Alexander Maslov

using System.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Fields.Expressions;
using NUnit.Framework;

namespace Aspose.Words.Tests.Fields
{
    /// <summary>
    /// Tests DoubleConstant behavior.
    /// </summary>
    [TestFixture]
    public class TestDoubleConstant : TestFieldsBase
    {
        [Test]
        public void TestTryParse()
        {
            Assert.That(DoubleConstant.TryParse(string.Empty), Is.EqualTo(null));
            Assert.That(DoubleConstant.TryParse(null), Is.EqualTo(null));

            CheckDoubleConstantParsing("0", 0.0, false, false);
            CheckDoubleConstantParsing("1", 1.0, false, false);
            CheckDoubleConstantParsing("-1", -1.0, false, false);
            CheckDoubleConstantParsing("1,000.00", 1000.0, true, false);
            CheckDoubleConstantParsing("-1,000.00", -1000.0, true, false);

            CheckDoubleConstantParsing("123.45e+2", 12345.0, false, false);
            CheckDoubleConstantParsing("0.12345e+5", 12345.0, false, false);
            CheckDoubleConstantParsing("-0.12345e+5", -12345.0, false, false);
            CheckDoubleConstantParsing("123.45e-2", 1.23450, false, false);

            CheckParsingCurrencySymbolBefore(
                FormatterPal.GetCurrencySymbolCurrent(),
                FormatterPal.GetDecimalSeparatorCurrent(),
                FormatterPal.GetNumberGroupSeparatorCurrent());

            SystemPal.SetCulture("ru-RU");

            CheckParsingCurrencySymbolAfter(
                FormatterPal.GetCurrencySymbolCurrent(),
                FormatterPal.GetDecimalSeparatorCurrent(),
                FormatterPal.GetNumberGroupSeparatorCurrent());

            SystemPal.SetCulture("fr-CH");

            // DV WORDSNET-17864 Supported by .NET 4.0 and above.

            CheckParsingCurrencySymbolAfter(
                "CHF",
                FormatterPal.GetDecimalSeparatorCurrent(),
                FormatterPal.GetNumberGroupSeparatorCurrent());

            // DV WORDSNET-17864 Unsupported by .NET 4.0 and above natively, see FormatterPal.NormalizeCurrencySymbols.

            CheckParsingCurrencySymbolBefore(
                "SFr.",
                FormatterPal.GetDecimalSeparatorCurrent(),
                FormatterPal.GetNumberGroupSeparatorCurrent());

            CheckParsingCurrencySymbolBefore(
                "fr.",
                FormatterPal.GetDecimalSeparatorCurrent(),
                FormatterPal.GetNumberGroupSeparatorCurrent());
        }

        private static void CheckDoubleConstantParsing(string inputString, double expectedValue, bool isUsesGroupSeparator, bool isCurrency)
        {
            DoubleConstant actual = DoubleConstant.TryParse(inputString);

            Assert.That(actual.ValueDouble, Is.EqualTo(expectedValue));
            Assert.That(actual.IsUsesGroupSeparator, Is.EqualTo(isUsesGroupSeparator));
            Assert.That(actual.IsCurrency, Is.EqualTo(isCurrency));
        }

        private static void CheckParsingCurrencySymbolBefore(string currensySymbol, char decimalSeparator, char groupSeparator)
        {
            CheckDoubleConstantParsing(string.Format("{0}0", currensySymbol), 0.0, false, true);
            CheckDoubleConstantParsing(string.Format("{0}1", currensySymbol), 1.0, false, true);
            CheckDoubleConstantParsing(string.Format("-{0}1", currensySymbol), -1.0, false, true);
            CheckDoubleConstantParsing(string.Format("{0}1000", currensySymbol), 1000.0, false, true);
            CheckDoubleConstantParsing(string.Format("{0} 1000{1}00", currensySymbol, decimalSeparator), 1000.0, false, true);
            CheckDoubleConstantParsing(string.Format("{0}1{2}000{1}00", currensySymbol, decimalSeparator, groupSeparator), 1000.0, true, true);
            CheckDoubleConstantParsing(string.Format("{0} 1{2}000{1}00", currensySymbol, decimalSeparator, groupSeparator), 1000.0, true, true);
            CheckDoubleConstantParsing(string.Format("-{0}1{2}000{1}00", currensySymbol, decimalSeparator, groupSeparator), -1000.0, true, true);
            CheckDoubleConstantParsing(string.Format("- {0}1{2}000{1}00", currensySymbol, decimalSeparator, groupSeparator), -1000.0, true, true);
        }

        private static void CheckParsingCurrencySymbolAfter(string currensySymbol, char decimalSeparator, char groupSeparator)
        {
            CheckDoubleConstantParsing(string.Format("0{0}", currensySymbol), 0.0, false, true);
            CheckDoubleConstantParsing(string.Format("1{0}", currensySymbol), 1.0, false, true);
            CheckDoubleConstantParsing(string.Format("-1{0}", currensySymbol), -1.0, false, true);
            CheckDoubleConstantParsing(string.Format("1000{0}", currensySymbol), 1000.0, false, true);
            CheckDoubleConstantParsing(string.Format("1000{1}00 {0}", currensySymbol, decimalSeparator), 1000.0, false, true);
            CheckDoubleConstantParsing(string.Format("1{2}000{1}00{0}", currensySymbol, decimalSeparator, groupSeparator), 1000.0, true, true);
            CheckDoubleConstantParsing(string.Format("1{2}000{1}00 {0}", currensySymbol, decimalSeparator, groupSeparator), 1000.0, true, true);
            CheckDoubleConstantParsing(string.Format("-1000{1}00 {0}", currensySymbol, decimalSeparator), -1000.0, false, true);
            CheckDoubleConstantParsing(string.Format("- 1{2}000{1}00{0}", currensySymbol, decimalSeparator, groupSeparator), -1000.0, true, true);
        }

        [Test]
        [TestCaseSource("TestNumberOfDigitsAfterDecimalpointCaseSource")]
        public int TestNumberOfDigitsAfterDecimalpoint(double value, string culture)
        {
            SystemPal.SetCulture(culture);
            DoubleConstant constant = new DoubleConstant(value);
            return constant.NumberOfDigitsAfterDecimalPoint;
        }

        public static IEnumerable<TestCaseData> TestNumberOfDigitsAfterDecimalpointCaseSource
        {
            get
            {
                double[] values = { 321, 34535.4, 84.67, 0.934, 74.3975 };
                string[] cultures = { "en-US", "en-NZ", "ru-RU", "de-DE" };
                List<TestCaseData> testCaseSource = new List<TestCaseData>();
                foreach (string culture in cultures)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        testCaseSource.Add(new TestCaseData(values[i], culture).Returns(i));
                    }
                }
                return testCaseSource;
            }
        }

    }
}
