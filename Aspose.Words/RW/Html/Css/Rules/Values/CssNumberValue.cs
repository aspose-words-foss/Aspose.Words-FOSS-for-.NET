// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/02/2013 by Alexey Butalov

using System.Text;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents an identifier CSS number value.
    /// </summary>
    internal class CssNumberValue : CssValue
    {
        internal CssNumberValue(double value)
            : base(CssValueType.Number, value)
        {
            mValue = value;
        }

        internal override void ToCss(StringBuilder sb)
        {
            sb.Append(FormatterPal.DoubleToStr2Decimals(mValue));
        }

        internal override double DoubleValue
        {
            get { return mValue; }
        }

        internal double QuirkyLengthToPoint()
        {
            return new CssLengthValue(mValue, CssUnit.Px).GetLength(CssUnit.Pt);
        }

        private readonly double mValue;
    }
}
