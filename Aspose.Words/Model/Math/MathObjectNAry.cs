// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin
using Aspose.Words.Settings;

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies an n-ary object, consisting of an n-ary object, a base (or operand), 
    /// and optional upper and lower limits. 
    /// Examples of n-ary objects are integrals, sum signs etc.
    /// </summary>
    internal class MathObjectNAry : MathObject
    {
        internal override bool CanInsert(Node node)
        {
            if (base.CanInsert(node))
                return true;
                
            if ((node.NodeType == NodeType.OfficeMath))
            {
                // This object can host arguments and also special objects subscript part and
                // superscript part.
                MathObjectType type = ((OfficeMath)node).MathObject.MathObjectType;
                return (type == MathObjectType.SubscriptPart) || (type == MathObjectType.SuperscriptPart);
            }

            return false;
        }

        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.NAry; }
        }

        /// <summary>
        /// Specifies the character to be attached to this N-Ary object.
        /// Default is Unicode character U+222B (INTEGRAL)
        /// </summary>
        internal char Character
        {
            get { return (char)FetchAttr(MathAttr.NaryChar); }
            set { SetAttr(MathAttr.NaryChar, value, value != DefaultCharacter); }
        }

        /// <summary>
        /// Specifies the growth property of this n-ary operator. 
        /// When false, n-ary operators such as integrals and summations do not grow to match the size of their operand height. 
        /// When true, the operator grows vertically to match its operand height. 
        /// Default: false.
        /// </summary>
        internal bool GrowToMatchOperand
        {
            get { return (bool)FetchAttr(MathAttr.GrowOperand); }
            set { SetAttr(MathAttr.GrowOperand, value, value); }
        }
        
        /// <summary>
        /// Specifies the location of limits in n-ary operators.
        /// There is no default value, so if this value is not read from OOXML, then it's "default" is
        /// <see cref="MathLimitLocation.Undefined"/>, meaning that its value must be obtained from Office Math document-wide options
        /// <see cref="MathProperties.NaryLimitLocation"/> and <see cref="MathProperties.IntegralLimitLocation"/>
        /// </summary>
        internal MathLimitLocation LimitLocation
        {
            get { return (MathLimitLocation)FetchAttr(MathAttr.LimitLocation); }
            set { SetAttr(MathAttr.LimitLocation, value, value != MathLimitLocation.Undefined); }
        }
        
        /// <summary>
        /// Specifies the n-ary Hide Superscript property
        /// Default: false.
        /// </summary>
        internal bool IsHideSubscript
        {
            get { return (bool)FetchAttr(MathAttr.IsHideSubscript); }
            set { SetAttr(MathAttr.IsHideSubscript, value, value); }
        }
        
        /// <summary>
        /// Specifies the n-ary Hide Subscript property.
        /// Default: false.
        /// </summary>
        internal bool IsHideSuperscript
        {
            get { return (bool)FetchAttr(MathAttr.IsHideSuperscript); }
            set { SetAttr(MathAttr.IsHideSuperscript, value, value); }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char DefaultCharacter = '\u222B';
    }
}
