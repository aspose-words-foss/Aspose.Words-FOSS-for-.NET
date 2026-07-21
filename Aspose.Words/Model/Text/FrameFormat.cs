// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/12/2016 by Alexey Morozov
using Aspose.Words.Drawing;

namespace Aspose.Words
{
    /// <summary>
    /// Represents frame related formatting for a paragraph.
    /// </summary>
    /// <remarks>
    /// <p>This object is always created. If a paragraph is a frame, then all properties will contain respective values, otherwise
    /// all properties are set to their defaults.</p>
    /// <p>Use <see cref="IsFrame" /> to check whether paragraph is a frame.</p>
    /// </remarks>
    public class FrameFormat
    {
        internal FrameFormat(IParaAttrSource parent)
        {
            mParent = parent;
        }

        /// <summary>
        /// Gets the rule for determining the height of the specified frame.
        /// </summary>
        public HeightRule HeightRule
        {
            get
            {
                Height height = (Height)mParent.FetchParaAttr(ParaAttr.FrameHeight);
                return height.Rule;
            }
        }

        /// <summary>
        /// Gets the height of the specified frame.
        /// </summary>
        public double Height
        {
            get
            {
                Height height = (Height)mParent.FetchParaAttr(ParaAttr.FrameHeight);
                return ConvertUtilCore.TwipToPoint(height.Value);
            }
        }

        /// <summary>
        /// Gets horizontal distance between a frame and the surrounding text, in points.
        /// </summary>
        public double HorizontalDistanceFromText
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.FrameHorizontalDistanceFromText)); }
        }

        /// <summary>
        /// Gets horizontal distance between the edge of the frame and the item specified by the <see cref="RelativeHorizontalPosition" /> property.
        /// </summary>
        public double HorizontalPosition
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.FrameLeft)); }
        }

        /// <summary>
        /// Gets the relative horizontal position of a frame.
        /// </summary>
        public RelativeHorizontalPosition RelativeHorizontalPosition
        {
            get { return (RelativeHorizontalPosition)mParent.FetchParaAttr(ParaAttr.FrameRelativeHorizontalPosition); }
        }

        /// <summary>
        /// Gets the relative vertical position of a frame.
        /// </summary>
        public RelativeVerticalPosition RelativeVerticalPosition
        {
            get { return (RelativeVerticalPosition)mParent.FetchParaAttr(ParaAttr.FrameRelativeVerticalPosition); }
        }

        /// <summary>
        /// Specifies vertical distance (in points) between a frame and the surrounding text.
        /// </summary>
        public double VerticalDistanceFromText
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.FrameVerticalDistanceFromText)); }
        }

        /// <summary>
        /// Gets vertical distance between the edge of the frame and the item specified by the <see cref="RelativeVerticalPosition" /> property.
        /// </summary>
        public double VerticalPosition
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.FrameTop)); }
        }

        /// <summary>
        /// Gets the width of the specified frame, in points.
        /// </summary>
        public double Width
        {
            get { return ConvertUtilCore.TwipToPoint((int)mParent.FetchParaAttr(ParaAttr.FrameWidth)); }
        }

        /// <summary>
        /// Gets vertical alignment of the specified frame.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)mParent.FetchParaAttr(ParaAttr.FrameVerticalAlignment); }
        }

        /// <summary>
        /// Gets horizontal alignment of the specified frame.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)mParent.FetchParaAttr(ParaAttr.FrameHorizontalAlignment); }
        }

        /// <summary>
        /// Returns <c>true</c> if the paragraph is a frame.
        /// </summary>
        public bool IsFrame
        {
            get { return IsFloatingCore(mParent); }
        }

        internal static bool IsFloatingCore(IParaAttrSource attrs)
        {
            if (HasHorizontalRule(attrs) || HasVerticalRule(attrs))
                return true;

            Height height = (Height)attrs.FetchParaAttr(ParaAttr.FrameHeight);
            if (height.Value > 0)
                return true;

            int frameWidth = (int)attrs.FetchParaAttr(ParaAttr.FrameWidth);
            if (frameWidth > 0)
                return true;

            // Also it seems that FrameWrapType other than Inline makes floater as well.
            // WORDSNET-6396 Always treat drop capitals as frames.
            WrapType wrapType = (WrapType)attrs.FetchParaAttr(ParaAttr.FrameWrapType);
            DropCapPosition dropCapPosition = (DropCapPosition)attrs.FetchParaAttr(ParaAttr.DropCapPosition);

            return !((wrapType == WrapType.Inline) && (dropCapPosition == DropCapPosition.None));
        }

        private static bool HasHorizontalRule(IParaAttrSource attrs)
        {
            int frameLeft = (int) attrs.FetchParaAttr(ParaAttr.FrameLeft);
            if (frameLeft > 0)
                return true;

            HorizontalAlignment horizontalAlignment =
                (HorizontalAlignment) attrs.FetchParaAttr(ParaAttr.FrameHorizontalAlignment);
                // According to tests these both values are defaults.
                if ((horizontalAlignment != HorizontalAlignment.None) &&
                    (horizontalAlignment != HorizontalAlignment.Left))
                    return true;

            RelativeHorizontalPosition relativeHorizontalPosition = 
                (RelativeHorizontalPosition) attrs.FetchParaAttr(ParaAttr.FrameRelativeHorizontalPosition);

            return relativeHorizontalPosition != RelativeHorizontalPosition.Column;
        }

        private static bool HasVerticalRule(IParaAttrSource attrs)
        {
            int frameTop = (int) attrs.FetchParaAttr(ParaAttr.FrameTop);
            if (frameTop > 0)
                return true;

            VerticalAlignment verticalAlignment =
                (VerticalAlignment) attrs.FetchParaAttr(ParaAttr.FrameVerticalAlignment);

            // According to tests these both values are defaults.
            if ((verticalAlignment != VerticalAlignment.None) &&
                (verticalAlignment != VerticalAlignment.Inline))
                    return true;

            RelativeVerticalPosition relativeVerticalPosition = 
                (RelativeVerticalPosition) attrs.FetchParaAttr(ParaAttr.FrameRelativeVerticalPosition);

            // According to tests these both values are default.
            if ((relativeVerticalPosition != RelativeVerticalPosition.Margin) &&
                (relativeVerticalPosition != RelativeVerticalPosition.Page))
                    return true;

            return false;
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IParaAttrSource mParent;
    }
}
