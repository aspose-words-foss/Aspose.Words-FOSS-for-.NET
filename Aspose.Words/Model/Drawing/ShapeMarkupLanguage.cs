// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/10/2014 by Andrey Noskov

using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies Markup language used for the shape.
    /// </summary>
    // JAVA: byte backing type added to resolve Shape's ctors signature conflict in java.
    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", 
        Justification = "byte backing type added to resolve Shape's ctors signature conflict in java.")]
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum ShapeMarkupLanguage : byte
    {
        /// <summary>
        /// Drawing Markup Language is used to define the shape.
        /// </summary>
        /// <remarks>
        /// This is the new standard for drawing for Office Open XML which has appeared first in ECMA-376 1st edition (2006), first appeared in MS Word 2007.
        /// </remarks>
        Dml,

        /// <summary>
        /// Vector Markup Language is used to define the shape. 
        /// </summary>
        /// <remarks>A deprecated format included in Office Open XML for legacy reasons only.</remarks>
        Vml
    }
}
