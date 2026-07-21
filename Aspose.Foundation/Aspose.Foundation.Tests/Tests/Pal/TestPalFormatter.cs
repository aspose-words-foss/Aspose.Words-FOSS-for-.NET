// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using System;
using Aspose.Common;
using NUnit.Framework;

namespace Aspose.Tests.Pal
{
    [TestFixture]
    public class TestPalFormatter
    {
        [SetUp]
        public void SetUp()
        {
            SystemPal.SetTestMode(true);
            SystemPal.SaveCulture();
            SystemPal.SetStandardCulture();
        }

        [TearDown]
        public void TearDown()
        {
            SystemPal.RestoreCulture();
        }

        [Test]
        [TestCase("en-NZ")]
        [TestCase("ru-RU")]
        public void TestDateTimeToXmlUtc(string culture)
        {
            SystemPal.SetCulture(culture);

            DateTime dateTime = new DateTime(2010, 12, 01, 9, 30, 11);
            Assert.That(FormatterPal.DateTimeToXmlUtc(dateTime), Is.EqualTo("2010-12-01T09:30:11Z"));
        }

        [Test]
        [TestCase("en-NZ")]
        [TestCase("ru-RU")]
        public void TestDateTimeToXmlNoTimezone(string culture)
        {
            SystemPal.SetCulture(culture);

            DateTime dateTime = new DateTime(2010, 12, 01, 9, 30, 11);
            Assert.That(FormatterPal.DateTimeToXmlNoTimezone(dateTime), Is.EqualTo("2010-12-01T09:30:11"));
        }

        [Test]
        [TestCase("0", 0.0)]
        [TestCase("0", -0.0)] // Negative zero is not expected in formatted string.
        [TestCase("123", 123.0)]
        [TestCase("0.7", 0.7)]
        [TestCase("123456", 123456.0)]
        [TestCase("123.456789012345", 123.456789012345)]
        [TestCase("1234567890123.45", 1234567890123.45)]
        [TestCase("123456789012345", 123456789012345.0)]
        [TestCase("0.123456789012345", 0.123456789012345)]
        [TestCase("0.000123456789012345", 0.000123456789012345)]
        [TestCase("0.0001", 0.0001)]
        [TestCase("-0.0001", -0.0001)]
        [TestCase("1E-05", 0.00001)]
        [TestCase("-1E-05", -0.00001)]
        [TestCase("1.23456789012346E-07", 0.0000001234567890123456)]
        [TestCase("1.23456789012345E-16", 0.000000000000000123456789012345)]
        [TestCase("123456789012346", 123456789012345.6)]
        [TestCase("1234567890123.46", 1234567890123.456)]
        // On .NET scientific notation can occur for large numbers too. But it is harder to detect than for small numbers.
        // So I think is an acceptable differences in .NET vs Java formatting for very large numbers.
#if JAVA
        [TestCase("1234567890123460", 1234567890123455.0)] // 5.5 rounds to 6.0
        [TestCase("1234567890123450", 1234567890123445.0)] // 4.5 rounds to 5.0
#elif NETSTANDARD
        [TestCase("1.23456789012346E+15", 1234567890123455.0)] // 5.5 rounds to 6.0
        [TestCase("1.23456789012344E+15", 1234567890123445.0)] // 4.5 rounds to 5.0
#else
        [TestCase("1.23456789012346E+15", 1234567890123455.0)] // 5.5 rounds to 6.0
        [TestCase("1.23456789012345E+15", 1234567890123445.0)] // 4.5 rounds to 5.0
#endif
        public void TestDoubleToStr(string result, double value)
        {
            Assert.That(FormatterPal.DoubleToStr(value), Is.EqualTo(result));
        }

        [Test]
        [TestCase(0.0, "")]
        [TestCase(0.0, "   ")]
        [TestCase(0.0, "abc")]
        [TestCase(0.0, "1,2")]
        [TestCase(0.0, "1;2")]
        [TestCase(0.0, "1 2")]
        [TestCase(0.0, "1 2/3")]
        [TestCase(0.0, "1/02/2005")]
        [TestCase(5.0, "5")]
        [TestCase(1.1, " 1.1")]
        [TestCase(1.2, " \t1.2 \t")]
        [TestCase(1.3, " +1.3")]
        [TestCase(-1.3, " -1.3")]
        [TestCase(-0.0000123, " -1.23E-05  ")]
        [TestCase(-0.0123, " -1.23e-02  ")]
        [TestCase(0.0000123, " 1.23E-05")]
        [TestCase(0.0123, " 1.23e-02")]
        public void TestParseDoubleInvariant(double result, string value)
        {
            Assert.That(FormatterPal.ParseDouble(value), Is.EqualTo(result));
        }

