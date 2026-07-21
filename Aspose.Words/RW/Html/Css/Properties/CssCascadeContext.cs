// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/09/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Incapsulates information used to resolve computed CSS values during the cascade.
    /// </summary>
    internal class CssCascadeContext
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="rootElementDeclarations">
        /// Computed CSS declarations of the root HTML element; null if the element has no root.
        /// </param>
        /// <param name="parentElementDeclarations">
        /// Computed CSS declarations of the parent HTML element; null if the element has no parent.
        /// </param>
        /// <param name="elementFontSize">
        /// Computed 'font-size' value of this element. Relative values of some properties refer to this value.
        /// If this value is zero, default font size is used.
        /// </param>
        /// <param name="isPseudoElement">
        /// Indicates whether the current HTML element is a pseudo-element.
        /// </param>
        internal CssCascadeContext(
            CssDeclarationCollection rootElementDeclarations, 
            CssDeclarationCollection parentElementDeclarations,
            double elementFontSize,
            bool isPseudoElement)
        {
            if (rootElementDeclarations != null)
                rootElementDeclarations.DebugCheckAllComputed();
            if (parentElementDeclarations != null)
                parentElementDeclarations.DebugCheckAllComputed();
            Debug.Assert(elementFontSize >= 0);

            mRootElementDeclarations = rootElementDeclarations;
            mParentElementDeclarations = parentElementDeclarations;
            mElementFontSize = elementFontSize;
            mIsPseudoElement = isPseudoElement;
        }

        /// <summary>
        /// Computed CSS declarations of the parent HTML element; null if the element has no parent.
        /// </summary>
        internal CssDeclarationCollection ParentElementDeclarations
        {
            get { return mParentElementDeclarations; }
        }

        /// <summary>
        /// Computed CSS declarations of the root HTML element; null if the element has no root.
        /// </summary>
        internal CssDeclarationCollection RootElementDeclarations
        {
            get { return mRootElementDeclarations; }
        }

        /// <summary>
        /// Computed 'font-size' value of this element. Relative values of some properties (for instance, 'line-height')
        /// refer to this value.
        /// </summary>
        internal double ElementFontSize
        {
            get { return mElementFontSize; }
        }

        /// <summary>
        /// Gets a value indicating whether this HTML element is a pseudo-element.
        /// </summary>
        internal bool IsPseudoElement
        {
            get { return mIsPseudoElement; }
        }

        private readonly CssDeclarationCollection mParentElementDeclarations;
        private readonly CssDeclarationCollection mRootElementDeclarations;
        private readonly double mElementFontSize;
        private readonly bool mIsPseudoElement;
    }
}
