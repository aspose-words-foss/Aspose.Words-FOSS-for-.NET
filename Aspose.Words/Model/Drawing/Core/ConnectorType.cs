// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/07/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Indicates the type of a connector. A connector is a shape used to join other shapes.
    /// </summary>
    /// <seealso cref="Shape.ConnectorType"/>
    internal enum ConnectorType
    {
        /// <summary>
        /// </summary>
        Straight = 0,
        /// <summary>
        /// </summary>
        Bent = 1,
        /// <summary>
        /// </summary>
        Curved = 2,
        /// <summary>
        /// The shape is not a connector.
        /// </summary>
        None = 3
    }
}
