// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/08/2009 by Dmitry Vorobyev

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// A complex implementation of field code tokenizer that iterates over a field code represented by nodes.
    /// </summary>
    internal sealed class NodeRangeFieldCodeTokenizer : IFieldCodeTokenizer, IDisposable
    {
        private NodeRangeFieldCodeTokenizer(Field field)
            : this(field, false)
        {
        }

        public void Dispose()
        {
            if (mNodeEnumerator != null)
                mNodeEnumerator.Dispose();
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal NodeRangeFieldCodeTokenizer(Field field, bool moveToFirstToken)
        {
            // This should be placed here because of Java.
            mExternalObjectInteractionStrategy = new FieldInteractionStrategy(field);

            Initialize(field.Start, field.GetFieldCodeRange(), moveToFirstToken, field.IsUpdating ? field.Updater.HiddenAttributeCache : null);
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal NodeRangeFieldCodeTokenizer(FieldArgument argument, bool moveToFirstToken)
        {
            Debug.Assert(argument.Field != null);

            // This should be placed here because of Java.
            mExternalObjectInteractionStrategy = new FieldArgumentInteractionStrategy(argument);

            Initialize(argument.Field.Start, argument.Range, moveToFirstToken,
                argument.Field.IsUpdating ? argument.Field.Updater.HiddenAttributeCache : null);

            // If the argument refers to a standalone field, then its range corresponds to the field result.
            // So we need to set the tokenizer's state as if it encountered a field separator at this point.
            if (argument.IsSingleFieldResult)
                mChildFieldNestingLevel++;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal NodeRangeFieldCodeTokenizer(FieldStart fieldStart, FieldChar fieldCodeEnd, bool moveToFirstToken)
        {
            // This should be placed here because of Java.
            mExternalObjectInteractionStrategy = null;

            Initialize(fieldStart, new NodeRange(fieldStart, false, fieldCodeEnd, false), moveToFirstToken, null);
        }

        private void Initialize(FieldStart fieldStart, NodeRange range, bool moveToFirstToken, HiddenAttributeCache hiddenAttributeCache)
        {
            mHiddenAttributeCache = (hiddenAttributeCache != null)
                ? hiddenAttributeCache
                : new HiddenAttributeCache();
            mAllowHiddenText = IsHidden(fieldStart);
            mNodeEnumerator = new NodeEnumerator(range);
            if (moveToFirstToken)
                MoveToNextToken();
        }

        /// <summary>
        /// Returns field code as a plain string.
        /// </summary>
        internal static string GetCodeAsString(Field field)
        {
            StringBuilder builder = new StringBuilder();
            using (NodeRangeFieldCodeTokenizer tokenizer = new NodeRangeFieldCodeTokenizer(field))
            {
                while (tokenizer.MoveToNextToken())
                {
                    switch (tokenizer.CurrentToken)
                    {
                        case FieldCodeToken.TextChar:
                            if (!tokenizer.IsEndOfToken)
                                builder.Append(tokenizer.CurrentChar);
                            break;
                        case FieldCodeToken.Paragraph:
                        case FieldCodeToken.Section:
                        case FieldCodeToken.NonTextNode:
                        case FieldCodeToken.ChildFieldStart:
                        case FieldCodeToken.ChildFieldSeparator:
                        case FieldCodeToken.ChildFieldEnd:
                            // Skip these.
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return builder.ToString();
        }

        [JavaConvertCheckedExceptions]
        public bool MoveToNextToken()
        {
            // WORDSNET-6971 Don't exit on grandchild fields.
            if (IsBeyondChildField &&
                (CurrentToken == FieldCodeToken.ChildFieldSeparator) &&
                (CurrentNode.NodeType == NodeType.FieldEnd))
            {
                // We must return a field end token on a next demand for a field which result is forcibly
                // considered to be empty.
                CurrentToken = FieldCodeToken.ChildFieldEnd;
                return true;
            }

            // WORDSNET-13861 Tables are required to be enclosed in double quotes when used as IF arguments.
            // However, do not allow crash when encountering one.
            Node unquotedTable = (CurrentNode != null) && (CurrentNode.NodeType == NodeType.Table) && !IsInDoubleQuotes
                ? CurrentNode
                : null;

            do
            {
                if (!MoveToNextNode())
                {
                    IsEof = true;
                    return false;
                }
            }
            while (!SetTokenFromNode(unquotedTable));

            return true;
        }

        /// <summary>
        /// WORDSNET-13263 Moves to next node, but skip hidden inline nodes, if field start is not hidden.
        /// </summary>
        private bool MoveToNextNode()
        {
            while (true)
            {
                if (!mNodeEnumerator.MoveToNextNode(true, IsByNode()))
                    return false;

                if (mAllowHiddenText)
                    return true;

                if (CurrentNode is FieldChar)
                    return true;

                Inline inlineNode = CurrentNode as Inline;
                if (inlineNode == null)
                    return true;

                if (!IsHidden(inlineNode))
                    return true;
            }
        }

        /// <summary>
        /// Sets current token from the current node.
        /// Returns true if successful, false to move to next node.
        /// </summary>
        private bool SetTokenFromNode(Node unquotedTable)
        {
            // WORDSNET-20330 Don't set token from current node if it's child of unquoted table, but extract nested fields.
            bool isChildOfUnquotedTable = (unquotedTable != null) && CurrentNode.IsAncestorNode(unquotedTable);

            if (CurrentNode is FieldChar)
                return SetTokenFromFieldChar() && !isChildOfUnquotedTable;

            if (isChildOfUnquotedTable)
                return false;

            if (IsLocked)
                return false;

            return SetTokenFromNonFieldChar();
        }

        private bool SetTokenFromFieldChar()
        {
            switch (CurrentNode.NodeType)
            {
                case NodeType.FieldStart:
                {
                    mLockCount++;
                    mChildFieldNestingLevel++;
                    mFieldStarts.Push((FieldStart)CurrentNode);

                    // WORDSNET-6971 Don't exit on grandchild fields.
                    if (IsInTopLevelChildField)
                    {
                        mTopLevelChildFieldStart = CurrentNode;
                        CurrentToken = FieldCodeToken.ChildFieldStart;

                        return !IsHiddenField(CurrentNode);
                    }

                    break;
                }
                case NodeType.FieldSeparator:
                {
                    if (IsInTopLevelChildField)
                        mTopLevelChildFieldSeparator = CurrentNode;

                    FieldSeparator fieldSeparator = (FieldSeparator)CurrentNode;

                    // If a field result is invisible we must not unlock it on a separator reach.
                    if (FieldUtil.IsFieldResultInvisible(fieldSeparator))
                        return false;

                    FieldStart fieldStart = mFieldStarts.Top();
                    if (IsHiddenFieldRevision(fieldStart, fieldSeparator, null))
                        return false;

                    mLockCount--;

                    // WORDSNET-6971 Don't exit on grandchild fields.
                    if (IsInTopLevelChildField)
                    {
                        CurrentToken = FieldCodeToken.ChildFieldSeparator;

                        return !IsHiddenField(CurrentNode);
                    }

                    break;
                }
                case NodeType.FieldEnd:
                {
                    if (IsInTopLevelChildField)
                    {
                        // Process encountered top level child field if needed.
                        OnChildFieldEncountered();

                        mTopLevelChildFieldStart = null;
                        mTopLevelChildFieldSeparator = null;
                    }

                    mChildFieldNestingLevel--;

                    FieldStart fieldStart = mFieldStarts.Count > 0 ? mFieldStarts.Pop() : null;
                    FieldEnd fieldEnd = (FieldEnd)CurrentNode;

                    bool forceEmptyResult = !fieldEnd.HasSeparator ||
                                            FieldUtil.IsFieldResultInvisible(fieldEnd) ||
                                            IsHiddenFieldRevision(fieldStart, null, fieldEnd);

                    if (forceEmptyResult)
                    {
                        // If a field result is forced to be empty we must unlock it on this stage to designate its presence.
                        mLockCount--;
                    }

                    // WORDSNET-6971 Don't exit on grandchild fields.
                    if (IsBeyondChildField)
                    {
                        // If a field result is forced to be empty then corresponding field separator token was not returned.
                        // So return it here to specify that its position matches corresponding field end token's position.
                        // This prevents the field result from being included to a parent field result.
                        CurrentToken = forceEmptyResult ? FieldCodeToken.ChildFieldSeparator : FieldCodeToken.ChildFieldEnd;

                        return !IsHiddenField(CurrentNode);
                    }

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        private bool IsHiddenFieldRevision(FieldStart fieldStart, FieldSeparator fieldSeparator, FieldEnd fieldEnd)
        {
            if (fieldStart == null)
                return false;

            // EV: Don't use the FieldChar.GetField method because of performance reasons.
            if (IsHidden(fieldStart))
                return true;

            if (fieldEnd == null)
            {
                Debug.Assert(fieldSeparator != null);

                FieldBundle fieldBundle = new FieldBundle();
                fieldBundle.FillFieldBundleOneWay(fieldSeparator, true);
                fieldEnd = fieldBundle.End;

                Debug.Assert(fieldEnd != null);
            }

            if (IsHidden(fieldEnd))
                return true;

            return false;
        }

        private static bool IsHiddenField(Node node)
        {
            FieldChar fieldChar = (FieldChar)node;
            return FieldUtil.IsDead(fieldChar.FieldType);
        }

        private void OnChildFieldEncountered()
        {
            if (mExternalObjectInteractionStrategy != null)
                mExternalObjectInteractionStrategy.NotifyChildFieldEncountered(this);
        }

        private bool SetTokenFromNonFieldChar()
        {
            switch (CurrentNode.NodeType)
            {
                case NodeType.Run:
                {
                    Run run = (Run)CurrentNode;

                    // Skip empty runs.
                    if (run.Text.Length == 0)
                        return false;

                    // Set the current text.
                    CurrentToken = FieldCodeToken.TextChar;
                    break;
                }
                case NodeType.Paragraph:
                    CurrentToken = FieldCodeToken.Paragraph;
                    break;
                case NodeType.Section:
                    CurrentToken = FieldCodeToken.Section;
                    break;
                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:
                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:
                case NodeType.MoveFromRangeStart:
                case NodeType.MoveFromRangeEnd:
                case NodeType.MoveToRangeStart:
                case NodeType.MoveToRangeEnd:
                case NodeType.SmartTag:
                case NodeType.StructuredDocumentTag:
                case NodeType.FormField:
                    // These should be skipped.
                    return false;
                default:
                    CurrentToken = FieldCodeToken.NonTextNode;
                    break;
            }

            return true;
        }

        private bool IsByNode()
        {
            if (CurrentNode == null)
                return false;

            switch (CurrentNode.NodeType)
            {
                case NodeType.FieldStart:
                case NodeType.FieldSeparator:
                case NodeType.FieldEnd:
                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:
                case NodeType.CommentRangeStart:
                case NodeType.CommentRangeEnd:
                case NodeType.SmartTag:
                case NodeType.StructuredDocumentTag:
                    return true;
                default:
                    return false;
            }
        }

        public DocumentPosition GetCurrentPosition()
        {
            switch (CurrentToken)
            {
                case FieldCodeToken.ChildFieldSeparator:
                    return DocumentPosition.CreatePositionAfter(CurrentNode);
                case FieldCodeToken.ChildFieldStart:
                case FieldCodeToken.ChildFieldEnd:
                    return DocumentPosition.CreatePositionBefore(CurrentNode);
                default:
                    // Do nothing.
                    break;
            }

            if (!CurrentNode.IsComposite || (CurrentNode.NodeType == NodeType.Shape))
            {
                return CurrentDocumentPosition.Clone();
            }
            else
            {
                return CurrentDocumentPosition.IsStart
                    ? CurrentNode.GetBeforeDocumentPosition()
                    : CurrentNode.GetAfterDocumentPosition();
            }
        }

        /// <summary>
        /// Gets the current position.
        /// </summary>
        private DocumentPosition CurrentDocumentPosition
        {
            get { return mNodeEnumerator.CurrentPosition; }
        }

        public Node CurrentNode
        {
            get { return mNodeEnumerator.CurrentNode; }
        }

        private bool IsLocked
        {
            get { return mLockCount > 0; }
        }

        private bool IsInTopLevelChildField
        {
            get { return mChildFieldNestingLevel == 1; }
        }

        private bool IsBeyondChildField
        {
            get { return mChildFieldNestingLevel == 0; }
        }

        public FieldCodeToken CurrentToken { get; private set; }

        public bool IsEndOfToken
        {
            get { return CurrentDocumentPosition.IsEnd; }
        }

        public char CurrentChar
        {
            get { return ((Run)CurrentNode).Text[CurrentDocumentPosition.Offset]; }
        }

        public bool IsEof { get; private set; }

        public int CurrentLocaleId
        {
            get
            {
                Inline currentInlineNode = CurrentNode as Inline;
                // Return LocaleId attribute of the current Inline node if it is defined.
                return currentInlineNode != null
                    ? currentInlineNode.Font.LocaleId
                    : RunPr.ProcessOrUserDefaultLanguageId;
            }
        }

        public int CurrentLocaleIdFarEast
        {
            get
            {
                Inline currentInlineNode = CurrentNode as Inline;
                // Return LocaleIdFarEast attribute of the current Inline node if it is defined.
                return currentInlineNode != null
                    ? currentInlineNode.Font.LocaleIdFarEast
                    : RunPr.ProcessOrUserDefaultLanguageId;
            }
        }

        public int CurrentLocaleIdBi
        {
            get
            {
                Inline currentInlineNode = CurrentNode as Inline;
                // Return LocaleIdBi attribute of the current Inline node if it is defined.
                return currentInlineNode != null
                    ? currentInlineNode.Font.LocaleIdBi
                    : RunPr.ProcessOrUserDefaultLanguageId;
            }
        }

        public bool CurrentBidi
        {
            get
            {
                Inline currentInlineNode = CurrentNode as Inline;
                // Return Bidi attribute of the current Inline node if it is defined.
                return (currentInlineNode != null) && currentInlineNode.Font.Bidi;
            }
        }

        public string CurrentFontName
        {
            get
            {
                Inline currentInlineNode = CurrentNode as Inline;
                return currentInlineNode != null
                    ? currentInlineNode.Font.Name
                    : null;
            }
        }

        private bool IsInDoubleQuotes
        {
            get { return ((mExternalObjectInteractionStrategy != null) && mExternalObjectInteractionStrategy.IsInDoubleQuotes); }
        }

        private bool IsHidden(Inline inline)
        {
            return mHiddenAttributeCache.GetHiddenAttribute(inline);
        }

        /// <summary>
        /// When implemented, defines how the tokenizer interacts with an external object.
        /// </summary>
        private interface IExternalObjectInteractionStrategy
        {
            /// <summary>
            /// Notifies an external object about that a top level child field has been encountered while field code parsing.
            /// </summary>
            [JavaThrows(true)]
            void NotifyChildFieldEncountered(NodeRangeFieldCodeTokenizer tokenizer);
            /// <summary>
            /// Gets a value indicating whether the current token is enclosed in double quotes.
            /// </summary>
            bool IsInDoubleQuotes { get; }
        }

        /// <summary>
        /// Defines how the tokenizer interacts with a field.
        /// </summary>
        private class FieldInteractionStrategy : IExternalObjectInteractionStrategy
        {
            internal FieldInteractionStrategy(Field field)
            {
                mField = field;
            }

            void IExternalObjectInteractionStrategy.NotifyChildFieldEncountered(NodeRangeFieldCodeTokenizer tokenizer)
            {
                if (mField.IsUpdating)
                {
                    mField.UpdateContext.NotifyChildFieldEncountered(
                        (FieldStart)tokenizer.mTopLevelChildFieldStart,
                        (FieldSeparator)tokenizer.mTopLevelChildFieldSeparator,
                        (FieldEnd)tokenizer.CurrentNode);
                }
            }

            bool IExternalObjectInteractionStrategy.IsInDoubleQuotes
            {
                get { return mField.IsUpdating && mField.UpdateContext.IsInDoubleQuotes; }
            }

            private readonly Field mField;
        }

        /// <summary>
        /// Defines how the tokenizer interacts with a field argument.
        /// </summary>
        private class FieldArgumentInteractionStrategy : IExternalObjectInteractionStrategy
        {
            internal FieldArgumentInteractionStrategy(FieldArgument argument)
            {
                mArgument = argument;
            }

            void IExternalObjectInteractionStrategy.NotifyChildFieldEncountered(NodeRangeFieldCodeTokenizer tokenizer)
            {
                // Do nothing.
            }

            bool IExternalObjectInteractionStrategy.IsInDoubleQuotes
            {
                get { return mArgument.IsInDoubleQuotes; }
            }

            private readonly FieldArgument mArgument;
        }

        private bool mAllowHiddenText;
        private readonly IExternalObjectInteractionStrategy mExternalObjectInteractionStrategy;
        private NodeEnumerator mNodeEnumerator;
        private int mLockCount;
        private int mChildFieldNestingLevel;
        private Node mTopLevelChildFieldStart;
        private Node mTopLevelChildFieldSeparator;
        private HiddenAttributeCache mHiddenAttributeCache;
        private readonly Stack<FieldStart> mFieldStarts = new Stack<FieldStart>();
    }
}
