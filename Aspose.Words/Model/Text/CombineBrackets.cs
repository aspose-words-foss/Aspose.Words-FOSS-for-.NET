// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/08/2011 by Alexey Morozov

namespace Aspose.Words
{
    /// <summary>
    /// Specifies whether the two sub-lines within one line are enclosed within a pair of brackets when displayed, 
    /// and the type of brackets that are displayed. 
    /// </summary>
    /// <remarks>See [MS-DOC] 2.9.332 UFEL.</remarks>
    internal enum CombineBrackets
    {
        None = 0x00,

        /// <summary>
        /// Specifies round - "()" - brackets.
        /// </summary>
        Round = 0x01,

        /// <summary>
        /// Specifies round - "[]" - brackets.
        /// </summary>
        Square = 0x02,

        /// <summary>
        /// Specifies round - "&lt;&gt;" - brackets.
        /// </summary>
        Angle = 0x03,

        /// <summary>
        /// Specifies round - "{}" - brackets.
        /// </summary>
        Curly = 0x04,

        /// <summary>
        /// Same as <see cref="None"/>.
        /// </summary>
        Default = None
    }
}
