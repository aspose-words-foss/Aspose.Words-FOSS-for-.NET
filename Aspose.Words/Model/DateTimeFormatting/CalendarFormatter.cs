// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/08/2016 by Edward Voronov

using System;
using System.Globalization;
using System.Text;
using System.Threading;

namespace Aspose.Words
{
    /// <summary>
    /// Formats datetime value according current culture and specific calendar.
    /// </summary>
    internal abstract class CalendarFormatter
    {
        protected CalendarFormatter(Calendar calendar)
        {
            Calendar = calendar;
            mCalendarType = calendar.GetType();
        }

        internal string Format(DateTime value, char token, int tokenLength, string formatString, int tokenPosition)
        {
            // WORDSNET-12995 .NET does not understand 'Y', only 'y'.
            // WORDSNET-4350 .Net does not understand 'D', only 'd'. Same for 'S'.
            if ((token == 'Y') || (token == 'D') || (token == 'S'))
                token = char.ToLower(token);

            CultureInfo culture = GetCulture(token, tokenLength);
            DateTimeFormatInfo formatProvider = GetFormatProvider(culture);
            if (formatProvider == null)
                return FormatCore(value, token, tokenLength);

            mDayPatternMet = mDayPatternMet || (token == 'd');

            // WORDSNET-14872 .NET treats a single char date/time format string as a standard (non-custom) specifier,
            // should prepend % so it treats it as a custom format string.
            StringBuilder pattern = new StringBuilder(tokenLength + 1);
            if (tokenLength == 1)
                pattern.Append('%');

            pattern.Append(token, tokenLength);

            AdjustPattern(value, pattern, token, tokenLength, culture, formatString, tokenPosition);

            // There is a difference between MS Word and .Net formatting of month name for some languages.
            // .Net outputs a genetive form if date pattern is present in the format. Word does not.
            // Besides, .Net outputs the first letter as upper case if day pattern is not present. Word does not.
            // Our implementation is somewhere in between： we make use of a genetive form if day pattern is present *before*
            // the month. Otherwise, we use normal form. So, for August 26 in Russian,
            // for "MMMM dd" we make "Август 26" vs Word's "август 26"
            // for "dd MMMM" we make "26 августа" vs Word's "26 август"
            bool useGenetiveMonth = UseGenetiveMonth(culture) && mDayPatternMet && (token == 'M');
            const string twoDigitDayPattern = "dd";
            if (useGenetiveMonth)
            {
                // This will make ToString() use genetive months.
                pattern.Append(twoDigitDayPattern);
            }

            string formattedValue = value.ToString(pattern.ToString(), formatProvider);

            return useGenetiveMonth
                ? formattedValue.Substring(0, formattedValue.Length - twoDigitDayPattern.Length)
                : formattedValue;
        }

        protected virtual void AdjustPattern(DateTime value, StringBuilder pattern, char token, int tokenLength, CultureInfo culture, string formatString, int tokenPosition)
        {
        }

        private DateTimeFormatInfo GetFormatProvider(CultureInfo culture)
        {
            if (culture == null)
                return null;

            DateTimeFormatInfo info = culture.DateTimeFormat;

            // The DateTimeFormatInfo.Calendar setter overrides some properties, e.g. month or day names. So, we do not apply new calendar if the current of the same type.
            if (info.Calendar.GetType() != mCalendarType)
            {
                info = (DateTimeFormatInfo)info.Clone();
                info.Calendar = Calendar;
            }

            return info;
        }

        private CultureInfo GetCulture(char token, int tokenLength)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            if (IsCultureSupportsCalendar(culture))
                return culture;

            if (UseInvariantCulture(token, tokenLength))
                return InvariantCulture;

            return null;
        }

        protected abstract bool UseInvariantCulture(char token, int tokenLength);

        protected abstract string FormatCore(DateTime value, char token, int tokenLength);

        protected abstract CultureInfo InvariantCulture { get; }

        protected Calendar Calendar { get; }

        private bool IsCultureSupportsCalendar(CultureInfo culture)
        {
            foreach (Calendar calendar in culture.OptionalCalendars)
            {
                if (calendar.GetType() == mCalendarType)
                    return true;
            }

            return false;
        }

        private static bool UseGenetiveMonth(CultureInfo culture)
        {
            switch (culture.LCID)
            {
                case (int)Language.BelarusianBelarus:
                case (int)Language.CzechCzechRepublic:
                case (int)Language.GreekGreece:
                case (int)Language.LithuanianLithuania:
                case (int)Language.PolishPoland:
                case (int)Language.RussianMoldova:
                case (int)Language.RussianRussia:
                case (int)Language.SlovakSlovakia:
                case (int)Language.UkrainianUkraine:
                    return true;
                default:
                    return false;
            }
        }

        private bool mDayPatternMet;

        private readonly Type mCalendarType;
    }
}
