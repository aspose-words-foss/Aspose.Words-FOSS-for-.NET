// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/01/2014 by Alexey Morozov

using System;
using System.IO;
using Aspose.Ss;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Ole.Moniker;

namespace Aspose.Words.RW.Ole.Ole2
{
    /// <summary>
    /// Implements [MS-OLEDS] 2.3.3 OLEStream. http://msdn.microsoft.com/en-us/library/dd942499.aspx
    /// </summary>
    internal class OleStream : Ole2StreamBase
    {
        internal static OleStream Read(MemoryStorage storage)
        {
            return storage.ContainsKey(OleStreamName) ? new OleStream(storage.GetStreamZeroPositioned(OleStreamName)) : null;
        }

        internal OleStream(OleObjectType objectType)
        {
            Debug.Assert(IsValidObjectType(objectType));

            mObjectType = objectType;

            mLocalUpdateTime = DateTime.Now;
            mLocalCheckUpdateTime = DateTime.Now;
            mRemoteUpdateTime = DateTime.Now;
        }

        private OleStream(MemoryStream stream)
        {
            Debug.Assert(stream.Position == 0);
            BinaryReader reader = new BinaryReader(stream);

            int version = reader.ReadInt32();
            Debug.Assert(version == 0x02000001);

            int flags = reader.ReadInt32();

            mObjectType = (flags == 0x00000001) ? OleObjectType.Linked : OleObjectType.Embedded;

            // Skip unused OLEUPDATE.
            reader.ReadInt32();

            int reserved1 = reader.ReadInt32();
            Debug.Assert(reserved1 == 0x00);

            // ReservedMoniker, ignored.
            int monikerSize = reader.ReadInt32();
            MonikerStream.Read(reader, monikerSize);

            // Below should be only present for Linked object.
            if (mObjectType == OleObjectType.Linked)
            {
                // RelativeSourceMoniker, ignored.
                monikerSize = reader.ReadInt32();
                MonikerStream.Read(reader, monikerSize); 

                monikerSize = reader.ReadInt32();
                mAbsoluteSourceMoniker = MonikerStream.Read(reader, monikerSize);

                int clsidIndicator = reader.ReadInt32();
                Debug.Assert(clsidIndicator == -1);
                mClsId = new Guid(reader.ReadBytes(16));

                // Skip unused reservedDisplayName.
                DocBinaryReader.ReadWCharWithLengthBytes(reader);
                // Skip unused reserved.
                reader.ReadInt32();

                mLocalUpdateTime = DateTime.FromFileTimeUtc(reader.ReadInt64());
                mLocalCheckUpdateTime = DateTime.FromFileTimeUtc(reader.ReadInt64());
                mRemoteUpdateTime = DateTime.FromFileTimeUtc(reader.ReadInt64());
            }

            FileMoniker pathMoniker = mAbsoluteSourceMoniker as FileMoniker;
            
            if(pathMoniker != null)
                mPath = StringUtil.HasChars(pathMoniker.UnicodePath) ? pathMoniker.UnicodePath : pathMoniker.AnsiPath;
        }

        protected override void Write(BinaryWriter writer)
        {
            // Version
            writer.Write(0x02000001);

            writer.Write(mObjectType == OleObjectType.Linked ? (Int32)0x01 : 0x00);

            // LinkUpdateOptions
            writer.Write((Int32)0x00000002);

            // Reserved1
            writer.Write((Int32)0x00);

            // ReservedMonikerStreamSize. We don't write this moniker.
            writer.Write((Int32)0x00);

            if (mObjectType == OleObjectType.Linked)
            {
                // RelativeSourceMonikerStreamSize. We don't write this moniker.
                writer.Write((Int32)0x00);

                // prepare AbsoluteSourceMoniker
                MonikerBase moniker;
                if (UriUtil.IsHrefWithScheme(mPath))
                {
                    // WORDSNET-16067 If link to URI resources, MS Word inserts UrlMoniker
                    moniker = new UrlMoniker(mPath);
                }
                else
                {
                    // For local files seems that we can skip AnsiPath and set only UnicodePath.
                    FileMoniker fileMoniker = new FileMoniker();
                    fileMoniker.UnicodePath = mPath;
                    moniker = fileMoniker;
                }
                MonikerStream.Write(writer, moniker);

                // ClsidIndicator
                writer.Write(-1);

                // ClsId
                writer.Write(mClsId.ToByteArray());

                // ReservedDisplayName
                writer.Write((Int32)0x00);

                // Reserved2 
                writer.Write((Int32)0x00);

                // LocalUpdateTime
                writer.Write(DateTimeUtil.ToFileTimeUtc(mLocalUpdateTime, null));

                // LocalCheckUpdateTime 
                writer.Write(DateTimeUtil.ToFileTimeUtc(mLocalCheckUpdateTime, null));

                // RemoteUpdateTime 
                writer.Write(DateTimeUtil.ToFileTimeUtc(mRemoteUpdateTime, null));
            }
        }

        protected override string Name
        {
            get { return OleStreamName; }
        }

        internal OleObjectType ObjectType
        {
            get { return mObjectType; }
        }

        internal string Path
        {
            get { return mPath; }
            set { mPath = value; }
        }

        internal Guid ClsId
        {
            get { return mClsId; }
            set { mClsId = value; }
        }

        /// <summary>
        /// Time when the container application last updated the RemoteUpdateTime field.
        /// </summary>
        internal DateTime LocalUpdateTime
        {
            get { return mLocalUpdateTime; }
            set { mLocalUpdateTime = value; }
        }

        /// <summary>
        /// Time when the container application last checked the update time of the linked object.
        /// </summary>
        internal DateTime LocalCheckUpdateTime
        {
            get { return mLocalCheckUpdateTime; }
            set { mLocalCheckUpdateTime = value; }
        }

        /// <summary>
        /// Time when the linked object was last updated.
        /// </summary>
        internal DateTime RemoteUpdateTime
        {
            get { return mRemoteUpdateTime; }
            set { mRemoteUpdateTime = value; }
        }

        private static bool IsValidObjectType(OleObjectType objectType)
        {
            return (objectType == OleObjectType.Linked) || (objectType == OleObjectType.Embedded);
        }

        private string mPath;

        private DateTime mLocalUpdateTime;
        private DateTime mLocalCheckUpdateTime;
        private DateTime mRemoteUpdateTime;

        private Guid mClsId;

        private readonly MonikerBase mAbsoluteSourceMoniker;

        private readonly OleObjectType mObjectType;
    }
}
