// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/12/2015 by Alexey Morozov

using System.IO;

namespace Aspose.Words.RW.Ole.Ole2
{
    /// <summary>
    /// Represents OCXDATA OLE2.0 stream which is used internally by ActiveX controls.
    /// </summary>
    internal class OcxDataStream : Ole2StreamBase
    {
        protected override void Write(BinaryWriter writer)
        {
            writer.Write(mData);
        }

        protected override string Name
        {
            get { return OcxDataStreamName; }
        }

        internal byte[] Data
        {
            get { return mData; }
            set { mData = value; }
        }

        private byte[] mData;
    }
}
