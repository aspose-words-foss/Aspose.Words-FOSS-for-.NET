// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/02/2011 by Alexey Titov

using Aspose.Drawing;
using Aspose.Words.RW.Dml.Writer;

namespace Aspose.Words.Drawing.Core.Dml.Colors.Modifiers
{
    /// <summary>
    /// Public interface of color modifiers.
    /// </summary>
    internal interface IDmlColorModifier
    {
        /// <summary>
        /// Applies the modifiers to the color.
        /// </summary>
        /// <returns>The modified color.</returns>
        DrColor Modify(DrColor color);

        IDmlColorModifier Clone();

        DmlColorModifierType ModifierType { [CodePorting.Translator.Cs2Cpp.CppConstMethod()] get; }

        void Write(string prefix, IDmlShapeWriterContext writer);
    }
}
