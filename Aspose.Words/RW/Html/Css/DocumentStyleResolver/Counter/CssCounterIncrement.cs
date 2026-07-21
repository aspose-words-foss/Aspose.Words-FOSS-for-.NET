// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/11/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a modification that increments a counter by a specified value.
    /// Corresponds to the 'counter-increment' CSS property.
    /// </summary>
    internal class CssCounterIncrement : CssCounterModification
    {
        internal CssCounterIncrement(string counterName, int incrementValue)
        {
            Debug.Assert(StringUtil.HasChars(counterName));

            mCounterName = counterName;
            mIncrementValue = incrementValue;
        }

        internal override void ApplyTo(CssCounters counters)
        {
            counters.Increment(mCounterName, mIncrementValue);
        }

#if DEBUG
        public override string ToString()
        {
            return (mIncrementValue == 1)
                ? string.Format("inc <{0}>", mCounterName)
                : string.Format("inc <{0}> by {1}", mCounterName, mIncrementValue);
        }
#endif

        private readonly string mCounterName;

        private readonly int mIncrementValue;
    }
}