        [Test]
        [TestCase("12345", 0)]
        [TestCase("12345.1", 1)]
        [TestCase("12345.12", 2)]
        [TestCase("12345.123", 3)]
        [TestCase("12345.1235", 4)]
        [TestCase("12345.12346", 5)]
        [TestCase("12345.123457", 6)]
        [TestCase("12345.1234568", 7)]
        [TestCase("12345.12345679", 8)]
        [TestCase("12345.123456789", 9)]
        public void TestDoubleToStrNDecimals(string result, int n)
        {
            const double value = 12345.1234567891;
            Assert.That(FormatterPal.DoubleToStrNDecimals(value, n), Is.EqualTo(result));
        }

        [Test]
        [TestCase("12345", 0)]
        [TestCase("12345.1", 1)]
        [TestCase("12345.12", 2)]
#if JAVA
        [TestCase("12345.123", 3)]
        [TestCase("12345.123", 4)]
        [TestCase("12345.123", 5)]
        [TestCase("12345.123", 6)]
        [TestCase("12345.123", 7)]
        [TestCase("12345.123", 8)]
        [TestCase("12345.123", 9)]
#else
        [TestCase("12345.12", 3)]
        [TestCase("12345.12", 4)]
        [TestCase("12345.12", 5)]
        [TestCase("12345.12", 6)]
        [TestCase("12345.12", 7)]
        [TestCase("12345.12", 8)]
        [TestCase("12345.12", 9)]
#endif
        public void TestFloatToStrNDecimals(string result, int n)
        {
            const float value = 12345.123456F;
            Assert.That(FormatterPal.FloatToStrNDecimals(value, n), Is.EqualTo(result));
        }

        [Test]
        [TestCase("0", 0.0)]
        [TestCase("123", 123.0)]
        [TestCase("123.46", 123.456789012345)]
        [TestCase("1234567890123.45", 1234567890123.45)]
        [TestCase("123456789012345", 123456789012345.0)]
        [TestCase("0.12", 0.123456789012345)]
        [TestCase("0", 0.000123456789012345)]
        [TestCase("0", 0.0001)]
        [TestCase("0", -0.0001)]
        [TestCase("0", 0.00001)]
        [TestCase("0", -0.00001)]
        [TestCase("0", 0.0000001234567890123456)]
        [TestCase("0", 0.000000000000000123456789012345)]
        [TestCase("123456789012346", 123456789012345.6)]
        [TestCase("1234567890123.46", 1234567890123.456)]
        [TestCase("1234567890123460", 1234567890123455.0)]// 5.5 rounds to 6
#if NETSTANDARD
        [TestCase("1234567890123440", 1234567890123445.0)]// 4.5 rounds to 5
#else
        [TestCase("1234567890123450", 1234567890123445.0)]// 4.5 rounds to 5
#endif
        public void TestDoubleToStr2Decimals(string result, double value)
        {
            Assert.That(FormatterPal.DoubleToStr2Decimals(value), Is.EqualTo(result));
        }

        [Test]
        public void TestIntToStrX()
        {
            Assert.That(FormatterPal.IntToStrX(0), Is.EqualTo("0"));
            Assert.That(FormatterPal.IntToStrX(0x1a0f), Is.EqualTo("1A0F"));
            Assert.That(FormatterPal.IntToStrX(unchecked((int)0xffffffff)), Is.EqualTo("FFFFFFFF"));

            Assert.That(FormatterPal.IntToStrXLower(unchecked((int)0xffff00ff)), Is.EqualTo("ffff00ff"));

            Assert.That(FormatterPal.IntToStrX2(0), Is.EqualTo("00"));
            // These do not truncate to the required length. Should they truncate or not?
            Assert.That(FormatterPal.IntToStrX2(0x1a0f), Is.EqualTo("1A0F"));
            Assert.That(FormatterPal.IntToStrX2(unchecked((int)0xffffffff)), Is.EqualTo("FFFFFFFF"));

            Assert.That(FormatterPal.IntToStrX2Lower(0), Is.EqualTo("00"));
            Assert.That(FormatterPal.IntToStrX2Lower(0xbeef), Is.EqualTo("beef"));

            Assert.That(FormatterPal.IntToStrX8(0), Is.EqualTo("00000000"));
            Assert.That(FormatterPal.IntToStrX8(0xbeef), Is.EqualTo("0000BEEF"));
            Assert.That(FormatterPal.IntToStrX8(unchecked((int)0xffffffff)), Is.EqualTo("FFFFFFFF"));
        }

