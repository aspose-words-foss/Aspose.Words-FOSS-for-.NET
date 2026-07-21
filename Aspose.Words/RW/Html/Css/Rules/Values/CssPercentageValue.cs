// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/02/2013 by Alexey Butalov

using System.Text;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents an identifier CSS percentage value.
    /// </summary>
    internal class CssPercentageValue : CssValue
    {
        internal CssPercentageValue(double value)
            : base(CssValueType.Percentage, value)
        {
            mValue = value;
        }

        internal override void ToCss(StringBuilder sb)
        {
            sb.AppendFormat("{0}%", FormatterPal.DoubleToStr2Decimals(mValue));
        }

        internal override double DoubleValue
        {
            get { return mValue; }
        }

        private readonly double mValue;
    }
}