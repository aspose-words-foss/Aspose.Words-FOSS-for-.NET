// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/06/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Caches the XE field code for reusing. See corresponding <see cref="FieldXE"/> properties for any details.
    /// </summary>
    internal class FieldCodeXE
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal FieldCodeXE(FieldXE field)
        {
            // WORDSNET-22419 NullReferenceException occurs during UpdateFields.
            TextRange = field.TextRange;
            Text = field.Text;
            IsBold = field.IsBold;
            IsItalic = field.IsItalic;
            EntryType = field.EntryType;
            EntryTypeCore = FieldIndexAndTablesUtil.GetIndexEntryType(EntryType);
            PageRangeBookmarkName = field.PageRangeBookmarkName;
            PageNumberReplacementRange = field.PageNumberReplacementRange;
            PageNumberReplacement = field.PageNumberReplacement;
            Yomi = field.Yomi;
        }

        /// <summary>
        /// Gets a node range of the text argument.
        /// </summary>
        internal NodeRange TextRange { get; }

        /// <summary>
        /// Gets the text of the entry.
        /// </summary>
        internal string Text { get; }

        /// <summary>
        /// Gets whether to apply bold formatting to the entry's page number.
        /// </summary>
        internal bool IsBold { get; }

        /// <summary>
        /// Gets whether to apply italic formatting to the entry's page number.
        /// </summary>
        internal bool IsItalic { get; }

        /// <summary>
        /// Gets an index entry type.
        /// </summary>
        internal string EntryType { get; }

        /// <summary>
        /// Gets an internal index entry type.
        /// </summary>
        internal int EntryTypeCore { get; }

        /// <summary>
        /// Gets the name of the bookmark that marks a range of pages that is inserted as the entry's page number.
        /// </summary>
        internal string PageRangeBookmarkName { get; }

        /// <summary>
        /// Gets a value indicating whether a page range bookmark name is provided through the field's code.
        /// </summary>
        internal bool HasPageRangeBookmarkName
        {
            get { return PageRangeBookmarkName != null; }
        }

        /// <summary>
        /// Gets a node range of the text argument to be used in place of a page number.
        /// </summary>
        internal NodeRange PageNumberReplacementRange { get; }

        /// <summary>
        /// Gets a text used in place of a page number.
        /// </summary>
        internal string PageNumberReplacement { get; }

        /// <summary>
        /// Gets a value indicating whether a text used in place of a page number is provided through the field's code.
        /// </summary>
        internal bool HasPageNumberReplacement
        {
            get { return PageNumberReplacement != null; }
        }

        /// <summary>
        /// Gets the yomi (first phonetic character for sorting indexes) for the index entry
        /// </summary>
        internal string Yomi { get; }
    }
}
