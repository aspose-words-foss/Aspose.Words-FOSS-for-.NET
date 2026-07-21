// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/08/2021 by Alexey Morozov

using Aspose.Words.Revisions;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Internal node used during comparison only.
    /// </summary>
    /// <remarks>
    /// AM. According to test Word compares SDT by content.
    /// Currently it was implemented using system marker nodes bounding SDT.
    /// Actually these marker classes are light-weight version of public StructuredDocumentRangeStart node.
    /// Main reason do not use existing StructuredDocumentTagRangeStart/End nodes is location limitation,
    /// they can be only immediate child of Body.
    /// </remarks>
    internal class SdtMarkerStart : Node, ITrackableNode
    {
        internal SdtMarkerStart(DocumentBase doc, StructuredDocumentTag sdt)
            : base(doc)
        {
            mInternalSdt = sdt;
        }

        internal StructuredDocumentTag InternalSdt
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                // Ensure that encapsulated node belongs to the same document as we can get another document
                // after some manipulation, for example, node cloned and appended to another document.
                // We need to consider making full implementation later instead of SDT encapsulation.
                if(mInternalSdt != null)
                    mInternalSdt.SetDocument(Document);

                return mInternalSdt;
            }

            set
            {
                mInternalSdt = value;
            }
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            SdtMarkerStart lhs = (SdtMarkerStart)base.Clone(isCloneChildren, cloningListener);
            lhs.InternalSdt = (StructuredDocumentTag)InternalSdt.Clone(isCloneChildren, cloningListener);

            // AM. When document is cloned we should try to preserve original Ids.
            // Otherwise range link between range start/end will be lost.
            // I don't like this, maybe it's better to handle link translation in Document.Clone()? Postpone for a while.
            lhs.InternalSdt.SetIdExplicitly(InternalSdt.Id);

            return lhs;
        }

        public override NodeType NodeType
        {
            get { return NodeType.System; }
        }
        public override bool Accept(DocumentVisitor visitor)
        {
            return true;
        }

        MoveRevision IMoveTrackableNode.MoveFromRevision
        {
            get
            {
                throw new System.NotImplementedException(ShouldNotBeCalledMessage);
            }
            set
            {
                throw new System.NotImplementedException(ShouldNotBeCalledMessage);
            }
        }

        MoveRevision IMoveTrackableNode.MoveToRevision
        {
            get
            {
                throw new System.NotImplementedException(ShouldNotBeCalledMessage);
            }
            set
            {
                throw new System.NotImplementedException(ShouldNotBeCalledMessage);
            }
        }

        void IMoveTrackableNode.RemoveMoveRevisions()
        {
            throw new System.NotImplementedException(ShouldNotBeCalledMessage);
        }

        EditRevision ITrackableNode.InsertRevision
        {
            get { return mInsertRevision; }
            set { mInsertRevision = value; }
        }

        EditRevision ITrackableNode.DeleteRevision
        {
            get { return mDeleteRevision; }
            set { mDeleteRevision = value; }
        }

        private StructuredDocumentTag mInternalSdt;
        private EditRevision mInsertRevision;
        private EditRevision mDeleteRevision;

        private const string ShouldNotBeCalledMessage = "Should not be called.";
    }
}
