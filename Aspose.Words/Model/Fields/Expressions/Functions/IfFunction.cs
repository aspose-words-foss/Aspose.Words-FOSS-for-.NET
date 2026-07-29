// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/01/2007 by Dmitry Vorobyev

namespace Aspose.Words.Fields.Expressions
{
    /// <summary>
    /// Represents an IF function.
    /// </summary>
    /// <remarks>
    /// Not in the OOXML specs.
    /// </remarks>
    internal class IfFunction : Function
    {
        protected override Constant EvaluateCore(ConstantCollection parameters)
        {
            if (parameters[0] is BooleanConstant)
                return (parameters[0].ValueBoolean) ? parameters[1] : parameters[2];
            return parameters[1];
        }

        protected override int ParameterCountMin
        {
            get { return 3; }
        }

        protected override int ParameterCountMax
        {
            get { return 3; }
        }

        protected override bool IsParameterTypeValid(ConstantType constantType)
        {
            return true;
        }
    }
}
