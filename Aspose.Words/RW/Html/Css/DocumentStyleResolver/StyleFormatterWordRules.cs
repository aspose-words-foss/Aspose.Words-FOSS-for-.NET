// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2016 by Victor Chebotok

using System;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Themes;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Gets and formats appropriate model styles when CSS formatting is applied to the document model. to paragraphs.
    /// Uses same formatting rules as MS Word.
    /// </summary>
    internal class StyleFormatterWordRules : StyleFormatter
    {
        internal StyleFormatterWordRules(
            Document document,
            CssStyleTracker cssStyleTracker,
            ParagraphFormatter paragraphFormatter,
            FontFormatter fontFormatter)
            : base(document, cssStyleTracker, paragraphFormatter, fontFormatter)
        {
            // Empty constructor.
        }

        internal override Style GetModelStyle(StyleType styleType)
        {
            switch (styleType)
            {
                case StyleType.Paragraph:
                    return GetParagraphStyle();
                case StyleType.Character:
                    return GetCharacterStyle();
                default:
                    return null;
            }
        }

        internal override void UpdatePredefinedStyles()
        {
            // Get default run properties from 'p' and 'span' selectors.
            foreach (CssStyleRule cssRule in CssStyleTracker.DocumentStyleRules)
            {
                foreach (CssSelector selector in cssRule.Selectors)
                {
                    // For each of 'p' selectors.
                    CssTypeSelector selectorAsType = selector as CssTypeSelector;
                    if (selectorAsType == null)
                    {
                        continue;
                    }

                    // Get paragraph properties from declarations.
                    if (selectorAsType.ElementName == "p")
                    {
                        ParagraphFormatter.Format(new ParagraphFormat(mParaPrFromParagraphSelector, Document.Styles),
                            cssRule.Declarations);
                    }

                    // Get run properties from declarations.
                    RunPr runPr;
                    StyleType styleType;
                    switch (selectorAsType.ElementName)
                    {
                        case "p":
                        {
                            runPr = mRunPrFromParagraphSelector;
                            styleType = StyleType.Paragraph;
                            break;
                        }
                        case "span":
                        {
                            runPr = mRunPrFromSpanSelector;
                            styleType = StyleType.Character;
                            break;
                        }
                        default:
                        {
                            continue;
                        }
                    }
                    FontFormatter.Format(new Font(runPr, Document), styleType, cssRule.Declarations);
                }
            }

            CssComputedDeclarationResolver resolver = new CssComputedDeclarationResolver(true);

            // Create styles from selectors.
            foreach (CssStyleRule cssRule in CssStyleTracker.DocumentStyleRules)
            {
                // WORDSNET-24917 Resolves specified CSS declarations for computing during updating the predefined styles.
                CssDeclarationCollection computedRuleDeclarations = resolver.ResolveToComputed(cssRule.Declarations);

                foreach (CssSelector selector in cssRule.Selectors)
                {
                    // Element name selectors "h1".."h6"
                    CssTypeSelector selectorAsType = selector as CssTypeSelector;
                    if (selectorAsType != null)
                    {
                        StyleIdentifier styleIdentifier = ElementNameToHeadingStyleIdentifier(selectorAsType.ElementName);
                        if (styleIdentifier != StyleIdentifier.Nil)
                        {
                            GetAndFormatStyle(styleIdentifier, computedRuleDeclarations);
                        }
                        continue;
                    }

                    // Class name selectors like ".someclass"
                    // MS Word discards CSS classes that have no declarations or are marked as "export-only".
                    // Note that the "msochpdefault" style is formatted elsewhere.
                    CssClassSelector selectorAsClass = selector as CssClassSelector;
                    if ((selectorAsClass != null) &&
                        (computedRuleDeclarations.Count > 0) &&
                        (!computedRuleDeclarations.ContainsIdentifier("mso-style-type", "export-only")) &&
                        !string.Equals(selectorAsClass.ClassName, MsoChpDefaultClassName, StringComparison.OrdinalIgnoreCase))
                    {
                        GetAndFormatStyle(selectorAsClass.ClassName, StyleType.Paragraph, computedRuleDeclarations);
                        continue;
                    }

                    // Element name + class name selectors like "p.someclass"
                    CssCompoundSelector selectorAsTypeAndClass = selector as CssCompoundSelector;
                    if (selectorAsTypeAndClass != null)
                    {
                        CssTypeSelector headAsType = selectorAsTypeAndClass.Head as CssTypeSelector;
                        CssClassSelector tailAsClass = selectorAsTypeAndClass.Tail as CssClassSelector;
                        if ((headAsType != null) && (tailAsClass != null))
                        {
                            StyleType styleType = (headAsType.ElementName == "span")
                                ? StyleType.Character
                                : StyleType.Paragraph;
                            GetAndFormatStyle(tailAsClass.ClassName, styleType, computedRuleDeclarations);
                        }
                    }
                }
            }
        }

        internal override Style StyleWithoutDeclarations
        {
            get { return null; }
        }

        protected Style GetAndFormatStyle(
            StyleIdentifier styleIdentifier,
            CssDeclarationCollection declarations)
        {
            // If the style exists, the basic formatting has been applied to it already.
            Style style = Document.Styles.GetBySti(styleIdentifier, false);
            if (style == null)
            {
                // The style doesn't exist so we created it and set up its formatting.
                style = Document.Styles.GetBySti(styleIdentifier, true);
                ApplyBasicFormatting(style);
            }

            ApplyAdditionalFormatting(style, declarations);
            return style;
        }

        protected virtual Style GetAndFormatStyle(
            string preferredName,
            StyleType styleType,
            CssDeclarationCollection declarations)
        {
            Debug.Assert(StringUtil.HasChars(preferredName));
            Style style;
            // WORDSNET-24917 Updates the 'MsoNormal' style in accordance with the MS Word behavior.
            // WORDSNET-27053 'MsoNormal' is only applicable to paragraphs.
            if ((styleType == StyleType.Paragraph) &&
                string.Equals(preferredName, MsoNormalClassName, StringComparison.OrdinalIgnoreCase))
            {
                style = GetAndFormatStyle(StyleIdentifier.Normal, declarations);
                style.RunPr.Remove(FontAttr.CharacterCategoryHint);
                // MS Word changes the style properties only if it has any declarations.
                if (declarations.Count > 0)
                {
                    ParaPr styleParaPr = style.ParaPr;
                    if (declarations["margin-left"] == null)
                    {
                        if (!styleParaPr.Contains(ParaAttr.SpaceBefore))
                            styleParaPr.SpaceBefore = 100;
                        if (!styleParaPr.Contains(ParaAttr.SpaceBeforeAuto))
                            styleParaPr.SpaceBeforeAuto = true;
                    }
                    if (declarations["margin-right"] == null)
                    {
                        if (!styleParaPr.Contains(ParaAttr.SpaceAfter))
                            styleParaPr.SpaceAfter = 100;
                        if (!styleParaPr.Contains(ParaAttr.SpaceAfterAuto))
                            styleParaPr.SpaceAfterAuto = true;
                    }
                }
            }
            else
            {
                StringToStringDictionary createdStylesDictionary;
                switch (styleType)
                {
                    case StyleType.Paragraph:
                        createdStylesDictionary = mParagraphStylesCreatedByName;
                        break;
                    case StyleType.Character:
                        createdStylesDictionary = mCharacterStylesCreatedByName;
                        break;
                    default:
                        Debug.Fail("Unexpected style type");
                        return null;
                }

                // MS Word converts CSS class names to lower case.
                preferredName = preferredName.ToLowerInvariant();

                // Try to reuse the style we've created earlier for the same preferred name.
                string usedName = createdStylesDictionary[preferredName];
                if (usedName == null)
                {
                    // No style to reuse. We'll create a new one after we choose an unique name for it.
                    usedName = preferredName;
                    Style existingStyle = Document.Styles.GetByName(usedName, false);
                    int i = 1;
                    while ((existingStyle != null) && (existingStyle.Type != styleType))
                    {
                        usedName = preferredName + FormatterPal.IntToStr(i);
                        i++;
                        existingStyle = Document.Styles.GetByName(usedName, false);
                    }
                }

                style = Document.Styles.GetByName(usedName, false);
                if (style == null)
                {
                    int istd = Document.Styles.GetNextFreeIstd();
                    style = Style.Create(styleType, istd, StyleIdentifier.User, usedName);
                    style.BasedOnIstd = (styleType == StyleType.Character)
                        ? StyleIndex.DefaultParagraphFont
                        : StyleIndex.Normal;
                    Document.Styles.Add(style);
                    ApplyBasicFormatting(style);
                    createdStylesDictionary.Add(preferredName, usedName);
                }

                ApplyAdditionalFormatting(style, declarations);
            }

            return style;
        }

        protected virtual void ApplyBasicFormatting(Style style)
        {
            Debug.Assert(style != null);
            // WORDSNET-24917 MS Word sets the following default values for the Normal Web style.
            if (style.StyleIdentifier == StyleIdentifier.NormalWeb)
            {
                style.SemiHidden = true;
                style.UnhideWhenUsed = true;
                style.Priority = 99;
            }
            ApplyDefaultFormatting(style);
            ApplyFormattingFromElementSelectors(style);
        }

        private Style GetParagraphStyle()
        {
            IHtmlElementProvider element = CssStyleTracker.CurrentElement;

            StyleIdentifier headingStyleIdentifier = ElementNameToHeadingStyleIdentifier(element.ElementName);
            if (headingStyleIdentifier != StyleIdentifier.Nil)
            {
                return GetAndFormatStyle(headingStyleIdentifier, CssDeclarationCollection.Empty);
            }

            // In most cases, imported <p> elements are formatted using the "Normal (Web)" style.
            //
            // Implicit <p> elements don't exist in the source HTML, so MS Word doesn't apply 'Normal (Web)' to them.
            //
            // WORDSNET-21335 When importing an AltChunk, MS Word applies the "Normal (Web)" style to text that
            // is inside a paragraph element in HTML and the "Normal" style to text that is outside HTML paragraphs
            // (is inside the HTML body or a table cell). In order to mimic this behavior, we apply the "Normal" style
            // to empty paragraphs and, as a result, to out-of-paragraph text that follows them.
            // Note that CurrentElementHasChildren may return "false" here in case a <p> element contains nothing
            // but an ::after pseudo-element. However, since MS Word doesn't support pseudo-elements, it also considers
            // such HTML paragraphs as containing no children.
            if ((element.ElementName == "p")
                && !element.IsImplicit
                && CssStyleTracker.CurrentElementHasChildren)
            {
                string[] classes = element.GetClasses();
                if (classes.Length > 0)
                {
                    string className = classes[0];

                    // WORDSNET-17035 "MsoNormal" class name has special meaning for MS Word: it is an alias for the
                    // "Normal" paragraph style. <p> elements with the "MsoNormal" class are formatted using the "Normal" style
                    // (not the "Web" version).
                    if (string.Equals(className, MsoNormalClassName, StringComparison.OrdinalIgnoreCase))
                    {
                        return GetAndFormatStyle(StyleIdentifier.Normal, CssDeclarationCollection.Empty);
                    }

                    return GetAndFormatStyle(className, StyleType.Paragraph, CssDeclarationCollection.Empty);
                }

                return GetAndFormatStyle(StyleIdentifier.NormalWeb, CssDeclarationCollection.Empty);
            }

            return GetAndFormatStyle(StyleIdentifier.Normal, CssDeclarationCollection.Empty);
        }

        private static StyleIdentifier ElementNameToHeadingStyleIdentifier(string elementName)
        {
            switch (elementName)
            {
                case "h1":
                    return StyleIdentifier.Heading1;
                case "h2":
                    return StyleIdentifier.Heading2;
                case "h3":
                    return StyleIdentifier.Heading3;
                case "h4":
                    return StyleIdentifier.Heading4;
                case "h5":
                    return StyleIdentifier.Heading5;
                case "h6":
                    return StyleIdentifier.Heading6;
                default:
                    return StyleIdentifier.Nil;
            }
        }

        private static StyleIdentifier ElementNameToCharacterStyleIdentifier(string elementName)
        {
            switch (elementName)
            {
                case "a":
                    return StyleIdentifier.Hyperlink;
                case "em":
                    return StyleIdentifier.Emphasis;
                case "strong":
                    return StyleIdentifier.Strong;
                default:
                    return StyleIdentifier.Nil;
            }
        }

        private static void ApplyDefaultFormatting(Style style)
        {
            // Only paragraph styles have default formatting in MS Word.
            if (style.Type != StyleType.Paragraph)
            {
                return;
            }

            ParaPr defaultParaPr = new ParaPr();
            defaultParaPr.Alignment = ParagraphAlignment.Left;
            defaultParaPr.SpaceBefore = 100;
            defaultParaPr.SpaceBeforeAuto = true;
            defaultParaPr.SpaceAfter = 100;
            defaultParaPr.SpaceAfterAuto = true;
            defaultParaPr.LineSpacing = 240;
            defaultParaPr.LineSpacingRule = LineSpacingRule.Multiple;
            defaultParaPr.KeepWithNext = false;
            defaultParaPr.KeepTogether = false;

            int defaultFontSize;
            switch (style.StyleIdentifier)
            {
                case StyleIdentifier.Heading1:
                    defaultFontSize = 48;
                    break;
                case StyleIdentifier.Heading2:
                    defaultFontSize = 36;
                    break;
                case StyleIdentifier.Heading3:
                    defaultFontSize = 27;
                    break;
                case StyleIdentifier.Heading5:
                    defaultFontSize = 20;
                    break;
                case StyleIdentifier.Heading6:
                    defaultFontSize = 15;
                    break;
                default:
                    defaultFontSize = 24;
                    break;
            }

            RunPr defaultRunPr = new RunPr();
            defaultRunPr.NameAscii = "Times New Roman";
            defaultRunPr.NameBi = "Times New Roman";
            defaultRunPr.NameOther = "Times New Roman";
            defaultRunPr.ComplexNameFarEast = ComplexFontName.FromTheme(ThemeFontCore.MinorEastAsia);
            defaultRunPr.Size = defaultFontSize;
            defaultRunPr.SizeBi = defaultFontSize;
            defaultRunPr.Color = DrColor.Empty;

            if (style.StyleIdentifier == StyleIdentifier.Heading1)
            {
                defaultRunPr.Kerning = 36;
            }
            if (style.IsHeading)
            {
                defaultRunPr.Bold = AttrBoolEx.True;
                defaultRunPr.BoldBi = AttrBoolEx.True;
                defaultRunPr.Italic = AttrBoolEx.False;
                defaultRunPr.ItalicBi = AttrBoolEx.False;
            }

            ApplyFormattingToStyle(style, defaultParaPr, defaultRunPr);
        }

        private void ApplyFormattingFromElementSelectors(Style style)
        {
            // In MS Word, declarations imported from 'p' and 'span' selectors are not applied to heading styles.
            if (style.IsHeading)
            {
                return;
            }

            ParaPr paraPr;
            RunPr runPr;
            switch (style.Type)
            {
                case StyleType.Paragraph:
                {
                    paraPr = mParaPrFromParagraphSelector;
                    runPr = mRunPrFromParagraphSelector;
                    break;
                }
                case StyleType.Character:
                {
                    paraPr = null;
                    runPr = mRunPrFromSpanSelector;
                    break;
                }
                default:
                {
                    return;
                }
            }
            ApplyFormattingToStyle(style, paraPr, runPr);
        }

        private void ApplyAdditionalFormatting(Style style, CssDeclarationCollection declarations)
        {
            if (((style.Type != StyleType.Paragraph) && (style.Type != StyleType.Character)) ||
                (declarations.Count == 0))
            {
                return;
            }

            if (style.Type == StyleType.Paragraph)
            {
                ParaPr paraPr = new ParaPr();
                ParagraphFormatter.Format(new ParagraphFormat(paraPr, Document.Styles), declarations);
                ApplyFormattingToStyle(style, paraPr, null);
            }

            RunPr runPr = new RunPr();
            FontFormatter.Format(Font.MakeFont(runPr, Document), style.Type, declarations);
            if (style.Type == StyleType.Character)
            {
                // MS Word applies all formatting to character styles, even if it duplicates inherited formatting. The reason
                // is that inherited formatting for a character style also depends on the paragraph style at the place where
                // text is located. Consequently, inherited formatting for a character style is not final at the moment
                // the style is formatted.
                runPr.CopyTo(style.RunPr);
            }
            else
            {
                ApplyFormattingToStyle(style, null, runPr);
            }
        }

        private static void ApplyFormattingToStyle(Style style, ParaPr newParaPr, RunPr newRunPr)
        {
            // Paragraph formatting.
            if (newParaPr != null)
            {
                for (int i = 0; i < newParaPr.Count; i++)
                {
                    int key = newParaPr.GetKey(i);
                    object desiredValue = newParaPr.GetByIndex(i);
                    Debug.Assert(desiredValue != null);
                    object inheritedValue = ((IParaAttrSource)style).FetchInheritedParaAttr(key);
                    if (desiredValue.Equals(inheritedValue))
                    {
                        // Don't add direct formatting. Reuse the inherited value.
                        style.ParaPr.Remove(key);
                    }
                    else
                    {
                        // Override the inherited value by direct formatting.
                        style.ParaPr.SetAttr(key, desiredValue);
                    }
                }
            }

            // Character formatting.
            if (newRunPr != null)
            {
                for (int i = 0; i < newRunPr.Count; i++)
                {
                    int key = newRunPr.GetKey(i);
                    object desiredValue = newRunPr.GetByIndex(i);
                    Debug.Assert(desiredValue != null);
                    object inheritedValue = style.GetInheritedFontAttr(key, true);
                    if (desiredValue.Equals(inheritedValue))
                    {
                        // Don't add direct formatting. Reuse the inherited value.
                        style.RunPr.Remove(key);
                    }
                    else
                    {
                        // Override the inherited value by direct formatting.
                        style.RunPr.SetAttr(key, desiredValue);
                    }
                }
            }
        }

        private Style GetCharacterStyle()
        {
            // In MS Word, there are built-in character styles for certain elements.
            StyleIdentifier styleIdentifierByName =
                ElementNameToCharacterStyleIdentifier(CssStyleTracker.CurrentElement.ElementName);
            if (styleIdentifierByName != StyleIdentifier.Nil)
            {
                return GetAndFormatStyle(styleIdentifierByName, CssDeclarationCollection.Empty);
            }

            // MS Word creates character styles from CSS class names only for "span" elements.
            if (CssStyleTracker.CurrentElement.ElementName == "span")
            {
                string[] classes = CssStyleTracker.CurrentElement.GetClasses();
                if (classes.Length > 0)
                {
                    // MS Word doesn't support multiple class names like <span class="c1 c2">.
                    string className = classes[0];
                    Style styleFromClass = GetAndFormatStyle(className, StyleType.Character, CssDeclarationCollection.Empty);

                    // It's possible that the style specified by the class name is not a character style and cannot be applied.
                    if (styleFromClass == null)
                    {
                        return null;
                    }

                    // MS Word applies run properties from '.class' selectors to character styles created by class name.
                    foreach (CssStyleRule cssRule in CssStyleTracker.DocumentStyleRules)
                    {
                        foreach (CssSelector selector in cssRule.Selectors)
                        {
                            // Class name selectors like ".someclass"
                            CssClassSelector selectorAsClass = selector as CssClassSelector;
                            if ((selectorAsClass != null) && (selectorAsClass.ClassName == classes[0]))
                            {
                                ApplyAdditionalFormatting(styleFromClass, cssRule.Declarations);
                            }
                        }
                    }

                    return styleFromClass;
                }
            }

            // In MS Word, if inline element doesn't have its own style it inherits style from parent span element.
            Style parentCharacterStyle = GetParentCharacterStyle();
            return (parentCharacterStyle != null)
                ? parentCharacterStyle
                : GetAndFormatStyle(StyleIdentifier.DefaultParagraphFont, CssDeclarationCollection.Empty);
        }

        private const string MsoNormalClassName = "msonormal";
        private const string MsoChpDefaultClassName = "msochpdefault";

        /// <summary>
        /// Paragraph properties imported from 'p' selectors. These properties are applied to all new paragraph styles.
        /// </summary>
        private readonly ParaPr mParaPrFromParagraphSelector = new ParaPr();

        /// <summary>
        /// Run properties imported from 'p' selectors. These properties are applied to all new paragraph styles.
        /// </summary>
        private readonly RunPr mRunPrFromParagraphSelector = new RunPr();

        /// <summary>
        /// Run properties imported from 'span' selectors. These properties are applied to all new character styles.
        /// </summary>
        private readonly RunPr mRunPrFromSpanSelector = new RunPr();

        private readonly StringToStringDictionary mParagraphStylesCreatedByName = new StringToStringDictionary();
        private readonly StringToStringDictionary mCharacterStylesCreatedByName = new StringToStringDictionary();
    }
}
