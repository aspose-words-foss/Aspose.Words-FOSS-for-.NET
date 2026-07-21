// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2015 by Nikolay Sezganov

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to propagate alignment styles from 'sup' or 'sub' tags to nested inline elements.
    /// </summary>
    internal class SupSubScriptResolver
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal SupSubScriptResolver()
        {
            mInfoStack = new Stack<CssVerticalAlign>();
        }

        internal CssVerticalAlign CurrentElementInfo
        {
            get { return mInfoStack.Peek(); }
        }

        /// <summary>
        /// Adds information about styles to stacks.
        /// </summary> 
        internal void Push(HtmlElementInfo elementInfo)
        {
            CssPropertyValue verticalAlignValue = null;

            // The style propagation to the nested element is allowed if element is not located in block-level element.
            if (!elementInfo.IsBlockLevelElement)
            {
                CssComputedDeclaration verticalAlignDeclaration = elementInfo.Declarations["vertical-align"] as CssComputedDeclaration;
                if (verticalAlignDeclaration != null)
                {
                    verticalAlignValue = verticalAlignDeclaration.Value;
                }

                if ((verticalAlignValue == null) && (mInfoStack.Count > 0))
                {
                    CssVerticalAlign prevInfo = mInfoStack.Peek();
                    verticalAlignValue = prevInfo.VerticalAlignValue;
                }
            }

            mInfoStack.Push(new CssVerticalAlign(verticalAlignValue));
        }

        /// <summary>
        /// Removes a top info.
        /// </summary>
        internal void Pop()
        {
            mInfoStack.Pop();
        }

        /// <summary>
        /// Stack stores information about 'sup' or 'sub' elements and their nested elements and
        /// style which is needed to propagate to nested inline elements.
        /// </summary>
        private readonly Stack<CssVerticalAlign> mInfoStack;
    }
}