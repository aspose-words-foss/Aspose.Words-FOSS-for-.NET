// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/01/2013 by Alexey Butalov

using System.Text;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Base CSS value class. 
    /// This class is immutable. All derived classes should be immutable also. 
    /// </summary>
    internal abstract class CssValue
    {
        /// <summary>
        /// Creates a new CSS value.
        /// </summary>
        protected CssValue(CssValueType valueType, object value)
        {
            mValueType = valueType;
            mValue = value;
        }

        public override bool Equals(object obj)
        {
            // Standard comparisons.
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (!(obj is CssValue))
                return false;

            // Invoke the class-specific implementation.
            return Equals((CssValue)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (mValue != null) ? mValue.GetHashCode() : 0;
                //Java-changed: casting enum to int is analog of GetHashCode().
                result = (result * 397) ^ (int)mValueType;
                return result;
            }
        }

        internal static CssValue CreateFontFamilyValue(string fontName)
        {
            // If all characters of a font name are allowed in CSS identifiers, the font name can be exported without quotes
            // (as an identifier).
            if (CssEscape.IsValidIdentifier(fontName))
                return new CssIdentifierValue(fontName);
            return new CssStringValue(fontName);
        }

        /// <summary>
        /// Parses a length value in pixels according to HTML 5 rules.
        /// </summary>
        /// <param name="s">The value to be parsed.</param>
        /// <returns>
        /// The length value, if the parsing was successful;
        /// null otherwise.
        /// </returns>
        /// <remarks>
        /// The algorithm is taken from here: https://html.spec.whatwg.org/multipage/rendering.html#maps-to-the-pixel-length-property
        /// </remarks>
        internal static CssValue ParseLegacyPixelLength(string s)
        {
            int value = HtmlUtil.ParseNonNegativeInteger(s);
            return (value >= 0)
                ? new CssLengthValue(value, CssUnit.Px)
                : null;
        }

        /// <summary>
        /// Parses a dimension value (either a pixel length or a percentage) according to the HTML 5 rules.
        /// </summary>
        /// <param name="s">The value to be parsed.</param>
        /// <returns>
        /// The parsed value, if parsing was successful;
        /// <c>null</c> otherwise.
        /// </returns>
        /// <remarks>
        /// The algorithm is taken from https://html.spec.whatwg.org/multipage/rendering.html#maps-to-the-dimension-property
        /// </remarks> 
        internal static CssValue ParseLegacyDimension(string s)
        {
            if (s == null)
                return null;

            // 1. Let input be the string being parsed.
            // 2. Let position be a pointer into input, initially pointing at the start of the string.
            int i = 0;

            // 3. Skip whitespace.
            while ((i < s.Length) && HtmlUtil.IsWhitespace(s[i]))
            {
                ++i;
            }

            // 4. If position is past the end of input, return an error.
            if (i >= s.Length)
            {
                return null;
            }

            // 5. If the character indicated by position is a "+" (U+002B) character, advance position to the next character.
            if (s[i] == '\x002B')
            {
                ++i;
            }

            // 6. Collect a sequence of characters that are "0" (U+0030) characters, and discard them.
            while ((i < s.Length) && (s[i] == '\x0030'))
            {
                ++i;
            }

            // 7. If position is past the end of input, return an error.
            if (i >= s.Length)
            {
                return null;
            }

            // 8. If the character indicated by position is not one of "1" (U+0031) to "9" (U+0039), then return an error.
            if (!StringUtil.IsDigit(s[i]))
            {
                return null;
            }

            // 9. Collect a sequence of characters in the range ASCII digits, and interpret the resulting sequence 
            // as a base-ten integer. Let value be that number.
            double value = 0;
            while ((i < s.Length) && StringUtil.IsDigit(s[i]))
            {
                value *= 10;
                int digit = s[i] - '0';
                value += digit;
                ++i;
            }

            // 10. If position is past the end of input, return value as a length.
            if (i >= s.Length)
            {
                return new CssLengthValue(value, CssUnit.Px);
            }

            // 11. If the character indicated by position is a "." (U+002E) character:
            if (s[i] == '\x002E')
            {
                // 11.1. Advance position to the next character.
                ++i;

                // 11.2. If position is past the end of input, or if the character indicated by position 
                // is not one of ASCII digits, then return value as a length.
                if ((i >= s.Length) || !StringUtil.IsDigit(s[i]))
                {
                    return new CssLengthValue(value, CssUnit.Px);
                }

                // 11.3. Let divisor have the value 1.
                // Note that the algorithm is slightly modified to reduce the number of divisions.
                // When the number of divisions is high, the result accuracy degrades considerably.
                double divisor = 1.0;
                double fraction = 0.0;

                do
                {
                    // 11.4. Fraction loop: Multiply divisor by ten.
                    divisor *= 10;
                    fraction *= 10;

                    // 11.5. Add the value of the character indicated by position, interpreted as a base-ten digit (0..9)
                    // and divided by divisor, to value.
                    int digit = s[i] - '0';
                    fraction += digit;

                    // 11.6. Advance position to the next character.
                    ++i;

                    // 11.7. If position is past the end of input, then return value as a length.
                    if (i >= s.Length)
                    {
                        value += fraction / divisor;
                        return new CssLengthValue(value, CssUnit.Px);
                    }

                    // 11.8. If the character indicated by position is one of ASCII digits, 
                    //return to the step labeled fraction loop in these substeps.
                } while (StringUtil.IsDigit(s[i]));

                value += fraction / divisor;
            }

            // 12. If position is past the end of input, return value as a length.
            if (i >= s.Length)
            {
                return new CssLengthValue(value, CssUnit.Px);
            }

            // 13. If the character indicated by position is a "%" (U+0025) character, return value as a percentage.
            if (s[i] == '\x0025')
            {
                return new CssPercentageValue(value);
            }

            // 14. Return value as a length.
            return new CssLengthValue(value, CssUnit.Px);
        }

        /// <summary>
        /// Parses a color value according to HTML 5 rules for legacy colors.
        /// </summary>
        /// <param name="s">The color value to be parsed.</param>
        /// <returns>
        /// The parsed color value, if parsing was sucessful;
        /// <c>null</c> otherwise.
        /// </returns>
        /// <remarks>
        /// The algorithm is taken from here: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#rules-for-parsing-a-legacy-colour-value
        /// </remarks>
        internal static CssValue ParseLegacyColor(string s)
        {
            // 1. Let input be the string being parsed.
            // 2. If input is the empty string, then return an error.
            if (!StringUtil.HasChars(s))
                return null;

            // 3. Strip leading and trailing whitespace from input.
            int begin = 0;
            while ((begin < s.Length) && HtmlUtil.IsWhitespace(s[begin]))
            {
                ++begin;
            }
            int end = s.Length - 1;
            while ((end >= begin) && HtmlUtil.IsWhitespace(s[end]))
            {
                --end;
            }
            int substringLength = end - begin + 1;
            if (substringLength < s.Length)
            {
                s = (substringLength > 0)
                    ? s.Substring(begin, substringLength)
                    : string.Empty;
            }

            // 4. If input is an ASCII case-insensitive match for the string "transparent", then return an error.
            if (StringUtil.EqualsIgnoreCase(s, "transparent"))
            {
                return null;
            }

            // 5. If input is an ASCII case-insensitive match for one of the keywords listed in the SVG color keywords section
            // of the CSS3 Color specification, then return the simple color corresponding to that keyword.
            DrColor knownColor = DrKnownColors.FromName(s);
            if (knownColor != DrColor.Empty)
            {
                return new CssIdentifierValue(StringUtil.AsciiLowerCase(s));
            }

            // 6. If input is four characters long, and the first character in input is a "#" (U+0023) character, 
            // and the last three characters of input are all in the range ASCII digits, U+0041 LATIN CAPITAL LETTER A 
            // to U+0046 LATIN CAPITAL LETTER F, and U+0061 LATIN SMALL LETTER A to U+0066 LATIN SMALL LETTER F, 
            // then run these substeps:
            bool isHexColorCode =
                (s.Length == 4) &&
                StringUtil.IsHexDigit(s[1]) &&
                StringUtil.IsHexDigit(s[2]) &&
                StringUtil.IsHexDigit(s[3]);
            if (isHexColorCode)
            {
                // 1. Let result be a simple color.

                // 2. Interpret the second character of input as a hexadecimal digit.
                // Let the red component of result be the resulting number multiplied by 17.
                int r = StringUtil.HexCharToDigit(s[1]) * 17;

                // 3. Interpret the third character of input as a hexadecimal digit. 
                // Let the green component of result be the resulting number multiplied by 17.
                int g = StringUtil.HexCharToDigit(s[2]) * 17;

                // 4. Interpret the fourth character of input as a hexadecimal digit. 
                // let the blue component of result be the resulting number multiplied by 17.
                int b = StringUtil.HexCharToDigit(s[3]) * 17;

                // 5. Return result.
                return CssHashValue.FromColor(r, g, b);
            }

            // 7. Replace any characters in input that have a Unicode code point greater than U+FFFF 
            // (i.e. any characters that are not in the basic multilingual plane) with the two-character string "00".

            // Characters from supplementary Unicode planes are not supported at the moment.
            // Surrogate characters will be replaced with zero characters at step 10.

            // 8. If input is longer than 128 characters, truncate input, leaving only the first 128 characters.
            StringBuilder sb = new StringBuilder(s);
            if (sb.Length > 128)
            {
                sb.Length = 128;
            }

            // 9. If the first character in input is a "#" (U+0023) character, remove it.
            if ((sb.Length > 0) && (sb[0] == '\x0023'))
            {
                sb.Remove(0, 1);
            }

            // 10. Replace any character in input that is not in the range ASCII digits, 
            // U+0041 LATIN CAPITAL LETTER A to U+0046 LATIN CAPITAL LETTER F, 
            // and U+0061 LATIN SMALL LETTER A to U+0066 LATIN SMALL LETTER F with the character "0" (U+0030).
            for (int i = 0; i < sb.Length; i++)
            {
                if (!StringUtil.IsHexDigit(sb[i]))
                {
                    sb[i] = '\x0030';
                }
            }

            // 11. While input's length is zero or not a multiple of three, append a "0" (U+0030) character to input.
            while ((sb.Length == 0) || ((sb.Length % 3) != 0))
            {
                sb.Append('\x0030');
            }

            // 12. Split input into three strings of equal length, to obtain three components. Let length be the length 
            // of those components (one third the length of input).
            int length = sb.Length / 3;

            // 13. If length is greater than 8, then remove the leading length-8 characters in each component, 
            // and let length be 8.
            if (length > 8)
            {
                length = 8;
            }

            // 14. While length is greater than two and the first character in each component is a "0" (U+0030) character,
            // remove that character and reduce length by one.
            while (length > 2)
            {
                bool firstCharacterIsZero = true;
                for (int i = 0; i < 3; i++)
                {
                    int offset = sb.Length / 3 * (i + 1) - length;
                    if (sb[offset] != '\x0030')
                    {
                        firstCharacterIsZero = false;
                        break;
                    }
                }
                if (firstCharacterIsZero)
                {
                    --length;
                }
                else
                {
                    break;
                }
            }

            // 15. If length is still greater than two, truncate each component, leaving only the first two characters in each.
            // The value 0xFF will be shifted left to the alpha component.
            int argb = 0xFF;
            for (int i = 0; i < 3; i++)
            {
                // 16. Let result be a simple color.
                // For red, green, and blue component:
                // 17-19. Interpret the component as a hexadecimal number. 
                // Let the corresponding component of result be the resulting number.
                int offset = sb.Length / 3 * (i + 1) - length;
                if (length == 1)
                {
                    // The component consists only of a low-order digit.
                    argb <<= 8;
                    argb |= StringUtil.HexCharToDigit(sb[offset]);
                }
                else
                {
                    // Process only two characters in each component (one hexadecimal byte).
                    argb <<= 4;
                    argb |= StringUtil.HexCharToDigit(sb[offset]);
                    argb <<= 4;
                    argb |= StringUtil.HexCharToDigit(sb[offset + 1]);
                }
            }

            // 20. Return result.
            return CssHashValue.FromColor(argb);
        }

        /// <summary>
        /// Parses a font size value according to the HTML 5 rules for legacy font sizes.
        /// </summary>
        /// <param name="s">The font size value to be parsed.</param>
        /// <returns>
        /// The parsed font size value, if parsing was successful;
        /// <c>null</c> otherwise.
        /// </returns>
        /// <remarks>
        /// The algorithm is taken from https://html.spec.whatwg.org/multipage/rendering.html#rules-for-parsing-a-legacy-font-size
        /// </remarks>
        internal static CssValue ParseLegacyFontSize(string s)
        {
            if (s == null)
                return null;

            // 1. Let input be the attribute's value.
            // 2. Let position be a pointer into input, initially pointing at the start of the string.
            int i = 0;

            // 3. Skip whitespace.
            while ((i < s.Length) && HtmlUtil.IsWhitespace(s[i]))
            {
                ++i;
            }

            // 4. If position is past the end of input, there is no presentational hint. Abort these steps.
            if (i >= s.Length)
            {
                return null;
            }

            // 5. If the character at position is a "+" (U+002B) character, then let mode be relative-plus, and advance position
            // to the next character. Otherwise, if the character at position is a "-" (U+002D) character, then let mode
            // be relative-minus, and advance position to the next character. Otherwise, let mode be absolute.
            int mode = 0;
            switch (s[i])
            {
                case '\x002B':
                    mode = 1;
                    ++i;
                    break;
                case '\x002D':
                    mode = -1;
                    ++i;
                    break;
                default:
                    break;
            }

            // 6. Collect a sequence of characters in the range ASCII digits, and let the resulting sequence be digits.
            // 8. Interpret digits as a base-ten integer. Let value be the resulting number.
            int oldPosition = i;
            int value = 0;
            while ((i < s.Length) && StringUtil.IsDigit(s[i]))
            {
                value *= 10;
                int digit = s[i] - '0';
                value += digit;
                ++i;

                // There is no need to read all digits, if the value is already known to be greater than the maximum.
                bool overflow = value > 10;
                if (overflow)
                {
                    break;
                }
            }

            // 7. If digits is the empty string, there is no presentational hint. Abort these steps.
            if (i == oldPosition)
            {
                return null;
            }

            // 9. If mode is relative-plus, then increment value by 3. 
            // If mode is relative-minus, then let value be the result of subtracting value from 3.
            if (mode > 0)
            {
                value += 3;
            }
            else if (mode < 0)
            {
                value = 3 - value;
            }

            // 10. If value is greater than 7, let it be 7.
            if (value > 7)
            {
                value = 7;
            }

            // 11. If value is less than 1, let it be 1.
            if (value < 1)
            {
                value = 1;
            }

            // 12. Set 'font-size' to the keyword corresponding to the value of value according to the following table:
            switch (value)
            {
                case 1:
                    return XSmall;
                case 2:
                    return Small;
                case 3:
                    return Medium;
                case 4:
                    return Large;
                case 5:
                    return XLarge;
                case 6:
                    return XxLarge;
                case 7:
                    return XxxLarge;
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        /// <summary>
        /// Determines whether the specified CSS value is equal to the current value.
        /// </summary>
        internal bool Equals(CssValue other)
        {
            // Standard reference comparisons.
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (ValueType != other.ValueType)
                return false;

            return DoEquals(other);
        }

        /// <summary>
        /// Determines whether the specified CSS value is equal to any specified value.
        /// </summary>
        internal bool EqualsAny(params CssValue[] other)
        {
            foreach (CssValue otherValue in other)
            {
                if (Equals(otherValue))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a CSS representation of the property value.
        /// </summary>
        internal string ToCss()
        {
            StringBuilder sb = new StringBuilder();
            ToCss(sb);
            return sb.ToString();
        }

        /// <summary>
        /// Outputs a CSS representation of the property value to StringBuilder.
        /// </summary>
        internal abstract void ToCss(StringBuilder sb);

        /// <summary>
        /// Returns a value indicating whether this CSS value is a valid length. Valid lengths are: number + unit, zero, and
        /// (in quirks mode) numbers without units.
        /// </summary>
        internal bool IsLength(bool allowUnitlessNumbers)
        {
            return (ValueType == CssValueType.Length) ||
                (allowUnitlessNumbers && (ValueType == CssValueType.Number)) ||
                Equals((CssValue)Zero); // Cast is needed for autoporting.
        }

        /// <summary>
        /// Converts this value to length, if possible.
        /// </summary>
        /// <param name="allowUnitlessNumbers">
        /// A flag indicating whether unitless numbers are accepted. Such values are treated as lengths in pixels. That is,
        /// "123" is converted to "123px".
        /// </param>
        /// <remarks>
        /// This method helps to process different representations of lengths allowed in CSS: normal lengths, unitless lengths,
        /// zero.
        /// Note that the unitless zero value is always accepted and is converted to zero points. That is, "0" is always
        /// converted to "0pt".
        /// </remarks>
        /// <returns>
        /// The converted length value or <c>null</c> if conversion is not possible.
        /// </returns>
        internal CssLengthValue ToLength(bool allowUnitlessNumbers)
        {
            if (ValueType == CssValueType.Length)
            {
                return (CssLengthValue)this;
            }

            if (ValueType == CssValueType.Number)
            {
                CssNumberValue numberValue = (CssNumberValue)this;
                if (numberValue.DoubleValue == 0)
                {
                    return ZeroPt;
                }

                if (allowUnitlessNumbers)
                {
                    return new CssLengthValue(numberValue.DoubleValue, CssUnit.Px);
                }
            }

            return null;
        }

        /// <summary>
        /// Tries to parse the CSS value as color.
        /// </summary>
        /// <returns>
        /// The parsed color value. If the CSS value doesn't represent a valid color, <c>null</c> value is returned.
        /// </returns>
        internal virtual DrColor ParseAsColor()
        {
            // Most types of CSS values cannot represent a color value.
            return null;
        }

        /// <summary>
        /// Determines whether the specified CSS value is equal to the current value.
        /// Override for class-specific implementation. Reference and null checks aren't needed.
        /// </summary>
        protected virtual bool DoEquals(CssValue other)
        {
            return mValue.Equals(other.mValue);
        }

        /// <summary>
        /// Value object.
        /// </summary>
        internal object Value
        {
            get { return mValue; }
        }

        /// <summary>
        /// Gets the value as a number. If the value cannot be converted into a number, returns <see cref="double.MinValue"/>.
        /// </summary>
        /// <remarks>
        /// This getter is expected to be overridden by subclasses that implement numeric values.
        /// </remarks>
        internal virtual double DoubleValue
        {
            get { return double.MinValue; }
        }

        /// <summary>
        /// Value type.
        /// </summary>
        internal CssValueType ValueType
        {
            get { return mValueType; }
        }

        internal static CssCommaValue Comma = new CssCommaValue();
        // Represents a U+002F SOLIDUS (/) value.
        internal static CssSolidusValue Solidus = new CssSolidusValue();

        internal static CssIdentifierValue Transparent = new CssIdentifierValue("transparent");
        internal static CssIdentifierValue None = new CssIdentifierValue("none");
        internal static CssIdentifierValue Nil = new CssIdentifierValue("nil");
        /// <summary>
        /// The 'inherit' value can be used to enforce inheritance of values, and it can also be used on properties that 
        /// are not normally inherited.
        /// </summary>
        internal static CssIdentifierValue Inherit = new CssIdentifierValue("inherit");
        /// <summary>
        /// The initial CSS keyword applies the initial value of a property to an element. 
        /// It is allowed on every CSS property and causes the element for which it is specified 
        /// to use the initial value of the property.
        /// </summary>
        internal static CssValue Initial = new CssIdentifierValue("initial");

        internal static CssNumberValue Zero = new CssNumberValue(0);
        internal static CssLengthValue ZeroPt = new CssLengthValue(0, CssUnit.Pt);
        internal static CssPercentageValue ZeroPercentage = new CssPercentageValue(0);

        internal static CssLengthValue OnePx = new CssLengthValue(1, CssUnit.Px);

        internal static CssIdentifierValue Ltr = new CssIdentifierValue("ltr");
        internal static CssIdentifierValue Rtl = new CssIdentifierValue("rtl");
        internal static CssIdentifierValue Auto = new CssIdentifierValue("auto");
        internal static CssIdentifierValue Avoid = new CssIdentifierValue("avoid");
        internal static CssIdentifierValue Capitalize = new CssIdentifierValue("capitalize");
        internal static CssIdentifierValue LowerCase = new CssIdentifierValue("lowercase");
        internal static CssIdentifierValue UpperCase = new CssIdentifierValue("uppercase");
        internal static CssIdentifierValue Normal = new CssIdentifierValue("normal");
        internal static CssIdentifierValue Underline = new CssIdentifierValue("underline");
        internal static CssIdentifierValue Overline = new CssIdentifierValue("overline");
        internal static CssIdentifierValue LineThrough = new CssIdentifierValue("line-through");
        internal static CssIdentifierValue Blink = new CssIdentifierValue("blink");
        internal static CssIdentifierValue SmallCaps = new CssIdentifierValue("small-caps");
        internal static CssIdentifierValue Italic = new CssIdentifierValue("italic");
        internal static CssIdentifierValue Oblique = new CssIdentifierValue("oblique");
        internal static CssIdentifierValue Bold = new CssIdentifierValue("bold");
        internal static CssIdentifierValue Bolder = new CssIdentifierValue("bolder");
        internal static CssIdentifierValue Lighter = new CssIdentifierValue("lighter");
        internal static CssIdentifierValue Char = new CssIdentifierValue("char");
        internal static CssIdentifierValue Always = new CssIdentifierValue("always");
        internal static CssIdentifierValue Top = new CssIdentifierValue("top");
        internal static CssIdentifierValue Bottom = new CssIdentifierValue("bottom");
        internal static CssIdentifierValue Middle = new CssIdentifierValue("middle");
        internal static CssIdentifierValue Start = new CssIdentifierValue("start");
        internal static CssIdentifierValue End = new CssIdentifierValue("end");
        internal static CssIdentifierValue Baseline = new CssIdentifierValue("baseline");
        internal static CssIdentifierValue Sub = new CssIdentifierValue("sub");
        internal static CssIdentifierValue Super = new CssIdentifierValue("super");
        internal static CssIdentifierValue TextBottom = new CssIdentifierValue("text-bottom");
        internal static CssIdentifierValue Repeat = new CssIdentifierValue("repeat");
        internal static CssIdentifierValue RepeatX = new CssIdentifierValue("repeat-x");
        internal static CssIdentifierValue RepeatY = new CssIdentifierValue("repeat-y");
        internal static CssIdentifierValue NoRepeat = new CssIdentifierValue("no-repeat");
        internal static CssIdentifierValue Scroll = new CssIdentifierValue("scroll");
        internal static CssIdentifierValue Fixed = new CssIdentifierValue("fixed");
        internal static CssIdentifierValue Embed = new CssIdentifierValue("embed");
        internal static CssIdentifierValue BidiOverride = new CssIdentifierValue("bidi-override");
        // alignment
        internal static CssIdentifierValue Left = new CssIdentifierValue("left");
        internal static CssIdentifierValue Right = new CssIdentifierValue("right");
        internal static CssIdentifierValue Center = new CssIdentifierValue("center");
        internal static CssIdentifierValue Justify = new CssIdentifierValue("justify");
        internal static CssIdentifierValue DistributeLetter = new CssIdentifierValue("distribute-letter");
        internal static CssIdentifierValue DistributeSpace = new CssIdentifierValue("distribute-space");
        internal static CssIdentifierValue LineEdge = new CssIdentifierValue("line-edge");
        // custom alignment
        // 'start' is treated as 'left' on left-to-right elements, and as 'right' on right-to-left.
        internal static CssIdentifierValue AwStart = new CssIdentifierValue("-aw-start");
        // This value behaves the same as 'inherit' except that an inherited '-aw-start' is interpreted against the parent's
        // 'direction' value and results in a computed value of either 'left' or 'right'.
        // See http://www.w3.org/TR/css-text-3/#text-align-property 
        internal static CssIdentifierValue AwMatchParent = new CssIdentifierValue("-aw-match-parent");
        // The following three custom alignment values are similar to the standard ones, except that they affect not only
        // inline-level but also block-level descendant elements. They represent values of the 'align' attribute in CSS.
        internal static CssIdentifierValue AwLeft = new CssIdentifierValue("-aw-left");
        internal static CssIdentifierValue AwRight = new CssIdentifierValue("-aw-right");
        internal static CssIdentifierValue AwCenter = new CssIdentifierValue("-aw-center");
        // display
        internal static CssIdentifierValue Inline = new CssIdentifierValue("inline");
        internal static CssIdentifierValue InlineBlock = new CssIdentifierValue("inline-block");
        internal static CssIdentifierValue Block = new CssIdentifierValue("block");
        internal static CssIdentifierValue ListItem = new CssIdentifierValue("list-item");

        internal static CssIdentifierValue RunIn = new CssIdentifierValue("run-in");
        internal static CssIdentifierValue InlineTable = new CssIdentifierValue("inline-table");
        internal static CssIdentifierValue Table = new CssIdentifierValue("table");
        internal static CssIdentifierValue TableRowGroup = new CssIdentifierValue("table-row-group");
        internal static CssIdentifierValue TableHeaderGroup = new CssIdentifierValue("table-header-group");
        internal static CssIdentifierValue TableFooterGroup = new CssIdentifierValue("table-footer-group");
        internal static CssIdentifierValue TableRow = new CssIdentifierValue("table-row");
        internal static CssIdentifierValue TableColumnGroup = new CssIdentifierValue("table-column-group");
        internal static CssIdentifierValue TableColumn = new CssIdentifierValue("table-column");
        internal static CssIdentifierValue TableCell = new CssIdentifierValue("table-cell");
        internal static CssIdentifierValue TableCaption = new CssIdentifierValue("table-caption");
        // visibility
        internal static CssIdentifierValue Visible = new CssIdentifierValue("visible");
        //
        internal static CssIdentifierValue Decimal = new CssIdentifierValue("decimal");
        internal static CssIdentifierValue DecimalLeadingZero = new CssIdentifierValue("decimal-leading-zero");
        internal static CssIdentifierValue LowerAlpha = new CssIdentifierValue("lower-alpha");
        internal static CssIdentifierValue UpperAlpha = new CssIdentifierValue("upper-alpha");
        internal static CssIdentifierValue LowerRoman = new CssIdentifierValue("lower-roman");
        internal static CssIdentifierValue UpperRoman = new CssIdentifierValue("upper-roman");
        internal static CssIdentifierValue LowerGreek = new CssIdentifierValue("lower-greek");
        internal static CssIdentifierValue LowerLatin = new CssIdentifierValue("lower-latin");
        internal static CssIdentifierValue UpperLatin = new CssIdentifierValue("upper-latin");
        internal static CssIdentifierValue Armenian = new CssIdentifierValue("armenian");
        internal static CssIdentifierValue Georgian = new CssIdentifierValue("georgian");
        internal static CssIdentifierValue ArabicIndic = new CssIdentifierValue("arabic-indic");
        internal static CssIdentifierValue Disc = new CssIdentifierValue("disc");
        internal static CssIdentifierValue Circle = new CssIdentifierValue("circle");
        internal static CssIdentifierValue Square = new CssIdentifierValue("square");
        internal static CssIdentifierValue Collapse = new CssIdentifierValue("collapse");
        internal static CssIdentifierValue Separate = new CssIdentifierValue("separate");
        internal static CssIdentifierValue Nowrap = new CssIdentifierValue("nowrap");
        internal static CssIdentifierValue PreWrap = new CssIdentifierValue("pre-wrap");
        internal static CssIdentifierValue PreLine = new CssIdentifierValue("pre-line");
        internal static CssIdentifierValue XxSmall = new CssIdentifierValue("xx-small");
        internal static CssIdentifierValue XSmall = new CssIdentifierValue("x-small");
        internal static CssIdentifierValue Small = new CssIdentifierValue("small");
        internal static CssIdentifierValue Large = new CssIdentifierValue("large");
        internal static CssIdentifierValue XLarge = new CssIdentifierValue("x-large");
        internal static CssIdentifierValue XxLarge = new CssIdentifierValue("xx-large");
        // The 'xxx-large' value is a non-CSS value used to indicate a font size 50% larger than 'xx-large'.
        // See https://html.spec.whatwg.org/multipage/rendering.html#rules-for-parsing-a-legacy-font-size
        // Maybe this property should be named something like "-aw-xxx-large", as it is non-standard.
        internal static CssIdentifierValue XxxLarge = new CssIdentifierValue("xxx-large");
        internal static CssIdentifierValue Both = new CssIdentifierValue("both");
        internal static CssIdentifierValue TextTop = new CssIdentifierValue("text-top");
        internal static CssIdentifierValue Pre = new CssIdentifierValue("pre");
        // Relative size
        internal static CssIdentifierValue Smaller = new CssIdentifierValue("smaller");
        internal static CssIdentifierValue Larger = new CssIdentifierValue("larger");

        internal static CssIdentifierValue LrTb = new CssIdentifierValue("lr-tb");
        internal static CssIdentifierValue RlTb = new CssIdentifierValue("rl-tb");
        internal static CssIdentifierValue TbRl = new CssIdentifierValue("tb-rl");
        internal static CssIdentifierValue BtRl = new CssIdentifierValue("bt-rl");
        internal static CssIdentifierValue TbLr = new CssIdentifierValue("tb-lr");
        internal static CssIdentifierValue BtLr = new CssIdentifierValue("bt-lr");
        internal static CssIdentifierValue HorizontalTb = new CssIdentifierValue("horizontal-tb");
        internal static CssIdentifierValue VerticalRl = new CssIdentifierValue("vertical-rl");
        internal static CssIdentifierValue VerticalLr = new CssIdentifierValue("vertical-lr");
        internal static CssIdentifierValue Medium = new CssIdentifierValue("medium");
        internal static CssIdentifierValue Thin = new CssIdentifierValue("thin");
        internal static CssIdentifierValue Thick = new CssIdentifierValue("thick");
        // border styles
        internal static CssIdentifierValue Hidden = new CssIdentifierValue("hidden");
        internal static CssIdentifierValue Dotted = new CssIdentifierValue("dotted");
        internal static CssIdentifierValue Dashed = new CssIdentifierValue("dashed");
        internal static CssIdentifierValue Solid = new CssIdentifierValue("solid");
        internal static CssIdentifierValue DoubleId = new CssIdentifierValue("double");//JAVA-changed name since (old) Double clashes with Double.MinValue in Java.
        internal static CssIdentifierValue Groove = new CssIdentifierValue("groove");
        internal static CssIdentifierValue Ridge = new CssIdentifierValue("ridge");
        internal static CssIdentifierValue Inset = new CssIdentifierValue("inset");
        internal static CssIdentifierValue Outset = new CssIdentifierValue("outset");
        // Custom border styles that are used to preserve MS Word-specific line styles during HTML round-trip.
        internal static CssIdentifierValue Single = new CssIdentifierValue("single");
        internal static CssIdentifierValue Hairline = new CssIdentifierValue("hairline");
        internal static CssIdentifierValue Dot = new CssIdentifierValue("dot");
        internal static CssIdentifierValue DashLargeGap = new CssIdentifierValue("dash-large-gap");
        internal static CssIdentifierValue DotDash = new CssIdentifierValue("dot-dash");
        internal static CssIdentifierValue DotDotDash = new CssIdentifierValue("dot-dot-dash");
        internal static CssIdentifierValue Triple = new CssIdentifierValue("triple");
        internal static CssIdentifierValue ThinThickSmallGap = new CssIdentifierValue("thin-thick-small-gap");
        internal static CssIdentifierValue ThickThinSmallGap = new CssIdentifierValue("thick-thin-small-gap");
        internal static CssIdentifierValue ThinThickThinSmallGap = new CssIdentifierValue("thin-thick-thin-small-gap");
        internal static CssIdentifierValue ThinThickMediumGap = new CssIdentifierValue("thin-thick-medium-gap");
        internal static CssIdentifierValue ThickThinMediumGap = new CssIdentifierValue("thick-thin-medium-gap");
        internal static CssIdentifierValue ThinThickThinMediumGap = new CssIdentifierValue("thin-thick-thin-medium-gap");
        internal static CssIdentifierValue ThinThickLargeGap = new CssIdentifierValue("thin-thick-large-gap");
        internal static CssIdentifierValue ThickThinLargeGap = new CssIdentifierValue("thick-thin-large-gap");
        internal static CssIdentifierValue ThinThickThinLargeGap = new CssIdentifierValue("thin-thick-thin-large-gap");
        internal static CssIdentifierValue Wave = new CssIdentifierValue("wave");
        internal static CssIdentifierValue DoubleWave = new CssIdentifierValue("double-wave");
        internal static CssIdentifierValue DashSmallGap = new CssIdentifierValue("dash-small-gap");
        internal static CssIdentifierValue DashDotStroker = new CssIdentifierValue("dash-dot-stroker");
        internal static CssIdentifierValue Emboss3D = new CssIdentifierValue("emboss3d");
        internal static CssIdentifierValue Engrave3D = new CssIdentifierValue("engrave3d");
        // font
        internal static CssIdentifierValue Caption = new CssIdentifierValue("caption");
        internal static CssIdentifierValue Icon = new CssIdentifierValue("icon");
        internal static CssIdentifierValue Menu = new CssIdentifierValue("menu");
        internal static CssIdentifierValue MessageBox = new CssIdentifierValue("message-box");
        internal static CssIdentifierValue SmallCaption = new CssIdentifierValue("small-caption");
        internal static CssIdentifierValue StatusBar = new CssIdentifierValue("status-bar");
        // 'box-sizing' property values (see http://www.w3.org/TR/css3-ui/#box-sizing)
        internal static CssIdentifierValue ContentBox = new CssIdentifierValue("content-box");
        internal static CssIdentifierValue PaddingBox = new CssIdentifierValue("padding-box");
        internal static CssIdentifierValue BorderBox = new CssIdentifierValue("border-box");
        // font-family
        internal static CssIdentifierValue Monospace = new CssIdentifierValue("monospace");
        // Lists
        internal static CssIdentifierValue Inside = new CssIdentifierValue("inside");
        internal static CssIdentifierValue Outside = new CssIdentifierValue("outside");
        // Images
        internal static CssIdentifierValue Absolute = new CssIdentifierValue("absolute");
        // Quotes (used in the 'content' property).
        internal static CssIdentifierValue OpenQuote = new CssIdentifierValue("open-quote");
        internal static CssIdentifierValue CloseQuote = new CssIdentifierValue("close-quote");
        internal static CssIdentifierValue NoOpenQuote = new CssIdentifierValue("no-open-quote");
        internal static CssIdentifierValue NoCloseQuote = new CssIdentifierValue("no-close-quote");
        // Color
        internal static CssHashValue Black = CssHashValue.FromColor(DrColor.Black);
        internal static CssHashValue Gray = CssHashValue.FromColor(DrColor.Gray);

        // "-aw-import" values.
        internal static CssIdentifierValue Ignore = new CssIdentifierValue("ignore");
        internal static CssIdentifierValue Spaces = new CssIdentifierValue("spaces");

        // SVG
        internal static CssIdentifierValue CurrentColor = new CssIdentifierValue("currentcolor");
        // stroke-linecap
        internal static CssIdentifierValue Butt = new CssIdentifierValue("butt");
        internal static CssIdentifierValue Round = new CssIdentifierValue("round");
        // stroke-linejoin
        internal static CssIdentifierValue Miter = new CssIdentifierValue("miter");
        internal static CssIdentifierValue Bevel = new CssIdentifierValue("bevel");
        // fill-rule
        internal static CssIdentifierValue Nonzero = new CssIdentifierValue("nonzero");
        internal static CssIdentifierValue Evenodd = new CssIdentifierValue("evenodd");
        // dominant-baseline
        internal static CssIdentifierValue Central = new CssIdentifierValue("central");
        internal static CssIdentifierValue Ideographic = new CssIdentifierValue("ideographic");
        internal static CssIdentifierValue Mathematical = new CssIdentifierValue("mathematical");
        internal static CssIdentifierValue Alphabetic = new CssIdentifierValue("alphabetic");
        internal static CssIdentifierValue Hanging = new CssIdentifierValue("hanging");

        // MsoHtml
        internal static CssIdentifierValue Yes = new CssIdentifierValue("yes");
        internal static CssIdentifierValue No = new CssIdentifierValue("no");
        internal static CssIdentifierValue Landscape = new CssIdentifierValue("landscape");
        internal static CssIdentifierValue Exactly = new CssIdentifierValue("exactly");
        internal static CssIdentifierValue WidowOrphan = new CssIdentifierValue("widow-orphan");
        internal static CssIdentifierValue LinesTogether = new CssIdentifierValue("lines-together");
        internal static CssIdentifierValue NoLineNumbers = new CssIdentifierValue("no-line-numbers");
        internal static CssIdentifierValue BodyText = new CssIdentifierValue("body-text");

        /// <summary>
        /// string, Color, double or CssValueCollection depending on the type of the value.
        /// </summary>
        private readonly object mValue;
        private readonly CssValueType mValueType;
    }
}
