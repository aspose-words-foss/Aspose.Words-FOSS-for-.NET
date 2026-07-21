// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/02/2013 by Alexey Butalov

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Aspose.Drawing;
using Aspose.Words.Fonts;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents an immutable collection of <see cref="CssDeclaration">CSS declarations</see>.
    /// </summary>
    /// <remarks>
    /// This collection is immutable so declarations stored in it cannot be modified unintentionally.
    /// This class does not preserve the order in which declarations are added.
    /// </remarks>
    internal class CssDeclarationCollection : IEnumerable<CssDeclaration>
    {
        /// <summary>
        /// Creates a new empty collection.
        /// </summary>
        internal CssDeclarationCollection()
        {
            mDeclarations = new CssDeclarationHashtable();
            mDeclarations.MakeReadOnly();
        }

        /// <summary>
        /// Creates a new collection containing only one declaration.
        /// </summary>
        /// <param name="declaration">A CSS declaration that should be added to the collection.</param>
        internal CssDeclarationCollection(CssDeclaration declaration)
        {
            mDeclarations = new CssDeclarationHashtable();
            mDeclarations.Add(declaration);
            mDeclarations.MakeReadOnly();
        }

        /// <summary>
        /// Creates a new collection containing one or more declarations.
        /// </summary>
        /// <param name="declarations">CSS declaration(s) that should be added to the collection.</param>
        internal CssDeclarationCollection(params CssDeclaration[] declarations)
        {
            mDeclarations = new CssDeclarationHashtable();
            mDeclarations.Add(declarations);
            mDeclarations.MakeReadOnly();
        }

        /// <summary>
        /// Creates a new collection containing the specified declarations.
        /// </summary>
        /// <param name="declarations">
        /// Zero or more instances of <see cref="CssDeclaration"/> that should be added to the collection.
        /// </param>
        internal CssDeclarationCollection(IEnumerable<CssDeclaration> declarations)
        {
            mDeclarations = new CssDeclarationHashtable();
            mDeclarations.Add(declarations);
            mDeclarations.MakeReadOnly();
        }

        /// <summary>
        /// Creates a new collection containing the specified declarations.
        /// This constructor is used for performance and memory optimization in
        /// <see cref="CssDeclarationCollectionBuilder.GetDeclarations"/> method.
        /// You should not call it directly.
        /// </summary>
        /// <param name="declarations">
        /// A hashtable that will be used as a backing field for the new collection. Values are instances of
        /// <see cref="CssDeclaration"/> and keys are corresponding property names (strings). The hashtable contents must not
        /// change after the new collection is created, because the collection must be immutable.
        /// </param>
        internal CssDeclarationCollection(CssDeclarationHashtable declarations)
        {
            Debug.Assert(declarations != null);
            Debug.Assert(declarations.IsReadOnly);
            mDeclarations = declarations;
        }

        public IEnumerator<CssDeclaration> GetEnumerator()
        {
            return mDeclarations.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator object that iterates through <see cref="CssDeclaration"/> instances stored in this collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            foreach (CssDeclaration declaration in mDeclarations)
            {
                hashCode = (hashCode * 397) ^ declaration.GetHashCode();
                hashCode = (hashCode * 397) ^ ((int)GetFlags(declaration.Property));
            }
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return (obj.GetType() == GetType()) && Equals((CssDeclarationCollection)obj);
        }

        protected bool Equals(CssDeclarationCollection other)
        {
            if (mDeclarations.Count != other.mDeclarations.Count)
            {
                return false;
            }

            foreach (CssDeclaration thisDeclaration in mDeclarations)
            {
                CssDeclaration otherDeclaration = other.mDeclarations[thisDeclaration.Property];
                if (!thisDeclaration.Equals(otherDeclaration))
                {
                    return false;
                }

                CssDeclarationFlags thisFlags = GetFlags(thisDeclaration.Property);
                CssDeclarationFlags otherFlags = other.GetFlags(thisDeclaration.Property);
                if (thisFlags != otherFlags)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a property length value measured in points if the property exists in the collection
        /// and property value is a correct length value. Otherwise returns <c>double.MinValue</c>.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        internal double GetLength(string propertyName)
        {
            CssDeclaration declaration = this[propertyName];
            return (declaration != null)
                ? CssUtil.LengthToPoint(declaration.Value)
                : double.MinValue;
        }

        /// <summary>
        /// Returns a property length value measured in points and indicates whether the value comes from default (user agent)
        /// CSS. If the property doesn't exists in the collection or if the property value is not a correct length value,
        /// the method returns <see cref="CssLength.ZeroDefault"/>.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        internal CssLength GetCssLength(string propertyName)
        {
            CssDeclaration declaration = this[propertyName];
            double value = (declaration != null)
                ? CssUtil.LengthToPoint(declaration.Value)
                : 0;
            bool isDefault = (declaration == null) || IsUserAgent(propertyName);
            return new CssLength(value, !isDefault);
        }

        /// <summary>
        /// Returns a property identifier value if the property exists in the collection
        /// and property value is a correct identifier value. Otherwise returns an empty string.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        internal string GetIdentifier(string propertyName)
        {
            CssDeclaration declaration = this[propertyName];
            bool isIdentifier = (declaration != null) && declaration.HasSingleValueOfType(CssValueType.Identifier);
            return isIdentifier
                ? ((CssIdentifierValue)declaration.Value.FirstValue).Value
                : string.Empty;
        }

        internal bool ContainsIdentifier(string propertyName, string value)
        {
            return StringUtil.EqualsIgnoreCase(GetIdentifier(propertyName), value);
        }

        /// <summary>
        /// Returns a property uri value if the property exists in the collection and property value is a correct uri value.
        /// Otherwise returns empty string.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        internal string GetUri(string propertyName)
        {
            CssDeclaration declaration = this[propertyName];
            bool isUri = (declaration != null) && declaration.HasSingleValueOfType(CssValueType.Uri);
            return isUri
                ? ((CssUriValue)declaration.Value.FirstValue).Value
                : string.Empty;
        }

        /// <summary>
        /// Returns a property number value if the property exists in the collection
        /// and property value is a correct number value. Otherwise returns <c>double.MinValue</c>.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        internal double GetNumber(string propertyName)
        {
            CssDeclaration declaration = this[propertyName];
            bool isNumber = (declaration != null) && declaration.HasSingleValueOfType(CssValueType.Number);
            return isNumber
                ? ((CssNumberValue)declaration.Value.FirstValue).DoubleValue
                : double.MinValue;
        }

        /// <summary>
        /// Returns a property percentage value if the property exists in the collection
        /// and property value is a correct percentage value. Otherwise returns <c>double.MinValue</c>.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        internal double GetPercentage(string propertyName)
        {
            CssDeclaration declaration = this[propertyName];
            bool isPercentage = (declaration != null) && declaration.HasSingleValueOfType(CssValueType.Percentage);
            return isPercentage
                ? ((CssPercentageValue)declaration.Value.FirstValue).DoubleValue
                : double.MinValue;
        }

        /// <summary>
        /// Returns a property string value if the property exists in the collection
        /// and property value is a correct string value. Otherwise returns an empty string. The returned string has no quotes.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        internal string GetString(string propertyName)
        {
            CssDeclaration declaration = this[propertyName];
            bool isString = (declaration != null) && declaration.HasSingleValueOfType(CssValueType.String);
            return isString
                ? ((CssStringValue)declaration.Value.FirstValue).Value
                : string.Empty;
        }

        /// <summary>
        /// Returns the property value as a string if the property exists in the collection and the property value
        /// is a valid identifier or string. Otherwise returns an empty string. The returned string has no enclosing quotes.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        internal string GetIdentifierOrString(string propertyName)
        {
            CssDeclaration declaration = this[propertyName];
            if ((declaration != null) && (declaration.Value.Count == 1))
            {
                CssValue cssValue = declaration.Value.FirstValue;
                if ((cssValue.ValueType == CssValueType.Identifier) || (cssValue.ValueType == CssValueType.String))
                {
                    return (string)cssValue.Value;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns a property color value if the property exists in the collection
        /// and property value is a correct color value. Otherwise returns <c>null</c>.
        /// </summary>
        /// <param name="propertyName">Property name.</param>
        internal DrColor GetColor(string propertyName)
        {
            CssDeclaration declaration = this[propertyName];
            return (declaration != null)
                ? declaration.Value.ParseAsColor()
                : null;
        }

        /// <summary>
        /// Returns font name accordingly to the value of 'font-family' CSS property using the font provider to check fonts
        /// for availability.
        /// </summary>
        /// <returns>
        /// Returns font name or empty string in case of missing declaration and other inconsistencies.
        /// </returns>
        internal string GetFontName(
            string propertyName,
            CssFontFaceProvider cssFontFaceProvider,
            DocumentFontProvider fontProvider)
        {
            CssDeclaration fontFamilyDeclaration = this[propertyName];
            if (fontFamilyDeclaration == null)
            {
                return string.Empty;
            }

            if (!(fontFamilyDeclaration.Value is CssFontFamilyPropertyValue))
            {
                return string.Empty;
            }

            // Check fonts from the "font-family" list for availability and return the name of the first font that is available.
            // Return the last font name from the list in case none of the fonts is available.
            string resolvedFontName = string.Empty;
            for (int i = 0; i < fontFamilyDeclaration.Value.Count; i++)
            {
                string requestedFontFamily;
                CssValue fontFamilyValue = fontFamilyDeclaration.Value[i];
                switch (fontFamilyValue.ValueType)
                {
                    case CssValueType.Identifier:
                        requestedFontFamily = GetGenericFontFamily(((CssIdentifierValue)fontFamilyValue).Value);
                        break;
                    case CssValueType.String:
                        requestedFontFamily = ((CssStringValue)fontFamilyValue).Value;
                        break;
                    default:
                        Debug.Fail("Unexpected type of a CSS value.");
                        continue;
                }

                // Here, we are trying to import the requested font from the @font-face CSS rule.
                if (cssFontFaceProvider != null)
                {
                    cssFontFaceProvider.ImportFontFaceToDocument(requestedFontFamily);
                }

                // FIX WORDSNET-17383 - Previously, we used FontPal.FindInstalledFontName to resolve font names.
                // Now we use font provider and font settings. It's useful if customer wants to control font substitution
                // while loading document.
                string resolvedFont = fontProvider.GetFontNameForHtml(requestedFontFamily);
                if (resolvedFont != null)
                {
                    resolvedFontName = resolvedFont;
                    break;
                }

                resolvedFontName = requestedFontFamily;
            }
            return resolvedFontName;
        }

        internal string GetFontName(
            CssFontFaceProvider cssFontFaceProvider,
            DocumentFontProvider fontProvider)
        {
            return GetFontName("font-family", cssFontFaceProvider, fontProvider);
        }

        /// <summary>
        /// Gets a copy of these declarations with all longhand properties substituted with their shorthand counterparts
        /// where possible.
        /// </summary>
        internal CssDeclarationCollection GetShorthandVersion()
        {
            ICollection<CssShorthandPropertyDef> shorthandProperties = CssPropertyDefFactory.ShorthandProperties;

            // MEMORY. Normally only a small subset of declarations is reducible to shorthands. We considerably decrease number
            // of memory allocations if we check declarations for reducibility before actually trying to reduce them.
            bool isReducible = false;
            foreach (CssShorthandPropertyDef shorthandProp in shorthandProperties)
            {
                if (shorthandProp.CanReduce(this))
                {
                    isReducible = true;
                    break;
                }
            }
            if (!isReducible)
            {
                return this;
            }

            CssDeclarationCollectionBuilder modifiedCollection = new CssDeclarationCollectionBuilder(this);
            foreach (CssShorthandPropertyDef shorthandProp in shorthandProperties)
            {
                shorthandProp.Reduce(modifiedCollection);
            }
            return modifiedCollection.GetDeclarations();
        }

        /// <summary>
        /// Removes each of the specified properties from the declarations if the property's value is user-agent default.
        /// </summary>
        internal CssDeclarationCollection WithoutUserAgentDeclarations(string[] userAgentPropertiesToRemove)
        {
            if ((userAgentPropertiesToRemove == null) || (userAgentPropertiesToRemove.Length == 0))
            {
                return this;
            }

            // The result builder is initialized lazily, because in the most common scenario there's nothing to remove.
            CssDeclarationCollectionBuilder modifiedCollection = null;
            foreach (string property in userAgentPropertiesToRemove)
            {
                // Remove the property if its value is user-agent default.
                if ((GetFlags(property) & CssDeclarationFlags.UserAgent) != 0)
                {
                    if (modifiedCollection == null)
                    {
                        modifiedCollection = new CssDeclarationCollectionBuilder(this);
                    }
                    modifiedCollection.Remove(property);
                }
            }
            return (modifiedCollection != null)
                ? modifiedCollection.GetDeclarations()
                : this;
        }

        /// <summary>
        /// Indicates whether this property comes from our User Agent stylesheet.
        /// </summary>
        internal bool IsUserAgent(string property)
        {
            return mDeclarations.GetFlag(property, CssDeclarationFlags.UserAgent);
        }

        /// <summary>
        /// Gets all flags that are set for a property.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstMethod]
        internal CssDeclarationFlags GetFlags(string property)
        {
            return mDeclarations.GetFlags(property);
        }

        /// <summary>
        /// Generates text representation of the declarations.
        /// Declarations are ordered by some preferable order and finished by our -aw-* CSS properties.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstMethod]
        internal string ToCss()
        {
            SortedList<string, CssDeclaration> sortedList =
                new SortedList<string, CssDeclaration>(new CssPropertyNameComparer());
            foreach (CssDeclaration declaration in mDeclarations)
            {
                sortedList.Add(declaration.Property, declaration);
            }

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, CssDeclaration> pair in sortedList)
            {
                if (sb.Length > 0)
                    sb.Append("; ");
                pair.Value.ToCss(sb);
            }
            return sb.ToString();
        }

        internal CssDeclarationHashtable GetHashtable()
        {
            Debug.Assert(mDeclarations.IsReadOnly);
            return mDeclarations;
        }

        /// <summary>
        /// In Debug checks that all declarations are resolved to computed values
        /// (all declarations are instances of <see cref="CssComputedDeclaration"/>).
        /// Does nothing in Release build.
        /// </summary>
        [Conditional("DEBUG")]
        internal void DebugCheckAllComputed()
        {
            foreach (CssDeclaration declaration in mDeclarations)
            {
                if (!(declaration is CssComputedDeclaration))
                {
                    Debug.Assert(false, "All declarations must be computed!");
                }
            }
        }

#if DEBUG
        public override string ToString()
        {
            return ToCss();
        }
#endif

        /// <summary>
        /// Returns the number of declarations in the collection.
        /// </summary>
        internal int Count
        {
            get { return mDeclarations.Count; }
        }

        /// <summary>
        /// Gets a CSS declaration for the specified property.
        /// </summary>
        /// <param name="property">CSS property name.</param>
        /// <returns>CSS declaration, if found; otherwise, null.</returns>
        internal CssDeclaration this[string property]
        {
            get { return mDeclarations[property]; }
        }

        /// <summary>
        /// An empty collection of CSS declarations.
        /// </summary>
        internal static readonly CssDeclarationCollection Empty = new CssDeclarationCollection();

        private static string GetGenericFontFamily(string value)
        {
            string result;
            switch (value.ToLowerInvariant())
            {
                case "serif":
                    result = "Times New Roman";
                    break;
                case "sans-serif":
                    result = "Arial";
                    break;
                case "cursive":
                    // http://www.kayskreations.net/fonts/fonttb.html
                    // http://www.ampsoft.net/webdesign-l/WindowsMacFonts.html
                    result = "Comic Sans MS";
                    break;
                case "fantasy":
                    // At the moment lets default to Arial. No browser-safe defaults.
                    result = "Arial";
                    break;
                case "monospace":
                    result = "Courier New";
                    break;
                default:
                    // If we cannot recognize the value as a generic family name, we just use it as a font name.
                    result = value;
                    break;
            }
            return result;
        }

        /// <summary>
        /// The backing collection that stores all declarations. The collection is always read-only.
        /// </summary>
        private readonly CssDeclarationHashtable mDeclarations;
    }
}
