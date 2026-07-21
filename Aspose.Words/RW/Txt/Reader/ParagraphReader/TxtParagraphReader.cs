// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/05/2012 by Alexey Butalov

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Words.Loading;

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Reads paragraphs from text file
    /// </summary>
    internal class TxtParagraphReader
    {
        #region Constructors

        internal TxtParagraphReader(StreamReader streamReader, TxtLoadOptions loadOptions)
        {
            if (streamReader == null)
                throw new ArgumentNullException("streamReader");

            mStreamReader = streamReader;
            mLoadOptions = loadOptions;
        }

        internal TxtParagraphReader(StreamReader streamReader) : this (streamReader, new TxtLoadOptions())
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads paragraphs.
        /// </summary>
        internal TxtParagraph[] Read()
        {
            return Read(null);
        }

        /// <summary>
        /// Reads paragraphs.
        /// </summary>
        /// <returns>Array of paragraphs</returns>
        internal TxtParagraph[] Read(LoadingProgressProcessor progressProcessor)
        {
            mParagraphs = new List<TxtParagraph>();
            TxtLine txtLine = ReadLine(progressProcessor);
            while (txtLine.LineType != TxtLineType.Eof)
            {
                switch (txtLine.LineType)
                {
                    case TxtLineType.TextLine:
                    case TxtLineType.DelimiterLine:
                    case TxtLineType.EmptyLine:
                    case TxtLineType.NumberingLine:
                        CreateNewParagraph(txtLine);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                txtLine = ReadLine(progressProcessor);
            }

            TxtParagraph[] paragraphs = new TxtParagraph[mParagraphs.Count];
            for (int i = 0; i < mParagraphs.Count; i++)
                paragraphs[i] = mParagraphs[i];
            return paragraphs;
        }

        /// <summary>
        /// Reads one text line from the source
        /// </summary>
        private TxtLine ReadLine(LoadingProgressProcessor progressProcessor)
        {
            TxtTrailingSpacesOptions trailingSpacesOption = mLoadOptions.TrailingSpacesOptions;
            TxtLeadingSpacesOptions leadingSpacesOption = mLoadOptions.LeadingSpacesOptions;

            string textLine = mStreamReader.ReadLine();

            // WORDSNET-22891 Implemented TXT loading progress notification.
            if (progressProcessor != null)
                progressProcessor.Execute(mStreamReader.BaseStream);

            if (textLine == null)
                return new TxtLine(TxtLineType.Eof);

            if (trailingSpacesOption == TxtTrailingSpacesOptions.Trim)
                textLine = textLine.TrimEnd(' ');

            if (leadingSpacesOption == TxtLeadingSpacesOptions.Trim)
                textLine = textLine.TrimStart(' ');

            string text = textLine.Trim(' ');
            if (text == "")
            {
                TxtLine newLine = new TxtLine(TxtLineType.EmptyLine);
                ProcessFormFeedCharacter(textLine, newLine);
                return newLine;
            }


            TxtLineType lineType = TxtLineType.TextLine;
            TxtNumbering numbering = null;
            if (IsDelimiterLine(text))
                lineType = TxtLineType.DelimiterLine;
            int leftIndent = GetLeftIndent(textLine);

            if ((leadingSpacesOption == TxtLeadingSpacesOptions.Preserve) && (leftIndent > 0))
            {
                // In this case we are forced to preserve leading spaces, which will not be converted to
                // LeftIndent, and DetectNumbering will not be performed. Just leave this textLine untouched.
                text = textLine;
                leftIndent = 0;
            }
            else
            {
                text = textLine.TrimStart(' ');
                if (lineType != TxtLineType.DelimiterLine)
                {
                    numbering = DetectNumbering(text, leftIndent);
                    if (numbering != null)
                    {
                        lineType = TxtLineType.NumberingLine;
                        // remove numbers from text
                        int textPos = numbering.Text.Length;
                        while ((textPos < text.Length) && (text[textPos] == ' '))
                            textPos++;
                        text = text.Remove(0, textPos);
                        leftIndent += textPos;
                    }
                }
            }

            TxtLine txtLine = new TxtLine(text, leftIndent, lineType);
            ProcessFormFeedCharacter(textLine, txtLine);

            txtLine.Numbering = numbering;
            return txtLine;
        }

        /// <summary>
        /// If form feed is at the end of the line, we'll set new section flag.
        /// </summary>
        private static void ProcessFormFeedCharacter(string textLine, TxtLine txtLine)
        {
            if ((textLine.Length != 0) && (textLine[textLine.Length - 1] == (char)0xC))
                txtLine.IsNewSectionRequested = true;
        }

        /// <summary>
        /// Determines left indent of the line.
        /// </summary>
        /// <returns>Returns left indent in chars</returns>
        private static int GetLeftIndent(string line)
        {
            int leftIndent = 0;
            int index = 0;
            bool whiteChar = true;
            while (whiteChar && (index < line.Length))
            {
                if (line[index] == ' ')
                    leftIndent++;
                else if (line[index] == '\t')
                    leftIndent += TabSize;
                else
                    whiteChar = false;
                index++;
            }
            return leftIndent;
        }

        /// <summary>
        /// Finds a previous paragraph with numbering (a. is previous for b., 1.2. is previous for 1.3.)
        /// </summary>
        private TxtParagraph FindPreviousNumberingParagraph(TxtNumbering numbering, TxtParagraph parentParagraph)
        {
            // WORDSNET-28110 Cached numbered paragraphs.
            List<TxtParagraph> numberedParagraphs;
            if (!mNumberedParagraphs.TryGetValue(numbering.NumberingStyle, out numberedParagraphs))
                return null;

            TxtParagraph prevParagraph = null;
            for (int i = numberedParagraphs.Count - 1; i >= 0; i--)
            {
                TxtParagraph p = numberedParagraphs[i];

                // WORDSNET-10456 Txt to Pdf/Doc/Docx/Txt convesion issue with number text
                // For nested numbering case: there is no need to move higher than parent paragraph
                if (parentParagraph == p)
                    break;

                if ((p.Numbering != null) && (p.Numbering.IsPrevSiblingFor(numbering)))
                {
                    if (p.Numbering.NumberingStyle.IsBullet && (p.Numbering.NumberPosition == numbering.NumberPosition))
                        return p;

                    if (p.Numbering.NumberingStyle.IsSet &&
                        ((prevParagraph == null) ||
                         (System.Math.Abs(p.Numbering.NumberPosition - numbering.NumberPosition) <
                          System.Math.Abs(prevParagraph.Numbering.NumberPosition - numbering.NumberPosition))))
                        prevParagraph = p;
                }

                // WORDSNET-10456 Txt to Pdf/Doc/Docx/Txt convesion issue with number text
                // For common numbering case: there is no need to move higher than start numbering
                if ((parentParagraph == null) && (p.Numbering != null) && (p.Numbering.ParentNumberingParagraph == null) &&
                    p.Numbering.IsStartNumbering())
                    break;
            }

            return prevParagraph;
        }

        /// <summary>
        /// Finds a parent paragraph with numbering (1.2. is parent for 1.2.1.)
        /// </summary>
        private TxtParagraph FindParentNumberingParagraph(TxtNumbering numbering)
        {
            // WORDSNET-28110 Cached numbered paragraphs.
            List<TxtParagraph> numberedParagraphs;
            if (!mNumberedParagraphs.TryGetValue(numbering.NumberingStyle, out numberedParagraphs))
                return null;

            if (numberedParagraphs.Count == 0)
                return null;

            TxtParagraph lastNumberedParagraph = numberedParagraphs[numberedParagraphs.Count - 1];
            return (lastNumberedParagraph != null) && numbering.IsFirstChildFor(lastNumberedParagraph.Numbering)
                ? lastNumberedParagraph
                : null;
        }

        /// <summary>
        /// Determines whether the line is delimiter line like ******************* or ===================
        /// </summary>
        private static bool IsDelimiterLine(string textLine)
        {
            const int delimiterLineMinLength = 5;

            if (!((textLine.Length >= delimiterLineMinLength) &&
                  ((textLine[0] == '*') || (textLine[0] == '-') || (textLine[0] == '=') || (textLine[0] == '~'))))
                return false;

            foreach (char c in textLine)
                if (c != textLine[0])
                    return false;

            return true;
        }

        /// <summary>
        /// Creates and returns the new paragraph
        /// </summary>
        private void CreateNewParagraph(TxtLine txtLine)
        {
            TxtParagraph paragraph = new TxtParagraph();

            paragraph.FirstLineIndent = txtLine.LeftIndent;
            paragraph.AppendText(txtLine.Text);
            paragraph.Numbering = txtLine.Numbering;
            paragraph.IsNewSectionRequested = txtLine.IsNewSectionRequested;
            mParagraphs.Add(paragraph);

            // WORDSNET-28110 Keep numbered paragraphs in a Dictionary
            // to provide quick access to its properties.
            if (txtLine.Numbering != null)
            {
                List<TxtParagraph> numberedParagraphs;
                if (!mNumberedParagraphs.TryGetValue(txtLine.Numbering.NumberingStyle, out numberedParagraphs))
                {
                    numberedParagraphs = new List<TxtParagraph>();
                    mNumberedParagraphs.Add(txtLine.Numbering.NumberingStyle, numberedParagraphs);
                }
                numberedParagraphs.Add(paragraph);
            }
        }

        /// <summary>
        /// Detects whether the line contains a numbering part.
        /// </summary>
        private TxtNumbering DetectNumbering(string text, int leftIndent)
        {
            TxtNumbering numbering = TxtNumberingDetector.Detect(text, mLoadOptions);
            if (numbering == null)
                return null;

            numbering.NumberPosition = leftIndent;
            numbering.ParentNumberingParagraph = FindParentNumberingParagraph(numbering);
            numbering.PrevNumberingParagraph = FindPreviousNumberingParagraph(numbering, numbering.ParentNumberingParagraph);
            // A correct numbering should be sibling, first or child.
            if ((numbering.PrevNumberingParagraph == null) && !numbering.IsStartNumbering() &&
                (numbering.ParentNumberingParagraph == null))
                return null;

            return numbering;
        }

        #endregion

        #region Fields

        private readonly StreamReader mStreamReader;
        private List<TxtParagraph> mParagraphs;

        /// <summary>
        /// The dictionary to keep numbered paragraphs.
        /// Key: numbering style;
        /// Value: List of paragraphs with numbering style specified in Key.
        /// </summary>
        private readonly Dictionary<TxtNumberingStyle, List<TxtParagraph>> mNumberedParagraphs =
            new Dictionary<TxtNumberingStyle, List<TxtParagraph>>();

        #endregion

        /// <summary>
        /// Tab size is the amount of space each tab
        /// </summary>
        private const int TabSize = 4;

        private readonly TxtLoadOptions mLoadOptions;
    }
}
