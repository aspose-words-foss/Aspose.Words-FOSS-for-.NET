// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2010 by Dmitry Vorobyev

using System;
using System.Text.RegularExpressions;
using Aspose.Common;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a constant whose value is date/time.
    /// </summary>
    internal class DateTimeConstant : Constant
    {
        internal DateTimeConstant(DateTime value)
            : this(value, CalendarType.Gregorian)
        {
        }

        internal DateTimeConstant(DateTime value, CalendarType calendarType)
        {
            mValue = value;
            mCalendarType = calendarType;
        }

        internal override string TryFormatDateTime(string format, int eastAsianLanguageId, IFieldResultFormatter resultFormatter)
        {
            Debug.Assert(format != null);

            string formattedResult = null;
            if (resultFormatter != null)
#if JAVA
                formattedResult = resultFormatter.formatDateTime(mValue.toJava(), format, mCalendarType);
#else
                formattedResult = resultFormatter.FormatDateTime(mValue, format, mCalendarType);
#endif

            return formattedResult ?? WordUtil.FormatDateTime(mValue, format, eastAsianLanguageId, mCalendarType);
        }

        internal override ConstantType ConstantType
        {
            get { return ConstantType.DateTime; }
        }

        internal override string ValueString
        {
            get { return mValue.ToString(); }
        }

        internal static DateTimeConstant TryParse(string value)
        {
            DateTime valueDateTime = FormatterPal.TryParseDateTimeInvariant(value);

            if (valueDateTime == DateTime.MinValue)
                valueDateTime = TryExtractDateTime(value);

            return valueDateTime != DateTime.MinValue
                ? new DateTimeConstant(valueDateTime)
                : null;
        }

        private static DateTime TryExtractDateTime(string value)
        {
            MatchCollection matches = gDateTimeRegex.Matches(value);

            for (int index = matches.Count - 1; index >= 0; index--)
            {
                Match match = matches[index];

                DateTime result = TryParseDateTimeInvariant(match.Value);
                if (result != DateTime.MinValue)
                    return result;

                string matchWithoutApMpDesignator = GetMatchWithoutApMpDesignator(match);
                result = FormatterPal.TryParseDateTimeInvariant(matchWithoutApMpDesignator);
                if (result != DateTime.MinValue)
                    return result;
            }

            return DateTime.MinValue;
        }

        private static DateTime TryParseDateTimeInvariant(string value)
        {
            return string.IsNullOrEmpty(value)
                ? DateTime.MinValue
                : FormatterPal.TryParseDateTimeInvariant(value);
        }

        private static string GetMatchWithoutApMpDesignator(Match match)
        {
            Group group = match.Groups[1];

            if (!group.Success)
                return null;

            string matchValue = match.Value;
            string groupValue = group.Value;

            return matchValue.Remove(matchValue.Length - groupValue.Length);
        }

        private readonly DateTime mValue;
        private readonly CalendarType mCalendarType;

        private static readonly Regex gDateTimeRegex = new Regex(@"\d+[\./]\d+[\./]\d+(?:\s+\d+[\.:]\d+(?:[\.:]\d+)?(\s+(?:am|pm))?)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}
