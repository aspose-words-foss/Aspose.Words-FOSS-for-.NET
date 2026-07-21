// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin
using System;
using System.Collections;
using System.Collections.Generic;


namespace Aspose.Words.Fields
{
    /// <summary>
    /// A collection of <see cref="FormField"/> objects that represent all the form fields in a range.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-form-fields/">Working with Form Fields</a> documentation article.</para>
    /// </summary>
    /// <dev>Facade wrapper for a collection of form field objects.</dev>
    public class FormFieldCollection : IEnumerable<FormField>
    {
        internal FormFieldCollection(Node parent)
        {
            mItems = parent.IsComposite
                ? ((CompositeNode)parent).GetChildNodes(NodeType.FormField, true)
                : EmptyNodeCollection.CreateEmpty();
        }

        internal FormFieldCollection(NodeRange nodeRange)
        {
            mItems = new NodeCollection(nodeRange.Document, new FormFieldMatcher(nodeRange), true);
        }

        /// <summary>
        /// Returns the number of form fields in the collection.
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Returns a form field at the specified index.
        /// </summary>
        /// <include file='..\..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the collection.</param>
        public FormField this[int index]
        {
            get { return (FormField)mItems[index]; }
        }

        /// <summary>
        /// Returns a form field by bookmark name.
        /// </summary>
        /// <remarks>
        /// Returns <c>null</c> if the form field with the specified bookmark name cannot be found.
        /// </remarks>
        /// <param name="bookmarkName">Case-insensitive bookmark name.</param>
        public FormField this[string bookmarkName]
        {
            get
            {
                ArgumentUtil.CheckHasChars(bookmarkName, "bookmarkName");

                for (int i = 0; i < Count; i++)
                {
                    if (StringUtil.EqualsIgnoreCase(this[i].Name, bookmarkName))
                        return this[i];
                }
                return null;
            }
        }

#if PYNET // Indexers for non int type is not supported in Python.
        /// <summary>
        /// Returns a form field by bookmark name.
        /// </summary>
        public FormField GetByName(string bookmarkName)
        {
            return this[bookmarkName];
        }
#endif

        /// <summary>
        /// Removes a form field with the specified name.
        /// </summary>
        /// <remarks>
        /// If there is a bookmark associated with the form field, the bookmark is not removed.
        /// </remarks>
        /// <param name="formField">The case-insensitive name of the form field to remove.</param>
        public void Remove(string formField)
        {
            ArgumentUtil.CheckHasChars(formField, "formField");
            RemoveInternal(this[formField]);
        }

        /// <summary>
        /// Removes a form field at the specified index.
        /// </summary>
        /// <remarks>
        /// If there is a bookmark associated with the form field, the bookmark is not removed.
        /// </remarks>
        /// <param name="index">The zero-based index of the form field to remove.</param>
        public void RemoveAt(int index)
        {
            RemoveInternal(this[index]);
        }

        /// <summary>
        /// Removes all form fields from this collection and from the document.
        /// </summary>
        public void Clear()
        {
            // SPEED This is a proper way for removing.
            int remaining = Count;
            while (remaining > 0)
            {
                RemoveAt(0);
                remaining--;
            }
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<FormField> GetEnumerator()
        {
#if !JAVA
            return mItems.GetNodeEnumerator<FormField>();
#else
            return mItems.getNodeEnumerator();
#endif
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static void RemoveInternal(FormField formField)
        {
            if (formField == null)
                throw new ArgumentNullException("formField");

            //Note I cannot call just Node.Remove because a form field consists of multiple nodes.
            formField.RemoveField();
        }

        private readonly NodeCollection mItems;

        private class FormFieldMatcher : NodeMatcher
        {
            internal FormFieldMatcher(NodeRange range)
            {
                mNodeRange = range;
                UpdateFormFieldsCache();
            }

            /// <summary>
            /// Returns true if the specified node matches the appropriate FormField node.
            /// </summary>
            internal override bool IsMatch(Node node)
            {
                if (mInitialChangeCount != Document.TreeChangeCount)
                    UpdateFormFieldsCache();

                if ((node.NodeType != NodeType.FormField) || mRangeIsEmpty)
                    return false;

                return mFormFields.Contains(node);
            }

            private void UpdateFormFieldsCache()
            {
                mInitialChangeCount = Document.TreeChangeCount;

                mRangeIsEmpty = mNodeRange.IsEmpty || mNodeRange.IsVoid ||
                                mNodeRange.Start.Node.IsRemoved ||
                                mNodeRange.End.Node.IsRemoved;

                if (mRangeIsEmpty)
                    return;

                mFormFields = NodeFinder.FindNodes(mNodeRange, NodeType.FormField);
            }

            internal override bool IsSkipMarkupNodes
            {
                get { return true; }
            }

            private Document Document
            {
                get { return mNodeRange.Document.FetchDocument(); }
            }

            private bool mRangeIsEmpty;
            private int mInitialChangeCount;
            private IList<Node> mFormFields;
            private readonly NodeRange mNodeRange;
        }
    }
}

