// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/02/2013 by Alexey Butalov

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Reads multiple CSS style sheets.
    /// </summary>
    internal class CssReader
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="baseUri">Base URI of the document. The base URI is used to resolve relative URI of external CSS documents.</param>
        /// <param name="htmlResourceLoader">HtmlResourceLoader used for loading resources.
        /// This is needed for @import rules when importing from container formats (MHTML).</param>
        /// <param name="documentMode">The mode in which the corresponding HTML document has been opened.</param>
        /// <param name="mimicMsWordBehavior">Indicates whether we should stick to MS Word's behavior when reading CSS.</param>
        internal CssReader(
            string baseUri,
            HtmlResourceLoader htmlResourceLoader,
            CssDocumentMode documentMode,
            bool mimicMsWordBehavior)
        {
            Debug.Assert(htmlResourceLoader != null);

            mHtmlResourceLoader = htmlResourceLoader;
            mStyleSheetsBeingProcessed = new HashSetGeneric<string>();
            mBaseUri = baseUri;
            if (mBaseUri == null)
                mBaseUri = string.Empty;
            mDocumentMode = documentMode;
            mNamespacePrefixResolver = new CssNamespacePrefixResolver();
            mMimicMsWordBehavior = mimicMsWordBehavior;
        }

        /// <summary>
        /// Reads CSS rules starting from this node.
        /// </summary>
        /// <param name="rootElement">Root element of a HTML document.</param>
        internal void ReadAllRules(IElementProvider rootElement)
        {
            StyleRules = new List<CssStyleRule>();
            PageRules = new List<CssPageRule>();
            FontFaceRules = new List<CssFontFaceRule>();
            ListRules = new List<CssListRule>();

            // WORDSNET-9318 Browsers read styles from "style" and "link" nodes
            // throughout the entire document before processing the document.
            CollectElementStyleInformation(rootElement);

            // WORDSNET-27811 MS Word supports only a small subset of CSS selectors. Remove rules with unsupported selectors.
            if (mMimicMsWordBehavior)
            {
                RemoveRulesWithSelectorsNotSupportedByMsWord();
            }

            // WORDSNET-18682 Optimize loaded stylesheet by combining style rules with equal selectors. This also
            // removes duplicate style rules from the stylesheet. As a side effect, rules with more than one selector are
            // split into multiple rules all sharing same declarations: one rule per selector.
            // MS Word processes style rules individually so we cannot optimize them when loading MsoHtml.
            if (!mMimicMsWordBehavior)
            {
                OptimizeStyleRules();
            }
        }

        private void RemoveRulesWithSelectorsNotSupportedByMsWord()
        {
            int i = 0;
            while (i < StyleRules.Count)
            {
                bool ruleHasUnsupportedSelectors = false;
                foreach (CssSelector selector in StyleRules[i].Selectors)
                {
                    MsoHtmlSelector msoHtmlSelector = MsoHtmlSelector.Parse(selector.ToCss());
                    if (!msoHtmlSelector.IsSupportedByMsWord())
                    {
                        ruleHasUnsupportedSelectors = true;
                        break;
                    }
                }
                if (ruleHasUnsupportedSelectors)
                {
                    // Remove the whole rule if any of its selectors is not supported by MS Word.
                    // Continue without incrementing the loop variable, because removal of a list item shifts indexes
                    // of the list's tail elements.
                    StyleRules.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }
        }

        private void OptimizeStyleRules()
        {
            // Contains optimized (combined) CSS rules indexed by their selectors. Note that after optimization each rule
            // has only one selector.
            StringToObjDictionary<CssStyleRule> selectorsToOptimizedRulesMap = new StringToObjDictionary<CssStyleRule>();

            bool anythingOptimized = false;
            foreach (CssStyleRule rule in StyleRules)
            {
                // Split rules by their selectors.
                foreach (CssSelector selector in rule.Selectors)
                {
                    // Since our CSS parser guarantees that in parsed CSS rules no CSS namespaces will share the same prefix,
                    // it's safe to compare selectors from different rules just by their CSS text.
                    string selectorCss = selector.ToCss();

                    CssStyleRule optimizedRule;
                    CssStyleRule ruleWithSameSelector = selectorsToOptimizedRulesMap[selectorCss];
                    if (ruleWithSameSelector != null)
                    {
                        // Found a rule with the same selector. Optimize two rules by combining their declarations.
                        CssDeclarationCollectionBuilder combinedDeclarationsBuilder =
                            new CssDeclarationCollectionBuilder(ruleWithSameSelector.Declarations);
                        // Combine declarations paying attention to their importance.
                        combinedDeclarationsBuilder.AddOrReplaceIfMoreOrEquallyImportant(rule.Declarations);

                        optimizedRule = new CssStyleRule(
                            ruleWithSameSelector.Selectors,
                            combinedDeclarationsBuilder.GetDeclarations());

                        anythingOptimized = true;
                    }
                    else
                    {
                        // No rules with this selector have been processed yet, so there is nothing to optimize.
                        // Remember the rule for further optimization.
                        // MEMORY: Try not to create new instances of selector lists.
                        CssSelector[] singleSelector = (rule.Selectors.Length == 1)
                            ? rule.Selectors
                            : new CssSelector[] { selector };
                        optimizedRule = new CssStyleRule(singleSelector, rule.Declarations);
                    }

                    // Update the optimized rule.
                    selectorsToOptimizedRulesMap[selectorCss] = optimizedRule;
                }
            }

            // Replace loaded rules with optimized version.
            if (anythingOptimized)
            {
                StyleRules.Clear();
                foreach (CssStyleRule styleRule in selectorsToOptimizedRulesMap.Values)
                {
                    StyleRules.Add(styleRule);
                }
            }
        }

        private void CollectElementStyleInformation(IElementProvider element)
        {
            // WORDSNET-20441 We used to do recursive tree traversal here but it caused stack overflow exceptions
            // on huge trees. Now we do non-recursive depth-first traversal instead.
            IElementProvider currentElement = element.GetFirstChildElement();
            while (currentElement != null)
            {
                FindStylesInNode(currentElement);

                IElementProvider firstChild = currentElement.GetFirstChildElement();
                if (firstChild != null)
                {
                    currentElement = firstChild;
                    continue;
                }

                IElementProvider nextSibling = currentElement.GetNextSiblingElement();
                if (nextSibling != null)
                {
                    currentElement = nextSibling;
                    continue;
                }

                IElementProvider parent = currentElement.GetParentElement();
                currentElement = null;
                while ((parent != null) && (parent != element))
                {
                    nextSibling = parent.GetNextSiblingElement();
                    if (nextSibling != null)
                    {
                        currentElement = nextSibling;
                        break;
                    }
                    parent = parent.GetParentElement();
                }
            }
        }

        /// <summary>
        /// Processes one embedded CSS style sheet read into string. At this stage nothing is populated
        /// to the document because we can have multiple style sheets. Styles should be
        /// consolidated and only after that imported to the model.
        /// </summary>
        /// <param name="styleSheet">The text with style sheet</param>
        private void ProcessEmbeddedStyleSheet(string styleSheet)
        {
            ProcessStyleSheetCore(styleSheet, "");
        }

        /// <summary>
        /// Processes external CSS style sheet read into string. At this stage nothing is populated
        /// to the document because we can have multiple style sheets. Styles should be
        /// consolidated and only after that imported to the model.
        /// </summary>
        /// <param name="stylesheetUri">Original URI of this style sheet.</param>
        private void ProcessExternalStyleSheet(string stylesheetUri)
        {
            Debug.Assert(StringUtil.HasChars(stylesheetUri));

            // Use the absolute URI of the stylesheet to make sure we load each stylesheet only once.
            string absoluteUri = UriUtil.ConstructUnescapedAbsoluteUri(mBaseUri, stylesheetUri);
            if (!mStyleSheetsBeingProcessed.Add(absoluteUri))
            {
                return;
            }

            string styleSheet = mHtmlResourceLoader.LoadCss(mBaseUri, stylesheetUri);
            if (styleSheet != null)
            {
                ProcessStyleSheetCore(styleSheet, stylesheetUri);
            }
        }

        /// <summary>
        /// Called on start boundary of any node when collecting style information.
        /// </summary>
        private void FindStylesInNode(IElementProvider element)
        {
            switch (element.ElementName)
            {
                case "style":
                    HandleStyle(element);
                    break;
                case "link":
                    HandleLink(element);
                    break;
                case "base":
                    // If found before a style sheet or link should affect them
                    mBaseUri = HtmlUtil.ValidateUri(element.GetAttributeValue("href", string.Empty));
                    break;
                default:
                    // Styles in other elements are ignored.
                    break;
            }
        }

        /// <summary>
        /// Reads embedded style information from CSS style sheet.
        /// </summary>
        private void HandleStyle(IElementProvider element)
        {
            // The HTML Specification says that:
            // Authors should not specify a type attribute on a style element. If the attribute is present, its value
            // must be an ASCII case-insensitive match for "text/css".
            // See https://html.spec.whatwg.org/multipage/obsolete.html#obsolete-but-conforming-features
            string typeValue = element.GetAttributeValue("type"); // Will be null if "type" is not present.
            if (!StringUtil.HasChars(typeValue) ||
                StringUtil.EqualsIgnoreCase(typeValue, "text/css"))
            {
                ProcessEmbeddedStyleSheet(element.GetInnerText());
            }
        }

        /// <summary>
        /// Reads external style information from CSS style sheet.
        /// Other kinds of links are currently not supported.
        /// </summary>
        private void HandleLink(IElementProvider element)
        {
            if (StringUtil.EqualsIgnoreCase(element.GetAttributeValue("type", "text/css"), "text/css") &&
                StringUtil.EqualsIgnoreCase(element.GetAttributeValue("rel", ""), "stylesheet"))
            {
                // WORDSNET-17788 Previously, we loaded CSS files with 'disabled' attribute.
                // Now we skip such files.
                bool isDisabled = element.GetAttributeValue("disabled") == string.Empty;
                if (isDisabled)
                    return;

                string href = HtmlUtil.ValidateUri(element.GetAttributeValue("href", string.Empty));
                if (href.Length > 0)
                {
                    string media = element.GetAttributeValue("media", string.Empty);
                    CssMediaQueryList mediaQueryList = CssParser.ParseMediaQueryList(media);
                    if (mediaQueryList.IsSupported)
                    {
                        // Resource might be inaccessible.
                        // At this moment just ignore the absence of stylesheet. Most common behavior
                        // is resilience in case when referenced file is inaccessible.
                        // Create the style sheet by either loading from URI or reusing already loaded bytes.
                        // We map binary from original URI since when we decode the resource we might not know the base URI.
                        // Base URI is constant so the relative URIs will be distinguishable.
                        // It's uncertain that the same style sheet will be loaded several times. So this mechanism is needed
                        // only to support HTML-based container formats such MHTML. Don't put loaded style sheets in the hash.
                        ProcessExternalStyleSheet(href);
                    }
                }
            }
        }

        /// <summary>
        /// Processes one CSS style sheet. At this stage nothing is populated to the document because we can have multiple
        /// style sheets. Styles should be consolidated and only after that imported to the model.
        /// </summary>
        /// <param name="styleSheet">The text with style sheet.</param>
        /// <param name="stylesheetUri">
        /// URI of this stylesheet to properly process @import rules. Can be empty but not null.
        /// </param>
        private void ProcessStyleSheetCore(string styleSheet, string stylesheetUri)
        {
            Debug.Assert(styleSheet != null);
            Debug.Assert(stylesheetUri != null);

            IList<CssRule> rules = CssParser.ParseStylesheet(
                styleSheet,
                mDocumentMode,
                stylesheetUri,
                mNamespacePrefixResolver);
            foreach (CssRule rule in rules)
            {
                ProcessRule(rule);
            }
        }

        private void ProcessRule(CssRule rule)
        {

            switch (rule.Type)
            {
                case CssRuleType.Import:
                {
                    CssImportRule importRule = (CssImportRule)rule;
                    if (importRule.MediaQuery.IsSupported)
                    {
                        ProcessExternalStyleSheet(importRule.Url);
                    }
                    break;
                }
                case CssRuleType.Media:
                {
                    ProcessMediaRule((CssMediaRule)rule);
                    break;
                }
                case CssRuleType.Page:
                {
                    PageRules.Add((CssPageRule)rule);
                    break;
                }
                case CssRuleType.Style:
                {
                    StyleRules.Add((CssStyleRule)rule);
                    break;
                }
                case CssRuleType.FontFace:
                {
                    FontFaceRules.Add((CssFontFaceRule)rule);
                    break;
                }
                case CssRuleType.List:
                {
                    ListRules.Add((CssListRule)rule);
                    break;
                }
                default:
                {
                    Debug.Assert(false);
                    break;
                }
            }
        }

        /// <summary>
        /// Processes rules nested in a @media rule.
        /// </summary>
        private void ProcessMediaRule(CssMediaRule rule)
        {
            if (!rule.MediaQueryList.IsSupported)
            {
                return;
            }

            foreach (CssRule nestedRule in rule.Rules)
            {
                ProcessRule(nestedRule);
            }
        }

        /// <summary>
        /// CSS style rules read by <see cref="ReadAllRules" /> function.
        /// </summary>
        internal IList<CssStyleRule> StyleRules { get; private set; }

        /// <summary>
        /// CSS @page rules read by <see cref="ReadAllRules" /> function.
        /// </summary>
        internal IList<CssPageRule> PageRules { get; private set; }

        /// <summary>
        /// CSS @font-face rules read by <see cref="ReadAllRules" /> function.
        /// </summary>
        internal IList<CssFontFaceRule> FontFaceRules { get; private set; }

        /// <summary>
        /// CSS @list rules read by <see cref="ReadAllRules" /> function.
        /// </summary>
        internal IList<CssListRule> ListRules { get; private set; }

        /// <summary>
        /// This hashset is needed to know what style sheets are already started processing. Prevents recursion and duplication.
        /// </summary>
        /// <remarks>
        /// Keys are absolute URIs of stylesheets.
        /// </remarks>
        private readonly HashSetGeneric<string> mStyleSheetsBeingProcessed;

        /// <summary>
        /// Caches all loaded resources and provides access to them which is independent on whether a resource already loaded or not yet.
        /// </summary>
        private readonly HtmlResourceLoader mHtmlResourceLoader;

        private readonly CssDocumentMode mDocumentMode;
        private readonly CssNamespacePrefixResolver mNamespacePrefixResolver;

        /// <summary>
        /// Used to construct full path for external CSS documents that specify relative path.
        /// </summary>
        private string mBaseUri;

        /// <summary>
        /// Indicates whether we should stick to MS Word's behavior when reading CSS.
        /// </summary>
        private readonly bool mMimicMsWordBehavior;
    }
}
