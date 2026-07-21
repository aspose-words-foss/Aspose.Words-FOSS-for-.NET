// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/05/2012 by Alexey Butalov

using Aspose.Bidi;
using Aspose.Charset;
using Aspose.Common;
using Aspose.Words.RW.Factories;

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// <para>The class is helper for <see cref="FileFormatDetector"/></para>
    /// <para>The class confirms or rejects an assumption that a provided stream is Plain Text</para>
    /// </summary>
    internal class TxtFormatDetector
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        private TxtFormatDetector(CustomTextReader textReader)
        {
            Debug.Assert(textReader != null);
            mTextReader = textReader;
        }

        /// <summary>
        /// Returns <see cref="FileFormatInfo"/> if Plain Text assumption has been confirmed, or null if rejected.
        /// </summary>
        internal static FileFormatInfo Detect(CustomTextReader streamReader)
        {
            return new TxtFormatDetector(streamReader).Detect();
        }

        /// <summary>
        /// Checks if the char is allowed in text
        /// </summary>
        internal static bool IsAllowedControlChar(char c, int codePage)
        {
            switch (c)
            {
                case ZeroChar:
                case ControlChar.TabChar:
                case ControlChar.LineFeedChar:
                case ControlChar.ParagraphBreakChar:
                case NextLineChar:
                case ControlChar.PageBreakChar:
                case ControlChar.CellChar:
                case ControlChar.LineBreakChar:
                    return true;
                // WORDSNET-23607 Allow 0x80 control char for Shift JIS encoding.
                case (char)0x80:
                    return (codePage == CodePage.WindowsJapaneseShiftJis);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the char is Unicode control character ISO 6429
        /// </summary>
        internal static bool IsControlChar(char c)
        {
            return (c <= 0x1F) || (c == 0x7F) || ((c >= 0x80) && (c <= 0x9F));
        }

        /// <summary>
        /// Checks if the char is zero.
        /// </summary>
        private static bool IsZeroChar(char c)
        {
            return (c == ZeroChar);
        }

        private static bool IsNextLineChar(char c)
        {
            return (c == ControlChar.LineFeedChar) || (c == NextLineChar);
        }

        private static bool IsWordDelimeterChar(char ch)
        {
            return (char.IsWhiteSpace(ch)) || (IsNextLineChar(ch)) || (ch == ',') || (ch == ';') || (ch == '|') ||
                (ch == ControlChar.PageBreakChar) || (ch == ControlChar.CellChar);
        }

        private FileFormatInfo Detect()
        {
            int totalCharsCount = 0;
            int linesCount = 0;
            int wordsCount = 0;
            int wordsLength = 0;
            int letterAndDigitCount = 0;
            int currentWordLength = 0;
            int nonPrintableCharCount = 0;
            int maxSequenceZeroCount = 0;
            int currentSequenceZeroCount = 0;
            bool hasRtlScript = false;

            while (mTextReader.HasChars)
            {
                char ch = mTextReader.ReadChar();
                if (CharacterClassifier.IsRtlScript(ch))
                    hasRtlScript = true;

                if (IsControlChar(ch) && !IsAllowedControlChar(ch, mTextReader.Encoding.CodePage))
                    nonPrintableCharCount++;

                if (IsZeroChar(ch))
                {
                    currentSequenceZeroCount++;
                    if (currentSequenceZeroCount > maxSequenceZeroCount)
                        maxSequenceZeroCount = currentSequenceZeroCount;
                }
                else
                    currentSequenceZeroCount = 0;

                totalCharsCount++;
                if (IsWordDelimeterChar(ch))
                {
                    if (currentWordLength != 0)
                    {
                        wordsCount++;
                        wordsLength += currentWordLength;
                        currentWordLength = 0;
                    }
                    if (IsNextLineChar(ch))
                        linesCount++;
                }
                else
                {
                    currentWordLength++;
                    if (char.IsLetterOrDigit(ch))
                        letterAndDigitCount++;
                }
            }

            if (currentWordLength != 0)
            {
                wordsCount++;
                wordsLength += currentWordLength;
            }

            int txtConfidence = 0;

            // checks letters ratio
            const int lettersRatioWeight = 2;
            const double legalLettersRatio = 0.3;
            if ((totalCharsCount > 0) && ((double)letterAndDigitCount / totalCharsCount >= legalLettersRatio))
            {
                // WORDSNET-7000 UnsupportedFileFormatException occurs when loading a TEXT file into DOM.
                // The problem occurs because of too strong criteria for text files detecting.
                // This criterion is weightier now and all criteria are weaker.
                txtConfidence += lettersRatioWeight;
            }

            // checks average line length
            const int legalLineMaxLength = 200;
            if ((linesCount > 0) && (totalCharsCount / linesCount < legalLineMaxLength))
            {
                txtConfidence++;
            }

            // the longest English word, that appears in a major dictionary, consist of 45 characters
            // Let's use this criterion and round it to 50.
            const int legalWordMaxLength = 50;
            // checks average word size
            if (wordsCount > 0)
            {
                int avgWordLength = (wordsLength / wordsCount);
                if (avgWordLength < legalWordMaxLength)
                {
                    txtConfidence++;
                }
                else
                {
                    // WORDSNET-26228 Decrease confidence on bad words length.
                    // WORDSNET-26255 Introduce weight for long strings.
                    txtConfidence -= avgWordLength/legalWordMaxLength;
                }
            }
            else
            {
                txtConfidence--;
            }

            // Maximum number of sequence zero character.
            const int legalSequenceZeroCount = 2;
            if (maxSequenceZeroCount > legalSequenceZeroCount)
                txtConfidence -= 2;

            // WORDSNET-7574 Incorrect encoding is used while loading TXT file.
            // Allow several non-printable characters in a text. (Let's believe that they are erroneous characters).
            // WORDSNET-13777 Wrong detection of text file that contains non-printable characters.
            const int nonPrintableCharMaxCount = 2;
            if (nonPrintableCharCount > nonPrintableCharMaxCount)
                txtConfidence -= 100 * nonPrintableCharCount / totalCharsCount;
            else
                txtConfidence++;

            if (txtConfidence >= 2)
            {
                FileFormatInfo fileFormatInfo = new FileFormatInfo();
                fileFormatInfo.SetLoadFormat(LoadFormat.Text);
                fileFormatInfo.SetEncoding(mTextReader.Encoding);
                fileFormatInfo.HasRtlScript = hasRtlScript;
                return fileFormatInfo;
            }

            return null;
        }

        private readonly CustomTextReader mTextReader;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char NextLineChar = (char)0x85;
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char ZeroChar = (char)0x00;
    }
}
