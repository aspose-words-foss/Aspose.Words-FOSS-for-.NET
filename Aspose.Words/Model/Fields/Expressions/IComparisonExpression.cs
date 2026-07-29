// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/08/2009 by Dmitry Vorobyev

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Implemented by the IF and COMPARE field code objects to represent an input for ComparisonEvaluator.
    /// </summary>
    internal interface IComparisonExpression
    {
        /// <summary>
        /// Gets the left hand expression.
        /// </summary>
        [JavaThrows(true)]
        string LeftExpression { get; }

        /// <summary>
        /// Gets the left hand expression field argument.
        /// </summary>
        FieldArgument LeftExpressionArgument { get; }

        /// <summary>
        /// Gets the comparison operator.
        /// </summary>
        [JavaThrows(true)]
        string ComparisonOperator { get; }

        /// <summary>
        /// Gets the right hand expression.
        /// </summary>
        [JavaThrows(true)]
        string RightExpression { get; }

        /// <summary>
        /// Gets the right hand expression field argument.
        /// </summary>
        FieldArgument RightExpressionArgument { get; }
    }
}
