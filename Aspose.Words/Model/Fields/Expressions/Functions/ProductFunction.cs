// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2007 by Dmitry Vorobyev

using Aspose.Common;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a PRODUCT function.
    /// </summary>
    internal class ProductFunction : AggregateFunction
    {
        protected override NullableDouble Apply(NullableDouble currentValue, double parameterValue)
        {
            if (currentValue.HasValue)
                return new NullableDouble(currentValue.Value * parameterValue);

            return new NullableDouble(parameterValue);
        }

        protected override NullableDouble FinalizeCalculation(NullableDouble currentValue, int parameterCount)
        {
            if (currentValue.HasValue)
                return currentValue;

            return new NullableDouble(0d);
        }

        protected override NullableDouble InitialValue
        {
            get { return NullableDouble.Null; }
        }

        protected override Constant NullCellReferenceSubstitution
        {
            get { return new DoubleConstant(0d); }
        }
    }
}
