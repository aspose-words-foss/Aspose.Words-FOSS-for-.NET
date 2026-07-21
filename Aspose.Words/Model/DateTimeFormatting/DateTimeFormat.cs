// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/08/2011 by Dmitry Matveenko
using System;
using System.Text;
using Aspose.Numbering;

namespace Aspose.Words
{
    /// <summary>
    /// Implements date and time formatting according to MS Word format strings (\\@ field switch argument).
    /// </summary>
    internal class DateTimeFormat
    {
        /// <summary>
        /// Creates a new instance of DateTimeFormat class for a given date.
        /// </summary>
        protected DateTimeFormat(DateTime dateTime)
            : this (dateTime, new GregorianCalendarFormatter())
        {
        }

        /// <summary>
        /// Creates a new instance of DateTimeFormat class for a given date.
        /// </summary>
        protected DateTimeFormat(DateTime dateTime, CalendarFormatter calendarFormatter)
        {
            DateToFormat = dateTime;
            mCalendarFormatter = calendarFormatter;
        }

        /// <summary>
        /// Formats a DateTime value according to the given MS Word format string (\\@ switch argument)
        /// and East Asian language ID.
        /// </summary>
        internal static string FormatDateTime(string wordFormat, DateTime d, int eastAsianLanguageId, CalendarType calendarType)
        {
            CalendarFormatter calendarFormatter;
            switch (calendarType)
            {
                case CalendarType.Gregorian:
                    calendarFormatter = new GregorianCalendarFormatter();
                    break;
                case CalendarType.Hijri:
                    calendarFormatter = new HijriCalendarFormatter();
                    break;
                case CalendarType.Hebrew:
                    calendarFormatter = new HebrewCalendarFormatter();
                    break;
                case CalendarType.SakaEra:
                case CalendarType.UmAlQura:
                    calendarFormatter = new GregorianCalendarFormatter();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("calendarType");
            }

            DateTimeFormat format = LanguageOnly.Compare(eastAsianLanguageId, LanguageOnly.Japanese)
                ? new JapaneseDateTimeFormat(d)
                : new DateTimeFormat(d, calendarFormatter);

            return format.FormatDateTimeCore(wordFormat);
        }

        /// <summary>
        /// Gets year from date, in Arabic digits.
        /// This is a default implementation that uses Gregorian year.
        /// When overridden in a derived class may use other calendars.
        /// </summary>
        protected virtual string GetYearInArabicDigits(int pictureElementLength, string wordFormat, int charIdx)
        {
            int intYear = DateToFormat.Year;
            string strYear = intYear.ToString();

            bool addLeadingZero = (pictureElementLength > 1) && (intYear < 10);

            return (addLeadingZero)
                ? "0" + strYear
                : strYear;
        }

        /// <summary>
        /// Gets year from date, in East Asian digits.
        /// This is a default implementation that uses Gregorian year.
        /// When overridden in a derived class may use other calendars.
        /// </summary>
        protected virtual string GetYearInEastAsianDigits(int pictureElementLength)
        {
            return GetGregorianYearInJapaneseDigits(DateToFormat);
        }

        private static string GetGregorianYearInJapaneseDigits(DateTime date)
        {
            const int digitsInYear = 4;
            return JapaneseNumber.ToJapaneseDigital(date.Year, digitsInYear);
        }

        /// <summary>
        /// Gets a day name in East Asian language.
        /// This is the default implementation for European languages that returns picture element as literal text.
        /// </summary>
        protected virtual string GetEastAsianDayName()
        {
            return "w";
        }

        /// <summary>
        /// Gets a day name in East Asian language.
        /// This is the default implementation for European languages that returns a week day number as East Asian numeral.
        /// </summary>
        protected virtual string GetEastAsianDayNameOrNumber()
        {
            int dayNumber = (int)DateToFormat.DayOfWeek;
            // Mimic Word. Strangely enough, it returns a numeral for all days except Sunday.
            return DateToFormat.DayOfWeek == DayOfWeek.Sunday
                ? "日"
                : JapaneseNumber.ToJapaneseCounting(dayNumber);
        }

