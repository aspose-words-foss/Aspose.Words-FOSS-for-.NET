// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/02/2013 by Alexey Butalov

using System.Text;
using Aspose.Common;
using Aspose.Drawing;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents an identifier CSS property value.
    /// </summary>
    internal class CssIdentifierValue : CssValue
    {
        internal CssIdentifierValue(string identifier)
            : base(CssValueType.Identifier, identifier)
        {
            Debug.Assert(StringUtil.HasChars(identifier));
            mIdentifier = identifier;
            mLowerCaseIdentifier = identifier.ToLowerInvariant();
        }

        internal override void ToCss(StringBuilder sb)
        {
            sb.Append(CssEscape.EscapeIdentifier(mIdentifier));
        }

        internal override DrColor ParseAsColor()
        {
            return CssColorParser.ParseColorName(mIdentifier);
        }

        protected override bool DoEquals(CssValue other)
        {
            // This behaviour increases performance by ~2%.
            return mLowerCaseIdentifier == ((CssIdentifierValue)other).mLowerCaseIdentifier;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = mLowerCaseIdentifier.GetHashCode();
                //Java-changed: casting enum to int is analog of GetHashCode().
                result = (result * 397) ^ (int)ValueType;
                return result;
            }
        }

        internal new string Value
        {
            get { return mIdentifier; }
        }

        private readonly string mIdentifier;
        /// <summary>
        /// Lower case identifier for performance optimization.
        /// </summary>
        private readonly string mLowerCaseIdentifier;
    }
}
