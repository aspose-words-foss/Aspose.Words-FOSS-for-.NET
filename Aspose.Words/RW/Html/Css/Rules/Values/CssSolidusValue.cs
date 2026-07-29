// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2013 by Alexey Butalov

using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a U+002F SOLIDUS (/) value.
    /// </summary>
    internal class CssSolidusValue : CssValue
    {
        internal CssSolidusValue()
            : base(CssValueType.Solidus, SolidusChar)
        {
            // Empty constructor.
        }

        internal override void ToCss(StringBuilder sb)
        {
            sb.Append(SolidusChar);
        }

        private const char SolidusChar = '/';
    }
}
