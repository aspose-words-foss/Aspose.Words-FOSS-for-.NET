// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/06/2017 by Victor Chebotok

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Matches a set of <see cref="CssStyleRule"/> against HTML elements during depth-first traversal of an HTML tree.
    /// </summary>
    internal class CssMatcher
    {
        internal CssMatcher(ICollection<CssStyleRule> cssRules, CssDocumentMode documentMode)
        {
            // It's important that we initialize the document mode value before we start caching rules with ID selectors.
            // Otherwise, normalization of ID values won't work correctly.
            mDocumentMode = documentMode;

            List<CssRuleMatcher> ruleMatchers = new List<CssRuleMatcher>(cssRules.Count);
            foreach (CssStyleRule rule in cssRules)
            {
                // SPEED - In order to speed up matching of ID selectors, we collect rules matched by each ID selector into
                // a dictionary.
                if (CacheRuleWithIdSelector(rule))
                {
                    // If the rule contains any other selector types in addition to ID selectors, we split the rule into two
                    // parts: one with ID selectors and another with all other selectors.
                    // This is needed to preserve the order of rule matching.
                    if (rule.Selectors.Length > 1)
                    {
                        List<CssSelector> nonIdSelectors = new List<CssSelector>();
                        foreach (CssSelector selector in rule.Selectors)
                        {
                            if (!(selector is CssIdSelector))
                                nonIdSelectors.Add(selector);
                        }
                        if (nonIdSelectors.Count > 0)
                        {
                            CssStyleRule nonIdSelectorsRule = new CssStyleRule(nonIdSelectors.ToArray(), rule.Declarations);
                            ruleMatchers.Add(new CssRuleMatcher(nonIdSelectorsRule, documentMode));
                        }
                    }
                }
                else
                {
                    ruleMatchers.Add(new CssRuleMatcher(rule, documentMode));
                }
            }
            mRuleMatchers = ruleMatchers.ToArray();
        }

        internal CssMatchedRules Push(IElementProvider element)
        {
            Debug.Assert(element != null);
#if DEBUG
            ++mDepth;
#endif
            // SPEED - In order to speed up matching of ID selectors, we match them separately from other types of selectors.
            // For ID selectors, we get matched CSS rules directly by element's ID value from a dictionary.
            CssMatchedRules matchedRules = MatchCssRulesByCssIdSelector(element);

            foreach (CssRuleMatcher ruleMatcher in mRuleMatchers)
            {
                List<CssSelector> matchedSelectors = ruleMatcher.Push(element);
                if (matchedSelectors != null)
                {
                    if (matchedRules == null)
                        matchedRules = new CssMatchedRules();

                    matchedRules.Add(matchedSelectors, ruleMatcher.Declarations);
                }
            }

            if (matchedRules == null)
            {
                matchedRules = CssMatchedRules.Empty;
            }
            return matchedRules;
        }

        internal void Pop()
        {
#if DEBUG
            Debug.Assert(mDepth > 0);
            --mDepth;
#endif
            foreach (CssRuleMatcher ruleMatcher in mRuleMatchers)
            {
                ruleMatcher.Pop();
            }
        }

        private bool CacheRuleWithIdSelector(CssStyleRule rule)
        {
            Debug.Assert(rule.Selectors.Length > 0);
            bool ruleContainsCssIdSelectors = false;
            foreach (CssSelector cssSelector in rule.Selectors)
            {
                if (cssSelector is CssIdSelector)
                {
                    CssIdSelector cssIdSelector = (CssIdSelector)cssSelector;

                    string id = NormalizeElementId(cssIdSelector.Id);
                    List<CssDeclarationCollection> matchedRulesDeclarations;
                    if (!mRulesDeclarationsMatchedByIdSelector.TryGetValue(id, out matchedRulesDeclarations))
                    {
                        matchedRulesDeclarations = new List<CssDeclarationCollection>();
                        mRulesDeclarationsMatchedByIdSelector.Add(id, matchedRulesDeclarations);
                    }
                    matchedRulesDeclarations.Add(rule.Declarations);
                    ruleContainsCssIdSelectors = true;
                }
            }
            return ruleContainsCssIdSelectors;
        }

        private CssMatchedRules MatchCssRulesByCssIdSelector(IElementProvider element)
        {
            CssMatchedRules matchedRules = null;

            string elementId = element.GetAttributeValue("id");
            if (elementId != null)
            {
                elementId = NormalizeElementId(elementId);

                List<CssDeclarationCollection> rulesDeclarationsMatchedById;
                if (mRulesDeclarationsMatchedByIdSelector.TryGetValue(elementId, out rulesDeclarationsMatchedById))
                {
                    matchedRules = new CssMatchedRules();
                    CssIdSelector idSelector = new CssIdSelector(elementId);
                    foreach (CssDeclarationCollection ruleDeclarations in rulesDeclarationsMatchedById)
                    {
                        List<CssSelector> list = new List<CssSelector>() { idSelector };
                        matchedRules.Add(list, ruleDeclarations);
                    }
                }
            }

            return matchedRules;
        }

        private string NormalizeElementId(string id)
        {
            return (mDocumentMode == CssDocumentMode.Quirks)
                ? id.ToLowerInvariant()
                : id;
        }

        private readonly CssRuleMatcher[] mRuleMatchers;

        /// <summary>
        /// List of CSS rules that are matched by certain CSS ID selectors.
        /// </summary>
        /// <remarks>
        /// Key - Value of a CSS ID selector (normalized to lower case if needed).
        /// Value - list of CSS rules matched by that selector.
        /// </remarks>
        private readonly Dictionary<string, List<CssDeclarationCollection>> mRulesDeclarationsMatchedByIdSelector =
            new Dictionary<string, List<CssDeclarationCollection>>();

        private readonly CssDocumentMode mDocumentMode;

#if DEBUG
        // Ensures push/pop calls are balanced. Used for debug purposes.
        private int mDepth;
#endif
    }
}
