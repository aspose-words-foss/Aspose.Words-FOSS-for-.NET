// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/09/2015 by Alexey Butalov

using System.Collections.Generic;
using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Implements a shorthand property value related to edges of a box, 
    /// like border-style, margin or padding, always use a consistent 1-to-4-value syntax representing edges.
    /// </summary>
    internal class CssShorthand14PropertyValue : CssPropertyValue
    {
        internal CssShorthand14PropertyValue(
            CssPropertyValue topValue,
            CssPropertyValue rightValue,
            CssPropertyValue bottomValue,
            CssPropertyValue leftValue)
            : base(new CssValueList(
                topValue.FirstValue,
                rightValue.FirstValue,
                bottomValue.FirstValue,
                leftValue.FirstValue))
        {
            Debug.Assert(topValue.Count == 1);
            Debug.Assert(rightValue.Count == 1);
            Debug.Assert(bottomValue.Count == 1);
            Debug.Assert(leftValue.Count == 1);

            mTopValue = topValue.FirstValue;
            mRightValue = rightValue.FirstValue;
            mBottomValue = bottomValue.FirstValue;
            mLeftValue = leftValue.FirstValue;
        }

        internal CssShorthand14PropertyValue(
            double topValue,
            double rightValue,
            double bottomValue,
            double leftValue,
            CssUnit unitType)
            : this(
                new CssPropertyValue(new CssLengthValue(topValue, unitType)),
                new CssPropertyValue(new CssLengthValue(rightValue, unitType)),
                new CssPropertyValue(new CssLengthValue(bottomValue, unitType)),
                new CssPropertyValue(new CssLengthValue(leftValue, unitType)))
        {
            // Empty constructor.
        }

        protected override void ValueToCss(StringBuilder sb)
        {
            List<CssValue> values = new List<CssValue>();
            if (mTopValue.Equals(mRightValue) && mTopValue.Equals(mBottomValue) && mTopValue.Equals(mLeftValue))
            {
                values.Add(mTopValue);
            }
            else if (mTopValue.Equals(mBottomValue) && mRightValue.Equals(mLeftValue))
            {
                values.Add(mTopValue);
                values.Add(mRightValue);
            }
            else if (mRightValue.Equals(mLeftValue))
            {
                values.Add(mTopValue);
                values.Add(mRightValue);
                values.Add(mBottomValue);
            }
            else
            {
                values.Add(mTopValue);
                values.Add(mRightValue);
                values.Add(mBottomValue);
                values.Add(mLeftValue);
            }

            bool firstValue = true;
            foreach (CssValue value in values)
            {
                if (!firstValue)
                    sb.Append(' ');
                value.ToCss(sb);
                firstValue = false;
            }
        }

        private readonly CssValue mTopValue;
        private readonly CssValue mRightValue;
        private readonly CssValue mBottomValue;
        private readonly CssValue mLeftValue;
    }
}
