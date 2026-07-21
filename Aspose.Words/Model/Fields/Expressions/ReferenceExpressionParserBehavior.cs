// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/03/2015 by Edward Voronov

using System.Collections.Generic;
using System.Text;
using Aspose.Common;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Performs behavior of expression parser related to bookmark and cell references.
    /// </summary>
    internal class ReferenceExpressionParserBehavior : IExpressionParserBehavior
    {
        internal ReferenceExpressionParserBehavior(bool allowLeadingStringConstants)
        {
            mAllowLeadingStringConstants = allowLeadingStringConstants;
        }

        internal bool ExpressionHasLeadingMinusOperator { get; private set; }

        List<object> IExpressionParserBehavior.NormalizeExpression(List<object> expression, FieldContext context)
        {
            if (!mAllowLeadingStringConstants)
            {
                if ((expression.Count > 0) && (expression[0] is StringConstant))
                    return null;
            }

            expression = ReplaceParentheses(expression);
            expression = RemoveOperators(expression);
            expression = RemoveStringConstants(expression);
            expression = AddSumOperator(expression);

            ExpressionHasLeadingMinusOperator = (expression.Count > 0) && (expression[0] is UnaryMinusOperator);

            return expression;
        }

        ExecutionQueue IExpressionParserBehavior.NormalizeExecutionQueue(ExecutionQueue queue, FieldContext context, string expression)
        {
            return queue;
        }

        bool IExpressionParserBehavior.IsDelimiter(char c, string expression, int position, StringBuilder builder)
        {
            char decimalSeparator = FormatterPal.GetDecimalSeparatorCurrent();
            char groupSeparator = FormatterPal.GetNumberGroupSeparatorCurrent();

            if ((c != groupSeparator) && (c != decimalSeparator))
                return false;

            return IsDecimalSeparatorDelimiter(builder, decimalSeparator) || IsGroupSeparatorDelimiter(c, expression, position, groupSeparator);
        }

        private static bool IsDecimalSeparatorDelimiter(StringBuilder builder, char decimalSeparator)
        {
            return builder.ToString().Contains(decimalSeparator.ToString());
        }

        private static bool IsGroupSeparatorDelimiter(char c, string expression, int position, char groupSeparator)
        {
            if (c != groupSeparator)
                return false;

            int groupSize = FormatterPal.GetNumberGroupSizeCurrent();
            if (groupSize == 0)
                return false;

            if (position + groupSize >= expression.Length)
                return true;

            for (int i = 0; i < groupSize; i++)
            {
                char nextChar = expression[position + i + 1];
                if (!char.IsDigit(nextChar))
                    return true;
            }

            return false;
        }

        private static List<object> ReplaceParentheses(List<object> items)
        {
            if (items.Count < 3)
                return items;

            List<object> result = new List<object>(items.Count);

            int i = 0;
            while (i < items.Count)
            {
                object item = items[i];

                if (IsDoubleConstantBetweenParentheses(items, i))
                {
                    result[result.Count - 1] = new UnaryMinusOperator(true);
                    i++;
                }

                result.Add(item);

                i++;
            }

            return result;
        }

        private static bool IsDoubleConstantBetweenParentheses(List<object> items, int index)
        {
            if (index == 0)
                return false;

            if (index == items.Count - 1)
                return false;

            if (!IsDoubleConstant(items[index]))
                return false;

            if (!IsLeftParenthesisOperator(items[index - 1]))
                return false;

            if (!IsRightParenthesisOperator(items[index + 1]))
                return false;

            return true;
        }

        private static List<object> RemoveOperators(List<object> items)
        {
            if (items.Count == 0)
                return items;

            List<object> result = new List<object>(items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                object item = items[i];
                Operator @operator = item as Operator;

                if ((@operator != null) && !@operator.IsUnary && IsBetweenStringConstant(items, i))
                    continue;

                result.Add(item);
            }

            return result;
        }

        private static bool IsBetweenStringConstant(List<object> items, int i)
        {
            return PreviousItemIsStringConstant(items, i) && NextItemIsStringConstant(items, i);
        }

        private static bool PreviousItemIsStringConstant(List<object> items, int i)
        {
            return (i != 0) && (items[i - 1] is StringConstant);
        }

        private static bool NextItemIsStringConstant(List<object> items, int i)
        {
            return (i != items.Count - 1) && (items[i + 1] is StringConstant);
        }

        private static List<object> RemoveStringConstants(List<object> items)
        {
            if (items.Count == 0)
                return items;

            List<object> result = new List<object>(items.Count);

            foreach (object item in items)
            {
                if (!(item is StringConstant))
                    result.Add(item);
            }

            return result;
        }

        private static List<object> AddSumOperator(List<object> items)
        {
            if (items.Count <= 1)
                return items;

            foreach (object item in items)
            {
                if (IsDoubleConstant(item) || IsUnaryOperator(item) || IsSubtractionOperator(item))
                    continue;

                return items;
            }

            List<object> result = new List<object>(items.Count * 2 - 1);

            result.Add(items[0]);

            for (int i = 1; i < items.Count; i++)
            {
                object lastItem = result[result.Count - 1];
                object currentItem = items[i];

                if ((IsDoubleConstant(currentItem) || IsUnaryMinusOperator(currentItem)) &&
                    (IsDoubleConstant(lastItem) || IsPercentOperator(lastItem)))
                    result.Add(new AdditionOperator());

                result.Add(currentItem);
            }

            return result;
        }

        private static bool IsDoubleConstant(object item)
        {
            return item is DoubleConstant;
        }

        private static bool IsPercentOperator(object item)
        {
            return item is UnaryPercentOperator;
        }

        private static bool IsUnaryOperator(object item)
        {
            return item is UnaryOperator;
        }

        private static bool IsUnaryMinusOperator(object item)
        {
            return item is UnaryMinusOperator;
        }

        private static bool IsLeftParenthesisOperator(object item)
        {
            return item is LeftParenthesisOperator;
        }

        private static bool IsRightParenthesisOperator(object item)
        {
            return item is RightParenthesisOperator;
        }

        private static bool IsSubtractionOperator(object item)
        {
            return item is SubtractionOperator;
        }

        private readonly bool mAllowLeadingStringConstants;
    }
}