        /// <summary>
        /// Gets a day name in East Asian language, for 'aaa' picture element.
        /// This is the default implementation for European languages that returns picture element as literal text.
        /// </summary>
        protected virtual string GetEastAsianDayNameAaa(int pictureElementLength)
        {
            // Just repeat 'a' the given number of times.
            return new string('a', pictureElementLength);
        }

        /// <summary>
        /// Gets an EastAsian AM/PM designator.
        /// This is the default implementation European languages.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetEastAsianAmpmDesignator()
        {
            return GetJapaneseAmpmHourDesignator(DateToFormat);
        }

        /// <summary>
        /// Formats a given DateTime value according to MS Word format string.
        /// </summary>
        private string FormatDateTimeCore(string wordFormat)
        {
            bool inQuotes = false;
            mFormattedValue.Length = 0;

            int charIdx = 0;
            while (charIdx < wordFormat.Length)
            {
                char currentChar = wordFormat[charIdx];
                int pictureElementLength = 1;

                if(currentChar == '\'')
                {
                    inQuotes = !inQuotes;
                }
                else if (inQuotes)
                {
                    mFormattedValue.Append(currentChar);
                }
                else
                {
                    pictureElementLength = ProcessAmpmPictureElement(wordFormat, charIdx);
                    bool ampmPictureElementFound = (pictureElementLength > 0);

                    if(!ampmPictureElementFound)
                    {
                        // Process the rest of the picture elements.
                        pictureElementLength = ProcessPictureElements(wordFormat, charIdx);
                    }
                }

                charIdx += pictureElementLength;
            }

            return mFormattedValue.ToString();
        }

        /// <summary>
        /// Gets a Japanese Emperor era name for g, gg, ggg patterns.
        /// </summary>
        private string GetJapaneseEra(int pictureElementLength)
        {
            string eraName;
            switch (pictureElementLength)
            {
                case 1:
                    eraName = JapaneseDateTimeFormat.GetJapaneseEraAbbreviatedEnglish(DateToFormat);
                    break;
                case 2:
                    eraName = JapaneseDateTimeFormat.GetJapaneseEraAbbreviated(DateToFormat);
                    break;
                case 3:
                    eraName = JapaneseDateTimeFormat.GetJapaneseEraName(DateToFormat);
                    break;
                default:
                    Debug.Fail("Unexpected character count value in era pattern");
                    eraName = string.Empty;
                    break;
            }
            return eraName;
        }

        /// <summary>
        /// Gets a Japanese Emperor era name for G, GG patterns.
        /// </summary>
        private string GetJapaneseEraGG(int pictureElementLength)
        {
            int gggAdjustedPictureElementLength = pictureElementLength + 1;
            return GetJapaneseEra(gggAdjustedPictureElementLength);
        }

        /// <summary>
        /// Appends a date formatted via pattern that is supported by ToString() to the output.
        /// </summary>
        /// <returns>The length of the processed picture element.</returns>
        private int AppendStandardPattern(string formatString, int tokenPosition, int maxCharRepetions)
        {
            int pictureElementLength = CountSameCharacters(formatString, tokenPosition, maxCharRepetions);

            string formattedFragment = mCalendarFormatter.Format(DateToFormat, formatString[tokenPosition], pictureElementLength, formatString, tokenPosition);
            mFormattedValue.Append(formattedFragment);

            return pictureElementLength;
        }

        /// <summary>
        /// Looks for different forms of AM/PM pattern at the given position of the format string.
        /// If AM/PM pattern is found, appends the formatted date to the output.
        /// </summary>
        /// <returns>The length of the AM/PM pattern found, or 0.</returns>
        private int ProcessAmpmPictureElement(string wordFormat, int charIdx)
        {
            string currentFormatSubstring = wordFormat.Substring(charIdx).ToUpper();

            // WORDSNET-7025 Added "tt" pattern for backward compatibility.
            string[] languageSpecificPictureElements = { "AM/PM", "PM/AM", "A/P", "P/A", "TT" };

            int pictureElementLength = TryAmpmPictureElements(currentFormatSubstring, languageSpecificPictureElements, false);

            bool patternFound = (pictureElementLength > 0);
            if (!patternFound)
            {
                string[] japanesePictureElements = {"JAM/JPM", "JPM/JAM", "AMPM", "PMAM"};
                pictureElementLength = TryAmpmPictureElements(currentFormatSubstring, japanesePictureElements, true);
            }

            return pictureElementLength;
        }

