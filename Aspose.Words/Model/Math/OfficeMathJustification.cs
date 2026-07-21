// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/12/2010 by Denis Darkin

using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Math
{
    /// <summary>
    /// Specifies the justification of the equation.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags",
        Justification = "Has explicit values, but isn't used as a flag.")]
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue",
        Justification = "Public API, as designed.")]
    public enum OfficeMathJustification
    {
        /// <summary>
        /// Justifies instances of mathematical text to the left with respect to each other, and centers the group of mathematical
        /// text (the Math Paragraph) with respect to the page.
        /// </summary>
        CenterGroup = 1,
    
        /// <summary>
        /// Centers each instance of mathematical text individually with respect to margins.
        /// </summary>
        Center = 2,
        
        /// <summary>
        /// Left justification of Math Paragraph.
        /// </summary>
        Left = 3,
        
        /// <summary>
        /// Right Justification of Math Paragraph.
        /// </summary>
        Right = 4,

        /// <summary>
        /// Inline position of Math.
        /// </summary>
        Inline = 7,

        /// <summary>
        /// Default value <see cref="CenterGroup"/>.
        /// </summary>
        Default = CenterGroup
    }
}
