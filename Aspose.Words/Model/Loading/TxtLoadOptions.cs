// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/03/2017 by Alexander Sevidov

using System.Drawing;

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Allows to specify additional options when loading <see cref="LoadFormat.Text"/> document into a <see cref="Document"/> object.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-load-options/">Specify Load Options</a> documentation article.</para>
    /// </summary>
    public class TxtLoadOptions : LoadOptions
    {
        /// <summary>
        /// Initializes a new instance of this class with default values.
        /// </summary>
        public TxtLoadOptions()
        {
        }

        /// <summary>
        /// Creates a new instance of this class from a specified <see cref="LoadOptions"/> object.
        /// </summary>
        internal TxtLoadOptions(LoadOptions loadOptions) : base(loadOptions)
        {
        }

        /// <summary>
        /// Gets or sets a boolean value indicating either automatic numbering detection
        /// will be performed while loading a document.
        /// The default value is <c>true</c>.
        /// </summary>
        /// <dev>
        /// IN. This solution should be redesigned some day, but for a moment we could not
        /// find more clever design for this feature.
        /// </dev>
        public bool AutoNumberingDetection
        {
            get { return mAutoNumberingDetection; }
            set { mAutoNumberingDetection = value; }
        }

        /// <summary>
        /// Allows to specify how numbered list items are recognized when document is imported from plain text format.
        /// The default value is <c>true</c>.</summary>
        /// <remarks>
        /// <para> If this option is set to <c>false</c>, lists recognition algorithm detects list paragraphs, when list numbers ends with
        /// either dot, right bracket or bullet symbols (such as "•", "*", "-" or "o").</para>
        /// <para> If this option is set to <c>true</c>, whitespaces are also used as list number delimiters:
        /// list recognition algorithm for Arabic style numbering (1., 1.1.2.) uses both whitespaces and dot (".") symbols.</para>
        /// </remarks>
        public bool DetectNumberingWithWhitespaces
        {
            get { return mDetectNumberingWithWhitespaces; }
            set { mDetectNumberingWithWhitespaces = value; }
        }

        /// <summary>
        /// Gets or sets preferred option of a trailing space handling.
        /// Default value is <see cref="TxtTrailingSpacesOptions.Trim"/>.
        /// </summary>
        public TxtTrailingSpacesOptions TrailingSpacesOptions
        {
            get { return mTrailingSpacesOptions; }
            set { mTrailingSpacesOptions = value; }
        }

        /// <summary>
        /// Gets or sets preferred option of a leading space handling.
        /// Default value is <see cref="TxtLeadingSpacesOptions.ConvertToIndent"/>.
        /// </summary>
        public TxtLeadingSpacesOptions LeadingSpacesOptions
        {
            get { return mLeadingSpacesOptions; }
            set { mLeadingSpacesOptions = value; }
        }

        /// <summary>
        /// Gets or sets a document direction.
        /// The default value is <see cref="Aspose.Words.Loading.DocumentDirection.LeftToRight"/>.
        /// </summary>
        public DocumentDirection DocumentDirection
        {
            get { return mDocumentDirection; }
            set { mDocumentDirection = value; }
        }

        /// <summary>
        /// Specifies either to detect hyperlinks in text.
        /// The default value is <c>false</c>.
        /// </summary>
        public bool DetectHyperlinks { get; set; }

        /// <summary>
        /// Specifies a <see cref="Color"/> to apply to hyperlinks detected in text.
        /// The default value is <see cref="Color.Blue"/>.
        /// <seealso cref="DetectHyperlinks"/>
        /// </summary>
        /// <dev>Internal for now, but a very possible candidate for a public API.
        /// When publish do not forget add 'seealso' to <see cref="DetectHyperlinks"/> property.
        /// Also note, the corresponding Test25529A() is added already.</dev>
        internal Color HyperlinksColor
        {
            get { return mHyperlinksColor; }
            set { mHyperlinksColor = value; }
        }

        /// <summary>
        /// Specifies a <see cref="Underline"/> to apply to hyperlinks detected in text.
        /// The default value is <see cref="Underline.Single"/>.
        /// <seealso cref="DetectHyperlinks"/>
        /// </summary>
        /// <dev>Internal for now, but a very possible candidate for a public API.
        /// When publish do not forget add 'seealso' to <see cref="DetectHyperlinks"/> property.
        /// Also note, the corresponding Test25529A() is added already.</dev>
        internal Underline HyperlinksUnderline
        {
            get { return mHyperlinksUnderline; }
            set { mHyperlinksUnderline = value; }
        }

        private bool mDetectNumberingWithWhitespaces = true;

        private TxtLeadingSpacesOptions mLeadingSpacesOptions = TxtLeadingSpacesOptions.ConvertToIndent;
        private TxtTrailingSpacesOptions mTrailingSpacesOptions = TxtTrailingSpacesOptions.Trim;

        private DocumentDirection mDocumentDirection = DocumentDirection.LeftToRight;

        private bool mAutoNumberingDetection = true;

        private Color mHyperlinksColor = Color.Blue;
        private Underline mHyperlinksUnderline = Underline.Single;
    }
}

