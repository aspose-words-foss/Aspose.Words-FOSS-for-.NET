// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/06/2018 by Alexander Sevidov

using System;
using System.Collections.Generic;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Revisions;

namespace Aspose.Words.RW.Nrx.Writer
{
    /// <summary>
    /// Base class of WML and DOCX fields writers.
    /// </summary>
    internal abstract class NrxFieldsWriter
    {
        protected NrxFieldsWriter()
        {
            mFieldCharsStack = new Stack<FieldChar>();
            mFieldStartCountStack = new Stack<int>();
        }

        /// <summary>
        /// Writes field delimiter with the specified name.
        /// Used to write field blocks for FieldStart, FieldSeparator and FieldEnd nodes.
        /// </summary>
        internal void WriteFldChar(FieldChar fieldChar)
        {
            // WORDSNET-22185 Skip field chars in a hyperlink field code.
            if (SkipThisInline && (TopFieldChar != fieldChar.GetField().Start))
                return;

            string fldCharName;

            switch (fieldChar.NodeType)
            {
                case NodeType.FieldStart:
                    {
                        fldCharName = "begin";

                        mFieldCharsStack.Push(fieldChar);
                        FieldStartCount++;
                        break;
                    }
                case NodeType.FieldSeparator:
                    {
                        fldCharName = "separate";

                        // Substitute FieldStart with FieldSeparator.
                        // We do not check here if it was actually FieldStart that was popped. It should be FieldStart in a
                        // valid document. And if the document is invalid, it is not our responsibility to validate it.
                        mFieldCharsStack.Pop();
                        mFieldCharsStack.Push(fieldChar);

                        FieldStartCount--;

                        break;
                    }
                case NodeType.FieldEnd:
                    {
                        fldCharName = "end";

                        if (mFieldCharsStack.Pop().NodeType == NodeType.FieldStart)
                            FieldStartCount--;
                        break;
                    }
                default:
                    throw new InvalidOperationException("Field delimiter expected.");
            }

            if (IsHyperlink(fieldChar) && CanWriteAsHyperlink(fieldChar))
            {
                // If shape is only child we assume link is written as shape property.
                if(!IsSingleShapeResult(fieldChar.GetField()))
                    WriteHyperlink(fieldChar);
            }
            else
            {
                WriteField(fieldChar, fldCharName);
            }
        }

        /// <summary>
        /// Writes hyperlink field start for a shape.
        /// </summary>
        internal void WriteHyperlinkStart(Shape shape)
        {
            WriteHyperlinkStart(
                shape.HyperlinkAddress,
                shape.HyperlinkSubAddress,
                shape.Target,
                shape.ScreenTip,
                null,
                true);
        }

        internal void PushFieldStartCounter()
        {
            mFieldStartCountStack.Push(0);
        }

        internal void PopFieldStartCounter()
        {
            mFieldStartCountStack.Pop();
        }

        /// <summary>
        /// Writes hyperlink field start.
        /// </summary>
        [JavaThrows(true)]
        protected abstract void WriteHyperlinkStart(
            string dest,
            string bookmark,
            string target,
            string screenTip,
            string docLocation,
            bool history);

        /// <summary>
        /// Closes hyperlink element. Symmetrical to WriteHyperlinkStart.
        /// </summary>
        internal abstract void WriteHyperlinkEnd();

        /// <summary>
        /// Writes hyperlink field start.
        /// </summary>
        [JavaThrows(true)]
        protected abstract void WriteField(FieldChar fieldChar, string fldCharName);

        private static bool IsHyperlink(FieldChar fieldChar)
        {
            return fieldChar.FieldType == FieldType.FieldHyperlink;
        }

