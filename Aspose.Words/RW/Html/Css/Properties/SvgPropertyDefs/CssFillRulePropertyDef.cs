// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/04/2020 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements the 'fill-rule' CSS property for SVG.
    /// </summary>
    internal class CssFillRulePropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssFillRulePropertyDef()
            : base(
                  "fill-rule",
                  true,
                  CssValue.Nonzero,
                  // nonzero | evenodd
                  CssValueFilter.Values(CssValue.Evenodd, CssValue.Nonzero))
        {
            // Empty constructor.
        }
    }
}
