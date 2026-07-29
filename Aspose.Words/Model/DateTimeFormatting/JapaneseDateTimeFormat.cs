// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/09/2011 by Dmitry Matveenko
using System;
using Aspose.Numbering;

namespace Aspose.Words
{
    /// <summary>
    /// Implements date and time formatting specific to Japanese language.
    /// </summary>
    internal class JapaneseDateTimeFormat: DateTimeFormat
    {
        /// <summary>
        /// Creates a new instance of JapaneseDateTimeFormat class for a given date.
        /// </summary>
        internal JapaneseDateTimeFormat(DateTime dateTime) : base(dateTime)
        {
        }

        /// <summary>
        /// Gets Japanese era name for a given date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        internal static string GetJapaneseEraName(DateTime date)
        {
            Era era = GetJapaneseEra(date);
            return era != null
                ? era.Name
                : string.Empty;
        }

        /// <summary>
        /// Gets an abbreviated Japanese era name for a given date.
        /// </summary>
        internal static string GetJapaneseEraAbbreviated(DateTime date)
        {
            Era era = GetJapaneseEra(date);
            return era != null
                ? era.Abbreviation
                : string.Empty;
        }

        /// <summary>
        /// Gets an abbreviated Japanese era name in English for a given date.
        /// </summary>
        internal static string GetJapaneseEraAbbreviatedEnglish(DateTime date)
        {
            Era era = GetJapaneseEra(date);
            return era != null
                ? era.EnglishAbbreviation
                : string.Empty;
        }

        /// <summary>
        /// Gets Japanese era year in Arabic digits.
        /// </summary>
        internal static string GetJapaneseYearInArabicDigits(int pictureElementLength, DateTime date, string wordFormat, int charIdx)
        {
            if (gMinSupportedDate > date)
                return string.Empty;

            int intYear = GetJapaneseEraYear(date);

            // Gannen (the first year of an era) case
            if ((intYear == 1) && wordFormat.Substring(charIdx).Contains("年"))
                return "元";

            bool addLeadingZero = (pictureElementLength > 1) && (intYear < 10);
            return addLeadingZero
                ? string.Format("0{0}", intYear)
                : intYear.ToString();
        }

        /// <summary>
        /// Gets Japanese era year in Arabic digits.
        /// </summary>
        protected override string GetYearInArabicDigits(int pictureElementLength, string wordFormat, int charIdx)
        {
            return GetJapaneseYearInArabicDigits(pictureElementLength, DateToFormat, wordFormat, charIdx);
        }

        /// <summary>
        /// Gets Japanese era year as Japanese numeral or Gregorian year in Japanese digits.
        /// </summary>
        /// <param name="pictureElementLength"></param>
        protected override string GetYearInEastAsianDigits(int pictureElementLength)
        {
            string japaneseYear;
            if(pictureElementLength > 1)
            {
                // Gregorian year in Japanese digits.
                japaneseYear = base.GetYearInEastAsianDigits(pictureElementLength);
            }
            else if (gMinSupportedDate <= DateToFormat)
            {
                // Japanese year as Japanese numeral.
                int intYear = GetJapaneseEraYear(DateToFormat);
                japaneseYear = JapaneseNumber.ToJapaneseCounting(intYear);
            }
            else
            {
                japaneseYear = string.Empty;
            }

            return japaneseYear;
        }

        /// <summary>
        /// Gets Japanese week day.
        /// </summary>
        protected override string GetEastAsianDayName()
        {
            return GetJapaneseDayNameAbbreviated(DateToFormat);
        }

        /// <summary>
        /// Gets Japanese week day.
        /// </summary>
        protected override string GetEastAsianDayNameOrNumber()
        {
            return GetJapaneseDayNameAbbreviated(DateToFormat);
        }

        /// <summary>
        /// Gets Japanese weed day for 'aaa' picture element.
        /// </summary>
        /// <param name="pictureElementLength"></param>
        protected override string GetEastAsianDayNameAaa(int pictureElementLength)
        {
            // Only aaa is a valid pattern, everything else is handled by the default implementation.
            const int requiredCharRepetions = 3;
            return requiredCharRepetions == pictureElementLength
                ? GetEastAsianDayName()
                : base.GetEastAsianDayNameAaa(pictureElementLength);
        }

        /// <summary>
        /// Gets AM/PM designator for Japanese AM/PM picture elements.
        /// </summary>
        protected override string GetEastAsianAmpmDesignator()
        {
            return DateToFormat.Hour < 12
                ? "午前"
                : "午後";
        }

        protected override int MaxCharEastAsianYearPictureElementRepetitions
        {
            get { return 2; }
        }

        /// <summary>
        /// Gets Japanese era.
        /// </summary>
        /// <returns>An object representing Japanese era for a given day or null if Japanese era is not defined for this date.</returns>
        private static Era GetJapaneseEra(DateTime date)
        {
            foreach(Era era in gEras)
            {
                if (date >= era.Beginning)
                    return era;
            }

            return null;
        }

        /// <summary>
        /// Gets Japanese era year as integer.
        /// </summary>
        private static int GetJapaneseEraYear(DateTime date)
        {
            Era era = GetJapaneseEra(date);

            if (era != null)
                return date.Year - era.Beginning.Year + 1;

            throw new ArgumentOutOfRangeException("date", date, "Japanese Era is not defined for the given date.");
        }

        /// <summary>
        /// Gets an abbreviated Japanese week day for a given date.
        /// </summary>
        private static string GetJapaneseDayNameAbbreviated(DateTime date)
        {
            switch(date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return "日";
                case DayOfWeek.Monday:
                    return "月";
                case DayOfWeek.Tuesday:
                    return "火";
                case DayOfWeek.Wednesday:
                    return "水";
                case DayOfWeek.Thursday:
                    return "木";
                case DayOfWeek.Friday:
                    return "金";
                case DayOfWeek.Saturday:
                    return "土";
                default:
                    // It should not happen.
                    return string.Empty;
            }
        }

        /// <summary>
        /// Stores the Japanese eras.
        /// </summary>
        /// <remarks>
        /// The eras should go in descending order.
        /// </remarks>
        private static readonly Era[] gEras =
        {
            new Era(new DateTime(2019, 05, 01), "令和", "令", "R"), // Reiwa
            new Era(new DateTime(1989, 01, 08), "平成", "平", "H"), // Heisei
            new Era(new DateTime(1926, 12, 25), "昭和", "昭", "S"), // Showa
            new Era(new DateTime(1912, 07, 30), "大正", "大", "T"), // Taisho
            new Era(new DateTime(1868, 01, 01), "明治", "明", "M")  // Meiji
        };

        /// <summary>
        /// Gets the minimal for which Japanese era and years are defined.
        /// </summary>
        private static readonly DateTime gMinSupportedDate = gEras[gEras.Length - 1].Beginning;

        private class Era
        {
            internal Era(DateTime beginning, string name, string abbreviation, string englishAbbreviation)
            {
                Beginning = beginning;
                Name = name;
                Abbreviation = abbreviation;
                EnglishAbbreviation = englishAbbreviation;
            }

            internal DateTime Beginning { get; }
            internal string Name { get; }
            internal string Abbreviation { get; }
            internal string EnglishAbbreviation { get; }
        }
    }
}
