// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/05/2014 by Alexey Butalov

using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html
{
    /// <summary>
    /// Represents four (top, right, bottom and left) CSS box dimensions measured in points.
    /// </summary>
    internal abstract class CssDimensions
    {
        protected CssDimensions(string edgeCssPropertyPrefix)
        {
            Debug.Assert(StringUtil.HasChars(edgeCssPropertyPrefix));
            mEdgeCssPropertyPrefix = edgeCssPropertyPrefix;
        }

        protected CssDimensions(CssDimensions other)
        {
            mEdgeCssPropertyPrefix = other.mEdgeCssPropertyPrefix;
            mTop = other.Top;
            mUseTop = other.UseTop;
            mRight = other.Right;
            mUseRight = other.UseRight;
            mBottom = other.Bottom;
            mUseBottom = other.UseBottom;
            mLeft = other.Left;
            mUseLeft = other.UseLeft;
        }

        public abstract object Clone();

        /// <summary>
        /// Converts logical dimensions to phisical dimensions.
        /// The physical dimensions always stay the same (top, right, bottom and left), while the logical dimensions change depending on writing mode, 
        /// direction and text orientation.
        /// </summary>
        /// <param name="blockFlowDirection">Block flow direction.</param>
        /// <param name="rtl">Indicates whether text direction is right-to-left.</param>
        /// <returns>A new instance of the class.</returns>
        protected CssDimensions ToPhisical(CssBlockFlowDirection blockFlowDirection, bool rtl)
        {
            CssDimensions phisicalBox = (CssDimensions)Clone();
            switch (blockFlowDirection)
            {
                case CssBlockFlowDirection.HorizontalTb:
                {
                    if (rtl)
                    {
                        phisicalBox.Right = mLeft;
                        phisicalBox.UseRight = mUseLeft;
                        phisicalBox.Left = mRight;
                        phisicalBox.UseLeft = mUseRight;
                    }
                    break; 
                }
                case CssBlockFlowDirection.VerticalRl:
                case CssBlockFlowDirection.VerticalLr:
                {
                    phisicalBox.Top = (rtl) ? mRight : mLeft;
                    phisicalBox.UseTop = (rtl) ? mUseRight : mUseLeft;
                    phisicalBox.Right = mTop;
                    phisicalBox.UseRight = mUseTop;
                    phisicalBox.Bottom = (rtl) ? mLeft : mRight;
                    phisicalBox.UseBottom = (rtl) ? mUseLeft : mUseRight;
                    phisicalBox.Left = mBottom;
                    phisicalBox.UseLeft = mUseBottom;
                    break;
                }
                default:
                {
                    Debug.Assert(false, "Unknown value!");
                    break;
                }
            }

            return phisicalBox;
        }

        /// <summary>
        /// Converts phisical dimensions to logical dimensions.
        /// The physical dimensions always stay the same (top, right, bottom and left), while the logical dimensions change depending on writing mode, 
        /// direction and text orientation.
        /// </summary>
        /// <param name="blockFlowDirection">Block flow direction.</param>
        /// <param name="rtl">Indicates whether text direction is right-to-left.</param>
        /// <returns>A new instance of the class.</returns>
        protected CssDimensions ToLogical(CssBlockFlowDirection blockFlowDirection, bool rtl)
        {
            CssDimensions logicalBox = (CssDimensions)Clone();
            switch (blockFlowDirection)
            {
                case CssBlockFlowDirection.HorizontalTb:
                {
                    if (rtl)
                    {
                        logicalBox.Right = mLeft;
                        logicalBox.UseRight = mUseLeft;
                        logicalBox.Left = mRight;
                        logicalBox.UseLeft = mUseRight;
                    }
                    break;
                }
                case CssBlockFlowDirection.VerticalRl:
                case CssBlockFlowDirection.VerticalLr:
                {
                    logicalBox.Top = mRight;
                    logicalBox.UseTop = mUseRight;
                    logicalBox.Right = (rtl) ? mTop : mBottom;
                    logicalBox.UseRight = (rtl) ? mUseTop : mUseBottom;
                    logicalBox.Bottom = mLeft;
                    logicalBox.UseBottom = mUseLeft;
                    logicalBox.Left = (rtl) ? mBottom : mTop;
                    logicalBox.UseLeft = (rtl) ? mUseBottom : mUseTop;
                    break;
                }
                default:
                {
                    Debug.Assert(false, "Unknown value!");
                    break;
                }
            }

            return logicalBox;
        }

        internal void ToStyle(CssStyle style)
        {
            if (mUseTop)
                style.SetLength(mEdgeCssPropertyPrefix + "top", mTop, CssUnit.Pt);
            if (mUseRight)
                style.SetLength(mEdgeCssPropertyPrefix + "right", mRight, CssUnit.Pt);
            if (mUseBottom)
                style.SetLength(mEdgeCssPropertyPrefix + "bottom", mBottom, CssUnit.Pt);
            if (mUseLeft)
                style.SetLength(mEdgeCssPropertyPrefix + "left", mLeft, CssUnit.Pt);
        }

        protected void FromStyle(CssStyle style)
        {
            Debug.Assert(style != null);
            CssDeclarationCollection declarations = style.GetDeclarations();
            double topValue = declarations.GetLength(mEdgeCssPropertyPrefix + "top");
            if (!MathUtil.IsMinValue(topValue))
            {
                Top = topValue;
            }
            double rightValue = declarations.GetLength(mEdgeCssPropertyPrefix + "right");
            if (!MathUtil.IsMinValue(rightValue))
            {
                Right = rightValue;
            }
            double bottomValue = declarations.GetLength(mEdgeCssPropertyPrefix + "bottom");
            if (!MathUtil.IsMinValue(bottomValue))
            {
                Bottom = bottomValue;
            }
            double leftValue = declarations.GetLength(mEdgeCssPropertyPrefix + "left");
            if (!MathUtil.IsMinValue(leftValue))
            {
                Left = leftValue;
            }
        }

        internal double Top
        {
            get { return mTop; }
            set
            {
                mTop = value;
                mUseTop = true;
            }
        }

        internal bool UseTop
        {
            get { return mUseTop; }
            set { mUseTop = value; }
        }

        internal double Right
        {
            get { return mRight; }
            set
            {
                mRight = value;
                mUseRight = true;
            }
        }

        internal bool UseRight
        {
            get { return mUseRight; }
            set { mUseRight = value; }
        }

        internal double Bottom
        {
            get { return mBottom; }
            set
            {
                mBottom = value;
                mUseBottom = true;
            }
        }

        internal bool UseBottom
        {
            get { return mUseBottom; }
            set { mUseBottom = value; }
        }

        internal double Left
        {
            get { return mLeft; }
            set
            {
                mLeft = value;
                mUseLeft = true;
            }
        }

        internal bool UseLeft
        {
            get { return mUseLeft; }
            set { mUseLeft = value; }
        }

        internal bool UseAll
        {
            get { return mUseTop && mUseRight && mUseBottom && mUseLeft; }
        }

        private readonly string mEdgeCssPropertyPrefix;
        private double mTop;
        private bool mUseTop;
        private double mRight;
        private bool mUseRight;
        private double mBottom;
        private bool mUseBottom;
        private double mLeft;
        private bool mUseLeft;
    }
}
