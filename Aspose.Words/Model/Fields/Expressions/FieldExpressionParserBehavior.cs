// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2015 by Edward Voronov

using System.Collections.Generic;
using System.Text;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Performs behavior of expression parser related to field.
    /// </summary>
    internal class FieldExpressionParserBehavior : IExpressionParserBehavior
    {
        internal FieldExpressionParserBehavior()
            : this(false)
        {
        }

        internal FieldExpressionParserBehavior(bool buildBookmarkReference)
            : this(buildBookmarkReference, true, true)
        {
        }

        internal FieldExpressionParserBehavior(bool buildBookmarkReference, bool resolveBookmarkReference, bool resolveCellReference)
        {
            mBuildBookmarkReference = buildBookmarkReference;
            mResolveBookmarkReference = resolveBookmarkReference;
            mResolveCellReference = resolveCellReference;
        }

        List<object> IExpressionParserBehavior.NormalizeExpression(List<object> expression, FieldContext context)
        {
            if (expression.Count == 0)
                return expression;

            if (!mResolveBookmarkReference && !mResolveCellReference)
                return expression;

            List<object> result = new List<object>(expression.Count);

            foreach (object item in expression)
            {
                StringConstant stringConstant = item as StringConstant;
                if (stringConstant != null)
                {
                    IExecutionItem reference = ParseReference(stringConstant.ValueString, context);
                    if (reference != null)
                    {
                        result.Add(reference);
                        continue;
                    }
                }

                result.Add(item);
            }

            return result;
        }

        ExecutionQueue IExpressionParserBehavior.NormalizeExecutionQueue(ExecutionQueue queue, FieldContext context, string expression)
        {
            if (queue.Count == 1)
            {
                IExecutionItem item = queue.Dequeue();

                StringConstant stringConstant = item as StringConstant;
                if (stringConstant != null)
                {
                    item = mBuildBookmarkReference
                        ? (IExecutionItem)new BookmarkReference(context, stringConstant.ValueString)
                        : new StringConstant(expression);
                }

                queue.Enqueue(item);
            }

            return queue;
        }

        bool IExpressionParserBehavior.IsDelimiter(char c, string expression, int position, StringBuilder builder)
        {
            return false;
        }

        /// <summary>
        /// Tries to parse the specified operand string to a reference.
        /// </summary>
        /// <param name="operand">The operand to parse.</param>
        /// <param name="context"></param>
        /// <returns>The parsed reference or null if the operand is not a reference.</returns>
        private IExecutionItem ParseReference(string operand, FieldContext context)
        {
            IExecutionItem cellReference = mResolveCellReference
                ? CellReference.TryParseAsRectangularRange(context, operand, false)
                : null;

            // WORDSNET-5569 Need to check the input parameter, it might be an empty string.
            // Bookmark name may not start with a digit, and should it happen, return the cell reference.
            if (StringUtil.HasChars(operand) && char.IsDigit(operand[0]))
            {
                return cellReference;
            }

            // That is the way Word works, 12812 shows that well. If a formula references "C2" from within a table cell
            // and there are both cell C2 and bookmark "C2" in the table, then bookmark reference has a higher priority.
            if (mResolveBookmarkReference && context.DocumentContainsBookmark(operand))
            {
                return new BookmarkReference(context, operand);
            }

            return cellReference;
        }

        private readonly bool mBuildBookmarkReference;
        private readonly bool mResolveBookmarkReference;
        private readonly bool mResolveCellReference;
    }
}
