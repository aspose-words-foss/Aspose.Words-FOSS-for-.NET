// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2019 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS @import rule.
    /// </summary>
    internal class CssImportRule : CssRule
    {
        internal CssImportRule(
            string url,
            CssMediaQueryList mediaQuery)
            : base(CssRuleType.Import)
        {
            Debug.Assert(StringUtil.HasChars(url));
            Debug.Assert(mediaQuery != null);

            Url = url;
            MediaQuery = mediaQuery;
        }

        internal string Url { get; }

        internal CssMediaQueryList MediaQuery { get; }

        internal override string ToCss()
        {
            return "@import url('" + Url + "') " + MediaQuery + ";";
        }
    }
}
