// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/06/2019 by Victor Chebotok

using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS @media rule.
    /// </summary>
    /// <remarks>
    /// See https://www.w3.org/TR/CSS2/media.html#at-media-rule
    /// Nested rules are style, @page, or other @media rules.
    /// </remarks>
    internal class CssMediaRule : CssRule
    {
        internal CssMediaRule(
            CssMediaQueryList mediaQueryList,
            CssRule[] rules)
            : base(CssRuleType.Media)
        {
            Debug.Assert(mediaQueryList != null);
            Debug.Assert(rules != null);

            MediaQueryList = mediaQueryList;
            Rules = rules;
        }

        internal CssMediaQueryList MediaQueryList { get; }

        internal CssRule[] Rules { get; }

        internal override string ToCss()
        {
            StringBuilder result = new StringBuilder();
            result.Append("@media ");
            result.Append(MediaQueryList);
            result.Append(" { ");
            bool isFirstRule = true;
            foreach (CssRule rule in Rules)
            {
                if (!isFirstRule)
                {
                    result.Append(' ');
                }
                isFirstRule = false;
                result.Append(rule.ToCss());
            }
            result.Append(" }");
            return result.ToString();
        }
    }
}
