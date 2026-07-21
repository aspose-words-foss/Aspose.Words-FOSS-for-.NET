// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/12/2015 by Victor Chebotok

using System.Collections.Generic;
using System.Drawing;
using Aspose.Words.Fonts;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS @font-face rule.
    /// </summary>
    /// <remarks>
    /// Please see http://www.w3.org/TR/css-fonts-3/#font-face-rule for more information.
    /// </remarks>
    internal class CssFontFaceRule : CssRule
    {
        internal CssFontFaceRule(CssDeclarationCollection declarations)
            : base(CssRuleType.FontFace)
        {
            Debug.Assert(declarations != null);

            Declarations = declarations;
        }

        internal bool DeclaresFontFamily(string fontFamily)
        {
            CssDeclaration fontFamilyDeclaration = Declarations["font-family"];
            if ((fontFamilyDeclaration != null) &&
                (fontFamilyDeclaration.Value is CssFontFamilyPropertyValue))
            {
                for (int i = 0; i < fontFamilyDeclaration.Value.Count; i++)
                {
                    string declaredFontFamily;
                    CssValue fontFamilyValue = fontFamilyDeclaration.Value[i];
                    switch (fontFamilyValue.ValueType)
                    {
                        case CssValueType.Identifier:
                        {
                            declaredFontFamily = ((CssIdentifierValue)fontFamilyValue).Value;
                            break;
                        }
                        case CssValueType.String:
                        {
                            declaredFontFamily = ((CssStringValue)fontFamilyValue).Value;
                            break;
                        }
                        default:
                        {
                            Debug.Fail("Unexpected type of a CSS value.");
                            continue;
                        }
                    }

                    if (StringUtil.EqualsOrdinalIgnoreCase(fontFamily, declaredFontFamily))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal IList<CssFontSource> GetSources()
        {
            CssDeclaration srcDeclaration = Declarations["src"];
            if ((srcDeclaration != null) && (srcDeclaration.Value is CssSrcPropertyValue))
            {
                CssSrcPropertyValue srcPropertyValue = (CssSrcPropertyValue)srcDeclaration.Value;
                return srcPropertyValue.FontSources;
            }

            return null;
        }

        internal FontStyle GetFontStyle()
        {
            return (FontStyle)GetFontStyleIntValue();
        }

        internal EmbeddedFontStyle GetEmbeddedFontStyle()
        {
            return (EmbeddedFontStyle)GetFontStyleIntValue();
        }

        internal override string ToCss()
        {
            return "@font-face { " + Declarations.GetShorthandVersion().ToCss() + " }";
        }

        internal CssDeclarationCollection Declarations { get; }

        /// <summary>
        /// Converts "bold"/"italic" style combination of this rule to an integer value in range [0..3].
        /// "bold" sets the low bit and "italic" sets the high bit.
        /// </summary>
        private int GetFontStyleIntValue()
        {
            // regular       = 0x0 (0b00)
            // bold          = 0x1 (0b01)
            // italic        = 0x2 (0b10)
            // bold + italic = 0x3 (0b11)
            int result = 0;
            result |= (CssUtil.IsBoldFont(Declarations, "font-weight") == NullableBool.True)
                ? 0x1
                : 0x0;
            result |= (CssUtil.IsItalicFont(Declarations, "font-style") == NullableBool.True)
                ? 0x2
                : 0x0;
            return result;
        }
    }
}
