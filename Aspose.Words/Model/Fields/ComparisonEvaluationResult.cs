// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/11/2020 by Edward Voronov

namespace Aspose.Words.Fields
{
    /// <summary>
    /// The comparison evaluation result.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    public sealed class ComparisonEvaluationResult
    {
        /// <summary>
        /// Creates a comparison evaluation result.
        /// </summary>
        public ComparisonEvaluationResult(bool result)
        {
            Result = result;
        }

        /// <summary>
        /// Creates a failed comparison evaluation result with the corresponding error message.
        /// </summary>
        public ComparisonEvaluationResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the comparison evaluation result.
        /// </summary>
        public bool Result { get; }

        /// <summary>
        /// Gets the failed comparison evaluation result's error message.
        /// </summary>
        public string ErrorMessage { get; }
    }
}
