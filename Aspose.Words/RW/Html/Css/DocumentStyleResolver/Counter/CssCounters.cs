// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/11/2015 by Victor Chebotok

using System.Text;
using Aspose.Collections;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Keeps track of all CSS counters during traversal of an HTML tree. Counters are accessed by name.
    /// </summary>
    internal class CssCounters
    {
        /// <summary>
        /// Resets a counter to an initial value. Corresponds to the 'counter-reset' CSS property.
        /// </summary>
        internal void Reset(string counterName, int initialValue)
        {
            // According to the CSS specification, 'none', 'initial, and 'inherit' must not be used as counter names
            // in the 'counter-reset' property.
            Debug.Assert(counterName != "none");
            Debug.Assert(counterName != "initial");
            Debug.Assert(counterName != "inherit");

            CssCounter counter = GetCounter(counterName);
            counter.Reset(initialValue);
        }

        /// <summary>
        /// Increments the value of a counter by a specified value. Corresponds to the 'counter-increment' CSS property.
        /// </summary>
        internal void Increment(string counterName, int incrementValue)
        {
            // According to the CSS specification, 'none', 'initial, and 'inherit' must not be used as counter names
            // in the 'counter-increment' property.
            Debug.Assert(counterName != "none");
            Debug.Assert(counterName != "initial");
            Debug.Assert(counterName != "inherit");

            CssCounter counter = GetCounter(counterName);
            counter.Increment(incrementValue);
        }

        /// <summary>
        /// Gets the current value of a counter. Corresponds to the 'counter' CSS function.
        /// </summary>
        internal int GetValue(string counterName)
        {
            CssCounter counter = GetCounter(counterName);
            return counter.GetValue();
        }

        /// <summary>
        /// Gets all scoped values of a counter, from outermost to innermost. Corresponds to the 'counters' CSS function.
        /// </summary>
        internal int[] GetAllValues(string counterName)
        {
            CssCounter counter = GetCounter(counterName);
            return counter.GetAllValues();
        }

        /// <summary>
        /// Starts processing an element during an HTML tree traversal.
        /// </summary>
        internal void IncreaseNesting()
        {
            ++mNestingLevel;
            foreach (CssCounter counter in mCounters.Values)
            {
                counter.IncreaseNesting();
            }
        }

        /// <summary>
        /// Stops processing an element and goes back to its parent during an HTML tree traversal.
        /// </summary>
        internal void DecreaseNesting()
        {
            foreach (CssCounter counter in mCounters.Values)
            {
                counter.DecreaseNesting();
            }
            --mNestingLevel;
        }

#if DEBUG
        public override string ToString()
        {
            if (mCounters.Count == 0)
            {
                return "<empty>";
            }

            StringBuilder result = new StringBuilder();
            StringToObjDictionary<CssCounter>.Enumerator enumerator = mCounters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (result.Length > 0)
                {
                    result.Append("; ");
                }

                string name = enumerator.CurrentKey;
                CssCounter counter = enumerator.CurrentValue;
                result.AppendFormat("<{0}>: {1}", name, counter);
            }
            return result.ToString();
        }
#endif

        /// <summary>
        /// Gets a counter by name. Creates a new counter if needed.
        /// </summary>
        /// <returns>
        /// The counter that has the specified name. Never <c>null</c>.
        /// </returns>
        private CssCounter GetCounter(string counterName)
        {
            CssCounter result = mCounters[counterName];
            if (result == null)
            {
                result = new CssCounter(mNestingLevel);
                mCounters.Add(counterName, result);
            }
            return result;
        }

        /// <summary>
        /// Stores counters that have been referenced so far. Keys are counter names and values are instances of
        /// <see cref="CssCounter"/>.
        /// </summary>
        private readonly StringToObjDictionary<CssCounter> mCounters = new StringToObjDictionary<CssCounter>();

        /// <summary>
        /// Keeps track of the current nesting level. This value is used to created counters that are first encountered on
        /// deeply nested levels.
        /// </summary>
        /// <remarks>
        /// 0 means the topmost level, 1 - child level, 2 - grand-child level, and so on.
        /// </remarks>
        private int mNestingLevel;
    }
}
