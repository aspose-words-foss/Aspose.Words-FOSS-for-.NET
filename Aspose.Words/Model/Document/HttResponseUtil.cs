// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/04/2016 by Dmitry Sokolov
using System;
using System.Text;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Implements encode string methods for HttpResponce header.
    /// </summary>
    [JavaDelete("We do not provide saving document to a response stream in Java.")]
    internal static class HttResponseUtil
    {
        /// <summary>
        /// Encodes all characters as hex if char is not in hex-encoding yet.      
        /// </summary>
        /// <param name="s">Incoming string for processing.</param>
        /// <param name="extFormat">True when incoming string needs to be convert to extended format.</param>
        internal static string EscapeMimeParameterValue(string s, bool extFormat)
        {
            const int hexBodyLen = 2; // Count of hex digits in hex-encoded character.            

            bool isByPass = false; // True when process old format hex-encoded character.
            int escHexCnt = 0;
            int hexBorder = s.Length - hexBodyLen; // Maximum position of first symbol in string with hex-encoded characters.         
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                // Check that current character is a part of hex-encoded characters.
                bool isHex = (escHexCnt > 0) || ((i < hexBorder) && Uri.IsHexEncoding(s, i));

                if (extFormat)
                {
                    // Process extended format character (filename*=). 
                    AppendNamePart(builder, c, !isHex);
                }
                else
                {
                    // Process old format character (filename=).
                    string hexEncodedStr = "";

                    if (isHex && (escHexCnt == 0))
                        hexEncodedStr = s.Substring(i, hexBodyLen + 1);

                    isByPass = EscapeOldFormat(builder, c, hexEncodedStr, escHexCnt, isHex, isByPass);
                }

                // Set value for hex encoding counter.
                if (isHex)
                    escHexCnt = (escHexCnt < hexBodyLen) ? ++escHexCnt : 0;

                // Reset flag when hex encoded character process complete.
                isByPass &= escHexCnt > 0;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Process input characters in old format.
        /// </summary>
        /// <param name="builder">Object for accumulate string characters.</param>
        /// <param name="c">Input character.</param>
        /// <param name="hexEncodedStr">String representation of hex encoded character.</param>
        /// <param name="escHexCnt">Hex encoding counter.</param>
        /// <param name="isHex">Flag showing that character is a part of hex-encoded characters.</param>
        /// <param name="isByPass"></param>
        /// <returns></returns>
        private static bool EscapeOldFormat(StringBuilder builder, char c, string hexEncodedStr, int escHexCnt, bool isHex, bool isByPass)
        {
            // Encode and decode characters if it is needed for properly download file by browser.
            // Safari works properly when ASCII characters is not hex-encoded.
            if (!isHex || isByPass)
            {
                bool needEncode = IsNeedEncode(c);
                AppendNamePart(builder, c, needEncode);
            }
            else if (escHexCnt == 0)
            {
                int curPos = 0;
                char curVal = Uri.HexUnescape(hexEncodedStr, ref curPos);
                isByPass = IsNeedEncode(curVal);
                curVal = isByPass ? c : curVal;
                AppendNamePart(builder, curVal, false);
            }

            return isByPass;
        }

        /// <summary>
        /// Check that input character should be encoded to hex code.
        /// </summary>
        /// <param name="c">Character for check.</param>
        /// <returns>True when character should be encoded.</returns>
        private static bool IsNeedEncode(char c)
        {
            // "," - 44. If character is not hex-encoded Safari and Chrome do not work properly.
            // ";" - 59. If character is not hex-encoded Safari does not work properly.            
            return ((c > 127) || (c == 44) || (c == 59));
        }

        /// <summary>
        /// Encode if it is need and add input data to character accumulator.
        /// </summary>
        /// <param name="builder">Object for accumulate string characters.</param>
        /// <param name="c">Input character.</param>
        /// <param name="needEncode">Flag showing that input character should be encoded.</param>
        private static void AppendNamePart(StringBuilder builder, char c, bool needEncode)
        {
            if (needEncode)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(c.ToString());
                foreach (byte b in bytes)
                    builder.Append(Uri.HexEscape((char)b));
            }
            else
            {
                builder.Append(c);
            }
        }
    }
}
