// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/01/2014 by Alexey Morozov

using System.IO;
using Aspose.Ss;

namespace Aspose.Words.RW.Ole.Ole2
{
    /// <summary>
    /// Implements Ole10NativeStream used to wrap OLE1 objects into OLE2 structured storage.
    /// </summary>
    internal class Ole10NativeStream : Ole2StreamBase
    {
        internal static Ole10NativeStream Read(MemoryStorage storage)
        {
            return storage.ContainsKey(Ole10NativeStreamName)
                       ? new Ole10NativeStream(new BinaryReader(storage.GetStreamZeroPositioned(Ole10NativeStreamName)))
                       : null;
        }

        internal Ole10NativeStream()
        {
        }

        private Ole10NativeStream(BinaryReader reader)
        {
            int nativeDataSize = reader.ReadInt32();
            mNativeData = reader.ReadBytes(nativeDataSize);
        }

        protected override void Write(BinaryWriter writer)
        {
            writer.Write(mNativeData.Length);
            writer.Write(mNativeData);
        }

        protected override string Name
        {
            get { return Ole10NativeStreamName; }
        }

        internal byte[] NativeData
        {
            get { return mNativeData; }
            set { mNativeData = value; }
        }

        private byte[] mNativeData;
    }
}
