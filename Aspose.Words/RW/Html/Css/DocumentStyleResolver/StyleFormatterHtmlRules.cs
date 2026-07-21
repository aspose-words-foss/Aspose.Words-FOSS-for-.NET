// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2016 by Victor Chebotok

using System;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Gets and formats appropriate model styles when CSS formatting is applied to the document model. to paragraphs.
    /// Uses formatting rules that are different from what MS Word uses but produce better looking results
    /// and are more confomant to the CSS and HTML specifications.
    /// </summary>
    internal class StyleFormatterHtmlRules : StyleFormatter
    {
        internal StyleFormatterHtmlRules(
            Document document,
            CssStyleTracker cssStyleTracker,
            int defaultParagraphIstd,
            int defaultFontIstd,
            FontFormatter fontFormatter,
            ParagraphFormatter paragraphFormatter)
            : base(document, cssStyleTracker, paragraphFormatter, fontFormatter)
        {
            mSelectorStyleCache = new HtmlSelectorStyleCache();

            mDefaultParagraphIstd = defaultParagraphIstd;
            mDefaultFontIstd = defaultFontIstd;
        }

        /// <summary>
        /// Computes default CSS styles for the given HTML document and applies this CSS to the predefined styles.
        /// Predefined styles are not created explicitly, just updated with CSS information.
        /// </summary>
        internal override void UpdatePredefinedStyles()
        {
            // Heading styles.
            // Note that there are 9 built-in heading styles in MS Word but only 6 heading elements in HTML (h1..h6).
            // However, HTML documents can define formating of headings 7..9 using the "-aw-style-name" property.
            Debug.Assert((int)StyleIdentifier.Heading1 == 1);
            Debug.Assert((int)StyleIdentifier.Heading9 == 9);
            for (StyleIdentifier sti = StyleIdentifier.Heading1; sti <= StyleIdentifier.Heading9; sti++)
            {
                CssDeclarationCollection headingStyleDeclarations = CssStyleTracker.GetPredefinedStyleDeclarations(sti);
                if (headingStyleDeclarations != null)
                {
                    Style style = Document.Styles.GetBySti(sti, true);
                    Format(style, headingStyleDeclarations);

                    // WORDSNET-9026 Use default font name for heading styles.
                    // FOSS: font substitution was removed; the default substitution font name was "Times New Roman".
                    if (headingStyleDeclarations["font-family"] == null)
                        style.Font.Name = "Times New Roman";
                    // WORDSNET-8693 By default, <h2> and <h5> elements use normal font, but 'Heading 2' and 'Heading 5' use italic font.
                    if (headingStyleDeclarations["font-style"] == null)
                        style.Font.Italic = false;
                }
            }

            // WORDSNET-12924 MS Word sets kerning to 18pt on the 'Heading 1' style and so do we. Otherwise text imported
            // from <h1> elements gets longer and may wrap inappropriately.
            // Note that we don't want to create the 'Heading 1' style here unless it's explicitly declared in the document's
            // stylesheet.
            Style heading1Style = Document.Styles.GetBySti(StyleIdentifier.Heading1, false);
            if (heading1Style != null)
            {
                heading1Style.Font.Kerning = 18;
            }

            // Normal style.
            UpdatePredefinedStyle(StyleIdentifier.Normal);

            // Supported character styles.
            foreach (StyleIdentifier styleIdentifier in CssDocumentStyleNames.GetCharacterStyleIdentifiers())
            {
                UpdatePredefinedStyle(styleIdentifier);
            }
        }

        /// <summary>
        /// Returns a style for the current HTML element. Can return null.
        /// </summary>
        internal override Style GetModelStyle(StyleType styleType)
        {
            mStyleWithoutDeclarations = null;

            CssDeclaration styleNameDeclaration = CssStyleTracker.ElementDeclarations[HtmlConstants.StyleName];
            if (styleNameDeclaration != null)
            {
                Style styleByStyleName = GetStyleByStyleNameDeclaration(styleNameDeclaration, styleType);
                if (styleByStyleName != null)
                {
                    return styleByStyleName;
                }
            }

            CssSelector preferableSelector = null;
            foreach (CssSelector selector in CssStyleTracker.CurrentElementSelectors)
            {
                if ((preferableSelector == null) || (selector.Specificity.CompareTo(preferableSelector.Specificity) == 1))
                    preferableSelector = selector;
            }
            if (preferableSelector != null)
            {
                return GetStyleBySelector(preferableSelector, styleType);
            }

            string elementName = CssStyleTracker.CurrentElement.ElementName;
            string classValue = CssStyleTracker.CurrentElement.GetAttributeValue("class", string.Empty);
            if (((elementName == "p") || (elementName == "span") || (elementName == "table")) && (classValue != string.Empty))
            {
                Style style = GetStyleByClassAttribute(classValue, styleType);
                mStyleWithoutDeclarations = style;
                return style;
            }

            string hrefAttributeValue = CssStyleTracker.CurrentElement.GetAttributeValue("href");
            return GetStyleByElement(
                CssStyleTracker.CurrentElement,
                hrefAttributeValue,
                styleType);
        }

        /// <summary>
        /// Gets style which is specified as element's class but has no corresponding declarations;
        /// </summary>
        internal override Style StyleWithoutDeclarations
        {
            get { return mStyleWithoutDeclarations; }
        }

        private void UpdatePredefinedStyle(StyleIdentifier styleIdentifier)
        {
            CssDeclarationCollection declarations = CssStyleTracker.GetPredefinedStyleDeclarations(styleIdentifier);
            if (declarations != null)
            {
                Style style = Document.Styles.GetBySti(styleIdentifier, true);
                Format(style, declarations);
            }
        }

        private Style GetStyleByStyleNameDeclaration(CssDeclaration styleNameDeclaration, StyleType styleType)
        {
            StyleIdentifier styleIdentifier = CssDocumentStyleNames.CssStyleNameToStyleIdentifier(styleNameDeclaration.Value);
            // The type of the specified style must match the requested style type. It is possible, for example, to specify
            // a character style (say, "-aw-style-name:hyperlink") for a HTML paragraph, and we want to ignore such situations.
            // This also handles situations where the value of "-aw-style-name" has not been recognized and the style identifier
            // is nil.
            return CssDocumentStyleNames.ValidateStyleType(styleIdentifier, styleType)
                ? Document.Styles.GetBySti(styleIdentifier, true)
                : null;
        }

        private Style GetStyleByElement(
            IHtmlElementProvider element,
            string hrefAttributeValue,
            StyleType styleType)
        {
            StyleIdentifier styleIdentifier = StyleIdentifier.Nil;
            if (styleType == StyleType.Paragraph)
            {
                switch (element.ElementName.ToLowerInvariant())
                {
                    case "h1":
                        styleIdentifier = StyleIdentifier.Heading1;
                        break;
                    case "h2":
                        styleIdentifier = StyleIdentifier.Heading2;
                        break;
                    case "h3":
                        styleIdentifier = StyleIdentifier.Heading3;
                        break;
                    case "h4":
                        styleIdentifier = StyleIdentifier.Heading4;
                        break;
                    case "h5":
                        styleIdentifier = StyleIdentifier.Heading5;
                        break;
                    case "h6":
                        styleIdentifier = StyleIdentifier.Heading6;
                        break;
                    default:
                        // Ignore.
                        break;
                }
            }

            // WORDSNET-27579 Map <a>, <em>, and <strong> to built-in character styles.
            if (styleType == StyleType.Character)
            {
                switch (element.ElementName.ToLowerInvariant())
                {
                    case "a":
                        // In HTML, an <a> element is a hyperlink only if it has a non-empty "href" attribute.
                        if (StringUtil.HasChars(hrefAttributeValue))
                        {
                            styleIdentifier = StyleIdentifier.Hyperlink;
                        }
                        break;
                    case "em":
                        styleIdentifier = StyleIdentifier.Emphasis;
                        break;
                    case "strong":
                        styleIdentifier = StyleIdentifier.Strong;
                        break;
                    default:
                        // Ignore.
                        break;
                }
            }

            // Return a built-in style. Create it if needed.
            if (styleIdentifier != StyleIdentifier.Nil)
            {
                return Document.Styles.GetBySti(styleIdentifier, true);
            }

            // In MS Word if inline element doesn't have its own style it inherits style from parent span element.
            if (styleType == StyleType.Character)
            {
                Style parentCharacterStyle = GetParentCharacterStyle();
                if (parentCharacterStyle != null)
                {
                    return parentCharacterStyle;
                }
            }

            // Return a default style. Create it if needed.
            int istd = GetDefaultStyleIstd(styleType);
            return (istd != StyleIndex.Nil)
                ? Document.Styles.GetByIstd(istd, true)
                : null;
        }

        /// <summary>
        /// Gets document style of the specified type for the selector.
        /// </summary>
        /// <param name="selector">CSS selector.</param>
        /// <param name="styleType">Document style type.</param>
        /// <returns>
        /// New or existing document style; null, if style isn't defined or there are too many styles in the document.
        /// </returns>
        private Style GetStyleBySelector(CssSelector selector, StyleType styleType)
        {
            Style style = mSelectorStyleCache.GetStyle(selector, styleType);
            if (style == null)
            {
                style = GetStyleBySelectorUncached(selector, styleType);
                mSelectorStyleCache.AddStyle(selector, styleType, style);
            }

            // A value returned from cache may be a substituted null style.
            if (mSelectorStyleCache.IsNullSubstitute(style))
            {
                style = null;
            }

            Debug.Assert((style == null) || (style.Document == Document));
            return style;
        }

        /// <summary>
        /// Gets document style of the specified type for the selector.
        /// </summary>
        /// <param name="selector">CSS selector.</param>
        /// <param name="styleType">Document style type.</param>
        /// <returns>
        /// New or existing document style; null, if style isn't defined or there are too many styles in the document.
        /// </returns>
        private Style GetStyleBySelectorUncached(CssSelector selector, StyleType styleType)
        {
            Style existingStyle = GetPredefinedStyle(selector, styleType);
            if (existingStyle != null)
            {
                return existingStyle;
            }

            string preferredStyleName = null;

            CssDeclarationCollection selectorDeclarations = CssStyleTracker.GetSelectorDeclarations(selector);
            if (selectorDeclarations != null)
            {
                CssDeclaration styleNameDeclaration = selectorDeclarations[HtmlConstants.StyleName];
                if (styleNameDeclaration != null)
                {
                    preferredStyleName = selectorDeclarations.GetString(HtmlConstants.StyleName);
                }
            }

            // WORDSNET-21412 If possible, we save names of character styles as class names of "span.class" selectors. Here
            // we try to restore such names.
            if (!StringUtil.HasChars(preferredStyleName) && (styleType == StyleType.Character))
            {
                preferredStyleName = ParseSpanClassSelector(selector);
            }

            if (!StringUtil.HasChars(preferredStyleName))
            {
                preferredStyleName = selector.GetPreferableStyleName();
            }

            // Try to find an existing style.
            existingStyle = Document.Styles[preferredStyleName];
            if ((existingStyle != null) && (existingStyle.Type != styleType))
            {
                // The type of the existing style doesn't match the requested type and the style cannot be reused.
                // We have to modify the name of imported style.
                preferredStyleName = string.Format("{0} {1}", preferredStyleName, GetStyleTypeName(styleType));
                existingStyle = null;
            }

            if (selectorDeclarations == null)
            {
                int istd = GetDefaultStyleIstd(styleType);
                return (istd != StyleIndex.Nil)
                    ? Document.Styles.GetByIstd(istd, true)
                    : null;
            }

            // In case the selector has no declarations we try to reuse an existing style as if there were no selector at all.
            if ((selectorDeclarations.Count == 0) && (existingStyle != null))
            {
                return existingStyle;
            }

            // We always create a new style first. We need a new style in order to compare its formatting to formatting of
            // styles imported before and decide whether we could reuse an existing style. If we decide to reuse, the new
            // style is removed.
            Style newStyle = CreateNewStyle(preferredStyleName, styleType);

            // Style creation will fail if there are too many styles in the document.
            if (newStyle == null)
            {
                return null;
            }

            newStyle.BasedOnIstd = GetBaseStyleIstd(newStyle, selector, selectorDeclarations);
            Format(newStyle, selectorDeclarations);

            // Compare formatting of imported and existing styles and decide if we can reuse an existing style.
            // Search for a custom (not built-in) style that is equal to the newly created one.
            // Skip the new style that we've just created. Note that the new style is not necessarily last on the list.
            // It might be inserted into the middle of the list in place of a previously removed style, so we should
            // also search for reuse candidates past the new style.
            // Skip built-in styles. We're importing a new style from HTML, so it is user-defined and it cannot
            // be mapped to a built-in style.
            string existingStyleName = preferredStyleName;
            int i = 0;
            Style reusableStyle;
            do
            {
                reusableStyle = Document.Styles.GetByName(existingStyleName, false);

                // It is important that the style names generated here match unique style names that are generated when new
                // styles are created. In this way we try to limit the number of reuse candidates and check only styles that
                // are most likely created by previous HTML import sessions.
                existingStyleName = string.Format("{0}_{1}", preferredStyleName, i);
                i++;
            }
            while ((reusableStyle != null) &&
                (ReferenceEquals(reusableStyle, newStyle) || reusableStyle.BuiltIn || (!reusableStyle.Equals(newStyle))));

            // An existing user-defined style can be reused.
            if (reusableStyle != null)
            {
                // So the new style is not needed.
                Document.Styles.RemoveCore(newStyle);
                return reusableStyle;
            }

            // No reusable existing style is found, so we return the newly created style.
            return newStyle;
        }

        /// <summary>
        /// Parses a selector that looks like "span.class" and returns the class name. If cannot parse, returns <c>null</c>.
        /// </summary>
        private static string ParseSpanClassSelector(CssSelector selector)
        {
            CssCompoundSelector compoundSelector = selector as CssCompoundSelector;
            if (compoundSelector == null)
            {
                return null;
            }

            CssTypeSelector head = compoundSelector.Head as CssTypeSelector;
            if ((head == null) || (head.ElementName != "span"))
            {
                return null;
            }

            CssClassSelector tail = compoundSelector.Tail as CssClassSelector;
            if (tail == null)
            {
                return null;
            }

            return tail.ClassName;
        }

        private Style GetStyleByClassAttribute(string classAttributeValue, StyleType styleType)
        {
            string styleName = classAttributeValue;

            // Try to find an existing style.
            Style style = Document.Styles[styleName];
            if ((style != null) && (style.Type != styleType))
            {
                styleName = string.Format("{0} {1}", styleName, GetStyleTypeName(styleType));
                style = Document.Styles[styleName];
            }
            if ((style != null) && (style.Type != styleType))
            {
                style = null;
            }

            return (style != null)
                ? style
                : CreateNewStyle(styleName, styleType);
        }

        /// <summary>
        /// Creates a new style of the specified type.
        /// </summary>
        /// <returns>
        /// A new style or <c>null</c> if there are too many styles in the document already.
        /// </returns>
        private Style CreateNewStyle(string preferredName, StyleType type)
        {
            // This method is not designed for creation of list styles.
            Debug.Assert(type != StyleType.List);

            if (mTooManyStyles)
            {
                return null;
            }

            int istd;
            try
            {
                istd = Document.Styles.GetNextFreeIstd();
            }
            catch (InvalidOperationException)
            {
                mTooManyStyles = true;
                return null;
            }

            string name = Document.Styles.GetUniqueStyleName(preferredName);
            Style style = Style.Create(type, istd, StyleIdentifier.User, name);
            Document.Styles.Add(style);
            style.BasedOnIstd = (type != StyleType.Table)
                ? GetDefaultStyleIstd(type)
                : StyleIndex.TableNormal;
            return style;
        }

        private static string GetStyleTypeName(StyleType styleType)
        {
            switch (styleType)
            {
                case StyleType.Paragraph:
                    return "Paragraph";
                case StyleType.Character:
                    return "Character";
                case StyleType.Table:
                    return "Table";
                default:
                    Debug.Assert(false);
                    return string.Empty;
            }
        }

        private Style GetPredefinedStyle(CssSelector selector, StyleType styleType)
        {
            if (styleType != StyleType.Paragraph)
                return null;

            StyleIdentifier sti;
            switch (selector.ToCss())
            {
                case "h1":
                    sti = StyleIdentifier.Heading1;
                    break;
                case "h2":
                    sti = StyleIdentifier.Heading2;
                    break;
                case "h3":
                    sti = StyleIdentifier.Heading3;
                    break;
                case "h4":
                    sti = StyleIdentifier.Heading4;
                    break;
                case "h5":
                    sti = StyleIdentifier.Heading5;
                    break;
                case "h6":
                    sti = StyleIdentifier.Heading6;
                    break;
                default:
                    sti = StyleIdentifier.Nil;
                    break;
            }

            return (sti != StyleIdentifier.Nil)
                       ? Document.Styles.GetBySti(sti, true)
                       : null;
        }

        /// <summary>
        /// Gets base style Istd for specified style and selector.
        /// </summary>
        /// <param name="style">Document style.</param>
        /// <param name="selector">CSS selector.</param>
        /// <param name="declarations">CSS declarations of the selector's rule.</param>
        /// <returns>Base style Istd.</returns>
        private int GetBaseStyleIstd(
            Style style,
            CssSelector selector,
            CssDeclarationCollection declarations)
        {
            if (style.Type == StyleType.Table)
            {
                return StyleIndex.TableNormal;
            }

            CssDeclaration parentStyleDeclaration = declarations[HtmlConstants.StyleParent];
            if (parentStyleDeclaration != null)
            {
                StyleIdentifier parentStyleIdentifier = CssDocumentStyleNames.CssStyleNameToStyleIdentifier(parentStyleDeclaration.Value);
                if (CssDocumentStyleNames.ValidateStyleType(parentStyleIdentifier, style.Type))
                {
                    Style baseStyle = style.Styles.GetBySti(parentStyleIdentifier, false);
                    if (baseStyle != null)
                    {
                        return baseStyle.Istd;
                    }
                }
            }

            if (selector is CssCompoundSelector)
            {
                CssCompoundSelector compoundSelector = (CssCompoundSelector)selector;
                // Style of a selector like element1.class1 should be based on the element1 style.
                if ((compoundSelector.Head is CssTypeSelector) && (compoundSelector.Tail is CssClassSelector))
                {
                    Style baseStyle = GetStyleBySelector(compoundSelector.Head, style.Type);
                    if (baseStyle != null)
                    {
                        return baseStyle.Istd;
                    }
                }
            }

            return GetDefaultStyleIstd(style.Type);
        }

        private int GetDefaultStyleIstd(StyleType styleType)
        {
            int istd;
            switch (styleType)
            {
                case StyleType.Paragraph:
                    istd = mDefaultParagraphIstd;
                    break;
                case StyleType.Character:
                    istd = mDefaultFontIstd;
                    break;
                case StyleType.Table:
                    istd = StyleIndex.Nil;
                    break;
                default:
                    istd = StyleIndex.Nil;
                    Debug.Assert(false);
                    break;
            }
            return istd;
        }

        private void Format(Style style, CssDeclarationCollection declarations)
        {
            if ((style.Type != StyleType.Paragraph) && (style.Type != StyleType.Character))
            {
                return;
            }

            declarations.DebugCheckAllComputed();
            if (style.Type == StyleType.Paragraph)
            {
                ParagraphFormatter.Format(style.ParagraphFormat, declarations);
            }
            FontFormatter.Format(style.Font, style.Type, declarations);

            AddAliases(style, declarations);
        }

        private static void AddAliases(Style style, CssDeclarationCollection declarations)
        {
            CssDeclaration styleAliasesDeclaration = declarations[HtmlConstants.StyleAliases];
            if (styleAliasesDeclaration == null)
            {
                return;
            }

            // Alias list is a sequence of string values.
            for (int i = 0; i < styleAliasesDeclaration.Value.Count; i++)
            {
                CssStringValue stringValue = styleAliasesDeclaration.Value[i] as CssStringValue;
                if (stringValue != null)
                {
                    string alias = stringValue.Value;

                    // MS Word combines style name and all aliases into a single comma-separated string. As a result, aliases
                    // cannot contain comma characters. It's possible to add such an alias to a style, but the alias
                    // will not work correctly and will not be preserved upon re-saving to MS Word's formats.
                    if (StringUtil.HasChars(alias) && (alias.IndexOf(',') < 0))
                    {
                        style.AddAlias(stringValue.Value);
                    }
                }
            }
        }

        private readonly HtmlSelectorStyleCache mSelectorStyleCache;

        private bool mTooManyStyles;

        /// <summary>
        /// Index of the style that all paragraph styles imported from CSS will be based on.
        /// </summary>
        private readonly int mDefaultParagraphIstd;

        /// <summary>
        /// Index of the style that all font styles imported from CSS will be based on.
        /// </summary>
        private readonly int mDefaultFontIstd;

        private Style mStyleWithoutDeclarations;
    }
}
