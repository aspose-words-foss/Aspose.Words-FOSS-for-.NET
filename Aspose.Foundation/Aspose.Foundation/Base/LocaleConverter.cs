// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/02/2021 by Roman Korchagin, Alexey Minenkov

using Aspose.Collections;

namespace Aspose
{
    /// <summary>
    /// Provides methods for converting <see cref="Language"/> values to their string representations (language tags).
    /// </summary>
    public static class LocaleConverter
    {
        /// <summary>
        /// Gets <see cref="Language"/> code for the specified language tag.
        /// </summary>
        /// <param name="tag">Language tag specified in the format used in .docx.</param>
        public static int DocxTagToLocale(string tag)
        {
            return TagToLocale(tag, gLanguageMapDocx, gOldTagToCodeDictionaryDocx);
        }

        /// <summary>
        /// Gets <see cref="Language"/> code for the specified language tag.
        /// </summary>
        /// <param name="tag">Language tag specified in the format used in WordML.</param>
        public static int WmlTagToLocale(string tag)
        {
            return TagToLocale(tag, gLanguageMapWml, gOldTagToCodeDictionaryWml);
        }

        /// <summary>
        /// Gets language tag (in the format used in .docx) for the specified <see cref="Language"/> code.
        /// </summary>
        public static string LocaleToDocxTag(int locale)
        {
            return gLanguageMapDocx.GetValue(locale, "");
        }

        /// <summary>
        /// Gets language tag (in the format used in WordML) for the specified <see cref="Language"/> code.
        /// </summary>
        public static string LocaleToWmlTag(int locale)
        {
            string code = gLanguageMapWml.TryGetValue(locale);

            if (code == null && gAdditionalLanguageMapWml.ContainsKey(locale))
                code = gAdditionalLanguageMapWml[locale];

            return code == null ? "" : code;
        }

        private static void AdditionalLanguageToMapWml(string langCode, int langId)
        {
            gAdditionalLanguageMapWml.Add(langId, langCode);
        }

        /// <summary>
        /// Returns <see cref="Language"/> code for the specified language tag
        /// or <see cref="Language.InvariantCulture"/> if the specified language tag does not exist.
        /// </summary>
        /// <remarks>
        /// Old tags are also supported for backward compatibility, so a different set of dictionaries is needed.
        /// </remarks>
        private static int TagToLocale(
            string tag, StringToIntBidirectionalMap mainDictionary, StringToIntDictionary oldTagToCodeDictionary)
        {
            int code = mainDictionary.TryGetValue(tag);

            if (code != int.MinValue)
                return code;

            return oldTagToCodeDictionary.ContainsKey(tag)
                ? oldTagToCodeDictionary[tag]
                : (int)Language.InvariantCulture;
        }

        private static StringToIntDictionary InitOldTagToCodeDictionaryDocx()
        {
            StringToIntDictionary oldTagToCodeDictionaryDocx = new StringToIntDictionary(false);
            oldTagToCodeDictionaryDocx.Add("ro-MO", (int)Language.RomanianMoldova);
            oldTagToCodeDictionaryDocx.Add("ru-MO", (int)Language.RussianMoldova);
            return oldTagToCodeDictionaryDocx;
        }

        private static StringToIntDictionary InitOldTagToCodeDictionaryWml()
        {
            StringToIntDictionary oldTagToCodeDictionaryWml = new StringToIntDictionary(false);
            oldTagToCodeDictionaryWml.Add("RO-MO", (int)Language.RomanianMoldova);
            oldTagToCodeDictionaryWml.Add("RU-MO", (int)Language.RussianMoldova);
            return oldTagToCodeDictionaryWml;
        }

        private static readonly StringToIntBidirectionalMap gLanguageMapDocx = new StringToIntBidirectionalMap(false);
        private static readonly StringToIntBidirectionalMap gLanguageMapWml = new StringToIntBidirectionalMap(false);
        private static readonly IntToObjDictionary<string> gAdditionalLanguageMapWml = new IntToObjDictionary<string>();
        private static readonly StringToIntDictionary gOldTagToCodeDictionaryDocx = InitOldTagToCodeDictionaryDocx();
        private static readonly StringToIntDictionary gOldTagToCodeDictionaryWml = InitOldTagToCodeDictionaryWml();

