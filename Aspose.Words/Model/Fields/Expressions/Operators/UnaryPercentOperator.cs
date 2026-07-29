// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents an unary percent operator.
    /// </summary>
    internal class UnaryPercentOperator : UnaryOperator
    {
        protected override Constant EvaluateCore(DoubleConstant operand)
        {
            // If field format does not specified, MS Word uses number of decimal places from argument of percent operator adding 2 for result.
            return DoubleConstant.CreateFrom(operand, operand.ValueDouble * 0.01, operand.NumberOfDigitsAfterDecimalPoint + 2);
        }

        internal override int Order
        {
            get { return 1; }
        }
    }
}
