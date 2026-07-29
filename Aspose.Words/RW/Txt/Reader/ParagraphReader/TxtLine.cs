// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/05/2012 by Alexey Butalov

using System;

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Text line's structure
    /// </summary>
    internal class TxtLine
    {
        #region Contructors

        internal TxtLine(TxtLineType lineType)
            : this("", 0, lineType)
        {
        }

        internal TxtLine(string text, int leftIndent, TxtLineType lineType)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            mLineType = lineType;
            mText = text;
            mLeftIndent = leftIndent;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Line's text
        /// </summary>
        internal string Text
        {
            get { return mText; }
        }

        /// <summary>
        /// Line's left indent size (in characters)
        /// </summary>
        internal int LeftIndent
        {
            get { return mLeftIndent; }
        }

        /// <summary>
        /// Line's type
        /// </summary>
        internal TxtLineType LineType
        {
            get { return mLineType; }
            set { mLineType = value; }
        }

        /// <summary>
        /// Returns true if FormFeed (0xC) character is terminating this line, meaning that we'll
        /// need to insert new Section after this line.
        /// </summary>
        internal bool IsNewSectionRequested
        {
            get { return mIsFormFeed; }
            set { mIsFormFeed = value;  }
        }

        /// <summary>
        /// Line's numbering
        /// </summary>
        internal TxtNumbering Numbering
        {
            get { return mNumbering; }
            set { mNumbering = value; }
        }

        #endregion

        #region Fields

        private readonly string mText;
        private readonly int mLeftIndent;
        private TxtLineType mLineType;
        private TxtNumbering mNumbering;
        private bool mIsFormFeed;

        #endregion
    }
}