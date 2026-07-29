// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/10/2011 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements the SKIPIF field.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Compares the values designated by the expressions <see cref="LeftExpression"/> and <see cref="RightExpression"/>
    /// in comparison using the operator designated by <see cref="ComparisonOperator"/>. If the comparison is true, SKIPIF
    /// cancels the current merge document, moves to the next data record in the data source, and starts a new merge document.
    /// If the comparison is false, the current merge document is continued.
    /// </remarks>
    public class FieldSkipIf : Field
    {
        internal override FieldUpdateAction UpdateCore()
        {
            return MergeFieldUtil.UpdateRuleField(this, "«Skip Record If...»");
        }

        internal Constant GetExpressionResult()
        {
            FieldCodeComparison fieldCode = new FieldCodeComparison(FieldCodeCache);
            return ComparisonEvaluator.Evaluate(this, fieldCode);
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

        private const int LeftExpressionArgumentIndex = 0;
        private const int ComparisonOperatorArgumentIndex = 1;
        private const int RightExpressionArgumentIndex = 2;
    }
}
