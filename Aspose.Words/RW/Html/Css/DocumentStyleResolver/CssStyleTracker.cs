// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/12/2016 by Victor Chebotok

using System.Collections.Generic;
using Aspose.Bidi;
using Aspose.Collections;
using Aspose.Common;
using Aspose.Words.Drawing;
using Aspose.Words.Revisions;
using Aspose.Words.RW.Html.Parser;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Keeps track of CSS formatting of HTML elements when an HTML tree is being traversed.
    /// </summary>
    internal class CssStyleTracker
    {
        /// <summary>
        /// Initializes an instance of the class.
        /// </summary>
        /// <param name="rules">CSS rules imported from the HTML document.</param>
        /// <param name="pageRules">CSS @page rules imported from the HTML document.</param>
        /// <param name="htmlElementInlineCssStyle">Inline CSS style of the HTML element.</param>
        /// <param name="bodyElementInlineCssStyle">Inline CSS style of the BODY element.</param>
        /// <param name="cssResolver">Instance of <see cref="CssResolver"/>.</param>
        /// <param name="elementCategorizer">Instance of <see cref="HtmlElementCategorizer"/>.</param>
        /// <param name="attributeResolver">Instance of <see cref="HtmlAttributeResolver"/>.</param>
        /// <param name="defaultElementStyleResolver">Instance of <see cref="DefaultElementStyleResolver"/>.</param>
        /// <param name="computedDeclarationResolver">Instance of <see cref="CssComputedDeclarationResolver"/>.</param>
        /// <param name="boxModel">Css box model.</param>
        /// <param name="resolveFormattingAsMsWord">
        /// Indicates whether we should stick to MS Word's rules when resolving CSS formatting.
        /// </param>
        /// <param name="useHtmlBlocks">
        /// Indicates whether to mimic MS Word's behavior and use <see cref="HtmlBlock"/> nodes to preserve borders and margins
        /// of block-level elements like BODY, DIV, or BLOCKQUOTE.
        /// </param>
        internal CssStyleTracker(
            IList<CssStyleRule> rules,
            IList<CssPageRule> pageRules,
            string htmlElementInlineCssStyle,
            string bodyElementInlineCssStyle,
            CssResolver cssResolver,
            HtmlElementCategorizer elementCategorizer,
            HtmlAttributeResolver attributeResolver,
            DefaultElementStyleResolver defaultElementStyleResolver,
            CssComputedDeclarationResolver computedDeclarationResolver,
            CssBoxModel boxModel,
            bool resolveFormattingAsMsWord,
            bool useHtmlBlocks)
        {
            Debug.Assert(rules != null);
            Debug.Assert(pageRules != null);
            Debug.Assert(htmlElementInlineCssStyle != null);
            Debug.Assert(bodyElementInlineCssStyle != null);
            Debug.Assert(cssResolver != null);
            Debug.Assert(elementCategorizer != null);
            Debug.Assert(attributeResolver != null);
            Debug.Assert(defaultElementStyleResolver != null);
            Debug.Assert(computedDeclarationResolver != null);
            Debug.Assert(boxModel != null);

            mStyleRules = rules;
            mPageRules = pageRules;
            mHtmlElementInlineCssStyle = htmlElementInlineCssStyle;
            mBodyElementInlineCssStyle = bodyElementInlineCssStyle;
            mCssResolver = cssResolver;
            mElementCategorizer = elementCategorizer;
            mAttributeResolver = attributeResolver;
            mDefaultElementStyleResolver = defaultElementStyleResolver;
            mComputedDeclarationResolver = computedDeclarationResolver;
            BoxModel = boxModel;

            mElements = new List<HtmlElementInfo>();
            mTextDecorationsStack = new Stack<CssTextDecoration>();
            mInlineBorderStack = new Stack<CssBorder>();
            mBackgroundResolver = new BackgroundResolver();
            mActiveBidiLevels = new IntList();
            mSupSubScriptResolver = new SupSubScriptResolver();
            mCounters = new CssCounters();

            mResolveFormattingAsMsWord = resolveFormattingAsMsWord;
            mUseHtmlBlocks = useHtmlBlocks;

            // SPEED - In order to speed up getting of CSS declarations by CSS selectors, we cache CSS selector values and
            // their corresponding CSS rule declarations.
            FillSelectorToComputedDeclarationsMap();

            CalculatePredefinedStyleDeclarations();
        }

        internal CssDeclarationCollection GetPredefinedStyleDeclarations(StyleIdentifier styleIdentifier)
        {
            return mPredefinedStyleDeclarations[(int)styleIdentifier];
        }

        /// <summary>
        /// Adds an element to the stack and calculates document style for this element.
        /// </summary>
        /// <param name="element">
        /// An HTML element to add.
        /// </param>
        /// <param name="updateCounters">
        /// Controls whether CSS counters must be updated after the element is added. Counters must be updated exactly once
        /// per each HTML element in a document tree.
        /// </param>
        /// <remarks>
        /// In order to correctly calculate CSS counters, HTML elements of a document tree must be processed in a depth-first 
        /// manner without returns (each element must be processed exactly once). However, the style resolver allows for
        /// reprocessing of HTML elements and we actively use this possibility, for example, to figure out structure of a table
        /// before processing its elements or to modify HTML tree and insert anonymous nodes into it. If we push an HTML
        /// element to the resolver more than once, we must use the <paramref name="updateCounters"/> flag to prevent repeated
        /// update of counters.
        /// </remarks>
        internal void PushElement(IHtmlElementProvider element, bool updateCounters)
        {
            // Counters are never updated for implicit (anonymous) elements, because there is no way to apply custom CSS styles
            // to such elements.
            if (element.IsImplicit)
            {
                Debug.Assert(!updateCounters);
            }

            mCssResolver.PushElement(element);
            CssDeclarationCollection declarations = mCssResolver.GetDeclarations();

            if (element.ElementName == "img")
            {
                // WORDSNET-10651 Correct CSS style of an <img> inside an absolutely positioned <span>
                // to make the <img> absolutely positioned too.
                declarations = CorrectStyleOfImagesInsidePositionedSpans(element, declarations);

                // WORDSNET-21954 Apply floating to the image if it is the only child of one or more floating parent
                // containers.
                declarations = CorrectStyleOfImagesInsideFloatingParents(element, declarations);
            }

            RealElementInfo elementInfo = new RealElementInfo(element, declarations,
                mCssResolver.GetDeclarations(HtmlElementPart.Before),
                mCssResolver.GetDeclarations(HtmlElementPart.After));
            PushElementInternal(elementInfo);

            // Counters must be updated after the element is pushed.
            if (updateCounters)
            {
                UpdateCounters(declarations);
            }

            // This method pushes real HTML elements only, not pseudo-elements. It mustn't accidentally switch the resolver
            // to a pseudo-element.
            Debug.Assert(CurrentPart == HtmlElementPart.Element);
        }

        /// <summary>
        /// Removes the top element from the stack.
        /// </summary>
        internal void PopElement()
        {
            // If we forgot to switch the resolver back to the real HTML element before popping it, let's do it now.
            Debug.Assert(CurrentPart == HtmlElementPart.Element);
            SwitchToPart(HtmlElementPart.Element, false);

            PopElementInternal();
        }

        internal void PushImplicitDiv()
        {
            if (mIsOnImplicitElement)
            {
                Debug.Fail("Unexpected state.");
                return;
            }

            // HtmlReader can create new paragraphs during processing of non-paragraph elements like <div>, <td> or <span>. 
            // We cannot apply CSS of such elements directly to model paragraphs, and we have to insert an implicit <p>
            // element in order to get correct CSS for paragraphs.
            if ((!CurrentElement.IsImplicit) && (!IsMappedToParagraphElement()))
            {
                mReturnToElement = null;
                mReturnToPart = HtmlElementPart.Element;

                if (IsMappedToRunElement())
                {
                    // We need to go back to the parent HTML element.
                    // If we are processing a real HTML element then we go back by popping that element as usual.
                    if (CurrentPart == HtmlElementPart.Element)
                    {
                        mReturnToElement = CurrentElement;
                        PopElement();
                    }
                    // If we are processing a pseudo-element, we go back by switching back to the main part
                    // of the current element.
                    else
                    {
                        mReturnToPart = CurrentPart;
                        SwitchToPart(HtmlElementPart.Element, false);
                    }
                }
                IHtmlElementProvider pElement = new HtmlElementNode("p", true);
                // Counters are never updated for implicit elements.
                PushElement(pElement, false);
                mIsOnImplicitElement = true;
            }
        }

        internal void PushImplicitSpan()
        {
            if (mIsOnImplicitElement)
            {
                Debug.Fail("Unexpected state.");
                return;
            }

            // WORDSNET-8856 We insert implicit (anonymous) span elements for text inside HTML blocks.
            // Code like <p style="...">Text</p> is processed as <p style="..."><span>Text</span></p>
            if ((!CurrentElement.IsImplicit) && (!IsMappedToRunElement()))
            {
                IHtmlElementProvider spanElement = new HtmlElementNode("span", true);
                // Counters are never updated for implicit elements.
                PushElement(spanElement, false);
                mIsOnImplicitElement = true;
                mReturnToElement = null;
                mReturnToPart = HtmlElementPart.Element;
            }
        }

        internal void PopImplicitElement()
        {
            if (!mIsOnImplicitElement)
            {
                return;
            }

            // Remove the implicit element we might have inserted and restore the original HTML tree structure.
            Debug.Assert(CurrentElement.IsImplicit);
            PopElement();

            // If we were processing a real HTML element then we restore the tree by pushing that element as usual.
            if (mReturnToElement != null)
            {
                // This element has been processed already and we should block repeated update of counters.
                // Otherwise, counter values will be corrupted.
                PushElement(mReturnToElement, false);
            }
            // If we were processing a pseudo-element, we restore the tree by switching to the part we were at.
            else if (mReturnToPart != HtmlElementPart.Element)
            {
                // This element has been processed already and we should block repeated update of counters.
                // Otherwise, counter values will be corrupted.
                SwitchToPart(mReturnToPart, false);
            }

            mIsOnImplicitElement = false;
            mReturnToElement = null;
            mReturnToPart = HtmlElementPart.Element;
        }

        /// <summary>
        /// Gets CSS block flow direction.
        /// </summary>
        internal CssBlockFlowDirection GetBlockFlowDirection()
        {
            string writingMode = ElementDeclarations.GetIdentifier("writing-mode");
            return (writingMode != string.Empty)
                       ? CssUtil.CssWritingModeToBlockFlowDirection(writingMode)
                       : CssBlockFlowDirection.HorizontalTb;
        }

        /// <summary>
        /// Indicates whether the block-level element currently being processed has RTL direction.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the block-level element currently being processed has RTL direction. 
        /// <c>false</c> if the element has LTR direction.
        /// </returns>
        internal bool IsBlockRtl()
        {
            return CurrentElementInfo.BlockLevelDirection == CssDirection.Rtl;
        }

        /// <summary>
        /// Returns a copy of currently active bidi levels.
        /// </summary>
        internal BidiLevelList GetActiveBidiLevels()
        {
            return new BidiLevelList(mActiveBidiLevels.ToArray());
        }

        /// <summary>
        /// Indicates whether text of HTML element currently being processed is preformatted.
        /// </summary>
        /// <returns>
        /// <c>true</c> if HTML element is preformatted.
        /// <c>false</c> if HTML element is not preformatted.
        /// </returns>
        internal bool IsPreformatted()
        {
            CssDeclaration whiteSpaceDeclaration = ElementDeclarations["white-space"];
            return (whiteSpaceDeclaration != null) && whiteSpaceDeclaration.Value.Equals(CssValue.Pre);
        }

        /// <summary>
        /// Indicates whether text of HTML element currently being processed is preformatted with line.
        /// </summary>
        /// <returns>
        /// <c>true</c> if HTML element is preformatted with line.
        /// <c>false</c> if HTML element is not preformatted with line.
        /// </returns>
        internal bool IsPreformattedWithLine()
        {
            CssDeclaration whiteSpaceDeclaration = ElementDeclarations["white-space"];
            return (whiteSpaceDeclaration != null) && whiteSpaceDeclaration.Value.Equals(CssValue.PreLine);
        }

        /// <summary>
        /// Indicates whether text of HTML element currently being processed is preformatted with wrap.
        /// </summary>
        /// <returns>
        /// <c>true</c> if HTML element is preformatted with wrap.
        /// <c>false</c> if HTML element is not preformatted with wrap.
        /// </returns>
        internal bool IsPreformattedWithWrap()
        {
            CssDeclaration whiteSpaceDeclaration = ElementDeclarations["white-space"];
            return (whiteSpaceDeclaration != null) && whiteSpaceDeclaration.Value.Equals(CssValue.PreWrap);
        }

        /// <summary>
        /// Determines whether the current element is a block-level element.
        /// </summary>
        internal bool IsBlockLevelElement()
        {
            return CurrentElementInfo.IsBlockLevelElement;
        }

        /// <summary>
        /// Determines whether the parent of current element is a block-level element.
        /// </summary>
        internal bool ParentElementIsBlockLevel()
        {
            HtmlElementInfo parentElementInfo = GetElementInfo(1);
            return (parentElementInfo != null) && parentElementInfo.IsBlockLevelElement;
        }

        /// <summary>
        /// Looks up the HTML tree and gets the nearest inline parent that maps to a built-in character style.
        /// </summary>
        internal string FindNearestInlineOverridableParentElement()
        {
            for (int depth = 0; depth < mElements.Count; ++depth)
            {
                HtmlElementInfo elementInfo = GetElementInfo(depth);

                // Stop if we meet a block-level parent.
                if ((elementInfo == null) || elementInfo.IsBlockLevelElement)
                {
                    return null;
                }

                // Recognized elements are:
                //  <a> with non-empty "href" attribute (maps to the "Hyperlink" style);
                //  <strong> (maps to the "Strong" style);
                //  <em> (maps to the "Emphasis" style).
                IHtmlElementProvider element = elementInfo.Element;
                string name = element.ElementName;
                if ((name == "a") && StringUtil.HasChars(element.GetAttributeValue("href")))
                {
                    return name;
                }
                else if ((name == "strong") || (name == "em"))
                {
                    return name;
                }

                // Elements with "class" attributes will have custom character styles applied.
                // That's why we stop searching if we meet such an element.
                string[] classes = element.GetClasses();
                if (classes.Length > 0)
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets parent HTML element "display" property value.
        /// </summary>
        internal CssDisplayType ParentElementDisplayType()
        {
            HtmlElementInfo parentElementInfo = GetElementInfo(1);
            return (parentElementInfo != null)
                ? parentElementInfo.DisplayType
                : CssDisplayType.None;
        }

        /// <summary>
        /// Switches the resolver to the specified part (pseudo-element) of the current real HTML element.
        /// </summary>
        /// <param name="part">
        /// Part of HTML element on which the switch will be done.
        /// </param>
        /// <param name="updateCounters">
        /// Controls whether CSS counters must be updated after switch to part. Counters must be updated only once.
        /// </param>
        /// <returns>
        /// A value indicating whether the resolver has been switched. In certain cases switching is impossible (for example,
        /// if the pseudo-element has no content and is thus invisible).
        /// </returns>
        /// <remarks>
        /// Technically, pseudo-elements are processed as normal child nodes of the current HTML element. However, their
        /// CSS declarations are resolved at the parent element's level, because pseudo-element selectors match the main HTML
        /// element. That is why we have to pre-resolve declarations of pseudo-elements and then use switching to process them.
        /// It is important that the resolver is switched in the correct order (i.e., ::before first and ::after last) and each
        /// part is processed no more than once, because pseudo-elements affect counters and violation of switch order will
        /// ruin numbering in the document.
        /// </remarks>
        internal bool SwitchToPart(HtmlElementPart part, bool updateCounters)
        {
            // Pseudo-elements other than ::before and ::after are not supported yet and their influence on CSS counters
            // hasn't been investigated. That is the reason why they are not allowed in this method.
            Debug.Assert((part == HtmlElementPart.Element) ||
                (part == HtmlElementPart.Before) ||
                (part == HtmlElementPart.After));

            // Switch back from a pseudo-element to the main element if needed.
            if (CurrentPart != HtmlElementPart.Element)
            {
                PopElementInternal();
            }

            // Remember the real HTML element that the requested pseudo-element belongs to.
            HtmlElementInfo realElementInfo = CurrentElementInfo;

            // Switch from the main element to the requested pseudo-element.
            if (part != HtmlElementPart.Element)
            {
                CssDeclarationCollection pseudoElementDeclarations = CurrentElementInfo.GetDeclarations(part);

                CssDeclaration contentDeclaration = pseudoElementDeclarations["content"];
                PseudoElementContent content = (contentDeclaration != null)
                    ? PseudoElementContent.Parse(contentDeclaration.Value)
                    : null;

                if (content != null)
                {
                    // Create a pseudo-element node.
                    IHtmlElementProvider pseudoElementNode = new HtmlElementNode("span", true);
                    // Push the pseudo-element. The pseudo-element cannot have nested pseudo-elements.
                    PseudoElementInfo elementInfo = new PseudoElementInfo(pseudoElementNode, pseudoElementDeclarations,
                        part, content);
                    PushElementInternal(elementInfo);

                    if (ElementDisplayState == HtmlElementDisplayState.None)
                    {
                        // The pseudo-element is invisible and we cannot switch the resolver to it.
                        PopElementInternal();
                    }
                    else if (updateCounters)
                    {
                        // The resolver has been switched to the requested pseudo-element. The pseudo-element is visible
                        // and it may affect counters.
                        UpdateCounters(pseudoElementDeclarations);

                        // The pseudo-element is considered a child of the corresponding real element.
                        realElementInfo.HasChildren = true;
                    }
                }
            }

            return CurrentPart == part;
        }

        /// <summary>
        /// Gets declarations for a selector.
        /// </summary>
        /// <param name="selector">CSS selector.</param>
        /// <returns>The selector declarations; null, if selector not found.</returns>
        internal CssDeclarationCollection GetSelectorDeclarations(CssSelector selector)
        {
            // SPEED - In order to speed up getting of CSS declarations by CSS selectors, we cache CSS selector values and
            // their corresponding CSS rule declarations.
            string selectorText = selector.ToCss();

            if (mDeclarationsBySelector.ContainsKey(selectorText))
                return mDeclarationsBySelector[selectorText];
            else
                return null;
        }

        /// <summary>
        /// Gets textual representation of the current part's generated content.
        /// Only pseudo-elements can have generated content.
        /// </summary>
        /// <returns>
        /// A text with generated content of the current part or an empty string if the current part has no generated content.
        /// </returns>
        internal string GetGeneratedContent()
        {
            PseudoElementContent content = CurrentElementInfo.GeneratedContent;
            if (content != null)
            {
                // The current element is a pseudo-element but we need to see attributes of the main element
                // (the parent of the pseudo-element) in order to generate pseudo-element's text.
                IHtmlElementProvider mainElement = GetElementInfo(1).Element;
                return GeneratedContentTextBuilder.GetText(content, mCounters, mainElement);
            }
            return string.Empty;
        }

        internal int GetListLevelCount()
        {
            PseudoElementContent content = CurrentElementInfo.GeneratedContent;
            if (content != null)
            {
                // The current element is a pseudo-element but we need to see attributes of the main element
                // (the parent of the pseudo-element) in order to generate pseudo-element's text.
                IHtmlElementProvider mainElement = GetElementInfo(1).Element;
                return GeneratedContentListLevelCounter.GetListLevelCount(content, mCounters, mainElement);
            }

            return 0;
        }

        internal GeneratedContentListLabelInfo GetListLabelInfo(int skipListLevelsCount, int currentListLevelNumber)
        {
            PseudoElementContent content = CurrentElementInfo.GeneratedContent;
            if (content != null)
            {
                // The current element is a pseudo-element but we need to see attributes of the main element
                // (the parent of the pseudo-element) in order to generate pseudo-element's text.
                IHtmlElementProvider mainElement = GetElementInfo(1).Element;
                return GeneratedContentListLabelInfoCreator.GetListLabelInfo(
                    content,
                    mCounters,
                    mainElement,
                    skipListLevelsCount,
                    currentListLevelNumber);
            }

            return null;
        }

        /// <summary>
        /// Gets the current value of a counter. Corresponds to the 'counter' CSS function.
        /// </summary>
        internal int GetCounterValue(string counterName)
        {
            return mCounters.GetValue(counterName);
        }

        internal SizeD GetEffectiveParentElementSize()
        {
            for (int i = mElements.Count - 2; i >= 0; i--)
            {
                HtmlElementInfo elementInfo = mElements[i];
                if (elementInfo.EffectiveSize != null)
                    return elementInfo.EffectiveSize;
            }

            return null;
        }

        internal LineBreakClear GetLineBreakClear()
        {
            string clearPropertyValue = ElementDeclarations.GetIdentifier("clear").ToLowerInvariant();
            switch (clearPropertyValue)
            {
                case "left":
                    return LineBreakClear.Left;
                case "right":
                    return LineBreakClear.Right;
                case "both":
                    return LineBreakClear.All;
                default:
                    // Ignore unknown values.
                    return LineBreakClear.None;
            }
        }

        /// <summary>
        /// Gets CSS rules declared in the HTML document's stylesheet. />.
        /// </summary>
        internal ICollection<CssStyleRule> DocumentStyleRules
        {
            get { return mStyleRules; }
        }

        /// <summary>
        /// Gets CSS selectors that match for the current element.
        /// </summary>
        internal IList<CssSelector> CurrentElementSelectors
        {
            get { return mCssResolver.GetSelectors(); }
        }

        internal bool IsEmpty
        {
            get { return mElements.Count == 0; }
        }

        /// <summary>
        /// Gets current HTML element CSS declarations. All returned declarations are guaranteed to be computed.
        /// </summary>
        internal CssDeclarationCollection ElementDeclarations
        {
            get { return CurrentElementInfo.GetDeclarations(HtmlElementPart.Element); }
        }

        /// <summary>
        /// Gets CSS declarations applied to the additional content inserted before the current HTML element
        /// (the ::before pseudo-element). All returned declarations are guaranteed to be computed.
        /// </summary>
        internal CssDeclarationCollection BeforePseudoElementDeclarations
        {
            get { return CurrentElementInfo.GetDeclarations(HtmlElementPart.Before); }
        }

        /// <summary>
        /// Gets an insertion revision for the current element. 
        /// Can be null, if the element doesn't have an insertion revision.
        /// </summary>
        internal EditRevision ElementInsertionRevision
        {
            get { return mAttributeResolver.InsertionRevision; }
        }

        /// <summary>
        /// Gets a deletion revision for the current element. 
        /// Can be null, if the element doesn't have a deletion revision.
        /// </summary>
        internal EditRevision ElementDeletionRevision
        {
            get { return mAttributeResolver.DeletionRevision; }
        }

        /// <summary>
        /// Gets current HTML element display state.
        /// </summary>
        internal HtmlElementDisplayState ElementDisplayState
        {
            get { return CurrentElementInfo.DisplayState; }
        }

        internal CssBoxModel BoxModel { get; private set; }

        /// <summary>
        /// Gets current HTML element.
        /// </summary>
        internal IHtmlElementProvider CurrentElement
        {
            get { return CurrentElementInfo.Element; }
        }

        /// <summary>
        /// Gets a value indicating whether the current HTML element has children (either real or pseudo-elements).
        /// </summary>
        /// <remarks>
        /// The value is valid only after the current HTML element is fully processed, including its ::before and ::after
        /// pseudo-elements.
        /// </remarks>
        internal bool CurrentElementHasChildren
        {
            get { return CurrentElementInfo.HasChildren; }
        }

        /// <summary>
        /// Gets current HTML element info.
        /// </summary>
        internal HtmlElementInfo CurrentElementInfo
        {
            get
            {
                HtmlElementInfo result = GetElementInfo(0);
                Debug.Assert(result != null);
                return result;
            }
        }

        /// <summary>
        /// Gets current HTML element "display" property value.
        /// </summary>
        internal CssDisplayType ElementDisplayType
        {
            get { return CurrentElementInfo.DisplayType; }
        }

        internal CssTextDecoration TextDecoration
        {
            get { return mTextDecorationsStack.Peek(); }
        }

        /// <summary>
        /// Inline-level border of the current element.
        /// </summary>
        /// <remarks>
        /// Only box borders (having all four sides: top, right, bottom, and left) are supported on inline level.
        /// Inline-level borders are propagated from parents to children.
        /// </remarks>
        internal CssBorder InlineBorder
        {
            get { return mInlineBorderStack.Peek(); }
        }

        internal ElementBackgroundInfo Background
        {
            get { return mBackgroundResolver.CurrentElementBackground; }
        }

        internal CssVerticalAlign SubSupScript
        {
            get { return mSupSubScriptResolver.CurrentElementInfo; }
        }

        internal int LocaleId
        {
            get { return mAttributeResolver.LocaleId; }
        }

        internal int LocaleIdBi
        {
            get { return mAttributeResolver.LocaleIdBi; }
        }

        internal int LocaleIdFarEast
        {
            get { return mAttributeResolver.LocaleIdFarEast; }
        }

        private void CalculatePredefinedStyleDeclarations()
        {
            // Check if formatting of any built-in styles is specified using rules with "-aw-style-name" declarations.
            foreach (CssStyleRule rule in mStyleRules)
            {
                CssDeclaration styleNameDeclaration = rule.Declarations[HtmlConstants.StyleName];
                if (styleNameDeclaration != null)
                {
                    StyleIdentifier styleIdentifier = CssDocumentStyleNames.CssStyleNameToStyleIdentifier(styleNameDeclaration.Value);
                    if (styleIdentifier != StyleIdentifier.Nil)
                    {
                        mPredefinedStyleDeclarations[(int)styleIdentifier] =
                            mComputedDeclarationResolver.ResolveToComputed(rule.Declarations);
                    }
                }
            }

            // h1..h6 document styles.
            Debug.Assert((int)StyleIdentifier.Heading1 == 1);
            Debug.Assert((int)StyleIdentifier.Heading6 == 6);
            for (StyleIdentifier sti = StyleIdentifier.Heading1; sti <= StyleIdentifier.Heading6; sti++)
            {
                // Skip built-in styles whose formatting is specified via rules with "-aw-style-name" declarations.
                if (mPredefinedStyleDeclarations.ContainsKey((int)sti))
                    continue;

                CssDeclarationCollectionBuilder styleDeclarationsBuilder = new CssDeclarationCollectionBuilder();

                CssDeclarationCollection defaultDeclarations = GetDefaultDeclarations(string.Format("h{0}", (int)sti));
                styleDeclarationsBuilder.AddOrReplace(defaultDeclarations);

                string headingElementName = "h" + FormatterPal.IntToStr((int)sti);
                CssDeclarationCollection selectorDeclarations = GetSelectorDeclarations(CssSelector.Element(headingElementName));
                if (selectorDeclarations != null)
                {
                    styleDeclarationsBuilder.AddOrReplace(selectorDeclarations);
                }

                mPredefinedStyleDeclarations[(int)sti] = styleDeclarationsBuilder.GetDeclarations();
            }

            // If formatting of the "Normal" style is not specified via a rule with a "-aw-style-name" declaration.
            if (!mPredefinedStyleDeclarations.ContainsKey((int)StyleIdentifier.Normal))
            {
                // "html" and "body" should affect the "Normal" style even when the HTML document uses inline CSS. In order to make
                // this happen we fill the "style" attribute values of the mock elements with inline CSS taken from real elements.

                MockHtmlElementProvider elementHtml = MockHtmlElementProvider.Create("html");
                elementHtml.AddAttribute("style", mHtmlElementInlineCssStyle);
                mCssResolver.PushElement(elementHtml);

                MockHtmlElementProvider elementBody = MockHtmlElementProvider.Create("body", elementHtml);
                elementBody.AddAttribute("style", mBodyElementInlineCssStyle);
                mCssResolver.PushElement(elementBody);

                CssDeclarationCollection bodyDeclarations = mCssResolver.GetDeclarations();

                CssDeclarationCollectionBuilder normalDeclarationsBuilder = new CssDeclarationCollectionBuilder(bodyDeclarations);
                normalDeclarationsBuilder.Remove("background-color");

                mPredefinedStyleDeclarations[(int)StyleIdentifier.Normal] = normalDeclarationsBuilder.GetDeclarations();

                mCssResolver.PopElement();
                mCssResolver.PopElement();
            }
        }

        private void PushElementInternal(HtmlElementInfo elementInfo)
        {
            IHtmlElementProvider element = elementInfo.Element;

            // It is a logical error to have an implicit element inside another implicit element.
            if (mElements.Count > 0)
            {
                Debug.Assert((!CurrentElement.IsImplicit) || (!element.IsImplicit));
            }

            // Determine whether the the element contains other elements or text nodes.
            elementInfo.HasChildren = (element.GetFirstChildElement() != null) || (element.GetInnerText().Length > 0);

            // Common info computing.
            elementInfo.IsBlockLevelElement = mElementCategorizer.IsBlockLevelElement(element, elementInfo.Declarations) ||
                (element.ElementName == "body");
            elementInfo.DisplayType = mElementCategorizer.GetDisplayType(elementInfo.Declarations);

            elementInfo.ParentBlockElement = GetLastBlockElementInfo();
            // Direction computing.
            if (elementInfo.IsBlockLevelElement)
                elementInfo.BlockLevelDirection = CssUtil.GetDirection(elementInfo.Declarations);
            else if (elementInfo.ParentBlockElement != null)
                elementInfo.BlockLevelDirection = CssUtil.GetDirection(elementInfo.ParentBlockElement.Declarations);

            elementInfo.InlineLevelDirection = CssUtil.GetDirection(elementInfo.Declarations);

            bool createsHtmlBlock = mUseHtmlBlocks &&
                CssUtil.CreatesHtmlBlock(elementInfo.Element.ElementName, elementInfo.Declarations, mResolveFormattingAsMsWord);

            // Indents and spacings computing. 
            if (!element.IsImplicit)
            {
                CssDisplayType parentDisplayType = IsEmpty
                    ? CssDisplayType.None
                    : CurrentElementInfo.DisplayType;
                BoxModel.Push(elementInfo.Element, elementInfo.Declarations, elementInfo.DisplayType, parentDisplayType, createsHtmlBlock);
            }

            // Display state computing.
            HtmlElementDisplayState displayState = (mElements.Count != 0)
                ? ElementDisplayState
                : HtmlElementDisplayState.Visible;

            if ((displayState != HtmlElementDisplayState.None) && (elementInfo.DisplayType == CssDisplayType.None))
            {
                displayState = HtmlElementDisplayState.None;
            }
            if (displayState != HtmlElementDisplayState.None)
            {
                CssDeclaration visibilityDeclaration = elementInfo.Declarations["visibility"];
                if (visibilityDeclaration != null)
                {
                    // The support for visibility:collapse is missing or partially incorrect in some modern browsers. We ignore it too.
                    if (visibilityDeclaration.Value.Equals(CssValue.Hidden) ||
                        visibilityDeclaration.Value.Equals(CssValue.Collapse))
                    {
                        displayState = HtmlElementDisplayState.Hidden;
                    }
                    else if (visibilityDeclaration.Value.Equals(CssValue.Visible))
                    {
                        displayState = HtmlElementDisplayState.Visible;
                    }
                }
            }
            elementInfo.DisplayState = displayState;

            ReadElementSize(elementInfo);

            mAttributeResolver.PushElement(element, elementInfo.Declarations);

            mElements.Add(elementInfo);

            RecalculateActiveBidiLevels(CurrentElementInfo, true);

            // Text decoration computing.
            ComputeCurrentTextDecoration();

            ComputeInlineBorder(createsHtmlBlock);

            mBackgroundResolver.PushBackground(CurrentElementInfo);

            mSupSubScriptResolver.Push(elementInfo);

            // Create a new counter nesting level for the element we've just pushed.
            mCounters.IncreaseNesting();
        }

        private static void ReadElementSize(HtmlElementInfo elementInfo)
        {
            double width = elementInfo.Declarations.GetLength("width");
            double minWidth = elementInfo.Declarations.GetLength("min-width");
            double height = elementInfo.Declarations.GetLength("height");
            double minHeight = elementInfo.Declarations.GetLength("min-height");

            double effectiveWidth = System.Math.Max(minWidth, width);
            double effectiveHeight = System.Math.Max(minHeight, height);

            if (!MathUtil.IsMinValue(effectiveWidth) && !MathUtil.IsMinValue(effectiveHeight))
                elementInfo.EffectiveSize = new SizeD(effectiveWidth, effectiveHeight);
        }

        private void PopElementInternal()
        {
            Debug.Assert(mElements.Count != 0);
            if (!CurrentElement.IsImplicit)
                BoxModel.Pop();

            RecalculateActiveBidiLevels(CurrentElementInfo, false);

            // Pseudo-elements are not pushed to the CSS rosolver because their declarations are resolved together with
            // declarations of main elements. That is why pseudo-elements are skipped from this step.
            if (CurrentPart == HtmlElementPart.Element)
            {
                mCssResolver.PopElement();
            }

            mElements.RemoveAt(mElements.Count - 1);
            mAttributeResolver.PopElement();
            mTextDecorationsStack.Pop();
            mInlineBorderStack.Pop();
            mBackgroundResolver.PopBackground();
            mSupSubScriptResolver.Pop();
            mCounters.DecreaseNesting();
        }

        /// <summary>
        /// Corrects CSS style of IMG elements that are inside absolutely positioned SPANs to make images absolutely positioned.
        /// </summary>
        /// <remarks>
        /// MS Word exports a floating image as an absolutely positioned span with an inline image inside. Aspose.Words does
        /// not support absolutely positioned spans but it do support absolutely positioned images. This method copies certain
        /// CSS properties from the span to the image to make the image absolutely positioned and suitable for HTML import.
        /// </remarks>
        private CssDeclarationCollection CorrectStyleOfImagesInsidePositionedSpans(
            IHtmlElementProvider element,
            CssDeclarationCollection declarations)
        {
            // The recognized constructions are:
            //
            //  <span style="position:absolute;..."><img src="..."></span>
            //
            // and
            //
            //  <span style="position:absolute;..."><a href="..."><img src="..."></a></span>
            //
            // Whitespace (but not meaningful text or other elements) is allowed between any of the elements.

            Debug.Assert(element.ElementName == "img");

            CssDeclarationCollection result = declarations;
            HtmlElementInfo parentInfo = GetElementInfo(0);

            // An optional anchor element.
            if ((parentInfo != null) &&
                (parentInfo.Element.ElementName == "a") &&
                parentInfo.Element.HasOnlyOneChildElementWithOptionalWhitespace())
            {
                parentInfo = GetElementInfo(1);
            }

            // An absolutely positioned span element.
            if ((parentInfo != null) &&
                (parentInfo.Element.ElementName == "span") &&
                parentInfo.Element.HasOnlyOneChildElementWithOptionalWhitespace())
            {
                CssDeclaration positionDeclaration = parentInfo.Declarations["position"];
                if ((positionDeclaration != null) && positionDeclaration.Value.Equals(CssValue.Absolute))
                {
                    CssDeclarationCollectionBuilder correctedDeclarations = new CssDeclarationCollectionBuilder(declarations);
                    correctedDeclarations.AddIfNotNullAndNotExist(positionDeclaration);
                    correctedDeclarations.AddIfNotNullAndNotExist(parentInfo.Declarations["z-index"]);
                    correctedDeclarations.AddIfNotNullAndNotExist(parentInfo.Declarations["margin-left"]);
                    correctedDeclarations.AddIfNotNullAndNotExist(parentInfo.Declarations["margin-right"]);
                    correctedDeclarations.AddIfNotNullAndNotExist(parentInfo.Declarations["margin-top"]);
                    correctedDeclarations.AddIfNotNullAndNotExist(parentInfo.Declarations["left"]);
                    result = correctedDeclarations.GetDeclarations();
                }
            }
            return result;
        }

        /// <summary>
        /// Determines whether the current element is directly mapped to a model run. 
        /// </summary>
        private bool IsMappedToRunElement()
        {
            return IsMappedToParaOrRunElement(CurrentElement) && !IsBlockLevelElement();
        }

        /// <summary>
        /// Determines whether the curent element is directly mapped to a model paragraph.
        /// </summary>
        private bool IsMappedToParagraphElement()
        {
            return IsMappedToParaOrRunElement(CurrentElement) && IsBlockLevelElement();
        }

        /// <summary>
        /// Determines whether the element is mapped to a model paragraph or run. 
        /// </summary>
        private static bool IsMappedToParaOrRunElement(IHtmlElementProvider element)
        {
            switch (element.ElementName)
            {
                case "div":
                case "li":
                case "p":
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                case "center":
                case "pre":
                case "blockquote":
                case "a":
                case "b":
                case "big":
                case "em":
                case "i":
                case "s":
                case "small":
                case "strike":
                case "strong":
                case "sub":
                case "sup":
                case "tt":
                case "u":
                case "span":
                case "font":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets last block-level element info stored in the element collection.
        /// </summary>
        /// <returns>Last block-level element info; null if there is no elements in the collections.</returns>
        private HtmlElementInfo GetLastBlockElementInfo()
        {
            HtmlElementInfo blockElementInfo = null;
            if (mElements.Count > 0)
            {
                HtmlElementInfo lastElementInfo = mElements[mElements.Count - 1];
                blockElementInfo = lastElementInfo.IsBlockLevelElement
                    ? lastElementInfo
                    : lastElementInfo.ParentBlockElement;
            }

            return blockElementInfo;
        }

        /// <summary>
        /// Computes text decoration for the current element.
        /// </summary>
        /// <remarks>
        /// Implemented rules:
        /// 'text-decoration' property is not inherited but is propagated from ancestor to descendant elements.
        /// 'text-decoration' property on descendant elements cannot have any effect on the decoration of the ancestor.
        /// 'text-decoration' property is not propagated to floating elements.
        /// Other rules need to be investigated or are browser-specific and are not implemented yet.
        /// http://www.w3.org/TR/CSS21/text.html#propdef-text-decoration
        /// </remarks>
        private void ComputeCurrentTextDecoration()
        {
            CssTextDecoration textDecoration = ((mTextDecorationsStack.Count > 0) && (CurrentElementInfo.Declarations["float"] == null))
                ? mTextDecorationsStack.Peek()
                : new CssTextDecoration();

            // In MS Word, 'text-decoration: none' clears all propagated decorations.
            textDecoration = textDecoration.Apply(CurrentElementInfo.Declarations, mResolveFormattingAsMsWord);

            mTextDecorationsStack.Push(textDecoration);
        }

        /// <summary>
        /// Computes and pushes the inline-level border of the current element.
        /// </summary>
        private void ComputeInlineBorder(bool createsHtmlBlock)
        {
            // WORDSNET-21063 Discard the inline border for an element for which the HTML block was created.
            if (CurrentElementInfo.IsBlockLevelElement || createsHtmlBlock)
            {
                mInlineBorderStack.Push(CssBorder.Empty);
            }
            else
            {
                CssBorder inlineBorder = CssBorder.CreateBoxBorder(CurrentElementInfo.Declarations);
                if (inlineBorder.IsUndefined && (mInlineBorderStack.Count > 0))
                {
                    // In case no supported border is specified on this element, propagate the inline border from its parent.
                    inlineBorder = mInlineBorderStack.Peek();
                }
                mInlineBorderStack.Push(inlineBorder);
            }
        }

        /// <summary>
        /// Gets HTML info of an element at the specified zero-based depth (0 - current element; 1 - its parent, and so on).
        /// </summary>
        /// <returns>
        /// HTML info of an element at the specified depth or <c>null</c>, if no element is found (depth is out of range).
        /// </returns>
        private HtmlElementInfo GetElementInfo(int depth)
        {
            int index = mElements.Count - depth - 1;
            return ((index >= 0) && (index < mElements.Count))
                ? mElements[index]
                : null;
        }

        /// <summary>
        /// Recalculates opened bidi levels when an HTML element is added or removed.
        /// </summary>
        private void RecalculateActiveBidiLevels(HtmlElementInfo elementInfo, bool isAdded)
        {
            CssUnicodeBidi unicodeBidi = CssUtil.GetUnicodeBidi(elementInfo.Declarations, elementInfo.IsBlockLevelElement);
            if (unicodeBidi == CssUnicodeBidi.Embed)
            {
                if (isAdded)
                {
                    if (elementInfo.InlineLevelDirection == CssDirection.Rtl)
                    {
                        mActiveBidiLevels.Add((int)BidiLevel.EmbedRtl);
                    }
                    else
                    {
                        mActiveBidiLevels.Add((int)BidiLevel.EmbedLtr);
                    }
                }
                else
                {
                    Debug.Assert(mActiveBidiLevels.Count > 0);
                    mActiveBidiLevels.RemoveAt(mActiveBidiLevels.Count - 1);
                }
            }
        }

        /// <summary>
        /// Modifies CSS counters according to the specified CSS declarations.
        /// </summary>
        private void UpdateCounters(CssDeclarationCollection elementDeclarations)
        {
            CssCounterModifications modifications = CssCounterModifications.Parse(elementDeclarations);
            modifications.ApplyTo(mCounters);
        }

        private HtmlElementPart CurrentPart
        {
            get
            {
                HtmlElementInfo info = GetElementInfo(0);
                return (info != null)
                    ? info.Part
                    : HtmlElementPart.Element;
            }
        }

        /// <summary>
        /// Returns CSS declarations of the all suitable page rules.
        /// </summary>
        internal CssDeclarationCollection GetAllPageDeclarations()
        {
            if (IsEmpty)
            {
                return CssDeclarationCollection.Empty;
            }

            string pageName = ElementDeclarations.GetIdentifier("page");

            List<CssPageRule> suitableRules = new List<CssPageRule>();
            foreach (CssPageRule rule in mPageRules)
            {
                if (rule.Selector.Matches(pageName))
                {
                    suitableRules.Add(rule);
                }
            }
            if (suitableRules.Count == 0)
                return CssDeclarationCollection.Empty;

            if (!StringUtil.HasChars(pageName) || (suitableRules.Count == 1))
                return mComputedDeclarationResolver.ResolveToComputed(suitableRules[0].Declarations);

            // Here we have several suitable rules. Some rules are common (applied to all pages) and others are specific and are
            // applied only to pages that have the specified name.
            // We should combine the CSS declarations from common and specific rules.
            CssDeclarationCollectionBuilder commonDeclarations = new CssDeclarationCollectionBuilder();
            CssDeclarationCollectionBuilder otherDeclarations = new CssDeclarationCollectionBuilder();
            foreach (CssPageRule rule in suitableRules)
            {
                if (rule.Selector.HasPageNames())
                {
                    otherDeclarations.AddOrReplace(rule.Declarations);
                }
                else
                {
                    commonDeclarations.AddOrReplace(rule.Declarations);
                }
            }
            commonDeclarations.AddOrReplace(otherDeclarations.GetDeclarations());

            return mComputedDeclarationResolver.ResolveToComputed(commonDeclarations.GetDeclarations());
        }

        internal CssDeclaration ParentTextAlign()
        {
            return (mElements.Count > 1)
                ? mElements[mElements.Count - 2].Declarations["text-align"]
                : null;
        }

        private CssDeclarationCollection GetDefaultDeclarations(string elementName)
        {
            IHtmlElementProvider element = new MockHtmlElementProvider(elementName);
            CssDeclarationCollection declarations = mDefaultElementStyleResolver.GetDeclarations(element);
            return mComputedDeclarationResolver.ResolveToComputed(declarations);
        }

        /// <summary>
        /// Creates dictionary to map CSS selector value to CSS rule declarations.
        /// </summary>
        private void FillSelectorToComputedDeclarationsMap()
        {
            // Collect declarations by selectors.
            Dictionary<string, CssDeclarationCollectionBuilder> declarationBuilderBySelector =
                new Dictionary<string, CssDeclarationCollectionBuilder>();
            foreach (CssStyleRule rule in mStyleRules)
            {
                foreach (CssSelector ruleSelector in rule.Selectors)
                {
                    string cssSelector = ruleSelector.ToCss();
                    CssDeclarationCollectionBuilder declarationBuilder;
                    if (!declarationBuilderBySelector.TryGetValue(cssSelector, out declarationBuilder))
                    {
                        declarationBuilder = new CssDeclarationCollectionBuilder();
                        declarationBuilderBySelector.Add(cssSelector, declarationBuilder);
                    }
                    // When combining declarations from multiple CSS rules, we must take their importance into account.
                    declarationBuilder.AddOrReplaceIfMoreOrEquallyImportant(rule.Declarations);
                }
            }

            // Compute and cache collected declarations.
            mDeclarationsBySelector.Clear();
            foreach (KeyValuePair<string, CssDeclarationCollectionBuilder> selectorAndBuilder in declarationBuilderBySelector)
            {
                string selector = selectorAndBuilder.Key;
                CssDeclarationCollectionBuilder declarationBuilder = selectorAndBuilder.Value;

                CssDeclarationCollection declarations = declarationBuilder.GetDeclarations();
                declarations = mComputedDeclarationResolver.ResolveToComputed(declarations);

                mDeclarationsBySelector.Add(selector, declarations);
            }
        }

        /// <summary>
        /// Applies floating to a non-floating image that is a single child of one or more floating parent containers.
        /// </summary>
        private CssDeclarationCollection CorrectStyleOfImagesInsideFloatingParents(
            IHtmlElementProvider element,
            CssDeclarationCollection declarations)
        {
            Debug.Assert(element.ElementName == "img");

            CssDeclarationCollection result = declarations;
            if (declarations["float"] == null)
            {
                CssDeclaration parentFloatDeclaration = GetParentWithOnlyChildFloatDeclaration();
                if (parentFloatDeclaration != null)
                {
                    CssDeclarationCollectionBuilder correctedDeclarations = new CssDeclarationCollectionBuilder(declarations);
                    correctedDeclarations.Add(parentFloatDeclaration);
                    result = correctedDeclarations.GetDeclarations();
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the nearest ancestor's float CSS declaration.
        /// </summary>
        private CssDeclaration GetParentWithOnlyChildFloatDeclaration()
        {
            // Discard `html` and `body` tags because they aren't affecting the floating.
            for (int i = mElements.Count - 1; i >= 2; i--)
            {
                HtmlElementInfo element = mElements[i];
                // The ancestor must have the only child because floating should apply on a single shape.
                if (element.Element.GetChildElements().Length > 1)
                    return null;

                CssDeclaration floatDeclaration = element.Declarations["float"];
                if (floatDeclaration != null)
                    return floatDeclaration;
            }
            return null;
        }

        private readonly IList<CssStyleRule> mStyleRules;
        private readonly IList<CssPageRule> mPageRules;
        private readonly string mHtmlElementInlineCssStyle;
        private readonly string mBodyElementInlineCssStyle;
        private readonly CssResolver mCssResolver;
        private readonly HtmlElementCategorizer mElementCategorizer;
        private readonly HtmlAttributeResolver mAttributeResolver;
        private readonly DefaultElementStyleResolver mDefaultElementStyleResolver;
        private readonly CssComputedDeclarationResolver mComputedDeclarationResolver;

        /// <summary>
        /// Currently opened parent HTML elements, from outermost to innermost. 
        /// Items are instances of <see cref="HtmlElementInfo"/>.
        /// </summary>
        private readonly List<HtmlElementInfo> mElements;

        /// <summary>
        /// Text decoration of all currently opened parent HTML elements, from outermost to innermost.
        /// Items are instances of <see cref="CssTextDecoration"/>.
        /// </summary>
        private readonly Stack<CssTextDecoration> mTextDecorationsStack;

        /// <summary>
        /// Inline-level borders of currently opened elements.
        /// </summary>
        private readonly Stack<CssBorder> mInlineBorderStack;
        private readonly BackgroundResolver mBackgroundResolver;

        /// <summary>
        /// Currently opened bidi levels. From outermost to innermost. Items are <see cref="BidiLevel"/> values.
        /// </summary>
        private readonly IntList mActiveBidiLevels;

        private readonly SupSubScriptResolver mSupSubScriptResolver;

        private readonly CssCounters mCounters;

        /// <summary>
        /// CSS declarations that are applied to certain built-in styles.
        /// Key is <see cref="StyleIdentifier"/> casted to <c>int</c>. Value is <see cref="CssDeclarationCollection"/>;
        /// </summary>
        private readonly IntToObjDictionary<CssDeclarationCollection> mPredefinedStyleDeclarations =
            new IntToObjDictionary<CssDeclarationCollection>();

        /// <summary>
        /// Indicates whether we are currently on he current implicit element (div or span).
        /// </summary>
        private bool mIsOnImplicitElement;

        /// <summary>
        /// A real HTML element to return to after we finish processing an implicit element. 
        /// <c>null</c> if we shall return to a pseudo-element.
        /// </summary>
        private IHtmlElementProvider mReturnToElement;

        /// <summary>
        /// A pseudo-element to return to after we finish processing the current implicit element. 
        /// <see cref="HtmlElementPart.Element"/> if we shall return to a real element.
        /// </summary>
        private HtmlElementPart mReturnToPart;

        /// <summary>
        /// Indicates whether we should stick to MS Word's rules when resolving CSS formatting.
        /// </summary>
        private readonly bool mResolveFormattingAsMsWord;

        /// <summary>
        /// Computed CSS declarations that are matched to certain CSS selector.
        /// </summary>
        /// <remarks>
        /// Key is CSS selector value. Value - collection of computed CSS declarations.
        /// </remarks>
        private readonly Dictionary<string, CssDeclarationCollection> mDeclarationsBySelector =
            new Dictionary<string, CssDeclarationCollection>();

        /// <summary>
        /// Indicates whether to mimic MS Word's behavior and use <see cref="HtmlBlock"/> nodes to preserve borders and margins
        /// of block-level elements like BODY, DIV, or BLOCKQUOTE.
        /// </summary>
        private readonly bool mUseHtmlBlocks;
    }
}
