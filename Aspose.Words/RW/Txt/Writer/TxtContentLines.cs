// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/04/2008 by Dmitry Vorobyev

using System.Collections.Generic;
using System.Text;

namespace Aspose.Words.RW.Txt.Writer
{
    /// <summary>
    /// Represents a collection of string builders each of which represents a content line in text-based formats.
    /// </summary>
    internal class TxtContentLines
    {
        /// <summary>
        /// Returns the current line. Adds lines if current line index is greater than the number of lines
        /// in the collection.
        /// </summary>
        internal StringBuilder CurrentLine
        {
            get
            {
                int linesToAdd = mCurrentLineIndex - mLines.Count + 1;

                for (int i = 0; i < linesToAdd; i++)
                    mLines.Add(new StringBuilder());

                return this[mCurrentLineIndex];
            }
        }

        /// <summary>
        /// The index of the current line.
        /// </summary>
        internal int CurrentLineIndex
        {
            get { return mCurrentLineIndex; }
            set { mCurrentLineIndex = value; }
        }

        /// <summary>
        /// Gets lines count.
        /// </summary>
        internal int Count
        {
            get { return mLines.Count; }
        }

        /// <summary>
        /// Gets a line at a specified index.
        /// </summary>
        internal StringBuilder this[int index]
        {
            get { return mLines[index]; }
        }

        /// <summary>
        /// Gets a length of the longest line.
        /// </summary>
        internal int MaxLength
        {
            get
            {
                int maxLength = 0;
                foreach (StringBuilder line in mLines)
                    maxLength = System.Math.Max(maxLength, line.Length);

                return maxLength;
            }
        }

        /// <summary>
        /// Removes all lines.
        /// </summary>
        internal void Clear()
        {
            mCurrentLineIndex = 0;
            mLines.Clear();
        }

        /// <summary>
        /// Adds a new line.
        /// </summary>
        internal void NewLine()
        {
            mCurrentLineIndex++;
        }

        private int mCurrentLineIndex;   
        private readonly List<StringBuilder> mLines = new List<StringBuilder>();
    }
}
