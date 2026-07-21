// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2010 by Dmitry Burov

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Type of handle positioning.
    /// Affects the position coordinates calculation.
    /// </summary>
    internal enum HandlePositionType
    {
        Unknown, 

        // Position the x(y) coordinate of adjust handle on the left(top) perimeter of it's shape.
        LeftTop,

        // Position the x(y) coordinate of adjust handle on the right(bottom) perimeter of it's shape.
        RightBottom,

        // Position the x(y) coordinate of adjust handle on the horizontal(vertical) center of it's shape.
        Center,

        // Position of x(y) coordinate of adjust handle is the result value of formula specified by index.
        Formula,

        // Position of x(y) coordinate of adjust handle is the adjust value specified by index.
        Adjust,

        // Position of x(y) coordinate of adjust handle is the constant value
        Constant
    }
}
