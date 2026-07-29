// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2005 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using Aspose.Bidi;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Drawing.Fonts;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Colors.Modifiers;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Fonts;
using Aspose.Words.Themes;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Contains font attributes (font name, font size, color, and so on) for an object.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fonts/">Working with Fonts</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>You do not create instances of the <see cref="Font"/> class directly. You just use
    /// <see cref="Font"/> to access the font properties of the various objects such as <see cref="Run"/>,
    /// <see cref="Paragraph"/>, <see cref="Aspose.Words.Style"/>, <see cref="DocumentBuilder"/>.</p>
    /// </remarks>
    public class Font : IBorderAttrSource, IShadingAttrSource, IFillable
    {

        /// <summary>
        /// Ctor for some unit testing. Creates a <see cref="Font"/> object on a detached collection of attributes.
        /// </summary>
        internal static Font MakeFont()
        {
            return MakeFont(new RunPr(), null);
        }

        /// <summary>
        /// Ctor for some unit testing. Creates a <see cref="Font"/> object on a detached collection of attributes.
        /// </summary>
        internal static Font MakeFont(IRunAttrSource parent)
        {
            return MakeFont(parent, null);
        }

        /// <summary>
        /// Ctor for some unit testing. Creates a <see cref="Font"/> object on a detached collection of attributes.
        /// </summary>
        internal static Font MakeFont(IRunAttrSource parent, DocumentBase doc)
        {
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            Font font = new Font(parent, doc);
            font = MemoryManagement.ExtendLifetime(font, parent);
            return font;
        }

        /// <summary>
        /// Ctor for normal use.
        /// </summary>
        /// <param name="parent">The object that provides the attributes.</param>
        /// <param name="doc">Used to retrieve style and theme, can be null.</param>
        internal Font(IRunAttrSource parent, DocumentBase doc)
        {
            mParent = parent;
            mDoc = doc;
        }

        /// <summary>
        /// Resets to default font formatting.
        /// </summary>
        /// <remarks>
        /// <p>Removes all font formatting specified explicitly on the object from which
        /// <see cref="Font"/> was obtained so the font formatting will be inherited from
        /// the appropriate parent.</p>
        /// </remarks>
        public void ClearFormatting()
        {
            mParent.ClearRunAttrs();
        }

        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        /// <remarks>
        /// <p>When getting, returns <see cref="NameAscii"/>.</p>
        /// <p>When setting, sets <see cref="NameAscii"/>, <see cref="NameBi"/>, <see cref="NameFarEast"/>
        /// and <see cref="NameOther"/> to the specified value.</p>
        /// </remarks>
        public string Name
        {
            get
            {
                CharacterCategory characterCategory = GetCharacterCategory();
                switch (characterCategory)
                {
                    case CharacterCategory.Other:
                        return NameOther;

                    case CharacterCategory.FarEast:
                        return NameFarEast;

                    case CharacterCategory.ComplexScript:
                        return NameBi;

                    default:
                        return NameAscii;
                }
            }
            set
            {
                ArgumentUtil.CheckHasChars(value, "value");
                // Have to be on separate lines for autoporting to work.
                NameAscii = value;
                NameBi = value;
                NameFarEast = value;
                NameOther = value;
            }
        }

        /// <summary>
        /// Returns or sets the font used for Latin text (characters with character codes from 0 (zero) through 127).
        /// </summary>
        /// <seealso cref="Name"/>
        public string NameAscii
        {
            get { return ComplexFontName.Resolve(FetchAttr(FontAttr.NameAscii), Theme); }
            set
            {
                ArgumentUtil.CheckHasChars(value, "value");
                mParent.SetRunAttr(FontAttr.NameAscii, ComplexFontName.FromName(value));
            }
        }

        /// <summary>
        /// Returns or sets the name of the font in a right-to-left language document.
        /// </summary>
        /// <seealso cref="Name"/>
        public string NameBi
        {
            get { return ComplexFontName.Resolve(FetchAttr(FontAttr.NameBi), Theme); }
            set
            {
                ArgumentUtil.CheckHasChars(value, "value");
                mParent.SetRunAttr(FontAttr.NameBi, ComplexFontName.FromName(value));
            }
        }

        /// <summary>
        /// Returns or sets an East Asian font name.
        /// </summary>
        /// <seealso cref="Name"/>
        public string NameFarEast
        {
            get { return ComplexFontName.Resolve(FetchAttr(FontAttr.NameFarEast), Theme); }
            set
            {
                ArgumentUtil.CheckHasChars(value, "value");
                mParent.SetRunAttr(FontAttr.NameFarEast, ComplexFontName.FromName(value));
            }
        }

        /// <summary>
        /// Returns or sets the font used for characters with character codes from 128 through 255.
        /// </summary>
        /// <seealso cref="Name"/>
        public string NameOther
        {
            get { return ComplexFontName.Resolve(FetchAttr(FontAttr.NameOther), Theme); }
            set
            {
                ArgumentUtil.CheckHasChars(value, "value");
                mParent.SetRunAttr(FontAttr.NameOther, ComplexFontName.FromName(value));
            }
        }

        /// <summary>
        /// Gets or sets the theme font in the applied font scheme that is associated with this <see cref="Font"/> object.
        /// </summary>
        /// <dev>
        /// <p>When getting, returns <see cref="ThemeFont"/> depending on character category of the text.</p>
        /// <p>When setting, sets <see cref="ThemeFontAscii"/>, <see cref="ThemeFontBi"/>,
        /// <see cref="ThemeFontFarEast"/> and <see cref="ThemeFontOther"/> to the specified value.</p>
        /// </dev>
        public ThemeFont ThemeFont
        {
            get
            {
                CharacterCategory characterCategory = GetCharacterCategory();
                switch (characterCategory)
                {
                    case CharacterCategory.Other:
                        return ThemeFontOther;

                    case CharacterCategory.FarEast:
                        return ThemeFontFarEast;

                    case CharacterCategory.ComplexScript:
                        return ThemeFontBi;

                    default:
                        return ThemeFontAscii;
                }
            }
            set
            {
                // Seems Word logic is quite complex here,
                // but we just apply the value with a corresponding region for a while.
                ThemeFontAscii = value;
                ThemeFontBi = value;
                ThemeFontFarEast = value;
                ThemeFontOther = value;
            }
        }

        /// <summary>
        /// Gets or sets the theme font used for Latin text (characters with character codes from 0 (zero) through 127)
        /// in the applied font scheme that is associated with this <see cref="Font"/> object.
        /// </summary>
        public ThemeFont ThemeFontAscii
        {
            get { return ((ComplexFontName)FetchAttr(FontAttr.NameAscii)).ThemeFont; }
            set
            {
                mParent.SetRunAttr(FontAttr.NameAscii, (value == ThemeFont.None)
                    ? ComplexFontName.FromName(NameAscii)
                    // Seems Word applies HAnsi here, but such logic is not entirely clear,
                    // so we apply Ascii, for a while.
                    : ComplexFontName.FromTheme(value, ThemeFontCore.Ascii));
            }
        }

        /// <summary>
        /// Gets or sets the East Asian theme font in the applied font scheme that is associated with this <see cref="Font"/> object.
        /// </summary>
        public ThemeFont ThemeFontFarEast
        {
            get { return ((ComplexFontName)FetchAttr(FontAttr.NameFarEast)).ThemeFont; }
            set
            {
                mParent.SetRunAttr(FontAttr.NameFarEast, (value == ThemeFont.None)
                    ? ComplexFontName.FromName(NameFarEast)
                    : ComplexFontName.FromTheme(value, ThemeFontCore.EastAsia));
            }
        }

        /// <summary>
        /// Gets or sets the theme font used for characters with character codes from 128 through 255
        /// in the applied font scheme that is associated with this <see cref="Font"/> object.
        /// </summary>
        public ThemeFont ThemeFontOther
        {
            get { return ((ComplexFontName)FetchAttr(FontAttr.NameOther)).ThemeFont; }
            set
            {
                mParent.SetRunAttr(FontAttr.NameOther, (value == ThemeFont.None)
                    ? ComplexFontName.FromName(NameOther)
                    : ComplexFontName.FromTheme(value, ThemeFontCore.HAnsi));
            }
        }

        /// <summary>
        /// Gets or sets the theme font in the applied font scheme that is associated with this <see cref="Font"/> object
        /// in a right-to-left language document.
        /// </summary>
        public ThemeFont ThemeFontBi
        {
            get { return ((ComplexFontName)FetchAttr(FontAttr.NameBi)).ThemeFont; }
            set
            {
                mParent.SetRunAttr(FontAttr.NameBi, (value == ThemeFont.None)
                        ? ComplexFontName.FromName(NameBi)
                        : ComplexFontName.FromTheme(value, ThemeFontCore.Bidi));
            }
        }

        /// <summary>
        /// Gets the font type which shall be used to format any ambiguous characters in the current run.
        /// </summary>
        internal CharacterCategory CharacterCategoryHint
        {
            get { return (CharacterCategory)FetchAttr(FontAttr.CharacterCategoryHint); }
        }

        /// <summary>
        /// Gets or sets the font size in points.
        /// </summary>
        public double Size
        {
            get { return ConvertUtilCore.HalfPointToPoint((int)FetchAttr(FontAttr.Size)); }
            set { mParent.SetRunAttr(FontAttr.Size, ConvertUtilCore.PointToHalfPoint(value)); }
        }

        /// <summary>
        /// Gets or sets the font size in points used in a right-to-left document.
        /// </summary>
        public double SizeBi
        {
            get { return ConvertUtilCore.HalfPointToPoint((int)FetchAttr(FontAttr.SizeBi)); }
            set { mParent.SetRunAttr(FontAttr.SizeBi, ConvertUtilCore.PointToHalfPoint(value)); }
        }

        /// <summary>
        /// True if the font is formatted as bold.
        /// </summary>
        public bool Bold
        {
            get { return GetBool(FontAttr.Bold); }
            set { SetBool(FontAttr.Bold, value); }
        }

        /// <summary>
        /// True if the right-to-left text is formatted as bold.
        /// </summary>
        public bool BoldBi
        {
            get { return GetBool(FontAttr.BoldBi); }
            set { SetBool(FontAttr.BoldBi, value); }
        }

        /// <summary>
        /// True if the font is formatted as italic.
        /// </summary>
        public bool Italic
        {
            get { return GetBool(FontAttr.Italic); }
            set { SetBool(FontAttr.Italic, value); }
        }

        /// <summary>
        /// True if the right-to-left text is formatted as italic.
        /// </summary>
        public bool ItalicBi
        {
            get { return GetBool(FontAttr.ItalicBi); }
            set { SetBool(FontAttr.ItalicBi, value); }
        }

        /// <summary>
        /// Gets font style.
        /// </summary>
        /// <remarks>
        /// This property concerns only left-to-right text.
        /// </remarks>
        internal FontStyle FontStyle
        {
            get
            {
                FontStyle result = FontStyle.Regular;
                if (Bold)
                    result |= FontStyle.Bold;
                if (Italic)
                    result |= FontStyle.Italic;

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the color of the font.
        /// </summary>
        public System.Drawing.Color Color
        {
            get { return ColorInternal.ToNativeColor(); }
            set { ColorInternal = DrColor.FromNativeColor(value); }
        }

        /// <summary>
        /// Gets or sets the theme color in the applied color scheme that is associated with this <see cref="Font"/> object.
        /// </summary>
        public ThemeColor ThemeColor
        {
            get
            {
                return ThemeColorConverter.FromString((string)FetchAttr(FontAttr.ThemeColor));
            }
            set
            {
                // Remove theme tint and shade to mimic Word VBA.
                mParent.RemoveRunAttr(FontAttr.ThemeTint);
                mParent.RemoveRunAttr(FontAttr.ThemeShade);

                if (value == ThemeColor.None)
                {
                    mParent.RemoveRunAttr(FontAttr.ThemeColor);
                }
                else
                {
                    mParent.SetRunAttr(FontAttr.ThemeColor, ThemeColorConverter.ToString(value));

                    // Setting ThemeColor in Word VBA also sets a corresponding resolved RGB color,
                    // but we decided just to remove non-theme color for a while.
                    mParent.RemoveRunAttr(FontAttr.Color);
                }
            }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens a color.
        /// </summary>
        /// <remarks>
        /// <para> The allowed values are in range from -1 (darkest) to 1 (lightest) for this property.</para>
        /// <para>Zero (0) is neutral.</para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Throw if set this property to a value less than -1 or more than 1.</exception>
        /// <exception cref="InvalidOperationException">Throw if set this property for <see cref="Font"/> object with non-theme colors.</exception>
        public double TintAndShade
        {
            get
            {
                // Note, Word stores inverted value (1 - tint) in the document.
                string value = (string)FetchAttr(FontAttr.ThemeTint);
                if (StringUtil.HasChars(value))
                    return 1.0d - ((double)FormatterPal.ParseHex(value) / 255);

                // Note, Word stores inverted value (1 + shade) in the document.
                value = (string)FetchAttr(FontAttr.ThemeShade);
                if (StringUtil.HasChars(value))
                    return -1.0d - ((double)FormatterPal.ParseHex(value) / -255);

                return 0.0d;
            }
            set
            {
                // Despite https://docs.microsoft.com/en-us/office/vba/api/excel.font.tintandshade
                // says that "This property works for both theme colors and non-theme colors.",
                // in fact it does not work in Word VBA for non-theme colors.
                if (ThemeColor == ThemeColor.None)
                    throw new InvalidOperationException("TintAndShade cannot be applied to a non-theme color.");

                ArgumentUtil.CheckRangeInclusive(value, -1.0d, 1.0d, "TintAndShade");

                if (MathUtil.IsZero(value))
                {
                    mParent.RemoveRunAttr(FontAttr.ThemeTint);
                    mParent.RemoveRunAttr(FontAttr.ThemeShade);
                }
                else
                {
                    if (value > 0)
                    {
                        // Note, Word stores inverted value (1 - tint) in the document.
                        string tint = FormatterPal.IntToStrX2((int)((1 - value) * 255));
                        mParent.SetRunAttr(FontAttr.ThemeTint, tint);
                        mParent.RemoveRunAttr(FontAttr.ThemeShade);
                    }
                    else
                    {
                        // Note, Word stores inverted value (1 + shade) in the document.
                        string shade = FormatterPal.IntToStrX2((int)((-1 - value) * -255));
                        mParent.SetRunAttr(FontAttr.ThemeShade, shade);
                        mParent.RemoveRunAttr(FontAttr.ThemeTint);
                    }
                }
                // Word also always updates a corresponding RGB color by resolving theme color with the new TintAndShade,
                // but we decided to remove the corresponding RGB color for a while, so nothing to update.
            }
        }

        /// <summary>
        /// Returns the present calculated color of the text (black or white) to be used for 'auto color'.
        /// If the color is not 'auto' then returns <see cref="Color"/>.
        /// </summary>
        /// <remarks>
        /// <p>
        /// When text has 'automatic color', the actual color of text is calculated automatically
        /// so that it is readable against the background color. As you change the background color,
        /// the text color will automatically switch to black or white in MS Word to maximize legibility.</p>
        /// </remarks>
        public Color AutoColor
        {
            get
            {
                // If color is not 'auto', then simply return it.
                if (Color != Color.Empty)
                    return Color;

                Shading shading = InlineHelper.TopVisibleShading(mParent);

                if (shading == null)
                {
                    // WORDSNET-26032 Word returns color of style font reference inside Dml shape.
                    IInline inline = mParent as IInline;
                    if ((inline != null) && (inline.ParentParagraph_IInline != null))
                    {
                        Shape parentShape = inline.ParentParagraph_IInline.FirstNonMarkupParentNode as Shape;
                        if ((parentShape != null) && parentShape.IsDmlShape)
                        {
                            if ((parentShape.DmlShape.Style != null) && !parentShape.DmlShape.Style.FontReference.Color.IsEmpty)
                            {
                                DrColor drColor = parentShape.DmlShape.Style.FontReference.Color.CreateDrColor(Theme, null);
                                return drColor.ToNativeColor();
                            }
                        }
                    }

                    return Color.Black;
                }

                return (FontColorResolver.ResolveAutoFontColorOnShading(shading) == DrColor.Black)
                           ? Color.Black
                           : Color.White;
            }
        }

        internal DrColor ColorInternal
        {
            get { return (DrColor)FetchAttr(FontAttr.Color); }
            set
            {
                mParent.SetRunAttr(FontAttr.Color, value);

                // Remove theme color when customer applies basic color.
                if (mParent.GetDirectRunAttr(FontAttr.ThemeColor) != null)
                    mParent.RemoveRunAttr(FontAttr.ThemeColor);
                if (mParent.GetDirectRunAttr(FontAttr.ThemeShade) != null)
                    mParent.RemoveRunAttr(FontAttr.ThemeShade);
                if (mParent.GetDirectRunAttr(FontAttr.ThemeTint) != null)
                    mParent.RemoveRunAttr(FontAttr.ThemeTint);
            }
        }

        /// <summary>
        /// True if the font is formatted as strikethrough text.
        /// </summary>
        public bool StrikeThrough
        {
            get { return GetBool(FontAttr.StrikeThrough); }
            set { SetBool(FontAttr.StrikeThrough, value); }
        }

        /// <summary>
        /// True if the font is formatted as double strikethrough text.
        /// </summary>
        public bool DoubleStrikeThrough
        {
            get { return GetBool(FontAttr.DoubleStrikeThrough); }
            set { SetBool(FontAttr.DoubleStrikeThrough, value); }
        }

        /// <summary>
        /// True if the font is formatted as shadowed.
        /// </summary>
        public bool Shadow
        {
            get { return GetBool(FontAttr.Shadow); }
            set { SetBool(FontAttr.Shadow, value); }
        }

        /// <summary>
        /// True if the font is formatted as outline.
        /// </summary>
        public bool Outline
        {
            get { return GetBool(FontAttr.Outline); }
            set { SetBool(FontAttr.Outline, value); }
        }

        /// <summary>
        /// True if the font is formatted as embossed.
        /// </summary>
        public bool Emboss
        {
            get { return GetBool(FontAttr.Emboss); }
            set { SetBool(FontAttr.Emboss, value); }
        }

        /// <summary>
        /// True if the font is formatted as engraved.
        /// </summary>
        public bool Engrave
        {
            get { return GetBool(FontAttr.Engrave); }
            set { SetBool(FontAttr.Engrave, value); }
        }

        /// <summary>
        /// True if the font is formatted as superscript.
        /// </summary>
        public bool Superscript
        {
            get { return (VerticalAlignment == RunVerticalAlignment.Superscript); }
            set
            {
                // WORDSNET-2004 We shouldn't set Superscript to false if current vertical alignment is subscript,
                // because by doing so we would change subscript to baseline and reset the Subscript property by modifying
                // the Superscript property. This behavior is confusing and it is better to keep vertical alignment unchanged.
                if (!value && Subscript)
                    return;

                VerticalAlignment = (value) ? RunVerticalAlignment.Superscript : RunVerticalAlignment.Baseline;
            }
        }

        /// <summary>
        /// True if the font is formatted as subscript.
        /// </summary>
        public bool Subscript
        {
            get { return (VerticalAlignment == RunVerticalAlignment.Subscript); }
            set
            {
                // WORDSNET-2004 We shouldn't set Subscript to false if current vertical alignment is superscript,
                // because by doing so we would change superscript to baseline and reset the Superscript property by modifying
                // the Subscript property. This behavior is confusing and it is better to keep vertical alignment unchanged.
                if (!value && Superscript)
                    return;

                VerticalAlignment = (value) ? RunVerticalAlignment.Subscript : RunVerticalAlignment.Baseline;
            }
        }

        /// <summary>
        /// True if the font is formatted as small capital letters.
        /// </summary>
        public bool SmallCaps
        {
            get { return GetBool(FontAttr.SmallCaps); }
            set { SetBool(FontAttr.SmallCaps, value); }
        }

        /// <summary>
        /// True if the font is formatted as all capital letters.
        /// </summary>
        public bool AllCaps
        {
            get { return GetBool(FontAttr.AllCaps); }
            set { SetBool(FontAttr.AllCaps, value); }
        }

        /// <summary>
        /// True if the font is formatted as hidden text.
        /// </summary>
        public bool Hidden
        {
            get { return GetBool(FontAttr.Hidden); }
            set { SetBool(FontAttr.Hidden, value); }
        }

        /// <summary>
        /// Gets or sets the type of underline applied to the font.
        /// </summary>
        public Underline Underline
        {
            get { return (Underline)FetchAttr(FontAttr.Underline); }
            set { mParent.SetRunAttr(FontAttr.Underline, value); }
        }

        /// <summary>
        /// Gets or sets the color of the underline applied to the font.
        /// </summary>
        public System.Drawing.Color UnderlineColor
        {
            get { return UnderlineColorInternal.ToNativeColor(); }
            set { UnderlineColorInternal = DrColor.FromNativeColor(value); }
        }

        /// <summary>
        /// Gets or sets the spacing type of the numeral being displayed.
        /// </summary>
        public NumSpacing NumberSpacing
        {
            get { return (NumSpacing)FetchAttr(FontAttr.OpenTypeNumSpacing); }
            set { mParent.SetRunAttr(FontAttr.OpenTypeNumSpacing, value); }
        }

        internal DrColor UnderlineColorInternal
        {
            get { return (DrColor)FetchAttr(FontAttr.UnderlineColor); }
            set { mParent.SetRunAttr(FontAttr.UnderlineColor, value); }
        }

        /// <summary>
        /// Gets or sets character width scaling in percent.
        /// </summary>
        public int Scaling
        {
            get { return (int)FetchAttr(FontAttr.Scaling); }
            set { mParent.SetRunAttr(FontAttr.Scaling, value); }
        }

        /// <summary>
        /// Returns or sets the spacing (in points) between characters .
        /// </summary>
        public double Spacing
        {
            get { return ConvertUtilCore.TwipToPoint(SpacingRaw); }
            set { SpacingRaw = ConvertUtilCore.PointToTwip(value); }
        }

        /// <summary>
        /// Returns or sets the spacing (in twips) between characters .
        /// </summary>
        internal int SpacingRaw
        {
            get { return (int)FetchAttr(FontAttr.Spacing); }
            set { mParent.SetRunAttr(FontAttr.Spacing, value); }
        }

        /// <summary>
        /// Returns line spacing of this font (in points).
        /// </summary>
        public double LineSpacing
        {
            get
            {
                CharacterCategory characterCategory = GetCharacterCategory();
                FontStyle fontStyle = GetFontStyle(characterCategory);
                float size = Bidi ? (float)this.SizeBi : (float)this.Size;

                // Line spacing depends from font family, font style (see method "FontFamily.GetLineSpacing"
                // in the "System.Drawing") and font size (see "DesignUnitsToPoints" method of the "TTFont" class).
                DrFont font = FontProvider.FetchDrFont(this.Name, size, fontStyle);

                // Line spacing depends from the font metrics. "PrinterMetrics" has to be used according
                // to compatibility option.
                // FOSS

                return font.LineSpacingPoints;
            }
        }

        /// <summary>
        /// Gets or sets the position of text (in points) relative to the base line.
        /// A positive number raises the text, and a negative number lowers it.
        /// </summary>
        public double Position
        {
            get { return ConvertUtilCore.HalfPointToPoint((int)FetchAttr(FontAttr.Position)); }
            set { mParent.SetRunAttr(FontAttr.Position, ConvertUtilCore.PointToHalfPoint(value)); }
        }

        /// <summary>
        /// Gets or sets the font size at which kerning starts.
        /// </summary>
        public double Kerning
        {
            get { return ConvertUtilCore.HalfPointToPoint((int)FetchAttr(FontAttr.Kerning)); }
            set { mParent.SetRunAttr(FontAttr.Kerning, ConvertUtilCore.PointToHalfPoint(value)); }
        }

        /// <summary>
        /// Gets or sets the highlight (marker) color.
        /// </summary>
        public System.Drawing.Color HighlightColor
        {
            get { return HighlightColorInternal.ToNativeColor(); }
            set { HighlightColorInternal = DrColor.FromNativeColor(value); }
        }

        internal DrColor HighlightColorInternal
        {
            get { return (DrColor)FetchAttr(FontAttr.HighlightColor); }
            set { mParent.SetRunAttr(FontAttr.HighlightColor, value); }
        }

        /// <summary>
        /// Gets or sets the font animation effect.
        /// </summary>
        public TextEffect TextEffect
        {
            get { return (TextEffect)FetchAttr(FontAttr.TextEffect); }
            set { mParent.SetRunAttr(FontAttr.TextEffect, value); }
        }

        /// <summary>
        /// Gets fill formatting for the <see cref="Font"/>.
        /// </summary>
        public Fill Fill
        {
            get
            {
                if (mFill == null)
                    mFill = new Fill(this);

                return mFill;
            }
        }

        /// <summary>
        /// Checks if particular DrawingML text effect is applied.
        /// </summary>
        /// <param name="dmlEffectType">DrawingML text effect type.</param>
        /// <returns><c>true</c> if particular DrawingML text effect is applied.</returns>
        public bool HasDmlEffect(TextDmlEffect dmlEffectType)
        {
            switch (dmlEffectType)
            {
                case TextDmlEffect.Glow:
                    return HasDirectAttr(FontAttr.EffectGlow);
                case TextDmlEffect.Fill:
                    return HasDirectAttr(FontAttr.EffectFill);
                case TextDmlEffect.Shadow:
                    return HasDirectAttr(FontAttr.EffectShadow);
                case TextDmlEffect.Outline:
                    return HasDirectAttr(FontAttr.EffectOutline);
                case TextDmlEffect.Effect3D:
                    return (HasDirectAttr(FontAttr.EffectProps3D) || HasDirectAttr(FontAttr.EffectScene3D));
                case TextDmlEffect.Reflection:
                    return HasDirectAttr(FontAttr.EffectReflection);
                default:
                    throw new InvalidOperationException("Unexpected TextDmlEffect.");
            }
        }

        /// <summary>
        /// Specifies whether the contents of this run shall have right-to-left characteristics.
        /// </summary>
        /// <remarks>
        /// <para>This property, when on, shall not be used with strongly left-to-right text. Any behavior under that condition is unspecified.
        /// This property, when off, shall not be used with strong right-to-left text. Any behavior under that condition is unspecified.</para>
        ///
        /// <para>When the contents of this run are displayed, all characters shall be treated as complex script characters for formatting
        /// purposes. This means that <see cref="BoldBi"/>, <see cref="ItalicBi"/>, <see cref="SizeBi"/> and a corresponding font name
        /// will be used when rendering this run.</para>
        ///
        /// <para>Also, when the contents of this run are displayed, this property acts as a right-to-left override for characters
        /// which are classified as "weak types" and "neutral types".</para>
        /// </remarks>
        public bool Bidi
        {
            get { return GetBool(FontAttr.Bidi); }
            set { SetBool(FontAttr.Bidi, value); }
        }

        /// <summary>
        /// Specifies whether the contents of this run shall be treated as complex script text regardless
        /// of their Unicode character values when determining the formatting for this run.
        /// </summary>
        public bool ComplexScript
        {
            get { return GetBool(FontAttr.ComplexScript); }
            set { SetBool(FontAttr.ComplexScript, value); }
        }

        /// <summary>
        /// True when the formatted characters are not to be spell checked.
        /// </summary>
        public bool NoProofing
        {
            get { return GetBool(FontAttr.NoProofing); }
            set { SetBool(FontAttr.NoProofing, value); }
        }

        /// <summary>
        /// Gets or sets the locale identifier (language) of the formatted characters.
        /// </summary>
        /// <remarks>
        /// For the list of locale identifiers see https://msdn.microsoft.com/en-us/library/cc233965.aspx
        /// </remarks>
        public int LocaleId
        {
            get { return (int)FetchAttr(FontAttr.LocaleId); }
            set { mParent.SetRunAttr(FontAttr.LocaleId, value); }
        }

        /// <summary>
        /// Gets or sets the locale identifier (language) of the formatted right-to-left characters.
        /// </summary>
        /// <remarks>
        /// For the list of locale identifiers see https://msdn.microsoft.com/en-us/library/cc233965.aspx
        /// </remarks>
        public int LocaleIdBi
        {
            get { return (int)FetchAttr(FontAttr.LocaleIdBi); }
            set { mParent.SetRunAttr(FontAttr.LocaleIdBi, value); }
        }

        /// <summary>
        /// Gets or sets the locale identifier (language) of the formatted Asian characters.
        /// </summary>
        /// <remarks>
        /// For the list of locale identifiers see https://msdn.microsoft.com/en-us/library/cc233965.aspx
        /// </remarks>
        public int LocaleIdFarEast
        {
            get { return (int)FetchAttr(FontAttr.LocaleIdFarEast); }
            set { mParent.SetRunAttr(FontAttr.LocaleIdFarEast, value); }
        }

        /// <summary>
        /// Returns a <see cref="Aspose.Words.Border"/> object that specifies border for the font.
        /// </summary>
        public Border Border
        {
            get
            {
                //<<GetOrCreateComplexAttr>> pattern
                Border border = (Border)mParent.GetDirectRunAttr(FontAttr.Border);
                if (border == null)
                {
                    Font parent = this;
                    border = new Border(parent, FontAttr.Border);
                    border = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(border, parent);
                    mParent.SetRunAttr(FontAttr.Border, border);
                }
                return border;
            }
        }

        /// <summary>
        /// Returns a <see cref="Aspose.Words.Shading"/> object that refers to the shading formatting for the font.
        /// </summary>
        public Shading Shading
        {
            get
            {
                //<<GetOrCreateComplexAttr>> pattern
                Shading shading = (Shading)mParent.GetDirectRunAttr(FontAttr.Shading);
                if (shading == null)
                {
                    Font parent = this;
                    shading = new Shading(this, FontAttr.Shading);
                    shading = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(shading, parent);
                    mParent.SetRunAttr(FontAttr.Shading, shading);
                }
                return shading;
            }
        }

        /// <summary>
        /// Gets or sets the character style applied to this formatting.
        /// </summary>
        public Style Style
        {
            get { return Styles.FetchByIstd(Istd, StyleIndex.DefaultParagraphFont); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if ((Styles != null) && (value.Document != Styles.Document))
                    throw new ArgumentException("This style belongs to a different document.");

                if (value.Type != StyleType.Character)
                    throw new ArgumentException("This style is not a character style.");

                Istd = value.Istd;
            }
        }

        /// <summary>
        /// Gets or sets the name of the character style applied to this formatting.
        /// </summary>
        public string StyleName
        {
            get { return Style.Name; }
            set { Style = Styles.FetchByName(value); }
        }

        /// <summary>
        /// Gets or sets the locale independent style identifier of the character style applied to this formatting.
        /// </summary>
        public StyleIdentifier StyleIdentifier
        {
            get { return Style.StyleIdentifier; }
            set { Style = Styles.FetchBySti(value); }
        }

        /// <summary>
        /// Specifies whether the current font should use the document grid characters per line settings
        /// when laying out.
        /// </summary>
        public bool SnapToGrid
        {
            get { return GetBool(FontAttr.SnapToGrid); }
            set { SetBool(FontAttr.SnapToGrid, value); }
        }

        /// <summary>
        /// Gets or sets the emphasis mark applied to this formatting.
        /// </summary>
        public EmphasisMark EmphasisMark
        {
            get { return (EmphasisMark)FetchAttr(FontAttr.EmphasisMark); }
            set { mParent.SetRunAttr(FontAttr.EmphasisMark, value); }
        }

        internal int Istd
        {
            get
            {
                object istd = mParent.GetDirectRunAttr(FontAttr.Istd);
                //When getting the style of the run, don't try to retrieve the value from the parent
                //since the parent is the style and it will result in an endless loop.
                return (istd != null) ? (int)istd : (int)RunPr.FetchDefaultAttr(FontAttr.Istd);
            }
            set
            {
                mParent.SetRunAttr(FontAttr.Istd, value);
            }
        }

        internal RunVerticalAlignment VerticalAlignment
        {
            get { return (RunVerticalAlignment)FetchAttr(FontAttr.VerticalAlignment); }
            set { mParent.SetRunAttr(FontAttr.VerticalAlignment, value); }
        }

        /// <summary>
        /// Returns the font name to use for the specified character category.
        /// </summary>
        internal string GetFontName(CharacterCategory category)
        {
            //Duplication of WordUtil.GetFontNameByCategory() - code moved here for speed.
            switch (category)
            {
                case CharacterCategory.Ascii:
                    return NameAscii;
                case CharacterCategory.ComplexScript:
                    return NameBi;
                case CharacterCategory.FarEast:
                    return NameFarEast;
                case CharacterCategory.Other:
                default:
                    return NameOther;
            }
        }

        /// <summary>
        /// Returns locale ID for the specified character category.
        /// </summary>
        internal int GetLocaleId(CharacterCategory category)
        {
            switch (category)
            {
                case CharacterCategory.ComplexScript:
                    return LocaleIdBi;
                case CharacterCategory.FarEast:
                    return LocaleIdFarEast;
                default:
                    return LocaleId;
            }
        }

        /// <summary>
        /// Indicates that parent inline has character category hint set.
        /// </summary>
        internal bool HasCharacterCategoryHint
        {
            get { return InlineHelper.HasCharacterCategoryHint(mParent); }
        }

        /// <summary>
        /// Gets the actual size of the font as it appears in the layout.
        /// The actual size could be smaller than the specified font size
        /// for small caps and/or subscript/superscript font.
        /// </summary>
        /// <param name="isIgnoreSmallCaps">We sometimes need to ignore the fact that the font is small caps.
        /// This is the case for uppercase characters as well as for calculating underlines.</param>
        internal float GetActualSize(bool isIgnoreSmallCaps)
        {
            double result = Size;

            if (!isIgnoreSmallCaps && SmallCaps)
                result *= SmallCapsSizeFactor;

            if (Subscript || Superscript)
                result *= SubscriptSizeFactor;

            return (float)result;
        }

        /// <summary>
        /// Gets the attribute from the attribute collection or from one of the parents.
        /// RK I'm not sure this should actually be in Font. But where else?
        /// </summary>
        internal object FetchAttr(int key)
        {
            Document doc = (mDoc != null) ? mDoc.FetchDocumentOrGlossaryMain() : null;

            RevisionsView view = (doc != null)
                ? doc.RevisionsView
                : RevisionsView.Original;

            // MF Refactored to let AE access this method.
            return InlineHelper.FetchAttr(mParent, key, view);
        }

        /// <summary>
        /// Checks whether direct attribute is specified in the collection
        /// </summary>
        internal bool HasDirectAttr(int key)
        {
            object value = mParent.GetDirectRunAttr(key);
            return value != null;
        }

        /// <summary>
        /// Returns <see cref="FontStyle"/> considering <paramref name="charCategory"/>.
        /// </summary>
        internal FontStyle GetFontStyle(CharacterCategory charCategory)
        {
            bool isBold;
            bool isItalic;

            if ((charCategory == CharacterCategory.FarEast) ||
                ((charCategory == CharacterCategory.Other) && (CharacterCategoryHint == CharacterCategory.FarEast)))
            {
                isBold = BoldBi;
                isItalic = ItalicBi;
            }
            else
            {
                isBold = Bold;
                isItalic = Italic;
            }

            FontStyle result;
            if (isBold)
            {
                result = FontStyle.Bold;
                if (isItalic)
                    result |= FontStyle.Italic;
            }
            else
            {
                result = isItalic ? FontStyle.Italic : FontStyle.Regular;
            }

            return result;
        }

        private bool GetBool(int key)
        {
            return InlineHelper.GetBool(mParent, key);
        }

        private void SetBool(int key, bool value)
        {
            mParent.SetRunAttr(key, AttrBoolEx.FromBool(value));
        }

        /// <summary>
        /// Determines current character category.
        /// [MS-OE376]: Office Implementation Information for ECMA-376 Standards Support.
        /// 2.1.89 Part 4 Section 2.3.2.24, rFonts (Run Fonts).
        /// </summary>
        private CharacterCategory GetCharacterCategory()
        {
            Run run = ParentRun;
            if ((run == null) || !StringUtil.HasChars(run.Text))
                return CharacterCategory.Ascii;

            if (ComplexScript || Bidi)
                return CharacterCategory.ComplexScript;

            // Special case from spec.
            if ((NameAscii == NameOther) && (NameFarEast == "Times New Roman") )
                return CharacterCategory.Ascii;

            CharacterCategory hint = (run.RunPr[FontAttr.CharacterCategoryHint] != null)
                ? (CharacterCategory)run.RunPr[FontAttr.CharacterCategoryHint]
                : CharacterCategory.Other;
            Language langEastAsia = (Language)LocaleIdFarEast;

            int ch = run.Text[0];
            UnicodeBlocks block = UnicodeUtil.GetUnicodeBlock(ch);

            if ((block == UnicodeBlocks.BasicLatin) || (block == UnicodeBlocks.ArabicPresentationFormsA) ||
                (block == UnicodeBlocks.ArabicPresentationFormsB) || (block == UnicodeBlocks.Hebrew) ||
                (block == UnicodeBlocks.Arabic) || (block == UnicodeBlocks.Syriac) ||
                (block == UnicodeBlocks.ArabicSupplement) || (block == UnicodeBlocks.Thaana) ||
                ((block == UnicodeBlocks.AlphabeticPresentationForms) && (ch >= 0xfb1d)))
                return CharacterCategory.Ascii;

            if ((hint == CharacterCategory.FarEast) && (block == UnicodeBlocks.Latin1Supplement) &&
                     ((ch == 0x00a1) || (ch == 0x00a4) || (ch == 0x00a7) || (ch == 0x00a8) || (ch == 0x00aa) ||
                      (ch == 0x00ad) || (ch == 0x00af) || (ch == 0x00d7) || (ch == 0x00f7) ||
                      ((ch >= 0x00b0) && (ch <= 0x00b4) || ((ch >= 0x00b6) && (ch <= 0x00ba)) ||
                       ((ch >= 0x00bc) && (ch <= 0x00bf)))))
                return CharacterCategory.FarEast;

            if ((hint == CharacterCategory.FarEast) &&
                     ((langEastAsia == Language.ChineseTraditional) || (langEastAsia == Language.ChineseSimplified)) &&
                     ((block == UnicodeBlocks.LatinExtendedAdditional) ||
                      ((block == UnicodeBlocks.Latin1Supplement) && (ch == 0x00e0) || (ch == 0x00e1) || (ch == 0x00ec) ||
                      (ch == 0x00ed) || (ch == 0x00f2) || (ch == 0x00f3) || (ch == 0x00f9) ||
                      (ch == 0x00fa) || (ch == 0x00fc) || ((ch >= 0x00e8) && (ch <= 0x00ea)))))
                return CharacterCategory.FarEast;

            if ((hint == CharacterCategory.FarEast) &&
                      ((block == UnicodeBlocks.LatinExtendedA) || (block == UnicodeBlocks.LatinExtendedB) ||
                       (block == UnicodeBlocks.GeneralPunctuation) || (block == UnicodeBlocks.SuperscriptsAndSubscripts) ||
                       (block == UnicodeBlocks.SpacingModifierLetters) || (block == UnicodeBlocks.CombiningDiacriticalMarks) ||
                       (block == UnicodeBlocks.CurrencySymbols) || (block == UnicodeBlocks.LetterlikeSymbols) ||
                       (block == UnicodeBlocks.NumberForms) || (block == UnicodeBlocks.Arrows) ||
                       (block == UnicodeBlocks.MathematicalOperators) || (block == UnicodeBlocks.MiscellaneousTechnical) ||
                       (block == UnicodeBlocks.ControlPictures) || (block == UnicodeBlocks.OpticalCharacterRecognition) ||
                       (block == UnicodeBlocks.EnclosedAlphanumerics) || (block == UnicodeBlocks.BoxDrawing) ||
                       (block == UnicodeBlocks.BlockElements) || (block == UnicodeBlocks.GeometricShapes) ||
                       (block == UnicodeBlocks.MiscellaneousSymbols) || (block == UnicodeBlocks.Dingbats) ||
                       (block == UnicodeBlocks.CJKRadicalsSupplement) || (block == UnicodeBlocks.PrivateUseArea) ||
                       (block == UnicodeBlocks.Cyrillic) || (block == UnicodeBlocks.IPAExtensions) ||
                       ((block == UnicodeBlocks.GreekAndCoptic) && (ch <= 0x03cf))))
                return CharacterCategory.FarEast;

            if ((block == UnicodeBlocks.HangulJamo) || (block == UnicodeBlocks.KangxiRadicals) ||
                     (block == UnicodeBlocks.IdeographicDescriptionCharacters) || (block == UnicodeBlocks.CJKSymbolsAndPunctuation) ||
                     (block == UnicodeBlocks.Hiragana) || (block == UnicodeBlocks.Katakana) ||
                     (block == UnicodeBlocks.Bopomofo) || (block == UnicodeBlocks.HangulCompatibilityJamo) ||
                     (block == UnicodeBlocks.Kanbun) || (block == UnicodeBlocks.EnclosedCJKLettersAndMonths) ||
                     (block == UnicodeBlocks.CJKCompatibility) || (block == UnicodeBlocks.CJKUnifiedIdeographsExtensionA) ||
                     (block == UnicodeBlocks.YiSyllables) || (block == UnicodeBlocks.YiRadicals) ||
                     (block == UnicodeBlocks.HangulSyllables) || (block == UnicodeBlocks.HighSurrogates) ||
                     (block == UnicodeBlocks.HighPrivateUseSurrogates) || (block == UnicodeBlocks.LowSurrogates) ||
                     (block == UnicodeBlocks.CJKCompatibilityIdeographs) || (block == UnicodeBlocks.CJKCompatibilityForms) ||
                     (block == UnicodeBlocks.SmallFormVariants) || (block == UnicodeBlocks.HalfwidthAndFullwidthForms) ||
                     ((block == UnicodeBlocks.CJKUnifiedIdeographs) && (ch <= 0x9faf)) ||
                     ((block == UnicodeBlocks.AlphabeticPresentationForms) && (ch <= 0xfb1c)))
                return CharacterCategory.FarEast;

            return CharacterCategory.Other;
        }

        /// <summary>
        /// Retrieves run attrs source.
        /// TODO: Alternative is to implement IRunAttrSource in this class. I'm not sure it's better.
        /// </summary>
        internal IRunAttrSource Parent
        {
            get { return mParent; }
        }

        private Run ParentRun
        {
            get { return mParent as Run; }
        }

        object IBorderAttrSource.GetDirectBorderAttr(int key)
        {
            return mParent.GetDirectRunAttr(key);
        }

        object IBorderAttrSource.FetchInheritedBorderAttr(int key)
        {
            return mParent.FetchInheritedRunAttr(key);
        }

        void IBorderAttrSource.SetBorderAttr(int key, object value)
        {
            mParent.SetRunAttr(key, value);
        }

        SortedList<BorderType, int> IBorderAttrSource.PossibleBorderKeys
        {
            get
            {
                Debug.Fail("The property is used in BorderCollection. Not supported for Font.");
                return null;
            }
        }

        object IShadingAttrSource.FetchInheritedShadingAttr(int key)
        {
            return mParent.FetchInheritedRunAttr(key);
        }

#region IFillable implementation
        /// <summary>
        /// Changes type of the fill to Solid.
        /// </summary>
        void IFillable.Solid()
        {
            DmlFill fill = GetFill(false);
            if ((fill == null) || (fill.DmlFillType != DmlFillType.SolidFill))
            {
                fill = CreateSolidFill();
                // Word VBA sets fully opaque black color to the new Solid fill.
                fill.ColorInternal = DrColor.Black;
            }
        }

        /// <summary>
        /// Gets a PresetTexture for the fill.
        /// </summary>
        PresetTexture IFillable.GetPresetTexture()
        {
            throw new InvalidOperationException(ErrorPresetTextured);
        }

        /// <summary>
        /// Gets a PatternType for the fill.
        /// </summary>
        PatternType IFillable.GetPatternType()
        {
            throw new InvalidOperationException(ErrorPatterned);
        }

        /// <summary>
        /// Changes type of the fill to preset texture.
        /// </summary>
        void IFillable.PresetTextured(PresetTexture presetTexture)
        {
            throw new InvalidOperationException(ErrorPresetTextured);
        }

        /// <summary>
        /// Changes type of the fill to a pattern.
        /// </summary>
        void IFillable.Patterned(PatternType patternType)
        {
            throw new InvalidOperationException(ErrorPatterned);
        }

                /// <summary>
        /// Changes type of the fill to gradient.
        /// </summary>
        void IFillable.TwoColorGradient(GradientStyle style, GradientVariant variant)
        {
            Theme theme = (Theme == null) ? Theme.BuiltInTheme : Theme;

            DmlFill curFill = GetFill(false);
            DmlColor color1 = ((curFill != null) && (curFill.DmlColorInternal != null))
                ? curFill.DmlColorInternal.Clone()
                // Word VBA sets fully opaque black color to the new Gradient fill.
                : DmlColor.CreateFromDrColor(DrColor.Black);

            DmlColor color2 = ((curFill != null) && (curFill.DmlColor2Internal != null))
                ? curFill.DmlColor2Internal.Clone()
                // Word VBA sets fully opaque black color to the new Gradient fill.
                : DmlColor.CreateFromDrColor(DrColor.Black);

            DmlFill gradientFill = new DmlGradientFill(color1, color2, style, variant, theme);
            ((IFillable)this).SetFill(gradientFill);
        }

        /// <summary>
        /// Changes type of the fill to gradient.
        /// </summary>
        void IFillable.OneColorGradient(GradientStyle style, GradientVariant variant, double degree)
        {
            Theme theme = (Theme == null) ? Theme.BuiltInTheme : Theme;

            DmlFill curFill = GetFill(false);
            DmlColor color1 = ((curFill != null) && (curFill.DmlColorInternal != null))
                ? curFill.DmlColorInternal.Clone()
                // Word VBA sets fully opaque black color to the new Gradient fill.
                : DmlColor.CreateFromDrColor(DrColor.Black);

            DmlColor color2 = color1.Clone();

            // Check either we need luminance modifier.
            if (!MathUtil.AreEqual(degree, 0.5))
            {
                DmlLuminanceModulation lumMod = new DmlLuminanceModulation();
                DmlLuminanceOffset lumOff = new DmlLuminanceOffset();
                if (MathUtil.IsLessOrEqual(degree, 0.5))
                {
                    lumMod.Value = degree * 2;
                    lumOff.Value = 0;
                }
                else
                {
                    lumMod.Value = (1 - degree) * 2;
                    lumOff.Value = 1 - lumMod.Value;
                }

                color2.ColorModifiers.Add(lumMod);
                color2.ColorModifiers.Add(lumOff);
            }

            DmlFill gradientFill = new DmlGradientFill(color1, color2, style, variant, theme);
            ((IFillable)this).SetFill(gradientFill);
        }

        /// <summary>
        /// Changes the fill type to single image.
        /// </summary>
        void IFillable.SetImage(byte[] imageBytes)
        {
            throw new InvalidOperationException("SetImage cannot be applied to Font.");
        }

        /// <summary>
        /// Sets specified fill to this object.
        /// </summary>
        void IFillable.SetFill(IFill fill)
        {
            DmlFill dmlFill = fill as DmlFill;
            if ((dmlFill == null) ||
                ((dmlFill.DmlFillType != DmlFillType.SolidFill) &&
                 (dmlFill.DmlFillType != DmlFillType.GradientFill) &&
                 (dmlFill.DmlFillType != DmlFillType.NoFill)))
                throw new InvalidOperationException("Invalid fill type for this object.");

            dmlFill.Parent = this;
            mParent.SetRunAttr(FontAttr.EffectFill, dmlFill);
        }

        /// <summary>
        /// Returns a double value representing transparency of a specified color.
        /// </summary>
        double IFillable.GetTransparency(DmlColor color)
        {
            if (color.Alpha == null)
                return 0.0;

            return color.Alpha.Value;
        }

        /// <summary>
        /// Sets a specified value to a transparency of specified color .
        /// </summary>
        void IFillable.SetTransparency(DmlColor color, double value)
        {
            color.UpdateAlpha(value);
        }

        #region The public properties of old Fill object for compatibility.

        /// <summary>
        /// Gets or sets a Color object that represents the foreground color for the fill.
        /// </summary>
        Color IFillable.FilledColor
        {
            get { return ((IFillable)this).FillableForeColor; }
            set { ((IFillable)this).FillableForeColor = value; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether a Fill applied to an object is visible.
        /// </summary>
        bool IFillable.OldOn
        {
            get { return ((IFillable)this).FillableVisible; }
            set { ((IFillable)this).FillableVisible = value; }
        }

        /// <summary>
        /// Gets or sets a double value between 0.0 (clear) and 1.0 (opaque) representing the degree
        /// of opacity of the specified fill.
        /// </summary>
        double IFillable.OldOpacity
        {
            get { return 1 - ((IFillable)this).FillableTransparency; }
            set { ((IFillable)this).FillableTransparency = 1 - value; }
        }

        /// <summary>
        /// Gets the raw bytes of the fill texture or pattern.
        /// </summary>
        /// <remarks>
        /// <p>The default value is <c>null</c>.</p>
        /// </remarks>
        byte[] IFillable.FillableImageBytes
        {
            // There can be only Solid, Gradient and NoFill fills in text effects, so there cannot be any image.
            get { return null; }
        }

        #endregion

        /// <summary>
        /// Gets or sets the alignment for tile texture fill.
        /// </summary>
        TextureAlignment IFillable.TextureAlignment
        {
            get { return TextureAlignment.None; }
            set { throw new InvalidOperationException(ErrorTextureAlignment); }
        }

        /// <summary>
        /// Gets or sets the angle of the gradient fill.
        /// </summary>
        double IFillable.GradientAngle
        {
            get
            {
                DmlFill fill = GetFill(false);
                if (fill == null)
                    return 0;

                return fill.Angle;
            }
            set
            {
                DmlFill fill = GetFill(false);
                if (fill != null)
                    fill.Angle = value;
            }
        }

        /// <summary>
        /// Gets the gradient variant for the fill as an integer value from 1 to 4 for most gradient fills.
        /// </summary>
        GradientVariant IFillable.GradientVariant
        {
            get
            {
                DmlFill fill = GetFill(false);
                if (fill == null)
                    return GradientVariant.None;

                return fill.GradientVariant;
            }
        }

        /// <summary>
        /// Gets the gradient style for the fill.
        /// </summary>
        GradientStyle IFillable.GradientStyle
        {
            get
            {
                DmlFill fill = GetFill(false);
                if (fill == null)
                    return GradientStyle.None;

                return fill.GradientStyle;
            }
        }

        /// <summary>
        /// Gets the gradient stops collection for the fill.
        /// </summary>
        GradientStopCollection IFillable.GradientStops
        {
            get
            {
                DmlGradientFill fill = GetFill(false) as DmlGradientFill;
                if (fill == null)
                    throw new InvalidOperationException(InvalidAction);

                return fill.GradientStopCollection;
            }
        }

        /// <summary>
        /// Gets or sets a Color object that represents the foreground color for the fill.
        /// </summary>
        /// <dev>
        /// Note, Word VBA always changes the type of fill to Solid when setting a new ForeColor
        /// but we do it only when current fill is NoFill for a while. Also Word VBA resets transparency
        /// to a fully opaque color, but we are not.
        /// </dev>
        Color IFillable.FillableForeColor
        {
            get
            {
                DmlFill fill = GetFill(false);
                if (fill == null)
                    return Color.Empty;

                return fill.ColorInternal.ToColorFixAlpha().ToNativeColor();
            }
            set
            {
                DmlFill fill = GetFill(true);
                // Word VBA allows to set up color for NoFill and changes the type of fill to Solid implicitly.
                if (fill.DmlFillType == DmlFillType.NoFill)
                    fill = CreateSolidFill();

                fill.ColorInternal = DrColor.FromNativeColor(value);
            }
        }

        /// <summary>
        /// Gets a Color object that represents the base foreground color without modifiers for the fill.
        /// </summary>
        Color IFillable.FillableBaseForeColor
        {
            get
            {
                DmlFill fill = GetFill(false);
                if (fill == null)
                    return Color.Empty;

                return fill.ColorInternalUnmodified.ToColorFixAlpha().ToNativeColor();
            }
        }

        /// <summary>
        /// Gets or sets a Color object that represents the background color for the fill.
        /// </summary>
        Color IFillable.FillableBackColor
        {
            get
            {
                DmlFill fill = GetFill(false);
                if (fill == null)
                    return Color.Empty;

                return fill.Color2Internal.ToColorFixAlpha().ToNativeColor();
            }
            set
            {
                DmlFill fill = GetFill(true);
                // Word VBA does not throw, but also does not change the color in 'none' fill.
                if (fill.DmlFillType == DmlFillType.NoFill)
                    return;

                // Seems the behavior for gradient in Word VBA depends on how we do it. If we set BackColor within
                // 'With' block, then the color is reset to black. But if we do it outside of 'With' block,
                // then the color is not changed. For a moment, there are no any reasons to follow such behavior.
                // So, let's set background color in gradient fill the same way, as we do it for shapes for uniformity.
                fill.Color2Internal = DrColor.FromNativeColor(value);
            }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the foreground color for the fill.
        /// </summary>
        ThemeColor IFillable.FillableForeThemeColor
        {
            get { return ThemeColor; }
            set { ThemeColor = value; }
        }

        /// <summary>
        /// Gets or sets a ThemeColor object that represents the background color for the fill.
        /// </summary>
        ThemeColor IFillable.FillableBackThemeColor
        {
            get { return ThemeColor.None; }
            set { throw new InvalidOperationException("Cannot set BackThemeColor to this Fill."); }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the foreground color.
        /// </summary>
        double IFillable.FillableForeTintAndShade
        {
            get { return TintAndShade; }
            set { TintAndShade = value; }
        }

        /// <summary>
        /// Gets or sets a double value that lightens or darkens the background color.
        /// </summary>
        double IFillable.FillableBackTintAndShade
        {
            get { return 0.0d; }
            set { throw new InvalidOperationException("Cannot apply BackTintAndShade to this Fill."); }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether a Fill applied to an object is visible.
        /// </summary>
        bool IFillable.FillableVisible
        {
            get
            {
                DmlFill fill = GetFill(false);
                if (fill == null)
                    return true;

                return fill.On;
            }
            set
            {
                DmlFill fill = GetFill(true);
                fill.On = value;
            }
        }

        /// <summary>
        /// Gets or sets a double value between 0.0 (opaque) and 1.0 (clear) representing the degree
        /// of transparency of the specified fill.
        /// </summary>
        double IFillable.FillableTransparency
        {
            get
            {
                DmlFill fill = GetFill(false);
                if (fill == null)
                    return 0.0;

                // Word VBA returns 1.0 for NoFill.
                if (fill.DmlFillType == DmlFillType.NoFill)
                    return 1.0;

                // DmlFill.Opacity in case of missing alpha returns the value (1.0) that is intended for the shape.
                // However, in text it should be inverted to (0.0), so check this special case here directly.
                if ((fill.DmlColorInternal == null) || (fill.DmlColorInternal.Alpha == null))
                    return 0.0;

                return fill.Opacity;
            }
            set
            {
                DmlFill fill = GetFill(true);

                // Word VBA allows to set up opacity (transparency) for NoFill and
                // changes the type of fill to Solid implicitly.
                if (fill.DmlFillType == DmlFillType.NoFill)
                    fill = CreateSolidFill();

                fill.Opacity = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether the fill rotates with the specified object.
        /// </summary>
        /// <dev>
        /// This property is applicable to shape objects only.
        /// When accessing this property in VBA for text an exception is thrown.
        /// </dev>
        bool IFillable.RotateWithObject
        {
            get { throw new InvalidOperationException(DmlFill.MsgInvalidAction); }
            set { throw new InvalidOperationException(DmlFill.MsgInvalidAction); }
        }

        /// <summary>
        /// Gets fill type.
        /// </summary>
        FillTypeCore IFillable.FillType
        {
            get
            {
                DmlFill fill = GetFill(false);
                if (fill == null)
                    return FillTypeCore.Solid;

                return fill.FillType;
            }
        }

        /// <summary>
        /// Gets IThemeProvider object.
        /// </summary>
        IThemeProvider IFillable.FillableThemeProvider
        {
            get { return Theme; }
        }

#endregion

        /// <summary>
        /// Creates a new Solid fill in this <see cref="Font"/> object.
        /// </summary>
        private DmlFill CreateSolidFill()
        {
            DmlFill solidFill = new DmlSolidFill();
            mParent.SetRunAttr(FontAttr.EffectFill, solidFill);

            solidFill.Parent = this;

            return solidFill;
        }

        /// <summary>
        /// Gets fill of this <see cref="Font"/> object.
        /// </summary>
        /// <param name="isAllowAutoCreate"> Indicates whether it is allowed to create
        /// default Solid fill when there is no fill set. </param>
        private DmlFill GetFill(bool isAllowAutoCreate)
        {
            DmlFill value = (DmlFill)mParent.GetDirectRunAttr(FontAttr.EffectFill);
            if (value == null)
                return (isAllowAutoCreate) ? CreateSolidFill() : null;

            value.Parent = this;
            return value;
        }

        private StyleCollection Styles
        {
            get
            {
                return mDoc != null
                    ? mDoc.Styles
                    : null;
            }
        }

        private Theme Theme
        {
            get
            {
                return mDoc != null
                    ? mDoc.GetThemeInternal()
                    : null;
            }
        }

        private DocumentFontProvider FontProvider
        {
            get
            {
                return mDoc != null
                    ? mDoc.FontProvider
                    : null;
            }
        }


        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly IRunAttrSource mParent;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocumentBase mDoc;

        private Fill mFill;

        private const float SmallCapsSizeFactor = 0.80f;
        private const float SubscriptSizeFactor = 0.62f;

        private const string ErrorPresetTextured = "Preset texture cannot be applied to Font.";
        private const string ErrorTextureAlignment = "TextureAlignment cannot be applied to Font.";
        private const string ErrorPatterned = "Pattern cannot be applied to Font.";
        private const string InvalidAction = "Object doesn't support this action.";
    }
}
