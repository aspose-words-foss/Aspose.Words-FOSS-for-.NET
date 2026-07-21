// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/02/2013 by Alexey Butalov

using System.Text;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents an identifier CSS string value.
    /// </summary>
    internal class CssStringValue : CssValue
    {
        internal CssStringValue(string value)
            : base(CssValueType.String, value)
        {
            mValue = value;
        }

        internal override void ToCss(StringBuilder sb)
        {
            sb.Append('\'');
            sb.Append(CssEscape.EscapeSingleQuotedString(mValue));
            sb.Append('\'');
        }

        internal new string Value
        {
            get { return mValue; }
        }

        private readonly string mValue;
    }
}
