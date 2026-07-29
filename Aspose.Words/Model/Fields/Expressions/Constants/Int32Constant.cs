// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/03/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a constant whose value is an integer number.
    /// </summary>
    internal class Int32Constant : Constant
    {
        internal Int32Constant(int value)
        {
            mValue = value;
        }

        internal override FieldFormattingResult TryFormatNumber(RichString format, Field field)
        {
            return DoubleConstant.TryFormatNumber(mValue, format, field);
        }

        internal override bool TryConvertToDouble(out double value)
        {
            value = mValue;
            return true;
        }

        internal override double ValueDouble
        {
            get { return mValue; }
        }

        internal override bool ValueBoolean
        {
            get { return (mValue != 0); }
        }

        internal override string ValueString
        {
            get { return mValue.ToString(); }
        }

        internal override ConstantType ConstantType
        {
            get { return ConstantType.Int32; }
        }

        private readonly int mValue;
    }
}
