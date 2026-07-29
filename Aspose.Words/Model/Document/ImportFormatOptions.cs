// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/12/2014 by Ilya Navrotskiy

namespace Aspose.Words
{
    /// <summary>
    /// Allows to specify various import options to format output.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-load-options/">Specify Load Options</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// The most of these options can be found under the advanced options menu in Word, 'cut, copy and paste' section.
    /// But some of them, such as <see cref="KeepSourceNumbering"/> have no analogue in Word.
    /// </dev>
    public class ImportFormatOptions
    {
        /// <summary>
        /// Gets or sets a boolean value that specifies how styles will be imported
        /// when they have equal names in source and destination documents.
        /// The default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>When this option is <b>enabled</b>, the source style will be expanded into a direct attributes inside a
        /// destination document, if <see cref="ImportFormatMode.KeepSourceFormatting"/> importing mode is used.</para>
        /// <para>When this option is <b>disabled</b>, the source style will be expanded only if it is numbered. Existing
        /// destination attributes will not be overridden, including lists.</para>
        /// </remarks>
        public bool SmartStyleBehavior { get; set; }

        /// <summary>
        /// Gets or sets a boolean value that specifies how the numbering will be imported when it clashes in source and
        /// destination documents.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool KeepSourceNumbering { get; set; }

        /// <summary>
        /// Gets or sets a boolean value that specifies that source formatting of textboxes content ignored
        /// if <see cref="ImportFormatMode.KeepSourceFormatting"/> mode is used.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool IgnoreTextBoxes
        {
            get { return mIgnoreTextBoxes; }
            set { mIgnoreTextBoxes = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value that specifies that source formatting of headers/footers content ignored
        /// if <see cref="ImportFormatMode.KeepSourceFormatting"/> mode is used.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool IgnoreHeaderFooter
        {
            get { return mIgnoreHeaderFooter; }
            set { mIgnoreHeaderFooter = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value that specifies whether pasted lists will be merged with surrounding lists.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool MergePastedLists { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating either to copy conflicting styles
        /// in <see cref="ImportFormatMode.KeepSourceFormatting"/> mode.
        /// The default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>By default, if a matching style already exists in a destination document, the source style formatting
        /// is expanded into direct node attributes and the style of this node is reset to a default.</para>
        /// <para> When this option is set to <c>true</c>, the source style will be forcibly copied
        /// into destination document with unique name and applied to the imported node.</para>
        /// <para> Note, in this case it is not guaranteed that formatting of the imported node in destination document
        /// will be preserved. </para>
        /// </remarks>
        public bool ForceCopyStyles { get; set; }

        /// <summary>
        /// Gets or sets a boolean value that specifies whether to adjust sentence and word spacing automatically.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool AdjustSentenceAndWordSpacing { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating whether to change a first imported section type
        /// to the <see cref="SectionStart.NewPage"/> forcibly when call
        /// <see cref="Document.AppendDocument(Aspose.Words.Document,Aspose.Words.ImportFormatMode, ImportFormatOptions)"/>.
        /// <para>The default value is <c>true</c>.</para>
        /// </summary>
        /// <remarks>
        /// Please note that this option is only relevant for the
        /// <see cref="Document.AppendDocument(Aspose.Words.Document,Aspose.Words.ImportFormatMode, ImportFormatOptions)"/>
        /// method and has no effect on other import-related methods.
        ///
        /// </remarks>
        /// <dev>
        /// We probably need to introduce some special options class for AppendDocument().
        /// Something, like AppendDocumentOptions class. But for a while we decided to place this option here.
        /// </dev>
        public bool AppendDocumentWithNewPage
        {
            get { return mAppendDocumentWithNewPage; }
            set { mAppendDocumentWithNewPage = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating that an existing list with the same definition should be used when importing
        /// paragraphs with numbering.
        /// </summary>
        internal bool UseExistingLists { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating that we are in <see cref="DocumentMerger"/> mode.
        /// </summary>
        internal bool IsMerger { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating either to insert a source document as inline into a destination document.
        /// The default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>This option has effect only for
        /// <see cref="DocumentBuilder.InsertDocument(Aspose.Words.Document,Aspose.Words.ImportFormatMode)"/> method.</para>
        /// <para>In this inline mode the content of the paragraph of the destination document,
        /// before which the source document is inserted, will be moved into the last
        /// paragraph of the inserted source document. Actually, this means that
        /// paragraph break of the last inserted paragraph is removed.</para>
        /// <para>Note, if the last node of the source document is not a paragraph, then this option is not working.</para>
        /// </remarks>
        internal bool InlineMode { get; set; }

        /// <summary>
        /// Gets or sets flag indicating that bookmarks with duplicate names being imported is renamed to be unique
        /// </summary>
        internal bool RenameDuplicateBookmarks { get; set; } = true;

        private bool mIgnoreTextBoxes = true;
        private bool mIgnoreHeaderFooter = true;
        private bool mAppendDocumentWithNewPage = true;
    }
}
