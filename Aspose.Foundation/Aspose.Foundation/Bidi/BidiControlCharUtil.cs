// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/04/2015 by Ilya Navrotskiy

namespace Aspose.Bidi
{
    /// <summary>
    /// Helper class for working with BiDi control characters <see cref="BidiChars"/>. 
    /// </summary>
    public static class BidiControlCharUtil
    {

        /// <summary>
        /// Returns BiDi control character based on the specified type and direction string.
        /// </summary>
        /// <param name="bidiControlCharType">String with name of BiDi control char type: 'dir' or 'bdo'</param>
        /// <param name="bidiControlCharDirection">String with name of BiDi control char text direction: 'ltr' or 'rtl'</param>
        public static char ToBidiChar(string bidiControlCharType, string bidiControlCharDirection)
        {
            if (bidiControlCharType == "dir")
                return (bidiControlCharDirection == "ltr") ? BidiChars.LRE : BidiChars.RLE;
            else
                return (bidiControlCharDirection == "ltr") ? BidiChars.LRO : BidiChars.RLO;
        }

        /// <summary>
        /// Returns one of the strings 'dir' or 'bdo', depending on the specified BiDi control character.
        /// </summary>
        public static string ToTypeString(char ch)
        {
            Debug.Assert(IsBidiControlCharStart(ch));

            if ((ch == BidiChars.RLE) || (ch == BidiChars.LRE))
                return "dir";
            else
                return "bdo";
        }

        /// <summary>
        /// Returns one of the strings 'rtl' or 'ltr', depending on the specified BiDi control character.
        /// </summary>
        public static string ToDirectionString(char ch)
        {
            Debug.Assert(IsBidiControlCharStart(ch));

            if ((ch == BidiChars.RLE) || (ch == BidiChars.RLO))
                return "rtl";
            else
                return "ltr";
        }

        /// <summary>
        /// Returns 'true' if specified character is one of the BiDi control character and
        /// it is not equal to 'PDF' closing control char.
        /// </summary>
        public static bool IsBidiControlCharStart(char ch)
        {
            if (IsBidiControlChar(ch))
                return (ch != BidiChars.PDF);

            return false;
        }

        /// <summary>
        /// Returns 'true' if the specified character is one of the BiDi control chars: LRE, RLE, LRO, RLO, PDF.
        /// </summary>
        private static bool IsBidiControlChar(char ch)
        {
            return ((ch == BidiChars.LRE) ||
                    (ch == BidiChars.RLE) ||
                    (ch == BidiChars.LRO) ||
                    (ch == BidiChars.RLO) ||
                    (ch == BidiChars.PDF));
        }
    }
}