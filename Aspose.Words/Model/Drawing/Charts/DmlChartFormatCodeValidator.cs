// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2022 by Ilya Egorov

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Common;

namespace Aspose.Words.Drawing.Charts
{
    /// <summary>
    /// Implements methods for preparing format code (3.8.31 numFmts (Number Formats) ) used to display chart values.
    /// </summary>
    internal static class DmlChartFormatCodeValidator
    {
        /// <summary>
        /// Resolves lower case and upper case 'm' in the format code.
        /// In MS Word format codes there is no difference between m and M, both of them can work as minutes and months. 
        /// If "m" or "mm" code is used immediately after the "h" or "hh" code (for h 1 ours) or immediately before the "ss"
        /// code (for seconds), the application shall display minutes instead of the month.
        /// </summary>
        internal static string CorrectDateFormatCode(string originalFormat)
        {
            // Replace all lowercase 'm' with uppercase 'M'.
            string result = originalFormat.Replace('m', 'M');

            // Replace sequence of 'M' immediately after 'h' with lower case 'm'.
            result = ResolveMinutesVsMonths(result, true);

            // Replace sequence of 'M' immediately before 's' with lower case 'm'.
            result = ResolveMinutesVsMonths(result, false);

            // WORDSNET-10975
            // '[$-409]m/d/yy h:mm AM/PM;@' is standard format code that contains condition, alternative format code and AM/PM.
            // Replace 'AM/PM' with 'tt', which is supported by .NET.
            result = result.Replace("AM/PM", "tt");
            // If [$-F800] is contained in the format code, use the long date format.
            if (gSysDateCodeRegex.IsMatch(result))
                return "D";
            // If [$-F400] is contained in the format code, use the long time format.
            if (gSysTimeCodeRegex.IsMatch(result))
                return "T";

            // remove 'g' from format code, in .NET it means the period or era. MS Word seems to ignore it (see TestJira13957)
            result = result.Replace("g", "");

            // Replace lower case 'h' with uppercase 'H', .NET behavior with lower case 'h' is different from upper case.
            if (!result.Contains("tt"))
                result = result.Replace('h', 'H');

            result = RemoveAdditionalConditions(result);
            return result;
        }

        /// <summary>
        /// Removes from the format code additional conditions.
        /// </summary>
        /// <param name="originalFormat">The specified string with the format code.</param>
        /// <returns>The format code without additional conditions</returns>
        internal static string RemoveAdditionalConditions(string originalFormat)
        {
            // Remove condition at the beginning of format code and alternative format code at the end.
            // [$-409] - This seem to be locale condition
            // WORDSNET-18649 Format code like [$-407] defines language to use. Remove this text from format code.
            string result = gLanguageCodeRegex.Replace(originalFormat, string.Empty);
            result = result.Replace("[$]", "");
            // WORDSNET-22540 Text with conditions, contained in the format code, should be removed.
            result = gSquareBracketRegex.Replace(result, string.Empty);
            // ;@ - is alternative format code.
            result = result.Replace(";@", "");
            return result;
        }

        /// <summary>
        /// Gets the actual format code based on the axis format code, series format code and value format code.
        /// </summary>
        /// <param name="axisFormatCode">The specified axis format code</param>
        /// <param name="isSourceLinked">Indicates, whether series values are linked with the source</param>
        /// <param name="valueFormatCode">The specified value format code</param>
        /// <param name="seriesFormatCode">The specified series format code</param>
        /// <returns>The actual format code</returns>
        internal static string GetFormatCode(string axisFormatCode, bool isSourceLinked, string valueFormatCode,
            string seriesFormatCode)
        {
            return IsGeneralFormatCode(axisFormatCode) || isSourceLinked
                ? (valueFormatCode == null) ? seriesFormatCode : valueFormatCode
                : axisFormatCode;
        }

        /// <summary>
        /// Gets <see cref="CultureInfo"/> taking into account language settings, if they are specified in format code.
        /// </summary>
        /// <param name="originalFormat">the specified format code</param>
        /// <returns><see cref="CultureInfo"/></returns>
        internal static CultureInfo GetCultureInfo(string originalFormat)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            Match match = gLanguageCodeRegex.Match(originalFormat);

            if (match.Length != 0)
            {
                string result = Regex.Replace(match.Value, @"\[\$(\w*)-", "0x");
                result = result.Replace("]", string.Empty);
                int languageValue = Convert.ToInt32(result, 16);
                Language language = (Language)languageValue;
                // Check if language value is defined,
                if (LocaleClassifier.IsDefined(language))
                    culture = CultureInfo.GetCultureInfo(languageValue);
            }

            return culture;
        }

        /// <summary>
        /// When date format 'm/d/yyyy' is specified, MS Word uses current culture date format: change the format
        /// in the same way.
        /// </summary>
        internal static string CorrectDefaultDateFormat(string formatCode)
        {
            return StringUtil.EqualsIgnoreCase(formatCode, "m/d/yyyy")
                ? SystemPal.GetCurrentCulture().DateTimeFormat.ShortDatePattern
                : formatCode;
        }