        static LocaleConverter()
        {
            gLanguageMapDocx.AddEntry("af", (int)Language.Afrikaans);
            gLanguageMapDocx.AddEntry("af-ZA", (int)Language.AfrikaansSouthAfrica);
            gLanguageMapDocx.AddEntry("sq", (int)Language.Albanian);
            gLanguageMapDocx.AddEntry("sq-AL", (int)Language.AlbanianAlbania);
            gLanguageMapDocx.AddEntry("gsw-FR", (int)Language.Alsatian);
            gLanguageMapDocx.AddEntry("am-ET", (int)Language.Amharic);
            gLanguageMapDocx.AddEntry("ar", (int)Language.Arabic);
            gLanguageMapDocx.AddEntry("ar-DZ", (int)Language.ArabicAlgeria);
            gLanguageMapDocx.AddEntry("ar-BH", (int)Language.ArabicBahrain);
            gLanguageMapDocx.AddEntry("ar-EG", (int)Language.ArabicEgypt);
            gLanguageMapDocx.AddEntry("ar-IQ", (int)Language.ArabicIraq);
            gLanguageMapDocx.AddEntry("ar-JO", (int)Language.ArabicJordan);
            gLanguageMapDocx.AddEntry("ar-KW", (int)Language.ArabicKuwait);
            gLanguageMapDocx.AddEntry("ar-LB", (int)Language.ArabicLebanon);
            gLanguageMapDocx.AddEntry("ar-LY", (int)Language.ArabicLibya);
            gLanguageMapDocx.AddEntry("ar-MA", (int)Language.ArabicMorocco);
            gLanguageMapDocx.AddEntry("ar-OM", (int)Language.ArabicOman);
            gLanguageMapDocx.AddEntry("ar-QA", (int)Language.ArabicQatar);
            gLanguageMapDocx.AddEntry("ar-SA", (int)Language.ArabicSaudiArabia);
            gLanguageMapDocx.AddEntry("ar-SY", (int)Language.ArabicSyria);
            gLanguageMapDocx.AddEntry("ar-TN", (int)Language.ArabicTunisia);
            gLanguageMapDocx.AddEntry("ar-AE", (int)Language.ArabicUAE);
            gLanguageMapDocx.AddEntry("ar-YE", (int)Language.ArabicYemen);
            gLanguageMapDocx.AddEntry("hy", (int)Language.Armenian);
            gLanguageMapDocx.AddEntry("hy-AM", (int)Language.ArmenianArmenia);
            gLanguageMapDocx.AddEntry("as-IN", (int)Language.Assamese);
            gLanguageMapDocx.AddEntry("az", (int)Language.Azeri);
            gLanguageMapDocx.AddEntry("az-Cyrl-AZ", (int)Language.AzeriCyrillic);
            gLanguageMapDocx.AddEntry("az-Latn-AZ", (int)Language.AzeriLatin);
            gLanguageMapDocx.AddEntry("eu", (int)Language.Basque);
            gLanguageMapDocx.AddEntry("eu-ES", (int)Language.BasqueBasque);
            gLanguageMapDocx.AddEntry("be", (int)Language.Belarusian);
            gLanguageMapDocx.AddEntry("be-BY", (int)Language.BelarusianBelarus);
            gLanguageMapDocx.AddEntry("bn-IN", (int)Language.Bengali);
            gLanguageMapDocx.AddEntry("bn-BD", (int)Language.BengaliBangladesh);
            gLanguageMapDocx.AddEntry("bs-Cyrl-BA", (int)Language.BosnianCyrillic);
            gLanguageMapDocx.AddEntry("bs-Latn-BA", (int)Language.BosnianLatin);
            gLanguageMapDocx.AddEntry("br-FR", (int)Language.Breton);
            gLanguageMapDocx.AddEntry("bg", (int)Language.Bulgarian);
            gLanguageMapDocx.AddEntry("bg-BG", (int)Language.BulgarianBulgaria);
            gLanguageMapDocx.AddEntry("my-MM", (int)Language.Burmese);
            gLanguageMapDocx.AddEntry("ca", (int)Language.Catalan);
            gLanguageMapDocx.AddEntry("ca-ES", (int)Language.CatalanCatalan);
            gLanguageMapDocx.AddEntry("chr-US", (int)Language.Cherokee);
            gLanguageMapDocx.AddEntry("zh-HK", (int)Language.ChineseHongKong);
            gLanguageMapDocx.AddEntry("zh-MO", (int)Language.ChineseMacao);
            gLanguageMapDocx.AddEntry("zh-CN", (int)Language.ChineseChina);
            gLanguageMapDocx.AddEntry("zh-Hans", (int)Language.ChineseSimplified);
            gLanguageMapDocx.AddEntry("zh-SG", (int)Language.ChineseSingapore);
            gLanguageMapDocx.AddEntry("zh-TW", (int)Language.ChineseTaiwan);
            gLanguageMapDocx.AddEntry("zh-Hant", (int)Language.ChineseTraditional);
            gLanguageMapDocx.AddEntry("hr", (int)Language.Croatian);
            gLanguageMapDocx.AddEntry("hr-BA", (int)Language.CroatianBozniaAndHerzegovina);
            gLanguageMapDocx.AddEntry("hr-HR", (int)Language.CroatianCroatia);
            gLanguageMapDocx.AddEntry("cs", (int)Language.Czech);
            gLanguageMapDocx.AddEntry("cs-CZ", (int)Language.CzechCzechRepublic);
            gLanguageMapDocx.AddEntry("da", (int)Language.Danish);
            gLanguageMapDocx.AddEntry("da-DK", (int)Language.DanishDenmark);
            gLanguageMapDocx.AddEntry("dv", (int)Language.Divehi);
            gLanguageMapDocx.AddEntry("dv-MV", (int)Language.DivehiMaldives);
            gLanguageMapDocx.AddEntry("nl", (int)Language.Dutch);
            gLanguageMapDocx.AddEntry("nl-NL", (int)Language.DutchNetherlands);
            gLanguageMapDocx.AddEntry("nl-BE", (int)Language.DutchBelgium);
            gLanguageMapDocx.AddEntry("bin-NG", (int)Language.Edo);
            gLanguageMapDocx.AddEntry("en", (int)Language.English);
            gLanguageMapDocx.AddEntry("en-AU", (int)Language.EnglishAustralia);
            gLanguageMapDocx.AddEntry("en-BZ", (int)Language.EnglishBelize);
            gLanguageMapDocx.AddEntry("en-CA", (int)Language.EnglishCanada);
            gLanguageMapDocx.AddEntry("en-029", (int)Language.EnglishCaribbean);
            gLanguageMapDocx.AddEntry("en-HK", (int)Language.EnglishHongKong);
            gLanguageMapDocx.AddEntry("en-IN", (int)Language.EnglishIndia);
            gLanguageMapDocx.AddEntry("en-ID", (int)Language.EnglishIndonesia);
            gLanguageMapDocx.AddEntry("en-IE", (int)Language.EnglishIreland);
            gLanguageMapDocx.AddEntry("en-JM", (int)Language.EnglishJamaica);
            gLanguageMapDocx.AddEntry("en-MY", (int)Language.EnglishMalaysia);
            gLanguageMapDocx.AddEntry("en-NZ", (int)Language.EnglishNewZealand);
            gLanguageMapDocx.AddEntry("en-PH", (int)Language.EnglishPhilippines);
            gLanguageMapDocx.AddEntry("en-SG", (int)Language.EnglishSingapore);
            gLanguageMapDocx.AddEntry("en-ZA", (int)Language.EnglishSouthAfrica);
            gLanguageMapDocx.AddEntry("en-TT", (int)Language.EnglishTrinidadAndTobago);
            gLanguageMapDocx.AddEntry("en-GB", (int)Language.EnglishUK);
            gLanguageMapDocx.AddEntry("en-US", (int)Language.EnglishUS);
            gLanguageMapDocx.AddEntry("en-ZW", (int)Language.EnglishZimbabwe);
            gLanguageMapDocx.AddEntry("et", (int)Language.Estonian);
            gLanguageMapDocx.AddEntry("et-EE", (int)Language.EstonianEstonia);
            gLanguageMapDocx.AddEntry("fo", (int)Language.Faeroese);
            gLanguageMapDocx.AddEntry("fo-FO", (int)Language.FaeroeseFaroeIslands);
            gLanguageMapDocx.AddEntry("fil-PH", (int)Language.Filipino);
            gLanguageMapDocx.AddEntry("fi", (int)Language.Finnish);
            gLanguageMapDocx.AddEntry("fi-FI", (int)Language.FinnishFinland);
            gLanguageMapDocx.AddEntry("fr", (int)Language.French);
            gLanguageMapDocx.AddEntry("fr-BE", (int)Language.FrenchBelgium);
            gLanguageMapDocx.AddEntry("fr-CM", (int)Language.FrenchCameroon);
            gLanguageMapDocx.AddEntry("fr-CA", (int)Language.FrenchCanada);
            gLanguageMapDocx.AddEntry("fr-CD", (int)Language.FrenchCongo);
            gLanguageMapDocx.AddEntry("fr-CI", (int)Language.FrenchCoteDIvoire);
            gLanguageMapDocx.AddEntry("fr-FR", (int)Language.FrenchFrance);
            gLanguageMapDocx.AddEntry("fr-HT", (int)Language.FrenchHaiti);
            gLanguageMapDocx.AddEntry("fr-LU", (int)Language.FrenchLuxembourg);
            gLanguageMapDocx.AddEntry("fr-ML", (int)Language.FrenchMali);
            gLanguageMapDocx.AddEntry("fr-MC", (int)Language.FrenchMonaco);
            gLanguageMapDocx.AddEntry("fr-MA", (int)Language.FrenchMorocco);
            gLanguageMapDocx.AddEntry("fr-RE", (int)Language.FrenchReunion);
            gLanguageMapDocx.AddEntry("fr-SN", (int)Language.FrenchSenegal);
            gLanguageMapDocx.AddEntry("fr-CH", (int)Language.FrenchSwitzerland);
            gLanguageMapDocx.AddEntry("fr-WINDIES", (int)Language.FrenchWestIndies);
            gLanguageMapDocx.AddEntry("fy-NL", (int)Language.FrisianNetherlands);
            gLanguageMapDocx.AddEntry("ff-NG", (int)Language.Fulfulde);
            gLanguageMapDocx.AddEntry("gd-GB", (int)Language.GaelicScotland);
            gLanguageMapDocx.AddEntry("gl", (int)Language.Galician);
            gLanguageMapDocx.AddEntry("gl-ES", (int)Language.GalicianGalician);
            gLanguageMapDocx.AddEntry("ka", (int)Language.Georgian);
            gLanguageMapDocx.AddEntry("ka-GE", (int)Language.GeorgianGeorgia);
            gLanguageMapDocx.AddEntry("de", (int)Language.German);
            gLanguageMapDocx.AddEntry("de-AT", (int)Language.GermanAustria);
            gLanguageMapDocx.AddEntry("de-DE", (int)Language.GermanGermany);
            gLanguageMapDocx.AddEntry("de-LI", (int)Language.GermanLiechtenstein);
            gLanguageMapDocx.AddEntry("de-LU", (int)Language.GermanLuxembourg);
            gLanguageMapDocx.AddEntry("de-CH", (int)Language.GermanSwitzerland);
            gLanguageMapDocx.AddEntry("el", (int)Language.Greek);
            gLanguageMapDocx.AddEntry("el-GR", (int)Language.GreekGreece);
            gLanguageMapDocx.AddEntry("gn-PY", (int)Language.Guarani);
            gLanguageMapDocx.AddEntry("gu", (int)Language.Gujarati);
            gLanguageMapDocx.AddEntry("gu-IN", (int)Language.GujaratiIndia);
            gLanguageMapDocx.AddEntry("ha-Latn-NG", (int)Language.Hausa);
            gLanguageMapDocx.AddEntry("haw-US", (int)Language.Hawaiian);
            gLanguageMapDocx.AddEntry("he", (int)Language.Hebrew);
            gLanguageMapDocx.AddEntry("he-IL", (int)Language.HebrewIsrael);
            gLanguageMapDocx.AddEntry("hi", (int)Language.Hindi);
            gLanguageMapDocx.AddEntry("hi-IN", (int)Language.HindiIndia);
            gLanguageMapDocx.AddEntry("hu", (int)Language.Hungarian);
            gLanguageMapDocx.AddEntry("hu-HU", (int)Language.HungarianHungary);
            gLanguageMapDocx.AddEntry("ibb-NG", (int)Language.Ibibio);
            gLanguageMapDocx.AddEntry("is", (int)Language.Icelandic);
            gLanguageMapDocx.AddEntry("is-IS", (int)Language.IcelandicIceland);
            gLanguageMapDocx.AddEntry("ig-NG", (int)Language.Igbo);
            gLanguageMapDocx.AddEntry("id", (int)Language.Indonesian);
            gLanguageMapDocx.AddEntry("id-ID", (int)Language.IndonesianIndonesia);
            gLanguageMapDocx.AddEntry("iu-Cans-CA", (int)Language.Inuktitut);
            gLanguageMapDocx.AddEntry("iu-Latn-CA", (int)Language.InuktitutLatinCanada);
            gLanguageMapDocx.AddEntry("ga-IE", (int)Language.IrishIreland);
            gLanguageMapDocx.AddEntry("it", (int)Language.Italian);
            gLanguageMapDocx.AddEntry("it-IT", (int)Language.ItalianItaly);
            gLanguageMapDocx.AddEntry("it-CH", (int)Language.ItalianSwitzerland);
            gLanguageMapDocx.AddEntry("ja", (int)Language.Japanese);
            gLanguageMapDocx.AddEntry("ja-JP", (int)Language.JapaneseJapan);
            gLanguageMapDocx.AddEntry("kn", (int)Language.Kannada);
            gLanguageMapDocx.AddEntry("kn-IN", (int)Language.KannadaIndia);
            gLanguageMapDocx.AddEntry("kr-NG", (int)Language.Kanuri);
            gLanguageMapDocx.AddEntry("ks-Deva", (int)Language.Kashmiri);
            gLanguageMapDocx.AddEntry("ks-Arab", (int)Language.KashmiriArabic);
            gLanguageMapDocx.AddEntry("kk", (int)Language.Kazakh);
            gLanguageMapDocx.AddEntry("kk-KZ", (int)Language.KazakhKazakhstan);
            gLanguageMapDocx.AddEntry("km-KH", (int)Language.Khmer);
            gLanguageMapDocx.AddEntry("sw", (int)Language.Kiswahili);
            gLanguageMapDocx.AddEntry("sw-KE", (int)Language.KiswahiliKenya);
            gLanguageMapDocx.AddEntry("kok", (int)Language.Konkani);
            gLanguageMapDocx.AddEntry("kok-IN", (int)Language.KonkaniIndia);
            gLanguageMapDocx.AddEntry("ko", (int)Language.Korean);
            gLanguageMapDocx.AddEntry("ko-KR", (int)Language.KoreanKorea);
            gLanguageMapDocx.AddEntry("ky", (int)Language.Kyrgyz);
            gLanguageMapDocx.AddEntry("ky-KG", (int)Language.KyrgyzKyrgyzstan);
            gLanguageMapDocx.AddEntry("lo-LA", (int)Language.Lao);
            gLanguageMapDocx.AddEntry("la-Latn", (int)Language.Latin);
            gLanguageMapDocx.AddEntry("lv", (int)Language.Latvian);
            gLanguageMapDocx.AddEntry("lv-LV", (int)Language.LatvianLatvia);
            gLanguageMapDocx.AddEntry("lt", (int)Language.Lithuanian);
            gLanguageMapDocx.AddEntry("lt-LT", (int)Language.LithuanianLithuania);
            gLanguageMapDocx.AddEntry("lb-LU", (int)Language.LuxembougishLuxemburg);
            gLanguageMapDocx.AddEntry("mk", (int)Language.Macedonian);
            gLanguageMapDocx.AddEntry("mk-MK", (int)Language.MacedonianFYROM);
            gLanguageMapDocx.AddEntry("ms", (int)Language.Malay);
            gLanguageMapDocx.AddEntry("ms-MY", (int)Language.MalayMalaysia);
            gLanguageMapDocx.AddEntry("ms-BN", (int)Language.MalayBruneiDarussalam);
            gLanguageMapDocx.AddEntry("ml-IN", (int)Language.Malayalam);
            gLanguageMapDocx.AddEntry("mt-MT", (int)Language.Maltese);
            gLanguageMapDocx.AddEntry("mni-IN", (int)Language.Manipuri);
            gLanguageMapDocx.AddEntry("mi-NZ", (int)Language.Maori);
            gLanguageMapDocx.AddEntry("arn-CL", (int)Language.MapudungunChile);
            gLanguageMapDocx.AddEntry("mr", (int)Language.Marathi);
            gLanguageMapDocx.AddEntry("mr-IN", (int)Language.MarathiIndia);
            gLanguageMapDocx.AddEntry("moh-CA", (int)Language.MohawkMohawk);
            gLanguageMapDocx.AddEntry("mn", (int)Language.Mongolian);
            gLanguageMapDocx.AddEntry("mn-MN", (int)Language.MongolianCyrillic);
            gLanguageMapDocx.AddEntry("mn-Mong-CN", (int)Language.MongolianMongolian);
            gLanguageMapDocx.AddEntry("ne-NP", (int)Language.Nepali);
            gLanguageMapDocx.AddEntry("ne-IN", (int)Language.NepaliIndia);
            gLanguageMapDocx.AddEntry("no", (int)Language.Norwegian);
            gLanguageMapDocx.AddEntry("nb-NO", (int)Language.NorwegianBokmal);
            gLanguageMapDocx.AddEntry("nn-NO", (int)Language.NorwegianNynorsk);
            gLanguageMapDocx.AddEntry("or-IN", (int)Language.Oriya);
            gLanguageMapDocx.AddEntry("om-Ethi-ET", (int)Language.Oromo);
            gLanguageMapDocx.AddEntry("pap-AN", (int)Language.Papiamentu);
            gLanguageMapDocx.AddEntry("ps-AF", (int)Language.Pashto);
            gLanguageMapDocx.AddEntry("fa", (int)Language.Persian);
            gLanguageMapDocx.AddEntry("fa-IR", (int)Language.PersianIran);
            gLanguageMapDocx.AddEntry("pl", (int)Language.Polish);
            gLanguageMapDocx.AddEntry("pl-PL", (int)Language.PolishPoland);
            gLanguageMapDocx.AddEntry("pt", (int)Language.Portuguese);
            gLanguageMapDocx.AddEntry("pt-BR", (int)Language.PortugueseBrazil);
            gLanguageMapDocx.AddEntry("pt-PT", (int)Language.PortuguesePortugal);
            gLanguageMapDocx.AddEntry("pa", (int)Language.Punjabi);
            gLanguageMapDocx.AddEntry("pa-IN", (int)Language.PunjabiIndia);
            gLanguageMapDocx.AddEntry("pa-PK", (int)Language.PunjabiPakistan);
            gLanguageMapDocx.AddEntry("quz-BO", (int)Language.QuechuaBolivia);
            gLanguageMapDocx.AddEntry("quz-EC", (int)Language.QuechuaEcuador);
            gLanguageMapDocx.AddEntry("quz-PE", (int)Language.QuechuaPeru);
            gLanguageMapDocx.AddEntry("ro", (int)Language.Romanian);
            gLanguageMapDocx.AddEntry("ro-MD", (int)Language.RomanianMoldova);
            gLanguageMapDocx.AddEntry("ro-RO", (int)Language.RomanianRomania);
            gLanguageMapDocx.AddEntry("rm-CH", (int)Language.RomanshSwitzerland);
            gLanguageMapDocx.AddEntry("ru", (int)Language.Russian);
            gLanguageMapDocx.AddEntry("ru-MD", (int)Language.RussianMoldova);
            gLanguageMapDocx.AddEntry("ru-RU", (int)Language.RussianRussia);
            gLanguageMapDocx.AddEntry("smn-FI", (int)Language.SamiInariFinland);
            gLanguageMapDocx.AddEntry("smj-NO", (int)Language.SamiLuleNorway);
            gLanguageMapDocx.AddEntry("smj-SE", (int)Language.SamiLuleSweden);
            gLanguageMapDocx.AddEntry("se-FI", (int)Language.SamiNorthernFinland);
            gLanguageMapDocx.AddEntry("se-NO", (int)Language.SamiNorthernNorway);
            gLanguageMapDocx.AddEntry("se-SE", (int)Language.SamiNothernSweden);
            gLanguageMapDocx.AddEntry("sms-FI", (int)Language.SamiSkoltFinland);
            gLanguageMapDocx.AddEntry("sma-NO", (int)Language.SamiSouthernNorway);
            gLanguageMapDocx.AddEntry("sma-SE", (int)Language.SamiSouthernSweden);
            gLanguageMapDocx.AddEntry("sa", (int)Language.Sanskrit);
            gLanguageMapDocx.AddEntry("sa-IN", (int)Language.SanskritIndia);
            gLanguageMapDocx.AddEntry("nso-ZA", (int)Language.Sepedi);
            gLanguageMapDocx.AddEntry("sr", (int)Language.Serbian);
            gLanguageMapDocx.AddEntry("sr-Latn-RS", (int)Language.SerbianLatin);
            gLanguageMapDocx.AddEntry("sr-Cyrl-BA", (int)Language.SerbianCyrillicBosniaAndHerzegovina);
            gLanguageMapDocx.AddEntry("sr-Cyrl-CS", (int)Language.SerbianCyrillicSerbiaAndMontenegro);
            gLanguageMapDocx.AddEntry("sr-Latn-BA", (int)Language.SerbianLatinBosniaAndHerzegovina);
            gLanguageMapDocx.AddEntry("sr-Latn-CS", (int)Language.SerbianLatinSerbiaAndMontenegro);
            gLanguageMapDocx.AddEntry("sd-Arab-PK", (int)Language.Sindhi);
            gLanguageMapDocx.AddEntry("sd-Deva-IN", (int)Language.SindhiDevanagaric);
            gLanguageMapDocx.AddEntry("si-LK", (int)Language.Sinhalese);
            gLanguageMapDocx.AddEntry("sk", (int)Language.Slovak);
            gLanguageMapDocx.AddEntry("sk-SK", (int)Language.SlovakSlovakia);
            gLanguageMapDocx.AddEntry("sl", (int)Language.Slovenian);
            gLanguageMapDocx.AddEntry("sl-SI", (int)Language.SlovenianSlovenia);
            gLanguageMapDocx.AddEntry("so-SO", (int)Language.Somali);
            gLanguageMapDocx.AddEntry("hsb-DE", (int)Language.Sorbian);
            gLanguageMapDocx.AddEntry("es", (int)Language.Spanish);
            gLanguageMapDocx.AddEntry("es-AR", (int)Language.SpanishArgentina);
            gLanguageMapDocx.AddEntry("es-BO", (int)Language.SpanishBolivia);
            gLanguageMapDocx.AddEntry("es-CL", (int)Language.SpanishChile);
            gLanguageMapDocx.AddEntry("es-CO", (int)Language.SpanishColombia);
            gLanguageMapDocx.AddEntry("es-CR", (int)Language.SpanishCostaRica);
            gLanguageMapDocx.AddEntry("es-DO", (int)Language.SpanishDominicanRepublic);
            gLanguageMapDocx.AddEntry("es-EC", (int)Language.SpanishEcuador);
            gLanguageMapDocx.AddEntry("es-SV", (int)Language.SpanishElSalvador);
            gLanguageMapDocx.AddEntry("es-GT", (int)Language.SpanishGuatemala);
            gLanguageMapDocx.AddEntry("es-HN", (int)Language.SpanishHonduras);
            gLanguageMapDocx.AddEntry("es-MX", (int)Language.SpanishMexico);
            gLanguageMapDocx.AddEntry("es-NI", (int)Language.SpanishNicaragua);
            gLanguageMapDocx.AddEntry("es-PA", (int)Language.SpanishPanama);
            gLanguageMapDocx.AddEntry("es-PY", (int)Language.SpanishParaguay);
            gLanguageMapDocx.AddEntry("es-PE", (int)Language.SpanishPeru);
            gLanguageMapDocx.AddEntry("es-PR", (int)Language.SpanishPuertoRico);
            gLanguageMapDocx.AddEntry("es-ES", (int)Language.SpanishSpainModernSort);
            gLanguageMapDocx.AddEntry("es-ES_tradnl", (int)Language.SpanishSpainTraditionalSort);
            gLanguageMapDocx.AddEntry("es-UY", (int)Language.SpanishUruguay);
            gLanguageMapDocx.AddEntry("es-VE", (int)Language.SpanishVenezuela);
            gLanguageMapDocx.AddEntry("st-ZA", (int)Language.Sutu);
            gLanguageMapDocx.AddEntry("sv", (int)Language.Swedish);
            gLanguageMapDocx.AddEntry("sv-FI", (int)Language.SwedishFinland);
            gLanguageMapDocx.AddEntry("sv-SE", (int)Language.SwedishSweden);
            gLanguageMapDocx.AddEntry("syr", (int)Language.Syriac);
            gLanguageMapDocx.AddEntry("syr-SY", (int)Language.SyriacSyria);
            gLanguageMapDocx.AddEntry("tg-Cyrl-TJ", (int)Language.Tajik);
            gLanguageMapDocx.AddEntry("tzm-Arab-MA", (int)Language.Tamazight);
            gLanguageMapDocx.AddEntry("tzm-Latn-DZ", (int)Language.TamazightLatin);
            gLanguageMapDocx.AddEntry("ta", (int)Language.Tamil);
            gLanguageMapDocx.AddEntry("ta-IN", (int)Language.TamilIndia);
            gLanguageMapDocx.AddEntry("tt", (int)Language.Tatar);
            gLanguageMapDocx.AddEntry("tt-RU", (int)Language.TatarRussia);
            gLanguageMapDocx.AddEntry("te", (int)Language.Telugu);
            gLanguageMapDocx.AddEntry("te-IN", (int)Language.TeluguIndia);
            gLanguageMapDocx.AddEntry("th", (int)Language.Thai);
            gLanguageMapDocx.AddEntry("th-TH", (int)Language.ThaiThailand);
            gLanguageMapDocx.AddEntry("bo-BT", (int)Language.TibetanButan);
            gLanguageMapDocx.AddEntry("bo-CN", (int)Language.TibetanChina);
            gLanguageMapDocx.AddEntry("ti-ER", (int)Language.TigrignaEritrea);
            gLanguageMapDocx.AddEntry("ti-ET", (int)Language.TigrignaEthiopia);
            gLanguageMapDocx.AddEntry("ts-ZA", (int)Language.Tsonga);
            gLanguageMapDocx.AddEntry("tn-ZA", (int)Language.Tswana);
            gLanguageMapDocx.AddEntry("tr", (int)Language.Turkish);
            gLanguageMapDocx.AddEntry("tr-TR", (int)Language.TurkishTurkey);
            gLanguageMapDocx.AddEntry("tk-TM", (int)Language.Turkmen);
            gLanguageMapDocx.AddEntry("uk", (int)Language.Ukrainian);
            gLanguageMapDocx.AddEntry("uk-UA", (int)Language.UkrainianUkraine);
            gLanguageMapDocx.AddEntry("ur", (int)Language.Urdu);
            gLanguageMapDocx.AddEntry("ur-PK", (int)Language.UrduPakistan);
            gLanguageMapDocx.AddEntry("ur-IN", (int)Language.UrduIndian);
            gLanguageMapDocx.AddEntry("uz", (int)Language.Uzbek);
            gLanguageMapDocx.AddEntry("uz-Cyrl-UZ", (int)Language.UzbekCyrillic);
            gLanguageMapDocx.AddEntry("uz-Latn-UZ", (int)Language.UzbekLatin);
            gLanguageMapDocx.AddEntry("ve-ZA", (int)Language.Venda);
            gLanguageMapDocx.AddEntry("vi", (int)Language.Vietnamese);
            gLanguageMapDocx.AddEntry("vi-VN", (int)Language.VietnameseVietnam);
            gLanguageMapDocx.AddEntry("cy-GB", (int)Language.Welsh);
            gLanguageMapDocx.AddEntry("xh-ZA", (int)Language.Xhosa);
            gLanguageMapDocx.AddEntry("ii-CN", (int)Language.Yi);
            gLanguageMapDocx.AddEntry("yi-Hebr", (int)Language.Yiddish);
            gLanguageMapDocx.AddEntry("yo-NG", (int)Language.Yoruba);
            gLanguageMapDocx.AddEntry("zu-ZA", (int)Language.Zulu);

            AdditionalLanguageToMapWml("AF", (int)Language.Afrikaans);
            gLanguageMapWml.AddEntry("AF", (int)Language.AfrikaansSouthAfrica);
            AdditionalLanguageToMapWml("SQ", (int)Language.Albanian);
            gLanguageMapWml.AddEntry("SQ", (int)Language.AlbanianAlbania);
            gLanguageMapWml.AddEntry("GSW-FR", (int)Language.Alsatian);
            gLanguageMapWml.AddEntry("AMH", (int)Language.Amharic);
            gLanguageMapWml.AddEntry("AR-DZ", (int)Language.ArabicAlgeria);
            gLanguageMapWml.AddEntry("AR-BH", (int)Language.ArabicBahrain);
            gLanguageMapWml.AddEntry("AR-EG", (int)Language.ArabicEgypt);
            gLanguageMapWml.AddEntry("AR-IQ", (int)Language.ArabicIraq);
            gLanguageMapWml.AddEntry("AR-JO", (int)Language.ArabicJordan);
            gLanguageMapWml.AddEntry("AR-KW", (int)Language.ArabicKuwait);
            gLanguageMapWml.AddEntry("AR-LB", (int)Language.ArabicLebanon);
            gLanguageMapWml.AddEntry("AR-LY", (int)Language.ArabicLibya);
            gLanguageMapWml.AddEntry("AR-MA", (int)Language.ArabicMorocco);
            gLanguageMapWml.AddEntry("AR-OM", (int)Language.ArabicOman);
            gLanguageMapWml.AddEntry("AR-QA", (int)Language.ArabicQatar);
            AdditionalLanguageToMapWml("AR-SA", (int)Language.Arabic);
            gLanguageMapWml.AddEntry("AR-SA", (int)Language.ArabicSaudiArabia);
            gLanguageMapWml.AddEntry("AR-SY", (int)Language.ArabicSyria);
            gLanguageMapWml.AddEntry("AR-TN", (int)Language.ArabicTunisia);
            gLanguageMapWml.AddEntry("AR-AE", (int)Language.ArabicUAE);
            gLanguageMapWml.AddEntry("AR-YE", (int)Language.ArabicYemen);
            AdditionalLanguageToMapWml("HY", (int)Language.Armenian);
            gLanguageMapWml.AddEntry("HY", (int)Language.ArmenianArmenia);
            gLanguageMapWml.AddEntry("AS", (int)Language.Assamese);
            gLanguageMapWml.AddEntry("AZ-CYR", (int)Language.AzeriCyrillic);
            AdditionalLanguageToMapWml("AZ-LATIN", (int)Language.Azeri);
            gLanguageMapWml.AddEntry("AZ-LATIN", (int)Language.AzeriLatin);
            AdditionalLanguageToMapWml("EU", (int)Language.Basque);
            gLanguageMapWml.AddEntry("EU", (int)Language.BasqueBasque);
            AdditionalLanguageToMapWml("BE", (int)Language.Belarusian);
            gLanguageMapWml.AddEntry("BE", (int)Language.BelarusianBelarus);
            gLanguageMapWml.AddEntry("BN", (int)Language.Bengali);
            gLanguageMapWml.AddEntry("0845", (int)Language.BengaliBangladesh);
            gLanguageMapWml.AddEntry("201A", (int)Language.BosnianCyrillic);
            gLanguageMapWml.AddEntry("141A", (int)Language.BosnianLatin);
            gLanguageMapWml.AddEntry("BR-FR", (int)Language.Breton);
            AdditionalLanguageToMapWml("BG", (int)Language.Bulgarian);
            gLanguageMapWml.AddEntry("BG", (int)Language.BulgarianBulgaria);
            gLanguageMapWml.AddEntry("MY", (int)Language.Burmese);
            AdditionalLanguageToMapWml("CA", (int)Language.Catalan);
            gLanguageMapWml.AddEntry("CA", (int)Language.CatalanCatalan);
            gLanguageMapWml.AddEntry("CHR", (int)Language.Cherokee);
            gLanguageMapWml.AddEntry("ZH-HK", (int)Language.ChineseHongKong);
            gLanguageMapWml.AddEntry("ZH-MO", (int)Language.ChineseMacao);
            gLanguageMapWml.AddEntry("ZH-CN", (int)Language.ChineseChina);
            gLanguageMapWml.AddEntry("ZH-SG", (int)Language.ChineseSingapore);
            gLanguageMapWml.AddEntry("ZH-TW", (int)Language.ChineseTaiwan);
            gLanguageMapWml.AddEntry("SH", (int)Language.CroatianBozniaAndHerzegovina);
            AdditionalLanguageToMapWml("HR", (int)Language.Croatian);
            gLanguageMapWml.AddEntry("HR", (int)Language.CroatianCroatia);
            AdditionalLanguageToMapWml("CS", (int)Language.Czech);
            gLanguageMapWml.AddEntry("CS", (int)Language.CzechCzechRepublic);
            AdditionalLanguageToMapWml("DA", (int)Language.Danish);
            gLanguageMapWml.AddEntry("DA", (int)Language.DanishDenmark);
            AdditionalLanguageToMapWml("DIV", (int)Language.Divehi);
            gLanguageMapWml.AddEntry("DIV", (int)Language.DivehiMaldives);
            AdditionalLanguageToMapWml("NL", (int)Language.Dutch);
            gLanguageMapWml.AddEntry("NL", (int)Language.DutchNetherlands);
            gLanguageMapWml.AddEntry("NL-BE", (int)Language.DutchBelgium);
            gLanguageMapWml.AddEntry("0466", (int)Language.Edo);
            gLanguageMapWml.AddEntry("EN-AU", (int)Language.EnglishAustralia);
            gLanguageMapWml.AddEntry("EN-BZ", (int)Language.EnglishBelize);
            gLanguageMapWml.AddEntry("EN-CA", (int)Language.EnglishCanada);
            gLanguageMapWml.AddEntry("EN-CARRIBEAN", (int)Language.EnglishCaribbean);
            gLanguageMapWml.AddEntry("EN-HK", (int)Language.EnglishHongKong);
            gLanguageMapWml.AddEntry("EN-IN", (int)Language.EnglishIndia);
            gLanguageMapWml.AddEntry("EN-ID", (int)Language.EnglishIndonesia);
            gLanguageMapWml.AddEntry("EN-IE", (int)Language.EnglishIreland);
            gLanguageMapWml.AddEntry("EN-JM", (int)Language.EnglishJamaica);
            gLanguageMapWml.AddEntry("EN-MY", (int)Language.EnglishMalaysia);
            gLanguageMapWml.AddEntry("EN-NZ", (int)Language.EnglishNewZealand);
            gLanguageMapWml.AddEntry("EN-PH", (int)Language.EnglishPhilippines);
            gLanguageMapWml.AddEntry("EN-SG", (int)Language.EnglishSingapore);
            gLanguageMapWml.AddEntry("EN-ZA", (int)Language.EnglishSouthAfrica);
            gLanguageMapWml.AddEntry("EN-TT", (int)Language.EnglishTrinidadAndTobago);
            gLanguageMapWml.AddEntry("EN-GB", (int)Language.EnglishUK);
            AdditionalLanguageToMapWml("EN-US", (int)Language.English);
            gLanguageMapWml.AddEntry("EN-US", (int)Language.EnglishUS);
            gLanguageMapWml.AddEntry("EN-ZW", (int)Language.EnglishZimbabwe);
            AdditionalLanguageToMapWml("ET", (int)Language.Estonian);
            gLanguageMapWml.AddEntry("ET", (int)Language.EstonianEstonia);
            AdditionalLanguageToMapWml("FO", (int)Language.Faeroese);
            gLanguageMapWml.AddEntry("FO", (int)Language.FaeroeseFaroeIslands);
            gLanguageMapWml.AddEntry("0464", (int)Language.Filipino);
            AdditionalLanguageToMapWml("FI", (int)Language.Finnish);
            gLanguageMapWml.AddEntry("FI", (int)Language.FinnishFinland);
            gLanguageMapWml.AddEntry("FR-BE", (int)Language.FrenchBelgium);
            gLanguageMapWml.AddEntry("FR-CM", (int)Language.FrenchCameroon);
            gLanguageMapWml.AddEntry("FR-CA", (int)Language.FrenchCanada);
            gLanguageMapWml.AddEntry("FR-CD", (int)Language.FrenchCongo);
            gLanguageMapWml.AddEntry("FR-CI", (int)Language.FrenchCoteDIvoire);
            AdditionalLanguageToMapWml("FR", (int)Language.French);
            gLanguageMapWml.AddEntry("FR", (int)Language.FrenchFrance);
            gLanguageMapWml.AddEntry("FR-HT", (int)Language.FrenchHaiti);
            gLanguageMapWml.AddEntry("FR-LU", (int)Language.FrenchLuxembourg);
            gLanguageMapWml.AddEntry("FR-ML", (int)Language.FrenchMali);
            gLanguageMapWml.AddEntry("FR-MC", (int)Language.FrenchMonaco);
            gLanguageMapWml.AddEntry("FR-MA", (int)Language.FrenchMorocco);
            gLanguageMapWml.AddEntry("FR-RE", (int)Language.FrenchReunion);
            gLanguageMapWml.AddEntry("FR-SN", (int)Language.FrenchSenegal);
            gLanguageMapWml.AddEntry("FR-CH", (int)Language.FrenchSwitzerland);
            gLanguageMapWml.AddEntry("FR-WINDIES", (int)Language.FrenchWestIndies);
            gLanguageMapWml.AddEntry("FY", (int)Language.FrisianNetherlands);
            gLanguageMapWml.AddEntry("0467", (int)Language.Fulfulde);
            gLanguageMapWml.AddEntry("GD", (int)Language.GaelicScotland);
            AdditionalLanguageToMapWml("GL", (int)Language.Galician);
            gLanguageMapWml.AddEntry("GL", (int)Language.GalicianGalician);
            AdditionalLanguageToMapWml("GEO/KAT", (int)Language.Georgian);
            gLanguageMapWml.AddEntry("GEO/KAT", (int)Language.GeorgianGeorgia);
            gLanguageMapWml.AddEntry("DE-AT", (int)Language.GermanAustria);
            AdditionalLanguageToMapWml("DE", (int)Language.German);
            gLanguageMapWml.AddEntry("DE", (int)Language.GermanGermany);
            gLanguageMapWml.AddEntry("DE-LI", (int)Language.GermanLiechtenstein);
            gLanguageMapWml.AddEntry("DE-LU", (int)Language.GermanLuxembourg);
            gLanguageMapWml.AddEntry("DE-CH", (int)Language.GermanSwitzerland);
            AdditionalLanguageToMapWml("EL", (int)Language.Greek);
            gLanguageMapWml.AddEntry("EL", (int)Language.GreekGreece);
            gLanguageMapWml.AddEntry("GN", (int)Language.Guarani);
            AdditionalLanguageToMapWml("GU", (int)Language.Gujarati);
            gLanguageMapWml.AddEntry("GU", (int)Language.GujaratiIndia);
            gLanguageMapWml.AddEntry("HA", (int)Language.Hausa);
            gLanguageMapWml.AddEntry("0475", (int)Language.Hawaiian);
            AdditionalLanguageToMapWml("HE", (int)Language.Hebrew);
            gLanguageMapWml.AddEntry("HE", (int)Language.HebrewIsrael);
            AdditionalLanguageToMapWml("HI", (int)Language.Hindi);
            gLanguageMapWml.AddEntry("HI", (int)Language.HindiIndia);
            AdditionalLanguageToMapWml("HU", (int)Language.Hungarian);
            gLanguageMapWml.AddEntry("HU", (int)Language.HungarianHungary);
            gLanguageMapWml.AddEntry("0469", (int)Language.Ibibio);
            AdditionalLanguageToMapWml("IS", (int)Language.Icelandic);
            gLanguageMapWml.AddEntry("IS", (int)Language.IcelandicIceland);
            gLanguageMapWml.AddEntry("0470", (int)Language.Igbo);
            AdditionalLanguageToMapWml("IN", (int)Language.Indonesian);
            gLanguageMapWml.AddEntry("IN", (int)Language.IndonesianIndonesia);
            gLanguageMapWml.AddEntry("IKU", (int)Language.Inuktitut);
            gLanguageMapWml.AddEntry("085D", (int)Language.InuktitutLatinCanada);
            gLanguageMapWml.AddEntry("GA", (int)Language.IrishIreland);
            AdditionalLanguageToMapWml("IT", (int)Language.Italian);
            gLanguageMapWml.AddEntry("IT", (int)Language.ItalianItaly);
            gLanguageMapWml.AddEntry("IT-CH", (int)Language.ItalianSwitzerland);
            AdditionalLanguageToMapWml("JA", (int)Language.Japanese);
            gLanguageMapWml.AddEntry("JA", (int)Language.JapaneseJapan);
            AdditionalLanguageToMapWml("KN", (int)Language.Kannada);
            gLanguageMapWml.AddEntry("KN", (int)Language.KannadaIndia);
            gLanguageMapWml.AddEntry("0471", (int)Language.Kanuri);
            gLanguageMapWml.AddEntry("KS-IN", (int)Language.Kashmiri);
            gLanguageMapWml.AddEntry("KS", (int)Language.KashmiriArabic);
            AdditionalLanguageToMapWml("KZ", (int)Language.Kazakh);
            gLanguageMapWml.AddEntry("KZ", (int)Language.KazakhKazakhstan);
            gLanguageMapWml.AddEntry("KHM", (int)Language.Khmer);
            AdditionalLanguageToMapWml("SW", (int)Language.Kiswahili);
            gLanguageMapWml.AddEntry("SW", (int)Language.KiswahiliKenya);
            AdditionalLanguageToMapWml("KOK", (int)Language.Konkani);
            gLanguageMapWml.AddEntry("KOK", (int)Language.KonkaniIndia);
            AdditionalLanguageToMapWml("KO", (int)Language.Korean);
            gLanguageMapWml.AddEntry("KO", (int)Language.KoreanKorea);
            AdditionalLanguageToMapWml("KY", (int)Language.Kyrgyz);
            gLanguageMapWml.AddEntry("KY", (int)Language.KyrgyzKyrgyzstan);
            gLanguageMapWml.AddEntry("LAO", (int)Language.Lao);
            gLanguageMapWml.AddEntry("LA", (int)Language.Latin);
            AdditionalLanguageToMapWml("LV", (int)Language.Latvian);
            gLanguageMapWml.AddEntry("LV", (int)Language.LatvianLatvia);
            AdditionalLanguageToMapWml("LT", (int)Language.Lithuanian);
            gLanguageMapWml.AddEntry("LT", (int)Language.LithuanianLithuania);
            gLanguageMapWml.AddEntry("046E", (int)Language.LuxembougishLuxemburg);
            AdditionalLanguageToMapWml("MK", (int)Language.Macedonian);
            gLanguageMapWml.AddEntry("MK", (int)Language.MacedonianFYROM);
            AdditionalLanguageToMapWml("MS", (int)Language.Malay);
            gLanguageMapWml.AddEntry("MS", (int)Language.MalayMalaysia);
            gLanguageMapWml.AddEntry("MS-BN", (int)Language.MalayBruneiDarussalam);
            gLanguageMapWml.AddEntry("ML", (int)Language.Malayalam);
            gLanguageMapWml.AddEntry("MT", (int)Language.Maltese);
            gLanguageMapWml.AddEntry("MNI", (int)Language.Manipuri);
            gLanguageMapWml.AddEntry("0481", (int)Language.Maori);
            gLanguageMapWml.AddEntry("047A", (int)Language.MapudungunChile);
            AdditionalLanguageToMapWml("MR", (int)Language.Marathi);
            gLanguageMapWml.AddEntry("MR", (int)Language.MarathiIndia);
            gLanguageMapWml.AddEntry("047C", (int)Language.MohawkMohawk);
            AdditionalLanguageToMapWml("MN", (int)Language.Mongolian);
            gLanguageMapWml.AddEntry("MN", (int)Language.MongolianCyrillic);
            gLanguageMapWml.AddEntry("MN-MN", (int)Language.MongolianMongolian);
            gLanguageMapWml.AddEntry("NE", (int)Language.Nepali);
            gLanguageMapWml.AddEntry("NE-IN", (int)Language.NepaliIndia);
            AdditionalLanguageToMapWml("NO-BOK", (int)Language.Norwegian);
            gLanguageMapWml.AddEntry("NO-BOK", (int)Language.NorwegianBokmal);
            gLanguageMapWml.AddEntry("NO-NYN", (int)Language.NorwegianNynorsk);
            gLanguageMapWml.AddEntry("OR", (int)Language.Oriya);
            gLanguageMapWml.AddEntry("OM", (int)Language.Oromo);
            gLanguageMapWml.AddEntry("0479", (int)Language.Papiamentu);
            gLanguageMapWml.AddEntry("0463", (int)Language.Pashto);
            AdditionalLanguageToMapWml("FA", (int)Language.Persian);
            gLanguageMapWml.AddEntry("FA", (int)Language.PersianIran);
            AdditionalLanguageToMapWml("PL", (int)Language.Polish);
            gLanguageMapWml.AddEntry("PL", (int)Language.PolishPoland);
            AdditionalLanguageToMapWml("PT-BR", (int)Language.Portuguese);
            gLanguageMapWml.AddEntry("PT-BR", (int)Language.PortugueseBrazil);
            gLanguageMapWml.AddEntry("PT", (int)Language.PortuguesePortugal);
            AdditionalLanguageToMapWml("PA", (int)Language.Punjabi);
            gLanguageMapWml.AddEntry("PA", (int)Language.PunjabiIndia);
            gLanguageMapWml.AddEntry("0846", (int)Language.PunjabiPakistan);
            gLanguageMapWml.AddEntry("046B", (int)Language.QuechuaBolivia);
            gLanguageMapWml.AddEntry("086B", (int)Language.QuechuaEcuador);
            gLanguageMapWml.AddEntry("0C6B", (int)Language.QuechuaPeru);
            gLanguageMapWml.AddEntry("RO-MD", (int)Language.RomanianMoldova);
            AdditionalLanguageToMapWml("RO", (int)Language.Romanian);
            gLanguageMapWml.AddEntry("RO", (int)Language.RomanianRomania);
            gLanguageMapWml.AddEntry("RM", (int)Language.RomanshSwitzerland);
            gLanguageMapWml.AddEntry("RU-MD", (int)Language.RussianMoldova);
            AdditionalLanguageToMapWml("RU", (int)Language.Russian);
            gLanguageMapWml.AddEntry("RU", (int)Language.RussianRussia);
            gLanguageMapWml.AddEntry("243B", (int)Language.SamiInariFinland);
            gLanguageMapWml.AddEntry("103B", (int)Language.SamiLuleNorway);
            gLanguageMapWml.AddEntry("143B", (int)Language.SamiLuleSweden);
            gLanguageMapWml.AddEntry("0C3B", (int)Language.SamiNorthernFinland);
            gLanguageMapWml.AddEntry("I-SAMI-NO", (int)Language.SamiNorthernNorway);
            gLanguageMapWml.AddEntry("083B", (int)Language.SamiNothernSweden);
            gLanguageMapWml.AddEntry("203B", (int)Language.SamiSkoltFinland);
            gLanguageMapWml.AddEntry("183B", (int)Language.SamiSouthernNorway);
            gLanguageMapWml.AddEntry("1C3B", (int)Language.SamiSouthernSweden);
            AdditionalLanguageToMapWml("SA", (int)Language.Sanskrit);
            gLanguageMapWml.AddEntry("SA", (int)Language.SanskritIndia);
            gLanguageMapWml.AddEntry("046C", (int)Language.Sepedi);
            gLanguageMapWml.AddEntry("1C1A", (int)Language.SerbianCyrillicBosniaAndHerzegovina);
            gLanguageMapWml.AddEntry("SR-CYR", (int)Language.SerbianCyrillicSerbiaAndMontenegro);
            gLanguageMapWml.AddEntry("181A", (int)Language.SerbianLatinBosniaAndHerzegovina);
            AdditionalLanguageToMapWml("SR", (int)Language.Serbian);
            gLanguageMapWml.AddEntry("SR", (int)Language.SerbianLatinSerbiaAndMontenegro);
            gLanguageMapWml.AddEntry("0859", (int)Language.Sindhi);
            gLanguageMapWml.AddEntry("SD", (int)Language.SindhiDevanagaric);
            gLanguageMapWml.AddEntry("045B", (int)Language.Sinhalese);
            AdditionalLanguageToMapWml("SK", (int)Language.Slovak);
            gLanguageMapWml.AddEntry("SK", (int)Language.SlovakSlovakia);
            AdditionalLanguageToMapWml("SL", (int)Language.Slovenian);
            gLanguageMapWml.AddEntry("SL", (int)Language.SlovenianSlovenia);
            gLanguageMapWml.AddEntry("SO", (int)Language.Somali);
            gLanguageMapWml.AddEntry("SB", (int)Language.Sorbian);
            gLanguageMapWml.AddEntry("ES-AR", (int)Language.SpanishArgentina);
            gLanguageMapWml.AddEntry("ES-BO", (int)Language.SpanishBolivia);
            gLanguageMapWml.AddEntry("ES-CL", (int)Language.SpanishChile);
            gLanguageMapWml.AddEntry("ES-CO", (int)Language.SpanishColombia);
            gLanguageMapWml.AddEntry("ES-CR", (int)Language.SpanishCostaRica);
            gLanguageMapWml.AddEntry("ES-DO", (int)Language.SpanishDominicanRepublic);
            gLanguageMapWml.AddEntry("ES-EC", (int)Language.SpanishEcuador);
            gLanguageMapWml.AddEntry("ES-SV", (int)Language.SpanishElSalvador);
            gLanguageMapWml.AddEntry("ES-GT", (int)Language.SpanishGuatemala);
            gLanguageMapWml.AddEntry("ES-HN", (int)Language.SpanishHonduras);
            gLanguageMapWml.AddEntry("ES-MX", (int)Language.SpanishMexico);
            gLanguageMapWml.AddEntry("ES-NI", (int)Language.SpanishNicaragua);
            gLanguageMapWml.AddEntry("ES-PA", (int)Language.SpanishPanama);
            gLanguageMapWml.AddEntry("ES-PY", (int)Language.SpanishParaguay);
            gLanguageMapWml.AddEntry("ES-PE", (int)Language.SpanishPeru);
            gLanguageMapWml.AddEntry("ES-PR", (int)Language.SpanishPuertoRico);
            AdditionalLanguageToMapWml("ES", (int)Language.Spanish);
            gLanguageMapWml.AddEntry("ES", (int)Language.SpanishSpainModernSort);
            gLanguageMapWml.AddEntry("ES-TRAD", (int)Language.SpanishSpainTraditionalSort);
            gLanguageMapWml.AddEntry("ES-UY", (int)Language.SpanishUruguay);
            gLanguageMapWml.AddEntry("ES-VE", (int)Language.SpanishVenezuela);
            gLanguageMapWml.AddEntry("SX", (int)Language.Sutu);
            gLanguageMapWml.AddEntry("SV-FI", (int)Language.SwedishFinland);
            AdditionalLanguageToMapWml("SV", (int)Language.Swedish);
            gLanguageMapWml.AddEntry("SV", (int)Language.SwedishSweden);
            AdditionalLanguageToMapWml("SYR", (int)Language.Syriac);
            gLanguageMapWml.AddEntry("SYR", (int)Language.SyriacSyria);
            gLanguageMapWml.AddEntry("TG", (int)Language.Tajik);
            gLanguageMapWml.AddEntry("045F", (int)Language.Tamazight);
            gLanguageMapWml.AddEntry("085F", (int)Language.TamazightLatin);
            AdditionalLanguageToMapWml("TA", (int)Language.Tamil);
            gLanguageMapWml.AddEntry("TA", (int)Language.TamilIndia);
            AdditionalLanguageToMapWml("TT", (int)Language.Tatar);
            gLanguageMapWml.AddEntry("TT", (int)Language.TatarRussia);
            AdditionalLanguageToMapWml("TE", (int)Language.Telugu);
            gLanguageMapWml.AddEntry("TE", (int)Language.TeluguIndia);
            AdditionalLanguageToMapWml("TH", (int)Language.Thai);
            gLanguageMapWml.AddEntry("TH", (int)Language.ThaiThailand);
            gLanguageMapWml.AddEntry("0851", (int)Language.TibetanButan);
            gLanguageMapWml.AddEntry("BO", (int)Language.TibetanChina);
            gLanguageMapWml.AddEntry("TI-ER", (int)Language.TigrignaEritrea);
            gLanguageMapWml.AddEntry("TI-ET", (int)Language.TigrignaEthiopia);
            gLanguageMapWml.AddEntry("TS", (int)Language.Tsonga);
            gLanguageMapWml.AddEntry("TN", (int)Language.Tswana);
            AdditionalLanguageToMapWml("TR", (int)Language.Turkish);
            gLanguageMapWml.AddEntry("TR", (int)Language.TurkishTurkey);
            gLanguageMapWml.AddEntry("TK", (int)Language.Turkmen);
            AdditionalLanguageToMapWml("UK", (int)Language.Ukrainian);
            gLanguageMapWml.AddEntry("UK", (int)Language.UkrainianUkraine);
            AdditionalLanguageToMapWml("ER", (int)Language.Urdu);
            gLanguageMapWml.AddEntry("ER", (int)Language.UrduPakistan);
            gLanguageMapWml.AddEntry("UZ-CYR", (int)Language.UzbekCyrillic);
            AdditionalLanguageToMapWml("UZ", (int)Language.Uzbek);
            gLanguageMapWml.AddEntry("UZ", (int)Language.UzbekLatin);
            gLanguageMapWml.AddEntry("VEN", (int)Language.Venda);
            AdditionalLanguageToMapWml("VI", (int)Language.Vietnamese);
            gLanguageMapWml.AddEntry("VI", (int)Language.VietnameseVietnam);
            gLanguageMapWml.AddEntry("CY", (int)Language.Welsh);
            gLanguageMapWml.AddEntry("XH", (int)Language.Xhosa);
            gLanguageMapWml.AddEntry("0478", (int)Language.Yi);
            gLanguageMapWml.AddEntry("JI", (int)Language.Yiddish);
            gLanguageMapWml.AddEntry("YO", (int)Language.Yoruba);
            gLanguageMapWml.AddEntry("ZU", (int)Language.Zulu);
        }
    }
}
