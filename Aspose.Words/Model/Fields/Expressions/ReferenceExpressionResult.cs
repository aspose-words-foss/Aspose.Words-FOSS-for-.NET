// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/10/2024 by Edward Voronov

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents result of the reference expression evaluation.
    /// </summary>
    internal class ReferenceExpressionResult
    {
        internal ReferenceExpressionResult(Constant result, bool hasLeadingMinusOperator)
        {
            Debug.Assert(result != null);

            Result = result;
            HasLeadingMinusOperator = hasLeadingMinusOperator;
        }

        internal Constant Result { get; }

        internal bool HasLeadingMinusOperator { get; }
    }
}
