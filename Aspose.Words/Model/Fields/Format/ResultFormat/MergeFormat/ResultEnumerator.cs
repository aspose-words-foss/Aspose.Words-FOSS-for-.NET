// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2010 by Dmitry Vorobyev

using System.Collections.Generic;
using Aspose.JavaAttributes;
using Aspose.Words.Tables;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Enumerates a field result represented by a collection of nodes.
    /// </summary>
    internal abstract class ResultEnumerator
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        protected ResultEnumerator(IEnumerable<Node> resultNodes, Paragraph startParagraph)
        {
            mNodeEnumerator = new EnumeratorWrapperPalGeneric<Node>(resultNodes.GetEnumerator());
            CurrentParagraph = startParagraph;
        }

        /// <summary>
        /// Moves to the next token within field result.
        /// </summary>
        /// <returns>A boolean value indicating whether the next token was successfully enumerated.</returns>
        internal virtual bool MoveNext()
        {
            if (mIsEof)
                return false;

            if (IsInitialized)
            {
                if (IsNextNodeParagraph)
                {
                    LastParagraph = CurrentParagraph;
                    CurrentParagraph = (Paragraph)NextNode;
                }

                if (IsNextNodeTable)
                {
                    LastParagraph = CurrentParagraph;
                    CurrentParagraph = null;
                }

                mCurrentToken = mNextToken;
                ApplyNext();
            }

            while (MoveToNextValidPosition())
            {
                OnNextNode();

                switch (NextNode.NodeType)
                {
                    case NodeType.Run:
                    {
                        char c = NextRun.Text[NextTextPosition];
                        mNextToken = GetToken(c);

                        if (mNextToken != ResultToken.WordCharacter)
                            mCurrentWordHasDigit = false;
                        else if (char.IsDigit(c))
                            mCurrentWordHasDigit = true;

                        if (IsEndOfCurrentToken())
                        {
                            // End of current token.
                            // Remember the current character as a part of next token as text position will be moved next time.
                            OnNextChar(c);
                            return true;
                        }

                        // Here we must look at the next token in order to decide if the current token is finished.
                        mCurrentToken = mNextToken;
                        // Remember the current character as a part of the current token.
                        OnChar(c);

                        break;
                    }
                    case NodeType.Paragraph:
                    {
                        mNextToken = ResultToken.PargaraphBreak;
                        mCurrentWordHasDigit = false;
                        // There is no sense to stop at undefined token, so let's move to next one.
                        // Note that further single call of MoveNext() always returns true due to its implementation,
                        // as mIsEof is false and mNextToken (which will be applied to current token) is defined at this point.
                        return IsCurrentTokenDefined || MoveNext();
                    }
                    case NodeType.Table:
                    {
                        mNextToken = ResultToken.Table;
                        CurrentTable = (Table)NextNode;

                        return IsCurrentTokenDefined || MoveNext();
                    }
                    case NodeType.StructuredDocumentTag:
                    {
                        // Do nothing
                        break;
                    }
                    default:
                    {
                        // MoveToNextValidPosition() should only give us runs and paragraphs
                        Debug.Fail("Invalid mNextNode.NodeType value.");
                        break;
                    }
                }
            }

            // End of range reached when parsing the current token. The token may still be not empty.
            return IsCurrentTokenDefined;
        }

        /// <summary>
        /// Moves to the next valid node or to the next valid position within the run.
        /// </summary>
        /// <returns></returns>
        private bool MoveToNextValidPosition()
        {
            do
            {
                if (IsInitialized && IsNextNodeRun && (NextTextPosition < NextRun.Text.Length))
                {
                    NextTextPosition++;
                }
                else if (mNodeEnumerator.MoveNext())
                {
                    // Successfully moved to the next node.
                    NextNode = mNodeEnumerator.Current;
                    NextTextPosition = 0;

                    ProcessFieldChar(NextNode);
                }
                else
                {
                    mIsEof = true;
                    return false;
                }
            }
            while (!IsNextPositionValid());

            // Found node and position are valid.
            return true;
        }

        private ResultToken GetToken(char c)
        {
            switch (c)
            {
                case '-':
                case '\'':
                case ControlChar.NonBreakingSpaceChar:
                case ControlChar.NonBreakingHyphenChar:
                    return ResultToken.WordCharacter;
                case '.':
                case ',':
                    return mCurrentWordHasDigit
                        ? ResultToken.WordCharacter
                        : ResultToken.Separator;
                default:
                    if (char.IsLetterOrDigit(c))
                        return ResultToken.WordCharacter;
                    else if (char.IsWhiteSpace(c))
                        return ResultToken.Space;
                    else
                        return ResultToken.Separator;
            }
        }

        private bool IsEndOfCurrentToken()
        {
            switch (mCurrentToken)
            {
                case ResultToken.WordCharacter:
                    return (mNextToken != ResultToken.WordCharacter);
                case ResultToken.Space:
                    return (mNextToken != ResultToken.Space);
                case ResultToken.Separator:
                case ResultToken.PargaraphBreak:
                    return true;
                case ResultToken.Undefined:
                    return false;
                case ResultToken.Table:
                    return true;
                default:
                    Debug.Fail("Invalid mCurrentToken value.");
                    return false;
            }
        }

        /// <summary>
        /// Locks processing of nodes if we are in a field code. Fields may appear in the result, for example, if they were
        /// copied within IF's TrueText or FalseText.
        /// </summary>
        /// <param name="node"></param>
        private void ProcessFieldChar(Node node)
        {
            switch (node.NodeType)
            {
                case NodeType.FieldStart:
                    mLockCount++;
                    break;
                case NodeType.FieldSeparator:
                    mLockCount--;
                    OnFieldResultBoundary();
                    break;
                case NodeType.FieldEnd:
                    if (!((FieldEnd)node).HasSeparator)
                        mLockCount--;

                    OnFieldResultBoundary();
                    break;
                default:
                    // Do nothing.
                    break;
            }
        }

        private bool IsNextPositionValid()
        {
            // We are inside a field code.
            if (IsLocked)
                return false;

            // We are inside a table token
            if (CurrentTable != null && NextNode.IsAncestorNode(CurrentTable))
                return false;

            // We are only interested in non-empty runs and paragraphs.
            switch (NextNode.NodeType)
            {
                case NodeType.Run:
                    return (NextTextPosition < NextNode.GetText().Length);
                case NodeType.Paragraph:
                case NodeType.StructuredDocumentTag:
                    return true;
                case NodeType.Table:
                    return true;
                default:
                    return false;
            }
        }

        protected virtual void OnNextNode()
        {
            // Do nothing.
        }

        [JavaThrows(true)]
        protected virtual void OnFieldResultBoundary()
        {
            // Do nothing.
        }

        [JavaThrows(true)]
        protected virtual void OnChar(char c)
        {
            // Do nothing.
        }

        protected virtual void OnNextChar(char c)
        {
            // Do nothing.
        }

        /// <summary>
        /// Performs additional actions relative to switch from current to next token.
        /// </summary>
        [JavaThrows(true)]
        protected virtual void ApplyNext()
        {
            // Do nothing.
        }

        private bool IsLocked
        {
            get { return (mLockCount > 0); }
        }

        /// <summary>
        /// Returns value indicating whether current token is paragraph break.
        /// </summary>
        internal bool IsCurrentTokenParagraphBreak
        {
            get
            {
                switch (mCurrentToken)
                {
                    case ResultToken.PargaraphBreak:
                        return true;
                    case ResultToken.WordCharacter:
                    case ResultToken.Separator:
                    case ResultToken.Space:
                    case ResultToken.Table:
                        return false;
                    default:
                        Debug.Fail("Ivalid mCurrentToken value.");
                        return false;
                }
            }
        }

        internal bool IsCurrentTokenTable
        {
            get
            {
                switch (mCurrentToken)
                {
                    case ResultToken.Table:
                        return true;

                    case ResultToken.PargaraphBreak:
                    case ResultToken.WordCharacter:
                    case ResultToken.Separator:
                    case ResultToken.Space:
                        return false;

                    default:
                        Debug.Fail("Ivalid mCurrentToken value.");
                        return false;
                }
            }
        }

        /// <summary>
        /// Returns value indicating whether current token has valid value and
        /// is not equal to <see cref="ResultToken.Undefined"/>.
        /// </summary>
        private bool IsCurrentTokenDefined
        {
            get
            {
                return mCurrentToken != ResultToken.Undefined;
            }
        }

        protected int NextTextPosition { get; private set; }

        protected Node NextNode { get; private set; }

        protected Run NextRun
        {
            get { return (Run)NextNode; }
        }

        internal bool IsInitialized
        {
            get { return (NextNode != null); }
        }

        internal Paragraph CurrentParagraph { get; private set; }

        internal Paragraph LastParagraph { get; private set; }

        /// <summary>
        /// Returns value indicating whether the next node is a run.
        /// </summary>
        protected bool IsNextNodeRun
        {
            get { return (NextNode.NodeType == NodeType.Run); }
        }

        /// <summary>
        /// Returns value indicating whether the next node is a paragraph.
        /// </summary>
        protected bool IsNextNodeParagraph
        {
            get { return (NextNode.NodeType == NodeType.Paragraph); }
        }

        protected bool IsNextNodeTable
        {
            get { return (NextNode.NodeType == NodeType.Table); }
        }

        internal Table CurrentTable { get; private set; }

        /// <summary>
        /// Returns true when the enumerator is in suspended mode, i.e. <see cref="MoveNext"/> always returns true.
        /// </summary>
        internal abstract bool IsSuspended { get; }

        private readonly EnumeratorWrapperPalGeneric<Node> mNodeEnumerator;
        private ResultToken mCurrentToken = ResultToken.Undefined;
        private ResultToken mNextToken;
        private bool mIsEof;
        private int mLockCount;
        private bool mCurrentWordHasDigit;
    }
}
