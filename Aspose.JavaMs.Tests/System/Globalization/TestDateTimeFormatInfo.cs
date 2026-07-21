// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/11/2016 by Konstantin Sidorenko

using System;
using System.Globalization;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Globalization
{
    [TestFixture]
    public class TestDateTimeFormatInfo
    {
        [Test]
        public void TestConstants()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.InvariantInfo;

            Assert.That(info.AMDesignator, Is.EqualTo("AM"));
            Assert.That(info.PMDesignator, Is.EqualTo("PM"));
            Assert.That(info.DateSeparator, Is.EqualTo("/"));
            Assert.That(info.FullDateTimePattern, Is.EqualTo("dddd, dd MMMM yyyy HH:mm:ss"));
            Assert.That(info.LongDatePattern, Is.EqualTo("dddd, dd MMMM yyyy"));
            Assert.That(info.MonthDayPattern, Is.EqualTo("MMMM dd"));
            Assert.That(info.LongTimePattern, Is.EqualTo("HH:mm:ss"));
            Assert.That(info.RFC1123Pattern, Is.EqualTo("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'"));
            Assert.That(info.ShortDatePattern, Is.EqualTo("MM/dd/yyyy"));
            Assert.That(info.ShortTimePattern, Is.EqualTo("HH:mm"));
            Assert.That(info.SortableDateTimePattern, Is.EqualTo("yyyy'-'MM'-'dd'T'HH':'mm':'ss"));
            Assert.That(info.UniversalSortableDateTimePattern, Is.EqualTo("yyyy'-'MM'-'dd HH':'mm':'ss'Z'"));
            Assert.That(info.TimeSeparator, Is.EqualTo(":"));
            Assert.That(info.YearMonthPattern, Is.EqualTo("yyyy MMMM"));

            // Not implemented in Java:
            //Assert.AreEqual("Gregorian Calendar", info.NativeCalendarName);
        }

        [Test]
        public void TestArrayConstants()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.InvariantInfo;

            string[] abbreviatedDayNames = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            Assert.That(info.AbbreviatedDayNames, Is.EqualTo(abbreviatedDayNames));

            string[] abbreviatedMonthNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "" };
            Assert.That(info.AbbreviatedMonthNames, Is.EqualTo(abbreviatedMonthNames));

            string[] dayNames = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
            Assert.That(info.DayNames, Is.EqualTo(dayNames));

            string[] monthNames =
            {
                "January", "February", "March", "April", "May", "June", "July", "August", "September",
                "October", "November", "December", ""
            };
            Assert.That(info.MonthNames, Is.EqualTo(monthNames));

            string[] shortestDayNames =
#if JAVA
                {"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"};
#else
                {"Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"};
#endif
            Assert.That(info.ShortestDayNames, Is.EqualTo(shortestDayNames));

            // Java-deleed code:
            // Genitive month Names not implemented in ms DateTimeFormatInfo: Java's DateFormatSymbols doesn't contain 
            // genitive month names because java adds genitive suffixes during formatting.
#if !JAVA
            string[] monthGenitiveNames =
            {
                "January", "February", "March", "April", "May", "June", "July", "August",
                "September", "October", "November", "December", ""
            };
            Assert.That(info.MonthGenitiveNames, Is.EqualTo(monthGenitiveNames));

            string[] abbreviatedMonthGenitiveNames = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "" };
            Assert.That(info.AbbreviatedMonthGenitiveNames, Is.EqualTo(abbreviatedMonthGenitiveNames));
