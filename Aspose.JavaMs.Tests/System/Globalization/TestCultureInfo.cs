// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/08/2020 by Vyacheslav Durin

using System;
using System.Globalization;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Globalization
{
    [TestFixture]
    /// <summary>
    /// Tests the methods of the CultureInfo class except the following
    /// 
    ///   CultureInfo.DefaultThreadCurrentCulture();
    ///   CultureInfo.DefaultThreadCurrentUICulture();
    ///   CultureInfo.CurrentUICulture(); 
    ///   CultureInfo.InstalledUICulture();
    ///   CultureInfo.GetCultureInfoByIetfLanguageTag();
    /// </summary>
    public class TestCultureInfo
    {
        [Test]
        public void TestGetCulturesByType()
        {
            // The numbers could be different, e.g. on my machine 
            // AllCultures = 850
            // SpecificCultures = 569
            // NeutralCultures = 281
            // So we just check that we can retrieve at least one record of each type.
            Assert.That(CultureInfo.GetCultures(CultureTypes.AllCultures).Length, Is.GreaterThanOrEqualTo(800));
            Assert.That(CultureInfo.GetCultures(CultureTypes.SpecificCultures).Length, Is.GreaterThanOrEqualTo(500));
            Assert.That(CultureInfo.GetCultures(CultureTypes.NeutralCultures).Length, Is.GreaterThanOrEqualTo(200));
        }

        [Test]
        public void TestCreateSpecificCultureRandomShortLanguage()
        {
            Assert.That(CultureInfo.CreateSpecificCulture("gib").DisplayName, Is.EqualTo("Invariant Language (Invariant Country)"));
            Assert.That(CultureInfo.CreateSpecificCulture("xyz").DisplayName, Is.EqualTo("Invariant Language (Invariant Country)"));
        }

#if NET40
        [Test]
        [ExpectedException(typeof(CultureNotFoundException))]
        public void TestCreateSpecificCultureRandomLongLanguage()
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("abcd");
        }
#endif

        [Test]
        public void TestCreateSpecificCulture()
        {
            AssertInvariantCulture(CultureInfo.CreateSpecificCulture("xc"));
            AssertInvariantCulture(CultureInfo.CreateSpecificCulture("xx"));
            Assert.That(CultureInfo.CreateSpecificCulture("lz-ZZ").DisplayName, Is.EqualTo("Unknown Locale (lz-ZZ)"));
            AssertInvariantCulture(CultureInfo.CreateSpecificCulture("lz"));
            AssertCustomRuEnCulture(CultureInfo.CreateSpecificCulture("ru-EN"));
            AssertCustomEnUaCulture(CultureInfo.CreateSpecificCulture("en-UA"));
        }

        [Test]
        public void TestInvariantCulture()
        {
            CultureInfo invariant = CultureInfo.InvariantCulture;
            AssertInvariantCulture(invariant);
            AssertGregorianCalendar(invariant.Calendar);
        }

        /// <summary>
        /// At the moment Java's test fails. 
        /// You will see that CultureInfo.GetCultureInfo("xx") returns 'Unknown Language (xx)'
        /// But CultureInfo.CreateSpecificCulture("xx") returns the Invariant culture
        /// 
        /// I haven't found any explanation how these methods work with custom (non-existing) cultures.
        /// There are many things are going on in C# native code. 
        /// I replicated the similar workflow at some scale, but in needs to be improved in the future.
        /// </summary>
        [Test, Ignore("Ignore the test that depend on local culture")]
        public void TestControversialConditions()
        {
            Assert.That(CultureInfo.GetCultureInfo("lz-ZZ").DisplayName, Is.EqualTo("Unknown Locale (lz-ZZ)"));
            Assert.That(CultureInfo.GetCultureInfo("xx").DisplayName, Is.EqualTo("Unknown Language (xx)"));

            AssertInvariantCulture(CultureInfo.CreateSpecificCulture("xx"));
            Assert.That(new CultureInfo("xx").DisplayName, Is.EqualTo("Unknown Language (xx)"));
            Assert.That(new CultureInfo("xx", false).DisplayName, Is.EqualTo("Unknown Language (xx)"));

            Assert.That(CultureInfo.CreateSpecificCulture("xx-XY").DisplayName, Is.EqualTo("Unknown Locale (xx-XY)"));
            Assert.That(new CultureInfo("xx-XY").DisplayName, Is.EqualTo("Unknown Locale (xx-XY)"));
        }

        [Test]
        public void TestGetCultureInfoByName()
        {
            AssertEnNzCulture(CultureInfo.GetCultureInfo("en-NZ"));
            AssertEnUsCulture(CultureInfo.GetCultureInfo("en-US"));
            AssertJaJpCulture(CultureInfo.GetCultureInfo("ja-JP"));
            AssertKoKpCulture(CultureInfo.GetCultureInfo("ko-KP"));
            AssertHeIlCulture(CultureInfo.GetCultureInfo("he-IL"));
            AssertHiInCulture(CultureInfo.GetCultureInfo("hi-IN")); // Java iw_IL
            AssertRuRuCulture(CultureInfo.GetCultureInfo("ru-RU"));
        }

#if NET40
        [Test]
        [ExpectedException(typeof(CultureNotFoundException))]
        public void TestGetCultureInfoByCultureNonExistingID()
        {
            CultureInfo.GetCultureInfo(70000);
        }
#endif

        [Test]
        public void TestGetCultureInfoByCultureID()
        {
            AssertEnNzCulture(CultureInfo.GetCultureInfo(5129));
            AssertEnUsCulture(CultureInfo.GetCultureInfo(1033));
            AssertJaJpCulture(CultureInfo.GetCultureInfo(1041));
            AssertHeIlCulture(CultureInfo.GetCultureInfo(1037));
            AssertHiInCulture(CultureInfo.GetCultureInfo(1081));
            AssertRuRuCulture(CultureInfo.GetCultureInfo(1049));

            AssertEsEsTraditionalCulture(CultureInfo.GetCultureInfo(1034));
        }

        [Test]
        public void TestCreateNewCultureInfoByName()
        {
            AssertRuRuCulture(new CultureInfo("ru-RU"));
            AssertRuCulture(new CultureInfo("ru"));
        }

        [Test]
        public void TestCreateNewCultureInfoByCultureId()
        {
            AssertEnNzCulture(new CultureInfo(5129));
            AssertRuRuCulture(new CultureInfo(1049));
        }

#if NET40
        [Test]
        [ExpectedException(typeof(CultureNotFoundException))]
        public void TestCreateNewCultureInfoByCultureNonExistingID()
        {
            new CultureInfo(70000);
        }
#endif

        private void AssertHiInCulture(CultureInfo candidate)
        {
            AssertCulture(1081, 0, "hi-IN", "Hindi (India)", "Hindi (India)",
                                  "हिन्दी (भारत)", "hi-IN", "hin",
                                   "HIN", "hi", candidate);
        }

        private void AssertHeIlCulture(CultureInfo candidate)
        {
            AssertCulture(1037, 1255, "he-IL", "Hebrew (Israel)", "Hebrew (Israel)",
                                   "עברית (ישראל)", "he-IL", "heb",
                                   "HEB", "he", candidate);
        }


        private void AssertKoKpCulture(CultureInfo candidate)
        {
            Console.WriteLine("KOREA=" + candidate.LCID);
            AssertCulture(4096, 0, "ko-KP", "Korean (North Korea)", "Korean (North Korea)",
                                     "한국어 (조선민주주의인민공화국)", "ko-KP", "kor",
                                     "ZZZ", "ko", candidate);
        }

        private void AssertJaJpCulture(CultureInfo candidate)
        {
            AssertCulture(1041, 932, "ja-JP", "Japanese (Japan)", "Japanese (Japan)",
                                     "日本語 (日本)", "ja-JP", "jpn",
                                     "JPN", "ja", candidate);
        }


        private void AssertEnNzCulture(CultureInfo candidate)
        {
            AssertCulture(5129, 1252, "en-NZ", "English (New Zealand)", "English (New Zealand)",
                           "English (New Zealand)", "en-NZ", "eng",
                           "ENZ", "en", candidate);
        }

        private void AssertEnUsCulture(CultureInfo candidate)
        {
            AssertCulture(1033, 1252, "en-US", "English (United States)", "English (United States)",
                           "English (United States)", "en-US", "eng",
                           "ENU", "en", candidate);
        }

        private void AssertRuCulture(CultureInfo candidate)
        {
            AssertCulture(25, 1251, "ru", "Russian", "Russian",
                           "русский", "ru", "rus",
                           "RUS", "ru", candidate);
            DateTimeFormatInfo dtfi = candidate.DateTimeFormat;
            AssertDateTimeFormat(dtfi, "dd.MM.yyyy", "H:mm", "d MMMM yyyy 'г.'",
                   "H:mm:ss", "d MMMM", "MMMM yyyy", ".", ":", "d MMMM yyyy 'г.' H:mm:ss",
                   "", "", "ddd, dd MMM yyyy HH':'mm':'ss 'GMT'", "yyyy'-'MM'-'dd'T'HH':'mm':'ss",
                    "yyyy'-'MM'-'dd HH':'mm':'ss'Z'"
                );
        }

        private void AssertRuRuCulture(CultureInfo candidate)
        {
            AssertCulture(1049, 1251, "ru-RU", "Russian (Russia)", "Russian (Russia)",
                           "русский (Россия)", "ru-RU", "rus",
                           "RUS", "ru", candidate);
        }


        private void AssertCustomRuEnCulture(CultureInfo candidate)
        {
            AssertCulture(4096, 1251, "ru-EN", "Unknown Locale (ru-EN)", "Unknown Locale (ru-EN)",
                "Unknown Locale (ru-EN)", "ru-EN", "rus",
                "RUS", "ru", candidate);
        }

        private void AssertCustomEnUaCulture(CultureInfo candidate)
        {
            AssertCulture(4096, 1252, "en-UA", "Unknown Locale (en-UA)", "Unknown Locale (en-UA)",
                "Unknown Locale (en-UA)", "en-UA", "eng",
                "ENU", "en", candidate);
        }

        private void AssertInvariantCulture(CultureInfo candidate)
        {
            AssertCulture(
                 127,
                 1252,
                 "",
                 "Invariant Language (Invariant Country)",
                 "Invariant Language (Invariant Country)",
                 "Invariant Language (Invariant Country)",
                 "",
                 "ivl",
                 "IVL",
                 "iv",
                 candidate);
        }

        private void AssertEsEsTraditionalCulture(CultureInfo candidate)
        {
            AssertCulture(candidate, 1034, "es-ES", "Spanish (Spain)", "Spanish (Spain, Traditional Sort)",
                "español (España, alfabetización tradicional)", "es-ES", "spa", "ESP", "es");
        }
        
        private void AssertCulture(int lcid, int ansiCodePage, string name, string displayName, string englishName, string nativeName, CultureInfo culture)
        {
            AssertCulture(lcid, ansiCodePage, name, displayName, englishName, nativeName, null, null, null, null, culture);
        }

        private void AssertCulture(int lcid, int ansiCodePage, string name,
            string displayName, string englishName, string nativeName,
            string ietfLanguageTag, string threeLetterIsoName, string threeLetterWinName,
            string twoLetterIsoLngName,
            CultureInfo culture
        )
        {
            Assert.That(culture, IsNot.Null());
            Assert.That(culture.TextInfo.ANSICodePage, Is.EqualTo(ansiCodePage));
            Assert.That(culture.LCID, Is.EqualTo(lcid));
            Assert.That(culture.Name, Is.EqualTo(name));
            Assert.That(culture.DisplayName, Is.EqualTo(displayName));
            Assert.That(culture.EnglishName, Is.EqualTo(englishName));
            Assert.That(culture.NativeName, Is.EqualTo(nativeName));
            Assert.That(culture.IetfLanguageTag, Is.EqualTo(ietfLanguageTag));
            Assert.That(culture.ThreeLetterISOLanguageName, Is.EqualTo(threeLetterIsoName));
            Assert.That(culture.ThreeLetterWindowsLanguageName, Is.EqualTo(threeLetterWinName));
            Assert.That(culture.TwoLetterISOLanguageName, Is.EqualTo(twoLetterIsoLngName));
            Assert.That(culture.ToString(), Is.EqualTo(culture.Name));
        }

        private void AssertCulture(CultureInfo culture, int lcid, params string[] names)
        {
            Assert.That(culture, IsNot.Null());
            Assert.That(culture.LCID, Is.EqualTo(lcid));

            string actual = string.Join(";", names);
            
            string[] expectedArray = new string[] {
                culture.Name, culture.DisplayName, culture.EnglishName, culture.NativeName, culture.IetfLanguageTag, 
                culture.ThreeLetterISOLanguageName, culture.ThreeLetterWindowsLanguageName, culture.TwoLetterISOLanguageName
            };
            string expected = string.Join(";", expectedArray);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private void AssertDateTimeFormat(DateTimeFormatInfo dateTimeFormatInfo,
            string shortDatePattern,
            string shortTimePattern,
            string longDatePattern,
            string longTimePattern,
            string monthDayPattern,
            string yearMonthPattern,
            string dateSeparator,
            string timeSeparator,
            string fullDateTimePattern,
            string pMDesignator,
            string aMDesignator,
            string rFC1123Pattern,
            string sortableDateTimePattern,
            string universalSortableDateTimePattern
            )
        {
            Assert.That(dateTimeFormatInfo, IsNot.Null());
            Assert.That(shortDatePattern, Is.EqualTo(dateTimeFormatInfo.ShortDatePattern));
            Assert.That(shortTimePattern, Is.EqualTo(dateTimeFormatInfo.ShortTimePattern));
            Assert.That(longDatePattern, Is.EqualTo(dateTimeFormatInfo.LongDatePattern));
            Assert.That(longTimePattern, Is.EqualTo(dateTimeFormatInfo.LongTimePattern));
            Assert.That(monthDayPattern, Is.EqualTo(dateTimeFormatInfo.MonthDayPattern));
            Assert.That(yearMonthPattern, Is.EqualTo(dateTimeFormatInfo.YearMonthPattern));
            Assert.That(dateSeparator, Is.EqualTo(dateTimeFormatInfo.DateSeparator));
            Assert.That(timeSeparator, Is.EqualTo(dateTimeFormatInfo.TimeSeparator));
            Assert.That(fullDateTimePattern, Is.EqualTo(dateTimeFormatInfo.FullDateTimePattern));
            Assert.That(pMDesignator, Is.EqualTo(dateTimeFormatInfo.PMDesignator));
            Assert.That(aMDesignator, Is.EqualTo(dateTimeFormatInfo.AMDesignator));
            Assert.That(rFC1123Pattern, Is.EqualTo(dateTimeFormatInfo.RFC1123Pattern));
            Assert.That(sortableDateTimePattern, Is.EqualTo(dateTimeFormatInfo.SortableDateTimePattern));
            Assert.That(universalSortableDateTimePattern, Is.EqualTo(dateTimeFormatInfo.UniversalSortableDateTimePattern));

        }

        private void AssertGregorianCalendar(Calendar calendar)
        {
            Assert.That(calendar, IsNot.Null());
            // Extend the verification if needed
        }

    }
}
