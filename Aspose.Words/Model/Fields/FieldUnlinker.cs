// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2016 by Edward Voronov

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Unlinks single or multimple fields.
    /// </summary>
    internal class FieldUnlinker
    {
        private FieldUnlinker(Document document)
        {
            mDocument = document;
            mFields = new List<Field>();
        }

        /// <summary>
        /// Unlinks the given field.
        /// </summary>
        internal static void UnlinkField(Field field)
        {
            Document document = field.FetchDocument();
            FieldUnlinker unlinker = new FieldUnlinker(document);
            unlinker.AddField(field);
            unlinker.UnlinkFields();
        }

        /// <summary>
        /// Unlinks fields contained in the given node.
        /// </summary>
        internal static void UnlinkFields(Node node)
        {
            Document document = node.FetchDocument();
            FieldUnlinker unlinker = new FieldUnlinker(document);

            foreach (Field field in FieldExtractor.ExtractToCollection(node))
                unlinker.AddField(field);

            unlinker.UnlinkFields();
        }

        /// <summary>
        /// Unlinks fields contained in the given range.
        /// </summary>
        internal static void UnlinkFields(NodeRange range)
        {
            Document document = range.Document.FetchDocument();
            FieldUnlinker unlinker = new FieldUnlinker(document);

            foreach (Field field in FieldExtractor.ExtractToCollection(range))
                unlinker.AddField(field);

            unlinker.UnlinkFields();
        }

        private void UnlinkFields()
        {
            foreach (Field field in mFields)
            {
                UpdateListLabelIfNeeded(field);
                field.UnlinkCore();
            }
        }

        private void UpdateListLabelIfNeeded(Field field)
        {
            if (mListLabelsUpdated)
                return;

            if (!FieldNumUtil.IsFieldNum(field.Type))
                return;

            mDocument.UpdateListLabels();
            mListLabelsUpdated = true;
        }

        private void AddField(Field field)
        {
            mFields.Add(field);
        }

        private readonly Document mDocument;
        private readonly List<Field> mFields;
        private bool mListLabelsUpdated;
    }
}
