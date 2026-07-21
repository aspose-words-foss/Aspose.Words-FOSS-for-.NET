// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/11/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a modification that resets a counter to an initial value. Corresponds to the 'counter-reset' CSS property.
    /// </summary>
    internal class CssCounterReset : CssCounterModification
    {
        internal CssCounterReset(string counterName, int initialValue)
        {
            Debug.Assert(StringUtil.HasChars(counterName));

            mCounterName = counterName;
            mInitialValue = initialValue;
        }

        internal override void ApplyTo(CssCounters counters)
        {
            counters.Reset(mCounterName, mInitialValue);
        }

#if DEBUG
        public override string ToString()
        {
            return (mInitialValue == 0)
                ? string.Format("reset <{0}>", mCounterName)
                : string.Format("reset <{0}> to {1}", mCounterName, mInitialValue);
        }
#endif

        private readonly string mCounterName;

        private readonly int mInitialValue;
    }
}