        internal static bool IsGeneralFormatCode(string formatCode)
        {
            return !StringUtil.HasChars(formatCode) || StringUtil.EqualsIgnoreCase(formatCode, GeneralFormat);
        }

        /// <summary>
        /// Gets format code with text in double quotes and chars escaped with backslash.
        /// </summary>
        internal static string[] GetFormatCodeParts(string format)
        {
            if (IsGeneralFormatCode(format))
                return new string[0];

            format = ValidateFormatCode(format);
            int indexOfGeneral = format.IndexOf(GeneralFormat, StringComparison.OrdinalIgnoreCase);

            if (indexOfGeneral < 0)
                return new string[0];
           
            string firstPart = RemoveQuotesAndSlashes(format.Substring(0, indexOfGeneral));
            int indexOfGeneralEnd = indexOfGeneral + GeneralFormat.Length;
            int lenght = format.Length - indexOfGeneralEnd;
            string secondPart = RemoveQuotesAndSlashes(format.Substring(indexOfGeneralEnd, lenght));

            return new string[] { firstPart, secondPart }; 
        }

        internal static string ValidateFormatCode(string formatCode)
        {
            // If source format code is empty string or null, return it as is.
            if (!StringUtil.HasChars(formatCode))
                return formatCode;

            // If format code is "general" or "standard" return "general" that means normal number format should be used.
            if (StringUtil.EqualsIgnoreCase(formatCode, GeneralFormat) ||
                StringUtil.EqualsIgnoreCase(formatCode, "standard") ||
                StringUtil.EqualsIgnoreCase(formatCode, "estandar") || // WORDSNET-15534 One more variation of general format.
                StringUtil.EqualsIgnoreCase(formatCode, "standaard")) // WORDSNET-10606 "Standaard" by mistake is treated as date format.
                return GeneralFormat;

            // Number format documentation http://exceldesignsolutions.com/a-comprehensive-guide-number-formats-excel
            // WORDSNET-11416 There might be colors in format code. We currently do not support this feature.
            // So simply remove colors. Colors are specified within [] brackets.
            // Color can be set using color name.
            formatCode = Regex.Replace(formatCode, @"\[([Bb]lack|[Ww]hite|[Rr]ed|[Gg]reen|[Bb]lue|[Yy]ellow|[Mm]agenta|[Cc]yan)\]", "");
            // Color index
            formatCode = Regex.Replace(formatCode, @"\[([Cc]olor\s*\d+)\]", "");
            // And color code.
            formatCode = Regex.Replace(formatCode, @"\[(\#\d+)\]", "");

            // WORDSNET-13446 Replace format code like [$€-813] with currency character.
            // Currency character is the first matching group.
            formatCode = Regex.Replace(formatCode, @"\[\$(\S{1})-\d+\]", "$1");

            // According to the documentation '?' digit placeholder follows the same rules as the 0 (zero). 
            // However it seems it behaves as '#', so replace '?' with '#'.
            formatCode = formatCode.Replace("?", "#");

            // * Displays the proceeding [single] character as many times is required to fill the width of the cell. 
            // This proceeding character will always be taken as a literal, regardless of what character is actually used.
            // Remove "*." sequence.
            // WORDSNET - 28617 Text inside double quotes should not be modified.
            formatCode = Regex.Replace(formatCode, @"(""[^""]*"")|\*.", "$1");

            // WORDSNET-15237 If format code is '@', use general format. '@' is special character in excel and means 
            // Displays the text entered in the cell. 
            if (formatCode == "@")
                formatCode = "";

            // According to 3.8.21 of specification the pattern "_)" should be replaced by a space character. This pattern is
            // used to line up the positive and negative values in axis labels. But the space must be trimmed when rendering
            // data labels (See ChartDataLabel.GetStringValue).
            return Regex.Replace(formatCode, @"_.", " ");
        }

        /// <summary>
        /// Replaces 'M' or 'MM' after 'h' or 'hh' or before 's' or 'ss' with lower case 'm' or 'mm'.
        /// afterHours flag indicates whether method should replace 'M' after 'h' or before 's'. 
        /// </summary>
        private static string ResolveMinutesVsMonths(string input, bool afterHours)
        {
            Regex regex = afterHours ? gRightM : gLeftM;
            int mmIndex = afterHours ? RightMGroup : LeftMGroup;

            Match match = regex.Match(input);
            if (StringUtil.HasChars(match.Value))
            {
                string mm = match.Groups[mmIndex].Value;
                return input.Replace(match.Value, match.Value.Replace(mm, mm.ToLower()));
            }

            return input;
        }

