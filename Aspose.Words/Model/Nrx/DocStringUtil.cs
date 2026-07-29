// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/02/2017 by Alexey Butalov


using System.Text;
using Aspose.Words.RW.Nrx.Reader;

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Utility methods for working with DOC strings.
    /// </summary>
    internal static class DocStringUtil
    {
        /// <summary>
        /// Gets a Unicode or Windows-1252 encoding that works well for apostrophes etc.
        /// </summary>
        internal static Encoding GetEncoding(bool isUnicode)
        {
            return (isUnicode) ? gUnicodeEncoding : gWin1252Encoding;
        }

        /// <summary>
        /// Returns the number of bytes needed to write a unicode string with a zero terminator.
        /// </summary>
        internal static int GetWCharLengthBytes(string value)
        {
            return GetWCharLengthChars(value) * 2;
        }

        /// <summary>
        /// Returns the number of characters in a unicode string plus one for zero terminator.
        /// </summary>
        internal static int GetWCharLengthChars(string value)
        {
            return value.Length + 1;
        }

        static DocStringUtil()
        {
            // WORDSNET-20508 Incomplete surrogate character. Lets start with very simple replacement fallback. 
            gUnicodeEncoding = Encoding.GetEncoding(1200,//"Unicode" 
                new EncoderReplacementFallBack(), new DecoderReplacementFallBack("\x01"));
        }

        private static readonly Encoding gUnicodeEncoding;
        private static readonly Encoding gWin1252Encoding = Encoding.GetEncoding(1252);
    }
}
