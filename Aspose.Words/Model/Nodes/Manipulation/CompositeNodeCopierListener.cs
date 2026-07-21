// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/01/2016 by Edward Voronov

using System.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// An implementation of <see cref="INodeCopierListener"/> that contains a collection of listeners and applies all of them one by one.
    /// </summary>
    internal class CompositeNodeCopierListener : INodeCopierListener
    {
        internal CompositeNodeCopierListener()
        {
            mListeners = new List<INodeCopierListener>();
        }

        internal void Add(INodeCopierListener listener)
        {
            mListeners.Add(listener);
        }

        internal void Remove(INodeCopierListener listener)
        {
            mListeners.Remove(listener);
        }

        void INodeCloningListener.NotifyNodeCloned(Node source, Node clone)
        {
            foreach (INodeCopierListener listener in mListeners)
                listener.NotifyNodeCloned(source, clone);
        }

        void INodeCopierListener.NotifyNodeRangeCopied(NodeRange sourceRange, NodeRange insertedRange)
        {
            foreach (INodeCopierListener listener in mListeners)
                listener.NotifyNodeRangeCopied(sourceRange, insertedRange);
        }

        private readonly List<INodeCopierListener> mListeners;
    }
}