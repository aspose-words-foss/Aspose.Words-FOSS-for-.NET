// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Evaluates an expression.
    /// </summary>
    internal static class ExpressionEvaluator
    {
        /// <summary>
        /// Evaluates an expression.
        /// </summary>
        internal static Constant EvaluateExpression(FieldContext fieldContext, string expression)
        {
            return Evaluate(fieldContext, expression, new FieldExpressionParserBehavior(false));
        }

        /// <summary>
        /// Evaluates an formula field expression.
        /// </summary>
        internal static Constant EvaluateFormulaExpression(Field field, string expression)
        {
            FieldContext fieldContext = new FieldContext(field);
            return Evaluate(fieldContext, expression, new FieldExpressionParserBehavior(true));
        }

        private static Constant Evaluate(FieldContext fieldContext, string expression, IExpressionParserBehavior behavior)
        {
            ExecutionQueue queue = ExpressionParser.Parse(fieldContext, expression, behavior);

            if (queue == null)
            {
                // An error has occurred during parsing.
                return ErrorConstant.CreateSyntaxError();
            }

            return ExpressionCalculator.Calculate(queue);
        }

        /// <summary>
        /// Evaluates text from cell or bookmark reference.
        /// </summary>
        internal static DoubleConstant EvaluateReferenceExpression(FieldContext fieldContext, string expression)
        {
            return EvaluateReferenceExpression(fieldContext, expression, true);
        }

        /// <summary>
        /// Evaluates text from cell or bookmark reference.
        /// </summary>
        internal static DoubleConstant EvaluateReferenceExpression(
            FieldContext fieldContext,
            string expression,
            bool allowLeadingStringConstants)
        {
            ReferenceExpressionResult result = EvaluateReferenceExpressionWithInfo(
                fieldContext,
                expression,
                allowLeadingStringConstants);

            return result != null
                ? (DoubleConstant)result.Result
                : null;
        }

        /// <summary>
        /// Evaluates text from cell or bookmark reference.
        /// </summary>
        internal static ReferenceExpressionResult EvaluateReferenceExpressionWithInfo(
            FieldContext fieldContext,
            string expression,
            bool allowLeadingStringConstants)
        {
            expression = expression
                .Replace('=', ' ')
                .Replace(ControlChar.CellChar, ' ')
                .Replace('\t', ' ')
                .Replace('\r', ' ');

            ReferenceExpressionParserBehavior behavior = new ReferenceExpressionParserBehavior(
                allowLeadingStringConstants);

            ExecutionQueue queue = ExpressionParser.Parse(
                fieldContext,
                expression,
                behavior);

            if (queue == null)
            {
                // An error has occurred during parsing.
                return null;
            }

            DoubleConstant result = ExpressionCalculator.Calculate(queue) as DoubleConstant;
            if (result == null)
                return null;

            return new ReferenceExpressionResult(result, behavior.ExpressionHasLeadingMinusOperator);
        }
    }
}
