// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/04/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Determines default (user agent) vertical margins of HTML elements.
    /// </summary>
    /// <remarks>
    /// These margins cannot be declared directly in the user agent style sheet, because in this case we will not be able
    /// to tell where a CSS margin property came from: the author style sheet or the user agent style sheet.
    /// </remarks>
    internal static class HtmlDefaultVerticalMargins
    {
        /// <summary>
        /// Gets a value indicating whether an HTML element has non-zero default (user agent) vertical margins.
        /// </summary>
        internal static bool AreNonZero(IHtmlElementProvider element)
        {
            return !MathUtil.IsZero(GetMargins(element));
        }

        /// <summary>
        /// Returns default (user agent) vertical margins of HTML elements, in em.
        /// </summary>
        /// <param name="element">An HTML element.</param>
        /// <returns>
        /// Vertical margins of the HTML element as declared in the default (user agent) style sheet.
        /// </returns>
        internal static double GetMargins(IHtmlElementProvider element)
        {
             switch (element.ElementName)
             {
                 case "h1":
                     return GetDefaultH1VerticalMargins(element);
                 case "h2":
                     return GetDefaultHeadingVerticalMargins(1);
                 case "h3":
                     return GetDefaultHeadingVerticalMargins(2);
                 case "h4":
                     return GetDefaultHeadingVerticalMargins(3);
                 case "h5":
                     return GetDefaultHeadingVerticalMargins(4);
                 case "h6":
                     return GetDefaultHeadingVerticalMargins(5);
                 case "dir":
                 case "menu":
                 case "ol":
                 case "ul":
                     return GetDefaultListVerticalMargins(element);
                 case "blockquote":
                 case "dl":
                 case "figure":
                 case "listing":
                 case "plaintext":
                 case "pre":
                 case "xmp":
                     return 1;
                 case "p":
                     // Implicit (anonymous) paragraphs do not have vertical margins.
                     return (element.IsImplicit)
                         ? 0
                         : 1;
                 case "hr":
                     return 0.5;
                 default:
                     return 0;
             }
         }

        private static double GetDefaultH1VerticalMargins(IHtmlElementProvider element)
        {
            int nestingLevel = 0;
            const int maxNestingLevel = 5;
            IElementProvider parent = element.GetParentElement();
            while ((parent != null) && (nestingLevel < maxNestingLevel))
            {
                if ((parent.ElementName == "article") ||
                    (parent.ElementName == "aside") ||
                    (parent.ElementName == "nav") ||
                    (parent.ElementName == "section"))
                {
                    ++nestingLevel;
                }
                parent = parent.GetParentElement();
            }

            return GetDefaultHeadingVerticalMargins(nestingLevel);
        }

        private static double GetDefaultHeadingVerticalMargins(int headingLevel)
        {
            double[] margins = new double[] { 0.67, 0.83, 1, 1.33, 1.67, 2.33 };
            return ((headingLevel >= 0) && (headingLevel < margins.Length))
                ? margins[headingLevel]
                : 1;
        }

        private static double GetDefaultListVerticalMargins(IHtmlElementProvider element)
        {
            bool isNested = false;
            IElementProvider parent = element.GetParentElement();
            while ((parent != null) && (!isNested))
            {
                if ((parent.ElementName == "ol") ||
                    (parent.ElementName == "ul") ||
                    (parent.ElementName == "dir") ||
                    (parent.ElementName == "menu"))
                {
                    isNested = true;
                }
                parent = parent.GetParentElement();
            }
            return (isNested)
                ? 0
                : 1;
        }
    }
}
