// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/10/2015 by Anton Savko

using Aspose.Words.RW.Html.Css;

namespace Aspose.Words.RW.Html.Reader.CommonBorder
{
    /// <summary>
    /// Stores information about top, right, bottom and left borders.
    /// </summary>
    internal class BordersInfo
    {
        internal BordersInfo(CssDeclarationCollection declarations)
        {
            Debug.Assert(declarations != null);

            Top = new BorderInfo(declarations, BorderType.Top, "padding-top");
            Right = new BorderInfo(declarations, BorderType.Right, "padding-right");
            Bottom = new BorderInfo(declarations, BorderType.Bottom, "padding-bottom");
            Left = new BorderInfo(declarations, BorderType.Left, "padding-left");
        }

        internal void ApplyBorderPropertiesTo(BorderCollection borders)
        {
            Debug.Assert(borders != null);

            Top.ApplyBorderPropertiesTo(borders.Top);
            Right.ApplyBorderPropertiesTo(borders.Right);
            Bottom.ApplyBorderPropertiesTo(borders.Bottom);
            Left.ApplyBorderPropertiesTo(borders.Left);
        }

        internal void ApplyPaddingTo(BorderCollection borders)
        {
            Debug.Assert(borders != null);

            Top.ApplyPaddingTo(borders.Top);
            Right.ApplyPaddingTo(borders.Right);
            Bottom.ApplyPaddingTo(borders.Bottom);
            Left.ApplyPaddingTo(borders.Left);
        }

        internal bool IsVisible
        {
            get { return Top.IsVisible || Right.IsVisible || Bottom.IsVisible || Left.IsVisible; }
        }

        internal bool HasPaddingOrVisibleBorders
        {
            get { return IsVisible || Top.HasPadding || Right.HasPadding || Bottom.HasPadding || Left.HasPadding; }
        }

        internal readonly BorderInfo Top;
        internal readonly BorderInfo Right;
        internal readonly BorderInfo Bottom;
        internal readonly BorderInfo Left;
    }
}
