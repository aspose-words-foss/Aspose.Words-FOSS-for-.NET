// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/03/2014 by Alexey Morozov

using System.IO;
using Aspose.JavaAttributes;
using Aspose.Ss;

namespace Aspose.Words.RW.Ole.Ole2
{
    /// <summary>
    /// Implements base class for miscellaneous OLE2 stream implementations.
    /// </summary>
    internal abstract class Ole2StreamBase
    {
        internal void Write(MemoryStorage storage)
        {
            MemoryStream stream = new MemoryStream();
            Write(new BinaryWriter(stream));
            storage[Name] = stream;
        }

        [JavaThrows(true)]
        protected abstract void Write(BinaryWriter writer);

        protected abstract string Name { get; }

        internal const string Ole10NativeStreamName = "\x0001Ole10Native";
        internal const string OcxNameStreamName = "\x0003OCXNAME";
        internal const string ObjInfoStreamName = "\x0003ObjInfo";
        internal const string PrintStreamName = "\x0003PRINT";
        internal const string OcxDataStreamName = "\x0003OCXDATA";
        internal const string CompObjStreamName = "\x0001CompObj";
        internal const string LinkInfoStreamName = "\x0003LinkInfo";
        internal const string AttachDescStreamName = "AttachDesc";
        internal const string OleStreamName = "\x0001Ole";
        internal const string MetaStreamName = "\x0003META";

        internal const string PicStreamName = "\x0003PIC";
    }
}
