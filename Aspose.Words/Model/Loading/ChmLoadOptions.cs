// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/09/2021 by Artem Shabarshin

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Allows to specify additional options when loading CHM document into a <see cref="Document"/> object.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-load-options/">Specify Load Options</a> documentation article.</para>
    /// </summary>
    public class ChmLoadOptions : LoadOptions
    {
        /// <summary>
        /// Initializes a new instance of this class with default values.
        /// </summary>
        public ChmLoadOptions()
        {
            // Empty constructor.
        }

        /// <summary>
        /// Initializes a new instance of this class with <see cref="LoadOptions"/> instance.
        /// </summary>
        internal ChmLoadOptions(LoadOptions loadOptions)
            : base(loadOptions)
        {
            // Empty constructor.
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        internal ChmLoadOptions(ChmLoadOptions chmLoadOptions)
            : base(chmLoadOptions)
        {
            OriginalFileName = chmLoadOptions.OriginalFileName;
        }

        internal override LoadOptions Clone()
        {
            return new ChmLoadOptions(this);
        }

        internal HtmlReaderSettings GetHtmlReaderSettings()
        {
            HtmlReaderSettings settings = new HtmlReaderSettings();
            settings.OriginalChmFileName = OriginalFileName;

            // We keep unreferenced bookmarks created for HTML elements with "id" attributes, because these elements
            // may be referenced from other HTML files in this CHM document that will be loaded later.
            settings.KeepUnreferencedIdBookmarks = true;

            return settings;
        }

        /// <summary>
        /// The name of the CHM file.
        /// Default value is <c>null</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// CHM documents may contain links that reference the same document by file name. Aspose.Words supports such links
        /// and normally uses <see cref="Document.OriginalFileName"/> to check whether the file referenced by a link
        /// is the file that is being loaded. If a document is loaded from a stream, its original file name should be specified
        /// explicitly via this property, since it cannot be determined automatically.
        /// </para>
        /// <para>
        /// If a CHM document is loaded from a file and a non-null value for this property is specified, the value will take
        /// priority over the actual name of the file stored in <see cref="Document.OriginalFileName"/>.
        /// </para>
        /// </remarks>
        public string OriginalFileName { get; set; }
    }
}
