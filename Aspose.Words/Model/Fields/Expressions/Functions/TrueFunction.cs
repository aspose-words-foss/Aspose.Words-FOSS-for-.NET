// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2007 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents a TRUE function.
    /// </summary>
    internal class TrueFunction : Function
    {
        protected override Constant EvaluateCore(ConstantCollection parameters)
        {
            return new BooleanConstant(true);
        }

        protected override int ParameterCountMin
        {
            get { return 0; }
        }

        protected override int ParameterCountMax
        {
            get { return 0; }
        }

        protected override bool IsParameterTypeValid(ConstantType constantType)
        {
            return constantType == ConstantType.Boolean;
        }
    }
}
