// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/01/2011 by Alexey Titov

using System;
using System.Drawing.Drawing2D;
using Aspose.Drawing;

namespace Aspose.Words.Drawing.Core.Dml.Outlines
{
    /// <summary>
    /// This class specifies decorations which can be added to the tail or the head of a line.
    /// </summary>
    internal abstract class DmlLineEndStyle
    {
        public abstract DmlLineEndStyle Clone();
        protected void CopyTo(DmlLineEndStyle target)
        {
            target.Length = Length;
            target.Width = Width;
            target.Type = Type;
        }

        public override bool Equals(object obj)
        {
            // Same instance.
            if (obj == this)
                return true;

            // Type or hashcode does not match.
            if (!ArgumentUtil.TypeAndHashCodeMatches(this, obj))
                return false;

            DmlLineEndStyle value = (DmlLineEndStyle)obj;

            return (value.Type == Type) && (value.Length == Length) && (value.Width == Width);
        }

        public override int GetHashCode()
        {
            int hash = Type.GetHashCode();
            hash ^= Length.GetHashCode();
            hash ^= Width.GetHashCode();
            return hash;
        }

        /// <summary>
        /// Specifies the line end length in relation to the line width.
        /// </summary>
        internal ArrowLength Length
        {
            get { return mLength; }
            set { mLength = value; }
        }

        /// <summary>
        /// Specifies the line end width in relation to the line width.
        /// </summary>
        internal ArrowWidth Width
        {
            get { return mWidth; }
            set { mWidth = value; }
        }

        /// <summary>
        /// Specifies the line end decoration,
        /// such as a triangle or arrowhead.
        /// </summary>
        internal ArrowType Type
        {
            get { return mType; }
            set { mType = value; }
        }

        private ArrowLength mLength = ArrowLength.Medium;
        private ArrowType mType = ArrowType.None;
        private ArrowWidth mWidth = ArrowWidth.Medium;
    }
}
