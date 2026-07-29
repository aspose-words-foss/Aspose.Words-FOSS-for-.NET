// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents any unknown (or unsupported by our CSS engine) CSS property definition.
    /// </summary>
    internal class CssUnknownPropertyDef : CssPropertyDef
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name">Property name.</param>
        internal CssUnknownPropertyDef(string name)
            : base(name, false)
        {
            // Empty constructor.
        }

        internal override CssDeclarationCollection CreateDeclarations(
            CssValueList cssValues,
            bool important,
            bool isInQuirksMode)
        {
            CssPropertyValue propertyValue = new CssPropertyValue(cssValues);
            return new CssDeclarationCollection(new CssSpecifiedDeclaration(Name, propertyValue, important));
        }
    }
}
