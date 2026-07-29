// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/01/2010 by Dmitry Vorobyev

using Aspose.Common;

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a base class for all functions whose argument is aggregate.
    /// </summary>
    internal abstract class AggregateFunction : Function
    {
        protected override Constant EvaluateCore(ConstantCollection parameters)
        {
            NullableDouble value = InitialValue;
            int count = 0;

            int numberOfDecimalPlaces = 0;

            foreach (Constant originalParameter in parameters)
            {
                Constant parameter = originalParameter.ConstantType != ConstantType.NullCellReference
                                     ? originalParameter
                                     : NullCellReferenceSubstitution;

                if (parameter.ConstantType == ConstantType.Null)
                {
                    if (CountNullValues)
                        count++;

                    continue;
                }

                count++;

                if (!(parameter is DoubleConstant))
                    return ErrorConstant.CreateSyntaxError();

                numberOfDecimalPlaces = ApplyNumberOfDecimalPlaces(numberOfDecimalPlaces, (DoubleConstant)parameter);
                value = Apply(value, parameter.ValueDouble);
            }

            value = FinalizeCalculation(value, count);
            if (value.HasValue)
                return DoubleConstant.CreateFrom(parameters, value.Value, numberOfDecimalPlaces);

            return NullConstant.Instance;
        }

        protected virtual NullableDouble Apply(NullableDouble currentValue, double parameterValue)
        {
            return currentValue;
        }

        protected virtual int ApplyNumberOfDecimalPlaces(int currentNumberOfDecimalPlaces, DoubleConstant parameter)
        {
            // If field format does not specified, MS Word uses maximum number of decimal places from arguments of most aggregate function for result.
            return System.Math.Max(currentNumberOfDecimalPlaces, parameter.NumberOfDigitsAfterDecimalPoint);
        }

        protected virtual NullableDouble FinalizeCalculation(NullableDouble currentValue, int parameterCount)
        {
            return currentValue;
        }

        protected virtual NullableDouble InitialValue
        {
            get { return new NullableDouble(0d); }
        }

        protected virtual Constant NullCellReferenceSubstitution
        {
            get { return NullConstant.Instance; }
        }

        protected virtual bool CountNullValues
        {
            get { return true; }
        }

        internal override bool IsAggregate
        {
            get { return true; }
        }

        protected override int ParameterCountMin
        {
            get { return 1; }
        }

        protected override int ParameterCountMax
        {
            get { return int.MaxValue; }
        }

        protected override bool IsParameterTypeValid(ConstantType constantType)
        {
            // Aggregate function self checks parameters types (see method EvaluateCore)
            return true;
        }
    }
}
