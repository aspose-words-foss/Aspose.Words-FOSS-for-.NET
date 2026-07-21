// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

using System;
using System.Drawing;
using Aspose.Collections;
using Aspose.Words.Drawing.Core.Dml.Colors;
using Aspose.Words.Drawing.Core.Dml.Common;
using Aspose.Words.Drawing.Core.Dml.Fills;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.ShapeEffects;

namespace Aspose.Words.Drawing.Core.Dml.Text
{
    /// <summary>
    /// 21.1.2.3.9 rPr (Text Run Properties)
    /// This element contains all run level text properties for the text runs within a containing paragraph.
    /// </summary>
    internal class DmlRunProperties : DmlExtensionListSource, IMathRunPr
    {
        internal DmlRunProperties()
        {
            mPropertyBag.ParentBagProvider = gDefaultProvider;
        }

        internal DmlRunProperties Clone()
        {
            DmlRunProperties lhs = (DmlRunProperties)MemberwiseClone();
            CopyTo(lhs);

            if (HasExtensions)
                lhs.Extensions = CloneExtensions();

            return lhs;
        }

        /// <summary>
        /// Copies all defined property values to the specified run properties.
        /// </summary>
        internal void CopyTo(DmlRunProperties properties)
        {
            properties.mPropertyBag = mPropertyBag.Clone();

            if (mEffects != null)
                properties.mEffects = mEffects.Clone();

            DmlTextPoints kerning = (DmlTextPoints)GetDirectProperty(DmlRunPropertiesIds.Kerning);
            if (kerning != null)
                properties.SetProperty(DmlRunPropertiesIds.Kerning, kerning.Clone());

            DmlTextPoints spacing = (DmlTextPoints)GetDirectProperty(DmlRunPropertiesIds.Spacing);
            if (spacing != null)
                properties.SetProperty(DmlRunPropertiesIds.Spacing, spacing.Clone());

            DmlTextPoints fontSize = (DmlTextPoints)GetDirectProperty(DmlRunPropertiesIds.FontSize);
            if (fontSize != null)
                properties.SetProperty(DmlRunPropertiesIds.FontSize, fontSize.Clone());

            DmlFill fill = (DmlFill)GetDirectProperty(DmlRunPropertiesIds.Fill);
            if (fill != null)
                properties.SetProperty(DmlRunPropertiesIds.Fill, fill.Clone());

            DmlOutline outline = (DmlOutline)GetDirectProperty(DmlRunPropertiesIds.Outline);
            if (outline != null)
                properties.SetProperty(DmlRunPropertiesIds.Outline, outline.Clone());

            DmlColor color = (DmlColor)GetDirectProperty(DmlRunPropertiesIds.HighlightColor);
            if (color != null)
                properties.SetProperty(DmlRunPropertiesIds.HighlightColor, color.Clone());

            DmlFont font = (DmlFont)GetDirectProperty(DmlRunPropertiesIds.LatinFont);
            if (font != null)
                properties.SetProperty(DmlRunPropertiesIds.LatinFont, font.Clone());

            font = (DmlFont)GetDirectProperty(DmlRunPropertiesIds.EastAsianFont);
            if (font != null)
                properties.SetProperty(DmlRunPropertiesIds.EastAsianFont, font.Clone());

            font = (DmlFont)GetDirectProperty(DmlRunPropertiesIds.SymbolFont);
            if (font != null)
                properties.SetProperty(DmlRunPropertiesIds.SymbolFont, font.Clone());

            font = (DmlFont)GetDirectProperty(DmlRunPropertiesIds.ComplexScriptFont);
            if (font != null)
                properties.SetProperty(DmlRunPropertiesIds.ComplexScriptFont, font.Clone());

            DmlHlink hyperlink = (DmlHlink)GetDirectProperty(DmlRunPropertiesIds.HlinkClick);
            if (hyperlink != null)
                properties.SetProperty(DmlRunPropertiesIds.HlinkClick, hyperlink.Clone());

            hyperlink = (DmlHlink)GetDirectProperty(DmlRunPropertiesIds.HlinkHover);
            if (hyperlink != null)
                properties.SetProperty(DmlRunPropertiesIds.HlinkHover, hyperlink.Clone());

            fill = (DmlFill)GetDirectProperty(DmlRunPropertiesIds.UnderlineFill);
            if (fill != null)
                properties.SetProperty(DmlRunPropertiesIds.UnderlineFill, fill.Clone());

            outline = (DmlOutline)GetDirectProperty(DmlRunPropertiesIds.UnderlineStroke);
            if (outline != null)
                properties.SetProperty(DmlRunPropertiesIds.UnderlineStroke, outline.Clone());
        }

