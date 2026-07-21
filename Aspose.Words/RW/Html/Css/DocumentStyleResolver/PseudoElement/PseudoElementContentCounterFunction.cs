// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/12/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents either 'counter' or 'counters' CSS function, which is a souce of generated content for pseudo-elements.
    /// Contains code that is common to both functions.
    /// </summary>
    internal abstract class PseudoElementContentCounterFunction : PseudoElementContentPart
    {
        protected PseudoElementContentCounterFunction(
            string counterName,
            NumberStyle counterStyle)
        {
            Debug.Assert(StringUtil.HasChars(counterName));
            Debug.Assert(counterStyle != NumberStyle.Bullet);

            CounterName = counterName;
            CounterStyle = counterStyle;
        }

        internal string CounterName { get; }

        internal NumberStyle CounterStyle { get; }

        /// <summary>
        /// Checks whether the value collection is a comma-separated list. Auxillary method for parsing of function parameters.
        /// </summary>
        protected static bool ArgumentsAreCommaSeparated(CssValueList arguments)
        {
            // Both 'counter' and 'counters' functions have common syntax: they accept a comma-separated list of arguments.

            // All even arguments must be commas so the total number of arguments must be odd.
            if ((arguments.Count % 2) != 1)
            {
                // Error. Arguments are not separated by commas.
                return false;
            }
            for (int i = 1; i < arguments.Count; i += 2)
            {
                if (arguments[i].ValueType != CssValueType.Comma)
                {
                    // Error. Arguments are not separated by commas.
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Auxillary method for parsing of function parameters.
        /// </summary>
        protected static string ParseCounterParameter(CssValue parameter)
        {
            return (parameter.ValueType == CssValueType.Identifier)
                ? ((CssIdentifierValue)parameter).Value
                : null;
        }

        protected static NumberStyle CounterStyleToNumberStyle(string counterStyle)
        {
            // Here we check only values, which are also used in HtmlListItemMarker class (except bullet values).
            // counter style values are case-insensitive.
            switch (counterStyle.ToLowerInvariant())
            {
                case "decimal":
                case "arabic-indic": // WORDSNET-26977 Add support for "arabic-indic" list style.
                    return NumberStyle.Arabic;
                case "decimal-leading-zero":
                    return NumberStyle.LeadingZero;
                case "lower-latin":
                case "lower-alpha":
                case "lower-greek":
                    return NumberStyle.LowercaseLetter;
                case "upper-latin":
                case "upper-alpha":
                case "upper-greek":
                    return NumberStyle.UppercaseLetter;
                case "lower-roman":
                    return NumberStyle.LowercaseRoman;
                case "upper-roman":
                    return NumberStyle.UppercaseRoman;
                default:
                    return DefaultNumberStyle;
            }
        }

        protected const NumberStyle DefaultNumberStyle = NumberStyle.Arabic;
    }
}
