// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/10/2007 by Konstantin Sidorenko
// 2016/02/09 by Anatoliy Sidorenko

using System;
using System.Globalization;
using System.Threading;
using Aspose.Common;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    /// <summary>
    /// NOTE about invariant locale (en-NZ).
    /// 
    /// Sometimes generated resources\CultureInfo.xml (used on Java) can differ from the CultureInfo-s used here
    /// (i.e. on .Net AW solution). In this case: this test will pass on .Net but fail on Java.
    /// 
    /// To fix see comments to CsPorer's XmlCultureInfoGenerator.
    /// </summary>
    [TestFixture]
    public class TestDateTime
    {
        [Test]
        public void TestConstants()
        {
            try
            {
                SetInvariantLocale();

                DateTime max = DateTime.MaxValue;
                Assert.That(3155378975999999999L, Is.EqualTo(max.Ticks));
                Assert.That(DateTimeKind.Unspecified, Is.EqualTo(max.Kind));
                Assert.That("31/12/9999 11:59:59 pm", Is.EqualTo(max.ToString()));

                DateTime min = DateTime.MinValue;
                Assert.That(0L, Is.EqualTo(min.Ticks));
                Assert.That(DateTimeKind.Unspecified, Is.EqualTo(min.Kind));
                Assert.That(1, Is.EqualTo(min.Day));
                Assert.That(1, Is.EqualTo(min.Month));
                Assert.That(1, Is.EqualTo(min.Year));
                Assert.That(0, Is.EqualTo(min.Hour));
                Assert.That(0, Is.EqualTo(min.Minute));
                Assert.That(0, Is.EqualTo(min.Second));
                Assert.That(0, Is.EqualTo(min.Millisecond));
                //But ToString works not as in .net for dates before Gregorian Calendar CutOver date
                //(see TestGregorianCutOver()). - fixed (hacked) for DateTime.MinValue only.
                Assert.That("1/01/0001 12:00:00 am", Is.EqualTo(min.ToString()));
                
                //round trip
                DateTime newMax = new DateTime(DateTime.MaxValue.Ticks);
                Assert.That(3155378975999999999L, Is.EqualTo(newMax.Ticks));
                Assert.That(DateTimeKind.Unspecified, Is.EqualTo(newMax.Kind));
                Assert.That("31/12/9999 11:59:59 pm", Is.EqualTo(newMax.ToString()));

                //Note: GregorianCutOver hack works not only for DateTime.MinValue instances,
                //for new DateTime(0L) works now too.
                DateTime newMin = new DateTime(DateTime.MinValue.Ticks);
                Assert.That(0L, Is.EqualTo(newMin.Ticks));
                Assert.That(DateTimeKind.Unspecified, Is.EqualTo(newMin.Kind));
                Assert.That("1/01/0001 12:00:00 am", Is.EqualTo(newMin.ToString()));
            }
            finally
            {
                RestoreLocale();
            }
        }

        [Test]
        public void TestCtors()
        {
            try
            {
                SetInvariantLocale();

                DateTime dateTime = new DateTime(1970, 1, 27);
                Assert.That(DateTimeKind.Unspecified, Is.EqualTo(dateTime.Kind));
                Assert.That("27/01/1970 12:00:00 am", Is.EqualTo(dateTime.ToString()));

                dateTime = new DateTime(2007, 12, 31, 23, 59, 59);
                Assert.That(DateTimeKind.Unspecified, Is.EqualTo(dateTime.Kind));
                Assert.That("31/12/2007 11:59:59 pm", Is.EqualTo(dateTime.ToString()));

                dateTime = new DateTime(2007, 12, 31, 23, 59, 59, 999);
                Assert.That(DateTimeKind.Unspecified, Is.EqualTo(dateTime.Kind));
                Assert.That("31/12/2007 11:59:59 pm", Is.EqualTo(dateTime.ToString()));

                dateTime = new DateTime(2007, 12, 31, 23, 59, 59, DateTimeKind.Local);
                Assert.That(DateTimeKind.Local, Is.EqualTo(dateTime.Kind));
                Assert.That("31/12/2007 11:59:59 pm", Is.EqualTo(dateTime.ToString()));

                dateTime = new DateTime(2007, 12, 31, 23, 59, 59, DateTimeKind.Utc);
                Assert.That(DateTimeKind.Utc, Is.EqualTo(dateTime.Kind));
                Assert.That("31/12/2007 11:59:59 pm", Is.EqualTo(dateTime.ToString()));
            }
            finally
            {
                RestoreLocale();
            }
        }

        /**
         * All dates before the Gregorian Calendar CutOver Date (October 15, 1582 in most vestern countries)
         * interpreted uncorrectly (or rather not as in .Net) by toUnixDate() method
         * because these mehtods use java.lang.Date internally.
         * DateTime.ToString() method does so too because uses toUnixDate() internally.
         */
        [Test]
        public void TestGregorianCutOver()
        {
            try
            {
                SetInvariantLocale();

                Assert.That("31/12/9999 11:59:59 pm", Is.EqualTo(new DateTime(9999, 12, 31, 23, 59, 59).ToString()));
                Assert.That("31/12/5555 11:59:59 pm", Is.EqualTo(new DateTime(5555, 12, 31, 23, 59, 59).ToString()));
                Assert.That("31/12/2099 11:59:59 pm", Is.EqualTo(new DateTime(2099, 12, 31, 23, 59, 59).ToString()));
                Assert.That("31/12/1999 11:59:59 pm", Is.EqualTo(new DateTime(1999, 12, 31, 23, 59, 59).ToString()));
                Assert.That("31/12/1700 11:59:59 pm", Is.EqualTo(new DateTime(1700, 12, 31, 23, 59, 59).ToString()));
                //the last correct date
                Assert.That("15/10/1582 11:59:59 pm", Is.EqualTo(new DateTime(1582, 10, 15, 23, 59, 59).ToString()));

                //hack for DateTime.MinValue.ToString() works also for DateTime(0).
                Assert.That("1/01/0001 12:00:00 am", Is.EqualTo(DateTime.MinValue.ToString()));
                Assert.That("1/01/0001 12:00:00 am", Is.EqualTo(new DateTime(1, 1, 1, 0, 0, 0).ToString()));
                Assert.That("1/01/0001 12:00:00 am", Is.EqualTo(new DateTime(0).ToString()));
            }
            finally
            {
                RestoreLocale();
            }
        }

        [Test]
        public void TestDateTimesBefore1582Year()
        {
            try
            {
                SetInvariantLocale();

                Assert.That("14/10/1582 11:59:59 pm", Is.EqualTo(new DateTime(1582, 10, 14, 23, 59, 59).ToString()));
                Assert.That("5/10/1582 11:59:59 pm", Is.EqualTo(new DateTime(1582, 10, 5, 23, 59, 59).ToString()));

                Assert.That("4/10/1582 11:59:59 pm", Is.EqualTo(new DateTime(1582, 10, 4, 23, 59, 59).ToString()));
                Assert.That("1/01/0001 1:01:01 am", Is.EqualTo(new DateTime(1, 1, 1, 1, 1, 1).ToString()));
            }
            finally
            {
                RestoreLocale();
            }
        }

        /**
         * Few "features" because of slightly different interpretation of dates before Gregorian Calendar
         * CutOver Date (October 15, 1582 in most vestern countries):
         *
         * 1. The DateTime.MinValue converted by ours DateTime.toUnixDate() to "3/01/0001" instead
         * of "1/01/0001".
         * 2. DateTime.ToString() does the same since uses DateTime.toUnixDate() internally.
         *
         * -- Fixed by hack only for DateTime.MinValue.toUnixDate() (so for .ToString() too).
         * Because DateTime.MinValue.ToString() used widely by unit tests.
         */
        [Test]
        public void TestDateTimeMinValueHack()
        {
            try
            {
                SetInvariantLocale();

                //What the hack works for:
                Assert.That("1/01/0001 12:00:00 am", Is.EqualTo(DateTime.MinValue.ToString()));

#if JAVA
                //Doesn't work for:
                Assert.assertNotSame(DateTime.MinValue.toJavaTicks(), DateTime.MinValueToUnixTicks);
#endif
                Assert.That(new DateTime(DateTime.MinValue.Ticks).ToString(), IsNot.SameAs(DateTime.MinValue.ToString()));
            }
            finally
            {
                RestoreLocale();
            }
        }

        [Test]
        public void TestLocalTime()
        {
            try
            {
                SetInvariantLocale();

                DateTime dateTime = new DateTime(2007, 12, 31, 0, 0, 0);
                Assert.That(DateTimeKind.Unspecified, Is.EqualTo(dateTime.Kind));
                Assert.That("31/12/2007 12:00:00 am", Is.EqualTo(dateTime.ToString()));

                DateTime localConverted = dateTime.ToLocalTime();
                Assert.That(DateTimeKind.Local, Is.EqualTo(localConverted.Kind));
                // This check depends on local timezone and even day light saving offset - 
                // the result will be different on different systems.
                //Assert.AreEqual(localConverted.ToString(), "31/12/2007 7:00:00 a.m.");
            }
            finally
            {
                RestoreLocale();
            }
        }

        [Test]
        public void TestUniversalTime()
        {
            try
            {
                SetInvariantLocale();

                DateTime dateTime = new DateTime(2007, 12, 31, 12, 0, 0);
                Assert.That(DateTimeKind.Unspecified, Is.EqualTo(dateTime.Kind));
                Assert.That("31/12/2007 12:00:00 pm", Is.EqualTo(dateTime.ToString()));
                
                DateTime universalConverted = dateTime.ToUniversalTime();
                Assert.That(DateTimeKind.Utc, Is.EqualTo(universalConverted.Kind));
                //sk: in my timezone this looks like this:
                //Assert.AreEqual(universalConverted.ToString(), "31/12/2007 06:00:00");
                //but we shuld calculate Local timezone offset so test can pass in any timezone:
#if JAVA
                int javaOffset = CurrentThread.getTimeZone().getOffset(dateTime.toJavaTicks());
                DateTime universalCalculated = dateTime.AddMilliseconds(-javaOffset);
                Assert.That(universalCalculated.ToString(), Is.EqualTo(universalConverted.ToString()));
#endif
            }
            finally
            {
                RestoreLocale();
            }
        }

        /**
         * FileTime is Utc by nature. So Local DateTime always converted to Utc, but Unspecified treated
         * differently: ToFileTime() treats Unspecified as Local, ToFileTimeUtc() - as Utc.
         */
        [Test]
        public void TestToFileTime()
        {
            try
            {
                SetInvariantLocale();

                //Utc doesn't converted by both methods.
                DateTime dateTime = new DateTime(2007, 12, 31, 12, 0, 0, DateTimeKind.Utc);
                Assert.That(128435760000000000L, Is.EqualTo(dateTime.ToFileTime()));
                Assert.That(128435760000000000L, Is.EqualTo(dateTime.ToFileTimeUtc()));

                //Local converted to Utc by both methods.
                //We shuld to calculate Local timezone offset so test can pass in any timezone.
                dateTime = new DateTime(2007, 12, 31, 12, 0, 0, DateTimeKind.Local);
                DateTime universalCalculated = dateTime.ToUniversalTime();
                Assert.That(universalCalculated.ToFileTime(), Is.EqualTo(dateTime.ToFileTime()));
                Assert.That(universalCalculated.ToFileTimeUtc(), Is.EqualTo(dateTime.ToFileTimeUtc()));
                Assert.That(dateTime.ToFileTimeUtc(), Is.EqualTo(dateTime.ToFileTime()));

                //ToFileTime() treats Unspecified as Local, ToFileTimeUtc() - as Utc.
                dateTime = new DateTime(2007, 12, 31, 12, 0, 0, DateTimeKind.Unspecified);
                Assert.That(universalCalculated.ToFileTime(), Is.EqualTo(dateTime.ToFileTime()));//Unspecified treated as Local.
                Assert.That(128435760000000000L, Is.EqualTo(dateTime.ToFileTimeUtc()));//Unspecified treated as Utc.
            }
            finally
            {
                RestoreLocale();
            }
        }

        /**
         * FileTime is Utc by nature.
         * FromFileTimeUtc() return Utc DateTime.
         * FromFileTime() returns DateTime comverted to Local timezone.
         */
        [Test]
        public void TestFromFileTime()
        {
            try
            {
                SetInvariantLocale();

                DateTime dateTime = DateTime.FromFileTimeUtc(128435760000000000L);
                Assert.That(DateTimeKind.Utc, Is.EqualTo(dateTime.Kind));
                Assert.That("31/12/2007 12:00:00 pm", Is.EqualTo(dateTime.ToString()));

                dateTime = DateTime.FromFileTime(128435760000000000L);
                Assert.That(DateTimeKind.Local, Is.EqualTo(dateTime.Kind));
                Assert.That("31/12/2007 12:00:00 pm", Is.EqualTo(dateTime.ToUniversalTime().ToString()));
            }
            finally
            {
                RestoreLocale();
            }
        }

        [Test]
        public void TestEquals()
        {
            //Equals() takes into account only ticks, DateTimeKind is ignored.
            DateTime dateTime1 = new DateTime(2000, 1, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            DateTime dateTime2 = new DateTime(2000, 1, 1, 1, 1, 1, 1, DateTimeKind.Local);
            Assert.That(dateTime1.Equals(dateTime2), Is.True);
            Assert.That(DateTime.Equals(dateTime1, dateTime2), Is.True);
            Assert.That(dateTime2.Ticks, Is.EqualTo(dateTime1.Ticks));
            Assert.That(dateTime2.Day, Is.EqualTo(dateTime1.Day));
            Assert.That(dateTime2.DayOfWeek, Is.EqualTo(dateTime1.DayOfWeek));
            Assert.That(dateTime2.DayOfYear, Is.EqualTo(dateTime1.DayOfYear));
            Assert.That(dateTime2.GetHashCode(), Is.EqualTo(dateTime1.GetHashCode()));
            Assert.That(dateTime2.Hour, Is.EqualTo(dateTime1.Hour));
            Assert.That(dateTime2.Millisecond, Is.EqualTo(dateTime1.Millisecond));
            Assert.That(dateTime2.Minute, Is.EqualTo(dateTime1.Minute));
            Assert.That(dateTime2.Month, Is.EqualTo(dateTime1.Month));
            Assert.That(dateTime2.Second, Is.EqualTo(dateTime1.Second));
            Assert.That(dateTime2.TimeOfDay, Is.EqualTo(dateTime1.TimeOfDay));
            Assert.That(dateTime2.Year, Is.EqualTo(dateTime1.Year));

            Assert.That(dateTime1.Kind != dateTime2.Kind, Is.True);

            TimeSpan span1 = new TimeSpan(1, 1, 1, 1, 1);
            TimeSpan span2 = new TimeSpan(1, 1, 1, 1, 1);
            Assert.That(span1.Equals(span2), Is.True);
            Assert.That(TimeSpan.Equals(span1, span2), Is.True);
        }

        [Test]
        public void TestAdd()
        {
            DateTime dateTime = new DateTime(2000, 1, 1, 1, 1, 1, 1);

            Assert.That(new DateTime(2000, 1, 2, 2, 2, 2, 2), Is.EqualTo(dateTime.Add(new TimeSpan(1, 1, 1, 1, 1))));
            Assert.That(new DateTime(2010, 1, 1, 1, 1, 1, 1), Is.EqualTo(dateTime.AddYears(10)));
            Assert.That(new DateTime(2000, 11, 1, 1, 1, 1, 1), Is.EqualTo(dateTime.AddMonths(10)));
            Assert.That(new DateTime(2000, 1, 11, 1, 1, 1, 1), Is.EqualTo(dateTime.AddDays(10)));
            Assert.That(new DateTime(2000, 1, 1, 11, 1, 1, 1), Is.EqualTo(dateTime.AddHours(10)));
            Assert.That(new DateTime(2000, 1, 1, 1, 11, 1, 1), Is.EqualTo(dateTime.AddMinutes(10)));
            Assert.That(new DateTime(2000, 1, 1, 1, 1, 11, 1), Is.EqualTo(dateTime.AddSeconds(10)));
            Assert.That(new DateTime(2000, 1, 1, 1, 1, 1, 11), Is.EqualTo(dateTime.AddMilliseconds(10)));
            Assert.That(new DateTime(2000, 1, 1, 1, 1, 1, 2), Is.EqualTo(dateTime.AddTicks(10000)));
        }

        [Test]
        public void TestSubstract()
        {
            DateTime dateTime1 = new DateTime(2001, 1, 2, 2, 2, 2, 2);
            DateTime dateTime2 = new DateTime(2001, 1, 1, 1, 1, 1, 1);
            TimeSpan difference = new TimeSpan(1, 1, 1, 1, 1);

            Assert.That(dateTime2, Is.EqualTo(dateTime1.Subtract(difference)));
            Assert.That(difference, Is.EqualTo(dateTime1.Subtract(dateTime2)));
        }

        [Test]
        public void TestToString()
        {
            try
            {
                SetInvariantLocaleAndTZ();

                DateTime dateTime = new DateTime(1970, 1, 27, 15, 45, 1, 100);

                Assert.That(dateTime.ToString("d"), Is.EqualTo("27/01/1970"));//american Locale hardcoded (month first)
                Assert.That(dateTime.ToString("D"), Is.EqualTo("Tuesday, 27 January 1970"));
                Assert.That(dateTime.ToString("f"), Is.EqualTo("Tuesday, 27 January 1970 3:45 pm"));
                Assert.That(dateTime.ToString("F"), Is.EqualTo("Tuesday, 27 January 1970 3:45:01 pm"));
                Assert.That(dateTime.ToString("g"), Is.EqualTo("27/01/1970 3:45 pm"));
                Assert.That(dateTime.ToString("G"), Is.EqualTo("27/01/1970 3:45:01 pm"));
                Assert.That(dateTime.ToString("m"), Is.EqualTo("27 January"));
                Assert.That(dateTime.ToString("M"), Is.EqualTo("27 January"));
                Assert.That(dateTime.ToString("r"), Is.EqualTo("Tue, 27 Jan 1970 15:45:01 GMT"));
                Assert.That(dateTime.ToString("R"), Is.EqualTo("Tue, 27 Jan 1970 15:45:01 GMT"));
                Assert.That(dateTime.ToString("s"), Is.EqualTo("1970-01-27T15:45:01"));
                Assert.That(dateTime.ToString("t"), Is.EqualTo("3:45 pm"));
                Assert.That(dateTime.ToString("T"), Is.EqualTo("3:45:01 pm"));
                Assert.That(dateTime.ToString("u"), Is.EqualTo("1970-01-27 15:45:01Z"));

                //Custom patterns.
                Assert.That(dateTime.ToString("dd MMMM yyyy"), Is.EqualTo("27 January 1970"));
                Assert.That(dateTime.ToString("dd MM yy"), Is.EqualTo("27 01 70"));
                Assert.That(dateTime.ToString("dddd, MMMM dd yyyy"), Is.EqualTo("Tuesday, January 27 1970"));
                Assert.That(dateTime.ToString("dddd, MMMM dd"), Is.EqualTo("Tuesday, January 27"));
                Assert.That(dateTime.ToString("M/yy"), Is.EqualTo("1/70"));
                Assert.That(dateTime.ToString("dd-MM-yy"), Is.EqualTo("27-01-70"));
                //Xml datetime pattern used in NrxUtil.dateTimeToXml()
                Assert.That(dateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'"), Is.EqualTo("1970-01-27T15:45:01Z"));

                //ToString() without format
                Assert.That(dateTime.ToString(), Is.EqualTo("27/01/1970 3:45:01 pm"));
                Assert.That(dateTime.ToShortDateString(), Is.EqualTo("27/01/1970"));
            }
            finally
            {
                RestoreLocaleAndTZ();
            }
        }

        /**
         * This test depends on time zone info and works correctly for "good old" timezones.
         * Timezone that was changed recently can work incorrectly on "non-fresh" jre that
         * doesn't include changed timezone data yet.
         *
         * For instance, russian Locales can fail here because of recent removing of winter time.
         * But they will work fine on very last jre that already includes the info.
         */
        [Test]
        public void TestToUtcString()
        {
            try
            {
                SetInvariantLocaleAndTZ();

                DateTime dateTimeUtc = new DateTime(1970, 1, 27, 15, 45, 1, 100, DateTimeKind.Utc);
                Assert.That(dateTimeUtc.ToString("U"), Is.EqualTo("Tuesday, 27 January 1970 3:45:01 pm"));
            }
            finally
            {
                RestoreLocaleAndTZ();
            }
        }

        [Test]
        public void TestArabic()
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetCulture("ar-SA");

                DateTime dateUTC = new DateTime(2007, 1, 1, 1, 1, 1, DateTimeKind.Utc);
                Assert.That(dateUTC.ToString("d"), Is.EqualTo("11/12/27"));
                DateTime date = new DateTime(2007, 1, 1);
                Assert.That(date.ToString("d"), Is.EqualTo("11/12/27"));
                DateTime universalTime = date.ToUniversalTime();
                Assert.That(universalTime.ToString("d"), Is.EqualTo("10/12/27"));
                DateTime universalTimeFromUtc = dateUTC.ToUniversalTime();
                Assert.That(universalTimeFromUtc.ToString("d"), Is.EqualTo("11/12/27"));
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestOverflow()
        {
            DateTime dateTime = new DateTime(DateTime.MaxValue.Ticks + 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestUnderflow()
        {
            DateTime dateTime = new DateTime(DateTime.MinValue.Ticks - 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWrongYear()
        {
            DateTime dateTime = new DateTime(10000, 12, 31);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWrongYear2()
        {
            DateTime dateTime = new DateTime(-1, 12, 31);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWrongMonth()
        {
            DateTime dateTime = new DateTime(2007, 13, 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWrongMonth2()
        {
            DateTime dateTime = new DateTime(2007, -1, 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWrongDay()
        {
            DateTime dateTime = new DateTime(2007, 1, 32);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestWrongDay2()
        {
            DateTime dateTime = new DateTime(2007, 1, -1);
        }

        [Test]
        public void TestDateCtor()
        {
            DateTime dt = new DateTime(2001, 10, 26, 21, 32, 52, 0, DateTimeKind.Utc);
            Assert.That(dt.Ticks, Is.EqualTo(631397287720000000));
        }

        [Test]
        public void TestToLocal()
        {
            DateTime dt = DateTime.Parse("2001-10-26T21:32:52+02:00");
            Assert.That(dt, IsNot.Null());

            //DateTime.ToUniversalTime() is used to eliminate local TimeZones.
            DateTime universal = dt.ToUniversalTime();

            Assert.That(19, Is.EqualTo(universal.Hour));
            Assert.That(26, Is.EqualTo(universal.Day));

            DateTime local = dt.ToLocalTime();
            DateTime expectedLocal = universal.ToLocalTime();

            Assert.That(expectedLocal.Hour, Is.EqualTo(local.Hour));
            Assert.That(expectedLocal.Day, Is.EqualTo(local.Day));
        }


        [Test]
        public void TestUtcToLocal()
        {
            DateTime dt = DateTime.Parse("2001-10-26T21:32:52");
            Assert.That(dt, IsNot.Null());
            Assert.That(21, Is.EqualTo(dt.Hour));
            Assert.That(26, Is.EqualTo(dt.Day));

            DateTime local = dt.ToLocalTime();
            //DateTime.ToUniversalTime() is used to eliminate local TimeZones.
            DateTime expectedLocal = local.ToUniversalTime().ToLocalTime();
            Assert.That(expectedLocal.Hour, Is.EqualTo(local.Hour));
            Assert.That(expectedLocal.Day, Is.EqualTo(local.Day));
        }

        [Test]
        public void TestDateTimeRoundtripKind()
        {
            string value = "2001-10-26T21:32:52+02:00";
            DateTime r = DateTime.Parse(value, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.RoundtripKind);

            Assert.That(r, IsNot.Null());
            //DateTime.ToUniversalTime() is used to eliminate local TimeZones.
            DateTime universal = r.ToUniversalTime();

            Assert.That(universal.Ticks, Is.EqualTo(631397215720000000));
            Assert.That(universal.Day, Is.EqualTo(26));
            Assert.That(universal.Hour, Is.EqualTo(19));
        }

        [Test]
        public void TestDateTimeAdjustToUniversal()
        {
            string value = "2001-10-26T21:32:52+02:00";
            DateTime r = DateTime.Parse(value, DateTimeFormatInfo.CurrentInfo, DateTimeStyles.AdjustToUniversal);
            Assert.That(r, IsNot.Null());
            Assert.That(r.Ticks, Is.EqualTo(631397215720000000));
            Assert.That(r.Day, Is.EqualTo(26));
            Assert.That(r.Hour, Is.EqualTo(19));
        }

        [Test]
        public void TestDateTimeToOADateConversion()
        {
            DateTime r = DateTime.Parse("2001-10-26T21:32:52+02:00").ToUniversalTime();
            // Test conversion to OADate
            double oadate = r.ToOADate();
            Assert.That(r, IsNot.Null());
            Assert.That(r.Ticks, Is.EqualTo(631397215720000000));
            Assert.That(oadate, Is.EqualTo(37190.81449074074d));
            // Test conversion from OADate
            r = DateTime.FromOADate(oadate);
            Assert.That(r.Ticks, Is.EqualTo(631397215720000000));
#if JAVA
            // Test conversion from Java Date
            java.util.Date date = r.toJava();
            String originalDate = date.toString();
            oadate = DateTime.fromJava(date).toOADate();
            Assert.That(oadate, Is.EqualTo(37190.81449074074d));
            date = DateTime.fromOADate(oadate).toJava();
            msAssert.areEqual(originalDate, date.toString());
#endif
        }

        // Share Locale and TimeZone set/restore methods to other TestData* classes.

        public static void SetInvariantLocale()
        {
            Thread.CurrentThread.CurrentCulture = gInvariantLocale;
        }

        public static void SetInvariantLocaleAndTZ()
        {
            SetInvariantLocale();
#if JAVA
            // .Net 2.0 does not support change of local TimeZone
            CurrentThread.setTimeZone(gInvariantTZ);
#endif
        }

        public static void RestoreLocale()
        {
            Thread.CurrentThread.CurrentCulture = gOriginalLocale;
        }

        public static void RestoreLocaleAndTZ()
        {
            RestoreLocale();
#if JAVA
            // .Net 2.0 does not support change of local TimeZone
            CurrentThread.setTimeZone(gOriginalTZ);
#endif
        }

        private static CultureInfo gOriginalLocale = Thread.CurrentThread.CurrentCulture;
        private static TimeZone gOriginalTZ = TimeZone.CurrentTimeZone;

#if JAVA
        // .Net 2.0 does not support change of local TimeZone
        private static TimeZone gInvariantTZ = TimeZone.getTimeZone("Pacific/Auckland");
#endif
        private static CultureInfo gInvariantLocale = new CultureInfo("en-NZ", false);

    }
}
