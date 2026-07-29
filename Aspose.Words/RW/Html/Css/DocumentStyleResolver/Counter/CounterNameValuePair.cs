// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/11/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// An auxiliary class used during parsing of CSS counter modifications. Countains a counter name and a related value. 
    /// </summary>
    internal class CounterNameValuePair
    {
        internal CounterNameValuePair(string counterName, int value)
        {
            Debug.Assert(StringUtil.HasChars(counterName));

            mCounterName = counterName;
            mValue = value;
        }

        internal string CounterName
        {
            get { return mCounterName; }
        }

        internal int Value
        {
            get { return mValue; }
        }

        private readonly string mCounterName;

        private readonly int mValue;
    }
}