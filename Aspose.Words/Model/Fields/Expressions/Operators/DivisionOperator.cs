// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a division operator.
    /// </summary>
    internal class DivisionOperator : BinaryOperator
    {
        protected override Constant EvaluateCore(DoubleConstant operand1, DoubleConstant operand2)
        {
            if (operand2.ValueDouble == 0d)
                return new ErrorConstant("!Zero Divide");

            // If field format does not specified, MS Word uses sum numbers of decimal places from arguments of division operator, but at least 2, for result.
            int numberOfDecimalPlaces = operand1.NumberOfDigitsAfterDecimalPoint + operand2.NumberOfDigitsAfterDecimalPoint;
            numberOfDecimalPlaces = System.Math.Max(numberOfDecimalPlaces, 2);

            return DoubleConstant.CreateFrom(operand1, operand2, operand1.ValueDouble / operand2.ValueDouble, numberOfDecimalPlaces);
        }

        internal override int Order
        {
            get { return 3; }
        }
    }
}
