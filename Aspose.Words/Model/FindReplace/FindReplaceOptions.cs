// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2016 by Alexey Morozov

using Aspose.Words.Markup;

namespace Aspose.Words.Replacing
{
    /// <summary>
    /// Specifies options for find/replace operations.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/find-and-replace/">Find and Replace</a> documentation article.</para>
    /// </summary>
    public class FindReplaceOptions
    {
        /// <summary>
        /// Initializes a new instance of the FindReplaceOptions class with default settings.
        /// </summary>
        public FindReplaceOptions()
        {
            mApplyFont = new Font(mApplyRunPr, null);
            mApplyParagraphFormat = new ParagraphFormat(mApplyParaPr, null);
        }

        /// <summary>
        /// Initializes a new instance of the FindReplaceOptions class with the specified direction.
        /// </summary>
        /// <param name="direction">The direction of the find and replace operation.</param>
        public FindReplaceOptions(FindReplaceDirection direction)
            : this()
        {
            mDirection = direction;
        }

        /// <summary>
        /// Initializes a new instance of the FindReplaceOptions class with the specified replacing callback.
        /// </summary>
        /// <param name="replacingCallback">The callback to use for replacing found text.</param>
        public FindReplaceOptions(IReplacingCallback replacingCallback)
            : this()
        {
            ReplacingCallback = replacingCallback;
        }

        /// <summary>
        /// Initializes a new instance of the FindReplaceOptions class with the specified direction and replacing callback.
        /// </summary>
        /// <param name="direction">The direction of the find and replace operation.</param>
        /// <param name="replacingCallback">The callback to use for replacing found text.</param>
        public FindReplaceOptions(FindReplaceDirection direction, IReplacingCallback replacingCallback)
            : this()
        {
            mDirection = direction;
            ReplacingCallback = replacingCallback;
        }

        /// <summary>
        /// Text formatting applied to new content.
        /// </summary>
        public Font ApplyFont
        {
            get { return mApplyFont; }
        }

        /// <summary>
        /// Paragraph formatting applied to new content.
        /// </summary>
        public ParagraphFormat ApplyParagraphFormat
        {
            get { return mApplyParagraphFormat; }
        }

        /// <summary>
        /// Selects direction for replace. Default value is <see cref="FindReplaceDirection.Forward" />.
        /// </summary>
        public FindReplaceDirection Direction
        {
            get { return mDirection; }
            set { mDirection = value; }
        }

        /// <summary>
        /// True indicates case-sensitive comparison, false indicates case-insensitive comparison.
        /// </summary>
        public bool MatchCase
        {
            get { return mMatchCase; }
            set { mMatchCase = value; }
        }

        /// <summary>
        /// True indicates the oldValue must be a standalone word.
        /// </summary>
        public bool FindWholeWordsOnly
        {
            get { return mFindWholeWordsOnly; }
            set { mFindWholeWordsOnly = value; }
        }

        /// <summary>
        /// The user-defined method which is called before every replace occurrence.
        /// </summary>
        public IReplacingCallback ReplacingCallback
        {
            get { return mReplacingCallback; }
            set { mReplacingCallback = value; }
        }