        /// <summary>
        /// Tries AM/PM picture elements from the given array.
        /// If AM/PM pattern is found, appends the formatted date to the output.
        /// </summary>
        private int TryAmpmPictureElements(string currentFormatSubstring, string[] pictureElements, bool isJapanesePattern)
        {
            int pictureElementLength = GetPrefixLength(currentFormatSubstring, pictureElements);
            bool patternFound = (pictureElementLength > 0);
            if (patternFound)
            {
                const string dotNetAmpmPattern = "tt";
                string ampmValue = isJapanesePattern
                    ? GetEastAsianAmpmDesignator()
                    : DateToFormat.ToString(dotNetAmpmPattern);

                mFormattedValue.Append(ampmValue);
            }
            return pictureElementLength;
        }

        /// <summary>
        /// Gets the length of the first prefix from a given array that starts the given string,
        /// or 0 if such prefix is not found.
        /// </summary>
        private static int GetPrefixLength(string hostString, string[] possiblePrefixes)
        {
            foreach(string prefix in possiblePrefixes)
            {
                Debug.Assert(StringUtil.HasChars(prefix));

                if (hostString.StartsWith(prefix, StringComparison.Ordinal))
                    return prefix.Length;
            }

            return 0;
        }

        /// <summary>
        /// Takes a picture element from the given position of the format string
        /// and appends the date formatted according to the picture element to the output.
        /// </summary>
        private int ProcessPictureElements(string wordFormat, int charIdx)
        {
            int pictureElementLength = 1;
            char currentChar = wordFormat[charIdx];
            int maxCharRepetions;
            string formattedFragment;

            switch (currentChar)
            {
                case 'Y':
                case 'y':
                case 'M':
                case 'D':
                case 'd':
                    maxCharRepetions = 4;
                    pictureElementLength = AppendStandardPattern(wordFormat, charIdx, maxCharRepetions);
                    break;
                case 'H':
                case 'h':
                case 'm':
                case 'S':
                case 's':
                    maxCharRepetions = 2;
                    pictureElementLength = AppendStandardPattern(wordFormat, charIdx, maxCharRepetions);
                    break;
                case 'g':
                    maxCharRepetions = 3;
                    pictureElementLength = CountSameCharacters(wordFormat, charIdx, maxCharRepetions);
                    formattedFragment = GetJapaneseEra(pictureElementLength);
                    mFormattedValue.Append(formattedFragment);
                    break;
                case 'G':
                    maxCharRepetions = 2;
                    pictureElementLength = CountSameCharacters(wordFormat, charIdx, maxCharRepetions);
                    formattedFragment = GetJapaneseEraGG(pictureElementLength);
                    mFormattedValue.Append(formattedFragment);
                    break;
                case 'n':
                    maxCharRepetions = 2;
                    pictureElementLength = CountSameCharacters(wordFormat, charIdx, maxCharRepetions);
                    formattedFragment = JapaneseDateTimeFormat.GetJapaneseYearInArabicDigits(pictureElementLength, DateToFormat, wordFormat, charIdx);
                    mFormattedValue.Append(formattedFragment);
                    break;
                case 'e':
                    maxCharRepetions = 2;
                    pictureElementLength = CountSameCharacters(wordFormat, charIdx, maxCharRepetions);
                    formattedFragment = GetYearInArabicDigits(pictureElementLength, wordFormat, charIdx);
                    mFormattedValue.Append(formattedFragment);
                    break;
                case 'E':
                    maxCharRepetions = MaxCharEastAsianYearPictureElementRepetitions;
                    pictureElementLength = CountSameCharacters(wordFormat, charIdx, maxCharRepetions);
                    formattedFragment = GetYearInEastAsianDigits(pictureElementLength);
                    mFormattedValue.Append(formattedFragment);
                    break;
                case 'w':
                    formattedFragment = GetEastAsianDayName();
                    mFormattedValue.Append(formattedFragment);
                    break;
                case 'W':
                    formattedFragment = GetEastAsianDayNameOrNumber();
                    mFormattedValue.Append(formattedFragment);
                    break;
                case 'a':
                    maxCharRepetions = 3;
                    pictureElementLength = CountSameCharacters(wordFormat, charIdx, maxCharRepetions);
                    formattedFragment = GetEastAsianDayNameAaa(pictureElementLength);
                    mFormattedValue.Append(formattedFragment);
                    break;
                case 'O':
                    AppendJapaneseNumeral(DateToFormat.Month);
                    break;
                case 'A':
                    AppendJapaneseNumeral(DateToFormat.Day);
                    break;
                case 'r':
                    maxCharRepetions = 2;
                    pictureElementLength = CountSameCharacters(wordFormat, charIdx, maxCharRepetions);
                    AppendJapaneseAmpmHourDesignator(pictureElementLength);
                    AppendJapaneseHour();
                    break;
                case 'R':
                    maxCharRepetions = 2;
                    pictureElementLength = CountSameCharacters(wordFormat, charIdx, maxCharRepetions);
                    AppendJapaneseAmpmHourDesignator(pictureElementLength);
                    AppendJapaneseHourR(pictureElementLength);
                    break;
                case 'I':
                    AppendJapaneseNumeral(DateToFormat.Minute);
                    break;
                case 'C':
                    AppendJapaneseNumeral(DateToFormat.Second);
                    break;
                default:
                    mFormattedValue.Append(currentChar);
                    break;
            }

            return pictureElementLength;
        }

