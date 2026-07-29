// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/05/2013 by Ivan Lyagin

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Chains <see cref="INodeChangingCallback"/> instances to invoke them sequentially on a document
    /// node changing event.
    /// </summary>
    internal class NodeChangingCallbackChain
    {
        /// <summary>
        /// Called just before a node belonging to a document is about to be inserted into another node.
        /// </summary>
        internal void NodeInserting(NodeChangingArgs args)
        {
            NodeChanging(NodeState.Inserting, args);
        }

        /// <summary>
        /// Called when a node belonging to a document has been inserted into another node. 
        /// </summary>
        internal void NodeInserted(NodeChangingArgs args)
        {
            NodeChanging(NodeState.Inserted, args);
        }

        /// <summary>
        /// Called just before a node belonging to a document is about to be removed from the document.
        /// </summary>
        internal void NodeRemoving(NodeChangingArgs args)
        {
            NodeChanging(NodeState.Removing, args);
        }

        /// <summary>
        /// Called when a node belonging to a document has been removed from its parent.
        /// </summary>
        internal void NodeRemoved(NodeChangingArgs args)
        {
            NodeChanging(NodeState.Removed, args);
        }

        private void NodeChanging(NodeState nodeState, NodeChangingArgs args)
        {
            Debug.Assert(args != null);

            // 1. Invoke internal callbacks.
            ChainItem nextChainItem = null;
            for (ChainItem chainItem = mRootChainItem; chainItem != null; chainItem = nextChainItem)
            {
                // Remember the next chain item allowing it to be removed in the corresponding callback.
                // The chain should not be broken in this case.
                nextChainItem = chainItem.NextChainItem;

                NodeChanging(chainItem.Callback, nodeState, args);
            }

            // 2. Invoke a user callback if any.
            if (mUserCallback != null)
                NodeChanging(mUserCallback, nodeState, args);
        }

        private static void NodeChanging(INodeChangingCallback callback, NodeState nodeState, NodeChangingArgs args)
        {
            switch (nodeState)
            {
                case NodeState.Inserting:
                    callback.NodeInserting(args);
                    break;
                case NodeState.Inserted:
                    callback.NodeInserted(args);
                    break;
                case NodeState.Removing:
                    callback.NodeRemoving(args);
                    break;
                case NodeState.Removed:
                    callback.NodeRemoved(args);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("nodeState");
            }
        }

        /// <summary>
        /// Adds the specified callback to the end of the invocation chain.
        /// </summary>
        internal void AddInternalCallback(INodeChangingCallback callback)
        {
            Debug.Assert(callback != null);

            ChainItem chainItemToAdd = new ChainItem(callback);
            if (mRootChainItem == null)
            {
                mRootChainItem = chainItemToAdd;
                return;
            }

            ChainItem chainItem = mRootChainItem;
            while (chainItem.NextChainItem != null)
                chainItem = chainItem.NextChainItem;

            chainItem.NextChainItem = chainItemToAdd;
        }

        /// <summary>
        /// Removes the specified callback from the invocation chain if the callback is contained in it.
        /// Returns <c>true</c> if the callback was removed.
        /// </summary>
        internal bool RemoveInternalCallback(INodeChangingCallback callback)
        {
            Debug.Assert(callback != null);

            ChainItem lastChainItem = null;
            for (ChainItem chainItem = mRootChainItem; chainItem != null; chainItem = chainItem.NextChainItem)
            {
                if (ReferenceEquals(chainItem.Callback, callback))
                {
                    if (chainItem == mRootChainItem)
                    {
                        mRootChainItem = chainItem.NextChainItem;
                    }
                    else
                    {
                        lastChainItem.NextChainItem = chainItem.NextChainItem;
                    }

                    chainItem.NextChainItem = null;
                    return true;
                }

                lastChainItem = chainItem;
            }

            return false;
        }

        /// <summary>
        /// Gets or sets a user-defined callback.
        /// </summary>
        internal INodeChangingCallback UserCallback
        {
            get { return mUserCallback; }
            set { mUserCallback = value; }
        }

        /// <summary>
        /// Returns a value indicating whether the chain does not contain any callbacks.
        /// </summary>
        internal bool IsEmpty
        {
            get { return ((mRootChainItem == null) && (mUserCallback == null)); }
        }

        /// <summary>
        /// Represents an item in the invocation chain.
        /// </summary>
        private class ChainItem
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            internal ChainItem(INodeChangingCallback callback)
            {
                mCallback = callback;
            }

            /// <summary>
            /// The callback.
            /// </summary>
            internal INodeChangingCallback Callback
            {
                get { return mCallback; }
            }

            /// <summary>
            /// The next item in the invocation chain.
            /// </summary>
            internal ChainItem NextChainItem
            {
                get { return mNextChainItem; }
                set { mNextChainItem = value; }
            }

            private readonly INodeChangingCallback mCallback;
            private ChainItem mNextChainItem;
        }

        /// <summary>
        /// Specifies a state of a node raising an event.
        /// </summary>
        private enum NodeState
        {
            Inserting,
            Inserted,
            Removing,
            Removed
        }

        private INodeChangingCallback mUserCallback;
        private ChainItem mRootChainItem;
    }
}
