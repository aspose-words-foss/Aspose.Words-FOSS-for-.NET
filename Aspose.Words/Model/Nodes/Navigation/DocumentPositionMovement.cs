// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/10/2010 by Dmitry Vorobyev

namespace Aspose.Words
{
    /// <summary>
    /// Specifies one of possible movements of <see cref="DocumentPosition"/>.
    /// </summary>
#if CPLUSPLUS
    public
#else
    internal
#endif
    enum DocumentPositionMovement
    {
        /// <summary>
        /// No movement done.
        /// </summary>
        None,
        /// <summary>
        /// Moved inside a node.
        /// </summary>
        Inside,
        /// <summary>
        /// Moved to the start or end of a node.
        /// </summary>
        StartEnd,
        /// <summary>
        /// Moved to a sibling of a node.
        /// </summary>
        Sibling,
        /// <summary>
        /// Moved to the parent of a node.
        /// </summary>
        Above,
        /// <summary>
        /// Moved to a child of a node.
        /// </summary>
        Below
    }
}
