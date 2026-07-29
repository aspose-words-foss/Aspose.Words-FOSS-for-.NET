// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/07/2010 by Roman Korchagin

using System;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Can be used to specify additional options when saving a document into the <see cref="Words.SaveFormat.Text"/> format.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-save-options/">Specify Save Options</a> documentation article.</para>
    /// </summary>
    public class TxtSaveOptions : TxtSaveOptionsBase
    {
        /// <summary>
        /// Specifies the format in which the document will be saved if this save options object is used.
        /// Can only be <see cref="Words.SaveFormat.Text"/>.
        /// </summary>
        public override SaveFormat SaveFormat
        {
            get { return SaveFormat.Text; }
            set
            {
                if (value != SaveFormat.Text)
                    throw new ArgumentException("An invalid SaveFormat for this options type was chosen.");
            }
        }

        /// <summary>
        /// <para>Specifies whether the program should simplify list labels in case of
        /// complex label formatting not being adequately represented by plain text.</para>
        /// <para>If set to <c>true</c>, numbered list labels are written in simple numeric format
        /// and itemized list labels as simple ASCII characters. The default value is <c>false</c>.</para>
        /// </summary>
        public bool SimplifyListLabels { get; set; }

        /// <summary>
        /// <para>Specifies whether to add bi-directional marks before each BiDi run when exporting in plain text format.</para>
        /// <para>The default value is <c>false</c>.</para>
        /// </summary>
        public bool AddBidiMarks { get; set; }

        /// <summary>
        /// Gets a <see cref="TxtListIndentation"/> object that specifies how many and which character to use for indentation of list levels.
        /// By default, it is zero count of character '\0', that means no indentation.
        /// </summary>
        public TxtListIndentation ListIndentation
        {
            get { return mListIndentation; }
        }

        /// <summary>
        /// Specifies whether the program should attempt to preserve layout of tables when saving in the plain text format.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool PreserveTableLayout { get; set; }

        /// <summary>
        /// Gets or sets an integer value that specifies the maximum number of characters per one line.
        /// The default value is 0, that means no limit.
        /// </summary>
        public int MaxCharactersPerLine { get; set; }

        /// <summary>
        /// Specifies how OfficeMath will be written to the output file.
        /// Default value is <see cref="TxtOfficeMathExportMode.Text"/>.
        /// </summary>
        public TxtOfficeMathExportMode OfficeMathExportMode { get; set; }

        private readonly TxtListIndentation mListIndentation = new TxtListIndentation();
    }
}
