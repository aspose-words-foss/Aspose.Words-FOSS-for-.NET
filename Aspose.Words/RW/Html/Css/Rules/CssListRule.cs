// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/07/2024 by Anton Savko

using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS list at-rule.
    /// </summary>
    internal class CssListRule : CssRule
    {
        internal CssListRule(
            CssListRulePrelude prelude,
            CssDeclarationCollection declarations)
            : base(CssRuleType.List)
        {
            Debug.Assert(prelude != null);
            Debug.Assert(declarations != null);
            Prelude = prelude;
            Declarations = declarations;
        }

        internal override string ToCss()
        {
            StringBuilder cssBuilder = new StringBuilder();
            cssBuilder.Append(Prelude.ToCss());
            cssBuilder.Append(" { ");
            cssBuilder.Append(Declarations.GetShorthandVersion().ToCss());
            cssBuilder.Append(" }");
            return cssBuilder.ToString();
        }

        internal CssListRulePrelude Prelude { get; }

        internal CssDeclarationCollection Declarations { get; }
    }
}
