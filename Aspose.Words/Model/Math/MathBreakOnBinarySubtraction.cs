// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/03/2011 by Denis Darkin
using Aspose.Words.Settings;

namespace Aspose.Words.Math
{
    /// <summary>
    /// Specifies how to represent subtraction on both sides of a line-wrapping break, 
    /// when the <see cref="MathProperties.BreakOnBinary"/> option is set to <see cref="MathBreakOnBinary.Repeat"/>. 
    /// The first character represents the sign at the end of the line with the
    /// break; the second represents the sign at the start of the wrapped line
    /// </summary>
    internal enum MathBreakOnBinarySubtraction
    {
        /// <summary>
        /// Repetition of subtraction sign after a line-wrapping break is minus on the first and second lines.
        /// </summary>
        MinusMinus = 0,

        /// <summary>
        /// Repetition of subtraction sign after a line-wrapping break is plus on the first line and minus on the second
        /// line.
        /// </summary>
        PlusMinus = 1,

        /// <summary>
        /// Repetition of subtraction sign after a line-wrapping break is minus on the first line and plus on the second line.
        /// </summary>
        MinusPlus = 2,
        
        /// <summary>
        /// Default os <see cref="MinusMinus"/>
        /// </summary>
        Default = MinusMinus
    }
}
