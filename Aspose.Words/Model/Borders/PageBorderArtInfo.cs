// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/07/2012 by Dmitry Bormashov

using System;

namespace Aspose.Words
{
    /// <summary>
    /// Contains information about border art.
    /// </summary>
    internal class PageBorderArtInfo
    {
        internal PageBorderArtInfo(int id, int contraction, int hExpansion, int vExpansion)
        {
            mId = id;
            mContraction = contraction;
            mHExpansion = hExpansion;
            mVExpansion = vExpansion;
            mTopElements = new byte[(int)PageBorderArtElementPosition.Last + 1][];
            mBottomElements = new byte[(int)PageBorderArtElementPosition.Last + 1][];
        }

        /// <summary>
        /// Border art id.
        /// </summary>
        internal int Id
        {
            get { return mId; }
        }

        /// <summary>
        /// Ratio shows expansion/contraction for middle elements in 
        /// horizontal part of border.
        /// </summary>
        internal float HorizontalExpansionContraction
        {
            get { return (float)mHExpansion / (float)mContraction; }
        }

        /// <summary>
        /// Ratio shows expansion/contraction for middle elements in 
        /// vertical part of border.
        /// </summary>
        internal float VerticalExpansionContraction
        {
            get { return (float)mVExpansion / (float)mContraction; }
        }

        /// <summary>
        /// Sets border art element.
        /// </summary>
        /// <param name="borderType">Border type.</param>
        /// <param name="elementPosition">Element position.</param>
        /// <param name="elementBytes">Element.</param>
        internal void SetElement(
            BorderType borderType,
            PageBorderArtElementPosition elementPosition,
            byte[] elementBytes)
        {
            switch (borderType)
            {
                case BorderType.Top:
                    mTopElements[(int)elementPosition] = elementBytes;
                    break;
                case BorderType.Bottom:
                    mBottomElements[(int)elementPosition] = elementBytes;
                    break;
                case BorderType.Left:
                    mLeftElement = elementBytes;
                    break;
                case BorderType.Right:
                    mRightElement = elementBytes;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("borderType");
            }
        }

        /// <summary>
        /// Gets border art element.
        /// </summary>
        /// <param name="borderType">Border type.</param>
        /// <param name="elementPosition">Element position.</param>
        /// <returns>Border art element.</returns>
        internal byte[] GetElement(BorderType borderType, PageBorderArtElementPosition elementPosition)
        {
            switch (borderType)
            {
                case BorderType.Top:
                    return mTopElements[(int)elementPosition];
                case BorderType.Bottom:
                    return mBottomElements[(int)elementPosition];
                case BorderType.Left:
                    return mLeftElement;
                case BorderType.Right:
                    return mRightElement;
                default:
                    return new byte[0];
            }
        }

        private readonly int mId;
        private readonly int mContraction;
        private readonly int mHExpansion;
        private readonly int mVExpansion;
        private readonly byte[][] mTopElements;
        private byte[] mRightElement;
        private readonly byte[][] mBottomElements;
        private byte[] mLeftElement;
    }
}
