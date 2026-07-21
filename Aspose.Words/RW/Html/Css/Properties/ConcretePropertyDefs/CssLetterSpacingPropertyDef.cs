// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/04/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'letter-spacing' CSS property.
    /// </summary>
    internal class CssLetterSpacingPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssLetterSpacingPropertyDef()
            : base(
                  "letter-spacing",
                  true,
                  CssValue.Normal,
                  // normal | <length>
                  CssValueFilter.AnyOf(
                      CssValueFilter.Value(CssValue.Normal),
                      CssValueFilter.QuirkyLength))
        {
            // Empty constructor.
        }
    }
}
