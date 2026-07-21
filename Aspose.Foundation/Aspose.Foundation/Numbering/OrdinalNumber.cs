// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2016 by Edward Voronov

using System;

namespace Aspose.Numbering
{
    /// <summary>
    /// Converts a numbers to an ordinal numbers.
    /// </summary>
    internal static class OrdinalNumber
    {
        /// <summary>
        /// Returns an ordinal number (1st, 2nd, 3rd, ...).
        /// </summary>
        internal static string ToOrdinalNumber(long value)
        {
            return string.Format("{0}{1}", value, EnglishOrdinalSuffix(value));
        }

        /// <summary>
        /// Returns an ordinal number considering localization. (1st, 2nd, 3rd, or 1-й, 2-й, 3-й...).
        /// </summary>
        internal static string ToLocalizedOrdinalNumber(long value, int localeId)
        {
            return string.Format("{0}{1}", value, LocalizedOrdinalSuffix(value, localeId));
        }

        /// <summary>
        /// Gets ordinal suffix based on localeId.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="localeId">Locale identifier (language).</param>
        private static string LocalizedOrdinalSuffix(long value, int localeId)
        {
            Language language = (Language)localeId;
            OrdinalGroup ordinalGroup = GetOrdinalGroup(language);
            switch (ordinalGroup)
            {
                case OrdinalGroup.English:
                    return EnglishOrdinalSuffix(value);
                case OrdinalGroup.EastEurope:
                    return ".";
                case OrdinalGroup.WestEurope:
                    return "º";
                case OrdinalGroup.CenterEurope:
                    return CenterEuropeOrdinalSuffix(value, language);
                case OrdinalGroup.Russian:
                    return "-й";
                case OrdinalGroup.Italian:
                    return "°";
                case OrdinalGroup.Greek:
                    return "ο";
                case OrdinalGroup.Danish:
                    return DanishOrdinalSuffix(value);
                case OrdinalGroup.Swedish:
                    return SwedishOrdinalSuffix(value);
                case OrdinalGroup.Catalan:
                    return CatalanOrdinalSuffix(value);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string CatalanOrdinalSuffix(long value)
        {
            switch ((int)Math.Abs(value))
            {
                case 1:
                case 3:
                    return "r";
                case 2:
                    return "n";
                case 4:
                    return "t";
                case 14:
                    return "èh";
                default:
                    return "è";
            }
        }

        private static string SwedishOrdinalSuffix(long value)
        {
            int twoDigit = (int)(Math.Abs(value) % 100);
            if (twoDigit < 10 || twoDigit > 20)
            {
                switch (twoDigit % 10)
                {
                    case 1:
                    case 2:
                        return ":a";
                    default:
                        // do nothing;
                        break;

                }
            }

            return ":e";
        }

        private static string DanishOrdinalSuffix(long value)
        {
            if (value == 0)
                return "te";

            switch ((int) Math.Abs(value) % 100)
            {
                case 0:
                    return "ende";
                case 1:
                    return "ste";
                case 2:
                    return "nden";
                case 3:
                    return "dje";
                case 4:
                    return "rde";
                case 5:
                case 6:
                case 11:
                case 12:
                case 30:
                    return "te";
                default:
                    return "nde";
            }
        }

        private static string EnglishOrdinalSuffix(long value)
        {
            int twoDigits = (int)(Math.Abs(value) % 100);
            if ((twoDigits < 4) || (twoDigits > 20))
            {
                switch (twoDigits % 10)
                {
                    case 1:
                        return "st";
                    case 2:
                        return "nd";
                    case 3:
                        return "rd";
                    default:
                        // do nothing;
                        break;
                }
            }

            return "th";
        }

        private static string CenterEuropeOrdinalSuffix(long value, Language language)
        {
            if (LanguageOnly.Compare(LanguageOnly.Dutch, (int)language))
                return "e";

            switch ((int) value)
            {
                case -1:
                case 1:
                    return "er";
                case 0:
                    return string.Empty;
                default:
                    return "e";
            }
        }

        private static OrdinalGroup GetOrdinalGroup(Language language)
        {
            switch (language)
            {
                case Language.Czech:
                case Language.German:
                case Language.Finnish:
                case Language.Hungarian:
                case Language.Norwegian:
                case Language.Polish:
                case Language.Croatian:
                case Language.Turkish:
                case Language.Slovenian:
                case Language.Estonian:
                case Language.Basque:
                case Language.CzechCzechRepublic:
                case Language.GermanGermany:
                case Language.FinnishFinland:
                case Language.HungarianHungary:
                case Language.NorwegianBokmal:
                case Language.PolishPoland:
                case Language.CroatianCroatia:
                case Language.TurkishTurkey:
                case Language.SlovenianSlovenia:
                case Language.EstonianEstonia:
                case Language.BasqueBasque:
                case Language.GermanSwitzerland:
                case Language.NorwegianNynorsk:
                case Language.SerbianLatinSerbiaAndMontenegro:
                case Language.GermanAustria:
                case Language.SerbianCyrillicSerbiaAndMontenegro:
                case Language.GermanLuxembourg:
                case Language.CroatianBozniaAndHerzegovina:
                case Language.GermanLiechtenstein:
                case Language.BosnianLatin:
                case Language.SerbianLatinBosniaAndHerzegovina:
                case Language.SerbianCyrillicBosniaAndHerzegovina:
                case Language.BosnianCyrillic:
                case Language.Serbian:
                    return OrdinalGroup.EastEurope;

                case Language.Spanish:
                case Language.Portuguese:
                case Language.SpanishSpainTraditionalSort:
                case Language.PortugueseBrazil:
                case Language.SpanishMexico:
                case Language.PortuguesePortugal:
                case Language.SpanishSpainModernSort:
                case Language.SpanishGuatemala:
                case Language.SpanishCostaRica:
                case Language.SpanishPanama:
                case Language.SpanishDominicanRepublic:
                case Language.SpanishVenezuela:
                case Language.SpanishColombia:
                case Language.SpanishPeru:
                case Language.SpanishArgentina:
                case Language.SpanishEcuador:
                case Language.SpanishChile:
                case Language.SpanishUruguay:
                case Language.SpanishParaguay:
                case Language.SpanishBolivia:
                case Language.SpanishElSalvador:
                case Language.SpanishHonduras:
                case Language.SpanishNicaragua:
                case Language.SpanishPuertoRico:
                    return OrdinalGroup.WestEurope;

                case Language.French:
                case Language.Dutch:
                case Language.FrenchFrance:
                case Language.DutchNetherlands:
                case Language.FrenchBelgium:
                case Language.DutchBelgium:
                case Language.FrenchCanada:
                case Language.FrenchSwitzerland:
                case Language.FrenchLuxembourg:
                case Language.FrenchMonaco:
                case Language.FrenchWestIndies:
                case Language.FrenchReunion:
                case Language.FrenchCongo:
                case Language.FrenchSenegal:
                case Language.FrenchCameroon:
                case Language.FrenchCoteDIvoire:
                case Language.FrenchMali:
                case Language.FrenchMorocco:
                case Language.FrenchHaiti:
                    return OrdinalGroup.CenterEurope;

                case Language.Catalan:
                case Language.CatalanCatalan:
                    return OrdinalGroup.Catalan;

                case Language.Danish:
                case Language.DanishDenmark:
                    return OrdinalGroup.Danish;

                case Language.Greek:
                case Language.GreekGreece:
                    return OrdinalGroup.Greek;

                case Language.Italian:
                case Language.ItalianItaly:
                case Language.ItalianSwitzerland:
                    return OrdinalGroup.Italian;

                case Language.Russian:
                case Language.RussianRussia:
                case Language.RussianMoldova:
                    return OrdinalGroup.Russian;

                case Language.Swedish:
                case Language.SwedishSweden:
                case Language.SwedishFinland:
                    return OrdinalGroup.Swedish;

                default:
                    return OrdinalGroup.English;
            }
        }

        private enum OrdinalGroup
        {
            English,
            EastEurope,
            WestEurope,
            CenterEurope,
            Russian,
            Italian,
            Greek,
            Danish,
            Swedish,
            Catalan
        }
    }
}
