// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2018 by Alexander Zhiltsov

using Aspose.Words.Settings;

namespace Aspose.Words
{
    /// <summary>
    /// Represents document page properties that are required to calculate page margin and content size.
    /// </summary>
    internal class PageProperties
    {
        /// <summary>
        /// Gets or sets the left margin of the page.
        /// </summary>
        internal int LeftMargin
        {
            get { return mLeftMargin; }
            set { mLeftMargin = value; }
        }

        /// <summary>
        /// Gets or sets the right margin of the page.
        /// </summary>
        internal int RightMargin
        {
            get { return mRightMargin; }
            set { mRightMargin = value; }
        }

        /// <summary>
        /// Gets or sets the top margin of the page.
        /// </summary>
        internal int TopMargin
        {
            get { return mTopMargin; }
            set { mTopMargin = value; }
        }

        /// <summary>
        /// Gets or sets the bottom margin of the page.
        /// </summary>
        internal int BottomMargin
        {
            get { return mBottomMargin; }
            set { mBottomMargin = value; }
        }

        /// <summary>
        /// Gets or sets the amount of extra space added to the margin for document binding.
        /// </summary>
        internal int Gutter
        {
            get { return mGutter; }
            set { mGutter = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether the document has the <see cref="DocPr.GutterAtTop"/>
        /// property set or not.
        /// </summary>
        internal bool IsGutterAtTopSet
        {
            get { return mIsGutterAtTopSet; }
            set { mIsGutterAtTopSet = value; }
        }

        /// <summary>
        /// Gets or sets whether Microsoft Word uses gutters for the section based on a right-to-left language
        /// or a left-to-right language.
        /// </summary>
        internal bool IsRtlGutter
        {
            get { return mIsRtlGutter; }
            set { mIsRtlGutter = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating what kind of swapping left and right margins on facing pages
        /// for multiple page documents is used.
        /// </summary>
        internal MultiplePagesType MultiplePages
        {
            get { return mMultiplePages; }
            set { mMultiplePages = value; }
        }

        /// <summary>
        /// Gets or sets the orientation of the page.
        /// </summary>
        internal Orientation Orientation
        {
            get { return mOrientation; }
            set { mOrientation = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether this page has the same orientation as the first section.
        /// </summary>
        internal bool OrientationMatchesFirstSection
        {
            get { return mOrientationMatchesFirstSection; }
            set { mOrientationMatchesFirstSection = value; }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether odd and even pages have different headers and footers.
        /// </summary>
        internal bool DifferentOddEvenPageHeader
        {
            get { return mDifferentOddEvenPageHeader; }
            set { mDifferentOddEvenPageHeader = value; }
        }

        private int mLeftMargin;
        private int mRightMargin;
        private int mTopMargin;
        private int mBottomMargin;
        private int mGutter;
        private bool mIsGutterAtTopSet;
        private bool mIsRtlGutter;
        private MultiplePagesType mMultiplePages;
        private Orientation mOrientation = Orientation.Portrait;
        private bool mOrientationMatchesFirstSection = true;
        private bool mDifferentOddEvenPageHeader;
    }
}
