// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/10/2015 by Alexey Butalov

using System.Text;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS degree value such as 90deg.
    /// </summary>
    internal class CssDegreeValue : CssValue
    {
        internal CssDegreeValue(double value)
            : base(CssValueType.Degree, value)
        {
            mValue = value;
        }

        internal override void ToCss(StringBuilder sb)
        {
            sb.AppendFormat("{0}deg", FormatterPal.DoubleToStr2Decimals(mValue));
        }

        protected override bool DoEquals(CssValue other)
        {
            if (!(other is CssDegreeValue))
                return false;

            const double tolerance = 0.005; // This accuracy is enough in HTML.
            CssDegreeValue otherDegree = (CssDegreeValue)other;
            return MathUtil.AreEqual(mValue, otherDegree.mValue, tolerance);
        }

        internal override double DoubleValue
        {
            get { return mValue; }
        }

        private readonly double mValue;
    }
}