// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/04/2020 by Edward Voronov

using System;
using Aspose.Common;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Rich string aware duplicate of the <see cref="StringUtil"/> class.
    /// Don't use <see cref="IString"/> abstraction in <see cref="StringUtil"/> for performance purpose.
    /// </summary>
    internal static class RichStringUtils
    {
        /// <summary>
        /// Formats the text string according to the specified char case.
        /// </summary>
        internal static RichString FormatCharCase(RichString s, CharCase charCase)
        {
            switch (charCase)
            {
                case CharCase.Upper:
                    return s.ToUpperInternal();
                case CharCase.Lower:
                    return s.ToLowerInternal();
                case CharCase.Caps:
                    return CapitalizeWords(s);
                case CharCase.FirstCap:
                    return CapitalizeString(s);
                case CharCase.DbChar:
                    return ToFullWidth(s);
                case CharCase.Default:
                    return s;
                default:
                    throw new InvalidOperationException("Unknown char case specified.");
            }
        }

        /// <summary>
        /// Capitalizes first non whitespace character that follows every whitespace.
        /// </summary>
        private static RichString CapitalizeWords(RichString s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            bool isInsideWord = false;
            // Use of StringBuilder seems to be simpler here. We can't use string.Split and string.Join
            // because different types and amount of whitespace is possible between words.
            RichStringBuilder builder = new RichStringBuilder(s);

            for (int i = 0; i < builder.Length; i++)
            {
                RichChar c = builder.GetInternal(i);
                if (StringUtil.IsWordBreakChar(c.ToSystemChar()))
                {
                    isInsideWord = false;
                }
                else if (!isInsideWord)
                {
                    builder.SetInternal(i, c.ToUpperInternal());
                    isInsideWord = true;
                }
                else
                {
                    builder.SetInternal(i, c.ToLowerInternal());
                }
            }

            return builder.ToRichString();
        }

        /// <summary>
        /// Capitalizes first non whitespace character of the string.
        /// </summary>
        private static RichString CapitalizeString(RichString s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            RichStringBuilder builder = new RichStringBuilder(s);
            for (int i = 0; i < builder.Length; i++)
            {
                RichChar c = builder.GetInternal(i);
                if (!StringUtil.IsWordBreakChar(c.ToSystemChar()))
                {
                    builder.SetInternal(i, c.ToUpperInternal());
                    break;
                }
            }

            return builder.ToRichString();
        }

        /// <summary>
        /// Converts ASCII numbers and letters to full-witdh numbers and letters.
        /// </summary>
        private static RichString ToFullWidth(RichString value)
        {
            if (RichStringBehaviour.IsNullOrEmptyInternal(value))
                return RichString.Empty;

            RichStringBuilder converted = new RichStringBuilder();
            foreach (IChar c in value)
            {
                char systemChar = c.ToSystemChar();

                if (systemChar > 0x20 && systemChar < 0x7F)
                {
                    converted.AppendInternal((char)(0xFF00 | (systemChar - 0x20)), (RichChar)c);
                }
                else if (systemChar == 0x20)
                {
                    converted.AppendInternal((char)0x3000, (RichChar)c);
                }
                else
                {
                    converted.AppendInternal((RichChar)c);
                }
            }

            return converted.ToRichString();
        }
    }
}
