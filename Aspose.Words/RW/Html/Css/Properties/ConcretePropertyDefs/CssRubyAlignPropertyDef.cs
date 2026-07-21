// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/10/2015 by Anton Savko

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'ruby-align' CSS property.
    /// </summary>
    internal class CssRubyAlignPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssRubyAlignPropertyDef()
            : base(
                "ruby-align",
                true,
                CssValue.Auto,
                CssValueFilter.Values(
                    CssValue.Auto,
                    CssValue.Left,
                    CssValue.Center,
                    CssValue.Right,
                    CssValue.DistributeLetter,
                    CssValue.DistributeSpace,
                    CssValue.LineEdge))
        {
            // Empty constructor.
        }
    }
}
