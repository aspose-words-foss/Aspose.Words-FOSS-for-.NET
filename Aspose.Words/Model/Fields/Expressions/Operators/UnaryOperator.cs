// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents an operator which requires one operand.
    /// </summary>
    internal abstract class UnaryOperator : Operator, IExecutionItem
    {
        Constant IExecutionItem.Evaluate(ConstantStack calculationStack)
        {
            Constant operand = calculationStack.Pop();
            if (operand.IsError)
                return operand;

            DoubleConstant doubleConstant = ParseDoubleOperand(operand);
            if (doubleConstant == null)
                return ErrorConstant.CreateSyntaxError();

            return EvaluateCore(doubleConstant);
        }

        /// <summary>
        /// Performs the actual evaluation of the operator.
        /// </summary>
        /// <param name="operand">The operand to operate on.</param>
        /// <returns></returns>
        protected abstract Constant EvaluateCore(DoubleConstant operand);

        internal override bool IsUnary
        {
            get { return true; }
        }
    }
}
