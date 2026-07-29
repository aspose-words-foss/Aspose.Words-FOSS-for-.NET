// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/01/2018 by Nikolay Sezganov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS dash array property value.
    /// </summary>
    internal class CssDashArrayPropertyValue : CssPropertyValue
    {
        internal CssDashArrayPropertyValue(CssValue value)
            : base(value)
        {
            // Empty constructor.
        }

        internal CssDashArrayPropertyValue(CssValueList values)
            : base(values)
        {
            // Empty constructor.
        }

        internal CssDashArrayPropertyValue(CssPropertyValue value)
            : base(value)
        {
            // Empty constructor.
        }

        /// <summary>
        /// 'none' dash array.
        /// </summary>
        internal static readonly CssDashArrayPropertyValue None = new CssDashArrayPropertyValue(CssValue.None);
    }
}
