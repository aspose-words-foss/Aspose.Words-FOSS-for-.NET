// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/11/2020 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// When implemented, allows to override default comparison expressions evaluation for the <see cref="FieldIf"/> and <see cref="FieldCompare"/> fields.
    /// </summary>
    /// <seealso cref="FieldOptions.ComparisonExpressionEvaluator"/>
    public interface IComparisonExpressionEvaluator
    {
        /// <summary>
        /// Evaluates comparison expression.
        /// </summary>
        /// <remarks>
        /// The implementation should return <c>null</c> to indicate that the default evaluation should be performed.
        /// </remarks>
        ComparisonEvaluationResult Evaluate(Field field, ComparisonExpression expression);
    }
}
