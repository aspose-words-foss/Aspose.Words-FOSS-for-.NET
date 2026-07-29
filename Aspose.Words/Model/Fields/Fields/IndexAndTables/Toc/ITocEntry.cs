// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2010 by Dmitry Vorobyev

using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// When implemented, represents an object that may participate in building a TOC (a paragraph with a special style
    /// or outline level or a TC field).
    /// </summary>
    internal interface ITocEntry
    {
        /// <summary>
        /// Inserts a hidden bookmark used to reference the entry from the TOC and mark the contents.
        /// </summary>
        /// <returns>Bookmark's node range.</returns>
        [JavaThrows(true)]
        NodeRange InsertBookmark(string bookmarkName);

        /// <summary>
        /// Gets the level of the entry.
        /// </summary>
        int Level { get; }

        /// <summary>
        /// Gets whether the page number should be omitted for this entry.
        /// </summary>
        [JavaThrows(true)]
        bool OmitPageNumber { get; }

        /// <summary>
        /// Gets <see cref="Paragraph"/> associated with the entry.
        /// </summary>
        /// <returns></returns>
        [JavaThrows(true)]
        Paragraph Paragraph { get; }

        /// <summary>
        /// Returns title of the entry.
        /// </summary>
        /// <returns></returns>
        [JavaThrows(true)]
        string GetDocumentOutlineTitle();

        /// <summary>
        /// Returns <see cref="NodeRange"/> associated with the entry label.
        /// </summary>
        [JavaThrows(true)]
        NodeRange GetLabelRange();

        /// <summary>
        /// Gets whether the entry is in field code.
        /// </summary>
        [JavaThrows(true)]
        bool IsInFieldCode { get; }

        /// <summary>
        /// Gets whether the entry supports bookmark which can be referenced by
        /// <see cref="FieldHyperlink"/>, <see cref="FieldPageRef"/> and <see cref="FieldSeq"/> fields.
        /// </summary>
        bool HasBookmark { get; }

        /// <summary>
        /// Gets whether the entry is populated from linked character style.
        /// </summary>
        bool IsLinkedStyleTocEntry { get; }

        /// <summary>
        /// Returns static sequence value of the entry to be used instead of the <see cref="FieldSeq"/> field.
        /// </summary>
        [JavaThrows(true)]
        int GetSequenceValue(string sequenceIdentifier);

        /// <summary>
        /// Returns static page number of the entry to be used instead of the <see cref="FieldPageRef"/> field.
        /// </summary>
        [JavaThrows(true)]
        int GetPageNumber();
    }
}
