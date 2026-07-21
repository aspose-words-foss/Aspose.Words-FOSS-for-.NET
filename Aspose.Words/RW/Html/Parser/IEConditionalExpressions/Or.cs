// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser.IEConditionalExpressions
{
    /// <summary>
    /// Represents a disjunction of subexpressions within a conditional expression. For example, 'ie | vml'.
    /// </summary>
    internal class Or : ConditionalExpression
    {
        internal Or(ConditionalExpression left, ConditionalExpression right)
        {
            Debug.Assert(left != null);
            Debug.Assert(right != null);

            mLeftExpression = left;
            mRightExpression = right;
        }
        
        internal override bool Matches(Features features)
        {
            return mLeftExpression.Matches(features) || mRightExpression.Matches(features);
        }

        public override string ToString()
        {
            return mLeftExpression.ToStringAsSubexpression() + " | " + mRightExpression.ToStringAsSubexpression();
        }

        private readonly ConditionalExpression mLeftExpression;
        private readonly ConditionalExpression mRightExpression;
    }
}