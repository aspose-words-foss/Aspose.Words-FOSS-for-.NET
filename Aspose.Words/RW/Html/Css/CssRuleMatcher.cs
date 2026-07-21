// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/06/2017 by Victor Chebotok

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Matches a <see cref="CssStyleRule"/> against HTML elements during depth-first traversal of an HTML tree.
    /// </summary>
    internal class CssRuleMatcher
    {
        internal CssRuleMatcher(CssStyleRule rule, CssDocumentMode documentMode)
        {
            mDeclarations = rule.Declarations;
            mSelectorMatchers = new CssSelectorMatcher[rule.Selectors.Length];
            for (int i = 0; i < rule.Selectors.Length; i++)
            {
                mSelectorMatchers[i] = rule.Selectors[i].CreateMatcher(documentMode);
            }
        }

        internal List<CssSelector> Push(IElementProvider element)
        {
            Debug.Assert(element != null);
#if DEBUG
            ++mDepth;
#endif

            List<CssSelector> matchedSelectors = null;
            foreach (CssSelectorMatcher selectorMatcher in mSelectorMatchers)
            {
                bool matches = selectorMatcher.Push(element);
                if (matches)
                {
                    if (matchedSelectors == null)
                        matchedSelectors =  new List<CssSelector>(mSelectorMatchers.Length);

                    matchedSelectors.Add(selectorMatcher.Selector);
                }
            }

            return matchedSelectors;
        }

        internal void Pop()
        {
#if DEBUG
            Debug.Assert(mDepth > 0);
            --mDepth;
#endif
            foreach (CssSelectorMatcher selectorMatcher in mSelectorMatchers)
            {
                selectorMatcher.Pop();
            }
        }

        internal CssDeclarationCollection Declarations
        {
            get { return mDeclarations; }
        }

#if DEBUG
        // Ensures push/pop calls are balanced. Used for debug purposes.
        private int mDepth;
#endif
        private readonly CssSelectorMatcher[] mSelectorMatchers;
        private readonly CssDeclarationCollection mDeclarations;
    }
}
