// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/01/2013 by Alexey Butalov

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base simple CSS selector class.
    /// http://www.w3.org/TR/css3-selectors/
    /// </summary>
    internal abstract class CssSelector
    {
        static CssSelector()
        {
            #region Names of attributes whose values are case-insensitive

            // The list of attributes is taken from here: http://rakaz.nl/2006/10/css-selector-bugs-case-sensitivity.html
            // This list is also used at least by Konqueror (https://bugs.kde.org/show_bug.cgi?id=135505)
            // and by Mozilla (https://bugzilla.mozilla.org/show_bug.cgi?id=357614)
            string[] names = new string[]
            {
                "accept",
                "accept-charset",
                "align",
                "alink",
                "axis",
                "bgcolor",
                "charset",
                "checked",
                "clear",
                "codetype",
                "color",
                "compact",
                "declare",
                "defer",
                "dir",
                "disabled",
                "enctype",
                "face",
                "frame",
                "hreflang",
                "http-equiv",
                "lang",
                "language",
                "link",
                "media",
                "method",
                "multiple",
                "nohref",
                "noresize",
                "noshade",
                "nowrap",
                "readonly",
                "rel",
                "rev",
                "rules",
                "scope",
                "scrolling",
                "selected",
                "shape",
                "target",
                "text",
                "type",
                "valign",
                "valuetype",
                "vlink"
            };

            #endregion

            // Attribute names are placed into a hash table for performance reasons.
            gNamesOfAttributesWithCaseInsensitiveValues = new Dictionary<string, bool>();
            foreach (string name in names)
            {
                gNamesOfAttributesWithCaseInsensitiveValues.Add(name, false);
            }
        }

        /// <summary>
        /// Creates a new type selector for the specified element name.
        /// </summary>
        internal static CssTypeSelector Element(string elementName)
        {
            return new CssTypeSelector(CssNamespace.Any, elementName);
        }

        /// <summary>
        /// Creates a new class selector for the specified class name.
        /// </summary>
        internal static CssClassSelector ClassSelector(string className)
        {
            return new CssClassSelector(className);
        }

        /// <summary>
        /// Creates a new ::before pseudo-element selector on an element with the specified name.
        /// </summary>
        internal static CssCompoundSelector Before(string elementName)
        {
            return new CssCompoundSelector(Element(elementName),
                new CssPseudoElementSelector(HtmlElementPart.Before, "before"));
        }

        /// <summary>
        /// Identifies what part of an HTML element this selector selects. It is either the element itself or any of its
        /// pseudo-elements.
        /// </summary>
        internal virtual HtmlElementPart SelectedPart
        {
            get
            {
                // Most selectors work not with pseudo-elements but with whole HTML elements.
                return HtmlElementPart.Element;
            }
        }

        /// <summary>
        /// Gets a style class name for the CSS selector.
        /// </summary>
        internal string GetPreferableStyleName()
        {
            string styleName = MakePreferableStyleName();
            Debug.Assert(StringUtil.HasChars(styleName));
            return styleName;
        }

        /// <summary>
        /// CSS Specificity of the selector.
        /// http://www.w3.org/TR/selectors/#specificity
        /// </summary>
        internal abstract CssSelectorSpecificity Specificity { get; }

        /// <summary>
        /// Gets the CSS declaration of the selector.
        /// </summary>
        internal abstract string ToCss();

        /// <summary>
        /// Indicates whether the value of the specified attribute should be compared by selectors using 
        /// a case-insensitive method.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>
        /// <c>true</c>, if the value of the attribute should be treated by selectors in a case-insensitive way.
        /// <c>false</c> otherwise.
        /// </returns>
        /// <remarks>
        /// Although the HTML 5 specification requires to treat almost all attribute values as case-sensitive
        /// for the purpose of CSS selector matching (see http://www.w3.org/TR/html5/selectors.html#case-sensitivity),
        /// there is a group of attributes whose values are treated as case-insensitive by all modern browsers
        /// for historical reasons. See http://rakaz.nl/2006/10/css-selector-bugs-case-sensitivity.html for detais.
        /// </remarks>
        internal static bool AttributeValueIsCaseInsensitive(string attributeName)
        {
            Debug.Assert(StringUtil.HasChars(attributeName));
            Debug.Assert(StringUtil.IsAsciiLowerCase(attributeName));

            return gNamesOfAttributesWithCaseInsensitiveValues.ContainsKey(attributeName);
        }

#if DEBUG
        public override string ToString()
        {
            return ToCss();
        }
#endif

        /// <summary>
        /// Creates an instance of <see cref="CssSelectorMatcher"/> to match this selector against HTML elements 
        /// in the specified mode.
        /// </summary>
        internal abstract CssSelectorMatcher CreateMatcher(CssDocumentMode documentMode);

        /// <summary>
        /// Override in derived classes and return a style class name for the CSS selector.
        /// </summary>
        /// <returns>The style class name for the CSS selector.</returns>
        protected abstract string MakePreferableStyleName();

        /// <summary>
        /// The list of the attributes whose values should be compared by selectors in a case-insensitive way.
        /// </summary>
        private static readonly Dictionary<string, bool> gNamesOfAttributesWithCaseInsensitiveValues =
            new Dictionary<string, bool>();
    }
}
