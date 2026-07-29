// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents an unary minus operator.
    /// </summary>
    internal class UnaryMinusOperator : UnaryOperator
    {
        internal UnaryMinusOperator()
            : this(false)
        {
        }

        internal UnaryMinusOperator(bool applyParenthesesNegativeStyle)
        {
            mApplyParenthesesNegativeStyle = applyParenthesesNegativeStyle;
        }

        protected override Constant EvaluateCore(DoubleConstant operand)
        {
            // If field format does not specified, MS Word uses number of decimal places from argument of minus operator for result.
            DoubleConstant result = DoubleConstant.CreateFrom(operand, -operand.ValueDouble, operand.NumberOfDigitsAfterDecimalPoint);

            if (mApplyParenthesesNegativeStyle)
                result.IsUsesNegativeParentheses = true;

            return result;
        }

        internal override int Order
        {
            get { return 1; }
        }

        internal override bool IsUnaryMinus
        {
            get { return true; }
        }

        private readonly bool mApplyParenthesesNegativeStyle;
    }
}
