// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/07/2012 by Alexey Noskov

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using Aspose.Bidi;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing.Charts.Core.ComplexTypes;
using Aspose.Words.Drawing.Charts.Core.SimpleTypes;
using Aspose.Words.Drawing.Core.Dml;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Drawing.Charts.Core
{
    internal static class DmlChartRenderingUtil
    {
        /// <summary>
        /// Indicates whether the specified <see cref="DmlChart"/> has filled series.
        /// </summary>
        internal static bool IsChartFilled(DmlChart chart)
        {
            if (chart == null)
                return false;

             if (chart.IsBarChart || (chart.ChartType == DmlChartType.AreaChart) || chart.IsPieChart)
                return true;

            if (chart.IsRadarChart)
            {
                DmlRadarChart radarChart = chart as DmlRadarChart;
                return (radarChart.RadarStyle == RadarStyle.Filled);
            }

            return false;
        }

        /// <summary>
        /// Returns the <see cref="DmlChartSpPr"/> that should be used to render negative bars.
        /// </summary>
        internal static DmlChartSpPr GetIfNegativSpPr(DmlExtension extension)
        {
            if (extension != null)
                return extension.DmlChartSpPr;

            // WORDSNET-27537 If extension is not specified, MS Word uses white color for the fill and black for border
            // to render the negative bar.
            DmlChartSpPr spPr = new DmlChartSpPr();
            DmlColor white = DmlColor.CreateFromDrColor(DrColor.White);
            DmlColor black = DmlColor.CreateFromDrColor(DrColor.Black);
            DmlSolidFill whiteFill = new DmlSolidFill(white);
            spPr.Fill = whiteFill;
            spPr.Outline.Fill = new DmlSolidFill(black);
            return spPr;
        }

         /// <summary>
        /// Indicates whether the outline is not rendered.
        /// </summary>
        internal static bool IsOutlineEmpty(DmlOutline outline)
        {
             return (outline == null) || (outline.Fill == null) || (outline.Fill.DmlFillType == DmlFillType.NoFill);
        }

        /// <summary>
        /// Splits the specified array of points into segments if array contains points that must be skipped.
        /// </summary>
        internal static IList<PointF[]> SplitToSegments(PointF[] points, int[] segmentsIndices)
        {
            if (segmentsIndices.Length % 2 > 0)
                throw new ArgumentException("Length of segment indexes array must be even.");

            List<PointF[]> segments = new List<PointF[]>();

            for (int i = 0; i < segmentsIndices.Length; i += 2)
            {
                int startIndex = segmentsIndices[i];
                int segmentLength = segmentsIndices[i + 1];

                PointF[] segment = new PointF[segmentLength];
                Array.Copy(points, startIndex, segment, 0, segmentLength);
                segments.Add(segment);
            }

            return segments;
        }

        /// <summary>
        /// Returns array of float values, each value equals a sum of Y values for the corresponding X value.
        /// The array is used to render stacked charts.
        /// </summary>
        internal static double[] GetStackedMaximums(DmlChart chart)
        {
            double[] maxValues = new double[chart.MaxPointsCount];
            foreach (ChartSeries series in chart.Series)
            {
                for (int i = 0; i < maxValues.Length; i++)
                {
                    // WORDSNET-13069 Stacked maximum value is a range between minimum and maximum stacked values,
                    // that is why calculate it as sum of absolute values.
                    DmlChartValue value = series.Y.Data.GetOriginalValue(i);
                    maxValues[i] += System.Math.Abs(DmlChartValue.IsNullOrNaN(value)? 0d : value.Value);
                }
            }

            // Validate stacked maximum values to prevent dividing by zero.
            for (int i = 0; i < maxValues.Length; i++)
            {
                // If maximum value is zero, reset it to 1 so that upon dividing,
                // the original value stay unchanged.
                if (MathUtil.IsZero(maxValues[i]))
                    maxValues[i] = 1.0f;
            }

            return maxValues;
        }

        /// <summary>
        /// Returns increment of tick index where labels should be rendered.
        /// If tick size is greater than default label font size, returns 1, that means
        /// label at each tick must be rendered.
        /// </summary>
        internal static int GetTickIndexIncrement(float tickSize, float labelSize)
        {
            if (MathUtil.IsGreaterOrEqual(tickSize,labelSize))
                return 1;

            return (int)(labelSize / tickSize + 1);
        }

        /// <summary>
        /// Gets the string representation of the specified <see cref="DateTime"/>.
        /// </summary>
        /// <param name="date">The specified <see cref="DateTime"/></param>
        /// <param name="originalFormat">The specified format code</param>
        /// <returns>The string representation of the date</returns>
        internal static string GetStringDate(DateTime date, string originalFormat)
        {
            CultureInfo ci = DmlChartFormatCodeValidator.GetCultureInfo(originalFormat);
            string correctedFormat = DmlChartFormatCodeValidator.CorrectDefaultDateFormat(originalFormat);
            correctedFormat = DmlChartFormatCodeValidator.CorrectDateFormatCode(correctedFormat);

            // WORDSNET-24366 "MMMMM" in format code should be replaced with the first letter of the month name (3.8.31).
            if (gMonthFirstLetterFormat.IsMatch(correctedFormat))
            {
                // The current culture is read-only. Clone to make changes.
                ci = ci.Clone() as CultureInfo;
                string[] firstLetterMonthNames = GetMonthFirstLettersArray(ci.DateTimeFormat.AbbreviatedMonthNames);
                DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
                // Replace the abbreviated name of the month with the first letter of month.
                dtfi.AbbreviatedMonthNames = firstLetterMonthNames;
                ci.DateTimeFormat = dtfi;
                // Replace the first letter of month format to the abbreviated name of the month.
                correctedFormat = gMonthFirstLetterFormat.Replace(correctedFormat, "MMM");
            }

            return date.ToString(correctedFormat, ci);
        }

        /// <summary>
        /// Gets the array of the first letters of the month names.
        /// </summary>
        private static string[]  GetMonthFirstLettersArray(string[] monthNames)
        {
            string[] firstLettersOfMonthNames = new string[13];

            for (int i = 0; i < monthNames.Length; i++)
                firstLettersOfMonthNames[i] = string.IsNullOrEmpty(monthNames[i]) ? monthNames[i] : monthNames[i].Substring(0, 1);

            return firstLettersOfMonthNames;
        }

        /// <summary>
        /// Gets the string representation of the specified double value.
        /// </summary>
        /// <remarks>
        /// A semicolon in the format code defines sections with separate format strings for positive, negative, and zero
        /// numbers. If the decimal point is defined in the format code, .Net applies the decimal point formatting, and then
        /// the positive/negative/zero formatting. I.e. the format code is "0.0; (0.0); zero" and the value is "-0.03" the
        /// result of value.ToString("0.0; (0.0); zero") is "zero". But MS Word first applies positive/negative/zero
        /// formatting and then the decimal point formatting. So, we have to emulate the MS Word behavior and manually
        /// repeat the sequence of conversions.
        /// </remarks>
        internal static string DoubleToString(double value, string formatCode)
        {
            CultureInfo ci = DmlChartFormatCodeValidator.GetCultureInfo(formatCode);
            string format = DmlChartFormatCodeValidator.RemoveAdditionalConditions(formatCode);
            string[] formats = format.Split(SectionSeparator);

            // WORDSNET-26910  According to OOXML spec, text between double quotes or chars escaped with backslash
            // in formatCode string should appear without changes in the resulting string.
            string[] additionalTextElements = DmlChartFormatCodeValidator.GetFormatCodeParts(formatCode);
            bool containsQuotedText = (formats.Length <= 1 && additionalTextElements.Length > 1);
            format = GetFormatDependingOnValue(value, containsQuotedText, formats);
            format = DmlChartFormatCodeValidator.ValidateFormatCode(format);

            value = (value < 0) && (formats.Length > 1) && !DmlChartFormatCodeValidator.IsGeneralFormatCode(format)
                ? System.Math.Abs(value)
                : value;

            // WORDSNET-22540 The value should be scaled by thousands, if the "comma" specified in the format code.
            string result = ApplyThousandsSeparator(value, format, ci);

            return containsQuotedText
                ? string.Format("{0}{1}{2}", additionalTextElements[0], result, additionalTextElements[1])
                : result;
        }

        /// <summary>
        /// Returns the format applied to the value.
        /// </summary>
        private static string GetFormatDependingOnValue(double value, bool containsQuotedText, string[] formats)
        {
            if (containsQuotedText)
                return DmlChartFormatCodeValidator.GeneralFormat;

            int positiveFormatIndex = 0;
            int negativeFormatIndex = 1;
            int zeroFormatIndex = 2;
            int formatsCount = formats.Length;
            int index;

            if (value > 0)
                index = positiveFormatIndex;
            else if (value < 0)
                index = (formatsCount > 1) ? negativeFormatIndex : 0;
            else
                index = (formatsCount > 2) ? zeroFormatIndex : 0;

            return formats[index];
        }

        /// <summary>
        /// Applies the thousands separator if present in the format code, and returns the string representation of the value.
        /// </summary>
        private static string ApplyThousandsSeparator(double value, string format, CultureInfo ci)
        {
            Match match = gThousandsRegex.Match(format);

            if (StringUtil.HasChars(match.Value))
            {
                string[] commaSplitted = match.Value.Split(ThousandsSeparator);
                int commaCount = commaSplitted.Length - 1;
                double divider = System.Math.Pow(1000, commaCount);
                value /= divider;
            }

            format = gThousandsRegex.Replace(format, "0");
            string result;

            if (StringUtil.HasChars(format) && DmlChartFormatCodeValidator.IsGeneralFormatCode(format))
                // WORDSNET-24367 The general format should be applied if the section format is "general".
                result = GetGeneralFormatStringValue(value);
            else
                // WORDSNET-20483 If one of the section is empty, the value corresponding to the section is not rendered.
                result = string.IsNullOrEmpty(format) ? string.Empty : FormatterPal.DoubleToStr(value, format, ci);

            return result;
        }

        /// <summary>
        /// Get string representation of the double value if format code is "General".
        /// </summary>
        internal static string GetGeneralFormatStringValue(double value)
        {
            return (((System.Math.Abs(value) < 1E-7) || (System.Math.Abs(value) > 9.9999999999E8)) && !MathUtil.IsZero(value))
                    ? FormatterPal.DoubleToExponential(value)
                    : FormatterPal.DoubleToStr9Decimals(value);
        }

        /// <summary>
        /// Returns the first day of a month (year) of the specified date according to the specified <see cref="AxisTimeUnit"/>.
        /// </summary>
        /// <param name="date">The specified <see cref="DateTime"/></param>
        /// <param name="timeUnit">The specified <see cref="AxisTimeUnit"/></param>
        /// <returns>The first day of the month or of the year</returns>
        internal static DateTime GetFirstDay(DateTime date, AxisTimeUnit timeUnit)
        {
            switch (timeUnit)
            {
                case AxisTimeUnit.Months:
                    // Return the first day of the month;
                    return date.AddDays(-date.Day + 1);
                case AxisTimeUnit.Years:
                    // Return the first day of the year.
                    return new DateTime(date.Year, 1, 1);
                case AxisTimeUnit.Days:
                default:
                    return date;
            }
        }

        /// <summary>
        /// Creates new <see cref="DmlChartTx"/> from the specified text string.
        /// </summary>
        internal static DmlChartTx CreateTx(string text)
        {
            DmlChartTx tx = CreateTxCore();

            if (string.IsNullOrEmpty(text))
                return tx;

            // Split text into paragraphs.
            string[] paragraphs = text.Split(new string[] { ControlChar.CrLf }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string paragraph in paragraphs)
                AddTxParagraph(tx, paragraph);

            return tx;
        }

        /// <summary>
        /// Creates new <see cref="DmlChartTx"/> from the specified texts strings.
        /// For each item in the enumeration a separate paragraph is created.
        /// </summary>
        internal static DmlChartTx CreateTx(IEnumerable<string> texts)
        {
            DmlChartTx tx = CreateTxCore();
            foreach (string text in texts)
                AddTxParagraph(tx, text);

            return tx;
        }

        internal static DmlChartTx CreateTxCore()
        {
            DmlChartTx tx = new DmlChartTx();
            tx.RichText = new DmlTextBody();
            return tx;
        }

        internal static void AddTxParagraph(DmlChartTx tx, string text)
        {
            AddTxParagraph(tx, new string[] { text}, new double[1]);
        }

        /// <summary>
        /// Adds <see cref="DmlParagraph"/> with specified text into <see cref="DmlChartTx"/> take into account the baseline value
        /// </summary>
        /// <remarks>
        /// The base line is used when subscript or superscript text should be rendered.
        /// </remarks>
        internal static void AddTxParagraph(DmlChartTx tx, string[] stringValues, double [] baseLineValues)
        {
            DmlParagraph paragraph = new DmlParagraph();
            tx.RichText.AddParagraph(paragraph);

            for (int i = 0; i < stringValues.Length; i++)
            {
                string text = stringValues[i];

                if (string.IsNullOrEmpty(text))
                    continue;

                DmlRun run = new DmlRun();
                run.Text = ReplaceUtf16Chars(text);

                // Set baseline for subscript or superscript rendering.
                if ((i < baseLineValues.Length) && !double.IsNaN(baseLineValues[i]))
                    run.RunProperties.Baseline = baseLineValues[i];

                paragraph.AddElement(run);
            }
        }

        /// <summary>
        /// Replaces UTF16 character codes with a string representation.
        /// </summary>
        /// <param name="text">Specified string</param>
        /// <returns>String with replaced UTF16 character codes</returns>
        internal static string ReplaceUtf16Chars(string text)
        {
            if (!gUtf16CodeRegex.IsMatch(text))
                return text;

            MatchCollection matches = gUtf16CodeRegex.Matches(text);

            foreach (Match mat in matches)
            {
                // Get the string representation of the Unicode code point.
                string utf16Code = "0" + mat.Value.Substring(1, mat.Value.Length - 2);
                // Converts string representation to integer.
                int utfChar = Convert.ToInt32(utf16Code, 16);
                // Get char and convert to string.
                string utf16String = UnicodeUtil.ConvertFromUtf32(utfChar);
                // Replace the char symbols.
                text = Regex.Replace(text, mat.Value, utf16String);
            }

            return text;
        }

        /// <summary>
        /// Converts the specified value to string that can be used as label for axis, trims the value if it is too long.
        /// </summary>
        internal static string GetTrimmedString(DmlChartValue value, int maxStringLength)
        {
            // Sometimes value might be null, in this case simply return an empty string.
            if (DmlChartValue.IsNullOrNone(value) || string.IsNullOrEmpty(value.StringValue))
                return string.Empty;


            return GetTrimmedString(value.StringValue, maxStringLength);
        }

        /// <summary>
        /// Trims the specified string if it is too long.
        /// </summary>
        internal static string GetTrimmedString(string stringVale, int maxStringLength)
        {

            if (string.IsNullOrEmpty(stringVale))
                return stringVale;

            string result = ReplaceUtf16Chars(stringVale);

            if (MathUtil.IsLessOrEqual(maxStringLength, 0f) || MathUtil.IsLessOrEqual(result.Length, maxStringLength))
                return result;

            // Text should not be split at the first char if the first char is a white space.
            string[] words = SpaceRegex.Split(result, 0, 1);
            int maxWordLength = 0;

            foreach (string word in words)
                maxWordLength = System.Math.Max(maxWordLength, word.Length);

            int maxLength = System.Math.Min(maxStringLength, maxWordLength);
            int firstWordLength = System.Math.Min(maxLength, words[0].Length);
            return (maxWordLength < maxStringLength) ? result : GetSrtingWithHorizontalEllipsis(result, firstWordLength);
        }

        /// <summary>
        /// Gets the specified number of letters form the string and appends horizontal ellipsis to the end.
        /// </summary>
        /// <param name="text">The specified text</param>
        /// <param name="numberOfLetters">The number of letters</param>
        /// <returns>The trimmed text with horizontal ellipsis</returns>
        internal static string GetSrtingWithHorizontalEllipsis(string text, int numberOfLetters)
        {
            return string.Format("{0}...", text.Substring(0, numberOfLetters));
        }

        /// <summary>
        /// Adds "space" after "hyphen".
        /// </summary>
        /// <param name="value">The specified string</param>
        /// <returns>String with replacement</returns>
        internal static string AddSpaceAfterHyphen(string value)
        {
            return gHyphenRegex.Replace(value, "- ");
        }

        internal static float CalculateDepth(DmlChart chart, float baseTickWidth, int depthPercent)
        {
            if (!chart.Is3D)
                return 0.0f;

            int barsCount = GetBarCount(chart);

            // The values are required to calculate size of base and depth.
            int gapWidth = (int)chart.ChartPr.GetProperty(DmlChartAttrs.GapWidth);
            int gapDepth = (int)chart.ChartPr.GetProperty(DmlChartAttrs.GapDepth);

            // It seems size of base equals width of bar.
            float baseSize = baseTickWidth / (barsCount + gapWidth / 100.0f);
            float depth = baseSize * ((float)depthPercent / 100) * (1 + (float)gapDepth / 100);

            return depth;
        }

        private static int GetBarCount(DmlChart chart)
        {
            Grouping grouping = chart.ChartPr.Grouping;

            if((chart.ChartType == DmlChartType.Bar3DChart) && (grouping == Grouping.Clustered))
                return chart.Series.Count;

            bool isDistributedSeries = ((chart.ChartType == DmlChartType.Bar3DChart) && (grouping == Grouping.Standard));
            isDistributedSeries |= (chart.ChartType == DmlChartType.Line3DChart);
            isDistributedSeries |= ((chart.ChartType == DmlChartType.Area3DChart) && !chart.IsStacked);

            return isDistributedSeries ? 2 : 1;
        }

        /// <summary>
        /// Returns point on the line defined by two given points with specified X coordinate.
        /// </summary>
        internal static PointF GetLinePoint(PointF p0, PointF p1, double x)
        {
            // Special case - vertical line.
            if (MathUtil.AreEqual(p0.X, p1.X))
                return new PointF((float)x, p0.Y + (p1.Y - p0.Y));

            double slope = (p1.Y - p0.Y) / (p1.X - p0.X);
            double y = slope * (x - p0.X) + p0.Y;
            return new PointF((float)x, (float)y);
        }
        /// <summary>
        /// Indicates whether the specified axis is logarithmic scaled and the value is less or equal to zero.
        /// </summary>
        /// <returns>"True" if the axis is logarithmic scaled and the value is less or equal to zero, otherwise - "false"</returns>
        internal static bool IsWrongLogScaledValue(ChartAxis axis, double value)
        {
            return axis.IsLogScaled && ((value < 0) || MathUtil.IsZero(value));
        }

        /// <summary>
        /// Indicates whether the specified angle is orthogonal.
        /// </summary>
        /// <param name="angle">The specified angle</param>
        /// <returns>"True" if angle in degrees is 90 or -90, otherwise "false"</returns>
        internal static bool IsOrthogonal(double angle)
        {
            double normalizeAngle = MathUtil.NormalizeAngle(angle);
            return MathUtil.AreEqual(normalizeAngle, 90.0d) || MathUtil.AreEqual(normalizeAngle, 270.0d);
        }

        /// <summary>
        /// Converts the specified value to twips, gets the smallest integral value that is less than or equal
        /// to the converted value, and converts it to emus.
        /// </summary>
        internal static double FloorTwipsToEmus(double emus)
        {
            return System.Math.Floor(emus / ConvertUtilCore.EmusPerTwip) * ConvertUtilCore.EmusPerTwip;
        }

        /// <summary>
        /// Regex matches if string contains whitespace characters.
        /// </summary>
        internal static readonly Regex SpaceRegex = new Regex("\\s");

        /// <summary>
        /// Separator used to split format code to the several formats for negative, positive and zero values.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char SectionSeparator = ';';

        /// <summary>
        /// Separator used to scale the number by one thousand.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char ThousandsSeparator = ',';

        /// <summary>
        /// Regex matches if string contains text like '_x...._' representing the UTF16 char code.
        /// </summary>
        private static readonly Regex gUtf16CodeRegex = new Regex("_x\\w{4}_");

        /// <summary>
        /// Regex matches if string contains text like '0,' representing thousands separator.
        /// </summary>
        private static readonly Regex gThousandsRegex = new Regex(@"0,+");

        /// <summary>
        /// Regex matches if string contains "hyphen".
        /// </summary>
        private static readonly Regex gHyphenRegex = new Regex("[\\-]");

        /// <summary>
        /// Regex matches if string contains "MMMMM".
        /// </summary>
        /// <remarks>
        /// A part of the format code that is used to convert datetime. The first letter of the month should be rendered
        /// (3.8.31 Dates and times).
        /// </remarks>
        private static readonly Regex gMonthFirstLetterFormat = new Regex("MMMMM");
    }
}
