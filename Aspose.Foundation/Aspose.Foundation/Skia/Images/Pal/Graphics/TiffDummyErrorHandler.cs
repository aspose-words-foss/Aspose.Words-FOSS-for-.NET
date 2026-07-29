// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/01/2024 by Denis Panov

#if NETSTANDARD
using System.IO;
using System;
using BitMiracle.LibTiff.Classic;

namespace Aspose.Skia.Images.Pal.Graphics
{
    /// <summary>
    ///  The implementation of <see cref="BitMiracle.LibTiff.Classic.TiffErrorHandler"> to ignore errors and warnings.
    /// </summary>
    internal class TiffDummyErrorHandler : TiffErrorHandler
    {
        public override void ErrorHandler(Tiff tif, string method, string format, params object[] args)
        {
        }

        public override void ErrorHandlerExt(Tiff tif, object clientData, string method, string format, params object[] args)
        {
        }

        
        public override void WarningHandler(Tiff tif, string method, string format, params object[] args)
        {
        }

        public override void WarningHandlerExt(Tiff tif, object clientData, string method, string format, params object[] args)
        {
        }
    }
}
#endif
