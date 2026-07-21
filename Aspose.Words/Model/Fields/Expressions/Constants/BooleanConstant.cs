// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a constant whose value is true or false.
    /// </summary>
    internal class BooleanConstant : Constant
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="value"></param>
        internal BooleanConstant(bool value)
        {
            mValue = value;
        }

        internal override double ValueDouble
        {
            get { return mValue ? 1d : 0d; }
        }

        internal override bool ValueBoolean
        {
            get { return mValue; }
        }

        internal override string ValueString
        {
            get { return mValue ? "1" : "0"; }
        }

        internal override ConstantType ConstantType
        {
            get { return ConstantType.Boolean; }
        }

        private readonly bool mValue;
    }
}
