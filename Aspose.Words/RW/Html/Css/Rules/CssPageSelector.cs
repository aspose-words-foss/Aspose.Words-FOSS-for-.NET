// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/06/2021 by Victor Chebotok

using System.Text;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// A list of page names of a @page rule.
    /// </summary>
    /// <remarks>
    /// When working with @page rules, we support only selectors that consist of a single page name only and we don't support
    /// page pseudo-classes. As a result, there's no need to cascade values of @page rules and to calculate specificity of their
    /// selectors.
    /// </remarks>
    internal class CssPageSelector
    {
        /// <summary>
        /// Constructor. Initializes a new instance of the class.
        /// </summary>
        /// <param name="pageNames">
        /// Names of pages that the @page rule matches. Null if the rule matches every page. Empty if the rule matches no pages.
        /// </param>
        internal CssPageSelector(string[] pageNames)
        {
            mPageNames = pageNames;
        }

        internal bool Matches(string pageName)
        {
            // According to the specification, the reserved keyword "auto" is a valid selector but it doesn't match any page.
            // This is because "auto" is the default value of the "page" CSS property.
            if (StringUtil.AsciiLowerCase(pageName) == "auto")
            {
                return false;
            }

            // Check if this selector matches every page.
            if (mPageNames == null)
            {
                return true;
            }

            foreach (string acceptedPageName in mPageNames)
            {
                if (acceptedPageName == pageName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the CSS page selector contains any page names.
        /// </summary>
        /// <returns></returns>
        internal bool HasPageNames()
        {
            return (mPageNames != null) && (mPageNames.Length > 0);
        }

        internal string ToCss()
        {
            // An empty selector. Matches any page.
            if (mPageNames == null)
            {
                return "";
            }

            // According to the specification, the reserved keyword "auto" is a valid selector but it doesn't match any
            // page. We use it as a placeholder for selectors that we recognize but don't support.
            if (mPageNames.Length == 0)
            {
                return "auto";
            }

            // List of page names (CSS identifiers).
            StringBuilder list = new StringBuilder();
            foreach (string pageName in mPageNames)
            {
                if (list.Length > 0)
                {
                    list.Append(", ");
                }
                list.Append(CssEscape.EscapeIdentifier(pageName));
            }
            return list.ToString();
        }

        /// <summary>
        /// Names of pages that this selector matches. null - matches any page. Empty - matches nothing.
        /// </summary>
        private readonly string[] mPageNames;
    }
}
