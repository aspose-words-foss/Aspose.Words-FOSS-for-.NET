// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/08/2009 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies a field switch type.
    /// </summary>
    internal enum FieldSwitchType
    {
        /// <summary>
        /// The switch is not recognized as a valid field specific switch for the field being parsed.
        /// </summary>
        Unknown,
        /// <summary>
        /// The switch is a simple flag that has no argument.
        /// </summary>
        Flag,
        /// <summary>
        /// A switch argument is expected to follow the switch.
        /// </summary>
        HasArgument
    }
}
