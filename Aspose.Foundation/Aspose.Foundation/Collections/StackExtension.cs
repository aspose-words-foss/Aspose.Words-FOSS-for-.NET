// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/03/2024 by Edward Voronov

using System;
using System.Collections.Generic;

namespace Aspose
{
    /// <summary>
    /// Extensions methods for the <see cref="Stack{T}"/> class.
    /// </summary>
    public static class StackExtension
    {
        /// <summary>
        /// Returns the object at the top of the <paramref name="stack"/> if it exists. Otherwise <c>null</c>.
        /// </summary>
        public static T Top<T>(this Stack<T> stack)
            where T : class
        {
            return stack.Count != 0
                ? stack.Peek()
                : null;
        }

        /// <summary>
        /// Removes and returns the object at the top of the <paramref name="stack"/>
        /// if it exists and can be assigned to <typeparamref name="TSub"/> type. Otherwise <c>null</c>.
        /// </summary>
        public static TBase PopIfInstanceOf<TBase>(this Stack<TBase> stack, Type tSub)
            where TBase : class
        {
            TBase top = stack.Top();
            return (top != null && top.GetType() == tSub)
                ? stack.Pop()
                : null;
        }
    }
}
