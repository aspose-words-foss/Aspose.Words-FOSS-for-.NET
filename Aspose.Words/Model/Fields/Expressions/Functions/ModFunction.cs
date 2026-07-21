// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2007 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a MOD function.
    /// </summary>
    internal class ModFunction : Function
    {
        protected override Constant EvaluateCore(ConstantCollection parameters)
        {
            double x = parameters[0].ValueDouble;
            double y = parameters[1].ValueDouble;

            if (y == 0d)
                return new ErrorConstant("!Zero Divide");

            // If field format does not specified, MS Word uses maximum number of decimal places from arguments of MOD function for result.
            DoubleConstant doubleFirstParameter = parameters[0] as DoubleConstant;
            DoubleConstant doubleSecondParameter = parameters[1] as DoubleConstant;
            int numberOfDecimalPlaces = System.Math.Max(
                doubleFirstParameter != null ? doubleFirstParameter.NumberOfDigitsAfterDecimalPoint : 0,
                doubleSecondParameter != null ? doubleSecondParameter.NumberOfDigitsAfterDecimalPoint : 0);
            return DoubleConstant.CreateFrom(parameters[0], parameters[1], x % y, numberOfDecimalPlaces);
        }

        protected override int ParameterCountMin
        {
            get { return 2; }
        }

        protected override int ParameterCountMax
        {
            get { return 2; }
        }

        protected override bool IsParameterTypeValid(ConstantType constantType)
        {
            return constantType == ConstantType.Double;
        }
    }
}
