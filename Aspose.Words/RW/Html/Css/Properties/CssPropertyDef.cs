// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/02/2013 by Alexey Butalov

using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base class for CSS properties definition.
    /// http://www.w3.org/TR/CSS21/syndata.html#declaration
    /// </summary>
    internal abstract class CssPropertyDef
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="name">Property name.</param>
        /// <param name="inherited">Indicates that the property is normally inherited.</param>
        protected CssPropertyDef(string name, bool inherited)
        {
            Debug.Assert(StringUtil.HasChars(name));
            Debug.Assert(StringUtil.IsAsciiLowerCase(name));

            Name = name;
            Inherited = inherited;
        }

        /// <summary> 
        /// Returns CSS declarations if the property can accept the specified value.
        /// </summary>
        /// <param name="cssValue">CSS value.</param>
        /// <param name="important">Indicates that the declarations should be marked as !important.</param>
        /// <param name="isInQuirksMode">
        /// Indicates whether the CSS document is being processed in the Quirks mode, as opposed to the Standards mode.
        /// </param>
        /// <returns>CSS declarations if the property can accept the specified value; null otherwise.</returns>
        /// <remarks>Used only for testing.</remarks>
        internal CssDeclarationCollection CreateDeclarations(
            CssValue cssValue,
            bool important,
            bool isInQuirksMode)
        {
            return CreateDeclarations(new CssValueList(cssValue), important, isInQuirksMode);
        }

        /// <summary> 
        /// Returns CSS declarations if the property can accept the specified values.
        /// </summary>
        /// <param name="cssValues">CSS values.</param>
        /// <param name="important">Indicates that the declaration should be marked as !important.</param>
        /// <param name="isInQuirksMode">
        /// Indicates whether the CSS document is being processed in the Quirks mode, as opposed to the Standards mode.
        /// </param>
        /// <returns>CSS declarations if the property can accept the specified values; null otherwise.</returns>
        internal abstract CssDeclarationCollection CreateDeclarations(
            CssValueList cssValues,
            bool important,
            bool isInQuirksMode);

        /// <summary>
        /// Resolves specified value to computed value. E.g. 'em' and 'ex' units are computed to absolute lengths.
        /// </summary>
        /// <remarks>
        /// The computed value is the result of resolving the specified value, generally absolutizing it in preparation for inheritance.
        /// A specified value can be either absolute (i.e., not relative to another value, as in red or 2mm) or relative (i.e., relative to another value, 
        /// as in auto, 2em). Computing a relative value generally absolutizes it:
        ///   - values with relative units (em, ex, vh, vw) must be made absolute by multiplying with the appropriate reference size
        ///   - certain keywords (e.g., smaller, bolder) must be replaced according to their definitions
        ///   - percentages on some properties must be multiplied by a reference value (defined by the property)
        ///   - valid relative URLs must be resolved to become absolute.
        /// </remarks>
        /// <param name="specifiedValue">Specified CSS value.</param>
        /// <param name="cssContext">CSS context.</param>
        /// <returns>Computed CSS value.</returns>
        internal virtual CssPropertyValue ToComputedValue(CssPropertyValue specifiedValue, CssCascadeContext cssContext)
        {
            // If we don't know how this property computes its value, leave it unchanged.
            return specifiedValue;
        }

        /// <summary>
        /// Applies CSS property value to a model table.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void ToTable(CssPropertyValue propertyValue, Table table)
        {
        }

        /// <summary>
        /// Applies CSS property value to a model row.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void ToRow(CssPropertyValue propertyValue, Row row)
        {
        }

        /// <summary>
        /// Applies CSS property value to a model cell format.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void ToCellFormat(CssPropertyValue propertyValue, CellFormat cellFormat)
        {
        }

        /// <summary>
        /// Applies CSS property value to a document.
        /// </summary>
        internal virtual void ToDocument(CssPropertyValue propertyValue, Document document)
        {
        }

        /// <summary>
        /// Applies CSS property value to a horizontal rule shape.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void ToHorizontalRule(CssPropertyValue propertyValue, Shape horizontalRuleShape)
        {
        }

        /// <summary>
        /// Applies CSS property value to a shape.
        /// </summary>
        [JavaThrows(true)]
        internal virtual void ToShape(CssPropertyValue propertyValue, Shape shape)
        {
        }

        /// <summary>
        /// The property name.
        /// </summary>
        internal string Name { get; }

        /// <summary>
        /// Indicates that the property is normally inherited.
        /// </summary>
        internal bool Inherited { get; }

        /// <summary>
        /// Initial value of the property i.e. the value to be used in the absence of any stylesheet or equivalent;
        /// <c>null</c> if the property has no an initial value.
        /// </summary>
        internal virtual CssPropertyValue InitialValue
        {
            get { return null; }
        }
    }
}
