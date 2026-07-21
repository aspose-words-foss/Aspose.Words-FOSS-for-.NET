// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/02/2014 by Alexey Butalov

using System;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// A collection of all four (top, right, bottom and left) CSS border objects.
    /// </summary>
    internal class CssBoxBorders
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal CssBoxBorders(CssBorder top, CssBorder right, CssBorder bottom, CssBorder left)
        {
            mTop = top;
            mRight = right;
            mBottom = bottom;
            mLeft = left;
        }

        /// <summary>
        /// Creates a CSS borders object from CSS border styles.
        /// </summary>
        internal static CssBoxBorders CreateBorders(
            CssDeclarationCollection declarations,
            bool invertInsetOutsetLineStyle)
        {
            CssBorder top = CssBorder.CreateBorder(declarations, BorderType.Top, invertInsetOutsetLineStyle);
            CssBorder right = CssBorder.CreateBorder(declarations, BorderType.Right, invertInsetOutsetLineStyle);
            CssBorder bottom = CssBorder.CreateBorder(declarations, BorderType.Bottom, invertInsetOutsetLineStyle);
            CssBorder left = CssBorder.CreateBorder(declarations, BorderType.Left, invertInsetOutsetLineStyle);
            return new CssBoxBorders(top, right, bottom, left);
        }

        /// <summary>
        /// Creates an empty CSS borders object.
        /// </summary>
        internal static CssBoxBorders CreateEmpty()
        {
            return new CssBoxBorders(CssBorder.Empty, CssBorder.Empty, CssBorder.Empty, CssBorder.Empty);
        }

        /// <summary>
        /// Applies CSS borders style, width and color styles to model borders.
        /// </summary>
        internal void ToModelBorders(BorderCollection modelBorders)
        {
            mTop.ToModelBorder(modelBorders.Top);
            mRight.ToModelBorder(modelBorders.Right);
            mBottom.ToModelBorder(modelBorders.Bottom);
            mLeft.ToModelBorder(modelBorders.Left);
        }

        /// <summary>
        /// Retrieves a CSS Border object by type.
        /// </summary>
        /// <param name="borderType">The type of the border to retrieve.</param>
        internal CssBorder this[BorderType borderType]
        {
            get
            {
                switch (borderType)
                {
                    case BorderType.Top:
                        return mTop;
                    case BorderType.Right:
                        return mRight;
                    case BorderType.Bottom:
                        return mBottom;
                    case BorderType.Left:
                        return mLeft;
                    default:
                        throw new InvalidOperationException("Unknown border type.");
                }
            }
        }

        /// <summary>
        /// Gets the left border.
        /// </summary>
        internal CssBorder Left
        {
            get { return mLeft; }
        }

        /// <summary>
        /// Gets the right border.
        /// </summary>
        internal CssBorder Right
        {
            get { return mRight; }
        }

        /// <summary>
        /// Gets the top border.
        /// </summary>
        internal CssBorder Top
        {
            get { return mTop; }
        }

        /// <summary>
        /// Gets the bottom border.
        /// </summary>
        internal CssBorder Bottom
        {
            get { return mBottom; }
        }

        private readonly CssBorder mLeft;
        private readonly CssBorder mRight;
        private readonly CssBorder mTop;
        private readonly CssBorder mBottom;
    }
}
