// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/05/2017 by Konstantin Sidorenko

#if !NETSTANDARD

using System;
using Aspose.Common;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Autoportable tests of Hebrew and Arabic DateTime formatting: the test should pass on Java after autoporting.
    ///
    /// Formally these tests should be in JavaMs.Tests module but moved here to:
    /// 1. Remove links from JavaMs.Tests to Aspose.Words (WordUtil.FormatDateTime()).
    /// 2. To place duplication in the same place: DateTime.ToString() and WordUtil.FormatDateTime()
    ///    have the same tests.
    /// </summary>
    [TestFixture]
    public class TestWordUtilFormatDateTime
    {
        [SetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WordUtil.FormatDateTime() should produce the same Hebrew DateTime string constants (month, day, etc. names) on .Net and Java.
        /// </summary>
        [Test]
        public void TestHebrewDateFormattingWordUtil()
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetCulture("he-IL");

                for (int i = 1; i <= gHebrewMonthDays.Length; i++)
                {
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, i), "d", 0, CalendarType.Hebrew), Is.EqualTo(gHebrewMonthDays[i - 1]), "Month's day - d");
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, i), "dd", 0, CalendarType.Hebrew), Is.EqualTo(gHebrewMonthDays[i - 1]), "Month's day - dd");

                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, i), "D", 0, CalendarType.Hebrew), Is.EqualTo(gHebrewMonthDays[i - 1]), "Month's day - D");
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, i), "DD", 0, CalendarType.Hebrew), Is.EqualTo(gHebrewMonthDays[i - 1]), "Month's day - DD");
                }

                // Short week day name - ddd
                string[] shortWeekDays = { "ב", "ג", "ד", "ה", "ו", "ש", "א" };
                for (int i = 1; i <= shortWeekDays.Length; i++)
                {
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, i), "ddd", 0, CalendarType.Hebrew), Is.EqualTo(shortWeekDays[i - 1]), "Short name of week's day - ddd");
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, i), "DDD", 0, CalendarType.Hebrew), Is.EqualTo(shortWeekDays[i - 1]), "Short name of week's day - DDD");
                }

                // Full week day name - dddd
                for (int i = 1; i <= gHebrewWeekDays.Length; i++)
                {
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, i), "dddd", 0, CalendarType.Hebrew), Is.EqualTo(gHebrewWeekDays[i - 1]), "Long name of week's day - dddd");
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, i), "DDDD", 0, CalendarType.Hebrew), Is.EqualTo(gHebrewWeekDays[i - 1]), "Long name of week's day - DDDD");
                }

                // Months - M,MM
                string[] shortMonths = { "ד'", "ה'", "ו'", "ז'", "ח'", "ט'", "י'", "י\"א", "י\"ב", "א'", "ב'", "ג'" };
                for (int i = 1; i <= shortMonths.Length; i++)
                {
                    Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, i, 1), "M", 0, CalendarType.Hebrew)), Is.EqualTo(shortMonths[i - 1]), "Months - M");
                    Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, i, 1), "MM", 0, CalendarType.Hebrew)), Is.EqualTo(shortMonths[i - 1]), "Months - MM");
                }

                // Months - MMM,MMMM
                string[] months = { "טבת", "שבט", "אדר", "ניסן", "אייר", "סיון", "תמוז", "אב", "אלול", "תשרי", "חשון", "כסלו" };
                for (int i = 1; i <= months.Length; i++)
                {
                    Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, i, 1), "MMM", 0, CalendarType.Hebrew)), Is.EqualTo(months[i - 1]), "Months - MMM");
                    Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, i, 1), "MMMM", 0, CalendarType.Hebrew)), Is.EqualTo(months[i - 1]), "Months - MMMM");
                }

                // Years - y,yy,yyy,yyyy
                string[] years = { "תשס\"ז", "תשס\"ח", "תשס\"ט", "תש\"ע", "תשע\"א", "תשע\"ב", "תשע\"ג", "תשע\"ד", "תשע\"ה", "תשע\"ו", "תשע\"ז", "תשע\"ח" };
                for (int i = 1; i <= years.Length; i++)
                {
                    Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007 + i - 1, 1, 1), "y", 0, CalendarType.Hebrew)), Is.EqualTo(years[i - 1]), "Years - y");
                    Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007 + i - 1, 1, 1), "yy", 0, CalendarType.Hebrew)), Is.EqualTo(years[i - 1]), "Years - yy");
                    Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007 + i - 1, 1, 1), "yyy", 0, CalendarType.Hebrew)), Is.EqualTo(years[i - 1]), "Years - yyy");
                    Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007 + i - 1, 1, 1), "yyyy", 0, CalendarType.Hebrew)), Is.EqualTo(years[i - 1]), "Years - yyyy");
                }

                // Hours - hh,HH
                for (int i = 1; i < 24; i++)
                {
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1).AddHours(i), "hh", 0, CalendarType.Hebrew), Is.EqualTo(AlignNumber(i > 12 ? i - 12 : i, 2)), "Hours - hh");
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1).AddHours(i), "HH", 0, CalendarType.Hebrew), Is.EqualTo(AlignNumber(i, 2)), "Hours - HH");
                }

                // Minutes, seconds
                for (int i = 0; i < 60; i++)
                {
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1).AddMinutes(i), "m", 0, CalendarType.Hebrew), Is.EqualTo(AlignNumber(i, 1)), "Minutes - m");
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1).AddMinutes(i), "mm", 0, CalendarType.Hebrew), Is.EqualTo(AlignNumber(i, 2)), "Minutes - mm");

                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1).AddSeconds(i), "s", 0, CalendarType.Hebrew), Is.EqualTo(AlignNumber(i, 1)), "Seconds - s");
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1).AddSeconds(i), "ss", 0, CalendarType.Hebrew), Is.EqualTo(AlignNumber(i, 2)), "Seconds - ss");
                }

                // AP/PM
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1, 10, 0, 0), "tt", 0, CalendarType.Hebrew), Is.EqualTo("AM"), "AP/PM - tt");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1, 17, 0, 0), "tt", 0, CalendarType.Hebrew), Is.EqualTo("PM"), "AP/PM - tt");


            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// DateTime.ToString() should produce the same Hebrew DateTime string constants (month, day, etc. names) on .Net and Java.
        /// </summary>
        [Test]
        public void TestHebrewDateFormattingToString()
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetCulture("he-IL");

                // ShortDatePattern	M/d/yyyy - d
                for (int i = 1; i <= 31; i++)
                    Assert.That(new DateTime(2007, 1, i).ToString("d"), Is.EqualTo(AlignNumber(i, 2) + "/01/2007"), "ShortDatePattern\tM/d/yyyy - d");

                // Month's day - dd
                for (int i = 1; i <= 31; i++)
                    Assert.That(new DateTime(2007, 1, i).ToString("dd"), Is.EqualTo(AlignNumber(i, 2)), "Month's day - dd");

                // Short week day name - ddd
                string[] shortWeekDays = { "יום ב", "יום ג", "יום ד", "יום ה", "יום ו", "שבת", "יום א" };
                for (int i = 1; i <= shortWeekDays.Length; i++)
                    Assert.That(new DateTime(2007, 1, i).ToString("ddd"), Is.EqualTo(shortWeekDays[i - 1]), "Short name of week's day - ddd");

                // Hebrew full week day name - dddd
                for (int i = 1; i <= gHebrewWeekDays.Length; i++)
                    Assert.That(new DateTime(2007, 1, i).ToString("dddd"), Is.EqualTo(gHebrewWeekDays[i - 1]), "Long name of week's day - dddd");

                // LongDatePattern dddd, MMMM dd, yyyy - D
                string[] longDates =
                {
                    "יום שני 01 ינואר 2007",
                    "יום שלישי 02 ינואר 2007",
                    "יום רביעי 03 ינואר 2007",
                    "יום חמישי 04 ינואר 2007",
                    "יום שישי 05 ינואר 2007",
                    "שבת 06 ינואר 2007",
                    "יום ראשון 07 ינואר 2007",
                };
                for (int i = 1; i <= longDates.Length; i++)
                    Assert.That(new DateTime(2007, 1, i).ToString("D"), Is.EqualTo(longDates[i - 1]), "LongDatePattern dddd, MMMM dd, yyyy - D");

                // The period or era - gg
                Assert.That(new DateTime(2007, 1, 1).ToString("gg"), Is.EqualTo("לספירה"), "The period or era - gg");

                // Months and date - M
                string[] dayAndMonths =
                {
                    "01 ינואר",
                    "01 פברואר",
                    "01 מרץ",
                    "01 אפריל",
                    "01 מאי",
                    "01 יוני",
                    "01 יולי",
                    "01 אוגוסט",
                    "01 ספטמבר",
                    "01 אוקטובר",
                    "01 נובמבר",
                    "01 דצמבר"
                };
                for (int i = 1; i <= dayAndMonths.Length; i++)
                    Assert.That(new DateTime(2007, i, 1).ToString("M"), Is.EqualTo(dayAndMonths[i - 1]), "Months and date - M");

                // Number of month - MM
                for (int i = 1; i <= dayAndMonths.Length; i++)
                    Assert.That(new DateTime(2007, i, 1).ToString("MM"), Is.EqualTo(AlignNumber(i, 2)), "Number of month - MM");

                // Short month name - MMM
                string[] shortMonths = { "ינו", "פבר", "מרץ", "אפר", "מאי", "יונ", "יול", "אוג", "ספט", "אוק", "נוב", "דצמ" };
                for (int i = 1; i <= shortMonths.Length; i++)
                    Assert.That(new DateTime(2007, i, 1).ToString("MMM"), Is.EqualTo(shortMonths[i - 1]), "Short month name - MMM");

                // Long month name - MMMM
                string[] longMonths =
                {
                    "ינואר",
                    "פברואר",
                    "מרץ",
                    "אפריל",
                    "מאי",
                    "יוני",
                    "יולי",
                    "אוגוסט",
                    "ספטמבר",
                    "אוקטובר",
                    "נובמבר",
                    "דצמבר"
                };
                for (int i = 1; i <= longMonths.Length; i++)
                    Assert.That(new DateTime(2007, i, 1).ToString("MMMM"), Is.EqualTo(longMonths[i - 1]), "Long month name - MMMM");

                // Month and year - y
                string[] yearAndMonths =
                {
                    "ינואר 2007",
                    "פברואר 2007",
                    "מרץ 2007",
                    "אפריל 2007",
                    "מאי 2007",
                    "יוני 2007",
                    "יולי 2007",
                    "אוגוסט 2007",
                    "ספטמבר 2007",
                    "אוקטובר 2007",
                    "נובמבר 2007",
                    "דצמבר 2007"
                };
                for (int i = 1; i <= yearAndMonths.Length; i++)
                    Assert.That(new DateTime(2007, i, 1).ToString("y"), Is.EqualTo(yearAndMonths[i - 1]), "Month and year - y");

                // Year - yy
                for (int i = 7; i <= 13; i++)
                    Assert.That(new DateTime(2000 + i, 1, 1).ToString("yy"), Is.EqualTo(AlignNumber(i, 2)), "Year - yy");

                // Year - yyyy
                for (int i = 2007; i <= 2013; i++)
                    Assert.That(new DateTime(i, 1, 1).ToString("yyyy"), Is.EqualTo(AlignNumber(i, 1)), "Year - yyyy");

                // Hours - hh,HH
                for (int i = 1; i < 24; i++)
                {
                    Assert.That(new DateTime(2007, 1, 1).AddHours(i).ToString("hh"), Is.EqualTo(AlignNumber(i > 12 ? i - 12 : i, 2)), "Hours - hh");
                    Assert.That(new DateTime(2007, 1, 1).AddHours(i).ToString("HH"), Is.EqualTo(AlignNumber(i, 2)), "Hours - HH");
                }

                // Minutes, seconds
                for (int i = 0; i < 60; i++)
                {
                    Assert.That(new DateTime(2007, 1, 1).AddMinutes(i).ToString("mm"), Is.EqualTo(AlignNumber(i, 2)), "Minutes - mm");
                    Assert.That(new DateTime(2007, 1, 1).AddSeconds(i).ToString("ss"), Is.EqualTo(AlignNumber(i, 2)), "Seconds - ss");
                }

                // AP/PM
                Assert.That(new DateTime(2007, 1, 1, 10, 0, 0).ToString("tt"), Is.EqualTo("AM"), "AP/PM - tt");
                Assert.That(new DateTime(2007, 1, 1, 17, 0, 0).ToString("tt"), Is.EqualTo("PM"), "AP/PM - tt");

            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// Check that WordUtil.FormatDateTime() produces the same Hebrew and Arabic string constants on different platforms.
        /// </summary>
        [Test]
        public void TestArabicDateFormattingWordUtil()
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetCulture("ar-SA");

                // Month's day - d,dd
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1), "d", 0, CalendarType.Hijri), Is.EqualTo("12"), "ShortDatePattern - d");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 20), "d", 0, CalendarType.Hijri), Is.EqualTo("2"), "ShortDatePattern - d");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 2, 18), "d", 0, CalendarType.Hijri), Is.EqualTo("1"), "ShortDatePattern - d");

                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1), "dd", 0, CalendarType.Hijri), Is.EqualTo("12"), "ShortDatePattern - dd");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 20), "dd", 0, CalendarType.Hijri), Is.EqualTo("02"), "ShortDatePattern - dd");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 2, 18), "dd", 0, CalendarType.Hijri), Is.EqualTo("01"), "ShortDatePattern - dd");

                // Short name of week's day - ddd
                string[] weekDays = { "الاثنين", "الثلاثاء", "الاربعاء", "الخميس", "الجمعة", "السبت", "الاحد" };
                for (int i = 1; i <= weekDays.Length; i++)
                {
                    Assert.That(NormalizeString(WordUtil.FormatDateTime(
                        new DateTime(2007, 1, i), "ddd", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString(weekDays[i - 1])), "Short name of week's day - ddd");
                    Assert.That(NormalizeString(WordUtil.FormatDateTime(
                        new DateTime(2007, 1, i), "DDD", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString(weekDays[i - 1])), "Short name of week's day - DDD");
                }

                // Long name of week's day - dddd
                string[] longWeekDays = { "الاثنين", "الثلاثاء", "الاربعاء", "الخميس", "الجمعة", "السبت", "الاحد" };
                for (int i = 1; i <= longWeekDays.Length; i++)
                {
                    Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 1, i), "dddd", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString(longWeekDays[i - 1])), "Long name of week's day - dddd");
                    Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 1, i), "DDDD", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString(longWeekDays[i - 1])), "Long name of week's day - DDDD");
                }

                // Months and date - M
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1), "M", 0, CalendarType.Hijri), Is.EqualTo("12"), "Months and date - M");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 20), "M", 0, CalendarType.Hijri), Is.EqualTo("1"), "Months and date - M");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 2, 18), "M", 0, CalendarType.Hijri), Is.EqualTo("2"), "Months and date - M");

                // Number of month - MM
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1), "MM", 0, CalendarType.Hijri), Is.EqualTo("12"), "Months and date - MM");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 20), "MM", 0, CalendarType.Hijri), Is.EqualTo("01"), "Months and date - MM");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 2, 18), "MM", 0, CalendarType.Hijri), Is.EqualTo("02"), "Months and date - MM");

                // Short month name - MMM
                Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 1, 1), "MMM", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString("ذو الحجة")), "Short month name - MMM");
                Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 1, 20), "MMM", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString("محرم")), "Short month name - MMM");
                Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 2, 19), "MMM", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString("صفر")), "Short month name - MMM");
                Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 3, 20), "MMM", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString("ربيع الاول")), "Short month name - MMM");
                Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 4, 18), "MMM", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString("ربيع الثاني")), "Short month name - MMM");
                Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 5, 18), "MMM", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString("جمادى الاولى")), "Short month name - MMM");
                Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 6, 16), "MMM", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString("جمادى الثانية")), "Short month name - MMM");
                Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 7, 15), "MMM", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString("رجب")), "Short month name - MMM");
                Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 8, 14), "MMM", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString("شعبان")), "Short month name - MMM");
                Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 9, 13), "MMM", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString("رمضان")), "Short month name - MMM");
                Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 10, 13), "MMM", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString("شوال")), "Short month name - MMM");
                Assert.That(NormalizeString(WordUtil.FormatDateTime(new DateTime(2007, 11, 11), "MMM", 0, CalendarType.Hijri)), Is.EqualTo(NormalizeString("ذو القعدة")), "Short month name - MMM");

                // Year - y, yy
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1), "y", 0, CalendarType.Hijri), Is.EqualTo("27"), "Year - y");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 20), "y", 0, CalendarType.Hijri), Is.EqualTo("28"), "Year - y");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2008, 1, 10), "y", 0, CalendarType.Hijri), Is.EqualTo("29"), "Year - y");

                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1), "yy", 0, CalendarType.Hijri), Is.EqualTo("27"), "Year - yy");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 20), "yy", 0, CalendarType.Hijri), Is.EqualTo("28"), "Year - yy");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2008, 1, 10), "yy", 0, CalendarType.Hijri), Is.EqualTo("29"), "Year - yy");

                // Year - yyy,yyyy
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1), "yyy", 0, CalendarType.Hijri), Is.EqualTo("1427"), "Year - yyy");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 20), "yyy", 0, CalendarType.Hijri), Is.EqualTo("1428"), "Year - yyy");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2008, 1, 10), "yyy", 0, CalendarType.Hijri), Is.EqualTo("1429"), "Year - yyy");

                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1), "yyyy", 0, CalendarType.Hijri), Is.EqualTo("1427"), "Year - yyyy");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 20), "yyyy", 0, CalendarType.Hijri), Is.EqualTo("1428"), "Year - yyyy");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2008, 1, 10), "yyyy", 0, CalendarType.Hijri), Is.EqualTo("1429"), "Year - yyyy");

                // Hours - hh,HH
                for (int i = 1; i < 24; i++)
                {
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1).AddHours(i), "hh", 0, CalendarType.Hijri), Is.EqualTo(AlignNumber(i > 12 ? i - 12 : i, 2)), "Hours - hh");
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1).AddHours(i), "HH", 0, CalendarType.Hijri), Is.EqualTo(AlignNumber(i, 2)), "Hours - HH");
                }

                // Minutes, seconds
                for (int i = 0; i < 60; i++)
                {
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1).AddMinutes(i), "m", 0, CalendarType.Hijri), Is.EqualTo(AlignNumber(i, 1)), "Minutes - m");
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1).AddMinutes(i), "mm", 0, CalendarType.Hijri), Is.EqualTo(AlignNumber(i, 2)), "Minutes - mm");

                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1).AddSeconds(i), "s", 0, CalendarType.Hijri), Is.EqualTo(AlignNumber(i, 1)), "Seconds - s");
                    Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1).AddSeconds(i), "ss", 0, CalendarType.Hijri), Is.EqualTo(AlignNumber(i, 2)), "Seconds - ss");
                }

                // AP/PM
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1, 10, 0, 0), "tt", 0, CalendarType.Hijri), Is.EqualTo("ص"), "AM/PM - tt");
                Assert.That(WordUtil.FormatDateTime(new DateTime(2007, 1, 1, 17, 0, 0), "tt", 0, CalendarType.Hijri), Is.EqualTo("م"), "AM/PM - tt");

            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// Autoportable tests of Hebrew and Arabic DateTime string constants.
        /// Formally this test should be in JavaMs.Tests module but leaved here to remove duplication
        /// and links from JavaMs.Tests to Aspose.Words.
        /// </summary>
        [Test]
        public void TestArabicDateFormattingToString()
        {
            SystemPal.SaveCulture();
            try
            {
                // Reset TestMode.
                // TestMode flag is used only to ensure CultureInfo ignores user overrides in Windows Control Panel.
                // But it works different for Java, .NET and .NET Standard platforms (One day shift in Arabic DateTime formatting)
                // That is why we decided to reset TestMode for this test, to get equals results on all platforms.
                SystemPal.SetTestMode(false);

                SystemPal.SetCulture("ar-SA");

                // ShortDatePattern - d
                Assert.That(new DateTime(2007, 1, 1).ToString("d"), Is.EqualTo("11/12/27"), "ShortDatePattern - d");
                Assert.That(new DateTime(2007, 1, 20).ToString("d"), Is.EqualTo("01/01/28"), "ShortDatePattern - d");
                Assert.That(new DateTime(2007, 2, 18).ToString("d"), Is.EqualTo("30/01/28"), "ShortDatePattern - d");

                // Month's day - dd
                Assert.That(new DateTime(2007, 1, 1).ToString("dd"), Is.EqualTo("11"), "Month's day - dd");
                Assert.That(new DateTime(2007, 1, 20).ToString("dd"), Is.EqualTo("01"), "Month's day - dd");
                Assert.That(new DateTime(2007, 2, 18).ToString("dd"), Is.EqualTo("30"), "Month's day - dd");

                // Short name of week's day - ddd
                string[] weekDays = { "الاثنين", "الثلاثاء", "الاربعاء", "الخميس", "الجمعة", "السبت", "الاحد" };
                for (int i = 1; i <= weekDays.Length; i++)
                {
                    Assert.That(NormalizeString(new DateTime(2007, 1, i).ToString("ddd")), Is.EqualTo(NormalizeString(weekDays[i - 1])), "Short name of week's day - ddd");
                    Assert.That(NormalizeString(new DateTime(2007, 1, i).ToString("dddd")), Is.EqualTo(NormalizeString(weekDays[i - 1])), "Short name of week's day - dddd");
                }

                // LongDatePattern - D
                Assert.That(NormalizeString(new DateTime(2007, 1, 1).ToString("D")), Is.EqualTo(NormalizeString("11/ذو الحجة/1427")), "LongDatePattern - D");
                Assert.That(NormalizeString(new DateTime(2007, 1, 20).ToString("D")), Is.EqualTo(NormalizeString("01/محرم/1428")), "LongDatePattern - D");
                Assert.That(NormalizeString(new DateTime(2007, 2, 19).ToString("D")), Is.EqualTo(NormalizeString("01/صفر/1428")), "LongDatePattern - D");

                // The period or era - gg
                Assert.That(NormalizeString(new DateTime(2007, 1, 1).ToString("gg")), Is.EqualTo(NormalizeString("بعد الهجرة")), "The period or era - gg");

                // Months and date - M
                Assert.That(NormalizeString(new DateTime(2007, 1, 1).ToString("M")), Is.EqualTo(NormalizeString("11 ذو الحجة")), "Months and date - M");
                Assert.That(NormalizeString(new DateTime(2007, 1, 20).ToString("M")), Is.EqualTo(NormalizeString("01 محرم")), "Months and date - M");
                Assert.That(NormalizeString(new DateTime(2007, 2, 19).ToString("M")), Is.EqualTo(NormalizeString("01 صفر")), "Months and date - M");

                // Number of month - MM
                Assert.That(new DateTime(2007, 1, 1).ToString("MM"), Is.EqualTo("12"), "Months and date - MM");
                Assert.That(new DateTime(2007, 1, 20).ToString("MM"), Is.EqualTo("01"), "Months and date - MM");
                Assert.That(new DateTime(2007, 2, 18).ToString("MM"), Is.EqualTo("01"), "Months and date - MM");

                // Short month name - MMM
                Assert.That(NormalizeString(new DateTime(2007, 1, 1).ToString("MMM")), Is.EqualTo(NormalizeString("ذو الحجة")), "Short month name - MMM");
                Assert.That(NormalizeString(new DateTime(2007, 1, 20).ToString("MMM")), Is.EqualTo(NormalizeString("محرم")), "Short month name - MMM");
                Assert.That(NormalizeString(new DateTime(2007, 2, 19).ToString("MMM")), Is.EqualTo(NormalizeString("صفر")), "Short month name - MMM");
                Assert.That(NormalizeString(new DateTime(2007, 3, 20).ToString("MMM")), Is.EqualTo(NormalizeString("ربيع الاول")), "Short month name - MMM");
                Assert.That(NormalizeString(new DateTime(2007, 4, 18).ToString("MMM")), Is.EqualTo(NormalizeString("ربيع الثاني")), "Short month name - MMM");
                Assert.That(NormalizeString(new DateTime(2007, 5, 18).ToString("MMM")), Is.EqualTo(NormalizeString("جمادى الاولى")), "Short month name - MMM");
                Assert.That(NormalizeString(new DateTime(2007, 6, 16).ToString("MMM")), Is.EqualTo(NormalizeString("جمادى الثانية")), "Short month name - MMM");
                Assert.That(NormalizeString(new DateTime(2007, 7, 15).ToString("MMM")), Is.EqualTo(NormalizeString("رجب")), "Short month name - MMM");
                Assert.That(NormalizeString(new DateTime(2007, 8, 14).ToString("MMM")), Is.EqualTo(NormalizeString("شعبان")), "Short month name - MMM");
                Assert.That(NormalizeString(new DateTime(2007, 9, 13).ToString("MMM")), Is.EqualTo(NormalizeString("رمضان")), "Short month name - MMM");
                Assert.That(NormalizeString(new DateTime(2007, 10, 13).ToString("MMM")), Is.EqualTo(NormalizeString("شوال")), "Short month name - MMM");
                Assert.That(NormalizeString(new DateTime(2007, 11, 11).ToString("MMM")), Is.EqualTo(NormalizeString("ذو القعدة")), "Short month name - MMM");

                // Long month name - MMMM
                Assert.That(NormalizeString(new DateTime(2007, 1, 1).ToString("MMMM")), Is.EqualTo(NormalizeString("ذو الحجة")), "Long month name - MMMM");
                Assert.That(NormalizeString(new DateTime(2007, 1, 20).ToString("MMMM")), Is.EqualTo(NormalizeString("محرم")), "Long month name - MMMM");
                Assert.That(NormalizeString(new DateTime(2007, 2, 19).ToString("MMMM")), Is.EqualTo(NormalizeString("صفر")), "Long month name - MMMM");
                Assert.That(NormalizeString(new DateTime(2007, 3, 20).ToString("MMMM")), Is.EqualTo(NormalizeString("ربيع الأول")), "Long month name - MMMM");
                Assert.That(NormalizeString(new DateTime(2007, 4, 18).ToString("MMMM")), Is.EqualTo(NormalizeString("ربيع الثاني")), "Long month name - MMMMM");
                Assert.That(NormalizeString(new DateTime(2007, 5, 18).ToString("MMMM")), Is.EqualTo(NormalizeString("جمادى الأولى")), "Long month name - MMMM");
                Assert.That(NormalizeString(new DateTime(2007, 6, 16).ToString("MMMM")), Is.EqualTo(NormalizeString("جمادى الثانية")), "Long month name - MMMM");
                Assert.That(NormalizeString(new DateTime(2007, 7, 15).ToString("MMMM")), Is.EqualTo(NormalizeString("رجب")), "Long month name - MMMM");
                Assert.That(NormalizeString(new DateTime(2007, 8, 14).ToString("MMMM")), Is.EqualTo(NormalizeString("شعبان")), "Long month name - MMMM");
                Assert.That(NormalizeString(new DateTime(2007, 9, 13).ToString("MMMM")), Is.EqualTo(NormalizeString("رمضان")), "Long month name - MMMM");
                Assert.That(NormalizeString(new DateTime(2007, 10, 13).ToString("MMMM")), Is.EqualTo(NormalizeString("شوال")), "Long month name - MMMM");
                Assert.That(NormalizeString(new DateTime(2007, 11, 11).ToString("MMMM")), Is.EqualTo(NormalizeString("ذو القعدة")), "Long month name - MMMM");

                // Month and year - y
                Assert.That(NormalizeString(new DateTime(2007, 1, 1).ToString("y")), Is.EqualTo(NormalizeString("ذو الحجة, 1427")), "Month and year - y");
                Assert.That(NormalizeString(new DateTime(2007, 1, 20).ToString("y")), Is.EqualTo(NormalizeString("محرم, 1428")), "Month and year - y");
                Assert.That(NormalizeString(new DateTime(2008, 1, 10).ToString("y")), Is.EqualTo(NormalizeString("محرم, 1429")), "Month and year - y");

                // Year - yy
                Assert.That(new DateTime(2007, 1, 1).ToString("yy"), Is.EqualTo("27"), "Year - yy");
                Assert.That(new DateTime(2007, 1, 20).ToString("yy"), Is.EqualTo("28"), "Year - yy");
                Assert.That(new DateTime(2008, 1, 10).ToString("yy"), Is.EqualTo("29"), "Year - yy");

                // Year - yyyy
                Assert.That(new DateTime(2007, 1, 1).ToString("yyyy"), Is.EqualTo("1427"), "Year - yyyy");
                Assert.That(new DateTime(2007, 1, 20).ToString("yyyy"), Is.EqualTo("1428"), "Year - yyyy");
                Assert.That(new DateTime(2008, 1, 10).ToString("yyyy"), Is.EqualTo("1429"), "Year - yyyy");

                // Hours - hh,HH
                for (int i = 1; i < 24; i++)
                {
                    Assert.That(new DateTime(2007, 1, 1).AddHours(i).ToString("hh"), Is.EqualTo(AlignNumber(i > 12 ? i - 12 : i, 2)), "Hours - hh");
                    Assert.That(new DateTime(2007, 1, 1).AddHours(i).ToString("HH"), Is.EqualTo(AlignNumber(i, 2)), "Hours - HH");
                }

                // Minutes, seconds
                for (int i = 0; i < 60; i++)
                {
                    Assert.That(new DateTime(2007, 1, 1).AddMinutes(i).ToString("mm"), Is.EqualTo(AlignNumber(i, 2)), "Minutes - mm");
                    Assert.That(new DateTime(2007, 1, 1).AddSeconds(i).ToString("ss"), Is.EqualTo(AlignNumber(i, 2)), "Seconds - ss");
                }

                // AM/PM
                Assert.That(new DateTime(2007, 1, 1, 10, 0, 0).ToString("tt"), Is.EqualTo("ص"), "AM/PM - tt");
                Assert.That(new DateTime(2007, 1, 1, 17, 0, 0).ToString("tt"), Is.EqualTo("م"), "AM/PM - tt");
            }
            finally
            {
                SystemPal.RestoreCulture();
                SystemPal.SetTestMode(true);
            }
        }

        private static string AlignNumber(int value, int amount)
        {
            return value.ToString().PadLeft(amount, '0');
        }

        private static string NormalizeString(string source)
        {
            return source.Trim()
                         .Replace('\u0623', '\u0627')
                         .Replace('\u0625', '\u0627');
        }

        // Hebrew Month's day names - d,dd
        private static readonly string[] gHebrewMonthDays =
        {
            "י\"א", "י\"ב", "י\"ג", "י\"ד", "ט\"ו", "ט\"ז", "י\"ז", "י\"ח", "י\"ט", "כ'",
            "כ\"א", "כ\"ב", "כ\"ג", "כ\"ד", "כ\"ה", "כ\"ו", "כ\"ז", "כ\"ח", "כ\"ט", "א'",
            "ב'", "ג'", "ד'", "ה'", "ו'", "ז'", "ח'", "ט'", "י'", "י\"א", "י\"ב"
        };

        // Hebrew full week day name - dddd
        private static readonly string[] gHebrewWeekDays = { "יום שני", "יום שלישי", "יום רביעי", "יום חמישי", "יום שישי", "שבת", "יום ראשון" };
    }
}

#endif
