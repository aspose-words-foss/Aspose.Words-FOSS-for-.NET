// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/01/2013 by Ivan Lyagin

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// A collection of <see cref="Field"/> objects that represents the fields in the specified range.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>An instance of this collection iterates fields which start fall within the specified range.</p>
    /// <p>The <see cref="FieldCollection"/> collection does not own the fields it contains, rather, is just a selection of fields.</p>
    /// <p>The <see cref="FieldCollection"/> collection is "live", i.e. changes to the children of the node object
    /// that it was created from are immediately reflected in the fields returned by the <see cref="FieldCollection"/>
    /// properties and methods.</p>
    /// </remarks>
    public class FieldCollection : IEnumerable<Field>
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="parent"></param>
        internal FieldCollection(Node parent)
        {
            mFieldStarts = parent.IsComposite
                ? ((CompositeNode)parent).GetChildNodes(NodeType.FieldStart, true)
                : EmptyNodeCollection.CreateEmpty();
        }

        internal FieldCollection(NodeRange nodeRange)
        {
            mFieldStarts = new NodeCollection(nodeRange.Document, new FieldMatcher(nodeRange), true);
        }

        /// <summary>
        /// Returns the number of the fields in the collection.
        /// </summary>
        public int Count
        {
            get { return mFieldStarts.Count; }
        }

        /// <summary>
        /// Returns a field at the specified index.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the collection.</param>
        public Field this[int index]
        {
            get
            {
                FieldStart fieldStart = (FieldStart)mFieldStarts[index];
                return (fieldStart != null) ? fieldStart.GetField() : null;
            }
        }

        /// <summary>
        /// Removes the specified field from this collection and from the document.
        /// </summary>
        /// <param name="field">A field to remove.</param>
#pragma warning disable CA1822 // public API. as designed
        public void Remove(Field field)
#pragma warning restore CA1822
        {
            if (field == null)
                throw new ArgumentNullException("field");

            field.Remove();
        }

        /// <summary>
        /// Removes a field at the specified index from this collection and from the document.
        /// </summary>
        /// <param name="index">An index into the collection.</param>
        public void RemoveAt(int index)
        {
            FieldStart fieldStart = (FieldStart)mFieldStarts[index];
            if (fieldStart == null)
                throw new ArgumentOutOfRangeException("index");

            Remove(fieldStart);
        }

        /// <summary>
        /// Removes the whole field by the specified field start.
        /// </summary>
        /// <param name="fieldStart"></param>
        private static void Remove(FieldStart fieldStart)
        {
            Debug.Assert(fieldStart != null);

            FieldEnd fieldEnd = FieldBundle.GetFieldBundle(fieldStart).End;
            NodeRemover.Remove(fieldStart, fieldEnd);
        }

        /// <summary>
        /// Removes all fields of this collection from the document and from this collection itself.
        /// </summary>
        public void Clear()
        {
            // Fields can be nested to each other so we can not rely on their count like we do for BookmarkCollection.
            while (true) // Break inside.
            {
                FieldStart fieldStart = (FieldStart)mFieldStarts[0];
                if (fieldStart == null)
                    break;

                Remove(fieldStart);
            }
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<Field> GetEnumerator()
        {
            return new FieldEnumerator(mFieldStarts);
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipEntity("C++ doesn't support untyped collection interfaces")]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class FieldEnumerator : IEnumerator<Field>
        {
            internal FieldEnumerator(NodeCollection fieldStarts)
            {
                mFieldStartsEnumerator = fieldStarts.GetEnumerator();
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            [JavaConvertCheckedExceptions]
            public Field Current
            {
                get
                {
                    FieldStart fieldStart = (FieldStart)mFieldStartsEnumerator.Current;
                    return fieldStart.GetField();
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            [JavaConvertCheckedExceptions]
            public bool MoveNext()
            {
                return mFieldStartsEnumerator.MoveNext();
            }

            [JavaDelete]
            public void Reset()
            {
                mFieldStartsEnumerator.Reset();
            }

            private readonly IEnumerator<Node> mFieldStartsEnumerator;
        }

        /// <summary>
        /// This is a live collection of field start nodes.
        /// </summary>
        private readonly NodeCollection mFieldStarts;

        private class FieldMatcher : NodeMatcher
        {
            internal FieldMatcher(NodeRange range)
            {
                mNodeRange = range;
                UpdateFieldsCache();
            }

            /// <summary>
            /// Returns true if the specified node matches the appropriate FieldStart node.
            /// </summary>
            internal override bool IsMatch(Node node)
            {
                if (mInitialChangeCount != Document.TreeChangeCount)
                    UpdateFieldsCache();

                if (mRangeIsEmpty || (node.NodeType != NodeType.FieldStart))
                    return false;

                return mFieldNodes.Contains(node);
            }

            private void UpdateFieldsCache()
            {
                mInitialChangeCount = Document.TreeChangeCount;

                mFieldNodes.Clear();

                mRangeIsEmpty = mNodeRange.IsEmpty || mNodeRange.IsVoid ||
                                mNodeRange.Start.Node.IsRemoved ||
                                mNodeRange.End.Node.IsRemoved;

                if (mRangeIsEmpty)
                    return;

                IList<Field> fields = FieldExtractor.ExtractToCollection(mNodeRange);
                foreach (Field field in fields)
                    mFieldNodes.Add(field.Start);
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
            private readonly List<Node> mFieldNodes = new List<Node>();
            private readonly NodeRange mNodeRange;
        }
    }
}
