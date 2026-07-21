// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/03/2011 by Denis Darkin

using System;

namespace Aspose.Words.Math
{
    /// <summary>
    /// Acts as a common container for different argument types in Office Math.
    /// The most generally used one is <see cref="Words.Math.MathObjectType.Argument"/>.
    /// Others are used occasionally as special arguments for other Office Math objects.
    /// <example>
    /// - <see cref="Words.Math.MathObjectType.Denominator"/> and <see cref="Words.Math.MathObjectType.Numerator"/> are
    /// the only special arguments that <see cref="Words.Math.MathObjectType.Fraction"/> can contain.
    /// </example>
    /// </summary>
    /// <remarks>
    /// Initially there were individual classes for each specific argument type.
    /// But it showed to be overkill: they didn't have any properties and were used in a similar fashion.
    /// So to reduce complexity, DD has mereged them into this.
    /// </remarks>
    internal class MathObjectArgumentBase : MathObject
    {
        internal MathObjectArgumentBase(MathObjectType type)
        {
            switch (type)
            {
                case MathObjectType.Argument:
                case MathObjectType.Degree:
                case MathObjectType.Denominator:
                case MathObjectType.FunctionName:
                case MathObjectType.Limit:
                case MathObjectType.Numerator:
                case MathObjectType.SubscriptPart:
                case MathObjectType.SuperscriptPart:
                    mMathObjectType = type;
                    break;
                default:
                    throw new InvalidOperationException("Please report exception");
            }
        }

        internal override bool CanInsert(Node node)
        {
            return CanBeMathArgument(node);
        }

        /// <summary>
        /// Specifies the size, or script level, of an argument.
        /// The default argument size is 0. Valid range is (-2, 2) inclusive.
        /// </summary>
        /// <remarks>
        /// This is the only property in math object that is not implemented throught <see cref="MathAttr"/>.
        /// The reason for it is that it is not stored in m:ctrlPr, it has its own singular pr containing only m:argSz el.
        /// </remarks>
        internal int ArgumentSize
        {
            get { return mArgSize; }
            set
            {
                int fixedValue = (value < -2) ? -2 : (value > 2) ? 2 : value;
                if (fixedValue != DefaultArgumentSize)
                    mArgSize = fixedValue;
            }
        }

        internal override bool NeedsMathObjectArgumentWrapper
        {
            get { return false; }
        }

        internal override bool CanBeArgument
        {
            get { return false; }
        }

        internal override MathObjectType MathObjectType
        {
            get { return mMathObjectType; }
        }

        private readonly MathObjectType mMathObjectType;

        private int mArgSize = DefaultArgumentSize;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int DefaultArgumentSize = 0;
    }
}
