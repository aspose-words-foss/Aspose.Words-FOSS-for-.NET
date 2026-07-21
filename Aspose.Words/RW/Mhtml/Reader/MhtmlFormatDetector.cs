// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/03/2012 by Andrey Soldatov

using System;
using System.Text;
using Aspose.Common;
using Aspose.Words.RW.Factories;

namespace Aspose.Words.RW.Mhtml.Reader
{
    /// <summary>
    /// The class is helper for <see cref="FileFormatDetector"/> and performs the following task:
    /// Confirms or rejects and assumption that a provided stream is Mhtml;
    /// </summary>
    /// <remarks>
    /// During detection process the class tries to hold the following conditions:
    /// 1. Read as few characters of provided stream as possible. It's implemented through special
    ///    <see cref="CustomTextReader"/> which has reading limit and stops reading when the limit is reached.
    /// 
    /// 2. Confirm as Mhtml even invalid Mhtml as long as the handled stream cannot be of other supported format.
    /// 
    /// The class contains the only static method accessible from outside: <see cref="Detect(CustomTextReader)"/>,
    /// use it to confirm or reject an assumption that a provided stream is Mhtml.
    /// </remarks>
    internal static class MhtmlFormatDetector
    {
        /// <summary>
        /// Returns <see cref="FileFormatInfo"/> if Mhtml assumption has been confirmed, or null if rejected.
        /// </summary>
        internal static FileFormatInfo Detect(CustomTextReader textReader)
        {
            textReader.SetEncodingLatin();

            //Internet Message Format Specification (RFC 2045, http://tools.ietf.org/html/rfc2045) states:
            // MIME-Version     
            //      MIME-Version header field is required at the top level of a message.  
            //      It is not required for each body part of a multipart entity.  
            //      It is required for the embedded headers of a body of type "message/rfc822" 
            //      or "message/partial" if and only if the embedded message is itself 
            //      claimed to be MIME-conformant.
            // Content-Type
            //      Default RFC 822 messages without a MIME Content-Type header are taken by this protocol
            //      to be plain text in the US-ASCII character set.
            // So we should not detect document as Mhtml, if there is neither Content-Type nor MIME-Version header 
            bool headerFound = false;
            while (!headerFound)
            {
                // WORDSNET-13191 Aspose.Words reads MHTML as a text file.
                // Only the empty line causes the parsing interrupt.
                // Lines with an incorrect format are ignored.
                // This behavior is more similar to the behavior of browsers. 
                string headerName;
                switch (ReadHeaderName(textReader, out headerName))
                {
                    case ReadHeaderResult.ContainsIllegalChars:
                        continue;
                    case ReadHeaderResult.EmptyLine:
                        return null;
                    case ReadHeaderResult.Success:
                        // Do nothing. It's ok.
                        break;
                    default:
                        Debug.Assert(false, "Unknown result type.");
                        break;
                }

                string headerValue = ReadHeaderValue(textReader);
                if (headerValue == null)
                    return null;

                if (StringUtil.EqualsIgnoreCase(headerName, "content-type"))
                {
                    // WORDSNET-27609 We detect a document as MHTML only if its content type is supported by our MHTML reader.
                    if (!IsSupportedContentType(headerValue))
                    {
                        // If the content type is not supported, this document cannot be loaded as MHTML.
                        return null;
                    }
                    headerFound = true;
                }
                else if (StringUtil.EqualsIgnoreCase(headerName, "mime-version"))
                {
                    headerFound = true;
                }

                textReader.ResetReadLimit();
            }

            FileFormatInfo fileFormatInfo = new FileFormatInfo();
            fileFormatInfo.SetLoadFormat(LoadFormat.Mhtml);
            fileFormatInfo.SetEncoding(Encoding.ASCII);
            return fileFormatInfo;
        }

        /// <summary>
        /// For header "Header-name: Header value[CR][LF]" reads "Header-name:" and
        /// returns header name and <see cref="ReadHeaderResult.Success"/>.
        /// For empty header name returns <see cref="ReadHeaderResult.EmptyLine"/>
        /// and <see cref="ReadHeaderResult.ContainsIllegalChars"/> if it contains illegal chars (including CR LF).
        /// </summary>
        private static ReadHeaderResult ReadHeaderName(CustomTextReader textReader, out string header)
        {
            bool waitForColon = false;
            StringBuilder headerName = new StringBuilder();
            header = string.Empty;
            ReadHeaderResult result = ReadHeaderResult.Success;
            while (textReader.HasChars)
            {
                char ch = textReader.ReadChar();

                switch (ch)
                {
                    case ' ':
                    case '\t':
                    {
                        waitForColon = true;
                        break;
                    }
                    case '\r':
                    {
                        if (textReader.HasChars)
                        {
                            ch = textReader.ReadChar();
                            if (ch != '\n')
                                textReader.StepBack();
                        }
                        return PrepareResult(headerName, out header);
                    }
                    case '\n':
                    {
                        return PrepareResult(headerName, out header);
                    }
                    case ':':
                    {
                        header = (headerName.Length != 0) ? headerName.ToString() : null;
                        return ((headerName.Length != 0) && (result == ReadHeaderResult.Success))
                            ? ReadHeaderResult.Success
                            : ReadHeaderResult.ContainsIllegalChars;
                    }
                    default:
                    {
                        if (!IsLegalHeaderFieldNameChar(ch) || waitForColon)
                            result = ReadHeaderResult.ContainsIllegalChars;
                        headerName.Append(ch);
                        break;
                    }
                }
            }

            return result;
        }

