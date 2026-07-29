// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the delimiter object, consisting of opening and closing delimiters (such as parentheses,
    /// braces, brackets, and vertical bars), and an element contained inside. The delimiter may have more than one
    /// element, with a designated separator character between each element.
    /// </summary>
    internal class MathObjectDelimiter : MathObject
    {
        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.Delimiter; }
        }

        /// <summary>
        /// This element specifies the beginning, or opening, delimiter character. Mathematical delimiters are enclosing
        /// characters such as parentheses, brackets, and braces.
        /// Default: Unicode character U+0028 (LEFT PARENTHESIS).
        /// </summary>
        internal char BeginningCharacter
        {
            get { return (char)FetchAttr(MathAttr.BeginChar); }
            set { SetAttr(MathAttr.BeginChar, value, value != DefaultBeginningCharacter); }
        }
        
        /// <summary>
        /// specifies the ending, or closing, delimiter character. Mathematical delimiters are enclosing
        /// characters such as parentheses, brackets, and braces. 
        /// Default: Unicode character U+0029 (RIGHT PARENTHESIS).
        /// </summary>
        internal char EndingCharacter
        {
            get { return (char)FetchAttr(MathAttr.EndChar); }
            set { SetAttr(MathAttr.EndChar, value, value != DefaultEndingCharacter); }
        }
        
        /// <summary>
        /// Specifies the character that separates base arguments e in the delimiter object
        /// Default: Unicode character U+2502 (BOX DRAWINGS LIGHT VERTICAL).
        /// </summary>
        internal char SeparatorCharacter
        {
            get { return (char)FetchAttr(MathAttr.SeparatorChar); }
            set { SetAttr(MathAttr.SeparatorChar, value, value != DefaultSeparatorCharacter); }
        }

        /// <summary>
        /// Specifies the growth property of delimiter. 
        /// When false, delimiters do not grow to match the size of their operand height. 
        /// When true, the delimiter grows vertically to match its operand height. 
        /// Default: true.
        /// </summary>
        /// <remarks>
        /// 7.1.2.31 dPr (Delimiter Properties) describes that "grow (n-ary Grow)" (§7.1.2.43) is used. But default value of
        /// "grow (n-ary Grow)" is "false". MS Word violates specification requirements and uses "true" as default value.
        /// Repeat this oddity and use "true" if the direct attribute is not set.
        /// </remarks>
        internal bool GrowToMatchOperand
        {
            get { return (GetDirectAttr(MathAttr.GrowOperand) == null) || (bool)FetchAttr(MathAttr.GrowOperand); }
            set { SetAttr(MathAttr.GrowOperand, value, !value); }
        }
        
        /// <summary>
        /// Specifies the shape of delimiters in this delimiter object.
        /// Default: <see cref="MathDelimiterShape.Default"/>
        /// </summary>
        /// <remarks>
        /// MS Office will ignore this property is <see cref="GrowToMatchOperand"/> is false.
        /// </remarks>
        internal MathDelimiterShape DelimiterShape
        {
            get { return (MathDelimiterShape)FetchAttr(MathAttr.DelimiterShape); }
            set { SetAttr(MathAttr.DelimiterShape, value, value != MathDelimiterShape.Default); }
        }

        /// <summary>
        /// default = '('
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char DefaultBeginningCharacter = '\u0028';

        /// <summary>
        /// default = ')'
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char DefaultEndingCharacter = '\u0029';
        
        /// <summary>
        /// default = '|'
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char DefaultSeparatorCharacter = '\u2502';
    }
}
