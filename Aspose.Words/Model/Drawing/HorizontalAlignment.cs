// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies horizontal alignment of a floating shape, text frame or floating table.
    /// </summary>
    /// <seealso cref="ShapeBase.HorizontalAlignment"/>
    /// <dev>
    /// Do not renumber. The values are taken from the RTF specification.
    /// </dev>
    [CppEnumEnableMetadata]
    public enum HorizontalAlignment
    {
        /// <summary>
        /// The object is explicitly positioned, usually using its <b>Left</b> property.
        /// </summary>
        None = 0,
        /// <summary>
        /// Same as <see cref="None"/>.
        /// </summary>
        Default = None,
        /// <summary>
        /// Specifies that the object shall be left aligned to the horizontal alignment base.
        /// </summary>
        Left = 1,
        /// <summary>
        /// Specifies that the object shall be centered with respect to the horizontal alignment base.
        /// </summary>
        Center = 2,
        /// <summary>
        /// Specifies that the object shall be right aligned to the horizontal alignment base.
        /// </summary>
        Right = 3,
        /// <summary>
        /// Specifies that the object shall be inside of the horizontal alignment base.
        /// </summary>
        Inside = 4,
        /// <summary>
        /// Specifies that the object shall be outside of the horizontal alignment base.
        /// </summary>
        Outside = 5,
    }
}
