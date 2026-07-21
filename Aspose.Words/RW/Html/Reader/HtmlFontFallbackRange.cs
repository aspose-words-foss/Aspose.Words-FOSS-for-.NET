// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/11/2014 by Victor Chebotok

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Represents a range of text that should be optionally rendered with a fallback font.
    /// </summary>
    internal class HtmlFontFallbackRange
    {
        /// <summary>
        /// Constructor. Creates a range without a fallback font.
        /// </summary>
        internal HtmlFontFallbackRange(string text)
            : this(text, null)
        {
            // Empty constructor.
        }

        /// <summary>
        /// Constructor. Creates a range with a fallback font.
        /// </summary>
        internal HtmlFontFallbackRange(string text, string fallbackFontName)
        {
            mText = text;
            mFallbackFontName = fallbackFontName;
        }

        internal string Text
        {
            get { return mText; }
        }

        internal string FallbackFontName
        {
            get { return mFallbackFontName; }
        }

        internal bool NeedsFontFallback
        {
            get { return StringUtil.HasChars(mFallbackFontName); }
        }

        private readonly string mText;
        private readonly string mFallbackFontName;
    }
}