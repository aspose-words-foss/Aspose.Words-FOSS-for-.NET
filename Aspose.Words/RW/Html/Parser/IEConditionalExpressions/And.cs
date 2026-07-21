// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser.IEConditionalExpressions
{
    /// <summary>
    /// Represents a conjunction of subexpressions within a conditional expression. For example, 'ie &amp; vml'.
    /// </summary>
    internal class And : ConditionalExpression
    {
        internal And(ConditionalExpression left, ConditionalExpression right)
        {
            Debug.Assert(left != null);
            Debug.Assert(right != null);

            mLeftExpression = left;
            mRightExpression = right;
        }

        internal override bool Matches(Features features)
        {
            return mLeftExpression.Matches(features) && mRightExpression.Matches(features);
        }

        public override string ToString()
        {
            return mLeftExpression.ToStringAsSubexpression() + " & " + mRightExpression.ToStringAsSubexpression();
        }

        private readonly ConditionalExpression mLeftExpression;
        private readonly ConditionalExpression mRightExpression;
    }
}