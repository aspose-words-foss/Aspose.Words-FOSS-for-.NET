// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/03/2011 by Alexey Titov

using System.Drawing;

namespace Aspose.Words.Drawing.Core.Dml.Fills
{
    /// <summary>
    /// Represents a rectangle defined by percentage offsets of its sides.
    /// </summary>
    internal class DmlPercentageOffsetRectangle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DmlPercentageOffsetRectangle" /> class.
        /// </summary>
        internal DmlPercentageOffsetRectangle()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DmlPercentageOffsetRectangle" /> class.
        /// </summary>
        internal DmlPercentageOffsetRectangle(double leftOffset, double topOffset, double rightOffset, double bottomOffset)
        {
            mLeftOffset = leftOffset;
            mTopOffset = topOffset;
            mRightOffset = rightOffset;
            mBottomOffset = bottomOffset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DmlPercentageOffsetRectangle" /> class.
        /// </summary>
        internal DmlPercentageOffsetRectangle(DmlPercentageOffsetRectangle other)
            : this(other.LeftOffset, other.TopOffset, other.RightOffset, other.BottomOffset)
        {
        }

        public bool Equals(DmlPercentageOffsetRectangle other)
        {
            if (ReferenceEquals(this, other))
                return true;

            return MathUtil.AreEqual(mLeftOffset, other.LeftOffset) &&
                   MathUtil.AreEqual(mTopOffset, other.TopOffset) &&
                   MathUtil.AreEqual(mRightOffset, other.RightOffset) &&
                   MathUtil.AreEqual(mBottomOffset, other.BottomOffset);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof (DmlPercentageOffsetRectangle))
                return false;
            return Equals((DmlPercentageOffsetRectangle)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = mBottomOffset.GetHashCode();
                result = (result*397) ^ mLeftOffset.GetHashCode();
                result = (result*397) ^ mRightOffset.GetHashCode();
                result = (result*397) ^ mTopOffset.GetHashCode();
                return result;
            }
        }

        public DmlPercentageOffsetRectangle Clone()
        {
            DmlPercentageOffsetRectangle result = new DmlPercentageOffsetRectangle();
            result.TopOffset = TopOffset;
            result.BottomOffset = BottomOffset;
            result.LeftOffset = LeftOffset;
            result.RightOffset = RightOffset;
            return result;
        }

        internal RectangleF Adjust(RectangleF rect)
        {
            if (IsEmpty)
                return rect;

            float w = rect.Width;
            float h = rect.Height;
            float l = (float)(rect.Left + w * LeftOffset);
            float r = (float)(rect.Right - w * RightOffset);
            float t = (float)(rect.Top + h * TopOffset);
            float b = (float)(rect.Bottom - h * BottomOffset);
            return RectangleF.FromLTRB(l, t, r, b);
        }

        /// <summary>
        /// Specifies the bottom edge of the rectangle.
        /// </summary>
        internal double BottomOffset
        {
            get { return mBottomOffset; }
            set { mBottomOffset = value; }
        }

        /// <summary>
        /// Specifies the top edge of the rectangle.
        /// </summary>
        internal double TopOffset
        {
            get { return mTopOffset; }
            set { mTopOffset = value; }
        }

        /// <summary>
        /// Specifies the left edge of the rectangle.
        /// </summary>
        internal double LeftOffset
        {
            get { return mLeftOffset; }
            set { mLeftOffset = value; }
        }

        /// <summary>
        /// Specifies the right edge of the rectangle.
        /// </summary>
        internal double RightOffset
        {
            get { return mRightOffset; }
            set { mRightOffset = value; }
        }

        internal bool IsEmpty
        {
            get
            {
                return MathUtil.IsZero(BottomOffset)
                       && MathUtil.IsZero(TopOffset)
                       && MathUtil.IsZero(LeftOffset)
                       && MathUtil.IsZero(RightOffset);
            }
        }

        private double mBottomOffset;
        private double mLeftOffset;
        private double mRightOffset;
        private double mTopOffset;
    }
}
