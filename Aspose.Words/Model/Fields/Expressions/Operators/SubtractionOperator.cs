// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a subtraction operator.
    /// </summary>
    internal class SubtractionOperator : BinaryOperator
    {
        protected override Constant EvaluateCore(DoubleConstant operand1, DoubleConstant operand2)
        {
            // If field format does not specified, MS Word uses maximum number of decimal places from arguments of subtraction operator for result.
            int numberOfDecimalPlaces = System.Math.Max(operand1.NumberOfDigitsAfterDecimalPoint,
                                                        operand2.NumberOfDigitsAfterDecimalPoint);
            return DoubleConstant.CreateFrom(operand1, operand2, operand1.ValueDouble - operand2.ValueDouble, numberOfDecimalPlaces);
        }

        internal override int Order
        {
            get { return 4; }
        }
    }
}