        [Test]
        public void TestInt64ToStrX()
        {
            Assert.That(FormatterPal.Int64ToStrX(0), Is.EqualTo("0"));
            Assert.That(FormatterPal.Int64ToStrX(0x1a0f), Is.EqualTo("1A0F"));
            Assert.That(FormatterPal.Int64ToStrX(unchecked((long)0xffffffffffffffff)), Is.EqualTo("FFFFFFFFFFFFFFFF"));

            Assert.That(FormatterPal.Int64ToStrX8(0), Is.EqualTo("00000000"));
            Assert.That(FormatterPal.Int64ToStrX8(0xbeef), Is.EqualTo("0000BEEF"));
            Assert.That(FormatterPal.Int64ToStrX8(0xffffffff), Is.EqualTo("FFFFFFFF"));
            Assert.That(FormatterPal.Int64ToStrX8(unchecked((long)0xffffffffffffffff)), Is.EqualTo("FFFFFFFFFFFFFFFF"));
        }

        [Test]
        public void TestIntToStr()
        {
            // IntToStr
            Assert.That(FormatterPal.IntToStr(0), Is.EqualTo("0"));
            Assert.That(FormatterPal.IntToStr(-0), Is.EqualTo("0"));
            Assert.That(FormatterPal.IntToStr(123), Is.EqualTo("123"));

            // IntToStrD2
            Assert.That(FormatterPal.IntToStrD2(0), Is.EqualTo("00"));
            Assert.That(FormatterPal.IntToStrD2(-0), Is.EqualTo("00"));
            Assert.That(FormatterPal.IntToStrD2(1), Is.EqualTo("01"));
            Assert.That(FormatterPal.IntToStrD2(123), Is.EqualTo("123"));

            // IntToStrD4
            Assert.That(FormatterPal.IntToStrD4(0), Is.EqualTo("0000"));
            Assert.That(FormatterPal.IntToStrD4(-0), Is.EqualTo("0000"));
            Assert.That(FormatterPal.IntToStrD4(123), Is.EqualTo("0123"));
            Assert.That(FormatterPal.IntToStrD4(12345678), Is.EqualTo("12345678"));

            // IntToStrD10
            Assert.That(FormatterPal.IntToStrD10(0), Is.EqualTo("0000000000"));
            Assert.That(FormatterPal.IntToStrD10(-0), Is.EqualTo("0000000000"));
            Assert.That(FormatterPal.IntToStrD10(123456789), Is.EqualTo("0123456789"));
        }

        [Test]
        [TestCase("en-NZ", ',', "$", '.', "d/MM/yyyy", "h:mm am/pm")]
        [TestCase("uk-UA", '\xa0', Grivna, ',', "dd.MM.yyyy", UkraineShortTimePattern)]
        public void TestGetCurrentThreadFormats(
            string culture,
            char groupSeparator,
            string currency,
            char decimalSeparator,
            string shortDatePattern,
            string shortTimePattern)
        {
            SystemPal.SetCulture(culture);

            Assert.That(FormatterPal.GetNumberGroupSeparatorCurrent(), Is.EqualTo(groupSeparator));
            Assert.That(FormatterPal.GetCurrencySymbolCurrent(), Is.EqualTo(currency));
            Assert.That(FormatterPal.GetDecimalSeparatorCurrent(), Is.EqualTo(decimalSeparator));
            Assert.That(FormatterPal.GetShortDatePatternCurrent(), Is.EqualTo(shortDatePattern));
            Assert.That(FormatterPal.GetShortTimePatternCurrent(), Is.EqualTo(shortTimePattern));
        }

        [Test]
        [TestCase(double.NaN, "abcd")]
        [TestCase(double.NaN, "$$123")]
        [TestCase(double.NaN, "123.45р.")]
        [TestCase(double.NaN, "123 45")]
        [TestCase(123456789.23, "123,456,,789.23")]
        [TestCase(-123.45, "   -$123.45  ")]
        [TestCase(0.123456789, ".123456789")]
        public void TestTryParseCurrencyCurrentNZD(double result, string value)
        {
            Assert.That(FormatterPal.TryParseCurrencyCurrent(value), Is.EqualTo(result));
        }

