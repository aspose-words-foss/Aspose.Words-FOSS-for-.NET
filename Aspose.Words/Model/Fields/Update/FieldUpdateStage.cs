// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/01/2011 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies a field update stage.
    /// </summary>
    /// <remarks>
    /// The order of items in this enum should correspond to the order of update stages. Change it carefully.
    /// </remarks>
    internal enum FieldUpdateStage
    {
        /// <summary>
        /// All fields are updated one by one on this stage.
        /// </summary>
        MainLoop,
        /// <summary>
        /// REF-like postponed fields are updated on this stage.
        /// </summary>
        DeferredUpdateRef,
        /// <summary>
        /// FCL postponed fields are updated on this stage.
        /// </summary>
        DeferredUpdateLayout
    }
}
