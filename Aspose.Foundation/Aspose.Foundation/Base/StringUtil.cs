// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/08/2014 by nislamov

using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.JavaAttributes;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose
{
    /// <summary>
    /// <para>Contains simple string utility methods (autoportable).</para>
    /// <para>Also see <see cref="Aspose.Bidi.UnicodeUtil"/> for some more utility functions.</para>
    /// </summary>
    public static class StringUtil
    {
        /// <summary>
        /// Converts the character to lower case if it is an upper-case ASCII letter ('A'..'Z').
        /// Otherwise, leaves the character unchanged.
        /// </summary>
        public static char AsciiLowerCase(char c)
        {
            return IsAsciiUpperCase(c)
                ? (char)(c - 'A' + 'a')
                : c;
        }

        /// <summary>
        /// Determines whether all ASCII letters in the specified text are lower case.
        /// </summary>
        public static bool IsAsciiLowerCase(string text)
        {
            if (text == null)
                return true;

            foreach (char c in text)
            {
                if (IsAsciiUpperCase(c))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Converts all upper-case ASCII letters in a string to lower case.
        /// </summary>
        public static string AsciiLowerCase(string text)
        {
            // SPEED. We optimistically assume that the source text is already converted and contains no upper-case ASCII
            // letters. This is usually true in scenarios where this method is used (parsing of CSS and HTML, for example).
            int i = 0;
            while (i < text.Length)
            {
                if (IsAsciiUpperCase(text[i]))
                {
                    // Found an upper-case ASCII letter. Conversion is needed.
                    break;
                }
                ++i;
            }

            // The fast path. No conversion is needed, because the source string contains no upper-case ASCII letters.
            if (i == text.Length)
            {
                return text;
            }

            // The slow path.
            // SPEED. It's faster to work with a char array than with a StringBuilder.
            char[] result = new char[text.Length];

            // Copy the beginning of the source string that contains no upper-case ASCII letters.
            text.CopyTo(0, result, 0, i);

            // Convert the rest of the string.
            while (i < text.Length)
            {
                result[i] = AsciiLowerCase(text[i]);
                ++i;
            }

            return new string(result);
        }

        /// <summary>
        /// Returns true if the character belongs to the Private Use Unicode Category.
        /// </summary>
        public static bool IsPrivateUseCategory(char c)
        {
            return StringUtilPal.IsPrivateUseCategory(c);
        }

        /// <summary>
        /// Returns true if the string is not null and not empty.
        /// </summary>
        public static bool HasChars(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Truncates a string from the right, then appends '...' for the total specified length.
        /// </summary>
        /// <param name="s">A string.</param>
        /// <param name="length">Length desired.</param>
        public static string Ellipsisize(string s, int length)
        {
            const string ellipsis = "...";
            return (length <= 0 || !HasChars(s)) ? string.Empty
                : ((length >= s.Length) ? s
                    : ((length <= ellipsis.Length) ? s.Substring(0, length)
                        : s.Substring(0, length - ellipsis.Length) + ellipsis));
        }

        /// <summary>
        /// Truncates a string from the right to the specified length.
        /// </summary>
        public static string Truncate(string value, int length)
        {
            Debug.Assert(length >= 0);

            if (value == null)
                return null;

            if (value.Length <= length)
                return value;

            return value.Substring(0, length);
        }

        /// <summary>
        /// This is a useful method to call before you, for example combine two strings and one of them might be null.
        /// In .NET such string addition will work okay, but in Java it will add the string "null". Using this method
        /// converts a null string into an empty string and allows to avoid the problem on Java.
        /// </summary>
        public static string NullToEmptyString(string value)
        {
            return value ?? string.Empty;
        }

        /// <summary>
        /// Checks if parameter is null and returns "null" in this case.
        /// </summary>
        [JavaDelete]
        public static string NullToString(this object value)
        {
            return (value ?? "null").ToString();
        }

        /// <summary>
        /// Platform abstract version of 'source.IndexOf(value, StringComparison.InvariantCultureIgnoreCase)'.
        /// </summary>
        public static int IndexOfIgnoreCase(string source, string value)
        {
            return source.IndexOf(value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Check if the text contains the specified string.
        /// Works both for .NET 2.0 and .NET 1.1
        /// </summary>
        public static bool Contains(string text, string searchStr, bool ignoreCase)
        {
            return text.IndexOf(searchStr,
                (ignoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) >= 0;
        }

        /// <summary>
        /// Detects strings consisting solely of spaces.
        /// </summary>
        [CppSkipDefinition(false)]
        public static bool ContainsOnlySpaces(string text)
        {
            foreach (char c in text)
            {
                if (c != ' ')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Detects strings consisting solely of whitespaces.
        /// </summary>
        [CppSkipDefinition(false)]
        public static bool ContainsOnlyWhitespaces(string text)
        {
            return (IndexOfNonWhitespace(text) == -1);
        }

        /// <summary>
        /// Returns true, if a specified string contains only ASCII characters.
        /// </summary>
        public static bool ContainsOnlyAscii(string value)
        {
            foreach (char c in value)
            {
                if (c > 127)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the index of the first occurence of a non-whitespace character within the specified string
        /// or <b>-1</b> if not found.
        /// </summary>
        [CppSkipDefinition(false)]
        public static int IndexOfNonWhitespace(string text)
        {
            return IndexOfNonWhitespace(text, 0);
        }

        /// <summary>
        /// Returns the index of the first occurence of a non-whitespace character within the specified string
        /// or <b>-1</b> if not found.
        /// </summary>
        [CppSkipDefinition(false)]
        public static int IndexOfNonWhitespace(string text, int start)
        {
            if ((text != null) && (start >= 0))
            {
                for (int i = start; i < text.Length; i++)
                {
                    if (!char.IsWhiteSpace(text[i]))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns true, if a specified string builder consists solely of whitespaces.
        /// </summary>
        public static bool ContainsOnlyWhitespaces(StringBuilder text)
        {
            return (IndexOfNonWhitespace(text) == -1);
        }

        /// <summary>
        /// Returns the index of the first occurence of a non-whitespace character within the specified string
        /// builder or <b>-1</b> if not found.
        /// </summary>
        public static int IndexOfNonWhitespace(StringBuilder text)
        {
            if (text != null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (!char.IsWhiteSpace(text[i]))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the last occurence of a non-whitespace character within the specified string
        /// builder or <b>-1</b> if not found.
        /// </summary>
        public static int LastIndexOfNonWhitespace(StringBuilder text)
        {
            if (text != null)
            {
                for (int i = text.Length - 1; i >= 0; i--)
                {
                    if (!char.IsWhiteSpace(text[i]))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns true, if a specified string builder ends with a specified substring.
        /// </summary>
        public static bool IsEndsWith(StringBuilder text, string endSubstr)
        {
            if ((text == null) || (endSubstr == null))
                return false;

            int i;
            int j;
            for (i = (text.Length - 1), j = (endSubstr.Length - 1); ((i >= 0) && (j >= 0)); i--, j--)
            {
                if (text[i] != endSubstr[j])
                    return false;
            }

            return (j == -1);
        }

        /// <summary>
        /// Returns the index of the first occurence of a whitespace character within the specified string
        /// or <b>-1</b> if not found.
        /// </summary>
        [CppSkipDefinition(false)]
        public static int IndexOfWhitespace(string text)
        {
            if (text != null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (char.IsWhiteSpace(text[i]))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the first occurrence of a character that is
        /// categorized as a Unicode letter within the specified string,
        /// or <b>-1</b> if not found.
        /// </summary>
        [CppSkipDefinition(false)]
        public static int IndexOfLetter(string text)
        {
            if (text != null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (char.IsLetter(text[i]))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the index of the first occurrence of a character that is
        /// considered to be a word break within the specified string,
        /// or <b>-1</b> if not found.
        /// </summary>
        [CppSkipDefinition(false)]
        public static int IndexOfWordBreak(string text)
        {
            if (text != null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    char c = text[i];
                    if (IsWordBreakChar(c))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes all whitespace characters from a specified string.
        /// </summary>
        public static StringBuilder RemoveAllWhitespaces(string text)
        {
            if (text == null)
                return null;

            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                if (!char.IsWhiteSpace(c))
                    sb.Append(c);
            }

            return sb;
        }

        /// <summary>
        /// Returns the index of the first character not equal to a specified one in a string starting from a specified index
        /// or <b>-1</b> if not found.
        /// </summary>
        public static int IndexOfNotEqualTo(string text, char value, int start)
        {
            if (text != null)
            {
                for (int i = start; i < text.Length; i++)
                {
                    if (text[i] != value)
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Detects strings containing any whitespaces.
        /// </summary>
        [CppSkipDefinition(false)]
        public static bool ContainsAnyWhitespaces(string text)
        {
            foreach (char c in text)
            {
                if (char.IsWhiteSpace(c))
                    return true;
            }

            return false;
        }

        public static bool IsWhiteSpace(int ch)
        {
            switch (ch)
            {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Converts hex digit chars 0-9, A-F, a-f to a correspondent digit 0-15.
        /// </summary>
        public static int HexCharToDigit(char digit)
        {
            int result = (digit > '9') ? (LetterToInt(digit) + 10) : (digit - '0');

            if ((result < 0) || (result > 15))
                throw new ArgumentOutOfRangeException("digit");

            return result;
        }

        /// <summary>
        /// Converts characters 'A'-'Z' (case insensitively) to integers 0-25.
        /// </summary>
        public static int LetterToInt(char letter)
        {
            int result = letter - ((letter <= 'Z') ? 'A' : 'a');

            if ((result < 0) || (result > 25))
                throw new ArgumentOutOfRangeException("letter");

            return result;
        }

        /// <summary>
        /// Checks if a character is a valid Latin letter: 'A'-'Z' or 'a'-'z'.
        /// SPEED Works faster than Char.IsLetter.
        /// </summary>
        /// <remarks>
        /// <paramref name="ch"/> is int to support both characters and bytes read by <see cref="Stream.ReadByte"/>.
        /// </remarks>
        public static bool IsLetter(int ch)
        {
            return
                ((ch >= 'a') && (ch <= 'z')) ||
                ((ch >= 'A') && (ch <= 'Z'));
        }

        /// <summary>
        /// Checks if a character is a valid Latin letter: 'A'-'Z' or 'a'-'z', or
        /// if a character is a valid decimal digit character: 0-9
        /// </summary>
        /// <remarks>
        /// <paramref name="ch"/> is int to support both characters and bytes read by <see cref="Stream.ReadByte"/>.
        /// </remarks>
        public static bool IsLetterOrDigit(int ch)
        {
            return IsLetter(ch) || IsDigit(ch);
        }

        /// <summary>
        /// Checks if a character is a valid hex digit character: 0-9, A-F, or a-f.
        /// </summary>
        public static bool IsHexDigit(char ch)
        {
            return
                ((ch >= '0') && (ch <= '9')) ||
                ((ch >= 'A') && (ch <= 'F')) ||
                ((ch >= 'a') && (ch <= 'f'));
        }

        /// <summary>
        /// Returns a value indicating whether the specified character is treated as double quote by MS Word.
        /// </summary>
        public static bool IsDoubleQuote(char ch)
        {
            switch (ch)
            {
                case '"':
                case '«':
                case '“':
                case '„':
                case '»':
                case '”':
                case '‟':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified character represents a punctuation mark.
        /// </summary>
        public static bool IsPunctuationMark(char c)
        {
            switch (c)
            {
                case '!':
                case '?':
                case '.':
                case ',':
                case ':':
                case ';':
                case '¡':
                case '¿':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether the specified character represents a dash (see https://en.wikipedia.org/wiki/Dash).
        /// </summary>
        public static bool IsDash(char ch)
        {
            switch (ch)
            {
                case '‒':
                case '–':
                case '—':
                case '―':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if string is valid positive decimal number.
        /// </summary>
        [CppSkipDefinition(false)]
        public static bool IsDecimal(string value)
        {
            foreach (char ch in value)
            {
                if (!IsDigit(ch))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if string is valid hexadecimal number.
        /// </summary>
        [CppSkipDefinition(false)]
        public static bool IsHexadecimal(string value)
        {
            foreach (char ch in value)
            {
                if (!IsHexDigit(ch))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Converts a byte array into the string representation, e.g. "B601FE00".
        /// </summary>
        /// <param name="bytes">Array of bytes to convert.</param>
        /// <param name="startIndex">The index of the byte to start the conversion from.</param>
        /// <param name="length">Number of bytes to convert.</param>
        /// <param name="reversed">Put bytes to string in reversed order.</param>
        /// <returns>String representation of the sequence of bytes, e.g. "B601FE00".</returns>
        [CppSkipDefinition(false)]
        public static string BytesToHex(byte[] bytes, int startIndex, int length, bool reversed)
        {
            StringBuilder stringBuilder = new StringBuilder(length * 2);

            int step = (reversed) ? -1 : +1;

            if (reversed)
                startIndex += length - 1;

            for (int i = 0; i < length; i++)
            {
                stringBuilder.Append((char[])ByteToHex(bytes[startIndex]));
                startIndex += step;
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Converts a byte into char array (with length of 2) of its string representation, e.g. 0x2A to {'2', 'A'}.
        /// </summary>
        [CppSkipDefinition(false)]
        public static char[] ByteToHex(byte b)
        {
            char[] hex = new char[2];

            hex[0] = gHexChars[(b >> 4) & 0x0F]; // Need to mask because byte is signed on Java.
            hex[1] = gHexChars[b & 0x0F];

            return hex;
        }

        /// <summary>
        /// Converts byte array into the string representation, e.g. "B601FE00".
        /// </summary>
        /// <param name="bytes">Array of bytes to convert.</param>
        /// <returns>String representation of the sequence of bytes, e.g. "B601FE00".</returns>
        public static string BytesToHex(byte[] bytes)
        {
            return BytesToHex(bytes, 0, bytes.Length, false);
        }

        /// <summary>
        /// Converts hex string into the byte array.
        /// Skip characters that is not valid hex characters.
        /// </summary>
        public static byte[] HexToBytes(string hexString)
        {
            int arrayLength = hexString.Length / 2;
            byte[] array = new byte[arrayLength];

            char high = '\0';
            int j = 0;
            foreach (char ch in hexString)
            {
                if (!IsHexDigit(ch))
                    continue;

                if (high == '\0')
                {
                    high = ch;
                }
                else
                {
                    char low = ch;
                    int v1 = HexCharToDigit(high);
                    int v2 = HexCharToDigit(low);
                    array[j++] = (byte)((v1 << 4) | v2);
                    high = '\0';
                }
            }

            // Some characters were stripped from string. Return new array.
            if (j < arrayLength)
            {
                byte[] newArray = new byte[j];
                Array.Copy(array, newArray, j);
                return newArray;
            }

            return array;
        }

        /// <summary>
        /// Compares strings using case insensitive compare and returns true if they are same.
        /// Use this method because it works for .NET 2.0 and .NET 1.1 and Java.
        /// </summary>
        public static bool EqualsIgnoreCase(string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compares strings using ordinal case insensitive compare and returns true if they are same.
        /// </summary>
        [CppSkipDefinition(false)]
        public static bool EqualsOrdinalIgnoreCase(string s1, string s2)
        {
            return EqualsOrdinalIgnoreCase(s1, s2, false);
        }
        [CppSkipEntity("Manual porting for better performance")]
        private static bool EqualsOrdinalIgnoreCase(string s1, string s2, bool isStartsWith)
        {
            if (ReferenceEquals(s1, s2))
                return true;

            if ((s1 == null) || (s2 == null))
                return false;

            int n1 = s1.Length;
            int n2 = s2.Length;
            if ((isStartsWith && (n1 < n2)) || (!isStartsWith && (n1 != n2)))
                return false;

            return CompareOrdinalIgnoreCaseCore(s1, s2, n2) == 0;
        }
        [CppSkipEntity("Manual porting for better performance")]
        private static int CompareOrdinalIgnoreCaseCore(string s1, string s2, int n)
        {
            for (int i = 0; i < n; i++)
            {
                int c1 = s1[i];
                int c2 = s2[i];

                // If chars are equal there is no need to compare them case-insensitively.
                if (c1 == c2)
                    continue;

                // Case-insensitive comparison of ascii chars taken from Rotor COMString::CaseInsensitiveCompHelper.
                if ((c1 | c2) <= 0x7F)
                {
                    // uppercase both chars.
                    if (c1 >= 'a' && c1 <= 'z')
                        c1 ^= 0x20;
                    if (c2 >= 'a' && c2 <= 'z')
                        c2 ^= 0x20;

                    // Return the (case-insensitive) difference between them.
                    if (c1 != c2)
                        return (c1 - c2);
                }
                else
                {
                    // Non-ascii chars comparison. Added for compatibility.
                    // Code taken from java.lang.String.CaseInsensitiveComparator.
                    c1 = char.ToUpperInvariant((char)c1);
                    c2 = char.ToUpperInvariant((char)c2);
                    if (c1 != c2)
                    {
                        c1 = char.ToLowerInvariant((char)c1);
                        c2 = char.ToLowerInvariant((char)c2);
                        if (c1 != c2)
                            return (c1 - c2);
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Compares strings using ordinal case insensitive compare and returns true if one string starts with the other.
        /// </summary>
        [CppSkipDefinition(false)]
        public static bool StartsWithOrdinalIgnoreCase(string source, string prefix)
        {
            return EqualsOrdinalIgnoreCase(source, prefix, true);
        }

        /// <summary>
        /// Checks if a character is a valid decimal digit character: 0-9.
        /// SPEED Works faster than Char.IsDigit.
        /// </summary>
        /// <remarks>
        /// <paramref name="ch"/> is int to support both characters and bytes read by Stream.ReadByte.
        /// </remarks>
        public static bool IsDigit(int ch)
        {
            return (ch >= '0') && (ch <= '9');
        }

        /// <summary>
        /// Removes all non-digit characters from the end of the string until the first digit character.
        /// </summary>
        public static string TrimEndNonDigits(string input)
        {
            for (int i = input.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(input[i]))
                    return input.Substring(0, i + 1);
            }
            return input;
        }

        /// <summary>
        /// Removes leading occurrences of whitespace characters of a specified length.
        /// </summary>
        public static string TrimStart(string value, int length)
        {
            int index = 0;
            while ((index < length) && (index < value.Length) && char.IsWhiteSpace(value[index]))
                index++;

            return value.Substring(index);
        }

        /// <summary>
        /// Removes leading occurrences of whitespace characters from a specified string builder.
        /// </summary>
        public static void TrimStart(StringBuilder sb)
        {
            int index = 0;
            while ((index < sb.Length) && char.IsWhiteSpace(sb[index]))
                index++;

            sb.Remove(0, index);
        }

        /// <summary>
        /// Removes all the trailing occurrences of whitespace characters from a specified string builder.
        /// </summary>
        public static void TrimEnd(StringBuilder sb)
        {
            int index = sb.Length - 1;
            while ((index >= 0) && char.IsWhiteSpace(sb[index]))
                index--;

            sb.Remove(index + 1, sb.Length - (index + 1));
        }

        /// <summary>
        /// Returns a new string containing characters
        /// of the specified string in the reversed order.
        /// </summary>
        public static string ReverseString(string str)
        {
            char[] chars = str.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        /// <summary>
        /// Remove all occurences of substring in the string.
        /// For instance, remove("abcdef defde", "def") returns "abc de".
        /// Don't use regexp, so this code run faster than str.replace(substr, string.Empty),
        /// at least in Java.
        /// </summary>
        public static string RemoveSubstring(string str, string substr)
        {
#if !JAVA
            if (!HasChars(str) || !HasChars(substr))
                return str;
            return str.Replace(substr, string.Empty);
#else
            int len1 = str.Length;
            int len2 = substr.Length;

            if (len1 == 0 || len2 == 0)
                return str;

            int n = 0;
            int i = 0;
            // at first, count occurrences
            while (i < len1 - len2 + 1)
            {
                bool match = true;
                for (int p = 0; p < len2; p++)
                {
                    if (str[i + p] != substr[p])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    i += len2;
                    n++;
                }
                else
                    i++;
            }

            if (n == 0)
                return str;

            // allocate and fill new buffer
            char[] result = new char[len1 - n*len2];
            i = 0;
            int j = 0;
            while (i < len1 - len2 + 1)
            {
                bool match = true;
                for (int p = 0; p < len2; p++)
                {
                    if (str[i + p] != substr[p])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    i += len2;
                }
                else
                {
                    result[j] = str[i];
                    i++;
                    j++;
                }
            }

            if (i < len1)
            {
                for (int p = i; p < len1; p++)
                    result[j++] = str[p];
            }

            return new String(result);
#endif
        }

        /// <summary>
        /// Helps to format comma separated list.
        /// </summary>
        public static string AppendCommaSeparated(string list, string item)
        {
            if (!HasChars(list))
                return item;

            if (!HasChars(item))
                return list;

            return string.Format("{0},{1}", list, item);
        }

        /// <summary>
        /// Helps to porting Environment.NewLine in Java.
        /// </summary>
        public static string NewLine
        {
            get
            {
                if (PlatformUtilPal.IsUnixLike())
                    return "\n";
                else
                    return "\r\n";
            }
        }

        /// <summary>
        /// Compares string objects using ordinal case insensitive compare.
        /// </summary>
        [CppSkipDefinition(false)]
        public static int CompareOrdinalIgnoreCase(object a, object b)
        {
            // Object equality and null string comparison used only in .Net. Taken from Rotor's String.Compare():
            // Our (.Net) paradigm is that null sorts less than any other string and
            // that two nulls sort as equal. (sk plus: two equal constant strings always stored in a single object).
            if (ReferenceEquals(a, b))
                return 0;
            if (a == null)
                return -1;    // null < non-null
            if (b == null)
                return 1;     // non-null > null

            string s1 = (string)a;
            string s2 = (string)b;
            int n1 = s1.Length;
            int n2 = s2.Length;

            int result = StringUtil.CompareOrdinalIgnoreCaseCore(s1, s2, Math.Min(n1, n2));

            return (result != 0) ? result : (n1 - n2);
        }

        /// <summary>
        /// Compares strings using CompareOptions.StringSort.
        /// </summary>
        /// <remarks>
        /// MS Word seems to use this method.
        /// The default method causes different results, WORDSNET-4640 is an example.
        /// </remarks>
        [CppSkipDefinition(false)]
        public static int CompareStringSort(string a, string b)
        {
            return StringUtilPal.CompareStringSort(a, b);
        }

        /// <summary>
        /// Outputs fields of the given object to a string. Used in logging.
        /// </summary>
        [JavaConvertCheckedExceptions]
        [CppSkipEntity("Manual porting by design")]
        public static string ObjectToLogString(object obj, string name)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("{0} Start\n", name);

            Type type = obj.GetType();
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo field in fields)
                builder.AppendFormat("{0}:{1}, 0x{1:X}\n", field.Name, field.GetValue(obj));

            builder.AppendFormat("{0} End", name);
            return builder.ToString();
        }

        /// <summary>
        /// Formats the text string according to the specified char case.
        /// </summary>
        public static string FormatCharCase(string s, CharCase charCase)
        {
            switch (charCase)
            {
                case CharCase.Upper:
                    return s.ToUpper();
                case CharCase.Lower:
                    return s.ToLower();
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
        /// Capitalizes first non whitespace character of the string.
        /// </summary>
        [CppSkipDefinition(false)]
        public static string CapitalizeString(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            StringBuilder builder = new StringBuilder(s);
            for (int i = 0; i < builder.Length; i++)
            {
                char c = builder[i];
                if (!IsWordBreakChar(c))
                {
                    builder[i] = char.ToUpper(c);
                    break;
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Capitalizes first non whitespace character that follows every whitespace.
        /// </summary>
        private static string CapitalizeWords(string s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            bool isInsideWord = false;
            // Use of StringBuilder seems to be simpler here. We can't use string.Split and string.Join
            // because different types and amount of whitespace is possible between words.
            StringBuilder builder = new StringBuilder(s);

            for (int i = 0; i < builder.Length; i++)
            {
                char c = builder[i];
                if (IsWordBreakChar(c))
                {
                    isInsideWord = false;
                }
                else if (!isInsideWord)
                {
                    builder[i] = char.ToUpper(c);
                    isInsideWord = true;
                }
                else
                {
                    builder[i] = char.ToLower(c);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Tries to convert the base64 string to a byte array.
        /// </summary>
        /// <remarks>
        /// This is a wrapper over <see cref="Convert.FromBase64String"/>.
        /// The method doesn't make an attempt to restore corrupted base64 string.
        /// </remarks>
        /// <returns>
        /// Returns a byte array that is equivalent to base64 string or an empty byte array if the conversion failed.
        /// </returns>
        public static byte[] TryConvertFromBase64(string base64String)
        {
            return TryConvertFromBase64String(base64String, false);
        }

        /// <summary>
        /// Safely converts the base64 string to a byte array.
        /// </summary>
        /// <remarks>This is a safe wrapper over <see cref="Convert.FromBase64String"/>.</remarks>
        /// <returns>
        /// Returns a byte array that is equivalent to base64 string or an empty byte array if the conversion failed.
        /// </returns>
        public static byte[] ConvertFromBase64Safe(string base64String)
        {
            return TryConvertFromBase64String(base64String, true);
        }

        /// <summary>
        /// Tries to convert the base64 string to a byte array either with an attempt to restore
        /// currupted base64 string or not.
        /// </summary>
        /// <remarks>
        /// The method can make an attempt to restore corrupted base64 string.
        /// This behavior is turned on/off by restoreBase64String flag.
        /// </remarks>
        private static byte[] TryConvertFromBase64String(string base64String, bool restoreBase64String)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                return new byte[0];
            }

            string valueToConvert = restoreBase64String ? RestoreBase64String(base64String) : base64String;
            try
            {
                return Convert.FromBase64String(valueToConvert);
            }
            catch (FormatException)
            {
                return new byte[0];
            }
        }

        /// <summary>
        /// Tries to restore corrupted base64 string.
        /// </summary>
        private static string RestoreBase64String(string base64String)
        {
            // The Base64 string must contain only the following characters: A-Z a-z 0-9 + / =
            const string regexPattern = @"[^A-Za-z0-9+/=]";
            Regex regex = new Regex(regexPattern);

            // Remove all symbols that are not in the allowed set.
            string restoredString = regex.Replace(base64String, "");
            // WORDSNET-27395 Some corrupted or incorrectly created base64 strings may contain 3 paddig characters '='
            // at the end to revert to the original encoding, mostly created with javascript.
            // Trim all '=' characters when we try to repair the base64 string.
            // For details, see https://www.w3.org/TR/xmlschema-2/#base64Binary.
            restoredString = restoredString.TrimEnd('=');

            // The Base64 string length must be a multiple of 4. If it is not, add '='.
            // It helps determine how many padding characters ('=') are needed to make the length a multiple of 4.
            int paddings = (4 - restoredString.Length % 4) % 4;
            restoredString = StringUtilPal.PadRight(restoredString, restoredString.Length + paddings, '=');

            return restoredString;
        }

        /// <summary>
        /// Converts ASCII numbers and letters to full-width numbers and letters.
        /// </summary>
        /// <param name="value">A value to convert.</param>
        /// <returns>A converted <paramref name="value"/>.</returns>
        private static string ToFullWidth(string value)
        {
            if (!HasChars(value))
            {
                return string.Empty;
            }

            StringBuilder converted = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 0x20 && c < 0x7F)
                {
                    converted.Append((char)(0xFF00 | (c - 0x20)));
                }
                else if (c == 0x20)
                {
                    converted.Append((char)0x3000);
                }
                else
                {
                    converted.Append(c);
                }
            }

            return converted.ToString();
        }

        /// <summary>
        /// Trim ending CRLF characters from memory stream
        /// Used for C++ port where libxml2 generated documents have such ending
        /// Do nothing on C# anc Java
        /// </summary>
        [CppSkipDefinition(false)]
        public static Stream TrimRightCrLfStream(Stream stream)
        {
#if CPLUSPLUS
            MemoryStream memoryStream = stream as MemoryStream;
            if (memoryStream != null)
            {
                byte[] buffer = memoryStream.GetBuffer();
                long length = memoryStream.Length;
                if (buffer[length - 2] == '\r' && buffer[length - 1] == '\n')
                {
                    return new MemoryStream(buffer, 0, (int)length - 2, false);
                }
            }

            return stream;
#else
            return stream;
#endif
        }

        /// <summary>
        /// Returns a value indicating whether the specified character is considered to be a word break
        /// in one of the capitalization algorithms.
        /// </summary>
        /// <param name="c"></param>
        public static bool IsWordBreakChar(char c)
        {
            // WORDSNET-7593 MS Word treats punctuation characters as word breaks.
            // It seems like the right way here is to return Char.IsWhiteSpace(c) || Char.IsPunctuation(c) || Char.IsSymbol(c)
            // but (!Char.IsLetterOrDigit(c)) is more perfomant although it might be not right in some rare cases.
            return !char.IsLetterOrDigit(c);
        }

        /// <summary>
        /// Returns a value indicating whether the specified character is an upper-case ASCII letter ('A'..'Z').
        /// </summary>
        private static bool IsAsciiUpperCase(char c)
        {
            return (c >= 'A') && (c <= 'Z');
        }

        // WORDSJAVA-3205 This need for Java testing
        public static bool IsNullOrEmpty(string str)
        {
            return string.IsNullOrEmpty(str);
        }

        private static readonly char[] gHexChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
    }
}
