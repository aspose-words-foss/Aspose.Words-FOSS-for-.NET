// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/06/2017 by Victor Chebotok

using System;
using Aspose.Collections;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Matches a <see cref="CssComplexSelector"/> against HTML elements during depth-first traversal of an HTML tree.
    /// </summary>
    internal class CssComplexSelectorMatcher : CssSelectorMatcher
    {
        internal CssComplexSelectorMatcher(CssComplexSelector selector, CssDocumentMode documentMode)
            : base(selector, documentMode)
        {
            mCombinators = selector.ToCombinatorArray();
            Debug.Assert(mCombinators.Length >= 2);
            mCombinatorIndexTransitions = new int[mCombinators.Length];
            
            // The transition table links 
            int combinatorIndex = mCombinatorIndexTransitions.Length - 1;
            mCombinatorIndexTransitions[mCombinatorIndexTransitions.Length - 1] = combinatorIndex;
            for (int i = mCombinatorIndexTransitions.Length - 1; i >= 1; i--)
            {
                if (mCombinators[i] is CssDescendantCombinator)
                {
                    mCombinatorIndexTransitions[i - 1] = combinatorIndex;
                    combinatorIndex = i - 1;
                }
                else
                {
                    // These transitions are not required and we fill them for debug purposes only.
                    mCombinatorIndexTransitions[i - 1] = -1;
                }
            }

            // First level states to start with.
            mCurrentCombinatorIndexes.Push(combinatorIndex);
            mSubtreeRoots.Push(0);
        }

        internal override bool Push(IElementProvider element)
        {
            Debug.Assert(mCurrentCombinatorIndexes.Count > 0);
            Debug.Assert(mSubtreeRoots.Count > 0);

            int combinatorIndex = mCurrentCombinatorIndexes.Peek();
            Debug.Assert(combinatorIndex >= 0);
            int subtreeRootLevel = mSubtreeRoots.Peek();
            Debug.Assert(subtreeRootLevel >= 0);

            // Implicit (anonymous) elements are not in the HTML tree and they cannot be selected by selectors.
            bool matched = (!element.IsImplicit) &&
                Matches(element, combinatorIndex, mCurrentCombinatorIndexes.Count, subtreeRootLevel);

            // SPEED. In order to speed up selector matching, we split complex expressions into sub-expressions by descendant
            // combinators. Sub-expressions are matched from left to right. As soon as we find an element that matches the last
            // selector of a sub-expression, we start looking for the next sub-expression. A sub-expression is matched in the
            // reversed order. That is, first we search for the last element, and then go backwards and match other selectors
            // of that sub-expression.
            // If the current sub-expression has been matched, at the next level we will be looking for the next sub-expression.
            int nextLevelCombinatorIndex = (matched)
                ? mCombinatorIndexTransitions[combinatorIndex]
                : combinatorIndex;
            Debug.Assert(nextLevelCombinatorIndex >= 0);
            mCurrentCombinatorIndexes.Push(nextLevelCombinatorIndex);

            if (nextLevelCombinatorIndex != combinatorIndex)
            {
                subtreeRootLevel = mCurrentCombinatorIndexes.Count;
            }
            mSubtreeRoots.Push(subtreeRootLevel);

            return matched && (combinatorIndex == (mCombinators.Length - 1));
        }

        internal override void Pop()
        {
            Debug.Assert(mCurrentCombinatorIndexes.Count > 1);
            Debug.Assert(mSubtreeRoots.Count > 1);
            mCurrentCombinatorIndexes.Pop();
            mSubtreeRoots.Pop();
        }

        private bool Matches(IElementProvider element, int combinatorIndex, int elementLevel, int subtreeRootLevel)
        {
            Debug.Assert(combinatorIndex >= 0);

            if (element == null)
            {
                return false;
            }

            if (!mCombinators[combinatorIndex].Selector.Selects(element, DocumentMode))
            {
                return false;
            }

            // Matched the rightmost selector of a sub-expression. Let's also check selectors to the left of the mathed one.
            CssCombinator combinator = mCombinators[combinatorIndex];
            // Sub-expressions are bounded by descendant combinators. Note that the first (leftmost) combinator is also always
            // descendant, so this condition prevents us from looking past the start of the combinator list.
            if (mCombinators[combinatorIndex] is CssDescendantCombinator)
            {
                return true;
            }
            if (combinator is CssGeneralSiblingCombinator)
            {
                // Go left the tree until we match another element.
                IElementProvider prevElement = element.GetPreviousSiblingElement();
                while (prevElement != null)
                {
                    if (Matches(prevElement, combinatorIndex - 1, elementLevel, subtreeRootLevel))
                    {
                        return true;
                    }
                    prevElement = prevElement.GetPreviousSiblingElement();
                }
                return false;
            }
            if (combinator is CssAdjacentSiblingCombinator)
            {
                // Go left the tree by one element.
                return Matches(element.GetPreviousSiblingElement(), combinatorIndex - 1, elementLevel, subtreeRootLevel);
            }
            if (combinator is CssChildCombinator)
            {
                // Go up the tree by one level.
                // We are not allowed to search beyond the current subtree root.
                return (elementLevel > subtreeRootLevel) &&
                    Matches(element.GetParentElement(), combinatorIndex - 1, elementLevel - 1, subtreeRootLevel);
                
            }
            throw new InvalidOperationException("Unknown combinator type.");
        }

        private readonly IntStack mCurrentCombinatorIndexes = new IntStack();
        private readonly IntStack mSubtreeRoots = new IntStack();
        private readonly int[] mCombinatorIndexTransitions;
        private readonly CssCombinator[] mCombinators;
    }
}
