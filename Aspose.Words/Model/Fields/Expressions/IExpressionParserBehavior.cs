// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2015 by Edward Voronov

using System.Collections.Generic;
using System.Text;
using Aspose.JavaAttributes;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Performs specific behavior of expression parser.
    /// </summary>
    internal interface IExpressionParserBehavior
    {
        /// <summary>
        /// Performs specific behavior with parsed expression.
        /// </summary>
        [JavaThrows(true)]
        List<object> NormalizeExpression(List<object> expression, FieldContext context);

        /// <summary>
        /// Performs specific behavior with builded execution queue.
        /// </summary>
        ExecutionQueue NormalizeExecutionQueue(ExecutionQueue queue, FieldContext context, string expression);

        /// <summary>
        /// Checks if the specified character is a delimiter.
        /// </summary>
        /// <param name="c">The character to check.</param>
        /// <param name="expression">The parsed expression.</param>
        /// <param name="position">The position of checked character.</param>
        /// <param name="builder">The current token.</param>
        /// <returns>True if the character is a delimiter.</returns>
        bool IsDelimiter(char c, string expression, int position, StringBuilder builder);
    }
}