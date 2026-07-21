// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/08/2012 by Vyacheslav Durin
// 2016/02/19 by Anatoliy Sidorenko

/**
 * http://www.dotnetperls.com/datetime-format
 * http://msdn.microsoft.com/en-us/library/az4se3k1.aspx
 * http://www.geekzilla.co.uk/View00FF7904-B510-468C-A2C8-F859AA20581F.htm
 * http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx
 */

using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestDateTimeParse
    {
        [Test]
        public void TestDateTimeString()
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                int nowadaysYear = DateTime.Now.Year;

                CheckTime("11:06", 11, 6, 0);
                CheckTime("11:07:03", 11, 7, 3);
                CheckDate("01/05/2012", 2012, 1, 5);
                CheckDate("2006-01-05", 2006, 1, 5);
                CheckTime("7:09:01 pm", 19, 9, 1);
                CheckTime("9:01 am", 9, 1, 0);
                CheckDateTime("7:09:02 pm 5/01/2006 ", 2006, 5, 1, 19, 9, 2);
                CheckDate("5 January 2006", 2006, 1, 5);
                CheckDate("Thursday, 5 January 2006", 2006, 1, 5);
                CheckDate("5/01/06", 2006, 5, 1);
                CheckDate("5.01.2006", 2006, 5, 1);
                CheckDate("Jan-06", nowadaysYear, 1, 6);
                CheckDate("5-Jan-06", 2006, 1, 5);
                CheckDate("5 Jan 06", 2006, 1, 5);
                CheckDateTime("2012-09-01 10:00:00", 2012, 9, 1, 10, 0, 0);
                CheckDate("2012/09/01", 2012, 9, 1);
                CheckDate("01 Sep 2012", 2012, 9, 1);
                CheckDate("5 Sep. 06", 2006, 9, 5);
                CheckDate("03 Feb. 2012", 2012, 2, 3);
                CheckDateTime("01 Sep 2012 10:00:00", 2012, 9, 1, 10, 0, 0);
                CheckDate("January 06", nowadaysYear, 1, 6);
            }
            finally
            {
                TestDateTime.RestoreLocale();
            }
        }

        [Test]
        public void TestDateTimeStringEnNZ()
        {
            try
            {
                TestDateTime.SetInvariantLocale();
                CheckDate("01/09/2002", 2002, 9, 1);
            }
            finally
            {
                TestDateTime.RestoreLocale();
            }
        }

        [Test]
        public void TestDateTimeStringEnUS()
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
                CheckDate("01/09/2002", 2002, 1, 9);
            }
            finally
            {
                TestDateTime.RestoreLocale();
            }
        }

        // Neither .Net or Java can parse it,
        [Test]
        public void testDateTimeStringNotParseable()
        {
            try
            {
                TestDateTime.SetInvariantLocale();
                // 1/01/0001 12:00:00 a.m.
                CheckDateTime("01.09.12 10.00", 1, 1, 1, 0, 0, 0);
                CheckDate("06-Thu-2011", 1, 1, 1);
                CheckDateTime("Jun-30 16:05", 1, 1, 1, 0, 0, 0);
                CheckDateTime("June, 30 16:5", 1, 1, 1, 0, 0, 0);
            }
            finally
            {
                TestDateTime.RestoreLocale();
            }
        }

        [Test]
        public void TestDateTimeStringTimeZones()
        {
            try
            {
                TestDateTime.SetInvariantLocaleAndTZ();
#if JAVA
                // .Net has DateTimeStyle.NONE by default, timezone ignored.
                CheckDateTime("Tue, 22 Aug 2006 06:30:07 GMT", 2006, 8, 22, 6, 30, 7);
                CheckDateTime("2008-12-23T10:01:00Z", 2008, 12, 23, 10, 1, 0);
#endif
            }
            finally
            {
                TestDateTime.RestoreLocaleAndTZ();
            }
        }

        [Test]
        public void TestDateTimeStringLocales()
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU", false);
                int nowadaysYear = DateTime.Now.Year;

                CheckDate("26 августа", nowadaysYear, 8, 26);
                CheckDateTime("01 Сен 2012 10:00:00", 2012, 9, 1, 10, 0, 0);
                CheckDateTime("5.01.2006 19:09:02", 2006, 1, 5, 19, 9, 2);
            }
            finally
            {
                TestDateTime.RestoreLocale();
            }
        }

        [Test]
        public void TestDateTimeNZLocale()
        {
            try
            {
                TestDateTime.SetInvariantLocale();
                CheckDateTime("5/01/2006 7:09:02 PM", 2006, 1, 5, 19, 9, 2);
            }
            finally
            {
                TestDateTime.RestoreLocale();
            }
        }

        [Test]
        public void TestParseExact()
        {
            try
            {
                TestDateTime.SetInvariantLocale();
                DateTimeFormatInfo dfti = CultureInfo.CurrentCulture.DateTimeFormat;
                DateTime dt = DateTime.ParseExact("09:01", "H:mm", dfti);
                DateTime dt2 = DateTime.ParseExact("17:01:05", "H:mm:ss", dfti);
                DateTime dt3 = DateTime.ParseExact("5/01/2006", "d/MM/yyyy", dfti);
                DateTime dt4 = DateTime.ParseExact("Wed, 01 Dec 2010 09:30:11 GMT", "ddd, d MMM yyyy HH':'mm':'ss GMT", dfti);
                DateTime dt5 = DateTime.ParseExact("01.09.12 10.00", "dd.MM.yy H.mm", dfti);
                DateTime dt6 = DateTime.ParseExact("2010-08-31T00:32:48Z",
                    new string[] { "yyyy-MM-ddTHH:mm:ss,FF", "yyyy-MM-ddTHH:mm:ssZ", "yyyy-MM-ddTHH:mm:sszzz" },
                    new CultureInfo("en-NZ", false).DateTimeFormat, DateTimeStyles.AdjustToUniversal);

                CheckTime(dt, 9, 1, 0);
                CheckTime(dt2, 17, 1, 5);
                CheckDate(dt3, 2006, 1, 5);
#if JAVA
                CheckDateTime(dt4, 2010, 12, 1, 9, 30, 11);
                CheckDateTime(dt5, 2012, 9, 1, 10, 0, 0);
                CheckDateTime(dt6, 2010, 8, 31, 0, 32, 48);
#endif
            }
            finally
            {
                TestDateTime.RestoreLocale();
            }
        }

        /// <summary>
        /// This is simplified cause of TestDocumentInformation.TestJira4290() fail.
        /// </summary>
        [Test]
        public void TestTryParseFormatInfo()
        {
            //the test sets such DateTimeFormatInfo (to emulate real word?)
            DateTimeFormatInfo formatInfo = new DateTimeFormatInfo();
            formatInfo.ShortDatePattern = "d/MM/yyyy'y.'";
            formatInfo.LongTimePattern = "'h: 'HH' m: 'mm";
            formatInfo.TimeSeparator = " m: ";

            //then it gets date from document and formats it to the string (using DateTimeFormatInfo)
            DateTime dateTime = new DateTime(2012, 6, 8, 16, 51, 0);
            string str = "8/06/2012y. h: 16 m: 51";
            DateTime parsed;
            DateTime.TryParse(str, formatInfo, DateTimeStyles.AdjustToUniversal, out parsed);
            Assert.That(parsed, Is.EqualTo(dateTime));

            string strWithNormalTimeSep = "8/06/2012y. 16:51";
            DateTime parsedWithNormalTimeSep;
            DateTime.TryParse(strWithNormalTimeSep, formatInfo,
                DateTimeStyles.AdjustToUniversal, out parsedWithNormalTimeSep);
            Assert.That(parsedWithNormalTimeSep, Is.EqualTo(dateTime));
        }

        [Test]
        public void TestTryParseHebrew()
        {
            DateTimeFormatInfo hebrewDTFI = new CultureInfo("he-IL", false).DateTimeFormat;
            DateTime dt;
            DateTime.TryParse("4 יולי 1986", hebrewDTFI, DateTimeStyles.None, out dt);
            DateTime dt2;
            DateTime.TryParse("23 פברואר 2012", hebrewDTFI, DateTimeStyles.None, out dt2);
            DateTime dt3;
            DateTime.TryParse("ג אדר ה'תשע\"ב", hebrewDTFI, DateTimeStyles.None, out dt3);

            CheckDate(new DateTime(1986, 7, 4, 0, 0, 0), dt.Year, dt.Month, dt.Day);
            CheckDate(new DateTime(2012, 2, 23, 0, 0, 0), dt2.Year, dt2.Month, dt2.Day);
            CheckDate(DateTime.MinValue, dt3.Year, dt3.Month, dt3.Day);
        }

        [Test]
        public void TestDateTimeFormatInfoForDifferentLocales()
        {
            DateTime dt = new DateTime(1986, 7, 4, 17, 45, 0);
            CultureInfo ru = new CultureInfo("ru-RU", false);
            CultureInfo us = new CultureInfo("en-US", false);
            CultureInfo ko = new CultureInfo("ko-KR", false);
            CultureInfo nz = new CultureInfo("en-NZ", false);
            CultureInfo fr = new CultureInfo("fr-FR", false);

            int style = (int)DateTimeStyles.AdjustToUniversal;
            DateTime parse;
            DateTime.TryParse("4 июля 1986 г. 17:45:00", ru.DateTimeFormat, (DateTimeStyles)style, out parse);
            Assert.That(dt, Is.EqualTo(parse));
            DateTime.TryParse("07/04/1986. 5:45:00 pm", us.DateTimeFormat, (DateTimeStyles)style, out parse);
            Assert.That(dt, Is.EqualTo(parse));
            DateTime.TryParse("04/07/1986. 5:45:00 PM", nz.DateTimeFormat, (DateTimeStyles)style, out parse);
            Assert.That(dt, Is.EqualTo(parse));
            DateTime.TryParse("1986년 07월 04일 오후 5:45:00", ko.DateTimeFormat, (DateTimeStyles)style, out parse);
            Assert.That(dt, Is.EqualTo(parse));
            DateTime.TryParse("4 juillet 1986 17:45:00", fr.DateTimeFormat, (DateTimeStyles)style, out parse);
            Assert.That(dt, Is.EqualTo(parse));
        }

        [Test]
        public void TestUkUaLocaleJ645()
        {
            DateTimeFormatInfo dtfi = new CultureInfo("uk-UA", false).DateTimeFormat;
            DateTime d = new DateTime(1986, 7, 4, 17, 45, 0);

            string dateStr = d.ToString("dddd d MMMM yyyy H:mm", dtfi);
            Assert.That(dateStr, Is.EqualTo("п'ятниця 4 липня 1986 17:45"));

            DateTime dt;
            DateTime.TryParse("п'ятниця 4 липня 1986 17:45", dtfi, DateTimeStyles.None, out dt);
            Assert.That(d, Is.EqualTo(dt));
        }

        private void CheckDate(DateTime actual, int year, int month, int day)
        {
            CheckDateTime(actual, year, month, day, 0, 0, 0);
        }

        private void CheckTime(DateTime actual, int hour, int min, int sec)
        {
            DateTime now = DateTime.Now;
            CheckDateTime(actual, now.Year, now.Month, now.Day, hour, min, sec);
        }

        private void CheckDateTime(DateTime actual, int year, int month, int day, int hour, int min, int sec)
        {
            DateTime expected = new DateTime(year, month, day, hour, min, sec);
            Assert.That(expected, Is.EqualTo(actual));
        }

        private void CheckDate(string dateStr, int year, int month, int day)
        {
            CheckDateTime(dateStr, year, month, day, 0, 0, 0);
        }

        private void CheckTime(string timeStr, int hour, int min, int sec)
        {
            DateTime now = DateTime.Now;
            CheckDateTime(timeStr, now.Year, now.Month, now.Day, hour, min, sec);
        }

        private void CheckDateTime(string dateTimeStr, int year, int month, int day, int hour, int min, int sec)
        {
            DateTime parsed;
#if JAVA
            parsed = DateTime.tryParse(dateTimeStr);
#else
            DateTime.TryParse(dateTimeStr, out parsed);
#endif

            DateTime expected = new DateTime(year, month, day, hour, min, sec);
            Assert.That(expected, Is.EqualTo(parsed));
        }
    }
}
