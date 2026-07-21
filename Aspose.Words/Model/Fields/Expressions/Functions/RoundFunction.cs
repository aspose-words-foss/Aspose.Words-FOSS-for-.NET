// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2007 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a ROUND function.
    /// </summary>
    internal class RoundFunction : Function
    {
        protected override Constant EvaluateCore(ConstantCollection parameters)
        {
            double x = parameters[0].ValueDouble;
            double y = parameters[1].ValueDouble;

            int numberOfDecimalPlaces = (int)System.Math.Floor(y);
            double result;

            if (numberOfDecimalPlaces < 0)
            {
                double p = System.Math.Pow(10, -numberOfDecimalPlaces);
                numberOfDecimalPlaces = 0;
                result = System.Math.Round(x/p)*p;
            }
            else
            {
                result = System.Math.Round(x, numberOfDecimalPlaces);
            }

            return DoubleConstant.CreateFrom(parameters[0], parameters[1], result, numberOfDecimalPlaces);
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
