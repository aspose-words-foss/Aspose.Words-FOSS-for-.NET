// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2007 by Dmitry Vorobyev

using Aspose.Common;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents an AVERAGE function.
    /// </summary>
    internal class AverageFunction : AggregateFunction
    {
        protected override NullableDouble Apply(NullableDouble currentValue, double parameterValue)
        {
            return new NullableDouble(currentValue.GetValueOrDefault() + parameterValue);
        }

        protected override int ApplyNumberOfDecimalPlaces(int currentNumberOfDecimalPlaces, DoubleConstant parameter)
        {
            // If field format does not specified, MS Word uses maximum number of decimal places from arguments of AVERAGE function, but at least 2, for result.
            return System.Math.Max(base.ApplyNumberOfDecimalPlaces(currentNumberOfDecimalPlaces, parameter), 2);
        }

        protected override NullableDouble FinalizeCalculation(NullableDouble currentValue, int parameterCount)
        {
            if (!currentValue.HasValue)
                return NullableDouble.Null;

            return new NullableDouble(currentValue.GetValueOrDefault() / parameterCount);
        }

        protected override NullableDouble InitialValue
        {
            get { return NullableDouble.Null; }
        }

        protected override bool CountNullValues
        {
            get { return false; }
        }
    }
}
