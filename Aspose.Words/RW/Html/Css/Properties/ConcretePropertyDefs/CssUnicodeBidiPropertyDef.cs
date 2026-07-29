// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/02/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'unicode-bidi' CSS property.
    /// </summary>
    internal class CssUnicodeBidiPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssUnicodeBidiPropertyDef()
            : base(
                "unicode-bidi",
                false,
                CssValue.Normal,
                // normal | embed | bidi-override
                CssValueFilter.Values(CssValue.Normal, CssValue.Embed, CssValue.BidiOverride))
        {
            // Empty constructor.
        }
    }
}
