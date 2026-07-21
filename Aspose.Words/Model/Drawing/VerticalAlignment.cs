// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies vertical alignment of a floating shape, text frame or a floating table.
    /// </summary>
    /// <seealso cref="ShapeBase.VerticalAlignment"/>
    /// <dev>
    /// Do not renumber. The values are taken from the RTF specification.
    /// </dev>
    [CppEnumEnableMetadata]
    public enum VerticalAlignment
    {
        /// <summary>
        /// The object is explicitly positioned, usually using its <b>Top</b> property.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies that the object shall be at the top of the vertical alignment base.
        /// </summary>
        Top = 1,
        /// <summary>
        /// Specifies that the object shall be centered with respect to the vertical alignment base.
        /// </summary>
        Center = 2,
        /// <summary>
        /// Specifies that the object shall be at the bottom of the vertical alignment base.
        /// </summary>
        Bottom = 3,
        /// <summary>
        /// Specifies that the object shall be inside of the horizontal alignment base.
        /// </summary>
        Inside = 4,
        /// <summary>
        /// Specifies that the object shall be outside of the vertical alignment base.
        /// </summary>
        Outside = 5,
        /// <summary>
        /// Not documented. Seems to be a possible value for floating paragraphs and tables.
        /// </summary>
        Inline = -1,

        /// <summary>
        /// Same as <see cref="None"/>.
        /// </summary>
        Default = None
    }
}