        internal void SetParentProperties(DmlRunProperties properties)
        {
            mPropertyBag.ParentBagProvider = new DmlHierarchicalPropertyBagParentContainer(properties.mPropertyBag);
        }

        internal DmlFont GetFont(DmlFontType fontType)
        {
            switch (fontType)
            {
                case DmlFontType.Latin:
                    return LatinFont;
                case DmlFontType.EastAsian:
                    return EastAsianFont;
                case DmlFontType.Symbol:
                    return SymbolFont;
                case DmlFontType.ComplexScript:
                    return ComplexScriptFont;
                default:
                    throw new ArgumentOutOfRangeException("fontType");
            }
        }

        /// <summary>
        /// Returns font that is to be applied to the run.
        /// </summary>
        internal DmlFont GetFont()
        {
            // If one of four fonts is set directly in run properties, return it.
            // Otherwise return Latin font inherited from parent.
            DmlFont latin = (DmlFont)GetDirectProperty(DmlRunPropertiesIds.LatinFont);
            if (latin != null)
                return latin;

            DmlFont ea = (DmlFont)GetDirectProperty(DmlRunPropertiesIds.EastAsianFont);
            if (ea != null)
                return ea;

            DmlFont cs = (DmlFont)GetDirectProperty(DmlRunPropertiesIds.ComplexScriptFont);
            if (cs != null)
                return cs;

            DmlFont sym = (DmlFont)GetDirectProperty(DmlRunPropertiesIds.SymbolFont);
            if (sym != null)
                return sym;

            return LatinFont;
        }

        internal FontStyle GetFontStyle()
        {
            FontStyle style = FontStyle.Regular;
            if (Bold)
                style |= FontStyle.Bold;
            if (Underline != Underline.None)
                style |= FontStyle.Underline;
            if (Italics)
                style |= FontStyle.Italic;
            if (Strikethrough != DmlTextStrike.No)
                style |= FontStyle.Strikeout;
            return style;
        }

        internal void SetParentProperties(DmlParagraph paragraph, DmlTextListStyles textListStyles)
        {
            mPropertyBag.ParentBagProvider = new DmlRunProperties.DmlTextRunParentBagProvider(paragraph, textListStyles);
        }

        internal void SetProperty(DmlRunPropertiesIds propertyId, object value)
        {
            mPropertyBag.SetProperty((int)propertyId, value);
        }

        internal object GetProperty(DmlRunPropertiesIds propertyId)
        {
            return mPropertyBag.GetProperty((int)propertyId);
        }

        internal object GetDirectProperty(DmlRunPropertiesIds propertyId)
        {
            return mPropertyBag.GetDirectProperty((int)propertyId);
        }

        /// <summary>
        /// Removes value of the specified property (clears the property).
        /// </summary>
        internal void Remove(DmlRunPropertiesIds propertyId)
        {
            mPropertyBag.Remove((int)propertyId);
        }

        /// <summary>
        /// Removes values of all properties.
        /// </summary>
        internal void RemoveAll()
        {
            mPropertyBag.RemoveAll();
        }

        /// <summary>
        /// Determines whether the mPropertyBag contains the specified property, which was set directly.
        /// </summary>
        /// <param name="propertyId">the property id</param>
        /// <returns>"true", if the  property was set directly, "false" otherwise</returns>
        internal bool IsPropertySpecified(DmlRunPropertiesIds propertyId)
        {
            return mPropertyBag.IsPropertySpecified((int)propertyId);
        }

