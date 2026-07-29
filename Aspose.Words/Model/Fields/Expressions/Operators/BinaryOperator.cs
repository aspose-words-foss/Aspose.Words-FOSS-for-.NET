// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents an operator which requires two operands.
    /// </summary>
    internal abstract class BinaryOperator : Operator, IExecutionItem
    {
        Constant IExecutionItem.Evaluate(ConstantStack calculationStack)
        {
            Constant operand2 = calculationStack.Pop();

            if (operand2.IsError)
                return operand2;

            // Binary operators works with "double" operands only. When one operands can not be parsed to double, we should show error.
            StringConstant stringConstant2 = operand2 as StringConstant;
            if (stringConstant2 != null)
                return ConvertStringConstantToError(stringConstant2);

            Constant operand1 = calculationStack.Pop();

            if (operand1.IsError)
                return operand1;

            StringConstant stringConstant1 = operand1 as StringConstant;
            if (stringConstant1 != null)
                return ConvertStringConstantToError(stringConstant1);

            DoubleConstant doubleConstant2 = ParseDoubleOperand(operand2);
            if (doubleConstant2 == null)
                return new DoubleConstant(0);

            DoubleConstant doubleConstant1 = ParseDoubleOperand(operand1);
            if (doubleConstant1 == null)
                return new DoubleConstant(0);

            return EvaluateCore(doubleConstant1, doubleConstant2);
        }

        private static ErrorConstant ConvertStringConstantToError(StringConstant stringConstant)
        {
            // WORDSNET-13697 Guard against empty string.
            if (StringUtil.HasChars(stringConstant.ValueString) && char.IsDigit(stringConstant.ValueString[0]))
                return ErrorConstant.CreateSyntaxError();

            return ErrorConstant.CreateBookmarkError(stringConstant.ValueString);
        }

        /// <summary>
        /// Performs the actual evaluation of the operator.
        /// </summary>
        /// <param name="operand1">The first operand to operate on.</param>
        /// <param name="operand2">The second operand to operate on.</param>
        /// <returns>The result of the evaluation.</returns>
        protected abstract Constant EvaluateCore(DoubleConstant operand1, DoubleConstant operand2);
    }
}
