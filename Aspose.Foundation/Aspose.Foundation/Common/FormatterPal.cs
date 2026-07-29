// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/06/2009 by Roman Korchagin

using System;
using System.Globalization;
using System.Text;
using Aspose.JavaAttributes;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Common
{
    /// <summary>
    /// FOR MANUAL PORTING
    ///
    /// This class provides method to format numbers and date time values into strings
    /// and also to parse them back into values.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    public static class FormatterPal
    {
        private static DateTimeFormatInfo ApplyMsWordTwoDigitYearMax(DateTimeFormatInfo dateTimeFormatInfo)
        {
            DateTimeFormatInfo result = (DateTimeFormatInfo)dateTimeFormatInfo.Clone();
            result.Calendar.TwoDigitYearMax = MsWordTwoDigitYearMax;
            return result;
        }

        /// <summary>
        /// Formats a DateTime value to XML string and adds the timezone designator
        /// as UTC offset (+hh:mm or -hh:mm).
        /// </summary>
        public static string DateTimeToXmlExplicitTimezone([CppArgumentKind(ArgumentKind.ConstReference)] DateTime value)
        {
            return value.ToString("yyyy-MM-ddTHH:mm:sszzz", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats a DateTime value to XML string and adds the UTC "Z" suffix.
        /// </summary>
        /// <remarks>
        /// Assumes that the specified value has <see cref="DateTimeKind.Utc"/>.
        /// </remarks>
        public static string DateTimeToXmlUtc([CppArgumentKind(ArgumentKind.ConstReference)] DateTime value)
        {
            return value.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// This is currently used by ODT export. Does not write any timezone suffix.
        /// </summary>
        public static string DateTimeToXmlNoTimezone([CppArgumentKind(ArgumentKind.ConstReference)] DateTime value)
        {
            return value.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
        }

        public static string DateTimeToStrPdfExplicitTimezone([CppArgumentKind(ArgumentKind.ConstReference)] DateTime value)
        {
            string result = value.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

            // PDF uses special format for timezone designator.
            // For example +03'30'
            string tzd = value.ToString("zzz", CultureInfo.InvariantCulture).Replace(':', '\'') + '\'';

            return result + tzd;
        }

        public static string DateTimeToStr_yyyyMMddHHmmssZ([CppArgumentKind(ArgumentKind.ConstReference)] DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssZ", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses an XML string into a date time value and returns it in UTC.
        /// If the string represents an invalid date, returns <see cref="DateTime.MinValue"/>.
        /// </summary>
        public static DateTime XmlToDateTime([CppArgumentKind(ArgumentKind.ConstReference)] string value)
        {
            return TryParseDateTime(value, CultureInfo.InvariantCulture, false);
        }

        /// <summary>
        /// Parses an XML string into a date time value ignoring time zone and returns it in UTC.
        /// If the string represents an invalid date, returns <see cref="DateTime.MinValue"/>.
        /// </summary>
        public static DateTime XmlToDateTimeIgnoreTimeZone(string value)
        {
            DateTimeOffset dtOffset = TryParseDateTimeOffset(value, CultureInfo.InvariantCulture);
            return (dtOffset == DateTimeOffset.MinValue) ? DateTime.MinValue : dtOffset.DateTime;
        }

        /// <summary>
        /// Parses an XML xs:dateTime string into a date time value and returns it in UTC.
        /// If the string represents and invalid date, returns <see cref="DateTime.MinValue"/>.
        /// </summary>
        /// <remarks>
        /// AM. This method is needed to parse dateTime only if it's in strict Xml formats.
        /// Recommended approach is to use XmlConvert class but I think it can produce porting problems.
        /// It seems that format list is incomplete but it will not make big problems in output document i.e throw or something.
        /// andrnosk: To fix WORDSNET-8479 I have added one more XmlDateTimeFormat, and also I have changed
        /// DateTimeStyles to AssumeLocal because MS Word uses local time zone to calculate datetime value.
        /// </remarks>
        public static DateTime XmlToDateTimeExact([CppArgumentKind(ArgumentKind.ConstReference)] string value)
        {
            DateTime result;
            //Returns DateTime.MinValue if can't parse
            DateTime.TryParseExact(value, gXmlDateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeLocal, out result);
            return result;
        }

        /// <summary>
        /// Formats date and time according to MIME header format.
        /// RK This is according to http://www.faqs.org/rfcs/rfc2822.html
        /// </summary>
        public static string DateTimeToStrRfc2822([CppArgumentKind(ArgumentKind.ConstReference)] DateTime value)
        {
            string dateTime = value.ToString(DateTimeRfc2822Format, DateTimeFormatInfo.InvariantInfo);
            // "Sun, 13 Feb 2011 14:03:04 +13:00" removes ":" in the timezone offset.
            dateTime = dateTime.Remove(dateTime.LastIndexOf(':'), 1);
            return dateTime;
        }

        public static DateTime ParseDateTimeRfc2822([CppArgumentKind(ArgumentKind.ConstReference)] string value)
        {
            // RK I have to specify exact format string because otherwise .NET 1.1 cannot recognize date time.
            return DateTime.ParseExact(value, DateTimeRfc2822Format, DateTimeFormatInfo.InvariantInfo);
        }

        private const string DateTimeRfc2822Format = "ddd, d MMM yyyy HH':'mm':'ss zzzz";

        /// <summary>
        /// Formats an integer value to XML string.
        /// </summary>
        public static string IntToXml(int value)
        {
            return IntToStr(value);
        }

        /// <summary>
        /// Formats an unsigned integer value to XML string.
        /// </summary>
        public static string UIntToXml(uint val)
        {
            return val.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats a long value to XML string.
        /// </summary>
        public static string LongToXml(long val)
        {
            return val.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses an XML string into an integer value.
        ///
        /// WORDSNET-6000 A document has a value "320.0" in an integer property. This is not valid according to
        /// the schema but MS Word reads it okay. We used to parse integer strictly and it threw.
        /// Fixed by parsing doubles and casting to int. This is not best, but okay for now.
        /// As a result of the fix returns 0 if cannot parse the string. This is not the best solution of course, but okay for now.
        /// </summary>
        public static int XmlToInt([CppArgumentKind(ArgumentKind.ConstReference)] string val)
        {
            return MathUtil.CastDoubleToInt(ParseDouble(val));
        }

        /// <summary>
        /// Parses an XML string into an short integer value.
        /// </summary>
        public static int XmlToShortInt([CppArgumentKind(ArgumentKind.ConstReference)] string val)
        {
            return MathUtil.DoubleToShort(ParseDouble(val));
        }

        /// <summary>
        /// Parses an XML string into an unsigned integer value.
        /// </summary>
        public static uint XmlToUInt([CppArgumentKind(ArgumentKind.ConstReference)] string val)
        {
            return uint.Parse(val);
        }

        /// <summary>
        /// Parses an XML string into a long value.
        /// </summary>
        public static long XmlToLong([CppArgumentKind(ArgumentKind.ConstReference)] string val)
        {
            return long.Parse(val);
        }

        /// <summary>
        /// Parses a string into a double value.
        ///
        /// Returns 0 if cannot parse the string. This is not the best solution of course, but okay for now.
        ///
        /// Allows leading sign.
        /// Allows leading and trailing spaces.
        /// Allows "." as a decimal point.
        /// Allows exponent.
        ///
        /// Yes, we do get values with exponents, for example in custom document properties.
        /// </summary>
        public static double ParseDouble([CppArgumentKind(ArgumentKind.ConstReference)] string val)
        {
            double result = TryParseDoubleInvariant(val);
            return double.IsNaN(result) ? 0 : result;
        }

        /// <summary>
        /// In .Net the method has the same body with above ParseDouble(string).
        /// But in java it differs.
        /// </summary>
        public static double ParseDoubleFloatPrecision([CppArgumentKind(ArgumentKind.ConstReference)] string val)
        {
            return ParseDouble(val);
        }

        /// <summary>
        /// Tries to parse a double with the invariant culture. Returns NaN if cannot parse.
        ///
        /// Allows leading sign
        /// Allows leading and trailing spaces.
        /// Allows "." as a decimal point.
        /// Allows exponent.
        /// </summary>
        public static double TryParseDoubleInvariant([CppArgumentKind(ArgumentKind.ConstReference)] string s)
        {
            return TryParseDoubleInvariant(s, false);
        }

        /// <summary>
        /// Tries to parse a double using the invariant culture with the parameter specifying whether to distinguish
        /// thousands separators. Returns NaN if cannot parse.
        /// </summary>
        /// <remarks>
        /// Allows leading sign
        /// Allows leading and trailing spaces.
        /// Allows "." as a decimal point.
        /// Allows exponent.
        /// </remarks>
        public static double TryParseDoubleInvariant(
            [CppArgumentKind(ArgumentKind.ConstReference)] string s,
            bool allowThousands)
        {
            double result;
            NumberStyles style = NumberStyles.Float;
            if (allowThousands)
                style |= NumberStyles.AllowThousands;

            return double.TryParse(s, style, CultureInfo.InvariantCulture, out result) ? result : double.NaN;
        }

        /// <summary>
        /// Tries to parse a float with the invariant culture. Returns NaN if cannot parse.
        ///
        /// Allows leading sign
        /// Allows leading and trailing spaces.
        /// Allows "." as a decimal point.
        /// Allows exponent.
        /// </summary>
        public static float TryParseFloatInvariant([CppArgumentKind(ArgumentKind.ConstReference)] string s)
        {
            float result;
            return float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out result) ? result : float.NaN;
        }

        /// <summary>
        /// Tries to parse a double with the current culture. Returns NaN if cannot parse.
        ///
        /// Allows leading sign.
        /// Allows leading and trailing spaces.
        /// Allows decimal point specified by the current culture.
        /// Allows exponent.
        /// </summary>
        public static double TryParseDoubleCurrent([CppArgumentKind(ArgumentKind.ConstReference)] string s)
        {
            return TryParseDoubleCurrent(s, false);
        }

        /// <summary>
        /// Tries to parse a double using the current culture with the parameter specifying whether to distinguish
        /// thousands separators. Returns NaN if cannot parse.
        /// </summary>
        /// <remarks>
        /// Allows leading sign.
        /// Allows leading and trailing spaces.
        /// Allows decimal point specified by the current culture.
        /// Allows exponent.
        /// </remarks>
        public static double TryParseDoubleCurrent(
            [CppArgumentKind(ArgumentKind.ConstReference)] string s,
            bool allowThousands)
        {
            double result;
            NumberStyles style = NumberStyles.Float;
            if (allowThousands)
                style |= NumberStyles.AllowThousands;

            return double.TryParse(s, style, CultureInfo.CurrentCulture, out result) ? result : double.NaN;
        }

        /// <summary>
        /// Returns true if the string represents a positive integer number or zero.
        /// </summary>
        public static bool IsPositiveIntegerOrZero([CppArgumentKind(ArgumentKind.ConstReference)] string value)
        {
            // We need to provide this just because Double.TryParse needs this.
            double result;
            return double.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out result);
        }

        /// <summary>
        /// Parses an integer using the invariant culture. Returns Int.MinValue if cannot parse.
        ///
        /// Allows leading sign.
        /// Allows leading and trailing spaces.
        /// </summary>
        public static int TryParseInt([CppArgumentKind(ArgumentKind.ConstReference)] string s)
        {
            NullableInt32 value = ParseNullableInt(s);
            return value.GetValueOrDefault(int.MinValue);
        }

        /// <summary>
        /// Parses an integer using the invariant culture.
        ///
        /// Allows leading sign.
        /// Allows leading and trailing spaces.
        /// </summary>
        public static NullableInt32 ParseNullableInt([CppArgumentKind(ArgumentKind.ConstReference)] string s)
        {
            int value;
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
                ? new NullableInt32(value)
                : NullableInt32.Null;
        }

        /// <summary>
        /// Parses an unsigned integer using the invariant culture. Returns Int.MinValue if cannot parse.
        ///
        /// Allows leading sign.
        /// Allows leading and trailing spaces.
        /// </summary>
        public static uint TryParseUInt([CppArgumentKind(ArgumentKind.ConstReference)] string s)
        {
            uint temp;
            return (uint.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out temp)) ? temp : unchecked((uint)int.MinValue);
        }

        /// <summary>
        /// Parses a long using the invariant culture. Returns Long.MinValue if cannot parse.
        ///
        /// Allows leading sign.
        /// Allows leading and trailing spaces.
        /// </summary>
        public static long TryParseLong([CppArgumentKind(ArgumentKind.ConstReference)] string s)
        {
            long temp;
            return (long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out temp)) ? temp : long.MinValue;
        }

        /// <summary>
        /// Try get int from the longest portion of string. Returns Int.MinValue if cannot parse.
        /// </summary>
        public static int TryParseIntPortion(string s)
        {
            int value = int.MinValue;

            // Try get int from the longest portion of string.
            while (s.Length > 0)
            {
                value = TryParseInt(s);

                if (value != int.MinValue)
                    break;

                s = s.Substring(0, s.Length - 1);
            }

            return value;
        }

        /// <summary>
        /// Uses the current culture for parsing. Returns NaN if cannot parse.
        ///
        /// Allows currency symbol.
        /// Allows decimal point.
        /// Allows leading and trailing sign.
        /// Allows leading and trailing whitespace.
        /// Allows thousands separator.
        /// Allows parenthesis.
        /// </summary>
        public static double TryParseCurrencyCurrent([CppArgumentKind(ArgumentKind.ConstReference)] string s)
        {
            double result;
            return double.TryParse(s, NumberStyles.Currency, CultureInfo.CurrentCulture, out result) ? result : double.NaN;
        }

        /// <summary>
        /// Returns <see cref="DateTime.MinValue"/> if cannot parse.
        /// </summary>
        public static DateTime TryParseDateTimeInvariant([CppArgumentKind(ArgumentKind.ConstReference)] string s)
        {
            return TryParseDateTimeInvariant(s, false, true);
        }

        public static DateTime TryParseDateTimeInvariant(
            [CppArgumentKind(ArgumentKind.ConstReference)] string s,
            bool isRoundtripKind)
        {
            return TryParseDateTimeInvariant(s, isRoundtripKind, false);
        }

        /// <summary>
        /// Returns <see cref="DateTime.MinValue"/> if cannot parse.
        /// </summary>
        /// <remarks>
        /// If the date time specifies a time zone, depending on <paramref name="isRoundtripKind"/> it either converts
        /// the value to UTC (<c>false</c>), or keeps original datetime kind (<c>true</c>).
        /// </remarks>
        public static DateTime TryParseDateTimeInvariant(
            [CppArgumentKind(ArgumentKind.ConstReference)] string s,
            bool isRoundtripKind,
            bool useMsWordTwoDigitYearMax)
        {
            DateTimeFormatInfo currentDateTimeFormat = GetCurrentDateTimeFormatInfo();
            DateTimeFormatInfo[] formats = new DateTimeFormatInfo[]
            {
                useMsWordTwoDigitYearMax ? ApplyMsWordTwoDigitYearMax(currentDateTimeFormat) : currentDateTimeFormat,

                // This cultures have opposite date formats: M/d/yyyy vs d/MM/yyyy. It's enough for majority cases.
                useMsWordTwoDigitYearMax ? gEnUsDateTimeFormatMsWordTwoDigitYearMax : gEnUsDateTimeFormat,
                useMsWordTwoDigitYearMax ? gEnNzDateTimeFormatMsWordTwoDigitYearMax : gEnNzDateTimeFormat
            };

            // WORDSNET-4632 MS Word attempts to parse date with various date format, not only current culture relative.
            foreach (DateTimeFormatInfo format in formats)
            {
                DateTime result = TryParseDateTime(s, format, isRoundtripKind);
                if (result != DateTime.MinValue)
                    return result;
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Returns <see cref="DateTime.MinValue"/> if cannot parse.
        /// </summary>
        public static DateTime TryParseDateTimeCurrent([CppArgumentKind(ArgumentKind.ConstReference)] string s)
        {
            return TryParseDateTimeCurrent(s, false);
        }

        /// <summary>
        /// Returns <see cref="DateTime.MinValue"/> if cannot parse.
        /// </summary>
        /// <remarks>
        /// If the date time specifies a time zone, depending on <paramref name="isRoundtripKind"/> it either converts
        /// the value to UTC (<c>false</c>), or keeps original datetime kind (<c>true</c>).
        /// </remarks>
        public static DateTime TryParseDateTimeCurrent(
            [CppArgumentKind(ArgumentKind.ConstReference)] string s,
            bool isRoundtripKind)
        {
            return TryParseDateTime(s, GetCurrentDateTimeFormatInfo(), isRoundtripKind);
        }

        /// <summary>
        /// Tries to parse a date time using the specified culture.
        /// If the date time specifies a time zone, depending on <paramref name="isRoundtripKind"/> it either converts
        /// the value to UTC (<c>false</c>), or keeps original datetime kind (<c>true</c>).
        /// If the date time value is invalid returns <see cref="DateTime.MinValue"/>.
        /// </summary>
        private static DateTime TryParseDateTime([CppArgumentKind(ArgumentKind.ConstReference)] string s,
            IFormatProvider provider, bool isRoundtripKind)
        {
            DateTime result;
            DateTimeStyles styles = isRoundtripKind ? DateTimeStyles.RoundtripKind : DateTimeStyles.AdjustToUniversal;
            return DateTime.TryParse(s, provider, styles, out result) ? result : DateTime.MinValue;
        }

        /// <summary>
        /// Tries to parse a date time offset using the specified culture.
        /// If the date time value is invalid returns <see cref="DateTimeOffset.MinValue"/>.
        /// </summary>
        private static DateTimeOffset TryParseDateTimeOffset(string s, IFormatProvider provider)
        {
            DateTimeOffset result;
            return DateTimeOffset.TryParse(s, provider, DateTimeStyles.None, out result) ? result : DateTimeOffset.MinValue;
        }

        /// <summary>
        /// Parses a string into an integer value.
        ///
        /// RK This throws when cannot parse. This is inconsistent with <see cref="ParseDouble"/>.
        ///
        /// Leading sign is allowed.
        /// Leading space is allowed.
        /// Trailing space is allowed.
        /// </summary>
        [CppSkipDefinition(false)] // replaced with C++ plain implementation
        public static int ParseInt([CppArgumentKind(ArgumentKind.ConstReference)] string val)
        {
            return int.Parse(val, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses a string storing int64 value.
        /// See <see cref="ParseInt"/> for additional concerns.
        /// </summary>
        [CppSkipDefinition(false)] // replaced with C++ plain implementation
        public static long ParseInt64([CppArgumentKind(ArgumentKind.ConstReference)] string val)
        {
            return long.Parse(val, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses a hexadecimal string into an integer value.
        ///
        /// RK This throws when cannot parse. This is inconsistent with <see cref="ParseDouble"/>.
        ///
        /// Leading space is allowed.
        /// Trailing space is allowed.
        /// 0x prefix is not allowed.
        /// </summary>
        [CppSkipDefinition(false)] // replaced with C++ plain implementation
        public static int ParseHex([CppArgumentKind(ArgumentKind.ConstReference)] string val)
        {
            return int.Parse(val, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Parses a hexadecimal string into a long value.
        ///
        /// RK This throws when cannot parse. This is inconsistent with <see cref="ParseDouble"/>.
        ///
        /// Leading space is allowed.
        /// Trailing space is allowed.
        /// 0x prefix is not allowed.
        /// </summary>
        [CppSkipDefinition(false)] // replaced with C++ plain implementation
        public static long ParseHex64([CppArgumentKind(ArgumentKind.ConstReference)] string val)
        {
            return long.Parse(val, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Try parses a hex string into an integer value.
        /// on error return int.MinValue
        /// </summary>
        public static int TryParseHex([CppArgumentKind(ArgumentKind.ConstReference)] string s)
        {
            int result;
            return int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result) ? result : int.MinValue;
        }

        /// <summary>
        /// Parses a string into a boolean value.
        /// Allows leading and trailing space.
        /// Recognized only case-insensitive "true" and "false".
        /// </summary>
        public static bool ParseBoolTrueFalse([CppArgumentKind(ArgumentKind.ConstReference)] string val)
        {
            return bool.Parse(val); // This is invariant culture of course.
        }

        /// <summary>
        /// More info see http://demos.aspose.com:8089/display/org/Do+NOT+use+bool.ToString%28%29
        /// </summary>
        public static string BoolToTrueFalseLower(bool val)
        {
            return (val) ? "true" : "false";
        }

        public static bool IsBoolTrueFalse([CppArgumentKind(ArgumentKind.ConstReference)] string val)
        {
            bool result;
            return bool.TryParse(val, out result);
        }

        public static string IntToStr(int val)
        {
            return val.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats a integer value into a string using a current culture.
        /// </summary>
        public static string IntToStrCurrentCulture(int val)
        {
            return val.ToString(CultureInfo.InvariantCulture);
        }

        public static string IntToStr(long val)
        {
            return val.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a string of decimal digits with the specified
        /// minimum number of digits desired in the resulting string.
        /// </summary>
        public static string IntToStr(int val, int precision)
        {
            string formatSpecifier = "D" + IntToStr(precision);
            return val.ToString(formatSpecifier, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a "01" string.
        /// </summary>
        public static string IntToStrD2(long val)
        {
            return val.ToString("D2", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a "001" string.
        /// </summary>
        public static string IntToStrD3(long val)
        {
            return val.ToString("D3", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a "0001" string.
        /// </summary>
        public static string IntToStrD4(long val)
        {
            return val.ToString("D4", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a "00001" string.
        /// </summary>
        public static string IntToStrD5(long val)
        {
            return val.ToString("D5", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a "0123456789" string.
        /// </summary>
        public static string IntToStrD10(int val)
        {
            return val.ToString("D10", CultureInfo.InvariantCulture);
        }

        public static string IntToStrNoZero(int val)
        {
            return (val == 0) ? "" : IntToStr(val);
        }

        /// <summary>
        /// Returns a hex string.
        /// </summary>
        public static string IntToStrX(int val)
        {
            return val.ToString("X", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a string of hex digits with the specified
        /// minimum number of digits desired in the resulting string.
        /// </summary>
        public static string IntToStrX(int val, int precision, bool useLowerCaseLetters)
        {
            string formatSpecifier = useLowerCaseLetters ? "x" : "X";
            if (precision > 0)
                formatSpecifier += IntToStr(precision);
            return val.ToString(formatSpecifier, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a hex string.
        /// </summary>
        public static string IntToStrXLower(int val)
        {
            return val.ToString("x", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a "1A" string.
        /// </summary>
        public static string IntToStrX2(int val)
        {
            return val.ToString("X2", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a "1a" string.
        /// </summary>
        public static string IntToStrX2Lower(int val)
        {
            return val.ToString("x2", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a "123A" string.
        /// </summary>
        public static string IntToStrX4(int val)
        {
            return val.ToString("X4", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a "12345A" string.
        /// </summary>
        public static string IntToStrX6(int val)
        {
            return val.ToString("X6", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a "1234567A" string.
        /// </summary>
        public static string IntToStrX8(int val)
        {
            return val.ToString("X8", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a hex string.
        /// </summary>
        public static string Int64ToStrX(long val)
        {
            return val.ToString("X", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a "1234567A" string.
        /// </summary>
        public static string Int64ToStrX8(long val)
        {
            return val.ToString("X8", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Formats a double value into a string using an invariant culture.
        ///
        /// There is a leading zero if needed.
        /// Fractions are only displayed if needed.
        ///
        /// On .NET precision is limit to 15 digits after which rounding occurs.
        /// On .NET returns the number in scientific notation if large number or less than E-5.
        ///
        /// On Java it seems the precision is greater and numbers are formatted fully.
        /// </summary>
        public static string DoubleToStr(double val)
        {
#if NETSTANDARD
            return RemoveZeroSign(val.ToString("G15", CultureInfo.InvariantCulture));
#else

            return RemoveZeroSign(val.ToString(CultureInfo.InvariantCulture));
#endif
        }

        /// <summary>
        /// Formats a double value into a string using a current culture.
        /// </summary>
        public static string DoubleToStrCurrentCulture(double val)
        {
            return RemoveZeroSign(val.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// This is used by VML Util.
        /// Add more info about this or refactor later.
        /// </summary>
        public static string DoubleToStr11HashesEZero(double val)
        {
            return RemoveZeroSign(val.ToString("###########e0", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Formats a value into a string using invariant culture and 'R' format.
        /// 'R' format specifier guarantees that a numeric value that is converted to a string will be parsed back into the same numeric value.
        /// Used for DML Charts. Slight difference in values might lead into incorrect chart rendering in MS Word.
        /// So need to use this format.
        /// </summary>
        public static string DoubleToStrRoundtrip(double val)
        {
#if NETSTANDARD
            // double..ToString("R") works differently after .NET Core 3.1 that causes a lot of gold diff red tests.
            //
            // Generally the double.ToString("R") has following logic:
            // - Try to convert the double to string in precision of 15.
            // - Convert the string back to double and compare to the original double.If they are the same, we return the converted string whose precision is 15.
            // - Otherwise, convert the double to string in precision of 17.
            //
            // Implement the same logic to get the same result in .NET Framework and .NET7 tests.

            string g15Str = val.ToString("G15", CultureInfo.InvariantCulture);
            double roundtrip = double.Parse(g15Str, CultureInfo.InvariantCulture);
            if (val == roundtrip)
                return g15Str;
            else
                return val.ToString("G17", CultureInfo.InvariantCulture);
#else
            return RemoveZeroSign(val.ToString("R", CultureInfo.InvariantCulture));
#endif
        }

        /// <summary>
        /// Formats a value into a string using invariant culture and 0.#E+0 format.
        /// </summary>
        public static string DoubleToExponential(double val)
        {
            return RemoveZeroSign(val.ToString("0.#E+0", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Formats a value into a string using invariant culture and '0.' - '0.#########' format.
        /// n denotes the number of '#' in the format. Note: n can be only from 0 to 9.
        /// </summary>
        public static string DoubleToStrNDecimals(double val, int n)
        {
            Debug.Assert(n >= 0 && n <= 9);
            val = Math.Round(val, n);
            return RemoveZeroSign(val.ToString(gFormats[n], CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Formats a value into a string using invariant culture and 0.## format.
        /// </summary>
        public static string DoubleToStr2Decimals(double val)
        {
            // RK This used '#.##' format in the before, but for very small numbers it output
            // an empty string which made invalid XML. It is better to output at least zero instead.
            return DoubleToStrNDecimals(val, 2);
        }

        /// <summary>
        /// Formats a value into a string using invariant culture and 0.######### format.
        /// </summary>
        public static string DoubleToStr9Decimals(double val)
        {
            return DoubleToStrNDecimals(val, 9);
        }

        /// <summary>
        /// Outputs all digits of a value. Never outputs in the scientific notation.
        ///
        /// Do not use method in PDF renderer directly, use PdfWriter.FloatToStr instead.
        /// </summary>
        public static string FloatAsDoubleToStr8Decimals(float val)
        {
            return FloatAsDoubleToStrNDecimals(val, 8);
        }

        /// <summary>
        /// Outputs all digits of a value. Never outputs in the scientific notation.
        ///
        /// Do not use method in PDF renderer directly, use PdfWriter.FloatToStr instead.
        /// </summary>
        public static string FloatAsDoubleToStrNDecimals(float val, int n)
        {
            Debug.Assert(n >= 0 && n <= 9);

            // RK In Java there seems to be no way to format a float with such pattern and currently
            // in Java we convert it to double and then format, but this causes difference in golds
            // because float 419.6 when converted to double equals 419.6000061xxxx.
            //
            // Therefore here in .NET we have to temporarily cast to double before formatting.
            // I think in the future we will write our own float formatting in Java
            // and change this back to float formatting.
            return RemoveZeroSign(((double)val).ToString(gFormats[n], CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Removes the sign from string representation of a zero. I.e. returns "0" for "-0" strings.
        /// This is used for .NET Core 3.0 and later versions.
        /// </summary>
        private static string RemoveZeroSign(string numString)
        {
#if NETSTANDARD
            // WORDSNET-27702 An empty string appears to be a valid argument.
            if ((numString.Length > 0) && numString[0] == '-')
            {
                for (int i = 1; i < numString.Length; i++)
                {
                    char c = numString[i];
                    if (char.IsDigit(c) && (c != '0'))
                        return numString;
                }

                return numString.Substring(1);
            }
#endif

            return numString;
        }

        /// <summary>
        /// Outputs all digits of a value. Never outputs in the scientific notation.
        ///
        /// Do not use method in PDF renderer directly, use PdfWriter.FloatToStr instead.
        /// </summary>
        public static string FloatToStr8Decimals(float val)
        {
            return FloatToStrNDecimals(val, 8);
        }

        /// <summary>
        /// Formats a value into a string using invariant culture and 0.######### format.
        /// </summary>
        public static string FloatAsDoubleToStr9Decimals(float val)
        {
            return FloatAsDoubleToStrNDecimals(val, 9);
        }

        /// <summary>
        /// Formats a value into a string using invariant culture and 0.######### format.
        /// </summary>
        public static string FloatToStr9Decimals(float val)
        {
            return FloatToStrNDecimals(val, 9);
        }

        /// <summary>
        /// Formats a value into a string using invariant culture and '0.' - '0.#########' format.
        /// n denotes the number of '#' in the format. Note: n can be only from 0 to 9.
        /// </summary>
        public static string FloatToStrNDecimals(float val, int n)
        {
            Debug.Assert(n >= 0 && n <= 9);
            return RemoveZeroSign(val.ToString(gFormats[n], CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Formats a float value into a string using an invariant culture.
        /// </summary>
        public static string FloatToStr(float val)
        {
            return RemoveZeroSign(val.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Used when we need to format an MS Word field value and the explicit format string is not given.
        /// We format using a set of flags that we've gathered from the original field value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isCurrency">Specifies whether to format as a currency or as a number.</param>
        /// <param name="isUsesGroupSeparator">This parameter only has effect when isCurrence is false.</param>
        /// <param name="numberOfDigitsAfterDecimalpoint">Nuber of digits after decimal point.</param>
        /// <param name="isUsesNegativeParentheses">Specifies whether to format negative number with parentheses or with minus symbol.</param>
        public static string NumberToStrMSWordWithNoFormat(double value, bool isCurrency, bool isUsesGroupSeparator, int numberOfDigitsAfterDecimalpoint, bool isUsesNegativeParentheses)
        {
            // WORDSNET-10143 A double value contains 15 decimal digits of precision. The Math.Round method throws an exception, if numberOfDigitsAfterDecimalpoint larger then 15.
            const int maxRoundingDigits = 15;
            value = Math.Round(value, numberOfDigitsAfterDecimalpoint > maxRoundingDigits ? maxRoundingDigits : numberOfDigitsAfterDecimalpoint, MidpointRounding.AwayFromZero);
            string format;
            if (isCurrency)
            {
                format = "c";
                if (numberOfDigitsAfterDecimalpoint > CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits)
                    format += numberOfDigitsAfterDecimalpoint;
            }
            else
            {
                // We originally had here a pattern with 15 digits after the fixed point, but .NET seems to sometimes reduce
                // the number of digits displayed, whereas Java does not reduce them. I don't think it is very important
                // to have the full 15 digit precision in MS Word document field evaluation so I cut down the precision
                // here to a level which lets all gold tests on .NET and Java pass.
                StringBuilder formatBuilder = new StringBuilder(isUsesGroupSeparator ? "#,##0." : "0.");
                for (int i = 0; i < numberOfDigitsAfterDecimalpoint; i++)
                {
                    // MS Word does not truncate first edning "0", if result is 0. So result should be "0.0".
                    if (i == 0 && (int)value == 0)
                    {
                        formatBuilder.Append('0');
                        continue;
                    }
                    formatBuilder.Append('#');
                }

                format = formatBuilder.ToString();
            }

            bool useNegativeParentheses = isUsesNegativeParentheses && value < 0;
            if (useNegativeParentheses)
                value = -value;

            string result = RemoveZeroSign(value.ToString(format, CultureInfo.CurrentCulture));

            if (useNegativeParentheses)
                result = string.Format("({0})", result);

            if (isCurrency && !isUsesGroupSeparator)
                result = result.Replace(CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator, "");

            return result;
        }

        public static string NumberToStrMSWord(double value, string format, NumberFormattingOptions options)
        {
            IString result = NumberToStrMSWord(
                value,
                SystemStringAdapter.Create(format),
                options,
                SystemStringBehaviour.Instance);
            return result.ToSystemString();
        }

        public static IString NumberToStrMSWord(
            double value,
            IString format,
            NumberFormattingOptions options,
            IStringBehaviour stringBehaviour)
        {

            IString invariantFormat = NumberFormattingOptionsUtil.HasFormatIsInInvariantCulture(options)
                ? format.Replace(@"\", @"\\")
                : LocalizedFormatToInvariantFormat(format, stringBehaviour);

            return NumberToStrMSWordCore(value, invariantFormat, options, stringBehaviour);
        }

        private static IString NumberToStrMSWordCore(
            double value,
            IString format,
            NumberFormattingOptions options,
            IStringBehaviour stringBehaviour)
        {
            // In .NET and Java the presence of a % character in a format string causes a number to be multiplied by 100
            // before it is formatted. Word seems to do the same when formatting form fields, but this doesn't happen when
            // formatting expressions in formulas. So we replace % with '%' when we don't want the multiplication to happen.
            // This code used a regex to replace, but the regex was hard to port to Java and I wrote this simplified code
            // on Java and then ported it back to .NET.
            if (!NumberFormattingOptionsUtil.HasIsMultiplyPercent(options))
            {
                bool isPercentFormat = (format.IndexOf("%") >= 0) && (format.IndexOf("'%'") < 0);
                if (isPercentFormat)
                    format = format.Replace("%", "'%'");
            }

            if (NumberFormattingOptionsUtil.HasLegacyNumberFormat(options))
            {
                string legacyFormat = EnsureDigitPlaceholderBeforeGroupSeparator(value, format.ToSystemString());
                string legacyResult = RemoveZeroSign(value.ToString(legacyFormat, CultureInfo.CurrentCulture));
                return stringBehaviour
                    .CreateBuilder()
                    .Append(legacyResult, null)
                    .ToIString();
            }

            return NumberFormatter.NumberToString(
                format,
                value,
                CultureInfo.CurrentCulture,
                options,
                stringBehaviour);
        }

        private static string EnsureDigitPlaceholderBeforeGroupSeparator(double value, string format)
        {
            // WORDSNET-6921 format string begin with comma - it is accepted by MS Word but invalid in .Net.
            // WORDSNET-7517 ".0,00" format string in Dutch culture - MS Word interprets '.' as thousands separator.
            // WORDSNET-7593 a format string can have multiple sections each of which can be started with any character
            // and can have no digit placeholder before a number group separator at the same time.
            //
            // 1. Start iterate through the source format characters.
            // 2. Continue with format builder characters if any change of the source format is to be applied.
            //
            // Bugs can not be reproduced if value less than 1000
            if (Math.Abs(value) < 1000)
                return format;

            StringBuilder formatBuilder = null;
            bool isDigitPlaceholderMet = false;
            bool isInLiteral = false;
            int i = 0;
            while (i < ((formatBuilder != null) ? formatBuilder.Length : format.Length))
            {
                char c = (formatBuilder != null) ? formatBuilder[i] : format[i];
                switch (c)
                {
                    case '0':
                    case '#':
                    case 'x':
                        // A digit placeholder is met.
                        if (!isInLiteral)
                            isDigitPlaceholderMet = true;
                        break;
                    case ',':
                        // An invariant group separator is met.
                        if (!isDigitPlaceholderMet)
                        {
                            // Create on the first demand.
                            if (formatBuilder == null)
                                formatBuilder = new StringBuilder(format);

                            // Add a zero-width digit placeholder.
                            formatBuilder.Insert(i, '#');
                            isDigitPlaceholderMet = true;

                            // Jump over inserted character.
                            i++;
                        }
                        break;
                    case ';':
                        // New section is started.
                        isDigitPlaceholderMet = false;
                        break;
                    case '\'':
                        // String literal.
                        isInLiteral = !isInLiteral;
                        break;
                    default:
                        break;
                }
                i++;
            }

            // Build new format if any change of the source format is to be applied.
            return formatBuilder != null
                ? formatBuilder.ToString()
                : format;
        }

        /// <summary>
        /// WORDSNET-711 Replaces the current number format items with invariant culture items because we use the .NET formatting facility.
        /// </summary>
        private static IString LocalizedFormatToInvariantFormat(IString format, IStringBehaviour stringBehaviour)
        {
            NumberFormatInfo currentInfo = NumberFormatInfo.CurrentInfo;
            IStringBuilder builder = stringBehaviour.CreateBuilder();
            int i = 0;

            while (i < format.Length)
            {
                IChar c = format[i];
                if (RegionMatches(format, i, currentInfo.NumberDecimalSeparator))
                {
                    builder.Append('.', c);
                    i += currentInfo.NumberDecimalSeparator.Length;
                }
                else if (RegionMatches(format, i, currentInfo.NumberGroupSeparator))
                {
                    builder.Append(',', c);
                    i += currentInfo.NumberGroupSeparator.Length;
                }
                else if (RegionMatches(format, i, currentInfo.PercentSymbol))
                {
                    builder.Append('%', c);
                    i += currentInfo.PercentSymbol.Length;
                }
                else if (RegionMatches(format, i, currentInfo.PerMilleSymbol))
                {
                    builder.Append('‰', c);
                    i += currentInfo.PerMilleSymbol.Length;
                }
                else
                {
                    switch (c.ToSystemChar())
                    {
                        case '.':
                        case ',':
                        case '\\':
                            builder.Append('\\', c);
                            break;
                        default:
                            break;
                    }

                    builder.Append(c);
                    i++;
                }
            }

            // WORDSJAVA-488 German group separator ' in Pattern is interpreted as quote start
            // under current Culture like English (and most other).
            // Let's try ' as group separator only if the pattern contains unmatched quotes.
            int numOfQuotes = 0;
            for (int j = 0; j < builder.Length; j++)
            {
                if (builder[j].ToSystemChar() == '\'' && (j == 0 || builder[j - 1].ToSystemChar() != '\\'))
                    numOfQuotes++;
            }
            if (numOfQuotes % 2 == 1)
                builder.Replace("#'##", "#,##");

            return builder.ToIString();
        }

        /// <summary>
        /// This method is a shortcut to make the calling code shorter.
        /// </summary>
        private static bool RegionMatches(IString a, int indexA, string b)
        {
            return string.CompareOrdinal(
                       a.ToSystemString(),
                       indexA,
                       b,
                       0,
                       b.Length) == 0;
        }

        public static string GetShortDatePatternCurrent()
        {
            return GetCurrentDateTimeFormatInfo().ShortDatePattern;
        }

        public static string GetShortTimePatternCurrent()
        {
            string donNetShortTimePattern = GetCurrentDateTimeFormatInfo().ShortTimePattern;

            // On implementing 4848 - Japanese date picture format elements, date formatting no longer
            // passes through non-Word patterns. Only Word patterns should be used.
            const string DotNetAmpmPattern = "tt";
            const string WordAmpmPattern = "am/pm";
            return donNetShortTimePattern.Replace(DotNetAmpmPattern, WordAmpmPattern);
        }

        /// <summary>
        /// Returns native that is platform-specific (i.e. .Net or Java) short date pattern
        /// of the current thread culture.
        /// </summary>
        /// <returns>short date pattern</returns>
        /// <remarks>
        /// This method is the opposite to GetXxxPatternCurrent method family,
        /// which tends to return MS Word-specific patterns.
        /// </remarks>
        public static string GetCurrentNativeShortDatePattern()
        {
            return GetShortDatePatternForCulture(null);
        }

        /// <summary>
        /// Returns native that is platform-specific (i.e. .Net or Java) long time pattern
        /// of the certain culture.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="culture"/> is null, the current thread culture is used.
        /// </remarks>
        public static string GetShortDatePatternForCulture(CultureInfo culture)
        {
            DateTimeFormatInfo dateTimeFormatInfo = GetCertainDateTimeFormatInfo(culture);
            return dateTimeFormatInfo.ShortDatePattern;
        }

        /// <summary>
        /// Returns native that is platform-specific (i.e. .Net or Java) long time pattern
        /// of the current thread culture.
        /// </summary>
        /// <returns>long time pattern</returns>
        /// <remarks>
        /// This method is the opposite to GetXxxPatternCurrent method family,
        /// which tends to return MS Word-specific patterns.
        /// </remarks>
        public static string GetCurrentNativeLongTimePattern()
        {
            return GetLongTimePatternForCulture(null);
        }

        /// <summary>
        /// Returns native that is platform-specific (i.e. .Net or Java) long time pattern
        /// of the certain culture.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="culture"/> is null, the current thread culture is used.
        /// </remarks>
        public static string GetLongTimePatternForCulture(CultureInfo culture)
        {
            DateTimeFormatInfo dateTimeFormatInfo = GetCertainDateTimeFormatInfo(culture);
            return dateTimeFormatInfo.LongTimePattern;
        }

        /// <summary>
        /// Returns time separator of the current thread culture.
        /// </summary>
        /// <returns>time separator</returns>
        public static string GetCurrentTimeSeparator()
        {
            return GetTimeSeparatorForCulture(null);
        }

        /// <summary>
        /// Returns time separator of the certain culture.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="culture"/> is null, the current thread culture is used.
        /// </remarks>
        /// <returns>time separator</returns>
        public static string GetTimeSeparatorForCulture(CultureInfo culture)
        {
            // Note that .Net implementation of the <see cref="DateTimeFormatInfo.TimeSeparator"/>
            // returns first occurance of time separator in long time pattern for underlying culture.
            // So Java code must implement the same behavior to avoid any related issues.
            DateTimeFormatInfo dateTimeFormatInfo = GetCertainDateTimeFormatInfo(culture);
            return dateTimeFormatInfo.TimeSeparator;
        }

        private static DateTimeFormatInfo GetCertainDateTimeFormatInfo(CultureInfo culture)
        {
            return culture != null
                ? culture.DateTimeFormat
                : GetCurrentDateTimeFormatInfo();
        }

        /// <summary>
        /// If <see cref="DateTimeFormatInfo.CurrentInfo"/> clone is present then returns
        /// its value, otherwise returns <see cref="DateTimeFormatInfo.CurrentInfo"/> itself.
        /// </summary>
        /// <returns><see cref="DateTimeFormatInfo"/> object for the current thread</returns>
        private static DateTimeFormatInfo GetCurrentDateTimeFormatInfo()
        {
            if (gCurrentDateTimeFormatInfoClone != null)
                return gCurrentDateTimeFormatInfoClone;

            return DateTimeFormatInfo.CurrentInfo;
        }

#region Tests

        /// <summary>
        /// Sets native short date pattern for the current thread culture in testing purpose.
        /// </summary>
        /// <param name="shortDatePattern">short date pattern</param>
        public static void SetCurrentNativeShortDatePattern(string shortDatePattern)
        {
            EnsureCurrentDateTimeFormatInfoClone();
            gCurrentDateTimeFormatInfoClone.ShortDatePattern = shortDatePattern;
        }

        /// <summary>
        /// Sets native long time pattern for the current thread culture in testing purpose.
        /// </summary>
        /// <param name="longTimePattern">long time pattern</param>
        public static void SetCurrentNativeLongTimePattern(string longTimePattern)
        {
            EnsureCurrentDateTimeFormatInfoClone();
            gCurrentDateTimeFormatInfoClone.LongTimePattern = longTimePattern;
        }

        /// <summary>
        /// Sets time separator of the current thread culture in testing purpose.
        /// </summary>
        /// <param name="timeSeparator">time separator</param>
        public static void SetCurrentTimeSeparator(string timeSeparator)
        {
            EnsureCurrentDateTimeFormatInfoClone();
            gCurrentDateTimeFormatInfoClone.TimeSeparator = timeSeparator;
        }

        /// <summary>
        /// Stores <see cref="DateTimeFormatInfo.CurrentInfo"/> clone if it's not stored yet.
        /// </summary>
        private static void EnsureCurrentDateTimeFormatInfoClone()
        {
            if (gCurrentDateTimeFormatInfoClone == null)
                gCurrentDateTimeFormatInfoClone = (DateTimeFormatInfo)DateTimeFormatInfo.CurrentInfo.Clone();
        }

        /// <summary>
        /// Sets current thread date/time format properties to initial values, which were
        /// before any of <see cref="FormatterPal"/> class method call changes to be applied.
        /// </summary>
        public static void SetInitialCurrentDateTimeFormatInfo()
        {
            gCurrentDateTimeFormatInfoClone = null;
        }

#endregion Tests

        public static char GetDecimalSeparatorCurrent()
        {
            string separator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
            return (StringUtil.HasChars(separator)) ? separator[0] : '.';
        }

        public static char GetNumberGroupSeparatorCurrent()
        {
            string separator = NumberFormatInfo.CurrentInfo.NumberGroupSeparator;
            return (StringUtil.HasChars(separator)) ? separator[0] : ',';
        }

        public static int GetNumberGroupSizeCurrent()
        {
            int[] sizes = NumberFormatInfo.CurrentInfo.NumberGroupSizes;
            return (sizes.Length > 0) ? sizes[0] : 0;
        }

        public static char GetListSeparatorCurrent()
        {
            string separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            return (StringUtil.HasChars(separator)) ? separator[0] : ',';
        }

        public static string GetCurrencySymbolCurrent()
        {
            return NumberFormatInfo.CurrentInfo.CurrencySymbol;
        }

        public static void SetCurrencySymbolCurrent(string currencySymbol)
        {
            NumberFormatInfo.CurrentInfo.CurrencySymbol = currencySymbol;
        }

        /// <summary>
        /// WORDSNET-3679 For "fr-CH" culture might be set either of the two currency symbols - "SFr." or "fr.".
        /// WORDSNET-17864 For "fr-CH" culture the currency symbol is "CHF" in .NET 4.0 and above.
        /// </summary>
        public static string NormalizeCurrencySymbols(string money)
        {
            string currentCulture = SystemPal.GetCurrentCultureName();

            if (FrenchSwissCulture.Equals(currentCulture, StringComparison.Ordinal))
            {
                money = NormalizeCurrencySymbols(money, OldMainFrChCurrency);
                money = NormalizeCurrencySymbols(money, OldOptionalFrChCurrency);

                // Looks like this group separator is also not supported by .NET 4.0.
                money = money.Replace("\'", string.Empty);
            }

            return money;
        }

        private static string NormalizeCurrencySymbols(string money, string oldCurrencySymbol)
        {
            if (money.IndexOf(oldCurrencySymbol, StringComparison.Ordinal) >= 0)
            {
                money = money.Replace(oldCurrencySymbol, string.Empty);
                money = string.Format("{0} {1}", money, FrChCurrency);
            }

            return money;
        }

        /// <summary>
        /// Parses an XML string into an unsigned integer value.
        /// </summary>
        public static int XmlToIntAsUnsigned([CppArgumentKind(ArgumentKind.ConstReference)] string val)
        {
            return XmlToInt(val) & 0xFFFF;
        }

        /// <summary>
        /// Returns true if the string represents an integer number.
        /// </summary>
        public static bool IsInteger([CppArgumentKind(ArgumentKind.ConstReference)] string value)
        {
            // We need to provide this just because Double.TryParse needs this.
            double result;
            return double.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        public static string DoubleToStr(double val, string format, CultureInfo ci)
        {
            return RemoveZeroSign(val.ToString(format, ci));
        }

        public static string DoubleToStr(double value, string format)
        {
            return DoubleToStr(value, format, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Stores <see cref="DateTimeFormatInfo.CurrentInfo"/> writable clone.
        /// </summary>
        [ThreadStatic]
        private static DateTimeFormatInfo gCurrentDateTimeFormatInfoClone;

        private const string FrenchSwissCulture = "fr-CH";
        private const string FrChCurrency = "CHF";
        private const string OldMainFrChCurrency = "SFr.";
        private const string OldOptionalFrChCurrency = "fr.";

        private static readonly string[] gXmlDateTimeFormats =
        {
            "yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz",
            "yyyy-MM-ddTHH:mm:ss.FFFFFFFK",
            "yyyy-MM-ddTHH:mm:ss.FFFFFFF",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss%K",
            // WORDSNET-24344 Added another one format.
            "yyyy-MM-dd"
        };

        private static readonly string[] gFormats =
        {
            "0.",
            "0.#",
            "0.##",
            "0.###",
            "0.####",
            "0.#####",
            "0.######",
            "0.#######",
            "0.########",
            "0.#########"
        };

        private static readonly DateTimeFormatInfo gEnUsDateTimeFormat = SystemPal.GetCulture("en-US").DateTimeFormat;
        private static readonly DateTimeFormatInfo gEnNzDateTimeFormat = SystemPal.GetCulture("en-NZ").DateTimeFormat;
        private static readonly DateTimeFormatInfo gEnUsDateTimeFormatMsWordTwoDigitYearMax = ApplyMsWordTwoDigitYearMax(gEnUsDateTimeFormat);
        private static readonly DateTimeFormatInfo gEnNzDateTimeFormatMsWordTwoDigitYearMax = ApplyMsWordTwoDigitYearMax(gEnNzDateTimeFormat);

        private const int MsWordTwoDigitYearMax = 2029;
    }
}
