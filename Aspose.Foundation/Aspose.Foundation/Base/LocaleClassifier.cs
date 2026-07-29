// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/03/2017 by Denis Shvydkiy

using Aspose.Common;

namespace Aspose
{
    public static class LocaleClassifier
    {
        /// <summary>
        /// Indicates whether the specified locale is Latin.
        /// </summary>
        public static bool IsLatin(int localeId)
        {
            switch ((Language)localeId)
            {
                case Language.Afrikaans:
                case Language.AfrikaansSouthAfrica:
                case Language.Albanian:
                case Language.AlbanianAlbania:
                case Language.Alsatian:
                case Language.Catalan:
                case Language.CatalanCatalan:
                case Language.BosnianLatin:
                case Language.Breton:
                case Language.Czech:
                case Language.CzechCzechRepublic:
                case Language.Danish:
                case Language.DanishDenmark:
                case Language.English:
                case Language.EnglishAustralia:
                case Language.EnglishBelize:
                case Language.EnglishCanada:
                case Language.EnglishCaribbean:
                case Language.EnglishHongKong:
                case Language.EnglishIndia:
                case Language.EnglishIndonesia:
                case Language.EnglishIreland:
                case Language.EnglishJamaica:
                case Language.EnglishMalaysia:
                case Language.EnglishNewZealand:
                case Language.EnglishPhilippines:
                case Language.EnglishSingapore:
                case Language.EnglishSouthAfrica:
                case Language.EnglishTrinidadAndTobago:
                case Language.EnglishUK:
                case Language.EnglishUS:
                case Language.EnglishZimbabwe:
                case Language.Finnish:
                case Language.FinnishFinland:
                case Language.French:
                case Language.FrenchBelgium:
                case Language.FrenchCameroon:
                case Language.FrenchCanada:
                case Language.FrenchCongo:
                case Language.FrenchCoteDIvoire:
                case Language.FrenchFrance:
                case Language.FrenchHaiti:
                case Language.FrenchLuxembourg:
                case Language.FrenchMali:
                case Language.FrenchMonaco:
                case Language.FrenchMorocco:
                case Language.FrenchReunion:
                case Language.FrenchSenegal:
                case Language.FrenchSwitzerland:
                case Language.FrenchWestIndies:
                case Language.German:
                case Language.GermanAustria:
                case Language.GermanGermany:
                case Language.GermanLiechtenstein:
                case Language.GermanLuxembourg:
                case Language.GermanSwitzerland:
                case Language.Igbo:
                case Language.Latin:
                case Language.Latvian:
                case Language.LatvianLatvia:
                case Language.Lithuanian:
                case Language.LithuanianLithuania:
                case Language.LuxembougishLuxemburg:
                case Language.Polish:
                case Language.PolishPoland:
                case Language.Portuguese:
                case Language.PortugueseBrazil:
                case Language.PortuguesePortugal:
                case Language.Spanish:
                case Language.SpanishArgentina:
                case Language.SpanishBolivia:
                case Language.SpanishChile:
                case Language.SpanishColombia:
                case Language.SpanishCostaRica:
                case Language.SpanishDominicanRepublic:
                case Language.SpanishEcuador:
                case Language.SpanishElSalvador:
                case Language.SpanishGuatemala:
                case Language.SpanishHonduras:
                case Language.SpanishMexico:
                case Language.SpanishNicaragua:
                case Language.SpanishPanama:
                case Language.SpanishParaguay:
                case Language.SpanishPeru:
                case Language.SpanishPuertoRico:
                case Language.SpanishSpainModernSort:
                case Language.SpanishSpainTraditionalSort:
                case Language.SpanishUruguay:
                case Language.SpanishVenezuela:
                case Language.Swedish:
                case Language.SwedishFinland:
                case Language.SwedishSweden:
                case Language.Xhosa:
                case Language.Zulu:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Indicates whether the current culture is Arabic.
        /// </summary>
        public static bool IsArabicCurrentCulture()
        {
           return (SystemPal.GetCurrentCultureTwoLetterName() == ArabicCultureTwoLetterName);
        }

        /// <summary>
        /// Indicates whether the specified locale is Cyrillic.
        /// </summary>
        public static bool IsCyrillic(int localeId)
        {
            switch ((Language)localeId)
            {
                case Language.Belarusian:
                case Language.BelarusianBelarus:
                case Language.Bulgarian:
                case Language.BulgarianBulgaria:
                case Language.BosnianCyrillic:
                case Language.Croatian:
                case Language.CroatianBozniaAndHerzegovina:
                case Language.CroatianCroatia:
                case Language.Kazakh:
                case Language.KazakhKazakhstan:
                case Language.Kyrgyz:
                case Language.KyrgyzKyrgyzstan:
                case Language.Macedonian:
                case Language.MacedonianFYROM:
                case Language.Russian:
                case Language.RussianMoldova:
                case Language.RussianRussia:
                case Language.Serbian:
                case Language.SerbianCyrillicBosniaAndHerzegovina:
                case Language.SerbianCyrillicSerbiaAndMontenegro:
                case Language.Ukrainian:
                case Language.UkrainianUkraine:
                    return true;

                default:
                    return false;
            }

        }

        /// <summary>
        /// Indicates whether the specified locale is Chinese Simplified.
        /// </summary>
        public static bool IsChineseSimplified(int localeId)
        {
            return ((localeId == (int)Language.ChineseChina) || (localeId == (int)Language.ChineseSingapore));
        }

        /// <summary>
        /// Indicates whether the specified locale is Chinese Traditional.
        /// </summary>
        public static bool IsChineseTraditional(int localeId)
        {
            Language lang = (Language)localeId;
            return
                lang == Language.ChineseTaiwan ||
                lang == Language.ChineseHongKong ||
                lang == Language.ChineseMacao;
        }

        /// <summary>
        /// Indicates whether the specified locale is Japanese.
        /// </summary>
        public static bool IsJapanese(int localeId)
        {
            switch((Language)localeId)
            {
                case Language.Japanese:
                case Language.JapaneseJapan:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Indicates whether the specified locale is either Chinese or Japanese.
        /// </summary>
        public static bool IsChineseOrJapanese(int localeId)
        {
            switch ((Language)localeId)
            {
                case Language.ChineseChina:
                case Language.ChineseHongKong:
                case Language.ChineseMacao:
                case Language.ChineseSingapore:
                case Language.ChineseTaiwan:
                case Language.Japanese:
                case Language.JapaneseJapan:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Indicates whether the specified locale is Korean.
        /// </summary>
        public static bool IsKorean(int locale)
        {
            return locale == (int)Language.Korean || locale == (int)Language.KoreanKorea;
        }

        /// <summary>
        /// Indicates if the specified locale is Chinese, Japanese or Korean.
        /// </summary>
        public static bool IsCjk(int locale)
        {
            return IsChineseOrJapanese(locale) || IsKorean(locale);
        }

        /// <summary>
        /// Indicates whether the specified locale is Arabic.
        /// </summary>
        public static bool IsArabic(int localeId)
        {
            return LanguageOnly.Compare(localeId, LanguageOnly.Arabic) ||
                LanguageOnly.Compare(localeId, LanguageOnly.Persian) ||
                LanguageOnly.Compare(localeId, LanguageOnly.Urdu);
        }

        /// <summary>
        /// Indicates whether the specified locale is Hebrew.
        /// </summary>
        public static bool IsHebrew(int localeId)
        {
            Language lang = (Language)localeId;

            return lang == Language.Hebrew ||
                   lang == Language.HebrewIsrael;
        }

        /// <summary>
        /// Indicates whether the specified Language enum value is defined.
        ///
        /// </summary>
        /// <remarks>
        /// The body of the switch clause is generated by TestLanguage.PrintSwitchBody() and tested by TestLanguage.TestEnumIsDefined().
        /// </remarks>
        public static bool IsDefined(Language value)
        {
            switch (value)
            {
                case Language.LanguageNotSet:
                case Language.Arabic:
                case Language.Bulgarian:
                case Language.Catalan:
                case Language.ChineseSimplified:
                case Language.Czech:
                case Language.Danish:
                case Language.German:
                case Language.Greek:
                case Language.English:
                case Language.Spanish:
                case Language.Finnish:
                case Language.French:
                case Language.Hebrew:
                case Language.Hungarian:
                case Language.Icelandic:
                case Language.Italian:
                case Language.Japanese:
                case Language.Korean:
                case Language.Dutch:
                case Language.Norwegian:
                case Language.Polish:
                case Language.Portuguese:
                case Language.Romanian:
                case Language.Russian:
                case Language.Croatian:
                case Language.Slovak:
                case Language.Albanian:
                case Language.Swedish:
                case Language.Thai:
                case Language.Turkish:
                case Language.Urdu:
                case Language.Indonesian:
                case Language.Ukrainian:
                case Language.Belarusian:
                case Language.Slovenian:
                case Language.Estonian:
                case Language.Latvian:
                case Language.Lithuanian:
                case Language.Persian:
                case Language.Vietnamese:
                case Language.Armenian:
                case Language.Azeri:
                case Language.Basque:
                case Language.Macedonian:
                case Language.Afrikaans:
                case Language.Georgian:
                case Language.Faeroese:
                case Language.Hindi:
                case Language.Malay:
                case Language.Kazakh:
                case Language.Kyrgyz:
                case Language.Kiswahili:
                case Language.Uzbek:
                case Language.Tatar:
                case Language.Punjabi:
                case Language.Gujarati:
                case Language.Tamil:
                case Language.Telugu:
                case Language.Kannada:
                case Language.Marathi:
                case Language.Sanskrit:
                case Language.Mongolian:
                case Language.Galician:
                case Language.Konkani:
                case Language.Syriac:
                case Language.Divehi:
                case Language.InvariantCulture:
                case Language.NoProof:
                case Language.ArabicSaudiArabia:
                case Language.BulgarianBulgaria:
                case Language.CatalanCatalan:
                case Language.ChineseTaiwan:
                case Language.CzechCzechRepublic:
                case Language.DanishDenmark:
                case Language.GermanGermany:
                case Language.GreekGreece:
                case Language.EnglishUS:
                case Language.SpanishSpainTraditionalSort:
                case Language.FinnishFinland:
                case Language.FrenchFrance:
                case Language.HebrewIsrael:
                case Language.HungarianHungary:
                case Language.IcelandicIceland:
                case Language.ItalianItaly:
                case Language.JapaneseJapan:
                case Language.KoreanKorea:
                case Language.DutchNetherlands:
                case Language.NorwegianBokmal:
                case Language.PolishPoland:
                case Language.PortugueseBrazil:
                case Language.RomanshSwitzerland:
                case Language.RomanianRomania:
                case Language.RussianRussia:
                case Language.CroatianCroatia:
                case Language.SlovakSlovakia:
                case Language.AlbanianAlbania:
                case Language.SwedishSweden:
                case Language.ThaiThailand:
                case Language.TurkishTurkey:
                case Language.UrduPakistan:
                case Language.IndonesianIndonesia:
                case Language.UkrainianUkraine:
                case Language.BelarusianBelarus:
                case Language.SlovenianSlovenia:
                case Language.EstonianEstonia:
                case Language.LatvianLatvia:
                case Language.LithuanianLithuania:
                case Language.Tajik:
                case Language.PersianIran:
                case Language.VietnameseVietnam:
                case Language.ArmenianArmenia:
                case Language.AzeriLatin:
                case Language.BasqueBasque:
                case Language.Sorbian:
                case Language.MacedonianFYROM:
                case Language.Sutu:
                case Language.Tsonga:
                case Language.Tswana:
                case Language.Venda:
                case Language.Xhosa:
                case Language.Zulu:
                case Language.AfrikaansSouthAfrica:
                case Language.GeorgianGeorgia:
                case Language.FaeroeseFaroeIslands:
                case Language.HindiIndia:
                case Language.Maltese:
                case Language.SamiNorthernNorway:
                case Language.GaelicScotland:
                case Language.Yiddish:
                case Language.MalayMalaysia:
                case Language.KazakhKazakhstan:
                case Language.KyrgyzKyrgyzstan:
                case Language.KiswahiliKenya:
                case Language.Turkmen:
                case Language.UzbekLatin:
                case Language.TatarRussia:
                case Language.Bengali:
                case Language.PunjabiIndia:
                case Language.GujaratiIndia:
                case Language.Oriya:
                case Language.TamilIndia:
                case Language.TeluguIndia:
                case Language.KannadaIndia:
                case Language.Malayalam:
                case Language.Assamese:
                case Language.MarathiIndia:
                case Language.SanskritIndia:
                case Language.MongolianCyrillic:
                case Language.TibetanChina:
                case Language.Welsh:
                case Language.Khmer:
                case Language.Lao:
                case Language.Burmese:
                case Language.GalicianGalician:
                case Language.KonkaniIndia:
                case Language.Manipuri:
                case Language.SindhiDevanagaric:
                case Language.SyriacSyria:
                case Language.Sinhalese:
                case Language.Cherokee:
                case Language.Inuktitut:
                case Language.Amharic:
                case Language.Tamazight:
                case Language.KashmiriArabic:
                case Language.Nepali:
                case Language.FrisianNetherlands:
                case Language.Pashto:
                case Language.Filipino:
                case Language.DivehiMaldives:
                case Language.Edo:
                case Language.Fulfulde:
                case Language.Hausa:
                case Language.Ibibio:
                case Language.Yoruba:
                case Language.QuechuaBolivia:
                case Language.Sepedi:
                case Language.LuxembougishLuxemburg:
                case Language.Igbo:
                case Language.Kanuri:
                case Language.Oromo:
                case Language.TigrignaEthiopia:
                case Language.Guarani:
                case Language.Hawaiian:
                case Language.Latin:
                case Language.Somali:
                case Language.Yi:
                case Language.Papiamentu:
                case Language.MapudungunChile:
                case Language.MohawkMohawk:
                case Language.Breton:
                case Language.Maori:
                case Language.Alsatian:
                case Language.ArabicIraq:
                case Language.ChineseChina:
                case Language.GermanSwitzerland:
                case Language.EnglishUK:
                case Language.SpanishMexico:
                case Language.FrenchBelgium:
                case Language.ItalianSwitzerland:
                case Language.DutchBelgium:
                case Language.NorwegianNynorsk:
                case Language.PortuguesePortugal:
                case Language.RomanianMoldova:
                case Language.RussianMoldova:
                case Language.SerbianLatin:
                case Language.SerbianLatinSerbiaAndMontenegro:
                case Language.SwedishFinland:
                case Language.UrduIndian:
                case Language.AzeriCyrillic:
                case Language.SamiNothernSweden:
                case Language.IrishIreland:
                case Language.MalayBruneiDarussalam:
                case Language.UzbekCyrillic:
                case Language.BengaliBangladesh:
                case Language.PunjabiPakistan:
                case Language.MongolianMongolian:
                case Language.TibetanButan:
                case Language.Sindhi:
                case Language.InuktitutLatinCanada:
                case Language.TamazightLatin:
                case Language.Kashmiri:
                case Language.NepaliIndia:
                case Language.QuechuaEcuador:
                case Language.TigrignaEritrea:
                case Language.ArabicEgypt:
                case Language.ChineseHongKong:
                case Language.GermanAustria:
                case Language.EnglishAustralia:
                case Language.SpanishSpainModernSort:
                case Language.FrenchCanada:
                case Language.SerbianCyrillicSerbiaAndMontenegro:
                case Language.SamiNorthernFinland:
                case Language.QuechuaPeru:
                case Language.ArabicLibya:
                case Language.ChineseSingapore:
                case Language.GermanLuxembourg:
                case Language.EnglishCanada:
                case Language.SpanishGuatemala:
                case Language.FrenchSwitzerland:
                case Language.CroatianBozniaAndHerzegovina:
                case Language.SamiLuleNorway:
                case Language.ArabicAlgeria:
                case Language.ChineseMacao:
                case Language.GermanLiechtenstein:
                case Language.EnglishNewZealand:
                case Language.SpanishCostaRica:
                case Language.FrenchLuxembourg:
                case Language.BosnianLatin:
                case Language.SamiLuleSweden:
                case Language.ArabicMorocco:
                case Language.EnglishIreland:
                case Language.SpanishPanama:
                case Language.FrenchMonaco:
                case Language.SerbianLatinBosniaAndHerzegovina:
                case Language.SamiSouthernNorway:
                case Language.ArabicTunisia:
                case Language.EnglishSouthAfrica:
                case Language.SpanishDominicanRepublic:
                case Language.FrenchWestIndies:
                case Language.SerbianCyrillicBosniaAndHerzegovina:
                case Language.SamiSouthernSweden:
                case Language.ArabicOman:
                case Language.EnglishJamaica:
                case Language.SpanishVenezuela:
                case Language.FrenchReunion:
                case Language.BosnianCyrillic:
                case Language.SamiSkoltFinland:
                case Language.ArabicYemen:
                case Language.EnglishCaribbean:
                case Language.SpanishColombia:
                case Language.FrenchCongo:
                case Language.SamiInariFinland:
                case Language.ArabicSyria:
                case Language.EnglishBelize:
                case Language.SpanishPeru:
                case Language.FrenchSenegal:
                case Language.ArabicJordan:
                case Language.EnglishTrinidadAndTobago:
                case Language.SpanishArgentina:
                case Language.FrenchCameroon:
                case Language.ArabicLebanon:
                case Language.EnglishZimbabwe:
                case Language.SpanishEcuador:
                case Language.FrenchCoteDIvoire:
                case Language.ArabicKuwait:
                case Language.EnglishPhilippines:
                case Language.SpanishChile:
                case Language.FrenchMali:
                case Language.ArabicUAE:
                case Language.EnglishIndonesia:
                case Language.SpanishUruguay:
                case Language.FrenchMorocco:
                case Language.ArabicBahrain:
                case Language.EnglishHongKong:
                case Language.SpanishParaguay:
                case Language.FrenchHaiti:
                case Language.ArabicQatar:
                case Language.EnglishIndia:
                case Language.SpanishBolivia:
                case Language.EnglishMalaysia:
                case Language.SpanishElSalvador:
                case Language.EnglishSingapore:
                case Language.SpanishHonduras:
                case Language.SpanishNicaragua:
                case Language.SpanishPuertoRico:
                case Language.ChineseTraditional:
                case Language.Serbian:
                    return true;
                default:
                    return false;
            }
        }

        private const string ArabicCultureTwoLetterName = "ar";
    }
}
