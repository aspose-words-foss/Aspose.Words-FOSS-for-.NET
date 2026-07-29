// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/07/2016 by Anton Savko

using Aspose.Words.Fonts;

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Allows to specify additional options when loading HTML document into a <see cref="Document"/> object.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-load-options/">Specify Load Options</a> documentation article.</para>
    /// </summary>
    public class HtmlLoadOptions : LoadOptions
    {
        /// <summary>
        /// Initializes a new instance of this class with default values.
        /// </summary>
        public HtmlLoadOptions()
        {
            // Empty constructor.
        }

        /// <summary>
        /// A shortcut to initialize a new instance of this class with the specified password to load an encrypted document.
        /// </summary>
        /// <param name="password">The password to open an encrypted document. Can be <c>null</c> or empty string.</param>
        public HtmlLoadOptions(string password)
            : base(password)
        {
            // Empty constructor.
        }

        /// <summary>
        /// A shortcut to initialize a new instance of this class with properties set to the specified values.
        /// </summary>
        /// <param name="loadFormat">The format of the document to be loaded.</param>
        /// <param name="password">The password to open an encrypted document. Can be <c>null</c> or empty string.</param>
        /// <param name="baseUri">The string that will be used to resolve relative URIs to absolute. Can be <c>null</c> or empty string.</param>
        public HtmlLoadOptions(LoadFormat loadFormat, string password, string baseUri)
            : base(loadFormat, password, baseUri)
        {
            // Empty constructor.
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        internal HtmlLoadOptions(HtmlLoadOptions htmlLoadOptions)
            : base(htmlLoadOptions)
        {
            mApplyFormattingAsMsWord = htmlLoadOptions.mApplyFormattingAsMsWord;
            mConvertSvgToEmf = htmlLoadOptions.mConvertSvgToEmf;
            mIgnoreNoscriptElements = htmlLoadOptions.mIgnoreNoscriptElements;
            mPreferredControlType = htmlLoadOptions.mPreferredControlType;
            mSupportVml = htmlLoadOptions.mSupportVml;
            mWebRequestTimeout = htmlLoadOptions.mWebRequestTimeout;
            BlockImportMode = htmlLoadOptions.BlockImportMode;
            SupportFontFaceRules = htmlLoadOptions.SupportFontFaceRules;
        }

        /// <summary>
        /// Initializes a new instance of this class with <see cref="LoadOptions"/> instance.
        /// </summary>
        internal HtmlLoadOptions(LoadOptions loadOptions)
            : base(loadOptions)
        {
            // Empty constructor.
        }

        internal override LoadOptions Clone()
        {
            return new HtmlLoadOptions(this);
        }

        internal HtmlReaderSettings GetHtmlReaderSettings()
        {
            HtmlReaderSettings settings = new HtmlReaderSettings();
            settings.ApplyFormattingAsMsWord = ApplyFormattingAsMsWord;
            settings.ConvertSvgToEmf = ConvertSvgToEmf;
            settings.IgnoreNoscriptElements = IgnoreNoscriptElements;
            settings.MswVersion = MswVersion;
            settings.PreferredControlType = PreferredControlType;
            settings.SupportVml = SupportVml;
            settings.UseHtmlBlocks = (BlockImportMode == BlockImportMode.Preserve) || ApplyFormattingAsMsWord;
            settings.SupportFontFaceRules = SupportFontFaceRules;
            return settings;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to support VML images.
        /// </summary>
        public bool SupportVml
        {
            get { return mSupportVml; }
            set { mSupportVml = value; }
        }

        /// <summary>
        /// The number of milliseconds to wait before the web request times out. The default value is 100000 milliseconds
        /// (100 seconds).
        /// </summary>
        /// <remarks>
        /// The number of milliseconds that Aspose.Words waits for a response, when loading external resources (images, style
        /// sheets) linked in HTML and MHTML documents.
        /// </remarks>
        public int WebRequestTimeout
        {
            get { return mWebRequestTimeout; }
            set { mWebRequestTimeout = value; }
        }

        /// <summary>
        /// Gets or sets preferred type of document nodes that will represent imported &lt;input&gt; and &lt;select&gt; elements.
        /// Default value is <see cref="HtmlControlType.FormField"/>.
        /// </summary>
        /// <remarks>
        /// Please note that setting this property does not guarantee that all imported controls will be of the specified type.
        /// If an HTML control is not representable with document nodes of the preferred type, Aspose.Words will use
        /// a compatible <see cref="HtmlControlType"/> for that control.
        /// </remarks>
        public HtmlControlType PreferredControlType
        {
            get { return mPreferredControlType; }
            set { mPreferredControlType = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore &lt;noscript&gt; HTML elements.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// Like MS Word, Aspose.Words does not support scripts and by default loads content of &lt;noscript&gt; elements
        /// into the resulting document. In most browsers, however, scripts are supported and content from &lt;noscript&gt;
        /// is not visible. Setting this property to <c>true</c> forces Aspose.Words to ignore all &lt;noscript&gt; elements
        /// and helps to produce documents that look closer to what is seen in browsers.
        /// </remarks>
        public bool IgnoreNoscriptElements
        {
            get { return mIgnoreNoscriptElements; }
            set { mIgnoreNoscriptElements = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to convert loaded SVG images to the EMF format.
        /// Default value is <c>false</c> and, if possible, loaded SVG images are stored as is without conversion.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Newer versions of MS Word support SVG images natively. If the MS Word version specified in load options supports
        /// SVG, Aspose.Words will store SVG images as is without conversion. If SVG is not supported, loaded SVG images will be
        /// converted to the EMF format.
        /// </para>
        /// <para>
        /// If, however, this option is set to <c>true</c>, Aspose.Words will convert loaded SVG images to EMF even if SVG
        /// images are supported by the specified version of MS Word.
        /// </para>
        /// </remarks>
        public bool ConvertSvgToEmf
        {
            get { return mConvertSvgToEmf; }
            set { mConvertSvgToEmf = value; }
        }

        /// <summary>
        /// Gets or sets a value that specifies how properties of block-level elements are imported.
        /// Default value is <see cref="BlockImportMode.Merge"/>.
        /// </summary>
        public BlockImportMode BlockImportMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to support @font-face rules and whether to load declared fonts.
        /// Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this option is enabled, fonts declared in @font-face rules are loaded and embedded into the resulting document's
        /// font definitions (see <see cref="DocumentBase.FontInfos"/>). This makes the loaded fonts available for rendering but
        /// doesn't automatically enable embedding of the fonts upon saving. In order to save the document with loaded fonts,
        /// the <see cref="FontInfoCollection.EmbedTrueTypeFonts"/> property of the <see cref="DocumentBase.FontInfos"/>
        /// collection should be set to <c>true</c>.
        /// </para>
        /// <para>
        /// Supported font formats are TTF, EOT, and WOFF.
        /// </para>
        /// <para>
        /// @font-face rules are not supported when loading SVG images.
        /// </para>
        /// </remarks>
        public bool SupportFontFaceRules { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the HTML reader should stick to MS Word behavior when converting CSS
        /// to model styles and formatting. Default value is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// If this property is <c>true</c>, the HTML reader mimics MS Word's behavior when converting CSS declarations
        /// to model styles and formatting. If this property is <c>false</c>, the reader is allowed to apply certain
        /// enhancements that make documents imported from HTML look closer to what is seen in browsers.
        /// </remarks>
        internal bool ApplyFormattingAsMsWord
        {
            get { return mApplyFormattingAsMsWord; }
            set { mApplyFormattingAsMsWord = value; }
        }

        private bool mSupportVml;
        private int mWebRequestTimeout = 100000;
        private bool mApplyFormattingAsMsWord;
        private HtmlControlType mPreferredControlType;
        private bool mIgnoreNoscriptElements;
        private bool mConvertSvgToEmf;
    }
}
