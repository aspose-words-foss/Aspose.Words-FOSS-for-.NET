// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2004 by Roman Korchagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Optimized for quickly finding a merge field by name.
    /// At the moment finds only MERGEFIELD fields, should probably be
    /// improved to find MACROBUTTON fields used as merge fields too?
    /// </summary>
    internal class MergeFieldFinder : DocumentVisitor
    {
        /// <summary>
        /// Helper function to find a merge field in the document. Returns the bookmark or null.
        /// </summary>
        internal static FieldMergeField FindMergeField(Node node, string fieldName)
        {
            MergeFieldFinder finder = new MergeFieldFinder();
            finder.mFieldName = fieldName;
            node.Accept(finder);
            return finder.mField;
        }

        /// <summary>
        /// Helper function to find a merge field in the document (search starts at the current node.
        /// If field not found - we're looking for it in the entire document).
        /// </summary>
        /// <returns>
        /// Returns the bookmark or null.
        /// </returns>
        internal static FieldMergeField FindMergeFieldFromNode(Node currentNode, string fieldName)
        {
            MergeFieldFinder finder = new MergeFieldFinder();
            finder.mFieldName = fieldName;
            finder.FindMergeFieldFromNode(currentNode);

            if (finder.mField != null)
                return finder.mField;

            // If currentNode is Document and the field is missing from the document, we should prevent double search.
            if (currentNode is Document)
                return null;

            // If field not found - we're looking for it from document start.
            // TODO: we should prevent double search in the end of document
            currentNode.Document.Accept(finder);

            return finder.mField;
        }

        private bool FindMergeFieldFromNode(Node currentNode)
        {
            Node child = currentNode;
            while (child != null)
            {
                if (!child.Accept(this))
                    return false;

                Node nextChild = child.NextSibling;
                child = nextChild;
            }

            if (currentNode.ParentNode == null || currentNode.ParentNode.NextSibling == null)
                return false;

            return FindMergeFieldFromNode(currentNode.ParentNode.NextSibling);
        }

        public override VisitorAction VisitFieldStart(FieldStart fieldStart)
        {
            // Find fields from nodes without delete revision only.
            if (!fieldStart.IsVisitorAcceptable(this))
                return VisitorAction.Continue;

            mIsInMergeField = (fieldStart.FieldType == FieldType.FieldMergeField);
            if (mIsInMergeField)
                mFieldStart = fieldStart;
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldSeparator(FieldSeparator fieldSeparator)
        {
            // Find fields from nodes without delete revision only.
            if (!fieldSeparator.IsVisitorAcceptable(this))
                return VisitorAction.Continue;

            if (mIsInMergeField)
                mFieldSeparator = fieldSeparator;
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {
            // Find fields from nodes without delete revision only.
            if (!fieldEnd.IsVisitorAcceptable(this))
                return VisitorAction.Continue;

            if (mIsInMergeField)
            {
                FieldMergeField field = (FieldMergeField)FieldFactory.CreateField(mFieldStart, mFieldSeparator, fieldEnd);
                if (StringUtil.EqualsIgnoreCase(field.FieldNameNoPrefix, mFieldName))
                {
                    //Set the result and terminate further search.
                    mField = field;
                    return VisitorAction.Stop;
                }
            }
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Informs the acceptor node if deleted nodes should be given to the visitor.
        /// </summary>
        /// <remarks>
        /// MergeFieldFinder does not visit deleted nodes.
        /// </remarks>
        internal override bool VisitsDeletedNodes
        {
            get { return false; }
        }

        /// <summary>
        /// Looking for this field name.
        /// </summary>
        private string mFieldName;
        private bool mIsInMergeField;
        private FieldStart mFieldStart;
        private FieldSeparator mFieldSeparator;
        /// <summary>
        /// Resulting field that was found.
        /// </summary>
        private FieldMergeField mField;
    }
}
