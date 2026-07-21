// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/01/2013 by Alexey Butalov

using System.Text;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a single CSS declaration. Each declaration consists of a property and a value. The value can be either
    /// specified or computed.
    /// </summary>
    /// <remarks>
    /// Only individual properties are allowed in this declaration block. Shorthand properties are converted
    /// into sets of individual properties during reading. This makes our CSS model simpler.
    /// This class is immutable. All derived classes should be immutable also. 
    /// </remarks>
    internal abstract class CssDeclaration
    {
        /// <summary>
        /// Creates a new instance of CssDeclaration class.
        /// </summary>
        /// <param name="value">Property value.</param>
        /// <param name="important">Indicates that the declaration is marked as !important.</param>
        protected CssDeclaration(CssPropertyValue value, bool important)
        {
            Debug.Assert(value != null);

            mValue = value;
            mImportant = important;
        }

        /// <summary>
        /// Outputs a CSS representation of the declaration to StringBuilder.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstMethod]
        internal void ToCss(StringBuilder sb)
        {
            sb.Append(Property);
            sb.Append(':');
            mValue.ToCss(sb);
            if (mImportant)
            {
                sb.Append(" !important");
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return (obj.GetType() == GetType()) && Equals((CssDeclaration)obj);
        }

        protected bool Equals(CssDeclaration other)
        {
            if (other == null)
                return false;
            return mImportant == other.mImportant && mValue.Equals(other.mValue);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (mImportant.GetHashCode() * 397) ^ mValue.GetHashCode();
            }
        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            ToCss(sb);
            return sb.ToString();
        }
#endif

        /// <summary>
        /// CSS property name (always lowercase).
        /// </summary>
        internal abstract string Property { get; }

        /// <summary>
        /// CSS property value.
        /// </summary>
        internal CssPropertyValue Value
        {
            get { return mValue; }
        }

        /// <summary>
        /// Indicates whether the CSS declaration is marked as !important.
        /// </summary>
        internal bool Important
        {
            get { return mImportant; }
        }

        internal bool HasSingleValueOfType(CssValueType valueType)
        {
            return (Value.Count == 1) && (Value.FirstValue.ValueType == valueType);
        }

        /// <summary>
        /// Either specified or computed CSS property value. 
        /// </summary>
        private readonly CssPropertyValue mValue;

        /// <summary>
        /// Indicates whether the CSS declaration is marked as !important.
        /// </summary>
        private readonly bool mImportant;


    }
}
