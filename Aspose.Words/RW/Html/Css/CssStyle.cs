// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/08/2004 by Roman Korchagin

using System.Collections;
using System.Collections.Generic;
using Aspose.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a single CSS declaration block.
    /// The names of the values in the collection are case insensitive.
    /// Note: don't set shorthand CSS properties! Use individual ones.
    /// </summary>
    /// <remarks>
    /// This class is a wrapper for <see cref="CssDeclarationCollectionBuilder"/> class and should be used in HTML export only.
    /// </remarks>
    internal class CssStyle : IEnumerable<CssDeclaration>
    {
        internal CssStyle()
        {
            mDeclarationBuilder = new CssDeclarationCollectionBuilder();
        }

        internal void SetIdentifier(string name, string value)
        {
            Debug.Assert(StringUtil.HasChars(value));
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration(name, new CssIdentifierValue(value)));
        }

        internal void SetString(string name, string value)
        {
            SetString(name, value, false);
        }

        internal void SetString(string name, string value, bool writeEmptyValue)
        {
            Debug.Assert(StringUtil.HasChars(value) || writeEmptyValue);
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration(name, new CssStringValue(value)));
        }

        internal void SetNumber(string name, double value)
        {
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration(name, new CssNumberValue(value)));
        }

        internal void SetPercentage(string name, double value)
        {
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration(name, new CssPercentageValue(value)));
        }

        internal void SetLength(string name, double value, CssUnit unit)
        {
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration(name, new CssLengthValue(value, unit)));
        }

        internal void SetColor(string name, DrColor value)
        {
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration(name, CssHashValue.FromColor(value)));
        }

        internal void SetUri(string name, string uriValue)
        {
            Debug.Assert(StringUtil.HasChars(uriValue));
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration(name, new CssUriValue(uriValue)));
        }

        internal void SetFunction(string name, string functionName, CssValueList functionArgs)
        {
            Debug.Assert(StringUtil.HasChars(functionName));
            Debug.Assert(functionArgs != null);
            Debug.Assert(functionArgs.Count > 0);

            mDeclarationBuilder.AddOrReplace(
                new CssSpecifiedDeclaration(name, new CssFunctionValue(functionName, functionArgs)));
        }

        internal void SetList(string name, CssValueList values)
        {
            Debug.Assert(values.Count > 0);
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration(name, new CssPropertyValue(values)));
        }

        /// <summary>
        /// Sets 'font-family' CSS property.
        /// </summary>
        internal void SetFontFamily(params string[] fontFamilyNames)
        {
            if (fontFamilyNames.Length != 0)
            {
                CssValueList values = new CssValueList();
                foreach (string fontFamilyName in fontFamilyNames)
                    values.Add(CssValue.CreateFontFamilyValue(fontFamilyName));
                mDeclarationBuilder.AddOrReplace(
                    new CssSpecifiedDeclaration("font-family", new CssFontFamilyPropertyValue(values)));
            }
        }

        /// <summary>
        /// Sets 'background' CSS property.
        /// </summary>
        internal void SetBackground(DrColor color)
        {
            mDeclarationBuilder.AddOrReplace(
                new CssSpecifiedDeclaration("background",
                    new CssBackgroundPropertyValue(CssHashValue.FromColor(color), null, null, null, null)));
        }

        /// <summary>
        /// Sets 'margin' CSS shorthand property.
        /// </summary>
        internal void SetMargin(CssValue top, CssValue right, CssValue bottom, CssValue left)
        {
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration("margin-top", top));
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration("margin-right", right));
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration("margin-bottom", bottom));
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration("margin-left", left));
        }

        /// <summary>
        /// Sets 'margin' CSS shorthand property.
        /// </summary>
        internal void SetMargin(double top, double right, double bottom, double left)
        {
            SetMargin(new CssLengthValue(top, CssUnit.Pt),
                new CssLengthValue(right, CssUnit.Pt),
                new CssLengthValue(bottom, CssUnit.Pt),
                new CssLengthValue(left, CssUnit.Pt));
        }

        /// <summary>
        /// Sets 'padding' CSS shorthand property.
        /// </summary>
        internal void SetPadding(double top, double right, double bottom, double left)
        {
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration("padding-top",
                new CssLengthValue(top, CssUnit.Pt)));
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration("padding-right",
                new CssLengthValue(right, CssUnit.Pt)));
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration("padding-bottom",
                new CssLengthValue(bottom, CssUnit.Pt)));
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration("padding-left",
                new CssLengthValue(left, CssUnit.Pt)));
        }

        /// <summary>
        /// Sets 'size' CSS property.
        /// </summary>
        internal void SetPageSize(double width, double height, CssUnit unit)
        {
            CssLengthValue widthValue = new CssLengthValue(width, unit);
            CssPageSizePropertyValue value = MathUtil.AreEqual(width, height)
                ? CssPageSizePropertyValue.CreateLength(widthValue)
                : CssPageSizePropertyValue.CreateLength(widthValue, new CssLengthValue(height, unit));
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration("size", value));
        }

        /// <summary>
        /// Sets 'border' CSS shorthand property.
        /// </summary>
        /// <param name="style">Style value.</param>
        /// <param name="width">Width value, can be <c>null</c>.</param>
        /// <param name="color">Colorvalue, can be <c>null</c>.</param>
        internal void SetBorder(CssValue style, CssValue width, CssValue color)
        {
            CssPropertyDef borderPropertyDef = CssPropertyDefFactory.GetPropertyDef("border");
            SetBorder(borderPropertyDef, style, width, color);
        }

        /// <summary>
        /// Sets 'border-right' CSS shorthand property.
        /// </summary>
        /// <param name="style">Style value.</param>
        /// <param name="width">Width value, can be <c>null</c>.</param>
        /// <param name="color">Colorvalue, can be <c>null</c>.</param>
        internal void SetBorderRight(CssValue style, CssValue width, CssValue color)
        {
            CssPropertyDef borderPropertyDef = CssPropertyDefFactory.GetPropertyDef("border-right");
            SetBorder(borderPropertyDef, style, width, color);
        }

        /// <summary>
        /// Sets 'border-bottom' CSS shorthand property.
        /// </summary>
        /// <param name="style">Style value.</param>
        /// <param name="width">Width value, can be <c>null</c>.</param>
        /// <param name="color">Colorvalue, can be <c>null</c>.</param>
        internal void SetBorderBottom(CssValue style, CssValue width, CssValue color)
        {
            CssPropertyDef borderPropertyDef = CssPropertyDefFactory.GetPropertyDef("border-bottom");
            SetBorder(borderPropertyDef, style, width, color);
        }

        /// <summary>
        /// Sets 'text-decoration' CSS shorthand property.
        /// </summary>
        internal void SetTextDecoration(CssValueList cssValues)
        {
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration("text-decoration",
                new CssTextDecorationPropertyValue(cssValues)));
        }

        /// <summary>
        /// Sets 'font' CSS shorthand property.
        /// </summary>
        internal void SetFont(CssValue fontSize, CssValue fontFamily)
        {
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration("font",
                new CssFontPropertyValue(fontSize, fontFamily)));
        }

        /// <summary>
        /// Sets 'text-decoration' CSS shorthand property.
        /// </summary>
        internal void SetTextDecoration(string value)
        {
            if (StringUtil.HasChars(value))
            {
                mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration("text-decoration",
                    new CssTextDecorationPropertyValue(new CssIdentifierValue(value))));
            }
        }

        /// <summary>
        /// Sets background linear-gradient CSS property.
        /// </summary>
        internal void SetLinearGradientBackground(CssValueList linearGradientValues)
        {
            CssValueList collection = new CssValueList();
            foreach (CssValue linearGradientValue in linearGradientValues)
            {
                if (collection.Count > 0)
                    collection.Add(new CssCommaValue());
                collection.Add(linearGradientValue);
            }
            mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration("background",
                new CssBackgroundPropertyValue(collection)));
        }

        /// <summary>
        /// Adds properties from another CssStyle.
        /// </summary>
        /// <param name="cssStyle"></param>
        internal void AddPropertiesFrom(CssStyle cssStyle)
        {
            if (cssStyle != null)
            {
                mDeclarationBuilder.AddOrReplace(cssStyle.mDeclarationBuilder);
            }
        }

        /// <summary>
        /// Adds the specified declaration to the style replacing a declaration with same property name if any exists.
        /// </summary>
        /// <remarks>
        /// This method has been added to improve performance.
        /// </remarks>
        /// <param name="declaration">CSS declaration.</param> 
        internal void AddOrReplace(CssDeclaration declaration)
        {
            mDeclarationBuilder.AddOrReplace(declaration);
        }

        /// <summary>
        /// Creates a CSS representation of the style.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstMethod]
        internal string ToCss()
        {
            return (mDeclarationBuilder.Count != 0)
                ? mDeclarationBuilder.GetDeclarations().ToCss()
                : string.Empty;
        }

