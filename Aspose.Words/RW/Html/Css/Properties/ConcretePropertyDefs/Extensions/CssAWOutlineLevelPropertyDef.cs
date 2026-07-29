// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2023 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    internal class CssAWOutlineLevelPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssAWOutlineLevelPropertyDef()
            : base(
                HtmlConstants.OutlineLevel,
                false,
                CssValue.Zero,
                // 0..9
                CssValueFilter.NonNegativeNumber)
        {
            // Empty constructor.
        }
    }
}
