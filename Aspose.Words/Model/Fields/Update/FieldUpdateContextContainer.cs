// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/11/2013 by Ivan Lyagin

using System;
using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// A base class representing a node of a field update tree.
    /// </summary>
    /// <remarks>
    /// A field update tree is composed of a <see cref="FieldUpdater"/> instance as its root node and
    /// <see cref="FieldUpdateContext"/> instances as its branch and leaf nodes.
    ///
    /// A <see cref="FieldUpdater"/> corresponds to a single field update session. A <see cref="FieldUpdateContext"/>
    /// instance corresponds to a single updated field contained in a document.
    ///
    /// That is a <see cref="FieldUpdater"/> instance contains <see cref="FieldUpdateContext"/> instances corresponding to
    /// the topmost fields in the document. <see cref="FieldUpdateContext"/> instances in turn are nested to each other
    /// exactly the same way as the corresponding fields are nested to each other's codes in the document.
    /// </remarks>
    internal abstract class FieldUpdateContextContainer : IEnumerable<FieldUpdateContext>
    {
        /// <summary>
        /// Inserts the specified child context to the given parent container before or after its specified child.
        /// This does not break any parent container enumeration if any.
        /// </summary>
        protected static void InsertChild(
            FieldUpdateContextContainer parent,
            FieldUpdateContext refChild,
            FieldUpdateContext newChild,
            bool isAfter)
        {
            // Ensure parent container.
            Debug.Assert((refChild == null) || (refChild.Parent == parent));

            // Ensure that the child context is not/was not inserted to another container.
            Debug.Assert(newChild.Parent == null);
            Debug.Assert(newChild.NextSibling == null);
            Debug.Assert(newChild.PreviousSibling == null);

            // Use the first or the last child if the reference child is missing.
            if (refChild == null)
                refChild = parent.GetEdgeChild(isAfter);

            if (refChild == null)
            {
                // This is the first child.
                parent.FirstChild = newChild;
                parent.mLastChild = newChild;
            }
            else
            {
                // The parent container already have children.
                refChild.RelinkSibling(isAfter, newChild);

                newChild.SetSibling(isAfter, refChild.GetSibling(isAfter));
                newChild.SetSibling(!isAfter, refChild);

                refChild.SetSibling(isAfter, newChild);
            }

            newChild.Parent = parent;
        }

        /// <summary>
        /// Removes the given child context from its parent container. This does not break any parent container
        /// enumeration if any.
        /// </summary>
        protected static void RemoveChild(FieldUpdateContext child)
        {
            // Ensure the child context is belongs to a container.
            if (child.Parent == null)
                return;

            child.NotifyUpdateProgressCurrentFieldIsUpdated();

            // Change sibling references of the remained child contexts, but do not change sibling references of
            // the context being removed not to break any existing child context enumeration.
            child.RelinkSibling(false, child.NextSibling);
            child.RelinkSibling(true, child.PreviousSibling);

            // Reset the parent container of the removed context, so it could not be removed multiple times
            // (see the guard at the top).
            child.Parent = null;
        }

        /// <summary>
        /// Makes the next or the previous sibling of this context reference another context instead of this one.
        /// </summary>
        internal void RelinkSibling(bool isNext, FieldUpdateContext newSibling)
        {
            if (this == Parent.GetEdgeChild(isNext))
            {
                Parent.SetEdgeChild(isNext, newSibling);
            }
            else
            {
                GetSibling(isNext).SetSibling(!isNext, newSibling);
            }
        }

        /// <summary>
        /// Returns the next or the previous sibling of this context.
        /// </summary>
        internal FieldUpdateContext GetSibling(bool isNext)
        {
            return isNext ? NextSibling : PreviousSibling;
        }

        /// <summary>
        /// Sets the next or the previous sibling of this context.
        /// </summary>
        internal void SetSibling(bool isNext, FieldUpdateContext newSibling)
        {
            if (isNext)
            {
                NextSibling = newSibling;
            }
            else
            {
                PreviousSibling = newSibling;
            }
        }

        /// <summary>
        /// Returns the first or the last child context of this container.
        /// </summary>
        private FieldUpdateContext GetEdgeChild(bool isLast)
        {
            return isLast ? mLastChild : FirstChild;
        }

        /// <summary>
        /// Sets the first or the last child context of this container.
        /// </summary>
        private void SetEdgeChild(bool isLast, FieldUpdateContext newChild)
        {
            if (isLast)
            {
                mLastChild = newChild;
            }
            else
            {
                FirstChild = newChild;
            }
        }

        /// <summary>
        /// Returns <see cref="IEnumerator"/> instance to iterate over child contexts within this container.
        /// Any enumeration is flexible for changes, i.e. you can remove the current context being enumerated
        /// or insert a new one right after it without any harm to the enumeration.
        /// </summary>
        public IEnumerator<FieldUpdateContext> GetEnumerator()
        {
            return new ChildContextEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void NotifyUpdateProgressChildrenFieldsAreUpdated()
        {
            foreach (FieldUpdateContext child in this)
            {
                child.NotifyUpdateProgressCurrentFieldIsUpdated();
                child.NotifyUpdateProgressChildrenFieldsAreUpdated();
            }
        }

        /// <summary>
        /// Gets the parent container for this container.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        internal FieldUpdateContextContainer Parent { get; set; }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        internal FieldUpdateContext PreviousSibling { get; set; }

        internal FieldUpdateContext NextSibling { get; set; }

        /// <summary>
        /// Gets a value indicating whether this container has any children.
        /// </summary>
        internal bool HasChildren
        {
            get { return FirstChild != null; }
        }

        internal FieldUpdateContext FirstChild { get; private set; }

        /// <summary>
        /// Implements <see cref="IEnumerator"/> to iterate over child contexts within the specified container.
        /// </summary>
        private sealed class ChildContextEnumerator : IEnumerator<FieldUpdateContext>
        {
            internal ChildContextEnumerator(FieldUpdateContextContainer parent)
            {
                mParent = parent;
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            bool IEnumerator.MoveNext()
            {
                mCurrentChild = (mCurrentChild == null)
                    ? mParent.FirstChild
                    : mCurrentChild.NextSibling;
                return (mCurrentChild != null);
            }

            void IEnumerator.Reset()
            {
                mCurrentChild = null;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public FieldUpdateContext Current
            {
                get
                {
                    if (mCurrentChild == null)
                        throw new InvalidOperationException();

                    return mCurrentChild;
                }
            }

            private readonly FieldUpdateContextContainer mParent;
            private FieldUpdateContext mCurrentChild;
        }

        private FieldUpdateContext mLastChild;

#if DEBUG
        // ReSharper disable once InconsistentNaming
        internal virtual void dd()
        {
            Debug.WriteLine(ToString());
            Debug.Indent();
            foreach (FieldUpdateContext context in this)
                context.dd();
            Debug.Unindent();
        }
#endif
    }
}
