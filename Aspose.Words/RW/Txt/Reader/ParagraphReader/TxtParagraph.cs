// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/06/2012 by Alexey Butalov

using System.Globalization;
using System.Text;
using Aspose.Words.Loading;

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Text paragraph's structure
    /// </summary>
    internal class TxtParagraph
    {
        #region Constuctors

        internal TxtParagraph()
        {
            mStringBuilder = new StringBuilder();
            mLinesCount = 0;
        }

        #endregion

        #region Properties

        internal int LeftIndent
        {
            get { return mLeftIndent; }
            set { mLeftIndent = value; }
        }

        internal int FirstLineIndent
        {
            get { return mFirstLineIndent; }
            set { mFirstLineIndent = value; }
        }

        internal int LinesCount
        {
            get { return mLinesCount; }
        }

        /// <summary>
        /// Numbering of paragraph or null otherwise
        /// </summary>
        internal TxtNumbering Numbering
        {
            get { return mNumbering; }
            set { mNumbering = value; }
        }

        /// <summary>
        /// Returns true if FormFeed (0xC) character is terminating this line, meaning that we'll
        /// need to insert new Section after this line.
        /// </summary>
        internal bool IsNewSectionRequested
        {
            get { return mIsFormFeed; }
            set { mIsFormFeed = value; }
        }

        #endregion

        #region Methods

        internal string GetText()
        {
            return mStringBuilder.ToString();
        }

        internal void AppendText(string text)
        {
            if (mStringBuilder.Length != 0)
                mStringBuilder.Append(" ");
            mStringBuilder.Append(text);
            mLinesCount++;
        }

        /// <summary>
        /// Writes paragraph into a specified builder.
        /// </summary>
        internal void Write(DocumentBuilder builder, TxtLoadOptions options)
        {
            string text = mStringBuilder.ToString();

            int i = 0;
            // WORDSNET-25529 Check hyperlink text to store it in model as HyperlinkField.
            if (options.DetectHyperlinks)
            {
                string uri;
                int uriIndex = UriUtil.FindUri(text, out uri);
                while (uriIndex != -1)
                {
                    // Write text before hyperlink.
                    if (i < uriIndex)
                    {
                        string plainText = text.Substring(i, uriIndex - i);
                        builder.Write(plainText);
                    }

                    string schemeName;
                    UriUtil.FindUriScheme(uri, 0, out schemeName);
                    schemeName = schemeName.ToLower(CultureInfo.InvariantCulture);
                    // Consider only the following scheme names as hyperlinks.
                    if (schemeName == "http" || schemeName == "https" || schemeName == "ftp")
                    {
                        builder.PushFont();
                        builder.Font.Color = options.HyperlinksColor;
                        builder.Font.Underline = options.HyperlinksUnderline;
                        builder.InsertHyperlink(uri, uri, false);
                        builder.PopFont();
                    }
                    else
                    {
                        builder.Write(uri);
                    }

                    i = uriIndex + uri.Length;
                    uriIndex = UriUtil.FindUri(text, i, out uri);
                }
            }

            // Write the rest of remained text.
            if (i < text.Length)
            {
                string plainText = (i == 0) ? text : text.Substring(i, text.Length - i);
                builder.Write(plainText);
            }

            builder.Writeln();
        }

        #endregion

        #region Fields

        private readonly StringBuilder mStringBuilder;
        private int mLeftIndent;
        private int mFirstLineIndent;
        private int mLinesCount;
        private TxtNumbering mNumbering;
        private bool mIsFormFeed;

        #endregion
    }
}