#endif
        }

        [Test]
        public void TestHebrewConstants()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.GetInstance(new CultureInfo("he-IL"));

            Assert.That(info.AMDesignator, Is.EqualTo("AM"));
            Assert.That(info.PMDesignator, Is.EqualTo("PM"));
            Assert.That(info.DateSeparator, Is.EqualTo("/"));
            Assert.That(info.FullDateTimePattern, Is.EqualTo("dddd dd MMMM yyyy HH:mm:ss"));
            Assert.That(info.LongDatePattern, Is.EqualTo("dddd dd MMMM yyyy"));
            Assert.That(info.MonthDayPattern, Is.EqualTo("dd MMMM"));
            Assert.That(info.LongTimePattern, Is.EqualTo("HH:mm:ss"));
            Assert.That(info.RFC1123Pattern, Is.EqualTo("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'"));
            Assert.That(info.ShortDatePattern, Is.EqualTo("dd/MM/yyyy"));
            Assert.That(info.ShortTimePattern, Is.EqualTo("HH:mm"));
            Assert.That(info.SortableDateTimePattern, Is.EqualTo("yyyy'-'MM'-'dd'T'HH':'mm':'ss"));
            Assert.That(info.UniversalSortableDateTimePattern, Is.EqualTo("yyyy'-'MM'-'dd HH':'mm':'ss'Z'"));
            Assert.That(info.TimeSeparator, Is.EqualTo(":"));
            Assert.That(info.YearMonthPattern, Is.EqualTo("MMMM yyyy"));
        }

        [Test]
        public void TestHebrewArrayConstants()
        {
            DateTimeFormatInfo info = DateTimeFormatInfo.GetInstance(new CultureInfo("he-IL"));

            string[] abbreviatedDayNames = { "יום א", "יום ב", "יום ג", "יום ד", "יום ה", "יום ו", "שבת" };
            Assert.That(info.AbbreviatedDayNames, Is.EqualTo(abbreviatedDayNames));

            string[] abbreviatedMonthNames = { "ינו", "פבר", "מרץ", "אפר", "מאי", "יונ", "יול", "אוג", "ספט", "אוק", "נוב", "דצמ", "" };
            Assert.That(info.AbbreviatedMonthNames, Is.EqualTo(abbreviatedMonthNames));

            string[] dayNames = { "יום ראשון", "יום שני", "יום שלישי", "יום רביעי", "יום חמישי", "יום שישי", "שבת" };
            Assert.That(info.DayNames, Is.EqualTo(dayNames));

            string[] monthNames =
            {
                "ינואר", "פברואר", "מרץ", "אפריל", "מאי", "יוני", "יולי", "אוגוסט",
                "ספטמבר", "אוקטובר", "נובמבר", "דצמבר", ""
            };
            Assert.That(info.MonthNames, Is.EqualTo(monthNames));

            string[] shortestDayNames = { "א", "ב", "ג", "ד", "ה", "ו", "ש" };
            Assert.That(info.ShortestDayNames, Is.EqualTo(shortestDayNames));

            // Java-deleed code: Genitive month Names not applicable to java, see comments above (TestArrayConstants()).
#if !JAVA
            string[] monthGenitiveNames =
            {
                "ינואר", "פברואר", "מרץ", "אפריל", "מאי", "יוני", "יולי", "אוגוסט",
                "ספטמבר", "אוקטובר", "נובמבר", "דצמבר", ""
            };
            Assert.That(info.MonthGenitiveNames, Is.EqualTo(monthGenitiveNames));

            string[] abbreviatedMonthGenitiveNames = { "ינו", "פבר", "מרץ", "אפר", "מאי", "יונ", "יול", "אוג", "ספט", "אוק", "נוב", "דצמ", "" };
            Assert.That(info.AbbreviatedMonthGenitiveNames, Is.EqualTo(abbreviatedMonthGenitiveNames));
#endif
        }

        /// <summary>
        /// WORDSJAVA-1680 Java can't properly parse "dd/MM/YYYY"-formatted Date when set to Canada culture
        /// - it reads "MM/dd/YYYY" instead (in US-style).
        /// </summary>
        [Test]
        public void TestJiraJ1680()
        {
            string value = "09/06/2015 15:39";
            DateTime result;

            // US
            CultureInfo culture = new CultureInfo("en-US");
            DateTime.TryParse(value, culture, DateTimeStyles.AdjustToUniversal, out result);

            Assert.That(6, Is.EqualTo(result.Day));
            Assert.That(9, Is.EqualTo(result.Month));

            // Canada
            culture = new CultureInfo("en-CA");
            DateTime.TryParse(value, culture, DateTimeStyles.AdjustToUniversal, out result);

            Assert.That(6, Is.EqualTo(result.Day));
            Assert.That(9, Is.EqualTo(result.Month));
        }
    }
}
