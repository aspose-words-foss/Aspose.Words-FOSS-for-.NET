// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/05/2005 by Roman Korchagin

using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Math;
using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Provides typed access to run attributes.
    /// </summary>
    internal class RunPr : WordAttrCollection, IRunAttrSource, IMathRunPr
    {
        internal override void AcceptFormatRevision()
        {
            AcceptRunSpecificFormatRevision(false);
        }

        /// <summary>
        /// If isSymbolicRun is true accepts format revision for RunPr that is originating from w:sym (or alike) element and contains non-Unicode symbol,
        /// otherwise When processing run properties we preserve only insert and delete revisions.
        /// </summary>
        /// <remarks>
        /// WORDSNET-7098
        /// </remarks>
        internal void AcceptRunSpecificFormatRevision(bool isSymbolicRun)
        {
            AcceptFormatRevisionCore(FontAttr.Istd, (isSymbolicRun) ? gRevisionPreservedSymbolAttributes : gRevisionPreservedRunAttributes);
        }

        internal object FetchAttr(int key, RunPrExpandFlags flags)
        {
            return GetAttr(key, ToValueVersion(flags), true);
        }

        internal object GetDirectAttr(int key, RunPrExpandFlags flags)
        {
            return GetAttr(key, ToValueVersion(flags), false);
        }

        #region IRunAttrSource

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        object IRunAttrSource.GetDirectRunAttr(int key)
        {
            return GetDirectAttr(key);
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        object IRunAttrSource.GetDirectRunAttr(int key, RevisionsView revisionsView)
        {
            return GetDirectAttr(key, revisionsView);
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        object IRunAttrSource.FetchInheritedRunAttr(int key)
        {
            return FetchInheritedAttr(key);
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        void IRunAttrSource.SetRunAttr(int key, object value)
        {
            SetAttr(key, value);
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        void IRunAttrSource.RemoveRunAttr(int key)
        {
            Remove(key);
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        void IRunAttrSource.ClearRunAttrs()
        {
            Clear();
        }

        #endregion

        internal static object FetchDefaultAttr(int key)
        {
            return gDefaults.FetchAttr(key);
        }

        internal static object GetDefaultAttr(int key)
        {
            return gDefaults.GetDirectAttr(key);
        }

        /// <summary>
        /// Fix ThemeColor inheritance problem.
        /// </summary>
        /// <remarks>
        /// This is just temporary workaround and relates to WORDSNET-3312
        /// See https://auckland.dynabic.com/wiki/display/org/WORDSNET-3312%2C+Expose+theme+information+to+Public+API.
        /// Color, ThemeColor, ThemeShade, ThemeTint should be combined into one complex attribute.
        /// </remarks>
        internal void ThemeColorInheritanceHack(AttrCollection dstAttrs)
        {
            bool hasThemeColor = Contains(FontAttr.ThemeColor);

            // WORDSNET-14587 When we expand with global defaults and there is no ThemeColor
            // in styles hierarchy and in direct attributes, then we will get this global
            // ThemeColor in dstAttrs. So, remove global default ThemeColor from destination
            // collection to eliminate possible difference in comparisons.
            if ((!hasThemeColor && Contains(FontAttr.Color)) || dstAttrs.IsDefaultValue(FontAttr.ThemeColor))
                dstAttrs.Remove(FontAttr.ThemeColor);

            // "ThemeShade", "ThemeTint" have meaningful when "ThemeColor" is specified.
            if (!hasThemeColor)
                return;

            // WORDSNET-14174 Replace value of the "ThemeShape" attribute with global default value
            // to fix inheritance logic. “ThemeColor”, “ThemeShade”, “ThemeTint” attributes MSW uses like
            // a single complex attribute.
            if (dstAttrs.Contains(FontAttr.ThemeShade) && !Contains(FontAttr.ThemeShade))
                dstAttrs.SetAttr(FontAttr.ThemeShade, "");

            if (dstAttrs.Contains(FontAttr.ThemeTint) && !Contains(FontAttr.ThemeTint))
                dstAttrs.SetAttr(FontAttr.ThemeTint, "");
        }

        private static ValueVersion ToValueVersion(RunPrExpandFlags flags)
        {
            if ((flags & RunPrExpandFlags.AfterChanges) != 0)
                return ValueVersion.AfterChanges;
#if JAVA
            if ((flags & RunPrExpandFlags.Revised | flags & RunPrExpandFlags.DocumentDefaults) != 0)
#else
            if ((flags & RunPrExpandFlags.Revised) != 0)
#endif
                return ValueVersion.Final;

            return ValueVersion.Original;
        }

        /// <summary>
        /// Returns the static collection of default attributes.
        /// </summary>
        protected override AttrCollection GetDefaults()
        {
            return gDefaults;
        }

        public bool IsDml
        {
            get { return false; }
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]

        int IMathRunPr.Count
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return Count; }
        }

        internal DrColor HighlightColor
        {
            get { return (DrColor)FetchAttr(FontAttr.HighlightColor); }
            set { SetAttr(FontAttr.HighlightColor, value); }
        }

        internal int RsidRPr
        {
            get { return (int)FetchAttr(FontAttr.RsidRPr); }
            set { SetAttr(FontAttr.RsidRPr, value); }
        }

        internal int RsidR
        {
            get { return (int)FetchAttr(FontAttr.RsidR); }
            set { SetAttr(FontAttr.RsidR, value); }
        }

        internal LineBreakClear LineBreakClear
        {
            get { return (LineBreakClear)FetchAttr(FontAttr.LineBreakClear); }
            set { SetAttr(FontAttr.LineBreakClear, value); }
        }

        /// <summary>
        /// Style index.
        /// </summary>
        internal int Istd
        {
            get { return (int)FetchAttr(FontAttr.Istd); }
            set { SetAttr(FontAttr.Istd, value); }
        }

        internal AttrBoolEx Bold
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.Bold); }
            set { SetAttr(FontAttr.Bold, value); }
        }

        internal AttrBoolEx Italic
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.Italic); }
            set { SetAttr(FontAttr.Italic, value); }
        }

        internal AttrBoolEx StrikeThrough
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.StrikeThrough); }
            set { SetAttr(FontAttr.StrikeThrough, value); }
        }

        internal AttrBoolEx Outline
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.Outline); }
            set { SetAttr(FontAttr.Outline, value); }
        }

        internal AttrBoolEx Shadow
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.Shadow); }
            set { SetAttr(FontAttr.Shadow, value); }
        }

        internal AttrBoolEx SmallCaps
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.SmallCaps); }
            set { SetAttr(FontAttr.SmallCaps, value); }
        }

        internal AttrBoolEx AllCaps
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.AllCaps); }
            set { SetAttr(FontAttr.AllCaps, value); }
        }

        internal AttrBoolEx Hidden
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.Hidden); }
            set { SetAttr(FontAttr.Hidden, value); }
        }

        internal AttrBoolEx WebHidden
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.WebHidden); }
            set { SetAttr(FontAttr.WebHidden, value); }
        }

        internal Underline Underline
        {
            get { return (Underline)FetchAttr(FontAttr.Underline); }
            set { SetAttr(FontAttr.Underline, value); }
        }

        internal int Spacing
        {
            get { return (int)FetchAttr(FontAttr.Spacing); }
            set { SetAttr(FontAttr.Spacing, value); }
        }

        internal DrColor Color
        {
            get { return (DrColor)FetchAttr(FontAttr.Color); }
            set { SetAttr(FontAttr.Color, value); }
        }

        internal AttrBoolEx Emboss
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.Emboss); }
            set { SetAttr(FontAttr.Emboss, value); }
        }

        internal AttrBoolEx Engrave
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.Engrave); }
            set { SetAttr(FontAttr.Engrave, value); }
        }

        internal int Size
        {
            get { return (int)FetchAttr(FontAttr.Size); }
            set { SetAttr(FontAttr.Size, value); }
        }

        internal int Position
        {
            get { return (int)FetchAttr(FontAttr.Position); }
            set { SetAttr(FontAttr.Position, value); }
        }

        internal RunVerticalAlignment VerticalAlignment
        {
            get { return (RunVerticalAlignment)FetchAttr(FontAttr.VerticalAlignment); }
            set { SetAttr(FontAttr.VerticalAlignment, value); }
        }

        internal int Kerning
        {
            get { return (int)FetchAttr(FontAttr.Kerning); }
            set { SetAttr(FontAttr.Kerning, value); }
        }

        internal string Name
        {
            get { return NameAscii; }
            set
            {
                // Have to be on separate lines for autoporting.
                NameAscii = value;
                NameBi = value;
                NameFarEast = value;
                NameOther = value;
            }
        }

        /// <summary>
        /// Font name for ASCII text.
        /// Note getter throws if font is defined by theme font,
        /// so use corresponding <see cref="ComplexNameAscii"/> property to get more control.
        /// </summary>
        internal string NameAscii
        {
            get { return ((ComplexFontName)FetchAttr(FontAttr.NameAscii)).Name; }
            set { SetAttr(FontAttr.NameAscii, ComplexFontName.FromName(value)); }
        }

        /// <summary>
        /// Font name for Far East Asian text.
        /// Note getter throws if font is defined by theme font,
        /// so use corresponding <see cref="ComplexNameFarEast"/> property to get more control.
        /// </summary>
        internal string NameFarEast
        {
            get { return ((ComplexFontName)FetchAttr(FontAttr.NameFarEast)).Name; }
            set { SetAttr(FontAttr.NameFarEast, ComplexFontName.FromName(value)); }
        }

        /// <summary>
        /// Font name for other text.
        /// Note getter throws if font is defined by theme font,
        /// so use corresponding <see cref="ComplexNameOther"/> property to get more control.
        /// </summary>
        internal string NameOther
        {
            get { return ((ComplexFontName)FetchAttr(FontAttr.NameOther)).Name; }
            set { SetAttr(FontAttr.NameOther, ComplexFontName.FromName(value)); }
        }

        /// <summary>
        /// Font name for bi-directional text.
        /// Note getter throws if font is defined by theme font,
        /// so use corresponding <see cref="ComplexNameBi"/> property to get more control.
        /// </summary>
        internal string NameBi
        {
            get { return ((ComplexFontName)FetchAttr(FontAttr.NameBi)).Name; }
            set { SetAttr(FontAttr.NameBi, ComplexFontName.FromName(value)); }
        }

        /// <summary>
        /// Theme font for ASCII text.
        /// Note getter throws if font is defined not by theme font,
        /// so use corresponding <see cref="ComplexNameAscii"/> property to get more control.
        /// </summary>
        internal ThemeFontCore ThemeAscii
        {
            get { return ((ComplexFontName)FetchAttr(FontAttr.NameAscii)).ThemeFontCore; }
            set { SetAttr(FontAttr.NameAscii, ComplexFontName.FromTheme(value)); }
        }

        /// <summary>
        /// Theme font for Far East Asian text.
        /// Note getter throws if font is defined not by theme font,
        /// so use corresponding <see cref="ComplexNameFarEast"/> property to get more control.
        /// </summary>
        internal ThemeFontCore ThemeFarEast
        {
            get { return ((ComplexFontName)FetchAttr(FontAttr.NameFarEast)).ThemeFontCore; }
            set { SetAttr(FontAttr.NameFarEast, ComplexFontName.FromTheme(value)); }
        }

        /// <summary>
        /// Theme font for other text.
        /// Note getter throws if font is defined not by theme font,
        /// so use corresponding <see cref="ComplexNameOther"/> property to get more control.
        /// </summary>
        internal ThemeFontCore ThemeOther
        {
            get { return ((ComplexFontName)FetchAttr(FontAttr.NameOther)).ThemeFontCore; }
            set { SetAttr(FontAttr.NameOther, ComplexFontName.FromTheme(value)); }
        }

        /// <summary>
        /// Theme font for bi-directional text.
        /// Note getter throws if font is defined not by theme font,
        /// so use corresponding <see cref="ComplexNameBi"/> property to get more control.
        /// </summary>
        internal ThemeFontCore ThemeBi
        {
            get { return ((ComplexFontName)FetchAttr(FontAttr.NameBi)).ThemeFontCore; }
            set { SetAttr(FontAttr.NameBi, ComplexFontName.FromTheme(value)); }
        }

        internal ComplexFontName ComplexNameAscii
        {
            get { return (ComplexFontName)FetchAttr(FontAttr.NameAscii); }
            set { SetAttr(FontAttr.NameAscii, value); }
        }

        internal ComplexFontName ComplexNameFarEast
        {
            get { return (ComplexFontName)FetchAttr(FontAttr.NameFarEast); }
            set { SetAttr(FontAttr.NameFarEast, value); }
        }

        internal ComplexFontName ComplexNameOther
        {
            get { return (ComplexFontName)FetchAttr(FontAttr.NameOther); }
            set { SetAttr(FontAttr.NameOther, value); }
        }

        internal ComplexFontName ComplexNameBi
        {
            get { return (ComplexFontName)FetchAttr(FontAttr.NameBi); }
            set { SetAttr(FontAttr.NameBi, value); }
        }

        internal AttrBoolEx BoldBi
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.BoldBi); }
            set { SetAttr(FontAttr.BoldBi, value); }
        }

        internal AttrBoolEx ItalicBi
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.ItalicBi); }
            set { SetAttr(FontAttr.ItalicBi, value); }
        }

        internal AttrBoolEx Bidi
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.Bidi); }
            set { SetAttr(FontAttr.Bidi, value); }
        }

        internal AttrBoolEx ComplexScript
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.ComplexScript); }
            set { SetAttr(FontAttr.ComplexScript, value); }
        }

        internal int Scaling
        {
            get { return (int)FetchAttr(FontAttr.Scaling); }
            set { SetAttr(FontAttr.Scaling, value); }
        }

        internal AttrBoolEx DoubleStrikeThrough
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.DoubleStrikeThrough); }
            set { SetAttr(FontAttr.DoubleStrikeThrough, value); }
        }

        internal TextEffect TextEffect
        {
            get { return (TextEffect)FetchAttr(FontAttr.TextEffect); }
            set { SetAttr(FontAttr.TextEffect, value); }
        }

        internal AttrBoolEx SnapToGrid
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.SnapToGrid); }
            set { SetAttr(FontAttr.SnapToGrid, value); }
        }

        internal int LocaleIdBi
        {
            get { return (int)FetchAttr(FontAttr.LocaleIdBi); }
            set { SetAttr(FontAttr.LocaleIdBi, value); }
        }

        internal int SizeBi
        {
            get { return (int)FetchAttr(FontAttr.SizeBi); }
            set { SetAttr(FontAttr.SizeBi, value); }
        }

        internal Border Border
        {
            get { return (Border)FetchAttr(FontAttr.Border); }
            set { SetAttr(FontAttr.Border, value); }
        }

        internal Shading Shading
        {
            get { return (Shading)FetchAttr(FontAttr.Shading); }
            set { SetAttr(FontAttr.Shading, value); }
        }

        internal int LocaleId
        {
            get { return (int)FetchAttr(FontAttr.LocaleId); }
            set { SetAttr(FontAttr.LocaleId, value); }
        }

        internal int LocaleIdFarEast
        {
            get { return (int)FetchAttr(FontAttr.LocaleIdFarEast); }
            set { SetAttr(FontAttr.LocaleIdFarEast, value); }
        }

        internal CharacterCategory CharacterCategoryHint
        {
            get { return (CharacterCategory)FetchAttr(FontAttr.CharacterCategoryHint); }
            set { SetAttr(FontAttr.CharacterCategoryHint, value); }
        }

        internal AttrBoolEx NoProofing
        {
            get { return (AttrBoolEx)FetchAttr(FontAttr.NoProofing); }
            set { SetAttr(FontAttr.NoProofing, value); }
        }

        internal DrColor UnderlineColor
        {
            get { return (DrColor)FetchAttr(FontAttr.UnderlineColor); }
            set { SetAttr(FontAttr.UnderlineColor, value); }
        }

        internal HyphenRule HyphenRule
        {
            get { return (HyphenRule)FetchAttr(FontAttr.HyphenRule); }
            set { SetAttr(FontAttr.HyphenRule, value); }
        }

        internal int HyphenChar
        {
            get { return (int)FetchAttr(FontAttr.HyphenChar); }
            set { SetAttr(FontAttr.HyphenChar, value); }
        }

        /// <summary>
        /// Specific for <see cref="OfficeMath"/> instances, don't set this property for regular <see cref="Run"/> instances.
        /// Specifies that the characters  in the run are literal; that is, they are to be interpreted literally and
        /// not be built up based on any implied mathematical meaning.
        /// </summary>
        internal bool MathIsLiteral
        {
            get { return (bool)FetchAttr(FontAttr.MathIsLiteral); }
            set { SetAttr(FontAttr.MathIsLiteral, value, value); }
        }

        /// <summary>
        /// Specific for <see cref="OfficeMath"/> instances, don't set this property for regular <see cref="Run"/> instances.
        /// Specifies that the run is normal text,  i.e., math italics and math spacing are not applied.
        /// </summary>
        /// <remarks>
        /// In a normal text run, no characters will trigger reformatting of a linear expression into a two-dimensional expression.
        /// </remarks>
        internal bool MathIsNormalText
        {
            get { return (bool)FetchAttr(FontAttr.MathIsNormalText); }
            set { SetAttr(FontAttr.MathIsNormalText, value, value); }
        }

        /// <summary>
        /// Specific for <see cref="OfficeMath"/> instances, don't set this property for regular <see cref="Run"/> instances.
        /// Describes the script applied to the characters in the run of mathematical text.
        /// </summary>
        /// <remarks>
        /// The XML includes the Unicode value of the character (between U+0000 and U+007F),  along with the script of the character.
        /// The application maps the value and script type to the appropriate Math Alphanumeric Unicode.
        /// </remarks>
        internal MathScript MathScript
        {
            get { return (MathScript)FetchAttr(FontAttr.MathScript); }
            set { SetAttr(FontAttr.MathScript, value, value != MathScript.Default); }
        }

        /// <summary>
        /// Specific for <see cref="OfficeMath"/> instances, don't set this property for regular <see cref="Run"/> instances.
        /// Describes the script applied to the characters in the run.
        /// </summary>
        /// <remarks>
        /// The XML includes the Unicode value of the character along with the style of the character.
        /// The application maps the value and style to the appropriate Math Alphanumeric Unicode range.
        /// </remarks>
        internal MathStyle MathStyle
        {
            get { return (MathStyle)FetchAttr(FontAttr.MathStyle); }
            set { SetAttr(FontAttr.MathStyle, value, value != MathStyle.Default); }
        }

        /// <summary>
        /// Specific for <see cref="OfficeMath"/> instances, don't set this property for regular <see cref="Run"/> instances.
        /// Specifies whether there is a manual line break at the start of a math run.
        /// Can be null, meaning that no line break is associated with this run.
        /// </summary>
        /// <remarks>
        /// Word ignores LineBreak if it is specified for a math run which does not begin with an operator.
        /// </remarks>
        internal MathLineBreak MathLineBreak
        {
            get { return (MathLineBreak)GetDirectAttr(FontAttr.MathLineBreak); }
            set { SetAttr(FontAttr.MathLineBreak, value, value != null); }
        }

        /// <summary>
        /// Specific for <see cref="OfficeMath"/> instances, don't set this property for regular <see cref="Run"/> instances.
        /// Specifies the alignment property on the box object. It is utilized only when the box is designated as
        /// an operator emulator. When true, this operator emulator serves as an alignment point; that is, designated
        /// alignment points in other equations can be aligned with it.
        /// </summary>
        internal bool MathIsAlignmentPoint
        {
            get { return (bool)FetchAttr(FontAttr.MathIsAlignmentPoint); }
            set { SetAttr(FontAttr.MathIsAlignmentPoint, value, value); }
        }

        /// <summary>
        /// Returns true if this RunPr instance has properties specific for a <see cref="OfficeMath"/>.
        /// </summary>
        internal bool IsMathRunPr
        {
            get
            {
                for (int i = 0; i < gMathKeysToCheck.Length; i++)
                {
                    if (GetDirectAttr(gMathKeysToCheck[i]) != null)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Mysterious property that MS Words sometimes inserts and sometimes doesn't.
        /// Although it has very defined meaning in iso 29500 (see. 17.3.2.22 oMath (Office Open XML Math))
        /// We don't insert it anywhere except if required by roundtripping.
        /// </summary>
        internal bool IsOMath
        {
            get { return (bool)FetchAttr(FontAttr.MathIsOMath); }
            set { SetAttr(FontAttr.MathIsOMath, value, value); }
        }

        /// <summary>
        /// Gets the object that represents info about field numbering revision.
        /// </summary>
        internal FieldNumberRevision NumberRevision
        {
            get { return (FieldNumberRevision)GetDirectAttr(RevisionAttr.NumberRevision); }
            set { SetAttr(RevisionAttr.NumberRevision, value); }
        }

        internal bool HasNumberRevision
        {
            get { return NumberRevision != null; }
        }

        /// <summary>
        /// Specifies <see cref="EmphasisMark" /> type for text.
        /// </summary>
        internal EmphasisMark EmphasisMark
        {
            get { return (EmphasisMark)FetchAttr(FontAttr.EmphasisMark); }
            set { SetAttr(FontAttr.EmphasisMark, value); }
        }

        /// <summary>
        /// Specifies <see cref="FarEastLayout" /> option for text.
        /// </summary>
        /// <remarks>Can be null.</remarks>
        internal FarEastLayout FarEastLayout
        {
            get { return (FarEastLayout)GetDirectAttr(FontAttr.FarEastLayout); }
            set { SetAttr(FontAttr.FarEastLayout, value); }
        }

        /// <summary>
        /// Container to store FallBack of AlternateContent and Requires attribute when Choice matched and can be stored in the model.
        /// </summary>
        internal AlternateContent AlternateContent
        {
            get { return (AlternateContent)GetDirectAttr(FontAttr.AlternateContent); }
            set { SetAttr(FontAttr.AlternateContent, value); }
        }

        /// <summary>
        /// Returns true if any theme font is specified.
        /// </summary>
        internal bool HasThemeFont
        {
            get
            {
                return ComplexNameAscii.IsThemeFont || ComplexNameBi.IsThemeFont ||
                    ComplexNameFarEast.IsThemeFont || ComplexNameOther.IsThemeFont;
            }
        }

        /// <summary>
        /// Returns true if all ComplexFontName are represented as ThemeFont.
        /// </summary>
        internal bool AreAllComplexFontNamesThemeFont
        {
            get
            {
                return ComplexNameAscii.IsThemeFont && ComplexNameBi.IsThemeFont &&
                       ComplexNameFarEast.IsThemeFont && ComplexNameOther.IsThemeFont;
            }
        }

        /// <summary>
        /// Sets font names for symbols.
        /// </summary>
        /// <param name="symbolFontName">Name of symbol's font</param>
        internal void SetSymbolFonts(string symbolFontName)
        {
            if (StringUtil.HasChars(symbolFontName))
            {
                // RK By looking at MS gold files, it seems only these two attributes must be set when reading symbol font.
                NameOther = symbolFontName;
                NameAscii = symbolFontName;

                // WORDSNET-28407 Set symbolic font to format revision as well.
                if (FormatRevision != null)
                {
                    RunPr revPr = (RunPr)FormatRevision.RevPr;
                    revPr.NameOther = symbolFontName;
                    revPr.NameAscii = symbolFontName;
                }
            }
        }

        /// <summary>
        /// Sets complex font names for symbols.
        /// </summary>
        /// <param name="symbolFontName">Name of symbol's complex font</param>
        internal void SetSymbolFonts(ComplexFontName symbolFontName)
        {
            if (symbolFontName.IsThemeFont)
            {
                ComplexNameAscii = symbolFontName;
                ComplexNameOther = symbolFontName;
            }
            else
            {
                SetSymbolFonts(symbolFontName.Name);
            }
        }

        /// <summary>
        /// Returns property collection depending on passed flags.
        /// </summary>
        /// <remarks>
        /// AM. This code almost duplicates ParaPr.GetSourceParaPr method
        /// but I don't see the good way to move it into base AttrCollection class.
        /// </remarks>
        internal RunPr GetSourceRunPr(RunPrExpandFlags flags)
        {
            RunPr sourcePr = this;

            if (FormatRevision != null)
            {
                if ((flags & RunPrExpandFlags.Revised) != 0)
                {
                    sourcePr = this.Clone();
                    sourcePr.AcceptFormatRevision();
                }
                else if ((flags & RunPrExpandFlags.AfterChanges) != 0)
                {
                    sourcePr = (RunPr)FormatRevision.RevPr;
                }
            }

            return sourcePr;
        }

        [CodePorting.Translator.Cs2Cpp.CppSkipEntity("Platform specific optimization")]
        protected override int DefaultCapacity
        {
            get
            {
                // WORDSNET-21593 Reduce default capacity to optimize memory consumption.
                return 4;
            }
        }

        /// <summary>
        /// Performs comparison of runPrs according to BiDi formatting.
        /// Returns true, if runPrs are the same, false - otherwise.
        /// </summary>
        internal static bool IsSameFormatting(RunPr runPrA, RunPr runPrB)
        {
            // WORDSNET-14279 Ignore keys depending on BiDi formatting.
            int[] keysToIgnore = runPrB.Bidi.ToBool()
                ? Run.KeysToIgnoreInComparisonOnJoinRtlRuns
                : Run.KeysToIgnoreInComparisonOnJoinLtrRuns;

            return runPrB.Equals(runPrA, keysToIgnore);
        }

        /// <summary>
        /// At the moment I don't define default values for complex attributes that cannot be inherited such
        /// as text columns or formatting revisions. There is no point in defining them since they cannot be inherited.
        ///
        /// Also I don't define default values for some undocumented attributes such as timestamp etc.
        /// This is because such attributes should not be inherited (we don't know what they are).
        ///
        /// So basically, if the attribute is not to be inherited and therefore not to be resolved up the
        /// inheritance chain to the default attributes - there is no point in defining it here.
        /// It can be defined and it will not hurt, but I think it should not be.
        /// </summary>
        private static RunPr InitDefaults()
        {
            RunPr defaults = new RunPr();

            defaults.Add(FontAttr.NameAscii, ComplexFontName.FromName(TimesNewRoman));
            defaults.Add(FontAttr.NameBi, ComplexFontName.FromName(TimesNewRoman));
            defaults.Add(FontAttr.NameFarEast, ComplexFontName.FromName(TimesNewRoman));
            defaults.Add(FontAttr.NameOther, ComplexFontName.FromName(TimesNewRoman));

            defaults.Add(FontAttr.LineBreakClear, LineBreakClear.None);

            defaults.Add(FontAttr.Size, 20);                   // half points
            defaults.Add(FontAttr.SizeBi, 20);                 // half points
            defaults.Add(FontAttr.Bold, AttrBoolEx.False);
            defaults.Add(FontAttr.BoldBi, AttrBoolEx.False);
            defaults.Add(FontAttr.Italic, AttrBoolEx.False);
            defaults.Add(FontAttr.ItalicBi, AttrBoolEx.False);

            defaults.Add(FontAttr.Color, DrColor.Empty);
            defaults.Add(FontAttr.Istd, StyleIndex.DefaultParagraphFont);

            // These are complex attributes. Be careful not to modify the actual values in them.
            defaults.Add(FontAttr.Border, new Border());
            defaults.Add(FontAttr.Shading, new Shading());

            defaults.Add(FontAttr.StrikeThrough, AttrBoolEx.False);
            defaults.Add(FontAttr.DoubleStrikeThrough, AttrBoolEx.False);
            defaults.Add(FontAttr.Shadow, AttrBoolEx.False);
            defaults.Add(FontAttr.Outline, AttrBoolEx.False);
            defaults.Add(FontAttr.Emboss, AttrBoolEx.False);
            defaults.Add(FontAttr.Engrave, AttrBoolEx.False);

            defaults.Add(FontAttr.VerticalAlignment, RunVerticalAlignment.Baseline);
            defaults.Add(FontAttr.SmallCaps, AttrBoolEx.False);
            defaults.Add(FontAttr.AllCaps, AttrBoolEx.False);
            defaults.Add(FontAttr.Hidden, AttrBoolEx.False);
            defaults.Add(FontAttr.WebHidden, AttrBoolEx.False);
            defaults.Add(FontAttr.SpecialHidden, AttrBoolEx.False);
            defaults.Add(FontAttr.Underline, Underline.None);
            defaults.Add(FontAttr.UnderlineColor, DrColor.Empty);

            defaults.Add(FontAttr.Scaling, 100);
            defaults.Add(FontAttr.Spacing, 0);
            defaults.Add(FontAttr.Position, 0);
            defaults.Add(FontAttr.Kerning, 0);

            defaults.Add(FontAttr.HighlightColor, DrColor.Empty);
            defaults.Add(FontAttr.TextEffect, TextEffect.None);
            defaults.Add(FontAttr.NoProofing, AttrBoolEx.False);
            defaults.Add(FontAttr.Bidi, AttrBoolEx.False);
            defaults.Add(FontAttr.ComplexScript, AttrBoolEx.False);
            defaults.Add(FontAttr.SnapToGrid, AttrBoolEx.True);

            defaults.Add(FontAttr.LocaleId, ProcessOrUserDefaultLanguageId);
            defaults.Add(FontAttr.LocaleIdBi, ProcessOrUserDefaultLanguageId);
            defaults.Add(FontAttr.LocaleIdFarEast, ProcessOrUserDefaultLanguageId);

            defaults.Add(FontAttr.CharacterCategoryHint, CharacterCategory.Other);
            // See "MS-DOC chapter 2.6.1 Character Properties". Used only in WML and DOC formats.
            defaults.Add(FontAttr.HyphenRule, HyphenRule.Normal);
            defaults.Add(FontAttr.HyphenChar, 0);

            defaults.Add(FontAttr.ThemeColor, "");
            defaults.Add(FontAttr.ThemeShade, "");
            defaults.Add(FontAttr.ThemeTint, "");
            defaults.Add(FontAttr.UnderlineThemeColor, "");
            defaults.Add(FontAttr.UnderlineThemeShade, "");
            defaults.Add(FontAttr.UnderlineThemeTint, "");

            defaults.Add(FontAttr.MathIsLiteral, false);
            defaults.Add(FontAttr.MathIsNormalText, false);
            defaults.Add(FontAttr.MathScript, MathScript.Default);
            defaults.Add(FontAttr.MathStyle, MathStyle.Default);
            defaults.Add(FontAttr.MathIsAlignmentPoint, false);

            defaults.Add(FontAttr.MathIsOMath, false);
            defaults.Add(FontAttr.EmphasisMark, EmphasisMark.None);

            defaults.Add(FontAttr.EffectGlow, DmlNullEffect);
            defaults.Add(FontAttr.EffectShadow, DmlNullEffect);
            defaults.Add(FontAttr.EffectReflection, DmlNullEffect);
            defaults.Add(FontAttr.EffectOutline, DmlNullEffect);
            defaults.Add(FontAttr.EffectFill, DmlNullEffect);
            defaults.Add(FontAttr.EffectScene3D, DmlNullEffect);
            defaults.Add(FontAttr.EffectProps3D, DmlNullEffect);
            defaults.Add(FontAttr.OpenTypeLigature, Ligature.None);
            defaults.Add(FontAttr.OpenTypeNumForm, NumForm.Default);
            defaults.Add(FontAttr.OpenTypeNumSpacing, NumSpacing.Default);
            defaults.Add(FontAttr.OpenTypeStylisticSets, StylisticSets.Default);
            defaults.Add(FontAttr.OpenTypeContextualAlternates, false);

            defaults.Add(FontAttr.FarEastLayout, new FarEastLayout());
            defaults.Add(FontAttr.FitText, new FitText(0, 0));

            defaults.Add(FontAttr.RsidRPr, 0);
            defaults.Add(FontAttr.RsidR, 0);

            return defaults;
        }

        internal static string DefaultNameAscii
        {
            get { return ((ComplexFontName)FetchDefaultAttr(FontAttr.NameAscii)).Name; }
        }

        internal static string DefaultNameBi
        {
            get { return ((ComplexFontName)FetchDefaultAttr(FontAttr.NameBi)).Name; }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int ProcessOrUserDefaultLanguageId = 0x400;

        internal static ThemeFontCore[] ThemeFontAttrs = new ThemeFontCore[] {
            ThemeFontCore.MinorAscii,
            ThemeFontCore.MinorHAnsi,
            ThemeFontCore.MinorEastAsia,
            ThemeFontCore.MinorBidi,
            ThemeFontCore.MajorAscii,
            ThemeFontCore.MajorHAnsi,
            ThemeFontCore.MajorEastAsia,
            ThemeFontCore.MajorBidi
        };

        private static DmlNullEffect DmlNullEffect
        {
            get
            {
                if (gDmlNullEffect == null)
                    gDmlNullEffect = new DmlNullEffect();

                return gDmlNullEffect;
            }
        }

        private static DmlNullEffect gDmlNullEffect;

        /// <summary>
        /// Keys to detect that RunPr has math-specific properties.
        /// </summary>
        private static readonly int[] gMathKeysToCheck = new int[] {
            FontAttr.MathIsLiteral,
            FontAttr.MathIsNormalText,
            FontAttr.MathLineBreak,
            FontAttr.MathScript,
            FontAttr.MathStyle,
            FontAttr.MathIsAlignmentPoint };

        /// <summary>
        /// Specifies what attributes to preserve during <see cref="AcceptFormatRevision"/>
        /// in presence of changed style when dealing with regular runs.
        /// </summary>
        private static readonly int[] gRevisionPreservedRunAttributes = new int[] {
            RevisionAttr.MoveFromRevision,
            RevisionAttr.MoveToRevision,
            RevisionAttr.InsertRevision,
            RevisionAttr.DeleteRevision };

        /// <summary>
        /// Specifies all font name keys.
        /// </summary>
        internal static readonly int[] FontNameAttributes = new int[] {
            FontAttr.NameAscii,
            FontAttr.NameBi,
            FontAttr.NameFarEast,
            FontAttr.NameOther };

        /// <summary>
        /// Specifies all locale keys.
        /// </summary>
        internal static readonly int[] LocaleAttributes = new int[] {
            FontAttr.LocaleId,
            FontAttr.LocaleIdBi,
            FontAttr.LocaleIdFarEast };

        /// <summary>
        /// Non-collapsible font attributes, <see cref="AttrCollection.IsIgnoredOnCollapse"/>.
        /// </summary>
        internal static readonly int[] NonCollapsibleFontKeys = { FontAttr.RsidRPr, FontAttr.RsidR, FontAttr.Sys_Symbol };

        /// <summary>
        /// Toggle properties, as specified in ECMA-376-1:2016, section 17.7.3.
        /// </summary>
        internal static readonly int[] ToggleAttributes =
        {
            FontAttr.Bold,
            FontAttr.BoldBi,
            FontAttr.AllCaps,
            FontAttr.Emboss,
            FontAttr.Italic,
            FontAttr.ItalicBi,
            FontAttr.Engrave,
            FontAttr.Outline,
            FontAttr.Shadow,
            FontAttr.SmallCaps,
            FontAttr.StrikeThrough,
            FontAttr.Hidden
        };

        /// <summary>
        /// Specifies what attributes to preserve during <see cref="AcceptFormatRevision"/>
        /// in presence of changed style when dealing with symbolic runs.
        /// </summary>
        private static readonly int[] gRevisionPreservedSymbolAttributes = new int[] {
            RevisionAttr.MoveFromRevision,
            RevisionAttr.MoveToRevision,
            RevisionAttr.InsertRevision,
            RevisionAttr.DeleteRevision,
            FontAttr.NameAscii,
            FontAttr.NameOther };

        private static readonly RunPr gDefaults = InitDefaults();

        private const string TimesNewRoman = "Times New Roman";
    }
}
