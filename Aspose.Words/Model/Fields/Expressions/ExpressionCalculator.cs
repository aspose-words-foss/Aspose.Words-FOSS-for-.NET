// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Calculates a formula value given a queue of execution items.
    /// </summary>
    internal static class ExpressionCalculator
    {
        /// <summary>
        /// Calculates the result of the execution queue prepared by <see cref="ExpressionParser"/>.
        /// </summary>
        /// <param name="queue">The queue to calculate.</param>
        /// <returns>The result of the calculation.</returns>
        internal static Constant Calculate(ExecutionQueue queue)
        {
            ConstantStack calculationStack = new ConstantStack();

            if (queue.Count == 0)
                return new ErrorConstant("!Unexpected End of Formula");

            while (queue.Count > 0)
            {
                IExecutionItem item = queue.Dequeue();

                Constant result = item.Evaluate(calculationStack);

                // If the item evaluates to an error, stop the evaluation immediately.
                if (result.IsError)
                    return result;

                calculationStack.Push(result);
            }

            Constant calculationResult = calculationStack.Pop();
            if (calculationStack.Count > 0)
                return new ErrorConstant("!Missing Operator");

            return calculationResult;
        }
    }
}
