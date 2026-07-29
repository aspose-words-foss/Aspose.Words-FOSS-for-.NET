// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2010 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// This element specifies the Group-Character object, consisting of a character drawn above or below text, often
    /// with the purpose of visually grouping items.
    /// </summary>
    internal class MathObjectGroupCharacter : MathObject
    {
        internal override MathObjectType MathObjectType
        {
            get { return MathObjectType.GroupCharacter; }
        }

        /// <summary>
        /// Specifies the character to be attached to this group char object.
        /// Default: U+23DF (BOTTOM CURLY BRACKET).
        /// </summary>
        internal char Character
        {
            get { return (char)FetchAttr(MathAttr.GroupChar); }
            set { SetAttr(MathAttr.GroupChar, value, value != DefaultCharacter);}
        }
        
        /// <summary>
        /// Specifies the position of the bar or group character in the parent object.
        /// Default: <see cref="MathPosition.Default"/>
        /// </summary>
        internal MathPosition Position
        {
            get { return (MathPosition)FetchAttr(MathAttr.GroupCharPosition); }
            set { SetAttr(MathAttr.GroupCharPosition, value, value != MathPosition.Default); }
        }

        /// <summary>
        /// Specifies the vertical layout of the groupChr object.
        /// Default: <see cref="MathVerticalJustification.Default"/>
        /// </summary>
        internal MathVerticalJustification VerticalJustification
        {
            get { return (MathVerticalJustification)FetchAttr(MathAttr.VerticalJustification); }
            set { SetAttr(MathAttr.VerticalJustification, value, value != MathVerticalJustification.Default); }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const char DefaultCharacter = '\u23DF';
    }
}
