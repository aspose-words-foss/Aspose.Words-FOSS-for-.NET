// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/11/2015 by Victor Chebotok

using System.Collections.Generic;
using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a series of modifications to CSS counters.
    /// </summary>
    /// <remarks>
    /// Modifications to counters are normally defined by 'counter-reset' and 'counter-increment' CSS properties.
    /// </remarks>
    internal class CssCounterModifications
    {
        /// <summary>
        /// Represents a number of modifications to CSS counters.
        /// </summary>
        internal static CssCounterModifications Parse(CssDeclarationCollection declarations)
        {
            CssCounterModifications result = Empty;

            // Counters are always reset first and then incremented, no matter what is the order in which 'counter-reset'
            // and 'counter-increment' declarations come.

            CssDeclaration counterReset = declarations["counter-reset"];
            if (counterReset != null)
            {
                List<CounterNameValuePair> modifications = ParsePropertyValue(counterReset.Value, 0);
                if ((result == Empty) && (modifications.Count > 0))
                {
                    result = new CssCounterModifications();
                }
                result.AddResetModifications(modifications);
            }

            CssDeclaration counterIncrement = declarations["counter-increment"];
            if (counterIncrement != null)
            {
                List<CounterNameValuePair> modifications = ParsePropertyValue(counterIncrement.Value, 1);
                if ((result == Empty) && (modifications.Count > 0))
                {
                    result = new CssCounterModifications();
                }
                result.AddIncrementModifications(modifications);
            }

            return result;
        }

        internal void ApplyTo(CssCounters counters)
        {
            foreach (CssCounterModification modification in mModifications)
            {
                modification.ApplyTo(counters);
            }
        }

#if DEBUG
        public override string ToString()
        {
            if (mModifications.Count == 0)
            {
                return "<empty>";
            }

            if (mModifications.Count == 1)
            {
                return mModifications[0].ToString();
            }

            StringBuilder result = new StringBuilder();
            result.Append(mModifications[0]);
            for (int i = 1; i < mModifications.Count; i++)
            {
                result.Append("; ");
                result.Append(mModifications[i]);
            }
            return result.ToString();
        }
#endif

        private void AddResetModifications(IEnumerable<CounterNameValuePair> modifications)
        {
            foreach (CounterNameValuePair counterAndValue in modifications)
            {
                Add(new CssCounterReset(counterAndValue.CounterName, counterAndValue.Value));
            }
        }

        private void AddIncrementModifications(IEnumerable<CounterNameValuePair> modifications)
        {
            foreach (CounterNameValuePair counterAndValue in modifications)
            {
                Add(new CssCounterIncrement(counterAndValue.CounterName, counterAndValue.Value));
            }
        }

        private void Add(CssCounterModification modification)
        {
            Debug.Assert(this != Empty);
            Debug.Assert(modification != null);

            mModifications.Add(modification);
        }

        /// <summary>
        /// Parses parameters of 'counter-reset' and 'counter-increment' properties. Both properties has the same syntax.
        /// </summary>
        /// <returns>
        /// A list of counter name/value pairs. Items are of the type <see cref="CounterNameValuePair"/>.
        /// </returns>
        private static List<CounterNameValuePair> ParsePropertyValue(CssPropertyValue propertyValue, int defaultValue)
        {
            List<CounterNameValuePair> result = new List<CounterNameValuePair>();

            string lastNameWithoutValue = null;
            for (int i = 0; i < propertyValue.Count; i++)
            {
                CssValue value = propertyValue[i];
                string counterName = ParseCounterName(value);
                if (counterName != null)
                {
                    if (lastNameWithoutValue != null)
                    {
                        result.Add(new CounterNameValuePair(lastNameWithoutValue, defaultValue));
                    }
                    lastNameWithoutValue = counterName;
                }
                else
                {
                    if (lastNameWithoutValue == null)
                    {
                        // Error. Expected an identifier but found something else.
                        result.Clear();
                        return result;
                    }

                    CssNumberValue increment = value as CssNumberValue;
                    if (increment != null)
                    {
                        double integerPart = System.Math.Truncate(increment.DoubleValue);
                        bool hasNoFractionalPart = MathUtil.AreEqual(integerPart, increment.DoubleValue);
                        if (hasNoFractionalPart)
                        {
                            // Constrain the counter value to a signed 32-bit integer.
                            int constrainedValue;
                            if (integerPart > int.MaxValue)
                            {
                                constrainedValue = int.MaxValue;
                            }
                            else if (integerPart < int.MinValue)
                            {
                                constrainedValue = int.MinValue;
                            }
                            else
                            {
                                constrainedValue = MathUtil.Truncate(integerPart);
                            }

                            result.Add(new CounterNameValuePair(lastNameWithoutValue, constrainedValue));
                            lastNameWithoutValue = null;
                        }
                        else
                        {
                            // Error. The specified number value is not an integer.
                            result.Clear();
                            return result;
                        }
                    }
                    else
                    {
                        // Error. Expected a number but found something else.
                        result.Clear();
                        return result;
                    }
                }
            }
            if (lastNameWithoutValue != null)
            {
                result.Add(new CounterNameValuePair(lastNameWithoutValue, defaultValue));
            }

            return result;
        }

        private static string ParseCounterName(CssValue value)
        {
            CssIdentifierValue counterName = value as CssIdentifierValue;
            // According to the CSS specification, keywords 'none', 'initial', and 'inherit' must not be used as counter names.
            if ((counterName != null) &&
                (!counterName.Equals(CssValue.None)) &&
                (!counterName.Equals(CssValue.Inherit)) &&
                (!counterName.Equals(CssValue.Initial)))
            {
                return counterName.Value;
            }
            return null;
        }

        internal static readonly CssCounterModifications Empty = new CssCounterModifications();

        private readonly List<CssCounterModification> mModifications = new List<CssCounterModification>();
    }
}