#if DEBUG
        public override string ToString()
        {
            return ToCss();
        }
#endif

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<CssDeclaration> GetEnumerator()
        {
            return mDeclarationBuilder.GetEnumerator();
        }

        /// <summary>
        /// Removes the attribute with the specified name from the style.
        /// </summary>
        /// <param name="key"></param>
        internal void Remove(string key)
        {
            mDeclarationBuilder.Remove(key);
        }

        internal CssDeclarationCollection GetDeclarations()
        {
            return mDeclarationBuilder.GetDeclarations();
        }

        /// <summary>
        /// Sets 'border[-(top|left|right|bottom)]' CSS shorthand property.
        /// </summary>
        /// <param name="borderPropertyDef"></param>
        /// <param name="style">Style value.</param>
        /// <param name="width">Width value, can be <c>null</c>.</param>
        /// <param name="color">Colorvalue, can be <c>null</c>.</param>
        private void SetBorder(
            CssPropertyDef borderPropertyDef,
            CssValue style,
            CssValue width,
            CssValue color)
        {
            Debug.Assert(style != null);

            CssValueList values = new CssValueList(style);
            if (width != null)
                values.Add(width);
            if (color != null)
                values.Add(color);
            CssDeclarationCollection declarations = borderPropertyDef.CreateDeclarations(values, false, false);
            Debug.Assert(declarations != null);
            mDeclarationBuilder.AddOrReplace(declarations);
        }

        /// <summary>
        /// Gets/sets a property by name.
        /// </summary>
        internal CssPropertyValue this[string name]
        {
            get
            {
                CssDeclaration declaration = mDeclarationBuilder[name];
                return (declaration == null)
                           ? null
                           : declaration.Value;
            }
            set { mDeclarationBuilder.AddOrReplace(new CssSpecifiedDeclaration(name, value)); }
        }

        /// <summary>
        /// Gets the number of attributes contained in the style.
        /// </summary>
        internal int Count
        {
            get { return mDeclarationBuilder.Count; }
        }

        /// <summary>
        /// Declaration collection builder, should store only individual CSS properties.
        /// </summary>
        private readonly CssDeclarationCollectionBuilder mDeclarationBuilder;
    }
}