        /// <summary>
        /// Checks if HYPERLINK field can be written as hyperlink.
        /// </summary>
        /// <remarks>
        /// See WORDSNET-10424 test for the details.
        /// </remarks>
        protected virtual bool CanWriteAsHyperlink(FieldChar fieldChar)
        {
            Debug.Assert(fieldChar.FieldType == FieldType.FieldHyperlink);

            Field field = fieldChar.GetField();
            // All field parts should have same parent.
            if ((field.Start.ParentNode != field.Separator.ParentNode) || (field.Start.ParentNode != field.End.ParentNode))
                return false;

            if (field.Start.RunPr.HasRevisions)
                return false;

            FieldCodeHyperlink fieldCode = FieldCodeHyperlink.Parse(field.GetFieldCode());

            if (UriUtil.IsSubAddressOnly(fieldCode.Address))
                return false;

            if (HasNestedField(field))
                return false;

            // Field code containing revision cannot be written as hyperlink element.
            if (HasFieldCodeRevision(field))
                return false;

            return true;
        }

        /// <summary>
        /// Checks that result of field is single shape node.
        /// </summary>
        private static bool IsSingleShapeResult(Field hyperlinkField)
        {
            Node lastChild = hyperlinkField.End.PreviousSibling;

            return (lastChild is ShapeBase) && (lastChild.PreviousSibling == hyperlinkField.Separator);
        }

        /// <summary>
        /// Writes HYPERLINK field in a "w:hyperlink" or "w:hlink" form (according to field format).
        /// </summary>
        private void WriteHyperlink(FieldChar fieldChar)
        {
            switch (fieldChar.NodeType)
            {
                case NodeType.FieldStart:
                    FieldCodeHyperlink fieldCode = FieldCodeHyperlink.Parse(fieldChar.GetField().GetFieldCode());

                    WriteHyperlinkStart(
                        fieldCode.Address,
                        fieldCode.SubAddress,
                        fieldCode.Target,
                        fieldCode.ScreenTip,
                        fieldCode.DocLocation,
                        !fieldCode.NoHistory);
                    break;

                case NodeType.FieldSeparator:
                    break;

                case NodeType.FieldEnd:
                    WriteHyperlinkEnd();
                    break;

                default:
                    throw new InvalidOperationException("Unknown field char occurred.");
            }
        }

        private static bool HasNestedField(Field field)
        {
            NodeRange fieldCodeRange = field.GetFieldCodeRange();
            foreach (Node node in fieldCodeRange)
            {
                if (node is FieldChar)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks that field has revision in field code area.
        /// </summary>
        private static bool HasFieldCodeRevision(Field field)
        {
            NodeRange fieldCodeRange = field.GetFieldCodeRange();
            foreach (Node node in fieldCodeRange)
            {
                ITrackableNode trackable = node as ITrackableNode;
                if(trackable == null)
                    continue;

                if((trackable.DeleteRevision != null) || (trackable.InsertRevision != null))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Skips field code for HYPERLINK field if it is written as hyperlink.
        /// </summary>
        internal bool SkipThisInline
        {
            get { return IsInsideFieldCode &&
                    IsHyperlink(TopFieldChar) &&
                    CanWriteAsHyperlink(TopFieldChar) &&
                    // WORDSNET-21886 Do not skip display text for nested hyperlinks.
                    TopFieldChar.NodeType == NodeType.FieldStart;
            }
        }

        /// <summary>
        /// Indicates that field code is currently written.
        /// </summary>
        internal bool IsInsideFieldCode
        {
            get { return FieldStartCount != 0; }
        }

        internal bool IsInsideField
        {
            get { return mFieldCharsStack.Count > 0; }
        }

        internal FieldChar TopFieldChar
        {
            get { return mFieldCharsStack.Top(); }
        }

        private int FieldStartCount
        {
            get
            {
                return (mFieldStartCountStack.Count > 0) ? mFieldStartCountStack.Peek() : 0;
            }
            set
            {
                // Replace current amount of field beginnings.
                if (mFieldStartCountStack.Count > 0)
                    mFieldStartCountStack.Pop();

                mFieldStartCountStack.Push(value);
            }
        }

        private readonly Stack<int> mFieldStartCountStack;
        private readonly Stack<FieldChar> mFieldCharsStack;
    }
}
