// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/08/2012 by Vyacheslav Durin
// 2016/02/18 by Anatoliy Sidorenko

using System;
using System.Globalization;
using Aspose.Common;
using NUnit.Framework;


namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestDateTimeFormatter
    {
        [Test]
        public void TestDateTimeToStringWithUnsupportedSymbols()
        {
            DateTimeFormatInfo locale = new CultureInfo("en-US", false).DateTimeFormat;
            DateTime d = new DateTime(2006, 8, 22, 6, 30, 7);
            Assert.That(d.ToString("yyyy-MM-ddTHH:mm:ss.fffffff", locale), Is.EqualTo("2006-08-22T06:30:07.0000000"));
        }

        [Test]
        public void TestDateTimeToString()
        {
            DateTimeFormatInfo locale = new CultureInfo("en-US", false).DateTimeFormat;
            DateTime d = new DateTime(2006, 8, 22, 6, 30, 7);

            Assert.That(d.ToString("MM/dd/yyyy", locale), Is.EqualTo("08/22/2006"));
            Assert.That(d.ToString("dddd, dd MMMM yyyy", locale), Is.EqualTo("Tuesday, 22 August 2006"));
            Assert.That(d.ToString("dddd, dd MMMM yyyy HH:mm", locale), Is.EqualTo("Tuesday, 22 August 2006 06:30"));
            Assert.That(d.ToString("dddd, dd MMMM yyyy hh:mm tt", locale), Is.EqualTo("Tuesday, 22 August 2006 06:30 AM"));
            Assert.That(d.ToString("dddd, dd MMMM yyyy H:mm", locale), Is.EqualTo("Tuesday, 22 August 2006 6:30"));
            Assert.That(d.ToString("dddd, dd MMMM yyyy h:mm tt", locale), Is.EqualTo("Tuesday, 22 August 2006 6:30 AM"));
            Assert.That(d.ToString("dddd, dd MMMM yyyy HH:mm:ss", locale), Is.EqualTo("Tuesday, 22 August 2006 06:30:07"));
            Assert.That(d.ToString("MM/dd/yyyy HH:mm", locale), Is.EqualTo("08/22/2006 06:30"));
            Assert.That(d.ToString("MM/dd/yyyy hh:mm tt", locale), Is.EqualTo("08/22/2006 06:30 AM"));
            Assert.That(d.ToString("MM/dd/yyyy H:mm", locale), Is.EqualTo("08/22/2006 6:30"));
            Assert.That(d.ToString("MM/dd/yyyy h:mm tt", locale), Is.EqualTo("08/22/2006 6:30 AM"));
            Assert.That(d.ToString("MM/dd/yyyy HH:mm:ss", locale), Is.EqualTo("08/22/2006 06:30:07"));
            Assert.That(d.ToString("MMMM dd", locale), Is.EqualTo("August 22"));
            Assert.That(d.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", locale), Is.EqualTo("Tue, 22 Aug 2006 06:30:07 GMT"));
            Assert.That(d.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", locale), Is.EqualTo("Tue, 22 Aug 2006 06:30:07 GMT"));
            Assert.That(d.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss", locale), Is.EqualTo("2006-08-22T06:30:07"));
            Assert.That(d.ToString("yyyy-MM-ddTHH:mm:ss", locale), Is.EqualTo("2006-08-22T06:30:07"));
            Assert.That(d.ToString("HH:mm", locale), Is.EqualTo("06:30"));
            Assert.That(d.ToString("hh:mm tt", locale), Is.EqualTo("06:30 AM"));
            Assert.That(d.ToString("H:mm", locale), Is.EqualTo("6:30"));
            Assert.That(d.ToString("h:mm tt", locale), Is.EqualTo("6:30 AM"));
            Assert.That(d.ToString("HH:mm:ss", locale), Is.EqualTo("06:30:07"));
            Assert.That(d.ToString("yyyy'-'MM'-'dd HH':'mm':'ss'Z'", locale), Is.EqualTo("2006-08-22 06:30:07Z"));
            Assert.That(d.ToString("yyyy-MM-dd HH:mm:ssZ", locale), Is.EqualTo("2006-08-22 06:30:07Z"));
            Assert.That(d.ToString("dddd, dd MMMM yyyy HH:mm:ss", locale), Is.EqualTo("Tuesday, 22 August 2006 06:30:07"));
            Assert.That(d.ToString("yyyy MMMM", locale), Is.EqualTo("2006 August"));
        }

        [Test]
        public void TestDateTimeToXmlUtc()
        {
            //yyyy-MM-dd'T'HH:mm:ss'Z'
            DateTime d = new DateTime(2010, 12, 1, 9, 30, 11);
            Assert.That(d.ToString("yyyy-MM-ddTHH:mm:ssZ"), Is.EqualTo("2010-12-01T09:30:11Z"));
        }

        [Test]
        public void TestDateTimeToXmlNoTimezone()
        {
            //yyyy-MM-dd'T'HH:mm:ss
            DateTime d = new DateTime(2010, 12, 1, 9, 30, 11);
            Assert.That(d.ToString("yyyy-MM-ddTHH:mm:ss"), Is.EqualTo("2010-12-01T09:30:11"));
        }

        [Test]
        public void TestDateTimeToStr_yyyyMMddHHmmssZ()
        {
            //yyyyMMddHHmmss'Z'
            DateTime d = new DateTime(2012, 9, 1, 10, 0, 0);
            Assert.That(d.ToString("yyyyMMddHHmmssZ"), Is.EqualTo("20120901100000Z"));
        }

        [Test]
        public void TestDateTimeToXmlRus()
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetCulture("ru-RU");

                DateTime d = new DateTime(2010, 12, 1, 9, 30, 11);
                Assert.That(d.ToString("yyyy-MM-ddTHH:mm:ssZ"), Is.EqualTo("2010-12-01T09:30:11Z"));
                Assert.That(d.ToString("yyyy-MM-ddTHH:mm:ss"), Is.EqualTo("2010-12-01T09:30:11"));
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        [Test]
        public void TestDateTimeToXml()
        {
            DateTime d = new DateTime(2010, 12, 1, 9, 30, 11);
            Assert.That(d.ToString("yyyy-MM-ddTHH:mm:ssZ"), Is.EqualTo("2010-12-01T09:30:11Z"));
            Assert.That(d.ToString("yyyy-MM-ddTHH:mm:ss"), Is.EqualTo("2010-12-01T09:30:11"));
        }

        [Test]
        public void TestInvariantNetFormatter()
        {
            try
            {
                TestDateTime.SetInvariantLocale();

                DateTime d = new DateTime(2010, 12, 1, 9, 30, 11);

                string longDatePattern = "dddd, dd MMMM yyyy";
                string longTimePattern = "HH:mm:ss";
                string shortDatePattern = "MM/dd/yyyy";
                string shortTimePattern = "HH:mm";
                string monthDayPattern = "MMMM dd";
                string yearMonthPattern = "yyyy MMMM";
                string fullDateTimePattern = "dddd, dd MMMM yyyy HH:mm:ss";
                string rFC1123Pattern = "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
                string sortableDateTimePattern = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";
                string universalSortableDateTimePattern = "yyyy'-'MM'-'dd HH':'mm':'ss'Z'";

                string longDatePatternResult = d.ToString(longDatePattern);
                string longTimePatternResult = d.ToString(longTimePattern);
                string shortDatePatternResult = d.ToString(shortDatePattern);
                string shortTimePatternResult = d.ToString(shortTimePattern);
                string monthDayPatternResult = d.ToString(monthDayPattern);
                string yearMonthPatternResult = d.ToString(yearMonthPattern);
                string fullDateTimePatternResult = d.ToString(fullDateTimePattern);
                string rFC1123PatternResult = d.ToString(rFC1123Pattern);
                string sortableDateTimePatternResult = d.ToString(sortableDateTimePattern);
                string universalSortableDateTimePatternResult = d.ToString(universalSortableDateTimePattern);

                Assert.That("Wednesday, 01 December 2010", Is.EqualTo(longDatePatternResult));
                Assert.That("09:30:11", Is.EqualTo(longTimePatternResult));
                Assert.That("12/01/2010", Is.EqualTo(shortDatePatternResult));
                Assert.That("09:30", Is.EqualTo(shortTimePatternResult));
                Assert.That("December 01", Is.EqualTo(monthDayPatternResult));
                Assert.That("2010 December", Is.EqualTo(yearMonthPatternResult));
                Assert.That("Wednesday, 01 December 2010 09:30:11", Is.EqualTo(fullDateTimePatternResult));
                Assert.That("Wed, 01 Dec 2010 09:30:11 GMT", Is.EqualTo(rFC1123PatternResult));
                Assert.That("2010-12-01T09:30:11", Is.EqualTo(sortableDateTimePatternResult));
                Assert.That("2010-12-01 09:30:11Z", Is.EqualTo(universalSortableDateTimePatternResult));
            }
            finally
            {
                TestDateTime.RestoreLocale();
            }
        }

        /// <summary>
        /// WORDSNET-13842 Date field not transformed correctly if used Hijri/Lunar Calendar. 
        /// </summary>
        [Test]
        public void TestJira13842OriginalFeedback()
        {
            DateTime d = new DateTime(2016, 12, 7);
            DateTimeFormatInfo englishFormat = new CultureInfo("en-US", false).DateTimeFormat;
            DateTimeFormatInfo hebrewFormat = new CultureInfo("he-IL", false).DateTimeFormat;

            Assert.That(d.ToString("dd MMMM yyyy", englishFormat), Is.EqualTo("07 December 2016"));
            Assert.That(d.ToString("dd MMMM yyyy", hebrewFormat), Is.EqualTo("07 דצמבר 2016"));
            Assert.That(d.ToString("dd MMMM yyyy", englishFormat), Is.EqualTo("07 December 2016"));
        }
    }
}
