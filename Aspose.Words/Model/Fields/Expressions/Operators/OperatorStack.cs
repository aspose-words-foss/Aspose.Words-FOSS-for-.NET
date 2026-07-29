// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2006 by Dmitry Vorobyev

using System.Collections.Generic;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a stack of operators.
    /// </summary>
    internal class OperatorStack
    {
        /// <summary>
        /// Pushes a operator into the stack.
        /// </summary>
        /// <param name="op">The operator to push.</param>
        internal void Push(Operator op)
        {
            Debug.Assert(op != null);
            mStack.Push(op);
        }

        /// <summary>
        /// Pops a operator from the stack.
        /// </summary>
        /// <returns>The retrieved operator.</returns>
        internal Operator Pop()
        {
            return mStack.Pop();
        }

        /// <summary>
        /// Returns the operator from the top of the stack without removing it.
        /// </summary>
        /// <returns>The retrieved operator.</returns>
        internal Operator Peek()
        {
            return mStack.Peek();
        }

        /// <summary>
        /// Gets the number of operators contained in this stack.
        /// </summary>
        internal int Count
        {
            get { return mStack.Count; }
        }

        private readonly Stack<Operator> mStack = new Stack<Operator>();
    }
}