        [Test]
        [TestCase(double.NaN, "abcd")]
        [TestCase(double.NaN, "$$123")]
        [TestCase(double.NaN, "$123")]
        [TestCase(123456789.23, "123\x00a0456\x00a0\x00a0789,23")]
#if JAVA
        // This is what I think an acceptable difference on Java.
        // Java refuses to parse this value because it does not accept a double space group separator.
        [TestCase(Double.NaN, "123 456  789,23")]
#else
        [TestCase(123456789.23d, "123 456  789,23")]
#endif
        [TestCase(-123.45, "   -123,45" + GrivnaSuffix + "  ")]
#if JAVA
        // This is what I think an acceptable difference on Java.
        // Java parser fails on spare spaces.
        [TestCase(123456.78, "123456,78 ₴")]
#else
        [TestCase(123456.78, "   123   456,78   ₴  ")]
#endif
        [TestCase(0.123456789, ",123456789" + GrivnaSuffix)]
        public void TestTryParseCurrencyCurrentUA(double result, string value)
        {
            SystemPal.SetCulture("uk-UA");

            Assert.That(FormatterPal.TryParseCurrencyCurrent(value), Is.EqualTo(result));
        }

        [Test]
        public void TestTryParseDateTimeCurrent()
        {
            DateTime expected = new DateTime(1998, 10, 21);
            //Java doesn't support format "YYYY/MM/DD" out of box.
            DateTime parsed = FormatterPal.TryParseDateTimeCurrent("1998/10/21");
            Assert.That(parsed, Is.EqualTo(expected));
        }

