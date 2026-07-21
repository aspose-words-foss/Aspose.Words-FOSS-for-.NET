// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2013 by Alexey Butalov

using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents the CSS class selector. For example, '.class'.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/css3-selectors/#class-html
    /// </remarks>
    internal class CssClassSelector : CssSimpleSelector
    {
        internal CssClassSelector(string className)
        {
            Debug.Assert(StringUtil.HasChars(className));
            mClassName = className;
        }

        internal override bool Selects(IElementProvider element, CssDocumentMode documentMode)
        {
            // For performance reasons, we use a separate method to get 'class' attribute value 
            // instead of fetching it from the attribute collection of the element.
            string[] classes = element.GetClasses();
            if (classes.Length == 0)
            {
                return false;
            }

            foreach (string className in classes)
            {
                if (documentMode == CssDocumentMode.Quirks)
                {
                    if (StringUtil.EqualsIgnoreCase(className, mClassName))
                    {
                        return true;
                    }
                }
                else
                {
                    // Ordinal case-sensitive comparison.
                    if (className == mClassName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal override CssSelectorSpecificity Specificity
        {
            get { return gSpecificity; }
        }

        internal override string ToCss()
        {
            // WORDSNET-12814 We use class names to store style names in HTML format. However, style names (even processed
            // with our style id generator) are not always valid CSS identifiers (for example, style names can start with
            // a digit) so we must properly escape such style names before exporting them as class names.
            return "." + CssEscape.EscapeIdentifier(mClassName);
        }

        internal string ClassName
        {
            get { return mClassName; }
        }

        protected override string MakePreferableStyleName()
        {
            return mClassName;
        }

        private readonly string mClassName;

        private static readonly CssSelectorSpecificity gSpecificity = new CssSelectorSpecificity(0, 1, 0);
    }
}
