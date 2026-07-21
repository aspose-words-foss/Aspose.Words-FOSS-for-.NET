// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2016 by Konstantin Sidorenko

using System;
using System.Globalization;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Globalization
{
    [TestFixture]
    public class TestCalendar 
    {

        [Test, Ignore("TestEras")]
        public void TestEras()
        {
            JapaneseCalendar japaneseCalendar = new JapaneseCalendar();
            JapaneseLunisolarCalendar calendarJapaneseLunisolar = new JapaneseLunisolarCalendar();
            KoreanCalendar koreanCalendar = new KoreanCalendar();
            KoreanLunisolarCalendar hcalendarKoreanLunisolar = new KoreanLunisolarCalendar();
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();
            TaiwanLunisolarCalendar taiwanLunisolarCalendar = new TaiwanLunisolarCalendar();
            ThaiBuddhistCalendar thaiBuddhistCalendar = new ThaiBuddhistCalendar();
            ChineseLunisolarCalendar calendarChineseLunisolar = new ChineseLunisolarCalendar();

            int[] eras = japaneseCalendar.Eras;
            DateTime minSupportedDateTime = japaneseCalendar.MinSupportedDateTime;
            DateTime maxSupportedDateTime = japaneseCalendar.MaxSupportedDateTime;
            int era1 = japaneseCalendar.GetEra(japaneseCalendar.MaxSupportedDateTime);
            int era2 = japaneseCalendar.GetEra(japaneseCalendar.MinSupportedDateTime);
            int era3 = japaneseCalendar.GetEra(japaneseCalendar.MinSupportedDateTime - new TimeSpan(1, 0, 0, 0));

            GregorianCalendar gregorianCalendar = new GregorianCalendar();
            int[] ints = gregorianCalendar.Eras;

            DateTimeFormatInfo dateTimeFormatInfo = new DateTimeFormatInfo(); 
        }
    }
}