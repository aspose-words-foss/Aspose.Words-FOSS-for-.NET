// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/09/2014 by Nikolay Sezganov

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Combines functionality for work with background.
    /// Stores and calculates current values of background colors.
    /// Applies current background color for paragraph, font, etc.  
    /// 
    /// Helps to emulate browser rendering mechanism.
    /// In some cases parent element background color is visible through child elements because child 
    /// elements are transparent and have their background-color as 'transparent' by default.
    /// 
    /// Potential problems:
    /// 1) Since we avoid duplicating container and child colors, 
    ///   explicit 'background-color' declaration will be omitted for childs having the same color as the container.
    /// 2) The element position in the document is not considered. Position may be absolute.
    /// </summary>
    internal class BackgroundResolver
    {
        internal BackgroundResolver()
        {
            mBackgroundStack = new Stack<ElementBackgroundInfo>();
        }

        /// <summary>
        /// Adds background info of element to the stack.
        /// </summary>
        internal void PushBackground(HtmlElementInfo elementInfo)
        {
            Debug.Assert(elementInfo != null);

            if (elementInfo == null)
                return;

            CssComputedDeclaration backgroundDeclaration = elementInfo.Declarations["background-color"] as CssComputedDeclaration;
            if ((backgroundDeclaration != null) && backgroundDeclaration.Value.Equals(CssValue.Transparent))
            {
                // Ignore transparent backgrounds so lower-level backgrounds will be visible through them.
                backgroundDeclaration = null;
            }

            // Background color already processed for these elements.
            bool isTopLevelContainer = (elementInfo.Element.ElementName == "html") ||
                                       (elementInfo.Element.ElementName == "body");

            CssComputedDeclaration blockBackgroundColor = CurrentElementBackground.BlockBackgroundDeclaration;
            CssComputedDeclaration inlineBackgroundColor = CurrentElementBackground.InlineBackgroundDeclaration;


            // Defines current background colors for block or inline elements.
            if (!isTopLevelContainer && (backgroundDeclaration != null))
            {
                if (elementInfo.IsBlockLevelElement)
                {
                    // Addition background for block elements resets background for inline elements.
                    blockBackgroundColor = backgroundDeclaration;
                    inlineBackgroundColor = null;
                }
                else
                {
                    inlineBackgroundColor = backgroundDeclaration;
                }
            }

            ElementBackgroundInfo item = new ElementBackgroundInfo(
                                                blockBackgroundColor,
                                                inlineBackgroundColor,
                                                isTopLevelContainer,
                                                CurrentElementBackground,
                                                IsContainer(elementInfo));

            mBackgroundStack.Push(item);
        }

        /// <summary>
        /// Removes a top background info.
        /// </summary>
        internal void PopBackground()
        {
            if (mBackgroundStack.Count == 0)
                return;

            mBackgroundStack.Pop();
        }

        /// <summary>
        /// Determines element which may contain other elements.
        /// Container is a html element which has counterpart in AW model.
        /// </summary>
        private static bool IsContainer(HtmlElementInfo elementInfo)
        {
            return (elementInfo.Element.ElementName == "body") || 
                   (elementInfo.DisplayType == CssDisplayType.Table) ||
                   (elementInfo.DisplayType == CssDisplayType.TableCell);
        }

        internal ElementBackgroundInfo CurrentElementBackground
        {
            get
            {
                if ((mBackgroundStack.Count == 0) || mBackgroundStack.Peek().IsTopLevelContainer)
                    return ElementBackgroundInfo.EmptyTopLevelContainer;
                return mBackgroundStack.Peek();
            }
        }

        private readonly Stack<ElementBackgroundInfo> mBackgroundStack;
    }
}
