// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/02/2013 by Alexey Butalov

using Aspose.Words.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to store an element information in <see cref="DocumentFormatter"/>.
    /// Stores intermediate values used for performance optimization.
    /// </summary>
    internal abstract class HtmlElementInfo
    {
        protected HtmlElementInfo(IHtmlElementProvider element, CssDeclarationCollection declarations)
        {
            Debug.Assert(element != null);
            Debug.Assert(declarations != null);
            declarations.DebugCheckAllComputed();

            Element = element;
            mDeclarations = declarations;
        }

        /// <summary>
        /// Element declarations. All declarations are computed.
        /// </summary>
        internal CssDeclarationCollection Declarations
        {
            get { return GetDeclarations(HtmlElementPart.Element); }
        }

        /// <summary>
        /// Gets the part of an HTML element that these properties relate to. 
        /// </summary>
        internal abstract HtmlElementPart Part { get; }

        /// <summary>
        /// Gets generated content of this element. Only pseudo-elements can have generated content.
        /// </summary>
        /// <returns>
        /// Generated content of this element or <c>null</c> if the element has no generated content.
        /// </returns>
        internal abstract PseudoElementContent GeneratedContent { get; }

        /// <summary>
        /// Gets CSS declarations applied to the ::before pseudo-element of this element.
        /// </summary>
        /// <returns>
        /// A collection of CSS declarations. The result is never <c>null</c> but it may be empty.
        /// </returns>
        internal abstract CssDeclarationCollection BeforePseudoElementDeclarations { get; }

        /// <summary>
        /// Gets CSS declarations applied to the ::after pseudo-element of this element.
        /// </summary>
        /// <returns>
        /// A collection of CSS declarations. The result is never <c>null</c> but it may be empty.
        /// </returns>
        internal abstract CssDeclarationCollection AfterPseudoElementDeclarations { get; }

        /// <summary>
        /// Gets CSS declarations from the specified part of the element. All declarations are computed.
        /// </summary>
        /// <returns>
        /// CSS declarations from the specified part of the element. The result is never <c>null</c>.
        /// </returns>
        internal CssDeclarationCollection GetDeclarations(HtmlElementPart part)
        {
            switch (part)
            {
                case HtmlElementPart.Element:
                    return mDeclarations;
                case HtmlElementPart.Before:
                    return BeforePseudoElementDeclarations;
                case HtmlElementPart.After:
                    return AfterPseudoElementDeclarations;
                default:
                    Debug.Assert(false);
                    return CssDeclarationCollection.Empty;
            }
        }

        /// <summary>
        /// HTML Element.
        /// </summary>
        internal IHtmlElementProvider Element;

        /// <summary>
        /// Parent block-level element info; null if parent element not found.
        /// </summary>
        internal HtmlElementInfo ParentBlockElement;

        /// <summary>
        /// Determines whether the element is a block-level element according to its CSS declarations.
        /// </summary>
        internal bool IsBlockLevelElement;

        /// <summary>
        /// Text direction of the block-level element currently being processed.
        /// </summary>
        internal CssDirection BlockLevelDirection;

        /// <summary>
        /// Text direction of the inline-level element currently being processed.
        /// </summary>
        internal CssDirection InlineLevelDirection;

        /// <summary>
        /// Element's display state based on CSS display:none property.
        /// </summary>
        internal HtmlElementDisplayState DisplayState;

        /// <summary>
        /// Element display type based on CSS "display" property.
        /// </summary>
        internal CssDisplayType DisplayType;

        /// <summary>
        /// A value indicating whether this HTML node contain any child nodes: either real or pseudo-elements.
        /// </summary>
        internal bool HasChildren;

        /// <summary>
        /// Absolute and effective size in points of this element or <c>null</c> if no sizes are specified.
        /// </summary>
        /// <remarks>
        /// Effective size is the element's absolute "width" and "height" values restricted by its "min-width" and "min-height"
        /// declarations.
        /// </remarks>
        internal SizeD EffectiveSize;

        private readonly CssDeclarationCollection mDeclarations;
    }
}
