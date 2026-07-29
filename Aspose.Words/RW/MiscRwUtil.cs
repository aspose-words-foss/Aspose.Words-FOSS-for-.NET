// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/06/2010 by Denis Darkin

using Aspose.Words.Fields;

namespace Aspose.Words.RW
{
    /// <summary>
    /// Miscellaneous common read/write utilities for all format import/export.
    /// </summary>
    internal static class MiscRwUtil
    {
        /// <summary>
        /// Creates a PAGE field, that displays current page number when document layout created.
        /// </summary>
        /// <remarks>Used by nrx (docx, wml), doc, rtf</remarks>
        /// <returns>A field represented as array of 5 nodes that are ready to be inserted into the document tree.</returns>
        internal static Node[] CreatePageField(DocumentBase doc, RunPr runPr)
        {
            return CreateFieldByType(doc, runPr, FieldType.FieldPage, "PAGE");
        }

        /// <summary>
        /// Creates a field by type.
        /// </summary>
        /// <returns>A field represented as array of 5 nodes that are ready to be inserted into the document tree.</returns>
        internal static Node[] CreateFieldByType(DocumentBase doc, RunPr runPr, FieldType fieldType, string data)
        {
            return new Node[] {
                new FieldStart(doc,  runPr.Clone(), fieldType),
                new Run(doc, data,  runPr.Clone()),
                new FieldSeparator(doc,  runPr.Clone(), fieldType, null),
                new Run(doc, "XXX",  runPr.Clone()),
                new FieldEnd(doc,  runPr.Clone(), fieldType, true) };
        }
    }
}
