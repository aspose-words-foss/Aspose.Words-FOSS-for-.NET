// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/08/2013 by Alexey Butalov

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Helps to determine category of a HTML element.
    /// </summary>
    /// <remarks>
    /// In general, HTML elements can be divided into two categories: block level and inline elements. 
    /// </remarks>
    internal class HtmlElementCategorizer
    {
        internal HtmlElementCategorizer(CssClassFactory classFactory)
        {
            Debug.Assert(classFactory != null);
            mClassFactory = classFactory;
        }

        static HtmlElementCategorizer()
        {
            mCssValueToDisplayTypeMap = new Dictionary<CssValue, CssDisplayType>();
            mCssValueToDisplayTypeMap.Add(CssValue.Inline, CssDisplayType.Inline);
            mCssValueToDisplayTypeMap.Add(CssValue.Block, CssDisplayType.Block);
            mCssValueToDisplayTypeMap.Add(CssValue.InlineBlock, CssDisplayType.InlineBlock);
            mCssValueToDisplayTypeMap.Add(CssValue.ListItem, CssDisplayType.ListItem);
            mCssValueToDisplayTypeMap.Add(CssValue.RunIn, CssDisplayType.RunIn);
            mCssValueToDisplayTypeMap.Add(CssValue.Table, CssDisplayType.Table);
            mCssValueToDisplayTypeMap.Add(CssValue.InlineTable, CssDisplayType.InlineTable);
            mCssValueToDisplayTypeMap.Add(CssValue.TableCaption, CssDisplayType.TableCaption);
            mCssValueToDisplayTypeMap.Add(CssValue.TableHeaderGroup, CssDisplayType.TableHeaderGroup);
            mCssValueToDisplayTypeMap.Add(CssValue.TableColumnGroup, CssDisplayType.TableColumnGroup);
            mCssValueToDisplayTypeMap.Add(CssValue.TableColumn, CssDisplayType.TableColumn);
            mCssValueToDisplayTypeMap.Add(CssValue.TableRowGroup, CssDisplayType.TableRowGroup);
            mCssValueToDisplayTypeMap.Add(CssValue.TableRow, CssDisplayType.TableRow);
            mCssValueToDisplayTypeMap.Add(CssValue.TableCell, CssDisplayType.TableCell);
            mCssValueToDisplayTypeMap.Add(CssValue.TableFooterGroup, CssDisplayType.TableFooterGroup);
            mCssValueToDisplayTypeMap.Add(CssValue.None, CssDisplayType.None);
        }

        /// <summary>
        /// Determines whether the element is a block-level element according to its CSS declarations.
        /// </summary>
        /// <param name="element">HTML element.</param>
        /// <param name="elementDeclarations">HTML element actual declarations. The actual declarations are the used declarations after any such adjustments have been made.</param>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Non-static to provide a consistent class interface where all methods are called in the same way.")]
        internal bool IsBlockLevelElement(IHtmlElementProvider element, CssDeclarationCollection elementDeclarations)
        {
            Debug.Assert(element != null);
            Debug.Assert(elementDeclarations != null);

            CssDeclaration displayDeclaration = elementDeclarations["display"];
            bool isBlock;
            if ((displayDeclaration == null) || displayDeclaration.Value.Equals(CssValue.None))
                isBlock = IsBlockElementByDefault(element.ElementName);
            else
                isBlock = !(displayDeclaration.Value.Equals(CssValue.Inline) ||
                            displayDeclaration.Value.Equals(CssValue.InlineBlock));
            return isBlock;
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
            Justification = "Non-static to provide a consistent class interface where all methods are called in the same way.")]
        internal CssDisplayType GetDisplayType(CssDeclarationCollection elementDeclarations)
        {
            Debug.Assert(elementDeclarations != null);

            CssDeclaration displayDeclaration = elementDeclarations["display"];

            if (displayDeclaration == null)
                return CssDisplayType.Inline;

            if (mCssValueToDisplayTypeMap.ContainsKey(displayDeclaration.Value.FirstValue))
                return  mCssValueToDisplayTypeMap[displayDeclaration.Value.FirstValue];

            return CssDisplayType.Inline;
        }

        /// <summary>
        /// Determines whether the element is a block-level element.
        /// </summary>
        /// <param name="element">HTML element.</param>
        /// <param name="documentStyleRules">CSS style rules of a HTML document.</param>
        internal bool IsBlockLevelElement(
            IHtmlElementProvider element, 
            ICollection<CssStyleRule> documentStyleRules)
        {
            Debug.Assert(element != null);
            Debug.Assert(documentStyleRules != null);

            Stack<IElementProvider> elementTreeStack = new Stack<IElementProvider>();
            IElementProvider htmlElement = element;
            while (htmlElement != null)
            {
                elementTreeStack.Push(htmlElement);
                htmlElement = htmlElement.GetParentElement();
            }

            CssResolver resolver = mClassFactory.CreateCssResolver(documentStyleRules);
            while (elementTreeStack.Count != 0)
            {
                resolver.PushElement(elementTreeStack.Pop());
            }

            return IsBlockLevelElement(element, resolver.GetDeclarations());
        }

        /// <summary>
        /// Checks whether this element is represented as block-level element.
        /// </summary>
        internal static bool IsBlockElementByDefault(string elementName)
        {
            switch (elementName)
            {
                case "address":
                case "article":
                case "aside":
                case "audio":
                case "blockquote":
                case "body":
                case "canvas":
                case "div":
                case "dl":
                case "dd":
                case "fieldset":
                case "figcaption":
                case "figure":
                case "footer":
                case "form":
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                case "hr":
                case "html":
                case "header":
                case "hgroup":
                case "li":
                case "noscript":
                case "ol":
                case "p":
                case "pre":
                case "table":
                case "tfoot":
                case "ul":
                case "video":
                case "xmp":
                case "listing":
                    return true;
                default:
                    return false;
            }
        }

        private readonly CssClassFactory mClassFactory;
        private static readonly Dictionary<CssValue, CssDisplayType> mCssValueToDisplayTypeMap;
    }
}
