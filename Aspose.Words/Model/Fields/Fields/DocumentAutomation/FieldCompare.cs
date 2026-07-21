// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/08/2009 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{

    /// <summary>
    /// Implements the COMPARE field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Compares the values designated by the expressions <see cref="LeftExpression"/> and <see cref="RightExpression"/>
    /// in comparison using the operator designated by <see cref="ComparisonOperator"/>.
    /// </remarks>
    public class FieldCompare : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            FieldCodeComparison fieldCode = new FieldCodeComparison(FieldCodeCache);

            Constant result = ComparisonEvaluator.Evaluate(this, fieldCode);

            if (!result.IsError)
                return new FieldUpdateActionApplyResult(this, result);
            else
                return new FieldUpdateActionInsertErrorMessage(this, result.ValueString);
        }

        /// <summary>
        /// Gets or sets the left part of the comparison expression.
        /// </summary>
        public string LeftExpression
        {
            get { return FieldCodeCache.GetArgumentAsString(LeftExpressionArgumentIndex, false); }
            set { FieldCodeCache.SetArgument(LeftExpressionArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the comparison operator.
        /// </summary>
        public string ComparisonOperator
        {
            get { return FieldCodeCache.GetArgumentAsString(ComparisonOperatorArgumentIndex); }
            set { FieldCodeCache.SetArgument(ComparisonOperatorArgumentIndex, value); }
        }

        /// <summary>
        /// Gets or sets the right part of the comparison expression.
        /// </summary>
        public string RightExpression
        {
            get { return FieldCodeCache.GetArgumentAsString(RightExpressionArgumentIndex, false); }
            set { FieldCodeCache.SetArgument(RightExpressionArgumentIndex, value); }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int LeftExpressionArgumentIndex = 0;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int ComparisonOperatorArgumentIndex = 1;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int RightExpressionArgumentIndex = 2;
    }
}