        /// <summary>
        /// True indicates that a text search is performed sequentially from top to bottom considering the text boxes.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <dev>
        /// WORDSNET-19357 Added option.
        /// </dev>
        public bool UseLegacyOrder
        {
            get { return mUseLegacyOrder; }
            set { mUseLegacyOrder = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating either to ignore text inside delete revisions.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool IgnoreDeleted
        {
            get { return mIgnoreDeleted; }
            set { mIgnoreDeleted = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating either to ignore text inside insert revisions.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool IgnoreInserted
        {
            get { return mIgnoreInserted; }
            set { mIgnoreInserted = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating either to ignore text inside fields.
        /// The default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>This option affects whole field (all nodes between
        /// <see cref="NodeType.FieldStart"/> and <see cref="NodeType.FieldEnd"/>).</para>
        /// <para>To ignore only field codes, please use corresponding option <see cref="IgnoreFieldCodes"/>.</para>
        /// </remarks>
        public bool IgnoreFields
        {
            get { return mIgnoreFields; }
            set { mIgnoreFields = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating either to ignore text inside field codes.
        /// The default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>This option affects only field codes (it does not ignore nodes between
        /// <see cref="NodeType.FieldSeparator"/> and <see cref="NodeType.FieldEnd"/>).</para>
        /// <para>To ignore whole field, please use corresponding option <see cref="IgnoreFields"/>.</para>
        /// </remarks>
        public bool IgnoreFieldCodes
        {
            get { return mIgnoreFieldCodes; }
            set { mIgnoreFieldCodes = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating either to ignore footnotes.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool IgnoreFootnotes { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating whether to recognize and use substitutions within replacement patterns.
        /// The default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// For the details on substitution elements please refer to:
        /// https://docs.microsoft.com/en-us/dotnet/standard/base-types/substitutions-in-regular-expressions.
        /// </remarks>
        public bool UseSubstitutions
        {
            get { return mUseSubstitutions; }
            set { mUseSubstitutions = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating that old find/replace algorithm is used.
        /// </summary>
        /// <remarks>
        /// Use this flag if you need exactly the same behavior as before advanced find/replace feature was introduced.
        /// Note that old algorithm does not support advanced features such as replace with breaks, apply formatting and so on.
        /// </remarks>
        public bool LegacyMode
        {
            get { return mLegacyMode; }
            set { mLegacyMode = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating either to ignore content of <see cref="StructuredDocumentTag"/>.
        /// The default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When this option is set to <c>true</c>, the content of <see cref="StructuredDocumentTag"/>
        /// will be treated as a simple text.
        /// </para>
        /// <para>
        /// Otherwise, <see cref="StructuredDocumentTag"/> will be processed as standalone Story
        /// and replacing pattern will be searched separately for each <see cref="StructuredDocumentTag"/>,
        /// so that if pattern crosses a <see cref="StructuredDocumentTag"/>, then replacement will not
        /// be performed for such pattern.
        /// </para>
        /// </remarks>
        public bool IgnoreStructuredDocumentTags { get; set; }

        /// <summary>
        /// <para>Gets or sets a boolean value indicating either it is allowed to replace paragraph break
        /// when there is no next sibling paragraph.</para>
        /// <para>The default value is <c>false</c>.</para>
        /// </summary>
        /// <remarks>
        /// This option allows to replace paragraph break when there is no next sibling paragraph to which all child
        /// nodes can be moved, by finding any (not necessarily sibling) next paragraph after the paragraph being replaced.
        /// </remarks>
        public bool SmartParagraphBreakReplacement { get; set; }

        /// <summary>
        /// <para>Gets or sets a boolean value indicating either to ignore shapes within a text.</para>
        /// <para>The default value is <c>false</c>.</para>
        /// </summary>
        public bool IgnoreShapes { get; set; }

        /// <summary>
        /// Gets or sets a boolean value indicating either to ignore text inside OfficeMath/>.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool IgnoreOfficeMath
        {
            get { return mIgnoreOfficeMath; }
            set { mIgnoreOfficeMath = value; }
        }

        internal RunPr ApplyRunPr
        {
            get { return mApplyRunPr; }
        }

        internal ParaPr ApplyParaPr
        {
            get { return mApplyParaPr; }
        }

        private FindReplaceDirection mDirection = FindReplaceDirection.Forward;

        private readonly Font mApplyFont;
        private readonly ParagraphFormat mApplyParagraphFormat;

        private readonly RunPr mApplyRunPr = new RunPr();
        private readonly ParaPr mApplyParaPr = new ParaPr();

        private bool mMatchCase;
        private bool mFindWholeWordsOnly;
        private IReplacingCallback mReplacingCallback;
        private bool mUseLegacyOrder;

        private bool mIgnoreDeleted;
        private bool mIgnoreInserted;
        private bool mIgnoreFields;
        private bool mIgnoreFieldCodes;

        private bool mIgnoreOfficeMath = true;

        private bool mUseSubstitutions;
        private bool mLegacyMode;
    }
}
