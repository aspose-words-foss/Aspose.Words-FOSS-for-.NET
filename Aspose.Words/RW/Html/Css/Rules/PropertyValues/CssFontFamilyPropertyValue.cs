// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/04/2013 by Alexey Butalov

using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS font family property value.
    /// </summary>
    internal class CssFontFamilyPropertyValue : CssPropertyValue
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal CssFontFamilyPropertyValue(CssValue fontFamily)
            : base(fontFamily)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal CssFontFamilyPropertyValue(params CssValue[] fontFamilies)
            : base(new CssValueList(fontFamilies))
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal CssFontFamilyPropertyValue(CssValueList fontFamilies)
            : base(fontFamilies)
        {
        }

        protected override void ValueToCss(StringBuilder sb)
        {
            const string delimiter = ", ";
            for (int i = 0; i < Count; i++)
            {
                if (i != 0)
                    sb.Append(delimiter);
                this[i].ToCss(sb);
            }
        }
    }
}