        [Test]
        public void TestTryParseDateInvariant()
        {
            DateTime expected = new DateTime(1926, 12, 25);
            DateTime parsed = FormatterPal.TryParseDateTimeInvariant("25 Dec 1926");
            Assert.That(parsed, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("123456,12" + GrivnaSuffix, 123456.123456, true, false, 2)]
        [TestCase("-123456,12" + GrivnaSuffix, -123456.123456, true, false, 2)]
        [TestCase("0,00" + GrivnaSuffix, 0.0, true, false, 2)]
        [TestCase("123\x00a0456,12" + GrivnaSuffix, 123456.123456, true, true, 2)]
        [TestCase("-123456,123456", -123456.123456, false, false, 9)]
        [TestCase("0,0", 0.0, false, false, 9)]
        [TestCase("0,0000001", 0.0000001, false, false, 9)]
        [TestCase("-123\x00a0456,123456", -123456.123456, false, true, 9)]
        [TestCase("0,0", 0.0, false, true, 9)]
        [TestCase("0,0000001", 0.0000001, false, true, 9)]
        public void TestNumberToStrMSWordWithNoFormat(string result, double value, bool isCurrency, bool isUsesGroupSeparator, int numberOfDigitsAfterDecimalpoint)
        {
            // RK This test used to have "ru-RU" culture, but under Windows 8 it behaves very strangely.
            // This test is supposed to be indifferent to the current culture set on the developer's machine,
            // but having Russian as the current culture was affecting the output e.g. "100 р." vs "100р.".
            // So I changed to use Ukrainian. It has same formatting mostly so I kept the code of the test the same, it just does not have the glitch.
            SystemPal.SetCulture("uk-UA");

            string actual = FormatterPal.NumberToStrMSWordWithNoFormat(value, isCurrency, isUsesGroupSeparator, numberOfDigitsAfterDecimalpoint, false);

            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        [TestCase("ru-RU", 123456.123456, "#\x00a0##0,##", NumberFormattingOptions.Default, "123\x00a0456,12")]
        [TestCase("ru-RU", 123456.123456, "# ##0,##", NumberFormattingOptions.Default, "123 456,12")]
        [TestCase("ru-RU", 123456.123456, "#\x00a0##0,##%", NumberFormattingOptions.Default, "123\x00a0456,12%")]
        [TestCase("ru-RU", 123456.123456, "#\x00a0##0,##'%'", NumberFormattingOptions.Default, "123\x00a0456,12%")]
        [TestCase("ru-RU", 123456.123456, "#\x00a0##0,##%", NumberFormattingOptions.IsMultiplyPercent, "12\x00a0345\x00a0612,35%")]
        [TestCase("ru-RU", 123456.123456, "#\x00a0##0,##'%'", NumberFormattingOptions.IsMultiplyPercent, "123\x00a0456,12%")]
        [TestCase("ru-RU", 19791105, "####/##/##", NumberFormattingOptions.Default, "1979/11/05")]
        [TestCase("ru-RU", 19791105, "0000/00/00", NumberFormattingOptions.Default, "1979/11/05")]
        [TestCase("ru-RU", 38527720206, "8 (###) ###'-'##'-'###", NumberFormattingOptions.Default, "8 (385) 277-20-206")]
        [TestCase("ru-RU", 1.01, "$###0.00", NumberFormattingOptions.Default, "$   0.01")]
        [TestCase("ru-RU", 1, "#.", NumberFormattingOptions.Default, "1.")]
        [TestCase("de-AT", 123456.123456, "#\x00a0##0,##", NumberFormattingOptions.Default, "123\x00a0456,12")]
        [TestCase("de-AT", 123456.123456, "# ##0,##", NumberFormattingOptions.Default, "123 456,12")]
        [TestCase("de-AT", 123456.123456, "#\x00a0##0,##%", NumberFormattingOptions.Default, "123\x00a0456,12%")]
        [TestCase("de-AT", 123456.123456, "#\x00a0##0,##'%'", NumberFormattingOptions.Default, "123\x00a0456,12%")]
        [TestCase("de-AT", 123456.123456, "#\x00a0##0,##%", NumberFormattingOptions.IsMultiplyPercent, "12\x00a0345\x00a0612,35%")]
        [TestCase("de-AT", 123456.123456, "#\x00a0##0,##'%'", NumberFormattingOptions.IsMultiplyPercent, "123\x00a0456,12%")]
        [TestCase("de-AT", 19791105, "####/##/##", NumberFormattingOptions.Default, "1979/11/05")]
        [TestCase("de-AT", 19791105, "0000/00/00", NumberFormattingOptions.Default, "1979/11/05")]
        [TestCase("de-AT", 38527720206, "8 (###) ###'-'##'-'###", NumberFormattingOptions.Default, "8 (385) 277-20-206")]
        [TestCase("de-AT", 1.01, "$###0.00", NumberFormattingOptions.Default, "$   0.01")]
        [TestCase("de-AT", 1, "#.", NumberFormattingOptions.Default, "1.")]
        public void TestNumberToStrMSWordWithLocalizedFormat(
            string cultureName,
            double value,
            string format,
            NumberFormattingOptions options,
            string result)
        {
            SystemPal.SetCulture(cultureName);

            string actual = FormatterPal.NumberToStrMSWord(value, format, options);

            Assert.That(actual, Is.EqualTo(result));
        }

        [Test]
        [TestCase("0", 0, ",0;(,0);0")]
        [TestCase("0.0", 0, ",0;(,0);0.0")]
        [TestCase("(0)%", 0, ",0;(,0);(,0.##)%")]
        [TestCase("(1)", -1, "#,###;(#,###);0")]
        [TestCase("1%", 1, ",0.##%;(,0.##)%;0")]
        public void TestNumberToStrMSWordWithLocalizedFormatWithLegacyNumberFormat(
            string result,
            double value,
            string format)
        {
            Assert.That(FormatterPal.NumberToStrMSWord(value, format, NumberFormattingOptions.LegacyNumberFormat), Is.EqualTo(result));
        }

        [Test]
        public void TestJiraJ1010()
        {
            Assert.That(FormatterPal.NumberToStrMSWord(0, ",0;(,0);'-'", NumberFormattingOptions.Default), Is.EqualTo("-"));
        }

        [Test]
        [TestCase("10,00", 10)]
        [TestCase("3,00", 3)]
        public void TestNumberToStrMSWordWithLocalizedFormatWithInvariantCultureNumberFormat(
            string result,
            double value)
        {
            SystemPal.SetCulture("nl-NL");

            Assert.That(FormatterPal.NumberToStrMSWord(value, "0.00", NumberFormattingOptions.FormatIsInInvariantCulture), Is.EqualTo(result));
        }

        [Test]
        [TestCase("01/02/2014", 1022014.0, "0#/##/####")]
        [TestCase(" 1/02/2014", 1022014.0, "##/##/####")]
        [TestCase("01/02/2014", 1022014.0, "00/00/0000")]
        [TestCase("01022014", 1022014.0, "0#######")]
        [TestCase("01022014", 1022014.0, "00000000")]
#if !JAVA
        // 0232014 is octal number in Java. Its decimal representation is 78860.
        // 0232014 is decimal number in .NET. Its decimal representation is 232014.
        // Porter omits the leading zero of octal numbers during porting, making these numbers decimal,
        // so we can not reproduce appropriate behavior in Java.
        [TestCase("00/23/2014", 0232014, "0#/##/####")]
        [TestCase("  /23/2014", 0232014, "##/##/####")]
        [TestCase("00/23/2014", 0232014, "00/00/0000")]
        [TestCase("00232014", 0232014, "0#######")]
        [TestCase("00232014", 0232014, "00000000")]
#endif
        public void TestJira939(string result, double value, string format)
        {
            Assert.That(FormatterPal.NumberToStrMSWord(value, format, NumberFormattingOptions.Default), Is.EqualTo(result));
        }

        [Test]
        [TestCase(4871352, "004A54B8")]
        [TestCase(int.MinValue, "00F73CVUM")]
        public void TestStrToHex(int result, string value)
        {
            Assert.That(FormatterPal.TryParseHex("004A54B8"), Is.EqualTo(4871352));
            Assert.That(FormatterPal.TryParseHex("00F73CVUM"), Is.EqualTo(int.MinValue));
        }

        [Test]
        [TestCase(658, "658abe343nc")]
        [TestCase(-100, "-100")]
        [TestCase(int.MinValue, "a-100")]
        public void TestTryParseIntPortion(int result, string value)
        {
            Assert.That(FormatterPal.TryParseIntPortion("658abe343nc"), Is.EqualTo(658));
            Assert.That(FormatterPal.TryParseIntPortion("-100"), Is.EqualTo(-100));
            Assert.That(FormatterPal.TryParseIntPortion("a-100"), Is.EqualTo(int.MinValue));
        }

        [Test]
        [TestCase("1,234,567.89", ",0.00")]
        [TestCase("1,234,567.89", ",#.00")]
        [TestCase("1,234,568", ",0")]
        [TestCase("1234567.89", "")]
        public void TestNumberToStrMSWordWithLocalizedFormatWithInvalidFormatString(string result, string format)
        {
            Assert.That(FormatterPal.NumberToStrMSWord(1234567.89, format, NumberFormattingOptions.Default), Is.EqualTo(result));
        }

        /// <summary>
        /// Tests that XmlToDateTimeExact always returns UTC time.
        /// </summary>
        [Test]
        [TestCase("2013-04-02T06:52:20.9977400-06:00")]
        [TestCase("2013-04-02T06:52:20.9977400")]
        public void TestXmlToDateTimeExact(string value)
        {
            Assert.That(FormatterPal.XmlToDateTimeExact(value).Kind, Is.EqualTo(DateTimeKind.Utc));
        }

        [Test]
        public void TestXmlToDateTimeExactParsingInvalid()
        {
            Assert.That(FormatterPal.XmlToDateTimeExact("invalid date"), Is.EqualTo(DateTime.MinValue));
        }

        [Test]
        [TestCase("2014-07-11T15:05:28.4647142Z")]
        [TestCase("2014-07-11T15:05:28.4647142+00:00")]
        [TestCase("2013-04-02T06:52:20.9977400-06:00")]
        [TestCase("2013-04-02T06:52:20.9977400")]
        public void TestXmlToDateTimeExactParsing(string value)
        {
            Assert.That(FormatterPal.XmlToDateTimeExact(value), IsNot.EqualTo(DateTime.MinValue));
        }

        /// <summary>
        /// WORDSNET-10143 ArgumentExceptionOutOfRangeException occurs when MailMerge.Execute is called after specifying nl-NL culture
        /// </summary>
        [Test]
        [TestCase(40, "-27,7891307224549")]
        [TestCase(31, "-27,7891307224549")]
        [TestCase(24, "-27,7891307224549")]
        [TestCase(16, "-27,7891307224549")]
        [TestCase(15, "-27,7891307224549")]
        [TestCase(14, "-27,7891307224549")]
        [TestCase(6, "-27,789131")]
        [TestCase(3, "-27,789")]
        [TestCase(2, "-27,79")]
        [TestCase(0, "-28")]
        public void TestDefect10143(int numberOfDigits, string expected)
        {
            SystemPal.SetCulture("nl-NL");

            const double value = -2.7789130722454869533128359829005491862E01;

            string actual = FormatterPal.NumberToStrMSWordWithNoFormat(value, false, false, numberOfDigits, false);
            Assert.That(actual, Is.EqualTo(expected));
        }

        /// <summary>
        /// WORDSNET-10165 Negative values are prefixed with space
        /// </summary>
        [Test]
        [TestCase("( 1)", -1, ",0;(#,0);0")]
        [TestCase("1,000.00", 1000, ",0.00")]
        [TestCase("(1)", -1, ",0;(,0);0")]
        [TestCase("(1.00)", -1, ",0;(,0.00);0")]
        [TestCase("(1,512)", -1512, ",0;(,0);0")]
        [TestCase("(1,246)", 1246, "(,0)")]
        public void TestDefect10165(string result, double value, string format)
        {
            Assert.That(FormatterPal.NumberToStrMSWord(value, format, NumberFormattingOptions.Default), Is.EqualTo(result));
        }

        /// <summary>
        /// WORDSJAVA-946 - Charts in Docx xml markup: roundtrip of number with big precision in scientific format.
        /// Check bugfix in NumberFormatter
        /// </summary>
        [Test]
        public void TestDoubleRoundtripFormat()
        {
            Assert.That(FormatterPal.DoubleToStrRoundtrip(1.0000000000000005E-8), Is.EqualTo("1.0000000000000005E-08"));
        }

        [Test]
        public void TestIsInteger()
        {
            // Java takes care about overflow, whereas .NET considers very big numbers outside the range [int.MinValue,
            // int.MaxValue] as a valid value (because uses Double.TryParse() internally).

            // So, FormatterPal.IsInteger() returns FALSE in Java and TRUE in .NET for all numbers outside of Integer
            // borders [int.MinValue, int.MaxValue]. In all other cases Java strive to copy .NET behaviour.

#if !JAVA
            string bigNumber = "012345678901234567890123456789012345678901234567890123456789";
            Assert.That(FormatterPal.IsInteger(bigNumber), Is.True);
            string positiveBigNumber = "+" + bigNumber;
            Assert.That(FormatterPal.IsInteger(positiveBigNumber), Is.True);
            string negativeBigNumber = "-" + bigNumber;
            Assert.That(FormatterPal.IsInteger(negativeBigNumber), Is.True);
#endif

            long val = (long)int.MinValue;
            Assert.That(FormatterPal.IsInteger(val.ToString()), Is.True);
            val = (long)int.MinValue + 1;
            Assert.That(FormatterPal.IsInteger(val.ToString()), Is.True);
            // Outside Integer borders: false for Java but true for .Net:
            val = (long)int.MinValue - 1;
#if JAVA
            Assert.That(FormatterPal.IsInteger(val.ToString()), Is.False);
#else
            Assert.That(FormatterPal.IsInteger(val.ToString()), Is.True);
#endif

            val = (long)int.MaxValue;
            Assert.That(FormatterPal.IsInteger(val.ToString()), Is.True);
            val = (long)int.MaxValue - 1;
            Assert.That(FormatterPal.IsInteger(val.ToString()), Is.True);
            // Outside Integer borders: false for Java but true for .Net:
            val = (long)int.MaxValue + 1;
#if JAVA
            Assert.That(FormatterPal.IsInteger(val.ToString()), Is.False);
#else
            Assert.That(FormatterPal.IsInteger(val.ToString()), Is.True);
#endif

            Assert.That(FormatterPal.IsInteger("++0"), Is.False);
            Assert.That(FormatterPal.IsInteger("--0"), Is.False);
            Assert.That(FormatterPal.IsInteger("0a"), Is.False);
            Assert.That(FormatterPal.IsInteger(" 2 0"), Is.False);
            Assert.That(FormatterPal.IsInteger(" -2 0"), Is.False);
            Assert.That(FormatterPal.IsInteger("\t\r\n -2 0 \n\r\t"), Is.False);
            Assert.That(FormatterPal.IsInteger("-a"), Is.False);
            Assert.That(FormatterPal.IsInteger("a"), Is.False);


            Assert.That(FormatterPal.IsInteger(" 2"), Is.True);
            Assert.That(FormatterPal.IsInteger("\t20"), Is.True);
            Assert.That(FormatterPal.IsInteger(" 20 "), Is.True);
            Assert.That(FormatterPal.IsInteger(" -20 "), Is.True);
            Assert.That(FormatterPal.IsInteger("\t\r\n -20 \n\r\t"), Is.True);
            Assert.That(FormatterPal.IsInteger("20"), Is.True);
            Assert.That(FormatterPal.IsInteger("+20"), Is.True);
            Assert.That(FormatterPal.IsInteger("-20"), Is.True);

            Assert.That(FormatterPal.IsInteger("123456"), Is.True);
            Assert.That(FormatterPal.IsInteger("+123456"), Is.True);
            Assert.That(FormatterPal.IsInteger("-123456"), Is.True);
        }

        [Test]
        [TestCase(1, "x###", "   1")]
        [TestCase(12, "x###", "  12")]
        [TestCase(123, "x###", " 123")]
        [TestCase(1234, "x###", "1234")]
        [TestCase(12345, "x###", "2345")]
        [TestCase(123456, "x###", "3456")]
        [TestCase(.123456789, "x###", "    ")]
        [TestCase(1.23456789, "x###", "   1")]
        [TestCase(12.3456789, "x###", "  12")]
        [TestCase(123.456789, "x###", " 123")]
        [TestCase(1234.56789, "x###", "1235")]
        [TestCase(12345.6789, "x###", "2346")]
        [TestCase(123456.789, "x###", "3457")]
        [TestCase(1234567.89, "x###", "4568")]
        [TestCase(12345678.9, "x###", "5679")]
        [TestCase(123456789.9, "x###", "6790")]
        [TestCase(.123456789, "x###.#", "    .1")]
        [TestCase(1.23456789, "x###.#", "   1.2")]
        [TestCase(12.3456789, "x###.#", "  12.3")]
        [TestCase(123.456789, "x###.#", " 123.5")]
        [TestCase(1234.56789, "x###.#", "1234.6")]
        [TestCase(12345.6789, "x###.#", "2345.7")]
        [TestCase(123456.789, "x###.#", "3456.8")]
        [TestCase(1234567.89, "x###.#", "4567.9")]
        [TestCase(12345678.9, "x###.#", "5678.9")]
        [TestCase(.123456789, "###.##x", "   .123")]
        [TestCase(1.23456789, "###.##x", "  1.235")]
        [TestCase(12.3456789, "###.##x", " 12.346")]
        [TestCase(123.456789, "###.##x", "123.457")]
        [TestCase(1234.56789, "###.##x", "1234.568")]
        [TestCase(12345.6789, "###.##x", "12345.679")]
        [TestCase(123456.789, "###.##x", "123456.789")]
        [TestCase(1, "'XXXX'x#####", "XXXX     1")]
        [TestCase(123456789, "'XXXX'x#####", "XXXX456789")]
        public void TestNumberFormatDropOption(double value, string format, string expected)
        {
            string actual = FormatterPal.NumberToStrMSWord(
                value,
                format,
                NumberFormattingOptions.FormatIsInInvariantCulture);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(@"\#", @"\1234")]
        [TestCase(@"\\#", @"\\1234")]
        [TestCase(@"\\\#", @"\\\1234")]
        [TestCase(@"#\", @"1234\")]
        [TestCase(@"#\\", @"1234\\")]
        [TestCase(@"#\\\", @"1234\\\")]
        [TestCase(@"\#\", @"\1234\")]
        [TestCase(@"\\#\\", @"\\1234\\")]
        [TestCase(@"\\\#\\\", @"\\\1234\\\")]
        [TestCase(@"\q\w\e\r#", @"\q\w\e\r1234")]
        [TestCase(@"#\q\w\e\r", @"1234\q\w\e\r")]
        [TestCase(@"\q#\w#\e#\r#", @"\q1\w2\e3\r4")]
        public void TestNumberFormatWithBackslashes(string format, string expected)
        {
            string actual = FormatterPal.NumberToStrMSWord(
                1234,
                format,
                NumberFormattingOptions.FormatIsInInvariantCulture);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void TestNumberFormatIgnoreUnmatchedDigitPlaceholder()
        {
            string actual = FormatterPal.NumberToStrMSWord(
                478,
                "#,###",
                NumberFormattingOptions.IgnoreUnmatchedDigitPlaceholder);

            Assert.That(actual, Is.EqualTo("478"));
        }

        private const string Grivna = "₴";
        // White space between number and grivna international symbol.
        private const string GrivnaSuffix = " " + Grivna;

#if NETSTANDARD
        private const string UkraineShortTimePattern = "HH:mm";
#else
        private const string UkraineShortTimePattern = "H:mm";
#endif
    }
}
