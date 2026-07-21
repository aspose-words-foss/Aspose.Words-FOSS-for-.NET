// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2015 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a single CSS declaration (a property and a value). The value is specified (can be context-dependent).
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/CSS2/cascade.html#specified-value for more information on specified CSS property values.
    /// </remarks>
    internal class CssSpecifiedDeclaration : CssDeclaration
    {
        internal CssSpecifiedDeclaration(string name, CssPropertyValue value, bool isImportant)
            : base(value, isImportant)
        {
            Debug.Assert(StringUtil.HasChars(name));
            Debug.Assert(name == name.ToLowerInvariant());

            if (!value.IsInherit && !value.IsInitial)
            {
                Debug.Assert((name != "font-family") || (value is CssFontFamilyPropertyValue));
                Debug.Assert((name != "background-position") || (value is CssBackgroundPositionPropertyValue));
                Debug.Assert((name != "background") || (value is CssBackgroundPropertyValue));
                Debug.Assert((name != "font") || (value is CssFontPropertyValue));
                Debug.Assert((name != "page-size") || (value is CssPageSizePropertyValue));
                Debug.Assert((name != "text-decoration") || (value is CssTextDecorationPropertyValue));
            }

            mName = name;
        }

        internal CssSpecifiedDeclaration(string name, CssPropertyValue value)
            : this(name, value, false)
        {
        }

        internal CssSpecifiedDeclaration(string name, CssValue cssValue, bool isImportant)
            : this(name, new CssPropertyValue(cssValue), isImportant)
        {
        }

        internal CssSpecifiedDeclaration(string name, CssValue cssValue)
            : this(name, cssValue, false)
        {
        }

        internal override string Property
        {
            get { return mName; }
        }

        private readonly string mName;
    }
}
