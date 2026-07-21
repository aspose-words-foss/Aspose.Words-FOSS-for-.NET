// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/03/2018 by Dmitry Burov

using System;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// Thrown while trying to create a bitmap. It may occur due to different reasons such as invalid parameters width, height, pixel format, etc.
    /// </summary>
    /// <remarks>
    /// Placed here in non-public API because it should be catched and converted to warning. <see cref="ImageShapeWriter.Write()"/>.
    /// </remarks>
#pragma warning disable CA1058 // Types should not extend certain base types
    public class CantCreateBitmapException : ApplicationException // Derive from ApplicationException for autoporting to work.
#pragma warning restore CA1058 // Types should not extend certain base types
    {
        public CantCreateBitmapException(Exception originalException) :
            base(ExceptionMessage, originalException)
        {
        }

        public CantCreateBitmapException(string message) :
            base(string.Format("{0}: {1}", ExceptionMessage, message))
        {
        }

        private const string ExceptionMessage = "Could not create the bitmap with the specified parameters. Possible lack of system resources.";
    }
}
