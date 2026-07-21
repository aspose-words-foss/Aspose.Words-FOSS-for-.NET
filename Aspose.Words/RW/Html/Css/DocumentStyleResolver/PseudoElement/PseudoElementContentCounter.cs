// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/11/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the 'counter' CSS function, which is a souce of generated content for pseudo-elements.
    /// </summary>
    internal class PseudoElementContentCounter : PseudoElementContentCounterFunction
    {
        internal PseudoElementContentCounter(
            string counterName,
            NumberStyle counterStyle)
            : base(
                  counterName,
                  counterStyle)
        {
            // Empty constructor.
        }

        /// <summary>
        /// Parses parameters of the 'counter' CSS function.
        /// </summary>
        /// <returns>
        /// Parsed content or <c>null</c> if parameters are malformed.
        /// </returns>
        internal static PseudoElementContentCounter Parse(CssFunctionValue value)
        {
            bool syntaxIsValid = (value.Name == "counter") &&
                ArgumentsAreCommaSeparated(value.Arguments) &&
                ((value.Arguments.Count == 1) || (value.Arguments.Count == 3));
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

            NumberStyle numberStyle = DefaultNumberStyle;
            if (value.Arguments.Count == 3)
            {
                // Parameter indexes are shifted because of comma separators.
                string counterStyle = ParseCounterParameter(value.Arguments[2]);
                if (counterStyle == null)
                {
                    // Error. The counter style must be a non-reserved identifier.
                    return null;
                }
                numberStyle = CounterStyleToNumberStyle(counterStyle);
            }

            return new PseudoElementContentCounter(counterName, numberStyle);
        }

        internal override void Accept(IPseudoElementContentPartVisitor visitor)
        {
            visitor.VisitCounter(CounterName, CounterStyle);
        }

#if DEBUG
        public override string ToString()
        {
            return "counter(" + CounterName + ")";
        }
#endif
    }
}
