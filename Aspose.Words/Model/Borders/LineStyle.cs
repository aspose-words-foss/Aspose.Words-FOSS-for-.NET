// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Michael Morozoff

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Specifies line style of a <see cref="Border"/>.
    /// </summary>
    [CppEnumEnableMetadata]
    public enum LineStyle
    {
        /// <summary>
        /// </summary>
        /// <dev>
        /// In MS Word binary format LineStyle.None means no border for all objects, except table cells.
        ///
        /// For table cells LineStyle.None means inherit from the default table border.
        ///
        /// Apparently this looks like a very bad hack made by someone at MS and I work around it
        /// by making LineStyle.None to mean "no border" for any object in the model including table cells.
        /// The translation is performed in the binary cell reader/writer.
        ///
        /// I also use a single approach to indicate that a border is inherited. At first the border attribute
        /// is simply missing, but if a use requests it, a Border object is instantiated in "inherited" mode.
        /// 
        /// DS 12.10.2010. Right now the enum contains the continuous range of values and it is used in 
        /// Border.HasValidLineStyle to define if a line style belongs to the enum or not. If new values
        /// are added to the enum then Border.HasValidLineStyle should be reworked correspondingly.
        /// </dev>
        None = 0,
        /// <summary>
        /// </summary>
        Single = 1,
        /// <summary>
        /// </summary>
        Thick = 2,
        /// <summary>
        /// </summary>
        Double = 3,
        /// <summary>
        /// </summary>
        Hairline = 5,
        /// <summary>
        /// </summary>
        Dot = 6,
        /// <summary>
        /// </summary>
        DashLargeGap = 7,
        /// <summary>
        /// </summary>
        DotDash = 8,
        /// <summary>
        /// </summary>
        DotDotDash = 9,
        /// <summary>
        /// </summary>
        Triple = 10,
        /// <summary>
        /// </summary>
        ThinThickSmallGap = 11,         // | ||
        /// <summary>
        /// </summary>
        ThickThinSmallGap = 12,         // || |
        /// <summary>
        /// </summary>
        ThinThickThinSmallGap = 13,     // | || |
        /// <summary>
        /// </summary>
        ThinThickMediumGap = 14,        // |  ||
        /// <summary>
        /// </summary>
        ThickThinMediumGap = 15,        // ||  |
        /// <summary>
        /// </summary>
        ThinThickThinMediumGap = 16,    // |  ||  |
        /// <summary>
        /// </summary>
        ThinThickLargeGap = 17,         // |   ||
        /// <summary>
        /// </summary>
        ThickThinLargeGap = 18,         // ||   |
        /// <summary>
        /// </summary>
        ThinThickThinLargeGap = 19,     // |   ||   |
        /// <summary>
        /// </summary>
        Wave = 20,
        /// <summary>
        /// </summary>
        DoubleWave = 21,
        /// <summary>
        /// </summary>
        DashSmallGap = 22,
        /// <summary>
        /// </summary>
        DashDotStroker = 23,
        /// <summary>
        /// </summary>
        Emboss3D = 24,
        /// <summary>
        /// </summary>
        Engrave3D = 25,
        /// <summary>
        /// </summary>
        Outset = 26,
        /// <summary>
        /// </summary>
        Inset = 27,

        // Codes 64 - 230 represent border art types and are used only for page borders.
        // They are represented using the PageBorderArt enum.
    }
}