        /// <summary>
        /// Returns <c>true</c> if this property collection redefines any properties of the parent collection.
        /// </summary>
        internal bool HasNonDefaultFormatting(int[] attributesToIgnore)
        {
            return mPropertyBag.HasNonDefaultFormatting(attributesToIgnore, null);
        }

        public bool IsDml
        {
            get { return true; }
        }

        public override StringToObjDictionary<DmlExtension> Extensions
        {
            get { return (StringToObjDictionary<DmlExtension>)GetProperty(DmlRunPropertiesIds.Extensions); }
            set { SetProperty(DmlRunPropertiesIds.Extensions, value); }
        }

        /// <summary>
        /// Specifies the alternate language to use when the generating application is displaying 
        /// the user interface controls. If this attribute is omitted, than the lang attribute is used here.
        /// </summary>
        internal Language AlternativeLanguage
        {
            get { return (Language)GetProperty(DmlRunPropertiesIds.AlternativeLanguage); }
            set { SetProperty(DmlRunPropertiesIds.AlternativeLanguage, value); }
        }

        /// <summary>
        /// Specifies whether a run of text is formatted as bold text. 
        /// If this attribute is omitted, than a value of 0, or false is assumed.
        /// </summary>
        internal bool Bold
        {
            get { return (bool)GetProperty(DmlRunPropertiesIds.Bold); }
            set { SetProperty(DmlRunPropertiesIds.Bold, value); }
        }

        /// <summary>
        /// Specifies the baseline for both the superscript and subscript fonts. 
        /// The size is specified using a percentage where 1% is equal to 1 percent of the font size and 
        /// 100% is equal to 100 percent font of the font size.
        /// Value is in fraction representation.
        /// </summary>
        internal double Baseline
        {
            get { return (double)GetProperty(DmlRunPropertiesIds.Baseline); }
            set { SetProperty(DmlRunPropertiesIds.Baseline, value); }
        }

        /// <summary>
        /// Specifies the link target name that is used to reference to the proper link properties in a custom XML part within the document.
        /// </summary>
        internal string BookmarkLinkTarget
        {
            get { return (string)GetProperty(DmlRunPropertiesIds.BookmarkLinkTarget); }
            set { SetProperty(DmlRunPropertiesIds.BookmarkLinkTarget, value); }
        }

        /// <summary>
        /// Specifies the capitalization that is to be applied to the text run. 
        /// This is a render-only modification and does not affect the actual characters stored in the text run. 
        /// This attribute is also distinct from the toggle function where the actual characters stored in the text run are changed.
        /// </summary>
        internal DmlCapitalization Capitalization
        {
            get { return (DmlCapitalization)GetProperty(DmlRunPropertiesIds.Capitalization); }
            set { SetProperty(DmlRunPropertiesIds.Capitalization, value); }
        }

        /// <summary>
        /// Specifies that the content of a text run has changed since the proofing tools have last been run. 
        /// Effectively this flags text that is to be checked again by the generating application 
        /// for mistakes such as spelling, grammar, etc.
        /// </summary>
        internal bool IsDirty
        {
            get { return (bool)GetProperty(DmlRunPropertiesIds.IsDirty); }
            set { SetProperty(DmlRunPropertiesIds.IsDirty, value); }
        }

        /// <summary>
        /// Specifies that when this run of text was checked for spelling, grammar, etc. 
        /// that a mistake was indeed found. This allows the generating application to effectively save the 
        /// state of the mistakes within the document instead of having to perform 
        /// a full pass check upon opening the document.
        /// </summary>
        internal bool HasSpellingError
        {
            get { return (bool)GetProperty(DmlRunPropertiesIds.HasSpellingError); }
            set { SetProperty(DmlRunPropertiesIds.HasSpellingError, value); }
        }