        /// <summary>
        /// Return true if string is date format code.
        /// Note: method uses assumption that if string contains date format specifies it is date format string.
        /// </summary>
        internal static bool IsDateFormat(string formatCode)
        {
            if (IsGeneralFormatCode(formatCode))
                return false;

            // First remove from the input string all text between double quotes and character with a backslash.
            // According to OOXML spec, text between double quotes or chars escaped with backslash
            // in formatCode string should appear without changes in the resulting string.
            string testString = gQuotedTextRegex.Replace(formatCode, "");

            // The second regex checks whether the resulting string contains date format specifies.
            return gDateFormatRegex.IsMatch(testString);
        }

        /// <summary>
        /// Indicates whether the format code is "Time Format" (hours, minutes or seconds and not "days", "months" and "years").
        /// </summary>
        internal static bool IsTimeFormat(string formatCode)
        {
            return IsDateFormat(formatCode) && !formatCode.Contains("d") && !formatCode.Contains("y") &&
               ((formatCode.IndexOf("h", StringComparison.OrdinalIgnoreCase) >= 0) ||
                (formatCode.IndexOf("s", StringComparison.OrdinalIgnoreCase) >= 0));
        }

        /// <summary>
        /// Return true if string is percent format code.
        /// Note: method uses assumption that if string contains '%' char it is percent format string.
        /// </summary>
        internal static bool IsPercentFormat(string formatCode)
        {
            if (IsGeneralFormatCode(formatCode))
                return false;

            // First remove from the input string all text between double quotes 
            // and character with a backslash.
            // According to OOXML spec, text between double quotes or chars escaped with backslash
            // in formatCode string should appear without changes in the resulting string.
            string testString = gQuotedTextRegex.Replace(formatCode, "");

            // The second regex checks whether the resulting string contains 
            // date format specifies.
            return gPercenteFormatRegex.IsMatch(testString);
        }

        private static string RemoveQuotesAndSlashes(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            int last = input.Length - 1;
            char slash = '\\';
            char quote = '\"';
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                bool isNextSlash = (i < last) && (input[i + 1] == slash);
                bool isQutesWithSlash = (c == quote) && isNextSlash;
                bool isTripleSlash = (c == slash) && isNextSlash  && (i < (last - 1)) && (input[i + 2] == slash);

                if ((c != quote) && (c != slash))
                {
                    sb.Append(c);
                }
                // Quotes and slashes should be removed from result.
                else
                {
                    // Combination of double quotes and backslash should be rendered as backslash.
                    // The triple backslash should be rendered as backslash.
                    if (isQutesWithSlash || isTripleSlash)
                    {
                        sb.Append(slash);
                        i += isQutesWithSlash ? 1 : 2;
                    }

                    continue;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Regex matches if string contains any of date format specifiers.
        /// </summary>
        private static readonly Regex gDateFormatRegex =
            new Regex("K|[ghHmst]{1,2}|z{1,3}|[dM]{1,4}|y{1,5}|[fF]{1,7}");

        /// <summary>
        /// Regex matches if string contains '%' char.
        /// </summary>
        private static readonly Regex gPercenteFormatRegex = new Regex("%");

        /// <summary>
        /// Regex matches 'M' or 'MM' immediately after 'h' or 'hh'. There might be delimiters between 'm' and 'h'.
        /// </summary>
        private static readonly Regex gRightM =
            new Regex("(h{1,2}([-\\$\\+\\(:\\^'\\{<=/\\)!&~}>\\s]|(\".*\")|(\\\\\\S))*)(M{1,2})", RegexOptions.Compiled);

        /// <summary>
        /// Regex matches 'M' or 'MM' immediately before 's' or 'ss'. There might be delimiters between 'm' and 's'.
        /// </summary>
        private static readonly Regex gLeftM =
            new Regex("(M{1,2})(([-\\$\\+\\(:\\^'\\{<=/\\)!&~}>\\s]|(\".*\")|(\\\\\\S))*s{1,2})");
       
        /// <summary>
        /// Regex matches text between double quotes and chars escaped with backslash.
        /// </summary>
        private static readonly Regex gQuotedTextRegex = new Regex("(\".*\")|(\\\\\\S)", RegexOptions.Compiled);

        /// <summary>
        /// Regex matches if string contains text like '[$-....]' representing language settings.
        /// </summary>
        private static readonly Regex gLanguageCodeRegex = new Regex(@"\[\$-\w*\]");

        /// <summary>
        /// Regex matches if string contains text like '[*]' representing additional conditions.
        /// </summary>
        private static readonly Regex gSquareBracketRegex = new Regex(@"\[\W*\w*\]");
        
        /// <summary>
        /// Regex matches if string contains text like '[$-F800]' representing system long date format.
        /// </summary>
        private static readonly Regex gSysDateCodeRegex = new Regex(@"\[\$-F800]");

        /// <summary>
        /// Regex matches if string contains text like '[$-....]' representing system time format.
        /// </summary>
        private static readonly Regex gSysTimeCodeRegex = new Regex(@"\[\$-F400]");

        internal const string GeneralFormat = "general";

        /// <summary>
        /// Index of M or MM group.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int RightMGroup = 5;

        /// <summary>
        /// Index of M or MM group.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int LeftMGroup = 1;
    }
}
