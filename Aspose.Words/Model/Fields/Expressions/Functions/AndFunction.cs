// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/07/2009 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents an AND function.
    /// </summary>
    internal class AndFunction : Function
    {
        protected override Constant EvaluateCore(ConstantCollection parameters)
        {
            return new BooleanConstant(parameters[0].ValueBoolean && parameters[1].ValueBoolean);
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
            return constantType == ConstantType.Boolean;
        }
    }
}
