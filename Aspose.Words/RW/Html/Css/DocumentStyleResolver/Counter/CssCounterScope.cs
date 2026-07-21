// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/11/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a scoped value of a CSS counter.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/2011/REC-CSS2-20110607/generate.html#scope to learn more about CSS counter scopes.
    /// </remarks>
    internal class CssCounterScope
    {
        internal CssCounterScope(int initialValue)
        {
            mValue = initialValue;
        }

        internal void Increment(int incrementValue)
        {
            // In modern browsers, counters are 32-bit signed integers. However, browsers behave differently when counter values
            // get too high or too low. We stick to IE behavior that doesn't allow counter values to overflow (in IE, counter
            // values don't wrap around min/max bounds).
            long newValue = (long)mValue + incrementValue;
            if (newValue > int.MaxValue)
            {
                newValue = int.MaxValue;
            }
            if (newValue < int.MinValue)
            {
                newValue = int.MinValue;
            }
            mValue = (int)newValue;
        }

        internal int Value
        {
            get { return mValue; }
        }

        private int mValue;
    }
}