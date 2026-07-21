// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/04/2012 by Alexey Morozov

using System;
using System.IO;
using Aspose.IO;
using Aspose.Ss;

namespace Aspose.Words.RW.Ole.Ole2
{
    /// <summary>
    /// Implements 2.1.4.1 ObjInfo Stream.
    /// Contains an ODT structure which specifies information about that embedded OLE object.
    /// </summary>
    internal class ObjInfoStream : Ole2StreamBase
    {
        internal static ObjInfoStream Read(MemoryStorage storage)
        {
            if (!storage.ContainsKey(ObjInfoStreamName))
                return null;
            
            // WORDSJAVA-2266 The resilience against a zero length stream.
            MemoryStream memoryStream = storage.GetStreamZeroPositioned(ObjInfoStreamName);
            if (memoryStream.Length == 0)
                return null;
            
            return new ObjInfoStream(new BinaryReader(memoryStream));
        }

        private ObjInfoStream()
        {
        }

        internal static ObjInfoStream DefaultControl()
        {
            ObjInfoStream objInfoStream = new ObjInfoStream();
            objInfoStream.mFlags1 = OdtPersist1.RecomposeOnResize | OdtPersist1.Ocx;
            objInfoStream.mFlags2 = OdtPersist2.QueriedEmf;
            objInfoStream.mClipboardFormat = OdtClipboardFormat.Metafile;

            return objInfoStream;
        }

        internal static ObjInfoStream DefaultObject(bool isIcon, bool isLink)
        {
            ObjInfoStream objInfoStream = new ObjInfoStream();
            objInfoStream.mFlags1 = (isIcon ? OdtPersist1.Icon : OdtPersist1.None) |
                (isLink ? OdtPersist1.Link : OdtPersist1.None);
            objInfoStream.mFlags2 = OdtPersist2.Emf;
            objInfoStream.mClipboardFormat = OdtClipboardFormat.Metafile;

            return objInfoStream;
        }

        internal ObjInfoStream(BinaryReader reader)
        {
            mFlags1 = (OdtPersist1)reader.ReadUInt16();
            mClipboardFormat = (OdtClipboardFormat)reader.ReadUInt16();

            // According to spec this member does not exist if the ObjInfo stream is not large enough to accommodate it.
            if (StreamUtil.HasEnoughBytesToRead(reader, 2))
                mFlags2 = (OdtPersist2)(reader.ReadUInt16() & 0x0f);
        }

        protected override void Write(BinaryWriter writer)
        {
            writer.Write((UInt16)mFlags1);
            writer.Write((UInt16)mClipboardFormat);
            writer.Write((UInt16)mFlags2);
        }

        protected override string Name
        {
            get { return ObjInfoStreamName; }
        }

        internal OdtPersist1 Flags1
        {
            get { return mFlags1; }
            set { mFlags1 = value; }
        }

        internal OdtPersist2 Flags2
        {
            get { return mFlags2; }
            set { mFlags2 = value; }
        }

        /// <summary>
        /// Indicates that object is drawn as icon.
        /// </summary>
        internal bool IsIcon
        {
            get { return (mFlags1 & OdtPersist1.Icon) != 0; }
            set
            {
                if(value)
                    mFlags1 |= OdtPersist1.Icon;
                else
                    mFlags1 &= ~OdtPersist1.Icon;
            }
        }

        private OdtPersist1 mFlags1;
        private OdtPersist2 mFlags2;
        private OdtClipboardFormat mClipboardFormat = OdtClipboardFormat.None;
    }
}
