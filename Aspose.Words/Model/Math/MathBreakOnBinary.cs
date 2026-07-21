// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/03/2011 by Denis Darkin

namespace Aspose.Words.Math
{
    /// <summary>
    /// Defines how to represent binary math operators with respect to a line-wrapping break. The line can wrap before 
    /// the operator or after the operator; alternately, the operator can appear both at the end of the first line and the
    /// beginning of the second.
    /// </summary>
    internal enum MathBreakOnBinary
    {
        /// <summary>
        /// When line-wrapping breaks occur on binary operators, 
        /// the binary operator appears before the break (at the end of the first line).
        /// </summary>
        Before = 0x00,
        
        /// <summary>
        /// When line-wrapping breaks occur on binary operators, 
        /// the binary operator appears after the break (at the start of the next line).
        /// </summary>
        After = 0x01,
        
        /// <summary>
        /// When line-wrapping breaks occur on binary operators, the binary operator appears on both sides of the break 
        /// (at the end of the first line and the start of the next line).
        /// </summary>
        Repeat = 0x02,

        /// <summary>
        /// Default is <see cref="Before"/>
        /// </summary>
        Default = Before
    }
}
