// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/07/2010 by Dmitry Vorobyev

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies whether a certain field update should be performed, postponed, cancelled or rejected.
    /// </summary>
    internal enum FieldUpdateStrategy
    {
        /// <summary>
        /// Specifies that a certain field update should be performed immediately.
        /// </summary>
        Update,
        /// <summary>
        /// Specifies that a certain field update should be postponed on current stage.
        /// </summary>
        Defer,
        /// <summary>
        /// Specifies that a certain field update should be cancelled on current stage.
        /// </summary>
        Skip,
        /// <summary>
        /// Specifies that a certain field update should be rejected.
        /// </summary>
        Reject
    }
}
