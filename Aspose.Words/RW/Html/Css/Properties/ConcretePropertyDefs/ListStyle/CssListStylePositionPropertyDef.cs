// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2014 by Anton Savko

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'list-style-position' CSS property.
    /// </summary>
    internal class CssListStylePositionPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssListStylePositionPropertyDef()
            : base(
                "list-style-position",
                true,
                CssValue.Outside,
                CssValueFilter.Values(
                    CssValue.Inside,
                    CssValue.Outside))
        {
            // Empty constructor.
        }
    }
}
