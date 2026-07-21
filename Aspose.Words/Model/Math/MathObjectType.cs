// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/12/2010 by Denis Darkin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Math
{
    /// <summary>
    /// Specifies type of an Office Math object.
    /// </summary>
    [CppEnumEnableMetadata]
    public enum MathObjectType
    {
        /// <summary>
        /// Instance of mathematical text.
        /// </summary>
        OMath, 
        
        /// <summary>
        /// Math paragraph, or display math zone, that contains one or more <see cref="OMath"/> elements that are in display mode.
        /// </summary>
        OMathPara,

        /// <summary>
        /// Accent function, consisting of a base and a combining diacritical mark.
        /// </summary>
        Accent,
        
        /// <summary>
        /// Bar function, consisting of a base argument and an overbar or underbar.
        /// </summary>
        Bar,
        
        /// <summary>
        /// Border Box object, consisting of a border drawn around an instance of mathematical text (such as a formula or equation)
        /// </summary>
        BorderBox,
        
        /// <summary>
        /// Box object, which is used to group components of an equation or other instance of mathematical text.
        /// </summary>
        Box,
        
        /// <summary>
        /// Delimiter object, consisting of opening and closing delimiters (such as parentheses, 
        /// braces, brackets, and vertical bars), and an element contained inside.
        /// </summary>
        Delimiter,

        /// <summary>
        /// Degree in the mathematical radical.
        /// </summary>
        Degree,
        
        /// <summary>
        /// Argument object. Encloses Office Math entities when they are used as arguments to other Office Math entities.
        /// </summary>
        Argument,
        
        /// <summary>
        /// Array object, consisting of one or more equations, expressions, or other mathematical text runs 
        /// that can be vertically justified as a unit with respect to surrounding text on the line.
        /// </summary>
        Array,
        
        /// <summary>
        /// Fraction object, consisting of a numerator and denominator separated by a fraction bar.
        /// </summary>
        Fraction,
        
        /// <summary>
        /// Denominator of a fraction object.
        /// </summary>
        Denominator,
        
        /// <summary>
        /// Numerator of the Fraction object.
        /// </summary>
        Numerator,
        
        /// <summary>
        /// Function-Apply object, which consists of a function name and an argument element acted upon.
        /// </summary>
        Function,
        
        /// <summary>
        /// Name of the function. For example, function names are sin and cos.
        /// </summary>
        FunctionName,
        
        /// <summary>
        /// Group-Character object, consisting of a character drawn above or below text, often 
        /// with the purpose of visually grouping items
        /// </summary>
        GroupCharacter,
        
        /// <summary>
        /// Lower limit of the <see cref="LowerLimit"/> object and 
        /// the upper limit of the <see cref="UpperLimit"/> function.
        /// </summary>
        Limit,
        
        /// <summary>
        /// Lower-Limit object, consisting of text on the baseline and reduced-size text immediately below it.
        /// </summary>
        LowerLimit,
        
        /// <summary>
        /// Upper-Limit object, consisting of text on the baseline and reduced-size text immediately above it.
        /// </summary>
        UpperLimit,
        
        /// <summary>
        /// Matrix object, consisting of one or more elements laid out in one or more rows and one or more columns.
        /// </summary>
        Matrix,
        
        /// <summary>
        /// Single row of the matrix.
        /// </summary>
        MatrixRow,
        
        /// <summary>
        /// N-ary object, consisting of an n-ary object, a base (or operand), and optional upper and lower limits.
        /// </summary>
        NAry,
        
        /// <summary>
        /// Phantom object.
        /// </summary>
        Phantom,
        
        /// <summary>
        /// Radical object, consisting of a radical, a base element, and an optional degree .
        /// </summary>
        Radical,

        /// <summary>
        /// Subscript of the object that can have subscript part.
        /// </summary>
        SubscriptPart,

        /// <summary>
        /// Superscript of the superscript object.
        /// </summary>
        SuperscriptPart,
        
        /// <summary>
        /// Pre-Sub-Superscript object, which consists of a base element and a subscript and superscript placed to the left of the base.
        /// </summary>
        PreSubSuperscript,
        
        /// <summary>
        /// Subscript object, which consists of a base element and a reduced-size script placed below and to the right.
        /// </summary>
        Subscript,
        
        /// <summary>
        /// Sub-superscript object, which consists of a base element, a reduced-size script placed below and to the right, and a reduced-size script placed above and to the right.
        /// </summary>
        SubSuperscript,

        /// <summary>
        /// Superscript object, which consists of a base element and a reduced-size script placed above and to the right.
        /// </summary>
        Supercript
    }
}