        private static ReadHeaderResult PrepareResult(StringBuilder headerName, out string header)
        {
            header = (headerName.Length != 0) ? headerName.ToString() : null;
            return (headerName.Length != 0)
                ? ReadHeaderResult.ContainsIllegalChars
                : ReadHeaderResult.EmptyLine;
        }

        /// <summary>
        /// For header "Header-name: Header value[CR][LF]" reads " Header value[CR][LF]" and
        /// returns either header value or null if fails.
        /// </summary>
        private static string ReadHeaderValue(CustomTextReader textReader)
        {
            bool hasLegalChars = false;
            bool emptyHeaderValue = true;
            StringBuilder headerValue = new StringBuilder();

            while (textReader.HasChars)
            {
                char ch = textReader.ReadChar();

                if (ch == '\0')
                    return null;

                headerValue.Append(ch);

                if (emptyHeaderValue && !StringUtil.IsWhiteSpace(ch))
                    emptyHeaderValue = false;

                // Skip normal chars.
                if ((ch != '\\') && (ch != '\r') && (ch != '\n'))
                {
                    if (!hasLegalChars)
                        hasLegalChars = IsLegalHeaderFieldValueChar(ch);

                    continue;
                }

                // Get ready for reading of the next char.
                if (textReader.HasChars)
                {
                    char nextCh = textReader.ReadChar();

                    // A char after '\\' must be skipped. So we don't do StepBack for it. Zero char is acceptable here.
                    if (ch == '\\')
                        continue;

                    // Zero char in text stream is an error.
                    if (nextCh == '\0')
                        return null;

                    // We want to know a char after '\r' and '\n' but we don't want to consume it.
                    textReader.StepBack();

                    // Treat this pair as one character '\n'. It will be handled during the next cycle.
                    if ((ch == '\r') && (nextCh == '\n'))
                        continue;

                    // Treat a space character after '\r' or '\n' as folded header.
                    // It's a good reason to reset reading limit. May be, the header is very long.
                    if ((nextCh == ' ') || (nextCh == '\t'))
                    {
                        textReader.ResetReadLimit();
                    }
                    else
                    {
                        // FIX 8491 According to this fix, we should allow an empty line as header value.
                        return (hasLegalChars || emptyHeaderValue)
                            ? headerValue.ToString().Trim()
                            : null;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Checks whether the character is legal for a header field name.
        /// </summary>
        private static bool IsLegalHeaderFieldNameChar(char ch)
        {
            // Internet Message Format Specification (RFC 5322, http://tools.ietf.org/html/rfc5322) states: 
            //      A header field name MUST be composed of printable US-ASCII characters (i.e., characters 
            //      that have values between 33 and 126, inclusive), except colon.
            // This rule is too loose for file format detection. For example, the following valid 
            // HTML document line agrees with this rule and is confused with a header field:
            //      <html><body><p>Name: John<p>
            // Analysis of common header names shows that they all are composed of letters, digits and hyphens
            // (for example, see http://www.cs.tut.fi/~jkorpela/headers.html and https://tools.ietf.org/html/rfc2076).
            // So we can use a stricter rule for header field name detection.
            return StringUtil.IsLetter(ch) || StringUtil.IsDigit(ch) || (ch == '-');
        }

        /// <summary>
        /// Checks whether the character is legal for a header field value.
        /// </summary>
        private static bool IsLegalHeaderFieldValueChar(char ch)
        {
            // Internet Message Format Specification (RFC 5322, http://tools.ietf.org/html/rfc5322) states:
            //          A field body may be composed of printable US-ASCII characters (i.e., characters 
            //          that have values between 33 and 126, inclusive) as well as the space (SP, ASCII value 32) 
            //          and horizontal tab (HTAB, ASCII value 9) characters (together known as the white space characters, WSP).
            // So we can use this rule for detecting char is appropriate or not.
            return ((ch >= 32) && (ch <= 126)) || (ch == 9);
        }

        /// <summary>
        /// Checks whether the content type specified in the "Content-Type" header value is supported by our MHTML reader.
        /// </summary>
        private static bool IsSupportedContentType(string contentTypeHeaderValue)
        {
            foreach (string prefix in gSupportedContentTypePrefixes)
            {
                if (contentTypeHeaderValue.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private static readonly string[] gSupportedContentTypePrefixes = new string[] { "multipart/", "text/", "image/" };
    }
}
