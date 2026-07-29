// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents an exponentiation operator.
    /// </summary>
    internal class ExponentiationOperator : BinaryOperator
    {
        protected override Constant EvaluateCore(DoubleConstant operand1, DoubleConstant operand2)
        {
            // If field format does not specified, MS Word uses sum numbers of decimal places from arguments of exponentiation operator for result.
            int numberOfDecimalPlaces = operand1.NumberOfDigitsAfterDecimalPoint + operand2.NumberOfDigitsAfterDecimalPoint;
            return DoubleConstant.CreateFrom(operand1, operand2, System.Math.Pow(operand1.ValueDouble, operand2.ValueDouble), numberOfDecimalPlaces);
        }

        internal override int Order
        {
            get { return 2; }
        }
    }
}
