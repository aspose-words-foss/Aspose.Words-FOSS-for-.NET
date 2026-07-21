// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/05/2012 by Alexey Butalov

using System;

namespace Aspose.Words.RW.Txt.Reader
{
    /// <summary>
    /// Contains numbering or bullet info parsed by <see cref="TxtNumberingDetector" />
    /// </summary>
    internal class TxtNumbering
    {
        #region Constructors

        internal TxtNumbering(TxtNumberingStyle numberingStyle, string[] numbers, string numberingText)
        {
            if (numberingStyle == null) 
                throw new ArgumentNullException("numberingStyle");
            if (numbers == null)
                throw new ArgumentNullException("numbers");
            if (numberingText == null) 
                throw new ArgumentNullException("numberingText");
            mNumberingStyle = numberingStyle;
            mNumbers = numbers;
            mText = numberingText;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns or sets the position (in chars) of the number or bullet in the text line
        /// </summary>
        internal int NumberPosition
        {
            get { return mNumberPosition; }
            set { mNumberPosition = value; }
        }

        /// <summary>
        /// Previous numbering paragraph (e.g. 1.1 is previous for 1.2)
        /// </summary>
        internal TxtParagraph PrevNumberingParagraph
        {
            get { return mPrevNumberingParagraph; }
            set { mPrevNumberingParagraph = value; }
        }

        /// <summary>
        /// Parent numbering paragraph (e.g. 1.1 is parent for 1.1.1)
        /// </summary>
        internal TxtParagraph ParentNumberingParagraph
        {
            get { return mParentNumberingParagraph; }
            set { mParentNumberingParagraph = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Numbering or bullet style
        /// </summary>
        internal TxtNumberingStyle NumberingStyle
        {
            get { return mNumberingStyle; }
        }

        /// <summary>
        /// Numbering or bullet text
        /// </summary>
        internal string Text
        {
            get { return mText; }
        }

        /// <summary>
        /// Numbers array for multilevel (like 1.2.3.1) numbering
        /// </summary>
        internal string[] Numbers
        {
            get { return mNumbers; }
        }

        /// <summary>
        /// Zero-based number of numbering level
        /// </summary>
        internal int ListLevelNumber
        {
            get { return mNumberingStyle.IsBullet ? 0 : mNumbers.Length - 1; }
        }

        /// <summary>
        /// Checks the numbering is first child for another numbering
        /// </summary>
        internal bool IsFirstChildFor(TxtNumbering numbering)
        {
            if (!mNumberingStyle.IsLevelsSupported ||
                (mNumberingStyle != numbering.NumberingStyle) ||
                !mNumberingStyle.IsStartNumber(GetLastNumber()) ||
                (mNumbers.Length - 1 != numbering.Numbers.Length))
                return false;

            for (int i = 0; i < mNumbers.Length - 1; i++)
            {
                if (mNumbers[i] != numbering.Numbers[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks the numbering is previous sibling for another numbering
        /// </summary>
        internal bool IsPrevSiblingFor(TxtNumbering numbering)
        {
            if (mNumberingStyle != numbering.NumberingStyle)
                return false;

            if (mNumberingStyle.IsBullet)
                return true;

            if (mNumbers.Length != numbering.Numbers.Length)
                return false;

            for (int i = 0; i < mNumbers.Length - 1; i++)
            {
                if (mNumbers[i] != numbering.Numbers[i])
                    return false;
            }

            return mNumberingStyle.GetNextNumber(GetLastNumber()) == numbering.GetLastNumber();
        }

        /// <summary>
        /// Returns true if the numbering can start a new list.
        /// </summary>
        internal bool IsStartNumbering()
        {
            return mNumberingStyle.IsBullet ||
                   ((mNumbers.Length == 1) && mNumberingStyle.IsStartNumber(mNumbers[0]));
        }

        /// <summary>
        /// Returns last number for multilevel numbering
        /// </summary>
        internal string GetLastNumber()
        {
            return mNumbers[mNumbers.Length - 1];
        }

        #endregion

        #region Fields

        private readonly TxtNumberingStyle mNumberingStyle;
        private readonly string mText;
        private readonly string[] mNumbers;
        private int mNumberPosition;
        private TxtParagraph mParentNumberingParagraph;
        private TxtParagraph mPrevNumberingParagraph;

        #endregion
    }
}