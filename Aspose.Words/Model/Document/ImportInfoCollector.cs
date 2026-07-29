// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/10/2018 by Ilya Navrotskiy

using Aspose.Collections.Generic;
using Aspose.Words.Lists;

namespace Aspose.Words
{
    /// <summary>
    /// Class for collecting all necessary information about cloned nodes during import operations.
    /// </summary>
    internal class ImportInfoCollector : INodeCloningListener
    {
        /// <summary>
        /// Collects necessary information about current importing node.
        /// </summary>
        public void NotifyNodeCloned(Node source, Node clone)
        {
            switch (clone.NodeType)
            {
                case NodeType.Section:
                {
                    if (mSrcDoc == null)
                        mSrcDoc = ((Section)source).Document;

                    // Initialize at each new section.
                    Init();
                    IsLastSection = ((Section)source).IsLastSection;
                    IsFirstSection = ((Section)source).IsFirstSection;
                    break;
                }
                case NodeType.Body:
                {
                    mBody = (Body)clone;
                    mIsInBody = true;
                    break;
                }
                case NodeType.HeaderFooter:
                {
                    mIsInBody = false;
                    break;
                }
                case NodeType.Paragraph:
                {
                    // WORDSNET-22709 Collect info about used in direct attributes ListDefIds within source document.
                    Paragraph srcPara = (Paragraph)source;
                    if (!HasStyleListInDirectAttrs)
                    {
                        object listId = srcPara.ParaPr[ParaAttr.ListId];
                        if (listId != null)
                        {
                            List list = mSrcDoc.Lists.GetListByListId((int)listId);
                            if ((list != null) && SrcStylesListDefIds.Contains(list.ListDefId))
                                HasStyleListInDirectAttrs = true;
                        }
                    }

                    // We need info about nodes only inside a Body.
                    if (mIsInBody)
                    {
                        Paragraph para = (Paragraph)clone;

                        int listId = (int)((IParaAttrSource)para).FetchParaAttr(ParaAttr.ListId);
                        if (listId != 0)
                            mListIds.Add(listId);
                    }
                    break;
                }
                case NodeType.Run:
                {
                    if ((FirstRun == null) && mIsInBody)
                        FirstRun = (Run)clone;
                    break;
                }
                default:
                    // do nothing
                    break;
            }
        }

        /// <summary>
        /// Initializes class members.
        /// </summary>
        private void Init()
        {
            mIsInBody = false;
            mBody = null;
            mFirstNode = null;
            mLastNode = null;
            mImportedParagraphs = null;
            mListIds.Clear();
        }

        /// <summary>
        /// Gets first imported non-annotation node inside Body.
        /// </summary>
        internal Node FirstNode
        {
            get
            {
                if ((mFirstNode == null) && (mBody != null))
                    mFirstNode = mBody.FirstNonAnnotationChild;

                return mFirstNode;
            }
        }

        /// <summary>
        /// Gets last imported non-annotation node inside Body.
        /// </summary>
        internal Node LastNode
        {
            get
            {
                if ((mLastNode == null) && (mBody != null))
                    mLastNode = mBody.LastNonAnnotationChild;

                return mLastNode;
            }
        }

        /// <summary>
        /// Returns true, if first non-annotation imported node inside Body is a Table.
        /// </summary>
        internal bool IsFirstNodeTable
        {
            get { return ((FirstNode != null) && (FirstNode.NodeType == NodeType.Table)); }
        }

        /// <summary>
        /// Returns true, if first non-annotation imported node inside Body is a SDT.
        /// </summary>
        internal bool IsFirstNodeSdt
        {
            get { return ((FirstNode != null) && (FirstNode.NodeType == NodeType.StructuredDocumentTag)); }
        }

        /// <summary>
        /// Returns true, if last non-annotation imported node inside Body is a SDT.
        /// </summary>
        internal bool IsLastNodeSdt
        {
            get { return ((LastNode != null) && (LastNode.NodeType == NodeType.StructuredDocumentTag)); }
        }

        /// <summary>
        /// Returns true, if imported content has lists.
        /// </summary>
        internal bool HasLists
        {
            get { return (mListIds.Count > 0); }
        }

        /// <summary>
        /// Returns true, if imported content has exactly one list.
        /// </summary>
        internal bool HasSingleList
        {
            get { return (mListIds.Count == 1); }
        }

        /// <summary>
        /// Gets deep collection of imported paragraphs inside Body.
        /// </summary>
        internal NodeCollection ImportedParagraphs
        {
            get
            {
                if ((mImportedParagraphs == null) && (mBody != null))
                    mImportedParagraphs = new NodeCollection(mBody, NodeType.Paragraph, true);

                return mImportedParagraphs;
            }
        }

        /// <summary>
        /// Returns true, if importing last section.
        /// </summary>
        internal bool IsLastSection { get; private set; }

        /// <summary>
        /// Returns true, if importing first section.
        /// </summary>
        internal bool IsFirstSection { get; private set; }

        /// <summary>
        /// Gets first imported Run inside a Body.
        /// </summary>
        internal Run FirstRun { get; private set; }

        /// <summary>
        /// The collection of ListDefIds used in styles of source document.
        /// </summary>
        private HashSetGeneric<int> SrcStylesListDefIds
        {
            get
            {
                if (mSrcStylesListDefIds == null)
                {
                    mSrcStylesListDefIds = new HashSetGeneric<int>();
                    foreach (Style style in mSrcDoc.Styles)
                    {
                        object listId = style.ParaPr[ParaAttr.ListId];
                        if (listId != null)
                        {
                            List list = mSrcDoc.Lists.GetListByListId((int)listId);
                            if ((list != null) && !mSrcStylesListDefIds.Contains(list.ListDefId))
                                mSrcStylesListDefIds.Add(list.ListDefId);
                        }
                    }
                }

                return mSrcStylesListDefIds;
            }
        }

        /// <summary>
        /// Indicates either some imported paragraph has ListId in direct attributes in source document,
        /// with the same ListDefId as one of the numbered styles in this document.
        /// </summary>
        internal bool HasStyleListInDirectAttrs { get; private set; }

        /// <summary>
        /// Indicates either cloned node is inside a Body.
        /// </summary>
        /// <remarks>
        /// Note, we mostly need info about nodes only inside a body. So, this flag is introduced just for a speed reason
        /// to avoid checking ancestor type for every cloned node.
        /// </remarks>
        private bool mIsInBody;

        /// <summary>
        /// The HashSet of list ids of imported paragraphs.
        /// </summary>
        private readonly HashSetGeneric<int> mListIds = new HashSetGeneric<int>();

        /// <summary>
        /// The HashSet of ListDefIds used in styles of source document.
        /// </summary>
        private HashSetGeneric<int> mSrcStylesListDefIds;

        private DocumentBase mSrcDoc;
        private Body mBody;
        private Node mFirstNode;
        private Node mLastNode;

        private NodeCollection mImportedParagraphs;
    }
}
