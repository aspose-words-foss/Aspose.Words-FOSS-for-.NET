// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/05/2021 by Vadim Saltykov

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies the fill pattern to be used to fill a shape.
    /// </summary>
    /// <dev>
    /// HatchStyle is not appropriate for DML, because Patterned() for DML can address all 55 patterns,
    /// and HatchStyle contains several identical values for different constants.
    /// </dev>
    public enum PatternType
    {
        /// <summary>
        /// No pattern.
        /// </summary>
        None = -1,

        /// <summary>
        /// 10% of the foreground color.
        /// </summary>
        Percent10 = 1,

        /// <summary>
        /// 20% of the foreground color.
        /// </summary>
        Percent20 = 2,

        /// <summary>
        /// 25% of the foreground color.
        /// </summary>
        Percent25 = 3,

        /// <summary>
        /// 30% of the foreground color.
        /// </summary>
        Percent30 = 4,

        /// <summary>
        /// 40% of the foreground color
        /// </summary>
        Percent40 = 5,

        /// <summary>
        /// 50% of the foreground color
        /// </summary>
        Percent50 = 6,

        /// <summary>
        /// 5% of the foreground color.
        /// </summary>
        Percent5 = 7,

        /// <summary>
        /// 60% of the foreground color.
        /// </summary>
        Percent60 = 8,

        /// <summary>
        /// 70% of the foreground color.
        /// </summary>
        Percent70 = 9,

        /// <summary>
        /// 75% of the foreground color.
        /// </summary>
        Percent75 = 10,

        /// <summary>
        /// 80% of the foreground color.
        /// </summary>
        Percent80 = 11,

        /// <summary>
        /// 90% of the foreground color.
        /// </summary>
        Percent90 = 12,

        /// <summary>
        /// Cross.
        /// </summary>
        Cross = 13,

        /// <summary>
        /// Dark downward diagonal.
        /// </summary>
        DarkDownwardDiagonal = 14,

        /// <summary>
        /// Dark horizontal.
        /// </summary>
        DarkHorizontal = 15,

        /// <summary>
        /// Dark upward diagonal.
        /// </summary>
        DarkUpwardDiagonal = 16,

        /// <summary>
        /// Dark vertical.
        /// </summary>
        DarkVertical = 17,

        /// <summary>
        /// Dashed downward diagonal.
        /// </summary>
        DashedDownwardDiagonal = 18,

        /// <summary>
        /// Dashed horizontal.
        /// </summary>
        DashedHorizontal = 19,

        /// <summary>
        /// Dashed upward diagonal.
        /// </summary>
        DashedUpwardDiagonal = 20,

        /// <summary>
        /// Dashed vertical.
        /// </summary>
        DashedVertical = 21,

        /// <summary>
        /// Diagonal brick.
        /// </summary>
        DiagonalBrick = 22,

        /// <summary>
        /// Diagonal cross.
        /// </summary>
        DiagonalCross = 23,

        /// <summary>
        /// Pattern divot.
        /// </summary>
        Divot = 24,

        /// <summary>
        /// Dotted diamond.
        /// </summary>
        DottedDiamond = 25,

        /// <summary>
        /// Dotted grid.
        /// </summary>
        DottedGrid = 26,

        /// <summary>
        /// Downward diagonal.
        /// </summary>
        DownwardDiagonal = 27,

        /// <summary>
        /// Horizontal.
        /// </summary>
        Horizontal = 28,

        /// <summary>
        /// Horizontal brick.
        /// </summary>
        HorizontalBrick = 29,

        /// <summary>
        /// Large checker board.
        /// </summary>
        LargeCheckerBoard = 30,

        /// <summary>
        /// Large confetti.
        /// </summary>
        LargeConfetti = 31,

        /// <summary>
        /// Large grid.
        /// </summary>
        LargeGrid = 32,

        /// <summary>
        /// Light downward diagonal.
        /// </summary>
        LightDownwardDiagonal = 33,

        /// <summary>
        /// Light horizontal.
        /// </summary>
        LightHorizontal = 34,

        /// <summary>
        /// Light upward diagonal.
        /// </summary>
        LightUpwardDiagonal = 36,

        /// <summary>
        /// Light vertical.
        /// </summary>
        LightVertical = 37,

        /// <summary>
        /// Narrow horizontal.
        /// </summary>
        NarrowHorizontal = 38,

        /// <summary>
        /// Narrow vertical.
        /// </summary>
        NarrowVertical = 39,

        /// <summary>
        /// Outlined diamond.
        /// </summary>
        OutlinedDiamond = 40,

        /// <summary>
        /// Plaid.
        /// </summary>
        Plaid = 41,

        /// <summary>
        /// Shingle.
        /// </summary>
        Shingle = 42,

        /// <summary>
        /// Small checker board.
        /// </summary>
        SmallCheckerBoard = 43,

        /// <summary>
        /// Small confetti.
        /// </summary>
        SmallConfetti = 44,

        /// <summary>
        /// Small grid.
        /// </summary>
        SmallGrid = 45,

        /// <summary>
        /// Solid diamond.
        /// </summary>
        SolidDiamond = 46,

        /// <summary>
        /// Sphere.
        /// </summary>
        Sphere = 47,

        /// <summary>
        /// Trellis.
        /// </summary>
        Trellis = 48,

        /// <summary>
        /// Upward diagonal.
        /// </summary>
        UpwardDiagonal = 49,

        /// <summary>
        /// Vertical.
        /// </summary>
        Vertical = 50,

        /// <summary>
        /// Wave.
        /// </summary>
        Wave = 51,

        /// <summary>
        /// Weave.
        /// </summary>
        Weave = 52,

        /// <summary>
        /// Wide downward diagonal.
        /// </summary>
        WideDownwardDiagonal = 53,

        /// <summary>
        /// Wide upward diagonal.
        /// </summary>
        WideUpwardDiagonal = 54,

        /// <summary>
        /// Zig zag.
        /// </summary>
        ZigZag = 55
    }
}
