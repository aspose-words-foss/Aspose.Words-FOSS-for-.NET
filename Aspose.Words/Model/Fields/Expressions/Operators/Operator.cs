// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2006 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a formula operator.
    /// </summary>
    internal abstract class Operator
    {
        /// <summary>
        /// Gets the order of evaluation of this operator.
        /// </summary>
        internal abstract int Order { get; }

        /// <summary>
        /// Returns true if this operator is a left parenthesis operator.
        /// </summary>
        internal virtual bool IsLeftParenthesis
        {
            get { return false; }
        }

        /// <summary>
        /// Returns true if this operator is a right parenthesis operator.
        /// </summary>
        internal virtual bool IsRightParenthesis
        {
            get { return false; }
        }

        /// <summary>
        /// Returns true if this operator is unary.
        /// </summary>
        internal virtual bool IsUnary
        {
            get { return false; }
        }

        /// <summary>
        /// Returns true if this operator is unary minus.
        /// </summary>
        internal virtual bool IsUnaryMinus
        {
            get { return false; }
        }

        protected static DoubleConstant ParseDoubleOperand(Constant operand)
        {
            DoubleConstant doubleConstant = operand as DoubleConstant;
            if (doubleConstant != null)
                return doubleConstant;

            BooleanConstant booleanConstant = operand as BooleanConstant;
            if (booleanConstant != null)
                return new DoubleConstant(booleanConstant.ValueDouble);

            return null;
        }
    }
}
