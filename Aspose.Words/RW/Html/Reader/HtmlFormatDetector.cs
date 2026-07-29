// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/02/2012 by Andrey Soldatov

using System;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.Charset;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Words.RW.Factories;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// <para>The class is a helper for <see cref="FileFormatDetector"/> and performs two tasks:</para>
    /// <para>
    /// 1. Confirms or rejects and assumption that a provided stream is HTML;
    /// </para>
    /// <para>
    /// 2. If HTML is confirmed, determines its encoding. This is needed because <see cref="Parser.HtmlDocument"/> cannot do it.
    /// </para>
    /// </summary>
    /// <remarks>
    /// During detection process the class tries to hold the following conditions:
    /// 1. Read as few characters of provided stream as possible. It's implemented through special
    ///    <see cref="CustomTextReader"/> which has reading limit and stops reading when the limit is reached.
    /// 
    /// 2. Confirm as Html even invalid Html as long as the handled stream cannot be of other supported format.
    /// 
    /// Encoding is determined in two stages:
    /// 1. Draft, which makes possible to read ASCII chars (ANSI, UTF-8, UTF-7, UTF-16BE, UTF-16LE, UTF-32BE, UTF-32LE);
    ///    It comes with <see cref="CustomTextReader"/>. If it's wrong, the stream will be refused and the detection process
    ///    will be called with another draft encoding.
    /// 
    /// 2. Precise, which makes possible to read national chars. It's determined by this class basing on XML/HTML headers.
    /// 
    /// The class contains the only static method accessible from outside: <see cref="Detect(CustomTextReader)"/>,
    /// use it to confirm or reject an assumption that a provided stream is HTML.
    /// </remarks>
    internal class HtmlFormatDetector
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        private HtmlFormatDetector(CustomTextReader textReader)
        {
            mTextReader = textReader;
            mSingleUseTags = new HashSetGeneric<string>();
            mSingleUseTags.Add("head");
            mSingleUseTags.Add("title");
            mSingleUseTags.Add("body");
        }

        /// <summary>
        /// Returns <see cref="FileFormatInfo"/> if Html assumption has been confirmed, or null if rejected.
        /// </summary>
        internal static FileFormatInfo Detect(CustomTextReader streamReader)
        {
            return new HtmlFormatDetector(streamReader).Detect();
        }

        private FileFormatInfo Detect()
        {
            // Sets Draft Encoding. Should be selected by a calling class.
            mEncoding = mTextReader.Encoding;
            Debug.Assert(mEncoding != null);

            // Precise Encoding can be found only when HTML structure is encoded by single-byte characters.
            // With UTF-8 HTML structure is encoded by single-byte characters too.
            if (HasMultiByteAsciiChar(mEncoding))
                mIsEncodingConfirmed = true;

            // Stop if either of the following is true:
            // 1. An assumption that the stream is Html is rejected
            // 2. An assumption that the stream is Html is confirmed *AND* encoding is confirmed
            while (!HasDecision || (IsHtmlConfirmed && !mIsEncodingConfirmed))
            {
                // Text between tags is ignored but we stop cycle if read limit has been reached
                // or syntax error has been found which breaks Html structure.
                if (!SkipText())
                    break;

                // Stop if read limit has been reached or syntax error has been found.
                if (!ProcessOneTag())
                    break;
            }

            FileFormatInfo fileFormatInfo = new FileFormatInfo();
            if (IsHtmlConfirmed)
            {
                fileFormatInfo.SetLoadFormat(LoadFormat.Html);
            }
            // WORDSNET-22633 The detector sets the encoding even if it is unsure if the document is in HTML format.
            // In some scenarios, the calling code knows better what the format of the document is and it only needs to
            // determine its encoding.
            fileFormatInfo.SetEncoding(mEncoding);
            return fileFormatInfo;
        }

        /// <summary>
        /// <para>Rewinds the handled stream until end of a text.</para>
        /// 
        /// <para>Returns whether reading of a tag was successful.</para>
        /// </summary>
        private bool SkipText()
        {
            while (mTextReader.HasChars)
            {
                char ch = mTextReader.ReadChar();
                switch (ch)
                {
                    case '<':
                        mTextReader.StepBack();
                        return true;
                    case '>':
                        return false;
                    default:
                        // SQ fix: nothing to do.
                        break;
                }
            }

            return false;
        }

        /// <summary>
        /// <para>Reads one tag or text block from the handled stream.</para>
        /// <para>Stops when reading limit set for the stream is reached. It's to avoid reading of whole big file during
        /// detection process. 'Good' tags can increase reading limit for the stream.</para>
        /// 
        /// <para>Returns whether reading of a tag was successful.</para>
        /// </summary>
        private bool ProcessOneTag()
        {
            // Uppercase/lowercase difference in tags isn't considered in detection.
            // One of purposes of the detection algorithm is to confirm Html even if it's invalid Html
            // if it cannot be treated as a file of any other supported format.
            string tagName = DetectTagName();

            switch (tagName)
            {
                case "?xml":
                case "!doctype":
                case "html":
                case "head":
                case "title":
                case "meta":
                    mTextReader.ResetReadLimit();
                    break;
                case "/head":
                case "body":
                    mTextReader.LockResetReadLimit();
                    break;
                case "script":
                    // WORDSNET-21454 Skip all content of a Script tag. Scripts require special processing, because
                    // they can contain "<" and ">" characters and their parts can be confused with HTML markup.
                    return SkipElement("</script>", ";{}");
                case "!--":
                    return SkipComment();
                case "style":
                    return SkipElement("</style>", ";{},");
                case "":
                    return false;
                default:
                    // SQ fix: nothing to do.
                    break;
            }

            return ProcessTagAttrs(tagName);
        }

        /// <summary>
        /// <para>Extracts tag name from the handled stream.</para>
        /// <para>Returns empty string if nothing has been read.</para>
        /// </summary>
        private string DetectTagName()
        {
            if (!mTextReader.HasChars)
                return "";

            char ch = mTextReader.ReadChar();
            Debug.Assert(ch == '<');

            mStringBuilder.Length = 0;

            while (mTextReader.HasChars)
            {
                ch = mTextReader.ReadChar();
                if (ch == 0)
                    return "";

                if (ch == '>')
                {
                    mTextReader.StepBack();
                    return mStringBuilder.ToString();
                }

                if (StringUtil.IsWhiteSpace(ch))
                    return mStringBuilder.ToString();

                if ((ch == '-') && (mStringBuilder.ToString() == "!-"))
                    return "!--";

                mStringBuilder.Append(StringUtil.AsciiLowerCase(ch));
            }

            return "";
        }

        /// <summary>
        /// <para>Rewinds the handled stream until end of a comment.</para>
        /// 
        /// <para>Returns whether reading of a tag was successful.</para>
        /// </summary>
        private bool SkipComment()
        {
            // Found characters of "-->" sequence.
            // Note that the dash characters of the comment start tag "<!--" are also taken into account by HTML browsers
            // when looking for the end tag. Consequently, the following sequences are all considered valid HTML comments:
            // "<!-->", "<!--->", and "<!---->". That's why the initial value of the dash character counter is 2.
            int dashesFound = 2;

            // WORDSNET-26980 We met a HTML document with a long URL in a comment but the read limit didn't allow us to skip
            // such a long comment and HTML format detection failed. Now we reset the read limit a number of times in order to
            // increase it and let the detector sucessfully skip long HTML comments. The repeat number was chosen arbitrarily
            // and can be changed as necessary.
            for (int i = 0; i < 3; i++)
            {
                while (mTextReader.HasChars)
                {
                    char ch = mTextReader.ReadChar();

                    switch (ch)
                    {
                        case '-':
                            dashesFound++;
                            break;
                        case '>':
                            if (dashesFound >= 2)
                                return true;
                            dashesFound = 0;
                            break;
                        case '\r':
                        case '\n':
                            mTextReader.ResetReadLimit();
                            dashesFound = 0;
                            break;
                        default:
                            dashesFound = 0;
                            break;
                    }
                }
                mTextReader.ResetReadLimit();
            }

            return false;
        }

        /// <summary>
        /// Skips content of a specified element.
        /// </summary>
        private bool SkipElement(string endTag, string delimiters)
        {
            LastTextMatcher endTagMatcher = new LastTextMatcher(endTag);

            while (mTextReader.HasChars)
            {
                char ch = mTextReader.ReadChar();

                if (endTagMatcher.MatchesAfterReadingChar(StringUtil.AsciiLowerCase(ch)))
                {
                    mTextReader.ResetReadLimit();
                    return true;
                }

                // Splits the whole block into parts to fit into the read limit. This is a very simplified validity check
                // for content of the element, which allows us to skip large elements without hitting the read limit.
                // We keep reading element's content while it looks valid.
                if (delimiters.IndexOf(ch) != -1)
                {
                    mTextReader.ResetReadLimit();
                }
            }

            return false;
        }

        /// <summary>
        /// <para>Reads all attributes for one tag until closing '>'.</para>
        /// <para>Returns whether reading of attributes was successful.</para>
        /// </summary>
        private bool ProcessTagAttrs(string tagName)
        {
            SortedStringListGeneric<string> tagAttrs = GetTagAttrs();

            // If (tagAttrs == null), reading of the tag contents was not successful, they are too long or ill-formed.
            if (tagAttrs == null)
                return false;

            // WORDSNET-22697 Single-use tags should be taken into account only once. A document that uses
            // any of these tags more than once is likely not a HTML document.
            if (mSingleUseTags.Contains(tagName))
            {
                mSingleUseTags.Remove(tagName);
                mHtmlConfidence++;
                return true;
            }

            switch (tagName)
            {
                case "?xml":
                    if (!mIsEncodingConfirmed)
                        ExtractXmlEncoding(tagAttrs);
                    break;
                case "!doctype":
                    CheckDoctype(tagAttrs);
                    break;
                case "html":
                    mHtmlConfidence = 3;
                    break;
                case "table":
                case "tr":
                case "td":
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                case "div":
                case "p":
                case "img":
                case "ol":
                case "ul":
                    // AS I could add other Html tags: span, div, a, b, i, u, em, strong, img, h1-h6, p, ul, ol, li.
                    // But I don't think we should open as Html any file with text looking as Html tag. It may be just a text.
                    // RK I have actually added most of the above tags to fix WORDSNET-6564 where html starts with <table>.
                    // andrnosk: I have added <ol> and <ul> to fix WORDSNET-8009.
                    mHtmlConfidence++;
                    break;
                case "meta":
                    mHtmlConfidence++;
                    if (!mIsEncodingConfirmed)
                        CheckContentType(tagAttrs);
                    break;
                default:
                    break;
            }

            return true;
        }

        #region Get Tag's Attributes

        /// <summary>
        /// <para>Parses input stream until the end of a tag or available characters.</para>
        /// 
        /// <para>Key of returned SortedStringList is attribute name, value is attribute value. Both are strings.</para>
        /// <para>Returns <c>null</c> if syntax is invalid.</para>
        /// </summary>
        private SortedStringListGeneric<string> GetTagAttrs()
        {
            SortedStringListGeneric<string> attributes = new SortedStringListGeneric<string>();

            while (true)
            {
                StringPair keyValuePair = new StringPair();

                if (DetectAttrKeyValuePair(keyValuePair))
                {
                    AddAttrAndResetReadLimitIfNeeded(attributes, keyValuePair);
                }
                else
                {
                    return DetectTagEnd() ? attributes : null;
                }
            }
        }

        /// <summary>
        /// <para>Detects tag's next attribute as Key-Value pair: [tag key1=value1 key2="value2" key3].</para>
        /// <para>Stores the result to the given StringPair. Returns <c>true</c> when successful.</para>
        /// </summary>
        private bool DetectAttrKeyValuePair(StringPair keyValuePair)
        {
            Debug.Assert(keyValuePair.Key == "");
            Debug.Assert(keyValuePair.Value == "");

            keyValuePair.Key = DetectAttrKeyOrValue();

            if (keyValuePair.Key == "")
                return false;

            while (mTextReader.HasChars)
            {
                char ch = mTextReader.ReadChar();

                if (StringUtil.IsWhiteSpace(ch))
                    continue;

                if (ch == '=')
                    break;

                mTextReader.StepBack();
                return true;
            }

            keyValuePair.Value = DetectAttrKeyOrValue();
            return true;
        }

        /// <summary>
        /// Detects one phrase which is a name of an attribute or its value.
        /// </summary>
        private string DetectAttrKeyOrValue()
        {
            while (mTextReader.HasChars)
            {
                char ch = mTextReader.ReadChar();

                if (StringUtil.IsWhiteSpace(ch))
                    continue;

                if ((ch == '\'') || (ch == '"'))
                {
                    return DetectQuotation(ch);
                }
                else
                {
                    mTextReader.StepBack();
                    return DetectWord();
                }
            }

            return "";
        }

        /// <summary>
        /// Detects one quoted string.
        /// </summary>
        private string DetectQuotation(char quoteChar)
        {
            mStringBuilder.Length = 0;

            while (mTextReader.HasChars)
            {
                char ch = mTextReader.ReadChar();
                if (ch == quoteChar)
                    break;
                mStringBuilder.Append(ch);
            }

            // FIX 11573. Allow empty string.
            return mStringBuilder.Length > 0 ? mStringBuilder.ToString() : "empty";
        }

        /// <summary>
        /// Detects one word.
        /// </summary>
        private string DetectWord()
        {
            mStringBuilder.Length = 0;

            while (mTextReader.HasChars)
            {
                char ch = mTextReader.ReadChar();

                if (StringUtil.IsWhiteSpace(ch))
                    break;

                if ((ch == '/') || (ch == '>') || (ch == '='))
                {
                    mTextReader.StepBack();
                    break;
                }

                mStringBuilder.Append(ch);
            }

            return mStringBuilder.ToString();
        }

        /// <summary>
        /// <para>Adds key-value pair to a SortedStringList of attributes.</para>
        /// <para>Clears provided key and value, preparing them to next attribute.</para>
        /// </summary>
        private void AddAttrAndResetReadLimitIfNeeded(SortedStringListGeneric<string> attrTable, StringPair keyValuePair)
        {
            if (keyValuePair.Key == "")
                return;

            string attrNameString = keyValuePair.Key.ToLowerInvariant();
            string attrValueString = keyValuePair.Value.ToLowerInvariant();
            attrTable[attrNameString] = attrValueString;

            // MS Word generates very long 'html' tag with many 'xmlns' attributes.
            // We increase read limit to find anything except them.
            if (attrNameString.StartsWith("xmlns:", StringComparison.Ordinal))
                mTextReader.ResetReadLimit();

            // WORDSNET-26054 Base64-encoded data URLs can be very long (for example, value of a 'src' attribute on an 'img'
            // tag).
            if (attrValueString.StartsWith("data:", StringComparison.Ordinal))
                mTextReader.ResetReadLimit();
        }

        #endregion

        /// <summary>
        /// Detects tag's end: ">" or "/>".
        /// </summary>
        private bool DetectTagEnd()
        {
            if (!mTextReader.HasChars)
                return false;

            char ch = mTextReader.ReadChar();

            if (ch == '>')
                return true;

            Debug.Assert(ch == '/');
            if (!mTextReader.HasChars)
                return false;

            ch = mTextReader.ReadChar();

            return ch == '>';
        }

        /// <summary>
        /// Sets encoding basing on 'encoding' attribute.
        /// </summary>
        private void ExtractXmlEncoding(SortedStringListGeneric<string> tagAttrs)
        {
            string encoding = tagAttrs.GetValueOrNull("encoding");
            if (encoding == null)
                return;

            SetEncodingSafe(encoding);
        }

        /// <summary>
        /// Checks if the document has a HTML DOCTYPE.
        /// </summary>
        private void CheckDoctype(SortedStringListGeneric<string> doctypeAttributes)
        {
            // The presence of correct HTML DOCTYPE increases the chance that the document is well-formed HTML.
            // WORDSNET-20891 If DOCTYPE of the document is malformed, it should be ignored and shouldn't increase
            // the "confidence level".
            if ((doctypeAttributes.Count > 0) && doctypeAttributes.ContainsKey("html"))
                mHtmlConfidence++;
        }

        /// <summary>
        /// Called for 'meta' tag to extract encoding.
        /// </summary>
        private void CheckContentType(SortedStringListGeneric<string> tagAttrs)
        {
            // We consider 2 cases:
            //   <meta charset="utf-8" />
            //   <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

            string httpEquiv = tagAttrs.GetValueOrNull("http-equiv");
            string charset = tagAttrs.GetValueOrNull("charset");

            // If only one 'charset' attribute exists (HTML5: charset="utf-8")
            if ((charset != null) && (httpEquiv == null))
            {
                SetEncodingSafe(charset);
                mHtmlConfidence = 3;
                return;
            }

            if ((httpEquiv == null) || !string.Equals(httpEquiv, "content-type", StringComparison.OrdinalIgnoreCase))
                return;

            string content = tagAttrs.GetValueOrNull("content");
            if (content == null)
                return;

            Match match = gContentTypetDetector.Match(content);
            if (match.Success)
            {
                string contentType = match.Groups[1].Value.ToLowerInvariant();
                if (contentType == "text/html")
                    mHtmlConfidence = 3;
            }
            // If charset joined with content type(HTML4: content="text/html; charset=utf-8")
            match = gCharsetDetector.Match(content);
            if (match.Success)
            {
                string matchedCharset = match.Groups[1].Value;
                SetEncodingSafe(matchedCharset);
            }
        }

        /// <summary>
        /// Sets encoding by its name. Never throws exceptions.
        /// </summary>
        private void SetEncodingSafe(string encodingString)
        {
            Debug.Assert(!mIsEncodingConfirmed);

            try
            {
                Encoding encoding = Encoding.GetEncoding(encodingString);
                if (!HasMultiByteAsciiChar(encoding))
                {
                    mEncoding = encoding;
                    mIsEncodingConfirmed = true;
                }
            }
            catch (ArgumentException)
            {
                // Does not throw exception. Leaves old value of the encoding.
            }
        }

        /// <summary>
        /// <para>Using charset or encoding keywords, HtmlFormatDetector can refine encoding provided from outside,
        /// but only from one where ASCII Html keywords are single-byte to another.</para>
        /// <para>For example, from ASCII to UTF-8, but not from ASCII to UTF-16.</para>
        /// </summary>
        private static bool HasMultiByteAsciiChar(Encoding encoding)
        {
#pragma warning disable SYSLIB0001 // Type or member is obsolete
            return
                encoding.Equals(Encoding.BigEndianUnicode) ||
                encoding.Equals(Encoding.Unicode) ||
                encoding.Equals(Encoding.GetEncoding(CodePage.CodePageUtf32BE)) ||
                encoding.Equals(Encoding.UTF32) ||
                encoding.Equals(Encoding.UTF7);
#pragma warning restore SYSLIB0001 // Type or member is obsolete
        }

        /// <summary>
        /// Whether Html format of the stream has been confirmed.
        /// </summary>
        private bool IsHtmlConfirmed
        {
            get { return mHtmlConfidence >= 3; }
        }

        /// <summary>
        /// Whether a decision is ready: Html or not Html.
        /// </summary>
        private bool HasDecision
        {
            get { return IsHtmlConfirmed || (mHtmlConfidence < 0); }
        }

        private class StringPair
        {
            internal string Key = "";
            internal string Value = "";
        }

        /// <summary>
        /// Matches last characters read from a stream against a text pattern using ordinal case-sensitive comparison.
        /// </summary>
        private class LastTextMatcher
        {
            internal LastTextMatcher(string textToMatch)
            {
                Debug.Assert(StringUtil.HasChars(textToMatch));
                mText = textToMatch;
                mLastChars = new char[textToMatch.Length];
            }

            internal bool MatchesAfterReadingChar(char c)
            {
                // Last read characters are stored in a fixed-size array that is used as a cycle list.
                mLastChars[mPosition] = c;
                mPosition = (mPosition + 1) % mLastChars.Length;
                ++mLength;

                if (mLength < mText.Length)
                {
                    return false;
                }

                mLength = mText.Length;
                // Compare last read characters with the text to find.
                for (int i = 0; i < mText.Length; i++)
                {
                    if (mText[i] != mLastChars[(mPosition + i) % mText.Length])
                    {
                        return false;
                    }
                }
                return true;
            }

            private readonly string mText;
            private readonly char[] mLastChars;
            private int mLength;
            private int mPosition;
        }

        /// <summary>
        /// It's to extract "test/html" from "text/html; charset=gb2312" string.
        /// </summary>
        private static readonly Regex gContentTypetDetector = new Regex("^['\"]?([^'\"; ]*)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// It's to extract "gb2312" from "text/html; charset=gb2312" string.
        /// </summary>
        private static readonly Regex gCharsetDetector = new Regex("charset=([^'\"; ]*)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly CustomTextReader mTextReader;

        /// <summary>
        /// Common StringBuilder for several methods of the class.
        /// Don't want to create another instance for each local task.
        /// </summary>
        private readonly StringBuilder mStringBuilder = new StringBuilder();

        /// <summary>
        /// <para>The larger this value the more confidence that the stream is Html.</para>
        /// <para>For a rule of the final decision, see <see cref="IsHtmlConfirmed"/>.</para>
        /// </summary>
        private int mHtmlConfidence;

        private Encoding mEncoding;
        private bool mIsEncodingConfirmed;

        /// <summary>
        /// Single-use tags. Each of these tags must be used no more than once in a well-formed HTML document.
        /// </summary>
        /// <remarks>
        /// Documents that contain more than one such tag are most likely not HTML documents.
        /// </remarks>
        private readonly HashSetGeneric<string> mSingleUseTags;
    }
}
