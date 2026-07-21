// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/06/2017 by Victor Chebotok

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Stores CSS style rules that match an HTML element. Rules are grouped by element parts they apply to.
    /// </summary>
    internal class CssMatchedRules
    {
        internal void Add(IList<CssSelector> selectors, CssDeclarationCollection declarations)
        {
            PickSelectorsAndAddRule(HtmlElementPart.Element, selectors, declarations);
            PickSelectorsAndAddRule(HtmlElementPart.Before, selectors, declarations);
            PickSelectorsAndAddRule(HtmlElementPart.After, selectors, declarations);
            // Other pseudo-element types are not supported at the moment.
        }

        /// <summary>
        /// Gets a list of <see cref="CssSelector"/> that select only the given part of this HTML element.
        /// </summary>
        internal IList<CssSelector> GetSelectors(HtmlElementPart part)
        {
            IList<CssSelector> result = new List<CssSelector>();
            IList<CssStyleRule> rules = GetRules(part);
            if (rules != null)
            {
                foreach (CssStyleRule elementRule in rules)
                {
                    foreach (CssSelector selector in elementRule.Selectors)
                    {
                        result.Add(selector);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets CSS rules that apply to the given part of this HTML element.
        /// </summary>
        internal IList<CssStyleRule> GetRules(HtmlElementPart part)
        {
            List<CssStyleRule> rules;
            if (part == HtmlElementPart.Element)
            {
                rules = mElementRules;
            }
            else if (mPseudoElementRules != null)
            {
                rules = mPseudoElementRules.GetValueOrNull(part);
                if (rules == null)
                {
                    rules = gEmptyRules;
                }
            }
            else
            {
                rules = gEmptyRules;
            }
            return rules;
        }

        private void PickSelectorsAndAddRule(
            HtmlElementPart part,
            IList<CssSelector> selectors,
            CssDeclarationCollection declarations)
        {
            List<CssSelector> pickedSelectors = new List<CssSelector>();

            // Pick selectors that match the given part of the element.
            foreach (CssSelector selector in selectors)
            {
                if (selector.SelectedPart == part)
                {
                    pickedSelectors.Add(selector);
                }
            }

            // Create and store a new rule containing only selectors that match the given part of the element.
            if (pickedSelectors.Count > 0)
            {
                CssSelector[] pickedSelectorsArray = pickedSelectors.ToArray();
                AddRule(part, new CssStyleRule(pickedSelectorsArray, declarations));
            }
        }

        private void AddRule(HtmlElementPart part, CssStyleRule rule)
        {
            if (part == HtmlElementPart.Element)
            {
                mElementRules.Add(rule);
            }
            else
            {
                if (mPseudoElementRules == null)
                {
                    mPseudoElementRules = new Dictionary<HtmlElementPart, List<CssStyleRule>>();
                }
                List<CssStyleRule> rules = mPseudoElementRules.GetValueOrNull(part);
                if (rules == null)
                {
                    rules = new List<CssStyleRule>();
                    mPseudoElementRules[part] = rules;
                }
                rules.Add(rule);
            }
        }

        internal static readonly CssMatchedRules Empty = new CssMatchedRules();

        private static readonly List<CssStyleRule> gEmptyRules = new List<CssStyleRule>(0);

        private readonly List<CssStyleRule> mElementRules = new List<CssStyleRule>();

        /// <summary>
        /// Most HTML elements don't have visible pseudo-elements. This field is lazily initialized when needed.
        /// </summary>
        private Dictionary<HtmlElementPart, List<CssStyleRule>> mPseudoElementRules;
    }
}
