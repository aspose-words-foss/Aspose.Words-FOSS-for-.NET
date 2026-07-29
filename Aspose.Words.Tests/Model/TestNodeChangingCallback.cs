// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/06/2017 by Dmitry Sokolov

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Test implementation of <see cref="INodeChangingCallback"/>.
    /// </summary>
    public class TestNodeChangingCallback : INodeChangingCallback
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal TestNodeChangingCallback()
            : this(null)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="documentToUnscribe">
        /// If specified, then the created instance is removed from a chain of the given document
        /// on the first listened node insertion.
        /// </param>
        internal TestNodeChangingCallback(Document documentToUnscribe)
        {
            mDocumentToUnscribe = documentToUnscribe;
        }

        void INodeChangingCallback.NodeInserting(NodeChangingArgs args)
        {
            // Do nothing.
        }

        void INodeChangingCallback.NodeInserted(NodeChangingArgs args)
        {
            mInsertedCount++;

            // Unscribe from events if the document is specified.
            if (mDocumentToUnscribe != null)
            {
                if (mDocumentToUnscribe.NodeChangingCallback == this)
                {
                    mDocumentToUnscribe.NodeChangingCallback = null;
                }
                else
                {
                    mDocumentToUnscribe.RemoveInternalNodeChangingCallback(this);
                }
            }
        }

        void INodeChangingCallback.NodeRemoving(NodeChangingArgs args)
        {
            // Do nothing.
        }

        void INodeChangingCallback.NodeRemoved(NodeChangingArgs args)
        {
            mRemovedCount++;
        }

        /// <summary>
        /// Gets the number of nodes which have been inserted while listening.
        /// </summary>
        internal int InsertedCount
        {
            get { return mInsertedCount; }
        }

        /// <summary>
        /// Gets the number of nodes which have been removed while listening.
        /// </summary>
        internal int RemovedCount
        {
            get { return mRemovedCount; }
        }

        private readonly Document mDocumentToUnscribe;
        private int mInsertedCount;
        private int mRemovedCount;
    }
}