        /// <summary>
        /// Specifies whether a run of text is formatted as italic text. 
        /// If this attribute is omitted, than a value of 0, or false is assumed.
        /// </summary>
        internal bool Italics
        {
            get { return (bool)GetProperty(DmlRunPropertiesIds.Italics); }
            set { SetProperty(DmlRunPropertiesIds.Italics, value); }
        }

        /// <summary>
        /// Specifies the minimum font size at which character kerning occurs for this text run. 
        /// Whole points are specified in increments of 100 starting with 100 being a point size of 1. 
        /// For instance a font point size of 12 would be 1200 and a font point size of 12.5 would be 1250. 
        /// If this attribute is omitted, than kerning occurs for all font sizes down to a 0 point font.
        /// </summary>
        internal DmlTextPoints Kerning
        {
            get { return (DmlTextPoints)GetProperty(DmlRunPropertiesIds.Kerning); }
            set { SetProperty(DmlRunPropertiesIds.Kerning, value); }
        }

        /// <summary>
        /// Specifies whether the numbers contained within vertical text continue vertically with the 
        /// text or whether they are to be displayed horizontally while the surrounding characters 
        /// continue in a vertical fashion. If this attribute is omitted, than a value of 0, or false is assumed.
        /// </summary>
        internal bool Kumimoji
        {
            get { return (bool)GetProperty(DmlRunPropertiesIds.Kumimoji); }
            set { SetProperty(DmlRunPropertiesIds.Kumimoji, value); }
        }

        /// <summary>
        /// Specifies the language to be used when the generating application is displaying the user interface controls. 
        /// If this attribute is omitted, than the generating application can select a language of its choice.
        /// </summary>
        internal Language Language
        {
            get { return (Language)GetProperty(DmlRunPropertiesIds.Language); }
            set { SetProperty(DmlRunPropertiesIds.Language, value); }
        }

        /// <summary>
        /// Specifies that a run of text has been selected by the user to not be checked for mistakes. 
        /// Therefore if there are spelling, grammar, etc mistakes within this text the generating application should ignore them.
        /// </summary>
        internal bool NoProofing
        {
            get { return (bool)GetProperty(DmlRunPropertiesIds.NoProofing); }
            set { SetProperty(DmlRunPropertiesIds.NoProofing, value); }
        }

        /// <summary>
        /// Specifies the normalization of height that is to be applied to the text run. 
        /// This is a render-only modification and does not affect the actual characters stored in the text run. 
        /// This attribute is also distinct from the toggle function where the actual characters stored in the text run are changed. 
        /// If this attribute is omitted, than a value of 0, or false is assumed.
        /// </summary>
        internal bool NormalizeHeights
        {
            get { return (bool)GetProperty(DmlRunPropertiesIds.NormalizeHeights); }
            set { SetProperty(DmlRunPropertiesIds.NormalizeHeights, value); }
        }

        /// <summary>
        /// Specifies a smart tag identifier for a run of text. This ID is unique throughout the presentation 
        /// and is used to reference corresponding auxiliary information about the smart tag.
        /// </summary>
        internal bool SmartTagsClean
        {
            get { return (bool)GetProperty(DmlRunPropertiesIds.SmartTagsClean); }
            set { SetProperty(DmlRunPropertiesIds.SmartTagsClean, value); }
        }

        /// <summary>
        /// Specifies a smart tag identifier for a run of text. This ID is unique throughout the presentation 
        /// and is used to reference corresponding auxiliary information about the smart tag.
        /// AW: int.MinValue means that the attribute isn't defined.
        /// SmartTagID should be a positive unsigned integer value.
        /// </summary>
        internal uint SmartTagID
        {
            get { return (uint)GetProperty(DmlRunPropertiesIds.SmartTagID); }
            set { SetProperty(DmlRunPropertiesIds.SmartTagID, value); }
        }

        /// <summary>
        /// Specifies the spacing between characters within a text run. 
        /// This spacing is specified numerically and should be consistently applied across the entire run 
        /// of text by the generating application. Whole points are specified in increments of 100 starting 
        /// with 100 being a point size of 1. For instance a font point size of 12 would be 1200 and a font 
        /// point size of 12.5 would be 1250. If this attribute is omitted than a value of 0 or no adjustment is assumed.
        /// </summary>
        internal DmlTextPoints Spacing
        {
            get { return (DmlTextPoints)GetProperty(DmlRunPropertiesIds.Spacing); }
            set { SetProperty(DmlRunPropertiesIds.Spacing, value); }
        }

