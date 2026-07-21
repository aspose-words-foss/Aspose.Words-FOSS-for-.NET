// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/02/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements 'background-repeat' CSS property.
    /// </summary>
    internal class CssBackgroundRepeatPropertyDef : CssIndividualSimplePropertyDef
    {
        internal CssBackgroundRepeatPropertyDef()
            : base(
                "background-repeat",
                false,
                CssValue.Repeat,
                // repeat | repeat-x | repeat-y | no-repeat
                CssValueFilter.Values(
                    CssValue.Repeat,
                    CssValue.RepeatX,
                    CssValue.RepeatY,
                    CssValue.NoRepeat))
        {
            // Empty constructor.
        }
    }
}
