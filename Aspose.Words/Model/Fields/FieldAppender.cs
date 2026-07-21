// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2015 by Vadim Polienko

using Aspose.Words.Revisions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Provides functions for inserting/appending fields either from DocumentBuilder or Paragraph.
    /// </summary>
    internal static class FieldAppender
    {
        /// <summary>Insert Field.</summary>
        /// <remarks>This method is called from DocumentBuilder or Paragraph.</remarks>
        /// <param name="fieldType">The field type to insert.</param>
        /// <param name="updateField">Whether to update the field immediately or not.</param>
        /// <param name="runPr">Attributes for inserted field.</param>
        /// <param name="parent">A composite node to insert the field into.</param>
        /// <param name="refNode">A reference node inside the parent node.</param>
        /// <param name="isAfter">Whether to insert the field after or before the reference node.</param>
        /// <returns>A <see cref="Field"/> object that represents the inserted field.</returns>
        internal static Field InsertField(FieldType fieldType, bool updateField, RunPr runPr, CompositeNode parent,
            Node refNode, bool isAfter)
        {
            string fieldCode = FieldUtil.FetchFieldCode(fieldType);

            runPr = runPr.Clone();
            RevisionUtil.RemoveRevisions(runPr);

            Field field = InsertField(string.Format(" {0} ", fieldCode), null, runPr, parent, refNode, isAfter);

            if (updateField)
                field.Update();

            return field;
        }

        /// <summary>Insert Field.</summary>
        /// <remarks>This method is called from DocumentBuilder or Paragraph.</remarks>
        /// <param name="fieldCode">The field code to insert (without curly braces).</param>
        /// <param name="runPr">Attributes for inserted field.</param>
        /// <param name="parent">A composite node to insert the field into.</param>
        /// <param name="refNode">A reference node inside the parent node.</param>
        /// <param name="isAfter">Whether to insert the field after or before the reference node.</param>
        /// <returns>A <see cref="Field"/> object that represents the inserted field.</returns>
        internal static Field InsertField(string fieldCode, RunPr runPr, CompositeNode parent, Node refNode, bool isAfter)
        {
            Field field = InsertField(fieldCode, null, runPr, parent, refNode, isAfter);
            field.Update();
            return field;
        }

        /// <summary>Insert Field.</summary>
        /// <remarks>This method is called from DocumentBuilder or Paragraph.</remarks>
        /// <param name="fieldCode">The field code to insert (without curly braces).</param>
        /// <param name="fieldValue">The field value to insert. Pass null for fields that do not have a value.</param>
        /// <param name="runPr">Attributes for inserted field.</param>
        /// <param name="parent">A composite node to insert the field into.</param>
        /// <param name="initialRefNode">A reference node inside the parent node.</param>
        /// <param name="isAfter">Whether to insert the field after or before the reference node.</param>
        /// <returns>A <see cref="Field"/> object that represents the inserted field.</returns>
        internal static Field InsertField(string fieldCode, string fieldValue, RunPr runPr, CompositeNode parent,
            Node initialRefNode, bool isAfter)
        {
            FieldSeparator separator;
            FieldEnd end;
            Node refNode = initialRefNode;

            FieldType fieldType = FieldUtil.GetFieldType(fieldCode);

            FieldStart start = InsertFieldStart(fieldType, runPr, parent, refNode, isAfter);
            refNode = start;

            Run fieldCodeNode = InsertFieldCode(fieldCode, runPr, parent, refNode, true);
            refNode = fieldCodeNode;

            if (FieldUtil.GetSeparatorPresence(fieldType) == FieldSeparatorPresence.Always)
            {
                separator = InsertFieldSeparator(fieldType, runPr, parent, refNode, true);
                refNode = separator;

                if (fieldValue != null)
                {
                    Run fieldValueNode = InsertRun(fieldValue, runPr, parent, refNode, true);
                    refNode = fieldValueNode;
                }

                end = InsertFieldEnd(fieldType, true, runPr, parent, refNode, true);
            }
            else
            {
                separator = null;
                end = InsertFieldEnd(fieldType, false, runPr, parent, refNode, true);
            }

            return FieldFactory.CreateField(start, separator, end);
        }

        /// <summary>
        /// Insert FieldStart (this method is called from DocumentBuilder or internally).
        /// </summary>
        /// <param name="fieldType">The field type to insert.</param>
        /// <param name="runPr">Attributes for inserted field.</param>
        /// <param name="parent">A composite node to insert the field into.</param>
        /// <param name="refNode">A reference node inside the parent node.</param>
        /// <param name="isAfter">Whether to insert the field after or before the reference node.</param>
        /// <returns>A <see cref="FieldStart"/> object that represents the inserted field start.</returns>
        internal static FieldStart InsertFieldStart(FieldType fieldType, RunPr runPr, CompositeNode parent,
            Node refNode, bool isAfter)
        {
            FieldStart fieldStart = new FieldStart(parent.Document, runPr.Clone(), fieldType);
            InsertNode(fieldStart, parent, refNode, isAfter);
            return fieldStart;
        }

        /// <summary>
        /// Insert FieldCode (this method is called from DocumentBuilder or internally).
        /// </summary>
        /// <param name="fieldCode">The field code to insert (without curly braces).</param>
        /// <param name="runPr">Attributes for inserted field.</param>
        /// <param name="parent">A composite node to insert the field into.</param>
        /// <param name="refNode">A reference node inside the parent node.</param>
        /// <param name="isAfter">Whether to insert the field after or before the reference node.</param>
        /// <returns>A <see cref="Run"/> object that represents the inserted code.</returns>
        internal static Run InsertFieldCode(string fieldCode, RunPr runPr, CompositeNode parent, Node refNode,
            bool isAfter)
        {
            Run fieldCodeNode = InsertRun(fieldCode, runPr, parent, refNode, isAfter);
            return fieldCodeNode;
        }

        /// <summary>
        /// Insert FieldSeparator (this method is called from DocumentBuilder or internally).
        /// </summary>
        /// <param name="fieldType">The field type to insert.</param>
        /// <param name="runPr">Attributes for inserted field.</param>
        /// <param name="parent">A composite node to insert the field into.</param>
        /// <param name="refNode">A reference node inside the parent node.</param>
        /// <param name="isAfter">Whether to insert the field after or before the reference node.</param>
        /// <returns>A <see cref="FieldSeparator"/> object that represents the inserted field separator.</returns>
        internal static FieldSeparator InsertFieldSeparator(FieldType fieldType, RunPr runPr, CompositeNode parent,
            Node refNode, bool isAfter)
        {
            FieldSeparator fieldSeparator = new FieldSeparator(parent.Document, runPr.Clone(), fieldType);
            InsertNode(fieldSeparator, parent, refNode, isAfter);
            return fieldSeparator;
        }

        /// <summary>
        /// Insert FieldEnd (this method is called from DocumentBuilder or internally).
        /// </summary>
        /// <param name="fieldType">The field type to insert.</param>
        /// <param name="hasSeparator">Whether this field has a field separator.</param>
        /// <param name="runPr">Attributes for inserted field.</param>
        /// <param name="parent">A composite node to insert the field into.</param>
        /// <param name="refNode">A reference node inside the parent node.</param>
        /// <param name="isAfter">Whether to insert the field after or before the reference node.</param>
        /// <returns>A <see cref="FieldEnd"/> object that represents the inserted field end.</returns>
        internal static FieldEnd InsertFieldEnd(FieldType fieldType, bool hasSeparator, RunPr runPr,
            CompositeNode parent, Node refNode, bool isAfter)
        {
            FieldEnd fieldEnd = new FieldEnd(parent.Document, runPr.Clone(), fieldType, hasSeparator);
            InsertNode(fieldEnd, parent, refNode, isAfter);
            return fieldEnd;
        }

        /// <summary>
        /// Insert text (either from DocumentBuilder or Paragraph).
        /// </summary>
        /// <param name="text">Text to insert.</param>
        /// <param name="runPr"></param>
        /// <param name="parent"></param>
        /// <param name="refNode"></param>
        /// <param name="isAfter"></param>
        private static Run InsertRun(string text, RunPr runPr, CompositeNode parent, Node refNode, bool isAfter)
        {
            Run newRunNode = new Run(parent.Document, text, runPr.Clone());
            InsertNode(newRunNode, parent, refNode, isAfter);
            return newRunNode;
        }

        /// <summary>
        /// Insert node (either from DocumentBuilder or Paragraph).
        /// </summary>
        /// <param name="node">Node to insert.</param>
        /// <param name="parent"></param>
        /// <param name="refNode"></param>
        /// <param name="isAfter"></param>
        private static void InsertNode(Node node, CompositeNode parent, Node refNode, bool isAfter)
        {
            if (isAfter)
            {
                if (refNode == null)
                    parent.AppendChild(node);
                else
                    parent.InsertAfter(node, refNode);
            }
            else
            {
                parent.InsertBefore(node, refNode ?? parent.LastChild);
            }
        }
    }
}
