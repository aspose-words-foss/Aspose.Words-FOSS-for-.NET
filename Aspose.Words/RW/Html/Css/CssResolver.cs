// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/02/2013 by Alexey Butalov

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Calculates CSS declarations for an HTML element using CSS cascading and inheritance principles.
    /// </summary>
    internal class CssResolver
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="documentStyleRules">CSS style rules of a HTML document.</param>
        /// <param name="defaultElementStyleResolver"></param>
        /// <param name="cascadingOrderResolver"></param>
        /// <param name="interdependencyResolver"></param>
        /// <param name="computedDeclarationResolver"></param>
        /// <param name="documentMode"></param>
        /// <param name="defaultQuirksBodyColor"></param>
        internal CssResolver(
            ICollection<CssStyleRule> documentStyleRules,
            DefaultElementStyleResolver defaultElementStyleResolver,
            CssCascadingOrderResolver cascadingOrderResolver,
            CssInterdependencyResolver interdependencyResolver,
            CssComputedDeclarationResolver computedDeclarationResolver,
            CssDocumentMode documentMode,
            DrColor defaultQuirksBodyColor)
        {
            Debug.Assert(documentStyleRules != null);
            Debug.Assert(defaultElementStyleResolver != null);
            Debug.Assert(cascadingOrderResolver != null);
            Debug.Assert(interdependencyResolver != null);
            Debug.Assert(computedDeclarationResolver != null);

            mCssMatcher = new CssMatcher(documentStyleRules, documentMode);
            mDefaultElementStyleResolver = defaultElementStyleResolver;
            mCascadingOrderResolver = cascadingOrderResolver;
            mInterdependencyResolver = interdependencyResolver;
            mDocumentMode = documentMode;
            mDefaultQuirksBodyColor = defaultQuirksBodyColor;
            mComputedDeclarationResolver = computedDeclarationResolver;
            mAncestorCss = new List<ResolvedCss>();
        }

        /// <summary>
        /// Adds an element to the stack and calculates CSS declarations for this element using the cascading and inheritance principles.
        /// </summary>
        internal void PushElement(IElementProvider element)
        {
            PushElement(element, null);
        }

        /// <summary>
        /// Adds an element to the stack and calculates CSS declarations for this element using the cascading and inheritance
        /// principles.
        /// </summary>
        /// <param name="element">An element to add.</param>
        /// <param name="additionalInlineDeclarations">
        /// Additional inline CSS declarations. Can be <c>null</c>. These declarations are processed as if they were appended
        /// to the text of the element's "style" attribute.
        /// </param>
        internal void PushElement(IElementProvider element, CssDeclarationCollection additionalInlineDeclarations)
        {
            CssDeclarationCollectionBuilder declarations = new CssDeclarationCollectionBuilder();
            CssMatchedRules matchedRules = mCssMatcher.Push(element);

            // Implicit elements don't exist in original HTML code and don't have declarations from cascade.
            if (!element.IsImplicit)
            {
                CalculateFromCascade(declarations, element, matchedRules.GetRules(HtmlElementPart.Element),
                    additionalInlineDeclarations);
            }

            CalculateFromParentElement(declarations);
            ResolveInterdependency(declarations, element);

            // A resolved declaration list must contain only computed (not specified) values.
            // This means that value computation must be the last step of declaration resolution.
            CssDeclarationCollection resolvedElementDeclarations = ResolveComputedValues(declarations, false);
            resolvedElementDeclarations.DebugCheckAllComputed();

            // Saving the "color" value for the body. This is the color the table gets in the quirk mode by default.
            if (element.ElementName == "body")
            {
                mBodyColor = resolvedElementDeclarations.GetColor("color");
            }

            ResolvedCss resolvedCss = new ResolvedCss(resolvedElementDeclarations, matchedRules);
            mAncestorCss.Add(resolvedCss);

            CalculateAndFillPseudoElementDeclarations(matchedRules, HtmlElementPart.Before);
            CalculateAndFillPseudoElementDeclarations(matchedRules, HtmlElementPart.After);
        }

        private void CalculateAndFillPseudoElementDeclarations(CssMatchedRules matchedRules, HtmlElementPart part)
        {
            // Fill declarations for the pseudo-element if there are any CSS rules for that pseudo-element in the
            // document's stylesheet. Implicit (anonymous auto-generated elements) don't have pseudo-elements.
            // Pseudo-elements that have no matched rules cannot have any content and are ignored.
            IList<CssStyleRule> rules = matchedRules.GetRules(part);
            if (rules.Count > 0)
            {
                CssDeclarationCollection pseudoElementDeclarations = CalculatePseudoElementDeclarations(rules);
                Debug.Assert(mAncestorCss.Count > 0);
                ResolvedCss currentCss = GetCurrentCss();

                // The pseudo-elements are technically children of the current HTML element and we must know parent's
                // CSS declarations in order to calculate declarations of the pseudo-elements.
                // That is why we have to use the following method that modifies already resolved CSS:
                // 1) calculate and store parent's declarations.
                // 2) use parent's declarations to calculate pseudo-element's declarations.
                // 3) store pseudo-element's declarations next to parent's declarations.
                currentCss.SetDeclarations(part, pseudoElementDeclarations);
            }
        }

        /// <summary>
        /// Removes a top element from the stack.
        /// </summary>
        internal void PopElement()
        {
            Debug.Assert(mAncestorCss.Count != 0);
            mAncestorCss.RemoveAt(mAncestorCss.Count - 1);
            mCssMatcher.Pop();
        }

        /// <summary>
        /// Returns CSS declarations calculated for the current HTML tree element.
        /// </summary>
        internal CssDeclarationCollection GetDeclarations()
        {
            return GetDeclarations(HtmlElementPart.Element);
        }

        /// <summary>
        /// Gets CSS declarations applied to the specified part of the current HTML element.
        /// </summary>
        internal CssDeclarationCollection GetDeclarations(HtmlElementPart part)
        {
            ResolvedCss currentCss = GetCurrentCss();
            return currentCss.GetDeclarations(part);
        }

        /// <summary>
        /// Returns CSS selectors matched for the current HTML tree element.
        /// </summary>
        internal IList<CssSelector> GetSelectors()
        {
            return GetCurrentCss().Selectors;
        }

        /// <summary>
        /// Returns CSS selector declarations matched for the current HTML tree element.
        /// </summary>
        internal CssDeclarationCollection GetSelectorDeclarations()
        {
            return GetCurrentCss().GetSelectorDeclarations();
        }

        internal DefaultElementStyleResolver DefaultElementStyleResolver
        {
            get { return mDefaultElementStyleResolver; }
            set { mDefaultElementStyleResolver = value; }
        }
        /// <summary>
        /// Calculates CSS declarations for a HTML tree element using the cascade principle.
        /// </summary>
        /// <param name="declarations">A declaration collection builder where calculated declarations will be stored.</param>
        /// <param name="element">HTML tree element.</param>
        /// <param name="matchedRules">CSS rules that match the specified element.</param>
        /// <param name="additionalInlineDeclarations">
        /// Additional inline CSS declarations. Can be <c>null</c>. These declarations are processed as if they were appended
        /// to the text of the element's "style" attribute.
        /// </param>
        /// <remarks>
        /// The cascade is a mechanism for determining which styles should be applied to a given element,
        /// based on the rules that have cascaded down from various sources.
        /// 1. Find all declarations that apply to a given element/property combination, for the target media type.
        /// 2. Sort declarations according to their importance (normal or important) and origin (author, user, or user agent). 
        ///    From highest to lowest precedence:
        ///    1.  user !important declarations
        ///    2.  author !important declarations
        ///    3.  author normal declarations
        ///    4.  user normal declarations
        ///    5.  user agent declarations
        /// 3. If declarations have the same importance and source, sort them by selector specificity.
        /// 4. Finally, if declarations have the same importance, source, and specificity, sort them by the order 
        ///   they are specified in the CSS. The last declaration wins.
        /// http://www.w3.org/TR/CSS21/cascade.html#cascade
        /// </remarks>
        private void CalculateFromCascade(
            CssDeclarationCollectionBuilder declarations,
            IElementProvider element,
            IEnumerable<CssStyleRule> matchedRules,
            CssDeclarationCollection additionalInlineDeclarations)
        {
            // Reset the cascade resolver to initial state.
            mCascadingOrderResolver.Clear();
            // Default CSS style and presentational hints.
            CssDeclarationCollection defaultStyleDeclarations = mDefaultElementStyleResolver.GetDeclarations(element);

            mCascadingOrderResolver.Add(defaultStyleDeclarations, CssDeclarationOrigin.UserAgent, CssSelectorSpecificity.PresentationalHintSpecificity);

            // WORDSNET-11639 Incorrect default font color of tables when importing an HTML document in Quirks mode.
            // In Quirks mode, tables get the 'color' value not from their parent elements, but from the <body> element.
            if ((mDocumentMode == CssDocumentMode.Quirks) && (element.ElementName == "table"))
            {
                // WORDSNET-11913 Insertion using DocumentBuilder.InsertHtml is always performed in Quirks mode.
                // By default the color should be black. In case of using the DocumentBuilder formatting (useBuilderFormatting=true), 
                // its color must be undefined.
                DrColor color = (mBodyColor != null) ? mBodyColor : mDefaultQuirksBodyColor;
                if (color != null)
                {
                    CssDeclarationCollectionBuilder builder = new CssDeclarationCollectionBuilder();
                    builder.Add(new CssSpecifiedDeclaration("color", CssHashValue.FromColor(color)));
                    mCascadingOrderResolver.Add(builder.GetDeclarations(), CssDeclarationOrigin.UserAgent, CssSelectorSpecificity.InlineStyleSpecificity);
                }
            }

            // Author style sheet.
            foreach (CssStyleRule rule in matchedRules)
            {
                foreach (CssSelector selector in rule.Selectors)
                {
                    mCascadingOrderResolver.Add(rule.Declarations, CssDeclarationOrigin.Author, selector.Specificity);
                }
            }

            // 'style' HTML attribute.
            CssDeclarationCollection styleDeclarations = null;
            IInlineStyleDeclarationsProvider declarationsProvider = element as IInlineStyleDeclarationsProvider;
            if (declarationsProvider != null)
            {
                styleDeclarations = declarationsProvider.InlineStyle;
            }
            else
            {
                string inlineStyle = element.GetAttributeValue("style");
                if (StringUtil.HasChars(inlineStyle))
                    styleDeclarations = CssParser.ParseDeclarations(inlineStyle, mDocumentMode);
            }
            if ((styleDeclarations != null) && (styleDeclarations.Count != 0))
            {
                mCascadingOrderResolver.Add(styleDeclarations, CssDeclarationOrigin.Author,
                    CssSelectorSpecificity.InlineStyleSpecificity);
            }

            // Additional inline declarations. These declarations are processed as if they were appended to the end of the
            // 'style' attribute.
            if (additionalInlineDeclarations != null)
            {
                mCascadingOrderResolver.Add(additionalInlineDeclarations, CssDeclarationOrigin.Author,
                    CssSelectorSpecificity.InlineStyleSpecificity);
            }

            mCascadingOrderResolver.Resolve(declarations);
        }

        /// <summary>
        /// Calculates CSS declarations for a HTML tree element using the inheritance principle.
        /// </summary>
        /// <remarks>
        /// Inheritance is the process by which elements inherit the values of properties from their ancestors in the DOM tree.
        /// http://www.w3.org/TR/CSS21/cascade.html#inheritance
        /// </remarks>
        private void CalculateFromParentElement(CssDeclarationCollectionBuilder declarations)
        {
            if (mAncestorCss.Count == 0)
                return;

            CssDeclarationCollection parentElementDeclarations = GetCurrentCss().Declarations;
            CssInheritanceResolver.Resolve(declarations, parentElementDeclarations);
        }

        /// <summary>
        /// Resolves some specified values to computed values. E.g. 'em' and 'ex' units are computed to absolute lengths.
        /// </summary>
        /// <param name="declarations"></param>
        /// <remarks>
        /// The computed value is the result of resolving the specified value, generally absolutizing it in preparation
        /// for inheritance.
        /// </remarks>
        /// <param name="isPseudoElement">
        /// Indicates whether the current HTML element is a pseudo-element.
        /// </param>
        /// <returns>CSS declarations with the computed values.</returns>
        private CssDeclarationCollection ResolveComputedValues(
            CssDeclarationCollectionBuilder declarations,
            bool isPseudoElement)
        {
            CssDeclarationCollection rootDeclarations = GetRootCss().Declarations;
            CssDeclarationCollection parentDeclarations = GetCurrentCss().Declarations;
            return mComputedDeclarationResolver.ResolveToComputed(declarations.GetDeclarations(), parentDeclarations,
                rootDeclarations, isPseudoElement);
        }

        /// <summary>
        /// Resolves certain interdependent CSS declarations.
        /// This is expected to be the final step of CSS declaration resolution.
        /// </summary>
        private void ResolveInterdependency(CssDeclarationCollectionBuilder declarations, IElementProvider element)
        {
            mInterdependencyResolver.Resolve(declarations, element);
        }

        /// <summary>
        /// Calculates CSS declarations applied a pseudo-element of the current HTML element.
        /// </summary>
        /// <param name="rules">A collection of <see cref="CssStyleRule"/> that match the pseudo-element.</param>
        private CssDeclarationCollection CalculatePseudoElementDeclarations(IList<CssStyleRule> rules)
        {
            Debug.Assert(rules.Count > 0);

            if (rules.Count == 0)
            {
                return CssDeclarationCollection.Empty;
            }

            CssDeclarationCollectionBuilder declarations = new CssDeclarationCollectionBuilder();

            // Calculate rules applied through the CSS cascade.
            mCascadingOrderResolver.Clear();
            foreach (CssStyleRule rule in rules)
            {
                foreach (CssSelector selector in rule.Selectors)
                {
                    mCascadingOrderResolver.Add(rule.Declarations, CssDeclarationOrigin.Author, selector.Specificity);
                }
            }
            mCascadingOrderResolver.Resolve(declarations);

            // Calculate style inherited from the parent element. The element itself is the parent for its pseudo-elements.
            Debug.Assert(mAncestorCss.Count > 0);
            CssDeclarationCollection parentElementDeclarations = GetDeclarations();
            CssInheritanceResolver.Resolve(declarations, parentElementDeclarations);

            return ResolveComputedValues(declarations, true);
        }

        /// <summary>
        /// Gets resolved CSS of the current (most recently pushed) element.
        /// </summary>
        /// <returns>
        /// Resolved CSS of the current element. If no elements have been processed yet, empty CSS is returned.
        /// </returns>
        private ResolvedCss GetCurrentCss()
        {
            return GetAncestorCss(1);
        }

        /// <summary>
        /// Gets resolved CSS of the root element.
        /// </summary>
        /// <returns>
        /// Resolved CSS of the root element. If no elements have been processed yet, empty CSS is returned.
        /// </returns>
        private ResolvedCss GetRootCss()
        {
            return GetAncestorCss(mAncestorCss.Count);
        }

        /// <summary>
        /// Gets resolved CSS for an ancestor element at the specified one-based depth.
        /// </summary>
        /// <param name="depth">One-based depth of the ancestor element to find (1 - most recently processed (current) element,
        /// 2 - its parent, etc.)</param>
        /// <returns>
        /// Resolved CSS of the ancestor element. If the element is not found (the specified depth is incorrect),
        /// empty CSS is returned.
        /// </returns>
        private ResolvedCss GetAncestorCss(int depth)
        {
            return (depth >= 1) && (depth <= mAncestorCss.Count)
                ? mAncestorCss[mAncestorCss.Count - depth]
                : ResolvedCss.Empty;
        }

        private readonly CssMatcher mCssMatcher;

        private DefaultElementStyleResolver mDefaultElementStyleResolver;
        private readonly CssCascadingOrderResolver mCascadingOrderResolver;
        private readonly CssInterdependencyResolver mInterdependencyResolver;

        /// <summary>
        /// Resolved CSS of HTML elements pushed to the resolver. Elements are of type <see cref="ResolvedCss"/> and are stored
        /// from outer to inner (the last element in the list is the most recently pushed).
        /// </summary>
        private readonly List<ResolvedCss> mAncestorCss;

        private DrColor mBodyColor;
        private readonly CssDocumentMode mDocumentMode;
        private readonly DrColor mDefaultQuirksBodyColor;
        private readonly CssComputedDeclarationResolver mComputedDeclarationResolver;
    }

    /// <summary>
    /// CSS declarations resolved for an HTML element.
    /// </summary>
    internal class ResolvedCss
    {
        internal ResolvedCss(CssDeclarationCollection declarations, CssMatchedRules rules)
        {
            Debug.Assert(declarations != null);
            Debug.Assert(rules != null);

            mDeclarations = declarations;
            mSelectors = rules.GetSelectors(HtmlElementPart.Element);
            mRules = rules;
        }

        /// <summary>
        /// CSS declarations of the element itself (not of any of its pseudo-elements).
        /// </summary>
        internal CssDeclarationCollection Declarations
        {
            get { return mDeclarations; }
        }

        /// <summary>
        /// Selectors that match the element itself (not any of its pseudo-elements).
        /// </summary>
        internal IList<CssSelector> Selectors
        {
            get { return mSelectors; }
        }

        internal CssDeclarationCollection GetSelectorDeclarations()
        {
            CssDeclarationCollectionBuilder declarationsBuilder = new CssDeclarationCollectionBuilder();
            CssCascadingOrderResolver resolver = new CssCascadingOrderResolver();
            foreach (CssStyleRule rule in mRules.GetRules(HtmlElementPart.Element))
            {
                foreach (CssSelector selector in rule.Selectors)
                {
                    resolver.Add(rule.Declarations, CssDeclarationOrigin.Author, selector.Specificity);
                }
            }
            resolver.Resolve(declarationsBuilder);
            return declarationsBuilder.GetDeclarations();
        }

        /// <summary>
        /// Gets declarations of the given part of the element.
        /// </summary>
        /// <returns>
        /// Css declarations of the given part of the element. If no declarations match the part, an empty collection
        /// is returned.
        /// </returns>
        internal CssDeclarationCollection GetDeclarations(HtmlElementPart part)
        {
            CssDeclarationCollection result;
            if (part == HtmlElementPart.Element)
            {
                result = mDeclarations;
            }
            else if (mPseudoElementDeclarations != null)
            {
                result = mPseudoElementDeclarations[(int)part];
                if (result == null)
                {
                    result = CssDeclarationCollection.Empty;
                }
            }
            else
            {
                result = CssDeclarationCollection.Empty;
            }

            Debug.Assert(result != null);
            return result;
        }

        /// <summary>
        /// Sets declarations of the given part of the element.
        /// </summary>
        /// <remarks>
        /// Only pseudo-element declarations can be set using this method. Declarations of the element itself must be set
        /// by using the constructor.
        /// Declarations can be set only once for any given part. Replacing declarations is not allowed.
        /// </remarks>
        internal void SetDeclarations(HtmlElementPart part, CssDeclarationCollection declarations)
        {
            Debug.Assert(part != HtmlElementPart.Element);
            Debug.Assert(declarations != null);

            if (mPseudoElementDeclarations == null)
            {
                mPseudoElementDeclarations = new IntToObjDictionary<CssDeclarationCollection>();
            }

            CssDeclarationCollection existingDeclarations = mPseudoElementDeclarations[(int)part];
            if (existingDeclarations == null)
            {
                mPseudoElementDeclarations[(int)part] = declarations;
            }
            else
            {
                // Replacing declarations is not allowed.
                Debug.Assert(false);
            }
        }

        /// <summary>
        /// Empty CSS. Contains no declarations and no selectors.
        /// </summary>
        internal static readonly ResolvedCss Empty = new ResolvedCss(CssDeclarationCollection.Empty, new CssMatchedRules());

        private readonly CssDeclarationCollection mDeclarations;

        private readonly IList<CssSelector> mSelectors;

        /// <summary>
        /// CSS declarations of pseudo-elements that belong to this HTML element.
        /// Key is <see cref="HtmlElementPart"/> casted to <c>int</c>.
        /// Key is never equal to <see cref="HtmlElementPart.Element"/>.
        /// Value is <see cref="CssDeclarationCollection"/>. Values are never <c>null</c>.
        /// </summary>
        /// <remarks>
        /// Most elements don't contain visible pseudo-elements and this field is lazily initialized when needed in order
        /// to save memory.
        /// </remarks>
        private IntToObjDictionary<CssDeclarationCollection> mPseudoElementDeclarations;

        private readonly CssMatchedRules mRules;
    }
}
