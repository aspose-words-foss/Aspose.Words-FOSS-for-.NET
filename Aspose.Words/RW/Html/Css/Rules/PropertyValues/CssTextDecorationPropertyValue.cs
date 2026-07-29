// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/04/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS text decoration property value.
    /// </summary>
    internal class CssTextDecorationPropertyValue : CssPropertyValue
    {
        internal CssTextDecorationPropertyValue(CssValue textDecoration)
            : base(textDecoration)
        {
        }

        internal CssTextDecorationPropertyValue(CssValueList values)
            : base(values)
        {
        }

        internal CssTextDecorationPropertyValue(CssPropertyValue value)
            : base(value)
        {
        }

        internal bool IsNone
        {
            get { return (Count == 1) && (FirstValue.Equals(CssValue.None)); }
        }

        internal bool IsUnderline
        {
            get { return Contains(CssValue.Underline); }
        }

        internal bool IsOverline
        {
            get { return Contains(CssValue.Overline); }
        }

        internal bool IsLineThrough
        {
            get { return Contains(CssValue.LineThrough); }
        }

        internal bool IsBlink
        {
            get { return Contains(CssValue.Blink); }
        }

        /// <summary>
        /// 'none' text decoration.
        /// </summary>
        internal static readonly CssTextDecorationPropertyValue None =
            new CssTextDecorationPropertyValue(CssValue.None);
    }
}
