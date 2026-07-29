// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/25/2016 by Andrey Noskov

namespace Aspose.Words.Drawing.Core.Dml.ShapeEffects
{
    /// <summary>
    /// Specifies the type of effect container, either sibling or tree.
    /// </summary>
    internal enum EffectDagType
    {
        /// <summary>
        /// Each effect is separately applied to the parent object.
        /// </summary>
        Sibling,

        /// <summary>
        /// Each effect is applied to the result of the previous effect.
        /// </summary>
        Tree
    }
}
