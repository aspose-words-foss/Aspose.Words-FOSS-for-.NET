// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/08/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Represents a rectangle whose coordinates can contain references to shape formulas.
    /// </summary>
    internal class PathRectangle
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal PathRectangle()
        {
        }

        /// <summary>
        /// Ctor for creating <see cref="PathRectangle"/> with non formula <see cref="PathValue"/>s.
        /// </summary>
        private PathRectangle(int left, int top, int right, int bottom)
        {
            Left = new PathValue(left);
            Top = new PathValue(top);
            Right = new PathValue(right);
            Bottom = new PathValue(bottom);
        }

        /// <summary>
        /// Returns true if this <see cref="PathRectangle"/> equals to <paramref name="other"/>.
        /// </summary>
        public bool Equals(PathRectangle other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Left.Equals(other.Left) &&
                Top.Equals(other.Top) &&
                Right.Equals(other.Right) &&
                Bottom.Equals(other.Bottom);
        }

        /// <summary>
        /// Clones this <see cref="PathRectangle"/>.
        /// </summary>
        internal PathRectangle Clone()
        {
            PathRectangle lhs = new PathRectangle();

            lhs.Left = Left.Clone();
            lhs.Top = Top.Clone();
            lhs.Right = Right.Clone();
            lhs.Bottom = Bottom.Clone();

            return lhs;
        }

        internal PathValue Left = new PathValue();
        internal PathValue Top = new PathValue();
        internal PathValue Right = new PathValue();
        internal PathValue Bottom = new PathValue();

        /// <summary>
        /// MS Word uses this as a default non-formula path rectangle.
        /// </summary>
        internal static readonly PathRectangle Default = new PathRectangle(3163, 3163, 18437, 18437);
    }
}
