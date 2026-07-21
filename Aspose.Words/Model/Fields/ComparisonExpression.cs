// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/11/2020 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// The comparison expression.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public sealed class ComparisonExpression
    {
        internal ComparisonExpression(string leftExpression, string comparisonOperator, string rightExpression)
        {
            LeftExpression = leftExpression;
            ComparisonOperator = comparisonOperator;
            RightExpression = rightExpression;
        }

        /// <summary>
        /// Gets the left expression.
        /// </summary>
        public string LeftExpression { get; }

        /// <summary>
        /// Gets the comparison operator.
        /// </summary>
        public string ComparisonOperator { get; }

        /// <summary>
        /// Gets the right expression.
        /// </summary>
        public string RightExpression { get; }
    }
}
