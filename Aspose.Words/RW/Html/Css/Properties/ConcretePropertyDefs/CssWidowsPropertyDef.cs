// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'widows' CSS property.
    /// </summary>
    internal class CssWidowsPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssWidowsPropertyDef()
            : base(
                "widows",
                true,
                new CssNumberValue(2),
                CssValueFilter.Number)
        {
            // Empty constructor.
        }
    }
}
