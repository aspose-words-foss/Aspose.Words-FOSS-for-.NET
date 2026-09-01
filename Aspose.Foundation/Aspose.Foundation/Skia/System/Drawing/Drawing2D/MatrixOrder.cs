// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/12/2017 by Alexey Noskov

#if NETSTANDARD || NET
namespace System.Drawing.Drawing2D
{
    public enum MatrixOrder
    {
        /// <summary>
        /// The new operation is applied before the old operation (default order).
        /// </summary>
        Prepend = 0,

        /// <summary>
        /// The new operation is applied after the old operation.
        /// </summary>
        Append = 1
    }
}
#endif
