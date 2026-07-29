// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/07/2012 by Ivan Lyagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Specifies field's child fields update process stage.
    /// </summary>
    internal enum FieldChildUpdateStage
    {
        /// <summary>
        /// Child fields which must be updated regardless to any conditions are being updated.
        /// </summary>
        Permanent,
        /// <summary>
        /// Child fields which must be updated due to some conditions are being updated.
        /// </summary>
        Conditional
    }
}