        protected virtual int MaxCharEastAsianYearPictureElementRepetitions
        {
            get { return 4; }
        }

        /// <summary>
        /// Counts up to maxRepetions occurrences of the character specified by the startPosition in the input string.
        /// </summary>
        private static int CountSameCharacters(string input, int startPosition, int maxRepetitions)
        {
            char startChar = input[startPosition];
            int repetitions = 1;
            int currentPosition = startPosition + 1;
            while ((repetitions < maxRepetitions) && (currentPosition < input.Length) && (input[currentPosition] == startChar))
            {
                repetitions++;
                currentPosition++;
            }
            return repetitions;
        }

        private void AppendJapaneseNumeral(int number)
        {
            mFormattedValue.Append(JapaneseNumber.ToJapaneseCounting(number));
        }

        /// <summary>
        /// Appends hours formatted according to r, rr patterns.
        /// </summary>
        private void AppendJapaneseHour()
        {
            int hour = To12Hour(DateToFormat.Hour);
            AppendJapaneseNumeral(hour);
        }

        private static int To12Hour(int hour24)
        {
            const int twelve = 12;
            int hour12 = hour24 % twelve;
            return (hour12 != 0)
                ? hour12
                : twelve;
        }

        /// <summary>
        /// Appends hours formatted according to R, RR patterns.
        /// </summary>
        private void AppendJapaneseHourR(int pictureElementLength)
        {
            string formattedFragment;
            int hour = DateToFormat.Hour;

            if (pictureElementLength == 1)
            {
                // 24-based hour as Japanese numeral.
                formattedFragment = JapaneseNumber.ToJapaneseCounting(hour);
            }
            else
            {
                // 12-based our in Arabic digits.
                hour = To12Hour(hour);
                formattedFragment = hour.ToString();
            }

            mFormattedValue.Append(formattedFragment);
        }

        /// <summary>
        /// Appends Japanese AM/PM hour designator if needed.
        /// </summary>
        private void AppendJapaneseAmpmHourDesignator(int pictureElementLength)
        {
            bool appendAmpm = pictureElementLength > 1;
            if (appendAmpm)
                mFormattedValue.Append(GetJapaneseAmpmHourDesignator(DateToFormat));
        }

        private static string GetJapaneseAmpmHourDesignator(DateTime date)
        {
            return (date.Hour < 12)
                ? "上午"
                : "下午";
        }

        /// <summary>
        /// Stores the date being formatted.
        /// </summary>
        protected readonly DateTime DateToFormat;

        private readonly CalendarFormatter mCalendarFormatter;

        /// <summary>
        /// Stores the formatted output.
        /// </summary>
        private readonly StringBuilder mFormattedValue = new StringBuilder();
    }
}
