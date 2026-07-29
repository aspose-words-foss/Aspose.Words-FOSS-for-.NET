// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/11/2015 by Victor Chebotok

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Keeps record of a single CSS counter during an HTML tree traversal.
    /// </summary>
    internal class CssCounter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initialNestingLevel">
        /// The nesting level that this counter is first encountered at. The value must be non-negative. 0 means the topmost
        /// level, 1 - child level, 2 - grand-child, and so on.
        /// </param>
        internal CssCounter(int initialNestingLevel)
        {
            Debug.Assert(initialNestingLevel >= 0);

            mScopes = new Stack<CssCounterScope>();

            // The counter might have been first encountered at a nested level. In that case, we need to fill all upper-level
            // scopes with null placeholders, so that nesting increase/decrease will work correctly.
            for (int i = 0; i < (initialNestingLevel + 1); i++)
            {
                mScopes.Push(null);
            }

            // At least one topmost scope is always created. The topmost (global) scope cannot be closed but it can be replaced.
            Debug.Assert(mScopes.Count > 0);
        }

        /// <summary>
        /// Resets the value of the counter to an initial value. Corresponds to the 'counter-reset' CSS property.
        /// </summary>
        internal void Reset(int initialValue)
        {
            Debug.Assert(mScopes.Count > 0);

            // Remove the current counter scope in order to replace it with a new one.
            mScopes.Pop();
            CssCounterScope newScope = new CssCounterScope(initialValue);

            // If the current HTML element has a parent (is not the root HTML element), we need to replace the parent's scope
            // as well, because according to CSS rules, a counter scope introduced on a child element lasts until the end
            // of the parent element. For details, see http://www.w3.org/TR/CSS2/generate.html#scope
            if (mScopes.Count > 0)
            {
                mScopes.Pop();
                mScopes.Push(newScope);
            }

            // Replace the current counter scope.
            mScopes.Push(newScope);

            Debug.Assert(mScopes.Count > 0);
        }

        /// <summary>
        /// Increments the value of the counter by a specified value. Corresponds to the 'counter-increment' CSS property.
        /// </summary>
        /// <remarks>
        /// If the counter hasn't been initialized yet, it will be reset to zero prior to incrementation.
        /// </remarks>
        internal void Increment(int incrementValue)
        {
            CssCounterScope currentScope = GetOrCreateInnermostScope();
            currentScope.Increment(incrementValue);
        }

        /// <summary>
        /// Starts processing an HTML element during tree traversal.
        /// </summary>
        internal void IncreaseNesting()
        {
            Debug.Assert(mScopes.Count > 0);

            // Here we don't introduce a new scoped version of the counter. We link the current scope to the parent's scope
            // in order to reuse it.
            mScopes.Push(mScopes.Peek());
        }

        /// <summary>
        /// Stops processing an HTML element and goes back to its parent during tree traversal.
        /// </summary>
        internal void DecreaseNesting()
        {
            // If a child element has created a new scoped version of the counter via 'counter-reset', it is destroyed here.
            mScopes.Pop();

            Debug.Assert(mScopes.Count > 0);
        }

        /// <summary>
        /// Gets the current value of this counter.
        /// </summary>
        /// <remarks>
        /// This method resets the counter to zero if it hasn't been initialized yet.
        /// The returned value is the same as the value returned by the 'counter' CSS function.
        /// </remarks>
        internal int GetValue()
        {
            CssCounterScope currentScope = GetOrCreateInnermostScope();
            return currentScope.Value;
        }

        /// <summary>
        /// Gets all scoped values of the counter, from outermost to innermost.
        /// </summary>
        /// <remarks>
        /// This method resets the counter to zero if it hasn't been initialized yet.
        /// The order in which values are returned correstponds to the order in which values are returned by the 'counters'
        /// CSS function.
        /// </remarks>
        /// <returns>
        /// All scoped values of the counter. The result always contains at least one element.
        /// </returns>
        internal int[] GetAllValues()
        {
            Debug.Assert(mScopes.Count > 0);

            // Find out different scopes and collect values from them.
            IntList values = new IntList();
            CssCounterScope currentScope = null;
            foreach (CssCounterScope scope in mScopes)
            {
                // Every time the scope changes we add a new scoped value.
                if (scope != currentScope)
                {
                    if (scope != null)
                    {
                        values.Add(scope.Value);
                    }
                    currentScope = scope;
                }
            }

            if (values.Count > 0)
            {
                // The stack enumerates its values from innermost to outermost and we need to reverse the order.
                values.Reverse();
                return values.ToArray();
            }

            // The counter hasn't been initialized yet and we need to create it here.
            CssCounterScope newScope = GetOrCreateInnermostScope();
            return new int[] { newScope.Value };
        }

#if DEBUG
        public override string ToString()
        {
            CssCounterScope currentScope = mScopes.Peek();
            return (currentScope != null)
                ? FormatterPal.IntToStr(currentScope.Value)
                : "0 <uninitialized>";
        }
#endif

        /// <summary>
        /// Gets or creates the current (innermost) scope of this counter.
        /// </summary>
        /// <returns>
        /// The current (innermost) scope of this counter. The result is never <c>null</c>. If there is no current scope, a new
        /// zero-initialized scope is created.
        /// </returns>
        private CssCounterScope GetOrCreateInnermostScope()
        {
            Debug.Assert(mScopes.Count > 0);
            CssCounterScope scope = mScopes.Peek();
            if (scope == null)
            {
                // Create a new scope.
                Reset(0);
                scope = mScopes.Peek();
                Debug.Assert(scope != null);
            }
            return scope;
        }

        /// <summary>
        /// Scoped values of the counter. Values are of type <see cref="CssCounterScope"/>.
        /// </summary>
        private readonly Stack<CssCounterScope> mScopes;
    }
}
