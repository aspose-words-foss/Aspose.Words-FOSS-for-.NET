// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/10/2010 by Dmitry Vorobyev

namespace Aspose.Words
{
    /// <summary>
    /// When implemented, receives notifications about <see cref="DocumentPosition"/> movements.
    /// </summary>
    internal interface IDocumentPositionListener
    {
        /// <summary>
        /// Receives notifications after <see cref="DocumentPosition"/> moved.
        /// </summary>
        /// <param name="movement">Specifies what movement was done.</param>
        void NotifyMoved(DocumentPositionMovement movement);
    }
}
