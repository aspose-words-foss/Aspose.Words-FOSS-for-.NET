// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/05/2013 by Alexey Butalov

using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS @page rule.
    /// </summary>
    /// <remarks>
    /// Authors can specify various aspects of a page box, such as its dimensions, orientation, and margins, within an @page
    /// rule. @page rules are allowed wherever rule-sets are allowed. An @page rule consists of the keyword ‘@page’, an optional
    /// comma-separated list of page selectors and a block of declarations. An @page rule can also contain other at-rules, 
    /// interleaved between declarations. http://www.w3.org/TR/css3-page/#at-page-rule
    /// </remarks>
    internal class CssPageRule : CssRule
    {
        /// <summary>
        /// Constructor. Initializes a new instance of the class.
        /// </summary>
        /// <param name="pageSelector">Selector of the @page rule.</param>
        /// <param name="declarations">Declarations of the @page rule. Nested at-rules are not supported.</param>
        internal CssPageRule(CssPageSelector pageSelector, CssDeclarationCollection declarations)
            : base(CssRuleType.Page)
        {
            Debug.Assert(pageSelector != null);
            Debug.Assert(declarations != null);

            Selector = pageSelector;
            Declarations = declarations;
        }

        /// <summary>
        /// Rule selector.
        /// </summary>
        internal CssPageSelector Selector { get; }

        /// <summary>
        /// Rule declarations.
        /// </summary>
        internal CssDeclarationCollection Declarations { get; }

        /// <summary>
        /// Gets the CSS declaration of the rule.
        /// </summary>
        internal override string ToCss()
        {
            StringBuilder css = new StringBuilder();
            css.Append("@page");

            string selectorCss = Selector.ToCss();
            if (StringUtil.HasChars(selectorCss))
            {
                css.Append(' ');
                css.Append(selectorCss);
            }

            css.Append(" { ");
            css.Append(Declarations.GetShorthandVersion().ToCss());
            css.Append(" }");
            return css.ToString();
        }
    }
}
