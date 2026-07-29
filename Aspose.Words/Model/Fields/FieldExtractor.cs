// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/01/2004 by Roman Korchagin

using System;
using System.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Extracts field nodes from a model into complete field objects.
    ///
    /// When processing nested field and extracting into an array, returns the innermost fields first!
    /// This was probably initially a defect, but then we had customers who relied on this behaviour an
    /// therefore it is kept for backward compatibility.
    ///
    /// When extracting from a part of a document tree you can get fields that start inside
    /// this part or end inside this part, but are not completely contained in it. Such fields
    /// are not included in the result.
    ///
    /// </summary>
    internal abstract class FieldExtractor : DocumentVisitor
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="fieldTypes">Types of fields to extract.</param>
        protected FieldExtractor(params FieldType[] fieldTypes)
        {
            mFieldTypes = fieldTypes;
            mExtractAllFields = (fieldTypes == null) || (fieldTypes.Length == 0);
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        protected FieldExtractor()
            : this(null)
        {
        }

        /// <summary>
        /// A static method to simplify invocation.
        /// </summary>
        internal static IList<Field> ExtractToCollection(Node node)
        {
            return ExtractToCollection(node, true);
        }

        /// <summary>
        /// A static method to simplify invocation.
        /// </summary>
        internal static IList<Field> ExtractToCollection(NodeRange range)
        {
            return ExtractToCollection(range, true, null);
        }

        /// <summary>
        /// A static method to simplify invocation.
        /// </summary>
        internal static List<Field> ExtractToCollection(Node node, bool isDeep)
        {
            return ExtractToCollection(node, isDeep, null);
        }

        /// <summary>
        /// A static method to simplify invocation.
        /// </summary>
        internal static List<Field> ExtractToCollection(Node node, bool isDeep, params FieldType[] fieldTypes)
        {
            FieldExtractorToCollection extractor = new FieldExtractorToCollection(isDeep, fieldTypes);
            extractor.Extract(node);
            return extractor.Fields;
        }

        /// <summary>
        /// A static method to simplify invocation.
        /// </summary>
        internal static IList<Field> ExtractToCollection(NodeRange range, bool isDeep, params FieldType[] fieldTypes)
        {
            FieldExtractorToCollection extractor = new FieldExtractorToCollection(isDeep, fieldTypes);
            extractor.Extract(range);
            return extractor.Fields;
        }

        /// <summary>
        /// A static method to simplify invocation.
        /// </summary>
        internal static List<Field> ExtractToCollection(IList<Node> nodes, bool isDeep)
        {
            return ExtractToCollection(nodes, isDeep, null);
        }

        /// <summary>
        /// A static method to simplify invocation.
        /// </summary>
        internal static List<Field> ExtractToCollection(
            IList<Node> nodes,
            bool isDeep,
            params FieldType[] fieldTypes)
        {
            FieldExtractorToCollection extractor = new FieldExtractorToCollection(isDeep, fieldTypes);
            extractor.Extract(nodes);
            return extractor.Fields;
        }

        /// <summary>
        /// A static method to simplify invocation.
        /// Key is <see cref="FieldStart"/> and value is <see cref="Field"/>.
        /// </summary>
        internal static IDictionary<Node, Field> ExtractToHashtable(Node node)
        {
            FieldExtractorToHashtable extractor = new FieldExtractorToHashtable();
            extractor.Extract(node);
            return extractor.Fields;
        }

        /// <summary>
        /// Extracts fields from the node and all of its children recursively.
        /// </summary>
        [JavaConvertCheckedExceptions]
        internal void Extract(Node node)
        {
            node.Accept(this);
        }

        /// <summary>
        /// Extracts fields from the listed nodes and all of their children recursively.
        /// </summary>
        /// <param name="nodes"></param>
        internal void Extract(IList<Node> nodes)
        {
            foreach (Node node in nodes)
                Extract(node);
        }

        /// <summary>
        /// Extracts fields from the nodes of the range.
        /// </summary>
        internal void Extract(NodeRange range)
        {
            using (NodeRangeFieldExtractHelper helper = new NodeRangeFieldExtractHelper(range, this))
                helper.Extract();
        }

        [JavaThrows(true)]
        public override VisitorAction VisitFieldStart(FieldStart fieldStart)
        {
            if (fieldStart.IsVisitorAcceptable(this))
                mFieldCharsStack.Push(fieldStart);

            return VisitorAction.Continue;
        }

        [JavaThrows(true)]
        public override VisitorAction VisitFieldSeparator(FieldSeparator fieldSeparator)
        {
            if (fieldSeparator.IsVisitorAcceptable(this))
                mFieldCharsStack.Push(fieldSeparator);

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {
            if (!fieldEnd.IsVisitorAcceptable(this))
                return VisitorAction.Continue;

            // WORDSNET-23717 FieldSeparator can be removed from the model. Don't throw.
            FieldSeparator fieldSeparator = (FieldSeparator)mFieldCharsStack.PopIfInstanceOf(typeof(FieldSeparator));
            if (fieldSeparator != null && fieldSeparator.IsDeleteRevision)
                fieldSeparator = null;

            // WORDSNET-23717 FieldStart can be removed from the model. Don't throw.
            FieldStart fieldStart = (FieldStart)mFieldCharsStack.PopIfInstanceOf(typeof(FieldStart));
            if (fieldStart == null)
            {
                // Ignore the orphan field end.
                return VisitorAction.Continue;
            }

            // Note, that we still push to stack and pop from stack field chars which corresponding fields
            // are not to be extracted, to make IsInField property work properly.
            if (!IsExtractNeeded(fieldStart, fieldSeparator, fieldEnd))
                return VisitorAction.Continue;

            // RK This code results in innermost fields being added to the returned array
            // before the outermost fields and therefore further processing of the field array
            // will process the innermost fields first.
            // Do not change this! Customers already rely on this behaviour.
            mCurrentFieldStart = fieldStart;
            mCurrentFieldSeparator = fieldSeparator;
            mCurrentFieldEnd = fieldEnd;

            OnFieldExtracted();

            mCurrentFieldStart = null;
            mCurrentFieldSeparator = null;
            mCurrentFieldEnd = null;
            mCurrentField = null;

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitHeaderFooterStart(HeaderFooter headerFooter)
        {
            // WORDSNET-11428 Fields within header/footer should be not extracted as child of body field, when it occupied several sections.
            mLastFieldStart = mCurrentFieldStart;
            mLastFieldSeparator = mCurrentFieldSeparator;
            mLastFieldEnd = mCurrentFieldEnd;
            mLastField = mCurrentField;
            mLastFieldCharsStack = mFieldCharsStack;

            mCurrentFieldStart = null;
            mCurrentFieldSeparator = null;
            mCurrentFieldEnd = null;
            mCurrentField = null;
            mFieldCharsStack = new Stack<FieldChar>();

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitHeaderFooterEnd(HeaderFooter headerFooter)
        {
            mCurrentFieldStart = mLastFieldStart;
            mCurrentFieldSeparator = mLastFieldSeparator;
            mCurrentFieldEnd = mLastFieldEnd;
            mCurrentField = mLastField;
            mFieldCharsStack = mLastFieldCharsStack;

            return VisitorAction.Continue;
        }

        private bool IsExtractNeeded(FieldStart fieldStart, FieldSeparator fieldSeparator, FieldEnd fieldEnd)
        {
            // WORDSNET-12253 Ignore fields which contain deleted chars.
            if (fieldStart.IsDeleteRevision || fieldEnd.IsDeleteRevision)
                return false;

            // Ignore fields which occupy multiple stories.
            if (!NodeUtil.AreNodesInSameStory(fieldStart, fieldEnd))
                return false;

            if ((fieldSeparator != null) && !NodeUtil.AreNodesInSameStory(fieldStart, fieldSeparator))
                return false;

            return (mExtractAllFields || (Array.IndexOf(mFieldTypes, fieldStart.FieldType) != -1));
        }

        [JavaThrows(true)]
        protected abstract void OnFieldExtracted();

        /// <summary>
        /// A lazily initialized <see cref="Field"/> instance representing the current extracted field.
        /// </summary>
        protected Field CurrentField
        {
            get
            {
                Debug.Assert(mCurrentFieldStart != null);

                // Do not create an instance of a field until it is really needed. In most cases we deal with the topmost
                // fields and hence there is no need to create nested fields' instances. Creating of a field instance can be
                // quite expensive, because some of the fields parse their field codes while creating.
                if (mCurrentField == null)
                {
                    mCurrentField = FieldFactory.CreateField(mCurrentFieldStart, mCurrentFieldSeparator, mCurrentFieldEnd);
                    mCurrentField = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(mCurrentField, mCurrentFieldStart, mCurrentFieldStart.Document);
                }
                return mCurrentField;
            }
        }

        /// <summary>
        /// Gets the current extracted field's type without creating of a <see cref="Field"/> instance. This property
        /// should be used to perform checks (if any) before accessing <see cref="CurrentField"/> not to create an unneeded
        /// <see cref="Field"/> instance.
        /// </summary>
        protected FieldType CurrentFieldType
        {
            get
            {
                Debug.Assert(mCurrentFieldStart != null);

                return mCurrentFieldStart.FieldType;
            }
        }

        protected bool IsInField
        {
            get { return (mFieldCharsStack.Count > 0); }
        }

#if DEBUG
        protected bool IsInFieldCode
        {
            get
            {
                if (!IsInField)
                    return false;

                return mFieldCharsStack.Peek().NodeType == NodeType.FieldStart;
            }
        }

        protected FieldStart ParentFieldStart
        {
            get
            {
                Debug.Assert(IsInFieldCode);
                return (FieldStart)mFieldCharsStack.Peek();
            }
        }

        protected bool IsInFieldResult
        {
            get
            {
                if (!IsInField)
                    return false;

                return mFieldCharsStack.Peek().NodeType == NodeType.FieldSeparator;
            }
        }

        protected FieldSeparator ParentFieldSeparator
        {
            get
            {
                Debug.Assert(IsInFieldResult);
                return (FieldSeparator)mFieldCharsStack.Peek();
            }
        }
#endif

        /// <summary>
        /// Helps to extract fields from a node range by traversing it.
        /// </summary>
        private class NodeRangeFieldExtractHelper : NodeTraverser
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="range"></param>
            /// <param name="fieldExtractor"></param>
            internal NodeRangeFieldExtractHelper(NodeRange range, FieldExtractor fieldExtractor)
                : base(range)
            {
                mFieldExtractor = fieldExtractor;
            }

            /// <summary>
            /// Extracts fields using the specified field extractor.
            /// </summary>
            internal void Extract()
            {
                Traverse();
            }

            protected override void OnNonCompositeNode()
            {
                mFieldExtractor.Extract(CurrentNode);
            }

            protected override void OnMiddleNodeAncestor()
            {
                mFieldExtractor.Extract(CurrentNode);
            }

            private readonly FieldExtractor mFieldExtractor;
        }

        /// <summary>
        /// Field begin and separator characters are pushed to this stack when processing nested fields.
        /// </summary>
        private Stack<FieldChar> mFieldCharsStack = new Stack<FieldChar>();

        private readonly FieldType[] mFieldTypes;
        private readonly bool mExtractAllFields;

        private FieldStart mCurrentFieldStart;
        private FieldSeparator mCurrentFieldSeparator;
        private FieldEnd mCurrentFieldEnd;
        private Field mCurrentField;

        private Stack<FieldChar> mLastFieldCharsStack = new Stack<FieldChar>();
        private FieldStart mLastFieldStart;
        private FieldSeparator mLastFieldSeparator;
        private FieldEnd mLastFieldEnd;
        private Field mLastField;
    }
}
