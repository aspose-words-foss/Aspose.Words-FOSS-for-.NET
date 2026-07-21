// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2005 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Provides data for methods of the <see cref="INodeChangingCallback"/> interface.
    ///
    /// <seealso cref="DocumentBase"/>
    /// <seealso cref="INodeChangingCallback"/>
    /// <seealso cref="NodeChangingAction"/>
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    public class NodeChangingArgs
    {
        internal NodeChangingArgs(Node node, Node oldParent, Node newParent, NodeChangingAction action)
        {
            mNode = node;
            mOldParent = oldParent;
            mNewParent = newParent;
            mAction = action;
        }

        /// <summary>
        /// Gets the <see cref="Node"/> that is being added or removed.
        /// </summary>
        public Node Node
        {
            get { return mNode; }
        }

        /// <summary>
        /// Gets the node's parent before the operation began.
        /// </summary>
        public Node OldParent
        {
            get { return mOldParent; }
        }

        /// <summary>
        /// Gets the node's parent that will be set after the operation completes.
        /// </summary>
        public Node NewParent
        {
            get { return mNewParent; }
        }

        /// <summary>
        /// Gets a value indicating what type of node change event is occurring.
        /// </summary>
        public NodeChangingAction Action
        {
            get { return mAction; }
        }

        private readonly Node mNode;
        private readonly Node mOldParent;
        private readonly Node mNewParent;
        private readonly NodeChangingAction mAction;
    }
}
