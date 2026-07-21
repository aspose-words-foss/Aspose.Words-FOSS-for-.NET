// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/12/2015 by Victor Chebotok

using Aspose.Words.RW.Html.Parser.IEConditionalExpressions;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Represents an Internet Explorer conditional expression.
    /// </summary>
    /// <remarks>
    /// For information on conditional expressions see https://msdn.microsoft.com/en-us/library/ms537512(v=vs.85).aspx
    /// </remarks>
    internal abstract class ConditionalExpression
    {
        /// <summary>
        /// Parses a text into a conditional expression.
        /// </summary>
        /// <param name="text">A text to parse.</param>
        /// <returns>Either the parsed expression or <c>null</c> if an error occurred.</returns>
        internal static ConditionalExpression Parse(string text)
        {
            IEConditionalExpressionsParser parser = new IEConditionalExpressionsParser();
            return parser.Parse(text);
        }

        /// <summary>
        /// Matches the expression against a set of features. Determines whether the set satisfies the expression.
        /// </summary>
        /// <param name="features">A set of features to match expression against.</param>
        /// <returns>A value indicating whether the expression matches the set.</returns>
        internal abstract bool Matches(Features features);

        internal string ToStringAsSubexpression()
        {
            return (IsSubexpression)
                ? "(" + ToString() + ")"
                : ToString();
        }

        protected virtual bool IsSubexpression
        {
            get { return true; }
        }
    }
}
