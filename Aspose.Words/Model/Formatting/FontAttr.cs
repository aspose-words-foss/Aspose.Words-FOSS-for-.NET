// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2005 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Attributes that can be defined for a text run.
    ///
    /// Note the constant values make sure the attributes are written
    /// into a binary file in a specific order and the order is important.
    /// </summary>
    [CppConstexpr]
    internal static class FontAttr
    {
        /// <summary>
        /// AttrBoolEx. Specifies that this line break does not indicate a line break but serves as a style separator.
        /// A style separator allows one paragraph to consist of parts that have different paragraph styles.
        /// Can only be applied to the paragraph break run properties.
        /// </summary>
        internal const int SpecialHidden = 10;
        /// <summary>
        /// Do not use anywhere except DOC import/export.
        /// Used by the binary writer to trigger writing of the first block of the system attributes.
        /// </summary>
        internal const int SysAttrs1Trigger = 16;
        /// <summary>
        /// Color. Default WordColor.Auto.
        /// </summary>
        internal const int HighlightColor = 20;
        /// <summary>
        /// int, no default
        ///
        /// Specifies a unique identifier used to track the editing session when the run properties
        /// were last modified in the main document.
        /// </summary>
        internal const int RsidRPr = 30;
        /// <summary>
        /// int, no default
        ///
        /// Specifies a unique identifier used to track the editing session when the run was
        /// added to the main document.
        /// </summary>
        internal const int RsidR = 40;
        /// <summary>
        /// LineBreakClear enum. Default LineBreakClear.None.
        /// </summary>
        internal const int LineBreakClear = 45;
        /// <summary>
        /// int, istd of the character style. Default StyleIndex.DefaultParagraphFont.
        /// </summary>
        internal const int Istd = 50;
        /// <summary>
        /// AttrBoolEx, bold font. Default BoolEx.False.
        /// </summary>
        internal const int Bold = 60;
        /// <summary>
        /// AttrBoolEx, italic font. Default BoolEx.False.
        /// </summary>
        internal const int Italic = 70;
        /// <summary>
        /// AttrBoolEx. Default BoolEx.False.
        /// </summary>
        internal const int StrikeThrough = 80;
        /// <summary>
        /// AttrBoolEx. Default BoolEx.False.
        /// </summary>
        internal const int Outline = 90;
        /// <summary>
        /// AttrBoolEx. Default BoolEx.False.
        /// </summary>
        internal const int Shadow = 100;
        /// <summary>
        /// AttrBoolEx. Default BoolEx.False.
        /// </summary>
        internal const int SmallCaps = 110;
        /// <summary>
        /// AttrBoolEx. Default BoolEx.False.
        /// </summary>
        internal const int AllCaps = 120;
        /// <summary>
        /// AttrBoolEx. Default BoolEx.False.
        /// </summary>
        internal const int Hidden = 130;
        /// <summary>
        /// AttrBoolEx. Default BoolEx.False.
        /// </summary>
        internal const int WebHidden = 132;
        /// <summary>
        /// Underline enum. Default Underline.None
        /// </summary>
        internal const int Underline = 140;
        /// <summary>
        /// int, spacing between chars in twips. Default 0.
        /// </summary>
        internal const int Spacing = 150;
        /// <summary>
        /// Color, font color. Default WordColor.Auto.
        /// </summary>
        internal const int Color = 160;
        /// <summary>
        /// AttrBoolEx. Default BoolEx.False.
        /// </summary>
        internal const int Emboss = 170;
        /// <summary>
        /// AttrBoolEx. Default BoolEx.False.
        /// </summary>
        internal const int Engrave = 180;
        /// <summary>
        /// int, font size in half points. Default 20.
        /// </summary>
        internal const int Size = 190;
        /// <summary>
        /// int, half points. Default 0.
        /// </summary>
        internal const int Position = 200;
        /// <summary>
        /// Do not use anywhere except DOC import/export.
        /// Used by the binary writer to trigger writing of the second block of the system attributes.
        /// </summary>
        internal const int SysAttrs2Trigger = 202;
        /// <summary>
        /// RunVerticalAlignment enum. Default VerticalAlignment.Baseline.
        /// </summary>
        internal const int VerticalAlignment = 210;
        /// <summary>
        /// int, half points. Default 0.
        /// </summary>
        internal const int Kerning = 220;
        /// <summary>
        /// string. font name for ascii chars 0-127. Default Times New Roman.
        /// </summary>
        internal const int NameAscii = 230;
        /// <summary>
        /// string, font name for Far East runs. Default Times New Roman.
        /// </summary>
        internal const int NameFarEast = 235;
        /// <summary>
        /// string, font name for ascii chars 128-255. Default Times New Roman.
        /// </summary>
        internal const int NameOther = 240;
        /// <summary>
        /// AttrBoolEx, bold font for rtl runs. Default BoolEx.False.
        /// </summary>
        internal const int BoldBi = 250;
        /// <summary>
        /// AttrBoolEx, italic font for rtl runs. Default BoolEx.False.
        /// </summary>
        internal const int ItalicBi = 260;
        /// <summary>
        /// AttrBoolEx. This corresponds to "rtl" in WordML. Default BoolEx.False.
        /// </summary>
        internal const int Bidi = 265;
        /// <summary>
        /// AttrBoolEx. This corresponds to "cs" in WordML. Default BoolEx.False.
        /// </summary>
        internal const int ComplexScript = 268;
        /// <summary>
        /// string, font name for RTL runs. Default Times New Roman.
        /// </summary>
        internal const int NameBi = 270;
        /// <summary>
        /// int, char width in percent. Default 100.
        /// </summary>
        internal const int Scaling = 290;
        /// <summary>
        /// AttrBoolEx. Default BoolEx.False.
        /// </summary>
        internal const int DoubleStrikeThrough = 300;
        /// <summary>
        /// TextEffect, animation effect enum. Default TextEffect.None.
        /// </summary>
        internal const int TextEffect = 310;
        /// <summary>
        /// AttrBoolEx, asian. Default BoolEx.False.
        /// </summary>
        internal const int SnapToGrid = 330;
        /// <summary>
        /// int. Default 0x0400.
        /// </summary>
        internal const int LocaleIdBi = 340;
        /// <summary>
        /// int, font size for rtl runs in half points. Default 20.
        /// </summary>
        internal const int SizeBi = 350;
        /// <summary>
        /// Border, complex attr, border around run. Default Border object.
        /// </summary>
        internal const int Border = 360;
        /// <summary>
        /// Shading, complex attribute. Default Shading object.
        /// </summary>
        internal const int Shading = 370;
        /// <summary>
        /// int. Default 0x0400.
        /// </summary>
        internal const int LocaleId = 380;
        /// <summary>
        /// int. Default 0x0400.
        /// </summary>
        internal const int LocaleIdFarEast = 390;
        /// <summary>
        /// A <see cref="CharacterCategory"/> value. Corresponds to rFonts.hint in WordML.
        /// Default CharacterCategory.Other.
        /// </summary>
        internal const int CharacterCategoryHint = 400;
        /// <summary>
        /// Do not use anywhere except DOC import/export.
        /// Used by the binary writer to trigger writing of few attributes in right place.
        /// </summary>
        internal const int WordXPAttrsTrigger = 402;
        /// <summary>
        /// AttrBoolEx. Default BoolEx.False.
        /// </summary>
        internal const int NoProofing = 440;
        /// <summary>
        /// Color. Default WordColor.Autor.
        /// </summary>
        internal const int UnderlineColor = 450;
        /// <summary>
        /// HyphenRule enum. Default HyphenRule.None.
        /// </summary>
        internal const int HyphenRule = 460;
        /// <summary>
        /// int. Default 0.
        /// </summary>
        internal const int HyphenChar = 470;
        /// <summary>
        /// Do not use anywhere except DOC import/export.
        /// int. Looks like the picture bullet id. Not always effective. See <see cref="PictureBulletFlags"/>.
        /// </summary>
        internal const int PictureBulletId = 480;
        /// <summary>
        /// Do not use anywhere except DOC import/export.
        /// enum. Looks like the picture bullet flag.
        /// </summary>
        internal const int PictureBulletFlags = 490;

        /// <summary>
        /// At the moment this is an unparsed string value.
        /// </summary>
        internal const int ThemeColor = 500;
        /// <summary>
        /// At the moment this is an unparsed string value.
        /// </summary>
        internal const int ThemeShade = 510;
        /// <summary>
        /// At the moment this is an unparsed string value.
        /// </summary>
        internal const int ThemeTint = 520;

        internal const int UnderlineThemeColor = 521;
        internal const int UnderlineThemeShade = 522;
        internal const int UnderlineThemeTint = 523;

        /// <summary>
        /// Specifies the justification of equations in the paragraph. Currently ignored.
        /// </summary>
        internal const int CFMathPr = 600;

        /// <summary>
        /// bool
        /// </summary>
        internal const int MathIsLiteral = 700;

        /// <summary>
        /// bool
        /// </summary>
        internal const int MathIsNormalText = 710;

        /// <summary>
        /// enum. <see cref="Aspose.Words.Math.MathScript"/>
        /// </summary>
        internal const int MathScript = 720;

        /// <summary>
        /// enum. <see cref="StyleType"/>
        /// </summary>
        internal const int MathStyle = 730;

        /// <summary>
        /// object, <see cref="Aspose.Words.Math.MathLineBreak"/>
        /// </summary>
        internal const int MathLineBreak = 740;

        /// <summary>
        /// bool
        /// </summary>
        internal const int MathIsAlignmentPoint = 750;

        /// <summary>
        /// bool.
        /// </summary>
        internal const int MathIsOMath = 760;

        /// <summary>
        /// enum. <see cref="Words.EmphasisMark" />.
        /// </summary>
        internal const int EmphasisMark = 770;

        /// <summary>
        /// Complex attribute.
        /// </summary>
        internal const int FarEastLayout = 780;

        /// <summary>
        /// Complex attribute.
        /// </summary>
        internal const int AlternateContent = 790;

        /// <summary>
        /// Raw XML. Specifies glow effect of text.
        /// </summary>
        internal const int EffectGlow = 810;

        /// <summary>
        /// Raw XML. Specifies shadow effect of text.
        /// </summary>
        internal const int EffectShadow = 815;

        /// <summary>
        /// Raw XML. Specifies reflection effect of text.
        /// </summary>
        internal const int EffectReflection = 820;

        /// <summary>
        /// Raw XML. Specifies outline effect of text.
        /// </summary>
        internal const int EffectOutline = 825;

        /// <summary>
        /// Raw XML. Specifies fill effect of text.
        /// </summary>
        internal const int EffectFill = 830;

        /// <summary>
        /// Raw XML. Specifies the 3-D scene properties of text, including camera and lighting.
        /// </summary>
        internal const int EffectScene3D = 835;

        /// <summary>
        /// Raw XML. Specifies the 3-D properties of text, including bevel, extrusion, contour, and material.
        /// </summary>
        internal const int EffectProps3D = 840;

        /// <summary>
        /// Specifies which kinds of ligatures <see cref="Ligature"/> to use when displaying the text.
        /// </summary>
        internal const int OpenTypeLigature = 850;

        /// <summary>
        /// Specifies the form <see cref="NumForm" />in which numerals are displayed.
        /// </summary>
        internal const int OpenTypeNumForm = 855;

        /// <summary>
        /// Specifies the form <see cref="NumSpacing"/> in which numerals are displayed.
        /// </summary>
        internal const int OpenTypeNumSpacing = 860;

        /// <summary>
        /// Specifies references to sets of character forms defined within the font to be used as stylistic sets.
        /// </summary>
        internal const int OpenTypeStylisticSets = 865;

        /// <summary>
        /// Specifies whether to display the characters using contextual alternates.
        /// </summary>
        internal const int OpenTypeContextualAlternates = 870;

        /// <summary>
        /// Specifies a width, in twips, to which text is expanded or condensed to fit.
        /// </summary>
        internal const int FitText = 880;

        /// <summary>
        /// <see cref="Ruby"/>. Specifies that run is actually phonetic guide run.
        /// </summary>
        internal const int Ruby = 885;

        /// <summary>
        /// bool. Indicates that run was read from Symbol. Used for unit testing only.
        /// </summary>
        internal const int Sys_Symbol = 890;
    }
}
