// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the 'counters' CSS function, which is a souce of generated content for pseudo-elements.
    /// </summary>
    internal class PseudoElementContentCounters : PseudoElementContentCounterFunction
    {
        internal PseudoElementContentCounters(
            string counterName,
            string separator,
            NumberStyle counterStyle)
            : base(
                  counterName,
                  counterStyle)
        {
            Debug.Assert(separator != null);
            mSeparator = separator;
        }

        /// <summary>
        /// Parses parameters of the 'counters' CSS function.
        /// </summary>
        /// <returns>
        /// Parsed content or <c>null</c> if parameters are malformed.
        /// </returns>
        internal static PseudoElementContentCounters Parse(CssFunctionValue value)
        {
            bool syntaxIsValid = (value.Name == "counters") &&
                ArgumentsAreCommaSeparated(value.Arguments) &&
                ((value.Arguments.Count == 3) || (value.Arguments.Count == 5));
            if (!syntaxIsValid)
            {
                // Error. Either function name or function syntax is invalid.
                return null;
            }

            string counterName = ParseCounterParameter(value.Arguments[0]);
            if (counterName == null)
            {
                // Error. The counter name must be a non-reserved identifier.
                return null;
            }

            // Parameter indexes are shifted because of comma separators.
            CssValue secondParameter = value.Arguments[2];
            if (secondParameter.ValueType != CssValueType.String)
            {
                // Error. The separator parameter must be a string.
                return null;
            }
            string separator = ((CssStringValue)secondParameter).Value;

            NumberStyle numberStyle = DefaultNumberStyle;
            if (value.Arguments.Count == 5)
            {
                // Parameter indexes are shifted because of comma separators.
                string counterStyle = ParseCounterParameter(value.Arguments[4]);
                if (counterStyle == null)
                {
                    // Error. The counter style must be a non-reserved identifier.
                    return null;
                }
                numberStyle = CounterStyleToNumberStyle(counterStyle);
            }

            return new PseudoElementContentCounters(counterName, separator, numberStyle);
        }

        internal override void Accept(IPseudoElementContentPartVisitor visitor)
        {
            visitor.VisitCounters(CounterName, mSeparator, CounterStyle);
        }

#if DEBUG
        public override string ToString()
        {
            return "counters(" + CounterName + ", \"" + mSeparator + "\")";
        }
#endif

        private readonly string mSeparator;
    }
}
