// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/02/2013 by Alexey Butalov

using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Resolves CSS declarations using CSS inheritance principle.
    /// </summary>
    /// <remarks>
    /// Each element in an HTML document is part of a tree, and every element except the initial 'html'
    /// element has a parent element that encloses it. Whatever styles are applied to that parent element 
    /// can be applied to the elements enclosed in it if the properties are inherited.
    /// http://www.w3.org/TR/CSS21/cascade.html#inheritance
    /// </remarks>
    internal class CssInheritanceResolver
    {
        /// <summary>
        /// Updates element's CSS declarations with declarations inherited from the parent element.
        /// </summary>
        /// <param name="elementDeclarations">Element's CSS declaration. This collection will be updated.</param>
        /// <param name="parentDeclarations">Parent element's computed CSS declarations.</param>
        internal static void Resolve(
            CssDeclarationCollectionBuilder elementDeclarations,
            CssDeclarationCollection parentDeclarations)
        {
            // Parent declarations must be computed before they can be inherited.
            parentDeclarations.DebugCheckAllComputed();

            // Each property may also have a cascaded value of 'inherit', which means that, for a given element, the property
            // takes as specified value the computed value of the element's parent (see errata in the CSS 2.1 Specification:
            // http://www.w3.org/Style/css2-updates/REC-CSS2-20110607-errata.html).
            // The 'inherit' value can be used to enforce inheritance of values, and it can also be used on properties that
            // are not normally inherited.
            List<string> inheritedPropertyNames = new List<string>();
            foreach (CssDeclaration elementDeclaration in elementDeclarations)
            {
                if (elementDeclaration.Value.IsInherit)
                {
                    inheritedPropertyNames.Add(elementDeclaration.Property);
                }
            }
            foreach (string propertyName in inheritedPropertyNames)
            {
                elementDeclarations.Remove(propertyName);
                CssDeclaration parentDeclaration = parentDeclarations[propertyName];
                if ((parentDeclaration != null) && (!parentDeclaration.Value.IsInherit))
                {
                    elementDeclarations.Add(parentDeclaration);
                    elementDeclarations.CopyFlags(propertyName, parentDeclarations);
                }
            }

            // Inherit parent's declarations that are normally inherited and are not overridden by element's declarations.
            foreach (CssDeclaration parentDeclaration in parentDeclarations)
            {
                CssPropertyDef propertyDef = CssPropertyDefFactory.GetPropertyDef(parentDeclaration.Property);
                if (propertyDef.Inherited && (elementDeclarations[parentDeclaration.Property] == null))
                {
                    elementDeclarations.Add(parentDeclaration);
                    elementDeclarations.CopyFlags(parentDeclaration.Property, parentDeclarations);
                }
            }
        }
    }
}