        /// <summary>
        /// Specifies whether a run of text is formatted as strikethrough text. 
        /// If this attribute is omitted, than no strikethrough is assumed.
        /// </summary>
        internal DmlTextStrike Strikethrough
        {
            get { return (DmlTextStrike)GetProperty(DmlRunPropertiesIds.Strikethrough); }
            set { SetProperty(DmlRunPropertiesIds.Strikethrough, value); }
        }

        /// <summary>
        /// Specifies the size of text within a text run. Whole points are specified in increments of 
        /// 100 starting with 100 being a point size of 1. For instance a font point size of 12 would 
        /// be 1200 and a font point size of 12.5 would be 1250. If this attribute is omitted, 
        /// than the value in defRPr should be used.
        /// </summary>
        internal DmlTextPoints FontSize
        {
            get { return (DmlTextPoints)GetProperty(DmlRunPropertiesIds.FontSize); }
            set { SetProperty(DmlRunPropertiesIds.FontSize, value); }
        }

        /// <summary>
        /// Specifies whether a run of text is formatted as underlined text. 
        /// If this attribute is omitted, than no underline is assumed.
        /// </summary>
        internal Underline Underline
        {
            get { return (Underline)GetProperty(DmlRunPropertiesIds.Underline); }
            set { SetProperty(DmlRunPropertiesIds.Underline, value); }
        }

        internal DmlFill Fill
        {
            get { return (DmlFill)GetProperty(DmlRunPropertiesIds.Fill); }
            set { SetProperty(DmlRunPropertiesIds.Fill, value); }
        }

        internal DmlOutline Outline
        {
            get { return (DmlOutline)GetProperty(DmlRunPropertiesIds.Outline); }
            set { SetProperty(DmlRunPropertiesIds.Outline, value); }
        }

        internal DmlColor HighlightColor
        {
            get { return (DmlColor)GetProperty(DmlRunPropertiesIds.HighlightColor); }
            set { SetProperty(DmlRunPropertiesIds.HighlightColor, value); }
        }

        internal DmlFont LatinFont
        {
            get { return (DmlFont)GetProperty(DmlRunPropertiesIds.LatinFont); }
            set { SetProperty(DmlRunPropertiesIds.LatinFont, value); }
        }

        internal DmlFont EastAsianFont
        {
            get { return (DmlFont)GetProperty(DmlRunPropertiesIds.EastAsianFont); }
            set { SetProperty(DmlRunPropertiesIds.EastAsianFont, value); }
        }

        internal DmlFont SymbolFont
        {
            get { return (DmlFont)GetProperty(DmlRunPropertiesIds.SymbolFont); }
            set { SetProperty(DmlRunPropertiesIds.SymbolFont, value); }
        }

        internal DmlFont ComplexScriptFont
        {
            get { return (DmlFont)GetProperty(DmlRunPropertiesIds.ComplexScriptFont); }
            set { SetProperty(DmlRunPropertiesIds.ComplexScriptFont, value); }
        }

        /// <summary>
        /// Collection of effects applied to the text element.
        /// </summary>
        /// <remarks>
        /// I did not manage to create document where effects were inherited from paragraph level.
        /// For now leave effects collection not inheritable (it is not in DmlHierarchicalPropertyBag).
        /// This means only directly applied effects has effect. 
        /// </remarks>
        internal DmlShapeEffectsCollection Effects
        {
            get { return mEffects; }
            set { mEffects = value; }
        }

        /// <summary>
        /// Specifies the on-click hyperlink information to be applied to a run of text. When the hyperlink text is clicked the 
        /// link will be fetched.
        /// </summary>
        internal DmlHlink HlinkClick
        {
            get { return (DmlHlink)GetProperty(DmlRunPropertiesIds.HlinkClick); }
            set { SetProperty(DmlRunPropertiesIds.HlinkClick, value); }
        }

