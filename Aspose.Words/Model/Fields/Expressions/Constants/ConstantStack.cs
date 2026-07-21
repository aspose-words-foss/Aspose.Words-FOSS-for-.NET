// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/11/2006 by Dmitry Vorobyev

using System.Collections.Generic;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a stack of constants.
    /// </summary>
    internal class ConstantStack
    {
        /// <summary>
        /// Pushes a constant into the stack.
        /// </summary>
        /// <param name="constant">The constant to push.</param>
        internal void Push(Constant constant)
        {
            Debug.Assert(constant != null);
            mStack.Push(constant);
        }

        /// <summary>
        /// Pops a constant from the stack.
        /// </summary>
        /// <returns>The retrieved constant.</returns>
        internal Constant Pop()
        {
            if (mStack.Count == 0)
                return ErrorConstant.CreateSyntaxError();
            return mStack.Pop();
        }

        /// <summary>
        /// Gets the number of constants contained in this stack.
        /// </summary>
        internal int Count
        {
            get { return mStack.Count; }
        }

        private readonly Stack<Constant> mStack = new Stack<Constant>();
    }
}
