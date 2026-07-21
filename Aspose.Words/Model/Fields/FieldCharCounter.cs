// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/04/2013 by Ivan Lyagin

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Sequentially counts encountered field chars indicating whether the current position is inside a field.
    /// </summary>
    internal class FieldCharCounter
    {
        /// <summary>
        /// Ctor to create a counter that does not consider a parent field if any.
        /// </summary>
        internal FieldCharCounter()
            : this(FieldCharCounterOptions.Default)
        {
        }

        /// <summary>
        /// Ctor to create a counter that can be started within a parent field's result.
        /// </summary>
        /// <remarks>
        /// Only direct parent field can be considered but other ancestor fields can not.
        /// </remarks>
        internal FieldCharCounter(bool isInFieldResult)
            : this (isInFieldResult ? FieldCharCounterOptions.InFieldResult : FieldCharCounterOptions.Default)
        {
        }

        internal FieldCharCounter(FieldCharCounterOptions options)
        {
            if ((options & FieldCharCounterOptions.InFieldResult) == FieldCharCounterOptions.InFieldResult)
                mFieldStarts.Push(null);

            mSafeMode = (options & FieldCharCounterOptions.SafeMode) == FieldCharCounterOptions.SafeMode;
        }

        /// <summary>
        /// If the specified node is of a field char type then counts it. Otherwise, does nothing.
        /// </summary>
        internal void VisitNode(Node node)
        {
            if (!IsValidFieldChar(node))
                return;

            switch (node.NodeType)
            {
                case NodeType.FieldStart:
                {
                    mEnteredFieldCodeCount++;
                    mFieldStarts.Push((FieldStart)node);
                    break;
                }
                case NodeType.FieldSeparator:
                {
                    ReduceEnteredFieldCodeCount();
                    break;
                }
                case NodeType.FieldEnd:
                {
                    FieldEnd fieldEnd = (FieldEnd)node;
                    if (!fieldEnd.HasSeparator)
                        ReduceEnteredFieldCodeCount();

                    ReduceEnteredFieldCount();
                    break;
                }
                default:
                    break;
            }
        }

        internal static bool IsValidFieldChar(Node node)
        {
            FieldChar fieldChar = node as FieldChar;

            if (fieldChar == null)
                return false;

            return !fieldChar.IsDeleteRevision;
        }

        private void ReduceEnteredFieldCodeCount()
        {
            mEnteredFieldCodeCount--;

            if (mEnteredFieldCodeCount < 0)
            {
                if (mSafeMode)
                {
                    mEnteredFieldCodeCount = 0;
                }
                else
                {
                    Debug.Fail("mEnteredFieldCodeCount >= 0");
                }
            }
        }

        private void ReduceEnteredFieldCount()
        {
            if (FieldsDepth > 0 || !mSafeMode)
                mFieldStarts.Pop();
        }

        /// <summary>
        /// Gets a value indicating if the current position is inside a field code.
        /// </summary>
        internal bool IsInFieldCode
        {
            get { return mEnteredFieldCodeCount != 0; }
        }

        /// <summary>
        /// Gets a value indicating if the current position is inside a field result.
        /// </summary>
        internal bool IsInFieldResult
        {
            get { return IsInField && !IsInFieldCode; }
        }

        /// <summary>
        /// Gets a value indicating if the current position is inside a field.
        /// </summary>
        internal bool IsInField
        {
            get { return FieldsDepth != 0; }
        }

        internal FieldStart CurrentField
        {
            get { return mFieldStarts.Top(); }
        }

        internal int FieldsDepth
        {
            get { return mFieldStarts.Count; }
        }

        private int mEnteredFieldCodeCount;

        private readonly Stack<FieldStart> mFieldStarts = new Stack<FieldStart>();
        private readonly bool mSafeMode;
    }
}
