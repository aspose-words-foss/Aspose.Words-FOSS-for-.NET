// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/03/2020 by Mikhail Nepreteamov

using Aspose.Words.RW.Markdown.Reader;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown AutolinkInline block.
    /// </summary>
    internal class AutolinkInlineBlock : ContainerBlock
    {
        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            return false;
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            Debug.Assert(IsOpened);

            Add(block);
            return true;
        }

        /// <summary>
        /// Writes the block into a model using specified context.
        /// </summary>
        internal override void Write(MarkdownReaderContext context)
        {
            context.Open(this);
            context.Builder.InsertHyperlink(base.Text, Url, false);
            context.Close(this);
        }

        /// <summary>
        /// Returns true, if a text between specified delimiters is valid AutoLink.
        /// </summary>
        internal static bool IsValid(Delimiter opening, Delimiter closing)
        {
            Debug.Assert((opening != null) && (closing != null));
            Debug.Assert(opening.IsBefore(closing));

            return IsValid(opening.Text, opening.End + 1, closing.Start - 1);
        }

        /// <summary>
        /// Returns true, if a specified text is valid AutoLink.
        /// </summary>
        internal static bool IsValid(string text, int startIndex, int endIndex)
        {
            return (IsValidUri(text, startIndex, endIndex) || IsValidEmail(text, startIndex, endIndex));
        }

        /// <summary>
        /// Returns true, if a specified string is valid URI.
        /// </summary>
        private static bool IsValidUri(string text, int startIndex, int endIndex)
        {
            Debug.Assert((startIndex >= 0) && (startIndex < text.Length));
            Debug.Assert((endIndex >= 0) && (endIndex < text.Length));

            int length = endIndex - startIndex + 1;

            int schemeSeparatorIdx = text.IndexOf(Colon, startIndex, length);
            // There must be a scheme separator.
            if (schemeSeparatorIdx == -1)
                return false;

            if (!IsValidScheme(text, startIndex, schemeSeparatorIdx - 1))
                return false;

            // Check URI characters.
            for (int i = schemeSeparatorIdx + 1; i <= endIndex; i++)
            {
                char c = text[i];
                if (MarkdownUtil.IsAsciiControlChar(c))
                    return false;

                if (c == MarkdownUtil.SpaceChar)
                    return false;

                if (c == MarkdownUtil.SoftLineBreakChar)
                    return false;

                if ((c == '<') || (c == '>'))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true, if a specified text is valid email address.
        /// </summary>
        private static bool IsValidEmail(string text, int startIndex, int endIndex)
        {
            Debug.Assert((startIndex >= 0) && (startIndex < text.Length));
            Debug.Assert((endIndex >= 0) && (endIndex < text.Length));

            int length = endIndex - startIndex + 1;

            // There must be at least @.
            if (length < 1)
                return false;

            char lastDomainChar = text[endIndex];
            // The domain cannot be empty.
            if ((lastDomainChar == CommercialAt) || (lastDomainChar == Dot))
                return false;

            // Find end of the local-part of the email address.
            int localPartEndIdx = text.IndexOf(CommercialAt, startIndex, length);
            if (localPartEndIdx == -1)
                return false;

            // Verify local-part of the email address.
            if (!IsValidEmailLocalPart(text, startIndex, localPartEndIdx - 1))
                return false;

            // Verify all domains of the email address.
            int domainStartIdx = localPartEndIdx + 1;
            while (domainStartIdx <= endIndex)
            {
                length = endIndex - domainStartIdx + 1;
                // The end is either position of the next dot or the position just after text length.
                int domainEndIdx = text.IndexOf(Dot, domainStartIdx, length);
                if (domainEndIdx == -1)
                    domainEndIdx = endIndex;
                else
                    domainEndIdx--;

                // Verify email domain.
                if (!IsValidEmailDomain(text, domainStartIdx, domainEndIdx))
                    return false;

                // Advance to the position just after the Dot separator.
                domainStartIdx = domainEndIdx + 2;
            }

            return true;
        }

        /// <summary>
        /// Returns true, if a specified string is valid scheme of the link.
        /// </summary>
        /// <remarks>
        /// ^[a-zA-Z][a-zA-Z0-9.+-]+
        /// </remarks>
        private static bool IsValidScheme(string text, int startIndex, int endIndex)
        {
            Debug.Assert((startIndex >= 0) && (startIndex < text.Length));
            Debug.Assert((endIndex >= 0) && (endIndex < text.Length));

            const int minSchemeLength = 2;
            const int maxSchemeLength = 32;

            int length = endIndex - startIndex + 1;

            // Check schema length is valid.
            if ((length < minSchemeLength) || (length > maxSchemeLength))
                return false;

            // The scheme must be started from a letter.
            if (!StringUtil.IsLetter(text[startIndex]))
                return false;

            for (int i = startIndex + 1; i <= endIndex; i++)
            {
                if (!StringUtil.IsLetterOrDigit(text[i]) &&
                    !ArrayUtil.FindCharInArray(gAllowedSpecialSchemeChars, text[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true, if a specified text is valid local-part of email address.
        /// </summary>
        private static bool IsValidEmailLocalPart(string text, int startIndex, int endIndex)
        {
            Debug.Assert((startIndex >= 0) && (startIndex < text.Length));
            Debug.Assert((endIndex >= 0) && (endIndex < text.Length));

            int length = endIndex - startIndex + 1;

            // Email local part can be empty.
            if (length < 1)
                return true;

            // The first character cannot be Dot.
            if (text[startIndex] == Dot)
                return false;

            for (int i = startIndex; i <= endIndex; i++)
            {
                char c = text[i];
                // These are only the allowed characters.
                if (!StringUtil.IsLetterOrDigit(c) && !ArrayUtil.FindCharInArray(gAllowedSpecialEmailChars, c))
                    return false;

                // Dots cannot appear consecutively.
                if ((c == Dot) && (text[i - 1] == Dot))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true, if a specified text is valid email domain.
        /// </summary>
        /// <remarks>
        /// It should be limited to a length of 63 characters and consisting of:
        /// - uppercase and lowercase Latin letters A to Z and a to z;
        /// - digits 0 to 9, provided that top-level domain names are not all-numeric;
        /// - hyphen -, provided that it is not the first or last character.
        /// This is so-called LDH rule (letters, digits, hyphen).
        /// </remarks>
        private static bool IsValidEmailDomain(string text, int startIndex, int endIndex)
        {
            Debug.Assert((startIndex >= 0) && (startIndex < text.Length));
            Debug.Assert((endIndex >= 0) && (endIndex < text.Length));

            const int minDomainLength = 1;
            const int maxDomainLength = 63;

            int length = endIndex - startIndex + 1;
            if ((length < minDomainLength) || (length > maxDomainLength))
                return false;

            // The first character must be letter or digit.
            if (!StringUtil.IsLetterOrDigit(text[startIndex]))
                return false;

            // The very last character cannot be a hyphen.
            if (text[endIndex] == Hyphen)
                return false;

            // The inner characters must be letters, digits or hyphen.
            for (int i = startIndex + 1; i <= endIndex; i++)
            {
                if (!StringUtil.IsLetterOrDigit(text[i]) && (text[i] != Hyphen))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// A type of the block.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.Autolink; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Inline; }
        }

        /// <summary>
        /// Gets text of the block.
        /// </summary>
        internal override string Text
        {
            get { return string.Format("{0}{1}{2}", OpeningDelimiter, base.Text, ClosingDelimiter); }
        }

        /// <summary>
        /// Gets a string value representing URL of the autolink.
        /// </summary>
        private string Url
        {
            get { return IsEmail ? string.Format("mailto:{0}", base.Text) : base.Text; }
        }

        /// <summary>
        /// Gets a boolean value indicating this is email autolink.
        /// </summary>
        private bool IsEmail
        {
            get
            {
                bool hasCommercialAt = false;
                foreach (char c in base.Text)
                {
                    switch (c)
                    {
                        case CommercialAt:
                            hasCommercialAt = true;
                            break;
                        case Colon:
                            return false;
                        default:
                            break;
                    }
                }
                return hasCommercialAt;
            }
        }

        /// <summary>
        /// Opening delimiter for AutolinkInlineBlock.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char OpeningDelimiter = AutoLinkOpeningDelimiter.Character;

        /// <summary>
        /// Closing delimiter for AutolinkInlineBlock.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char ClosingDelimiter = AutoLinkClosingDelimiter.Character;

        /// <summary>
        /// Allowed email address special characters.
        /// </summary>
        private static readonly char[] gAllowedSpecialEmailChars = {'.', '+', '-', '!', '#', '$', '%', '&', '\'', '*',
            '/', '=', '?', '^', '_', '`', '{' , '|', '}', '~' };

        /// <summary>
        /// Allowed URI scheme special characters.
        /// </summary>
        private static readonly char[] gAllowedSpecialSchemeChars = {'.', '+', '-'};

        /// <summary>Commercial "At" character.</summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char CommercialAt = '@';
        /// <summary>Colon character.</summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char Colon = ':';
        /// <summary>Dot character.</summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char Dot = '.';
        /// <summary>Hyphen character.</summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char Hyphen = '-';
    }
}
