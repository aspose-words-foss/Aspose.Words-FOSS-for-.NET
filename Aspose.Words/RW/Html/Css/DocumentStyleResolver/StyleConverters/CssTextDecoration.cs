// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2017 by Victor Chebotok

using Aspose.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Stores 'text-decoration' CSS value. Allows to combine decorations.
    /// </summary>
    internal class CssTextDecoration
    {
        internal CssTextDecoration()
        {
            // Empty constructor.
        }

        private CssTextDecoration(
            NullableBool underline,
            NullableBool strikeThrough,
            DrColor underlineColor)
        {
            mUnderline = underline;
            mStrikeThrough = strikeThrough;
            mUnderlineColor = underlineColor;
        }

        internal CssTextDecoration Apply(CssDeclarationCollection declarations, bool clearOnNone)
        {
            NullableBool underline = mUnderline;
            NullableBool strikeThrough = mStrikeThrough;
            DrColor underlineColor = mUnderlineColor;

            // 'text-decoration: none' is not propagated to inner elements so we don't propagate False values.
            if (underline == NullableBool.False)
            {
                underline = NullableBool.NotDefined;
            }
            if (strikeThrough == NullableBool.False)
            {
                strikeThrough = NullableBool.NotDefined;
            }

            CssTextDecorationPropertyValue textDecorationValue = GetTextDecoration(declarations);
            if (textDecorationValue != null)
            {
                if (textDecorationValue.IsNone)
                {
                    if (clearOnNone)
                    {
                        underline = NullableBool.False;
                        underlineColor = null;
                        strikeThrough = NullableBool.False;
                    }
                }
                else
                {
                    if (textDecorationValue.IsUnderline)
                    {
                        underline = NullableBool.True;
                        underlineColor = declarations.GetColor("color");
                    }
                    if (textDecorationValue.IsLineThrough)
                    {
                        strikeThrough = NullableBool.True;
                    }
                }
            }

            return new CssTextDecoration(underline, strikeThrough, underlineColor);
        }

        /// <summary>
        /// Creates a copy of this text decoration instance but with underline style and color removed.
        /// </summary>
        internal CssTextDecoration WithoutUnderline()
        {
            return new CssTextDecoration(NullableBool.NotDefined, mStrikeThrough, null);
        }

        internal NullableBool Underline
        {
            get { return mUnderline; }
        }

        internal DrColor UnderlineColor
        {
            get { return mUnderlineColor; }
        }

        internal NullableBool StrikeThrough
        {
            get { return mStrikeThrough; }
        }

        private static CssTextDecorationPropertyValue GetTextDecoration(CssDeclarationCollection declarations)
        {
            Debug.Assert(declarations != null);

            CssDeclaration declaration = declarations["text-decoration"];
            if (declaration == null)
            {
                return null;
            }

            CssTextDecorationPropertyValue textDeclarationValue = declaration.Value as CssTextDecorationPropertyValue;
            return (textDeclarationValue != null)
                ? textDeclarationValue
                : null;
        }

        private readonly NullableBool mUnderline;
        private readonly DrColor mUnderlineColor;
        private readonly NullableBool mStrikeThrough;
    }
}
