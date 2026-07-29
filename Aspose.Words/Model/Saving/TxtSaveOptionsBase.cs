// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/07/2010 by Roman Korchagin

using System;
using System.Text;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// The base class for specifying additional options when saving a document into a text based formats.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/specify-save-options/">Specify Save Options</a> documentation article.</para>
    /// </summary>
    public abstract class TxtSaveOptionsBase : SaveOptions
    {
        /// <summary>
        /// Specifies the encoding to use when exporting in text formats. 
        /// Default value is <ms><b>Encoding.UTF8</b></ms><java>'UTF-8' Charset</java><cpp><b>Encoding.UTF8</b></cpp>.
        /// </summary>
        /// <javaName>Encoding(java.nio.charset.Charset)</javaName>
        public Encoding Encoding
        {
            get { return mEncoding; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                mEncoding = value;
            }
        }

        /// <summary>
        /// Specifies the string to use as a paragraph break when exporting in text formats.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <see cref="ControlChar.CrLf"/>.</p>
        /// </remarks>
        public string ParagraphBreak
        {
            get { return mParagraphBreak; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "ParagraphBreak");
                mParagraphBreak = value;
            }
        }

        /// <summary>
        /// <para>Allows to specify whether the page breaks should be preserved during export.</para>
        /// <para>The default value is <c>false</c>.</para>
        /// </summary>
        /// <remarks>
        /// The property affects only page breaks that are inserted explicitly into a document. 
        /// It is not related to page breaks that MS Word automatically inserts at the end of each page.
        /// </remarks>
        public bool ForcePageBreaks
        {
            get { return mForcePageBreaks; }
            set { mForcePageBreaks = value; }
        }

        /// <summary>
        /// Specifies the way headers and footers are exported to the text formats.
        /// Default value is <see cref="TxtExportHeadersFootersMode.PrimaryOnly"/>.
        /// </summary>
        public TxtExportHeadersFootersMode ExportHeadersFootersMode
        {
            get { return mExportHeadersFootersMode; }
            set { mExportHeadersFootersMode = value; }
        }

        private Encoding mEncoding = Encoding.UTF8;
        private string mParagraphBreak = ControlChar.CrLf;

        private bool mForcePageBreaks;
        private TxtExportHeadersFootersMode mExportHeadersFootersMode = TxtExportHeadersFootersMode.PrimaryOnly;
    }
}
