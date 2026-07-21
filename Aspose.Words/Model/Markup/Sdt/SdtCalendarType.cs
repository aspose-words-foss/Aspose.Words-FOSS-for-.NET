// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/07/2010 by Denis Darkin

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Specifies the possible types of calendars which can be used to specify <see cref="StructuredDocumentTag.CalendarType"/>
    /// in an Office Open XML document.
    /// </summary>
    public enum SdtCalendarType
    {
        /// <summary>
        /// Used as default value in OOXML. Equals <see cref="Gregorian"/>.
        /// </summary>
        Default,

        /// <summary>
        /// Specifies that the Gregorian calendar, as defined in ISO  8601, shall be used. 
        /// This calendar should be localized into the appropriate language.
        /// </summary>
        Gregorian = Default,

        /// <summary>
        /// Specifies that the Gregorian calendar, as defined in ISO 8601, shall be used.
        /// The values for this calendar should be presented in Arabic.
        /// </summary>
        GregorianArabic,

        /// <summary>
        /// Specifies that the Gregorian calendar, as defined in ISO 8601, shall be used.
        /// The values for this calendar should be presented in Middle East French.
        /// </summary>
        GregorianMeFrench,

        /// <summary>
        /// Specifies that the Gregorian calendar, as defined in ISO 8601, shall be used.
        /// The values for this calendar should be presented in English.
        /// </summary>
        GregorianUs,

        /// <summary>
        /// Specifies that the Gregorian calendar, as defined in ISO 8601, shall be used.
        /// The values for this calendar should be the representation of the English strings in the corresponding Arabic characters 
        /// (the Arabic transliteration of the English for the Gregorian calendar).
        /// </summary>
        GregorianXlitEnglish,

        /// <summary>
        /// Specifies that the Gregorian calendar, as defined in ISO 8601, shall be used.
        /// The values for this calendar should be the representation of the French strings in the corresponding Arabic characters 
        /// (the Arabic transliteration of the French for the Gregorian calendar).
        /// </summary>
        GregorianXlitFrench,

        /// <summary>
        /// Specifies that the Hebrew lunar calendar, as described by the Gauss formula for Passover [CITATION] 
        /// and The Complete Restatement of Oral Law (Mishneh Torah),shall be used.
        /// </summary>
        Hebrew,

        /// <summary>
        /// Specifies that the Hijri lunar calendar, as described by the Kingdom of Saudi Arabia, 
        /// Ministry of Islamic Affairs, Endowments, Da‘wah and Guidance, shall be used.
        /// </summary>
        Hijri,

        /// <summary>
        /// Specifies that the Japanese Emperor Era calendar, as described by 
        /// Japanese Industrial Standard JIS X 0301, shall be used.
        /// </summary>
        Japan,

        /// <summary>
        /// Specifies that the Korean Tangun Era calendar, 
        /// as described by Korean Law Enactment No. 4, shall be used.
        /// </summary>
        Korea,

        /// <summary>
        /// Specifies that no calendar should be used.
        /// </summary>
        /// <remarks>
        /// Usually in AW, None is the first and default value for enums, but not in this case.
        /// None is not default for OOXML, instead <see cref="Gregorian"/> is default and is first member of this enum.
        /// </remarks>
        None,

        /// <summary>
        /// Specifies that the Saka Era calendar, as described by the Calendar Reform Committee of India, 
        /// as part of the Indian Ephemeris and Nautical Almanac, shall be used.
        /// </summary>
        Saka,

        /// <summary>
        /// Specifies that the Taiwanese calendar, as defined by the Chinese National Standard CNS 7648, shall be used.
        /// </summary>
        Taiwan,

        /// <summary>
        /// Specifies that the Thai calendar, as defined by the Royal Decree of H.M. King Vajiravudh (Rama VI) in 
        /// Royal Gazette B. E. 2456 (1913 A.D.) and by the decree of Prime Minister Phibunsongkhram (1941 A.D.) to 
        /// start the year on the Gregorian January 1 and to map year zero to Gregorian year 543 B.C., shall be used.
        /// </summary>
        Thai 
    }
}
