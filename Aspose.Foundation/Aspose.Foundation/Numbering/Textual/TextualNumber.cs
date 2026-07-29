// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/03/2012 by Andrey Soldatov

using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Aspose.Common;

namespace Aspose.Numbering
{
    /// <summary>
    /// This class is a helper for <see cref="NumberConverterCore"/>,
    /// its <see cref="ToFullString(long,bool,bool,Language)"/> method converts a number into a textual (cardinal or ordinal) representation.
    /// </summary>
    public class TextualNumber
    {
        /// <summary>
        /// No Ctor.
        /// </summary>
        private TextualNumber(EuropeanTextualNumberProvider numberProvider)
        {
            mNumberProvider = numberProvider;
        }

        /// <summary>
        /// <para>Returns a cardinal (one, two, three, ...) or ordinal (first, second, third, ...) number.</para>
        /// <para>Makes the first letter lower or uppercase as specified.</para>
        /// </summary>
        /// <remarks>
        /// <para>Called as utility method from <see cref="NumberConverterCore"/>. Try to not use it from inside the project.</para>
        /// <para>When <paramref name="language"/> is <see cref="Language.LanguageNotSet"/>, it uses culture of current thread.</para>
        /// </remarks>
        internal static string ToFullString(long value, bool isCardinal, bool isFirstLetterUppercase, Language language)
        {
#if !CPLUSPLUS && !OPTIMIZED
            Debug.AssertCallingClass(typeof(NumberConverterCore));
#endif

            StringBuilder builder = GetFullStringBuilder(value, isCardinal, isFirstLetterUppercase, language);

            return builder.ToString();
        }

        /// <summary>
        /// Returns textual representation of a number as in the example:
        /// "one hundred twenty-two and 16/100" if given value is 123.1616,
        /// using <see cref="CultureInfo"/> of the current thread.
        /// </summary>
        public static string ToDollarText(double value)
        {
            return ToDollarText(value, 0);
        }

        /// <summary>
        /// Returns textual representation of a number as in the example:
        /// "one hundred twenty-two and 16/100" if given value is 123.1616.
        /// </summary>
        /// <remarks>
        /// <para>When <paramref name="language"/> is <see cref="Language.LanguageNotSet"/>, it uses culture of current thread.</para>
        /// </remarks>
        public static string ToDollarText(double value, Language language)
        {
            long dollars = (long)value;
            StringBuilder builder = GetFullStringBuilder(dollars, true, false, language);

            builder
                .Append(' ')
                .Append(GetTextualNumberProvider(language).And)
                .Append(' ');

            int cents = MathUtil.DoubleToInt((value - dollars) * 100);
            builder.Append(FormatterPal.IntToStrD2(cents));
            builder.Append("/100");

            return builder.ToString();
        }

        private static StringBuilder GetFullStringBuilder(long value, bool isCardinal, bool isFirstLetterUppercase, Language language)
        {
            TextualNumber textualNumber = new TextualNumber(GetTextualNumberProvider(language));

            return GetFullStringBuilder(value, isCardinal, isFirstLetterUppercase, textualNumber);
        }

        private static StringBuilder GetFullStringBuilder(long value, bool isCardinal, bool isFirstLetterUppercase, TextualNumber textualNumber)
        {
            StringBuilder builder = (value == 0)
                ? new StringBuilder(textualNumber.mNumberProvider.Zero(isCardinal))
                : textualNumber.ToFullStringCore(value, isCardinal);

            builder.Insert(0, textualNumber.mNumberProvider.GetNumberPrefix(value, isCardinal));
            builder.Append(textualNumber.mNumberProvider.GetNumberSuffix(value, isCardinal));

            if ((builder.Length > 0) && (textualNumber.mNumberProvider.IsFirstLetterAlwaysUppercase || isFirstLetterUppercase))
                builder[0] = char.ToUpper(builder[0]);

            return builder;
        }

        private StringBuilder ToFullStringCore(long value, bool isCardinal)
        {
            Debug.Assert(value > 0);

            StringBuilder builder = new StringBuilder();

            List<int> thousands = GroupSplitter.SplitToGroups(value, 3);

            for (int i = thousands.Count - 1; i > 0; i--)
            {
                int thousand = thousands[i];

                if (thousand == 0)
                    continue;

                if ((thousand != 1) || mNumberProvider.IncludeOneInBigNumbers)
                {
                    if (mNumberProvider.IncludeThousandsWord(value))
                    {
                        bool hasHigherDigit = (i < thousands.Count - 1);
                        AppendThousand(builder, thousand, true, hasHigherDigit, true, isCardinal, thousands, i);
                    }
                }

                mNumberProvider.AppendWord(
                    builder,
                    mNumberProvider.GetBigNumeral(i, isCardinal, thousand, thousands),
                    mNumberProvider.UseThousandSeparator(isCardinal, thousand, thousands, i));
            }

            if (thousands[0] == 0)
            {
                if (!isCardinal)
                    mNumberProvider.AppendThousandsOrdinalSuffix(builder);
            }
            else
            {
                AppendThousand(builder, thousands[0], isCardinal, thousands.Count > 1, false, isCardinal, thousands, 0);
            }

            return builder;
        }

