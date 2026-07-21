// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/01/2013 by Alexey Butalov

using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS style rule. A style rule has two main parts: one or more selectors, and one or more declarations.
    /// </summary>
    internal class CssStyleRule : CssRule
    {
        internal CssStyleRule()
            : base(CssRuleType.Style)
        {
            // Empty constructor.
        }

        internal CssStyleRule(CssSelector selector, CssDeclarationCollection declarations)
            : this(new CssSelector[] { selector }, declarations)
        {
            // Empty constructor.
        }

        internal CssStyleRule(CssSelector[] selectors, CssDeclarationCollection declarations)
            : this()
        {
            Debug.Assert(selectors != null);
            Debug.Assert(selectors.Length > 0);
            Debug.Assert(declarations != null);

            Selectors = selectors;
            Declarations = declarations;
        }

        internal CssStyleRule(CssSelector selector, CssStyle style)
            : this(selector, style.GetDeclarations())
        {
            // Empty constructor.
        }

        internal CssStyleRule(CssSelector[] selectors, CssStyle style)
            : this(selectors, style.GetDeclarations())
        {
            // Empty constructor.
        }

        /// <summary>
        /// Rule selector.
        /// </summary>
        internal CssSelector[] Selectors { get; }

        /// <summary>
        /// Rule declarations.
        /// </summary>
        internal CssDeclarationCollection Declarations { get; }

        /// <summary>
        /// Gets the CSS declaration of the rule. The returned declaration is compatible with CSS 2.1.
        /// </summary>
        internal override string ToCss()
        {
            StringBuilder result = new StringBuilder();
            foreach (CssSelector selector in Selectors)
            {
                if (result.Length > 0)
                {
                    result.Append(", ");
                }
                result.Append(selector.ToCss());
            }

            result.Append(" { ");
            result.Append(Declarations.GetShorthandVersion().ToCss());
            result.Append(" }");
            return result.ToString();
        }
    }
}
