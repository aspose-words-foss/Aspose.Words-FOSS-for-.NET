// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/01/2025 by Victor Chebotok

using System.Text;
using Aspose.Common;
using Aspose.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    internal class CssHashValue : CssValue
    {
        internal CssHashValue(string text)
            : base(CssValueType.Hash, text)
        {
            Text = text;
        }

        internal static CssHashValue FromColor(DrColor color)
        {
            return FromColor(color.ToArgb());
        }

        internal static CssHashValue FromColor(int argb)
        {
            int b = argb & 0xFF;
            argb >>= 8;
            int g = argb & 0xFF;
            argb >>= 8;
            int r = argb & 0xFF;
            return FromColor(r, g, b);
        }

        internal static CssHashValue FromColor(int r, int g, int b)
        {
            return new CssHashValue(
                FormatterPal.IntToStrX2Lower(r & 0xFF) +
                FormatterPal.IntToStrX2Lower(g & 0xFF) +
                FormatterPal.IntToStrX2Lower(b & 0xFF));
        }

        internal override void ToCss(StringBuilder sb)
        {
            sb.Append('#');
            sb.Append(Text);
        }

        internal override DrColor ParseAsColor()
        {
            return CssColorParser.ParseHexColor(Text);
        }

        internal string Text { get; }

        protected override bool DoEquals(CssValue other)
        {
            return Text == ((CssHashValue)other).Text;
        }
    }
}