        /// <summary>
        /// Appends textual representation of <paramref name="value"/> (from 0 to 999 inclusively)
        /// to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">StringBuilder to place a string.</param>
        /// <param name="value">Source value from 0 to 999 inclusively.</param>
        /// <param name="isCardinal">Whether the appended string is cardinal.</param>
        /// <param name="hasHigherDigits">True signals that there are significant digits above these 3</param>
        /// <param name="hasLowerDigits">True signals that there are significant digits after these.</param>
        /// <param name="isCardinalOriginal">Whether the cardinal conversion.</param>
        /// <param name="thousands">Original value split into groups.</param>
        /// <param name="thousandIndex">Current group index.</param>
        private void AppendThousand(StringBuilder builder, int value, bool isCardinal, bool hasHigherDigits, bool hasLowerDigits, bool isCardinalOriginal, List<int> thousands, int thousandIndex)
        {
            Debug.Assert(value >= 0 && value <= 999);

            int hundreds = value / 100;
            value -= 100 * hundreds;
            if (hundreds != 0)
                mNumberProvider.AppendHundreds(builder, hundreds, isCardinal, value != 0, hasHigherDigits, hasLowerDigits, isCardinalOriginal, thousands, thousandIndex);

            mNumberProvider.AppendHundred(builder, value, hundreds, isCardinal, (hundreds != 0) || hasHigherDigits, hasLowerDigits, isCardinalOriginal, thousands, thousandIndex);
        }

        /// <summary>
        /// Returns number provider by the specified <paramref name="language"/>.
        /// </summary>
        private static EuropeanTextualNumberProvider GetTextualNumberProvider(Language language)
        {
            CultureInfo cultureInfo;
            try
            {
                cultureInfo = CultureInfo.GetCultureInfo((int)language);
            }
            catch
            {
                // Use current culture if locale Id has some illegal value.
                // For example, 0 - 'undefined', 0x400 - 'default'.
                cultureInfo = SystemPal.GetCurrentCulture();
            }

            return GetTextualNumberProvider(cultureInfo.Name);
        }

        /// <summary>
        /// Returns number provider by the specified <paramref name="cultureName"/>.
        /// </summary>
        private static EuropeanTextualNumberProvider GetTextualNumberProvider(string cultureName)
        {
            switch (cultureName)
            {
                case "fr": // French
                case "fr-BE":// French (Belgium)
                case "fr-CA": // French (Canada)
                case "fr-FR": // French (France)
                case "fr-LU": // French (Luxembourg)
                case "fr-MC": // French (Principality of Monaco)
                case "fr-CH": // French (Switzerland)
                case "fy-NL": // Frisian (Netherlands)
                    return new FrenchTextualNumber();
                case "de": // German
                case "de-AT": // German (Austria)
                case "de-DE": // German (Germany)
                case "de-LI": // German (Liechtenstein)
                case "de-LU": // German (Luxembourg)
                case "de-CH": // German (Switzerland)
                    return new GermanTextualNumber();
                case "pt": // Portuguese
                case "pt-BR": // Portuguese (Brazil)
                case "pt-PT": // Portuguese (Portugal)
                    return new PortugueseTextualNumber();
                case "es": // Spanish
                case "es-AR": // Spanish (Argentina)
                case "es-BO": // Spanish (Bolivia)
                case "es-CL": // Spanish (Chile)
                case "es-CO": // Spanish (Colombia)
                case "es-CR": // Spanish (Costa Rica)
                case "es-DO": // Spanish (Dominican Republic)
                case "es-EC": // Spanish (Ecuador)
                case "es-SV": // Spanish (El Salvador)
                case "es-GT": // Spanish (Guatemala)
                case "es-HN": // Spanish (Honduras)
                case "es-MX": // Spanish (Mexico)
                case "es-NI": // Spanish (Nicaragua)
                case "es-PA": // Spanish (Panama)
                case "es-PY": // Spanish (Paraguay)
                case "es-PE": // Spanish (Peru)
                case "es-PR": // Spanish (Puerto Rico)
                case "es-ES": // Spanish (Spain)
                case "es-ES_tradnl": // Spanish (Spain, Traditional Sort)
                case "es-US": // Spanish (United States)
                case "es-UY": // Spanish (Uruguay)
                case "es-VE": // Spanish (Venezuela)
                    return new SpanishTextualNumber();
                case "ca": // Catalan
                case "ca-ES": // Catalan (Spain)
                    return new CatalanTextualNumber();
                case "nl": // Dutch (Netherlands)
                case "nl-NL": // Dutch (Netherlands)
                case "nl-BE": // Dutch (Belgium)
                    return new DutchTextualNumber();
                case "cs-CZ":
                    return new CzechTextualNumber();
                case "hr-HR":
                    return new CroatianTextualNumber();
                case "pl-PL":
                    return new PolishTextualNumber();
                case "ro-RO":
                    return new RomanianTextualNumber();
                case "eu-ES":
                    return new BasqueTextualNumber();
                case "it-IT":
                    return new ItalianTextualNumber();
                default: // default is English
                    return new EnglishTextualNumber();
            }
        }

        private readonly EuropeanTextualNumberProvider mNumberProvider;
    }
}
