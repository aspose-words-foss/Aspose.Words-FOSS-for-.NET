// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a left parenthesis operator.
    /// </summary>
    /// <remarks>
    /// This is a special operator that is missing from the listing of expression operators. Actually, this is not
    /// an operator because it does not have operands and cannot be evaluated. But it is convenient to treat
    /// it as an operator during parsing because it allows to control the order and due to other reasons.
    /// </remarks>
    internal class LeftParenthesisOperator : Operator
    {
        internal override bool IsLeftParenthesis
        {
            get { return true; }
        }

        internal override int Order
        {
            get { return 9; }
        }
    }
}
