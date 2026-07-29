// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/06/2013 by Ivan Lyagin

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements <see cref="INodeCopierListener"/> in the way to append fake results of FWR-fields before their starts
    /// while copying nodes to the result of a reference field.
    /// </summary>
    internal class FieldFakeResultAppender : INodeCopierListener
    {
        internal FieldFakeResultAppender(Field refField)
            : this(refField, null)
        {
        }

        internal FieldFakeResultAppender(Field refField, IFieldFakeResultNodeModifier nodeModifier)
        {
            mRefField = refField;
            mNodeModifier = nodeModifier;
        }

        void INodeCloningListener.NotifyNodeCloned(Node source, Node clone)
        {
            // Process field starts only.
            if (source.NodeType != NodeType.FieldStart)
                return;

            // Do not process removed fields.
            FieldStart sourceFieldStart = (FieldStart)source;
            if (sourceFieldStart.IsDeleteRevision)
                return;

            switch (sourceFieldStart.FieldType)
            {
                case FieldType.FieldFormDropDown:
                {
                    // Do not process FORMDROPDOWN fields inside TOCs' results.
                    if (mRefField.Type != FieldType.FieldTOC)
                        CollectClone(clone);
                    break;
                }
                default:
                {
                    if (FieldNumUtil.IsFieldNum(sourceFieldStart.FieldType))
                        CollectCloneAndSource(sourceFieldStart, (FieldStart)clone);
                    break;
                }
            }
        }

        /// <summary>
        /// Remembers the given cloned node for a further processing.
        /// </summary>
        private void CollectClone(Node clone)
        {
            if (!HasClones)
            {
                // Create on the first demand.
                mClones = new List<Node>();
            }

            mClones.Add(clone);
        }

        /// <summary>
        /// Remembers the given cloned node and its source node for a further processing.
        /// </summary>
        private void CollectCloneAndSource(FieldStart source, FieldStart clone)
        {
            FieldStart rootSource = null;
            if (!HasClonesToSourcesMap)
            {
                // Create on the first demand.
                mClonesToSourcesMap = new Dictionary<FieldStart, FieldStart>();
            }
            else
            {
                // We may deal with a clone of a tracked clone provided by INodeModifier implementation for example.
                // So let's check if this is the case and use a root source (i.e. a source of the source) if it is.
                rootSource = mClonesToSourcesMap.GetValueOrNull(source);
            }

            mClonesToSourcesMap[clone] = (rootSource != null) ? rootSource : source;
        }

        void INodeCopierListener.NotifyNodeRangeCopied(NodeRange sourceRange, NodeRange insertedRange)
        {
            AppendFakeResultsForClones();
            AppendFakeResultsForClonesBySources();
        }

        /// <summary>
        /// Appends fake results for collected cloned nodes.
        /// </summary>
        private void AppendFakeResultsForClones()
        {
            if (!HasClones)
                return;

            foreach (Node node in mClones)
            {
                FieldStart clone = (FieldStart)node;
                // Ensure that the tracked clone has been added to a document as its addition could be rejected
                // by INodeModifier implementation. Skip the clone in this case.
                if (IsNodeDetachedFromDocument(clone))
                    continue;

                // At the moment only FORMDROPDOWN fields are processed.
                // Convert the check below to a switch statement if you are about to support other field types here.
                Debug.Assert(clone.FieldType == FieldType.FieldFormDropDown);
                AppendFieldFormDropDownFakeResult(clone);
            }
        }

        /// <summary>
        /// Appends the fake result for a single FORMDROPDOWN field with the given field start.
        /// </summary>
        private static void AppendFieldFormDropDownFakeResult(FieldStart clone)
        {
            // Get the form field. Do nothing, if it is missing.
            FormField formField = clone.FormField;
            if (formField == null)
                return;

            DocumentBuilder builder = new DocumentBuilder(clone.FetchDocument());
            builder.MoveTo(clone);

            // Write the form field's result. It seems like MS Word uses LTR embedding here all the time.
            FieldTextHelper.WriteTextBidiAware(builder, formField.Result, false);
        }

        /// <summary>
        /// Appends fake results for collected cloned nodes based on their source nodes.
        /// </summary>
        private void AppendFakeResultsForClonesBySources()
        {
            if (!HasClonesToSourcesMap)
                return;

            foreach (KeyValuePair<FieldStart, FieldStart> entry in mClonesToSourcesMap)
            {
                // Ensure that the tracked clone has been added to a document as its addition could be rejected
                // by INodeModifier implementation. Skip the clone in this case.
                FieldStart clone = entry.Key;
                if (IsNodeDetachedFromDocument(clone))
                    continue;

                FieldStart source = entry.Value;

                // At the moment only LISTNUM, AUTONUM, AUTONUMOUT and AUTONUMLGL fields are supported.
                // Convert the check below to a switch statement if you are about to support other field types here.
                Debug.Assert(FieldNumUtil.IsFieldNum(clone.FieldType));
                AppendFieldNumFakeResult(source, clone);
            }
        }

        /// <summary>
        /// Appends the fake result for a single LISTNUM, AUTONUM, AUTONUMOUT or AUTONUMLGL field with the given field start.
        /// </summary>
        private void AppendFieldNumFakeResult(FieldStart source, FieldStart clone)
        {
            // Ensure that the source field start and the cloned one belong to the same document.
            Debug.Assert(source.FetchDocument() == clone.FetchDocument());

            IList<Run> runs = FieldNumUtil.GetFakeResultList(source, mRefField);
            foreach (Run run in runs)
            {
                if (mNodeModifier != null)
                    mNodeModifier.Modify(run);

                clone.InsertPrevious(run);
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified node belongs to a document.
        /// </summary>
        private static bool IsNodeDetachedFromDocument(Node node)
        {
            return node.IsRemoved;
        }

        private bool HasClones
        {
            get { return (mClones != null); }
        }

        private bool HasClonesToSourcesMap
        {
            get { return (mClonesToSourcesMap != null); }
        }

        private readonly Field mRefField;
        private readonly IFieldFakeResultNodeModifier mNodeModifier;
        private Dictionary<FieldStart, FieldStart> mClonesToSourcesMap;
        private List<Node> mClones;
    }
}
