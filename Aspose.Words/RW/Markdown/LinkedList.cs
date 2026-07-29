// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/03/2013 by Ivan Pereshein

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Aspose.IO;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Iterator over list of objects where each object has position relative other objects.
    /// List is indexed using bidirectional linked list.
    /// </summary>
    internal sealed class LinkedList
    {
        internal LinkedList()
        {
            mState = new LinkedListState();
        }

        private LinkedList(LinkedList parentList)
        {
            mState = parentList.mState;
            mCurrentNode = parentList.mCurrentNode;
            mPrevItem = parentList.mPrevItem;
        }

        /// <summary>
        /// Moves before the first object in the collection.
        /// Implementation for <see cref="IEnumerator{T}"/>
        /// </summary>
        internal void Reset()
        {
            mPrevItem = null;
            mCurrentNode = null;
        }

        /// <summary>
        /// Returns the current item.
        /// Implementation for <see cref="IEnumerator{T}"/>
        /// </summary>
        internal ILinkedListNode Current
        {
            get
            {
                if (mCurrentNode == null)
                    return null;

                if (mCurrentNode.IsNotIncluded)
                {
                    while (mCurrentNode != null && mCurrentNode.IsNotIncluded)
                        mCurrentNode = mCurrentNode.NextNode;

                    mPrevItem = mCurrentNode == null ? null : mCurrentNode.PrevNode;
                }

                return mCurrentNode;
            }
        }

        /// <summary>
        /// Gets item which is after the <see cref="Current"/>.
        /// Returns Null if there is no such item, or if item is Null itself.
        /// </summary>
        internal ILinkedListNode Next
        {
            get
            {
                return (mCurrentNode == null) ? null : mCurrentNode.NextNode;
            }
        }

        /// <summary>
        /// Gets item which is before the <see cref="Current"/>.
        /// Returns Null if there is no such item, or if item is Null itself.
        /// </summary>
        internal ILinkedListNode Previous
        {
            get
            {
                return (mCurrentNode == null) ? null : mCurrentNode.PrevNode;
            }
        }

        /// <summary>
        /// Moves to the next object in the linked list, or void if moved from the last item.
        /// Implementation for <see cref="IEnumerator{T}"/>
        /// </summary>
        internal bool MoveNext()
        {
            if (IsEmpty)
                return false;

            if (mCurrentNode == null)
            {
                if (mPrevItem == null)
                    return MoveFirst();

                return false;
            }

            mPrevItem = mCurrentNode;
            mCurrentNode = mCurrentNode.NextNode;

            // Skip removed/not-added nodes if any.
            while ((mCurrentNode != null) && mCurrentNode.IsNotIncluded)
                mCurrentNode = mCurrentNode.NextNode;

            if (mCurrentNode != null)
                mPrevItem = mCurrentNode.PrevNode;

            return mCurrentNode != null;
        }

        /// <summary>
        /// Moves to the previous item in the linked list, or void if moved from the first item.
        /// </summary>
        internal bool MovePrevious()
        {
            if (IsEmpty)
                return false;

            if (mCurrentNode != null && mCurrentNode.PrevNode != mPrevItem)
                mPrevItem = mCurrentNode.PrevNode;

            if (mPrevItem == null)
            {
                if (mCurrentNode == null)
                    return false;

                Reset();

                return false;
            }
            mCurrentNode = mPrevItem;
            mPrevItem = mPrevItem.PrevNode;

            return true;
        }

        /// <summary>
        /// Moves to the head of linked list.
        /// </summary>
        internal bool MoveFirst()
        {
            if (IsEmpty)
                return false;

            mCurrentNode = mState.Head;
            mPrevItem = null;

            return true;
        }

        /// <summary>
        /// Moves to the tail in the linked list.
        /// </summary>
        internal bool MoveLast()
        {
            if (IsEmpty)
                return false;

            mCurrentNode = mState.Tail;
            mPrevItem = mCurrentNode.PrevNode;

            return true;
        }

        /// <summary>
        /// Moves iterator to the specified item. Throws if movement fails.
        /// </summary>
        internal void MoveObject(ILinkedListNode obj)
        {
            if (!MoveObjectSafe(obj))
                throw new InvalidOperationException("Object is not in the list.");
        }

        /// <summary>
        /// Moves iterator to the specified item. Returns False if movement fails.
        /// </summary>
        internal bool MoveObjectSafe(ILinkedListNode obj)
        {
            if (obj.IsNotIncluded)
                return false;
            mCurrentNode = obj;
            mPrevItem = mCurrentNode.PrevNode;
            return true;
        }

        /// <summary>
        /// Returns True if iterator is over the first object in the collection.
        /// </summary>
        internal bool IsFirst
        {
            get
            {
                if (mCurrentNode == null)
                    throw new InvalidOperationException("There is no current object.");

                return mCurrentNode == mState.Head;
            }
        }

        /// <summary>
        /// Returns True if iterator is over the last object in the collection.
        /// </summary>
        internal bool IsLast
        {
            get
            {
                if (mCurrentNode == null)
                    throw new InvalidOperationException("There is no current object.");

                return mCurrentNode == mState.Tail;
            }
        }

        /// <summary>
        /// Returns True if iterator is not over an object. This happens if iterator
        /// was moved after the last item or before the first item or <see cref="Reset"/>.
        /// </summary>
        internal bool IsVoid
        {
            get { return mCurrentNode == null; }
        }

        /// <summary>
        /// Returns true if the linked list does not contain any items.
        /// </summary>
        internal bool IsEmpty
        {
            get { return mState.Head == null; }
        }

        /// <summary>
        /// Returns True if iterator is beyond the last item in the linked list.
        /// </summary>
        internal bool IsAfter
        {
            get { return mCurrentNode == null && mPrevItem != null; }
        }

        /// <summary>
        /// Returns True, if LinkedList contains two or more nodes.
        /// </summary>
        internal bool IsMoreThanOneNode
        {
            get { return !IsEmpty && mState.Head.NextNode != null; }
        }

        /// <summary>
        /// Returns True if left object is positioned before the right object in the list.
        /// </summary>
        internal static bool IsThisOrder(ILinkedListNode left, ILinkedListNode right)
        {
            Debug.Assert(left != null);
            Debug.Assert(right != null);

            // TODO Normally we shall make sure that left and right are from the same list, which is this one.

            if (left == right)
                return false;

            if (left.Index != right.Index)
                return left.Index < right.Index;

            if (left.SecondaryIndex != right.SecondaryIndex)
                return left.SecondaryIndex < right.SecondaryIndex;

            // This happens if both indexes match. Internal error.
            throw new InvalidOperationException("Linked list is in invalid state.");
        }

        /// <summary>
        /// Insert first item inside the empty linked list.
        /// </summary>
        private void InsertFirstItem(ILinkedListNode node)
        {
            node.Index = mState.GetNextIndex();
            mState.Head = node;
            mState.Tail = node;
            mPrevItem = node;
        }

        /// <summary>
        /// Inserts an item before the current item (into current item's range). Iterator is not moved.
        /// </summary>
        internal void Insert(ILinkedListNode node)
        {
            if (!node.IsNotIncluded)
                throw new InvalidOperationException("An object already included into list.");

            if (IsEmpty)
            {
                InsertFirstItem(node);
            }
            else
            {
                if (mCurrentNode == null)
                    throw new InvalidOperationException("There is no current object.");

                node.PrevNode = mCurrentNode.PrevNode;
                mCurrentNode.PrevNode = node;
                node.NextNode = mCurrentNode;

                if (node.PrevNode == null)
                {
                    Debug.Assert(mCurrentNode == mState.Head);
                    node.Index = node.NextNode.Index - 1;
                    mState.Head = node;
                }
                else
                {
                    node.PrevNode.NextNode = node;

                    // If there is a skipped index (if a node was removed).
                    if (node.NextNode.Index > node.PrevNode.Index + 1)
                    {
                        node.Index = node.PrevNode.Index + 1;
                    }
                    else
                    {
                        node.Index = node.PrevNode.Index;
                        CalculateSecondaryIndex(node);
                    }
                }

                mPrevItem = mCurrentNode.PrevNode;
            }
        }

        /// <summary>
        /// Append an item after the current object. Iterator is not moved.
        /// </summary>
        internal bool Append(ILinkedListNode node)
        {
            if (!node.IsNotIncluded)
                throw new InvalidOperationException("An object already included into list.");

            if (mCurrentNode == null)
            {
                Debug.Assert(IsEmpty);
                InsertFirstItem(node);
            }
            else
            {

                ILinkedListNode next = mCurrentNode.NextNode;
                mCurrentNode.NextNode = node;
                node.PrevNode = mCurrentNode;
                node.NextNode = next;

                if (next == null)
                {
                    Debug.Assert(mState.Tail == mCurrentNode);
                    mState.Tail = node;
                    node.Index = mState.GetNextIndex();
                }
                else
                {
                    next.PrevNode = node;

                    // If there is a skipped index (if a node was removed).
                    if (next.Index >  mCurrentNode.Index + 1)
                    {
                        node.Index = mCurrentNode.Index + 1;
                    }
                    else
                    {
                        node.Index = mCurrentNode.Index;
                        CalculateSecondaryIndex(node);
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Removes current object. Iterator is moved to the next object.
        /// Cannot be called "Remove" because it will clash with Iterator.remove() when auto ported to Java.
        /// </summary>
        internal void RemoveCurrentNode()
        {
            if (mCurrentNode == null)
                return;

            ILinkedListNode prev = mCurrentNode.PrevNode;
            ILinkedListNode next = mCurrentNode.NextNode;

            if (prev == null)
                mState.Head = next;
            else
                prev.NextNode = next;

            if (next == null)
                mState.Tail = prev;
            else
                next.PrevNode = prev;

            mCurrentNode.MarkAsRemoved();

            mCurrentNode = next;
            mPrevItem = prev;
        }

        /// <summary>
        /// Returns copy of this instance which shares same collection (even if it's empty).
        /// </summary>
        internal LinkedList Clone()
        {
            LinkedList clone = new LinkedList(this);
            return clone;
        }

        #region Implementation

        private static void CalculateSecondaryIndex(ILinkedListNode node)
        {
            // WORDSNET-24888 API performance issue with large documents.
            // Performance optimization: Use dispersed secondary indexes to minimize following nodes
            // secondary index updating time.
            const uint BigIncrement = 1000000000;
            const uint SmallIncrement = 100000;

            if (node.NextNode.Index != node.Index)
            {
                node.SecondaryIndex = node.PrevNode.SecondaryIndex + BigIncrement;
            }
            else
            {
                long secondaryIndex = node.PrevNode.SecondaryIndex + 1;
                if (secondaryIndex < node.NextNode.SecondaryIndex)
                {
                    node.SecondaryIndex = secondaryIndex;
                }
                else
                {
                    secondaryIndex = node.PrevNode.SecondaryIndex + SmallIncrement;
                    node.SecondaryIndex = secondaryIndex;
                    ILinkedListNode nextNode = node.NextNode;
                    while ((nextNode != null) && (nextNode.Index == node.Index) &&
                        (nextNode.SecondaryIndex <= secondaryIndex))
                    {
                        secondaryIndex += SmallIncrement;
                        nextNode.SecondaryIndex = secondaryIndex;
                        nextNode = nextNode.NextNode;
                    }
                }
            }
        }

        private readonly LinkedListState mState;
        private ILinkedListNode mCurrentNode;
        private ILinkedListNode mPrevItem;

#if DEBUG
        internal void dd(int? first = 0, int length = 100)
        {
            if (IsEmpty)
            {
                Debug.WriteLine("-->\t*");
            }
            else
            {
                LinkedList bbt = Clone();
                if (IsVoid && !IsAfter)
                    Debug.WriteLine("-->\t*");
                if (first != null)
                {
                    bbt.Reset();
                    int temp = (int)first;
                    while (--temp >= 0)
                        bbt.MoveNext();
                }
                else
                {
                    // Need bbt before the first node to dump.
                    // When first == null w want to dump from current node
                    bbt.MovePrevious();
                }

                while (bbt.MoveNext() && 0 <= --length)
                {
                    // NOTE This has side effect on 'this' as it could change mCurrentNode if it's currently over deleted object.
                    if (this.Current == bbt.Current)
                        Debug.Write("-->");
                    Debug.WriteLine(string.Format("\t{0} '{1}'", first++, bbt.Current));
                }
                if (IsAfter)
                    Debug.WriteLine("-->\t*");
            }
        }

        /// <summary>
        /// This is used to dd() this instance while indenting fields.
        /// </summary>
        internal void ddif(int first = 0, int length = 100)
        {
            using (new FieldIndentTraceListener("Default"))
                dd(first, length);
        }

        private class FieldIndentTraceListener : TraceListener, IDisposable
        {
            public FieldIndentTraceListener(string nestedName)
            {
                _Nested = Debug.Listeners[nestedName];
                Debug.Assert(_Nested != null);
                Debug.Listeners.Remove(_Nested);
                Debug.Listeners.Add(this);
            }

            public override void Write(string message)
            {
                UpdateIndent(ref message);
                _Nested.Write(message);
            }

            public override void WriteLine(string message)
            {
                UpdateIndent(ref message);
                _Nested.WriteLine(message);
            }

            protected override void Dispose(bool disposing)
            {
                Debug.Listeners.Remove(this);
                Debug.Listeners.Add(_Nested);
                _Nested = null;
            }

            private void UpdateIndent(ref string message)
            {
                _Level += _Delta;
                _Delta = 0;

                Match m = _FieldRegex.Match(message);
                if (m.Success)
                {
                    switch (m.Groups[1].Value.ToLowerInvariant())
                    {
                        case "start": _Delta = 1; break;
                        case "end": _Level--; break;
                        case "point": _Level--; _Delta = 1; break;
                        default: Debug.Fail("Unrecognized field character"); break;
                    }
                }

                message = message.Replace("\t", "    ");
                if (_Level > 0)
                    message = new string(' ', _Level) + message;
            }

            private static readonly Regex _FieldRegex = new Regex(@"None-Field(Point|Start|End)(\d+)\[([^\]]+)\]",
                RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            private TraceListener _Nested;
            private int _Level;
            private int _Delta;
        }
#endif

        /// <summary>
        /// For unit tests only.
        /// </summary>
        internal ILinkedListNode GetHead_Test
        {
            get { return mState.Head; }
        }

        /// <summary>
        /// For unit tests only.
        /// </summary>
        internal ILinkedListNode GetTail_Test
        {
            get { return mState.Tail; }
        }

        /// <summary>
        /// For unit tests only.
        /// </summary>
        internal ILinkedListNode GetNode_Test(string p)
        {
            ILinkedListNode node = mState.Head;

            while (node != null)
            {
                if (node.ToString() == p)
                    return node;
                node = node.NextNode;
            }

            return node;
        }

        #endregion

    }

}
