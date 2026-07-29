// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser.IEConditionalExpressions
{
    /// <summary>
    /// Represents a negation of a part of a conditional expression. For example: '!vml' or '!(ie &amp; vml)'.
    /// </summary>
    internal class Not : ConditionalExpression
    {
        internal Not(ConditionalExpression expression)
        {
            Debug.Assert(expression != null);
            mExpression = expression;
        }
        
        internal override bool Matches(Features features)
        {
            return !mExpression.Matches(features);
        }

        public override string ToString()
        {
            return "!" + mExpression.ToStringAsSubexpression();
        }

        private readonly ConditionalExpression mExpression;
    }
}