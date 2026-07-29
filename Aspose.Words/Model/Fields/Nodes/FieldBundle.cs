// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/10/2006 by Roman Korchagin

using System;
using System.Text;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// A lightweight structure to deal with all three nodes of a field at once.
    /// </summary>
    internal struct FieldBundle
    {
        internal FieldBundle(FieldStart start, FieldSeparator separator, FieldEnd end)
        {
            Start = start;
            Separator = separator;
            End = end;
        }

        /// <summary>
        /// Returns text between field start and field separator (or field end if there is no separator).
        /// </summary>
        internal string GetFieldCode()
        {
            return GetFieldCode(true);
        }

        /// <summary>
        /// Returns text between field start and field separator (or field end if there is no separator).
        /// </summary>
        internal string GetFieldCode(bool includeChildFieldCodes)
        {
            NodeTextCollectorOptions options = new NodeTextCollectorOptions();

            bool hidden = Start.Font.Hidden;
            options.AllowHiddenText = hidden;
            options.AllowDeletedText = hidden;
            options.IsFieldResultMode = !includeChildFieldCodes;
            return NodeTextCollector.GetText(Start, false, FieldCodeEnd, false, options);
        }

        /// <summary>
        /// Gets field code, just enough to determine the field type.
        /// Will not return complete field code for fields that don't have same grand parent.
        /// </summary>
        private string GetFieldCodeLimited(bool allowHiddenText)
        {
            StringBuilder builder = new StringBuilder();
            Node curNode = Start.NextSibling;
            Node fieldCodeEnd = FieldCodeEnd;

            int fieldDepth = 0;
            while ((curNode != null) && (curNode != fieldCodeEnd))
            {
                bool breakLoop = false;
                switch (curNode.NodeType)
                {
                    case NodeType.Run:

                        if (fieldDepth != 0)
                            break;

                        if (!allowHiddenText && ((Run)curNode).IsHiddenOrDeleted)
                            break;

                        builder.Append(curNode.GetText());

                        break;

                    case NodeType.FieldStart:

                        if (FieldUtil.IsDead(((FieldStart)curNode).FieldType))
                        {
                            fieldDepth++;
                        }
                        // WORDSNET-5572 put whole content of INCLUDEPICTURE field into SourceFullname (in field code), include nested fields
                        else if (Start.FieldType != FieldType.FieldIncludePicture)
                        {
                            // WORDSNET-5997 We should prevent the reading runs of nested field because it might lead to wrong field type determination.
                            // For example, outer field for such fields as {{MERGEFIELD somename}} gets type MergeField instead of FieldNone.
                            // This will lead to errors during executing MailMerge.Execute method.
                            breakLoop = fieldDepth == 0;
                        }

                        break;

                    case NodeType.FieldEnd:

                        if (FieldUtil.IsDead(((FieldEnd)curNode).FieldType))
                            fieldDepth--;

                        break;

                    default:
                        // Do nothing.
                        break;
                }

                if (breakLoop)
                    break;

                curNode = curNode.NextSibling;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns a field for the field bundle.
        /// </summary>
        internal Field GetField()
        {
            return FieldFactory.CreateField(this);
        }

        internal FieldType ParseFieldType()
        {
            string fieldCode = Start.ParentNode != FieldCodeEnd.ParentNode
                                   ? GetFieldCode()
                                   : GetFieldCodeLimited(Start.IsHiddenOrDeleted);
            return FieldUtil.GetFieldType(fieldCode);
        }

        internal void DetermineFieldType()
        {
            FieldType = ParseFieldType();
        }

        internal void RemoveFieldNodes()
        {
            if (Start != null)
                Start.Remove();

            if (Separator != null)
                Separator.Remove();

            if (End != null)
                End.Remove();
        }

        /// <summary>
        /// Returns either separator or field end node.
        /// </summary>
        internal FieldChar FieldCodeEnd
        {
            get { return Separator != null ? (FieldChar)Separator : End; }
        }

        internal FieldType FieldType
        {
            get { return Start.FieldType; }
            set
            {
                Start.FieldType = value;
                End.FieldType = value;
                if (Separator != null)
                    Separator.FieldType = value;
            }
        }

        [CppWeakPtr]
        internal FieldStart Start { [CppConstMethod] get; set; }

        internal FieldSeparator Separator { get; set; }

        internal bool HasSeparator
        {
            get { return Separator != null; }
        }

        internal FieldEnd End { [CppConstMethod] get; set; }

        internal bool IsLocked
        {
            get { return Start.IsLocked; }
            set
            {
                Start.IsLocked = value;
                End.IsLocked = value;
                if (Separator != null)
                    Separator.IsLocked = value;
            }
        }

        internal bool IsDirty
        {
            get { return Start.IsDirty; }
            set
            {
                Start.IsDirty = value;
                End.IsDirty = value;
                if (Separator != null)
                    Separator.IsDirty = value;
            }
        }

        /// <summary>
        /// Updates flags IsDirty, IsLocked in FieldStart, FieldSeparator and FieldEnd if any of them has these flags set.
        /// </summary>
        internal void UpdateDirtyLocked()
        {
            IsDirty = Start.IsDirty || End.IsDirty || ((Separator != null) && Separator.IsDirty);
            IsLocked = Start.IsLocked || End.IsLocked || ((Separator != null) && Separator.IsLocked);
        }

        /// <summary>
        /// Gets the whole field bundle by the given field char.
        /// </summary>
        internal static FieldBundle GetFieldBundle(FieldChar fieldChar)
        {
            FieldBundle fieldBundle = GetFieldBundleNoSeparatorCheck(fieldChar);

            // We must find the corresponding field separator if the field end specifies that it has one and
            // we must not find it otherwise. If it is not satisfied then the model is invalid.
            if (fieldBundle.End.HasSeparator != (fieldBundle.Separator != null))
                throw CreateInvalidModelException();

            return fieldBundle;
        }

        /// <summary>
        /// Gets the whole field bundle by the given field char.
        /// </summary>
        /// <remarks>
        /// Doesn't throw Invalid Model Exception for missed separator.
        /// </remarks>
        internal static FieldBundle GetFieldBundleNoSeparatorCheck(FieldChar fieldChar)
        {
            FieldBundle fieldBundle = new FieldBundle();

            switch (fieldChar.NodeType)
            {
                case NodeType.FieldStart:
                    fieldBundle.FillFieldBundleOneWay(fieldChar, true);
                    break;
                case NodeType.FieldSeparator:
                    fieldBundle.FillFieldBundleOneWay(fieldChar, true);
                    fieldBundle.FillFieldBundleOneWay(fieldChar, false);
                    break;
                case NodeType.FieldEnd:
                    fieldBundle.FillFieldBundleOneWay(fieldChar, false);
                    break;
                default:
                    // A field char can not have a node type other than one of the listed above.
                    // If it has then the model is invalid.
                    throw CreateInvalidModelException();
            }

            return fieldBundle;
        }

        /// <summary>
        /// Fills the specified field bundle by field chars starting from the specified field char and
        /// moving one way (forward or backward) through the document.
        /// </summary>
        internal void FillFieldBundleOneWay(FieldChar fieldChar, bool isForward)
        {
            bool isFieldSeparator = fieldChar.NodeType == NodeType.FieldSeparator;
            FieldNestingLevel nestingLevel = new FieldNestingLevel(isForward, isFieldSeparator);
            if (isFieldSeparator)
            {
                // We need to set the bundle's field separator to null in case when we are moving from a field separator
                // to be able to determine that we encounter not more than one corresponding field separator while moving.
                Separator = null;
            }

            DocumentPosition currentPosition = isForward
                ? DocumentPosition.CreatePositionAfter(fieldChar)
                : DocumentPosition.CreatePositionBefore(fieldChar);

            while (true) // Return inside.
            {
                Node currentNode = currentPosition.Node;
                switch (currentNode.NodeType)
                {
                    case NodeType.FieldStart:
                        if (nestingLevel.AcceptFieldStart())
                        {
                            Start = (FieldStart)currentNode;
                            ValidateFieldChar(Start, fieldChar);

                            // If we are moving backward and the corresponding field start is found, it's time to return.
                            if (!isForward)
                                return;
                        }
                        break;
                    case NodeType.FieldSeparator:
                        if (nestingLevel.IsTop)
                        {
                            // If we encountered more than one corresponding field separator while moving then the model is invalid.
                            if (Separator != null)
                                throw CreateInvalidModelException();

                            Separator = (FieldSeparator)currentNode;
                            ValidateFieldChar(Separator, fieldChar);
                        }
                        break;
                    case NodeType.FieldEnd:
                        if (nestingLevel.AcceptFieldEnd())
                        {
                            End = (FieldEnd)currentNode;
                            ValidateFieldChar(End, fieldChar);

                            // If we are moving forward and the corresponding field end is found, it's time to return.
                            if (isForward)
                                return;
                        }
                        break;
                    default:
                        break;
                }

                // Move to the next node.
                do
                {
                    // If the movement is failed then the model is invalid, i.e. a field start must have
                    // the corresponding field end and vice versa.
                    if (!currentPosition.Move(null, isForward, true, true, false, false))
                        throw CreateInvalidModelException();
                }
                while (currentPosition.Node == currentNode);
            }
        }

        /// <summary>
        /// Validates the given field char. Throws if field chars' field types are different.
        /// </summary>
        /// <param name="fieldChar">A field char to validate.</param>
        /// <param name="referenceFieldChar">A valid field char to compare with.</param>
        private static void ValidateFieldChar(FieldChar fieldChar, FieldChar referenceFieldChar)
        {
            if (fieldChar.FieldType != referenceFieldChar.FieldType)
                throw CreateInvalidModelException();
        }

        /// <summary>
        /// Creates an exception object used to denote that the document model is invalid.
        /// </summary>
        private static InvalidOperationException CreateInvalidModelException()
        {
            return new InvalidOperationException("Invalid document model. Operation can not be completed.");
        }

        /// <summary>
        /// Represents a relative field nesting level.
        /// </summary>
        private class FieldNestingLevel
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            /// <param name="isForward">
            /// A value indicating whether a movement through the document is forward or backward.
            /// </param>
            /// <param name="startFromTop">
            /// A value indicating whether the counting should be started from the top level instead of zero.
            /// </param>
            internal FieldNestingLevel(bool isForward, bool startFromTop)
            {
                mIsForward = isForward;
                mValue = startFromTop ? TopValue : 0;
            }

            /// <summary>
            /// Changes the nesting level accordingly to a field start encountering and the document movement direction.
            /// </summary>
            /// <returns>
            /// A value indicating that the operation has led the nesting level to become zero or to stop being zero.
            /// </returns>
            internal bool AcceptFieldStart()
            {
                int boundaryValue = mIsForward ? 1 : 0;
                return ++mValue == boundaryValue;
            }

            /// <summary>
            /// Changes the nesting level accordingly to a field end encountering and the document movement direction.
            /// </summary>
            /// <returns>
            /// A value indicating that the operation has led the nesting level to become zero or to stop being zero.
            /// </returns>
            internal bool AcceptFieldEnd()
            {
                int boundaryValue = mIsForward ? 0 : -1;
                return --mValue == boundaryValue;
            }

            /// <summary>
            /// Gets a value indicating whether the nesting level is top according to the document movement direction.
            /// </summary>
            internal bool IsTop
            {
                get { return mValue == TopValue; }
            }

            /// <summary>
            /// Gets a value of the top nesting level according to the document movement direction.
            /// </summary>
            private int TopValue
            {
                get { return mIsForward ? 1 : -1; }
            }

            private readonly bool mIsForward;
            private int mValue;
        }
    }
}
