// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/02/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'orphans' CSS property.
    /// </summary>
    internal class CssOrphansPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssOrphansPropertyDef()
            : base(
                "orphans",
                true,
                new CssNumberValue(2),
                CssValueFilter.Number)
        {
            // Empty constructor.
        }
    }
}
