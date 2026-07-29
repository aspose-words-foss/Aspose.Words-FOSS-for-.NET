// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2013 by Alexey Butalov

using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a comma (,) value.
    /// </summary>
    internal class CssCommaValue : CssValue
    {
        internal CssCommaValue()
            : base(CssValueType.Comma, CommaChar)
        {
            // Empty constructor.
        }

        internal override void ToCss(StringBuilder sb)
        {
            sb.Append(CommaChar);
        }

        private const char CommaChar = ',';
    }
}
