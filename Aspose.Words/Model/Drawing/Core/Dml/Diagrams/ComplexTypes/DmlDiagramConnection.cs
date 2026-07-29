// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.3.2 cxn (Connection)
    /// </summary>
    internal class DmlDiagramConnection
    {
        internal DmlModelId ModelId
        {
            get { return mModelId; }
            set { mModelId = value; }
        }

        internal DmlConnectionType Type
        {
            get { return mType; }
            set { mType = value; }
        }

        internal DmlModelId DestinationId
        {
            get { return mDestinationId; }
            set { mDestinationId = value; }
        }

        internal int DestinationOrder
        {
            get { return mDestinationOrder; }
            set { mDestinationOrder = value; }
        }

        internal DmlModelId SourceId
        {
            get { return mSourceId; }
            set { mSourceId = value; }
        }

        internal int SourceOrder
        {
            get { return mSourceOrder; }
            set { mSourceOrder = value; }
        }

        internal DmlModelId ParentTransitionId
        {
            get { return mParentTransitionId; }
            set { mParentTransitionId = value; }
        }

        internal DmlModelId SiblingTransitionId
        {
            get { return mSiblingTransitionId; }
            set { mSiblingTransitionId = value; }
        }

        internal string PresentationId
        {
            get { return mPresentationId; }
            set { mPresentationId = value; }
        }

        private DmlModelId mModelId;
        private DmlConnectionType mType = DmlConnectionType.ParentOf;
        private DmlModelId mDestinationId;
        private int mDestinationOrder;
        private DmlModelId mSourceId;
        private int mSourceOrder;
        private DmlModelId mParentTransitionId;
        private DmlModelId mSiblingTransitionId;
        private string mPresentationId;
    }
}
