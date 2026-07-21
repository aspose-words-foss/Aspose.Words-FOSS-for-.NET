// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/06/2019 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS rule: either style or at-rule.
    /// </summary>
    internal abstract class CssRule
    {
        protected CssRule(CssRuleType type)
        {
            Type = type;
        }

        internal CssRuleType Type { get; }

        internal abstract string ToCss();

#if DEBUG
        public override string ToString()
        {
            return ToCss();
        }
#endif
    }
}
