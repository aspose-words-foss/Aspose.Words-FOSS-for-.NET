// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/08/2022 by Victor Chebotok

using System;
using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Stack of CSS rule lists used by our CSS parser.
    /// </summary>
    /// <remarks>
    /// This class is used for parsing content of nested @media rules in an iterative manner.
    /// </remarks>
    internal class CssRuleListStack
    {
        internal void BeginNestedMediaRule(CssMediaQueryList mediaQueryList)
        {
            if (nestedRuleLists == null)
                nestedRuleLists = new Stack<List<CssRule>>();
            if (nestedMediaRuleQueries == null)
                nestedMediaRuleQueries = new Stack<CssMediaQueryList>();

            List<CssRule> mediaRuleNestedRules = new List<CssRule>();
            nestedRuleLists.Push(mediaRuleNestedRules);
            nestedMediaRuleQueries.Push(mediaQueryList);
        }

        internal void EndNestedMediaRule()
        {
            if ((nestedRuleLists.Count == 0) || (nestedMediaRuleQueries.Count == 0))
            {
                throw new InvalidOperationException("The stack of nested rule lists is empty.");
            }

            nestedMediaRuleQueries.Pop();
            nestedRuleLists.Pop();
        }

        internal bool IsTopLevel
        {
            get { return (nestedRuleLists == null) || (nestedRuleLists.Count == 0); }
        }

        internal List<CssRule> CurrentRuleList
        {
            get
            {
                return IsTopLevel
                    ? mTopLevelRuleList
                    : nestedRuleLists.Peek();
            }
        }

        internal CssMediaQueryList CurrentMediaQueryList
        {
            get
            {
                return IsTopLevel
                    ? null
                    : nestedMediaRuleQueries.Peek();
            }
        }

        private readonly List<CssRule> mTopLevelRuleList = new List<CssRule>();
        private Stack<List<CssRule>> nestedRuleLists = null;
        private Stack<CssMediaQueryList> nestedMediaRuleQueries = null;
    }
}
