// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2015 by Victor Chebotok

using Aspose.Words.Drawing;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a single CSS declaration (a property and a value). The value of this declaration is computed
    /// (context dependency is resolved).
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/TR/CSS2/cascade.html#computed-value for more information on computed CSS property values.
    /// </remarks>
    internal class CssComputedDeclaration : CssDeclaration
    {
        internal CssComputedDeclaration(
            CssPropertyDef propertyDef,
            CssPropertyValue value,
            bool important,
            CssPropertyValue originalSpecifiedValue)
            : base(value, important)
        {
            Debug.Assert(propertyDef != null);
            Debug.Assert(originalSpecifiedValue != null);
            mPropertyDef = propertyDef;
            OriginalSpecifiedValue = originalSpecifiedValue;
        }

        /// <summary>
        /// Applies the CSS declaration to a model table.
        /// </summary>
        internal void ToTable(Table table)
        {
            mPropertyDef.ToTable(Value, table);
        }

        /// <summary>
        /// Applies the CSS declaration to a model row.
        /// </summary>
        internal void ToRow(Row row)
        {
            mPropertyDef.ToRow(Value, row);
        }

        /// <summary>
        /// Applies the CSS declaration to a model cell format.
        /// </summary>
        internal void ToCellFormat(CellFormat cellFormat)
        {
            mPropertyDef.ToCellFormat(Value, cellFormat);
        }

        /// <summary>
        /// Applies the CSS declaration to a document.
        /// </summary>
        internal void ToDocument(Document document)
        {
            mPropertyDef.ToDocument(Value, document);
        }

        /// <summary>
        /// Applies the CSS declaration to a horizontal rule shape.
        /// </summary>
        internal void ToHorizontalRule(Shape horizontalRuleShape)
        {
            mPropertyDef.ToHorizontalRule(Value, horizontalRuleShape);
        }

        /// <summary>
        /// Applies the CSS declaration to a shape.
        /// </summary>
        internal void ToShape(Shape shape)
        {
            mPropertyDef.ToShape(Value, shape);
        }

        /// <summary>
        /// The original specified value for which this value has been computed.
        /// </summary>
        internal CssPropertyValue OriginalSpecifiedValue { get; }

        internal override string Property
        {
            get { return mPropertyDef.Name; }
        }

        private readonly CssPropertyDef mPropertyDef;
    }
}
