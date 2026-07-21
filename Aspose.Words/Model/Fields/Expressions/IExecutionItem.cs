// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/11/2006 by Dmitry Vorobyev

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a formula execution item such as a constant, operator, function, or reference.
    /// </summary>
    internal interface IExecutionItem
    {
        /// <summary>
        /// Evaluates the execution item.
        /// </summary>
        /// <param name="calculationStack">The current calculation stack.</param>
        /// <returns>The result of the evaluation.</returns>
        [JavaThrows(true)]
        Constant Evaluate(ConstantStack calculationStack);
    }
}
