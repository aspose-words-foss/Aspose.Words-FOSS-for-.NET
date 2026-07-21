// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/07/2009 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the = (formula) field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Calcualtes the result of an expression.
    /// </remarks>
    public class FieldFormula : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            Constant result = Evaluate();

            if (!result.IsError)
                return new FieldUpdateActionApplyResult(this, result);
            else
                return new FieldUpdateActionInsertErrorMessage(this, result.ValueString);
        }

        internal Constant Evaluate()
        {
            // Formulas are a special kind of fields where we never care about extracting field code tokens
            // and should treat the whole code as a single string.
            string fieldCode = NodeRangeFieldCodeTokenizer.GetCodeAsString(this);
            // Remove '=' at the begining.
            fieldCode = fieldCode.TrimStart().Substring(1);

            return Evaluate(fieldCode);
        }

        private Constant Evaluate(string expression)
        {
            Constant result = ExpressionEvaluator.EvaluateFormulaExpression(this, expression);

            if (result is StringConstant)
            {
                if (string.IsNullOrEmpty(result.ValueString))
                    return new DoubleConstant(0d);
                else
                    return ErrorConstant.CreateSyntaxError();
            }
            else if (result is NullConstant)
            {
                return new StringConstant(string.Empty);
            }

            return result;
        }
    }
}
