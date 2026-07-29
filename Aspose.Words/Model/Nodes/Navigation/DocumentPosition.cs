// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Michael Morozoff

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a position in the document tree.
    /// </summary>
    internal class DocumentPosition
    {
        /// <summary>
        /// Initializes new instance of this class.
        /// </summary>
        internal DocumentPosition(Node node) : this(node, 0)
        {
        }

        /// <summary>
        /// Initializes new instance of this class.
        /// </summary>
        internal DocumentPosition(Node node, int offset) : this(node, offset, new DefaultAccessor())
        {
        }

        /// <summary>
        /// Initializes new instance of this class.
        /// </summary>
        internal DocumentPosition(Node node, int offset, IDocumentTreeAccessor accessor)
        {
            Debug.Assert(accessor != null);
            Debug.Assert(offset >= 0);

            mNode = node;
            mAccessor = accessor;
            Offset = offset;
            ResetLength();
        }

        /// <summary>
        /// Returns a position before the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static DocumentPosition CreatePositionBefore(Node node)
        {
            DocumentPosition position = new DocumentPosition(node);
            position.MoveNodeStart();
            return position;
        }

        /// <summary>
        /// Returns a position after the specified node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static DocumentPosition CreatePositionAfter(Node node)
        {
            DocumentPosition position = new DocumentPosition(node);
            position.MoveNodeEnd();
            return position;
        }

        /// <summary>
        /// Gets or sets node.
        /// </summary>
        internal Node Node
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mNode; }
            set
            {
                mNode = value;

                // Reset cached length of the node.
                ResetLength();
            }
        }

        /// <summary>
        /// Gets or sets offset into the node.
        /// </summary>
        /// <remarks>If node is composite valid values are 0 (before the node) and 1 (after the node).</remarks>
        /// <remarks>Cannot normalize value in setter because mNode may be null.</remarks>
        internal int Offset { [CodePorting.Translator.Cs2Cpp.CppConstMethod] get; set; }

        /// <summary>
        /// Returns true if the position is before the node.
        /// </summary>
        internal bool IsStart
        {
            get { return (0 >= Offset); }
        }

        /// <summary>
        /// Returns true if the position is after the node.
        /// </summary>
        internal bool IsEnd
        {
            get { return Length <= Offset; }
        }

        /// <summary>
        /// Returns true if node is null.
        /// </summary>
        internal bool IsVoid
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return (null == Node); }
        }

        /// <summary>
        /// Moves to the start of the <see cref="Node"/>.
        /// </summary>
        internal void MoveNodeStart()
        {
            Offset = 0;
        }

        /// <summary>
        /// Moves to the end of the <see cref="Node"/>.
        /// </summary>
        internal void MoveNodeEnd()
        {
            Offset = Length;
        }

        /// <summary>
        /// Moves to the next position.
        /// </summary>
        /// <param name="root">Root of the scope subtree. Position will not go above the root node.</param>
        /// <param name="isForward">True if position moves to the next node (to the right); False if moves to the previous node (to the left).</param>
        /// <param name="isDeep">True if position stops at child nodes of composites; False if it doesn't descend composites.</param>
        /// <param name="isGlobal">True if position moves between stories; False if position moves within the current story.</param>
        /// <param name="isInside">True if position stops at inner offsets of the runs.</param>
        /// <param name="isAround">True if position stops before and after the node; False if position stops before the node.</param>
        /// <returns>True if moved successfully; False otherwise.</returns>
        internal bool Move(
            Node root,
            bool isForward,
            bool isDeep,
            bool isGlobal,
            bool isInside,
            bool isAround)
        {
            return Move(root, isForward, isDeep, isGlobal, isInside, isAround, null);
        }

        /// <summary>
        /// Moves to the next position.
        /// </summary>
        /// <param name="root">Root of the scope subtree. Position will not go above the root node.</param>
        /// <param name="isForward">True if position moves to the next node (to the right); False if moves to the previous node (to the left).</param>
        /// <param name="isDeep">True if position stops at child nodes of composites; False if it doesn't descend composites.</param>
        /// <param name="isGlobal">True if position moves between stories; False if position moves within the current story.</param>
        /// <param name="isInside">True if position stops at inner offsets of the runs.</param>
        /// <param name="isAround">True if position stops before and after the node; False if position stops before the node.</param>
        /// <param name="listener">An object that receives notifications about movements.</param>
        /// <returns>True if moved successfully; False otherwise.</returns>
        internal bool Move(
            Node root,
            bool isForward,
            bool isDeep,
            bool isGlobal,
            bool isInside,
            bool isAround,
            IDocumentPositionListener listener)
        {
            // First move.
            if (IsVoid)
            {
                // If node is not specified and we have no root then we cannot move.
                if (null == root)
                    throw new ArgumentNullException("root");

                Node = root;
                Offset = (isForward ? 0 : Length);
                return true;
            }

            // Normalize offset.
            Offset = System.Math.Min(Length, System.Math.Max(0, Offset));

            DocumentPositionMovement movement = DocumentPositionMovement.None;

            if (MoveInside(isForward, isInside, isAround))
                movement = DocumentPositionMovement.Inside;
            else if (MoveBelow(root, isForward, isDeep, isGlobal))
                movement = DocumentPositionMovement.Below;
            else if (MoveStartEnd(isForward, isAround))
                movement = DocumentPositionMovement.StartEnd;
            else if (MoveSibling(root, isForward))
                movement = DocumentPositionMovement.Sibling;
            else if (MoveAbove(root, isForward, isDeep, isGlobal, isInside, isAround))
                movement = DocumentPositionMovement.Above;

            if (movement != DocumentPositionMovement.None)
            {
                if (listener != null)
                    listener.NotifyMoved(movement);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="isByNode">If True then doesn't stop inside runs.</param>
        internal bool MoveNext(bool isByNode)
        {
            bool result = Move(null, true, true, true, !isByNode, true);

            if (result && !IsEndOfNonComposite)
            {
                // Move over a non-composite end.
                result = Move(null, true, true, true, !isByNode, true);
            }

            return result;
        }

        /// <summary>
        /// Gets a boolean value indicating if the current position is at the end of a non-composite node.
        /// </summary>
        /// <remarks>
        /// Empty runs have both end and start at offset 0.
        /// The method will return true for 0 offset of empty runs.
        /// </remarks>
        internal bool IsEndOfNonComposite
        {
            get { return !Node.IsComposite && IsEnd; }
        }

        /// <summary>
        /// Returns true if this instance is logically equal to the specified one.
        /// </summary>
        internal bool IsEqual(DocumentPosition position)
        {
            if (position == null)
                return false;

            return (IsSameNode(position)) && (Offset == position.Offset);
        }

        /// <summary>
        /// Returns true if this position references the same node as the specified one.
        /// </summary>
        internal bool IsSameNode(DocumentPosition position)
        {
            if (position == null)
                return false;

            return (Node == position.Node);
        }

        /// <summary>
        /// Returns a shallow copy of this <see cref="DocumentPosition"/>.
        /// </summary>
        /// <returns></returns>
        internal DocumentPosition Clone()
        {
            return (DocumentPosition)MemberwiseClone();
        }

        /// <summary>
        /// Checks and moves to either next or previous offset inside the node.
        /// </summary>
        private bool MoveInside(bool isForward, bool isInside, bool isAround)
        {
            if (Node is CompositeNode)
                return false;

            if (!isInside)
                return false;

            if (isForward)
            {

                if (Offset + (isAround ? 1 : 2) > Length)
                    return false;

                Offset++;
            }
            else
            {
                if (Offset - (isAround ? 1 : 2) < 0)
                    return false;

                Offset--;
            }

            return true;
        }

        /// <summary>
        /// Checks and moves to either first or last child of the node.
        /// </summary>
        private bool MoveBelow(Node root, bool isForward, bool isDeep, bool isGlobal)
        {
            if (!isDeep)
                return false;

            if (!(isForward && IsStart || !isForward && IsEnd))
                return false;

            // Don't descend into other story, except if it is root node.
            if (!isGlobal && Node != root && NodeUtil.IsStoryNodeType(Node))
                return false;

            if (!Node.IsComposite)
                return false;

            CompositeNode oldNode = (CompositeNode)Node;

            if (mAccessor.FirstChild(oldNode) == null)
                return false;

            Node = (isForward ? mAccessor.FirstChild(oldNode) : mAccessor.LastChild(oldNode));
            Offset = (isForward ? 0 : Length);

            return true;
        }

        /// <summary>
        /// Checks and moves to either start of end of the node.
        /// </summary>
        private bool MoveStartEnd(bool isForward, bool isAround)
        {
            if (!isAround)
                return false;

            if (isForward)
            {
                if (IsEnd)
                    return false;

                MoveNodeEnd();
            }
            else
            {
                if (IsStart)
                    return false;

                MoveNodeStart();
            }

            return true;
        }

        /// <summary>
        /// Checks and moves to either next or previous sibling node.
        /// </summary>
        private bool MoveSibling(Node root, bool isForward)
        {
            if (root == Node)
                return false;

            if (isForward && null != mAccessor.NextSibling(Node))
            {
                Node = mAccessor.NextSibling(Node);
                Offset = 0;
                return true;
            }

            if (!isForward && null != mAccessor.PreviousSibling(Node))
            {
                Node = mAccessor.PreviousSibling(Node);
                Offset = Length;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks and moves above.
        /// </summary>
        private bool MoveAbove(Node root, bool isForward, bool isDeep, bool isGlobal, bool isInside, bool isAround)
        {
            if (Node == root)
                return false;

            Node parent = mAccessor.ParentNode(Node);

            if (null == parent)
                return false;

            if (!isGlobal && root != parent && NodeUtil.IsStoryNodeType(parent))
                return false;

            Node = parent;
            Offset = (isForward ? Length : 0);

            return (isAround || Move(root, isForward, isDeep, isGlobal, isInside, false));
        }

        /// <summary>
        /// Gets length of the current node.
        /// </summary>
        private int Length
        {
            get
            {
                // This will always calculate zero length for Zlns since in structs cannot initialize mLength to -1.
                if (mLength == NotInitialized)
                {
                    // WORDSNET-15827 Set non-zero length for run with “Ruby" to avoid wrong positioning in the document.
                    if ((NodeType.Run == Node.NodeType) && !((Run)Node).RunPr.Contains(FontAttr.Ruby))
                        mLength = Node.GetTextLength();
                    else
                        mLength = 1;
                }
                return mLength;
            }
        }

        internal void ResetLength()
        {
            mLength = NotInitialized;
        }

        /// <summary>
        /// Interface for document tree access.
        /// </summary>
        internal interface IDocumentTreeAccessor
        {
            CompositeNode ParentNode(Node node);
            Node NextSibling(Node node);
            Node PreviousSibling(Node node);
            Node FirstChild(Node node);
            Node LastChild(Node node);
        }

        private class DefaultAccessor : IDocumentTreeAccessor
        {
            public CompositeNode ParentNode(Node node) { return node.ParentNode; }

            public Node FirstChild(Node node)
            {
                CompositeNode composite = node as CompositeNode;
                return composite != null? composite.FirstChild : null;
            }

            public Node LastChild(Node node)
            {
                CompositeNode composite = node as CompositeNode;
                return composite != null? composite.LastChild : null;
            }

            public Node NextSibling(Node node) { return node.NextSibling; }

            public Node PreviousSibling(Node node) { return node.PreviousSibling; }
        }

        internal static readonly DocumentPosition Void = new DocumentPosition(null);
        private readonly IDocumentTreeAccessor mAccessor;
        private const int NotInitialized = -1;
        private Node mNode;
        private int mLength;

#if DEBUG
        public override string ToString()
        {
            return IsVoid ? "Void" : Offset + " " + Node.ToString();
        }
#endif
    }
}