        /// <summary>
        /// Specifies the mouse-over hyperlink information to be applied to a run of text. When the mouse is hovered over 
        /// this hyperlink text the link will be fetched.
        /// </summary>
        internal DmlHlink HlinkHover
        {
            get { return (DmlHlink)GetProperty(DmlRunPropertiesIds.HlinkHover); }
            set { SetProperty(DmlRunPropertiesIds.HlinkHover, value); }
        }

        /// <summary>
        /// Specifies the fill color of an underline for a run of text.
        /// </summary>
        internal DmlFill UnderlineFill
        {
            get { return (DmlFill)GetProperty(DmlRunPropertiesIds.UnderlineFill); }
            set { SetProperty(DmlRunPropertiesIds.UnderlineFill, value); }
        }

        /// <summary>
        /// Specifies that the fill color of an underline for a run of text should be of the same color as the text 
        /// run within which it is contained.
        /// </summary>
        internal bool UnderlineFillTx
        {
            get { return (bool)GetProperty(DmlRunPropertiesIds.UnderlineFillTx); }
            set { SetProperty(DmlRunPropertiesIds.UnderlineFillTx, value); }
        }

        /// <summary>
        /// Specifies the properties for the stroke of the underline that will be present within a run of text.
        /// </summary>
        internal DmlOutline UnderlineStroke
        {
            get { return (DmlOutline)GetProperty(DmlRunPropertiesIds.UnderlineStroke); }
            set { SetProperty(DmlRunPropertiesIds.UnderlineStroke, value); }
        }

        /// <summary>
        /// Specifies that the stroke style of an underline for a run of text should be of the same as the text run 
        /// within which it is contained.
        /// </summary>
        internal bool UnderlineStrokeTx
        {
            get { return (bool)GetProperty(DmlRunPropertiesIds.UnderlineStrokeTx); }
            set { SetProperty(DmlRunPropertiesIds.UnderlineStrokeTx, value); }
        }

        /// <summary>
        /// Specifies whether the text is right-to-left or left-to-right in its flow direction. 
        /// If this attribute is omitted, then a value of 0, or left-to-right is implied.
        /// </summary>
        internal bool RightToLeftFlowDirection
        {
            get { return (bool)GetProperty(DmlRunPropertiesIds.RightToLeftFlowDirection); }
            set { SetProperty(DmlRunPropertiesIds.RightToLeftFlowDirection, value); }
        }

        internal bool IsEmpty
        {
            get { return (mPropertyBag.Count == 0); }
        }

        /// <summary>
        /// Returns number of properties set explicitly.
        /// </summary>
        public int Count
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
            get { return mPropertyBag.Count; }
        }

        private static readonly IDmlHierarchicalPropertyBagParentProvider gDefaultProvider =
            new DmlHierarchicalPropertyBagParentContainer(DmlRunPropertiesDefaults.Instance);

        private IDmlHierarchicalPropertyBag mPropertyBag = new DmlHierarchicalPropertyBag();
        private DmlShapeEffectsCollection mEffects;

        internal class DmlTextRunParentBagProvider : IDmlHierarchicalPropertyBagParentProvider
        {
            public DmlTextRunParentBagProvider(DmlParagraph paragraph, DmlTextListStyles textListStyles)
            {
                mParentBag = textListStyles.GetTextListStyle(paragraph.Properties.Level).DefaultRunProperties.mPropertyBag;
            }

            public IDmlHierarchicalPropertyBag ParentBag
            {
                get { return mParentBag; }
            }

            public IDmlHierarchicalPropertyBagParentProvider Clone()
            {
                DmlTextRunParentBagProvider lhs = (DmlTextRunParentBagProvider)MemberwiseClone();
                if (mParentBag != null)
                    lhs.mParentBag = mParentBag.Clone();
                return lhs;
            }
            
            private IDmlHierarchicalPropertyBag mParentBag;
        }
    }
}
