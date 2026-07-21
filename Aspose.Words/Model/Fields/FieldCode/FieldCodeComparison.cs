// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/08/2009 by Dmitry Vorobyev

using Aspose.Words.Fields.Expressions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Parses field code of the COMPARE and NEXTIF fields for evaluating comparison expression.
    /// Should only be used during field update.
    /// </summary>
    internal class FieldCodeComparison : IComparisonExpression
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="fieldCode"></param>
        internal FieldCodeComparison(FieldCode fieldCode)
        {
            mFieldCode = fieldCode;
        }

        string IComparisonExpression.LeftExpression
        {
            get { return mFieldCode.GetArgumentAsString(LeftExpressionArgumentIndex, false); }
        }

        FieldArgument IComparisonExpression.LeftExpressionArgument
        {
            get { return mFieldCode.GetArgument(LeftExpressionArgumentIndex); }
        }

        string IComparisonExpression.ComparisonOperator
        {
            get { return mFieldCode.GetArgumentAsString(ComparisonOperatorArgumentIndex, false); }
        }

        string IComparisonExpression.RightExpression
        {
            get { return mFieldCode.GetArgumentAsString(RightExpressionArgumentIndex, false); }
        }

        FieldArgument IComparisonExpression.RightExpressionArgument
        {
            get { return mFieldCode.GetArgument(RightExpressionArgumentIndex); }
        }

        private readonly FieldCode mFieldCode;

        private const int LeftExpressionArgumentIndex = 0;
        private const int ComparisonOperatorArgumentIndex = 1;
        private const int RightExpressionArgumentIndex = 2;
    }
}
